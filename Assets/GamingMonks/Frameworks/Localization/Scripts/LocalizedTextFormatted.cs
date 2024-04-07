using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks.Localization
{
    /// <summary>
    /// This script can be attached to any UI Text component with text tag.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedTextFormatted : MonoBehaviour
    {
        #pragma warning disable 0649
        [Tooltip("Assign Text tag containing localized text.")]
        [SerializeField] string txtTag;

        [SerializeField] string formattedValue1;
        [SerializeField] string formattedValue2;
        #pragma warning restore 0649

        TextMeshProUGUI thisText;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            thisText = GetComponent<TextMeshProUGUI>();

            if (txtTag == null) {
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            LocalizationManager.OnLocalizationInitializedEvent += OnLocalizationInitialized;
            LocalizationManager.OnLanguageChangedEvent += OnLanguageChanged;

            LocalizeContent();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            LocalizationManager.OnLocalizationInitializedEvent -= OnLocalizationInitialized;
            LocalizationManager.OnLanguageChangedEvent -= OnLanguageChanged;
        }

        /// <summary>
        /// Event callback on localization initializes.
        /// </summary>
        void OnLocalizationInitialized(LocalizedLanguage lang, bool isLocalizationSupported)
        {
            if (isLocalizationSupported) {
                LocalizeContent();
            }
        }

        /// <summary>
        /// Event callback on language change.
        /// </summary>
        void OnLanguageChanged(LocalizedLanguage lang)
        {
            LocalizeContent();
        }

        void LocalizeContent()
        {
            //if (LocalizationManager.Instance.hasLanguageChanged)
            {
                thisText.SetFormattedTextWithTag(txtTag, formattedValue1, formattedValue2);
            }
        }
    }
}
