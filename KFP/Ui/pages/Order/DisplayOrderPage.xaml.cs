using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using KFP.DATA;
using Microsoft.EntityFrameworkCore;
using KFP.DATA_Access;
using KFP.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayOrderPage : Page
    {
        private DisplayOrderVM ViewModel;
        public DisplayOrderPage()
        {
            ViewModel = Ioc.Default.GetService<DisplayOrderVM>();
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var parameters = e.Parameter as List<Object>;

                if(parameters == null || parameters.Count == 0)
                    return;
                int orderId = (int)parameters.FirstOrDefault();
                ViewModel.LoadOrder(orderId);
            }
        }
    }
}
