using CommunityToolkit.Mvvm.ComponentModel;
using KFP.DATA;
using Microsoft.UI.Windowing;
using System;
using System.ComponentModel;

namespace KFP.Services
{
    public class AppDataService
    {
        Windows.Storage.ApplicationDataContainer Settings =
        Windows.Storage.ApplicationData.Current.RoamingSettings;

        /// <section>
        /// General Settings
        /// </section>
        public Currency Currency
        {
            get
            {
                var value = (string)Settings.Values["Currency"];
                if (value != null && value != "")
                {
                    try
                    {
                        Currency currrency = (Currency)Enum.Parse(typeof(Currency), value);
                        return currrency;
                    }
                    catch
                    {
                        return Currency.USD;
                    }
                }
                else
                {
                    return Currency.USD;
                }
            }
            set
            {
                Settings.Values["Currency"] = value.ToString();
            }
        }

        public string AppLanguage
        {
            get
            {
                var value = (string)Settings.Values["Applanguage"];
                if (value != null && value != "")
                {
                    return value;
                }
                else
                {
                    return "en-US";
                }

            }
            set
            {
                Settings.Values["Applanguage"] = value;
            }
        }

        public AppWindowPresenterKind WindowPresenterKind
        {
            get
            {
                if (Settings.Values["WindowPresenterKind"] != null)
                    return (AppWindowPresenterKind)Settings.Values["WindowPresenterKind"];
                else
                    return AppWindowPresenterKind.Default;
            }
            set
            {
                Settings.Values["WindowPresenterKind"] = (int)value;
            }
        }

        /// <section>
        /// Establishment Settings
        /// </section>
        public int NumberOfTables
        {
            get
            {
                if (Settings.Values["NumberOfTables"] != null)
                    return (int)Settings.Values["NumberOfTables"];
                else
                    return 0;
            }
            set
            {
                Settings.Values["NumberOfTables"] = value;
            }
        }



