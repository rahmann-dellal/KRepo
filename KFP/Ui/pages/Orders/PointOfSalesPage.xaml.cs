using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using KFP.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PointOfSalesPage : Page
    {
        PointOfSalesVM ViewModel;
        public PointOfSalesPage()
        {
            ViewModel = Ioc.Default.GetService<PointOfSalesVM>();
            this.InitializeComponent();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedTableNumber))
            {
                tablesFlyout.Hide();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var parameters = e.Parameter as List<Object>;

                if (parameters == null || parameters.Count == 0)
                    return;
                var parameter = parameters.FirstOrDefault();
                if (parameter == null) {
                    return;
                }
                if (parameter is int)
                {
                    // If the parameter is an int, load the order with that ID
                    int orderId = (int)parameter;
                    ViewModel.loadOrder(orderId);
                }
                else if (parameter is TablesElement)
                {
                    ViewModel.SelectedTableNumber = ((TablesElement)parameter).TableNumber;
                    ViewModel.IsSetOnCounter = false;
                    ViewModel.IsSetForDelivery = false;
                }
            }
        }
    }
}
