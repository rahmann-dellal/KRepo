using KFP.DATA;
using KFP.Services;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Converters
{
    class MenuItemTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(string))
            {
                MenuItemType? type = (MenuItemType?)value;
                if (type == MenuItemType.Main)
                { 
                    return StringLocalisationService.getStringWithKey("Main");
                }
                else if (type == MenuItemType.Addon)
                {
                    return StringLocalisationService.getStringWithKey("Addon");
                }
                else if (type == MenuItemType.Universal || type == null)
                {
                    return StringLocalisationService.getStringWithKey("Universal");
                }
            }
            throw new Exception();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
