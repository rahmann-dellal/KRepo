using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA;
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
using System.Globalization;
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
    public sealed partial class SessionPage : Page
    {

        private SessionManager _sessionManager;
        private AppDataService _appDataService;
        public Session CurentSession
        {
            get
            {
                return _sessionManager.CurrentSession;
            }
        }
        public AppUser LoggedInUser
        {
            get
            {
                return _sessionManager.LoggedInUser;
            }
        }
        public string startedAt
        {
            get
            {
                return CurentSession.Start.ToString(new CultureInfo(_appDataService.AppLanguage));
            }
        }
        private DispatcherTimer timer;
        private string activeFor;
        DispatcherTimer dispatcherTimer;
        public SessionPage()
        {
            _sessionManager = Ioc.Default.GetService<SessionManager>();
            _appDataService = Ioc.Default.GetService<AppDataService>();
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
            this.Unloaded += (s, e) => { if (dispatcherTimer != null) dispatcherTimer.Stop(); };
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 1); //10000000 x 100 nanoseconds = 1 second
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            TimeSpan timeSpan = DateTime.Now - CurentSession.Start;
            timerTextBlock.Text = string.Format("{0:hh\\:mm\\:ss}", timeSpan);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            _sessionManager.EndSession();
        }

    }
}
