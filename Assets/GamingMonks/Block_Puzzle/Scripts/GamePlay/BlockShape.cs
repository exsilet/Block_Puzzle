using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GamingMonks.UITween;
using System.Collections;
using UnityEngine.SceneManagement;

namespace GamingMonks
{
    /// <summary>
    /// This script compont is attached to all the block shape on the game. This script will handles the actual game play and user interaction with the game.
    /// Each block shapes represents a grid format where unrequired blocks will be disabled to form a required shape.
    /// </summary>
    public class BlockShape : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
    {
        //Row size of block shape grid.
        [SerializeField] int rowSize;

        //Column size of block shape grid.
        [SerializeField] int columnSize;

        // List of all blocks that are being highlighted. Will keep updating runtime.
        List<Block> highlightingBlocks = new List<Block>();

        // Will set to true after slight time of user touches the block shape.
        bool shouldDrag = false;

        bool canRotate = true;
        // Cached instance of this block shape.
        Transform thisTransform;

        // Sprite tag represents image sprite on the block shape. You can configure sprite tag from inspector. You can also look UITheme for sprite tag and its associated sprite settings.
        public string spriteTag = "";

        // Type of block shape is adavce or standard. Only used for saving progress and add to required pool in forming level from previous progress.
        public bool isAdvanceShape = false;
        public bool isGray = false;
        public bool canPlaceOnBlockerStick = false;
        Sprite thisBlockSprite = null;

        // Time untill user can rotate shape on taking pointer up to rotate shape. will work onyl if rotation of shape is allowed.
        float pointerDownTime;

        // List of all active blocks inside block shape.
        List<Transform> activeBlocks = new List<Transform>();

        float dragOffset = 1.0F;

        // to set the sprite og child block
        [HideInInspector] public SpriteType specialBlockSpriteType;
        [HideInInspector] public bool specialBlockEnabled = false;
        private SpriteType blockShapeSpriteType;

        // block on the grid 
        public Block specialBlockHittedBlock { get; private set; }

        // Index of the block in the shape on which special block is placed.
        public int specialBlockIndex = -1;

        // Child block of blockShape on which special sprite is set (Magnet)
        public Transform specialBlockActivated { get; private set; }

        [HideInInspector] public bool isBlockerStickEnabled = false;

        private int m_rotation;
        [SerializeField] private BlockShapeType m_blockShapeType;

        [SerializeField] private Image m_blockImage;

        /// <summary>
        /// Awakes the instance and inintializes and prepare block shape.
        /// </summary>
        private void Awake()
        {
            thisTransform = transform;

            if (GamePlayUI.Instance.currentGameMode != GameMode.Level)
            {
                dragOffset = GamePlayUI.Instance.currentModeSettings.shapeDragPositionOffset;
            }
            else
            {
                dragOffset = GamePlayUI.Instance.gamePlaySettings.globalLevelShapeDragPositionOffset;
            }
            m_blockImage = GetComponent<Image>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            PrepareBlockShape();
            if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                isBlockerStickEnabled = GamePlayUI.Instance.currentLevel.BlockerStick.enabled;
            }
        }

        public int Rotation
        {
            get
            {
                return m_rotation;
            }
        }

        public BlockShapeType BlockShapeType
        {
            get
            {
                return m_blockShapeType;
            }
        }

        public List<Transform> ActiveBlocks
        {
            get
            {
                return activeBlocks;
            }
        }

        public Image GetBlockShapeImage()
        {
            return m_blockImage;
        }

        /// <summary>
        /// Prepared block shape based on the settings user has selecyted in game settings scriptable. Typically handles size of blocks and space inbetween blocks inside block shape.
        /// </summary>
        public void PrepareBlockShape()
        {
            bool doUpdateSprite = false;
            thisBlockSprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteTag);

            if (thisBlockSprite != null)
            {
                doUpdateSprite = true;
            }

            // Fetched the size of block that should be used.
            float blockSize = GamePlayUI.Instance.currentGameMode == GameMode.Level ? GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize : GamePlayUI.Instance.currentModeSettings.blockSize;

            // Fetched the space between blocks that should be used.
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;

            // Starting points represents point from where block shape grid should start inside block shape.
            float startPointX = GetStartPointX(blockSize, columnSize);
            float startPointY = GetStartPointY(blockSize, rowSize);

