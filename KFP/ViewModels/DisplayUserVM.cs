using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using KFP.Ui.pages;
using System;
using System.Collections.Generic;

namespace KFP.ViewModels
{
    public partial class DisplayUserVM : ObservableObject
    {
        private AppUser _user;
        private NavigationService _navigationService;
        private KFPContext _dbContext;
        private SessionManager _sessionService;
        public AppUser User { 
            get
            {
                return _user;

            } 
            set
            {
                SetProperty(ref _user, value);
                _navigationService.SetNewHeader(_user.UserName);
            }
        }

        public int userID
        {
            get
            {
                return User.AppUserID;
            }
            set
            {
                User = _dbContext.AppUsers.Find(value);
            }
        }

        public DisplayUserVM(KFPContext dbcontext, NavigationService ns, SessionManager sessionService)
        {
            _dbContext = dbcontext;
            _navigationService = ns;
            _sessionService = sessionService;
        }

        [RelayCommand(CanExecute =nameof(canEditUser))]
        public void goToEditUser()
        {
            if (_user != null)
            {
                _navigationService.navigateTo(typeof(EditUserPage), new List<Object> { User.AppUserID });
            }
        }

        public bool canEditUser()
        {
            if (_sessionService.LoggedInUser.AppUserID == User.AppUserID)
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

        [RelayCommand(CanExecute = nameof(canDeleteUser))]
        public void DeleteUser()
        {
            _dbContext.AppUsers.Remove(User);
            _dbContext.SaveChanges();
            _navigationService.navigateTo(typeof(UserListPage), null); //UserListPage should not be accessible from the view model but we will let this one slide ;)
        }

        public bool canDeleteUser()
        {
            if (_sessionService.LoggedInUser.AppUserID == User.AppUserID)
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
    }
}
