using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using System.Windows.Input;
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
        public EditOrderVM orderVM;
        public ObservableCollection<TableListElement> TableListElements { get; set; }
        public ObservableCollection<Order> CurrentOrders { get; set; }

        private KFPContext dbContext;
        public ObservableCollection<MenuItem> MenuItems { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        private KFP.DATA.Order _currentOrder = new();
        public KFP.DATA.Order CurrentOrder
        {
            get => _currentOrder;
            set { _currentOrder = value; OnPropertyChanged(); }
        }


        private bool _isSetOnCounter = true;

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
        public bool IsSetForDelivery
        {
            get => _isSetForDelivery;
            set
            {
                _isSetForDelivery = value;
                OnPropertyChanged(nameof(IsSetForDelivery));
            }
        }

        private int? _selectedTableNumber = null;
        public int? SelectedTableNumber
        {
            get => _selectedTableNumber;
            set
            {
                _selectedTableNumber = value;
                OnPropertyChanged(nameof(SelectedTableNumber));
            }
        }

        public string Notes { get; set; } = string.Empty;
        public int numberOfTables { get; set; }
        public bool HasTables => numberOfTables > 0;
        public RelayCommand TakeOrderCommand { get; }
        public RelayCommand ConfirmPaymentCashCommand { get; }
        public RelayCommand ConfirmPaymentCardCommand { get; }
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
            ConfirmPaymentCashCommand = new RelayCommand(() => ConfirmPayment("Cash"));
            ConfirmPaymentCardCommand = new RelayCommand(() => ConfirmPayment("Card"));
            this.menuItemSelectorVM = menuItemSelectorVM;
            this.orderVM = orderVM;

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

            CurrentOrders = new ObservableCollection<Order>(dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Session)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready)
                .ToList());

            TableListElements = new ObservableCollection<TableListElement>();
            for (int i = 1; i <= numberOfTables; i++)
            {
                var element = new TableListElement(i, this);
                TableListElements.Add(element);
                if (CurrentOrders.Count > 0)
                {
                    var order = CurrentOrders.FirstOrDefault(o => o.TableNumber == i);
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
        }

        private void MenuItemSelectorVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(menuItemSelectorVM.SelectedMenuItem))
            {
                orderVM.OnMenuItemSelected(menuItemSelectorVM.SelectedMenuItem);
            }
        }

        public void loadOrder(Order order)
        {
            CurrentOrder = order;
            if (CurrentOrder != null)
            {
                //TODO: Load order
            }
        }


        private void TakeOrder()
        {
            CurrentOrder = orderVM.order;
            CurrentOrder.Status = OrderStatus.Preparing;
            CurrentOrder.SetPreparingAt = DateTime.Now;
            CurrentOrder.Session = _sessionManager.CurrentSession;
            CurrentOrder.SessionId = _sessionManager.CurrentSession.SessionId;
            CurrentOrder.AppUser = _sessionManager.LoggedInUser;
            CurrentOrder.AppUserId = _sessionManager.LoggedInUser?.AppUserID;
            CurrentOrder.AppUserName = _sessionManager.LoggedInUser?.UserName;
            CurrentOrder.CreatedAt = DateTime.Now;
            CurrentOrder.TotalPrice = orderVM.orderTotalPrice;
            if (!String.IsNullOrEmpty(Notes))
            {
                CurrentOrder.Notes = Notes;
            }

            if (IsSetOnCounter)
            {
                CurrentOrder.Type = OrderType.Counter;
            }
            else if (IsSetForDelivery)
            {
                CurrentOrder.Type = OrderType.Delivery;
                if (!String.IsNullOrEmpty(DeliveryInfo.CustomerName) || !String.IsNullOrEmpty(DeliveryInfo.PhoneNumber) || !String.IsNullOrEmpty(DeliveryInfo.Address))
                {
                    CurrentOrder.DeliveryInfo = DeliveryInfo; 
                }
            }
            else
            {
                CurrentOrder.Type = OrderType.Table;
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
            if (result > 0) {
                _navigationService.navigateTo(KioberFoodPage.DisplayOrderPage, new List<object> { CurrentOrder.Id });
            }
        }

        public bool canTakeOrder
        {
            get
            {
                if (orderVM.order == null)
                    return false;
                if (orderVM.order.OrderItems.Count == 0)
                    return false;

                return true;
            }
        }
        public bool isOrderEmpty => orderVM.order == null || orderVM.order.OrderItems.Count == 0;

        private void ConfirmPayment(string method)
        {
            // handle payment logic
            CurrentOrder.Status = OrderStatus.Completed;
            CurrentOrder.CompletedAt = DateTime.Now;
        }
    }

    public class TableListElement
    {
        public TableListElement(int tableNumber, PointOfSalesVM orderingVM)
        {
            TableNumber = tableNumber;
            parentVM = orderingVM;
            selectTableCommand = new RelayCommand(() =>
            {
                if (order != null)
                {
                    orderingVM.loadOrder(order);
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

        public RelayCommand selectTableCommand { get; set; }
    }
}
