using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
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
    public class SubscriptionService
    {
        private AppDataService _appDataService;
        private readonly HttpService _httpService;
        private const string SubscriptionCheckEndpoint = "https://app.kiober.com/api/subscription/check";

        public SubscriptionService(AppDataService appDataService, HttpService httpService)
        {
            _appDataService = appDataService;
            _httpService = httpService;
        }
        public async Task<SubscriptionRecord?> CheckSubscriptionAsync(string productId, bool fakeResultTrue = false)
        {
            try
            {
                var requestData = new { ProductId = productId, fakeTrue = fakeResultTrue };

                var subscription =  await _httpService.PostAsync<SubscriptionRecord>(
                    SubscriptionCheckEndpoint,
                    requestData
                );
                return subscription;
            }
            catch (Exception ex)
            {
                // log or handle error
                return null;
            }
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

        //public void StartFreeTrial()
        //{
        //    if (!FreeTrialExpired())
        //    {
        //        if(!OnFreeTrial())
        //        {
        //            // Set the free trial end date to 14 days from now
        //            _appDataService.FreeTrialEndDate = DateTime.Now.AddDays(14);
        //        }
        //    }
        //}
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
