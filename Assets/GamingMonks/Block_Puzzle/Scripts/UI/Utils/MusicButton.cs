using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	public class MusicButton : MonoBehaviour 
	{
		// The button to toggle music, assigned from inspector.
		public Button btnMusic;
		// The image of the button.
		public Image btnMusicImage;
		//// The On sprite for music.
		//public Sprite musicOnSprite;
		//// The off sprite for music.
		//public Sprite musicOffSprite;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start()
		{
			btnMusic.onClick.AddListener(() => {
				if (InputManager.Instance.canInput ()) {
					UIFeedback.Instance.PlayButtonPressEffect();
					ProfileManager.Instance.ToggleMusicStatus();
					ProfileManager.Instance.ToggleSoundStatus();
				}
			});
		}

		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		void OnEnable()
		{
			ProfileManager.OnMusicStatusChangedEvent += OnMusicStatusChanged;
			initMusicStatus();
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		void OnDisable()
		{ 			
            ProfileManager.OnMusicStatusChangedEvent -= OnMusicStatusChanged;
		}

		/// <summary>
		/// Inits the music status.
		/// </summary>
		void initMusicStatus()
		{
			if(ProfileManager.Instance.IsMusicEnabled)
			{
				btnMusicImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag("MusicON");
			}
			else
			{
				btnMusicImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag("MusicOFF");
            }
		}

		/// <summary>
		/// Raises the music status changed event.
		/// </summary>
		void OnMusicStatusChanged (bool isMusicEnabled)
		{
			if(isMusicEnabled)
			{
				btnMusicImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag("MusicON");
            }
			else
			{
				btnMusicImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag("MusicOFF");
            }
		}	
	}
}