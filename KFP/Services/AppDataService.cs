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
        /// Diner Settings
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



        public string? RestaurantName
        {
            get
            {
                if (Settings.Values["RestaurantName"] != null)
                    return (string)Settings.Values["RestaurantName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["RestaurantName"] = value;
            }
        }

        public string? RestaurantPhoneNumber1
        {
            get
            {
                if (Settings.Values["RestaurantPhoneNumber"] != null)
                    return (string)Settings.Values["RestaurantPhoneNumber"];
                else
                    return null;
            }
            set
            {
                Settings.Values["RestaurantPhoneNumber"] = value;
            }
        }
        public string? RestaurantPhoneNumber2
        {
            get
            {
                if (Settings.Values["RestaurantPhoneNumber2"] != null)
                    return (string)Settings.Values["RestaurantPhoneNumber2"];
                else
                    return null;
            }
            set
            {
                Settings.Values["RestaurantPhoneNumber2"] = value;
            }
        }

        public string? RestaurantAddress
        {
            get
            {
                if (Settings.Values["RestaurantAddress"] != null)
                    return (string)Settings.Values["RestaurantAddress"];
                else
                    return null;
            }
            set
            {
                Settings.Values["RestaurantAddress"] = value;
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

        public bool PrintDinerNameWithInvoice
        {
            get
            {
                if (Settings.Values["PrintDinerNameWithInvoice"] != null)
                    return (bool)Settings.Values["PrintDinerNameWithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerNameWithInvoice"] = value;
            }
        }

        public bool PrintDinerAddressWithInvoice
        {
            get
            {
                if (Settings.Values["PrintDinerAddressWithInvoice"] != null)
                    return (bool)Settings.Values["PrintDinerAddressWithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerAddressWithInvoice"] = value;
            }
        }

        public bool PrintDinerPhoneNumber1WithInvoice
        {
            get
            {
                if (Settings.Values["PrintDinerPhoneNumber1WithInvoice"] != null)
                    return (bool)Settings.Values["PrintDinerPhoneNumber1WithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerPhoneNumber1WithInvoice"] = value;
            }
        }
        public bool PrintDinerPhoneNumber2WithInvoice
        {
            get
            {
                if (Settings.Values["PrintDinerPhoneNumber2WithInvoice"] != null)
                    return (bool)Settings.Values["PrintDinerPhoneNumber2WithInvoice"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerPhoneNumber2WithInvoice"] = value;
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
        public bool PrintDinerNameWithPreBill
        {
            get
            {
                if (Settings.Values["PrintDinerNameWithPreBill"] != null)
                    return (bool)Settings.Values["PrintDinerNameWithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerNameWithPreBill"] = value;
            }
        }

        public bool PrintDinerAddressWithPreBill
        {
            get
            {
                if (Settings.Values["PrintDinerAddressWithPreBill"] != null)
                    return (bool)Settings.Values["PrintDinerAddressWithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerAddressWithPreBill"] = value;
            }
        }

        public bool PrintDinerPhoneNumber1WithPreBill
        {
            get
            {
                if (Settings.Values["PrintDinerPhoneNumber1WithPreBill"] != null)
                    return (bool)Settings.Values["PrintDinerPhoneNumber1WithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerPhoneNumber1WithPreBill"] = value;
            }
        }

        public bool PrintDinerPhoneNumber2WithPreBill
        {
            get
            {
                if (Settings.Values["PrintDinerPhoneNumber2WithPreBill"] != null)
                    return (bool)Settings.Values["PrintDinerPhoneNumber2WithPreBill"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintDinerPhoneNumber2WithPreBill"] = value;
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
