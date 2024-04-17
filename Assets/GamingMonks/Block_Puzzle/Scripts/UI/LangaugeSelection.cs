using System;
using System.Collections.Generic;
using GamingMonks.Localization;
using TMPro;
using Unity.VisualScripting;
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


        private string[] langRu = { "Английский", "Испанский", "Русский", "Турецкий" };
        private string[] langEn = { "English", "Español", "Russian", "Turkish" };
        private string[] langEs = { "Ingles", "Española", "Rusa", "Turca" };
        private string[] langTr = { "İngilizce", "İspanyolca", "Rusça", "Türkçe" };
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
		///
		//private Transform[] languages;
        public List<GameObject> TList = new List<GameObject>();

        private void Start() {
			PrepareLanguageSelectionScreen();

            for (int i = 0; i < languageListContent.transform.childCount; i++)
                TList.Add(languageListContent.transform.GetChild(i).gameObject);

            //Debug.Log(TList.Count);
        }

        private void Update()
        {
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "EN")
            {
                for (int i = 0; i < 4; i++)
                {
                    TList[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = langEn[i];
                }

            }
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "ES")
            {
                for (int i = 0; i < 4; i++)
                {
                    TList[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = langEs[i];
                }
            }
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "RU")
            {
                for (int i = 0; i < 4; i++)
                {
                    TList[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = langRu[i];
                }
            }
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "TR")
            {
                for (int i = 0; i < 4; i++)
                {
                    TList[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = langTr[i];
                }
            }
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
