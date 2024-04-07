using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamingMonks 
{
    /// <summary>
    /// This script displays the balance of currency and keeps updating on change of it during game.
    /// </summary>
    public class CurrencyDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtCoinsBalance;
        [SerializeField] GameObject rewardAnimation;
        [SerializeField] GameObject parentPanel;

        int lastBalance = 0;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            CurrencyManager.OnCurrencyUpdated += OnCurrencyUpdatedEvent;
        }
        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable() {
            RefreshCurrencyBalance(); 
        }

        /// <summary>
        /// Event callback for currecy balance change.
        /// </summary>
        private void OnCurrencyUpdatedEvent(int currencyBalance) {
            if (transform.parent.gameObject.activeSelf)
            {
                StartCoroutine(PlayCurrencyIncreaseCounter(currencyBalance));
            }
        }

        /// <summary>
        /// Refreshes currency balance on enable.
        /// </summary>
        private void RefreshCurrencyBalance() {
            int currencyBalance = CurrencyManager.Instance.GetCurrentCoinsBalance();
            txtCoinsBalance.text = currencyBalance.ToString("N0");
            lastBalance = currencyBalance;
        }

        /// <summary>
        /// Currency amount update animation.
        /// </summary>
        IEnumerator PlayCurrencyIncreaseCounter(int currentBalance) 
        {
            int iterations = 10;
            int balanceDifference = (currentBalance - lastBalance);
            int balanceChangeEachIteration = (balanceDifference / iterations);

            int updatedBalance = lastBalance;

            yield return new WaitForSeconds(0.75F);
            for(int i = 0; i < iterations; i++) {
                updatedBalance += balanceChangeEachIteration;
                txtCoinsBalance.text = updatedBalance.ToString("N0");
                AudioController.Instance.PlayClipLow(AudioController.Instance.addCoinsSoundChord, 0.15F);
                yield return new WaitForSeconds(0.05F);
            }
            txtCoinsBalance.text = currentBalance.ToString("N0");
            lastBalance = currentBalance;
        } 
    }
}
                            