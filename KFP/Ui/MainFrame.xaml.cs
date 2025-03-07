using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using KFP.DATA;
using KFP.Messages;
using KFP.Services;
using KFP.Ui.pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui
{
    public sealed partial class MainFrame : UserControl
    {
        private NavigationViewItem? _selectedNVI;
        private SessionManager _sessionManager;
        public NavigationViewItem? selectedNVI {
            private get{
                return _selectedNVI;
            }
            set
            {
                _selectedNVI = value;
                NavView.SelectedItem = _selectedNVI;
            }
        }
        public MainFrame()
        {
            this.InitializeComponent();
            WeakReferenceMessenger.Default.Register<UserAddedMessage>(this, (r, m) => OnUserAdded(m.UserId));
            _sessionManager = Ioc.Default.GetService<SessionManager>();
        }

        private void NavView_ItemInvoked(NavigationView sender,
                                         NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItem InvokedNVI = (NavigationViewItem)args.InvokedItemContainer;
            if (InvokedNVI != selectedNVI)
            {
                if (InvokedNVI == POSNVI)
                {
                    selectedNVI = POSNVI;
                    ContentFrame.Navigate(typeof(POSPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("POS");
                }
                else if (InvokedNVI == ListUsersNVI && isAllowedRole(UserRole.Manager))
                {
                    selectedNVI = ListUsersNVI;
                    ContentFrame.Navigate(typeof(UserListPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("User_List");
                }
                else if (InvokedNVI == AddUserNVI && isAllowedRole(UserRole.Manager))
                {
                    selectedNVI = AddUserNVI;
                    ContentFrame.Navigate(typeof(EditUserPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Add_User");
                }
                else if (InvokedNVI == SessionNVI)
                {
                    selectedNVI = SessionNVI;
                    ContentFrame.Navigate(typeof(SessionPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Session");
                }
                else if (InvokedNVI == AboutNVI)
                {
                    selectedNVI = AboutNVI;
                    ContentFrame.Navigate(typeof(AboutPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("About");
                }
                else if(args.IsSettingsInvoked && isAllowedRole(UserRole.Manager))
                {
                    selectedNVI = null;
                    ContentFrame.Navigate(typeof(SettingsPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Settings");
                }
            }
        }


        private bool isAllowedRole(UserRole role)
        {
            return _sessionManager.LoggedInUser.IsAllowedRole(role);
        }
        private Visibility isVisibleForRole(UserRole role)
        {
            if (isAllowedRole(role))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        private void OnUserAdded(int userId)
        {
            ContentFrame.Navigate(typeof(DisplayUserPage), new List<object>
                    {
                       userId
                    });
            NavView.SelectedItem = null;
            selectedNVI = null;
        }
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.PaneTitle = StringLocalisationService.getStringWithKey("Menu");
        }
    }
}
