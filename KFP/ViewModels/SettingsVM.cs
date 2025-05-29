using KFP.DATA;
using KFP.Services;
using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.ViewModels
{
    public class SettingsVM : KioberViewModelBase
    {
        private AppDataService _appDataService;
        private AppState _appState;
        private IPrintingService _printingService;
        public List<Currency> Currencies { get; set; }
        public List<string> Printers { get; private set; }
        public SettingsVM(AppDataService appDataService, AppState appState, IPrintingService printingService)
        {
            _appDataService = appDataService;
            _appState = appState;
            Currencies = Enum.GetValues(typeof(Currency)).Cast<Currency>().ToList(); ;
            _printingService = printingService;
            Printers = _printingService.GetAvailablePrinters();
        }

        //General Settings
        public Currency Currency
        {
            get
            {
                return _appDataService.Currency;
            }
            set
            {
                _appDataService.Currency = value;
            }
        }

        public string language
        {
            get
            {
                return _appDataService.AppLanguage;
            }
            set
            {
                _appDataService.AppLanguage = value;
            }
        }

        public AppWindowPresenterKind WindowPresenterKind
        {
            get
            {
                return _appDataService.WindowPresenterKind;
            }
            set
            {
                _appDataService.WindowPresenterKind = value;
                _appState.WindowPresenterKind = value;
            }
        }

        //Diner
        public double? NumberOfTables
        {
            get
            {
                return _appDataService.NumberOfTables;
            }
            set
            {
                if (value == null)
                {
                    _appDataService.NumberOfTables = 25;
                    _appState.DinerHasTables = true;
                }
                else
                {
                    _appDataService.NumberOfTables = (int)value;
                    _appState.DinerHasTables = (int)value > 0;
                }
            }
        }

        public string? RestaurantName
        {
            get
            {
                return _appDataService.RestaurantName;
            }
            set
            {
                _appDataService.RestaurantName = value;
            }
        }
        public string? RestaurantPhoneNumber1
        {
            get
            {
                return _appDataService.RestaurantPhoneNumber1;
            }
            set
            {
                _appDataService.RestaurantPhoneNumber1 = value;
            }
        }

        public string? RestaurantPhoneNumber2
        {
            get
            {
                return _appDataService.RestaurantPhoneNumber2;
            }
            set
            {
                _appDataService.RestaurantPhoneNumber2 = value;
            }
        }
        public string? RestaurantAddress
        {
            get
            {
                return _appDataService.RestaurantAddress;
            }
            set
            {
                _appDataService.RestaurantAddress = value;
            }
        }

        //Orders
        public double? OrderLateDelay
        {
            get
            {
                return _appDataService.OrderLateDelay.Minutes;
            }
            set
            {
                _appDataService.OrderLateDelay = TimeSpan.FromMinutes(value ?? 0);
            }
        }

        public double? OrderOverDueDelay
        {
            get
            {
                return _appDataService.OrderOverdueDelay.Minutes;
            }
            set
            {
                _appDataService.OrderOverdueDelay = TimeSpan.FromMinutes(value ?? 0);
            }
        }

        //Printing

        public string? KitchenPrinterName
        {
            get
            {
                return _appDataService.KitchenPrinterName;
            }
            set
            {
                _appDataService.KitchenPrinterName = value;
            }
        }
        public string? OrderPrinterName
        {
            get
            {
                return _appDataService.OrderPrinterName;
            }
            set
            {
                _appDataService.OrderPrinterName = value;
            }
        }
        public string? InvoicePrinterName
        {
            get
            {
                return _appDataService.InvoicePrinterName;
            }
            set
            {
                _appDataService.InvoicePrinterName = value;
            }
        }
        public bool IsKitchenPrinterEnabled
        {
            get
            {
                return _appDataService.IsKitchenPrinterEnabled;
            }
            set
            {
                _appDataService.IsKitchenPrinterEnabled = value;
                OnPropertyChanged(nameof(IsKitchenPrinterEnabled));
            }
        }
        public bool IsOrderPrinterEnabled
        {
            get
            {
                return _appDataService.IsOrderPrinterEnabled;
            }
            set
            {
                _appDataService.IsOrderPrinterEnabled = value;
                OnPropertyChanged(nameof(IsOrderPrinterEnabled));
            }
        }
        public bool IsInvoicePrinterEnabled
        {
            get
            {
                return _appDataService.IsInvoicePrinterEnabled;
            }
            set
            {
                _appDataService.IsInvoicePrinterEnabled = value;
                OnPropertyChanged(nameof(IsInvoicePrinterEnabled));
            }
        }
    }
}
