using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using Microsoft.EntityFrameworkCore;
using KFP.Services;

namespace KFP.ViewModels
{
    public class PointOfSalesVM : KioberViewModelBase
    {
        private AppDataService _appDataService;
        private SessionManager _sessionManager;
        private NavigationService _navigationService;
        public MenuItemSelectorVM menuItemSelectorVM;
        public EditOrderVM editOrderVM;
        public ObservableCollection<TableListElement> TableListElements { get; set; }
        public ObservableCollection<Order> WaitingOrders { get; set; } //list of pending orders displyed in the tables flyout

        private KFPContext dbContext;

        private KFP.DATA.Order _currentOrder = new();
        public KFP.DATA.Order CurrentOrder //order being created or edited
        {
            get => _currentOrder;
            set { _currentOrder = value; OnPropertyChanged(); }
        }


        private bool _isSetOnCounter = true; //current order set on counter

        public bool IsSetOnCounter
        {
            get => _isSetOnCounter;
            set
            {
                _isSetOnCounter = value;
                OnPropertyChanged(nameof(IsSetOnCounter));
            }
        }

        private DeliveryInfo _deliveryInfo = new();
        public DeliveryInfo DeliveryInfo
        {
            get => _deliveryInfo;
            set
            {
                _deliveryInfo = value;
                OnPropertyChanged(nameof(DeliveryInfo));
            }
        }
        private bool _isSetForDelivery = false;
        public bool IsSetForDelivery //current order set for delivery
        {
            get => _isSetForDelivery;
            set
            {
                _isSetForDelivery = value;
                OnPropertyChanged(nameof(IsSetForDelivery));
            }
        }

        private int? _selectedTableNumber = null;
        public int? SelectedTableNumber //current order table number
        {
            get => _selectedTableNumber;
            set
            {
                _selectedTableNumber = value;
                OnPropertyChanged(nameof(SelectedTableNumber));
            }
        }

        public string Notes { get; set; } = string.Empty;
        public int numberOfTables { get; set; } //number of tables in the restaurant as set in the app settings
        public bool HasTables => numberOfTables > 0;
        public RelayCommand TakeOrderCommand { get; }
        public RelayCommand SetOnCounterCommand { get; set; }
        public RelayCommand SetForDeliveryCommand { get; set; }

        public PointOfSalesVM(KFPContext context, MenuItemSelectorVM menuItemSelectorVM, EditOrderVM orderVM, AppDataService appDataService, SessionManager sessionManager, NavigationService ns)
        {
            _appDataService = appDataService;
            _navigationService = ns;
            _sessionManager = sessionManager;
            dbContext = context;

            numberOfTables = _appDataService.NumberOfTables;

            TakeOrderCommand = new RelayCommand(() => TakeOrder(), () => canTakeOrder);
            this.menuItemSelectorVM = menuItemSelectorVM;
            this.editOrderVM = orderVM;

            menuItemSelectorVM.PropertyChanged += MenuItemSelectorVM_PropertyChanged;

            SetOnCounterCommand = new RelayCommand(() =>
            {
                IsSetOnCounter = true;
                IsSetForDelivery = false;
                SelectedTableNumber = null;
                SetOnCounterCommand.NotifyCanExecuteChanged();
                SetForDeliveryCommand.NotifyCanExecuteChanged();
            }, () => IsSetOnCounter != true);
            SetForDeliveryCommand = new RelayCommand(() =>
            {
                IsSetOnCounter = false;
                IsSetForDelivery = true;
                SelectedTableNumber = null;
                SetOnCounterCommand.NotifyCanExecuteChanged();
                SetForDeliveryCommand.NotifyCanExecuteChanged();
            });

            WaitingOrders = new ObservableCollection<Order>(dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Session)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready)
                .ToList());

            TableListElements = new ObservableCollection<TableListElement>();
            for (int i = 1; i <= numberOfTables; i++)
            {
                var element = new TableListElement(i, this, _navigationService);
                TableListElements.Add(element);
                if (WaitingOrders.Count > 0)
                {
                    var order = WaitingOrders.FirstOrDefault(o => o.TableNumber == i);
                    if (order != null)
                    {
                        element.order = order;
                    }
                }
            }

