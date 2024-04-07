using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GamingMonks {
    public class HealthPanelController : MonoBehaviour
    {
        [SerializeField] private Button btnAd;
        [SerializeField] private TextMeshProUGUI txtRemainLife;
        [SerializeField] private TextMeshProUGUI txtPurchaseWithCoinAmount;
        private void OnEnable()
        {
            txtRemainLife.text = HealthController.Instance.GetCurrentHealth().ToString();
            txtPurchaseWithCoinAmount.text = GamePlay.Instance.appSettings.lifeCost.ToString();

            // if (!AdmobManager.Instance.GetRewardedAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadRewardedAd();
            // }
        }

        public void OnEarnOneLifeBywatchingAdButtonPressed()
        {
            // if (!AdmobManager.Instance.GetRewardedAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadRewardedAd();
            // }

            AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetLife);
            UIController.Instance.healthPanel.Deactivate();
        }

        public void OnEarnFiveLifeBySpendCoinsButtonPressed()
        {
            //HealthController.Instance.currentEnergy += 5;
            if (CurrencyManager.Instance.GetCurrentCoinsBalance() >= 50)
            {
                CurrencyManager.Instance.DeductCoins(50);
                int val = HealthController.Instance.currentEnergy += 5;
                PlayerPrefs.SetInt("currentEnergy", val);
                HealthController.Instance.UpdateEnergy();
                HealthController.Instance.UpdateEnergyTimer();
                UIController.Instance.healthPanel.Deactivate();
            }
            else
            {
                if (UIController.Instance.shopScreen.gameObject.activeSelf)
                {
                    gameObject.Deactivate();
                }
                else
                {
                    UIController.Instance.OpenShopScreen();
                }

            }
        }

        public void OnQuitButtonClick()
        {
            UIController.Instance.healthPanel.Deactivate();
        }
    }
}