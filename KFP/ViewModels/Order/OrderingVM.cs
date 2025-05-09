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

namespace KFP.ViewModels
{
    public class OrderingVM : KioberViewModelBase
    {
        public MenuItemSelectorVM menuItemSelectorVM;
        public OrderVM orderVM;

        private KFPContext dbContext;
        public ObservableCollection<MenuItem> MenuItems { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        private KFP.DATA.Order _currentOrder = new();
        public KFP.DATA.Order CurrentOrder
        {
            get => _currentOrder;
            set { _currentOrder = value; OnPropertyChanged(); }
        }

        private OrderItem? _selectedOrderItem;
        public OrderItem? SelectedOrderItem
        {
            get => _selectedOrderItem;
            set { _selectedOrderItem = value; OnPropertyChanged(); }
        }

        public ICommand SendToKitchenCommand { get; }
        public ICommand ConfirmPaymentCashCommand { get; }
        public ICommand ConfirmPaymentCardCommand { get; }

        public OrderingVM(KFPContext context, MenuItemSelectorVM menuItemSelectorVM, OrderVM orderVM)
        {
            dbContext = context;
            SendToKitchenCommand = new RelayCommand(() => SendToKitchen());
            ConfirmPaymentCashCommand = new RelayCommand(() => ConfirmPayment("Cash"));
            ConfirmPaymentCardCommand = new RelayCommand(() => ConfirmPayment("Card"));
            this.menuItemSelectorVM = menuItemSelectorVM;
            this.orderVM = orderVM;

            menuItemSelectorVM.PropertyChanged += MenuItemSelectorVM_PropertyChanged;
        }

        private void MenuItemSelectorVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(menuItemSelectorVM.SelectedMenuItem))
            {
                orderVM.OnMenuItemSelected(menuItemSelectorVM.SelectedMenuItem);
            } 
        }


        private void SendToKitchen()
        {
            CurrentOrder.Status = OrderStatus.Preparing;
            CurrentOrder.isPreparing = DateTime.Now;
        }

        private void ConfirmPayment(string method)
        {
            // handle payment logic
            CurrentOrder.Status = OrderStatus.Completed;
            CurrentOrder.CompletedAt = DateTime.Now;
        }
    }

}
