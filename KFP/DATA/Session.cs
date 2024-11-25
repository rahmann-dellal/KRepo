using System;

namespace KFP.DATA
{
    public class Session : ModelBase
    {
        public int SessionId { get; set; }
        public DateTime Start { get; set; }

        private DateTime? _end;
        public DateTime? End
        {
            get
            {
                return _end;
            }
            set
            {
                SetProperty(ref  _end, value);
                OnPropertyChanged(nameof(isSessionActive));
            }
        }

        public bool isSessionActive
        {
            get
            {
                return End == null || End <= DateTime.Now;
            }
        }
        public AppUser appUser { get; set; }

        public Session() {
            
        }
        public Session(AppUser usr)
        {
            Start = DateTime.Now;
            appUser = usr;
        }
    }
}
