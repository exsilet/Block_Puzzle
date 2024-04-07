using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{   
    /// <summary>
    /// This script component is attached to all buttons daily reward poppup. It handles, displays rewards and giveaway.
    /// </summary>
    public class DailyRewards : MonoBehaviour
    {
        [SerializeField] List<DailyRewardPanel> allDayRewards;
        [SerializeField] Button btnContinue;

        /// <summary>
        /// Closes popup.
        /// </summary>
        public void OnCloseButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {   
                UIFeedback.Instance.PlayButtonPressEffect();
                gameObject.Deactivate();
            }
        }


        /// <summary>
        /// Continue will close screen after processing daily rewards.
        /// </summary>
        public void OnContinueButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                gameObject.Deactivate();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            btnContinue.interactable = false;
            UIController.Instance.EnableCurrencyBalanceButton();
            int currentRewardDay = DailyRewardManager.Instance.currentRewardDay;
            PrepareDailyRewardScreen(currentRewardDay);

            Invoke("EnableContinueButton",3F);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()  {
            UIController.Instance.Invoke("DisableCurrencyBalanceButton",0.1F);
        }


        /// <summary>
        /// Prepares daily reward from this method.
        /// </summary>
        private void PrepareDailyRewardScreen(int currentRewardDay)
        {
            List<int> displayRewardDays = new List<int>();
            List<AllDayRewards> displayRewards = new List<AllDayRewards>();

            //if (currentRewardDay % 8 == 0)
            //{
            //    currentRewardDay = 1;
            //}
            for (int index = 1; index <= allDayRewards.Count; index++)
            {
                displayRewardDays.Add(index);
            }


            foreach (int rewardDay in displayRewardDays) {
                displayRewards.Add(GetRewardForDay(rewardDay));
            }
            int dailyRewardDay = 1;
            bool rewardsCompleted = false;
            if (!rewardsCompleted)
            {
                if (currentRewardDay % 7 != 0)
                {
                    dailyRewardDay = currentRewardDay % 7;
                }
                else
                {
                    dailyRewardDay = 7;
                    rewardsCompleted = true;
                }
            }
            else
            {
                dailyRewardDay = 1;
                rewardsCompleted = false;
            }

            for (int index = 0; index < allDayRewards.Count; index++) {
                allDayRewards[index].SetReward(displayRewardDays[index], dailyRewardDay, displayRewards[index]);
            }
        }

        /// <summary>
        /// Returns reward for amount for the given day.
        /// </summary>
        AllDayRewards GetRewardForDay(int day)
        {
            if(day < ProfileManager.Instance.GetAppSettings().dailyRewardsSettings.allDayRewards.Length) {
                return ProfileManager.Instance.GetAppSettings().dailyRewardsSettings.allDayRewards[day-1];
            } else {
                day = ProfileManager.Instance.GetAppSettings().dailyRewardsSettings.allDayRewards.Length - 1;
                return ProfileManager.Instance.GetAppSettings().dailyRewardsSettings.allDayRewards[day];
            }
        }

        void EnableContinueButton() {
            btnContinue.interactable = true;
        }
    }
}
