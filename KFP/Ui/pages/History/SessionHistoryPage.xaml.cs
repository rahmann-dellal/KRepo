using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using KFP.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Text;
using KFP.Services;
using Microsoft.UI;
using System.Globalization;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionHistoryPage : Page
    {
        private SessionHistoryVM viewModel;
        private Style dialogButtonStyle
        {
            get
            {
                Style buttonStyle = new Style(typeof(Button));
                buttonStyle.Setters.Add(new Setter(FrameworkElement.WidthProperty, 130));
                buttonStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, 90));
                buttonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, 2));
                buttonStyle.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.Bold));
                return buttonStyle;
            }
        }
        public SessionHistoryPage()
        {
            viewModel = Ioc.Default.GetService<SessionHistoryVM>()!;
            viewModel.showConfirmDeleteSessionDialog = ShowConfirmDeleteOrderDialog;
            viewModel.showConfirmDeleteSessionRangeDialog = ShowConfirmDeleteSessionRangeDialog;
            this.InitializeComponent();
        }

        public async Task<bool> ShowConfirmDeleteOrderDialog(int OrderId)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Delete_Session");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Deleting_Session") + OrderId;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes_4");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("No_4");
            confirmDialog.CloseButtonStyle = dialogButtonStyle;
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

        public async Task<bool> ShowConfirmDeleteSessionRangeDialog(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string start = startDate.DateTime.ToString(new CultureInfo(viewModel.appDataService.AppLanguage)) ?? "";
            string end = endDate.DateTime.ToString(new CultureInfo(viewModel.appDataService.AppLanguage)) ?? "";

            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Delete_All_Sessions_From") + " " + start + " " +
                StringLocalisationService.getStringWithKey("To_4") + " " + end;
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Deletings_All_Sessions_For_DateRage");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes_4");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("No_4");
            confirmDialog.CloseButtonStyle = dialogButtonStyle;
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
    }
}
