using System.Collections;
using GamingMonks.Ads;
using GamingMonks.Localization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamingMonks
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI buttonLevelsText;
        [SerializeField] private GameObject btn_Menu;
        [SerializeField] private GameObject btn_retry;
        [SerializeField] private GameObject btn_tryLevels;
        [SerializeField] private GameObject subSorryText;

        int rewardAmount = 0;
        int totalLinesCompleted = 0;
        int gameOverId = 0;

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		private void OnEnable() 
		{
            if (PlayerPrefs.GetInt("firstLevelShown") == 1)
            {
                PlayerPrefs.SetInt("firstFailClassicMode", 1);
            }
            if (!PlayerPrefs.HasKey("firstFailClassicMode") && PlayerPrefs.GetInt("firstFailClassicMode") == 0)
            {
                PlayerPrefs.SetInt("firstFailClassicMode", 1);
                btn_Menu.SetActive(false);
                btn_retry.SetActive(false);
                btn_tryLevels.SetActive(true);
                subSorryText.SetActive(false);
            }
            else
            {
                btn_Menu.SetActive(true);
                btn_retry.SetActive(true);
                btn_tryLevels.SetActive(false);
                subSorryText.SetActive(true);
            }

            if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                buttonLevelsText.text = "Levels";
            }
            else
            {
                buttonLevelsText.text = "Menu";
            }
            if (UIController.Instance.gameRetryAndQuitScreen.gameObject.activeSelf)
            {
                UIController.Instance.gameRetryAndQuitScreen.gameObject.Deactivate();
            }

            // Pauses the game when it gets enabled.
            GamePlayUI.Instance.PauseGame();
        }

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() 
        {
            //rewardAnimation.SetActive(false);
            //UIController.Instance.DisableCurrencyBalanceButton();
            
            // Resume the game when it gets disabled.
            GamePlayUI.Instance.ResumeGame();
        }

        public void OnTryLevelbuttonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.OpenHomeScreenFromGameOver();
            }
        }

        /// <summary>
        /// Try to show Interstitial ad on game over if ad is available.
        /// </summary>
         void TryShowingInterstitial() 
        {
            if(AdManager.Instance.adSettings.showInterstitialOnGameOver) 
            {
                if(AdManager.Instance.IsInterstitialAvailable()) {
                    AdManager.Instance.ShowInterstitial();
                }
            }
        }

        /// <summary>
        /// Sets game data and score on game over.
        /// </summary>
        public void SetGameData(GameOverReason reason, int score, int _totalLinesCompleted, GameMode gameMode)
        {   
            totalLinesCompleted = _totalLinesCompleted;

            // Number of time game over shown. Also total game play counts.
            gameOverId = PlayerPrefs.GetInt("gameOverId",0);
            gameOverId += 1;
            PlayerPrefs.SetInt("gameOverId", gameOverId);

            if(ProfileManager.Instance.gameOverReviewSessions.Contains(gameOverId)) {
                InputManager.Instance.DisableTouchForDelay(2F);
                Invoke("CheckForReview",2F);
            }
        }

        void CheckForReview() {
             UIController.Instance.CheckForReviewAppPopupOnGameOver(gameOverId);
        }

        void ShowRewardAnimation() 
        {
            CurrencyManager.Instance.AddCoins(rewardAmount);
        }

        /// <summary>
        /// Continue button click listener.
        /// </summary>
        public void OnContinueButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay(1F);
                UIFeedback.Instance.PlayButtonPressEffect();
            }
        }

        /// <summary>
        /// Return button click listener.
        /// </summary>
        public void OnRetryButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                gameObject.SetActive(false);
                GamePlayUI.Instance.RestartGame();
            }
        }
        /// <summary>
        /// Return button click listener.
        /// </summary>
        public void OnLevelsButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();

                if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                {
                    HealthController.Instance.UseEnergy();
                    UIController.Instance.LoadLevelSelectionScreenFromGameOver();
                }
                else
                {
                    UIController.Instance.OpenHomeScreenFromGameOver();
                }
            }
        }

        /// <summary>
        /// Replay button click listner.
        /// </summary>
        public void OnReplayButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                StartCoroutine(RestartGame());
            }
        }

        /// <summary>
        /// Restarts game.
        /// </summary>
        IEnumerator RestartGame()
        {
            gameObject.Deactivate();
            UIController.Instance.Pop("GameOver");
            GameProgressTracker.Instance.ClearProgressData();
            GamePlayUI.Instance.ResetGame();
            yield return new WaitForSeconds(0.1f);
            TargetController.Instance.DestroyTargetsOnReloadLevel();
            AdmobManager.Instance.ResetAdPreferences();
            GamePlayUI.Instance.StartGamePlay(GamePlayUI.Instance.currentGameMode);
        }
    }
}


