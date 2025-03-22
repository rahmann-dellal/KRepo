using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayMenuItemPage : Page
    {

        private MenuItem _menuItem;
        private NavigationService _navigationService;
        private KFPContext _dbContext;
        public DisplayMenuItemPage()
        {
            _navigationService = Ioc.Default.GetService<NavigationService>();
            _dbContext = Ioc.Default.GetService<KFPContext>();
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                try
                {
                    var parameters = e.Parameter as List<Object>;
                    int ItemID = (int)parameters.FirstOrDefault();
                    _menuItem = _dbContext.MenuItems.Find(ItemID);   
                }
                catch { }
            }
        }


    }
}
