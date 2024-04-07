using UnityEngine;

namespace GamingMonks
{
    /// <summary>
    /// Selection on mode to be played.
    /// </summary>
    public class SelectMode : MonoBehaviour
    {
        /// <summary>
        /// Close button listener.
        /// </summary>
        public void OnCloseButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                gameObject.Deactivate();
            }
        }

        /// <summary>
        /// Classic mode button listener.
        /// </summary>
        public void OnClassicModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Classic);
            }
        }

        /// <summary>
        /// Time mode button listener.
        /// </summary>
        public void OnTimeModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Timed);
            }
        }

        /// <summary>
        /// Blast mode button listener.
        /// </summary>
        public void OnBlastModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Blast);
            }
        }

        /// <summary>
        /// Advance mode button listener.
        /// </summary>
        public void OnAdvanceModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadGamePlay(GameMode.Advance);
            }
        }

        /// <summary>
        /// Level mode button listener.
        /// </summary>
        public void OnLevelModeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.LoadLevelSelectionScreen();
            }
        }

        /// <summary>
        /// Back button listener.
        /// </summary>
        public void OnBackButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.OpenHomeScreenFromModeSelection();
            }
        }
    }
}
