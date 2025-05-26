using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.ViewModels
{
    public class OrdersListVM : KioberViewModelBase
    {
        public DispatcherTimer dispatcherTimer;

        public ShowConfirmCancelOrderDialog showConfirmCancelOrderDialog { get; set; } = null!;
        public ShowSetOrderCompletedDialog showSetOrderCompletedDialog { get; set; } = null!;

        public ObservableCollection<Order> WaitingOrders { get; set; }
        public ObservableCollection<OrdersListElement> FilterdOrders { get; set; } = new ObservableCollection<OrdersListElement>();
        public bool isFilterdListEmpty  { get => FilterdOrders.Count == 0; }
        public KFPContext dbContext;
        private AppDataService _appDataService;
        public NavigationService navigationService { get; private set; }
        public Currency currency { get; set; }

        private bool _counterFilter = true;
        public bool CounterFilter { get
            {
                return _counterFilter;
            }
            set 
            {
                _counterFilter = value;
                OnPropertyChanged(nameof(CounterFilter));
                FilterOrders();
            } 
        }
        private bool _tableFilter = true;
        public bool TableFilter
        {
            get
            {
                return _tableFilter;
            }
            set
            {
                _tableFilter = value;
                OnPropertyChanged(nameof(TableFilter));
                FilterOrders();
            }
        }

        private bool _deliveryFilter = true;
        public bool DeliveryFilter {
            get
            {
                return _deliveryFilter;
            }
            set
            {
                _deliveryFilter = value;
                OnPropertyChanged(nameof(DeliveryFilter));
                FilterOrders();
            }
        }
        public bool HasTables => _appDataService?.NumberOfTables > 0;

        public RelayCommand NavigateToPOSCommand { get; set; }
        public RelayCommand NavigatetoTablesCommand { get; set; }
        public OrdersListVM(KFPContext context, AppDataService appDataService, NavigationService navigationService)
        {
            dbContext = context;
            _appDataService = appDataService;
            currency = _appDataService.Currency;
            WaitingOrders = new ObservableCollection<Order>(dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Session)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready)
                .ToList());

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 1); //10000000 x 100 nanoseconds = 1 second

            FilterdOrders = new ObservableCollection<OrdersListElement>(WaitingOrders.Select(o => new OrdersListElement(o,this)).ToList());
            this.navigationService = navigationService;

            NavigateToPOSCommand = new RelayCommand(() => this.navigationService.navigateTo(KioberFoodPage.POS));
            NavigatetoTablesCommand = new RelayCommand(() => this.navigationService.navigateTo(KioberFoodPage.TablesPage));

            dispatcherTimer.Start();
        }

        private void FilterOrders()
        {
            FilterdOrders.Clear();
            foreach (var order in WaitingOrders)
            {
                if ((order.orderLocation == OrderLocation.Counter && CounterFilter) ||
                    (order.orderLocation == OrderLocation.Table && TableFilter) ||
                    (order.orderLocation == OrderLocation.Delivery && DeliveryFilter))
                {
                    FilterdOrders.Add(new OrdersListElement (order, this));
                }
            }
            OnPropertyChanged(nameof(isFilterdListEmpty));
        }
    }

    public partial class OrdersListElement : ObservableObject
    {
        public Order order { get; set; }
        public string orderLocation
        {
            get
            {
                if (order.orderLocation == OrderLocation.Counter)
                    return StringLocalisationService.getStringWithKey("Counter");
                else if (order.orderLocation == OrderLocation.Table)
                    return StringLocalisationService.getStringWithKey("Table") + " " + order.TableNumber;
                else
                    return StringLocalisationService.getStringWithKey("Delivery");
            }
        }
        public string OrderStartTime { get => order.SetPreparingAt?.ToString("HH:mm") ?? ""; }
        public TimeSpan OrderTimePassed
        {
            get
            {
                if (order.SetPreparingAt == null)
                    return TimeSpan.Zero;
                else
                    return DateTime.Now - order.SetPreparingAt.Value;
            }
        }
        public string OrderTimePassedString { 
            get {
                if (OrderTimePassed < TimeSpan.FromDays(1))
                    return string.Format("{0:hh\\:mm\\:ss}", OrderTimePassed);
                else
                    return string.Format("{0:dd}d {1:hh\\:mm\\:ss}", OrderTimePassed, OrderTimePassed);
            } 
        }

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
                    return StringLocalisationService.getStringWithKey("Waiting");
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
                    return numberOfItems + " " + StringLocalisationService.getStringWithKey("Item");
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
                        foreach (var element in parentVM.FilterdOrders.Where(e => e != this))
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

        public OrdersListVM parentVM;

        public OrdersListElement(Order order, OrdersListVM parentVM)
        {
            this.order = order;
            this.parentVM = parentVM;
            parentVM.dispatcherTimer.Tick += DispatcherTimer_Tick1;
        }

        private void DispatcherTimer_Tick1(object? sender, object e)
        {
            OnPropertyChanged(nameof(OrderTimePassedString));
            OnPropertyChanged(nameof(OrderTimePassed));
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
            }
        }

        [RelayCommand]
        public async Task setOrderCompleted()
        {
            var result = await parentVM.showSetOrderCompletedDialog(order.Id);
            if (!result)
            {
                return;
            }
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.Now;
            if (parentVM != null)
            {
                parentVM.FilterdOrders.Remove(this);
                parentVM.WaitingOrders.Remove(order);
                parentVM.dbContext.Orders.Update(order);
                parentVM.dbContext.SaveChanges();
            }
        }

        [RelayCommand(CanExecute = nameof(OrderPaymentPending))]
        public void EditOrder()
        {
            if (parentVM != null)
            {
                parentVM.navigationService.navigateTo(KioberFoodPage.POS, new List<object> { order.Id });
            }
        }
        public bool OrderPaymentPending
        {
            get => order.paymentMethod != PaymentMethod.Cash && order.paymentMethod != PaymentMethod.Card;
        }
        [RelayCommand(CanExecute = nameof(OrderPaymentPending))]
        public async Task CancelOrder()
        {
            var result = await parentVM.showConfirmCancelOrderDialog(order.Id);
            if (!result)
            {
                return;
            }
            order.Status = OrderStatus.Cancelled;
            order.CanceledAt = DateTime.Now;
            if (parentVM != null)
            {
                parentVM.FilterdOrders.Remove(this);
                parentVM.WaitingOrders.Remove(order);
                parentVM.dbContext.Orders.Update(order);
                parentVM.dbContext.SaveChanges();
            }
        }
    }
}
