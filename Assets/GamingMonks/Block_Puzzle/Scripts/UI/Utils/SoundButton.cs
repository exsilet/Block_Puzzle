using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	public class SoundButton : MonoBehaviour 
	{
		// The button to turn on/off sound.
		public Button btnSound;	
		// Image of the button on which sound sprite will get assigned. Default on
		public Image btnSoundImage; 
		// Sound on sprite.
		public Sprite soundOnSprite;
		// Sounf off sprite.
		public Sprite soundOffSprite;

		/// <summary>
    	/// Start is called on the frame when a script is enabled just before
    	/// any of the Update methods is called the first time.
    	/// </summary>
		void Start()
		{
			btnSound.onClick.AddListener(() => 
			{
				if (InputManager.Instance.canInput ()) {
					UIFeedback.Instance.PlayButtonPressEffect();
					ProfileManager.Instance.ToggleSoundStatus();
				}
			});
		}

		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		void OnEnable()
		{
			ProfileManager.OnSoundStatusChangedEvent += OnSoundStatusChanged;
			initSoundStatus ();
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		void OnDisable()
		{
			ProfileManager.OnSoundStatusChangedEvent -= OnSoundStatusChanged;
		}

		/// <summary>
		/// Inits the sound status.
		/// </summary>
		void initSoundStatus()
		{
			if(ProfileManager.Instance.IsSoundEnabled)
			{
				btnSoundImage.sprite = soundOnSprite;
			}
			else
			{
				btnSoundImage.sprite = soundOffSprite;
			}
		}

		/// <summary>
		/// Raises the sound status changed event.
		/// </summary>
		void OnSoundStatusChanged (bool isSoundEnabled)
		{
			if(isSoundEnabled)
			{
				btnSoundImage.sprite = soundOnSprite;
			}
			else
			{
				btnSoundImage.sprite = soundOffSprite;
			}
		}	
	}
}