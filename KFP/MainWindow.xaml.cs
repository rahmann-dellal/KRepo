using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA_Access;
using KFP.Services;
using KFP.Ui;
using KFP.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

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

        public MainWindow()
        {
            _sessionManager = Ioc.Default.GetService<SessionManager>();
            _appState = Ioc.Default.GetService<AppState>();

            this.InitializeComponent();

            this.Title = "Kiober Food POS";
            //Icon to display on titlebar
            this.AppWindow.SetIcon("Assets/Images/Logo/logo-64.ico");
            this.AppWindow.SetPresenter(_appState.WindowPresenterKind);

            _sessionManager.PropertyChanged += onCurrentSessionChange;
            _appState.PropertyChanged += AppState_PropertyChanged;

            populateWindow();
        }
        private void AppState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.WindowPresenterKind))
            {
                this.AppWindow.SetPresenter(_appState.WindowPresenterKind);
            }
        }

        private void onCurrentSessionChange(object? sender, PropertyChangedEventArgs e)
        {
            populateWindow();
        }
        private void populateWindow()
        {
            if (_sessionManager.isSessionActive)
            {
                var mainframe = new MainFrame();
                var ns = Ioc.Default.GetService<NavigationService>();
                ns.MainFrame = mainframe;
                this.Content = mainframe;
            }
            else
            {
                this.Content = new ClockInFrame();
            }
        }
    }
}
