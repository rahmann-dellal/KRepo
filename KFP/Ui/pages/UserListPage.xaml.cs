using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
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
using System.Collections.ObjectModel;
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
    public sealed partial class UserListPage : Page
    {
        public ObservableCollection<UserListElement> UserListElements { get; set; } = new ObservableCollection<UserListElement>();

        private KFPContext _dbContext;
        private NavigationService _navigationService;
        private SessionManager _sessionManager;
        public UserListPage()
        {
            _dbContext = Ioc.Default.GetService<KFPContext>();
            _navigationService = Ioc.Default.GetService<NavigationService>();
            _sessionManager = Ioc.Default.GetService<SessionManager>();

            foreach (var user in _dbContext.AppUsers)
            {
                UserListElements.Add(new UserListElement(user, UserListElements, _dbContext, _sessionManager, _navigationService));
            }
            this.InitializeComponent();
        }
    }

    public partial class UserListElement
    {
        private NavigationService _navigationService { get; set; }
        private KFPContext _dbContext;
        private SessionManager _sessionService;
        private ObservableCollection<UserListElement> _userListElements;
        public UserListElement(AppUser user, ObservableCollection<UserListElement> userListElements, KFPContext dbContext, SessionManager sessionService, NavigationService navigationService)
        {
            _dbContext = dbContext;
            _sessionService = sessionService;
            User = user;
            _userListElements = userListElements;
            _navigationService = navigationService;
        }

        public bool IsConnected
        {
            get
            {
                return _sessionService.LoggedInUser.AppUserID == User.AppUserID;
            }
        }

        public AppUser User { get; set; }
        [RelayCommand(CanExecute = nameof(DeleteUserCanExcute))]
        public void DeleteUser()
        {
            _dbContext.AppUsers.Remove(User);
            _dbContext.SaveChanges();
            _userListElements.Remove(this);
        }

        public bool DeleteUserCanExcute()
        {
            if (this.IsConnected)
            {
                return false;
            }
            else if (_sessionService.LoggedInUser.Role < User.Role)
            {
                return false;
            }
            else if (_sessionService.LoggedInUser.Role != UserRole.Admin && _sessionService.LoggedInUser.Role == User.Role)
            {
                return false;
            }
            else
                return true;
        }

        [RelayCommand (CanExecute = nameof(GoToEditUserCanExecute))]
        public void GoToEditUser()
        {
            _navigationService.navigateTo(typeof(EditUserPage), new List<object>() { User.AppUserID });
        }

        public bool GoToEditUserCanExecute()
        {
            if (this.IsConnected)
            {
                return true;
            }
            else if (_sessionService.LoggedInUser.Role < User.Role)
            {
                return false;
            }
            else if (_sessionService.LoggedInUser.Role != UserRole.Admin && _sessionService.LoggedInUser.Role == User.Role)
            {
                return false;
            }
            else
                return true;
        }
    }
}
