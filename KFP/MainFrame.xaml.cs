using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using KFP.Ui;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP
{
    public sealed partial class MainFrame : UserControl
    {
        private SessionManager _sessionManager;
        private AppState _appState;
        public MainFrame()
        {
            _sessionManager = Ioc.Default.GetService<SessionManager>()!;
            _appState = Ioc.Default.GetService<AppState>()!;
            this.InitializeComponent();
            _sessionManager.PropertyChanged += onCurrentSessionChange;
            populateWindow();
        }
        private void onCurrentSessionChange(object? sender, PropertyChangedEventArgs e)
        {
            populateWindow();
        }
        private void populateWindow()
        {
            if (_sessionManager.isSessionActive)
            {
                var operationFrame = new OperationFrame();
                var ns = Ioc.Default.GetService<NavigationService>()!;
                ns.OperationFrame = operationFrame;
                this.Content = operationFrame;
            }
            else
            {
                this.Content = new ClockInFrame();
            }
        }
    }
}
