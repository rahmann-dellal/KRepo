using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using KFP.DATA;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    public sealed partial class DisplayOrderView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(order),
                typeof(KFP.DATA.Order),
                typeof(DisplayOrderView),
                new PropertyMetadata(null));

        public KFP.DATA.Order order
        {
            get => (KFP.DATA.Order)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        public DisplayOrderView()
        {
            this.InitializeComponent();
        }

        public bool isListEmpty => order == null || order.OrderItems == null || order.OrderItems.Count == 0;
    }
}
