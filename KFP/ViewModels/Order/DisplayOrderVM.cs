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
    public delegate Task<bool> ShowConfirmCashPaymentDialog();
    public delegate Task<bool> ShowConfirmCardPaymentDialog();
    public delegate Task<bool> ShowSetOrderCompletedDialog();
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

        public bool HasTables;

        public bool IsSetOnCounter => order?.Type == OrderLocation.Counter;
        public int? SelectedTableNumber => order?.TableNumber ?? null;
        public bool IsSetForDelivery => order?.Type == OrderLocation.Delivery;
        public Currency Currency;
        public bool isPaidCash { get; set; }
        public bool isPaidCard { get; set; }
        public bool PaymentDeferred { get; set; }
        public bool PaymentSet { get => isPaidCard || isPaidCash || PaymentDeferred; }
        public bool PaymentPending { 
            get {
                    return !PaymentSet;
                } 
        }
        public bool CanEditOrder
        {
            get
            {
                return !isPaidCard && !isPaidCash;
            }
        }
        public DisplayOrderVM(KFPContext context, NavigationService ns, AppDataService ads)
        {
            dbContext = context;
            _navigationService = ns;
            _appDataService = ads;
            _currency = _appDataService.Currency;
            HasTables = _appDataService.NumberOfTables > 0;
            Currency = _appDataService.Currency;
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
            OnPropertyChanged();
        }

        [RelayCommand(CanExecute = nameof(CanEditOrder))]
        public async Task CancelOrder()
        {
            bool confirmation = await  showConfirmCancelOrderDialog(order.Id);
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


        [RelayCommand]
        public async Task ConfirmCashPayment ()
        {
            bool confirmation = await showConfirmCashPaymentDialog();
            if (!confirmation)
            {
                return;
            }
            isPaidCash = true;
            isPaidCard = false;
            PaymentDeferred = false;
            Invoice invoice = CreateInvoice(PaymentMethod.Cash);
            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(isPaidCash));
            OnPropertyChanged(nameof(isPaidCard));
            OnPropertyChanged(nameof(PaymentDeferred));
            OnPropertyChanged(nameof(CanEditOrder));
            ChangePaymentMethodeCommand.NotifyCanExecuteChanged();
            //TODO:Print invoice
        }

        [RelayCommand]
        public async Task ConfirmCardPayment()
        {
            bool confirmation = await showConfirmCardPaymentDialog();
            if (!confirmation)
            {
                return;
            }
            isPaidCard = true;
            isPaidCash = false;
            PaymentDeferred = false;
            Invoice invoice = CreateInvoice(PaymentMethod.Card);
            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(isPaidCard));
            OnPropertyChanged(nameof(isPaidCash));
            OnPropertyChanged(nameof(PaymentDeferred));
            OnPropertyChanged(nameof(CanEditOrder));
            ChangePaymentMethodeCommand.NotifyCanExecuteChanged();
            //TODO:Print invoice
        }

        [RelayCommand]
        public void DeferPayment()
        {
            PaymentDeferred = true;
            isPaidCard = false;
            isPaidCash = false;
            OnPropertyChanged(nameof(PaymentSet));
            OnPropertyChanged(nameof(PaymentPending));
            OnPropertyChanged(nameof(PaymentDeferred));
            OnPropertyChanged(nameof(isPaidCard));
            OnPropertyChanged(nameof(isPaidCash));

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
        }

        public bool canChangePaymentMethode()
        {
            return !isPaidCard && !isPaidCash;
        }
        public Invoice CreateInvoice(PaymentMethod paymentMethod)
        {
            Invoice invoice = new Invoice
            {
                OrderId = order.Id,
                AppUserId = order.AppUserId,
                TotalPrice = order.TotalPrice,
                paymentMethod = paymentMethod,
                AppUserName = order.AppUserName,
                IssuedAt = DateTime.UtcNow,
            };

            foreach(var oi in order.OrderItems)
            {
                invoice.Sales.Add (new Sale()
                {
                    ItemName = oi.MenuItemName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    MenuItem = oi.MenuItem,
                    Invoice = invoice,
                });
                if (oi.AddOns != null && oi.AddOns.Count > 0)
                {
                    foreach (var addOn in oi.AddOns)
                    {
                        invoice.Sales.Add(new Sale()
                        {
                            ItemName = addOn.MenuItemName,
                            UnitPrice = addOn.UnitPrice,
                            Quantity = addOn.Quantity,
                            Invoice = invoice,
                        });
                    }
                }
            }
            dbContext.Invoices.Add(invoice);
            dbContext.Orders.Update(order);
            dbContext.SaveChanges();
            return invoice;
        }

        [RelayCommand]
        public void NewOrder()
        {
            _navigationService.navigateTo(KioberFoodPage.POS);
        }
        [RelayCommand]
        public async Task OrderCompletedCommand()
        {
            bool confirmation = await showSetOrderCompletedDialog();
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
    }
}
