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
        public BitmapImage LoadBitmapImage(string uri)
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

        public async Task<BitmapImage> ConvertToBitmapImage(byte[]? imageData)
        {
            
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            try
            {
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(imageData.AsBuffer());
                    stream.Seek(0);

                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting image: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> ResizeImageIfNeeded(byte[]? imageData, bool thumbnail = false)
        {
            double resultHeight = 300.0;
            double resultWidth = 300.0;
            if (thumbnail)
            {
                resultHeight = 100.0; resultWidth = 100.0;
            }

            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            try
            {
                using (InMemoryRandomAccessStream inputStream = new InMemoryRandomAccessStream())
                {
                    await inputStream.WriteAsync(imageData.AsBuffer());
                    inputStream.Seek(0);

                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(inputStream);
                    uint originalWidth = decoder.PixelWidth;
                    uint originalHeight = decoder.PixelHeight;

                    if (originalWidth <= resultWidth && originalHeight <= resultHeight)
                    {
                        return imageData; // No resizing needed
                    }

                    double scale = Math.Min(resultWidth / originalWidth, resultHeight / originalHeight);
                    uint newWidth = (uint)(originalWidth * scale);
                    uint newHeight = (uint)(originalHeight * scale);

                    using (InMemoryRandomAccessStream outputStream = new InMemoryRandomAccessStream())
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);
                        encoder.BitmapTransform.ScaledWidth = newWidth;
                        encoder.BitmapTransform.ScaledHeight = newHeight;
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                        await encoder.FlushAsync();

                        byte[] resizedImageData = new byte[outputStream.Size];
                        await outputStream.ReadAsync(resizedImageData.AsBuffer(), (uint)outputStream.Size, InputStreamOptions.None);
                        return resizedImageData;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resizing image: {ex.Message}");
                return null;
            }
        }
        public async Task<string?> SaveImageToFile(byte[]? imageData, string filePath = null)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            try
            {
                if(filePath == null)
                {
                    string fileName = Path.GetRandomFileName();
                    string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    filePath = Path.Combine(appDataFolder, fileName);
                }
                
                await File.WriteAllBytesAsync(filePath, imageData);
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image to file: {ex.Message}");
                return null;
            }
        }

        public string? GetThumbUriFromPictureUri(string? pictureUri)
        {
            if (pictureUri == null)
            {
                return null;
            }
            string fileName = Path.GetFileName(pictureUri);
            string? PictureFolder = Path.GetDirectoryName(pictureUri);
            if (PictureFolder == null) {
                return null;
            }
            string thumbFileName = "THUMB_" + fileName;
            string thumbUri = Path.Combine(PictureFolder, thumbFileName);
            return thumbUri;
        }
    }
}