            // Will keep updating with iterations.
            float currentPositionX = startPointX;
            float currentPositionY = startPointY;

            int index = 0;

            float blockRotation = (360.0F - transform.localEulerAngles.z);
            m_rotation = (int)transform.localEulerAngles.z;

            for (int row = 0; row < rowSize; row++)
            {
                for (int column = 0; column < columnSize; column++)
                {
                    // Sets the position and  size on block inside block shape.
                    RectTransform blockElement = GetBlockInsideGrid(index);
                    blockElement.localPosition = new Vector3(currentPositionX, currentPositionY, 0);
                    blockElement.localEulerAngles = new Vector3(0, 0, blockRotation);

                    currentPositionX += (blockSize + blockSpace);
                    blockElement.sizeDelta = Vector3.one * blockSize;

                    if (doUpdateSprite)
                    {
                        blockElement.GetComponent<Image>().sprite = thisBlockSprite;
                    }
                    index++;
                }
                currentPositionX = startPointX;
                currentPositionY -= (blockSize + blockSpace);
            }

            // Will add all the actibve blocks to list that will be used during gameplay.
            foreach (Transform t in thisTransform)
            {
                if (t.gameObject.activeSelf)
                {
                    activeBlocks.Add(t);
                }
            }

            // set the special Sprite on the child block
            if (specialBlockEnabled)
            {
                if (transform.childCount > 0)
                {
                    if(specialBlockIndex == -1)
                    {
                        specialBlockIndex = Random.Range(0, activeBlocks.Count);
                    }

                    if (specialBlockSpriteType == SpriteType.Kite)
                    {
                        specialBlockActivated = activeBlocks[specialBlockIndex];
                    }
                    else
                    {
                        specialBlockActivated = activeBlocks[specialBlockIndex].transform.GetChild(0);
                    }

                    Image blockLayerImage = specialBlockActivated.GetComponent<Image>();
                    blockLayerImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(specialBlockSpriteType.ToString());
                    blockLayerImage.enabled = true;
                }
            }

