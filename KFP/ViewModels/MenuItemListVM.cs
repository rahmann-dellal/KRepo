using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using KFP.Ui.pages;
using Microsoft.UI.Xaml.Controls;

namespace KFP.ViewModels
{
    public delegate Task<bool> ShowConfirmDeleteDialog(string itemName);
    public partial class MenuItemListVM : KioberViewModelBase
    {
        public const int PageSize = 8;

        public ObservableCollection<MenuItemListElement> MenuItemlistElements { get; private set; } = new ObservableCollection<MenuItemListElement>();
        private KFPContext _dbContext;
        private NavigationService _navigationService;
        private ImageConverter _imageConverter;
        private FileSystemAccess _fileAccess;

        public ShowConfirmDeleteDialog showConfirmDeleteDialog;


        public ObservableCollection<RelayCommand> PageCommands = new ObservableCollection<RelayCommand>();

        private int _selectedPage = 0;
        public bool isPagesCommandsVisible { get { return PageCommands.Count > 1; } }
        public bool isEmptyList { get { return MenuItemlistElements.Count == 0; } }
        public bool ListNotEmpty { get { return MenuItemlistElements.Count != 0; } }
        public int SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (_selectedPage != value)
                {
                    _selectedPage = value;
                    if (_selectedPage > 0 && _selectedPage >= PageCommands.Count)
                    {
                        _selectedPage = PageCommands.Count - 1;
                    }
                    if (_selectedPage >= 0)
                    {
                        LoadPage(_selectedPage);
                    }
                    OnPropertyChanged(nameof(SelectedPage));
                }
            }
        }

        public MenuItemListVM(KFPContext dbContext, NavigationService navigationService, ImageConverter imageConverter, FileSystemAccess fileSystemAccess)
        {
            _dbContext = dbContext;
            _navigationService = navigationService;
            _imageConverter = imageConverter;
            _fileAccess = fileSystemAccess;
            PageCommands.Clear();
            int NumberOfMenuItems = _dbContext.MenuItems.Count();
            int numberOfPages = (int)Math.Ceiling((double)NumberOfMenuItems / PageSize);
            for (int i = 0; i < numberOfPages; i++)
            {
                int currentPage = i; // Capture the current value of i  
                var pageCommand = new RelayCommand(() =>
                {
                    SelectedPage = currentPage;
                });
                PageCommands.Add(pageCommand);
            }
            PageCommands.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(isPagesCommandsVisible)); };
        }


        public void LoadPage(int page = 0)
        {
            if (page < 0)
            {
                throw new Exception("Cannot load negative page number ");
            }
            if (page >= PageCommands.Count)
            {
                page = PageCommands.Count - 1;
            }
            MenuItemlistElements.Clear();
            var MenuItems = _dbContext.MenuItems.Skip(page * PageSize).Take(PageSize).ToList();
            foreach (var menuItem in MenuItems)
            {
                var mile = new MenuItemListElement(menuItem, this, showConfirmDeleteDialog, _dbContext, _navigationService, _imageConverter, _fileAccess);
                MenuItemlistElements.Add(mile);
            }
            OnPropertyChanged(nameof(isEmptyList));
            OnPropertyChanged(nameof(ListNotEmpty));
        }

        [RelayCommand]
        public void GoToAddMenuItem()
        {
            _navigationService.navigateTo(KioberFoodPage.AddMenuItem);
        }
    }

    public partial class MenuItemListElement
    {

        public MenuItem menuItem;
        public string? ThumbnailUri
        {
            get
            {
                if (menuItem.pictureUri != null)
                {
                    return imageConverter.GetThumbUriFromPictureUri(menuItem.pictureUri);
                }
                else
                {
                    return "ms-appx:///Assets/Images/Food100.png";
                }
            }
        }
        private ShowConfirmDeleteDialog showConfirmDeleteDialog;
        private ImageConverter imageConverter;
        private FileSystemAccess fileAccess;
        private KFPContext _dbContext;
        private NavigationService _navigationService;
        private MenuItemListVM parentVM;



        public MenuItemListElement(MenuItem menuItem, MenuItemListVM parentVM,
            ShowConfirmDeleteDialog showConfirmDeleteDialog, KFPContext dbContext, NavigationService ns,
            ImageConverter imageConverter, FileSystemAccess fileSystemAccess)
        {
            this.menuItem = menuItem;
            this.showConfirmDeleteDialog = showConfirmDeleteDialog;
            this.imageConverter = imageConverter;
            this.fileAccess = fileSystemAccess;
            this._dbContext = dbContext;
            this._navigationService = ns;
            this.parentVM = parentVM;
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
        public async Task DeleteMenuItem()
        {
            var result = await showConfirmDeleteDialog(menuItem.ItemName);
            if (result == true)
            {
                fileAccess.DeleteFile(menuItem.pictureUri);
                fileAccess.DeleteFile(ThumbnailUri);
                _dbContext.MenuItems.Remove(menuItem);
                var result2 = _dbContext.SaveChanges();
                if (result2 > 0)
                {
                    int NumberOfMenuItems = _dbContext.MenuItems.Count();
                    int numberOfPages = (int)Math.Ceiling((double)NumberOfMenuItems / MenuItemListVM.PageSize);
                    if (parentVM.PageCommands.Count() > numberOfPages)
                    {
                        if (parentVM.SelectedPage > 0)
                        {
                            parentVM.SelectedPage--;
                        }
                        else
                        {
                            parentVM.LoadPage(parentVM.SelectedPage);
                        }

                        parentVM.PageCommands.RemoveAt(parentVM.PageCommands.Count() - 1);

                    }
                    else
                    {
                        parentVM.LoadPage(parentVM.SelectedPage);
                    }
                }
            }
        }
    }
}
