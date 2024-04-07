using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GamingMonks.Utils;
using GamingMonks.UITween;
using DigitalRuby.LightningBolt;
using GamingMonks.Features;
using GamingMonks.Feedbacks;
namespace GamingMonks
{
    /// <summary>
    /// This class component is attached to all blocks in the grid. 
    /// </summary>
	public class Block : MonoBehaviour
    {

        // Returns rowId 
        public int RowId
        {
            get
            {
                return _rowId;
            }
            private set
            {
                _rowId = value;
            }
        }

        //Returns columnId
        public int ColumnId
        {
            get
            {
                return _columnId;
            }
            private set
            {
                _columnId = value;
            }
        }

        // Represents row id of block in the grid.
        private int _rowId;

        // Represents columnId id of block in the grid.
        private int _columnId;

        // Block is filled  with current playing block shape.
         public bool isFilled = false;

        // Block is available to place block shape or not.
        [System.NonSerialized] public bool isAvailable = true;

        #region Blast Mode Specific
        // Whether Block contains bomb. Applied only to time mode.
        [System.NonSerialized] public bool isBalloonBomb = false;

        [System.NonSerialized] public bool hasTimeBomb = false;

        // Instance on bomb on the block. 
        [System.NonSerialized] public Bomb thisTimeBomb = null;

        [System.NonSerialized] public BalloonBomb thisBalloonBomb = null;
        #endregion

        // Default sprite tag on the block. Will update runtime.
        public string defaultSpriteTag;

        // Sprite that is assigned on the block. Will update runtime.
        public string assignedSpriteTag;

        //Default sprite that is assigned to block.
        Sprite defaultSprite;

        // Box collide attached to this block.
        public BoxCollider2D thisCollider { get; private set; }

        // Image component on the block. Assigned from Inspector.
        [SerializeField] public Image blockImage;
        [SerializeField] public Image blockImageLayer1;
        [SerializeField] public Image blockImageLayer2;
        [SerializeField] public Image conveyorImage;
        [SerializeField] Image highlightLayer;

        [SerializeField] private GameObject m_bubbleAnimation;
        [SerializeField] private ParticleSystem m_simpleBlockBreakEffect;

        #region Ice Machine Specific
        // Whether Block contains IceMachine
        [System.NonSerialized] public bool isIceMachine = false;
        
        // Instance of Ice machine
        [System.NonSerialized] public IceMachine thisIceMachine = null;
        #endregion
        
        public SpriteType spriteType;
        public SpriteType secondarySpriteType;
        public bool hasStages = false;
        [System.NonSerialized] public bool isIceBomb = false;
        public int stage = 0;
        public MilkShop milkShop;

        [System.NonSerialized] public ConveyorMoverBlock conveyorMoverBlock = null;
        [System.NonSerialized] public JewelMoverBlock jewelMoverBlock = null;
        [System.NonSerialized] public Jewel jewel = null;
        [System.NonSerialized] public IceBomb thisIceBomb = null;
        [System.NonSerialized] public MusicalPlayer thisMusicalPlayer = null;
        [System.NonSerialized] public MagicHat thisMagicHat = null;
        [System.NonSerialized] public Diamond diamond;
        [System.NonSerialized] public bool isHitByKite;
        public SpriteType defaultSpritetype;
        [System.NonSerialized] public bool hasBlockerStick = false;
        [System.NonSerialized] public bool isHitByBoxingGlove = false;
        
        // Whether Block contains these.
        public bool hasDiamond = false;
        public bool hasMilkShop = false;
        public bool isHittedByBalloonBomb;
        [System.NonSerialized] public bool hasJewel = false;
        public bool canScaleUp = false;
        [System.NonSerialized] public bool justBorn = false;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            /// Initializes the collider component on Awake.
            thisCollider = GetComponent<BoxCollider2D>();
            spriteType = SpriteType.Empty;
        }

        /// <summary>
        /// Assignes logical position on block on the grid.
        /// </summary>
        public void SetBlockLocation(int rowIndex, int columnIndex)
        {
            RowId = rowIndex;
            ColumnId = columnIndex;
        }

        /// <summary>
        /// Highlights block with given sprite.
        /// </summary>
        public void Highlight(Sprite sprite)
        {
            blockImage.sprite = sprite;
            blockImage.enabled = true;
            isFilled = true;
        }

        // BombPowerUp will call
        public void Highlight()
        {
            highlightLayer.enabled = true;
        }

        /// <summary>
        /// Resets block to its default state.
        /// </summary>
        public void Reset()
        {
            if (spriteType==SpriteType.Kite){
                defaultSprite=ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            }
            if (!isAvailable)
            {
                blockImage.sprite = defaultSprite;
                //isFilled = true;
            }
            else
            {
                blockImage.enabled = false;
                isFilled = false;
            }
            highlightLayer.enabled = false;
        }

        /// <summary>
        /// Places block from the block shape. Typically will be called during gameplay.
        /// </summary>
        public void PlaceBlock(Sprite sprite, string spriteTag)
        {
            thisCollider.enabled = false;
            if(sprite != null)
            {
                blockImage.enabled = true;
                blockImage.sprite = sprite;
                blockImage.color = blockImage.color.WithNewA(1);
            }
            defaultSprite = sprite;
            isFilled = true;
            isAvailable = false;
            assignedSpriteTag = spriteTag;
        }

        
        // will be called when Magnet containing blockshape placed on grid
        public void PlaceBlock(Sprite sprite, string spriteTag, SpriteType spriteType)
        {
            if (spriteType == SpriteType.Kite)
            {
                sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            }
            else
            {
                blockImageLayer1.enabled = true;
                blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
                blockImageLayer1.color = blockImage.color.WithNewA(1);
            }

            if (this.spriteType == SpriteType.Bubble)
            {
                if (spriteType == SpriteType.Kite)
                {
                    secondarySpriteType = spriteType;
                }
                else
                {
                    secondarySpriteType = this.spriteType;
                    this.spriteType = spriteType == SpriteType.Magnet ? SpriteType.MagnetWithBubble : spriteType;
                }
            }
            else
            {
                this.spriteType = spriteType;
            }

            PlaceBlock(sprite, spriteTag);

            if (this.spriteType is SpriteType.Magnet or SpriteType.MagnetWithBubble)
            {
                Magnet.Instance.CheckForRowOrColoumClear(_rowId, _columnId);
            }
        }

