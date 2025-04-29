using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using KFP.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuItemListPage : Page
    {
        MenuItemListVM viewModel;
        public MenuItemListPage()
        {
            viewModel = Ioc.Default.GetService<MenuItemListVM>();
            viewModel.showConfirmDeleteDialog = ShowConfirmDeleteDialog;
            this.InitializeComponent();
        }
        
        public async Task<bool> ShowConfirmDeleteDialog(string itemName)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Are_you_sure_to_continue ");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Deleting") + itemName;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Delete");
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

        private void MyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RelayCommand? command = e.AddedItems.FirstOrDefault() as RelayCommand;
            command?.Execute(null);
        }

        private async void myPage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadPageAsync();
        }
    }
}
