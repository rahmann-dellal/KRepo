using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Collections.Specialized.BitVector32;

namespace KFP.ViewModels
{
    public class PageCommand : ObservableObject
    {
        public int PageNumber { get; }
        public List<PageCommand> ContainerList { get; set; }
        public RelayCommand Command { get; protected set; }
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
                            if (item != this) { 
                                item.IsSelected = false;
                                item.OnPropertyChanged(nameof(IsSelected));
                            }
                        });
                    }

                    Command.NotifyCanExecuteChanged();
                }
            }
        }

        public PageCommand(int pageNumber, Action execute, List<PageCommand> containerList) 
        {
            Command = new RelayCommand(() => {
                execute.Invoke();
                IsSelected = true;
            }, () => IsSelected == false);
            ContainerList = containerList;
            PageNumber = pageNumber;
        }
    }
}
