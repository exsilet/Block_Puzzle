using UnityEngine;

namespace GamingMonks
{
	/// <summary>
	/// Scriptable class for the ad manager settings.
	/// </summary>
    public class AdSettings : ScriptableObject
    {
        #region ConsentSettings
		// User consent required or not.
        public bool enableConsent = true;

		// Consent to required, not requird or only required to EEA users.
		public ConsentSelection consentSelection = ConsentSelection.RequiredAll;
        #endregion

        #region AdNetworkSelection
		// Ads enabled for game or not.
        public bool adsEnabled = true;

		// Selected ad network.
        public AdNetworkSelection selectedAdNetwork = AdNetworkSelection.UnityAds;

		// Banner ad should be used or not.
        public bool bannerAdsEnabled = false;

		// Should load banner of game start automatically or not.
        public bool showBannerOnLoad = false;

		// Interstitial ad should be used or not.
        public bool interstitialAdsEnabled = true;

		// Should show interstitial on game over.
        public bool showInterstitialOnGameOver = true;

        // Minimum Duration between 2 interstitial ads.
        public int delayBetweenIngerstitials;

		// Rewarded ad should be used or not.
        public bool rewardedAdsEnabled = true;
        #endregion
    }
}

#if HB_EEA_CONSENT
[System.Serializable]
public enum ConsentSelection
{
    NotRequired,        // Consent not required.
    ReqiuredOnlyInEEA,  // Consent only required to users being to EEA.
    RequiredAll         // Consent required to all.
}
#else 
[System.Serializable]
public enum ConsentSelection
{
    NotRequired,        // Consent not required.
    RequiredAll         // Consent required to all.
}
#endif

/// <summary>
/// Selection of active ad network.
/// </summary>
[System.Serializable]
public enum AdNetworkSelection
{
    UnityAds,       
    GoogleMobileAds,
    IronSource,
    AppLovinMax,
    Custom
}

/// <summary>
/// Position of banner ad.
/// </summary>
[System.Serializable]
public enum BannerAdPosition
{
    TOP_RIGHT,
    TOP_CENTER,
    TOP_LEFT,
    CENTER,
    BOTTOM_RIGHT,
    BOTTOM_CENTER,
    BOTTOM_LEFT
}

public class ConsentResponse
{
    public bool is_request_in_eea_or_unknown;
}

public enum UserLocation
{
    Unknown = 0,
    InEEA = 1,
    NotInEEA = 2
}