        public string? EstablishmentName
        {
            get
            {
                if (Settings.Values["EstablishmentName"] != null)
                    return (string)Settings.Values["EstablishmentName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["EstablishmentName"] = value;
            }
        }

        public string? EstablishmentPhoneNumber1
        {
            get
            {
                if (Settings.Values["EstablishmentPhoneNumber"] != null)
                    return (string)Settings.Values["EstablishmentPhoneNumber"];
                else
                    return null;
            }
            set
            {
                Settings.Values["EstablishmentPhoneNumber"] = value;
            }
        }
        public string? EstablishmentPhoneNumber2
        {
            get
            {
                if (Settings.Values["EstablishmentPhoneNumber2"] != null)
                    return (string)Settings.Values["EstablishmentPhoneNumber2"];
                else
                    return null;
            }
            set
            {
                Settings.Values["EstablishmentPhoneNumber2"] = value;
            }
        }

        public string? EstablishmentAddress
        {
            get
            {
                if (Settings.Values["EstablishmentAddress"] != null)
                    return (string)Settings.Values["EstablishmentAddress"];
                else
                    return null;
            }
            set
            {
                Settings.Values["EstablishmentAddress"] = value;
            }
        }

        /// <section>
        /// Order Settings
        /// </section>
        public TimeSpan OrderLateDelay
        {
            get
            {
                if (Settings.Values["OrderLateDelay"] != null)
                    return TimeSpan.FromMinutes((int)Settings.Values["OrderLateDelay"]);
                else
                    return TimeSpan.FromMinutes(20);
            }
            set
            {
                Settings.Values["OrderLateDelay"] = (int)value.TotalMinutes;
            }
        }


        public TimeSpan OrderOverdueDelay
        {
            get
            {
                if (Settings.Values["OrderOverdueDelay"] != null)
                    return TimeSpan.FromMinutes((int)Settings.Values["OrderOverdueDelay"]);
                else
                    return TimeSpan.FromMinutes(30);
            }
            set
            {
                Settings.Values["OrderOverdueDelay"] = (int)value.TotalMinutes;
            }
        }

        /// <section>
        /// Printing Settings
        /// </section>
        public string? PreBillPrinterName
        {
            get
            {
                if (Settings.Values["PreBillPrinterName"] != null)
                    return (string)Settings.Values["PreBillPrinterName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["PreBillPrinterName"] = value;
            }
        }
        public string? InvoicePrinterName
        {
            get
            {
                if (Settings.Values["InvoicePrinterName"] != null)
                    return (string)Settings.Values["InvoicePrinterName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["InvoicePrinterName"] = value;
            }
        }

        public string? KitchenPrinterName
        {
            get
            {
                if (Settings.Values["KitchenPrinterName"] != null)
                    return (string)Settings.Values["KitchenPrinterName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["KitchenPrinterName"] = value;
            }
        }

        public bool IsKitchenPrinterEnabled
        {
            get
            {
                if (Settings.Values["IsKitchenPrinterEnabled"] != null)
                    return (bool)Settings.Values["IsKitchenPrinterEnabled"];
                else
                    return false;
            }
            set
            {
                Settings.Values["IsKitchenPrinterEnabled"] = value;
            }
        }
        public bool IsPreBillPrinterEnabled
        {
            get
            {
                if (Settings.Values["IsPreBillPrinterEnabled"] != null)
                    return (bool)Settings.Values["IsPreBillPrinterEnabled"];
                else
                    return false;
            }
            set
            {
                Settings.Values["IsPreBillPrinterEnabled"] = value;
            }
        }
        public bool IsInvoicePrinterEnabled
        {
            get
            {
                if (Settings.Values["IsInvoicePrinterEnabled"] != null)
                    return (bool)Settings.Values["IsInvoicePrinterEnabled"];
                else
                    return false;
            }
            set
            {
                Settings.Values["IsInvoicePrinterEnabled"] = value;
            }
        }

        public bool PrintEstablishmentNameWithInvoice
        {
            get
            {
                if (Settings.Values["PrintEstablishmentNameWithInvoice"] != null)
                    return (bool)Settings.Values["PrintEstablishmentNameWithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentNameWithInvoice"] = value;
            }
        }

        public bool PrintEstablishmentAddressWithInvoice
        {
            get
            {
                if (Settings.Values["PrintEstablishmentAddressWithInvoice"] != null)
                    return (bool)Settings.Values["PrintEstablishmentAddressWithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentAddressWithInvoice"] = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber1WithInvoice
        {
            get
            {
                if (Settings.Values["PrintEstablishmentPhoneNumber1WithInvoice"] != null)
                    return (bool)Settings.Values["PrintEstablishmentPhoneNumber1WithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentPhoneNumber1WithInvoice"] = value;
            }
        }
        public bool PrintEstablishmentPhoneNumber2WithInvoice
        {
            get
            {
                if (Settings.Values["PrintEstablishmentPhoneNumber2WithInvoice"] != null)
                    return (bool)Settings.Values["PrintEstablishmentPhoneNumber2WithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentPhoneNumber2WithInvoice"] = value;
            }
        }

        public bool PrintCashierNameWithInvoice
        {
            get
            {
                if (Settings.Values["PrintCashierNameWithInvoice"] != null)
                    return (bool)Settings.Values["PrintCashierNameWithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintCashierNameWithInvoice"] = value;
            }
        }
        public bool PrintEstablishmentNameWithPreBill
        {
            get
            {
                if (Settings.Values["PrintEstablishmentNameWithPreBill"] != null)
                    return (bool)Settings.Values["PrintEstablishmentNameWithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentNameWithPreBill"] = value;
            }
        }

        public bool PrintEstablishmentAddressWithPreBill
        {
            get
            {
                if (Settings.Values["PrintEstablishmentAddressWithPreBill"] != null)
                    return (bool)Settings.Values["PrintEstablishmentAddressWithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentAddressWithPreBill"] = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber1WithPreBill
        {
            get
            {
                if (Settings.Values["PrintEstablishmentPhoneNumber1WithPreBill"] != null)
                    return (bool)Settings.Values["PrintEstablishmentPhoneNumber1WithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentPhoneNumber1WithPreBill"] = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber2WithPreBill
        {
            get
            {
                if (Settings.Values["PrintEstablishmentPhoneNumber2WithPreBill"] != null)
                    return (bool)Settings.Values["PrintEstablishmentPhoneNumber2WithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentPhoneNumber2WithPreBill"] = value;
            }
        }
        public bool PrintCashierNameWithPreBill
        {
            get
            {
                if (Settings.Values["PrintCashierNameWithPreBill"] != null)
                    return (bool)Settings.Values["PrintCashierNameWithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintCashierNameWithPreBill"] = value;
            }
        }
    }
}
