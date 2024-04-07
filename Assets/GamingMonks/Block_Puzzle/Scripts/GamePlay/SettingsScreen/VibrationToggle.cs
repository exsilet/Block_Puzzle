using GamingMonks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrationToggle : MonoBehaviour
{

    [SerializeField] private Slider m_Slider;

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
            m_Slider.value = 1;
        }
        else
        {
            m_Slider.value = 0;
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
            m_Slider.value = 1;
        }
        else
        {
            m_Slider.value = 0;
        }
    }

    public void OnToggle()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            ProfileManager.Instance.TogglVibrationStatus();
        }
    }
}
