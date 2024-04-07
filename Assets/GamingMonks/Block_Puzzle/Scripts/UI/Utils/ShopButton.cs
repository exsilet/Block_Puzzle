using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	[RequireComponent(typeof(Button))]
	public class ShopButton : MonoBehaviour 
	{
		Button btnShop;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		private void Awake() {
			btnShop = GetComponent<Button>();	
		}

		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		private void OnEnable() {
			btnShop.onClick.AddListener(OnShopButtonClicked);
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() {
			btnShop.onClick.RemoveListener(OnShopButtonClicked);
		}

		void OnShopButtonClicked() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				UIController.Instance.shopScreen.gameObject.Activate();
			}
		}
	}
}