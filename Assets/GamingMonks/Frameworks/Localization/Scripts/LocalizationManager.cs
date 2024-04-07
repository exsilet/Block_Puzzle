using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace GamingMonks.Localization
{
	/// <summary>
	/// This scriptable class handles the ingame localization and store configurations.
	/// </summary>
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        public LocalizationSettings localizationSettings;

        LocalizedLanguage currentLanaguage;

        [System.NonSerialized] public List<LocalizedLanguage> allLocalizedLanaguages;
        [System.NonSerialized] public bool hasInitialised = false;
        [System.NonSerialized] public bool isLocalizationSupported = false;
        [System.NonSerialized] public bool hasLanguageChanged = false;

        public static event Action<LocalizedLanguage, bool> OnLocalizationInitializedEvent;
        public static event Action<LocalizedLanguage> OnLanguageChangedEvent;

        Dictionary<string, string> localizedTags = new Dictionary<string, string>();

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Invoke("Initialise", 0.1F);
        }

		/// <summary>
		/// Initialize and invokes required callback for the localization.
		/// </summary>
        void Initialise()
        {
            if (!hasInitialised)
            {
                if (localizationSettings == null)
                {
					// Initalize the localization settings scriptable and loads its from resources.
                    localizationSettings = (LocalizationSettings)Resources.Load("LocalizationSettings");
                }

				// All only properly configured languages to active language list.
                if (localizationSettings != null && localizationSettings.useLocalization)
                {
                    allLocalizedLanaguages = new List<LocalizedLanguage>();

                    foreach (LocalizedLanguage lang in localizationSettings.localizedLanguages)
                    {
                        if (lang.isLanguageEnabled && lang.languageCode != "" && lang.localizedTextFile != null)
                        {
                            allLocalizedLanaguages.Add(lang);
                        }
                    }

					// If the there is more than 0 langauge means localization is active to be continu to localize ingame content.
                    if (allLocalizedLanaguages.Count > 0)
                    {
                        isLocalizationSupported = true;
                    }

                    if (isLocalizationSupported)
                    {
                        currentLanaguage = GetCurrentLanguage();
                    }
                }
            }

            if (currentLanaguage == null)
            {
                currentLanaguage = new LocalizedLanguage();
                currentLanaguage.languageCode = "EN";
            }

            if (OnLocalizationInitializedEvent != null)
            {
                OnLocalizationInitializedEvent.Invoke(currentLanaguage, isLocalizationSupported);
            }

            hasInitialised = true;
        }

		/// <summary>
		/// Returns current localized language configuration.
		/// </summary>
        public LocalizedLanguage GetCurrentLanguage()
        {
            if (!isLocalizationSupported)
            {
                return null;
            }

            if (currentLanaguage != null)
            {
                return currentLanaguage;
            }

            if (!PlayerPrefs.HasKey("currentLang"))
            {
                if (localizationSettings.localizeToSystemDetectedLanguage)
                {
                    LocalizedLanguage autoDetectLang = allLocalizedLanaguages.Find(o => o.languageName.Contains(Application.systemLanguage.ToString()));
                    if (autoDetectLang != null)
                    {
                        SetLocalizedLanguageOnAutoDetect(autoDetectLang);
                        return autoDetectLang;
                    }
                }
            }

            LocalizedLanguage defaultLang = null;
            if (allLocalizedLanaguages.Count >= localizationSettings.defaultLangauge)
            {
                defaultLang = allLocalizedLanaguages[localizationSettings.defaultLangauge];
            }
            string languageCode = PlayerPrefs.GetString("currentLang", defaultLang.languageCode);
            LocalizedLanguage lang = allLocalizedLanaguages.Find(o => o.languageCode.Equals(languageCode));
            SetLocalizedLanguageOnAutoDetect(lang);
            return lang;
        }

		/// <summary>
		/// Set detected system language as in-game language.
		/// </summary>
        void SetLocalizedLanguageOnAutoDetect(LocalizedLanguage lang)
        {
            if (lang != null)
            {
                if (lang.languageCode != "EN")
                {
                    hasLanguageChanged = true;
                }
                currentLanaguage = lang;
                InitLocalizedContent();
                PlayerPrefs.SetString("currentLang", lang.languageCode);
            }
        }

		/// <summary>
		/// Sets given language as active in-game language.
		/// </summary>
        public void SetLocalizedLanguage(LocalizedLanguage lang)
        {
            if (lang != null)
            {
                currentLanaguage = lang;
                InitLocalizedContent();
                PlayerPrefs.SetString("currentLang", lang.languageCode);
                hasLanguageChanged = true;

                if (OnLanguageChangedEvent != null)
                {
                    OnLanguageChangedEvent.Invoke(lang);
                }
            }
        }

		/// <summary>
		/// Initializes and saves all the localized tag for the active language.
		/// </summary>
        void InitLocalizedContent()
        {
            XDocument xDocLocalizedContent = XDocument.Parse(currentLanaguage.localizedTextFile.ToString());
            localizedTags = new Dictionary<string, string>();

            var allElements = xDocLocalizedContent.Root.Elements("string");

            foreach (XElement ele in allElements)
            {
                localizedTags.Add(ele.Attribute("name").Value, ele.Attribute("text").Value);
            }
        }

		/// <summary>
		/// Returns localized text for the given tag.
		/// </summary>
        public string GetTextWithTag(string key)
        {
            string val = "";
            localizedTags.TryGetValue(key, out val);
            return val;
        }
    }

    public enum LocalizedFormat
    {
        SINGLE_FORMAT = 0,
        DOUBLE_FORMAT = 1
    }
}