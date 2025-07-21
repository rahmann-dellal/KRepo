using KFP.Ui;
using KFP.Ui.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Services
{
    public class NavigationService : INavigationService
    {
        public MainFrame MainFrame {get; set;}
        public NavigationService() {
        }
        public void navigateTo(Type pageType, List<Object> parameters = null){
            MainFrame.ContentFrame.Navigate(pageType, parameters);
            MainFrame.selectedNVI = null;
        }

        public void SetNewHeader(string newHeader)
        {
            MainFrame.NavView.Header = newHeader;
        }

        public void navigateTo(KioberFoodPage pageType, List<object> parameters = null)
        {
            if(pageType == KioberFoodPage.POS)
            {
                navigateTo(typeof(PointOfSalesPage), parameters);
            }
            else if (pageType == KioberFoodPage.DisplayOrderPage)
            {
                navigateTo(typeof(DisplayOrderPage), parameters);
            }
            else if(pageType == KioberFoodPage.OrdersListPage)
            {
                navigateTo(typeof(OrdersListPage), parameters);
            }
            else if (pageType == KioberFoodPage.TablesPage)
            {
                navigateTo(typeof(TablesPage), parameters);
            }
            else if (pageType == KioberFoodPage.MenuItemList)
            {
                navigateTo(typeof(MenuItemListPage), parameters);
            }
            else if (pageType == KioberFoodPage.AddMenuItem)
            {
                navigateTo(typeof(EditMenuItemPage), parameters);
            }
            else if (pageType == KioberFoodPage.EditMenuItem)
            {
                navigateTo(typeof(EditMenuItemPage), parameters);
            }
            else if (pageType == KioberFoodPage.DisplayMenuItem)
            {
                navigateTo(typeof(DisplayMenuItemPage), parameters);
            }
            else if (pageType == KioberFoodPage.Categories)
            {
                navigateTo(typeof(CategoriesPage), parameters);
            }
            else if (pageType == KioberFoodPage.OrdersHistoryPage)
            {
                navigateTo(typeof(OrdersHistoryPage), parameters);
            }
            else if (pageType == KioberFoodPage.SessionHistoryPage)
            {
                navigateTo(typeof(SessionHistoryPage), parameters);
            }
            else if (pageType == KioberFoodPage.DisplayUser)
            {
                navigateTo(typeof(DisplayUserPage), parameters);
            }
            else if (pageType == KioberFoodPage.EditUser)
            {
                navigateTo(typeof(EditUserPage), parameters);
            }
            else if (pageType == KioberFoodPage.AddUser)
            {
                navigateTo(typeof(EditUserPage), parameters);
            }
            else if (pageType == KioberFoodPage.UserList)
            {
                navigateTo(typeof(UserListPage), parameters);
            }
            else if (pageType == KioberFoodPage.About)
            {
                navigateTo(typeof(AboutPage), parameters);
            }
            else if (pageType == KioberFoodPage.Settings)
            {
                navigateTo(typeof(SettingsPage), parameters);
            }
            else if (pageType == KioberFoodPage.Account)
            {
                navigateTo(typeof(AccountPage), parameters);
            }
        }
    }
}
