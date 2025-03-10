using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using KFP.Ui.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui
{
    public sealed partial class ClockInFrame : UserControl
    {
        private KFPContext _context;
        private SessionManager _sessionManager;

        public List<AppUser> AppUsers { get; private set; }

        private AppUser? _selectedUser;


        public AppUser? selectedUser { 
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                userContentControl.DataContext = selectedUser;
                if (value != null)
                {
                    selectUserPanel.Visibility = Visibility.Collapsed;
                    SelectedUserPanelFadeIn();
                }
                else
                {
                    SelectedUserPanelFadeOut();
                    selectUserPanel.Visibility = Visibility.Visible;
                }
            }
        }

        public bool isUserSelected { 
            get
            {
                return selectedUser != null;
            }
        }
        public ClockInFrame()
        {
            _context = Ioc.Default.GetService<KFPContext>();
            _sessionManager = Ioc.Default.GetService<SessionManager>();
            AppUsers = _context.AppUsers.ToList();
            
            this.InitializeComponent();

            if (AppUsers.Count == 1)
            {
                selectedUser = AppUsers[0];
                UselectUserButton.Visibility = Visibility.Collapsed;
            }

            cancelAndExitButton.Content = StringLocalisationService.getStringWithKey("cancelAndExit");
            pinInput.PromptText = StringLocalisationService.getStringWithKey("please_enter_pin");
            pinInput.PinIsValidText = StringLocalisationService.getStringWithKey("WaitingForLoggin");
            pinInput.PinChanged += onPinChange;
        }

        private void onPinChange(object? sender, EventArgs e)
        {
            if (pinInput.isPinValid && selectedUser != null)
            {
                var Logintask = _sessionManager.tryLogin(selectedUser, pinInput.PIN);
                Logintask.Wait();
                if(Logintask.Result == false)
                {
                    pinInput.PromptText = StringLocalisationService.getStringWithKey("Wrong_PIN_Try_again");
                    pinInput.resetPin();
                }
            }
        }

        private void cancelAndExitButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void UselectUserButton_Click(object sender, RoutedEventArgs e)
        {
            if(AppUsers.Count > 1) { 
                selectedUser = null;
            }
        }

        private void SelectedUserPanelFadeIn()
        {
            selectedUserPanel.Visibility = Visibility.Visible;
            ShowSelectedUserStoryboard.Begin();
        }

        private void SelectedUserPanelFadeOut()
        {
            selectedUserPanel.Visibility = Visibility.Collapsed;
            HideSelectedUserStoryboard.Begin();
        }
    }
}
