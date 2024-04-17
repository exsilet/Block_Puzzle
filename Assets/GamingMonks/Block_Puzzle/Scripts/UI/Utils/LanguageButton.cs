using GamingMonks.Localization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GamingMonks
{
    /// <summary>
    /// This script is attached to each langauge selection button. This script will change app language to
    /// selected on pressing buhtton.!--
    /// </summary>
    public class LanguageButton : MonoBehaviour
    {
        /// Instance of current button's localize language.
        LocalizedLanguage currentButtonLanaguage;

        #pragma warning disable 0649
		// Name of langauge.
        [SerializeField] TextMeshProUGUI txtLangaugeName;

		// Check mark enabled if current language is this.
        [SerializeField] Image imgCheckMark;

		//// Line below localization button.
  //      [SerializeField] Image imgLine;
        #pragma warning restore 0649


        // Language is active?
        bool isActiveLangauge = false;

        /// <summary>
        ///  Initializes language button and restores its state.
        /// </summary>
        ///
        private string[] langRu = { "Английский", "Испанский", "Русский", "Турецкий" };
        private string[] langEn = { "English", "Español", "Russian", "Turkish" };
        private string[] langEs = { "Ingles", "Española", "Rusa", "Turca" };
        private string[] langTr = { "İngilizce", "İspanyolca", "Rusça", "Türkçe" };

        public void SetLanaguage(LocalizedLanguage lang, bool isActive = false)
        {
            currentButtonLanaguage = lang;
            txtLangaugeName.text = currentButtonLanaguage.langaugeDisplayName;
            Debug.Log(txtLangaugeName.text);
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "EN")
            {
                txtLangaugeName.text = langEn[Array.IndexOf(langEn, txtLangaugeName.text)];
            }
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "ES")
            {
                Debug.Log(Array.IndexOf(langEn, txtLangaugeName.text));
                txtLangaugeName.text = langEs[Array.IndexOf(langEn, txtLangaugeName.text)];
            }
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "RU")
            {
                txtLangaugeName.text = langRu[Array.IndexOf(langEn, txtLangaugeName.text)];
            }
            if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "TR")
            {
                txtLangaugeName.text = langTr[Array.IndexOf(langEn, txtLangaugeName.text)];
            }
            
            isActiveLangauge = isActive;
            imgCheckMark.enabled = isActiveLangauge;
        }

        /// <summary>
        /// Language select button listener.
        /// </summary>
        public void OnLanagueButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();

                if (!isActiveLangauge)
                {
                    LocalizationManager.Instance.SetLocalizedLanguage(currentButtonLanaguage);
                }
            }
            //if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "EN")
            //{  
            //    txtLangaugeName.text = langEn[Array.IndexOf(langEn, txtLangaugeName.text)];
            //}
            //if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "ES")
            //{
            //    txtLangaugeName.text = langEs[Array.IndexOf(langEs, txtLangaugeName.text)];
            //}
            //if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "RU")
            //{
            //    txtLangaugeName.text = langRu[Array.IndexOf(langRu, txtLangaugeName.text)];
            //}
            //if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "TR")
            //{
            //    txtLangaugeName.text = langTr[Array.IndexOf(langTr, txtLangaugeName.text)];
            //}
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            LocalizationManager.OnLanguageChangedEvent += OnLanguageChanged;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            LocalizationManager.OnLanguageChangedEvent -= OnLanguageChanged;
        }

        /// <summary>
        /// App langauge will be changed and language change callback will be invoked if current language is different then selected.
        /// </summary>
        void OnLanguageChanged(LocalizedLanguage lang)
        {
            if (currentButtonLanaguage.languageCode.Equals(lang.languageCode))
            {
                if (!isActiveLangauge)
                {
                    imgCheckMark.enabled = true;
                    isActiveLangauge = true;
                }
            }
            else
            {
                if (isActiveLangauge)
                {
                    imgCheckMark.enabled = false;
                }
                isActiveLangauge = false;
            }
        }
    }
}
