using System.Collections;
using GamingMonks.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	/// <summary>
	/// Settings screen controlls different user selection like sound, music, langauge etc.
	/// </summary>
    public class SettingsScreen : MonoBehaviour
    {
        #pragma warning disable 0649
        //[SerializeField] GameObject dataSettingsOption;
        [SerializeField] GameObject supportButton;
        //[SerializeField] GameObject vibrationToggleButton;
        //[SerializeField] GameObject selectLanguageButton;
        //[SerializeField] TextMeshProUGUI txtVersion;
        private bool isFocus = false;
        private bool isProcessing = false;
#pragma warning restore 0649

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            //dataSettingsOption.SetActive((PlayerPrefs.GetInt("ConsentRequired", 0) == 0) ? false : true);
            supportButton.SetActive((ProfileManager.Instance.GetAppSettings().enableSupportURL) ? true : false);
           // vibrationToggleButton.SetActive((ProfileManager.Instance.GetAppSettings().enableVibrations) ? true : false);

            int activeLanguages = 0;
            foreach (LocalizedLanguage lang in LocalizationManager.Instance.allLocalizedLanaguages)
            {
                if (lang.isLanguageEnabled)
                {
                    activeLanguages += 1;
                }
            }
            //selectLanguageButton.SetActive(((LocalizationManager.Instance.isLocalizationSupported) && (activeLanguages > 1)));
           // txtVersion.text = "Version : " + Application.version;
        }

		/// <summary>
		/// Close button click listener.
		/// </summary>
        public void OnCloseButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                gameObject.Deactivate();
            }
        }


        /// <summary>
        /// App review click listener.
        /// </summary>
        public void OnAppReviewButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.OpenAppReviewScreen();
            }
        }

        /// <summary>
        /// select theme review click listener.
        /// </summary>
        public void OnSelectThemeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.settingScreen.Deactivate();
                UIController.Instance.selectThemeScreen.Activate();
            }
        }

        /// <summary>
        /// Support button click listener.
        /// </summary>
        public void OnSupportButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                StartCoroutine(NavigateToUrl(ProfileManager.Instance.GetAppSettings().supportURL));
            }
        }

        /// <summary>
        /// Share button click listener.
        /// </summary>
        public void OnShareButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();

#if UNITY_ANDROID

                if (!isProcessing)
                {
                    StartCoroutine(ShareTextInAnroid());
                }

#else
		Debug.Log("No sharing set up for this platform.");
#endif
            }
        }

#if UNITY_ANDROID

        public IEnumerator ShareTextInAnroid()
        {
            var shareSubject = "I'm Playing Block-O-Mania, Free Download and Play it Now."; //Subject text
            var shareMessage = "I'm Playing Block-O-Mania, Free Download and Play it Now. Link: " + //Message text
                                          "https://play.google.com/store/apps/details?id=com.gamingmonk.blockpuzzleblockomania"; //Your link

            isProcessing = true;

            if (!Application.isEditor)
            {
                //Create intent for action send
                AndroidJavaClass intentClass =
                    new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject =
                    new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>
                    ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                //put text and subject extra
                intentObject.Call<AndroidJavaObject>("setType", "text/plain");

                intentObject.Call<AndroidJavaObject>
                    ("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
                intentObject.Call<AndroidJavaObject>
                    ("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

                //call createChooser method of activity class
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

                AndroidJavaObject currentActivity =
                    unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject chooser =
                    intentClass.CallStatic<AndroidJavaObject>
                    ("createChooser", intentObject, "Share your high score");
                currentActivity.Call("startActivity", chooser);
            }

            yield return new WaitUntil(() => isFocus);
            isProcessing = false;
        }

#endif
        void OnApplicationFocus(bool focus)
        {
            isFocus = focus;
        }

        /// <summary>
        /// Privacy policy button click listener.
        /// </summary>
        public void OnPrivacyPolicyButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                StartCoroutine(NavigateToUrl(ProfileManager.Instance.GetAppSettings().privacyPolicyURL));
            }
        }

		/// <summary>
		/// Data privacy button click listener.
		/// </summary>
        public void OnDataSettingsButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.ShowConsentDialogue();
            }
        }

		/// <summary>
		/// Lanaguage select button click listener.
		/// </summary>
        public void OnSelectLanguageButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.lanagueSelectionScreen.Activate();
                //gameObject.Deactivate();
            }
        }

		/// <summary>
		/// Navigate to given URL.
		/// </summary>
        IEnumerator NavigateToUrl(string url)
        {
            yield return new WaitForSeconds(0.2F);
            Application.OpenURL(url);
        }
    }
}
