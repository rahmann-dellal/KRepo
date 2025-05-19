using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using KFP.DATA;
using Microsoft.EntityFrameworkCore;
using KFP.DATA_Access;
using KFP.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Text;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayOrderPage : Page
    {
        private DisplayOrderVM ViewModel;
        private Style dialogButtonStyle { get
            {
                Style buttonStyle = new Style(typeof(Button));
                buttonStyle.Setters.Add(new Setter(FrameworkElement.WidthProperty, 130));
                buttonStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, 90));
                buttonStyle.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.Bold));
                return buttonStyle;
            } 
        }
        public DisplayOrderPage()
        {
            ViewModel = Ioc.Default.GetService<DisplayOrderVM>();
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var parameters = e.Parameter as List<Object>;

                if(parameters == null || parameters.Count == 0)
                    return;
                int orderId = (int)parameters.FirstOrDefault();
                ViewModel.showConfirmCancelOrderDialog = ShowConfirmCancelOrderDialog;
                ViewModel.showConfirmCashPaymentDialog = ShowConfirmCashPaymentDialog;
                ViewModel.showConfirmCardPaymentDialog = ShowConfirmCardPaymentDialog;
                ViewModel.showSetOrderCompletedDialog = ShowSetOrderCompletedDialog;
                ViewModel.LoadOrder(orderId);
            }
        }

        public async Task<bool> ShowConfirmCancelOrderDialog(int OrderId)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Cancel_Order");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Canceling_Order") + OrderId;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("No");
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

        public async Task<bool> ShowConfirmCashPaymentDialog(double? total, string currency)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Title = $"{total?.ToString("F2")} {currency}";
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Confirm_Cash_Payment");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Confirm");
            confirmDialog.CloseButtonStyle = dialogButtonStyle;
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");

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

        public async Task<bool> ShowConfirmCardPaymentDialog(double? total, string currency)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Title = $"{total?.ToString("F2")} {currency}";
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Confirm_Card_Payment") ;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Confirm");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
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

        public async Task<bool> ShowSetOrderCompletedDialog(int orderId)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Complete_Order") + "  #" + orderId ;
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Confrim_Order_Completed");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Confirm");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
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
