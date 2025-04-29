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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    public sealed partial class OrderView : UserControl
    {
        public static readonly DependencyProperty AllowEditingProperty =
            DependencyProperty.Register(
                nameof(AllowEditing),
                typeof(bool),
                typeof(OrderView),
                new PropertyMetadata(false));

        public bool AllowEditing
        {
            get => (bool)GetValue(AllowEditingProperty);
            set => SetValue(AllowEditingProperty, value);
        }

        public OrderView()
        {
            this.InitializeComponent();
        }
    }
}
