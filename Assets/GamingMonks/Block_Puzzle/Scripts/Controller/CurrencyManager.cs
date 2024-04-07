using System;
using GamingMonks.Ads;
using UnityEngine;

namespace GamingMonks
{
    /// <summary>
    /// This script controlls and manages the ingame currecy, its balace, addition or subtraction of balance.
    /// </summary>
    public class CurrencyManager : Singleton<CurrencyManager>
    {
        public static event Action<int> OnCurrencyUpdated;

        public int currentBalance = 10;
        public int currentHealth;
        public int currentRotatePowerUp;
        public int currentSingleBlockPowerUp;
        public int currentBombPowerUp;
        bool hasInitialised = false;

        public event Action<int, int,int,int> OnValuesChanged;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            Initialise();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            AdManager.OnRewardedAdRewardedEvent += OnRewardedAdRewarded;
        }

        /// <summary>
        /// This function is called when the object becomes disabled and inactive.
        /// </summary>
        private void OnDisable()
        {
            AdManager.OnRewardedAdRewardedEvent -= OnRewardedAdRewarded;
        }

        /// <summary>
        /// Initialize and restores the balance  and displays it.
        /// </summary>
        void Initialise()
        {
            UpdatePowerUps();
            if (PlayerPrefs.HasKey("currentBalance"))
            {
                currentBalance = PlayerPrefs.GetInt("currentBalance");              
            }
            else
            {
                currentBalance = ProfileManager.Instance.GetAppSettings().defaultCoinAmount;

            }
            hasInitialised = true;
        }

        /// <summary>
        /// Returns current balance.
        /// </summary>
        public int GetCurrentCoinsBalance()
        {
            if (!hasInitialised)
            {
                Initialise();
            }
            return currentBalance;
        }


        /// <summary>
        /// Add coins to current balance.
        /// </summary>
        public void AddCoins(int coinsAmount)
        {
            if (coinsAmount > 0) {
                currentBalance += coinsAmount;
            }
            SaveCurrencyBalance();
            if (OnCurrencyUpdated != null)
            {
                OnCurrencyUpdated.Invoke(currentBalance);
            }
            AudioController.Instance.PlayClip(AudioController.Instance.addCoinsSound);
        }

        public void SetPacks(Reward packs)
        {
            if (packs.coin > 0)
            {
                currentBalance += packs.coin;
            }
            SaveCurrencyBalance();
            if (OnCurrencyUpdated != null)
            {
                OnCurrencyUpdated.Invoke(currentBalance);
            }
            AudioController.Instance.PlayClip(AudioController.Instance.addCoinsSound);
            //currentHealth += packs.health;
            currentRotatePowerUp += packs.rotatePowerUp;
            currentSingleBlockPowerUp += packs.singleBlockPowerUp;
            currentBombPowerUp += packs.bombPowerUp;
            SavePackValues();
        }

        /// <summary>
        /// Will deduct given amount from balance if enough balance is available.
        /// </summary>
        public bool DeductCoins(int coinsAmount)
        {
            if (currentBalance >= coinsAmount)
            {
                currentBalance -= coinsAmount;
                SaveCurrencyBalance();

                if (OnCurrencyUpdated != null) {
                    OnCurrencyUpdated.Invoke(currentBalance);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save currency balance.
        /// </summary>
        void SaveCurrencyBalance()
        {
            PlayerPrefs.SetInt("currentBalance", currentBalance);
        }

        public void SavePackValues()
        {
            PlayerPrefs.SetInt("currentHealth", currentHealth);
            PlayerPrefs.SetInt("currentRotatePowerUp", currentRotatePowerUp);
            PlayerPrefs.SetInt("currentBombPowerUp", currentBombPowerUp);
            PlayerPrefs.SetInt("currentSingleBlockPowerUp", currentSingleBlockPowerUp);
            OnValuesChanged?.Invoke(currentHealth, currentRotatePowerUp, currentBombPowerUp, currentSingleBlockPowerUp);
        }

        public void SavePackValues(int lifeAmount, int rotateAmount, int bombAmount, int singleBlockAmount)
        {
            currentHealth += lifeAmount;
            currentRotatePowerUp += rotateAmount;
            currentSingleBlockPowerUp += singleBlockAmount;
            currentBombPowerUp += bombAmount;
            PlayerPrefs.SetInt("currentHealth", currentHealth);
            PlayerPrefs.SetInt("currentRotatePowerUp", currentRotatePowerUp);
            PlayerPrefs.SetInt("currentBombPowerUp", currentBombPowerUp);
            PlayerPrefs.SetInt("currentSingleBlockPowerUp", currentSingleBlockPowerUp);
            OnValuesChanged?.Invoke(currentHealth, currentRotatePowerUp, currentBombPowerUp, currentSingleBlockPowerUp);
        }

        public void UpdatePowerUps()
        {
            currentHealth = PlayerPrefs.GetInt("currentHealth");
            currentRotatePowerUp = PlayerPrefs.GetInt("currentRotatePowerUp");
            currentBombPowerUp = PlayerPrefs.GetInt("currentBombPowerUp");
            currentSingleBlockPowerUp = PlayerPrefs.GetInt("currentSingleBlockPowerUp");
            OnValuesChanged?.Invoke(currentHealth, currentRotatePowerUp, currentBombPowerUp, currentSingleBlockPowerUp);
        }

        /// <summary>
        /// Adds currecy balance from the rewarded ad.
        /// </summary>
        void OnRewardedAdRewarded(string tag)
        {
            switch(tag) {
                case "FreeCoins":
                AddCoins(ProfileManager.Instance.GetAppSettings().watchAdsRewardCoin);
                UIController.Instance.purchaseSuccessScreen.Activate();
                break;
            }
        }
    }
}
