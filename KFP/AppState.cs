using CommunityToolkit.Mvvm.ComponentModel;
using KFP.Services;
using Microsoft.UI.Windowing;

namespace KFP
{
    public partial class AppState : ObservableObject
    {
        private SessionManager _sessionManager;
        private AppDataService _appDataService;
        public bool isSessionActive
        {
            get
            {
                return _sessionManager.isSessionActive;
            }
        }
        [ObservableProperty]
        private AppWindowPresenterKind windowPresenterKind;

        [ObservableProperty]
        private bool establishmentHasTables;
        public AppState(SessionManager sessionManager, AppDataService appDataService)
        {
            _sessionManager = sessionManager;
            _appDataService = appDataService;
            windowPresenterKind = appDataService.WindowPresenterKind;
            EstablishmentHasTables = appDataService.NumberOfTables > 0;
        }
    }
}
