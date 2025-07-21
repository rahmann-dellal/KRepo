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
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.Components
{
    public sealed partial class DateRangePicker : UserControl
    {
        public DateRangePicker()
        {
            this.InitializeComponent();
            BuildQuickFilters();
        }

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register(nameof(StartDate), typeof(DateTimeOffset), typeof(DateRangePicker), new PropertyMetadata(DateTimeOffset.Now.AddDays(-2)));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register(nameof(EndDate), typeof(DateTimeOffset), typeof(DateRangePicker), new PropertyMetadata(DateTimeOffset.Now));

        public static readonly DependencyProperty ApplyRangeCommandProperty =
            DependencyProperty.Register(nameof(ApplyRangeCommand), typeof(ICommand), typeof(DateRangePicker), new PropertyMetadata(null));

        public DateTimeOffset StartDate
        {
            get => (DateTimeOffset)GetValue(StartDateProperty);
            set => SetValue(StartDateProperty, value);
        }

        public DateTimeOffset EndDate
        {
            get => (DateTimeOffset)GetValue(EndDateProperty);
            set => SetValue(EndDateProperty, value);
        }

        public ICommand ApplyRangeCommand
        {
            get => (ICommand)GetValue(ApplyRangeCommandProperty);
            set => SetValue(ApplyRangeCommandProperty, value);
        }

        // Individual filter properties
        public SelectableCommand AllTimeFilter { get; private set; }
        public SelectableCommand TwoDaysFilter { get; private set; }
        public SelectableCommand LastWeekFilter { get; private set; }
        public SelectableCommand LastMonthFilter { get; private set; }

        private void BuildQuickFilters()
        {
            var filters = new List<SelectableCommand>();

            AllTimeFilter = new SelectableCommand(() =>
            {
                StartDate = new DateTimeOffset(2025,3,1,0,0,0,0,new TimeSpan(0));
                EndDate = DateTimeOffset.Now;
                ApplyRangeCommand?.Execute((StartDate, EndDate));
            }, filters);

            TwoDaysFilter = new SelectableCommand(() =>
            {
                StartDate = DateTimeOffset.Now.AddDays(-2);
                EndDate = DateTimeOffset.Now;
                ApplyRangeCommand?.Execute((StartDate, EndDate));
            }, filters);

            LastWeekFilter = new SelectableCommand(() =>
            {
                StartDate = DateTimeOffset.Now.AddDays(-7);
                EndDate = DateTimeOffset.Now;
                ApplyRangeCommand?.Execute((StartDate, EndDate));
            }, filters);

            LastMonthFilter = new SelectableCommand(() =>
            {
                StartDate = DateTimeOffset.Now.AddMonths(-1);
                EndDate = DateTimeOffset.Now;
                ApplyRangeCommand?.Execute((StartDate, EndDate));
            }, filters);

            // Add all to the same container list
            filters.AddRange(new[] { AllTimeFilter, TwoDaysFilter, LastWeekFilter, LastMonthFilter });
            TwoDaysFilter.IsSelected = true; // Default selection
        }

        private void OnCustomDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            bool DateChanged = false; // to Ensure that date change is not done programmatically

            // Unselect all quick filters
            AllTimeFilter.IsSelected = false;
            TwoDaysFilter.IsSelected = false;
            LastWeekFilter.IsSelected = false;
            LastMonthFilter.IsSelected = false;
            if(sender is DatePicker datePicker) // Check if sender is a DatePicker
            { // Update StartDate or EndDate because the methode might execute before the values are updated
                if (datePicker.Name == nameof(StartDateDatePicker))
                {
                    if (e.NewDate != StartDate)
                    {
                        StartDate = e.NewDate;
                        DateChanged = true;
                    }
                }
                else if (datePicker.Name == nameof(EndDateDatePicker))
                {
                    if (e.NewDate != EndDate)
                    {
                        EndDate = e.NewDate;
                        DateChanged = true;
                    }
                }
            }
            // If either date changed, execute the command
            if (DateChanged) { 
                ApplyRangeCommand?.Execute((StartDate, EndDate));
            }
        }
    }
}
