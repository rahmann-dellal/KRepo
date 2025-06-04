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
        /// Establishment Settings
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
                    _appState.EstablishmentHasTables = true;
                }
                else
                {
                    _appDataService.NumberOfTables = (int)value;
                    _appState.EstablishmentHasTables = (int)value > 0;
                }
            }
        }

        public string? EstablishmentName
        {
            get
            {
                return _appDataService.EstablishmentName;
            }
            set
            {
                _appDataService.EstablishmentName = value;
            }
        }
        public string? EstablishmentPhoneNumber1
        {
            get
            {
                return _appDataService.EstablishmentPhoneNumber1;
            }
            set
            {
                _appDataService.EstablishmentPhoneNumber1 = value;
            }
        }

        public string? EstablishmentPhoneNumber2
        {
            get
            {
                return _appDataService.EstablishmentPhoneNumber2;
            }
            set
            {
                _appDataService.EstablishmentPhoneNumber2 = value;
            }
        }
        public string? EstablishmentAddress
        {
            get
            {
                return _appDataService.EstablishmentAddress;
            }
            set
            {
                _appDataService.EstablishmentAddress = value;
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

        public bool PrintEstablishmentNameWithInvoice
        {
            get
            {
                return _appDataService.PrintEstablishmentNameWithInvoice;
            }
            set
            {
                _appDataService.PrintEstablishmentNameWithInvoice = value;
            }
        }

        public bool PrintEstablishmentAddressWithInvoice
        {
            get
            {
                return _appDataService.PrintEstablishmentAddressWithInvoice;
            }
            set
            {
                _appDataService.PrintEstablishmentAddressWithInvoice = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber1WithInvoice
        {
            get
            {
                return _appDataService.PrintEstablishmentPhoneNumber1WithInvoice;
            }
            set
            {
                _appDataService.PrintEstablishmentPhoneNumber1WithInvoice = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber2WithInvoice
        {
            get
            {
                return _appDataService.PrintEstablishmentPhoneNumber2WithInvoice;
            }
            set
            {
                _appDataService.PrintEstablishmentPhoneNumber2WithInvoice = value;
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

        public bool PrintEstablishmentNameWithPreBill
        {
            get
            {
                return _appDataService.PrintEstablishmentNameWithPreBill;
            }
            set
            {
                _appDataService.PrintEstablishmentNameWithPreBill = value;
            }
        }
        public bool PrintEstablishmentAddressWithPreBill
        {
            get
            {
                return _appDataService.PrintEstablishmentAddressWithPreBill;
            }
            set
            {
                _appDataService.PrintEstablishmentAddressWithPreBill = value;
            }
        }
        public bool PrintEstablishmentPhoneNumber1WithPreBill
        {
            get
            {
                return _appDataService.PrintEstablishmentPhoneNumber1WithPreBill;
            }
            set
            {
                _appDataService.PrintEstablishmentPhoneNumber1WithPreBill = value;
            }
        }
        public bool PrintEstablishmentPhoneNumber2WithPreBill
        {
            get
            {
                return _appDataService.PrintEstablishmentPhoneNumber2WithPreBill;
            }
            set
            {
                _appDataService.PrintEstablishmentPhoneNumber2WithPreBill = value;
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
