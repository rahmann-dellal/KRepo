using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA;
using KFP.DATA_Access;
using KFP.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static KFP.Ui.Components.NumberPad;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui
{
    public sealed partial class ClockInFrame : UserControl
    {
        private KFPContext _context;
        private SessionManager _sessionManager;
        private AppDataService _appDataService;
        public List<AppUser> AppUsers { get; private set; }

        private AppUser? _selectedUser;
        private double OpeningCash = 0;
        private bool OpeningCashSet = false;
        private bool DefaultUserLogin
        {
            get
            {
                return _appDataService.DefaultUserLogin;
            }
        }
        private bool showOpeningCashInput { get => !OpeningCashSet; }

        public AppUser? selectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                userContentControl.DataContext = selectedUser;
                if (value != null)
                {
                    selectUserPanel.Visibility = Visibility.Collapsed;
                    SelectedUserPanelFadeIn();
                    pinInput.resetPin();
                    CashNumberPad.Focus(FocusState.Programmatic);

                }
                else
                {
                    SelectedUserPanelFadeOut();
                    selectUserPanel.Visibility = Visibility.Visible;
                }
            }
        }

        public bool isUserSelected
        {
            get
            {
                return selectedUser != null;
            }
        }
        public ClockInFrame()
        {
            _context = Ioc.Default.GetService<KFPContext>();
            _sessionManager = Ioc.Default.GetService<SessionManager>();
            _appDataService = Ioc.Default.GetService<AppDataService>();
            AppUsers = _context.AppUsers.ToList();

            this.InitializeComponent();

            if (AppUsers.Count == 1)
            {
                selectedUser = AppUsers[0];
                UselectUserButton.Width = 0;
                UselectUserButton.Height = 0;
                UselectUserButton.Visibility = Visibility.Collapsed;
            }

            var lastSession = _context.Sessions.OrderByDescending(s => s.End)
                .FirstOrDefault();
            if (lastSession != null)
            {
                OpeningCash = lastSession.ClosingCash;
            } else
            {
                OpeningCash = 0;
            }
            numberBox.Value = OpeningCash;

            cancelAndExitButton.Content = StringLocalisationService.getStringWithKey("ClockInFrame_cancelAndExit");
            pinInput.PromptText = StringLocalisationService.getStringWithKey("ClockInFrame_please_enter_pin");
            pinInput.PinIsValidText = StringLocalisationService.getStringWithKey("WaitingForLoggin");
            pinInput.PinChanged += onPinChange;
        }

        private void onPinChange(object? sender, EventArgs e)
        {
            if (pinInput.isPinValid && selectedUser != null)
            {
                var Logintask = _sessionManager.tryLogin(selectedUser, pinInput.PIN, OpeningCash);
                Logintask.Wait();
                if (Logintask.Result == false)
                {
                    pinInput.PromptText = StringLocalisationService.getStringWithKey("ClockInFrame_Wrong_PIN_Try_again");
                    pinInput.resetPin();
                }
            }
        }

        private void cancelAndExitButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void UselectUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppUsers.Count > 1)
            {
                selectedUser = null;
                UsersListView.SelectedItem = null;
            }
            openingCashInput.Visibility = Visibility.Visible;
            pinInput.Visibility = Visibility.Collapsed;
        }

        private void SelectedUserPanelFadeIn()
        {
            selectedUserPanel.Visibility = Visibility.Visible;
            ShowSelectedUserStoryboard.Begin();
        }

        private void SelectedUserPanelFadeOut()
        {
            selectedUserPanel.Visibility = Visibility.Collapsed;
            HideSelectedUserStoryboard.Begin();
        }


        private void NumberPadBackButton_Click(object sender, EventArgs e)
        {
            TextBox targetTextBox = numberBox.NumberTextBox;

            int selStart = targetTextBox.SelectionStart;
            int selLength = targetTextBox.SelectionLength;

            if (selLength > 0)
            {
                targetTextBox.Text = targetTextBox.Text.Remove(selStart, selLength);
                targetTextBox.SelectionStart = selStart;
            }
            else if (selStart > 0)
            {
                targetTextBox.Text = targetTextBox.Text.Remove(selStart - 1, 1);
                targetTextBox.SelectionStart = selStart - 1;
            }
        }

        private void NumberPadButton_Click(object sender, CharacterEventArgs e)
        {
            TextBox targetTextBox = numberBox.NumberTextBox;

            string input = e.Character;
            int selectionStart = targetTextBox.SelectionStart;
            int selectionLength = targetTextBox.SelectionLength;

            if (selectionLength > 0)
            {
                // Replace the selected text
                targetTextBox.Text = targetTextBox.Text
                    .Remove(selectionStart, selectionLength)
                    .Insert(selectionStart, input);

                targetTextBox.SelectionStart = selectionStart + input.Length;
            }
            else
            {
                // Insert at cursor position
                targetTextBox.Text = targetTextBox.Text
                    .Insert(selectionStart, input);

                targetTextBox.SelectionStart = selectionStart + input.Length;
            }
        }

        private void numberBoxButton_Click(object sender, RoutedEventArgs e)
        {
            if (DefaultUserLogin) {
                defaultLoginTextBlock.Visibility = Visibility.Visible;
            }
            OpeningCash = numberBox.Value?? 0;
            openingCashInput.Visibility = Visibility.Collapsed;
            pinInput.Visibility = Visibility.Visible;
            pinInput.Focus(FocusState.Programmatic);
        }
    }
}
