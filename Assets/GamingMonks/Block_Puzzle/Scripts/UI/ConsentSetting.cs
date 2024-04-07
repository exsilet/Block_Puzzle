using System.Collections;
using GamingMonks.Ads;
using UnityEngine;

namespace GamingMonks
{
    public class ConsentSetting : MonoBehaviour
    {
        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake()
        {
            PlayerPrefs.SetInt("ConsentRequired", 1);
        }


        /// <summary>
        /// Privacy button click event, will open privacy policy url.
        /// </summary>
        public void OnPrivacyPolicyButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                StartCoroutine(NavigateToUrl(ProfileManager.Instance.GetAppSettings().privacyPolicyURL));
            }
        }

        IEnumerator NavigateToUrl(string url)
        {
            yield return new WaitForSeconds(0.2F);
            Application.OpenURL(url);
        }

        /// <summary>
        /// Approve/Accept consent.
        /// </summary>
        public void OnContinueButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                AdManager.Instance.SetConsentStatus(true);
                gameObject.Deactivate();
            }
        }

        /// <summary>
        /// Not accepting consent.
        /// </summary>
        public void OnNotNowButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                AdManager.Instance.SetConsentStatus(false);
                gameObject.Deactivate();
            }
        }
    }
}
