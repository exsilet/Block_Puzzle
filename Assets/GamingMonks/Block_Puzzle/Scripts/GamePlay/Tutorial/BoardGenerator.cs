using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks.Tutorial
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

        public GameObject gridBackground;
        public GameObject gridDesign;
        public GameObject gridBorder;
        public float gridOffset = 45;
        GamePlaySettings gamePlaySettings;


#pragma warning restore 0649

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
        public void GenerateBoard()
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();

            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;

            // Fetched the size of block that should be used.
            float blockSize = GamePlayUI.Instance.currentGameMode == GameMode.Level ? gamePlaySettings.globalLevelModeBlockSize : GamePlayUI.Instance.currentModeSettings.blockSize;
            
            // Fetched the space between blocks that should be used.
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;

            //Set background and design Grid Size and design type
            //gridBackground.GetComponent<RectTransform>().sizeDelta = new Vector2((rowSize * blockSize) + ((rowSize - 1) * blockSpace) + gridOffset, (columnSize * blockSize) + ((columnSize - 1) * blockSpace) + gridOffset);
            gridDesign.GetComponent<RectTransform>().sizeDelta = new Vector2((rowSize * blockSize) + ((rowSize - 1) * blockSpace) + gridOffset, (columnSize * blockSize) + ((columnSize - 1) * blockSpace) + gridOffset);
            //gridBorder.GetComponent<RectTransform>().sizeDelta = new Vector2((rowSize * blockSize) + ((rowSize - 1) * blockSpace), (columnSize * blockSize) + ((columnSize - 1) * blockSpace));
            gridDesign.GetComponent<Image>().sprite = ThemeManager.Instance.GetBlockSpriteWithTag(rowSize.ToString());

            // Starting points represents point from where block shape grid should start inside block shape.
            float startPointX = GetStartPointX(blockSize, columnSize);
            float startPointY = GetStartPointY(blockSize, rowSize);

            // Will keep updating with iterations.
            float currentPositionX = startPointX;
            float currentPositionY = startPointY;

            GamePlayUI.Instance.gamePlay.allRows = new List<List<Block>>();
            Sprite blockBGSprite = ThemeManager.Instance.GetBlockSpriteWithTag(blockTemplate.GetComponent<Block>().defaultSpriteTag);

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
                    block.GetComponent<BoxCollider2D>().enabled = false;
                    block.GetComponent<Image>().color = new Color(1,1,1,0.3F);
                }
                currentPositionX = startPointX;
                currentPositionY -= (blockSize + blockSpace);

                GamePlayUI.Instance.gamePlay.allRows.Add(blockRow);
            }

            GamePlay.Instance.OnBoardGridReady();
        }

        public void UpdateBoardForTutorial(int step) 
        {
            if(step == 1) {
                int i = 0;
                int j = 0;
                List<Block> middleRow1 = GamePlay.Instance.GetEntireRow(4);
                foreach(Block block in middleRow1) {
                    if(i < 4 || i > 5)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    i++;
                }

                List<Block> middleRow2 = GamePlay.Instance.GetEntireRow(5);
                foreach (Block block in middleRow2)
                {
                    if (j < 4 || j > 5)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    j++;
                }
                GamePlayUI.Instance.boardHighlightImage.SetActive(true);
                
            }
            else if (step == 2)
            {
                int i = 0;
                int j = 0;
                int k = 0;
                List<Block> middleColumn1 = GamePlay.Instance.GetEntirColumn(4);

                foreach (Block block in middleColumn1)
                {
                    if (i < 4 || i > 4)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    i++;
                }
                List<Block> middleColumn2 = GamePlay.Instance.GetEntirColumn(5);

                foreach (Block block in middleColumn2)
                {
                    if (j < 4 || j > 4)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    j++;
                }

                List<Block> middleColumn3 = GamePlay.Instance.GetEntirColumn(6);

                foreach (Block block in middleColumn3)
                {
                    if (k < 4 || k > 6)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    k++;
                }
                GamePlayUI.Instance.boardHighlightImage.SetActive(true);
            }
            else if (step == 3)
            {
                int i = 0;
                int j = 0;
                int k = 0;
                int l = 0;
                int m = 0;
                int n = 0;
                List<Block> middleColumn1 = GamePlay.Instance.GetEntirColumn(4);
                foreach (Block block in middleColumn1)
                {
                    if (i < 5 || i > 5)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    i++;
                }

                List<Block> middleColumn2 = GamePlay.Instance.GetEntirColumn(5);
                foreach (Block block in middleColumn2)
                {
                    if (j < 4 || j > 6)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    j++;
                }

                List<Block> middleColumn3 = GamePlay.Instance.GetEntirColumn(6);
                foreach (Block block in middleColumn3)
                {
                    if (k < 5 || k > 5)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    k++;
                }

                List<Block> middleRow1 = GamePlay.Instance.GetEntireRow(4);
                foreach (Block block in middleRow1)
                {
                    if (l < 4 ||l > 6)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    l++;
                }

                List<Block> middleRow2 = GamePlay.Instance.GetEntireRow(5);
                foreach (Block block in middleRow2)
                {
                    if (m < 4 || m > 6)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    m++;
                }

                List<Block> middleRow3 = GamePlay.Instance.GetEntireRow(6);
                foreach (Block block in middleRow3)
                {
                    if (n < 4 || n > 6)
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        int randomNumber = Random.Range(0, gamePlaySettings.blockSpriteColorTags.Length);
                        block.PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(gamePlaySettings.blockSpriteColorTags[randomNumber]), gamePlaySettings.blockSpriteColorTags[randomNumber]);
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    else
                    {
                        block.GetComponent<BoxCollider2D>().enabled = true;
                        block.GetComponent<Image>().color = new Color(1, 1, 1, 1F);
                    }
                    n++;
                }

                GamePlayUI.Instance.boardHighlightImage.SetActive(true);
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
        }
    }
}
