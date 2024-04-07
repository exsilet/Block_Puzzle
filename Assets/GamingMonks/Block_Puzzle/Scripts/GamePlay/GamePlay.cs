using System.Collections;
using System.Collections.Generic;
using GamingMonks.Utils;
using UnityEngine;
using System;
using GamingMonks.Features;
using GamingMonks.Feedbacks;
using UnityEngine.Serialization;
using TMPro;


namespace GamingMonks
{
    public class GamePlay : Singleton<GamePlay>
    {
        [Header("Public Class Members")]
        [Tooltip("BoardGenerator Script Reference")]
        public BoardGenerator boardGenerator;

        [Tooltip("BlockShapesController Script Reference")]
        public BlockShapesController blockShapeController;

        public GameObject pearlAndShellController;
        public Transform instantiatedGameObjectsParent;
        
        [Header("Other Public Members")]
        #region Blast Mode Specific
        public GameObject bombTemplate;
        public GameObject balloonBombTemplate;
        public IceBomb iceBomb;
        #endregion

        //List of all Blocks in Row X Column format.
        [System.NonSerialized] public List<List<Block>> allRows = new List<List<Block>>();
        [System.NonSerialized] public List<List<Block>> allColumns = new List<List<Block>>();
        [System.NonSerialized] public List<List<Block>> allShapes = new List<List<Block>>();

        //List of rows highlight while dragging shape. Will keep updating runtime. 
        [System.NonSerialized] public List<int> highlightingRows = new List<int>();

        //List of columns highlight while dragging shape. Will keep updating runtime. 
        [System.NonSerialized] public List<int> highlightingColumns = new List<int>();

        // Saves highlighting rows as cached to reduce iterations . Will keep updating runtime. 
        List<int> cachedHighlightingRows = new List<int>();

        // Saves highlighting columns as cached to reduce iterations . Will keep updating runtime. 
        List<int> cachedHighlightingColumns = new List<int>();

        [Header("Prefab References")]
        public GameObject Kite;
        public GameObject iceMachine;
        [HideInInspector] public int musicNoteCount;
        [HideInInspector] public int birdCount;
        [HideInInspector] public int rocketPositionIndex = 0;
        public GameObject musicalNote;
        public Diamond diamondPrefab;
        public MilkShop milkShopPrefab;
        public MagicHat magicHatPrefab;
        [FormerlySerializedAs("musicPlayerPrefab")] public MusicalPlayer musicalPlayerPrefab;
        [HideInInspector] public string completedLineSpriteTag = "";
        public GameObject lightningBolt;
        public GameObject rocket;
        
        [Header("Lists")]
        public List<GameObject> instantiatedGameObjects;
        public List<Block> blockers = new List<Block>();
        public List<GameObject> movingRocketsList = new List<GameObject>();
        public List<GameObject> movingKitesList = new List<GameObject>();//list of active kites if this list is not empty Checking of Blockshapescanbeplaced will pause untill all kite moves 
        public event Action OnRowColoumCleared;

        public bool jewelMachineEnabled { get; private set; }
        public bool isBoardReady = false;
        public bool canHatOrMusicDiscClear = false;//if the Music disc or Magic hat is getting cleared then we should check blockshape can be placed after the Magic hat and music disc cleared
        public bool blockerStickEnabled { get; private set; }

        // How many Moves player has played in the game till now.. 
        private int m_numberOfMoves = 0;
        
        // Maximum moves allowed in the game to achieve target.
        public int maxMovesAllowed = 0;

        public bool canScaleUp = false;
        public AppSettings appSettings { get; private set; }

        #region Blast Mode Specific
        

        /// <summary>
        /// Places bomb at random empty block on grid.
        /// </summary>
        public void PlaceBlastModeBombAtRandomPlace()
        {
            Block emptyBlock = GetRandomIsFilledBlock();

            if (emptyBlock != null)
            {
                emptyBlock.PlaceBomb(GamePlayUI.Instance.blastModeCounter);
            }
        }

