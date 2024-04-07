using System.Collections.Generic;
using GamingMonks.Feedbacks;
using UnityEngine;
using TMPro;
using GamingMonks.UITween;

namespace GamingMonks
{
    public class BalloonBomb : MonoBehaviour
    {
        #pragma warning disable 0649
        // Text of remaining coundown on bomb.
        [SerializeField] TextMeshProUGUI txtCounter;
        #pragma warning restore 0649

        // Remaining coundown amount on bomb.
        [System.NonSerialized] public int remainingCounter;// = 3;
        public Block block = null;

        private void Awake()
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            rectTransform.sizeDelta = new Vector2(size-5, size-5);
        }
        
        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            /// Registers game status callbacks.
            // remainingCounter = GamePlayUI.Instance.currentLevel.remainingcounter;
            GamePlayUI.OnShapePlacedEvent += GamePlayUI_OnShapePlacedEvent;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            /// Unregisters game status callbacks.
            GamePlayUI.OnShapePlacedEvent -= GamePlayUI_OnShapePlacedEvent;
        }

        /// <summary>
        /// Sets the given counter on bomb.
        /// </summary>
        public void SetCounter(int remainCounter)
        {
            remainingCounter = remainCounter;
            txtCounter.text = remainCounter.ToString();
        }

        /// <summary>
        /// Counter will keep reduding upon each block shape placed and will lead to game over or recue state on reaching to zero.
        /// </summary>
        private void GamePlayUI_OnShapePlacedEvent()
        {
            if (block.spriteType != SpriteType.BalloonBomb)
            {
                return;
            }
            remainingCounter -= 1;
            txtCounter.transform.LocalScale(Vector3.zero, 0.1F).OnComplete(() =>
            {
                txtCounter.text = remainingCounter.ToString();
            txtCounter.transform.LocalScale(Vector3.one, 0.1F).OnComplete(() =>
                {
                    if (remainingCounter <= 0)
                    {
                        transform.LocalScale(Vector3.zero, 0.1F).OnComplete(() =>
                        {
                            GamingMonksFeedbacks.Instance.PrepareBlockBreakEffect();
                            DestroyBlocks();
                            DestroyBalloonBomb();
                        });
                    }   
                });
            });
        }

        public void DestroyBlocks()
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();
            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;

            List<Block> destroyByBomb = new List<Block>();

            for (int i = block.ColumnId - 1; i <= block.ColumnId + 1; i++)
            {
                List<Block> lowerRow  = new List<Block>();
                List<Block> thisRow  = new List<Block>();
                List<Block> upperRow  = new List<Block>();

                if (i >= columnSize || i < 0)
                {
                    continue;
                }

                if (block.RowId > 0)
                {
                    upperRow = GamePlay.Instance.allRows[block.RowId - 1];
                    if (upperRow[i].isFilled)
                        destroyByBomb.Add(upperRow[i]);
                }
                
                
                if (i != block.ColumnId)
                {
                    thisRow = GamePlay.Instance.allRows[block.RowId];
                    if (thisRow[i].isFilled)
                        destroyByBomb.Add(thisRow[i]);
                }
                
                
                if (block.RowId < rowSize - 1)
                {
                    lowerRow = GamePlay.Instance.allRows[block.RowId + 1];
                    if (lowerRow[i].isFilled)
                        destroyByBomb.Add(lowerRow[i]);
                }
            }
            CheckForNeighbourBombs(ref destroyByBomb);
            foreach (Block b in destroyByBomb)
            {
                b.isHittedByBalloonBomb = true;
                if (b.spriteType is SpriteType.MilkBottle)
                {
                    b.milkShop.CollectMilkBottle();
                }
                else
                {
                    b.ClearBlock();
                }
            }
        }
        
        private void CheckForNeighbourBombs(ref List<Block> destroyByBomb)
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();
            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;
            List<Block> lowerRow  = new List<Block>();
            List<Block> thisRow  = new List<Block>();
            List<Block> upperRow  = new List<Block>();
            
            thisRow = GamePlay.Instance.allRows[block.RowId];
            
            #region Straight Checks

            #region Upper Block
            if (block.RowId > 0)
            {
                upperRow = GamePlay.Instance.allRows[block.RowId - 1];
                // Checks Upwards
                if (upperRow[block.ColumnId].spriteType == SpriteType.BalloonBomb)
                {
                    if (upperRow[block.ColumnId].thisBalloonBomb.remainingCounter > remainingCounter)
                    {
                        destroyByBomb.Add(upperRow[block.ColumnId]);
                    }
                    else
                    {
                        //  If there's no column left side.
                        if (block.ColumnId > 0)
                        {
                            destroyByBomb.Remove(upperRow[block.ColumnId - 1]);
                            destroyByBomb.Remove(thisRow[block.ColumnId - 1]);
                        }
                        destroyByBomb.Remove(upperRow[block.ColumnId]);
                        destroyByBomb.Remove(thisRow[block.ColumnId]);
                    
                        //  If there's no column right side.
                        if (block.ColumnId < columnSize - 1)
                        {
                            destroyByBomb.Remove(upperRow[block.ColumnId + 1]);
                            destroyByBomb.Remove(thisRow[block.ColumnId + 1]);
                        }
                    }
                }
            }
            #endregion
            
            #region Lower Block
            if (block.RowId < rowSize - 1)
            {                
                lowerRow = GamePlay.Instance.allRows[block.RowId + 1];
                // Checks Downwards
                if (lowerRow[block.ColumnId].spriteType == SpriteType.BalloonBomb)
                {
                    if (lowerRow[block.ColumnId].thisBalloonBomb.remainingCounter > remainingCounter)
                    {
                        destroyByBomb.Add(lowerRow[block.ColumnId]);
                    }
                    else
                    {
                        //  If there's no column left side.
                        if (block.ColumnId > 0)
                        {
                            destroyByBomb.Remove(lowerRow[block.ColumnId - 1]);
                            destroyByBomb.Remove(thisRow[block.ColumnId - 1]);  
                        }
                        destroyByBomb.Remove(lowerRow[block.ColumnId]);
                        destroyByBomb.Remove(thisRow[block.ColumnId]);
                        
                        //  If there's no column right side.
                        if (block.ColumnId < columnSize - 1)
                        {
                            destroyByBomb.Remove(thisRow[block.ColumnId + 1]);
                            destroyByBomb.Remove(lowerRow[block.ColumnId + 1]);
                        }
                    }
                }
            }
            #endregion
            
            #region Left Block
            if (block.ColumnId > 0)
            {
                // Checks Left
                if (thisRow[block.ColumnId - 1].spriteType == SpriteType.BalloonBomb)
                {
                    if (thisRow[block.ColumnId - 1].thisBalloonBomb.remainingCounter > remainingCounter)
                    {
                        destroyByBomb.Add(thisRow[block.ColumnId - 1]);
                    }
                    else
                    {
                        // If there's no row upwards.
                        if (block.RowId > 0)
                        {
                            destroyByBomb.Remove(upperRow[block.ColumnId]);
                            destroyByBomb.Remove(upperRow[block.ColumnId - 1]);
                        }
                        
                        destroyByBomb.Remove(thisRow[block.ColumnId]);
                        destroyByBomb.Remove(thisRow[block.ColumnId - 1]);
                        
                        //  If there's no row downwards. 
                        if (block.RowId < rowSize - 1)
                        {
                            destroyByBomb.Remove(lowerRow[block.ColumnId - 1]);
                            destroyByBomb.Remove(lowerRow[block.ColumnId]);
                        }
                    }
                }
            }
            #endregion

            #region Right Block
            if (block.ColumnId < columnSize - 1)
            {
                // Checks Right
                if (thisRow[block.ColumnId + 1].spriteType == SpriteType.BalloonBomb)
                {
                    if (thisRow[block.ColumnId + 1].thisBalloonBomb.remainingCounter > remainingCounter)
                    {
                        destroyByBomb.Add(thisRow[block.ColumnId + 1]);
                    }
                    else
                    {
                        //  If there's no row Upwards. 
                        if (block.RowId < 0)
                        {
                            destroyByBomb.Remove(upperRow[block.ColumnId]);
                            destroyByBomb.Remove(upperRow[block.ColumnId + 1]);
                        }
                        destroyByBomb.Remove(thisRow[block.ColumnId]);
                        destroyByBomb.Remove(thisRow[block.ColumnId  + 1]);
                        
                        //  If there's no row Downwards. 
                        if (block.RowId < rowSize - 1)
                        {
                            destroyByBomb.Remove(lowerRow[block.ColumnId + 1]);
                            destroyByBomb.Remove(lowerRow[block.ColumnId]);
                        }
                    }
                }
            }
            #endregion
            
            #endregion
            
            #region Diagonal Checks
            if (block.RowId > 1)
            {
                #region TopLeft Block
                if (block.ColumnId > 1)
                {
                    // Checks top Left
                    if (upperRow[block.ColumnId - 1].spriteType == SpriteType.BalloonBomb)
                    {
                        if (upperRow[block.ColumnId - 1].thisBalloonBomb.remainingCounter > remainingCounter)
                        {
                            destroyByBomb.Add(upperRow[block.ColumnId - 1]);
                        }
                        else
                        {
                            destroyByBomb.Remove(upperRow[block.ColumnId]);
                            destroyByBomb.Remove(thisRow[block.ColumnId - 1]);
                        }
                    }
                }
                #endregion

                #region TopRight Block
                if (block.ColumnId < columnSize - 1)
                {
                    // Checks Top Right
                    if (upperRow[block.ColumnId + 1].spriteType == SpriteType.BalloonBomb)
                    {
                        if (upperRow[block.ColumnId + 1].thisBalloonBomb.remainingCounter > remainingCounter)
                        {
                            destroyByBomb.Add(upperRow[block.ColumnId + 1]);
                        }
                        else
                        {
                            destroyByBomb.Remove(upperRow[block.ColumnId]);
                            destroyByBomb.Remove(thisRow[block.ColumnId + 1]);
                        }
                    }
                }
                #endregion
            }

            if (block.RowId < rowSize - 1)
            {
                #region LowerLeft Block
                if (block.ColumnId > 1)
                {
                    // Checks lower left
                    if (lowerRow[block.ColumnId - 1].spriteType == SpriteType.BalloonBomb)
                    {
                        if (lowerRow[block.ColumnId - 1].thisBalloonBomb.remainingCounter > remainingCounter)
                        {
                            destroyByBomb.Add(lowerRow[block.ColumnId - 1]);
                        }
                        else
                        {
                            destroyByBomb.Remove(thisRow[block.ColumnId - 1]);
                            destroyByBomb.Remove(lowerRow[block.ColumnId]);
                        }
                    }
                }
                #endregion
                
                #region LowerRight Block
                if (block.ColumnId < columnSize - 1)
                {
                    // Checks lower right
                    if (lowerRow[block.ColumnId + 1].spriteType == SpriteType.BalloonBomb)
                    {
                        if (lowerRow[block.ColumnId + 1].thisBalloonBomb.remainingCounter > remainingCounter)
                        {
                            destroyByBomb.Add(lowerRow[block.ColumnId + 1]);
                        }
                        else
                        {
                            destroyByBomb.Remove(thisRow[block.ColumnId + 1]);
                            destroyByBomb.Remove(lowerRow[block.ColumnId]);
                        }
                    }
                }
                #endregion
            }
            #endregion
        }
        
        public void DestroyBalloonBomb()
        {
            block.Clear();
            GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);
            Destroy(this.gameObject);
        }
    }
}