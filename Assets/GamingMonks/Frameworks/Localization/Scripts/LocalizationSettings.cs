using UnityEngine;

namespace GamingMonks.Localization
{
	/// <summary>
	/// Scriptable instance of localization settings containing all info regarding in game localization. 
	/// </summary>
    public class LocalizationSettings : ScriptableObject
    {
        public bool useLocalization = true;
        public LocalizedLanguage[] localizedLanguages;
        public int defaultLangauge;
        public bool localizeToSystemDetectedLanguage;
    }

	/// <summary>
	/// Localization info and settings for langauge.
	/// </summary>
    [System.Serializable]
    public class LocalizedLanguage
    {
		// Name of langauges.
        public string languageName;

		// Language enabled to use or not.
        public bool isLanguageEnabled = true;

		// Disaply name of langauges. 
        public string langaugeDisplayName;

		// Langauge code.
        public string languageCode;

		// Flag of langauge.
        public Sprite languageFlag;

		// Localized string file for the configured langauge.
        public TextAsset localizedTextFile;
    }
}