using GamingMonks.Localization;
using TMPro;
using UnityEngine;

namespace GamingMonks
{
    public class TopPanelWithModeContext : MonoBehaviour
    {
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI otherModesText;
        public GameObject bestScorePanel;
        public GameObject settingsPopUp;
        public RectTransform coinPanelIcon;
        public bool canShopBeOpened = true;
        //public Image musicToggleSprite;

        private void OnEnable()
        {
            settingsPopUp.gameObject.SetActive(false);
            //musicToggleSprite.sprite = PlayerPrefs.GetInt("isMusicEnabled") == 1 ? ThemeManager.Instance.GetBlockSpriteWithTag("MusicON") : ThemeManager.Instance.GetBlockSpriteWithTag("MusicOFF");
        }

        public void OnSettingsPressed()
        {
            if (InputManager.Instance.canInput() && GamePlayUI.Instance.currentGameMode != GameMode.Tutorial)
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                settingsPopUp.gameObject.SetActive(!settingsPopUp.gameObject.activeSelf);
            }
        }

        //public void OnAudioPressed()
        //{
        //    if (InputManager.Instance.canInput())
        //    {
        //        InputManager.Instance.DisableTouchForDelay();
        //        UIFeedback.Instance.PlayButtonPressEffect();
        //        musicToggleSprite.sprite = PlayerPrefs.GetInt("isMusicEnabled") == 1 ? ThemeManager.Instance.GetBlockSpriteWithTag("MusicON") : ThemeManager.Instance.GetBlockSpriteWithTag("MusicOFF");
        //        ProfileManager.Instance.ToggleMusicStatus();
                
        //    }
        //}

        public void OnRetryPressed()
        {
            if (InputManager.Instance.canInput())
            {
                if (settingsPopUp.activeSelf)
                {
                    settingsPopUp.SetActive(false);
                }
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
                GamePlay.Instance.blockShapeController.EnableAllBlockShapeContainerInput();
                UIController.Instance.OpenGameRetryAndQuit("Restart?");
                //GamePlayUI.Instance.RestartGame();
            }
        }

        public void OnBackButtonPressed()
        {

            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
                GamePlay.Instance.blockShapeController.EnableAllBlockShapeContainerInput();
                if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                {
                    UIController.Instance.OpenGameRetryAndQuit("Quit?");
                }
                else
                {
                    UIController.Instance.OpenScreenFromTopPanel();
                }
                //UIController.Instance.OpenScreenFromTopPanel();
            }
        }

        public void OnShopButtonPressed()
        {
            if (InputManager.Instance.canInput() && canShopBeOpened)
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
                GamePlay.Instance.blockShapeController.EnableAllBlockShapeContainerInput();
                UIController.Instance.OpenShopScreen();
            }
        }

        public void SetModeText(GameMode gameMode)
        {
            if(gameMode == GameMode.Level)
            {
                otherModesText.text = "";
                otherModesText.gameObject.SetActive(false);
                levelText.text = LocalizationManager.Instance.GetTextWithTag("txtLevel") + GamePlayUI.Instance.GetGameModeText(GameMode.Level);
                levelText.gameObject.SetActive(true);
            }
            else
            {
                levelText.text = "";
                levelText.gameObject.SetActive(false);
                otherModesText.text = GamePlayUI.Instance.GetGameModeText(gameMode);
                otherModesText.gameObject.SetActive(true);
            }
            bestScorePanel.SetActive(gameMode != GameMode.Level);
        }
    }
}

