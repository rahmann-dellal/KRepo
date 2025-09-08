
using System.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using KFP.Ui;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private SessionManager _sessionManager;
        private AppState _appState;
        private SubscriptionService _subscriptionService;
        private AppDataService _appDataService;
        public MainWindow()
        {
            _sessionManager = Ioc.Default.GetService<SessionManager>()!;
            _appState = Ioc.Default.GetService<AppState>()!;
            _subscriptionService = Ioc.Default.GetService<SubscriptionService>()!;
            _appDataService = Ioc.Default.GetService<AppDataService>()!;

            this.InitializeComponent();

            this.Title = "Kiober Food POS";
            //Icon to display on titlebar
            this.AppWindow.SetIcon("Assets/Images/Logo/logo-64.ico");
            this.AppWindow.SetPresenter(_appState.WindowPresenterKind);

            if (this.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }
            _appDataService.SubscriptionType = null;
            _appDataService.ExpiryDate = null;
            _appState.PropertyChanged += AppState_PropertyChanged;
            _appState.OnSubscriptionStatusChanged += (s,e) =>
            {
                populateWindow();
            };
            populateWindow();
        }
        private void AppState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.WindowPresenterKind))
            {
                this.AppWindow.SetPresenter(_appState.WindowPresenterKind);
            }
        }
        private void populateWindow()
        {
            var subscriptionRecord = _subscriptionService.GetLocalSupscriptionRecord();
            if (subscriptionRecord != null && subscriptionRecord.isActive())
            {
                this.Content = new MainFrame();
            }
            else
            {
                this.Content = new SubscriptionFrame();
            }
        }

        private void Window_Closed_1(object sender, WindowEventArgs args)
        {
            if (_sessionManager.isSessionActive)
                _sessionManager.EndSession().Wait();

            return;
        }
    }
}
