using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;

namespace KFP.ViewModels
{
    public abstract partial class MenuItemVM : KioberViewModelBase
    {
        protected FileSystemAccess fileSystemAccess;
        protected ImageConverter imageConverter;
        protected AppDataService appDataService;
        protected StringLocalisationService stringLocalisationService;
        protected INavigationService navigationService;
        protected KFPContext DbContext;

        public ShowFileDialogAndGetFileUri showFileDialogAndGetFileUri;
        public ShowConfirmSaveDialog showConfirmSaveDialog;

        public string _itemNameErrorMessage;
        public string ItemNameErrorMessage
        {
            get => _itemNameErrorMessage;
            set
            {
                _itemNameErrorMessage = value;
                OnPropertyChanged(nameof(ItemNameErrorMessage));
            }
        }

        protected MenuItem model;

        public string _itemName;
        public string ItemName
        {
            get => _itemName;
            set
            {
                if (value != null && !String.IsNullOrEmpty(value.Trim()))
                {
                    var newName = value.Trim();
                    MenuItem existingItem;
                    if (model != null)
                    {
                        existingItem = DbContext.MenuItems.FirstOrDefault(x => x.ItemName == newName && x.Id != model.Id);
                    }
                    else {
                        existingItem = DbContext.MenuItems.FirstOrDefault(x => x.ItemName == newName);
                    }

                    if (existingItem != null)
                    {
                        ItemNameErrorMessage = StringLocalisationService.getStringWithKey("You_already_have_an_item_with_the_same_name");
                    }
                    else
                    {
                        ItemNameErrorMessage = null;
                    }
                }
                else
                {
                    ItemNameErrorMessage = StringLocalisationService.getStringWithKey("please_provide_name_for_item");
                }
                SetProperty(ref _itemName, value);
            }
        }

        [ObservableProperty]
        public double salePrice;
        
        private string? _pictureUri;
        public string? PictureUri
        {
            get => _pictureUri;
            set
            {
                SetProperty(ref _pictureUri, value);
                SelectPictureCommand.NotifyCanExecuteChanged();
                RemovePictureCommand.NotifyCanExecuteChanged();
            }
        }

        [ObservableProperty]
        public MenuItemType? menuItemType;

        public Array MenuItemTypeEnumValues
        {
            get
            {
                return Enum.GetValues(typeof(KFP.DATA.MenuItemType));
            }
        }

        public string Currency { get; set; }

        protected List<Category> allCategories;

        public ObservableCollection<Category> choiseCategories { get; set; }
        public ObservableCollection<Category> assignedCategories { get; set; }

        private Category? _selectedCategoryToAdd = null;
        public Category? SelectedCategoryToAdd
        {
            get => _selectedCategoryToAdd;
            set
            {
                _selectedCategoryToAdd = value;
                AddCategoryCommand.NotifyCanExecuteChanged();
            }
        }
        private Category? _selectedCategoryToRemove = null;
        public Category? SelectedCategoryToRemove
        {
            get => _selectedCategoryToRemove;
            set
            {
                _selectedCategoryToRemove = value;
                RemoveCategoryCommand.NotifyCanExecuteChanged();
            }
        }

        public MenuItemVM(FileSystemAccess fileSystemAccess, ImageConverter imageConverter, AppDataService appDataService,
               INavigationService navigationService, KFPContext dbContext)
        {
            this.DbContext = dbContext;
            this.fileSystemAccess = fileSystemAccess;
            this.imageConverter = imageConverter;
            this.navigationService = navigationService;
            this.appDataService = appDataService;

            Currency = appDataService.Currency.ToString();
            allCategories = DbContext.Categories.ToList();
            if (allCategories != null)
            {
                choiseCategories = new ObservableCollection<Category>(allCategories);
            }
            else
            {
                allCategories = new List<Category>();
                choiseCategories = new ObservableCollection<Category>();
            }
            assignedCategories = new ObservableCollection<Category>();
            this.PropertyChanged += OnPropertyChanged;
        }

        [RelayCommand(CanExecute = nameof(canSave))]
        public abstract Task Save();
        public abstract bool canSave();

        [RelayCommand(CanExecute = nameof(canReset))]
        public abstract void Reset();
        public abstract bool canReset();

        [RelayCommand(CanExecute = nameof(canAddCategory))]
        public  void AddCategory()
        {
            if (SelectedCategoryToAdd != null)
            {
                assignedCategories.Add(SelectedCategoryToAdd);
                choiseCategories.Remove(SelectedCategoryToAdd);
                SelectedCategoryToAdd = null;
                SaveCommand.NotifyCanExecuteChanged();
                ResetCommand.NotifyCanExecuteChanged();
            }
        }

        public bool canAddCategory()
        {
            return SelectedCategoryToAdd != null;
        }

        [RelayCommand(CanExecute = nameof(canRemoveCategory))]
        public  void RemoveCategory()
        {
            if (SelectedCategoryToRemove != null)
            {
                choiseCategories.Add(SelectedCategoryToRemove);
                assignedCategories.Remove(SelectedCategoryToRemove);
                SelectedCategoryToRemove = null;
                SaveCommand.NotifyCanExecuteChanged();
                ResetCommand.NotifyCanExecuteChanged();
            }
        }

        public  bool canRemoveCategory()
        {
            return SelectedCategoryToRemove != null;
        }


        [RelayCommand(CanExecute = nameof(canRemovePicture))]
        public  void RemovePicture()
        {
            fileSystemAccess.DeleteFile(PictureUri);
            fileSystemAccess.DeleteFile(imageConverter.GetThumbUriFromPictureUri(PictureUri));
            PictureUri = null;
        }
        public  bool canRemovePicture()
        {
            return !string.IsNullOrEmpty(PictureUri);
        }


        [RelayCommand(CanExecute = nameof(canSelectPicture))]
        public async Task SelectPicture()
        {
            var fileUri = await showFileDialogAndGetFileUri();

            if (fileUri != null)
            {
                byte[]? imageData = fileSystemAccess.ReadFile(fileUri);
                if (imageData != null)
                {

                    imageData = await imageConverter.ResizeImageIfNeeded(imageData, false);
                    if (imageData != null)
                    {
                        PictureUri = await imageConverter.SaveImageToFile(imageData);
                    }
                    byte[]? thumbData = await imageConverter.ResizeImageIfNeeded(imageData, true);
                    if (thumbData != null)
                    {
                        string? thumbPath = imageConverter.GetThumbUriFromPictureUri(PictureUri);
                        if (thumbPath != null) { 
                            await imageConverter.SaveImageToFile(thumbData, imageConverter.GetThumbUriFromPictureUri(PictureUri));
                        }
                    }
                }
            }
        }
        public bool canSelectPicture()
        {
            return string.IsNullOrEmpty(PictureUri);
        }

        protected void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        }

        public bool IsValid()
        {
            if (ItemNameErrorMessage != null)
            {
                return false;
            }
            if (SalePrice < 0)
            {
                return false;
            }
            return true;
        }
    }
}