        /// <summary>
        /// Places block from the block shape. Typically will be called when place something on the that block.
        /// </summary>
        public void PlaceBlock(SpriteType spriteType)
        {
            thisCollider.enabled = false;
            isFilled = true;
            isAvailable = false;
            assignedSpriteTag = spriteType.ToString();
        }
        
        // will get called when clearing milkbottle
        public void ChangeBlockImage()
        {
            blockImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            blockImage.color = blockImage.color.WithNewA(1);
        }

        // will get called for the conveyorBelt after moving block
        public void ResetBlock()
        {
            isFilled = false;
            isAvailable = true;
            thisCollider.enabled = true;
            secondarySpriteType = SpriteType.Empty;
            assignedSpriteTag = defaultSpriteTag;
            blockImage.sprite = null;
            blockImage.enabled = false;
            blockImageLayer1.sprite = null;
            blockImageLayer1.enabled = false;
            hasStages = false;
            stage = 0;

            if (hasJewel)
            {
                jewel.SetParentBlock(null);
                jewel = null;
                hasJewel = false;
                GamePlay.Instance.blockers.Remove(this);
            }
            
            if(isBalloonBomb)
            {
                thisBalloonBomb.block = null;
                isBalloonBomb = false;
                thisBalloonBomb = null;
            }

            if(isIceBomb)
            {
                thisIceBomb.block = null;
                isIceBomb = false;
                thisIceBomb = null;
            }
            if (hasDiamond)
            {
                // GamePlay.Instance.isBoardReady = false;
                diamond.currentBlock = null;
                hasDiamond = false;
                diamond = null;
            }

            if (thisMusicalPlayer != null)
            {
                thisMusicalPlayer = null;
            }
            if (thisMagicHat != null)
            {
                thisMagicHat = null;
            }

            if (spriteType != SpriteType.Bubble)
            {
                blockImageLayer2.enabled = false;
                SetBlockSpriteType(SpriteType.Empty);
            }
            if (isIceMachine)
            {
                isIceMachine = false;
                thisIceMachine.block = null;
                thisIceMachine = null;
            }
        }

