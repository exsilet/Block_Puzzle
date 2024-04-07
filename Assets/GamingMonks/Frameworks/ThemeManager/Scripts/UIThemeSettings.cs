using UnityEngine;

namespace GamingMonks
{
	/// <summary>
	/// Theme settings scriptable instance for all the ingame themes.
	/// </summary>
    public class UIThemeSettings : ScriptableObject
    {
		// Theme selection should be used or not.
        public bool useUIThemes = true;

		// Id of default theme.
        public int defaultTheme = 0;

		/// <summary>
		/// List all of thr UI Themes.
		/// </summary>
        public ThemeConfig[] allThemeConfigs;
    }

    [System.Serializable]
    public class ThemeConfig
    {
		// Theme enabled or not.
        public bool isEnabled = true;

		// Name of the theme.
        public string themeName;

		// Demo sprite to display how this theme will look during game.
        public Sprite demoSprite;

		// UI theme scriptable that contains all color and sprite tags.
        public UITheme uiTheme;

        // 0 - LOCKED, 1 - UNLOCKED
        public int defaultStatus;

        // Cost to unlock theme [GEMS]
        public int unlockCost;

        public IAPProductID themeIAPID;
    }
}