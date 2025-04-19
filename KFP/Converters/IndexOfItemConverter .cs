using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace KFP.Converters
{
    public class IndexOfItemConverter : IValueConverter
    {
        public IList SourceList { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (SourceList == null || value == null)
                return -1;

            return SourceList.IndexOf(value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
