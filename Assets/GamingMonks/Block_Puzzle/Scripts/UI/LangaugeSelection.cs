using GamingMonks.Localization;
using UnityEngine;

namespace GamingMonks
{
	/// <summary>
	/// This script is attached to language selection popup and sets app language as user selected language. This script also prepares screen and
	/// </summary>
	public class LangaugeSelection : MonoBehaviour {
		
		#pragma warning disable 0649
		[SerializeField] GameObject languageButtonTemplate;
		[SerializeField] GameObject languageListContent;
		#pragma warning restore 0649

		/// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
		private void Start() {
			PrepareLanguageSelectionScreen();
		}

		/// <summary>
		/// Create selection button for each available language.
		/// </summary>
		void PrepareLanguageSelectionScreen() 
		{
			LocalizedLanguage currentLanguage = LocalizationManager.Instance.GetCurrentLanguage();
			
			if(currentLanguage != null) {
				foreach(LocalizedLanguage lang in LocalizationManager.Instance.allLocalizedLanaguages) {
					if(lang.isLanguageEnabled) {
						GetLanguageButton(lang, lang.languageCode.Equals(currentLanguage.languageCode));
					}
				}
			}
		}


		/// <summary>
		/// Instantiates a button from templete.
		/// </summary>
		/// <returns></returns>
		GameObject GetLanguageButton(LocalizedLanguage lang, bool isActive = false) {
			GameObject langButton = (GameObject) Instantiate (languageButtonTemplate);
			langButton.transform.SetParent(languageListContent.transform);
			langButton.name = lang.languageName;
			langButton.transform.localScale = Vector3.one;
			langButton.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			langButton.transform.SetAsLastSibling(); 
			langButton.GetComponent<LanguageButton>().SetLanaguage(lang, isActive);
			langButton.SetActive(true);
			return langButton;
		}	

		// Close button listener.
		public void OnCloseButtonPressed() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				gameObject.Deactivate();
			}
		}
	}
}
