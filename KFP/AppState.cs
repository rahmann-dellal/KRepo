using CommunityToolkit.Mvvm.ComponentModel;
using KFP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP
{
    public class AppState : ObservableObject
    {
        private SessionManager _sessionManager;
        public bool isSessionActive
        {
            get
            {
                return _sessionManager.isSessionActive;
            }
        }

        public AppState(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }
    }
}
