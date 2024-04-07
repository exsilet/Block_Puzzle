using System;
using UnityEngine;

namespace GamingMonks.Tutorial
{
    public class GamePlayUI : Singleton<GamePlayUI>
    {
        [Header("Public Class Members")]
        [Tooltip("GamePlay Script Reference")]
        public GamePlay gamePlay;

        [System.NonSerialized] public GameModeSettings currentModeSettings;

        // Stores current playing mode.
        [System.NonSerialized] public GameMode currentGameMode;

        // GamePlay Setting Scriptable Instance. Initializes on awake.
        public GamePlaySettings gamePlaySettings
        {
            get;  private set;
        }

        #region  Game Status event callbacks.
        //Event action for shape place callback.
        public static event Action OnShapePlacedEvent;
        #endregion

        public GameObject boardHighlightImage;
        public Vector2 boardHighlightOffset;
        public GameObject shapeDragHandImage;

        //public Text txtTutorialText1;
        //public Text txtTutorialText2;

        public GameObject tutorialCompletePopup;

        int tutorialStep = 1;

        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake() 
        {
            // Initializes the GamePlay Settings Scriptable.
            if (gamePlaySettings == null)  {
                gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings");
            }
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            StartTutorial(GameMode.Tutorial);
        }
       
        /// <summary>
        /// Starts game with selected game mode.
        /// </summary>
        public void StartTutorial(GameMode gameMode) 
        {
            currentGameMode = gameMode;
            currentModeSettings = gamePlaySettings.tutorialModeSettings;

            //This is little static code for the tutorial.
            //currentModeSettings.boardSize = BoardSize.BoardSize_5X5;
            currentModeSettings.boardSize = BoardSize.BoardSize_10X10;

            // Enables gameplay screen if not active.
            if (!gamePlay.gameObject.activeSelf) {
                gamePlay.gameObject.SetActive(true);
            }
            
            // Generated gameplay grid.
            gamePlay.boardGenerator.GenerateBoard();
            gamePlay.boardGenerator.UpdateBoardForTutorial(1);

            // Board Generator will create and initialize board with progress data if available.
            gamePlay.blockShapeController.PrepareShapeContainer(tutorialStep);

            //txtTutorialText1.text = LocalizationManager.Instance.GetTextWithTag("txtTutorial1");
            //txtTutorialText1.SetAlpha(1, 0.3F);

            // Adjust size of highlighting block image.
            boardHighlightImage.transform.localScale = Vector3.one * (currentModeSettings.blockSize / 90F);
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();
            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;
            // Fetched the size of block that should be used.
            float blockSize = GamePlayUI.Instance.currentModeSettings.blockSize;

            // Fetched the space between blocks that should be used.
            float blockSpace = GamePlayUI.Instance.currentModeSettings.blockSpace;

            Vector2 startPosition = new Vector2(GamePlayUI.Instance.GetStartPointX(blockSize,columnSize), GamePlayUI.Instance.GetStartPointY(blockSize,rowSize));
            boardHighlightImage.transform.localPosition = new Vector3(startPosition.x+(4*(blockSize + blockSpace + boardHighlightOffset.x)),startPosition.y-(3 * (blockSize + blockSpace + boardHighlightOffset.y)), 0);
            
        }

        /// <summary>
        /// Horizontal position from where block grid will start.
        /// </summary>
        public float GetStartPointX(float blockSize, int rowSize)
        {
            float totalWidth = (blockSize * rowSize) + ((rowSize - 1) * GamePlayUI.Instance.currentModeSettings.blockSpace);
            return -((totalWidth / 2) - (blockSize / 2));
        }

        /// <summary>
        /// Vertical position from where block grid will start.
        /// </summary>
        public float GetStartPointY(float blockSize, int columnSize)
        {
            float totalHeight = (blockSize * columnSize) + ((columnSize - 1) * GamePlayUI.Instance.currentModeSettings.blockSpace);
            return ((totalHeight / 2) - (blockSize / 2));
        }

