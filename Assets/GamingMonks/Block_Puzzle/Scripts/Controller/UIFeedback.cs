using GamingMonks.HapticFeedback;
using UnityEngine;

namespace GamingMonks
{ 
	public class UIFeedback : Singleton<UIFeedback>  
	{
		/// Play Haptic/Vibration Light.
		public void PlayHapticLight() {
			if(ProfileManager.Instance.IsVibrationEnabled) { 
				HapticFeedbackGenerator.Haptic(HapticFeedback.FeedbackType.LightImpact);
			}
		}

		/// Play Haptic/Vibration Medium.
		public void PlayHapticMedium() {
			if(ProfileManager.Instance.IsVibrationEnabled) { 
				HapticFeedbackGenerator.Haptic(HapticFeedback.FeedbackType.MediumImpact);
			}
		}

		/// Play Haptic/Vibration Heavy.
		public void PlayHapticHeavy() {
			if(ProfileManager.Instance.IsVibrationEnabled) { 
				HapticFeedbackGenerator.Haptic(HapticFeedback.FeedbackType.HeavyImpact);
			}
		}


		/// Plays Button Click Sound and Haptic Feedback.
		public void PlayButtonPressEffect() {
			AudioController.Instance.PlayButtonClickSound();
			PlayHapticLight();
		}
		
		/// Plays Block Shape Pick Effect.
		public void PlayBlockShapePickEffect() {
			AudioController.Instance.PlayBlockShapePickSound();
			PlayHapticLight();
		}

		/// Plays Block Shape Pick Effect.
		public void PlayBlockShapePlaceEffect() {
			AudioController.Instance.PlayBlockShapePlaceSound();
			PlayHapticLight();
		}

		public void PlayBoomerangMovingEffect()
        {
			AudioController.Instance.PlayBoomerangSound();
        }

		public void StopAudio()
        {
			AudioController.Instance.StopBoomerangSound();
        }


		/// Plays Block Shape Pick Effect.
		public void PlayBlockShapeResetEffect() {
			AudioController.Instance.PlayBlockShapeResetSound();
			PlayHapticLight();
		}
	}
}