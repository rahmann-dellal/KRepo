using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KFP.Services
{
    public enum SubscriptionType
    {
        FreeTrial,
        Expired,
        Cancelled,
        Basic,
    }

    public class SubscriptionRecord
    {
        public string ProductId { get; set; } = string.Empty;
        public SubscriptionType Type { get; set; } // "Subscription" or "FreeTrial"
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }

        public bool isActive()
        {
            if (Type != SubscriptionType.Expired && Type != SubscriptionType.Cancelled && ExpiryDate.HasValue)
            {
                return DateTimeOffset.Now < ExpiryDate.Value;
            }
            return false; // If no expiry date is set, consider it inactive
        }
    }
    public class StartFreeTrialRequest
    {
        public string ProductId { get; set; } = string.Empty;
    }
    public class CheckoutRequest
    {
        public string ProductId { get; set; } = string.Empty;
    }
    public class GetCheckoutLinkResponse
    {
        public string CheckoutUrl { get; set; } = string.Empty;
    }
    public class SubscriptionService
    {
        private AppDataService _appDataService;
        private readonly HttpService _httpService;
        private const string SubscriptionCheckEndpoint = "https://app.kiober.com/api/subscription/check";
        private const string CheckoutEndpoint = "https://app.kiober.com/api/subscription/getcheckoutlink";
        private const string startFreeTrialEndpoint = "https://app.kiober.com/api/subscription/startfreetrial";


        public SubscriptionService(AppDataService appDataService, HttpService httpService)
        {
            _appDataService = appDataService;
            _httpService = httpService;
        }
        public async Task<SubscriptionRecord?> StartFreeTrialAsync(string productId)
        {
            var request = new StartFreeTrialRequest
            {
                ProductId = productId
            };

            // Calls your existing PostAsync<T>
            var record = await _httpService.PostAsync<SubscriptionRecord?>(
                startFreeTrialEndpoint,
                request
            );
            saveSubscriptionRecord(record);
            return record;
        }
        private void saveSubscriptionRecord(SubscriptionRecord? record)
        {
            if (record != null)
            {
                if (_appDataService.SubscriptionType != null)
                {
                    if (_appDataService.SubscriptionType == SubscriptionType.Basic && _appDataService.ExpiryDate > record.ExpiryDate)
                    {
                        // do nothing as the basic subscription is longer than the free trial
                        return ;
                    }
                    //OR
                    if (record.Type == SubscriptionType.FreeTrial && _appDataService.ExpiryDate > record.ExpiryDate)
                    {
                        // do nothing as the existing free trial is longer than the new one
                        return ;
                    }
                }
                _appDataService.SubscriptionType = record.Type;
                _appDataService.ExpiryDate = record.ExpiryDate;
            }
        }
        public async Task<SubscriptionRecord?> CheckSubscriptionAsync(string productId, bool fakeResultTrue = false)
        {
                var requestData = new { ProductId = productId };

                var subscription =  await _httpService.PostAsync<SubscriptionRecord>(
                    SubscriptionCheckEndpoint,
                    requestData
                );
                saveSubscriptionRecord(subscription);
                return subscription;
        }
        public string GetProductId()
        {
            if(_appDataService.ProductId == null)
            {
                string? id = null;
                if (WindowsIdentity.GetCurrent().User != null)
                { 
                    id = WindowsIdentity.GetCurrent().User?.Value;
                }
                if(id == null)
                {
                    id = Guid.NewGuid().ToString();
                }

                // Optional: hash it for cleaner storage
                using var sha = SHA256.Create();
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(id));
                _appDataService.ProductId = Convert.ToBase64String(hash);
            }
            return _appDataService.ProductId;
        }

        /// <summary>
    /// Asks the backend for a Stripe Checkout link.
    /// </summary>
    public async Task<string?> GetCheckoutLinkAsync(CheckoutRequest request)
    {
        try
        {
            GetCheckoutLinkResponse? checkoutResponse = await _httpService.PostAsync<GetCheckoutLinkResponse>(CheckoutEndpoint, request);

            if (checkoutResponse == null || string.IsNullOrEmpty(checkoutResponse.CheckoutUrl))
            {
                // You can log or handle the HTTP error here
                return null;
            }

            // Optional: validate it's a valid URL
            if (Uri.IsWellFormedUriString(checkoutResponse.CheckoutUrl, UriKind.Absolute))
            {
                return checkoutResponse.CheckoutUrl;
            }

            return null;
        }
        catch (Exception ex)
        {
            // Handle network errors, timeouts, etc.
            Console.WriteLine($"Error getting checkout link: {ex.Message}");
            return null;
        }
    }
        public SubscriptionRecord? GetLocalSupscriptionRecord()
        {
            if(_appDataService.SubscriptionType != null)
            {
                SubscriptionRecord subscriptionRecord = new() {
                    Type = _appDataService.SubscriptionType.Value,
                    ExpiryDate = _appDataService.ExpiryDate
                };
                return subscriptionRecord;
            } else
            {
                return null;
            }
        }

        //public bool FreeTrialExpired()
        //{
        //    // Check if the free trial has expired based on the current date and the trial end date stored in app data
        //    var trialEndDate = _appDataService.FreeTrialEndDate;
        //    if (trialEndDate.HasValue)
        //    {
        //        return DateTime.Now > trialEndDate.Value;
        //    }
        //    return true; // If no trial end date is set, consider it expired
        //}

        //public bool OnFreeTrial()
        //{
        //    // Check if the user is currently on a free trial
        //    var trialEndDate = _appDataService.FreeTrialEndDate;
        //    if (trialEndDate.HasValue)
        //    {
        //        return DateTime.Now <= trialEndDate.Value;
        //    }
        //    return false; // If no trial end date is set, consider not on free trial
        //}
    }
}