            SetBlockShapeSpriteType();
        }

        private void SetBlockShapeSpriteType()
        {
            switch (spriteTag)
            {
                case "b1":
                    blockShapeSpriteType = SpriteType.Blue;
                    break;

                case "b2":
                    blockShapeSpriteType = SpriteType.Cyan;
                    break;

                case "b3":
                    blockShapeSpriteType = SpriteType.Green;
                    break;

                case "b4":
                    blockShapeSpriteType = SpriteType.Pink;
                    break;

                case "b5":
                    blockShapeSpriteType = SpriteType.Purple;
                    break;

                case "b6":
                    blockShapeSpriteType = SpriteType.Red;
                    break;
            }
        }

        /// <summary>
        /// Sets tag of sprite to block shape from the settings.
        /// </summary>
        public void SetSpriteTag(string tag)
        {
            spriteTag = tag;
        }

        #region Input Handling
        /// <summary>
        /// Pointer down on block shape.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null && !isGray)
            {
                // Checks whether this block shape is touched on pointer down.
                Transform clickedObject = eventData.pointerCurrentRaycast.gameObject.transform;

                if (clickedObject == thisTransform)
                {
                    if (UIController.Instance.topPanelWithModeContext.settingsPopUp.activeSelf)
                    {
                        UIController.Instance.topPanelWithModeContext.settingsPopUp.SetActive(false);
                    }
                    GamePlay.Instance.blockShapeController.DisableBlockShapeContainerInput(this);
                    Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);

                    if (!GamePlayUI.Instance.currentModeSettings.allowRotation)
                    {
                        UIFeedback.Instance.PlayBlockShapePickEffect();
                        thisTransform.LocalScale(Vector3.one,0.05F);// - new Vector3(0.13f, 0.13f, 0), 0.05F);
                        thisTransform.Position(new Vector3(pos.x, (pos.y + dragOffset), 0), 0.05F);
                    }
                    else
                    {
                        thisTransform.localScale = Vector3.one;// - new Vector3(0.13f, 0.13f, 0);
                        pointerDownTime = Time.time;
                    }
                    // Shape can be dragged now.
                    shouldDrag = true;
                }
            }
        }


        /// <summary>
        /// Begins dragging of block shape.
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (shouldDrag && !isGray)
            {
                canRotate = false;
                gameObject.GetComponent<Canvas>().sortingOrder = 4;
                Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
                pos.z = transform.localPosition.z;
                thisTransform.localScale = Vector3.one;// - new Vector3(0.13f, 0.13f, 0);
                thisTransform.position = new Vector3(pos.x, (pos.y + dragOffset), 0);
            }
        }

        /// <summary>
        /// Action to taken on pointer up.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            GamePlay.Instance.blockShapeController.EnableAllBlockShapeContainerInput();
            shouldDrag = false;
            if (!isGray)
            {
                UIFeedback.Instance.PlayBlockShapePlaceEffect();
            }
            bool canPlaceShape = TryPlacingShape();

            if (canPlaceShape)
            {
                GamePlayUI.Instance.OnShapePlaced();
                
                int count = 0;
                foreach (List<Block> blocks in GamePlay.Instance.allRows)
                {
                    foreach (Block block in blocks)
                    {
                        if (!block.isFilled)
                        {
                            count += 1;
                        }
                    }
                }
                
                gameObject.transform.SetParent(null);
                Destroy(gameObject);
                GamePlay.Instance.NumberOfMoves++;

                if (RotatePowerUp.Instance.IsActive)
                {
                    RotatePowerUp.Instance.DeactivateRotateOnUsed(true);
                }
                else if (SingleBlockPowerUp.Instance.IsActive)
                {
                    SingleBlockPowerUp.Instance.DeactivateSingleBlockPowerUpOnUsed(true);
                }

                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "LimitedMoves")
                {
                    if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                    {
                        UIController.Instance.UpdateMovesCount(--GamePlay.Instance.maxMovesAllowed);
                        if (GamePlay.Instance.maxMovesAllowed <= 0)
                        {
                            GamePlay.Instance.blockShapeController.DisableAllBlockShapeContainerInput();
                            GamePlay.Instance.CheckOutOfMove();
                        }

                    }
                }
            }
            else
            {
                if (!GamePlayUI.Instance.currentModeSettings.allowRotation)
                {
                    // Will reset shape and move to its original position and scale on leaving pointer from block shape.
                    if (!isGray)
                    {
                        UIFeedback.Instance.PlayBlockShapeResetEffect();
                    }
                    ResetShape();
                }
                else
                {
                    // Will check for rotation of shape on releasing finger if rotation of shape is allowed from game settings.
                    CheckForShapeRotation();
                }
            }
        }

        /// <summary>
        /// Will check if shape should be rotated or not on pointer up. Rotating of shape is allowed only for 0.3 seconds after pointer down and if not dragged.
        /// </summary>
        void CheckForShapeRotation()
        {
            float pointerUpTime = Time.time;
            bool isRotationDetected = ((pointerUpTime - pointerDownTime) < 0.3F);

            if (isRotationDetected || GamePlayUI.Instance.currentModeSettings.allowRotation && canRotate)
            {
                ResetShapeWithAddRotation();
            }
            else
            {
                ResetShape();
            }
        }

        /// <summary>
        /// Handles block shape dragging event.
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if (shouldDrag && !isGray)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
                pos = new Vector3(pos.x, (pos.y + dragOffset), 0F);
                thisTransform.position = pos;
                thisTransform.localScale = Vector3.one;// - new Vector3(0.13f, 0.13f, 0);
                CheckCanPlaceShape();
            }
        }
        #endregion


        /// <summary>
        // Returns the horizontal starting point from where grid should start.
        /// </summary>
        public float GetStartPointX(float blockSize, int rowSize)
        {
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;
            float totalWidth = (blockSize * rowSize) + ((rowSize - 1) * blockSpace);
            return -((totalWidth / 2) - (blockSize / 2));
        }

        /// <summary>
        // Returns the vertical starting point from where grid should start.
        /// </summary>
        public float GetStartPointY(float blockSize, int columnSize)
        {
            float blockSpace = GamePlayUI.Instance.currentGameMode == GameMode.Level ? GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSpace : GamePlayUI.Instance.currentModeSettings.blockSpace;
            float totalHeight = (blockSize * columnSize) + ((columnSize - 1) * blockSpace);
            return ((totalHeight / 2) - (blockSize / 2));
        }

        /// <summary>
        /// Returns recttransfrom component of the block at the given index. 
        /// </summary>
        public RectTransform GetBlockInsideGrid(int index)
        {
            return thisTransform.GetChild(index).GetComponent<RectTransform>();
        }

        /// <summary>
        /// Checks whether shape can be placed at the current position while dragging it. 
        /// </summary>
        bool CheckCanPlaceShape()
        {
            List<Block> hittingBlocks = new List<Block>();
            List<int> hittingRows = new List<int>();
            List<int> hittingColumns = new List<int>();

            foreach (Transform t in activeBlocks)
            {
                Block hittingBlock = GetHittingBlock(t);
                if (hittingBlock == null || hittingBlocks.Contains(hittingBlock))
                {
                    StopHighlight();
                    GamePlay.Instance.StopHighlight();
                    return false;
                }
                hittingBlocks.Add(hittingBlock);

                // Row Id of block which is interacting with block shape will be added to list. Used to highlight lines that can be completed by placing block shape at current position.
                if (!hittingRows.Contains(hittingBlock.RowId))
                {
                    if (GamePlay.Instance.CanHighlightRow(hittingBlock.RowId))
                    {
                        hittingRows.Add(hittingBlock.RowId);
                    }
                }

                // Column Id of block which is interacting with block shape will be added to list. Used to highlight lines that can be completed by placing block shape at current position.
                if (!hittingColumns.Contains(hittingBlock.ColumnId))
                {
                    if (GamePlay.Instance.CanHighlightColumn(hittingBlock.ColumnId))
                    {
                        hittingColumns.Add(hittingBlock.ColumnId);
                    }
                }

                // Will be called when user ends touch/mouse from the block shape and block shape will try to place on grid. Will go back to original if block shape cann not be placed.
                if (hittingBlocks.Count == activeBlocks.Count)
                {
                    foreach (Block block in hittingBlocks)
                    {
                        block.Highlight(thisBlockSprite);
                    }
                    GamePlay.Instance.HighlightAllRows(hittingRows, thisBlockSprite);
                    GamePlay.Instance.HighlightAllColmns(hittingColumns, thisBlockSprite);

                    StopHighlight(hittingBlocks);

                    // Will stop highlight all rows and columns except for given. 
                    GamePlay.Instance.StopHighlight(hittingRows, hittingColumns);

                    highlightingBlocks.AddRange(hittingBlocks);
                    GamePlay.Instance.highlightingRows.AddRange(hittingRows);
                    GamePlay.Instance.highlightingColumns.AddRange(hittingColumns);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Will be called when user ends touch/mouse from the block shape and block shape will try to place on grid. Will go back to original if block shape cann not be placed.
        /// </summary>
        bool TryPlacingShape()
        {
            List<Block> hittingBlocks = new List<Block>();
            List<int> completedRows = new List<int>();
            List<int> completedColumns = new List<int>();

            foreach (Transform t in activeBlocks)
            {
                Block hittingBlock = GetHittingBlock(t);
                if (hittingBlock == null || hittingBlocks.Contains(hittingBlock))
                {
                    StopHighlight();
                    GamePlay.Instance.StopHighlight();
                    return false;
                }
                hittingBlocks.Add(hittingBlock);

                // Row id of block will be added to list if entire row is goint to finish on placing current shape.
                if (!completedRows.Contains(hittingBlock.RowId))
                {
                    if (GamePlay.Instance.IsRowCompleted(hittingBlock.RowId))
                    {
                        completedRows.Add(hittingBlock.RowId);
                        GamePlay.Instance.completedLineSpriteTag = spriteTag;

                    }
                }

                // Column id of block will be added to list if entire column is goint to finish on placing current shape.
                if (!completedColumns.Contains(hittingBlock.ColumnId))
                {
                    if (GamePlay.Instance.IsColumnCompleted(hittingBlock.ColumnId))
                    {
                        completedColumns.Add(hittingBlock.ColumnId);
                        GamePlay.Instance.completedLineSpriteTag = spriteTag;
                    }
                }

                // Amount of blocks on grid that are interacting with block shape should be excact to amount of active blocks in the the blockshape. All the hitting blocks should be unique.
                if (hittingBlocks.Count == activeBlocks.Count)
                {
                    if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                    {
                        GamePlay.Instance.allShapes.Add(hittingBlocks);
                    }
                    for (int i = 0; i < hittingBlocks.Count; i++)
                    {
                        Block block = hittingBlocks[i];
                        if (specialBlockEnabled && specialBlockHittedBlock == block)
                        {
                            if (specialBlockSpriteType != SpriteType.Kite)
                                block.secondarySpriteType = blockShapeSpriteType;

                            block.PlaceBlock(thisBlockSprite, spriteTag, specialBlockSpriteType);
                        }
                        else
                        {
                            if (block.spriteType == SpriteType.Bubble)
                            {
                                block.secondarySpriteType = blockShapeSpriteType;
                            }
                            else
                            {
                                block.spriteType = blockShapeSpriteType;
                            }
                            block.PlaceBlock(thisBlockSprite, spriteTag);
                        }
                    }

                    /*
                    foreach (Block block in hittingBlocks)
                    {
                        block.PlaceBlock(thisBlockSprite, spriteTag);
                    }
                    */

                    // Will clear all rows that completed by placing current shape.
                    if (completedRows.Count > 0)
                    {
                        //GamePlay.Instance.ClearRows(completedRows);

                        foreach (int i in completedRows)
                        {
                            List<Block> blocks = GamePlay.Instance.GetEntireRow(i);
                            foreach (Block b in blocks)
                            {
                                if (b.spriteType is SpriteType.Empty or SpriteType.Blue or SpriteType.Orange or SpriteType.Pink or SpriteType.Purple
                                    or SpriteType.Red or SpriteType.Yellow or SpriteType.Green or SpriteType.Cyan)
                                {
                                    b.SetBlockSpriteType(blockShapeSpriteType);

                                }
                                b.assignedSpriteTag = spriteTag;
                            }
                            GamePlay.Instance.ClearRow(i);
                        }
                    }

                    // Will clear all columns that completed by placing current shape.
                    if (completedColumns.Count > 0)
                    {
                        //GamePlay.Instance.ClearColumns(completedColumns);

                        foreach (int i in completedColumns)
                        {
                            List<Block> blocks = GamePlay.Instance.GetEntirColumn(i);
                            foreach (Block b in blocks)
                            {
                                if (b.spriteType is SpriteType.Empty or SpriteType.Blue or SpriteType.Orange or SpriteType.Pink or SpriteType.Purple
                                    or SpriteType.Red or SpriteType.Yellow or SpriteType.Green or SpriteType.Cyan)
                                {
                                    b.SetBlockSpriteType(blockShapeSpriteType);
                                }
                                b.assignedSpriteTag = spriteTag;
                            }
                            GamePlay.Instance.ClearColoum(i);
                        }
                    }

                    int linesCleared = (completedRows.Count + completedColumns.Count);
                    // Adds score based on the number of rows, columnd and blocks cleares. final calculation will be done in score manager.

                    if (GamePlayUI.Instance.currentGameMode != GameMode.Level)
                    {
                        GamePlayUI.Instance.scoreManager.AddScore(linesCleared, activeBlocks.Count);
                    }

                    if (linesCleared > 0) {
                        AudioController.Instance.PlayLineBreakSound(completedRows.Count + completedColumns.Count);
                        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                        {
                            BoxingGlove.Instance.OnRowCleared(linesCleared);
                        }
                    }

                    #region TimeMode Specific
                    if (GamePlayUI.Instance.currentGameMode == GameMode.Timed)
                    {
                        // Will add line completion bonus time to timer.
                        GamePlayUI.Instance.timeModeProgresssBar.AddTime((GamePlayUI.Instance.timeModeAddSecondsOnLineBreak * (completedRows.Count + completedColumns.Count)));
                    }
                    #endregion
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Returns block that is interecting with current block shape. Returns null if not any.
        /// </summary>
        Block GetHittingBlock(Transform draggingBlock)
        {
            if (isBlockerStickEnabled && !canPlaceOnBlockerStick)
            {
                // return null if hit with BlockerStick
                RaycastHit2D[] hits = Physics2D.BoxCastAll(draggingBlock.position, new Vector2(0.35f, 0.35f), 0, Vector2.zero);
                foreach (RaycastHit2D h in hits)
                {
                    if (h.collider.gameObject.GetComponent<BlockerStick>() != null)
                    {
                        return null;
                    }
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(draggingBlock.position, Vector2.zero, 1);
            if (hit.collider != null && hit.collider.GetComponent<Block>() != null)
            {
                if (specialBlockEnabled && (draggingBlock.transform.GetChild(0).GetComponent<Image>().enabled ||
                                            draggingBlock.transform == specialBlockActivated))
                {
                    specialBlockHittedBlock = hit.collider.GetComponent<Block>();
                }
                return hit.collider.GetComponent<Block>();
            }
            return null;
        }

        /// <summary>
        /// Stops highlighting all blocks from highlightingBlocks list except for given blocks.
        /// </summary>
        void StopHighlight(List<Block> excludingList)
        {
            foreach (Block b in highlightingBlocks)
            {
                if (!excludingList.Contains(b))
                {
                    b.Reset();
                }
            }
            highlightingBlocks.Clear();
        }

        /// <summary>
        /// Stops highlighting all blocks.
        /// </summary>
        void StopHighlight()
        {
            foreach (Block b in highlightingBlocks)
            {
                b.Reset();
            }
            highlightingBlocks.Clear();
        }

        /// <summary>
        /// Reset shape and will move it to its original position. Typically called when it fails to place on grid.
        /// </summary>
        public void ResetShape()
        {
            if (gameObject.activeSelf)
            {
                thisTransform.LocalPosition(Vector3.zero, 0.25F);
                thisTransform.LocalScale((Vector3.one * GamePlayUI.Instance.currentModeSettings.shapeInactiveSize), 0.25F);
                StartCoroutine(DelayBeforeChangingSortingLayer());
                canRotate = true;
            }
        }

        IEnumerator DelayBeforeChangingSortingLayer()
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Canvas>().sortingOrder = 1;
        }

        /// <summary>
        /// Adds rotation to block shape at its original position.
        /// </summary>
        void ResetShapeWithAddRotation()
        {
            float newRotation = (transform.localEulerAngles.z - 90);
            InputManager.Instance.DisableTouchForDelay(0.2F);
            transform.LocalRotationToZ(newRotation, 0.2F).OnComplete(() => {
                ResetShape();
                CheckBlockShapeCanPlaced();
            });
        }

        public float GetRotation()
        {
            return transform.localEulerAngles.z;
        }

        public void SetRotation(float blockRotation)
        {
            transform.localEulerAngles = new Vector3(0, 0, blockRotation);
        }

        /// <summary>
        /// Marks the shape Gray and resets it's positions back to the container.
        /// </summary>
        public void ShapeGrayed()
        {
            for (int i = 0; i < ActiveBlocks.Count; i++)
            {
                Image image = ActiveBlocks[i].GetComponent<Image>();
                image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.WhiteSprite.ToString());
                image.color = Color.gray;
            }
            isGray = true;
            UIFeedback.Instance.PlayBlockShapeResetEffect();
            ResetShape();
        }

        public void CheckBlockShapeCanPlaced()
        {
            if (this != null)
            {
                bool shapeCanbePlaced = GamePlay.Instance.blockShapeController.blockShapePlacementChecker.CheckShapeCanbePlacedWithoutRotate(this);
               
                if (shapeCanbePlaced)
                {
                    for (int i = 0; i < ActiveBlocks.Count; i++)
                    {
                        Image image = ActiveBlocks[i].GetComponent<Image>();
                        image.color = Color.white;
                        image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteTag);

                        if (specialBlockSpriteType == SpriteType.Kite)
                        {
                            if (ActiveBlocks[i] == specialBlockActivated)
                            {
                                image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(specialBlockSpriteType.ToString());
                            }
                        }

                    }
                    isGray = false;
                }
                else
                {
                    for (int i = 0; i < ActiveBlocks.Count; i++)
                    {
                        Image image = ActiveBlocks[i].GetComponent<Image>();
                        image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.WhiteSprite.ToString());
                        image.color = Color.gray;
                    }
                    isGray = true;
                }
            }
        }
    }
}
