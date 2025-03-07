using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Messages;
using KFP.Services;
using Microsoft.EntityFrameworkCore;
using System;
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
                    _navigationService.SetNewHeader(_user.UserName);
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

        public EditUserVM(KFPContext dbcontext, NavigationService ns)
        {
            _dbContext = dbcontext;
            _navigationService = ns;
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
                WeakReferenceMessenger.Default.Send(new UserAddedMessage(User.AppUserID));
            }
            return result;
        }
    }
}
