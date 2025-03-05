using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources.Core;

namespace KFP.Services
{
    /// <summary>
    /// Provide string localisation
    /// </summary>
    public static class StringLocalisationService
    {
        private static AppDataService appDataService;

        private static ResourceContext resourceContext;
        private static ResourceMap resourceMap;

        static StringLocalisationService()
        {
            appDataService = Ioc.Default.GetService<AppDataService>();

            resourceContext = new ResourceContext();
            resourceContext.QualifierValues["Language"] = appDataService.AppLanguage;
            resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        }

        public static string getStringWithKey(string key)
        {
            ResourceCandidate value = null;
            try
            {
                if (resourceMap != null)
                {
                    value = resourceMap.GetValue(key, resourceContext);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error retrieving resource for key: " + key + ". Exception: " + ex.Message);
                return "";
            }

            if (value == null)
            {
                Debug.WriteLine("Resource not found for key: " + key);
                return "";
            }

            try
            {
                return value.ValueAsString;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error retrieving resource value for key: " + key + ". Exception: " + ex.Message);
                return "";
            }
        }
    }
}
