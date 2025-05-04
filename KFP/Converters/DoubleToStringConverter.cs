using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace KFP.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                string format = parameter as string ?? "F2";
                return doubleValue.ToString(format);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string strValue && double.TryParse(strValue, out double result))
            {
                return result;
            }
            return 0.0;
        }
    }
}
