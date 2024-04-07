using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    public class VibrationButton : MonoBehaviour
    {
        // The button to toggle Vibration, assigned from inspector.
        public Button btnVibration;
        // The image of the button.
        public Image btnVibrationImage;
        // The On sprite for Vibration.
        public Sprite VibrationOnSprite;
        // The off sprite for Vibration.
        public Sprite VibrationOffSprite;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            btnVibration.onClick.AddListener(() =>
            {
                if (InputManager.Instance.canInput())
                {
                    UIFeedback.Instance.PlayButtonPressEffect();
                    ProfileManager.Instance.TogglVibrationStatus();
                }
            });
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        void OnEnable()
        {
            ProfileManager.OnVibrationStatusChangedEvent += OnVibrationStatusChanged;
            initVibrationStatus();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            ProfileManager.OnVibrationStatusChangedEvent -= OnVibrationStatusChanged;
        }

        /// <summary>
        /// Inits the Vibration status.
        /// </summary>
        void initVibrationStatus()
        {
            if (ProfileManager.Instance.IsVibrationEnabled)
            {
                btnVibrationImage.sprite = VibrationOnSprite;
                //btnVibrationImage.color = new Color(1,1,1,1);
            }
            else
            {
                btnVibrationImage.sprite = VibrationOffSprite;
                //btnVibrationImage.color = new Color(1,1,1,0.7F);
            }
        }

        /// <summary>
        /// Raises the Vibration status changed event.
        /// </summary>
        /// <param name="isVibrationEnabled">If set to <c>true</c> is Vibration enabled.</param>
        void OnVibrationStatusChanged(bool isVibrationEnabled)
        {
            if (isVibrationEnabled)
            {
                btnVibrationImage.sprite = VibrationOnSprite;
                //btnVibrationImage.color = new Color(1,1,1,1);
            }
            else
            {
                btnVibrationImage.sprite = VibrationOffSprite;
                //btnVibrationImage.color = new Color(1,1,1,0.7F);
            }
        }
    }
}