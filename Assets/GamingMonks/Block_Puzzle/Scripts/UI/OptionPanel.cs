using UnityEngine;

namespace GamingMonks
{
	/// <summary>
	/// Varies option button listner attached to this on home screen.
	/// </summary>
    public class OptionPanel : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] GameObject themeSettingButton;
        #pragma warning restore 0649

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            if(!ThemeManager.Instance.UIThemeEnabled) {
                themeSettingButton.SetActive(false);
            }
        }

		/// <summary>
		/// Opens setting screen.
		/// </summary>
        public void OnSettingsButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.settingScreen.Activate();
            }
        }

		/// <summary>
		/// Opens shop popup.
		/// </summary>
        public void OnShopButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.shopScreen.gameObject.Activate();
            }
        }

		/// <summary>
		/// Opens language selection popup.
		/// </summary>
        public void OnSelectLangaugeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.lanagueSelectionScreen.Activate();
            }
        }

		/// <summary>
		/// Opens review popup or store review nag.
		/// </summary>
        public void OnRateButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.reviewScreen.Activate();
            }
        }	

		/// <summary>
		/// Open theme selection popup.
		/// </summary>
        public void OnThemeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.selectThemeScreen.Activate();
            }
        }
    }
}

