using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using KFP.Helpers;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using KFP.Messages;
using Windows.System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditMenuItemPage : Page
    {
        private ObservableCollection<Category> AllCategories = new ObservableCollection<Category>();
        private ObservableCollection<Category> AssignedCategories = new ObservableCollection<Category>();

        private string Currency;

        private AppDataService _appDataService;
        private KFPContext _dbContext;
        private NavigationService _navigationService;

        private byte[]? _picture;

        public byte[]? Picture
        {
            get => _picture;
            set
            {
                _picture = value;
                OnPropertyChanged(nameof(Picture));
            }
        }

        private string _itemName;
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    itemNameErrorBlock.Text = StringLocalisationService.getStringWithKey("please_provide_name_for_item");
                    itemNameErrorBlock.Visibility = Visibility.Visible;
                }
                else if(_dbContext.MenuItems.Any(x => x.ItemName == value))
                {
                    itemNameErrorBlock.Text = StringLocalisationService.getStringWithKey("You_already_have_an_item_with_the_same_name");
                    itemNameErrorBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    itemNameErrorBlock.Visibility = Visibility.Collapsed;
                }
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        private double _itemPrice = double.NaN;

        public string _itemPriceString;

        public string ItemPriceString
        {
            get { return _itemPriceString; }
            set
            {
                _itemPriceString = value;
                try {
                    if (value.Contains(','))
                    {
                        value = value.Replace(',', '.');
                    }
                    var val = double.Parse(value);
                    if(val != ItemPrice && val >= 0)
                    {
                        ItemPrice = val;
                        itemPriceErrorBlock.Visibility = Visibility.Collapsed;
                    }
                }
                catch {
                    ItemPrice = double.NaN;
                    itemPriceErrorBlock.Text = StringLocalisationService.getStringWithKey("Invalid_entry");
                    itemPriceErrorBlock.Visibility = Visibility.Visible;
                }
                OnPropertyChanged(nameof(ItemPriceString));
            }
        }
        public double ItemPrice
        {
            get { return _itemPrice; }
            set
            {
                _itemPrice = value;
                OnPropertyChanged(nameof(ItemPrice));
            }
        }

        private MenuItemType? _selectedMenuItemType = null;
        public MenuItemType? SelectedMenuItemType
        {
            get => _selectedMenuItemType;
            set
            {
                _selectedMenuItemType = value;
                OnPropertyChanged(nameof(SelectedMenuItemType));
            }
        }

        private Category _selectedCategoryToAdd = null;
        private Category selectedCategoryToAdd
        {
            get
            {
                return _selectedCategoryToAdd;
            }
            set
            {
                _selectedCategoryToAdd = value;
                addCategoryCommand.NotifyCanExecuteChanged();
            }
        }

        private Category _selectedCategoryToRemove = null;
        private Category selectedCategoryToRemove
        {
            get
            {
                return _selectedCategoryToRemove;
            }
            set
            {
                _selectedCategoryToRemove = value;
                RemoveCategoryCommand.NotifyCanExecuteChanged();
            }
        }
        public EditMenuItemPage()
        {
            _appDataService = Ioc.Default.GetService<AppDataService>();
            _dbContext = Ioc.Default.GetService<KFPContext>();
            _navigationService = Ioc.Default.GetService<NavigationService>();
            var categories = _dbContext.Categories.ToList();
            foreach (var cat in categories)
            {
                AllCategories.Add(cat);
            }
            Currency = _appDataService.Currency.ToString();
            this.InitializeComponent();
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        public async Task Save()
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("save_changes");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Confirm");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
            confirmDialog.XamlRoot = this.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var menuItem = new MenuItem(ItemName, ItemPrice, SelectedMenuItemType);
                menuItem.Categories = AssignedCategories.ToList();
                menuItem.picture = await ImageConverter.ResizeImageIfNeeded(Picture);
                _dbContext.MenuItems.Add(menuItem);
                var res = _dbContext.SaveChanges();

                if (res > 0)
                {
                    _navigationService.navigateTo(typeof(DisplayMenuItemPage), new List<object>() { menuItem.Id});
                }
            }
        }

        public bool CanSave() {
                return !string.IsNullOrEmpty(itemNameTextBox.Text) &&
                    !Double.IsNaN(ItemPrice);
        }

        [RelayCommand(CanExecute = nameof(CanReset))]
        public void ResetForm()
        {
            // Reset all fields to their default values
            itemNameTextBox.Text = string.Empty;
            MenuItemTypeRadioButtons.SelectedIndex = -1;
            ItemPriceString = "";
            itemPriceNumberBox.Text = string.Empty;
            imageGraber.LoadedImage = null;
            AllCategories.Clear();
            AssignedCategories.Clear();
            var categories = _dbContext.Categories.ToList();
            foreach (var cat in categories)
            {
                AllCategories.Add(cat);
            }
            _selectedCategoryToAdd = null;
            _selectedCategoryToRemove = null;
            SelectedMenuItemType = default(MenuItemType);
            addCategoryCommand.NotifyCanExecuteChanged();
            RemoveCategoryCommand.NotifyCanExecuteChanged();
            NothingTextBlock.Visibility = Visibility.Visible;
            ResetFormCommand.NotifyCanExecuteChanged();
        }

        public bool CanReset()
        {
            return !string.IsNullOrEmpty(ItemName) ||
                   MenuItemTypeRadioButtons.SelectedIndex != -1 ||
                   !Double.IsNaN(ItemPrice) ||
                   AssignedCategories.Count > 0 || Picture != null;
        }

        [RelayCommand (CanExecute = nameof(canAddCategory))]
        public void AddCategory()
        {
            AssignedCategories.Add((Category)allCategoriesCombo.SelectedItem);
            AllCategories.Remove((Category)allCategoriesCombo.SelectedItem);
            NothingTextBlock.Visibility = Visibility.Collapsed;
            ResetFormCommand.NotifyCanExecuteChanged();

        }
        public bool canAddCategory()
        {
            return allCategoriesCombo.SelectedItem != null;
        }

        [RelayCommand(CanExecute = nameof(canRemove))]
        public void RemoveCategory()
        {
            AllCategories.Add((Category)AssignedCotegoriesListView.SelectedItem);
            AssignedCategories.Remove((Category)AssignedCotegoriesListView.SelectedItem);
            if(AssignedCategories.Count == 0)
            {
                NothingTextBlock.Visibility = Visibility.Visible;
            }
            ResetFormCommand.NotifyCanExecuteChanged();

        }
        public bool canRemove()
        {
            return AssignedCategories.Count > 0 && AssignedCotegoriesListView.SelectedItem != null;
        }

        private Array getTypeEnumValues()
        {
            return Enum.GetValues(typeof(KFP.DATA.MenuItemType));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            resetFormCommand.NotifyCanExecuteChanged();
            saveCommand.NotifyCanExecuteChanged();
        }
    }
}
