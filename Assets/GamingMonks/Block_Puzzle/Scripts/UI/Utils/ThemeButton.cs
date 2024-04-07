using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GamingMonks.Utils;

namespace GamingMonks
{
	/// <summary>
	/// Theme button is attached to all theme selection buttons.
	/// </summary>
	public class ThemeButton : MonoBehaviour  
	{
        ThemeConfig currentSelectedTheme;

		[SerializeField] Text txtThemeName;
        [SerializeField] Image imgBg;
		[SerializeField] Image imgBlockSample;
        [SerializeField] Button btnSelect;
		[SerializeField] Button btnUnlock;
		[SerializeField] Button btnActive;
		[SerializeField] GameObject imgBorder;
		[SerializeField] RectTransform gemsIcon;
		[SerializeField] Text txtUnlockPrice;

        bool isActiveTheme = false;
		bool isUnlockedTheme = false;

		/// <summary>
		/// Visually prepares how this theme will look like.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="isActive"></param>
		public void SetTheme(ThemeConfig settings, bool unlockStatus, bool isActive = false) 
		{
            currentSelectedTheme = settings;
            imgBg.color = settings.uiTheme.colorTags.FirstOrDefault(o => o.tagName == "GPBG").tagColor;
            txtThemeName.text = settings.themeName;
            imgBlockSample.sprite = settings.demoSprite;
            txtThemeName.color = settings.uiTheme.colorTags.FirstOrDefault(o => o.tagName == "PopUpText").tagColor;
            
			btnSelect.GetComponent<Image>().color = settings.uiTheme.colorTags.FirstOrDefault(o => o.tagName == "PopUpButton").tagColor;
			btnUnlock.GetComponent<Image>().color = settings.uiTheme.colorTags.FirstOrDefault(o => o.tagName == "UnlockThemeButton").tagColor;
			btnActive.GetComponent<Image>().color = settings.uiTheme.colorTags.FirstOrDefault(o => o.tagName == "PopUpTitle").tagColor;

			txtUnlockPrice.text = settings.unlockCost.ToString();
			isUnlockedTheme = unlockStatus;
            isActiveTheme = isActive;
			imgBorder.SetActive(isActiveTheme);
            if (!unlockStatus) {
				imgBlockSample.color = imgBlockSample.color.WithNewA(0.1F);
                btnUnlock.gameObject.SetActive(true);
			} else {
                if (isActive) {
					btnActive.gameObject.SetActive(true);
				} else {
					btnSelect.gameObject.SetActive(true);
				}
			}
		}

		/// <summary>
		/// Theme selection button click listener.
		/// </summary>
		public void OnThemeButtonPressed() {
			if(InputManager.Instance.canInput()) {
				InputManager.Instance.DisableTouchForDelay();
				UIFeedback.Instance.PlayButtonPressEffect();

				if(PlayerPrefs.HasKey("ThemeUnlockStatus_"+currentSelectedTheme.themeName)) {
					ThemeManager.Instance.SetTheme(currentSelectedTheme);
				} else {
					UnlockTheme();
				}
			}
		}

		private void UnlockTheme() 
		{
			if(CurrencyManager.Instance.DeductCoins(currentSelectedTheme.unlockCost)) {
				UIController.Instance.PlayDeductCoinsAnimation(gemsIcon.position, 0.1F);
				PlayerPrefs.SetInt("ThemeUnlockStatus_"+currentSelectedTheme.themeName, 1);
				isUnlockedTheme = true;
				Invoke("UpdateUIAfterUnlock",1.2F);
			} else {
				UIController.Instance.shopScreen.gameObject.Activate();
			}
		}

		private void UpdateUIAfterUnlock() {
			btnUnlock.gameObject.SetActive(false);
			btnSelect.gameObject.SetActive(true);
            imgBlockSample.color = imgBlockSample.color.WithNewA(1F);
        }

		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		private void OnEnable() 
		{
			ThemeManager.OnThemeChangedEvent += OnThemeChanged;
			if(currentSelectedTheme != null)
			{
                if ((PlayerPrefs.GetInt("ThemeUnlockStatus_" + currentSelectedTheme.themeName, 0) == 1))
                {
                    UpdateUIAfterUnlock();
                }
            }
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() {
            ThemeManager.OnThemeChangedEvent -= OnThemeChanged;
        }

		/// <summary>
		/// Theme change event callback
		/// </summary>
		/// <param name="themeName"></param>
		void OnThemeChanged(string themeName) {
			Debug.Log(isUnlockedTheme);
			if(isUnlockedTheme) {
				if(currentSelectedTheme.themeName.Equals(themeName)) {
					if(!isActiveTheme) {
						imgBorder.SetActive(true);
						isActiveTheme = true;
						btnActive.gameObject.SetActive(true);
						btnSelect.gameObject.SetActive(false);
					}
				} else {
					if(isActiveTheme) {
						imgBorder.SetActive(false);
						btnActive.gameObject.SetActive(false);
						btnSelect.gameObject.SetActive(true);
                    }
					isActiveTheme = false;	
				}
			}
		}
	}
}
