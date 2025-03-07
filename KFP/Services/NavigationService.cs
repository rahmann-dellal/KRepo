using KFP.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Services
{
    public class NavigationService
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
    }
}
