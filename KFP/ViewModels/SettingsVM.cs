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

        /// <section>
        /// General Settings
        /// </section>
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

        /// <section>
        /// Diner Settings
        /// </section>
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

        public string? DinerName
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
        public string? DinerPhoneNumber1
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

        public string? DinerPhoneNumber2
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
        public string? DinerAddress
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

        /// <section>
        /// Order Settings
        /// </section>
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

        public double? OrderOverdueDelay
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

        /// <section>
        /// Printing Settings
        /// </section>
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
        public string? PreBillPrinterName
        {
            get
            {
                return _appDataService.PreBillPrinterName;
            }
            set
            {
                _appDataService.PreBillPrinterName = value;
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
        public bool IsPreBillPrinterEnabled
        {
            get
            {
                return _appDataService.IsPreBillPrinterEnabled;
            }
            set
            {
                _appDataService.IsPreBillPrinterEnabled = value;
                OnPropertyChanged(nameof(IsPreBillPrinterEnabled));
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

        public bool PrintDinerNameWithInvoice
        {
            get
            {
                return _appDataService.PrintDinerNameWithInvoice;
            }
            set
            {
                _appDataService.PrintDinerNameWithInvoice = value;
            }
        }

        public bool PrintDinerAddressWithInvoice
        {
            get
            {
                return _appDataService.PrintDinerAddressWithInvoice;
            }
            set
            {
                _appDataService.PrintDinerAddressWithInvoice = value;
            }
        }

        public bool PrintDinerPhoneNumber1WithInvoice
        {
            get
            {
                return _appDataService.PrintDinerPhoneNumber1WithInvoice;
            }
            set
            {
                _appDataService.PrintDinerPhoneNumber1WithInvoice = value;
            }
        }

        public bool PrintDinerPhoneNumber2WithInvoice
        {
            get
            {
                return _appDataService.PrintDinerPhoneNumber2WithInvoice;
            }
            set
            {
                _appDataService.PrintDinerPhoneNumber2WithInvoice = value;
            }
        }

        public bool PrintCashierNameWithInvoice
        {
            get
            {
                return _appDataService.PrintCashierNameWithInvoice;
            }
            set
            {
                _appDataService.PrintCashierNameWithInvoice = value;
            }
        }

        public bool PrintDinerNameWithPreBill
        {
            get
            {
                return _appDataService.PrintDinerNameWithPreBill;
            }
            set
            {
                _appDataService.PrintDinerNameWithPreBill = value;
            }
        }
        public bool PrintDinerAddressWithPreBill
        {
            get
            {
                return _appDataService.PrintDinerAddressWithPreBill;
            }
            set
            {
                _appDataService.PrintDinerAddressWithPreBill = value;
            }
        }
        public bool PrintDinerPhoneNumber1WithPreBill
        {
            get
            {
                return _appDataService.PrintDinerPhoneNumber1WithPreBill;
            }
            set
            {
                _appDataService.PrintDinerPhoneNumber1WithPreBill = value;
            }
        }
        public bool PrintDinerPhoneNumber2WithPreBill
        {
            get
            {
                return _appDataService.PrintDinerPhoneNumber2WithPreBill;
            }
            set
            {
                _appDataService.PrintDinerPhoneNumber2WithPreBill = value;
            }
        }
        public bool PrintCashierNameWithPreBill
        {
            get
            {
                return _appDataService.PrintCashierNameWithPreBill;
            }
            set
            {
                _appDataService.PrintCashierNameWithPreBill = value;
            }
        }
    }
}
