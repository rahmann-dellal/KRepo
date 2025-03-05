using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using KFP.Services;
using KFP.Ui;
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
        private MainFrame _mainFrame;
        public MainWindow(MainFrame mainFrame, SessionManager sessionManager)
        {
            _mainFrame = mainFrame;
            _sessionManager = sessionManager;
            this.Title = "Kiober Food POS";
            //Icon to display on titlebar
            this.AppWindow.SetIcon("Assets/Images/Logo/logo-64.ico");
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
                this.Content = _mainFrame;
            }
            else
            {
                this.Content = new ClockInFrame();
            }
        }
    }
}
