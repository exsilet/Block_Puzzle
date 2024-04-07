using System;   
using UnityEngine;

namespace GamingMonks 
{
    /// <summary>
    /// This sigletion class will track and handles daily reward during game.
    /// </summary>
    public class DailyRewardManager : Singleton<DailyRewardManager>
    {
        bool hasInitialised = false;
        bool isDailyRewardAvailable = false;

        public int currentRewardDay = 1;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start() {
             Initialise();
        }

        void Initialise() {
            if(!hasInitialised) {
                hasInitialised = true;
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable() {
            SessionManager.OnSessionUpdatedEvent += OnSessionUpdated;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable() {
            SessionManager.OnSessionUpdatedEvent -= OnSessionUpdated;
        }

        private void OnSessionUpdated(SessionInfo info) 
        {
            if(ProfileManager.Instance.GetAppSettings().useDailyRewards) {
                if(info.currentSessionCount == 2) 
                {
                    currentRewardDay = 1;
                    isDailyRewardAvailable = true;
                    UIController.Instance.ShowDailyRewardsPopup();
                } else {
                    CheckForDailyReward();
                }
            }
        }

        /// <summary>
        /// Checks if day has changed and daily reward is available or not.
        /// </summary>
        void CheckForDailyReward() 
        {
            DateTime lastRewardCollectionDate = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("lastRewardCollectionDate",DateTime.Now.ToBinary().ToString()))).Date;
            DateTime currentDate = DateTime.Now.Date;
            int totalDays = (int) (currentDate - lastRewardCollectionDate).TotalDays;

            if(totalDays < 0) {
                PlayerPrefs.DeleteKey("lastRewardDay");
                isDailyRewardAvailable = false;
                currentRewardDay = 0;
                return;
            }
            if(totalDays >= 1) {
                //if(currentRewardDay %8 != 0) {
                int lastRewardDay = PlayerPrefs.GetInt("lastRewardDay",0);
                currentRewardDay = (lastRewardDay + 1);
                isDailyRewardAvailable = true;

                if(isDailyRewardAvailable) {
                    UIController.Instance.ShowDailyRewardsPopup();
                }
            }
        }

        /// <summary>
        /// Callback sent to all game objects when the player pauses.
        /// </summary>
        /// <param name="pauseStatus">The pause state of the application.</param>
        private void OnApplicationPause(bool pauseStatus) 
        {
            if(pauseStatus) {
            } else {
                if(hasInitialised) {
                /// Checks if daily reward is available on app resume.
                    CheckForDailyReward();
                }
            }
        }

        /// <summary>
        /// Saves info of daily reward collection and curreny day.
        /// </summary>
        public void SaveCollectRewardInfo() 
        {
            PlayerPrefs.SetInt("lastRewardDay",currentRewardDay);
            PlayerPrefs.SetString("lastRewardCollectionDate",DateTime.Now.ToBinary().ToString());
        }
    }
}