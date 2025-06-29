using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.ViewModels.Helpers;
using KFP.ViewModels.Helpers.ObjectSelector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KFP.ViewModels.History
{
    public class OrdersHistoryVM : ObservableObject
    {
        private readonly KFPContext _dbContext;

        private DateTimeOffset? _startDate = DateTime.Today.AddDays(-2);
        private DateTimeOffset? _endDate = DateTime.Now;
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalPages;



        private ObservableCollection<Order> _orders = new();
        private List<Order> _allResults = new();

        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    DeselectAll(QuickFilters);
                    LoadOrders();
                }
            }
        }

        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    DeselectAll(QuickFilters);
                    LoadOrders();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value)) { 
                    UpdatePagedOrders();
                    NextPageCommand.NotifyCanExecuteChanged();
                    PreviousPageCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set => SetProperty(ref _totalPages, value);
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            private set => SetProperty(ref _orders, value);
        }

        public ObservableCollection<SelectableCommand> PageCommands { get; } = new();
        public List<SelectableCommand> QuickFilters { get; } = new();

        public SelectableCommand TwoDaysQuickFilter;
        public SelectableCommand LastWeekQuickFilter;
        public SelectableCommand LastMonthQuickFilter;

        public SwitchCommand SortByDateSwitch;
        public SwitchCommand SortByPriceSwitch;
        
        public RelayCommand PreviousPageCommand { get; private set; }

        public RelayCommand NextPageCommand { get; private set; }
        

        public OrdersHistoryVM(KFPContext dbContext)
        {
            _dbContext = dbContext;

            PreviousPageCommand = new RelayCommand(() =>
            {
                if (CurrentPage > 1)
                    CurrentPage--;
            },
            () => {
                return CurrentPage > 1;
            });

            NextPageCommand = new RelayCommand(() =>
            {
                if (CurrentPage < TotalPages)
                    CurrentPage++;
            },
            () => {
                return CurrentPage < PageCommands.Count;
            });

            InitializeQuickFilters();
            InitializeSortSwitches();

            LoadOrders();
        }

        private void InitializeQuickFilters()
        {
            QuickFilters.Clear();

            TwoDaysQuickFilter = new SelectableCommand(() =>
            {
                StartDate = DateTime.Today.AddDays(-2);
                EndDate = DateTime.Now;
                LoadOrders();
            }, QuickFilters);

            LastWeekQuickFilter = new SelectableCommand(() =>
            {
                StartDate = DateTime.Today.AddDays(-7);
                EndDate = DateTime.Now;
                LoadOrders();
            }, QuickFilters);

            LastMonthQuickFilter = new SelectableCommand(() =>
            {
                StartDate = DateTime.Today.AddMonths(-1);
                EndDate = DateTime.Now;
                LoadOrders();
            }, QuickFilters);

            QuickFilters.Add(TwoDaysQuickFilter);
            QuickFilters.Add(LastWeekQuickFilter);
            QuickFilters.Add(LastMonthQuickFilter);

            // Default selection
            TwoDaysQuickFilter.IsSelected = true;
        }

        private void InitializeSortSwitches()
        {
            SortByDateSwitch = new SwitchCommand(() =>
            {
                LoadOrders();
            });

            SortByPriceSwitch = new SwitchCommand(() =>
            {
                LoadOrders();
            });
        }

        private void LoadOrders()
        {
            _allResults = QueryOrders(StartDate?.DateTime ?? DateTime.Today.AddDays(-2), EndDate?.DateTime ?? DateTime.Now);
            TotalPages = (_allResults.Count + _pageSize - 1) / _pageSize;
            CurrentPage = 1;
            UpdatePagedOrders();
            GeneratePageCommands();
        }

        private void UpdatePagedOrders()
        {
            var paged = _allResults
                .Skip((CurrentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            Orders = new ObservableCollection<Order>(paged);
            UpdatePageSelection();
        }

        private void GeneratePageCommands()
        {
            PageCommands.Clear();
            var list = new List<SelectableCommand>();

            for (int i = 1; i <= TotalPages; i++)
            {
                int page = i;
                list.Add(new PageCommand(i, () =>
                {
                    CurrentPage = page;
                }, list));
            }

            foreach (var cmd in list)
                PageCommands.Add(cmd);

            UpdatePageSelection();
        }

        private void UpdatePageSelection()
        {
            if(PageCommands.Count > 0) {
                PageCommands.ElementAt(CurrentPage - 1).IsSelected = true;
            }
        }

        private List<Order> QueryOrders(DateTime start, DateTime end)
        {
            var query = _dbContext.Orders
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end);

            // Determine sort direction for date and price
            bool sortDateDesc = SortByDateSwitch.IsSwitched;
            bool sortPriceDesc = SortByPriceSwitch.IsSwitched;

            // Apply primary sort: DATE
            if (sortDateDesc)
                query = query.OrderByDescending(o => o.CreatedAt);
            else
                query = query.OrderBy(o => o.CreatedAt);

            // Apply secondary sort: PRICE (optional, only if selected)
            if (sortPriceDesc)
                query = ((IOrderedQueryable<Order>)query).ThenByDescending(o => o.TotalPrice ?? 0);
            else if (!sortPriceDesc && SortByPriceSwitch.IsSwitched)
                query = ((IOrderedQueryable<Order>)query).ThenBy(o => o.TotalPrice ?? 0);

            return query.ToList();
        }

        private void DeselectAll(IEnumerable<SelectableCommand> list)
        {
            foreach (var item in list)
                item.IsSelected = false;
        }
    }

}
