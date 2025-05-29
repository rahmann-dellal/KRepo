using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA;
using KFP.Services;
using KFP.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsVM ViewModel;


        public SettingsPage()
        {
            ViewModel = Ioc.Default.GetService<SettingsVM>()!;
            this.InitializeComponent();
            FullScreenRadio.IsChecked = ViewModel.WindowPresenterKind == AppWindowPresenterKind.FullScreen;
            OverlappedRadio.IsChecked = ViewModel.WindowPresenterKind == AppWindowPresenterKind.Overlapped;
        }

        private void RadioButtonFullScreen_Checked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.WindowPresenterKind != AppWindowPresenterKind.FullScreen)
            {
                ViewModel.WindowPresenterKind = AppWindowPresenterKind.FullScreen;
            }
        }

        private void RadioButton_CheckedOverlapped(object sender, RoutedEventArgs e)
        {
            if (ViewModel.WindowPresenterKind != AppWindowPresenterKind.Overlapped)
            {
                ViewModel.WindowPresenterKind = AppWindowPresenterKind.Overlapped;
            }
        }
    }
}
