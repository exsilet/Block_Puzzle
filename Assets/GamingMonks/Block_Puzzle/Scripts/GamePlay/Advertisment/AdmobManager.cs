using UnityEngine.Events;
using UnityEngine;
using System;

namespace GamingMonks
{
    public class AdmobManager : Singleton<AdmobManager>
    {
        private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
        private DateTime appOpenExpireTime;
        // private AppOpenAd appOpenAd;
        // private BannerView bannerView;
        // private InterstitialAd interstitialAd;
        // private RewardedAd rewardedAd;
        // private RewardedInterstitialAd rewardedInterstitialAd;
        [SerializeField] private string interstitialAdUnitID = "";
        [SerializeField] private string rewardedAdUnitID = "";
        private float deltaTime;
        private bool isShowingAppOpenAd;
        public UnityEvent OnAdLoadedEvent;
        public UnityEvent OnAdFailedToLoadEvent;
        public UnityEvent OnAdOpeningEvent;
        public UnityEvent OnAdFailedToShowEvent;
        public UnityEvent OnUserEarnedRewardEvent;
        public UnityEvent OnAdClosedEvent;
        public bool showFpsMeter = true;
        public bool isRewardedShown = false;
        public bool isAppPurchased = false;
        public bool hasInitializedAds = false;
        public bool isRescueRewarded = false;
        public int countToNotShowAds;
        public bool isuserwatchedfullAd = false;
        public bool isrewardedShowedToRescue = false;
        public RewardAdType rewardAdType;// { get; private set; }
        //public TextMeshProUGUI fpsMeter;
        //public TextMeshProUGUI statusText;


        #region UNITY MONOBEHAVIOR METHODS

