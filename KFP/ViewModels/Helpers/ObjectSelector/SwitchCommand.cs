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
        public List<SwitchCommand>? ContainerList { get; set; }
        public RelayCommand Command { get; private set; }
        private bool? _isSwitched = false;
        public bool? IsSwitched
        {
            get => _isSwitched;
            set
            {
                if (SetProperty(ref _isSwitched, value))
                {
                        ContainerList?.ForEach(item =>
                        {
                            if (item != this) { 
                                item._isSwitched = null;
                                item.OnPropertyChanged(nameof(IsSwitched));
                            }
                        });
                }
            }
        }
        public SwitchCommand(Action action, List<SwitchCommand>? containerList)
        {
            Command = new RelayCommand(() =>
                {
                    if(IsSwitched == null)
                        IsSwitched = false; // Set to false if it was null
                    else
                        IsSwitched = !IsSwitched; // Toggle the sorting order
                    action.Invoke();
                }
            );
            ContainerList = containerList;
        }
    }
}
