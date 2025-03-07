using CommunityToolkit.Mvvm.ComponentModel;
using KFP.Services;
using Microsoft.UI.Windowing;

namespace KFP
{
    public partial class AppState : ObservableObject
    {
        private SessionManager _sessionManager;
        public bool isSessionActive
        {
            get
            {
                return _sessionManager.isSessionActive;
            }
        }
        [ObservableProperty]
        private AppWindowPresenterKind windowPresenterKind;
        public AppState(SessionManager sessionManager, AppDataService appDataService)
        {
            _sessionManager = sessionManager;
            windowPresenterKind = appDataService.WindowPresenterKind;
        }
    }
}
