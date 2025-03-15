using System;
using Microsoft.UI.Xaml.Media.Imaging;

namespace KFP.Helpers
{
    public class ImageConverter
    {
        public static BitmapImage LoadBitmapImage(string uri)
        {
            try
            {
                // Create a BitmapImage and set the source
                //BitmapImage bitmapImage = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
                BitmapImage bitmapImage = new BitmapImage(new Uri(uri));
                return bitmapImage;
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., file not found)
                Console.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }
    }
}
