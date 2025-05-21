using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.Components
{
    public delegate void CharacterPressedEventHandler(object sender, NumberPad.CharacterEventArgs e);

    public sealed partial class NumberPad : UserControl
    {
        public event EventHandler BackSpacePressed;
        public event CharacterPressedEventHandler buttonPressed;

        public NumberPad()
        {
            this.InitializeComponent();
            b0.Click += (s, e) => OnButtonPressed("0");
            b1.Click += (s, e) => OnButtonPressed("1");
            b2.Click += (s, e) => OnButtonPressed("2");
            b3.Click += (s, e) => OnButtonPressed("3");
            b4.Click += (s, e) => OnButtonPressed("4");
            b5.Click += (s, e) => OnButtonPressed("5");
            b6.Click += (s, e) => OnButtonPressed("6");
            b7.Click += (s, e) => OnButtonPressed("7");
            b8.Click += (s, e) => OnButtonPressed("8");
            b9.Click += (s, e) => OnButtonPressed("9");
            bDot.Click += (s, e) => OnButtonPressed(".");

            this.KeyDown += NumberPad_KeyDown;
        }

        private void OnBackSpaceButtonClicked(object sender, RoutedEventArgs e)
        {
            BackSpacePressed?.Invoke(this, EventArgs.Empty);
        }

        private void OnButtonPressed(string character)
        {
            buttonPressed?.Invoke(this, new CharacterEventArgs(character));
        }

        public class CharacterEventArgs : EventArgs
        {
            public string Character { get; }

            public CharacterEventArgs(string character)
            {
                Character = character;
            }
        }

        public void FadeOut()
        {
            ExitStoryboard.Begin();
        }

        public void FadeIn()
        {
            EnterStoryboard.Begin();
        }

        private void NumberPad_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Back)
            {
                OnBackSpaceButtonClicked(this, new RoutedEventArgs());
            }
            //intercept key strokes of numbers from 0 to 9
            else if (e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9)
            {
                int number = (int)e.Key - (int)Windows.System.VirtualKey.Number0;
                OnButtonPressed(number+"");
            }
            else if (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9)
            {
                int number = (int)e.Key - (int)Windows.System.VirtualKey.NumberPad0;
                OnButtonPressed(number+"");
            }
            else if (e.Key == Windows.System.VirtualKey.Decimal || e.Key == Windows.System.VirtualKey.Separator
                || ((int)e.Key) == 190 || ((int)e.Key) == 188)
            {
                OnButtonPressed(".");
            }

        }
    }
}
