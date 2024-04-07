using System.Collections;
using System.Collections.Generic;
using GamingMonks.Localization;
using UnityEngine;

#if HB_UNITYIAP
using UnityEngine.Purchasing;
#endif

namespace GamingMonks
{
    public class GMIAPListener : MonoBehaviour
    {
        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable() {
            IAPManagers.OnPurchaseSuccessfulEvent += OnPurchaseSuccessful;
            IAPManagers.OnPurchaseFailedEvent += OnPurchaseFailed;
            IAPManagers.OnRestoreCompletedEvent += OnRestoreCompleted;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable() {
            IAPManagers.OnPurchaseSuccessfulEvent -= OnPurchaseSuccessful;
            IAPManagers.OnPurchaseFailedEvent -= OnPurchaseFailed;
            IAPManagers.OnRestoreCompletedEvent -= OnRestoreCompleted;
        }

        void OnPurchaseSuccessful(Product purchasedProduct)
        {
            switch (purchasedProduct.rewardType)
            {
                case RewardType.RemoveAds:
                    ProfileManager.Instance.SetAppAsAdFree();
                    UIController.Instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag("txtSuccess"), LocalizationManager.Instance.GetTextWithTag("txtInappSuccessMsg"));
                    break;

               case RewardType.Coin:
                    CurrencyManager.Instance.AddCoins(purchasedProduct.reward.coin);
                    UIController.Instance.purchaseSuccessScreen.Activate();
                    break;
                case RewardType.Pack:
                    CurrencyManager.Instance.SetPacks(purchasedProduct.reward);
                    UIController.Instance.purchaseSuccessScreen.Activate();
                    break;

                case RewardType.Theme:
                    ThemeManager.Instance.UnlockTheme(purchasedProduct.Id);
                    UIController.Instance.purchaseSuccessScreen.Activate();
                    break;

                case RewardType.OTHER:
                    break;
            }
        }

        void OnPurchaseFailed(string reason) {
            new CommonDialogueInfo().SetTitle(LocalizationManager.Instance.GetTextWithTag("txtOops")).
			SetMessage(LocalizationManager.Instance.GetTextWithTag("txtPurchaseFail")).
			SetMessageType(CommonDialogueMessageType.Info).
			SetOnConfirmButtonClickListener(()=> {
				UIController.Instance.commonMessageScreen.Deactivate();
                //UIController.Instance.shopScreen.gameObject.Activate();
			}).Show();
        }

        void OnRestoreCompleted(bool result) {
            if(result) {
                UIController.Instance.ShowMessage(("txtSuccess"), LocalizationManager.Instance.GetTextWithTag("txtInAppRestored"));
            } else {
                UIController.Instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag("txtAlert"), LocalizationManager.Instance.GetTextWithTag("txtNoRestore"));
            }
        }
    }   
}
