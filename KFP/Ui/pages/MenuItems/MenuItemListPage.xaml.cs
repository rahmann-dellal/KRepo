using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuItemListPage : Page
    {
        private ObservableCollection<MenuItemListElement> MenuItemlistElements = new ObservableCollection<MenuItemListElement>();
        private KFPContext _dbContext;
        private NavigationService _navigationService;
        private ImageConverter _imageConverter;
        public MenuItemListPage()
        {
            _dbContext = Ioc.Default.GetService<KFPContext>();
            _navigationService = Ioc.Default.GetService<NavigationService>();
            _imageConverter = Ioc.Default.GetService<ImageConverter>();

            var MenuItems = _dbContext.MenuItems;
            foreach (var menuItem in MenuItems)
            {
                var mile = new MenuItemListElement(menuItem, MenuItemlistElements, this, _dbContext, _navigationService);
                MenuItemlistElements.Add(mile);
            }
            this.InitializeComponent();
        }

        private void myPage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var menuItemlistElement in MenuItemlistElements)
            {
                //menuItemlistElement.bitmapImage = await ImageConverter.ConvertToBitmapImage(menuItemlistElement.menuItem.thumbnail);
                if(menuItemlistElement.menuItem.pictureUri != null) { 
                    string pictureUri = menuItemlistElement.menuItem.pictureUri;
                    var imageConverter = new ImageConverter();
                    string? thumbnailUri = imageConverter.GetThumbUriFromPictureUri(pictureUri);
                    menuItemlistElement.bitmapImage = _imageConverter.LoadBitmapImage(thumbnailUri);
                }
                if (menuItemlistElement.bitmapImage == null)
                {
                    menuItemlistElement.bitmapImage = _imageConverter.LoadBitmapImage("ms-appx:///Assets/Images/Food100.png");
                }
            }
            MenuItemslistView.ItemsSource = MenuItemlistElements;
        }
    }

    public partial class MenuItemListElement : INotifyPropertyChanged
    {

        public MenuItem menuItem;
        private BitmapImage _bitmapImage;
        public BitmapImage bitmapImage
        {
            get

            {
                return _bitmapImage;
            }
            set
            {
                _bitmapImage = value;
                OnPropertyChanged();
            }
        }
        private Page parentPage;
        private KFPContext _dbContext;
        private NavigationService _navigationService;
        private ObservableCollection<MenuItemListElement> MenuItemlistElements;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MenuItemListElement(MenuItem menuItem, ObservableCollection<MenuItemListElement> menuItemlistElements, Page parentPage, KFPContext dbContext, NavigationService ns)
        {
            this.menuItem = menuItem;
            this.parentPage = parentPage;
            this._dbContext = dbContext;
            this._navigationService = ns;
            this.MenuItemlistElements = menuItemlistElements;
        }

        [RelayCommand]
        public void goToDisplayMenuItem()
        {
            _navigationService.navigateTo(typeof(DisplayMenuItemPage), new List<object> { menuItem.Id });
        }

        [RelayCommand]
        public void goToEditMenuItem()
        {
            _navigationService.navigateTo(typeof(EditMenuItemPage), new List<object> { menuItem.Id });
        }

        [RelayCommand]
        public async void DeleteMenuItem()
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("Are_you_sure_to_continue ");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Deleting") + menuItem.ItemName;
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Delete");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
            confirmDialog.XamlRoot = parentPage.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var imageConverter = Ioc.Default.GetService<ImageConverter>();
                var fileAccess = Ioc.Default.GetService<FileSystemAccess>();
                fileAccess.DeleteFile(menuItem.pictureUri);
                string? thumbnailUri = imageConverter.GetThumbUriFromPictureUri(menuItem.pictureUri);
                fileAccess.DeleteFile(thumbnailUri);
                _dbContext.MenuItems.Remove(menuItem);
                var result2 = _dbContext.SaveChanges();
                if (result2 > 0)
                {
                    MenuItemlistElements.Remove(this);
                }
            }
        }
    }
}
