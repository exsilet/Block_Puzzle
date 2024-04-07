using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GamingMonks
{
    public class TopPanelWithHearts : MonoBehaviour
    {
        public RectTransform coinPanelIcon;
        public void OnBackButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.OpenScreenFromTopPanel();
            }
        }

        public void OnShopButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.OpenShopScreen();
            }
        }

        public void OnSettingsButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.OpenSettingsScreen();
            }
        }

        public void OnHealthBtnPressed()
        {
            UIController.Instance.OpenLivesPanel();
        }
    }
}
