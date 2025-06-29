using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.ViewModels
{
    public class SelectableCommand : ObservableObject
    {
        public List<SelectableCommand> ContainerList { get; set; }
        public RelayCommand Command { get; private set; }
        public bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    if (value)
                    {
                        ContainerList?.ForEach(item =>
                        {
                            if (item != this)
                                item.IsSelected = false;
                        });
                    }

                    Command.NotifyCanExecuteChanged();
                }
            }
        }



        public SelectableCommand(Action action, List<SelectableCommand> containerList)
        {
            Command = new RelayCommand(() => {
                action.Invoke();
                IsSelected = true;
            }, () => IsSelected == false);
            ContainerList = containerList;
        }
    }
}