        public void TutorialStepCompleted() 
        {
            tutorialStep += 1;
            gamePlay.boardGenerator.UpdateBoardForTutorial(tutorialStep);   
            gamePlay.blockShapeController.PrepareShapeContainer(tutorialStep);

            if (tutorialStep == 2)
            {

                // Adjust size of highlighting block image.
                boardHighlightImage.transform.localScale = Vector3.one * (currentModeSettings.blockSize / 90F) * 1.5f;
                BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();
                int rowSize = (int)boardSize;
                int columnSize = (int)boardSize;
                // Fetched the size of block that should be used.
                float blockSize = GamePlayUI.Instance.currentModeSettings.blockSize;

                // Fetched the space between blocks that should be used.
                float blockSpace = GamePlayUI.Instance.currentModeSettings.blockSpace;

                Vector2 startPosition = new Vector2(GamePlayUI.Instance.GetStartPointX(blockSize, columnSize), GamePlayUI.Instance.GetStartPointY(blockSize, rowSize));
                boardHighlightImage.transform.localPosition = new Vector3(startPosition.x + (4 * (blockSize + blockSpace + boardHighlightOffset.x)) +(blockSize+blockSpace)/2, startPosition.y - (3 * (blockSize + blockSpace + boardHighlightOffset.y)) - (blockSize + blockSpace) / 2, 0);

                //txtTutorialText1.SetAlpha(0, 0);
                //txtTutorialText2.SetAlpha(0, 0);
                //txtTutorialText2.text = "";

                //txtTutorialText1.text = LocalizationManager.Instance.GetTextWithTag("txtTutorial3");
                //txtTutorialText1.SetAlpha(1, 0.3F);

            }
            //if (tutorialStep == 3)
            //{

            //    //txtTutorialText1.SetAlpha(0, 0);
            //    //txtTutorialText2.SetAlpha(0, 0);
            //    //txtTutorialText2.text = "";

            //    //txtTutorialText1.text = LocalizationManager.Instance.GetTextWithTag("txtTutorial3");
            //    //txtTutorialText1.SetAlpha(1, 0.3F);

            //}

            if (tutorialStep == 4) 
            {
                //GamePlay.Instance.boardGenerator.gameObject.SetActive(false);
                //GamePlay.Instance.blockShapeController.gameObject.SetActive(false);

                //boardHighlightImage.SetActive(false);
                //tutorialCompletePopup.Activate(false);
                CurrencyManager.Instance.SavePackValues(0,4,4,4);
                //UIController.Instance.LoadGamePlay(GameMode.Classic);
                UIController.Instance.LoadClassicFromTutorial();
                AnalyticsManager.Instance.TutorialEndedEvent();
                //FBManeger.Instance.CompletedTutorial();
                PlayerPrefs.SetInt("tutorialShown",1);
            }
        }

        public void ContinueGamePlay() {
            if (InputManager.Instance.canInput()) {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.gameScreen_Tutorial.gameObject.Deactivate();
                UIController.Instance.selectModeScreen.gameObject.Activate();
                //UIController.Instance.LoadGamePlay(UIController.Instance.cachedSelectedMode);
            }
        }

        /// <summary>
        /// Returns size of the current grid.
        /// </summary>
        /// <returns></returns>
        public BoardSize GetBoardSize() {
            return currentModeSettings.boardSize;
        }

        // Invokes callback for OnShapePlaced Event.
        public void OnShapePlaced() {


            if(OnShapePlacedEvent != null) {
                OnShapePlacedEvent.Invoke();
            }

            //if(tutorialStep == 1) 
            //{
            //    if(txtTutorialText2.text == "") {
            //        txtTutorialText2.text = LocalizationManager.Instance.GetTextWithTag("txtTutorial2");
            //        txtTutorialText2.SetAlpha(1, 0.3F);
            //    }
            //}
        }
    }
}
