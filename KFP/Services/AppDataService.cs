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

        public string? OrderPrinterName
        {
            get
            {
                if (Settings.Values["OrderPrinterName"] != null)
                    return (string)Settings.Values["OrderPrinterName"];
                else
                    return null;
            }
            set
            {
                Settings.Values["OrderPrinterName"] = value;
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
                if(Settings.Values["IsKitchenPrinterEnabled"] != null)
                    return (bool)Settings.Values["IsKitchenPrinterEnabled"];
                else
                    return false;
            }
            set
            {
                Settings.Values["IsKitchenPrinterEnabled"] = value;
            }
        }
        public bool IsOrderPrinterEnabled
        {
            get
            {
                if (Settings.Values["IsOrderPrinterEnabled"] != null)
                    return (bool)Settings.Values["IsOrderPrinterEnabled"];
                else
                    return false;
            }
            set
            {
                Settings.Values["IsOrderPrinterEnabled"] = value;
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
    }
}
