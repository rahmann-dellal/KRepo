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
        public Currency Currency {
            get
            {
                var value = (string) Settings.Values["Currency"];
                if (value != null && value != "")
                {
                    try 
                    { 
                        Currency currrency =  (Currency)Enum.Parse(typeof(Currency), value);
                        return currrency;
                    } catch
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
                Settings.Values["WindowPresenterKind"] = (int) value;
            }
        }

    }
}
