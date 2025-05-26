using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using KFP.Services;

namespace KFP.ViewModels
{
    public class TablesVM
    {
        private NavigationService _navigationService;
        public RelayCommand NavigateToPOSCommand { get; set; }
        public RelayCommand NavigateToOrdersCommand { get; set; }

        public TablesVM(NavigationService ns)
        {
            _navigationService = ns;
            NavigateToPOSCommand = new RelayCommand(() => _navigationService.navigateTo(KioberFoodPage.POS));
            NavigateToOrdersCommand = new RelayCommand(() => _navigationService.navigateTo(KioberFoodPage.OrdersListPage));

        }
    }
}
