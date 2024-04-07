using System.Collections.Generic;
using System.Linq;
using GamingMonks.UITween;
using UnityEngine;
using GamingMonks.Utils;
using UnityEngine.UI;
using GamingMonks.Tutorial;

namespace GamingMonks
{
    /// <summary>
    /// This script controlls the block shapes that being place/played on board grid. It controlls spawning of block shapes and organizing it.
    /// </summary>
    public class BlockShapesController : MonoBehaviour
    {
        // All The Block shape containers are added via inspector.
        [SerializeField] List<ShapeContainer> allShapeContainers;

        // Instance of block shape placement checker script component to check if given block shape can be placed on board grid or not.
        [System.NonSerialized] public BlockShapePlacementChecker blockShapePlacementChecker;

        // Pool of all block shape prepared with probability. Will stay unchanged during gameplay.
        List<BlockShapeInfo> blockShapesPoolFromLevel = new List<BlockShapeInfo>();

        // Upcoming block shapes pool copies elements from blockShapPool and keeps updating with game progress.
        List<BlockShapeInfo> upcomingBlockShapesFromLevel = new List<BlockShapeInfo>();

        List<BlockShapeProbabilityInfo> blockShapesPoolFromGameMode = new List<BlockShapeProbabilityInfo>();
        List<BlockShapeProbabilityInfo> upcomingBlockShapesFromGameMode = new List<BlockShapeProbabilityInfo>();

        // Pool of all colors for block shape. Will stay unchanged during gameplay.
        private List<string> blockColoursPool = new List<string>();
        private List<string> upcomingBlockColoursPool = new List<string>();
        
        // Size of block shape when its inactive and inside block shape container.
        Vector3 shapeInactiveScale = Vector3.one;

        bool hasInitialized = false;

        int totalShapesPlaced = 0;

        //public LevelSO levelSO;
        //List<GameObject> fixedBlockShape = new List<GameObject>();
        List<BlockShapeInfo> fixedBlockShapeInfo;
        private Level m_currentLevel;
        private int blockShapeCount = 0;
        private GamePlaySettings m_gamePlaySettings;