        // set sprite for the conveyor belt
        public void ChangeConveyorSprite(string spriteTag)
        {
            defaultSpriteTag = spriteTag;
            conveyorImage.enabled = true;
            conveyorImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteTag);
            
        }

        /// <summary>
        /// Highlight Conveyor block direction sprite. 
        /// </summary>
        public IEnumerator HighlightConveyor()
        {
            conveyorImage.color = conveyorImage.color.WithNewA(1);
            yield return new WaitForSeconds(.2f);
            conveyorImage.color = conveyorImage.color.WithNewA(.7f);
        }
        
        /// <summary>
        /// Places block from the block shape. Typically will be called when game starting with progress from previos session.
        /// </summary>
        public void PlaceBlock(string spriteTag)
        {
            Sprite sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteTag);
            thisCollider.enabled = false;
            blockImage.enabled = true;
            blockImage.sprite = sprite;
            blockImage.color = blockImage.color.WithNewA(1);
            defaultSprite = sprite;
            isFilled = true;
            isAvailable = false;
            assignedSpriteTag = spriteTag;
        }

        /// <summary>
        /// Clears block. Will be called when line containing this block will get completed. This is typical animation effect of how completed block shoudl disappear.
        /// references - 159, 166, 500 (default code)
        /// </summary>
        public void Clear()
        {
            transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            
            // BlockImage will scale down to 0 in 0.35 seconds. and will reset to scale 1 on animation completion.
            UIFeedback.Instance.PlayHapticLight();
            blockImage.transform.LocalScale(Vector3.zero, 0.35F).OnComplete(() =>
            {
                blockImage.transform.localScale = Vector3.one;
                blockImage.sprite = null;
            });

            blockImage.transform.LocalRotationToZ(90, 0.2F).OnComplete(()=> {
                blockImage.transform.localEulerAngles = Vector3.zero;
            });

            transform.GetComponent<Image>().SetAlpha(1, 0.35F).SetDelay(0.3F);

            // Opacity of block image will set to 0 in 0.3 seconds. and will reset to 1 on animation completion.
            blockImage.SetAlpha(0.5F, 0.3F).OnComplete(() =>
            {
                blockImage.enabled = false;
            });
            blockImageLayer1.SetAlpha(0.5F, 0.3F).OnComplete(() =>
            {
                blockImageLayer1.enabled = false;
            });
            blockImageLayer2.SetAlpha(0.5F, 0.3F).OnComplete(() =>
            {
                blockImageLayer2.enabled = false;
            });

           
            if (isFilled == true)
            {
                if (thisMusicalPlayer == null && thisMagicHat == null && !isIceBomb && !isBalloonBomb) 
                {
                    PlayVariousBlockBreakEffect();
                }
            }

            /*
            isFilled = false;
            isAvailable = true;
            thisCollider.enabled = true;
            assignedSpriteTag = defaultSpriteTag;
            SetBlockSpriteType(SpriteType.Empty);
            */
            if (!hasMilkShop)
            {
                isFilled = false;
                isAvailable = true;
                thisCollider.enabled = true;
                assignedSpriteTag = defaultSpriteTag;
                spriteType = SpriteType.Empty;
                secondarySpriteType = SpriteType.Empty;
            }
            isHitByBoxingGlove = false;
            isHittedByBalloonBomb = false;

            #region Blast Mode Specific
            if (isBalloonBomb)
            {
                isBalloonBomb = false;
                Destroy(thisBalloonBomb.gameObject);
                thisBalloonBomb = null;
                //ClearBomb();
            }

            if (isIceBomb)
            {
                isIceBomb = false;
                Destroy(thisIceBomb.gameObject);
                thisIceBomb = null;
                //ClearIceBomb();
            }
            if (hasTimeBomb)
            {
                hasTimeBomb = false;
                ClearBomb();
            }
            #endregion

            #region Instantiated GameObjects
            if (isIceMachine)
            {
                isIceMachine = false;
                ClearIceMachine();
            }

            if (thisMusicalPlayer != null)
            {
                DestroyMusicalPlayer();
            }

            if (thisMagicHat != null)
            {
                DestroyMagicHat();
            }
            
            if (hasJewel)
            {
                Destroy(jewel.gameObject);
                jewel = null;
                hasJewel = false;
            }
            #endregion

            // GamePlay.Instance.InvokeTHatFunc();
        }
        
        /// <summary>
        /// This function will pick the random particle like smiley, ThumbsUp, Hearts etc. and play after changing the color based on the block's color 
        /// </summary>
        private void PlayVariousBlockBreakEffect()
        {
           GameObject particleObj = Instantiate(GamingMonksFeedbacks.Instance.currentBlockBreakEffect,
                transform.position, Quaternion.identity, TargetController.Instance.particleParent);

            ParticleSystem[] particleSystems = particleObj.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                ParticleSystem.MainModule mainModule = ps.main;
                switch (assignedSpriteTag)
                {
                    case "b1" or "Blue":
                        Color blueColor = new Color32(4, 126, 226, 255);
                        mainModule.startColor = blueColor;
                        break;

                    case "b2" or "Cyan":
                        Color cyanColor = new Color32(10, 212, 246, 255);
                        mainModule.startColor = cyanColor;
                        break;

                    case "b3" or "Green":
                        Color greenColor = new Color32(44, 214, 25, 255);
                        mainModule.startColor = greenColor;
                        break;

                    case "b4" or "Pink":
                        Color pinkColor = new Color32(252, 67, 153, 255);
                        mainModule.startColor = pinkColor;
                        break;

                    case "b5" or "Purple":
                        Color purpleColor = new Color32(125, 38, 205, 255);
                        mainModule.startColor = purpleColor;
                        break;

                    case "b6" or "Red":
                        Color redColor = new Color32(244, 12, 22, 255);
                        mainModule.startColor = redColor;
                        break;
                }
            }

            particleObj.SetActive(true);
            Destroy(particleObj, 5f);
        }

        #region Blast Mode Specific
        /// <summary>
        /// Place a new bomb on the block.
        /// </summary>
        /// 

        public void PlaceBomb(int remainCounter)
        {
            GameObject bomb = (GameObject)Instantiate(GamePlay.Instance.bombTemplate, blockImage.transform);
            thisCollider.enabled = false;
            // bomb.transform.SetParent(blockImage.transform);
            // bomb.transform.localScale = Vector3.zero;
            bomb.transform.localPosition = Vector3.zero;
            thisTimeBomb = bomb.GetComponent<Bomb>();
            thisTimeBomb.SetCounter(remainCounter);
            bomb.SetActive(true);
            // bomb.transform.LocalScale(Vector3.one, 0.25F);
            isFilled = true;
            isAvailable = false;
            hasTimeBomb = true;
            blockImage.enabled = false;
        }

        public void PlaceBalloonBomb(int remainCounter)
        {
            GameObject bomb = (GameObject)Instantiate(GamePlay.Instance.balloonBombTemplate, blockImage.transform);
            GamePlay.Instance.instantiatedGameObjects.Add(gameObject);
            bomb.transform.localPosition = Vector3.zero;
            thisBalloonBomb = bomb.GetComponent<BalloonBomb>();
            thisBalloonBomb.block = this;
            bomb.SetActive(true);
            thisBalloonBomb.SetCounter(remainCounter);
            bomb.transform.LocalScale(Vector3.one, 0.25F);
            spriteType = SpriteType.BalloonBomb;
            thisCollider.enabled = false;
            isFilled = true;
            isAvailable = false;
            isBalloonBomb = true;
            blockImage.enabled = false;
        }

        public void PlaceIceBomb(int remainCounter)
        {
            IceBomb bomb = Instantiate(GamePlay.Instance.iceBomb,transform.position,
                Quaternion.identity, blockImage.transform);
            bomb.transform.localScale = Vector3.zero;
            thisIceBomb = bomb;
            bomb.GetComponent<IceBomb>().block = this;
            thisIceBomb.SetCounter(remainCounter);
            bomb.gameObject.SetActive(true);
            bomb.transform.LocalScale(Vector3.one, 0.25F);
            spriteType = SpriteType.IceBomb;
            thisCollider.enabled = false;
            isFilled = true;
            isAvailable = false;
            isIceBomb = true;
            blockImage.enabled = false;
        }
        
        private void PlaceMusicalPlayer()
        {
            MusicalPlayer musicalPlayer = Instantiate(GamePlay.Instance.musicalPlayerPrefab, transform.position,
                Quaternion.identity, blockImage.transform);
            thisMusicalPlayer = musicalPlayer;
            musicalPlayer.transform.LocalScale(Vector3.one, 0.25F);
            PlaceBlock(spriteType);
            GamePlay.Instance.instantiatedGameObjects.Add(musicalPlayer.gameObject);
        }

        private void PlaceMagicHat()
        {
            MagicHat magicHat = Instantiate(GamePlay.Instance.magicHatPrefab, transform.position,
               Quaternion.identity, blockImage.transform);
            thisMagicHat = magicHat;
            magicHat.transform.LocalScale(Vector3.one, 0.25F);
            PlaceBlock(spriteType);
            GamePlay.Instance.instantiatedGameObjects.Add(magicHat.gameObject);
        }
        
        /// <summary>
        /// Remove attached bomb from the block. Will be executes when block gets cleared on completing line.
        /// </summary>
        void ClearBomb()
        {
            if (blockImage.transform.childCount > 0)
            {
                UIFeedback.Instance.PlayHapticHeavy();
                thisTimeBomb.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
                {
                    Destroy(thisTimeBomb.gameObject);
                    thisTimeBomb = null;
                });
            }
        }

        void ClearIceBomb()
        {
            if (blockImage.transform.childCount > 0)
            {
                //UIFeedback.Instance.PlayHapticHeavy();
                thisIceBomb.transform.LocalScale(Vector3.zero, 0.15F).OnComplete(() =>
                {
                    //thisBomb.GetComponent<BalloonBomb>().destroyableBlocks.Clear();
                    Destroy(thisIceBomb.gameObject);
                    thisIceBomb = null;
                });
            }
        }

        /// <summary>
        /// Remove attached bomb from the block. Will be executes when block gets cleared on completing line.
        /// </summary>
        public void ClearBombExplicitly()
        {
            isFilled = false;
            isAvailable = true;
            thisCollider.enabled = true;
            assignedSpriteTag = defaultSpriteTag;
            hasTimeBomb = false;
            
            if (blockImage.transform.childCount > 0)
            {
                UIFeedback.Instance.PlayHapticHeavy();
                thisTimeBomb.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
                {
                    Destroy(thisTimeBomb.gameObject);
                    thisTimeBomb = null;
                });
            }
        }

        /// <summary>
        /// Returns Remaining counter on bomb attached to block.static Applies only to blast mode.
        /// </summary>
        public int GetRemainingCounter()
        {
            if (thisBalloonBomb != null)
            {
                return thisBalloonBomb.remainingCounter;
            }
            return GamePlayUI.Instance.blastModeCounter;
        }
        #endregion

        #region Ice Machine
        public void PlaceIceMachine(int remainCounter, bool hasStages, int stage)
        {
            this.hasStages = hasStages;
            this.stage = stage;
            spriteType = SpriteType.IceMachine;
            GameObject iceMachine = (GameObject)Instantiate(GamePlay.Instance.iceMachine, blockImage.transform);
            if (hasStages)
            {
                Image image = iceMachine.gameObject.GetComponent<Image>();
                image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString() + "Stage" + this.stage);
                assignedSpriteTag = spriteType.ToString() + "Stage" + this.stage;
            }

            thisCollider.enabled = false;
            iceMachine.transform.position = transform.position;
            thisIceMachine = iceMachine.GetComponent<IceMachine>();
            thisIceMachine.block = this;
            thisIceMachine.SetCounter(remainCounter);
            iceMachine.SetActive(true);
            iceMachine.transform.LocalScale(Vector3.one, 0.25F);
            GamePlay.Instance.instantiatedGameObjects.Add(iceMachine);
            GamePlay.Instance.blockers.Add(this);
            isFilled = true;
            isAvailable = false;
            isIceMachine = true;
        }

        private void DestroyMusicalPlayer()
        {
            thisMusicalPlayer.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
            {
                GamePlay.Instance.instantiatedGameObjects.Remove(thisMusicalPlayer.gameObject);
                Destroy(thisMusicalPlayer.gameObject);
                thisMusicalPlayer = null;
            });
        }

        private void DestroyMagicHat()
        {
            thisMagicHat.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
            {
                GamePlay.Instance.instantiatedGameObjects.Remove(thisMagicHat.gameObject);
                GamePlay.Instance.blockers.Remove(this);
                Destroy(thisMagicHat.gameObject);
                thisMagicHat = null;
            });
        }

        void ClearIceMachine()
        {
            thisIceMachine.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
            {
                GamePlay.Instance.instantiatedGameObjects.Remove(thisIceMachine.gameObject);
                GamePlay.Instance.blockers.Remove(this);
                Destroy(thisIceMachine.gameObject);
                thisIceMachine = null;
            });
        }
        #endregion

        public void ClearBlock()
        {
            switch (spriteType)
            {
                case SpriteType.Bubble:
                    GamePlay.Instance.blockers.Remove(this);
                    blockImageLayer2.enabled = false;
                    TargetController.Instance.UpdateTarget(transform, spriteType);
                    PlayBubbleBreakEffect();
                    if (secondarySpriteType is SpriteType.Kite or SpriteType.Spell or SpriteType.HorizontalSpell or 
                        SpriteType.VerticalSpell or SpriteType.ColouredJewel or SpriteType.UnColouredJewel)
                    {
                        spriteType = secondarySpriteType;
                        ClearBlock();
                        return;
                    }
                    Clear();
                    break;

                case SpriteType.BubbleWithIce:
                    if (stage > 1)
                    {
                        --stage;
                        PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice + "Stage" + stage);
                        assignedSpriteTag = spriteType + "Stage" + stage;
                    }
                    else
                    {
                        PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                        TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                        hasStages = false;
                        stage = 0;
                        blockImageLayer2.enabled = false;
                        spriteType = SpriteType.Bubble;
                        assignedSpriteTag = spriteType.ToString();
                    }
                    break;
                
                case SpriteType.Hat:
                    
                    if (GamePlay.Instance.birdCount > 0)
                    {
                        GamePlay.Instance.birdCount -= 1;
                        TargetController.Instance.UpdateTarget(transform, spriteType);

                        //If target is equals to 0 after updating then destroy hats.
                        if (GamePlay.Instance.birdCount <= 0)
                        {
                            GamePlay.Instance.ClearAllBlockHat();
                        }
                    }
                    break;

                case SpriteType.Magnet:
                    GamePlay.Instance.blockers.Remove(this);
                    blockImageLayer1.enabled = false;
                    TargetController.Instance.UpdateTarget(transform, SpriteType.Magnet);
                    if (secondarySpriteType != SpriteType.Empty)
                    {
                        TargetController.Instance.UpdateTarget(transform, secondarySpriteType);
                        TargetController.Instance.UpdateTarget(transform, SpriteType.AllColourBlock);
                    }
                    Clear();
                    break;

                case SpriteType.MagnetWithBubble:
                    GamePlay.Instance.blockers.Remove(this);
                    blockImageLayer2.enabled = false;
                    blockImageLayer1.enabled = false;
                    TargetController.Instance.UpdateTarget(transform, SpriteType.Bubble);
                    PlayBubbleBreakEffect();
                    Clear();
                    break;

                case SpriteType.Kite:
                    GamePlay.Instance.blockers.Remove(this);
                    blockImage.enabled = false;
                    if (secondarySpriteType == SpriteType.Bubble)
                    {
                        TargetController.Instance.UpdateTarget(transform, secondarySpriteType);
                    }
                    GameObject kite = Instantiate(GamePlay.Instance.Kite, transform.position, GamePlay.Instance.Kite.transform.rotation,
                                                    GamePlay.Instance.instantiatedGameObjectsParent);
                    GamePlay.Instance.movingKitesList.Add(kite);
                    GamePlay.Instance.instantiatedGameObjects.Add(kite);
                    kite.transform.localScale = Vector3.one;
                    Clear();
                    break;

                case SpriteType.IceMachine:
                    if (hasStages)
                    {
                        Image iceImage = thisIceMachine.gameObject.GetComponent<Image>();
                        if (stage > 1)
                        {
                            --stage;
                            iceImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString() + "Stage" + this.stage);
                            PlaySimpleBlockBreakEffect(iceImage.sprite);
                            assignedSpriteTag = spriteType + "Stage" + stage;
                            
                        }
                        else
                        {
                            hasStages = false;
                            stage = 0;
                            thisIceMachine.block.justBorn = true;
                            Clear();
                        }
                        
                    }
                    else
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        thisIceMachine.block.justBorn = true;
                        Clear();
                    }
                    break;

                case SpriteType.MusicalPlayer:
                    thisMusicalPlayer.ProduceMusicalNote();
                    break;

                case SpriteType.MusicalNode:
                    GamePlay.Instance.blockers.Remove(this);
                    TargetController.Instance.UpdateTarget(transform, spriteType);
                    this.blockImageLayer1.enabled = false;
                    Clear();
                    break;

                case SpriteType.JewelMachine:
                    //TargetController.Instance.UpdateTarget(this, spriteType);
                    break;

                case SpriteType.ColouredJewel:
                    TargetController.Instance.UpdateTarget(transform, spriteType);
                    GamePlay.Instance.blockers.Remove(this);
                    Clear();
                    break;

                case SpriteType.UnColouredJewel:
                    GamePlay.Instance.blockers.Remove(this);
                    Clear();
                    break;

                case SpriteType.Diamond:
                    break;

                case SpriteType.MagnetWithIce:
                    if (hasStages)
                    {
                        if (stage > 1)
                        {
                            --stage;
                            PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                            blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice + "Stage" + stage);
                            assignedSpriteTag = spriteType + "Stage" + stage;
                        }
                        else
                        {
                            TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                            hasStages = false;
                            stage = 0;
                            blockImageLayer2.enabled = false;
                            spriteType = SpriteType.Magnet;
                            PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                            Magnet.Instance.CheckForRowOrColoumClear(_rowId, _columnId);
                            assignedSpriteTag = spriteType.ToString();
                        }
                    }
                    else
                    {
                        TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                        blockImageLayer2.enabled = false;
                        spriteType = SpriteType.Magnet;
                        PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                        Magnet.Instance.CheckForRowOrColoumClear(_rowId, _columnId);
                        assignedSpriteTag = spriteType.ToString();                        
                    }
                    break;

                case SpriteType.CloseShell:
                    if (isHitByBoxingGlove)
                    {
                        isHitByBoxingGlove = false;
                        break;
                    }
                   
                    spriteType = SpriteType.OpenShell;
                    assignedSpriteTag = spriteType.ToString();
                  
                    break;

                case SpriteType.OpenShell:
                    TargetController.Instance.UpdateTarget(transform, SpriteType.OpenShell);
                    GamePlay.Instance.blockers.Remove(this);
                    PearlAndShellController.Instance.shells.Remove(this);
                    Clear();
                    break;

                case SpriteType.Spell:
                    Clear();
                    PlayColoumBreakingEffect(this._columnId);
                    PlayRowBreakingEffect(this._rowId);
                    break;

                case SpriteType.HorizontalSpell:
                    Clear();
                    PlayRowBreakingEffect(this._rowId);
                    break;

                case SpriteType.VerticalSpell:
                    Clear();
                    PlayColoumBreakingEffect(this._columnId);
                    break;

                case SpriteType.Star:
                    TargetController.Instance.UpdateTarget(transform, SpriteType.Star);
                    blockImageLayer1.enabled = false;
                    GamePlay.Instance.blockers.Remove(this);
                    Clear();
                    break;

                case SpriteType.RedGiftBox:
                    if (GamePlay.Instance.completedLineSpriteTag == "b6" || isHitByKite || isHitByBoxingGlove || isHittedByBalloonBomb
                        || Magnet.Instance.isRowCompletedByPlacingMagnet)
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        this.SetBlockSpriteType(SpriteType.Empty);
                        TargetController.Instance.UpdateTarget(transform, defaultSpritetype);
                        Clear();
                    }
                    break;

                case SpriteType.PinkGiftBox:
                    if (GamePlay.Instance.completedLineSpriteTag == "b4" || isHitByKite || isHitByBoxingGlove || isHittedByBalloonBomb
                        || Magnet.Instance.isRowCompletedByPlacingMagnet)
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        this.SetBlockSpriteType(SpriteType.Empty);
                        TargetController.Instance.UpdateTarget(transform, defaultSpritetype);
                        Clear();
                    }
                    break;

                case SpriteType.BlueGiftBox:
                    if (GamePlay.Instance.completedLineSpriteTag == "b1" || isHitByKite || isHitByBoxingGlove || isHittedByBalloonBomb
                        || Magnet.Instance.isRowCompletedByPlacingMagnet)
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        this.SetBlockSpriteType(SpriteType.Empty);
                        TargetController.Instance.UpdateTarget(transform, defaultSpritetype);
                        Clear();
                    }
                    break;
                
                case SpriteType.GreenGiftBox:
                    if (GamePlay.Instance.completedLineSpriteTag == "b3" || isHitByKite || isHitByBoxingGlove || isHittedByBalloonBomb
                        || Magnet.Instance.isRowCompletedByPlacingMagnet)
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        this.SetBlockSpriteType(SpriteType.Empty);
                        TargetController.Instance.UpdateTarget(transform, defaultSpritetype);
                        Clear();
                    }
                    break;
                
                case SpriteType.CyanGiftBox:
                    if (GamePlay.Instance.completedLineSpriteTag == "b2" || isHitByKite || isHitByBoxingGlove || isHittedByBalloonBomb
                        || Magnet.Instance.isRowCompletedByPlacingMagnet)
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        this.SetBlockSpriteType(SpriteType.Empty);
                        TargetController.Instance.UpdateTarget(transform, defaultSpritetype);
                        Clear();
                    }
                    break;
                
                case SpriteType.PurpleGiftBox:
                    if (GamePlay.Instance.completedLineSpriteTag == "b5" || isHitByKite || isHitByBoxingGlove || isHittedByBalloonBomb
                        || Magnet.Instance.isRowCompletedByPlacingMagnet)
                    {
                        GamePlay.Instance.blockers.Remove(this);
                        this.SetBlockSpriteType(SpriteType.Empty);
                        TargetController.Instance.UpdateTarget(transform, defaultSpritetype);
                        Clear();
                    }
                    break;
                
                case SpriteType.Ice:
                    if (secondarySpriteType == SpriteType.IceBomb)
                    {
                        thisIceBomb.iceSparkParticle.Play();
                    }
                    
                    if (hasStages)
                    {
                        if (stage > 1)
                        {
                            --stage;
                            PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                            blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice + "Stage" + stage);
                            assignedSpriteTag = spriteType + "Stage" + stage;
                        }
                        else
                        {
                            PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                            TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                            hasStages = false;
                            stage = 0;
                            blockImageLayer2.enabled = false;
                            if (secondarySpriteType is SpriteType.CloseShell or SpriteType.OpenShell)
                            {
                                if (!GamePlay.Instance.pearlAndShellController.activeSelf)
                                {
                                    GamePlay.Instance.pearlAndShellController.SetActive(true);
                                }  
                                
                                if (!PearlAndShellController.Instance.shells.Contains(this))
                                {
                                    PearlAndShellController.Instance.shells.Add(this);
                                    justBorn = true;
                                }
                            }

                            DecodeSpriteType(secondarySpriteType);
                            spriteType = secondarySpriteType;
                            secondarySpriteType = SpriteType.Empty;
                            assignedSpriteTag = spriteType.ToString();
                            GamePlay.Instance.blockers.Remove(this);
                        }
                    }
                    else
                    {
                        PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                        TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                        blockImageLayer2.enabled = false;
                        if (secondarySpriteType is SpriteType.CloseShell or SpriteType.OpenShell)
                        {
                            if (!GamePlay.Instance.pearlAndShellController.activeSelf)
                            {
                                GamePlay.Instance.pearlAndShellController.SetActive(true);
                            }

                            if (!PearlAndShellController.Instance.shells.Contains(this))
                            {
                                PearlAndShellController.Instance.shells.Add(this);
                                justBorn = true;
                            }
                        }
                        spriteType = secondarySpriteType;
                        secondarySpriteType = SpriteType.Empty;
                        GamePlay.Instance.blockers.Remove(this);
                    }
                    break;

                case SpriteType.StarWithIce:
                    if (hasStages)
                    {
                        if (stage > 1)
                        {
                            --stage;
                            PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                            blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice + "Stage" + stage);
                            assignedSpriteTag = spriteType + "Stage" + stage;
                        }
                        else
                        {
                            PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                            TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                            hasStages = false;
                            stage = 0;
                            blockImageLayer2.enabled = false;
                            spriteType = SpriteType.Star;
                            assignedSpriteTag = spriteType.ToString();
                        }
                    }
                    else
                    {
                        PlaySimpleBlockBreakEffect(blockImageLayer2.sprite);
                        TargetController.Instance.UpdateTarget(transform, SpriteType.Ice);
                        blockImageLayer2.enabled = false;
                        spriteType = SpriteType.Star;
                        assignedSpriteTag = spriteType.ToString();
                    }
                    break;
                    
                default:
                    if (hasStages)
                    {
                        if (stage > 1)
                        {
                            --stage;
                            Sprite sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType + "Stage" + stage);
                            blockImage.sprite = sprite;
                            defaultSprite = sprite;
                            assignedSpriteTag = spriteType + "Stage" + stage;
                            PlaySimpleBlockBreakEffect(blockImage.sprite);
                        }
                        else
                        {
                            TargetController.Instance.UpdateTarget(transform, spriteType);
                            hasStages = false;
                            stage = 0;
                            GamePlay.Instance.blockers.Remove(this);
                            Clear();
                        }
                    }
                    else
                    {
                        TargetController.Instance.UpdateTarget(transform, spriteType);
                        GamePlay.Instance.blockers.Remove(this);
                        Clear();
                    }
                    break;
            }
        }

        public GameObject GetBubbleGameObject()
        {
            return m_bubbleAnimation;
        }

        // gets called during board generation
        public void SetBlock(SpriteType spriteType, SpriteType secondarySpriteType, bool hasStages, int stage)
        {
            this.spriteType = spriteType;
            this.secondarySpriteType = secondarySpriteType;
            this.hasStages = hasStages;
            this.stage = stage;
            
            switch (spriteType)
            {
                case SpriteType.Bubble:
                    if (this.secondarySpriteType != SpriteType.Empty)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(secondarySpriteType.ToString()), spriteType.ToString());
                    }
                    blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
                    blockImageLayer2.enabled = true;
                    blockImageLayer2.color = blockImage.color.WithNewA(1);
                    GamePlay.Instance.blockers.Add(this);
                    PlayBubbleAnimation();
                    break;

                case SpriteType.BubbleWithIce:
                    if (this.secondarySpriteType != SpriteType.Empty)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(secondarySpriteType.ToString()), spriteType.ToString());
                    }
                    blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice + "Stage" + this.stage);
                    blockImageLayer2.enabled = true;
                    blockImageLayer2.color = blockImage.color.WithNewA(1);
                    GamePlay.Instance.blockers.Add(this);
                    PlayBubbleAnimation();
                    break;
                
                case SpriteType.MagnetWithBubble:
                    blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Magnet.ToString());
                    blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Bubble.ToString());
                    blockImageLayer1.color = blockImage.color.WithNewA(1);
                    blockImageLayer2.color = blockImage.color.WithNewA(1);
                    blockImageLayer1.enabled = true;
                    blockImageLayer2.enabled = true;
                    PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Yellow.ToString()), spriteType.ToString());
                    PlayBubbleAnimation();
                    break;

                case SpriteType.Ice:
                    if (this.secondarySpriteType != SpriteType.Empty)
                    {
                        if (secondarySpriteType is SpriteType.MusicalNode or SpriteType.Magnet)
                        {
                            blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(secondarySpriteType.ToString());
                            blockImageLayer1.color = blockImage.color.WithNewA(1);
                            blockImageLayer1.enabled = true;
                            assignedSpriteTag = spriteType.ToString();
                        }
                        else if (this.secondarySpriteType == SpriteType.Hat)
                        {
                            PlaceMagicHat();
                        }
                        else if (this.secondarySpriteType == SpriteType.HatWithIce)
                        {
                            this.secondarySpriteType = SpriteType.Hat;
                        }
                        else if (this.secondarySpriteType != SpriteType.BalloonBomb && this.secondarySpriteType != SpriteType.IceBomb)
                        {
                            PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(secondarySpriteType.ToString()), spriteType.ToString());
                        }

                        if (secondarySpriteType == SpriteType.IceBomb)
                        {
                            if(thisIceBomb != null)
                                thisIceBomb.iceSparkParticle.Pause();
                        }
                    }
                    if (hasStages)
                    {
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString() + "Stage" + this.stage);
                        blockImageLayer2.color = blockImage.color.WithNewA(1);
                        blockImageLayer2.enabled = true;
                    }
                    else
                    {
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString());
                        blockImageLayer2.color = blockImage.color.WithNewA(1);
                        blockImageLayer2.enabled = true;
                    }
                    
                    if (!GamePlay.Instance.blockers.Contains(this))
                    {
                        GamePlay.Instance.blockers.Add(this);   
                    }
                    break;
                
                case SpriteType.Diamond:
                    diamond = Instantiate(GamePlay.Instance.diamondPrefab, GamePlay.Instance.instantiatedGameObjectsParent);
                    diamond.Initialize(this);
                    GamePlay.Instance.instantiatedGameObjects.Add(diamond.gameObject);
                    break;

                case SpriteType.MagnetWithIce:
                    if (secondarySpriteType != SpriteType.Empty)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(this.secondarySpriteType.ToString()), spriteType.ToString());
                    }
                    
                    if (hasStages)
                    {
                        blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Magnet.ToString());
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString() + "Stage" + this.stage);
                        blockImageLayer1.color = blockImage.color.WithNewA(1);
                        blockImageLayer2.color = blockImage.color.WithNewA(1);
                        blockImageLayer1.enabled = true;
                        blockImageLayer2.enabled = true;
                    }
                    else
                    {
                        blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Magnet.ToString());
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString());
                        blockImageLayer1.color = blockImage.color.WithNewA(1);
                        blockImageLayer2.color = blockImage.color.WithNewA(1);
                        blockImageLayer1.enabled = true;
                        blockImageLayer2.enabled = true;
                    }
                    break;

                case SpriteType.JewelMachine:
                    PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString()), spriteType.ToString());
                    isFilled = false;
                    
                    if(!GamePlay.Instance.jewelMachineEnabled)
                    {
                        JewelMachineController.Instance.enabled = true;
                    }
                    JewelMachineController.Instance.AddJewelMachineColoumId(_columnId);
                    break;

                case SpriteType.MilkShop:
                    MilkShop milkShop = Instantiate(GamePlay.Instance.milkShopPrefab, transform.position,
                                                            Quaternion.identity, GamePlay.Instance.instantiatedGameObjectsParent);
                    GamePlay.Instance.instantiatedGameObjects.Add(milkShop.gameObject);
                    // milkShop.transform.localScale = Vector3.one;
                    milkShop.gameObject.SetActive(true);
                    StartCoroutine(milkShop.MarkOccupiedBlocksUnAvail(this));
                    break;
                
                case SpriteType.Star:
                    blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Star.ToString());
                    blockImageLayer1.color = blockImage.color.WithNewA(1);
                    blockImageLayer1.enabled = true;
                    if (secondarySpriteType != SpriteType.Empty)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(secondarySpriteType.ToString()), spriteType.ToString());
                    }
                    GamePlay.Instance.blockers.Add(this);
                    break;
                
                case SpriteType.Magnet:
                    if (this.secondarySpriteType != SpriteType.Empty)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(secondarySpriteType.ToString()), spriteType.ToString());
                    }
                    blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
                    blockImageLayer1.color = blockImage.color.WithNewA(1);
                    blockImageLayer1.enabled = true;
                    PlaceBlock(spriteType);
                    break;
            
                case SpriteType.Kite:
                    blockImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
                    blockImage.color = blockImage.color.WithNewA(1);
                    blockImage.enabled = true;
                    PlaceBlock(spriteType);
                    break;
                
                case SpriteType.StarWithIce:
                    if (secondarySpriteType != SpriteType.Empty)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(this.secondarySpriteType.ToString()), spriteType.ToString());
                    }
                    
                    if (hasStages)
                    {
                        blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Star.ToString());
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString() + "Stage" + this.stage);
                        blockImageLayer1.color = blockImage.color.WithNewA(1);
                        blockImageLayer2.color = blockImage.color.WithNewA(1);
                        blockImageLayer1.enabled = true;
                        blockImageLayer2.enabled = true;
                    }
                    else
                    {
                        blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Star.ToString());
                        blockImageLayer2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString());
                        blockImageLayer1.color = blockImage.color.WithNewA(1);
                        blockImageLayer2.color = blockImage.color.WithNewA(1);
                        blockImageLayer1.enabled = true;
                        blockImageLayer2.enabled = true;
                    }
                    GamePlay.Instance.blockers.Add(this);
                    break;
                
                case SpriteType.MusicalPlayer:
                    PlaceMusicalPlayer();
                    GamePlay.Instance.blockers.Add(this);
                    break;

                case SpriteType.Hat:
                    PlaceMagicHat();
                    GamePlay.Instance.blockers.Add(this);
                    break;

                default:
                    if (hasStages)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString() + "Stage" + this.stage),
                            spriteType.ToString() + "Stage" + this.stage);
                    }
                    else
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString()), spriteType.ToString());
                    }
                    if (this.spriteType is SpriteType.OpenShell or SpriteType.Panda or SpriteType.Hat or SpriteType.MusicalPlayer or
                        SpriteType.IceMachine or SpriteType.BlueGiftBox or SpriteType.CyanGiftBox or SpriteType.GreenGiftBox or 
                        SpriteType.PinkGiftBox or SpriteType.PurpleGiftBox or SpriteType.RedGiftBox or SpriteType.YellowGiftBox)
                    {
                        GamePlay.Instance.blockers.Add(this);
                    }
                    break;
            }
        }

        private  void PlayBubbleAnimation()
        {
            // blockImageLayer2.enabled = false;
            m_bubbleAnimation.SetActive(true);
        }

        private void PlayBubbleBreakEffect()
        {
            m_bubbleAnimation.SetActive(false);
            //GameObject bubbleBreakEffect = Instantiate<ParticleSystem>(m_bubbleBreakEffect, transform).gameObject;
            //Destroy(bubbleBreakEffect, 1);
        }

        public void SetBlockSpriteType(SpriteType spriteType)
        {
            this.spriteType = spriteType;
        }


        public void PlaySimpleBlockBreakEffect(Sprite sprite)
        {
            ParticleSystem blockBreakParticle = GameObject.Instantiate(m_simpleBlockBreakEffect, transform.position,
                                                Quaternion.identity, GamePlay.Instance.instantiatedGameObjectsParent);
            blockBreakParticle.textureSheetAnimation.SetSprite(0, sprite);
            blockBreakParticle.Play();
            Destroy(blockBreakParticle.gameObject, 2);
        }


        /// <summary>
        /// This will be used to break PandaStage enums and store it's values accordingly to the variables.. 
        /// </summary>
        private void DecodeSpriteType(SpriteType spriteTypeToDecode)
        {
            switch (spriteTypeToDecode)
            {
                case SpriteType.PandaStage1:
                    secondarySpriteType = SpriteType.Panda;
                    GamePlay.Instance.blockers.Add(this);
                    break;
                
                case SpriteType.PandaStage2:
                    secondarySpriteType = SpriteType.Panda;
                    hasStages = true;
                    stage = 2;
                    GamePlay.Instance.blockers.Add(this);
                    break;
                
                case SpriteType.PandaStage3:
                    secondarySpriteType = SpriteType.Panda;
                    hasStages = true;
                    stage = 3;
                    GamePlay.Instance.blockers.Add(this);
                    break;
                
                case SpriteType.PandaStage4:
                    secondarySpriteType = SpriteType.Panda;
                    hasStages = true;
                    stage = 4;
                    GamePlay.Instance.blockers.Add(this);
                    break;
                
                case SpriteType.PandaStage5:
                    secondarySpriteType = SpriteType.Panda;
                    hasStages = true;
                    stage = 5;
                    GamePlay.Instance.blockers.Add(this);
                    break;
            }
        }
        

        #region Electric Spell
        private void PlayColoumBreakingEffect(int coloumId)
        {
            List<Block> blocks = GamePlay.Instance.GetEntirColumn(coloumId);
            InstantiateLightning(blocks[0], blocks[blocks.Count - 1]);
            GamePlay.Instance.ClearColoum(coloumId);
        }

        private void PlayRowBreakingEffect(int rowId)
        {
            List<Block> blocks = GamePlay.Instance.GetEntireRow(rowId);
            InstantiateLightning(blocks[0], blocks[blocks.Count - 1]);
            GamePlay.Instance.ClearRow(rowId);
        }

        private void InstantiateLightning(Block startPosition, Block endPosition)
        {
            GameObject lightningBolt = (GameObject)Instantiate(GamePlay.Instance.lightningBolt) as GameObject;
            lightningBolt.transform.position = this.transform.position;
            lightningBolt.transform.SetParent(GamePlay.Instance.boardGenerator.transform);
            //LightningBoltScript lightning = Instantiate<LightningBoltScript>();
            lightningBolt.gameObject.GetComponent<LightningBoltScript>().StartObject = startPosition.gameObject;
            //lightning.StartObject = startPosition.gameObject;
            lightningBolt.gameObject.GetComponent<LightningBoltScript>().EndObject = endPosition.gameObject;
            lightningBolt.SetActive(true);

            Destroy(lightningBolt.gameObject, 0.5f);
        }
        #endregion
    }
}
