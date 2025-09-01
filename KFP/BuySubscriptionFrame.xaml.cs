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
using KFP.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KFP
{
    public sealed partial class BuySubscriptionFrame : UserControl
    {
        SubscriptionService _subscriptionService;
        public event EventHandler? OnClosePressed;
        public BuySubscriptionFrame()
        {
            _subscriptionService = Ioc.Default.GetService<SubscriptionService>()!;
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var checkoutUrl = await _subscriptionService.GetCheckoutLinkAsync(new CheckoutRequest
            {
                ProductId = "3344"
            });
            if (!string.IsNullOrEmpty(checkoutUrl))
            {
                // Replace Border content with WebView2
                var webView = new WebView2();
                await webView.EnsureCoreWebView2Async();
                webView.Source = new Uri(checkoutUrl);

                BorderContainer.Child = webView;
            }
            else
            {
                // Show error message
                BorderContainer.Child = new TextBlock
                {
                    Text = "Failed to load checkout page.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
        }

        public void RaiseOnClosePressed()
        {
            OnClosePressed?.Invoke(this, EventArgs.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseOnClosePressed();
        }
    }
}
