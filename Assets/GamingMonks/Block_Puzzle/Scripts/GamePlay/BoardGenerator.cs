using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    /// <summary>
    /// This script component will generte the board with given size and will also place blocks from previos session if there is progress.
    /// </summary>
	public class BoardGenerator : MonoBehaviour
    {
#pragma warning disable 0649
        // Prefab template of block.
        [SerializeField] GameObject blockTemplate;

        // Parent inside which all blocks will be generated. Typically root of block grid.
        [SerializeField] GameObject blockRoot;

        //public GameObject gridBackground;
        public GameObject gridDesign;
        //public GameObject gridBorder;
        //public float gridBackgroundOffset = 45;
        //public float gridBorderOffset = 45;
        public float gridDesignOffset = 45;

        GamePlaySettings gamePlaySettings;

        private void Awake()
        {
            // Initializes the GamePlay Settings Scriptable.
            if (gamePlaySettings == null)
            {
                gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings");
            }
        }


        /// <summary>
        /// Generates the block grid based on game settings and will also set progress from previoius session if any.
        /// </summary>
        public void GenerateBoard(ProgressData progressData)
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();

            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;

            // Fetched the size of block that should be used.
            float blockSize = GamePlayUI.Instance.currentGameMode == GameMode.Level ? gamePlaySettings.globalLevelModeBlockSize : GamePlayUI.Instance.currentModeSettings.blockSize;
            // Fetched the space between blocks that should be used.
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;

            //Set background and design Grid Size and design type
            //gridBackground.GetComponent<RectTransform>().sizeDelta = new Vector2((rowSize * blockSize) + ((rowSize - 1) * blockSpace) + gridBackgroundOffset, (columnSize * blockSize) + ((columnSize - 1) * blockSpace) + gridBackgroundOffset);
            gridDesign.GetComponent<RectTransform>().sizeDelta = new Vector2((rowSize * blockSize) + ((rowSize - 1) * blockSpace) + gridDesignOffset, (columnSize * blockSize) + ((columnSize - 1) * blockSpace) + gridDesignOffset);
            //gridBorder.GetComponent<RectTransform>().sizeDelta = new Vector2((rowSize * blockSize) + ((rowSize - 1) * blockSpace) + gridBorderOffset, (columnSize * blockSize) + ((columnSize - 1) * blockSpace) + gridBorderOffset);
            gridDesign.GetComponent<Image>().sprite = ThemeManager.Instance.GetBlockSpriteWithTag(rowSize.ToString());

            // Starting points represents point from where block shape grid should start inside block shape.
            float startPointX = GetStartPointX(blockSize, columnSize);
            float startPointY = GetStartPointY(blockSize, rowSize);

            // Will keep updating with iterations.
            float currentPositionX = startPointX;
            float currentPositionY = startPointY;

            GamePlayUI.Instance.gamePlay.allRows = new List<List<Block>>();
            GamePlayUI.Instance.gamePlay.allColumns = new List<List<Block>>();

            Sprite blockBGSprite = ThemeManager.Instance.GetBlockSpriteWithTag(blockTemplate.GetComponent<Block>().defaultSpriteTag);
            Level activeLevel = GamePlayUI.Instance.currentLevel;
            bool hasJewelMachine = false;
            bool hasShell = false;
            GamePlay.Instance.blockers.Clear();
            // Iterates through all rows and columns to generate grid.
            for (int row = 0; row < rowSize; row++)
            {
                List<Block> blockRow = new List<Block>();

                for (int column = 0; column < columnSize; column++)
                {
                    // Spawn a block instance and prepares it.
                    RectTransform blockElement = GetBlockInsideGrid();
                    blockElement.localPosition = new Vector3(currentPositionX, currentPositionY, 0);
                    currentPositionX += (blockSize + blockSpace);
                    blockElement.sizeDelta = Vector3.one * blockSize;
                    blockElement.GetComponent<BoxCollider2D>().size = Vector3.one * blockSize;
                    blockElement.GetComponent<Image>().sprite = blockBGSprite;
                    blockElement.name = "block-" + row + "" + column;

                    // Sets blocks logical position inside grid and its default sprite.
                    Block block = blockElement.GetComponent<Block>();
                    block.gameObject.SetActive(true);
                    block.SetBlockLocation(row, column);
                    blockRow.Add(block);
                    block.assignedSpriteTag = block.defaultSpriteTag;
                    
                    
                    if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                    {
                        if (activeLevel.rows[row].coloum[column].spriteType != SpriteType.Empty)
                        {
                            bool hasStages = activeLevel.rows[row].coloum[column].hasStages;
                            int stage = activeLevel.rows[row].coloum[column].stage;
                            int remainingCounter = activeLevel.rows[row].coloum[column].remainingCounter;

                            SpriteType spriteType = activeLevel.rows[row].coloum[column].spriteType;
                            SpriteType secondarySprite = activeLevel.rows[row].coloum[column].secondarySpriteType;

                            #region Grid Blocks Placing
                            if (spriteType == SpriteType.BalloonBomb)
                            {
                                blockElement.GetComponent<Block>().PlaceBalloonBomb(remainingCounter);
                            }
                            else if (spriteType == SpriteType.IceBomb)
                            {
                                blockElement.GetComponent<Block>().PlaceIceBomb(remainingCounter);
                            }
                            else if (spriteType == SpriteType.IceMachine)
                            {
                                blockElement.GetComponent<Block>().PlaceIceMachine(remainingCounter, hasStages, stage);
                            }
                            else
                            {
                                if (secondarySprite == SpriteType.BalloonBomb)
                                {
                                    blockElement.GetComponent<Block>().PlaceBalloonBomb(remainingCounter);
                                }

                                if (spriteType is SpriteType.BlueGiftBox or SpriteType.CyanGiftBox or SpriteType.GreenGiftBox 
                                    or SpriteType.PinkGiftBox or SpriteType.PurpleGiftBox or SpriteType.RedGiftBox)
                                {
                                    blockElement.GetComponent<Block>().defaultSpritetype = secondarySprite;
                                }
                                
                                blockElement.GetComponent<Block>().SetBlock(spriteType, secondarySprite, hasStages, stage);
                            }
                            #endregion

                            if (!hasShell && spriteType is SpriteType.CloseShell or SpriteType.CloseShell)
                            {
                                hasShell = true;
                            }

                            if (!hasJewelMachine && spriteType == SpriteType.JewelMachine)
                            {
                                hasJewelMachine = true;
                            }
                        }
                    }
                    
                }
                currentPositionX = startPointX;
                currentPositionY -= (blockSize + blockSpace);

                GamePlayUI.Instance.gamePlay.allRows.Add(blockRow);
            }
            
            GamePlay.Instance.OnBoardGridReady();
            GamePlay.Instance.blockShapeController.PrepareShapeContainer(progressData);
            
            if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                if (GamePlayUI.Instance.currentLevel.ConveyorBelts.enabled)
                {
                    ConveyorBeltController.Instance.enabled = true;
                    ConveyorBeltController.Instance.SetConveyor();
                }

                if (hasJewelMachine && activeLevel.JewelMachine.enabled)
                {
                    JewelMachineController.Instance.StartJewelMachine(activeLevel.JewelMachine.counter, activeLevel.JewelMachine.GemsToSpwan);
                }

                if (activeLevel.BlockerStick.enabled)
                {
                    BlockerStickSpwaner.Instance.SpawnBlockerStick();
                }
            }
            else
            {
                // Sets progress and status to each blocks if there is any from previos session.
                if (progressData != null)
                {
                    int rowIndex = 0;
                    foreach (string blockRow in progressData.gridData)
                    {
                        int columnIndex = 0;
                        string[] rowData = blockRow.Split(',');
                        foreach (string blockData in rowData)
                        {
                            if (rowIndex < rowSize && columnIndex < columnSize)
                            {
                                SetBlockStatus(GamePlay.Instance.allRows[rowIndex][columnIndex], blockData);
                            }
                            columnIndex++;
                        }
                        rowIndex++;
                    }

                    #region Blast Mode Specific
                    //Places all bombs with its counter from previous session.Applies to blast mode only.
                    foreach (BombInfo bombInfo in progressData.allBombInfo)
                    {
                        GamePlay.Instance.allRows[bombInfo.rowId][bombInfo.columnId].PlaceBomb(bombInfo.remainCounter);
                    }
                    #endregion
                }
            }
            if (hasShell)
            {
                GamePlay.Instance.pearlAndShellController.SetActive(true);
            }
        }

        /// <summary>
        /// Will set block status if there is any from previos session progress.
        /// </summary>
        void SetBlockStatus(Block block, string statusData)
        {
            bool isAvailable = true;
            bool.TryParse(statusData.Split('-')[0], out isAvailable);
            string spriteTag = statusData.Split('-')[1];
            if (!isAvailable)
            {
                block.PlaceBlock(statusData.Split('-')[1]);
            }
        }

        /// <summary>
        /// Horizontal position from where block grid will start.
        /// </summary>
        public float GetStartPointX(float blockSize, int rowSize)
        {
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;
            float totalWidth = (blockSize * rowSize) + ((rowSize - 1) * blockSpace);
            return -((totalWidth / 2) - (blockSize / 2));
        }

        /// <summary>
        /// Vertical position from where block grid will start.
        /// </summary>
        public float GetStartPointY(float blockSize, int columnSize)
        {
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;
            float totalHeight = (blockSize * columnSize) + ((columnSize - 1) * blockSpace);
            return ((totalHeight / 2) - (blockSize / 2));
        }

        /// <summary>
        /// Spawn a new block instance and sets its block root as its parent.
        /// </summary>
        public RectTransform GetBlockInsideGrid()
        {
            GameObject block = (GameObject)(Instantiate(blockTemplate)) as GameObject;
            block.transform.SetParent(blockRoot.transform);
            block.transform.localScale = Vector3.one;
            return block.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Resets Grid and removes all blocks from it.
        /// </summary>
        public void ResetGame()
        {
            blockRoot.ClearAllChild();
            GamePlay.Instance.DestroyInstantiatedGameObjects();
            GamePlay.Instance.pearlAndShellController.SetActive(false);
            ConveyorBeltController.Instance.enabled = false;
            if(GamePlay.Instance.jewelMachineEnabled)
            {
                JewelMachineController.Instance.ResetMachine();
                JewelMachineController.Instance.enabled = false;
                JewelMachineController.Instance.isJewelMachineHasInitialized = false;
            }

            if(GamePlay.Instance.blockerStickEnabled)
            {
                BlockerStickSpwaner.Instance.ResetBlockerStick();
            }

            GamePlay.Instance.isBoardReady = false;
            GamePlay.Instance.canHatOrMusicDiscClear = false;
            TargetController.Instance.canCollectAnyEmptyBlock = false;
            PowerUpsController.Instance.Reset();
            GamePlayUI.Instance.isGameWon = false;
            if(GamePlay.Instance.movingKitesList != null)
            {
                GamePlay.Instance.movingKitesList.Clear();
            }
            GamePlay.Instance.allShapes.Clear();
            if (GamePlay.Instance.movingRocketsList != null)
            {
                GamePlay.Instance.movingRocketsList.Clear();
            }
        }
    }
}
