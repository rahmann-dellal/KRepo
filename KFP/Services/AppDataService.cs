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

        public string RestaurantName
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
    }
}
