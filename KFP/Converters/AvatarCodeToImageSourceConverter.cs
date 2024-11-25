using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Converters
{
    public class AvatarCodeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var avatarCode = (int)value;
            if (avatarCode >= 1 && avatarCode <= 10)
            {
                return new BitmapImage(new Uri("ms-appx:///Kiober POS/Assets/Images/Toons/toon_" + avatarCode + ".png"));
            }
            else
            {
                return new BitmapImage(new Uri("ms-appx:///Kiober POS/Assets/Images/Toons/boringAvatar.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
