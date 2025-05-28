using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace KFP.Converters
{
    public class TimePassedToBruchConverter : IValueConverter
    {
        private AppDataService _appDataService;
        private TimeSpan lateDelay;
        private TimeSpan OverdueDelay;
        public TimePassedToBruchConverter()
        {
            _appDataService = Ioc.Default.GetService<AppDataService>()!;
            lateDelay = _appDataService.OrderLateDelay;
            OverdueDelay = _appDataService.OrderOverdueDelay;
        }
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value is not TimeSpan) {
                return null;
            }
            TimeSpan timePassed = (TimeSpan)value;
            if(targetType == typeof(Brush))
            {
                if (timePassed < lateDelay)
                {
                    return new SolidColorBrush(Colors.Black);
                }
                else if (timePassed < OverdueDelay)
                {
                    return new SolidColorBrush(Color.FromArgb(0xFF,0xCC,0x77,0));
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
            else if (targetType == typeof(Thickness))
            {
                if (timePassed < lateDelay)
                {
                    return new Thickness(0);
                }
                else
                {
                    return new Thickness(1);
                }
            } else { 
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
