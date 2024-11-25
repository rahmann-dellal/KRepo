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
using KFP.Helpers;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.Components
{
    public sealed partial class PINInput : UserControl
    {
        private int[] pin = new int[6];
        private int pinLength = 0;
        public string PIN { get; private set; }

        public string PinHash
        {
            get
            {
                return PasswordHasher.HashPassword(PIN);
            }
        }
        public bool isPinValid { get; private set; }

        //PinPad displays 6 black dots but isn't storing anything
        public bool IsSleeping { get; set; }
        public bool isPinEmpty { get { return pinLength == 0; } }

        public event EventHandler PinChanged;

        private Border[] PinBlockes = new Border[6];
        private bool isPinPadFocused;

        //text that prompts the user to enter the pin code
        public static readonly DependencyProperty PromptTextProperty = DependencyProperty.Register(
            "PromptText", typeof(string), typeof(PINInput), null);
        public string PromptText
        {
            get => (string)GetValue(PromptTextProperty);
            set
            {
                SetValue(PromptTextProperty, (string)value);
                promptTextBlock.Text = (string)value;
            }
        }

        public static readonly DependencyProperty PinIsValidTextProperty = DependencyProperty.Register(
            "PinIsValidText", typeof(string), typeof(PINInput), null);

        public string PinIsValidText
        {
            get => (string)GetValue(PinIsValidTextProperty);
            set
            {
                SetValue(PinIsValidTextProperty, (string)value);
                pinIsValidBlock.Text = (string)value;
            }
        }

        public static readonly DependencyProperty ShowPinProperty = DependencyProperty.Register(
            "ShowPinProperty", typeof(bool), typeof(PINInput), new PropertyMetadata(true));

        public bool ShowPin
        {
            get => (bool)GetValue(ShowPinProperty);
            set
            {
                SetValue(ShowPinProperty, (bool)value);
            }
        }
        public PINInput()
        {
            this.InitializeComponent();

            for (int i = 0; i < PinBlockes.Length; i++)
            {
                PinBlockes[i] = new Border();
                PinBlockes[i].BorderThickness = new Thickness(1);
                PinBlockes[i].BorderBrush = new SolidColorBrush(Colors.DarkGray);
                PinBlockes[i].CornerRadius = new CornerRadius(3);
                PinBlockes[i].Width = 35;
                PinBlockes[i].Height = 38;
                TextBlock txtBlock = new TextBlock();
                txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
                txtBlock.VerticalAlignment = VerticalAlignment.Center;
                txtBlock.FontSize = 30;
                PinBlockes[i].Child = txtBlock;
                pinStackPanel.Children.Add(PinBlockes[i]);
            }
            promptTextBlock.Text = PromptText;
            pinIsValidBlock.Text = PinIsValidText;


            b0.Click += (o, e) => AddToPin(0);
            b1.Click += (o, e) => AddToPin(1);
            b2.Click += (o, e) => AddToPin(2);
            b3.Click += (o, e) => AddToPin(3);
            b4.Click += (o, e) => AddToPin(4);
            b5.Click += (o, e) => AddToPin(5);
            b6.Click += (o, e) => AddToPin(6);
            b7.Click += (o, e) => AddToPin(7);
            b8.Click += (o, e) => AddToPin(8);
            b9.Click += (o, e) => AddToPin(9);

            bBack.Click += (o, e) => RemoveFromPin();
            bReset.Click += (o, e) => resetPin();

            this.KeyDown += ClockInFrame_KeyDown;
        }

        private void AddToPin(int number)
        {
            if (pinLength <= 5)
            {
                IsSleeping = false;
                pin[pinLength] = number;
                pinLength++;
                repopulatePinBoxes();
                PinChanged?.Invoke(this, new EventArgs());
            }
        }

        private void RemoveFromPin()
        {
            if (pinLength > 0)
            {
                IsSleeping = false;
                pinLength--;
                repopulatePinBoxes();
                PinChanged?.Invoke(this, new EventArgs());
            }
        }

        public void resetPin()
        {
            IsSleeping = false;
            pinLength = 0;
            repopulatePinBoxes();
            PinChanged?.Invoke(this, new EventArgs());
        }

        public void sleep()
        {
            pinLength = 0;
            IsSleeping = true;
            isPinValid = false;
            promptTextButton.Visibility = Visibility.Visible;
            pinIsValidButton.Visibility = Visibility.Collapsed;

            for (int i = 0; i < 6; i++)
            {
                ((TextBlock)PinBlockes[i].Child).Text = "\u25CF";
            }
        }

        private void repopulatePinBoxes()
        {
            if (pinLength == 6)
            {
                isPinValid = true;
                promptTextButton.Visibility = Visibility.Collapsed;
                pinIsValidButton.Visibility = Visibility.Visible;
            }
            else
            {
                isPinValid = false;
                promptTextButton.Visibility = Visibility.Visible;
                pinIsValidButton.Visibility = Visibility.Collapsed;
            }
            for (int i = 6; i > pinLength; i--)
            {
                ((TextBlock)PinBlockes[i - 1].Child).Text = "";
            }
            for (int i = 0; i < pinLength; i++)
            {
                if (ShowPin)
                    ((TextBlock)PinBlockes[i].Child).Text = pin[i] + "";
                else
                    ((TextBlock)PinBlockes[i].Child).Text = "\u25CF";
            }
            if (pinLength < 6 && isPinPadFocused)
            {
                ((TextBlock)PinBlockes[pinLength].Child).Text = "_";
            }

            PIN = "" + pin[0] + "" + pin[1] + "" + pin[2] + "" + pin[3] + "" + pin[4] + "" + pin[5];
        }


        //intercept key strokes
        private void ClockInFrame_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Back)
            {
                RemoveFromPin();
            }
            //intercept key strokes of numbers from 0 to 9
            else if (e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9)
            {
                int number = (int)e.Key - (int)Windows.System.VirtualKey.Number0;
                AddToPin(number);
            }
        }



        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            isPinPadFocused = false;
            repopulatePinBoxes();
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            isPinPadFocused = true;
            repopulatePinBoxes();
        }

        public void FadeOut()
        {
            ExitStoryboard.Begin();
        }

        public void FadeIn()
        {
            EnterStoryboard.Begin();
        }

    }
}
