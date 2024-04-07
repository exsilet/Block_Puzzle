using UnityEngine;

namespace GamingMonks.Ads
{
    /// <summary>
    /// This class component will be added to game dynamically if Ironsource is selected as active ad network.
    /// All the callbacks will be forwarded to ad manager.
    /// </summary>
	public class IronSourceAdManager : AdHelper
    {
        IronSourceAdsSettings settings;

        /// <summary>
        /// Initialized the ad network.
        /// </summary>
        public override void InitializeAdNetwork()
        {
            settings = (IronSourceAdsSettings)(Resources.Load("AdNetworkSettings/IronSourceAdsSettings"));
            #if HB_IRONSOURCE
            IronSource.Agent.init (settings.GetAppId(), IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
            IronSource.Agent.setConsent(AdManager.Instance.consentAllowed);
            #endif
            Invoke("StartLoadingAds", 2F);
        }

        /// <summary>
        /// Loads ads after initialization.
        /// </summary>
        public void StartLoadingAds()
        {
            RequestBannerAds();
            RequestInterstitial();
            RequestRewarded();
        }

        // Requests banner ad.        
        public void RequestBannerAds()
        {
            #if HB_IRONSOURCE
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, settings.GetBannerPosition());
            #endif
        }

        // Requests intestitial ad.
        public void RequestInterstitial()
        {
            #if HB_IRONSOURCE
            IronSource.Agent.loadInterstitial();
            #endif
        }

        // Requests rewarded ad. Rewarded ad loading and caching is done automatically with ironsource sdk.
        public void RequestRewarded()
        {
        }

        // Shows banner ad.
        public override void ShowBanner()
        {
            #if HB_IRONSOURCE
            IronSource.Agent.displayBanner();
            #endif
        }

        // Hides banner ad.
        public override void HideBanner()
        {
            #if HB_IRONSOURCE
            IronSource.Agent.hideBanner();
            #endif
        }

        // Check if interstial ad ready to show.
        public override bool IsInterstitialAvailable()
        {
            #if HB_IRONSOURCE
            return IronSource.Agent.isInterstitialReady();
            #endif
            return false;
        }

        // Shows interstitial ad if available.
        public override void ShowInterstitial()
        {
            #if HB_IRONSOURCE
            if(IsInterstitialAvailable()) {
                IronSource.Agent.showInterstitial();
            } else {
                RequestInterstitial();
            }
            #endif
        }
        // Checks if rewarded ad ready to show.
        public override bool IsRewardedAvailable()
        {
            #if HB_IRONSOURCE
            return IronSource.Agent.isRewardedVideoAvailable();
            #endif
            return false;
        }

        // Shows rewarded ad if loaded.
        public override void ShowRewarded()
        {
            if (IsRewardedAvailable())
            {
                #if HB_IRONSOURCE
                IronSource.Agent.showRewardedVideo();
                #endif
            }
        }

        #if HB_IRONSOURCE
        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable() {
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;        
            IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent; 
            IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent; 
            IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;    

            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable() 
        {
            IronSourceEvents.onBannerAdLoadedEvent -= BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent -= BannerAdLoadFailedEvent;        
            IronSourceEvents.onBannerAdClickedEvent -= BannerAdClickedEvent; 
            IronSourceEvents.onBannerAdScreenPresentedEvent -= BannerAdScreenPresentedEvent; 
            IronSourceEvents.onBannerAdScreenDismissedEvent -= BannerAdScreenDismissedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent -= BannerAdLeftApplicationEvent;    

            IronSourceEvents.onInterstitialAdReadyEvent -= InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;        
            IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent; 
            IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent; 
            IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;

            IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent; 
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent; 
            IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;
        }
        
        #region Banner Callbacks
        //Banner ad  event callbacks.
        void BannerAdLoadedEvent() {
            if(AdManager.Instance.adSettings.showBannerOnLoad) {
                ShowBanner();
            }
            AdManager.Instance.OnBannerLoaded();
        }
        
        void BannerAdLoadFailedEvent (IronSourceError error) {
            Invoke("RequestBannerAds",2F);
            AdManager.Instance.OnBannerLoadFailed(error.getDescription());
        }

        void BannerAdClickedEvent () {
        }

        void BannerAdScreenPresentedEvent () {
        }

        void BannerAdScreenDismissedEvent() {
        }

        void BannerAdLeftApplicationEvent() {
        }
        #endregion

        #region Interstitial Callback
        // Interstitial ad event callbacks.
        void InterstitialAdLoadFailedEvent (IronSourceError error) {
            Invoke("RequestInterstitial",2F);
            AdManager.Instance.OnInterstitialLoadFailed(error.getDescription());
        }
        
        void InterstitialAdShowSucceededEvent() {
            AdManager.Instance.OnInterstitialShown();
        }
        
        void InterstitialAdShowFailedEvent(IronSourceError error) {
            Debug.Log("IS :: Interstitial Show Failed..");
        }
        
        void InterstitialAdClickedEvent () {
        }
        
        void InterstitialAdClosedEvent () {
            RequestInterstitial();
            AdManager.Instance.OnInterstitialClosed();
        }
        
        void InterstitialAdReadyEvent() {
            AdManager.Instance.OnInterstitialLoaded();
        }
        
        void InterstitialAdOpenedEvent() {
            
        }
        #endregion

        #region Rewarded Callbacks
        // Rewarded ad event callbacks.
        void RewardedVideoAdOpenedEvent() {
            AdManager.Instance.OnRewardedShown();
        }  
       
        void RewardedVideoAdClosedEvent() {
            RequestRewarded();
            AdManager.Instance.OnRewardedClosed();
        }
       
        void RewardedVideoAvailabilityChangedEvent(bool available) {
            bool rewardedVideoAvailability = available;

            if(available) {
                AdManager.Instance.OnRewardedLoaded();
            } else {
                AdManager.Instance.OnRewardedLoadFailed("fail");
            }
        }
        
        void RewardedVideoAdStartedEvent() {
        }
        
        void RewardedVideoAdEndedEvent() {
            RequestRewarded();
        }
        
        void RewardedVideoAdRewardedEvent(IronSourcePlacement placement) {
            AdManager.Instance.OnRewardedAdRewarded();
        }   
        
        void RewardedVideoAdShowFailedEvent (IronSourceError error){
        }
        #endregion
        #endif
    }
}
