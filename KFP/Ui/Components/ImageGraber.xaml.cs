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
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.Components
{
    public sealed partial class ImageGraber : UserControl
    {
        ImageBrush? DefaultBackGround = null;
        public BitmapImage DisplayedBitmapImage { get; private set; }

        public static readonly DependencyProperty LoadedImageProperty =
            DependencyProperty.Register(
                nameof(LoadedImage),
                typeof(byte[]),
                typeof(ImageGraber),
                new PropertyMetadata(null, OnLoadedImageChanged));

        public byte[]? LoadedImage
        {
            get => (byte[]?)GetValue(LoadedImageProperty);
            set => SetValue(LoadedImageProperty, value);
        }

        public ImageGraber()
        {
            this.InitializeComponent();
            DefaultBackGround = new ImageBrush() { ImageSource = ImageConverter.LoadBitmapImage("ms-appx:///Assets/Images/Food.png") };
            ImageGrid.Background = DefaultBackGround;
        }
        private static async void OnLoadedImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ImageGraber)d;
            var imgData = (byte[]?)e.NewValue;

            if (imgData != null)
            {
                imgData = await ImageConverter.ResizeImageIfNeeded(imgData);
                control.LoadedImage = imgData;
                BitmapImage bitmapImage = await ImageConverter.ConvertToBitmapImage(imgData);
                control.DisplayedBitmapImage = bitmapImage;
                control.ImageGrid.Background = new ImageBrush { ImageSource = bitmapImage };
                control.picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                control.cancel_picture.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
            else
            {
                control.DisplayedBitmapImage = null;
                control.ImageGrid.Background = control.DefaultBackGround;
                control.picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                control.cancel_picture.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }

        public async Task OpenFileDialogAndUploadImageAsync()
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
                return;

            LoadedImage = File.ReadAllBytes(file.Path);
        }

        private void Border_MouseLeave(object sender, PointerRoutedEventArgs e)
        {
                this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                bordurePhoto.Background = null;
                ImageGrid.Margin = new Microsoft.UI.Xaml.Thickness(0);
        }

        private void Border_MouseEnter(object sender, PointerRoutedEventArgs e)
        {
            if (DisplayedBitmapImage == null)
            {
                this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                bordurePhoto.Background = new SolidColorBrush(Color.FromArgb(255, 165, 247, 182));
                ImageGrid.Margin = new Microsoft.UI.Xaml.Thickness(2);
            }
        }

        private async void bordurePhoto_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (DisplayedBitmapImage == null)
                await OpenFileDialogAndUploadImageAsync();
        }

        private void cancel_picture_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Reset the LoadedImage property
            LoadedImage = null;
        }
    }
}