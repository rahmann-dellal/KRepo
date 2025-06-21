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
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Media.Animation;
using KFP.DATA;
using System.Collections.Immutable;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    public sealed partial class MenuItemSelector : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(MenuItemSelectorVM),
                typeof(MenuItemSelector),
                new PropertyMetadata(null));

        public MenuItemSelectorVM ViewModel
        {
            get
            {
                return (MenuItemSelectorVM)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
                ViewModelLoaded();
            }
        }
        public MenuItemSelector()
        {
            var menuItemTypes = Enum.GetValues(typeof(MenuItemType)).Cast<MenuItemType>().ToList();
            this.InitializeComponent();
        }

        private void ViewModelLoaded()
        {
            if (ViewModel != null)
            {
                if (ViewModel.MenuItemTypeFilter == MenuItemType.Main)
                    TypeSelectorBar.SelectedItem = Main;
                else if (ViewModel.MenuItemTypeFilter == MenuItemType.Addon)
                    TypeSelectorBar.SelectedItem = AddOns;
                else if (ViewModel.MenuItemTypeFilter == MenuItemType.Drink)
                    TypeSelectorBar.SelectedItem = Drink;
                else if (ViewModel.MenuItemTypeFilter == MenuItemType.Other)
                    TypeSelectorBar.SelectedItem = Other;
                else
                    TypeSelectorBar.SelectedItem = all;

                ViewModel.LoadAsync().ConfigureAwait(false);
            }
        }

        private static void OnSelectedMenuItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuItemSelector selector && selector.ViewModel != null)
            {
                selector.ViewModel.SelectedMenuItem = e.NewValue as MenuItem;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Create a scale animation for the button
                var scaleTransform = new ScaleTransform();
                button.RenderTransform = scaleTransform;
                button.RenderTransformOrigin = new Point(0.5, 0.5);

                var animation = new Storyboard();

                // Scale up animation
                var scaleUpX = new DoubleAnimation
                {
                    To = 1.08,
                    Duration = new Duration(TimeSpan.FromMilliseconds(40)),
                    AutoReverse = true
                };
                Storyboard.SetTarget(scaleUpX, button);
                Storyboard.SetTargetProperty(scaleUpX, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");

                var scaleUpY = new DoubleAnimation
                {
                    To = 1.08,
                    Duration = new Duration(TimeSpan.FromMilliseconds(40)),
                    AutoReverse = true
                };
                Storyboard.SetTarget(scaleUpY, button);
                Storyboard.SetTargetProperty(scaleUpY, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");

                animation.Children.Add(scaleUpX);
                animation.Children.Add(scaleUpY);

                // Start the animation
                animation.Begin();
            }
        }

        private void TypeSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (sender.SelectedItem == Main)
            {
                ViewModel.MenuItemTypeFilter = MenuItemType.Main;
            }
            else if (sender.SelectedItem == AddOns)
            {
                ViewModel.MenuItemTypeFilter = MenuItemType.Addon;
            }
            else if (sender.SelectedItem == Drink)
            {
                ViewModel.MenuItemTypeFilter = MenuItemType.Drink;
            }
            else if (sender.SelectedItem == Other)
            {
                ViewModel.MenuItemTypeFilter = MenuItemType.Other;
            }
            else
            {
                ViewModel.MenuItemTypeFilter = null;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                List<Category> AddedCategories = e.AddedItems.Cast<Category>().ToList();
                List<Category> RemovedCategories = e.RemovedItems.Cast<Category>().ToList();

                if (AddedCategories.Count > 0)
                {
                    foreach (var cat in AddedCategories)
                    {
                        if (!ViewModel.CategoryFilter.Contains(cat))
                            ViewModel.CategoryFilter.Add(cat);
                    }
                }
                if (RemovedCategories.Count > 0)
                {
                    foreach (var cat in RemovedCategories)
                    {
                        if (ViewModel.CategoryFilter.Contains(cat))
                            ViewModel.CategoryFilter.Remove(cat);
                    }
                }
            }
        }
    }
}
