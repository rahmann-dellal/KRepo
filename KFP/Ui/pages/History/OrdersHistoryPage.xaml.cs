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
using KFP.Services;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using System.Globalization;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrdersHistoryPage : Page
    {
        private OrdersHistoryVM viewModel;
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
        public OrdersHistoryPage()
        {
            viewModel = Ioc.Default.GetService<OrdersHistoryVM>()!;
            viewModel.showConfirmDeleteOrderDialog = ShowConfirmDeleteOrderDialog;
            viewModel.showConfirmDeleteRangeOrderDialog = ShowConfirmDeleteRangeOrderDialog;
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        public async Task<bool> ShowConfirmDeleteOrderDialog(int OrderId)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("OrdersHistoryPage_Delete_Order");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("OrdersHistoryPage_Deleting_Order") + OrderId;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("OrdersHistoryPage_Yes");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("OrdersHistoryPage_No");
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

        public async Task<bool> ShowConfirmDeleteRangeOrderDialog(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string start = startDate.DateTime.ToString(new CultureInfo(viewModel.appDataService.AppLanguage)) ?? "";
            string end = endDate.DateTime.ToString(new CultureInfo(viewModel.appDataService.AppLanguage)) ?? "";

            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("OrdersHistoryPage_Delete_All_Orders_From") + " " + start + " " +
                StringLocalisationService.getStringWithKey("OrdersHistoryPage_To") + " " + end;
            confirmDialog.Title = StringLocalisationService.getStringWithKey("OrdersHistoryPage_Deletings_All_Orders_For_DateRage");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("OrdersHistoryPage_Yes");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.PrimaryButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.OrangeRed)));
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("OrdersHistoryPage_No");
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
