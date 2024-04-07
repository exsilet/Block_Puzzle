using GamingMonks.Ads;
using UnityEngine;
using UnityEngine.UI;
using GamingMonks.Localization;
using TMPro;

    namespace GamingMonks
{
    /// <summary>
    /// This script is used to rescue game using coins or watching video.
    /// </summary>
    public class RescueGame : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtTitle;
        [SerializeField] RectTransform coinsIcon;
        [SerializeField] TextMeshProUGUI txtRescueCoinAmount;
        [SerializeField] Button BtnRescueWithAds;

        [SerializeField] GameObject levelModesSpecificPanel;
        [SerializeField] GameObject otherModesSpecificPanel;
        [SerializeField] TextMeshProUGUI otherModesScoreTxt;
        [SerializeField] TextMeshProUGUI otherModesBestScoreTxt;


        bool attemptedRescueWithCoins = false;
        string rescueVideoTag = "RescueGame";

        bool isRescueDone = false;

        public void SetRescueReason(GameOverReason reason)
        {
            switch (reason)
            {
                case GameOverReason.GRID_FILLED:
                    txtTitle.SetTextWithTag("txtGameOver_gridfull");
                    break;

                case GameOverReason.BOMB_BLAST:
                    txtTitle.SetTextWithTag("txtGameOver_bombexplode");
                    break;

                case GameOverReason.TIME_OVER:
                    txtTitle.SetTextWithTag("txtGameOver_timeover");
                    break;
                
                case GameOverReason.OUT_OF_MOVE:
                    txtTitle.SetTextWithTag("txtGameOver_outofmove");
                    break;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            // if (!AdmobManager.Instance.GetRewardedAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadRewardedAd();
            // }

            //setup Mode specific panels
            if (UIController.Instance.powerUpContextPanel.gameObject.activeSelf)
            {
                UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
            }
            if (UIController.Instance.gameRetryAndQuitScreen.gameObject.activeSelf)
            {
                UIController.Instance.gameRetryAndQuitScreen.gameObject.Deactivate();
            }
            levelModesSpecificPanel.gameObject.SetActive(GamePlayUI.Instance.currentGameMode == GameMode.Level);
            otherModesSpecificPanel.gameObject.SetActive(GamePlayUI.Instance.currentGameMode != GameMode.Level);
            if(GamePlayUI.Instance.currentGameMode != GameMode.Level)
            {
                otherModesScoreTxt.text = PlayerPrefs.GetInt("rescueScreenScore").ToString("N0");
                otherModesBestScoreTxt.text = ProfileManager.Instance.GetBestScore(GamePlayUI.Instance.currentGameMode).ToString("N0");
            }

            txtRescueCoinAmount.text = ProfileManager.Instance.GetAppSettings().rescueGameCoinsCost.ToString();
            // Pauses the game when it gets enabled.
            GamePlayUI.Instance.PauseGame();
           
            AdManager.OnRewardedAdRewardedEvent += OnRewardedAdRewarded;
            UIController.Instance.EnableCurrencyBalanceButton();

            //if(AdManager.Instance.IsRewardedAvailable()) {
                BtnRescueWithAds.GetComponent<CanvasGroup>().alpha = 1.0F;
            if (AdmobManager.Instance.isRewardedShown)
            {
                BtnRescueWithAds.interactable = false;
            }
            else
            {
                BtnRescueWithAds.interactable = true;
            }
            //} else {
                //BtnRescueWithAds.GetComponent<CanvasGroup>().alpha = 0.2F;
                //BtnRescueWithAds.interactable = false;
            //}
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            /// Resumes the game when it gets enabled.
            AdManager.OnRewardedAdRewardedEvent -= OnRewardedAdRewarded;
            attemptedRescueWithCoins = false;
            UIController.Instance.DisableCurrencyBalanceButton();
            isRescueDone = false;
        }


        /// <summary>
        /// Will rescue game after showing rewarded video ad.
        /// </summary>
        public void ContinueWithAdsButtonPressed()
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            
            if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
                GamePlay.Instance.blockShapeController.EnableAllBlockShapeContainerInput();
            
            // if (!AdmobManager.Instance.GetRewardedAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadRewardedAd();
            // }
            switch (GamePlayUI.Instance.currentGameOverReason)
            {
                case GameOverReason.GRID_FILLED:
                    AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetSingleBlocks);
                    break;
                case GameOverReason.TIME_OVER:
                    AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetExtraTime);
                    break;
                case GameOverReason.BOMB_BLAST:
                    AdmobManager.Instance.ShowRewardedAd(RewardAdType.RemoveBombs);
                    break;
                case GameOverReason.OUT_OF_MOVE:
                    AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetExtraMoves);
                    break;
            }
            
            AnalyticsManager.Instance.ContinueGameEvent(true, 0);
            GamePlayUI.Instance.ResumeGame();
            //Invoke("ResumeGameWithRescue", 1F);
            //if (InputManager.Instance.canInput())
            //{
            //ShowRewardedToRescue();
            //}
        }

        /// <summary>
        /// Will rescue game with coins.
        /// </summary>
        public void ContinueWithCoinsButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                if (CurrencyManager.Instance.DeductCoins(ProfileManager.Instance.GetAppSettings().rescueGameCoinsCost))
                {
                    if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
                        GamePlay.Instance.blockShapeController.EnableAllBlockShapeContainerInput();
                    
                    UIController.Instance.PlayDeductCoinsAnimation(coinsIcon.position, 0.1F);
                    Invoke("ResumeGameWithRescue", 1.5F);
                    AnalyticsManager.Instance.ContinueGameEvent(false, ProfileManager.Instance.GetAppSettings().rescueGameCoinsCost);
                    //FBManeger.Instance.SpentCredit(ProfileManager.Instance.GetAppSettings().rescueGameCoinsCost, null);
                }
                else
                {
                    attemptedRescueWithCoins = true;
                    
                    //Will open shop if not having enough coins.
                    UIController.Instance.OpenShopScreen();
                    
                    UIController.Instance.DisableCurrencyBalanceButton();
                }
            }
        }

        /// <summary>
        /// Will start/continnue game with rescue successful.
        /// </summary>
        void ResumeGameWithRescue()
        {
            isRescueDone = true;
            InputManager.Instance.DisableTouchForDelay(1.5F);
            GamePlayUI.Instance.OnRescueSuccessful();
            gameObject.Deactivate();
        }

        /// <summary>
        /// Closes pause screen and resumes gameplay.
        /// </summary>
        public void OnCloseButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                isRescueDone = false;
                UIFeedback.Instance.PlayButtonPressEffect();
                GamePlayUI.Instance.OnRescueCancelled();
                gameObject.Deactivate();
            }
        }

        void ShowRewardedToRescue()
        {
            if (AdManager.Instance.IsRewardedAvailable()) {
                AdManager.Instance.ShowRewardedWithTag(rescueVideoTag);
            }
        }

        /// <summary>
        ///  Rewarded Ad Successful.see
        /// </summary>
        void OnRewardedAdRewarded(string watchVidoTag)
        {
            if (watchVidoTag == rescueVideoTag)
            {
                isRescueDone = true;
                GamePlayUI.Instance.OnRescueSuccessful();
                gameObject.Deactivate();
            }
        }

        /// <summary>
        ///  Not in use. THis method can be called if rescue should be executed on sufficient balance received.
        /// </summary>
        public void ReattemptRescueWithCoins()
        {
            if (attemptedRescueWithCoins)
            {
                if (CurrencyManager.Instance.DeductCoins(ProfileManager.Instance.GetAppSettings().rescueGameCoinsCost))
                {
                    UIController.Instance.PlayDeductCoinsAnimation(coinsIcon.position, 0.1F);
                    Invoke("ResumeGameWithRescue", 1.5F);
                }
            }
        }
    }
}

