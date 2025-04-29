using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using Microsoft.EntityFrameworkCore;

namespace KFP.ViewModels
{
    public partial class MenuItemSelectorVM : KioberViewModelBase
    {
        private KFPContext dbContext;
        private ImageConverter imageConverter;
        private AppDataService appDataService;
        public List<MenuItem> AllMenuItems { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        public ObservableCollection<MenuItemElement> FilteredMenuItems { get; set; } = new();

        private ObservableCollection<Category> _categoryFilter;
        public ObservableCollection<Category> CategoryFilter
        {
            get => _categoryFilter;
            set { _categoryFilter = value; ApplyFilter(); }
        }

        private MenuItemType? _menuItemTypeFilter;
        public MenuItemType? MenuItemTypeFilter
        {
            get => _menuItemTypeFilter;
            set { _menuItemTypeFilter = value; ApplyFilter(); }
        }

        private MenuItem? _selectedMenuItem;
        public MenuItem? SelectedMenuItem
        {
            get => _selectedMenuItem;
            set { _selectedMenuItem = value; OnPropertyChanged(); }
        }

        public bool IsLoading { get; set; }
        private Currency Currency;

        public MenuItemSelectorVM(KFPContext context, ImageConverter imageConverter, AppDataService appDataService)
        {
            dbContext = context;
            this.imageConverter = imageConverter;
            this.appDataService = appDataService;
            Currency = appDataService.Currency;
            CategoryFilter = new ObservableCollection<Category>();
            CategoryFilter.CollectionChanged += (s, e) => ApplyFilter();
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));

            var menuItems = await dbContext.MenuItems
                .Include(m => m.Categories)
                .ToListAsync();
            var categories = await dbContext.Categories.ToListAsync();

            AllMenuItems.Clear();
            foreach (var item in menuItems)
                AllMenuItems.Add(item);

            Categories.Clear();
            foreach (var cat in categories)
                Categories.Add(cat);

            ApplyFilter();
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
        }

        private void ApplyFilter()
        {
            var filtered = AllMenuItems.AsEnumerable();

            foreach (var category in CategoryFilter)
            {
                filtered = filtered.Where(i => i.Categories != null ? i.Categories.Contains(category) : false);
                if (filtered.Count() == 0)
                    break;
            }
            if (MenuItemTypeFilter != null)
                filtered = filtered.Where(i => i.MenuItemType == MenuItemTypeFilter);

            FilteredMenuItems.Clear();
            foreach (var item in filtered)
                FilteredMenuItems.Add(new MenuItemElement(item, imageConverter, Currency));
        }

        public List<MenuItemType> getMenuItemTypes()
        {
            return Enum.GetValues(typeof(MenuItemType)).Cast<MenuItemType>().ToList();
        }

        [RelayCommand]
        public void SelectMenuItem(MenuItemElement menuItemElement)
        {
            if (menuItemElement != null)
            {
                foreach (var item in FilteredMenuItems)
                {
                    item.IsSelected = false;
                }
                menuItemElement.IsSelected = true;
                SelectedMenuItem = menuItemElement.MenuItem;
            }
        }
    }

    public class MenuItemElement : ObservableObject
    {
        public MenuItem MenuItem { get; set; }
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { SetProperty(ref _isSelected, value); }
        }
        public Currency Currency { get; set; }
        private ImageConverter imageConverter;

        public string? ThumbnailUri
        {
            get
            {
                if (MenuItem.pictureUri != null)
                {
                    return imageConverter.GetThumbUriFromPictureUri(MenuItem.pictureUri);
                }
                else
                {
                    return "ms-appx:///Assets/Images/Food100.png";
                }
            }
        }
        public MenuItemElement(MenuItem menuItem, ImageConverter imageConverter, Currency currency)
        {
            this.imageConverter = imageConverter;
            MenuItem = menuItem;
            Currency = currency;
        }
    }
}
