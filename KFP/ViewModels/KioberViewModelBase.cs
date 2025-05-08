using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA_Access;
using KFP.Helpers;
using KFP.Services;
using Microsoft.EntityFrameworkCore;

namespace KFP.ViewModels
{
    public class KioberViewModelBase : ObservableObject
    {
        protected KioberViewModelBase()
        {
        }

        // Removed the override keyword as the base method is not marked as virtual, abstract, or override
        //public new void OnPropertyChanged(string propertyName)
        //{
        //    base.OnPropertyChanged(propertyName);
        //}
    }
}
