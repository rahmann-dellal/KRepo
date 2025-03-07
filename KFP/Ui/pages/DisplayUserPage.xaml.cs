using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.ViewModels;
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
    public sealed partial class DisplayUserPage : Page
    {
        private DisplayUserVM viewModel;
        public DisplayUserPage()
        {
            this.InitializeComponent();
            viewModel = Ioc.Default.GetService<DisplayUserVM>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                try
                {
                    var parameters = e.Parameter as List<Object>;
                    int userID = (int)parameters.FirstOrDefault();
                    viewModel.userID = userID;
                }
                catch { }
            }
        }

    }
}
