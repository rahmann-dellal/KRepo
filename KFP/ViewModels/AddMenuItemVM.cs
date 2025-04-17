using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;

namespace KFP.ViewModels
{
    public class AddMenuItemVM : MenuItemVM
    {
        public AddMenuItemVM(FileSystemAccess fileSystemAccess, ImageConverter imageConverter, AppDataService appDataService, INavigationService navigationService, KFPContext dbContext) : base(fileSystemAccess, imageConverter, appDataService, navigationService, dbContext)
        {
        }

        public override bool canReset()
        {
            if((ItemName != null && ItemName != string.Empty && ItemName != "") || MenuItemType != null
                || SalePrice != 0 || PictureUri != null || assignedCategories.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool canSave()
        {
            return IsValid();
        }

        public override async Task Reset()
        {
            ItemName = string.Empty;
            MenuItemType = null;
            SalePrice = 0;
            RemovePicture();
            List<Category> categoriesToRemove = new List<Category>(assignedCategories);
            foreach (var category in categoriesToRemove)
            {
                assignedCategories.Remove(category);
                choiseCategories.Add(category);
            }
        }

        public override async Task Save()
        {
            bool confirm = await showConfirmSaveDialog();
            
            if ( confirm)
            {
                model = new MenuItem()
                {
                    ItemName = ItemName,
                    SalePrice = SalePrice,
                    MenuItemType = MenuItemType,
                    pictureUri = PictureUri,
                    Categories = new List<Category>(assignedCategories),
                };
                DbContext.Add(model);
                DbContext.SaveChanges();
                navigationService.navigateTo(KioberFoodPage.DisplayMenuItem, new List<object> { model.Id });
            }
        }
    }
}
