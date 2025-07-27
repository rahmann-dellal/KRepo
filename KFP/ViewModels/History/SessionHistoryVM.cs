using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using KFP.ViewModels.Helpers.ObjectSelector;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KFP.ViewModels
{
    public delegate Task<bool> ShowConfirmDeleteSessionDialog(int sessionId);
    public delegate Task<bool> ShowConfirmDeleteSessionsRangeDialog(DateTimeOffset startDate, DateTimeOffset endDate);

    public partial class SessionHistoryVM : KioberViewModelBase
    {
        public readonly KFPContext dbContext;
        public AppDataService appDataService;
        public ShowConfirmDeleteSessionDialog showConfirmDeleteSessionDialog { get; set; } = null!;
        public ShowConfirmDeleteSessionsRangeDialog showConfirmDeleteSessionRangeDialog { get; set; } = null!;

        [ObservableProperty] private DateTimeOffset startDate;
        [ObservableProperty] private DateTimeOffset endDate;
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int totalPages = 1;
        private const int PageSize = 9;
        public Currency currency { get; set; }

        public ICommand ApplyDateRangeCommand { get; }
        public ICommand SelectPageCommand { get; }
        public SwitchCommand SortByDateCommand { get; }

        public ObservableCollection<SessionHistoryListElement> DisplayList = new();
        public bool isDisplayedSessionsEmpty
        {
            get
            {
                return DisplayList == null || DisplayList.Count <= 0;
            }
        }
        public bool DisplayedSessionsNotEmpty
        {
            get
            {
                return DisplayList != null && DisplayList.Count > 0;
            }
        }
        public double TotalForSelectedDateRange = 0;

        public SessionHistoryVM(KFPContext dbContext, AppDataService appDataService)
        {
            this.dbContext = dbContext;
            this.appDataService = appDataService;
            StartDate = DateTimeOffset.Now.AddDays(-2);
            EndDate = DateTimeOffset.Now;
            currency = this.appDataService.Currency;

            ApplyDateRangeCommand = new RelayCommand(() => {
                CurrentPage = 1; // Reset to first page when applying a new date range
                SetRange(startDate, endDate);
            });

            SelectPageCommand = new RelayCommand<int>(page =>
            {
                CurrentPage = Math.Clamp(page, 1, TotalPages);
                LoadResult();
            });
            SortByDateCommand = new SwitchCommand(() =>
            {
                CurrentPage = 1; // Reset to first page when changing sort order
                LoadResult();
            }, null);
            LoadResult();
        }

        private void SetRange(DateTimeOffset start, DateTimeOffset end)
        {
            StartDate = start;
            EndDate = end;
            LoadResult();
        }
        public void LoadResult()
        {
            var query = dbContext.Sessions.Include(o => o.appUser)
                .Where(s => s.Start >= StartDate.DateTime && s.Start <= EndDate.DateTime && (s.End != null));

            query = SortByDateCommand.IsSwitched switch
            {
                true => query.OrderByDescending(o => o.Start),
                _ => query.OrderBy(o => o.Start)
            };
                
            TotalPages = query.Count() / PageSize + (query.Count() % PageSize > 0 ? 1 : 0);
            CurrentPage = Math.Min(CurrentPage, TotalPages);
            TotalForSelectedDateRange = query.Sum(s => s.TotalCardPayments + (s.ClosingCash - s.OpeningCash)); 
            List<Session> ResultList = query.Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            DisplayList.Clear(); // Clear the previous list before adding new items
            foreach (var session in ResultList)
            {
                DisplayList.Add(new SessionHistoryListElement(session, this));
            }

            OnPropertyChanged(nameof(isDisplayedSessionsEmpty));
            OnPropertyChanged(nameof(DisplayedSessionsNotEmpty));
            OnPropertyChanged(nameof(TotalForSelectedDateRange));
            DeleteAllSelectedSessionsCommand.NotifyCanExecuteChanged();
        }
        [RelayCommand(CanExecute = nameof(DisplayedSessionsNotEmpty))]
        public async Task DeleteAllSelectedSessions()
        {
            var result = await showConfirmDeleteSessionRangeDialog(StartDate, EndDate);
            if (!result)
            {
                return;
            }
            var toDelete = dbContext.Sessions.Where(s => s.Start >= StartDate.DateTime && s.Start <= EndDate.DateTime);
            dbContext.Sessions.RemoveRange(toDelete);
            await dbContext.SaveChangesAsync();
            LoadResult();
        }
    }
    public partial class SessionHistoryListElement : ObservableObject
    {
        public SessionHistoryVM parentVM;
        public Session session { get; set; }

        public string SessionStart { get => session.Start.ToString(new CultureInfo(parentVM.appDataService.AppLanguage)) ?? ""; }
        public string SessionEnd { get => session.End?.ToString(new CultureInfo(parentVM.appDataService.AppLanguage)) ?? ""; }

        public string OpeningCash { get => $"{session.OpeningCash.ToString("F2")} {parentVM.currency}"; }
        public string ClosingCash { get => $"{session.ClosingCash.ToString("F2")} {parentVM.currency}"; }
        public string TotalCardPayments { get => $"{session.TotalCardPayments.ToString("F2")} {parentVM.currency}"; }
        public string TotalPayments { get => $"{(session.TotalCardPayments + (session.ClosingCash - session.OpeningCash)).ToString("F2")} {parentVM.currency}"; }


        public SessionHistoryListElement(Session session, SessionHistoryVM parentVM)
        {
            this.session = session;
            this.parentVM = parentVM;
        }

        [RelayCommand]
        public async Task DeleteSession()
        {
            var result = await parentVM.showConfirmDeleteSessionDialog(session.SessionId);
            if (!result)
            {
                return;
            }
            if (parentVM != null)
            {
                parentVM.dbContext.Sessions.Remove(session);
                parentVM.dbContext.SaveChanges();
                parentVM.LoadResult();
            }
        }
    }
}
