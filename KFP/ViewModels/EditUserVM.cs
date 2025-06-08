using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Messages;
using KFP.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KFP.ViewModels
{
    public class EditUserVM : ObservableObject
    {
        public enum UserPageMode { Edition, Addtition}

        public UserPageMode _pageMode;

        private AppUser _user;
        private NavigationService _navigationService;
        private KFPContext _dbContext;
        private SessionManager _sessionManager;
        private AppDataService _appDataService;

        //used in case the reset button is clicked and the previous value will be retreived
        public string OldUserName = "";
        public UserRole OldRole;
        public int OldAvatarCode;
        public string OldPINHash;

        public AppUser User
        {
            get
            {
                return _user;

            }
            set
            {
                SetProperty(ref _user, value);
                if(_pageMode == UserPageMode.Edition)
                { 
                    _navigationService.SetNewHeader(StringLocalisationService.getStringWithKey("Editing") + _user.UserName);
                }
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
                OldUserName = User.UserName;
                OldRole = User.Role;
                OldAvatarCode = User.avatarCode;
                OldPINHash = User.PINHash;
            }
        }

        public EditUserVM(KFPContext dbcontext, NavigationService ns, SessionManager sessionManager, AppDataService appDataService)
        {
            _dbContext = dbcontext;
            _navigationService = ns;
            _sessionManager = sessionManager;
            _appDataService = appDataService;
        }

        public bool UserhasPrivilegesOf(UserRole role)
        {
            return _sessionManager.LoggedInUser.HasPrivelegesOf(role);
        }

        public bool isNotLoggedUser() //user isn't editing it self
        {
            return userID != _sessionManager.LoggedInUser.AppUserID;
        }

        public Boolean UserNameAlreadyExists(string username)
        {
            if(this._pageMode== UserPageMode.Addtition &&
                _dbContext.AppUsers.Where(u => u.UserName == username).Any())
            {
                return true;
            } else if (this._pageMode == UserPageMode.Edition)
            {
                return _dbContext.AppUsers.Where(u => u.UserName == username && u.AppUserID != this.userID).Any();
            }
            return false;
        }
        public async Task<int> saveUserToDBAsync()
        {
            
            if (_pageMode == UserPageMode.Edition)
            {
                _dbContext.Entry(User).State = EntityState.Modified;
            }
            else
            {
                _dbContext.AppUsers.Add(User);
            }
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                // if this is the first time a user is added or modified, remove the default user login flag
                if (_appDataService.DefaultUserLogin == true)
                {
                    _appDataService.DefaultUserLogin = false;
                }
                StrongReferenceMessenger.Default.Send(new UserAddedMessage(User.AppUserID));
            }
            return result;
        }
    }
}
