using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using Microsoft.EntityFrameworkCore;

namespace KFP.ViewModels
{
    public delegate Task<string> ShowFileDialogAndGetFileUri();
    public delegate Task<bool> ShowConfirmSaveDialog();

    public partial class EditMenuItemVM : MenuItemVM
    {
        private MenuItem oldValue;

        
        public EditMenuItemVM(FileSystemAccess fileSystemAccess, 
                 ImageConverter imageConverter,
                 AppDataService appDataService,
                 INavigationService navigationService,
                 KFPContext dbContext) : base (fileSystemAccess, imageConverter, appDataService, navigationService, dbContext)
        {
        }
        public void Load(int menuItemID, ShowFileDialogAndGetFileUri showFileDialogAndGetFileUri,
            ShowConfirmSaveDialog showConfirmSaveDialog)
        {
            //model = DbContext.MenuItems.Find(menuItemID);
            model = DbContext.MenuItems
            .Include(m => m.Categories)
            .FirstOrDefault(m => m.Id == menuItemID);

            if (model == null)
            {
                throw new Exception("MenuItem not found");
            }
            this.ItemName = model.ItemName;
            this.MenuItemType = model.MenuItemType;
            this.SalePrice = model.SalePrice;
            this.PictureUri = model.pictureUri;
            oldValue = new MenuItem
            {
                pictureUri = model.pictureUri,
                ItemName = model.ItemName,
                SalePrice = model.SalePrice,
                MenuItemType = model.MenuItemType,
                Categories = model.Categories != null ? new List<Category>(model.Categories) : new List<Category>(),
            };
            if (model.Categories != null)
            {
                foreach (var category in oldValue.Categories)
                {
                    assignedCategories.Add(category);
                }
            }
            choiseCategories.Clear();
            foreach (var category in allCategories)
            {
                if (!assignedCategories.Contains(category))
                {
                    choiseCategories.Add(category);
                }
            }
            this.showFileDialogAndGetFileUri = showFileDialogAndGetFileUri;
            this.showConfirmSaveDialog = showConfirmSaveDialog;
        }

        public bool modelHasChanged()
        {
            if (PictureUri != oldValue.pictureUri ||
                SalePrice != oldValue.SalePrice ||
                ItemName != oldValue.ItemName ||
                MenuItemType != oldValue.MenuItemType)
            {
                return true;
            }
            bool categoriesChanged = false;
            foreach (var category in assignedCategories)
            {
                if (!oldValue.Categories.Contains(category))
                {
                    categoriesChanged = true;
                    break;
                }
            }
            foreach (var category in oldValue.Categories)
            {
                if (!assignedCategories.Contains(category))
                {
                    categoriesChanged = true;
                    break;
                }
            }
            if (categoriesChanged)
            {
                return true;
            }
            return false;
        }

        public override async Task Save()
        {
            bool confirm = await showConfirmSaveDialog();
            if (model != null && confirm)
            {
                model.ItemName = ItemName;
                model.MenuItemType = MenuItemType;
                model.SalePrice = SalePrice;
                model.pictureUri = PictureUri;
                model.Categories = new List<Category>(assignedCategories);
                DbContext.Update(model);
                DbContext.SaveChanges();
                navigationService.navigateTo(KioberFoodPage.DisplayMenuItem, new List<object> { model.Id});
            }
        }
        public override bool canSave()
        {
            return modelHasChanged() && IsValid();
        }

        public override void Reset()
        {
            PictureUri = oldValue.pictureUri;
            SalePrice = oldValue.SalePrice;
            ItemName = oldValue.ItemName;
            MenuItemType = oldValue.MenuItemType;
            assignedCategories.Clear();
            foreach (var category in oldValue.Categories)
            {
                assignedCategories.Add(category);
            }
            choiseCategories.Clear();
            foreach (var category in allCategories)
            {
                if (!assignedCategories.Contains(category))
                {
                    choiseCategories.Add(category);
                }
            }
        }
        public override bool canReset()
        {
            return modelHasChanged();
        }
    }
}
