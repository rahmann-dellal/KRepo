using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.DATA;
using KFP.Services;
using KFP.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP.Ui.pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditUserPage : Page
    {
        private EditUserVM viewModel;
        private Regex usernameRegex = new Regex(@"^(?=.{3,16}$)(?![-_])[\p{L}\p{N}](?:[\p{L}\p{N}_-]*[\p{L}\p{N}])?(?<![-_])$");
        public EditUserPage()
        {
            this.InitializeComponent();
            viewModel = Ioc.Default.GetService<EditUserVM>();
            pinPad.PinChanged += pinPad_PinChanged;
            pinPad.PromptText = StringLocalisationService.getStringWithKey("Choose_a_six_digits_PIN_code");
            pinPad.PinIsValidText = StringLocalisationService.getStringWithKey("Thank_you");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var parameters = e.Parameter as List<Object>;
                if (parameters != null && parameters.Count > 0)
                {
                    viewModel._pageMode = EditUserVM.UserPageMode.Edition;
                    int userID = (int)parameters.FirstOrDefault();
                    viewModel.userID = userID;
                }
            }
            else
            {
                viewModel._pageMode = EditUserVM.UserPageMode.Addtition;
                viewModel.User = new AppUser();
                viewModel._pageMode = EditUserVM.UserPageMode.Addtition;
            }
            ResetForm();
        }

        private void ResetForm()
        {

            if (viewModel._pageMode == EditUserVM.UserPageMode.Addtition)
            {
                RoleComboBox.SelectedIndex = -1;
                avatarListBox.SelectedIndex = -1;
                userNameTextBox.Text = "";
                pinPad.resetPin();
            }
            else
            {
                RoleComboBox.SelectedIndex = (int)viewModel.OldRole;
                avatarListBox.SelectedIndex = viewModel.OldAvatarCode;
                userNameTextBox.Text = viewModel.OldUserName;
                pinPad.sleep();
            }
            formChanged();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            formChanged();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            formChanged();
        }

        private void validateUsername()
        {
            if (userNameTextBox.Text.Length > 0 && !usernameRegex.IsMatch(userNameTextBox.Text))
            {
                usernameErrorBlock.Visibility = Visibility.Visible;

                if (userNameTextBox.Text.StartsWith("-") || userNameTextBox.Text.StartsWith("_") ||
                    userNameTextBox.Text.EndsWith("-") || userNameTextBox.Text.EndsWith("_"))
                {
                    usernameErrorBlock.Text = StringLocalisationService.getStringWithKey("Username_cannot_start_with_letter_underscore");
                }

                else if (userNameTextBox.Text.Length < 4 || userNameTextBox.Text.Length > 16)
                {
                    usernameErrorBlock.Text = StringLocalisationService.getStringWithKey("Username_between_4_16");
                }
                else
                {
                    usernameErrorBlock.Text = StringLocalisationService.getStringWithKey("please_enter_a_valid_username");
                }
            }
            else
            {
                usernameErrorBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void validateRole()
        {
            if (userNameTextBox.Text.Length > 0 && RoleComboBox.SelectedIndex < 0 && usernameRegex.IsMatch(userNameTextBox.Text))
            {
                RoleErrorBlock.Visibility = Visibility.Visible;
                RoleErrorBlock.Text = StringLocalisationService.getStringWithKey("Please_choose_user_role");
            }
            else
            {
                RoleErrorBlock.Visibility = Visibility.Collapsed;
            }
        }
        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = StringLocalisationService.getStringWithKey("save_changes");
            confirmDialog.Title = StringLocalisationService.getStringWithKey("Confirm");
            confirmDialog.PrimaryButtonText = StringLocalisationService.getStringWithKey("Yes");
            confirmDialog.CloseButtonText = StringLocalisationService.getStringWithKey("Cancel");
            confirmDialog.XamlRoot = this.XamlRoot;
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {

                viewModel.User.UserName = userNameTextBox.Text;
                viewModel.User.Role = (UserRole)((ComboBoxItem)RoleComboBox.SelectedItem).DataContext;
                if (pinPad.IsSleeping)
                {
                    viewModel.User.PINHash = viewModel.OldPINHash;
                }
                else
                {
                    viewModel.User.PINHash = pinPad.PinHash;
                }

                viewModel.User.avatarCode = this.avatarListBox.SelectedIndex;
                DisplayProgressRing();
                var savingUser = viewModel.saveUserToDBAsync();
            }
        }
        public void DisplaySuccessMessage()
        {
            this.Content = new TextBlock()
            {
                Text = StringLocalisationService.getStringWithKey("Operation_was_seccessfull"),
                Margin = new Thickness(0, 40, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 50, 200, 50))
            };
        }
        public void DisplayProgressRing()
        {
            this.Content = new ProgressRing()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 50,
                Height = 50,
            };
        }
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void avatarListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            formChanged();
        }


        private void pinPad_PinChanged(object sender, System.EventArgs e)
        {
            formChanged();
        }

        private void formChanged()
        {
            validateUsername();
            validateRole();

            if (usernameRegex.IsMatch(userNameTextBox.Text) &&
                RoleComboBox.SelectedIndex >= 0)
            {
                if (viewModel._pageMode == EditUserVM.UserPageMode.Addtition)
                {
                    pinPad.FadeIn();
                }
                if (pinPad.isPinValid || pinPad.IsSleeping)
                {
                    //checking if any changes were made to activate the save button
                    if (pinPad.IsSleeping && userNameTextBox.Text == viewModel.OldUserName
                        && (UserRole)RoleComboBox.SelectedIndex == viewModel.OldRole
                        && avatarListBox.SelectedIndex == viewModel.OldAvatarCode)
                    {
                        saveButton.IsEnabled = false;
                        saveButton.Background = null;
                    }
                    else
                    {
                        saveButton.IsEnabled = true;
                        saveButton.Background = new SolidColorBrush(Colors.LightGreen);
                    }
                }
                else
                {
                    saveButton.IsEnabled = false;
                    saveButton.Background = null;
                }
            }
            else
            {
                if (viewModel._pageMode == EditUserVM.UserPageMode.Addtition)
                {
                    pinPad.FadeOut();
                }
                saveButton.IsEnabled = false;
            }
            if (userNameTextBox.Text.Length == 0 && RoleComboBox.SelectedIndex < 0 && avatarListBox.SelectedIndex < 0 && pinPad.isPinEmpty)
            {
                resetButton.IsEnabled = false;
            }
            else
            {
                resetButton.IsEnabled = true;
            }
        }

        private Visibility UserHasPrivelegesOf(UserRole role)
        {
            if (viewModel.UserhasPrivilegesOf(role))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        //User cannot change his own role
        private Visibility isNotLoggedUser()
        {
            if (viewModel.isNotLoggedUser()) return Visibility.Visible;
            else return Visibility.Collapsed;
        }
    }
}
