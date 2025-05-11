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
    public class OrderingVM : KioberViewModelBase
    {
        private AppDataService _appDataService;
        public MenuItemSelectorVM menuItemSelectorVM;
        public OrderVM orderVM;
        public ObservableCollection<TableListElement> TableListElements { get; set; }
        public ObservableCollection<Order> PendingOrders { get; set; }

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
        public int numberOfTables { get; set; }
        public bool HasTables => numberOfTables > 0;
        public ICommand SetPreparingCommand { get; }
        public ICommand ConfirmPaymentCashCommand { get; }
        public ICommand ConfirmPaymentCardCommand { get; }
        public RelayCommand SetOnCounterCommand { get; set; }
        public RelayCommand SetForDeliveryCommand { get; set; }

        public OrderingVM(KFPContext context, MenuItemSelectorVM menuItemSelectorVM, OrderVM orderVM, AppDataService appDataService)
        {
            _appDataService = appDataService;
            numberOfTables = _appDataService.NumberOfTables;
            dbContext = context;
            SetPreparingCommand = new RelayCommand(() => SetPreparing());
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
            }, () => IsSetForDelivery != true);

            PendingOrders = new ObservableCollection<Order>(dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Session)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready)
                .ToList());

            TableListElements = new ObservableCollection<TableListElement>();
            for (int i = 1; i <= numberOfTables; i++)
            {
                var element = new TableListElement(i, this);
                TableListElements.Add(element);
                if (PendingOrders.Count > 0)
                {
                    var order = PendingOrders.FirstOrDefault(o => o.TableNumber == i);
                    if (order != null)
                    {
                        element.order = order;
                    }
                }
            }
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


        private void SetPreparing()
        {
            CurrentOrder.Status = OrderStatus.Preparing;
            CurrentOrder.SetPreparingAt = DateTime.Now;
        }

        private void ConfirmPayment(string method)
        {
            // handle payment logic
            CurrentOrder.Status = OrderStatus.Completed;
            CurrentOrder.CompletedAt = DateTime.Now;
        }
    }

    public class TableListElement
    {
        public TableListElement(int tableNumber, OrderingVM orderingVM)
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
        private OrderingVM parentVM;
        public int TableNumber { get; set; }
        public string TableName => $"Table {TableNumber}";
        public Order? order { get; set; }

        public RelayCommand selectTableCommand { get; set; }
    }
}
