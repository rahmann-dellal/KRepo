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
        //represents the ressource map of the Kiober POS library (Kiober POS/Resources)
        private static ResourceMap rootResourceMap;
        //represents the ressource map of project using the library (Resources)
        private static ResourceMap callerResourceMap;

        static StringLocalisationService()
        {
            appDataService = Ioc.Default.GetService<AppDataService>();

            resourceContext = new ResourceContext();
            resourceContext.QualifierValues["Language"] = appDataService.AppLanguage;
            callerResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            rootResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Kiober POS/Resources");

        }

        public static string getStringWithKey(string key)
        {
            ResourceCandidate value = null;
            try
            {
                if (callerResourceMap != null)
                {
                    value = callerResourceMap.GetValue(key, resourceContext);
                }
                if (value == null && rootResourceMap != null)
                {
                    value = rootResourceMap.GetValue(key, resourceContext);
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
