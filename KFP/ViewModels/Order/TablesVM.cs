using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;


namespace KFP.ViewModels
{
    public class TablesVM
    {
        private KFPContext _dbContext;
        private NavigationService _navigationService;
        private AppDataService _appDataService;
        public DispatcherTimer dispatcherTimer;

        public List<Order> TableOrders { get; set; }
        public ObservableCollection<TablesElement> Tables { get; set; } = new ObservableCollection<TablesElement>();
        public int numberOfTables { get; set; }
        public RelayCommand NavigateToPOSCommand { get; set; }
        public RelayCommand NavigateToOrdersCommand { get; set; }

        public TablesVM(NavigationService ns, KFPContext context, AppDataService appDataService)
        {
            _dbContext = context;
            _navigationService = ns;
            NavigateToPOSCommand = new RelayCommand(() => _navigationService.navigateTo(KioberFoodPage.POS));
            NavigateToOrdersCommand = new RelayCommand(() => _navigationService.navigateTo(KioberFoodPage.OrdersListPage));
            _appDataService = appDataService;
            numberOfTables = _appDataService.NumberOfTables;
            TableOrders = _dbContext.Orders
                .Where(o => (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready)
                        && o.orderLocation == OrderLocation.Table).ToList();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 1);

            for (int i = 1; i <= numberOfTables; i++)
            {
                var element = new TablesElement(i, _navigationService, appDataService.Currency, dispatcherTimer);
                Tables.Add(element);
                if (TableOrders.Count > 0)
                {
                    var order = TableOrders.FirstOrDefault(o => o.TableNumber == i);
                    if (order != null)
                    {
                        element.order = order;
                    }
                }
            }
            dispatcherTimer.Start();
        }
    }

    public partial class TablesElement : ObservableObject
    {
        public DispatcherTimer dispatcherTimer;

        private NavigationService _navigationService;
        public int TableNumber { get; set; }
        public Order? order { get; set; }
        public int? OrderId { get; set; }
        public Currency Currency { get; set; }
        public TimeSpan? OrderTimePassed
        {
            get
            {
                if (order == null)
                    return null;
                else if  (order.SetPreparingAt == null)
                    return TimeSpan.Zero;
                else
                    return DateTime.Now - order.SetPreparingAt.Value;
            }
        }
        public string OrderTimePassedString
        {
            get
            {
                if (OrderTimePassed < TimeSpan.FromDays(1))
                    return string.Format("{0:hh\\:mm\\:ss}", OrderTimePassed);
                else
                    return string.Format("{0:dd}d {1:hh\\:mm\\:ss}", OrderTimePassed, OrderTimePassed);
            }
        }
        public TablesElement(int tableNumber, NavigationService ns, Currency currency, DispatcherTimer timer, Order? order = null)
        {
            _navigationService = ns;
            TableNumber = tableNumber;
            this.order = order;
            OrderId = order?.Id;
            Currency = currency;
            dispatcherTimer = timer;
            dispatcherTimer.Tick += DispatcherTimer_Tick1;
        }
        private void DispatcherTimer_Tick1(object? sender, object e)
        {
            OnPropertyChanged(nameof(OrderTimePassedString));
            OnPropertyChanged(nameof(OrderTimePassed));
        }

        public string TableName
        {
            get
            {
                return StringLocalisationService.getStringWithKey("Table") + " " + TableNumber;
            }
        }

        [RelayCommand]
        public void NavigateToDisplayOrder()
        {
            if (order != null)
            {
                var parameters = new List<object> { order.Id };
                _navigationService.navigateTo(KioberFoodPage.DisplayOrderPage, parameters);
            }
            else
            {
                var parameters = new List<object> { this };
                _navigationService.navigateTo(KioberFoodPage.POS, parameters);
            }
        }
    }
}
