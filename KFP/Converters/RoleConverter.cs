using KFP.DATA;
using KFP.Services;
using Microsoft.UI.Xaml.Data;
using System;

namespace KFP.Converters
{
    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(string))
            {
                UserRole role = (UserRole)value;
                if (role == UserRole.Admin)
                {
                    return StringLocalisationService.getStringWithKey("Admin");
                }
                else if (role == UserRole.Manager)
                {
                    return StringLocalisationService.getStringWithKey("Manager");
                }
                else if (role == UserRole.Cashier)
                {
                    return StringLocalisationService.getStringWithKey("Cashier");
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
