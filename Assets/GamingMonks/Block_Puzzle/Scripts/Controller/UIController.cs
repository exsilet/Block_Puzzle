using System.Collections;
using System.Collections.Generic;
using GamingMonks.Localization;
using UnityEngine;
using GamingMonks.UITween;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace GamingMonks
{
    /// <summary>
    /// UIController controlls the entire UI Navigation of the game.
    /// </summary>
    public class UIController : Singleton<UIController>
    {
        List<string> screenStack = new List<string>();

        [SerializeField] Canvas UICanvas;

        [Header("UI Screens")]
        public HomeScreen homeScreen;
        public GamePlayUI gameScreen;
        public Tutorial.GamePlayUI gameScreen_Tutorial;
       
        public LevelSelectionScreen levelSelectionScreen;
        public ShopScreen shopScreen;
        public TopPanelWithHearts topPanelWithHearts;
        public TopPanelWithModeContext topPanelWithModeContext;
        public PowerUpContextPanel powerUpContextPanel;
        public GoalScreen goalScreen;
        public GameRetryAndQuit gameRetryAndQuitScreen;
        

        [Header("Public Members.")]
        //public GameObject shopScreen;
        public GameObject settingScreen;
        public GameObject consentScreen;
        public GameObject reviewScreen;
        public GameObject selectModeScreen;
        public GameObject pauseGameScreen;
        public GameObject rescueGameScreen;
        public GameObject selectThemeScreen;
        public GameObject purchaseSuccessScreen;
        public GameObject commonMessageScreen;
        public GameObject dailyRewardScreen;
        public GameObject gameOverScreen;
        public GameObject gameWinScreen;
        public GameObject lanagueSelectionScreen;
        public GameObject currencyBalanceButton;
        public GameObject guideScreen;
        public GameObject movesGuideScreen;
        public GameObject healthPanel;
        public GameObject contextualOfferScreen;

        public GameObject tipView;

        [Header("Other Public Members")]
        public RectTransform ShopButtonCoinsIcon;
        public Transform RuntimeEffectSpawnParent;
        public RectTransform freeRewardPanelCoinSprite;
        public GameObject adsPanel;
        public GameObject adsOverTxt;

        public GameObject toastNotification;
        public TextMeshProUGUI txtGotReward;
        public TextMeshProUGUI movesCountTxt;
        public GameObject boomrangHUD;
        public TextMeshProUGUI movesText;
        // Ordered popup stack is used when another popup tries to open when already a popup is opened. Ordered stack will control it and add upcoming popups
        // to queue so it will load automatically when alreay existing popup gets closed.
        List<string> orderedPopupStack = new List<string>();

        [System.NonSerialized] public GameMode cachedSelectedMode = GameMode.Classic;

        private string prevScreen = "";

        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake()
        {
            Scene scene = SceneManager.GetActiveScene();
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            /// Enables home screen on game start.
            bool showTutorial = false;

            if (!PlayerPrefs.HasKey("tutorialShown"))
            {
                GamePlaySettings gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings");
                showTutorial = gamePlaySettings.tutorialModeSettings.modeEnabled;

                if (!showTutorial)
                {
                    PlayerPrefs.SetInt("tutorialShown", 1);
                }
            }
            if (showTutorial)
            {
                topPanelWithHearts.gameObject.SetActive(false);
                topPanelWithModeContext.gameObject.SetActive(true);
                homeScreen.gameObject.SetActive(false);
                gameScreen_Tutorial.gameObject.Activate();
                topPanelWithModeContext.SetModeText(GameMode.Tutorial);
                AnalyticsManager.Instance.GameLaunchEvent();
                AnalyticsManager.Instance.TutorialStartedEvent();
                //cachedSelectedMode = gameMode;
            }
            else
            {        
                homeScreen.gameObject.Activate();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            // Registeres session update callback.
            SessionManager.OnSessionUpdatedEvent += OnSessionUpdated;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            // Unregisteres session update callback.
            SessionManager.OnSessionUpdatedEvent -= OnSessionUpdated;
        }

        /// <summary>
        /// Session Updated callback.
        /// </summary>
        private void OnSessionUpdated(SessionInfo info) {
            CheckForReviewAppPopupOnLauch(info.currentSessionCount);
        }

        /// <summary>
        /// Try to show review screen if app setting has review popup on current session id.
        /// </summary>
        void CheckForReviewAppPopupOnLauch(int currentSessionCount)
        {
            bool canShowReviewPopup = true;

            if ((!ProfileManager.Instance.GetAppSettings().showReviewPopupOnLaunch))
            {
                canShowReviewPopup = false;
                return;
            }

            if (PlayerPrefs.HasKey("AppRated") && ProfileManager.Instance.GetAppSettings().neverShowReviewPopupIfRated)
            {
                canShowReviewPopup = false;
                return;
            }

            if (canShowReviewPopup && (ProfileManager.Instance.appLaunchReviewSessions.Contains(currentSessionCount))) {
                ShowReviewPopup();
            }
        }

        public IEnumerator ShowAdNotAvailableToastNotification()
        {
            toastNotification.SetActive(true);
            yield return new WaitForSeconds(2f);
            toastNotification.SetActive(false);
        }
        /// <summary>
        /// Try to show review screen if app setting has review popup on current gameover id.
        /// </summary>
        public void CheckForReviewAppPopupOnGameOver(int currentGameOver)
        {
            bool canShowReviewPopup = true;

            if ((!ProfileManager.Instance.GetAppSettings().showReviewPopupOnGameOver))
            {
                canShowReviewPopup = false;
                return;
            }

            if (PlayerPrefs.HasKey("AppRated") && ProfileManager.Instance.GetAppSettings().neverShowReviewPopupIfRated)
            {
                canShowReviewPopup = false;
                return;
            }

            if (canShowReviewPopup && (ProfileManager.Instance.gameOverReviewSessions.Contains(currentGameOver))) {
                ShowReviewPopup();
            }
        }

        /// <summary>
        /// Handles the device back button, this will be used for android only. 
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (InputManager.Instance.canInput())
                {
                    if (screenStack.Count > 0)
                    {
                        ProcessBackButton(Peek());
                    }
                }
            }
            // string s = "";
            // foreach (var item in screenStack)
            // {
            //     s += item + ",";
            // }
            // Debug.Log(s);
        }

        /// <summary>
        /// Adds the latest activated gameobject to stack.
        /// </summary>
        public void Push(string screenName)
        {
            if (!screenStack.Contains(screenName))
            {
                screenStack.Add(screenName);
            }
        }

        /// <summary>
        /// Returns the name of last activated gameobject from the stack.
        /// </summary>
        public string Peek()
        {
            if (screenStack.Count > 0)
            {
                return screenStack[screenStack.Count - 1];
            }
            return "";
        }

        /// <summary>
        /// Removes the last gameobject name from the stack.
        /// </summary>
        public void Pop(string screenName)
        {
            if (screenStack.Contains(screenName))
            {
                screenStack.Remove(screenName);

                if (orderedPopupStack.Contains(screenName))
                {
                    orderedPopupStack.Remove(screenName);

                    if (orderedPopupStack.Count > 0) {
                        ShowDailogFromStack();
                    }
                }
            }
        }

        /// <summary>
        /// On pressing back button of device, the last added popup/screen will get deactivated based on state of the game. 
        /// </summary>
        void ProcessBackButton(string currentScreen)
        {
            switch (currentScreen)
            {
                case "HomeScreen":
                    QuitGamePopup();
                    break;

                case "SelectMode":
                    selectModeScreen.gameObject.Deactivate();
                    break;

                case "LevelSelectionScreen":
                    OpenScreenFromTopPanel();
                    break;

                case "GamePlay":
                    if (!gameWinScreen.activeSelf)
                    {
                        OpenGameRetryAndQuit("Quit?");
                    }
                    else
                    {
                        if (!PlayerPrefs.HasKey("isUserAdFree") && !(PlayerPrefs.GetInt("isUserAdFree") == 1))
                        {
                            AdmobManager.Instance.ShowInterstitialAd();
                        }
                        UIController.Instance.LoadLevelSelectionScreenFromGameWin();
                    }
                    break;

                case "Gameplay_Tutorial":
                    break;

                case "ShopScreen":
                    OpenScreenFromTopPanel();
                    break;

                case "Settings":
                    settingScreen.gameObject.Deactivate();
                    break;

                case "CommonMessageScreen":
                    commonMessageScreen.gameObject.Deactivate();
                    break;

                case "PurchaseSuccessScreen":
                    purchaseSuccessScreen.gameObject.Deactivate();
                    break;

                case "ReviewAppScreen":
                    reviewScreen.gameObject.Deactivate();
                    break;

                case "SelectLangauge":
                    lanagueSelectionScreen.gameObject.Deactivate();
                    break;

                case "RescueGame":
                    GamePlayUI.Instance.OnRescueCancelled();
                    rescueGameScreen.Deactivate();
                    UIController.Instance.Pop("RescueGame");
                    break;

                case "GameOver":
                    if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                    {
                        gameOverScreen.gameObject.Deactivate();
                        UIController.Instance.Pop("GameOver");
                        HealthController.Instance.UseEnergy();
                        UIController.Instance.LoadLevelSelectionScreenFromGameOver();
                    }
                    else
                    {
                        UIController.Instance.OpenHomeScreenFromGameOver();
                    }
                    break;

                case "GameWin":
                    if (!PlayerPrefs.HasKey("isUserAdFree") && !(PlayerPrefs.GetInt("isUserAdFree") == 1))
                    {
                        AdmobManager.Instance.ShowInterstitialAd();
                    }
                    UIController.Instance.LoadLevelSelectionScreenFromGameWin();
                    break;

                case "GoalScreen":
                    goalScreen.gameObject.Deactivate();
                    break;

                case "GameQuitAndRetry":
                    gameRetryAndQuitScreen.gameObject.Deactivate();
                    GamePlayUI.Instance.ResumeGame();
                    break;

                case "MoreLivesScreen":
                    healthPanel.gameObject.Deactivate();
                    break;

                case "TopPanel-PowerUps":
                    break;

                case "DailyRewardScreen":
                    dailyRewardScreen.gameObject.Deactivate();
                    break;

                case "GuideScreen":
                    guideScreen.gameObject.Deactivate();
                    break;

                case "ContexualOfferScreen":
                    contextualOfferScreen.gameObject.Deactivate();
                    break;

                case "ConsentSetting":
                    consentScreen.gameObject.Deactivate();
                    break;
            }
        }

        /// <summary>
        /// Opens a quit game popup.
        /// </summary>
        void QuitGamePopup()
        {
            new CommonDialogueInfo().SetTitle(LocalizationManager.Instance.GetTextWithTag("txtQuitTitle")).
            SetMessage(LocalizationManager.Instance.GetTextWithTag("txtQuitConfirm")).
            SetPositiveButtomText(LocalizationManager.Instance.GetTextWithTag("txtNo")).
            SetNegativeButtonText(LocalizationManager.Instance.GetTextWithTag("txtYes")).
            SetMessageType(CommonDialogueMessageType.Confirmation).
            SetOnPositiveButtonClickListener(() => {
                UIController.Instance.commonMessageScreen.Deactivate();

            }).SetOnNegativeButtonClickListener(() =>
            {
                QuitGame();
                UIController.Instance.commonMessageScreen.Deactivate();
            }).Show();
        }

        // Quits the game.
        public void QuitGame()
        {
            Invoke("QuitGameAfterDelay", 0.5F);
        }

        /// <summary>
        /// Quits game after little delay.  Waiting for poup animation to get completed.
        /// </summary>
        void QuitGameAfterDelay()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_ANDROID
                //On Android, on quitting app, App actully won't quit but will be sent to background. So it can be load fast while reopening. 
				AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				activity.Call<bool>("moveTaskToBack" , true); 
            #elif UNITY_IOS
				Application.Quit();
            #endif
        }

        /// <summary>
        /// Show common pop-up. 
        /// </summary>
        public void ShowMessage(string title, string message)
        {
            new CommonDialogueInfo().SetTitle(title).
            SetMessage(message).
            SetMessageType(CommonDialogueMessageType.Info).
            SetOnConfirmButtonClickListener(() => {
                UIController.Instance.commonMessageScreen.Deactivate();
            }).Show();
        }

        /// <summary>
        /// Unload unused asset. please call this on safe place as it might give a slight lag. 
        /// </summary>
        public void ClearCache()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        /// <summary>
        /// Shows Consent Dialogue.
        /// </summary>
        public void ShowConsentDialogue()
        {
            orderedPopupStack.Add(consentScreen.name);
            ShowDailogFromStack();
        }

        /// <summary>
        /// Opens Daily Reward screen if day has changed.
        /// </summary>
        public void ShowDailyRewardsPopup()
        {
            orderedPopupStack.Add(dailyRewardScreen.name);
            ShowDailogFromStack();
        }

        /// <summary>
        /// Open review popup if all conditions to show review screen satisfies.
        /// </summary>
        private void ShowReviewPopup()
        {
            bool canShowReviewPopup = false;
           
            #if UNITY_IOS
            canShowReviewPopup = UnityEngine.iOS.Device.RequestStoreReview();
           
            if (canShowReviewPopup)
            {
                PlayerPrefs.SetInt("AppRated", 1);
            }
            #endif

            if (!canShowReviewPopup) {
                //orderedPopupStack.Add(reviewScreen.name);
                ShowDailogFromStack();
            }
        }

        /// <summary>
        /// Controlls the ordered stack.
        /// </summary>
        void ShowDailogFromStack()
        {
            if (orderedPopupStack.Count > 0)
            {
                string screenName = orderedPopupStack[0];

                switch (screenName)
                {
                    case "ConsentSetting":
                        if (!consentScreen.activeSelf)
                        {
                            consentScreen.Activate();
                        }
                        break;

                    case "DailyRewardScreen":
                        if (!dailyRewardScreen.activeSelf)
                        {
                            dailyRewardScreen.Activate();
                        }

                        break;

                    case "ReviewAppScreen":
                        if (!reviewScreen.activeSelf)
                        {
                            //reviewScreen.Activate();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Disables home and select mode screen and opens gameplay.
        /// </summary>
        public void LoadGamePlay(GameMode gameMode)
        {
            TargetController.Instance.DestroyTargetsOnReloadLevel();

            homeScreen.gameObject.Deactivate();
            selectModeScreen.gameObject.Deactivate();

            topPanelWithHearts.gameObject.SetActive(false);
            topPanelWithModeContext.gameObject.SetActive(true);
            gameScreen.gameObject.Activate();
            topPanelWithModeContext.SetModeText(gameMode);
            if (topPanelWithModeContext.settingsPopUp.activeSelf)
            {
                topPanelWithModeContext.settingsPopUp.SetActive(false);
            }
            GamePlayUI.Instance.ResetGame();
            gameScreen.GetComponent<GamePlayUI>().StartGamePlay(gameMode);          
        }

        #region MyChanges
        /// <summary>
        /// Disables home, modes screen and select level selection screen.
        /// </summary>
        public void LoadLevelSelectionScreen()
        {
            StartCoroutine(LoadLevelSelectionScreenCoroutine());
        }

        IEnumerator LoadLevelSelectionScreenCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            homeScreen.gameObject.Deactivate();
            selectModeScreen.gameObject.Deactivate();
            topPanelWithHearts.gameObject.SetActive(true);
            topPanelWithModeContext.gameObject.SetActive(false);
            levelSelectionScreen.gameObject.Activate();
        }

        /// <summary>
        /// Disables game screen,game win screen and select level selection screen.
        /// </summary>
        public void LoadLevelSelectionScreenFromGameWin()
        {
            GamePlayUI.Instance.ResetGame();
            gameWinScreen.gameObject.SetActive(false);
            gameScreen.gameObject.SetActive(false);
            UIController.Instance.Pop("GamePlay");
            topPanelWithHearts.gameObject.SetActive(true);
            topPanelWithModeContext.gameObject.SetActive(false);
            levelSelectionScreen.gameObject.Activate();
            BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
            BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
        }

        /// <summary>
        /// Disables game screen,game over and select level selection screen.
        /// </summary>
        public void LoadLevelSelectionScreenFromGameOver()
        {
            gameOverScreen.gameObject.SetActive(false);
            UIController.Instance.Pop("GameOver");
            GamePlayUI.Instance.ResetGame();
            gameScreen.gameObject.SetActive(false);
            UIController.Instance.Pop("GamePlay");
            topPanelWithHearts.gameObject.SetActive(true);
            topPanelWithModeContext.gameObject.SetActive(false);
            levelSelectionScreen.gameObject.Activate();
            BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
            BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
        }

        /// <summary>
        /// Open Home screen when user presses cancel button from level selection screen.
        /// </summary>
        public void OpenScreenFromTopPanel()
        {
            StartCoroutine(OpenScreenFromTopPanelCoroutine());
        }

        IEnumerator OpenScreenFromTopPanelCoroutine()
        {
            if(Peek() != "ShopScreen" && GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                GamePlayUI.Instance.ResetGame();
            }
            
            yield return new WaitForSeconds(0.1f);
            if (Peek() == "LevelSelectionScreen")
            {
                topPanelWithHearts.gameObject.SetActive(false);
                levelSelectionScreen.gameObject.Deactivate();
                homeScreen.gameObject.Activate();
            }
            else if(Peek() == "GamePlay" || Peek() == "GameQuitAndRetry")
            {
                GamePlayUI.Instance.scorePanel.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                GamePlayUI.Instance.targetPanel.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;

                if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                {
                    gameScreen.gameObject.Deactivate();
                    topPanelWithHearts.gameObject.SetActive(true);
                    topPanelWithModeContext.gameObject.SetActive(false);
                    levelSelectionScreen.gameObject.Activate();
                }
                else
                {
                    gameScreen.gameObject.SetActive(false);
                    UIController.Instance.Pop("GamePlay");
                    topPanelWithModeContext.gameObject.SetActive(false);
                    if (tipView.activeSelf)
                    {
                        tipView.Deactivate();
                    }
                    homeScreen.gameObject.Activate();
                }
            }
            else if (Peek() == "ShopScreen")
            {
                CloseShopScreen();
            }
        }

        public void CloseShopScreen()
        {
            shopScreen.gameObject.Deactivate();
            rescueGameScreen.gameObject.GetComponent<Canvas>().sortingOrder = 4;
            if (prevScreen == "MoreLivesScreen")
            {
                healthPanel.GetComponent<Canvas>().sortingOrder = 5;
            }
            else if (prevScreen == "GamePlay" || prevScreen == "RescueGame")
            {
                BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
                BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
                topPanelWithHearts.gameObject.SetActive(false);
                topPanelWithModeContext.gameObject.SetActive(true);
                if (rescueGameScreen.activeSelf)
                {
                    EnableCurrencyBalanceButton();
                }
            }
            
            topPanelWithHearts.GetComponent<Canvas>().sortingOrder = 3;
        }

        public void OpenHomeScreenFromModeSelection()
        {
            StartCoroutine(OpenHomeScreenFromModeSelectionCoroutine());
        }

        IEnumerator OpenHomeScreenFromModeSelectionCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            selectModeScreen.gameObject.Deactivate();
            homeScreen.gameObject.Activate();
        }


        public void LoadGoalScreenFromLevelSelection(int levelNumber)
        {
            StartCoroutine(LoadGoalScreenFromLevelSelectionCoroutine(levelNumber));
        }
        IEnumerator LoadGoalScreenFromLevelSelectionCoroutine(int levelNumber)
        {
            yield return new WaitForSeconds(0.1f);
            goalScreen.gameObject.Activate();
            goalScreen.SetGoals(levelNumber);
            DynamicOfferService.Instance.ShowGoalScreenOffer();
        }

        public void OpenGameRetryAndQuit(string title)
        {
            StartCoroutine(OpenGameRetryAndQuitCoroutine(title));
        }

        IEnumerator OpenGameRetryAndQuitCoroutine(string title)
        {
            yield return new WaitForSeconds(0.1f);
            //gameRetryAndQuitScreen.gameObject.SetActive(true);
            gameRetryAndQuitScreen.gameObject.Activate();
            gameRetryAndQuitScreen.SetTitleText(title);
            DynamicOfferService.Instance.ShowLevelQuitAndRetryOffer();
        }

        public void OpenGameWinScreen()
        {
            StartCoroutine(OpenGameWinScreenCoroutine());
        }

        IEnumerator OpenGameWinScreenCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            gameWinScreen.gameObject.SetActive(true);
            DynamicOfferService.Instance.ShowLevelWinOffer();
        }

        public void OpenSettingsScreen()
        {
            StartCoroutine(OpenSettingsScreenCoroutine());
        }

        IEnumerator OpenSettingsScreenCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            settingScreen.gameObject.Activate();
        }

        public void OpenAppReviewScreen()
        {
            StartCoroutine(OpenAppReviewScreenCoroutine());
        }

        IEnumerator OpenAppReviewScreenCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            reviewScreen.gameObject.SetActive(true);
        }

        public void OpenPowerUpContextPanel(PowerUp powerUpName)
        {
            powerUpContextPanel.gameObject.Activate();
            powerUpContextPanel.SetPanelContext(powerUpName);
        }

        public void LoadFirstLevelFromHomeScreen()
        {
            PlayerPrefs.SetInt("firstLevelShown", 1);
            topPanelWithModeContext.gameObject.SetActive(true);
            //homeScreen.gameObject.Deactivate();
            if (homeScreen.gameObject.activeSelf)
            {
                homeScreen.gameObject.Deactivate();
            }
            gameOverScreen.Deactivate();
            gameScreen.GetComponent<GamePlayUI>().level = 1;
            gameScreen.GetComponent<GamePlayUI>().levelToLoad = GamePlayUI.Instance.gamePlaySettings.levelToLoadInfo[0];
            topPanelWithModeContext.SetModeText(GameMode.Level);
            
            if (topPanelWithModeContext.settingsPopUp.activeSelf)
            {
                topPanelWithModeContext.settingsPopUp.SetActive(false);
            }
            if (powerUpContextPanel.gameObject.activeSelf)
            {
                powerUpContextPanel.gameObject.Deactivate();
            }
            gameScreen.gameObject.Activate();
            TargetController.Instance.DestroyTargetsOnReloadLevel();
            gameScreen.GetComponent<Animator>().SetTrigger("GamePlay");
            BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
            BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
            GamePlayUI.Instance.currentGameMode = GameMode.Level;
            GamePlayUI.Instance.RestartGame();
            //gameScreen.GetComponent<GamePlayUI>().StartGamePlay(gameMode);
            guideScreen.gameObject.SetActive(GamePlayUI.Instance.CheckGuideScreenForLevel());
        }

        public void LoadGamePlayFromLevelSelection(GameMode gameMode, int levelNumber)
        {
            topPanelWithHearts.gameObject.SetActive(false);
            topPanelWithModeContext.gameObject.SetActive(true);
            levelSelectionScreen.gameObject.Deactivate();

            gameScreen.GetComponent<GamePlayUI>().level = levelNumber;
            gameScreen.GetComponent<GamePlayUI>().levelToLoad = GamePlayUI.Instance.gamePlaySettings.levelToLoadInfo[levelNumber - 1];
            topPanelWithModeContext.SetModeText(gameMode);
            if (topPanelWithModeContext.settingsPopUp.activeSelf)
            {
                topPanelWithModeContext.settingsPopUp.SetActive(false);
            }
            if (powerUpContextPanel.gameObject.activeSelf)
            {
                powerUpContextPanel.gameObject.Deactivate();
            }
            gameScreen.gameObject.Activate();
            TargetController.Instance.DestroyTargetsOnReloadLevel();
            gameScreen.GetComponent<Animator>().SetTrigger("GamePlay");
            BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
            BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
            GamePlayUI.Instance.currentGameMode = gameMode;
            GamePlayUI.Instance.RestartGame();
            //gameScreen.GetComponent<GamePlayUI>().StartGamePlay(gameMode);
            guideScreen.gameObject.SetActive(GamePlayUI.Instance.CheckGuideScreenForLevel());
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "LimitedMoves")
            {
                movesGuideScreen.gameObject.SetActive(GamePlayUI.Instance.CheckMovesGuideScreenForLevel());
            }
        }

        public void LoadGameplayFromTutorial(GameMode gameMode, int levelNumber)
        {
            gameScreen_Tutorial.gameObject.Deactivate();
            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
            {
                PlayerPrefs.SetInt("CurrentLevel", 200);
            }
            topPanelWithModeContext.gameObject.SetActive(true);
            gameScreen.GetComponent<GamePlayUI>().level = levelNumber;
            gameScreen.GetComponent<GamePlayUI>().levelToLoad = GamePlayUI.Instance.gamePlaySettings.levelToLoadInfo[levelNumber-1];
            topPanelWithModeContext.SetModeText(gameMode);
            if (topPanelWithModeContext.settingsPopUp.activeSelf)
            {
                topPanelWithModeContext.settingsPopUp.SetActive(false);
            }
            gameScreen.gameObject.Activate();
            TargetController.Instance.DestroyTargetsOnReloadLevel();
            gameScreen.GetComponent<Animator>().SetTrigger("GamePlay");
            BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
            BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = 1;
            GamePlayUI.Instance.currentGameMode = gameMode;
            GamePlayUI.Instance.RestartGame();
            guideScreen.gameObject.SetActive(GamePlayUI.Instance.CheckGuideScreenForLevel());
        }

        public void LoadClassicFromTutorial()
        {
            gameScreen_Tutorial.gameObject.Deactivate();
            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
            {
                PlayerPrefs.SetInt("CurrentLevel", 1);
            }
            topPanelWithModeContext.gameObject.SetActive(true);
            topPanelWithModeContext.SetModeText(GameMode.Classic);
            if (topPanelWithModeContext.settingsPopUp.activeSelf)
            {
                topPanelWithModeContext.settingsPopUp.SetActive(false);
            }
            gameScreen.gameObject.Activate();
            gameScreen.GetComponent<Animator>().SetTrigger("GamePlay");
            GamePlayUI.Instance.currentGameMode = GameMode.Classic;
            GamePlayUI.Instance.RestartGame();
        }

        public void OpenContextualOfferScreen()
        {
            contextualOfferScreen.Activate();
        }

        public void CloseContextualOfferScreen(GameObject activeGameObject)
        {
            activeGameObject.gameObject.SetActive(false);
            contextualOfferScreen.Deactivate();
        }

        public void OpenShopScreen()
        {
            StartCoroutine(OpenShopScreenCoroutine());
        }
        IEnumerator OpenShopScreenCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            topPanelWithHearts.GetComponent<Canvas>().sortingOrder = 4;
            if(!shopScreen.gameObject.activeSelf)
            {
                prevScreen = Peek();
            }
            if (Peek() == "LevelSelectionScreen")
            {
                shopScreen.gameObject.Activate();
            }
            else if(Peek() == "GamePlay")
            {
                BoxingGlove.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                BombPowerUps.Instance.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                topPanelWithModeContext.gameObject.SetActive(false);
                topPanelWithHearts.gameObject.SetActive(true);
                
                shopScreen.gameObject.Activate();
                rescueGameScreen.gameObject.GetComponent<Canvas>().sortingOrder = 0;
            }
            else if (Peek() == "RescueGame")
            {
                topPanelWithModeContext.gameObject.SetActive(false);
                topPanelWithHearts.gameObject.SetActive(true);
                shopScreen.gameObject.Activate();
                rescueGameScreen.gameObject.GetComponent<Canvas>().sortingOrder = 0;
            }
            else if(Peek() == "MoreLivesScreen")
            {
                healthPanel.gameObject.GetComponent<Canvas>().sortingOrder = 0;
                topPanelWithModeContext.gameObject.SetActive(false);
                topPanelWithHearts.gameObject.SetActive(true);

                shopScreen.gameObject.Activate();
            }
        }

        public Transform GetMaxMovesTransform()
        {
            return movesCountTxt.transform;
        }

        public IEnumerator DeactivateBoomRangHUD()
        {
            yield return new WaitForSeconds(0.9f);
            boomrangHUD.SetActive(false);
        }
        #endregion

        public bool IsGamePlay()
        {
            if (Peek().Equals(gameScreen.name))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Open Home screen when user presses home button from gameover screen.
        /// </summary>
        public void OpenHomeScreenFromGameOver()
        {
            GamePlayUI.Instance.ResetGame();
            topPanelWithHearts.gameObject.SetActive(false);
            topPanelWithModeContext.gameObject.SetActive(false);
            gameScreen.gameObject.SetActive(false);
            gameOverScreen.SetActive(false);
            UIController.Instance.Pop("GameOver");
            homeScreen.gameObject.Activate();
            //selectModeScreen.Activate();
        }

        public void GameWinUIToggle()
        {
            StartCoroutine(GameWinUIToggleCoroutine());
        }

        IEnumerator GameWinUIToggleCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            if (UIController.Instance.adsPanel.activeSelf)
            {
                UIController.Instance.adsPanel.SetActive(false);
                UIController.Instance.adsOverTxt.SetActive(true);
            }
        }

        #region Live Panel
        public void OpenLivesPanel()
        {
            if (HealthController.Instance.currentEnergy < 5)
            {
                StartCoroutine(OpenLivesPanelCoroutine());
            }
        }

        IEnumerator OpenLivesPanelCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            healthPanel.gameObject.Activate();
        }
        #endregion

        /// <summary>
        /// Open Home screen when user presses home button from pause screen during gameplay.
        /// </summary>
        public void OpenHomeScreenFromPauseGame() {
            StartCoroutine(OpenHomeScreenFromPauseGameCoroutine());
        }

        IEnumerator OpenHomeScreenFromPauseGameCoroutine()
        {
            GamePlayUI.Instance.ResetGame();
            yield return new WaitForSeconds(0.1f);
            gameScreen.gameObject.Deactivate();
            pauseGameScreen.Deactivate();
            homeScreen.gameObject.Activate();
        }

         /// <summary>
        /// Enables currency balance button. Currency balance button will be shown during shop screen, reward adding or reducing current only.
        /// </summary>
        public void EnableCurrencyBalanceButton() {
            currencyBalanceButton.GetComponent<CanvasGroup>().SetAlpha(1, 0.3F);
        }

        /// <summary>
        /// Disable currency balance button.
        /// </summary>
        public void DisableCurrencyBalanceButton()
        {
            if(! (selectThemeScreen.activeSelf || /*rescueGameScreen.activeSelf || /*gameOverScreen.activeSelf ||*/ shopScreen.gameObject.activeSelf || purchaseSuccessScreen.activeSelf))
            {
                if (currencyBalanceButton != null && currencyBalanceButton.activeSelf) {
                    currencyBalanceButton.GetComponent<CanvasGroup>().SetAlpha(0, 0.3F);
                }
            }
        }

        public void PlayAddCoinsAnimationAtPosition(Vector3 position, Vector3 destinationPos, float delay, Sprite[] sprite) 
        {
            GameObject rewardAnim = (GameObject)Instantiate(Resources.Load("RewardAnimation")) as GameObject;
            rewardAnim.transform.SetParent(RuntimeEffectSpawnParent);
            rewardAnim.GetComponent<RectTransform>().position = position;
            rewardAnim.transform.localScale = Vector3.one;
            rewardAnim.GetComponent<RewardAddAnimation>().PlayCoinsBalanceUpdateAnimation(destinationPos, delay, sprite);
        }

        public void PlayDeductCoinsAnimation(Vector3 position, float delay) {
            GameObject rewardAnim = (GameObject)Instantiate(Resources.Load("RewardAnimation")) as GameObject;
            rewardAnim.transform.SetParent(RuntimeEffectSpawnParent);
            rewardAnim.GetComponent<RectTransform>().position = ShopButtonCoinsIcon.position;
            rewardAnim.transform.localScale = Vector3.one;
            rewardAnim.GetComponent<RewardAddAnimation>().PlayCoinsBalanceUpdateAnimation(position, delay, null);
        }
        
        public void ShowTipWithTextIdAtPosition(Vector2 tipPosition, Vector2 anchor, string tipText) {
			ShowTipAtPosition(tipPosition, anchor, LocalizationManager.Instance.GetTextWithTag(tipText));
		}

		public void ShowTipWithTextIdAtPosition(Vector2 tipPosition, Vector2 anchor, string tipText, float duration) {
            ShowTipAtPosition(tipPosition, anchor, LocalizationManager.Instance.GetTextWithTag(tipText), duration);
		}

        public void ShowTipAtPosition(Vector2 tipPosition, Vector2 anchor, string tipText) {
			tipView.GetComponent<TipView>().ShowTipAtPosition(tipPosition, anchor, tipText);
            tipView.Activate(false);
		}

		public void ShowTipAtPosition(Vector2 tipPosition, Vector2 anchor, string tipText, float duration) {
            tipView.GetComponent<TipView>().ShowTipAtPosition(tipPosition, anchor, tipText, duration);
            tipView.Activate(false);
		}

        public void ShowTimeModeTip() {
             if(!PlayerPrefs.HasKey("timeTip")) 
            {
                UIController.Instance.ShowTipWithTextIdAtPosition(new Vector2(0, -1475F), new Vector2(0.5F, 1), "txtTimeTip1", 6F);
                Invoke("ShowTimeModeTip2",5F);
            }
        }

        void ShowTimeModeTip2() {
            UIController.Instance.ShowTipWithTextIdAtPosition(new Vector2(0, -1475F), new Vector2(0.5F, 1), "txtTimeTip2", 7F);
            PlayerPrefs.SetInt("timeTip",1);
        }

        public void ShowBombPlaceTip() {
            UIController.Instance.ShowTipWithTextIdAtPosition(new Vector2(0,520F), new Vector2(0.5F, 0), "txtBombTip", 4.5F);
        }

        public void ShowGotRewardTip()
        {
            StartCoroutine(ShowGotRewardTipCoroutine());
        }

        IEnumerator ShowGotRewardTipCoroutine()
        {
            txtGotReward.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            txtGotReward.gameObject.SetActive(false);
        }

        public void UpdateMovesCount(int val)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name != "LimitedMoves")
            {
                return;
            }
            else
            {
                if (val <= 0)
                {
                    val = 0;
                }
                movesCountTxt.transform.LocalScale(Vector3.one * 1.2F, 0.2F).OnComplete(() =>
                {
                    movesCountTxt.text = val.ToString();
                    movesCountTxt.transform.LocalScale(Vector3.one, 0.2F);
                });
            }
        }
        
    }
}