using GamingMonks.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.Example;

namespace GamingMonks
{
    public class GameWin : MonoBehaviour
    {
        public TextMeshProUGUI levelText;
        //[SerializeField] private TextMeshProUGUI getExtraCoinsWithAds;
        public Button adButton;
        public Button continueButton;
        
        [Tooltip("Transform of a coin image from where coin collect animation will be played.")]
        [SerializeField] private RectTransform coinAnimationOrigin;
        private Sprite[] coinSprites = new Sprite[2];
        //private LevelSO levelSO;


        private void Awake()
        {
            //if (levelSO == null)
            //{
            //    levelSO = (LevelSO)Resources.Load("LevelSettings");
            //}
        }

        private void OnEnable()
        {
            // if (!AdmobManager.Instance.GetInterstitialAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadInterstitialAd();
            // }
            //
            // if (!AdmobManager.Instance.GetRewardedAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadRewardedAd();
            // }
            YandexGame.OpenVideoEvent += OpenAd;
            YandexGame.RewardVideoEvent += AddRewardCoins;
            YandexGame.CloseVideoEvent += CloseAd;


            UIController.Instance.adsPanel.SetActive(true);
            UIController.Instance.adsOverTxt.SetActive(false);
            if(UIController.Instance.gameRetryAndQuitScreen.gameObject.activeSelf)
            {
                UIController.Instance.gameRetryAndQuitScreen.gameObject.Deactivate();
            }
            levelText.text = LocalizationManager.Instance.GetTextWithTag("txtLevel") + GamePlayUI.Instance.GetGameModeText(GameMode.Level);
            //getExtraCoinsWithAds.text = LocalizationManager.Instance.GetTextWithTag("txtGetCoin") + GamePlay.Instance.appSettings.watchAdsRewardCoin;
            adButton.interactable = true;
            continueButton.gameObject.SetActive(GamePlayUI.Instance.level < 300/*levelSO.Levels.Length*/);
            //AddRewardCoins();
        }

        private void OnDisable()
        {
            YandexGame.OpenVideoEvent -= OpenAd;
            YandexGame.RewardVideoEvent -= AddRewardCoins;
            YandexGame.CloseVideoEvent -= CloseAd;
        }

        //public void OpenRewardAd()
        //{
        //    YandexGame.RewVideoShow(0);
        //}

        public void OpenAd()
        {
            Time.timeScale = 0;
        }

        public void CloseAd()
        {
            Debug.Log("closed");
            Time.timeScale = 1;
        }

        public void OnLevelsButtonPressed()
        {
            //if (InputManager.Instance.canInput())
            //{
            // if (!AdmobManager.Instance.GetInterstitialAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadInterstitialAd();
            // }
            UIFeedback.Instance.PlayButtonPressEffect();
            if (!PlayerPrefs.HasKey("isUserAdFree") && !(PlayerPrefs.GetInt("isUserAdFree") == 1))
            {
                YandexGame.FullscreenShow();
                //AdmobManager.Instance.ShowInterstitialAd();
            }
            UIController.Instance.LoadLevelSelectionScreenFromGameWin();
            //}
        }

        public void AddRewardCoins(int id)
        {
            //if(GamePlayUI.Instance.gameWinReward)
            //{
                coinSprites[0] = ThemeManager.Instance.GetBlockSpriteWithTag("SingleCoin");
                UIController.Instance.PlayAddCoinsAnimationAtPosition(coinAnimationOrigin.position,
                    UIController.Instance.topPanelWithModeContext.coinPanelIcon.position, 0, coinSprites);
                CurrencyManager.Instance.AddCoins(5);
                GamePlayUI.Instance.gameWinReward = false;
            //}

        }

        public void OnContinueButtonPressed()
        {
            //if (InputManager.Instance.canInput())
            //{
            // if (!AdmobManager.Instance.GetInterstitialAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadInterstitialAd();
            // }
            
            UIFeedback.Instance.PlayButtonPressEffect();
            UIController.Instance.gameWinScreen.Deactivate();
            if (!PlayerPrefs.HasKey("isUserAdFree") && !(PlayerPrefs.GetInt("isUserAdFree") == 1))
            {
                //AdmobManager.Instance.ShowInterstitialAd();
                YandexGame.FullscreenShow();
            }
            UIController.Instance.LoadGamePlayFromLevelSelection(GameMode.Level,
                GamePlayUI.Instance.level + 1);
            //AdmobManager.Instance.isRewardedShown = false;
            //}
        }

        public void GetFiveCoinsByWatchingAdBtnPressed(int idReward)
        {
            //AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetExtraCoins);
            YandexGame.RewVideoShow(idReward);
        }
    }
}

