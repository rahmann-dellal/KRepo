using CommunityToolkit.Mvvm.DependencyInjection;
using KFP.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP
{
    public sealed partial class SubscriptionFrame : UserControl
    {
        private AppState _appstate;
        private Microsoft.UI.Dispatching.DispatcherQueue dispatcher;
        private SubscriptionService _subscriptionService;
        public SubscriptionFrame()
        {
            _appstate = Ioc.Default.GetService<AppState>()!;
            _subscriptionService = Ioc.Default.GetService<SubscriptionService>()!;
            this.InitializeComponent();
            dispatcher = this.DispatcherQueue;
            CheckServerForSubscription();
        }

        private void CheckServerForSubscription()
        {
            _subscriptionService.CheckSubscriptionAsync(_subscriptionService.GetProductId(), false).ContinueWith(async (action) =>
            {
                SubscriptionRecord? result = null;
                try
                {
                    result = await action;
                    if (result != null && result.isActive())
                    {
                        dispatcher.TryEnqueue(() =>
                        {
                            _appstate.RaiseSubscriptionStatusChanged();
                        });
                    }
                    else
                    {
                        dispatcher.TryEnqueue(() =>
                        {
                            SubscriptionGrid.Visibility = Visibility.Visible;
                            LoadingGrid.Visibility = Visibility.Collapsed;
                        });
                    }
                }
                catch (Exception ex)
                {
                    dispatcher.TryEnqueue(() =>
                    {
                        SubscriptionGrid.Visibility = Visibility.Collapsed;
                        LoadingGrid.Visibility = Visibility.Collapsed;
                        ErrorGrid.Visibility = Visibility.Visible;
                        if (ex is NetworkException)
                        {
                            NoNetworkErrorText.Visibility = Visibility.Visible;
                            CantReachServerErrorText.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            NoNetworkErrorText.Visibility = Visibility.Collapsed;
                            CantReachServerErrorText.Visibility = Visibility.Visible;
                        }
                    });
                }

            });
        }
        //links
        private async void TermsLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://kiober.com/terms");
            try
            {
                bool success = await Launcher.LaunchUriAsync(uri);
                if (!success)
                {
                    await ShowLinkFallbackDialog("Terms of Service", uri);
                }
            }
            catch (Exception ex)
            {
                await ShowLinkFallbackDialog("Terms of Service", uri, ex.Message);
            }
        }

        private async void PrivacyLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://kiober.com/privacy");
            try
            {
                bool success = await Launcher.LaunchUriAsync(uri);
                if (!success)
                {
                    await ShowLinkFallbackDialog("Privacy Policy", uri);
                }
            }
            catch (Exception ex)
            {
                await ShowLinkFallbackDialog("Privacy Policy", uri, ex.Message);
            }
        }

        private async Task ShowLinkFallbackDialog(string title, Uri uri, string errorMessage = null)
        {
            var dialog = new ContentDialog
            {
                Title = $"{title} Link",
                Content = errorMessage == null
                    ? $"We couldn’t open the link automatically.\n\nYou can copy and open it manually:\n{uri}"
                    : $"Error: {errorMessage}\n\nYou can copy and open this link manually:\n{uri}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // ensure dialog attaches to window
            };

            await dialog.ShowAsync();
        }

        private void BuySubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            BuySubscriptionBorder.Visibility = Visibility.Visible;
            ChoiceGrid.Visibility = Visibility.Collapsed;
            var buySubscriptionFrame = new BuySubscriptionFrame();
            buySubscriptionFrame.OnClosePressed += (s, args) =>
            {
                BuySubscriptionBorder.Visibility = Visibility.Collapsed;
                BuySubscriptionBorder.Child = null;
                ChoiceGrid.Visibility = Visibility.Visible;
            };
            BuySubscriptionBorder.Child = buySubscriptionFrame;
        }

        private async void StartTrialButton_Click(object sender, RoutedEventArgs e)
        {
            ChoiceGrid.Visibility = Visibility.Collapsed;
            BuySubscriptionBorder.Visibility = Visibility.Collapsed;
            LoadingGrid.Visibility = Visibility.Visible;

            try
            {
                var trial = await _subscriptionService.StartFreeTrialAsync(_subscriptionService.GetProductId());
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error Starting Trial",
                    Content = $"An error occurred while starting the free trial: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot // ensure dialog attaches to window
                };
                await dialog.ShowAsync();
                ChoiceGrid.Visibility = Visibility.Visible;
                LoadingGrid.Visibility = Visibility.Collapsed;
                return;
            }
            dispatcher.TryEnqueue(() =>
            {
                _appstate.RaiseSubscriptionStatusChanged();
            });
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            SubscriptionGrid.Visibility = Visibility.Collapsed;
            LoadingGrid.Visibility = Visibility.Visible;
            ErrorGrid.Visibility = Visibility.Collapsed;
            CheckServerForSubscription();
        }

        private void CancelExitButton1_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
