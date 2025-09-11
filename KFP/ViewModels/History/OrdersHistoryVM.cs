using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using KFP.ViewModels.Helpers;
using KFP.ViewModels.Helpers.ObjectSelector;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KFP.ViewModels
{
    public delegate Task<bool> ShowConfirmDeleteOrderDialog(int orderId);
    public delegate Task<bool> ShowConfirmDeleteRangeOrderDialog(DateTimeOffset startDate, DateTimeOffset endDate);
    public partial class OrdersHistoryVM : KioberViewModelBase
    {
        public readonly KFPContext dbContext;
        public INavigationService navigationService { get; private set; }
        public AppDataService appDataService;
        private const int PageSize = 9;
        public Currency currency { get; set; }

        [ObservableProperty] private DateTimeOffset startDate;
        [ObservableProperty] private DateTimeOffset endDate;
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int totalPages = 1;

        public ObservableCollection<OrdersHistoryListElement> DisplayList = new();

        public ShowConfirmDeleteOrderDialog showConfirmDeleteOrderDialog { get; set; } = null!;
        public ShowConfirmDeleteRangeOrderDialog showConfirmDeleteRangeOrderDialog { get; set; } = null!;
        public bool isDisplayedOrdersEmpty
        {
            get
            {
                return DisplayList == null || DisplayList.Count <= 0;
            }
        }
        public bool DisplayedOrdersNotEmpty
        {
            get
            {
                return DisplayList != null && DisplayList.Count > 0;
            }
        }
        public SwitchCommand SortByDateCommand { get; }
        public SwitchCommand SortByPriceCommand { get; }


        public ICommand ApplyDateRangeCommand { get; }
        public ICommand SelectPageCommand { get; }

        private List<SwitchCommand> sortCommands = new();
        public double TotalForSelectedDateRange = 0;
        public OrdersHistoryVM(KFPContext dbContext, AppDataService appDataService, INavigationService ns)
        {
            this.dbContext = dbContext;
            navigationService = ns;
            this.appDataService = appDataService;
            currency = this.appDataService.Currency;
            StartDate = DateTime.Today.AddDays(-2);
            EndDate = DateTime.Now;

            ApplyDateRangeCommand = new RelayCommand(() => {
                CurrentPage = 1; // Reset to first page when applying a new date range
                SetRange(startDate, endDate);
                });

            SelectPageCommand = new RelayCommand<int>(page =>
            {
                CurrentPage = Math.Clamp(page, 1, TotalPages);
                LoadOrders();
            });

            sortCommands = new();
            SortByDateCommand = new SwitchCommand(() =>
            {
                CurrentPage = 1; // Reset to first page when changing sort order
                ApplySorting("Date");
            }, sortCommands);
            SortByPriceCommand = new SwitchCommand(() => {
                CurrentPage = 1; // Reset to first page when changing sort order
                ApplySorting("Price"); 
            }, sortCommands);
            sortCommands.AddRange(new[] { SortByDateCommand, SortByPriceCommand });
            SortByDateCommand.IsSwitched = true;

            LoadOrders();
        }
        
        private void SetRange(DateTimeOffset start, DateTimeOffset end)
        {
                StartDate = start;
                EndDate = end;
                LoadOrders();
        }

        private void ApplySorting(string sortBy)
        {
            _SortBy = sortBy;
            LoadOrders();
        }

        private string _SortBy = "Date";

        public void LoadOrders()
        {
            var query = dbContext.Orders.Include(o => o.OrderItems)
                .Where(o => o.CreatedAt >= StartDate.DateTime && o.CreatedAt <= EndDate.DateTime && (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled));

            query = _SortBy switch
            {
                "Price" => SortByPriceCommand.IsSwitched switch
                { 
                    true => query.OrderByDescending(o => o.TotalPrice ?? 0),
                    _ => query.OrderBy(o => o.TotalPrice ?? 0)
                },
                _ => SortByDateCommand.IsSwitched switch {
                    true => query.OrderByDescending(o => o.CreatedAt),
                    _ => query.OrderBy(o => o.CreatedAt)
                }
            };
            TotalPages = query.Count() / PageSize + (query.Count() % PageSize > 0 ? 1 : 0);
            CurrentPage = Math.Min(CurrentPage, TotalPages);
            TotalForSelectedDateRange = (double) (query.Where(o => o.Status != OrderStatus.Cancelled && (o.paymentMethod == PaymentMethod.Card || o.paymentMethod == PaymentMethod.Cash)).Sum(o => (o.TotalPrice != null) ? o.TotalPrice : 0.0 ) ?? 0.0); // making sure that we don't get null values
            List<Order> ResultList = query.Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            DisplayList.Clear(); // Clear the previous list before adding new items
            foreach (var order in ResultList)
            {
                DisplayList.Add(new OrdersHistoryListElement(order, this));
            }
            
            OnPropertyChanged(nameof(isDisplayedOrdersEmpty));
            OnPropertyChanged(nameof(DisplayedOrdersNotEmpty));
            OnPropertyChanged(nameof(TotalForSelectedDateRange));
            DeleteAllSelectedOrdersCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(DisplayedOrdersNotEmpty))]
        public async Task DeleteAllSelectedOrders()
        {
            var result = await showConfirmDeleteRangeOrderDialog(StartDate, EndDate);
            if (!result)
            {
                return;
            }
            var toDelete = dbContext.Orders.Where(o => o.CreatedAt >= StartDate.DateTime && o.CreatedAt <= EndDate.DateTime);
            dbContext.Orders.RemoveRange(toDelete);
            await dbContext.SaveChangesAsync();
            LoadOrders();
        }
    }

    public partial class OrdersHistoryListElement : ObservableObject
    {
        public Order order { get; set; }
        public string orderLocation
        {
            get
            {
                if (order.orderLocation == OrderLocation.Counter)
                    return StringLocalisationService.getStringWithKey("Counter_1");
                else if (order.orderLocation == OrderLocation.Table)
                    return StringLocalisationService.getStringWithKey("Table") + " " + order.TableNumber;
                else
                    return StringLocalisationService.getStringWithKey("Delivery1");
            }
        }
        public string OrderStartTime { get => order.SetPreparingAt?.ToString(new CultureInfo(parentVM.appDataService.AppLanguage)) ?? ""; }

        public string TotalPrice { get => $"{order.TotalPrice?.ToString("F2")} {parentVM.currency}"; }
        public int? TableNumber { get => order.TableNumber; }
        public string OrderId { get => "#" + order.Id.ToString(); }
        public string payment
        {
            get
            {
                if (order.paymentMethod == PaymentMethod.Cash)
                    return StringLocalisationService.getStringWithKey("Cash");
                else if (order.paymentMethod == PaymentMethod.Card)
                    return StringLocalisationService.getStringWithKey("Card");
                else
                    return StringLocalisationService.getStringWithKey("OrdersHistoryPage_Unpaid");
            }
        }
        public string Status
        {
            get
            {
                if (order.Status == OrderStatus.Completed)
                    return StringLocalisationService.getStringWithKey("OrdersHistoryPage_Completed");
                else if (order.Status == OrderStatus.Cancelled)
                    return StringLocalisationService.getStringWithKey("OrdersHistoryPage_Cancelled");
                else
                    return "";
            }
        }
        public int numberOfItems
        {
            get
            {
                if (order.OrderItems == null)
                    return 0;
                else
                    return order.OrderItems.Count;
            }
        }
        public string numberOfItemsString
        {
            get
            {
                if (numberOfItems == 0)
                    return StringLocalisationService.getStringWithKey("NoItems");
                else if (numberOfItems == 1)
                    return numberOfItems + " " + StringLocalisationService.getStringWithKey("Item_4");
                else
                    return numberOfItems + " " + StringLocalisationService.getStringWithKey("Items");
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    if (value)
                    {
                        foreach (var element in parentVM.DisplayList.Where(e => e != this))
                        {
                            element.IsSelected = false;
                        }
                    }
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(isUnselected));
                }
            }
        }
        public bool isUnselected => !IsSelected;

        public OrdersHistoryVM parentVM;

        public OrdersHistoryListElement(Order order, OrdersHistoryVM parentVM)
        {
            this.order = order;
            this.parentVM = parentVM;
        }

        [RelayCommand]
        public void selectOrder()
        {
            if (IsSelected)
            {
                IsSelected = false;
            }
            else
            {
                IsSelected = true;
            }
        }

        [RelayCommand]
        public void showOrderDetails()
        {
            if (parentVM != null)
            {
                parentVM.navigationService.navigateTo(KioberFoodPage.DisplayOrderPage, new List<object> { order.Id });
                parentVM.navigationService.SetNewHeader(null);
            }
        }

        public bool OrderPaymentPending
        {
            get => order.paymentMethod != PaymentMethod.Cash && order.paymentMethod != PaymentMethod.Card;
        }

        [RelayCommand]
        public async Task DeleteOrder()
        {
            var result = await parentVM.showConfirmDeleteOrderDialog(order.Id);
            if (!result)
            {
                return;
            }
            if (parentVM != null)
            {
                parentVM.dbContext.Orders.Remove(order);
                parentVM.dbContext.SaveChanges();
                parentVM.LoadOrders();
            }
        }
    }
}
