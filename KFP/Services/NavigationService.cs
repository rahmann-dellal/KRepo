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
        private MainFrame _mainFrame;
        public NavigationService(MainFrame mainFrame) {
            _mainFrame = mainFrame;
        }
        public void navigateTo(Type pageType, List<Object> parameters = null){
            _mainFrame.ContentFrame.Navigate(pageType, parameters);
            _mainFrame.selectedNVI = null;
        }

        public void SetNewHeader(string newHeader)
        {
            _mainFrame.NavView.Header = newHeader;
        }
    }
}
