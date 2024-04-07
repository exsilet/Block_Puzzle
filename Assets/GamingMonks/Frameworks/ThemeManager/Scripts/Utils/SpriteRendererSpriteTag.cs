using UnityEngine;

namespace GamingMonks
{
    /// <summary>
    /// Attach this script to any sprite renderer to associalte it with sprite tag.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererSpriteTag : MonoBehaviour
    {
        SpriteRenderer thisRenderer;
        #pragma warning disable 0649
        [SerializeField] string spriteTag;
		#pragma warning restore 0649

        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake()
        {
            thisRenderer = GetComponent<SpriteRenderer>();

            if (ThemeManager.Instance.hasInitialised)
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            ThemeManager.OnThemeInitializedEvent += ThemeManager_OnThemeInitializedEvent;
            ThemeManager.OnThemeChangedEvent += ThemeManager_OnThemeChangedEvent;

            if (ThemeManager.Instance.hasInitialised)
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            ThemeManager.OnThemeInitializedEvent -= ThemeManager_OnThemeInitializedEvent;
            ThemeManager.OnThemeChangedEvent -= ThemeManager_OnThemeChangedEvent;
        }

        /// <summary>
        /// Theme Manager initialization callback.
        /// </summary>
        private void ThemeManager_OnThemeInitializedEvent(string themeName)
        {
            UpdateUI();
        }

        /// <summary>
        /// Theme changed callback.
        /// </summary>
        private void ThemeManager_OnThemeChangedEvent(string themeName)
        {
            UpdateUI();
        }

        /// <summary>
        /// Updates associated tag based on selected theme.
        /// </summary>
        private void UpdateUI()
        {
            if (thisRenderer)
            {
                thisRenderer.SetSpriteWithThemeId(spriteTag);
            }
        }
    }
}
