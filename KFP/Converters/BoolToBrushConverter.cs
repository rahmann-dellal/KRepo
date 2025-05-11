using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using System.Globalization;

namespace KFP.Converters
{
    internal class BoolToBrushConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            bool BoolValue = KioberConverter.ToBool(value);
            if (targetType == typeof(Thickness))
            {
                int thickness = 0;
                if (parameter != null && parameter is string parameterString) // Fix CS1061
                {
                    thickness = int.TryParse(parameterString, out int parsedThickness) ? parsedThickness : 0;
                    return BoolValue ? new Thickness(thickness) : new Thickness(0);
                }
                return BoolValue ? new Thickness(4) : new Thickness(0);
            }
            if (targetType == typeof(Brush))
            {
                Color? color = null;
                if (parameter != null && parameter is string parameterString) // Fix CS1061
                {
                    try
                    {
                        color = ColorHelper.FromArgb(
                            byte.Parse(parameterString.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture), // Fix CS0103
                            byte.Parse(parameterString.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture), // Fix CS0103
                            byte.Parse(parameterString.Substring(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture), // Fix CS0103
                            byte.Parse(parameterString.Substring(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)  // Fix CS0103
                        );
                    }
                    catch
                    {
                        color = null;
                    }
                }
                return BoolValue ? new SolidColorBrush(color ?? Colors.LawnGreen) : new SolidColorBrush(Colors.Black);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
