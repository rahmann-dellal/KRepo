using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Windowing;
using System;
using System.ComponentModel;

namespace KFP.Services
{
    public class AppDataService 
    {
        Windows.Storage.ApplicationDataContainer localSettings = 
        Windows.Storage.ApplicationData.Current.LocalSettings;

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
