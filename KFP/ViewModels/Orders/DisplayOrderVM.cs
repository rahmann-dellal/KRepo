using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;

namespace KFP.ViewModels
{
    public delegate Task<bool> ShowConfirmCancelOrderDialog(int orderId);
    public delegate Task<bool> ShowConfirmCashPaymentDialog(double? total, string currency);
    public delegate Task<bool> ShowConfirmCardPaymentDialog(double? total, string currency);
    public delegate Task<bool> ShowSetOrderCompletedDialog(int orderId);
    public partial class DisplayOrderVM : KioberViewModelBase
    {
        private Order _order;

        public Order order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public ShowConfirmCancelOrderDialog showConfirmCancelOrderDialog { get; set; } = null!;
        public ShowConfirmCashPaymentDialog showConfirmCashPaymentDialog { get; set; } = null!;
        public ShowConfirmCardPaymentDialog showConfirmCardPaymentDialog { get; set; } = null!;
        public ShowSetOrderCompletedDialog showSetOrderCompletedDialog { get; set; } = null!;

        private KFPContext dbContext;
        private NavigationService _navigationService;
        private AppDataService _appDataService;
        private Currency _currency;
        private SessionManager _sessionManager;
        private IPrintingService _printingService;

        public bool HasTables;

        public bool IsSetOnCounter => order?.orderLocation == OrderLocation.Counter;
        public int? SelectedTableNumber => order?.TableNumber ?? null;
        public bool IsSetForDelivery => order?.orderLocation == OrderLocation.Delivery;
        public Currency Currency;
        public bool isPaidCash { get; set; }
        public bool isPaidCard { get; set; }
        public bool PaymentDeferred { get; set; }
        public bool PaymentSet { get => isPaidCard || isPaidCash || PaymentDeferred; }
        public bool PaymentPending
        {
            get
            {
                return !PaymentSet && !isOrderFinalized;
            }
        }
        public bool CanEditOrder
        {
            get
            {
                return !isPaidCard && !isPaidCash && !isOrderFinalized;
            }
        }

        public RelayCommand NavigateToOrdersCommand { get; set; }
        public RelayCommand NavigatetoTablesCommand { get; set; }
        public DisplayOrderVM(KFPContext context, NavigationService ns, AppDataService ads, SessionManager sessionManager, IPrintingService printingService)
        {
            dbContext = context;
            _navigationService = ns;
            _appDataService = ads;
            _currency = _appDataService.Currency;
            HasTables = _appDataService.NumberOfTables > 0;
            Currency = _appDataService.Currency;
            _sessionManager = sessionManager;
            NavigateToOrdersCommand = new RelayCommand(() => _navigationService.navigateTo(KioberFoodPage.OrdersListPage));
            NavigatetoTablesCommand = new RelayCommand(() => _navigationService.navigateTo(KioberFoodPage.TablesPage));
            _printingService = printingService;
        }

        public void LoadOrder(int orderId)
        {
            order = dbContext.Orders.Include(o => o.DeliveryInfo)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.AddOns)
                .FirstOrDefault(o => o.Id == orderId)!;
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            if (order.paymentMethod == PaymentMethod.Cash)
            {
                isPaidCash = true;
                isPaidCard = false;
                PaymentDeferred = false;
            }
            else if (order.paymentMethod == PaymentMethod.Card)
            {
                isPaidCard = true;
                isPaidCash = false;
                PaymentDeferred = false;
            }
            else
            {
                isPaidCard = false;
                isPaidCash = false;
                PaymentDeferred = false;
            }
            OnPropertyChanged();
        }

        [RelayCommand(CanExecute = nameof(CanEditOrder))]
        public async Task CancelOrder()
        {
            bool confirmation = await showConfirmCancelOrderDialog(order.Id);
            if (!confirmation)
            {
                return;
            }
            order.Status = OrderStatus.Cancelled;
            order.CanceledAt = DateTime.UtcNow;
            dbContext.Update(order);
            dbContext.SaveChanges();
            _navigationService.navigateTo(KioberFoodPage.POS);
        }

        [RelayCommand(CanExecute = nameof(CanEditOrder))]
        public void EditOrder()
        {
            _navigationService.navigateTo(KioberFoodPage.POS, new List<object>() { order.Id });
        }

