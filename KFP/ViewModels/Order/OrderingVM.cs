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

        public ICommand AddQuantityCommand { get; }
        public ICommand ReduceQuantityCommand { get; }
        public ICommand DeleteOrderItemCommand { get; }
        public ICommand SendToKitchenCommand { get; }
        public ICommand ConfirmPaymentCashCommand { get; }
        public ICommand ConfirmPaymentCardCommand { get; }
        public ICommand ClearOrderCommand { get; }

        public OrderingVM(KFPContext context)
        {
            dbContext = context;
            AddQuantityCommand = new RelayCommand(() => AddQuantity());
            ReduceQuantityCommand = new RelayCommand(() => ReduceQuantity());
            DeleteOrderItemCommand = new RelayCommand(() => DeleteOrderItem());
            SendToKitchenCommand = new RelayCommand(() => SendToKitchen());
            ConfirmPaymentCashCommand = new RelayCommand(() => ConfirmPayment("Cash"));
            ConfirmPaymentCardCommand = new RelayCommand(() => ConfirmPayment("Card"));
            ClearOrderCommand = new RelayCommand(() => ClearOrder());
        }

        private void AddQuantity()
        {
            if (SelectedOrderItem != null)
                SelectedOrderItem.Quantity++;
        }

        private void ReduceQuantity()
        {
            if (SelectedOrderItem != null && SelectedOrderItem.Quantity > 1)
                SelectedOrderItem.Quantity--;
        }

        private void DeleteOrderItem()
        {
            if (SelectedOrderItem != null)
                CurrentOrder.OrderItems.Remove(SelectedOrderItem);
        }

        private void SendToKitchen()
        {
            CurrentOrder.Status = OrderStatus.Preparing;
            CurrentOrder.SendToKitchenAt = DateTime.Now;
        }

        private void ConfirmPayment(string method)
        {
            // handle payment logic
            CurrentOrder.Status = OrderStatus.Completed;
            CurrentOrder.CompletedAt = DateTime.Now;
        }

        private void ClearOrder()
        {
            CurrentOrder = new KFP.DATA.Order();
        }
    }

}
