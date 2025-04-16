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
    /// <summary>
    /// If given an ImageUri, it will display the image. If no image is given, it will display a default image.
    /// If given a command, it will execute the command On click.
    /// Also Displays a prompt message when command can execute.
    /// </summary>
    public sealed partial class InteractiveImageDisplay : UserControl
    {
        private ImageConverter _imageConverter;
        ImageBrush? DefaultBackGround = null;

        public static readonly DependencyProperty LoadedImageUriProperty = DependencyProperty.Register(
            nameof(LoadedImageUriProperty),
            typeof(string),
            typeof(InteractiveImageDisplay),
            new PropertyMetadata(null, OnLoadedImageUriChanged));

        public string? LoadedImageUri
        {
            get => (string?)GetValue(LoadedImageUriProperty);
            set => SetValue(LoadedImageUriProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
                                                                        nameof(Command),
                                                                        typeof(ICommand),
                                                                        typeof(InteractiveImageDisplay),
                                                                        new PropertyMetadata(null));

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }


        private async static void OnLoadedImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InteractiveImageDisplay)d;
            if (e.NewValue != null)
            {
                if (File.Exists((string)e.NewValue))
                {
                    BitmapImage bitmapImage = control._imageConverter.LoadBitmapImage((string)e.NewValue);
                    control.ImageGrid.Background = new ImageBrush { ImageSource = bitmapImage };
                    control.picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    control.LoadedImageUri = null;
                }
            }
            else
            {
                control.ImageGrid.Background = control.DefaultBackGround;
                if (control.Command != null && control.Command.CanExecute(null))
                {
                    control.picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                }
            }
        }

        public InteractiveImageDisplay()
        {
            _imageConverter = Ioc.Default.GetService<ImageConverter>();
            this.InitializeComponent();
            DefaultBackGround = new ImageBrush() { ImageSource = _imageConverter.LoadBitmapImage("ms-appx:///Assets/Images/Food.png") };
            ImageGrid.Background = DefaultBackGround;
        }


        private void Border_MouseLeave(object sender, PointerRoutedEventArgs e)
        {
            this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            bordurePhoto.Background = null;
            ImageGrid.Margin = new Microsoft.UI.Xaml.Thickness(0);
        }

        private void Border_MouseEnter(object sender, PointerRoutedEventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                bordurePhoto.Background = new SolidColorBrush(Color.FromArgb(255, 165, 247, 182));
                ImageGrid.Margin = new Microsoft.UI.Xaml.Thickness(2);
            }
        }

        private async void bordurePhoto_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                Command.Execute(null);
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Command == null || !Command.CanExecute(null))
            {
                picture_textbox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}