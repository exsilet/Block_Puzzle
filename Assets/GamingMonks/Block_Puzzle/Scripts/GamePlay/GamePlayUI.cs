using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace GamingMonks
{
    public enum GameOverReason 
    {
        GRID_FILLED,    // If there is no enough space to place existing blocks. Applies to all game mode.
        TIME_OVER,      // If timer finishing. Applied only to time mode.
        BOMB_BLAST,      // If Counter on placed bomb reaches to 0. Applies only to blast mode.
        OUT_OF_MOVE     // If player failed to achieve the target in the given max moves.
    }

    public class GamePlayUI : Singleton<GamePlayUI>
    {
        [SerializeField] private Image _fontPole8;
        [SerializeField] private Image _fontPole9;
        [SerializeField] private Image _fontPole10;

        [Header("Public Class Members")]
        [Tooltip("GamePlay Script Reference")]
        public GamePlay gamePlay;

        [Tooltip("ScoreManager Script Reference")]
        public ScoreManager scoreManager;
        public GameObject scorePanel;
        public GameObject targetPanel;

        [Tooltip("PowerUps Panel GameObject Reference")]
        public GameObject powerUpsPanel;

        [Tooltip("ProgressData Script Reference")]
        public ProgressData progressData;

        [Tooltip("TimeModeProgresssBar Script Reference")]
        public TimeModeProgresssBar timeModeProgresssBar;

        [Tooltip("InGameMessage Script Reference To Show Message")]
        public InGameMessage inGameMessage;

        public Canvas screensCanvas;
        
        private LevelSO levelSO;

        [System.NonSerialized] public GameModeSettings currentModeSettings;

        // Stores current playing mode.
        [System.NonSerialized] public GameMode currentGameMode;

        // GamePlay Setting Scriptable Instance. Initializes on awake.
        public GamePlaySettings gamePlaySettings
        {
            get; private set;   
        }

        #region  Game Status event callbacks.
        //Event action for game start callback.
        public static event Action<GameMode> OnGameStartedEvent;

        //Event action for shape place callback.
        public static event Action OnShapePlacedEvent;
        
        //Event action for game finish callback.
        public static event Action<GameMode> OnGameOverEvent;

        //Event action for game win callback.
        public static event Action<GameMode> OnGameWinEvent;

        //Event action for game pause callback.
        public static event Action<GameMode, bool> OnGamePausedEvent;

        //Event action for guide Screen.
        public static event Action OnGuideScreenEvent;
        #endregion

        public bool isGameWon = false;
        
        // Total lines clear during gameplay.
        [HideInInspector] public int totalLinesCompleted = 0;

        // Resuce used for the game or not.
        [HideInInspector] public bool rescueDone = false;

        // Reason for game over. Will Initialize at game over or rescue.
        [HideInInspector] public GameOverReason currentGameOverReason;

        public Level currentLevel { get; private set; }
        public int level;
        public int levelToLoad;
        public int debugTest = 0;
        public bool gameWinReward = false;

        private float m_levelStartingTime;
        private float m_levelCompletingTime;

        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake() {
            // Initializes the GamePlay Settings Scriptable.
            if (gamePlaySettings == null)  {
                gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings");
            }
            if (levelSO == null)
            {
                levelSO = (LevelSO)Resources.Load("LevelSettings");
            }
            if (debugTest != 0)
            {
                PlayerPrefs.SetInt("CurrentLevel", debugTest);
            }
            
            // scorePanel.gameObject.GetComponent<Canvas>().sortingOrder = -1;
            // targetPanel.gameObject.GetComponent<Canvas>().sortingOrder = -1;
        }

        /// <summary>
        /// Starts game with selected game mode.
        /// </summary>
        public void StartGamePlay(GameMode gameMode)
        {
            AdmobManager.Instance.isRewardedShown = false;
            if (gameMode == GameMode.Level)
            {
                //if (PlayerPrefs.HasKey("IsIAPpurchased") && PlayerPrefs.GetInt("IsIAPpurchased") == 1)
                //{
                    int attemptsRemaining = PlayerPrefs.GetInt("AttemptsToPlayLevelWithoutAds");
                    PlayerPrefs.SetInt("AttemptsToPlayLevelWithoutAds", attemptsRemaining - 1);
                //}

                currentGameMode = GameMode.Level;
                currentModeSettings = levelSO.Levels[levelToLoad - 1].Mode.GameModeSettings;
                FonGenerate(currentModeSettings);
                currentLevel = levelSO.Levels[levelToLoad-1];
                GamePlay.Instance.allShapes.Clear();
                gamePlay.maxMovesAllowed = currentModeSettings.maxMoves;
                gamePlay.maxMovesAllowed += gamePlaySettings.globalLevelModeMaxMovesOffset;
                UIController.Instance.movesCountTxt.gameObject.SetActive(true);
                UIController.Instance.movesText.gameObject.SetActive(true);
                UIController.Instance.UpdateMovesCount(gamePlay.maxMovesAllowed);
                
                scorePanel.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                targetPanel.gameObject.GetComponent<Canvas>().sortingOrder = 1;
                TargetController.Instance.InitializeTargets(currentLevel.Goal);
                GameProgressTracker.Instance.DeleteProgress(currentGameMode);
                m_levelStartingTime = Time.realtimeSinceStartup;
            }
            else
            {
                currentGameMode = gameMode;
                currentModeSettings = GetCurrentModeSettings();
                FonGenerate(currentModeSettings);

                scorePanel.gameObject.GetComponent<Canvas>().sortingOrder = 1;
                targetPanel.gameObject.GetComponent<Canvas>().sortingOrder = -1;
                //gamePlay.ResetGame();
                //scoreManager.ResetGame();
                //progressData = null;
            }

            //toggle powerups panel
            powerUpsPanel.SetActive(gameMode == GameMode.Level);
            
            if (!gamePlay.blockShapeController.gameObject.activeSelf)
            {
                gamePlay.blockShapeController.gameObject.SetActive(true);
            }

            //GameProgressTracker.Instance.DeleteProgress(currentGameMode);
            // Checks if the there is user progerss from previos session.
            bool hasPreviosSessionProgress = GameProgressTracker.Instance.HasGameProgress(currentGameMode);
            if (hasPreviosSessionProgress) {
                progressData = GameProgressTracker.Instance.GetGameProgress(currentGameMode);
            }

            // Enables gameplay screen if not active.
            if (!gamePlay.gameObject.activeSelf) {
                gamePlay.gameObject.SetActive(true);
            }

            // Generated gameplay grid.
            gamePlay.boardGenerator.GenerateBoard(progressData);

            // gamePlay.blockShapeController.PrepareShapeContainer(progressData);
            //}
            
            #region Time Mode Specific
            // Will enable timer start seeking it. If there is previos session data then timer will start from remaining duration.
            //if(gameMode == GameMode.Timed) {
            if (currentGameMode == GameMode.Timed)
            {
                timeModeProgresssBar.gameObject.SetActive(true);
                timeModeProgresssBar.SetTimer((progressData != null) ? progressData.remainingTimer : timeModeInitialTimer);
                timeModeProgresssBar.StartTimer();
            } else {
                if (timeModeProgresssBar.gameObject.activeSelf) {
                    timeModeProgresssBar.gameObject.SetActive(false);
                }
            }

            if(progressData != null) {
                totalLinesCompleted = progressData.totalLinesCompleted;
                rescueDone = progressData.rescueDone;
            }
            #endregion


            // Invokes Game Start Event Callback.
            if(OnGameStartedEvent != null) {
                OnGameStartedEvent.Invoke(currentGameMode);
            }

            GameProgressTracker.Instance.UpdateGameModePlayedCount(gameMode);
            AnalyticsManager.Instance.GamePlayedEvent();
            if(gameMode == GameMode.Classic)
            {
                //FBManeger.Instance.ClassicModePlayed();
            }
            ShowInitialTip();
        }

        void ShowInitialTip() {
             
             switch(currentGameMode) {
                 case GameMode.Timed:
                    UIController.Instance.ShowTimeModeTip();
                 break;
             }
        }


        /// <summary>
        /// Returns size of the current grid.
        /// </summary>
        /// <returns></returns>
        public BoardSize GetBoardSize() {
            return currentModeSettings.boardSize;
        }

        /// <summary>
        /// Returns game settings for the current game mode.
        /// </summary>
        GameModeSettings GetCurrentModeSettings()
        {
            switch (currentGameMode)
            {
                case GameMode.Classic:
                    return gamePlaySettings.classicModeSettings;
                case GameMode.Timed:
                    return gamePlaySettings.timeModeSettings;
                case GameMode.Blast:
                    return gamePlaySettings.blastModeSettings;
                case GameMode.Advance:
                    return gamePlaySettings.advancedModeSettings;
            }
            return gamePlaySettings.classicModeSettings;
        }

        public List<BlockShapeInfo> GetFixedBlockShapesInfo()
        {
            return levelSO.Levels[levelToLoad - 1].FixedBlockShape.ToList();
        }

        public List<BlockShapeInfo> GetStandardBlockShapes()
        {
            if (currentGameMode == GameMode.Level)
            {
                return levelSO.Levels[levelToLoad - 1].standardBlockShapesInfo.ToList();
            }
            return gamePlaySettings.standardBlockShapesInfo.ToList();
        }

        // Returns of list of all advanced block shapes.
        public List<BlockShapeInfo> GetAdvancedBlockShapes()
        {
            if (currentGameMode == GameMode.Level)
            {
                Debug.Log(level);
                Debug.Log(levelToLoad);
                return levelSO.Levels[levelToLoad - 1].advancedBlockShapesInfo.ToList();
            }
            return gamePlaySettings.advancedBlockShapesInfo.ToList();
        }

        public List<BlockShapeProbabilityInfo> GetLevelBlockShapeInfo()
        {
            BlockShapeLevelListInfo[] blockInfo = null;
            BlockShapeLevelInfo blockShapeLevelInfo;

            for (int i = 0; i < gamePlaySettings.blockShapeLevelInfo.Length; i++)
            {
                blockShapeLevelInfo = gamePlaySettings.blockShapeLevelInfo[i];
                if (levelToLoad >= blockShapeLevelInfo.minLevel && levelToLoad <= blockShapeLevelInfo.maxLevel)
                {
                    blockInfo = new BlockShapeLevelListInfo[blockShapeLevelInfo.blockShapeInfoList.Length];
                    blockInfo = blockShapeLevelInfo.blockShapeInfoList;
                    break;
                }
            }
            
            List<BlockShapeProbabilityInfo> blockShapeInfo = new List<BlockShapeProbabilityInfo>();
            BlockShapeProbabilityInfo info;
           
            if(blockInfo != null)
            {
                for (int i = 0; i < blockInfo.Length; i++)
                {
                    if (blockInfo[i] != null && blockInfo[i].spawnProbability >= 1)
                    {
                        info = new BlockShapeProbabilityInfo();
                        info.blockShape = blockInfo[i].blockShape;
                        info.spawnProbality = blockInfo[i].spawnProbability;
                        info.spawnCount = 0;
                        blockShapeInfo.Add(info);
                    }
                }
            }
            return blockShapeInfo;
        }

        public List<string> GetColoursTag()
        {
            List<string> colourTags = new List<string>();
            if(currentGameMode == GameMode.Level)
            {
                if(currentModeSettings.pickShapeColorsFromThisLevel)
                {
                    SpriteInfo[] spriteInfos = currentLevel.spriteInfo;

                    for (int i = 0; i < spriteInfos.Length; i++)
                    {
                        for (int j = 0; j < spriteInfos[i].probability; j++)
                        {
                            colourTags.Add(spriteInfos[i].spriteTag);
                        }
                    }
                    return colourTags;
                }
                colourTags.AddRange(gamePlaySettings.blockSpriteColorTags);
                return colourTags;
            }
            return gamePlaySettings.blockSpriteColorTags.ToList();
        }

        // Returns of list of all standard block shapes.
        public List<BlockShapeInfo> GetStandardBlockShapesInfo() {
            return gamePlaySettings.standardBlockShapesInfo.ToList();
        }

        // Returns of list of all advanced block shapes.
        public List<BlockShapeInfo> GetAdvancedBlockShapesInfo() {
            return gamePlaySettings.advancedBlockShapesInfo.ToList();
        }

        public BlockShapeInfo GetSingleBlockShape()
        {
            return gamePlaySettings.standardBlockShapesInfo[0];
        }

        // Returns score to be added on for each block cleared.
        public int blockScore {
            get {
                return gamePlaySettings.blockScore;
            }
        }

        // Returns score to be added on for each line cleared.
        public int singleLineBreakScore
        {
            get  {
                return gamePlaySettings.singleLineBreakScore;
            }
        }

        // Returns score multiplier to be added on for each line cleared when more than 1 lines cleared together.
        public int multiLineScoreMultiplier {
            get {
                return gamePlaySettings.multiLineScoreMultiplier;
            }
        }

        #region Time Mode Specific
        // Returns Intial timer for time mode.
        public float timeModeInitialTimer {
            get {
                return gamePlaySettings.initialTime;
            }
        }

        // Returns seconds to be added 
        public float timeModeAddSecondsOnLineBreak {
            get {
                return gamePlaySettings.addSecondsOnLineBreak;
            }
        }
        #endregion

        #region Blast Mode Specific
        // Returns initial counter when on bomb.
        public int blastModeCounter {
            get {
                return gamePlaySettings.blastModeCounter;
            }
        }

        //Retuens after how many block shape place new bomb should be placed.
        public int addBombAfterMoves {
            get {
                return gamePlaySettings.addBombAfterMoves;
            }
        }
        #endregion

        public bool rewardOnGameOver {
            get {
                return gamePlaySettings.rewardOnGameOver;
            }
        }

        public bool giveFixedReward { 
            get {
                return gamePlaySettings.giveFixedReward;
            }
        }

        public int fixedRewardAmount {
            get {
                return gamePlaySettings.fixedRewardAmount;
            }
        }

        public float rewardPerLine {
            get {
                return gamePlaySettings.rewardPerLineCompleted;
            }
        }

        // Invokes callback for OnShapePlaced Event.
        public void OnShapePlaced() {
            if(OnShapePlacedEvent != null) {
                OnShapePlacedEvent.Invoke();
            }
        }

        public bool CanRescueGame() {
            if(!rescueDone && currentModeSettings.allowRescueGame) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if game can be rescued.
        /// </summary>
        public void TryRescueGame(GameOverReason reason) {
                currentGameOverReason = reason;
            UIController.Instance.topPanelWithModeContext.canShopBeOpened = false;
                StartCoroutine(TryRescueGameEnumerator(reason));
        }

        IEnumerator TryRescueGameEnumerator(GameOverReason reason) 
        {
            //inGameMessage.ShowMessage(reason);
            yield return new WaitForSeconds(1.5F);//1.5f
            UIController.Instance.topPanelWithModeContext.canShopBeOpened = true;
            if (!isGameWon)
            {
                GameProgressTracker.Instance.ClearProgressData();

                if (CanRescueGame())
                {
                    GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
                    UIController.Instance.rescueGameScreen.gameObject.SetActive(true);
                    UIController.Instance.Push("RescueGame");
                    UIController.Instance.rescueGameScreen.GetComponent<RescueGame>().SetRescueReason(reason);
                    DynamicOfferService.Instance.ShowLevelLooseOffer();
                }
                else
                {
                    GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
                    OnGameOver();
                }
            }
        }

        public void OnRescueCancelled() {
            OnGameOver();
        }

        /// <summary>
        /// Resume Game With Rescue Done
        /// </summary>
        public void OnRescueSuccessful() {
            gamePlay.PerformRescueAction(currentGameOverReason);
			rescueDone = true;

            GameProgressTracker.Instance.SaveProgressExplicitly();
            gamePlay.blockShapeController.UpdateShapeContainers();
            ResumeGame();
		}

        /// <summary>
        /// Pauses the game on pressing pause button.
        /// </summary>
        public void OnPauseButtonPressed() {
            if (InputManager.Instance.canInput()) {
                UIFeedback.Instance.PlayButtonPressEffect();
                #region Time Mode Specific
                if(currentGameMode == GameMode.Timed && timeModeProgresssBar.GetRemainingTimer() < 5F) {
                    return;
                }
                #endregion
                UIController.Instance.pauseGameScreen.Activate();
            }
        }

        /// <summary>
        /// Will be called on game over. 
        /// </summary>
        public void OnGameOver() {
            if(OnGameOverEvent != null) {
                OnGameOverEvent.Invoke(currentGameMode);
            }
            m_levelCompletingTime = Time.realtimeSinceStartup;
            AnalyticsManager.Instance.GameLooseEvent();
            if (currentGameMode == GameMode.Level)
            {
                GameProgressTracker.Instance.UpdateFreqToWinLevel(level);
                AnalyticsManager.Instance.OutOfMoveEvent();
                AnalyticsManager.Instance.LevelEvent(level, false, "Lose");
            }

            //if (currentGameMode == GameMode.Level)
            //{
            //    UIController.Instance.LoadLevelSelectionScreenFromGameOver();
            //}
            //else
            //{
                UIController.Instance.gameOverScreen.Activate();
                UIController.Instance.Push("GameOver");
                UIController.Instance.gameOverScreen.GetComponent<GameOver>().SetGameData(currentGameOverReason, scoreManager.GetScore(), totalLinesCompleted, currentGameMode);
                //UIController.Instance.gameOverScreen.Deactivate();
                //UIController.Instance.OpenHomeScreenFromGameOver();
            //}
        }

        /// <summary>
        /// Will be called on game win.
        /// </summary>
        public void OnGameWin()
        {
            //if (GamePlay.Instance.iceBombsOnGrid.Count > 0)
            //{
            //    foreach (var iceBomb in GamePlay.Instance.iceBombsOnGrid)
            //    {
            //        iceBomb.iceSparkParticle.gameObject.SetActive(false);
            //    }
            //}
            if (OnGameWinEvent != null)
            {
                OnGameWinEvent.Invoke(currentGameMode);
            }
            if (PlayerPrefs.GetInt("CurrentLevel") == level && PlayerPrefs.GetInt("CurrentLevel") < 300/*levelSO.Levels.Length*/)
            {
                PlayerPrefs.SetInt("CurrentLevel", level + 1);
                gameWinReward = true;
            }

            if (gamePlay.blockShapeController.gameObject.activeSelf)
            {
                gamePlay.blockShapeController.gameObject.SetActive(false);
            }
            
            if (PowerUpsController.Instance.gameObject.activeSelf)
            {
                PowerUpsController.Instance.gameObject.SetActive(false);
            }

            if (UIController.Instance.shopScreen.gameObject.activeSelf)
            {
                UIController.Instance.CloseShopScreen();
            }
            
            InputManager.Instance.EnableTouch();
            UIController.Instance.OpenGameWinScreen();

            m_levelCompletingTime = Time.realtimeSinceStartup;
            AnalyticsManager.Instance.GameWinEvent();
            if(currentGameMode == GameMode.Level)
            {
                GameProgressTracker.Instance.UpdateFreqToWinLevel(level);
                AnalyticsManager.Instance.LevelEvent(level, true, "Win");
                //FBManeger.Instance.LevelCompleted(level);
                GameProgressTracker.Instance.ResetFreqToWinLevel(level);
            }
        }

        public float GetTimeToCompleteLevel()
        {
            return (m_levelCompletingTime - m_levelStartingTime);
        }
        /// <summary>
        /// Resets game.
        /// </summary>
        public void ResetGame() {
            progressData = null;
            totalLinesCompleted = 0;
            rescueDone = false;
            isGameWon = false;
            gamePlay.ResetGame();
            scoreManager.ResetGame();
            //YandexGame.FullscreenShow();
        }

         #region Time Mode Specific
        /// <summary>
        /// Returns Remaining Timer.
        /// </summary>
        public int GetRemainingTimer() {
            return (currentGameMode == GameMode.Timed) ? timeModeProgresssBar.GetRemainingTimer() : 0;
        }
        #endregion  

        /// <summary>
        /// Pauses game.
        /// </summary>
        public void PauseGame() {
            if(OnGamePausedEvent != null) {
                OnGamePausedEvent.Invoke(currentGameMode, true);
            }
        }
        
        /// <summary>
        /// Resumes game.
        /// </summary>
        public void ResumeGame() {
            if (OnGamePausedEvent != null) {
                OnGamePausedEvent.Invoke(currentGameMode, false);
            }
        }

        /// <summary>
        /// Will rest game to empty state and start new game with same selected mode.
        /// </summary>
        public void RestartGame() {
            GameProgressTracker.Instance.ClearProgressData();
            ResetGame();
            TargetController.Instance.DestroyTargetsOnReloadLevel();
            AdmobManager.Instance.ResetAdPreferences();
            StartGamePlay(currentGameMode);
            //YandexGame.FullscreenShow();
        }

        public string GetGameModeText(GameMode gameMode)
        {
            if (gameMode == GameMode.Level)
            {
                return level.ToString();
            }
            else
            {
                return gameMode.ToString() + "Mode";
            }
        }

        public bool CheckGuideScreenForLevel()
        {
            if (levelSO.Levels[levelToLoad-1].guide.enabled)
            {
                return true;
            }
            return false;
        }

        public bool CheckMovesGuideScreenForLevel()
        {
            if(GamePlayUI.Instance.level==3)
           // if (PlayerPrefs.GetInt("CurrentLevel") == 3)
            {
                return true;
            }
            return false;
        }

        private void FonGenerate(GameModeSettings modeSettings)
        {
            if (modeSettings.boardSize == BoardSize.BoardSize_8X8)
            {
                _fontPole8.gameObject.SetActive(true);
                _fontPole9.gameObject.SetActive(false);
                _fontPole10.gameObject.SetActive(false);
            }
            else if (modeSettings.boardSize == BoardSize.BoardSize_9X9)
            {
                _fontPole8.gameObject.SetActive(false);
                _fontPole9.gameObject.SetActive(true);
                _fontPole10.gameObject.SetActive(false);
            }
            else if (modeSettings.boardSize == BoardSize.BoardSize_10X10)
            {
                _fontPole8.gameObject.SetActive(false);
                _fontPole9.gameObject.SetActive(false);
                _fontPole10.gameObject.SetActive(true);
            }
        }
    }
}