            orderVM.OrderItemElements.CollectionChanged += (s, e) =>
            {
                TakeOrderCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(canTakeOrder));
                OnPropertyChanged(nameof(isOrderEmpty));
            };
            loadOrder(new KFP.DATA.Order());
        }

        private void MenuItemSelectorVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(menuItemSelectorVM.SelectedMenuItem))
            {
                editOrderVM.OnMenuItemSelected(menuItemSelectorVM.SelectedMenuItem);
            }
        }

        public void loadOrder(int OrderId)
        {
            var order = dbContext.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns)
                .Include(o => o.Session)
                .Include(o => o.DeliveryInfo)
                .FirstOrDefault(o => o.Id == OrderId);
            if (order != null) { 
                loadOrder(order);
            }
        }
        public void loadOrder(Order order)
        {
            CurrentOrder = order;
            if (CurrentOrder == null)
            {
                CurrentOrder = new KFP.DATA.Order();

            }
            editOrderVM.loadOrder(CurrentOrder);
            IsSetOnCounter = CurrentOrder.orderLocation == OrderLocation.Counter;
            IsSetForDelivery = CurrentOrder.orderLocation == OrderLocation.Delivery;
            DeliveryInfo = CurrentOrder.DeliveryInfo;
            Notes = CurrentOrder.Notes;
            SelectedTableNumber = CurrentOrder.TableNumber;
        }


        private void TakeOrder()
        {
            CurrentOrder.Status = OrderStatus.Preparing;
            CurrentOrder.SetPreparingAt = DateTime.Now;
            if(CurrentOrder.Session == null)
            {
                CurrentOrder.Session = _sessionManager.CurrentSession;
                CurrentOrder.SessionId = _sessionManager.CurrentSession.SessionId;
                CurrentOrder.AppUser = _sessionManager.LoggedInUser;
                CurrentOrder.AppUserId = _sessionManager.LoggedInUser?.AppUserID;
                CurrentOrder.AppUserName = _sessionManager.LoggedInUser?.UserName;
            }
            
            if(CurrentOrder.CreatedAt != null) { 
                CurrentOrder.CreatedAt = DateTime.Now;
            }
            CurrentOrder.TotalPrice = editOrderVM.orderTotalPrice;
            if (!String.IsNullOrEmpty(Notes))
            {
                CurrentOrder.Notes = Notes;
            }

            if (IsSetOnCounter)
            {
                CurrentOrder.orderLocation = OrderLocation.Counter;
            }
            else if (IsSetForDelivery)
            {
                CurrentOrder.orderLocation = OrderLocation.Delivery;
                if (!String.IsNullOrEmpty(DeliveryInfo.CustomerName) || !String.IsNullOrEmpty(DeliveryInfo.PhoneNumber) || !String.IsNullOrEmpty(DeliveryInfo.Address))
                {
                    CurrentOrder.DeliveryInfo = DeliveryInfo;
                }
            }
            else
            {
                CurrentOrder.orderLocation = OrderLocation.Table;
                CurrentOrder.TableNumber = SelectedTableNumber;
            }
            if (dbContext.Orders.Any(o => o.Id == CurrentOrder.Id))
            {
                dbContext.Orders.Update(CurrentOrder);
            }
            else
            {
                dbContext.Orders.Add(CurrentOrder);
            }
            var result = dbContext.SaveChanges();
            if (result > 0)
            {
                _navigationService.navigateTo(KioberFoodPage.DisplayOrderPage, new List<object> { CurrentOrder.Id });
            }
        }

        public bool canTakeOrder
        {
            get
            {
                if (CurrentOrder == null)
                    return false;
                if (CurrentOrder.OrderItems.Count == 0)
                    return false;

                return true;
            }
        }
        public bool isOrderEmpty => CurrentOrder == null || CurrentOrder.OrderItems.Count == 0;
    }

    public class TableListElement
    {
        public TableListElement(int tableNumber, PointOfSalesVM POSVM, NavigationService navigationService)
        {
            _navigationService = navigationService;
            TableNumber = tableNumber;
            parentVM = POSVM;
            selectTableCommand = new RelayCommand(() =>
            {
                if (order != null)
                {
                    _navigationService.navigateTo(KioberFoodPage.DisplayOrderPage, new List<object> { order.Id });
                }
                parentVM.SelectedTableNumber = TableNumber;
                parentVM.IsSetOnCounter = false;
                parentVM.IsSetForDelivery = false;
                parentVM.SetOnCounterCommand.NotifyCanExecuteChanged();
                parentVM.SetForDeliveryCommand.NotifyCanExecuteChanged();
            }, () =>
            parentVM.SelectedTableNumber != TableNumber
            );
        }
        private PointOfSalesVM parentVM;
        public int TableNumber { get; set; }
        public string TableName => $"Table {TableNumber}";
        public Order? order { get; set; }
        private NavigationService _navigationService;

        public RelayCommand selectTableCommand { get; set; }
    }
}
