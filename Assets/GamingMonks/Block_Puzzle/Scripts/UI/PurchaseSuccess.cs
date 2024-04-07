using UnityEngine;

namespace GamingMonks 
{   
    /// <summary>
    /// This script is attached to purchase success popup.
    /// </summary>
    public class PurchaseSuccess : MonoBehaviour
    {
        public RectTransform rewardAnimPosition;

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable() {
            UIController.Instance.PlayAddCoinsAnimationAtPosition(Vector3.zero, UIController.Instance.ShopButtonCoinsIcon.position, 0.2F, null);
        }

        /// <summary>
        /// Close button click listener.
        /// </summary>
        public void OnCloseButtonPressed() {
			if(InputManager.Instance.canInput()) {
                UIFeedback.Instance.PlayButtonPressEffect();
				gameObject.Deactivate();
			}
		}

        /// <summary>
        /// Ok button click listener.
        /// </summary>
        public void OnOkButtonPressed() {
			if(InputManager.Instance.canInput()) {
                UIFeedback.Instance.PlayButtonPressEffect();
				gameObject.Deactivate();
			}
		}
    }
}
