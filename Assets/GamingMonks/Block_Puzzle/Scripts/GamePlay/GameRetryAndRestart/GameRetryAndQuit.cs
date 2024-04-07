using System.Collections;
using TMPro;
using UnityEngine;


namespace GamingMonks
{
    public class GameRetryAndQuit : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        [SerializeField] private GameObject textYouWillLoseOneLife;
        [SerializeField] private GameObject txtAreYouSureWantToQuit;
        [SerializeField] private GameObject imgHeart;

        private void OnEnable()
        {
            if (GamePlayUI.Instance.currentGameMode != GameMode.Level)
            {
                txtAreYouSureWantToQuit.SetActive(true);
                textYouWillLoseOneLife.SetActive(false);
                imgHeart.SetActive(false);
            }
            else
            {
                txtAreYouSureWantToQuit.SetActive(false);
                textYouWillLoseOneLife.SetActive(true);
                imgHeart.SetActive(true);
            }
            /// Pauses the game when it gets enabled.
            GamePlayUI.Instance.PauseGame();
        }

        private void OnDisable()
        {
            GamePlayUI.Instance.ResumeGame();
        }

        public void SetTitleText(string title)
        {
            titleText.text = title;
        }

        public void OnYesButton()
        {
            if (titleText.text == "Quit?") 
            {
                UIController.Instance.OpenScreenFromTopPanel();
                HealthController.Instance.UseEnergy();
                AnalyticsManager.Instance.LevelEvent(GamePlayUI.Instance.level, false, "Quit");
            }
            else
            {
                if (PlayerPrefs.GetInt("currentEnergy") > 0)
                {
                    HealthController.Instance.UseEnergy();
                    StartCoroutine(RestartGame());
                }
                else
                {
                    UIController.Instance.OpenScreenFromTopPanel();
                    UIController.Instance.OpenLivesPanel();
                    AnalyticsManager.Instance.LevelEvent(GamePlayUI.Instance.level, false, "Quit Due to life over");
                }

            }
            this.gameObject.Deactivate();
        }

        IEnumerator RestartGame()
        {
            GameProgressTracker.Instance.ClearProgressData();
            GamePlayUI.Instance.ResetGame();
            yield return new WaitForSeconds(0.1f);
            TargetController.Instance.DestroyTargetsOnReloadLevel();
            AdmobManager.Instance.ResetAdPreferences();
            GamePlayUI.Instance.StartGamePlay(GamePlayUI.Instance.currentGameMode);
        }
        
        public void OnCancel()
        {
            this.gameObject.Deactivate();
            GamePlayUI.Instance.ResumeGame();
        }
    }
}

