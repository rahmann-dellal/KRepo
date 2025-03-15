using KFP.Helpers;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI;
using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.Media.Capture;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.Components
{
    public sealed partial class ImageGraber : UserControl
    {
        ImageBrush? DefaultBackGround = null;
        public BitmapImage MenuItemBitmapImage { get; private set; }

        public ImageGraber()
        {
            this.InitializeComponent();
            DefaultBackGround = new ImageBrush() { ImageSource = ImageConverter.LoadBitmapImage("ms-appx:///Assets/Images/Food.png") };
            ImageGrid.Background = DefaultBackGround;
        }

        public async Task OpenFileDialogAndStoreImageAsync()
        {
            var picker = new FileOpenPicker();

            // Make the picker work in WinUI 3
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            // Filter for image files
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
                return; // No file selected

            // Load into BitmapImage
            var bitmapImage = new BitmapImage();
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(fileStream);
            }

            // Store the selected image in the property
            MenuItemBitmapImage = bitmapImage;
            ImageGrid.Background = new ImageBrush { ImageSource = bitmapImage };
            picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            cancel_picture.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        private void Border_MouseLeave(object sender, PointerRoutedEventArgs e)
        {
                this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                bordurePhoto.Background = null;
                ImageGrid.Margin = new Microsoft.UI.Xaml.Thickness(0);
        }

        private void Border_MouseEnter(object sender, PointerRoutedEventArgs e)
        {
            if (MenuItemBitmapImage == null)
            {
                this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                bordurePhoto.Background = new SolidColorBrush(Color.FromArgb(255, 165, 247, 182));
                ImageGrid.Margin = new Microsoft.UI.Xaml.Thickness(2);
            }
        }

        private async void bordurePhoto_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (MenuItemBitmapImage == null)
                await OpenFileDialogAndStoreImageAsync();
        }

        private void cancel_picture_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MenuItemBitmapImage = null;
            ImageGrid.Background = DefaultBackGround;
            picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            cancel_picture.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}