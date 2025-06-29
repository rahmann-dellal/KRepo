using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.ViewModels.Helpers.ObjectSelector
{
    public class SwitchCommand : ObservableObject
    {
        public RelayCommand Command { get; private set; }
        private bool _isSwitched = false;
        public bool IsSwitched
        {
            get => _isSwitched;
            set
            {
                SetProperty(ref _isSwitched, value);
            }
        }
        public SwitchCommand(Action action)
        {
            Command = new RelayCommand(() =>
                {
                    action.Invoke();
                    IsSwitched = !IsSwitched; // Toggle the sorting order
                }
            );
        }
    }
}
