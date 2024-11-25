using CommunityToolkit.Mvvm.ComponentModel;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Services
{
    public class SessionManager : ObservableObject
    {
        private KFPContext _dbContext;
        private Session _currentSession;

        public AppUser LoggedInUser
        {
            get
            {
                return CurrentSession?.appUser;
            }
        }

        public Session CurrentSession
        {
            get
            {
                return _currentSession;
            }
            private set
            {
                SetProperty(ref _currentSession, value);
                OnPropertyChanged(nameof(isSessionActive));
            }
        }
        public bool isSessionActive
        {
            get
            {
                if (CurrentSession == null)
                {
                    return false;
                }
                else
                {
                    return CurrentSession.isSessionActive;
                }
            }
        }

        public SessionManager(KFPContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<bool> tryLogin(AppUser user, string pin)
        {
            if (isPinCorrect(user, pin))
            {
                StartSession(user);
                return true;
            }
            else
                return false;
        }
        private bool isPinCorrect(int userID, string pin)
        {
            AppUser user = _dbContext.AppUsers.Find(userID);
            return isPinCorrect(user, pin);
        }
        private bool isPinCorrect(AppUser user, string pin)
        {
            if (PasswordHasher.verifyPassword(pin, user.PINHash))
                return true;
            else return false;
        }
        private void StartSession(AppUser user)
        {
            CurrentSession = new Session(user);
            CurrentSession.PropertyChanged += (o, e) =>
            {
                this.OnPropertyChanged(nameof(CurrentSession));
                this.OnPropertyChanged(nameof(isSessionActive));
            };

        }

        public async Task EndSession()
        {
            //store the session in the database for future reference
            if (CurrentSession.End == null)
            {
                CurrentSession.End = DateTime.Now;
            }
            try
            {
                _dbContext.Add<Session>(CurrentSession);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            CurrentSession = null;
        }

    }
}
