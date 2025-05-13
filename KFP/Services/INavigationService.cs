using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Services
{
    public interface INavigationService
    {
        public void navigateTo(KioberFoodPage pageType, List<Object> parameters = null);
        public void SetNewHeader(string newHeader);
    }
    public enum KioberFoodPage
    {
        POS,
        DisplayOrderPage,
        MenuItemList,
        AddMenuItem,
        EditMenuItem,
        DisplayMenuItem,
        Categories,
        DisplayUser,
        EditUser,  
        AddUser,
        UserList,
        About,
        Settings,
        Session,
    }
}
