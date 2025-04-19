using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Converters
{
    internal class avatarConnectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool connected = (bool)value;
            if (targetType == typeof(Thickness))
            {
                return connected ? new Thickness(4) : new Thickness(0);
            }
            if (targetType == typeof(Brush))
            {
                return connected ? new SolidColorBrush(Colors.LawnGreen) : null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
