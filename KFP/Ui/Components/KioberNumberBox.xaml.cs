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

namespace KFP.Ui.Components
{
    public sealed partial class KioberNumberBox : UserControl
    {
        double oldValue = 0;
        public KioberNumberBox()
        {
            this.InitializeComponent();
            this.NumberTextBox.TextChanged += NumberTextBox_TextChanged;
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null && double.TryParse(textBox.Text, out double newValue))
            {
                if(textBox.Text.Contains(","))
                {
                    textBox.Text = textBox.Text.Replace(",", ".");
                }
                Value = newValue;
                oldValue = newValue;
                if (AllowOnlyPositiveValues && newValue < 0)
                {
                    textBox.Text = -newValue + "";
                    Value = -newValue;
                    oldValue = -newValue;
                    return;
                }
                if (AllowOnlyIntgers && newValue % 1 != 0)
                {
                    textBox.Text = Math.Floor(newValue) + "";
                    Value = Math.Floor(newValue);
                    oldValue = Math.Floor(newValue);
                    return;
                }
                if(MaxValue.HasValue && newValue > MaxValue)
                {
                    textBox.Text = MaxValue + "";
                    Value = MaxValue;
                    oldValue = MaxValue.Value;
                    return;
                }
            }
            else if (textBox != null && string.IsNullOrEmpty(textBox.Text))
            {
                Value = 0;
                oldValue = 0;
            }
            else if (textBox != null)
            {
                textBox.Text = oldValue.ToString();
            }
        }


        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double?),
                typeof(KioberNumberBox),
                new PropertyMetadata(null, OnValueChanged));

        public static readonly DependencyProperty AllowOnlyPositiveValuesProperty =
            DependencyProperty.Register(
                nameof(AllowOnlyPositiveValues),
                typeof(bool),
                typeof(KioberNumberBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty AllowOnlyIntgersProperty =
            DependencyProperty.Register(
                nameof(AllowOnlyIntgers),
                typeof(bool),
                typeof(KioberNumberBox),
                new PropertyMetadata(false));
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(double?),
                typeof(KioberNumberBox),
                new PropertyMetadata(null, OnValueChanged));
        public bool AllowOnlyIntgers
        {
            get => (bool)GetValue(AllowOnlyIntgersProperty);
            set => SetValue(AllowOnlyIntgersProperty, value);
        }

        public double? MaxValue
        {
            get => (double?)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set
            {
                if (AllowOnlyPositiveValues && value < 0)
                {
                    value = 0;
                }
                SetValue(ValueProperty, value);
            }
        }

        public bool AllowOnlyPositiveValues
        {
            get => (bool)GetValue(AllowOnlyPositiveValuesProperty);
            set => SetValue(AllowOnlyPositiveValuesProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (KioberNumberBox)d;
            control.NumberTextBox.Text = e.NewValue?.ToString();
        }

        //private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if (!char.IsDigit((char)e.Key) && e.Key != Windows.System.VirtualKey.Back)
        //    {
        //        e.Handled = true;
        //    }
        //}

        private void OnUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (Value.HasValue)
            {
                Value++;
            }
            else
            {
                Value = 0;
            }
        }

        private void OnDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (Value.HasValue)
            {
                if (AllowOnlyPositiveValues && Value <= 0)
                {
                    return;
                }
                Value--;
            }
            else
            {
                Value = 0;
            }
        }
    }
}
