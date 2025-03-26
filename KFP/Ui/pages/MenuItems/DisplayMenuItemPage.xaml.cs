using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using KFP.Ui.Components;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayMenuItemPage : Page
    {

        private MenuItem _menuItem;
        private NavigationService _navigationService;
        private AppDataService _appDataService;
        private KFPContext _dbContext;
        private Currency _currency;

        public DisplayMenuItemPage()
        {
            _navigationService = Ioc.Default.GetService<NavigationService>();
            _dbContext = Ioc.Default.GetService<KFPContext>();
            _appDataService = Ioc.Default.GetService<AppDataService>();
            _currency = _appDataService.Currency;
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                try
                {
                    var parameters = e.Parameter as List<Object>;
                    int ItemID = (int)parameters.FirstOrDefault();
                    _menuItem = _dbContext.MenuItems.Find(ItemID);
                    _navigationService.SetNewHeader(_menuItem.ItemName);
                    if(_menuItem.Categories == null || _menuItem.Categories.Count == 0)
                    {
                        EmptyTextBlock.Visibility = Visibility.Visible;
                    }

                    if (_menuItem.pictureUri != null)
                    {
                        picture.Source = ImageConverter.LoadBitmapImage(_menuItem.pictureUri);
                    }
                    else
                    {
                        picture.Source = ImageConverter.LoadBitmapImage("ms-appx:///Assets/Images/Food.png");
                    }
                }
                catch { }
            }
        }

        [RelayCommand]
        public async void Delete()
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Are_you_sure_to_continue ");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Deleting") + _menuItem.ItemName;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Delete");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
            confirmDialog.XamlRoot = this.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _dbContext.Remove(_menuItem);
                _dbContext.SaveChanges();
                _navigationService.navigateTo(typeof(MenuItemListPage));
            }    
        }

        [RelayCommand]
        public void GoToEditPage()
        {
            if (_menuItem != null)
            {
                _navigationService.navigateTo(typeof(EditMenuItemPage), new List<Object> { _menuItem.Id });
            }
        }

    }
}
