using UnityEngine;

namespace GamingMonks.Ads
{
    /// <summary>
    /// Abstract class for invoking ads.
    /// </summary>
    public abstract class AdHelper : MonoBehaviour
    {
        // Initializes the ad network.
        public abstract void InitializeAdNetwork();

        // Shows banner ad.
        public abstract void ShowBanner();
        
        // Hides banner ad.
        public abstract void HideBanner();
        
        //  Checks if interstitil ad is available.
        public abstract bool IsInterstitialAvailable();
        
        // Shows interstitial ad.
        public abstract void ShowInterstitial();
        
        // Checks if rewarded ad is available.
        public abstract bool IsRewardedAvailable();
        
        // Shows rewarded ad.
        public abstract void ShowRewarded();
    }
}