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
using Microsoft.Windows.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.UserProfile;

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


        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null || e.AddedItems.Count == 0)
                return;

            var selectedLanguage = e.AddedItems[0] as Language;

            if (selectedLanguage?.Code == ViewModel.language.Code)
                return; // no change

            var result = await showConfirmLanguageChangeDialog();
            if (!result) {
                LanguageComboBox.SelectedItem = ViewModel.language;
                return; // User cancelled the language change
            }
            ViewModel.language = selectedLanguage;
            ApplicationLanguages.PrimaryLanguageOverride = selectedLanguage.Code;
            await Task.Delay(500);
            restartApp();
        }
        public void restartApp()
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;

            // Start a new instance
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true
            });

            // Close current instance
            Application.Current.Exit();  //or Process.GetCurrentProcess().Kill();
        }
        public async Task<bool> showConfirmLanguageChangeDialog()
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("App_Restart_Needed_to_apply_language_change ");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("RestartNeeded");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Restart");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel_3");
            confirmDialog.DefaultButton = ContentDialogButton.Primary;
            confirmDialog.XamlRoot = this.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }
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
