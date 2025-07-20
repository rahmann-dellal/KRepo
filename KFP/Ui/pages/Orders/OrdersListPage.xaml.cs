using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using KFP.ViewModels;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrdersListPage : Page
    {
        private OrdersListVM ViewModel;
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
        public OrdersListPage()
        {
            ViewModel = Ioc.Default.GetService<OrdersListVM>()!;
            ViewModel.showConfirmCancelOrderDialog = ShowConfirmCancelOrderDialog;
            ViewModel.showSetOrderCompletedDialog = ShowSetOrderCompletedDialog;
            this.InitializeComponent();
        }

        public async Task<bool> ShowConfirmCancelOrderDialog(int OrderId)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Cancel_Order2");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Canceling_Order2") + OrderId;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes");
            confirmDialog.PrimaryButtonStyle = dialogButtonStyle;
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("No_2");
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
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Complete_Order2") + "  #" + orderId;
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Confrim_Order_Completed2");
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
