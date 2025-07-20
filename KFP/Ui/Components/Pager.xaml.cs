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
using CommunityToolkit.Mvvm.Input;
using KFP.ViewModels;
using System.Windows.Input;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.Components
{
    public sealed partial class Pager : UserControl
    {
        public Pager()
        {
            PreviousPageCommand = new RelayCommand(() =>
            {
                if (SelectedPage > 1)
                    SelectPageCommand?.Execute(SelectedPage - 1);
            }, () => SelectedPage > 1);

            NextPageCommand = new RelayCommand(() =>
            {
                if (SelectedPage < TotalPages)
                    SelectPageCommand?.Execute(SelectedPage + 1);
            }, () => SelectedPage < TotalPages);


            this.InitializeComponent();
            BuildPageCommands();
            UpdateNavigation();
        }

        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(Pager),
                new PropertyMetadata(1, OnTotalPagesChanged));

        public static readonly DependencyProperty SelectedPageProperty =
            DependencyProperty.Register(nameof(SelectedPage), typeof(int), typeof(Pager),
                new PropertyMetadata(1, OnSelectedPageChanged));

        public static readonly DependencyProperty SelectPageCommandProperty =
            DependencyProperty.Register(nameof(SelectPageCommand), typeof(ICommand), typeof(Pager),
                new PropertyMetadata(null));

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        public int SelectedPage
        {
            get => (int)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }

        public ICommand SelectPageCommand
        {
            get => (ICommand)GetValue(SelectPageCommandProperty);
            set => SetValue(SelectPageCommandProperty, value);
        }

        public ObservableCollection<PageCommand> PageCommands { get; private set; } = new();

        public RelayCommand PreviousPageCommand { get; private set; }
        public RelayCommand NextPageCommand { get; private set; }

        private static void OnTotalPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pager = (Pager)d;
            pager.BuildPageCommands();
            pager.UpdateSelectedState();
            pager.UpdateNavigation();
        }

        private static void OnSelectedPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pager = (Pager)d;
            pager.UpdateSelectedState();
            pager.UpdateNavigation();
        }

        private void BuildPageCommands()
        {
            PageCommands.Clear();
            List<PageCommand> commandsList = new List<PageCommand>();

            for (int i = 1; i <= TotalPages; i++)
            {
                int pageNumber = i;
                var cmd = new PageCommand(pageNumber, () =>
                {
                    SelectPageCommand?.Execute(pageNumber);
                }, commandsList);

                PageCommands.Add(cmd);
            }
            commandsList.AddRange(PageCommands.ToList());
        }

        private void UpdateSelectedState()
        {
            if (SelectedPage < 1)
                SelectedPage = 1;
            if (TotalPages > 1 && SelectedPage > TotalPages)
                SelectedPage = TotalPages;
            if (PageCommands.Count == 0)
                return;
            if(SelectedPage > 0 && SelectedPage < PageCommands.Count) { 
                PageCommands[SelectedPage - 1].IsSelected = true;
            }
            //foreach (var cmd in PageCommands) //TODO : edit
            //    cmd.IsSelected = (cmd.PageNumber == SelectedPage);
        }

        private void UpdateNavigation()
        {
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
        }
    }
}
