using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KFP.ViewModels
{
    public class PageCommand : SelectableCommand
    {
        public int PageNumber { get; }

        public PageCommand(int pageNumber, Action execute, List<SelectableCommand> containerList) : base(execute, containerList)
        {
            PageNumber = pageNumber;
        }
    }
}
