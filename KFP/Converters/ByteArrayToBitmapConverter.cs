using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Helpers;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace KFP.Converters
{
    class ByteArrayToBitmapConverter : IValueConverter
    {
        ImageConverter imageConverter;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte[]? byteArray = (byte[]?)value;
            if (byteArray == null)
            {
                return null;
            }
            else
            {
                return ConvertToBitmapImageAsync(byteArray).GetAwaiter().GetResult();
            }
        }

        private async Task<BitmapImage> ConvertToBitmapImageAsync(byte[] byteArray)
        {
            imageConverter = Ioc.Default.GetService<ImageConverter>();
            return await imageConverter.ConvertToBitmapImage(byteArray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