        public void Start()
        {
            //MobileAds.SetiOSAppPauseOnBackground(true);           

            // Initialize the Google Mobile Ads SDK.
            //MobileAds.Initialize(HandleInitCompleteAction);

            // Listen to application foreground / background events.
            //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        // private void HandleInitCompleteAction(InitializationStatus initstatus)
        // {
        //
        //     // Callbacks from GoogleMobileAds are not guaranteed to be called on
        //     // the main thread.
        //     // In this example we use MobileAdsEventExecutor to schedule these calls on
        //     // the next Update() loop.
        //     MobileAdsEventExecutor.ExecuteInUpdate(() =>
        //     {
        //         //statusText.text = "Initialization complete.";
        //         //RequestBannerAd();
        //         hasInitializedAds = true;
        //         RequestAndLoadInterstitialAd();
        //         RequestAndLoadRewardedAd();
        //     });
        // }

        #endregion

        #region Mobile Ad Initializer Function

        public void InitializeAds()
        {
            //MobileAds.SetiOSAppPauseOnBackground(true);
            //UnityAds.SetConsentMetaData("gdpr.consent", true);
            //List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

            //List<string> deviceIds = new List<string>();
            //deviceIds.Add("78e2e910-3e73-49ab-a08b-a315924a581d");
            //deviceIds.Add("78e2e910-3e73-49ab-a08b-a315924a581d");
            //RequestConfiguration requestConfiguration = new RequestConfiguration
            //    .Builder()
            //    .SetTestDeviceIds(deviceIds)
            //    .build();
            //RequestConfiguration requestConfiguration =
            //    new RequestConfiguration.Builder()
            //    .SetSameAppKeyEnabled(true).build();
            //MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            // Add some test device IDs (replace with your own device IDs).
            //#if UNITY_IPHONE
            //        deviceIds.Add("78e2e910-3e73-49ab-a08b-a315924a581d");
            //#elif UNITY_ANDROID
            //        deviceIds.Add("78e2e910-3e73-49ab-a08b-a315924a581d");
            //#endif

            //        deviceIds.Add("78e2e910-3e73-49ab-a08b-a315924a581d");
            //        // Configure TagForChildDirectedTreatment and test device IDs.
            //        RequestConfiguration requestConfiguration =
            //            new RequestConfiguration.Builder()
            //            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            //            .SetTestDeviceIds(deviceIds).build();
            //MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            //MobileAds.Initialize(HandleInitCompleteAction);

            // Listen to application foreground / background events.
            //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        #endregion

        #region HELPER METHODS

        // private AdRequest CreateAdRequest()
        // {
        //     return new AdRequest.Builder()
        //         .AddKeyword("unity-admob-sample")
        //         .Build();
        // }

        #endregion

        #region Get Ad refernces
        // public InterstitialAd GetInterstitialAd()
        // {
        //     return interstitialAd;
        // }
        //
        // public RewardedAd GetRewardedAd()
        // {
        //     return rewardedAd;
        // }

        #endregion

        public void RequestBannerAd()
        {
            PrintStatus("Requesting Banner ad.");

            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#endif

            // Clean up banner before reusing
            // if (bannerView != null)
            // {
            //     bannerView.Destroy();
            // }
            //
            // // Create a 320x50 banner at top of the screen
            // bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
            //
            // // Add Event Handlers
            // bannerView.OnAdLoaded += (sender, args) =>
            // {
            //     PrintStatus("Banner ad loaded.");
            //     OnAdLoadedEvent.Invoke();
            // };
            // bannerView.OnAdFailedToLoad += (sender, args) =>
            // {
            //     PrintStatus("Banner ad failed to load with error: " + args.LoadAdError.GetMessage());
            //     OnAdFailedToLoadEvent.Invoke();
            // };
            // bannerView.OnAdOpening += (sender, args) =>
            // {
            //     PrintStatus("Banner ad opening.");
            //     OnAdOpeningEvent.Invoke();
            // };
            // bannerView.OnAdClosed += (sender, args) =>
            // {
            //     PrintStatus("Banner ad closed.");
            //     OnAdClosedEvent.Invoke();
            // };
            // bannerView.OnPaidEvent += (sender, args) =>
            // {
            //     string msg = string.Format("{0} (currency: {1}, value: {2}",
            //                                 "Banner ad received a paid event.",
            //                                 args.AdValue.CurrencyCode,
            //                                 args.AdValue.Value);
            //     PrintStatus(msg);
            // };
            //
            // // Load a banner ad
            // bannerView.LoadAd(CreateAdRequest());
        }

        public void DestroyBannerAd()
        {
            // if (bannerView != null)
            // {
            //     bannerView.Destroy();
            // }
        }


        #region INTERSTITIAL ADS

        public void RequestAndLoadInterstitialAd()
        {
            PrintStatus("Requesting Interstitial ad.");

#if UNITY_EDITOR
        string adUnitId = "unused";
#endif

            // Clean up interstitial before using it
            // if (interstitialAd != null)
            // {
            //     interstitialAd.Destroy();
            // }
            //
            // interstitialAd = new InterstitialAd(adUnitId);
            // Debug.Log("Interstitial" + adUnitId.ToString());
            // // Add Event Handlers
            // interstitialAd.OnAdLoaded += (sender, args) =>
            // {
            //     PrintStatus("Interstitial ad loaded.");
            // //ShowInterstitialAd();
            // OnAdLoadedEvent.Invoke();
            // };
            // interstitialAd.OnAdFailedToLoad += (sender, args) =>
            // {
            //     PrintStatus("Interstitial ad failed to load with error: " + args.LoadAdError.GetMessage());
            //     OnAdFailedToLoadEvent.Invoke();
            // };
            // interstitialAd.OnAdOpening += (sender, args) =>
            // {
            //     PrintStatus("Interstitial ad opening.");
            //     OnAdOpeningEvent.Invoke();
            // };
            // interstitialAd.OnAdClosed += (sender, args) =>
            // {
            //     PrintStatus("Interstitial ad closed.");
            // //if (interstitialAd.IsLoaded() == false)
            // //{
            // RequestAndLoadInterstitialAd();
            //
            // //}
            // OnAdClosedEvent.Invoke();
            // };
            // interstitialAd.OnAdDidRecordImpression += (sender, args) =>
            // {
            //     PrintStatus("Interstitial ad recorded an impression.");
            // };
            // interstitialAd.OnAdFailedToShow += (sender, args) =>
            // {
            //     PrintStatus("Interstitial ad failed to show.");
            // };
            // interstitialAd.OnPaidEvent += (sender, args) =>
            // {
            //     string msg = string.Format("{0} (currency: {1}, value: {2}",
            //                                 "Interstitial ad received a paid event.",
            //                                 args.AdValue.CurrencyCode,
            //                                 args.AdValue.Value);
            //     PrintStatus(msg);
            // };
            //
            // // Load an interstitial ad
            // interstitialAd.LoadAd(CreateAdRequest());
            //ShowInterstitialAd();
        }


        public void ShowInterstitialAd()
        {
            int ateemptsRemaining = PlayerPrefs.GetInt("AttemptsToPlayLevelWithoutAds");
            if (ateemptsRemaining <= 0)
            {
                isAppPurchased = false;
            }
            else
            {
                isAppPurchased = true;
            }

            // if (interstitialAd != null && interstitialAd.IsLoaded() && isRewardedShown == false
            //     && GamePlayUI.Instance.level > 10 && !isAppPurchased)
            // {
            //     interstitialAd.Show();
            // }
            // else
            // {
            //     PrintStatus("Interstitial ad is not ready yet.");
            // }
        }

        public void DestroyInterstitialAd()
        {
            // if (interstitialAd != null)
            // {
            //     interstitialAd.Destroy();
            // }
        }

        #endregion

        #region REWARDED ADS

        public void RequestAndLoadRewardedAd()
        {
            PrintStatus("Requesting Rewarded ad.");
#if UNITY_EDITOR
        string adUnitId = "unused";
#endif
            // if (rewardedAd != null)
            // {
            //     rewardedAd.Destroy();
            // }
            //
            // // create new rewarded ad instance
            // //rewardedAd = new RewardedAd(adUnitId);
            //
            // // Add Event Handlers
            // rewardedAd.OnAdLoaded += (sender, args) =>
            // {
            //     PrintStatus("Reward ad loaded.");
            // //ShowRewardedAd();
            // OnAdLoadedEvent.Invoke();
            // };
            // rewardedAd.OnAdFailedToLoad += (sender, args) =>
            // {
            //     PrintStatus("Reward ad failed to load.");
            //     OnAdFailedToLoadEvent.Invoke();
            // };
            // rewardedAd.OnAdOpening += (sender, args) =>
            // {
            //     PrintStatus("Reward ad opening.");
            //     OnAdOpeningEvent.Invoke();
            // };
            // rewardedAd.OnAdFailedToShow += (sender, args) =>
            // {
            //     PrintStatus("Reward ad failed to show with error: " + args.AdError.GetMessage());
            //     OnAdFailedToShowEvent.Invoke();
            // };
            // rewardedAd.OnAdClosed += (sender, args) =>
            // {
            //     PrintStatus("Reward ad closed.");
            //     isRewardedShown = true;
            //     CheckIfUserSkippedAd();
            //     RequestAndLoadRewardedAd();
            //     OnAdClosedEvent.Invoke();
            // };
            // rewardedAd.OnUserEarnedReward += (sender, args) =>
            // {
            //     PrintStatus("User earned Reward ad reward: " + args.Amount);
            //     isuserwatchedfullAd = true;
            //     GiveRewards(rewardAdType);
            //     OnUserEarnedRewardEvent.Invoke();
            // };
            // rewardedAd.OnAdDidRecordImpression += (sender, args) =>
            // {
            //     PrintStatus("Reward ad recorded an impression.");
            // };
            // rewardedAd.OnPaidEvent += (sender, args) =>
            // {
            //     string msg = string.Format("{0} (currency: {1}, value: {2}",
            //                                 "Rewarded ad received a paid event.",
            //                                 args.AdValue.CurrencyCode,
            //                                 args.AdValue.Value);
            //     PrintStatus(msg);
            // };
            //
            // // Create empty ad request
            //rewardedAd.LoadAd(CreateAdRequest());
        }

        public void DestroyRewarded()
        {
            // if (rewardedAd != null)
            // {
            //     rewardedAd.Destroy();
            // }
        }

        public void ShowRewardedAd(RewardAdType _rewardAdType)
        {
            SetRewardType(_rewardAdType);
            //RequestAndLoadRewardedAd();
            // if (rewardedAd != null && rewardedAd.IsLoaded())
            // {
            //     rewardedAd.Show();
            //     //FBManeger.Instance.WatchedRewardedAd();
            // }
            // else
            // {
            //     StartCoroutine(UIController.Instance.ShowAdNotAvailableToastNotification());
            //     PrintStatus("Rewarded ad is not ready yet.");
            // }
        }

        async void CheckIfUserSkippedAd()
        {
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(0.2f));
            if (isuserwatchedfullAd && UIController.Instance.rescueGameScreen.activeSelf)
            {
                isuserwatchedfullAd = false;
                UIController.Instance.rescueGameScreen.SetActive(false);
                UIController.Instance.Pop("RescueGame");
            }
            else
            {
                if (UIController.Instance.rescueGameScreen.activeSelf)
                {
                    UIController.Instance.rescueGameScreen.SetActive(false);
                    UIController.Instance.gameOverScreen.SetActive(true);
                    UIController.Instance.Pop("RescueGame");
                }
            }
        }

        private void SetRewardType(RewardAdType _rewardAdType)
        {
            rewardAdType = _rewardAdType;
        }

        public void RequestAndLoadRewardedInterstitialAd()
        {
            PrintStatus("Requesting Rewarded Interstitial ad.");

            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#endif

            // Create an interstitial.
            // RewardedInterstitialAd.LoadAd(adUnitId, CreateAdRequest(), (rewardedInterstitialAd, error) =>
            // {
            //     if (error != null)
            //     {
            //         PrintStatus("Rewarded Interstitial ad load failed with error: " + error);
            //         return;
            //     }
            //
            //     this.rewardedInterstitialAd = rewardedInterstitialAd;
            //     PrintStatus("Rewarded Interstitial ad loaded.");
            //
            // // Register for ad events.
            // this.rewardedInterstitialAd.OnAdDidPresentFullScreenContent += (sender, args) =>
            //     {
            //         PrintStatus("Rewarded Interstitial ad presented.");
            //     };
            //     this.rewardedInterstitialAd.OnAdDidDismissFullScreenContent += (sender, args) =>
            //     {
            //         PrintStatus("Rewarded Interstitial ad dismissed.");
            //         this.rewardedInterstitialAd = null;
            //     };
            //     this.rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            //     {
            //         PrintStatus("Rewarded Interstitial ad failed to present with error: " +
            //                                                                 args.AdError.GetMessage());
            //         this.rewardedInterstitialAd = null;
            //     };
            //     this.rewardedInterstitialAd.OnPaidEvent += (sender, args) =>
            //     {
            //         string msg = string.Format("{0} (currency: {1}, value: {2}",
            //                                     "Rewarded Interstitial ad received a paid event.",
            //                                     args.AdValue.CurrencyCode,
            //                                     args.AdValue.Value);
            //         PrintStatus(msg);
            //     };
            //     this.rewardedInterstitialAd.OnAdDidRecordImpression += (sender, args) =>
            //     {
            //         PrintStatus("Rewarded Interstitial ad recorded an impression.");
            //     };
            // });
        }

        public void ShowRewardedInterstitialAd()
        {
            // if (rewardedInterstitialAd != null)
            // {
            //     rewardedInterstitialAd.Show((reward) =>
            //     {
            //         PrintStatus("Rewarded Interstitial ad Rewarded : " + reward.Amount);
            //     });
            // }
            // else
            // {
            //     PrintStatus("Rewarded Interstitial ad is not ready yet.");
            // }
        }

        #endregion

        #region APPOPEN ADS

        // public bool IsAppOpenAdAvailable
        // {
        //     get
        //     {
        //         return (!isShowingAppOpenAd
        //                 && appOpenAd != null
        //                 && DateTime.Now < appOpenExpireTime);
        //     }
        // }
        //
        // public void OnAppStateChanged(AppState state)
        // {
        //     // Display the app open ad when the app is foregrounded.
        //     UnityEngine.Debug.Log("App State is " + state);
        //
        //     // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
        //     MobileAdsEventExecutor.ExecuteInUpdate(() =>
        //     {
        //         if (state == AppState.Foreground)
        //         {
        //             ShowAppOpenAd();
        //         }
        //     });
        // }

        public void RequestAndLoadAppOpenAd()
        {
            PrintStatus("Requesting App Open ad.");
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-8801497656721723~7304872503";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/5662855259";
#else
        string adUnitId = "unexpected_platform";
#endif
            // create new app open ad instance
            // AppOpenAd.LoadAd(adUnitId,
            //                  ScreenOrientation.Portrait,
            //                  CreateAdRequest(),
            //                  OnAppOpenAdLoad);
        }

        // private void OnAppOpenAdLoad(AppOpenAd ad, AdFailedToLoadEventArgs error)
        // {
        //     if (error != null)
        //     {
        //         PrintStatus("App Open ad failed to load with error: " + error);
        //         return;
        //     }
        //
        //     PrintStatus("App Open ad loaded. Please background the app and return.");
        //     this.appOpenAd = ad;
        //     this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;
        // }

        public void ShowAppOpenAd()
        {
            // if (!IsAppOpenAdAvailable)
            // {
            //     return;
            // }
            //
            // // Register for ad events.
            // this.appOpenAd.OnAdDidDismissFullScreenContent += (sender, args) =>
            // {
            //     PrintStatus("App Open ad dismissed.");
            //     isShowingAppOpenAd = false;
            //     if (this.appOpenAd != null)
            //     {
            //         this.appOpenAd.Destroy();
            //         this.appOpenAd = null;
            //     }
            // };
            // this.appOpenAd.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            // {
            //     PrintStatus("App Open ad failed to present with error: " + args.AdError.GetMessage());
            //
            //     isShowingAppOpenAd = false;
            //     if (this.appOpenAd != null)
            //     {
            //         this.appOpenAd.Destroy();
            //         this.appOpenAd = null;
            //     }
            // };
            // this.appOpenAd.OnAdDidPresentFullScreenContent += (sender, args) =>
            // {
            //     PrintStatus("App Open ad opened.");
            // };
            // this.appOpenAd.OnAdDidRecordImpression += (sender, args) =>
            // {
            //     PrintStatus("App Open ad recorded an impression.");
            // };
            // this.appOpenAd.OnPaidEvent += (sender, args) =>
            // {
            //     string msg = string.Format("{0} (currency: {1}, value: {2}",
            //                                 "App Open ad received a paid event.",
            //                                 args.AdValue.CurrencyCode,
            //                                 args.AdValue.Value);
            //     PrintStatus(msg);
            // };
            //
            // isShowingAppOpenAd = true;
            // appOpenAd.Show();
        }

        #endregion


        #region AD INSPECTOR

        public void OpenAdInspector()
        {
            PrintStatus("Open ad Inspector.");

            // MobileAds.OpenAdInspector((error) =>
            // {
            //     if (error != null)
            //     {
            //         PrintStatus("ad Inspector failed to open with error: " + error);
            //     }
            //     else
            //     {
            //         PrintStatus("Ad Inspector opened successfully.");
            //     }
            // });
        }

        #endregion

        #region Utility

        ///<summary>
        /// Log the message and update the status text on the main thread.
        ///<summary>
        private void PrintStatus(string message)
        {
        //     MobileAdsEventExecutor.ExecuteInUpdate(() =>
        //     {
        //     //statusText.text = message;
        // });
        }

        #endregion

        #region Switch case to give rewards

        public void GiveRewards(RewardAdType _rewardAdType)
        {
            _rewardAdType = rewardAdType;
            switch (_rewardAdType)
            {
                case RewardAdType.GetSingleBlocks:
                    GamePlay.Instance.PerformRescueAction(GameOverReason.GRID_FILLED);
                    break;
                
                case RewardAdType.GetExtraMoves:
                    GamePlay.Instance.PerformRescueAction(GameOverReason.OUT_OF_MOVE);
                    break;
                
                case RewardAdType.GetExtraTime:
                    GamePlay.Instance.PerformRescueAction(GameOverReason.TIME_OVER);
                    break;
                
                case RewardAdType.RemoveBombs:
                    GamePlay.Instance.PerformRescueAction(GameOverReason.BOMB_BLAST);
                    break;
                
                case RewardAdType.GetExtraCoins:

                    UIController.Instance.GameWinUIToggle();
                    CurrencyManager.Instance.AddCoins(GamePlay.Instance.appSettings.watchAdsRewardCoin);
                    // UIController.Instance.txtGotReward.text = "You Got " + GamePlay.Instance.appSettings.watchAdsRewardCoin.ToString() + " Coins";
                    // UIController.Instance.ShowGotRewardTip();
                    break;

                case RewardAdType.GetFreeExtraCoinsFromLevelSelectionPanel:
                    UIController.Instance.PlayAddCoinsAnimationAtPosition(UIController.Instance.freeRewardPanelCoinSprite.position,
    UIController.Instance.topPanelWithHearts.coinPanelIcon.position, 0, null);
                    CurrencyManager.Instance.AddCoins(GamePlay.Instance.appSettings.watchAdsRewardCoin);
                    break;

                case RewardAdType.GetLife:
                    //HealthController.Instance.currentEnergy += 1;
                    int val = HealthController.Instance.currentEnergy += GamePlay.Instance.appSettings.watchAdsRewardLife;
                    PlayerPrefs.SetInt("currentEnergy", val);
                    HealthController.Instance.UpdateEnergy();
                    HealthController.Instance.UpdateEnergyTimer();
                    // UIController.Instance.txtGotReward.text = "You Got " + GamePlay.Instance.appSettings.watchAdsRewardLife.ToString() + " Life";
                    // UIController.Instance.ShowGotRewardTip();
                    break;
            }

            #endregion
        }

        public void ResetAdPreferences()
        {
            isRewardedShown = false;
        }
    }

    public enum RewardAdType
    {
        Empty,
        GetSingleBlocks,
        GetExtraMoves,
        GetExtraTime,
        GetExtraCoins,
        GetFreeExtraCoinsFromLevelSelectionPanel,
        GetLife,
        RemoveBombs,
    }
}