        public async void PlaceBombAtRandomPlace()
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.7f)); 
            
            Level activeLevel = GamePlayUI.Instance.currentLevel;
            BombType bombType = activeLevel.BombType;


            if (bombType == BombType.IceBomb)
            {
                Block emptyBlock = GetRandomIsFilledBlock();
                if (emptyBlock != null)
                {
                    emptyBlock.PlaceIceBomb(GamePlayUI.Instance.currentLevel.remainingcounter);
                }
            }
            if (bombType == BombType.BalloonBomb)
            {
                Block emptyBlock = GetRandomIsFilledBlock();
                if (emptyBlock != null)
                {
                    emptyBlock.PlaceBalloonBomb(GamePlayUI.Instance.currentLevel.remainingcounter);
                    
                    //  If line gets completed by placing balloon bomb then clear that.
                    if (IsRowCompleted(emptyBlock.RowId))
                    {
                        ClearRow(emptyBlock.RowId);
                    }

                    if (IsColumnCompleted(emptyBlock.ColumnId))
                    {
                        ClearColoum(emptyBlock.ColumnId);
                    }
                }
            }
        }
        /// <summary>
        /// Returns random empty block from grid.
        /// </summary>
        public Block GetRandomEmptyBlockForRocket()
        {
            List<Block> emptyBlocks = new List<Block>();
            foreach (List<Block> blocks in allRows)
            {
                emptyBlocks.AddRange(blocks.FindAll(o => o.isAvailable));
            }
            if (emptyBlocks.Count > 0)
            {
                emptyBlocks.Shuffle();
                Block emptyBlock = emptyBlocks[0];
                return emptyBlock;
            }
            return null;
        }
        /// <summary>
        /// Returns random empty block from grid.
        /// </summary>
        public Block GetRandomEmptyBlock()
        {
            List<Block> emptyBlocks = new List<Block>();
            foreach (List<Block> blocks in allRows)
            {
                emptyBlocks.AddRange(blocks.FindAll(o => o.isAvailable));
            }
            if (emptyBlocks.Count > 0)
            {
                emptyBlocks.Shuffle();
                Block emptyBlock = emptyBlocks[0];
                int iterations = 0;
                while (emptyBlock.spriteType == SpriteType.Bubble && iterations < 100)
                {
                    emptyBlocks.Shuffle();
                    emptyBlock = emptyBlocks[0];
                    iterations++;
                }

                return emptyBlock.spriteType != SpriteType.Bubble ? emptyBlock : null;

            }
            return null;
        }

        /// <summary>
        /// Returns List of all bombs of the block grid.
        /// </summary>
        public BombInfo[] GetAllBombInfo()
        {
            List<Block> allBombs = new List<Block>();

            foreach (List<Block> blocks in allRows) {
                allBombs.AddRange(blocks.FindAll(o => o.hasTimeBomb));
            }

            BombInfo[] allBombsInfo = new BombInfo[allBombs.Count];
            int bombIndex = 0;

            foreach (Block block in allBombs)
            {
                allBombsInfo[bombIndex] = new BombInfo(block.RowId, block.ColumnId, block.thisTimeBomb.remainingCounter);
                bombIndex++;
            }

            return allBombsInfo;
        }
        #endregion
       
        private void Awake()
        {
            // Initializes the AppSettings Scriptable.
            if (appSettings == null)  {
                appSettings = (AppSettings)Resources.Load("AppSettings");
            }
        } 
       
        /// <summary>
        /// Will get called when board grid gets initialized.
        /// </summary>
        public void OnBoardGridReady()
        {
            int totalRows = allRows.Count;
            for (int rowId = 0; rowId < allRows[0].Count; rowId++) {
                 List<Block> thisColumn = new List<Block>();
                 for (int columnId = 0; columnId < totalRows; columnId++) { 
                     thisColumn.Add(allRows[columnId][rowId]);
                 }
                 allColumns.Add(thisColumn);
            }
            //isBoardReady = true;
            //foreach(GameObject obj in instantiatedGameObjects)
            //{
            //    Diamond diamond = obj.gameObject.GetComponent<Diamond>();
            //    if ( diamond != null)
            //    {
            //        diamond.enabled = true;
            //    }
            //}
            StartCoroutine(EnableDiamond());
        }

        public int NumberOfMoves
        {
            get
            {
                return m_numberOfMoves;
            }

            set
            {
                m_numberOfMoves = value;
            }
        }

        public IEnumerator EnableDiamond()
        {
            yield return new WaitForSeconds(0.5f);
            isBoardReady = true;
        }
        
        /// <summary>
        /// Clears all given rows from the grid.
        /// </summary>
        public void ClearRows(List<int> rowIds)
        {
            foreach (int rowId in rowIds)
            {
                StartCoroutine(ClearAllBlocks(GetEntireRow(rowId)));
            }
            GamePlayUI.Instance.totalLinesCompleted += rowIds.Count;
        }

        /// <summary>
        /// Clears all given columns from the grid.
        /// </summary>
        public void ClearColumns(List<int> columnIds)
        {
            foreach (int columnId in columnIds)
            {
                StartCoroutine(ClearAllBlocks(GetEntirColumn(columnId)));
            }
            GamePlayUI.Instance.totalLinesCompleted += columnIds.Count;
        }
        
        public void ClearRow(int rowId)
        {
            StartCoroutine(ClearAllBlocks(GetEntireRow(rowId)));
        }

        public void ClearColoum(int coloumId)
        {
            StartCoroutine(ClearAllBlocks(GetEntirColumn(coloumId)));
        }
        
        IEnumerator ClearAllBlocks(List<Block> allBlocks)
        {
            bool canClear = true;
            int bottleCount = 0;
            GamingMonksFeedbacks.Instance.PrepareBlockBreakEffect();
            
            foreach (Block block in allBlocks)
            {
                if (block.spriteType == SpriteType.MilkBottle)
                {
                    if (bottleCount >= 2)
                    {
                        canClear = true;
                        bottleCount = 0;
                    }

                    // MilkShop block clear only one block at a time
                    if (canClear)
                    {
                        block.milkShop.CollectMilkBottle();
                        canClear = false;
                    }
                    bottleCount++;
                    continue;
                }
                
                if (TargetController.Instance.canCollectAnyEmptyBlock)
                {
                    if(block.spriteType is SpriteType.Red or SpriteType.Yellow 
                        or SpriteType.Blue or SpriteType.Green or SpriteType.Cyan
                        or SpriteType.Purple or SpriteType.Pink or SpriteType.Orange)
                    {
                        TargetController.Instance.UpdateTarget(block.transform, SpriteType.AllColourBlock);
                    }
                }
                block.ClearBlock();
                //yield return new WaitForSeconds(0.01F);
                yield return null;
            }

            //levelCleared?.Invoke();
            Magnet.Instance.SetIsRowCompletedByPlacingMagnet(false);
            OnRowColoumCleared?.Invoke();
            blockShapeController.CheckAllShapesCanbePlaced();
            completedLineSpriteTag = null;
            /*
            //Below calculation is done so blocks starts clearing from center to end on both sides.
            int middleIndex = (allBlocks.Count % 2 == 0) ? (allBlocks.Count / 2) : ((allBlocks.Count / 2) + 1);
            int leftIndex = (middleIndex - 1);
            int rightIndex = middleIndex;
            int totalBlocks = allBlocks.Count;

            for (int i = 0; i < middleIndex; i++, leftIndex--, rightIndex++)
            {
                if (leftIndex >= 0)
                {
                    allBlocks[leftIndex].Clear();
                }
                if (rightIndex < totalBlocks)
                {
                    allBlocks[rightIndex].Clear();
                }
                yield return new WaitForSeconds(0.03F);
            }
            */
            yield return 0;
        }

    #region Magnet
         /// <summary>
        /// Clears all given rows from the grid.
        /// </summary>
        public void ClearRowsByMagnet(List<int> rowIds)
        {
            foreach (int rowId in rowIds)
            {
                StartCoroutine(ClearAllBlocksByMagnet(GetEntireRow(rowId)));
            }
            GamePlayUI.Instance.totalLinesCompleted += rowIds.Count;
        }

        /// <summary>
        /// Clears all given columns from the grid.
        /// </summary>
        public void ClearColumnsByMagnet(List<int> columnIds)
        {
            foreach (int columnId in columnIds)
            {
                StartCoroutine(ClearAllBlocksByMagnet(GetEntirColumn(columnId)));
            }
            GamePlayUI.Instance.totalLinesCompleted += columnIds.Count;
        }
        public void ClearRowByMagnet(int rowId)
        {
            StartCoroutine(ClearAllBlocksByMagnet(GetEntireRow(rowId)));
        }

        public void ClearColoumByMagnet(int coloumId)
        {
            StartCoroutine(ClearAllBlocksByMagnet(GetEntirColumn(coloumId)));
        }


        /// <summary>
        /// Clears all given blocks from the board. On Completion state of block will be empty.
        /// </summary>
        IEnumerator ClearAllBlocksByMagnet(List<Block> allBlocks)
        {
            bool canClear = true;
            int bottleCount = 0;
            GamingMonksFeedbacks.Instance.PrepareBlockBreakEffect();

            //Below calculation is done so blocks starts clearing from center to end on both sides.
            int middleIndex = (allBlocks.Count % 2 == 0) ? (allBlocks.Count / 2) : ((allBlocks.Count / 2) + 1);
            int leftIndex = (middleIndex - 1);
            int rightIndex = middleIndex;
            int totalBlocks = allBlocks.Count;

            for (int i = 0; i < middleIndex; i++, leftIndex--, rightIndex++)
            {
                if (leftIndex >= 0)
                {
                    if (allBlocks[leftIndex].spriteType == SpriteType.MilkBottle)
                    {
                        if (bottleCount >= 2)
                        {
                            canClear = true;
                            bottleCount = 0;
                        }

                        // MilkShop block clear only one block at a time
                        if (canClear)
                        {
                            allBlocks[leftIndex].milkShop.CollectMilkBottle();
                            canClear = false;
                        }
                        bottleCount++;
                        continue;
                    }   
                
                
                    if (TargetController.Instance.canCollectAnyEmptyBlock)
                    {
                        if(allBlocks[leftIndex].spriteType is SpriteType.Red or SpriteType.Yellow 
                        or SpriteType.Blue or SpriteType.Green or SpriteType.Cyan
                        or SpriteType.Purple or SpriteType.Pink or SpriteType.Orange)
                        {   
                            TargetController.Instance.UpdateTarget(allBlocks[leftIndex].transform, SpriteType.AllColourBlock);
                        }
                    }
                    allBlocks[leftIndex].ClearBlock();
                }
                //allBlocks[leftIndex].Clear();
                
                if (rightIndex <= totalBlocks)
                {
                    if (allBlocks[rightIndex].spriteType == SpriteType.MilkBottle)
                    {
                        if (bottleCount >= 2)
                        {
                            canClear = true;
                            bottleCount = 0;
                        }

                        // MilkShop block clear only one block at a time
                        if (canClear)
                        {
                            allBlocks[rightIndex].milkShop.CollectMilkBottle();
                            canClear = false;
                        }
                        bottleCount++;
                        continue;
                    }
                
                
                    if (TargetController.Instance.canCollectAnyEmptyBlock)
                    {
                        if(allBlocks[rightIndex].spriteType is SpriteType.Red or SpriteType.Yellow 
                        or SpriteType.Blue or SpriteType.Green or SpriteType.Cyan
                        or SpriteType.Purple or SpriteType.Pink or SpriteType.Orange)
                        {
                            TargetController.Instance.UpdateTarget(allBlocks[rightIndex].transform, SpriteType.AllColourBlock);
                        }
                    }
                    allBlocks[rightIndex].ClearBlock();
                    //allBlocks[rightIndex].Clear();
                }
                //yield return new WaitForSeconds(0.03F);
                yield return null;
            }
            Magnet.Instance.SetIsRowCompletedByPlacingMagnet(false);
            OnRowColoumCleared?.Invoke();
            blockShapeController.CheckAllShapesCanbePlaced();
            completedLineSpriteTag = null;
        }
        #endregion


        /// <summary>
        /// Returns all blocks from the given row.
        /// </summary>
        public List<Block> GetEntireRow(int rowId) {
            return allRows[rowId];
        }

        /// <summary>
        /// Returns all blocks from the given column.
        /// </summary>
        public List<Block> GetEntirColumn(int columnId)
        {
            return allColumns[columnId];
        }

        /// <summary>
        /// Returns true if row is about to complete on current block shape being placed otherwise false.
        /// </summary>
        public bool CanHighlightRow(int rowId) {
            return allRows[rowId].Find (o => o.isFilled == false) == null;
        }

        /// <summary>
        /// Returns true if given row if all blocks in given row are filled. Otherwise false.
        /// </summary>
        public bool IsRowCompleted(int rowId) {
             return allRows[rowId].Find (o => o.isFilled == false) == null;
        }

        /// <summary>
        /// Returns true if column is about to complete on current block shape being placed otherwise false.
        /// </summary>
        public bool CanHighlightColumn(int columnId)
        {
             return allColumns[columnId].Find (o => o.isFilled == false) == null;
        }

        /// <summary>
        /// Returns true if given column if all blocks in given row are filled. Otherwise false.
        /// </summary>
        public bool IsColumnCompleted(int columnId)
        {
            return allColumns[columnId].Find (o => o.isFilled == false) == null;
        }

        /// <summary>
        /// Highlights all block of from the row with given sprite.
        /// </summary>
        void HighlightRow(int rowId, Sprite sprite)
        {
            if (!cachedHighlightingRows.Contains(rowId))
            {
                foreach (Block block in allRows[rowId]) {
                    if (block.spriteType is SpriteType.Empty or SpriteType.Red or SpriteType.Blue or SpriteType.Green or 
                        SpriteType.Cyan or SpriteType.Yellow or SpriteType.Purple or SpriteType.Pink or SpriteType.Orange or 
                        SpriteType.Bubble or SpriteType.Magnet or SpriteType.Star or SpriteType.MusicalNode)
                    {
                        block.Highlight(sprite);
                    }
                }               
                cachedHighlightingRows.Add(rowId);
            }
        }

        /// <summary>
        /// Highlights all block of from the column with given sprite.
        /// </summary>
        void HighlightColumn(int columnId, Sprite sprite)
        {
            if (!cachedHighlightingColumns.Contains(columnId))
            {
                foreach (Block block in GetEntirColumn(columnId)) {
                    if (block.spriteType is SpriteType.Empty or SpriteType.Red or SpriteType.Blue or SpriteType.Green or 
                        SpriteType.Cyan or SpriteType.Yellow or SpriteType.Purple or SpriteType.Pink or SpriteType.Orange or 
                        SpriteType.Bubble or SpriteType.Magnet or SpriteType.Star or SpriteType.MusicalNode)
                    {
                        block.Highlight(sprite);
                    }
                }                
                cachedHighlightingColumns.Add(columnId);
            }
        }


        /// <summary>
        /// Highlights all rows with given sprite.
        /// </summary>
        public void HighlightAllRows(List<int> hittingRows, Sprite sprite)
        {
            foreach (int row in hittingRows)
            {
                HighlightRow(row, sprite);
            }
        }

        /// <summary>
        /// Highlights all columns with given sprite.
        /// </summary>
        public void HighlightAllColmns(List<int> hittingColumns, Sprite sprite)
        {
            foreach (int column in hittingColumns)
            {
                HighlightColumn(column, sprite);
            }
        }

        /// <summary>
        /// Stops highlighting all rows and all columns that is being highlighted.
        /// </summary>
        public void StopHighlight()
        {
            foreach (int row in highlightingRows)
            {
                StopHighlightingRow(row);
            }

            foreach (int column in highlightingColumns)
            {
                StopHighlightingColumn(column);
            }

            highlightingRows.Clear();
            highlightingColumns.Clear();
        }

        /// <summary>
        /// Stops highlighting all rows and all columns that is being highlighted except for given rows and column ids.
        /// </summary>
        public void StopHighlight(List<int> excludingRows, List<int> excludingColumns)
        {
            foreach (int row in highlightingRows)
            {
                if (!excludingRows.Contains(row))
                {
                    StopHighlightingRow(row);
                }
            }

            foreach (int column in highlightingColumns)
            {
                if (!excludingColumns.Contains(column))
                {
                    StopHighlightingColumn(column);
                }
            }
            highlightingRows.Clear();
            highlightingColumns.Clear();
        }

        /// <summary>
        /// Stops highlighting the given row.
        /// </summary>
        void StopHighlightingRow(int rowId)
        {
            canScaleUp = false;
            foreach (Block block in GetEntireRow(rowId)) {
                block.Reset();
            }

            if (cachedHighlightingRows.Contains(rowId)) {
                cachedHighlightingRows.Remove(rowId);
            }
        }

        /// <summary>
        /// Stops highlighting the given column.
        /// </summary>
        void StopHighlightingColumn(int columnId)
        {
            canScaleUp = false;
            foreach (Block block in GetEntirColumn(columnId)) {
                 block.Reset();
            }
            if (cachedHighlightingColumns.Contains(columnId)) {
                cachedHighlightingColumns.Remove(columnId);
            }
        }

        /// <summary>
        /// Reset the game. All the data, grid, and all UI will reset as fresh game.
        /// </summary>
        public void ResetGame()
        {
            boardGenerator.ResetGame();
            blockShapeController.ResetGame();
            NumberOfMoves = 0;
        }

        /// <summary>
        /// Activates rescue screen when user gets out of move.
        /// </summary>
        public async void CheckOutOfMove()
        {
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(0.7f));
            if (TargetController.Instance.isAllTargetsCollected)
            {
                return;
            }

            if (movingKitesList.Count <= 0 && !TargetController.Instance.isAllTargetsCollected)
            {
                blockShapeController.DisableAllBlockShapeContainerInput();
                maxMovesAllowed = 0;
                if (BoxingGlove.Instance.CanBoxingGloveUse && !TargetController.Instance.isAllTargetsCollected)
                {
                    BoxingGlove.Instance.OnBoxingGloveButtonPress();
                }
                else
                {
                    if (!BoxingGlove.Instance.IsBoxingGloveAlreadyActivated())
                    {
                        GamePlayUI.Instance.TryRescueGame(GameOverReason.OUT_OF_MOVE);
                    }
                }
            }

        }

        #region Rescue Specific Code
        public void PerformRescueAction(GameOverReason reason) 
        {
            switch(reason) 
            {
                //Rescue for Grid Filled and no new shape can be placed.
                //Below code will clear 3 lines horizontally and vertically from the grid.
                case GameOverReason.GRID_FILLED:
                    //ClearBoardLinesForRescue();
                    if (GamePlayUI.Instance.currentGameMode == GameMode.Timed)
                    {
                        GamePlayUI.Instance.timeModeProgresssBar.AddTime(appSettings.watchAdsRewardTime);
                        SpawnSingleBlocksForRescue();
                    }
                    else if (GamePlayUI.Instance.currentGameMode == GameMode.Blast)
                    {
                        RemoveCriticalBombs();
                        SpawnSingleBlocksForRescue();
                    }
                    else
                    {
                        SpawnSingleBlocksForRescue();
                    }
                    break;

                case GameOverReason.TIME_OVER:

                    if (GamePlayUI.Instance.currentGameMode == GameMode.Timed)
                    {
                        // Will add 15 seconds to tmer and will rescue game.
                        GamePlayUI.Instance.timeModeProgresssBar.AddTime(appSettings.watchAdsRewardTime);
                        SpawnSingleBlocksForRescue();
                    }
                    break;

                case GameOverReason.BOMB_BLAST:
                RemoveCriticalBombs();
                bool canAnyShapePlacedBlastRescue = blockShapeController.CheckBlockShapeCanPlaced();
                if(!canAnyShapePlacedBlastRescue) {
                    //ClearBoardLinesForRescue();
                    SpawnSingleBlocksForRescue();
                }
                break;
                
                case GameOverReason.OUT_OF_MOVE:
                    IncreaseMoves(appSettings.watchAdsRewardMoves);
                    UIController.Instance.UpdateMovesCount(maxMovesAllowed);
                    break;
            }
        }

        private void IncreaseMoves(int val)
        {
            maxMovesAllowed += val;
        }
        
        /// <summary>
        /// Spawns 3 single blocks after rescue is done.
        /// </summary>
        private void SpawnSingleBlocksForRescue()
        {
            string[] spriteTags = new string[3];
            int randomShapeType = UnityEngine.Random.Range(0, 2);
            BlockShapeInfo[] blockShapeInfo = randomShapeType == 0 ? GamePlayUI.Instance.GetStandardBlockShapes().ToArray() : GamePlayUI.Instance.GetAdvancedBlockShapes().ToArray();
            for (int i = 0; i < spriteTags.Length; i++)
            {
                if(randomShapeType == 0)
                {
                    spriteTags[i] = blockShapeController.GetRandomColor();
                }
                else
                {
                    spriteTags[i] = blockShapeController.GetRandomColor();
                }                
            }
            blockShapeController.SetSingleBlock(spriteTags);
        }
        #endregion

        /// <summary>
        /// Clear 3X3 lines from board.
        /// </summary>
        void ClearBoardLinesForRescue() {
            List<int> linesToClear = GetMiddleLinesFromGrid(3);

            GamePlay.Instance.ClearRows(linesToClear);
            GamePlay.Instance.ClearColumns(linesToClear);

            int linesCleared = (linesToClear.Count + linesToClear.Count);
            GamePlayUI.Instance.scoreManager.AddScore(linesCleared,0);

            if(linesCleared > 0) {
                AudioController.Instance.PlayLineBreakSound(linesCleared);
            }
        }

        void RemoveCriticalBombs() {
            BombInfo[] allBombInfo  = GetAllBombInfo();

            foreach(BombInfo bombInfo in allBombInfo) {
                if(bombInfo.remainCounter <= 4) {
                    allRows[bombInfo.rowId][bombInfo.columnId].ClearBombExplicitly();
                }
            }
        }

        //Returns the middle lines index from the grid. 
        // This logic can be sorten. :D
        public List<int> GetMiddleLinesFromGrid(int noOfLines) 
        {
            List<int> lines = new List<int>();
            int totalLines =  (int)GamePlayUI.Instance.GetBoardSize();
            int middleIndex = 0;

            if(totalLines % 2 == 0) 
            {
                middleIndex = ((totalLines / 2) - 1);

                if(noOfLines % 2 == 0) {
                    int sideLines = (noOfLines / 2);

                    for(int lineIndex = (middleIndex - (sideLines-1)); lineIndex <= middleIndex; lineIndex++) {
                        lines.Add(lineIndex);
                    } 
                    
                    for(int lineIndex = (middleIndex+1); lineIndex <= (middleIndex + sideLines); lineIndex++) {
                        lines.Add(lineIndex);
                    }
                } else {
                    int sideLines = (noOfLines / 2);

                    for(int lineIndex = (middleIndex - (sideLines)); lineIndex <= middleIndex; lineIndex++) {
                        lines.Add(lineIndex);
                    } 
                    
                    for(int lineIndex = (middleIndex+1); lineIndex <= (middleIndex + sideLines); lineIndex++) {
                        lines.Add(lineIndex);
                    }
                }

            } 
            else {
                middleIndex = (totalLines / 2);
                if(noOfLines % 2 == 0) {
                    int sideLines = (noOfLines / 2);

                    for(int lineIndex = (middleIndex - (sideLines)); lineIndex <= middleIndex; lineIndex++) {
                        lines.Add(lineIndex);
                    } 
                    
                    for(int lineIndex = (middleIndex+1); lineIndex < (middleIndex + sideLines); lineIndex++) {
                        lines.Add(lineIndex);
                    }
                } else {
                    int sideLines = (noOfLines / 2);

                    for(int lineIndex = (middleIndex - (sideLines)); lineIndex <= middleIndex; lineIndex++) {
                        lines.Add(lineIndex);
                    } 
                    
                    for(int lineIndex = (middleIndex+1); lineIndex <= (middleIndex + sideLines); lineIndex++) {
                        lines.Add(lineIndex);
                    }
                }
            }
            return lines;
        }

        public async void ClearAllBlockHat()
        {
            canHatOrMusicDiscClear = true;
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(2f));

            foreach (List<Block> blocks in allRows)
            {
                foreach (Block block in blocks)
                {
                    if (block.spriteType == SpriteType.Hat || block.secondarySpriteType == SpriteType.Hat)
                    {
                        if(block.spriteType == SpriteType.Ice)
                        {
                            //TargetController.Instance.PlayBlockBreakEffect(block.transform, block.blockImageLayer2.sprite);
                            block.PlaySimpleBlockBreakEffect(block.blockImageLayer2.sprite);
                        }
                        blockers.Remove(block);
                        block.SetBlockSpriteType(SpriteType.Empty);
                        block.ClearBlock();
                        //block.Clear();
                        canHatOrMusicDiscClear = false;
                        blockShapeController.CheckAllShapesCanbePlaced();
                    }
                }
            }
        }

        public Block GetFilledBlockForIceMachine()
        {
            List<Block> filledBlocks = new List<Block>();
            foreach (List<Block> blocks in allRows)
            {
                filledBlocks.AddRange(blocks.FindAll(o => o.isFilled && o.spriteType is not 
                                        SpriteType.IceMachine and not SpriteType.MilkBottle and not SpriteType.MagnetWithBubble));
            }

            if (filledBlocks.Count > 0)
            {
                filledBlocks.Shuffle();
                return filledBlocks[0];
            }
            return null;
        }

        public Block GetRandomIsFilledBlock()
        {
            List<Block> filledBlocks = new List<Block>();
            foreach (List<Block> blocks in allRows)
            {
                filledBlocks.AddRange(blocks.FindAll(o => o.isFilled));
            }
            /*
            if (filledBlocks.Count > 0)
            {
                filledBlocks.Shuffle();
                return filledBlocks[0];
            }
            */
            List<Block> fill = new List<Block>();
            foreach (Block b in filledBlocks)
            {
                if (b.spriteType is SpriteType.Empty or SpriteType.Red or SpriteType.Blue or SpriteType.Cyan or SpriteType.Green
                    or SpriteType.Purple or SpriteType.Orange or SpriteType.Pink or SpriteType.Yellow)
                {
                    fill.Add(b);
                }
            }
            if (fill.Count > 0)
            {
                fill.Shuffle();
                return fill[0];
            }
            return null;
        }

        public List<Block> GetRandomIsFilledBlockForRocket()
        {
            //List<Block> filledBlocks = new List<Block>();
            //foreach (List<Block> blocks in allShapes)
            //{
            //    filledBlocks.AddRange(blocks.FindAll(o => o.isFilled));             
            //}
            //if (filledBlocks.Count > 0)
            //{
            //    filledBlocks.Shuffle();
            //    filledBlocks[0].isFilled = false;
            //    return filledBlocks[0];
            //}
            //return null;

            if(allShapes.Count == 0)
            {
                return GetEmptyOrFilledBlockForRocket(TargetController.Instance.totalRocketGlobal);
            }
            int randomNumber = UnityEngine.Random.Range(0, allShapes.Count);
            List<Block> fillBlocksList = new List<Block>();
            fillBlocksList = allShapes[randomNumber];
            allShapes.RemoveAt(randomNumber);
            return fillBlocksList;
            
        }

        public List<Block> GetEmptyOrFilledBlockForRocket(int rocketNumber)
        {
            List<Block> boardBlocks = new List<Block>();
            foreach (List<Block> blocks in allRows)
            {
                boardBlocks.AddRange(blocks.FindAll(o => o.isFilled));
            }
            if(boardBlocks.Count < rocketNumber)
            {
                for(int i = 0; i < rocketNumber - boardBlocks.Count; i++)
                {
                   boardBlocks.Add(GamePlay.Instance.GetRandomEmptyBlockForRocket());
                }
            }
            return boardBlocks;
           
        }

        public int GetFilledBlockCount()
        {
            List<Block> filledBlocks = new List<Block>();
            foreach (List<Block> blocks in allRows)
            {
                filledBlocks.AddRange(blocks.FindAll(o => o.isFilled));
            }
            return filledBlocks.Count;
        }

        public async void ClearAllBlockMusicDiscs()
        {
            canHatOrMusicDiscClear = true;
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(0.8f));
            foreach (List<Block> blocks in allRows)
            {
                foreach (Block block in blocks)
                {
                    if (block.spriteType == SpriteType.MusicalPlayer || block.secondarySpriteType== SpriteType.MusicalPlayer)
                    {
                        if (block.spriteType == SpriteType.Ice)
                        {
                            block.PlaySimpleBlockBreakEffect(block.blockImageLayer2.sprite);
                        }
                        blockers.Remove(block);
                        block.SetBlockSpriteType(SpriteType.Empty);
                        block.ClearBlock();
                        canHatOrMusicDiscClear = false;
                        blockShapeController.CheckAllShapesCanbePlaced();
                    }
                }
            }
        }

        public void DestroyInstantiatedGameObjects()
        {
            for (int i = 0; i < instantiatedGameObjects.Count;)
            {
                GameObject obj = instantiatedGameObjects[i];
                Destroy(obj);
                instantiatedGameObjects.RemoveAt(i);
            }
        }

        public void SetBlockerStickStatus(bool status)
        {
            blockerStickEnabled = status;
        }

        public void SetJewelMachineStatus(bool status)
        {
            jewelMachineEnabled = status;
        }
    }
}
