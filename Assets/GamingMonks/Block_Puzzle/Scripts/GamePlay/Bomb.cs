using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GamingMonks.UITween;

namespace GamingMonks
{
    /// <summary>
    /// The script component attached to Bomb object. Will be used during blast mode only.
    /// </summary>
	public class Bomb : MonoBehaviour
    {
        #pragma warning disable 0649
        // Text of remaining coundown on bomb.
        [SerializeField] Text txtCounter;
        
        //Particle Ring
        [SerializeField] GameObject ringParticle;
        #pragma warning restore 0649

        // Remaining coundown amount on bomb.
        [System.NonSerialized] public int remainingCounter = 9;

        private void Awake()
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            rectTransform.sizeDelta = new Vector2(size, size);
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable() {
            /// Registers game status callbacks.
            GamePlayUI.OnShapePlacedEvent += GamePlayUI_OnShapePlacedEvent;  
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable() {
            /// Uregisters game status callbacks.
            GamePlayUI.OnShapePlacedEvent -= GamePlayUI_OnShapePlacedEvent;
        }

        /// <summary>
        /// Sets the given counter on bomb.
        /// </summary>
        public void SetCounter(int remainCounter) {
            remainingCounter = remainCounter;
            txtCounter.text = remainCounter.ToString();
        }

        /// <summary>
        /// Counter will keep reduding upon each block shape placed and will lead to game over or recue state on reaching to zero.
        /// </summary>
        private async void GamePlayUI_OnShapePlacedEvent() {
            await Task.Delay(System.TimeSpan.FromSeconds(0.2f));
            remainingCounter -= 1;
            txtCounter.transform.LocalScale(Vector3.zero, 0.15F).OnComplete(() =>
            {
                txtCounter.text = remainingCounter.ToString();
                ringParticle.SetActive(true);
                txtCounter.transform.LocalScale(Vector3.one, 0.15F).OnComplete(() => {
                    if (remainingCounter <= 0) {
                        GamePlayUI.Instance.TryRescueGame(GameOverReason.BOMB_BLAST);
                    }
                });
            });
        }
    }
}

