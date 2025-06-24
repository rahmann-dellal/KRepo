using CommunityToolkit.Mvvm.ComponentModel;
using KFP.DATA;
using Microsoft.UI.Windowing;
using System;
using System.ComponentModel;
using Windows.System.UserProfile;

namespace KFP.Services
{
    public class AppDataService
    {
        Windows.Storage.ApplicationDataContainer Settings =
        Windows.Storage.ApplicationData.Current.RoamingSettings;

        public bool DefaultUserLogin
        {
            get
            {
                if (Settings.Values["DefaultUserLogin"] != null)
                    return (bool)Settings.Values["DefaultUserLogin"];
                else
                    return false;
            }
            set
            {
                Settings.Values["DefaultUserLogin"] = value;
            }
        }

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
                    return GlobalizationPreferences.Languages[0].Substring(0, 2);
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
        public string? ReceiptPrinterName
        {
            get
            {
                if (Settings.Values["ReceiptPrinterName"] != null)
                    return (string)Settings.Values["ReceiptPrinterName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["ReceiptPrinterName"] = value;
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
        public bool IsReceiptPrinterEnabled
        {
            get
            {
                if (Settings.Values["IsReceiptPrinterEnabled"] != null)
                    return (bool)Settings.Values["IsReceiptPrinterEnabled"];
                else
                    return false;
            }
            set
            {
                Settings.Values["IsReceiptPrinterEnabled"] = value;
            }
        }

        public bool PrintEstablishmentNameWithReceipt
        {
            get
            {
                if (Settings.Values["PrintEstablishmentNameWithReceipt"] != null)
                    return (bool)Settings.Values["PrintEstablishmentNameWithReceipt"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentNameWithReceipt"] = value;
            }
        }

        public bool PrintEstablishmentAddressWithReceipt
        {
            get
            {
                if (Settings.Values["PrintEstablishmentAddressWithReceipt"] != null)
                    return (bool)Settings.Values["PrintEstablishmentAddressWithReceipt"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentAddressWithReceipt"] = value;
            }
        }

        public bool PrintEstablishmentPhoneNumber1WithReceipt
        {
            get
            {
                if (Settings.Values["PrintEstablishmentPhoneNumber1WithReceipt"] != null)
                    return (bool)Settings.Values["PrintEstablishmentPhoneNumber1WithReceipt"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentPhoneNumber1WithReceipt"] = value;
            }
        }
        public bool PrintEstablishmentPhoneNumber2WithReceipt
        {
            get
            {
                if (Settings.Values["PrintEstablishmentPhoneNumber2WithReceipt"] != null)
                    return (bool)Settings.Values["PrintEstablishmentPhoneNumber2WithReceipt"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintEstablishmentPhoneNumber2WithReceipt"] = value;
            }
        }

        public bool PrintCashierNameWithReceipt
        {
            get
            {
                if (Settings.Values["PrintCashierNameWithReceipt"] != null)
                    return (bool)Settings.Values["PrintCashierNameWithReceipt"];
                else
                    return true;
            }
            set
            {
                Settings.Values["PrintCashierNameWithReceipt"] = value;
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
