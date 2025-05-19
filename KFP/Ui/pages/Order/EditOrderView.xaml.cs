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
using KFP.ViewModels;
using KFP.DATA;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    public sealed partial class EditOrderView : UserControl
    {

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(EditOrderVM),
                typeof(EditOrderView),
                new PropertyMetadata(null));

        public EditOrderVM ViewModel
        {
            get => (EditOrderVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public EditOrderView()
        {
            this.InitializeComponent();
        }

        private void OrderView_Loaded(object sender, RoutedEventArgs e)
        {
            //Callback to bring into view the added Item
            if (ViewModel != null)
            {
                ViewModel.onAddedAddonDelegate = (OrderItemListElement element) =>
                {
                    BringIntoView(element);
                };
                ViewModel.OrderItemElements.CollectionChanged += (s, args) =>
                {
                    if (args.NewItems != null && args.NewItems.Count > 0)
                    {
                        OrderItemListElement? addedItem = args.NewItems[0] as OrderItemListElement;
                        BringIntoView(addedItem);
                    }
                };
            }
        }

        //Bring into view the added Item
        private async void BringIntoView(OrderItemListElement? element)
        {
            if (element != null)
            {
                await Task.Delay(50);
                if (element.ParentOrderItemElement != null)
                {
                    var parentContainer = OrderTreeView.ContainerFromItem(element.ParentOrderItemElement) as TreeViewItem;
                    if (parentContainer != null)
                        parentContainer.IsExpanded = true;
                }
                var container = OrderTreeView.ContainerFromItem(element) as TreeViewItem;
                container?.StartBringIntoView();
            }
        }
    }
}
