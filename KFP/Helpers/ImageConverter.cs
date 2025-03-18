using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KFP.Helpers
{
    public class ImageConverter
    {
        public static BitmapImage LoadBitmapImage(string uri)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(uri));
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

        // Convert byte[] to BitmapImage
       
    }
}
