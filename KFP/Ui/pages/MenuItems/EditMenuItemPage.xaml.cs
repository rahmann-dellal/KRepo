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
        private string _itemName;
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        private double _itemPrice = double.NaN;
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
            var categories = _dbContext.Categories.ToList();
            foreach (var cat in categories)
            {
                AllCategories.Add(cat);
            }
            Currency = _appDataService.Currency.ToString();
            this.InitializeComponent();
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            var menuItem = new MenuItem(ItemName, ItemPrice, SelectedMenuItemType);
            menuItem.Categories = AssignedCategories.ToList();
            menuItem.picture = imageGraber.LoadedImage;
            _dbContext.MenuItems.Add(menuItem);
            _dbContext.SaveChanges();
        }

        public bool CanSave() {
            return !string.IsNullOrEmpty(itemNameTextBox.Text) &&
                !Double.IsNaN(itemPriceNumberBox.Value);
        }

        [RelayCommand(CanExecute = nameof(CanReset))]
        public void ResetForm()
        {
            // Reset all fields to their default values
            itemNameTextBox.Text = string.Empty;
            MenuItemTypeRadioButtons.SelectedIndex = -1;
            itemPriceNumberBox.Value = double.NaN;
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
            return !string.IsNullOrEmpty(itemNameTextBox.Text) ||
                   MenuItemTypeRadioButtons.SelectedIndex != -1 ||
                   !Double.IsNaN(itemPriceNumberBox.Value) ||
                   AssignedCategories.Count > 0;
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
