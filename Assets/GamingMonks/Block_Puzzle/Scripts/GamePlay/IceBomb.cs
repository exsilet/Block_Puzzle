using System;
using GamingMonks.UITween;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GamingMonks
{
    public class IceBomb : MonoBehaviour
    {
#pragma warning disable 0649
        // Text of remaining coundown on bomb.
        [SerializeField] TextMeshProUGUI txtCounter;
#pragma warning restore 0649

        // Remaining countdown amount on bomb.
        [System.NonSerialized] public int remainingCounter;
        public Block block = null;

        [SerializeField] private ParticleSystem m_iceBombBreakEffect;
        public ParticleSystem iceSparkParticle;

        private void Awake()
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            rectTransform.sizeDelta = new Vector2(size, size);
        }
        
        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            /// Registers game status callbacks.
            GamePlayUI.OnShapePlacedEvent += GamePlayUI_OnShapePlacedEvent;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            /// Uregisters game status callbacks.
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
            if (block.spriteType != SpriteType.IceBomb)
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
                            SpreadIce();
                            block.isIceBomb = false;
                            block.thisIceBomb = null;
                            Destroy(gameObject, 1);
                        });
                    }
                });
            });

        }

        private async void SpreadIce()
        {
            if (GamePlayUI.Instance.currentLevel.ConveyorBelts.enabled || GamePlayUI.Instance.currentLevel.JewelMachine.enabled)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.3f));
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
            }
            
            PlayIceBombBlastEffect();
            
            // It will spread on it's own block..
            block.SetBlock(SpriteType.Ice, SpriteType.Pink, false, 0);
            
            List<Block> iceBlocks = GetDestroyableBlocks();
            foreach (Block iceBlock in iceBlocks)
            {
                if(iceBlock.spriteType == SpriteType.IceMachine)
                    continue;
                
                int stage = 0;
                if(iceBlock.spriteType != SpriteType.Panda)
                    stage = iceBlock.stage;
                
                stage += 1;
                if (stage >= 4)
                {
                    continue;
                }

                if (iceBlock.spriteType == SpriteType.Panda)
                {
                    iceBlock.secondarySpriteType = (SpriteType) Enum.Parse(typeof(SpriteType), iceBlock.spriteType + 
                                                                            "Stage" + iceBlock.stage);
                    iceBlock.spriteType = SpriteType.Ice;
                }
                else if (iceBlock.spriteType == SpriteType.Star)
                {
                    iceBlock.spriteType = SpriteType.StarWithIce;
                }
                else if (iceBlock.spriteType is SpriteType.StarWithIce or SpriteType.Ice or SpriteType.BubbleWithIce)
                {
                    if (!iceBlock.hasStages)
                    {
                        stage += 1;
                    }
                }
                else if (iceBlock.spriteType is SpriteType.Bubble)
                {
                    iceBlock.spriteType = SpriteType.BubbleWithIce;
                }
                else if (iceBlock.spriteType is SpriteType.Hat)
                {
                    iceBlock.secondarySpriteType = SpriteType.HatWithIce;
                    iceBlock.spriteType = SpriteType.Ice;
                }
                else if (iceBlock.spriteType != SpriteType.Ice)
                {
                    iceBlock.secondarySpriteType = iceBlock.spriteType;
                    iceBlock.spriteType = SpriteType.Ice;
                }
            
                iceBlock.SetBlock(iceBlock.spriteType, iceBlock.secondarySpriteType ,true, stage);
            }
        }
        
        private List<Block> GetDestroyableBlocks()
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();
            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;

            List<Block> destroyByBomb = new List<Block>();


            for (int i = block.ColumnId - 1; i <= block.ColumnId + 1; i++)
            {
                if (i >= columnSize || i < 0)
                {
                    continue;
                }

                if (block.RowId > 0)
                {
                    List<Block> upperRow = GamePlay.Instance.allRows[block.RowId - 1];
                    if (upperRow[i].isFilled && !upperRow[i].hasMilkShop)
                        destroyByBomb.Add(upperRow[i]);
                }

                if (i != block.ColumnId)
                {
                    List<Block> thisRow = GamePlay.Instance.allRows[block.RowId];
                    if (thisRow[i].isFilled && !thisRow[i].hasMilkShop)
                        destroyByBomb.Add(thisRow[i]);
                }
                
                
                if (block.RowId < rowSize - 1)
                {
                    List<Block> lowerRow = GamePlay.Instance.allRows[block.RowId + 1];
                    if (lowerRow[i].isFilled && !lowerRow[i].hasMilkShop)
                        destroyByBomb.Add(lowerRow[i]);
                }

            }
            return destroyByBomb;
        }

        public void PlayIceBombBlastEffect()
        {
            ParticleSystem effect = Instantiate(m_iceBombBreakEffect, transform.position,
                                    Quaternion.identity, GamePlay.Instance.instantiatedGameObjectsParent);
            Destroy(effect.gameObject, 2);
        }
    }
}