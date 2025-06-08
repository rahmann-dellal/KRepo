using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using KFP.DATA;
using KFP.Messages;
using KFP.Services;
using KFP.Ui.pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui
{
    public sealed partial class MainFrame : UserControl
    {
        private NavigationViewItem? _selectedNVI;
        private SessionManager _sessionManager;
        private AppState _appState;
        public bool EstablishmentHasTables { get; set; } = true;

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
            StrongReferenceMessenger.Default.Register<UserAddedMessage>(this, (r, m) => OnUserAdded(m.UserId));
            _sessionManager = Ioc.Default.GetService<SessionManager>()!;
            _appState = Ioc.Default.GetService<AppState>()!;
            _appState.PropertyChanged += (s,e) => { 
                EstablishmentHasTables = _appState.EstablishmentHasTables; 
                if(EstablishmentHasTables)
                {
                    TablesNVI.Visibility = Visibility.Visible;
                }
                else
                {
                    TablesNVI.Visibility = Visibility.Collapsed;
                }
            };

            selectedNVI = POSNVI;
            ContentFrame.Navigate(typeof(PointOfSalesPage));
            NavView.Header = null;//StringLocalisationService.getStringWithKey("POS");
        }
        ~MainFrame()
        {
            StrongReferenceMessenger.Default.Unregister<UserAddedMessage>(this);
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
                    ContentFrame.Navigate(typeof(PointOfSalesPage));
                    NavView.Header = null;//StringLocalisationService.getStringWithKey("POS");
                }
                else if (InvokedNVI == TablesNVI)
                {
                    selectedNVI = TablesNVI;
                    ContentFrame.Navigate(typeof(TablesPage));
                    NavView.Header = null;
                }
                else if (InvokedNVI == OrdersNVI)
                {
                    selectedNVI = OrdersNVI;
                    ContentFrame.Navigate(typeof(OrdersListPage));
                    NavView.Header = null;
                }
                else if (InvokedNVI == ListMenuItemsNVI && UserHasPrivelegesOf(UserRole.Manager))
                {
                    selectedNVI = ListMenuItemsNVI;
                    ContentFrame.Navigate(typeof(MenuItemListPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Menu_items_list");
                }
                else if (InvokedNVI == AddMenuItemNVI && UserHasPrivelegesOf(UserRole.Manager))
                {
                    selectedNVI = AddMenuItemNVI;
                    ContentFrame.Navigate(typeof(EditMenuItemPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Add_menu_item");
                }
                else if (InvokedNVI == CategoriesNVI && UserHasPrivelegesOf(UserRole.Manager))
                {
                    selectedNVI = CategoriesNVI;
                    ContentFrame.Navigate(typeof(CategoriesPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Menu_Item_Categories");
                }
                else if (InvokedNVI == ListUsersNVI && UserHasPrivelegesOf(UserRole.Manager))
                {
                    selectedNVI = ListUsersNVI;
                    ContentFrame.Navigate(typeof(UserListPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("User_List");
                }
                else if (InvokedNVI == AddUserNVI && UserHasPrivelegesOf(UserRole.Manager))
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
                else if (InvokedNVI == AccountNVI)
                {
                    selectedNVI = AccountNVI;
                    ContentFrame.Navigate(typeof(AccountPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Account");
                }
                else if (InvokedNVI == AboutNVI)
                {
                    selectedNVI = AboutNVI;
                    ContentFrame.Navigate(typeof(AboutPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("About");
                }
                else if(args.IsSettingsInvoked && UserHasPrivelegesOf(UserRole.Manager))
                {
                    selectedNVI = null;
                    ContentFrame.Navigate(typeof(SettingsPage));
                    NavView.Header = StringLocalisationService.getStringWithKey("Settings");
                }
            }
        }


        private bool UserHasPrivelegesOf(UserRole role)
        {
            return _sessionManager.LoggedInUser.HasPrivelegesOf(role);
        }
        private Visibility isVisibleForRole(UserRole role)
        {
            if (UserHasPrivelegesOf(role))
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
