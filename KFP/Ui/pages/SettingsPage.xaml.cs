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

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (sender.SelectedItem == generalSettingsSelectorItem)
            {
                scrollToElement(GeneralSettingsTextBlock);
            }
            else if (sender.SelectedItem == EstablishmentSelectorItem)
            {
                scrollToElement(EstablishmentTextBlock);
            }
            else if (sender.SelectedItem == OrdersSelectorItem)
            {
                scrollToElement(OrdersTextBlock);
            }
            else if (sender.SelectedItem == PrintingSelectorItem)
            {
                scrollToElement(PrintingTextBlock);
            }
        }

        private void scrollToElement(FrameworkElement element)
        {
            // Get current scroll offset
            double currentOffset = scrollViewer.VerticalOffset;

            // Get the position of the element relative to the ScrollViewer
            GeneralTransform transform = element.TransformToVisual(scrollViewer);
            Point position = transform.TransformPoint(new Point(0, scrollViewer.VerticalOffset));
            double targetOffset = position.Y;


            scrollViewer.ChangeView(null, targetOffset, null);
        }

        private void scrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            selectorBar.SelectedItem = null; // Reset the selected item when scrolling stops
        }
    }
}
