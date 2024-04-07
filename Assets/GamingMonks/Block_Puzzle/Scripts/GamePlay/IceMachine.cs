using System;
using UnityEngine;
using TMPro;
using GamingMonks.UITween;
using System.Threading.Tasks;

namespace GamingMonks
{
    public class IceMachine : MonoBehaviour
    {
#pragma warning disable 0649
        // Text of remaining coundown on bomb.
        [SerializeField] TextMeshProUGUI txtCounter;
#pragma warning restore 0649

        // Remaining coundown amount on bomb.
        [System.NonSerialized] public int remainingCounter;
        [System.NonSerialized] private int currentCounter;
        [SerializeField] private ParticleSystem trailEffect;
        public bool canClearIce;
        public Block block = null;
        [SerializeField] private ParticleSystem splashEffect;
        
        private void Awake()
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            rectTransform.sizeDelta = new Vector2(size-10, size-10);
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
            currentCounter = remainingCounter;
            txtCounter.text = remainCounter.ToString();
        }

        /// <summary>
        /// Counter will keep reduding upon each block shape placed and will lead to game over or recue state on reaching to zero.
        /// </summary>
        private void GamePlayUI_OnShapePlacedEvent()
        {
            if(block.justBorn)
                return;
            
            currentCounter -= 1;
            txtCounter.transform.LocalScale(Vector3.zero, 0.15F).OnComplete(() =>
            {
                txtCounter.text = currentCounter.ToString();
                txtCounter.transform.LocalScale(Vector3.one, 0.15F).OnComplete(() =>
                {
                    if (block.isAvailable == false && currentCounter <= 0 && block.spriteType == SpriteType.IceMachine)
                    {
                        txtCounter.text = currentCounter.ToString();
                        trailEffect.transform.position = transform.position;
                        trailEffect.gameObject.SetActive(true);
                        trailEffect.Play();
                        Move();
                        currentCounter = remainingCounter;
                        txtCounter.text = currentCounter.ToString();
                    }
                });
            });
        }

        private async void Move()
        {
            if (GamePlayUI.Instance.currentLevel.ConveyorBelts.enabled || GamePlayUI.Instance.currentLevel.JewelMachine.enabled)
            {
                await Task.Delay(System.TimeSpan.FromSeconds(0.6f));
            }
            else
            {
                await Task.Delay(System.TimeSpan.FromSeconds(0.1f));
            }
            
            float time = 0;
            Block b = GamePlay.Instance.GetFilledBlockForIceMachine();
            

            while (time < 0.35f)
            {
                await Task.Delay(System.TimeSpan.FromSeconds(0.01f));
                time += Time.deltaTime;
                trailEffect.transform.position = Vector3.Lerp(trailEffect.transform.position, b.gameObject.transform.position, time);
            }
            trailEffect.Stop();
            splashEffect.transform.position = b.transform.position;
            splashEffect.Play();
            
            int stage = 0;
            if (b.spriteType != SpriteType.Panda)
                stage = b.stage;
            
            stage += 1;
            if (stage >= 4)
            {
                return;
            }

            if (b.spriteType == SpriteType.Panda)
            {
                b.secondarySpriteType = (SpriteType) Enum.Parse(typeof(SpriteType), b.spriteType + "Stage" + b.stage);
                b.spriteType = SpriteType.Ice;
            }
            else if (b.spriteType == SpriteType.Star)
            {
                b.spriteType = SpriteType.StarWithIce;
            }
            else if (b.spriteType is SpriteType.StarWithIce or SpriteType.Ice or SpriteType.BubbleWithIce)
            {
                if (!b.hasStages)
                {
                    stage += 1;
                }
            }
            else if (b.spriteType is SpriteType.Bubble)
            {
                b.spriteType = SpriteType.BubbleWithIce;
            }
            else if (b.spriteType is SpriteType.Hat)
            {
                b.secondarySpriteType = SpriteType.HatWithIce;
                b.spriteType = SpriteType.Ice;
            }
            else if (b.spriteType != SpriteType.Ice)
            {
                b.secondarySpriteType = b.spriteType;
                b.spriteType = SpriteType.Ice;
            }
        
            b.SetBlock(b.spriteType, b.secondarySpriteType ,true, stage);
        }
    }
}
