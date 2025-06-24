using CommunityToolkit.Common;
using KFP.DATA;
using KFP.Services;
using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KFP.ViewModels
{
    public class SettingsVM : KioberViewModelBase
    {

        private AppDataService _appDataService;
        private AppState _appState;
        private IPrintingService _printingService;
        public List<Currency> Currencies { get; set; }
        public List<Language> Languages { get; set; }
        public List<string> Printers { get; private set; }
        public SettingsVM(AppDataService appDataService, AppState appState, IPrintingService printingService)
        {
            _appDataService = appDataService;
            _appState = appState;
            Currencies = Enum.GetValues(typeof(Currency)).Cast<Currency>().ToList(); ;
            _printingService = printingService;
            Printers = _printingService.GetAvailablePrinters();
            Languages = new List<Language>
            {
                new Language("en"),
                new Language("fr")
            };
            var code = _appDataService.AppLanguage;
            _language = Languages.FirstOrDefault(l => l.Code == code) ?? Languages[0];
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

        private Language _language;
        public Language language
        {
            get
            {
                return _language;
            }
            set
            {
                if (_language?.Code == value?.Code)
                    return;

                _language = value;
                _appDataService.AppLanguage = value.Code;
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
        public string? ReceiptPrinterName
        {
            get
            {
                return _appDataService.ReceiptPrinterName;
            }
            set
            {
                _appDataService.ReceiptPrinterName = value;
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
        public bool IsReceiptPrinterEnabled
        {
            get
            {
                return _appDataService.IsReceiptPrinterEnabled;
            }
            set
            {
                _appDataService.IsReceiptPrinterEnabled = value;
                OnPropertyChanged(nameof(IsReceiptPrinterEnabled));
            }
        }

        public bool PrintEstablishmentNameWithReceipt
        {
            get
            {
                return _appDataService.PrintEstablishmentNameWithReceipt;
            }
            set
            {
                _appDataService.PrintEstablishmentNameWithReceipt = value;
            }
        }

        public bool PrintEstablishmentAddressWithReceipt
        {
            get
            {
                return _appDataService.PrintEstablishmentAddressWithReceipt;
            }
            set
            {
                _appDataService.PrintEstablishmentAddressWithReceipt = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber1WithReceipt
        {
            get
            {
                return _appDataService.PrintEstablishmentPhoneNumber1WithReceipt;
            }
            set
            {
                _appDataService.PrintEstablishmentPhoneNumber1WithReceipt = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber2WithReceipt
        {
            get
            {
                return _appDataService.PrintEstablishmentPhoneNumber2WithReceipt;
            }
            set
            {
                _appDataService.PrintEstablishmentPhoneNumber2WithReceipt = value;
            }
        }

        public bool PrintCashierNameWithReceipt
        {
            get
            {
                return _appDataService.PrintCashierNameWithReceipt;
            }
            set
            {
                _appDataService.PrintCashierNameWithReceipt = value;
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

    public class Language
    {
        public Language() { }
        public Language(string code)
        {
            Code = code;
        }
        public string Code { get; set; } = "en";
        public string Name
        {
            get
            {
                return Code switch
                {
                    "en" => "English",
                    "fr" => "Français",
                    _ => "Unknown"
                };
            }
        }
    }
}