        [RelayCommand]
        public async Task ConfirmCashPayment()
        {
            bool confirmation = await showConfirmCashPaymentDialog(order.TotalPrice, Currency.ToString());
            if (!confirmation)
            {
                return;
            }
            isPaidCash = true;
            isPaidCard = false;
            PaymentDeferred = false;
            order.paymentMethod = PaymentMethod.Cash;
            PaymentReceipt receipt = CreateReceipt(PaymentMethod.Cash);
            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(isPaidCash));
            OnPropertyChanged(nameof(isPaidCard));
            OnPropertyChanged(nameof(PaymentDeferred));
            OnPropertyChanged(nameof(CanEditOrder));
            OnPropertyChanged(nameof(canSetOrderCompleted));
            setOrderCompletedCommand.NotifyCanExecuteChanged();
            ChangePaymentMethodeCommand.NotifyCanExecuteChanged();
            if (receipt != null)
            {
                _printingService.PrintReceipt(receipt);
            }
        }

        [RelayCommand]
        public async Task ConfirmCardPayment()
        {
            bool confirmation = await showConfirmCardPaymentDialog(order.TotalPrice, Currency.ToString());
            if (!confirmation)
            {
                return;
            }
            isPaidCard = true;
            isPaidCash = false;
            PaymentDeferred = false;
            order.paymentMethod = PaymentMethod.Card;
            PaymentReceipt receipt = CreateReceipt(PaymentMethod.Card);
            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(isPaidCard));
            OnPropertyChanged(nameof(isPaidCash));
            OnPropertyChanged(nameof(PaymentDeferred));
            OnPropertyChanged(nameof(CanEditOrder));
            OnPropertyChanged(nameof(canSetOrderCompleted));
            setOrderCompletedCommand.NotifyCanExecuteChanged();
            ChangePaymentMethodeCommand.NotifyCanExecuteChanged();
            if (receipt != null)
            {
                _printingService.PrintReceipt(receipt);
            }
        }

        [RelayCommand]
        public void DeferPayment()
        {
            PaymentDeferred = true;
            isPaidCard = false;
            isPaidCash = false;
            order.paymentMethod = null;
            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(PaymentDeferred));
            OnPropertyChanged(nameof(isPaidCard));
            OnPropertyChanged(nameof(isPaidCash));
            OnPropertyChanged(nameof(canSetOrderCompleted));
            setOrderCompletedCommand.NotifyCanExecuteChanged();
            ChangePaymentMethodeCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(canChangePaymentMethode))]
        public void ChangePaymentMethode()
        {

            isPaidCard = false;
            isPaidCash = false;
            PaymentDeferred = false;

            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(canSetOrderCompleted));
        }

        public bool canChangePaymentMethode
        {
            get => !isPaidCard && !isPaidCash;
        }
        public PaymentReceipt CreateReceipt(PaymentMethod paymentMethod)
        {
            PaymentReceipt receipt = new PaymentReceipt
            {
                OrderId = order.Id,
                AppUserId = order.AppUserId,
                TotalPrice = order.TotalPrice,
                paymentMethod = paymentMethod,
                AppUserName = order.AppUserName,
                IssuedAt = DateTime.UtcNow,
                SessionId = _sessionManager.CurrentSession.SessionId,
                Session = _sessionManager.CurrentSession,
            };

            foreach (var oi in order.OrderItems)
            {
                receipt.Sales.Add(new Sale()
                {
                    ItemName = oi.MenuItemName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    MenuItem = oi.MenuItem,
                    Receipt = receipt,
                });
                if (oi.AddOns != null && oi.AddOns.Count > 0)
                {
                    foreach (var addOn in oi.AddOns)
                    {
                        receipt.Sales.Add(new Sale()
                        {
                            ItemName = addOn.MenuItemName,
                            UnitPrice = addOn.UnitPrice,
                            Quantity = addOn.Quantity,
                            Receipt = receipt,
                        });
                    }
                }
            }
            dbContext.PaymentReceipts.Add(receipt);
            dbContext.Orders.Update(order);
            dbContext.SaveChanges();
            return receipt;
        }

        [RelayCommand]
        public void NewOrder()
        {
            _navigationService.navigateTo(KioberFoodPage.POS);
        }
        [RelayCommand(CanExecute = nameof(canSetOrderCompleted))]
        public async Task SetOrderCompleted()
        {
            bool confirmation = await showSetOrderCompletedDialog(order.Id);
            if (!confirmation)
            {
                return;
            }
            order.CompletedAt = DateTime.UtcNow;
            order.Status = OrderStatus.Completed;
            dbContext.Update(order);
            dbContext.SaveChanges();
            _navigationService.navigateTo(KioberFoodPage.POS);
        }
        public bool canSetOrderCompleted
        {
            get => !isOrderFinalized && PaymentSet;
        }

        public bool isOrderFinalized
        {
            get
            {
                return order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled;
            }
        }
        public string Status
        {
            get
            {
                if (order.Status == OrderStatus.Completed)
                    return StringLocalisationService.getStringWithKey("Completed2");
                else if (order.Status == OrderStatus.Cancelled)
                    return StringLocalisationService.getStringWithKey("Cancelled2");
                else
                    return "";
            }
        }
    }
}
