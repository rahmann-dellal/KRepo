using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;

namespace KFP.Services
{
    /// <summary>
    /// Provide string localisation
    /// </summary>
    public class StringLocalisationService
    {
        private static AppDataService appDataService;

        private static ResourceContext resourceContext;
        private static ResourceMap resourceMap;
        private static ResourceMap ErrorsResourceMap;
        private static ResourceMap ToolTipsResourceMap;
        static StringLocalisationService()
        {
            appDataService = Ioc.Default.GetService<AppDataService>();

            resourceContext = new ResourceContext();
            resourceContext.QualifierValues["Language"] = appDataService.AppLanguage;
            resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            ErrorsResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("ErrorMessages");
            ToolTipsResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("ToolTips");
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
                if (value == null && ErrorsResourceMap != null)
                {
                    value = ErrorsResourceMap.GetValue(key, resourceContext);
                }
                if (value == null && ToolTipsResourceMap != null)
                {
                    value = ToolTipsResourceMap.GetValue(key, resourceContext);
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
