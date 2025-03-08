using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using KFP.ViewModels;
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayUserPage : Page
    {
        private DisplayUserVM viewModel;
        public DisplayUserPage()
        {
            this.InitializeComponent();
            viewModel = Ioc.Default.GetService<DisplayUserVM>();
            viewModel.DisplayConfirmDialog = DisplayConfirmDialog;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                try
                {
                    var parameters = e.Parameter as List<Object>;
                    int userID = (int)parameters.FirstOrDefault();
                    viewModel.userID = userID;
                }
                catch { }
            }
        }

        public async Task<Boolean> DisplayConfirmDialog()
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Are_you_sure_to_continue ");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Deleting") + viewModel.User.UserName ;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Delete");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
            confirmDialog.XamlRoot = this.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else return false;
        }

    }
}
