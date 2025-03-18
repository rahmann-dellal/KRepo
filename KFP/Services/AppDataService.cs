using CommunityToolkit.Mvvm.ComponentModel;
using KFP.DATA;
using Microsoft.UI.Windowing;
using System;
using System.ComponentModel;

namespace KFP.Services
{
    public class AppDataService 
    {
        Windows.Storage.ApplicationDataContainer localSettings = 
        Windows.Storage.ApplicationData.Current.LocalSettings;
        public Currency Currency {
            get
            {
                var value = (string) localSettings.Values["Currency"];
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
                localSettings.Values["Currency"] = value.ToString();
            }
        }

        public string AppLanguage
        {
            get
            {
                var value = (string)localSettings.Values["Applanguage"];
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
                localSettings.Values["Applanguage"] = value;
            }
        }
        
        public AppWindowPresenterKind WindowPresenterKind
        {
            get
            {
                if (localSettings.Values["WindowPresenterKind"] != null)
                    return (AppWindowPresenterKind)localSettings.Values["WindowPresenterKind"];
                else
                    return AppWindowPresenterKind.Default;
            }
            set
            {
                localSettings.Values["WindowPresenterKind"] = (int) value;
            }
        }

    }
}
