using GamingMonks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;


    /// <summary>
    /// This function is called when the behaviour becomes enabled or active.
    /// </summary>
    void OnEnable()
    {
        ProfileManager.OnSoundStatusChangedEvent += OnSoundStatusChanged;
        initSoundStatus();
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
        if (ProfileManager.Instance.IsSoundEnabled)
        {
            m_Slider.value = 1;
        }
        else
        {
            m_Slider.value = 0;
        }
    }

    /// <summary>
    /// Raises the sound status changed event.
    /// </summary>
    void OnSoundStatusChanged(bool isSoundEnabled)
    {
        if (isSoundEnabled)
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
            ProfileManager.Instance.ToggleSoundStatus();
            ProfileManager.Instance.ToggleMusicStatus();
        }
    }
}