        private int m_spawnedBlockShape;
        private int m_index = -1;
        private bool m_pickBlockShapeFromLevel = false;
        private bool m_pickColoursFromLevel = false;

        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake()
        {
            // Initializes the GamePlay Settings Scriptable.
            if (m_gamePlaySettings == null)  {
                m_gamePlaySettings = (GamePlaySettings)Resources.Load("GamePlaySettings");
            }
            
            blockShapePlacementChecker = GetComponent<BlockShapePlacementChecker>();
           
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            ///  Registers game status callbacks.
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
        /// Prepares all block shapes based on gameplay settings.
        /// </summary>
        public void PrepareShapeContainer()
        {
            shapeInactiveScale = Vector3.one * GamePlayUI.Instance.currentModeSettings.shapeInactiveSize;
            PrepareShapePool();
            PrepareColourPool();

            PrepareUpcomingShapes();
            FillAllShapeContainers();
        }

        /// <summary>
        /// Prepares all block shapes based on gameplay settings.
        /// </summary>
        public void PrepareShapeContainer(ProgressData progressData)
        {
            m_currentLevel = GamePlayUI.Instance.currentLevel;
            shapeInactiveScale = Vector3.one * GamePlayUI.Instance.currentModeSettings.shapeInactiveSize;
            PrepareShapePool();
            PrepareColourPool();
            PrepareUpcomingShapes();

            if (progressData != null && GamePlayUI.Instance.currentGameMode != GameMode.Level)
            {
                FillAllShapeContainers(progressData.currentShapesInfo);

                totalShapesPlaced = progressData.totalShapesPlaced;
            }
            else
            {
                FillAllShapeContainers();
            }
        }

        /// <summary>
        /// Prepares a block shape pool with given probability amount as logical blocks shape references based on gameplay settings.
        /// </summary>
        void PrepareShapePool()
        {
            if (!hasInitialized)
            {
                if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
                {
                    fixedBlockShapeInfo = new List<BlockShapeInfo>();
                    fixedBlockShapeInfo = GamePlayUI.Instance.GetFixedBlockShapesInfo();

                    if (m_currentLevel.Mode.GameModeSettings.pickShapeFromThisLevel)
                    {
                        m_pickBlockShapeFromLevel = true;

                        if (GamePlayUI.Instance.currentModeSettings.standardShapeAllowed)
                        {
                            List<BlockShapeInfo> standardBlockShapes = GamePlayUI.Instance.GetStandardBlockShapes();
                            foreach (BlockShapeInfo shapeInfo in standardBlockShapes)
                            {
                                for (int i = 0; i < shapeInfo.spawnProbability; i++)
                                {
                                    blockShapesPoolFromLevel.Add(shapeInfo);
                                }
                            }
                        }

                        if (GamePlayUI.Instance.currentModeSettings.advancedShapeAllowed)
                        {
                            List<BlockShapeInfo> advancedShapes = GamePlayUI.Instance.GetAdvancedBlockShapes();
                            foreach (BlockShapeInfo info in advancedShapes)
                            {
                                for (int i = 0; i < info.spawnProbability; i++)
                                {
                                    info.blockShape.GetComponent<BlockShape>().isAdvanceShape = true;
                                    blockShapesPoolFromLevel.Add(info);
                                }
                            }
                        }
                    }
                    else
                    {
                        blockShapesPoolFromGameMode.AddRange(GamePlayUI.Instance.GetLevelBlockShapeInfo());
                    }
                }
                else
                {
                    if (GamePlayUI.Instance.currentModeSettings.standardShapeAllowed)
                    {
                        List<BlockShapeInfo> standardBlockShapes = GamePlayUI.Instance.GetStandardBlockShapes();
                        foreach (BlockShapeInfo shapeInfo in standardBlockShapes)
                        {
                            for (int i = 0; i < shapeInfo.spawnProbability; i++)
                            {
                                blockShapesPoolFromLevel.Add(shapeInfo);
                            }
                        }
                    }

                    if (GamePlayUI.Instance.currentModeSettings.advancedShapeAllowed)
                    {
                        List<BlockShapeInfo> advancedShapes = GamePlayUI.Instance.GetAdvancedBlockShapes();
                        foreach (BlockShapeInfo info in advancedShapes)
                        {
                            for (int i = 0; i < info.spawnProbability; i++)
                            {
                                info.blockShape.GetComponent<BlockShape>().isAdvanceShape = true;
                                blockShapesPoolFromLevel.Add(info);
                            }
                        }
                    }
                }
                
                hasInitialized = true;
            }
        }

       
        private void PrepareColourPool()
        {
            blockColoursPool.AddRange(GamePlayUI.Instance.GetColoursTag());
            if(GamePlayUI.Instance.currentModeSettings.pickShapeColorsFromThisLevel)
            {
                m_pickColoursFromLevel = true;
                PrepareUpcomingColourPool();
            }
        }

        private void PrepareUpcomingColourPool()
        {
            upcomingBlockColoursPool.AddRange(blockColoursPool);
            upcomingBlockColoursPool.Shuffle();
        }

        /// <summary>
        /// Adds a block shape on all the block containers.
        /// </summary>
        void FillAllShapeContainers()
        {
            foreach (ShapeContainer shapeContainer in allShapeContainers) {
                FillShapeInContainer(shapeContainer);
            }
        }

        /// <summary>
        /// Adds a block shape in the given shape container with animation effect.
        /// </summary>
        void FillShapeInContainer(ShapeContainer shapeContainer)
        {
            if(!shapeContainer.gameObject.activeInHierarchy)
            {
                shapeContainer.gameObject.SetActive(true);
            }

            if (shapeContainer.blockShape == null)
            {
                BlockShape blockShape = GetBlockShape();
                blockShape.transform.SetParent(shapeContainer.blockParent);
                shapeContainer.blockShape = blockShape;
                blockShape.transform.localScale = shapeInactiveScale;
                blockShape.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero.WithNewX(1500);
                blockShape.GetComponent<RectTransform>().AnchorX(0, 0.5F).SetDelay(0.3F).SetEase(Ease.EaseOut);
                
                if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
                {
                    if (m_currentLevel.SpecialBlockShape.Length > 0)
                    {
                        blockShapeCount++;

                        if (m_currentLevel.SpecialBlockShape[0].allowSpecialBlockShape &&
                            blockShapeCount >= m_currentLevel.SpecialBlockShape[0].probability)
                        {
                            blockShapeCount = 0;
                            blockShape.specialBlockEnabled = true;
                            blockShape.specialBlockSpriteType = GamePlayUI.Instance.currentLevel.SpecialBlockShape[0].spriteType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a block shape to last shape container. Typically will be called when gameplay setting have always keep all shapes filled.
        /// Upon placing a shape, all shapes will reorder and last shape container needs to add new block shape.
        /// </summary>
        void FillLastShapeContainer()
        {
            ShapeContainer shapeContainer = allShapeContainers[allShapeContainers.Count - 1];

            // Only add shape container is empty.
            if (shapeContainer.blockShape == null)
            {
                BlockShape blockShape = GetBlockShape();
                blockShape.transform.SetParent(shapeContainer.blockParent);
                shapeContainer.blockShape = blockShape;
                blockShape.transform.localScale = shapeInactiveScale;
                blockShape.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero.WithNewX(300);
                blockShape.GetComponent<RectTransform>().AnchorX(0, 0.3F).SetDelay(0.1F).SetEase(Ease.EaseOut);
                
                if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
                {
                    if (m_currentLevel.SpecialBlockShape.Length > 0)
                    {
                        blockShapeCount++;

                        if (m_currentLevel.SpecialBlockShape[0].allowSpecialBlockShape &&
                            blockShapeCount >= m_currentLevel.SpecialBlockShape[0].probability)
                        {
                            blockShapeCount = 0;
                            blockShape.specialBlockEnabled = true;
                            blockShape.specialBlockSpriteType = GamePlayUI.Instance.currentLevel.SpecialBlockShape[0].spriteType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds blocks to all shape containers from the previous session progress. Typically called when there is progress from previos session.
        /// </summary>
        void FillAllShapeContainers(ShapeInfo[] currentShapesInfo)
        {
            int shapeIndex = 0;
            foreach (ShapeInfo shapeInfo in currentShapesInfo)
            {
                BlockShapeInfo info = null;
                if (shapeInfo.isAdvanceShape)
                {
                    info = GamePlayUI.Instance.GetAdvancedBlockShapesInfo().ToList().Find(o => o.blockShape.name == shapeInfo.shapeName);
                }
                else
                {
                    info = GamePlayUI.Instance.GetStandardBlockShapesInfo().ToList().Find(o => o.blockShape.name == shapeInfo.shapeName);
                }

                FillShapeInContainer(allShapeContainers[shapeIndex], info, shapeInfo.shapeRotation);
                shapeIndex++;
            }

            CheckBlockShapeCanPlaced();
        }

        /// <summary>
        /// Adds block shapes to shape container with given info. Typically called when there is progress from previos session.
        /// </summary>
        void FillShapeInContainer(ShapeContainer shapeContainer, BlockShapeInfo info, float rotation)
        {
            if (shapeContainer.blockShape == null)
            {
                if (info != null)
                {
                    BlockShape blockShape = GetBlockShape(info, rotation);
                    blockShape.transform.SetParent(shapeContainer.blockParent);
                    shapeContainer.blockShape = blockShape;
                    blockShape.transform.localScale = shapeInactiveScale;
                    blockShape.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero.WithNewX(1500);
                    blockShape.GetComponent<RectTransform>().AnchorX(0, 0.5F).SetDelay(0.3F).SetEase(Ease.EaseOut);
                }
            }
        }

        /// <summary>
        /// Returns a specific block shape with given rotation. Typically used when placing block shapes from previous session.
        /// </summary>
        BlockShape GetBlockShape(BlockShapeInfo info, float rotation)
        {
            GameObject upcomingShape = (GameObject)Instantiate(info.blockShape);
            upcomingShape.name = upcomingShape.name.Replace("(Clone)", "");
            upcomingShape.transform.localEulerAngles = Vector3.zero.WithNewZ(rotation);
            return upcomingShape.GetComponent<BlockShape>();
        }

        /// <summary>
        /// Gets a block shape from the upcoming block shape pool. will handle empty state and fill upcoming shapes pool if detected empty.
        /// </summary>
        BlockShape GetBlockShape()
        {
            if (fixedBlockShapeInfo != null && fixedBlockShapeInfo.Count > 0)
            {
                GameObject upcomingFixShape = (GameObject)Instantiate(fixedBlockShapeInfo[0].blockShape);
                upcomingFixShape.name = upcomingFixShape.name.Replace("(Clone)", "");
                int randorRotation = fixedBlockShapeInfo[0].spawnProbability;
                upcomingFixShape.transform.localEulerAngles = Vector3.zero.WithNewZ(90 * randorRotation);
                upcomingFixShape.GetComponent<BlockShape>().SetSpriteTag(GetSpriteTag());
                fixedBlockShapeInfo.RemoveAt(0);
                return upcomingFixShape.GetComponent<BlockShape>();
            }

            GameObject spawningBlockshape = null;
            if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                if(m_pickBlockShapeFromLevel)
                {
                    if (upcomingBlockShapesFromLevel.Count <= 0)
                    {
                        PrepareUpcomingShapes();
                    }
                    spawningBlockshape = upcomingBlockShapesFromLevel[0].blockShape;
                    upcomingBlockShapesFromLevel.RemoveAt(0);
                } 
                else
                {
                    m_spawnedBlockShape++;
                    bool isFound = false;
                    float prob = 0;

                    while (!isFound)
                    {
                        m_index++;
                        if (m_index >= upcomingBlockShapesFromGameMode.Count)
                        {
                            m_index = 0;
                        }

                        prob = (upcomingBlockShapesFromGameMode[m_index].spawnCount * 100 / m_spawnedBlockShape);
                        if (upcomingBlockShapesFromGameMode[m_index].spawnProbality > prob)
                        {
                            isFound = true;
                        }
                    }

                    BlockShapeProbabilityInfo upcomingBlockShape = new BlockShapeProbabilityInfo();
                    upcomingBlockShape = upcomingBlockShapesFromGameMode[m_index];
                    upcomingBlockShape.spawnCount++;
                    spawningBlockshape = upcomingBlockShape.blockShape;

                    upcomingBlockShapesFromGameMode.RemoveAt(m_index);
                    upcomingBlockShapesFromGameMode.Add(upcomingBlockShape);
                }
            }
            else
            {
                if (upcomingBlockShapesFromLevel.Count <= 0)
                {
                    PrepareUpcomingShapes();
                }
                spawningBlockshape = upcomingBlockShapesFromLevel[0].blockShape;
                upcomingBlockShapesFromLevel.RemoveAt(0);
            }
           
            GameObject upcomingShape = (GameObject)Instantiate(spawningBlockshape);
            upcomingShape.name = upcomingShape.name.Replace("(Clone)", "");
            int randomRotation = UnityEngine.Random.Range(0, 4);
            upcomingShape.transform.localEulerAngles = Vector3.zero.WithNewZ(90 * randomRotation);
            BlockShape blockShapeToBeUsed = upcomingShape.GetComponent<BlockShape>();
            blockShapeToBeUsed.SetSpriteTag(GetSpriteTag());

            return blockShapeToBeUsed;
        }

        public string GetSpriteTag()
        {
            string spriteTag = "";
            if(GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                if (m_pickColoursFromLevel)
                {
                    if (upcomingBlockColoursPool.Count <= 0)
                    {
                        PrepareUpcomingColourPool();
                    }
                    spriteTag = upcomingBlockColoursPool[0];
                    upcomingBlockColoursPool.RemoveAt(0);
                    return spriteTag;
                }
                return GetRandomColor();
            }
            return GetRandomColor();
        }

        /// <summary>
        /// Prepares a pool of upcoming shapes.
        /// </summary>
        void PrepareUpcomingShapes()
        {
            if(m_pickBlockShapeFromLevel || GamePlayUI.Instance.currentGameMode != GameMode.Level)
            {
                upcomingBlockShapesFromLevel.AddRange(blockShapesPoolFromLevel);
                upcomingBlockShapesFromLevel.Shuffle();
            }
            else
            {
                upcomingBlockShapesFromGameMode.AddRange(blockShapesPoolFromGameMode);
                upcomingBlockShapesFromGameMode.Shuffle();
            }
        }

        /// <summary>
        /// Returns a random color selecting from GameplaySettings Scriptable Object.
        /// </summary>
        public string GetRandomColor()
        {
            int randomValue = Random.Range(0, blockColoursPool.Count);
            return blockColoursPool[randomValue];
        }
        
        /// <summary>
        /// Checks status of all block shape containers and fill or reorder based on gameplay settings and status.
        /// </summary>
        public void UpdateShapeContainers()
        {
            if (IsAllShapeContainerEmpty())
            {
                FillAllShapeContainers();
            }
            else
            {
                if (GamePlayUI.Instance.currentModeSettings.alwaysKeepFilled)
                {
                    ReorderShapeContainer();
                }
            }

            Invoke("CheckAllShapesCanbePlaced", 1F);
        }

        /// <summary>
        /// Reorders all shape containers after any block shape placed on the board.
        /// </summary>
        void ReorderShapeContainer()
        {
            ShapeContainer emptyShapeContainer = null;
            int emptyBlockIndex = 0;

            for (int i = 0; i < allShapeContainers.Count; i++)
            {

                if (allShapeContainers[i].blockShape != null)
                {
                    if (emptyShapeContainer != null)
                    {
                        BlockShape blockShape = allShapeContainers[i].blockShape;
                        blockShape.transform.SetParent(emptyShapeContainer.blockParent);
                        emptyShapeContainer.blockShape = blockShape;
                        allShapeContainers[i].blockShape = null;
                        blockShape.GetComponent<RectTransform>().AnchorX(0, 0.3F).SetDelay(0.1F).SetEase(Ease.EaseOut);
                        i = emptyBlockIndex;
                        emptyShapeContainer = null;
                    }
                }
                else
                {
                    if (emptyShapeContainer == null)
                    {
                        emptyShapeContainer = allShapeContainers[i];
                        emptyBlockIndex = i;
                    }
                }
            }
            FillLastShapeContainer();
        }

        /// <summary>
        /// Whether all block shape containers are empty or not.
        /// </summary>
        public bool IsAllShapeContainerEmpty()
        {
            foreach (ShapeContainer rect in allShapeContainers)
            {
                if (rect.blockShape != null)
                {
                    return false;
                }
            }
            return true;
        }

        #region Registered Events Callback
        /// <summary>
        /// Callback when any block shape places on board.
        /// </summary>
        private void GamePlayUI_OnShapePlacedEvent()
        {
            if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                if (GamePlayUI.Instance.currentLevel.ConveyorBelts.enabled || GamePlayUI.Instance.currentLevel.JewelMachine.enabled)
                {
                    Invoke("UpdateShapeContainers", 0.6F);
                }
                else
                {
                    Invoke("UpdateShapeContainers", 0.1F);
                }
            }
            else
            {
                Invoke("UpdateShapeContainers", 0.1F);
            }
            totalShapesPlaced += 1;
            
            #region Blast Mode Specific
            //if (GamePlayUI.Instance.currentGameMode == GameMode.Blast)
            if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
            {
                Level activeLevel = GamePlayUI.Instance.currentLevel;
                if (!activeLevel.allowBombs)
                {
                    return;
                }
                else
                {
                    if ((totalShapesPlaced % GamePlayUI.Instance.currentLevel.addBombAfterMoves) == 0)
                    {
                        GamePlay.Instance.PlaceBombAtRandomPlace();

                        //if(!PlayerPrefs.HasKey("bombTip")) {
                        //    UIController.Instance.ShowBombPlaceTip();
                        //    PlayerPrefs.SetInt("bombTip",1);
                        //}
                    }
                }
            }
            if (GamePlayUI.Instance.currentGameMode == GameMode.Blast)
            {
                if ((totalShapesPlaced % GamePlayUI.Instance.addBombAfterMoves) == 0)
                {
                    GamePlay.Instance.PlaceBlastModeBombAtRandomPlace();

                    if (!PlayerPrefs.HasKey("bombTip"))
                    {
                        UIController.Instance.ShowBombPlaceTip();
                        PlayerPrefs.SetInt("bombTip", 1);
                    }
                }
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Checks if any block shape from all containers can be placed on board. Game will go to rescue or gameover state upon returning false.
        /// </summary>
        public void CheckAllShapesCanbePlaced()
        {
            if (GamePlayUI.Instance.isGameWon || !GamePlay.Instance.isBoardReady || JewelMachineController.Instance.isMoving || GamePlay.Instance.canHatOrMusicDiscClear
                || GamePlay.Instance.movingKitesList.Count > 0)
                return;

            if (!IsAllShapeContainerEmpty())
            {
                bool canAnyShapePlaced = CheckBlockShapeCanPlaced();

                if (!canAnyShapePlaced)
                {
                    if (BoxingGlove.Instance.CanBoxingGloveUse)
                    {
                        BoxingGlove.Instance.OnBoxingGloveButtonPress();
                    }
                    else
                    {
                        if (!BoxingGlove.Instance.IsBoxingGloveAlreadyActivated() && PlayerPrefs.GetInt("firstFailClassicMode") != 0
                            && !UIController.Instance.gameOverScreen.activeSelf)
                        {
                            GamePlayUI.Instance.TryRescueGame(GameOverReason.GRID_FILLED);
                        }
                        else
                        {
                            GamePlayUI.Instance.OnGameOver();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if any block shape from all containers can be placed on board. Shapes that can't placed will have lesser opacity.
        /// </summary>
        public bool CheckBlockShapeCanPlaced()
        {
            bool canAnyShapePlaced = false;
            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                if (shapeContainer.blockShape != null)
                {
                    bool shapeCanbePlaced = blockShapePlacementChecker.CheckShapeCanbePlaced(shapeContainer.blockShape);
                    //shapeContainer.blockShape.GetComponent<CanvasGroup>().alpha = (shapeCanbePlaced) ? 1F : 0.5F;
                    #region Block Shape Fading When Shape Cannot place on Grid

                    string spritetag = shapeContainer.blockShape.GetComponent<BlockShape>().spriteTag;
                    if (shapeCanbePlaced)
                    {
                        for (int i = 0; i < shapeContainer.blockShape.ActiveBlocks.Count; i++)
                        {
                            Image image = shapeContainer.blockShape.ActiveBlocks[i].GetComponent<Image>();
                            image.color = Color.white;
                            image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spritetag);

                            if (shapeContainer.blockShape.specialBlockSpriteType == SpriteType.Kite)
                            {
                                if (shapeContainer.blockShape.ActiveBlocks[i] ==
                                    shapeContainer.blockShape.specialBlockActivated)
                                {
                                    image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(shapeContainer.blockShape.specialBlockSpriteType.ToString());
                                }
                            }
                            
                        }
                        shapeContainer.blockShape.isGray = false;
                    }
                    else
                    {
                        //shapeContainer.blockShape.ShapeGrayed();

                        for (int i = 0; i < shapeContainer.blockShape.ActiveBlocks.Count; i++)
                        {
                            Image image = shapeContainer.blockShape.ActiveBlocks[i].GetComponent<Image>();
                            image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.WhiteSprite.ToString());
                            image.color = Color.gray;
                        }
                        shapeContainer.blockShape.isGray = true;
                        
                    }
                    #endregion
                    if (shapeCanbePlaced)
                    {
                        canAnyShapePlaced = true;
                    }
                }
            }
            return canAnyShapePlaced;
        }

        /// <summary>
        /// Returns status of all block shape containers. Typically called when saving board progress.
        /// </summary>
        public ShapeInfo[] GetCurrentShapesInfo()
        {
            ShapeInfo[] allShapesInfo = new ShapeInfo[allShapeContainers.Count];
            int shapeIndex = 0;

            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                if (shapeContainer.blockShape != null)
                {
                    bool isAdvanceShape = shapeContainer.blockShape.isAdvanceShape;
                    allShapesInfo[shapeIndex] = new ShapeInfo(shapeContainer.blockShape.isAdvanceShape, shapeContainer.blockShape.name, shapeContainer.blockShape.transform.localEulerAngles.z);
                }
                else
                {
                    //allShapesInfo[shapeIndex] = new ShapeInfo(false, null, 0);
                    allShapesInfo[shapeIndex] = null;
                }
                shapeIndex++;
            }
            return allShapesInfo;
        }

        public ShapeData[] GetCurrentShapesInfoWithSpecialSpriteType()
        {
            ShapeData[] allShapesInfo = new ShapeData[allShapeContainers.Count];
            int shapeIndex = 0;

            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                if (shapeContainer.blockShape != null)
                {
                    bool isAdvanceShape = shapeContainer.blockShape.isAdvanceShape;
                    allShapesInfo[shapeIndex] = new ShapeData(shapeContainer.blockShape.isAdvanceShape, shapeContainer.blockShape.name, shapeContainer.blockShape.transform.localEulerAngles.z, 
                                                              shapeContainer.blockShape.specialBlockEnabled, shapeContainer.blockShape.specialBlockSpriteType, shapeContainer.blockShape.specialBlockIndex);
                }
                else
                {
                    //allShapesInfo[shapeIndex] = new ShapeInfo(false, null, 0);
                    allShapesInfo[shapeIndex] = null;
                }
                shapeIndex++;
            }
            return allShapesInfo;
        }

        /// <summary>
        /// Returns total nuber of block shapes placed during gameplay.
        /// </summary>
        public int GetTotalShapesPlaced()
        {
            return totalShapesPlaced;
        }

        /// <summary>
        /// Returns the middle block shape transform.
        /// </summary>
        public Transform GetMiddleBlockShapePosition()
        {
            return allShapeContainers[1].transform;
        }

        /// <summary>
        /// Toggles block shape container.
        /// </summary>
        public void ToggleBlockShapeContainer(bool value)
        {
            foreach (var allShape in allShapeContainers)
            {
                allShape.gameObject.SetActive(value);
            }
        }

        public void ResetAllShapesPosition()
        {
            foreach (var allShape in allShapeContainers)
            {
                if(allShape.gameObject.activeSelf && allShape.blockShape != null)
                {
                    allShape.blockShape.transform.localPosition = Vector3.zero;
                    allShape.blockShape.transform.localScale = Vector3.one * GamePlayUI.Instance.currentModeSettings.shapeInactiveSize;
                    allShape.blockShape.gameObject.GetComponent<Canvas>().sortingOrder = 1;
                }
            }
        }

        /// <summary>
        /// Sets the block shape rotations
        /// </summary>
        public void SetAllBlockShapeRotations(float[] rotations)
        {
            for(int i = 0;i < allShapeContainers.Count; i++)
            {
                if (allShapeContainers[i].blockShape != null)
                {
                    allShapeContainers[i].blockShape.SetRotation(rotations[i]);
                    allShapeContainers[i].blockShape.CheckBlockShapeCanPlaced();
                }
            }
        }

        /// <summary>
        /// Returns the block shape rotations
        /// </summary>
        public float[] GetAllBlockShapeRotations()
        {
            float[] allRotations = new float[3];
            for (int i = 0; i < allShapeContainers.Count; i++)
            {
                if (allShapeContainers[i].blockShape != null)
                {
                    allRotations[i] = allShapeContainers[i].blockShape.GetRotation();
                }
                else
                {
                    allRotations[i] = 0;
                }
            }
            return allRotations;
        }

        /// <summary>
        /// Returns all sprite types from the containers
        /// </summary>
        public string[] GetAllSpriteTags()
        {
            List<string> spriteTags = new List<string>();
            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                if (shapeContainer.blockShape != null)
                {
                    spriteTags.Add(shapeContainer.blockShape.spriteTag);
                }
                else
                {
                    spriteTags.Add(GetRandomColor());
                }
            }
            return spriteTags.ToArray();
        }

        public BlockShape[] GetAllBlockShapes()
        {
            List<BlockShape> blockShapes = new List<BlockShape>();
            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                if (shapeContainer.blockShape != null)
                {
                    blockShapes.Add(shapeContainer.blockShape);
                }
                else
                {
                    blockShapes.Add(null);
                }
            }
            return blockShapes.ToArray();
        }


        public void SetSingleBlock(string[] spriteTags)
        {
            //allShapeContainers.Clear();
            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                shapeContainer.Reset();
            }
            for(int i = 0; i < spriteTags.Length; i++)
            {
                SetAllBlockShapesToSingleBlockShape(allShapeContainers[i], spriteTags[i]);
            }
        }

        public void SetAllBlockShapesToSingleBlockShape(ShapeContainer shapeContainer, string spriteTags)
        {
            if (shapeContainer.blockShape == null)
            {
                if(string.IsNullOrEmpty(spriteTags))
                {
                    spriteTags = GamePlay.Instance.blockShapeController.GetRandomColor();
                }
                GameObject singleBlockShape = (GameObject)Instantiate(GamePlayUI.Instance.GetSingleBlockShape().blockShape.gameObject);
                singleBlockShape.name = "Shape-1";
                BlockShape blockShape = singleBlockShape.GetComponent<BlockShape>();
                blockShape.transform.SetParent(shapeContainer.blockParent);
                blockShape.SetSpriteTag(spriteTags);
                blockShape.isAdvanceShape = false;
                shapeContainer.blockShape = blockShape;
                blockShape.transform.localScale = shapeInactiveScale;
                blockShape.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

            }
        }

        public void RevertSingleBlock(ShapeData[] originalBlockShapes, string[] spriteTags)
        {
            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                shapeContainer.Reset();
            }
            for (int i = 0; i < spriteTags.Length; i++)
            {
                RevertAllBlockShapeToOriginalValue(allShapeContainers[i], originalBlockShapes[i], spriteTags[i]);
            }
        }

        public void RevertAllBlockShapeToOriginalValue(ShapeContainer shapeContainer, ShapeData originalBlockShapes, string spriteTags)
        {
            if (shapeContainer.blockShape == null && originalBlockShapes != null)
            {
                BlockShapeInfo blockShapeInfo = null;
                if (originalBlockShapes.isAdvanceShape)
                {
                    blockShapeInfo = GamePlayUI.Instance.GetAdvancedBlockShapesInfo().ToList().Find(o => o.blockShape.name == originalBlockShapes.shapeName);
                }
                else
                {
                    blockShapeInfo = GamePlayUI.Instance.GetStandardBlockShapesInfo().ToList().Find(o => o.blockShape.name == originalBlockShapes.shapeName);
                }

                BlockShape blockShape = GetBlockShape(blockShapeInfo, originalBlockShapes.shapeRotation);
                blockShape.transform.SetParent(shapeContainer.blockParent);
                blockShape.SetSpriteTag(spriteTags);
                shapeContainer.blockShape = blockShape;
                blockShape.transform.localScale = shapeInactiveScale;
                blockShape.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                if(originalBlockShapes.isSpecialShape)
                {
                    blockShape.specialBlockEnabled = true;
                    blockShape.specialBlockSpriteType = originalBlockShapes.spriteType;
                    blockShape.specialBlockIndex = originalBlockShapes.specialBlockIndex;
                }
            }
        }

        public void DisableBlockShapeContainerInput(BlockShape blockShape)
        {
            foreach(ShapeContainer shapeContainer in allShapeContainers)
            {
                if(shapeContainer.blockShape != null)
                {
                    if (shapeContainer.blockShape != blockShape)
                    {
                        shapeContainer.blockShape.GetBlockShapeImage().raycastTarget = false;
                    }
                }
            }
        }

        public void DisableAllBlockShapeContainerInput()
        {
            foreach(ShapeContainer shapeContainer in allShapeContainers)
            {
                if(shapeContainer.blockShape != null)
                {
                    shapeContainer.blockShape.GetBlockShapeImage().raycastTarget = false;
                }
            }
        }
        
        public void EnableAllBlockShapeContainerInput()
        {
            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                if(shapeContainer.blockShape != null)
                {
                    shapeContainer.blockShape.GetBlockShapeImage().raycastTarget = true;
                }
            }
        }

        public List<ShapeContainer> AllShapeContainer
        {
            get
            {
                return allShapeContainers;
            }
        }

        /// <summary>
        /// Resets all block shape containers.
        /// </summary>
        public void ResetGame()
        {
            m_index = -1;
            m_spawnedBlockShape = 0;
            totalShapesPlaced = 0;
            hasInitialized = false;
            m_pickColoursFromLevel = false;
            m_pickBlockShapeFromLevel = false;

            foreach (ShapeContainer shapeContainer in allShapeContainers)
            {
                shapeContainer.Reset();
            }

            blockColoursPool.Clear();
            if (upcomingBlockColoursPool != null)
            {
                upcomingBlockColoursPool.Clear();
            }

            if (blockShapesPoolFromLevel != null)
            {
                blockShapesPoolFromLevel.Clear();
            }

            if(upcomingBlockShapesFromLevel != null)
            {
                upcomingBlockShapesFromLevel.Clear();
            }

            if (fixedBlockShapeInfo != null)
            {
                fixedBlockShapeInfo.Clear();
            }

            if(blockShapesPoolFromGameMode != null)
            {
                blockShapesPoolFromGameMode.Clear();
               
            }

            if (upcomingBlockShapesFromGameMode != null)
            {
                upcomingBlockShapesFromGameMode.Clear();
            }
            /*
            if(m_pickColoursFromLevel)
            {
                upcomingBlockColoursPool.Clear();
                m_pickColoursFromLevel = false;
            }

            if (m_pickBlockShapeFromLevel)
            {
                blockShapesPoolFromLevel.Clear();
                upcomingBlockShapesFromLevel.Clear();
                fixedBlockShapeInfo.Clear();
                m_pickBlockShapeFromLevel = false;
            }
            else
            {
                blockShapesPoolFromGameMode.Clear();
                upcomingBlockShapesFromGameMode.Clear();
                m_index = -1;
                m_spawnedBlockShape = 0;
            }
            */
        }
    }
}