using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.EntityFrameworkCore;

namespace KFP.ViewModels
{

    class DisplayOrderVM : KioberViewModelBase
    {
        private Order _order;

        public Order order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }
        
        private KFPContext dbContext;
        private NavigationService _navigationService;
        private AppDataService _appDataService;
        private Currency _currency;

        public bool HasTables;

        public bool IsSetOnCounter => order?.Type == OrderType.Counter;
        public int? SelectedTableNumber => order?.TableNumber ?? null;
        public bool IsSetForDelivery => order?.Type == OrderType.Delivery;
        public Currency Currency;
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
    }
}
