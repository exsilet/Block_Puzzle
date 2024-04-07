using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{	
	public class HomeScreen : MonoBehaviour 
	{
        #pragma warning disable 0649
        //[SerializeField] Button m_btnClassicMode;
        //[SerializeField] Button m_btnTimeMode;
        //[SerializeField] Button m_btnBlastMode;
        //[SerializeField] Button m_btnAdvanceMode;
        #region MyChanges
        [SerializeField] Button m_otherMode;
        [SerializeField] Button m_btnClassicMode;
        [SerializeField] Button m_btnLevelMode;
        [SerializeField] GameObject m_handAnimation;
        [SerializeField] GameObject m_tag;
        #endregion
#pragma warning restore 0649

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            GamePlaySettings gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings"); 
            
            //if(!gamePlaySettings.classicModeSettings.modeEnabled) {
            //    m_btnClassicMode.gameObject.SetActive(false);
            //}

            //if(!gamePlaySettings.timeModeSettings.modeEnabled) {
            //    m_btnTimeMode.gameObject.SetActive(false);
            //}

            //if(!gamePlaySettings.blastModeSettings.modeEnabled) {
            //    m_btnBlastMode.gameObject.SetActive(false);
            //}

            //if(!gamePlaySettings.advancedModeSettings.modeEnabled) {
            //    m_btnAdvanceMode.gameObject.SetActive(false);
            //}
            gamePlaySettings = null;
        }

        private void OnEnable()
        {
            m_tag.SetActive(PlayerPrefs.GetInt("CurrentLevel") <= 3);
            //if (PlayerPrefs.GetInt("tutorialShown") == 1 && PlayerPrefs.GetInt("firstLevelShown") == 0)
            //{

            //}
            if (PlayerPrefs.GetInt("CurrentLevel") > 1)
            {
                PlayerPrefs.SetInt("firstLevelShown", 1);
            }

            if (PlayerPrefs.GetInt("firstLevelShown") == 1)
            {
                m_handAnimation.SetActive(false);
                m_otherMode.interactable = true;
                m_btnClassicMode.interactable = true;
            }
            else
            {
                m_handAnimation.SetActive(true);
                m_btnClassicMode.interactable = false;
                m_otherMode.interactable = false;
                m_tag.SetActive(true);
            }
        }


        /// <summary>
        /// Action on pressing play button on home screen. This is not used and Select Mode screen is also not in use.
        /// </summary>
        public void OnClassicModeButtonPressed() 
        {
            if(InputManager.Instance.canInput()) {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                //bool showTutorial = false;

                //if (!PlayerPrefs.HasKey("tutorialShown"))
                //{
                //    GamePlaySettings gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings");
                //    showTutorial = gamePlaySettings.tutorialModeSettings.modeEnabled;

                //    if (!showTutorial)
                //    {
                //        PlayerPrefs.SetInt("tutorialShown", 1);
                //    }
                //}
                //if (showTutorial)
                //{
                //    m_handAnimation.SetActive(false);
                //    m_otherMode.interactable = true;
                //    m_btnLevelMode.interactable = true;
                //    UIController.Instance.topPanelWithHearts.gameObject.SetActive(false);
                //    UIController.Instance.topPanelWithModeContext.gameObject.SetActive(true);
                //    UIController.Instance.homeScreen.gameObject.SetActive(false);
                //    UIController.Instance.gameScreen_Tutorial.gameObject.Activate();
                //    UIController.Instance.topPanelWithModeContext.SetModeText(GameMode.Tutorial);
                //    //cachedSelectedMode = gameMode;
                //}
                //else
                //{
                    //Opens mode selection screen.
                    //UIController.Instance.selectModeScreen.Activate();
                    UIController.Instance.LoadGamePlay(GameMode.Classic);
                    //UIController.Instance.LoadLevelSelectionScreen();
                //}
            }
        }

        public void OnOtherModesPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.selectModeScreen.Activate();
            }
        }

        ///// <summary>
        ///// Classic mode button listener.
        ///// </summary>
        //public void OnClassicModeButtonPressed()
        //{
        //    if (InputManager.Instance.canInput())
        //    {
        //        UIFeedback.Instance.PlayButtonPressEffect();
        //        UIController.Instance.LoadGamePlay(GameMode.Classic);
        //    }
        //}

        /// <summary>
        /// Time mode button listener.
        /// </summary>
        public void OnTimeModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Timed);
            }
        }

        /// <summary>
        /// Blast mode button listener.
        /// </summary>
        public void OnBlastModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Blast);
            }
        }

        /// <summary>
        /// Advance mode button listener.
        /// </summary>
        public void OnAdvanceModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Advance);
            }
        }

        #region MyChanges
        public void OnLevelModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                if (PlayerPrefs.GetInt("CurrentLevel") >= 2)
                {
                    PlayerPrefs.SetInt("firstLevelShown", 1);
                }
                UIFeedback.Instance.PlayButtonPressEffect();
                if (!PlayerPrefs.HasKey("firstLevelShown"))
                {
                    PlayerPrefs.SetInt("firstLevelShown", 1);
                    PlayerPrefs.SetInt("firstFailClassicMode", 1);
                    m_handAnimation.SetActive(false);
                    UIController.Instance.LoadFirstLevelFromHomeScreen();
                }
                else
                {
                    UIController.Instance.LoadLevelSelectionScreen();
                }
                
            }
        }
        #endregion

    }
}
