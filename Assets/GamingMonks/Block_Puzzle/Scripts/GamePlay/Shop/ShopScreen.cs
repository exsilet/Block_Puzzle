using GamingMonks.Ads;
using GamingMonks.Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GamingMonks
{
	public class ShopScreen : MonoBehaviour 
	{
		#pragma warning disable 0649
		//[SerializeField] RectTransform mainContentRect;
		[SerializeField] GameObject btnRemoveAds;
		//[SerializeField] CanvasGroup btnWatchVideo;
		//[SerializeField] Text txtWatchVideoReward;
		[SerializeField] ScrollRect scrollRect;
		[SerializeField] RectTransform contentPanel;
		[SerializeField] RectTransform firstSpecialPack;
		[SerializeField] RectTransform firstCoinPack;
		[SerializeField] RectTransform firstThemePack;

#pragma warning restore 0649

        //Vector2 currentContentSize;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
			//currentContentSize = mainContentRect.sizeDelta;
		}
		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		private void OnEnable() 
		{
			//UIController.Instance.EnableCurrencyBalanceButton();
			contentPanel.anchoredPosition = Vector2.zero;
            GamePlayUI.Instance.PauseGame();
            IAPManagers.OnPurchaseSuccessfulEvent += OnPurchaseSuccessful;         
            UpdateShopScreen();
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() {
			// Don't hide gems button if rescue screen is open.
            UIController.Instance.Invoke("DisableCurrencyBalanceButton",0.1F);
            IAPManagers.OnPurchaseSuccessfulEvent -= OnPurchaseSuccessful;
            GamePlayUI.Instance.ResumeGame();
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
		/// Purchase button click listener.
		/// </summary>
		public void OnPurhcaseButtonClicked() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				//gameObject.Deactivate();
			}
		}

		/// <summary>
		/// Purchase button click listener.
		/// </summary>
		public void OnGetFreeGemsButtonPressed() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				//AdManager.Instance.ShowRewardedWithTag("FreeGems");
			}
		}

		/// <summary>
        /// Purchase Rewards will be processed from here. You can adjust your code based on your requirements.
        /// </summary>
        /// <param name="productInfo"></param>
		/// 
		/*
		void OnPurchaseSuccessful(ProductInfo productInfo) {
			Invoke("UpdateShopScreen", 0.2F);
		}
		*/
		void OnPurchaseSuccessful(Product product)
		{
			Invoke("UpdateShopScreen", 0.2F);
		}

	void UpdateShopScreen() 
		{
			//txtWatchVideoReward.text = string.Format(LocalizationManager.Instance.GetTextWithTag("txtGems_FR"), ProfileManager.Instance.GetAppSettings().watchVideoRewardAmount);
			//if(!AdManager.Instance.IsRewardedAvailable()) {
			//	btnWatchVideo.alpha = 0.5F;
			//	btnWatchVideo.interactable = false;
			//} else {
			//	btnWatchVideo.alpha = 1.0F;
			//	btnWatchVideo.interactable = true;
			//}

 			if (ProfileManager.Instance.IsAppAdFree()) {
				btnRemoveAds.SetActive(false);
				//mainContentRect.sizeDelta = new Vector2(currentContentSize.x, currentContentSize.y - btnRemoveAds.GetComponent<RectTransform>().sizeDelta.y);
			} else {
				btnRemoveAds.SetActive(true);
			}
		}
		void SnapTo(RectTransform target)
		{
			Canvas.ForceUpdateCanvases();
			contentPanel.anchoredPosition = new Vector2(0, scrollRect.transform.InverseTransformPoint(contentPanel.position).y) - new Vector2(0, scrollRect.transform.InverseTransformPoint(target.position).y+(target.sizeDelta.y/2));
            scrollRect.inertia = false;
            StartCoroutine(RemoveInertia());
        }

		IEnumerator RemoveInertia()
		{
			yield return new WaitForSeconds(0.5f);
            scrollRect.inertia = true;
        }


    public void OnSpecialPacksBtnPressed()
        {
			SnapTo(firstSpecialPack);
        }
    public void OnCoinsPacksBtnPressed()
    {
        SnapTo(firstCoinPack);
    }
    public void OnThemePackBtnPressed()
    {
        SnapTo(firstThemePack);
        }
    }

}
