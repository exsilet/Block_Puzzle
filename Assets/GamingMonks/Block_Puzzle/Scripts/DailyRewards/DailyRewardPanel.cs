using UnityEngine;
using UnityEngine.UI;
using GamingMonks.Utils;
using GamingMonks.Localization;
using TMPro;

namespace GamingMonks
{
    /// <summary>
    /// This script component is attached to all buttons daily reward poppup. It handles, displays rewards and giveaway.
    /// daily reward for the current day.
    /// </summary>
    public class DailyRewardPanel : MonoBehaviour
    {
        int coinAmount = 0;
        int lifeAmount = 0;
        int rotateAmount = 0;
        int singleBlockAmount = 0;
        int bombAmount = 0;

        [SerializeField] TextMeshProUGUI txtDay;
        [SerializeField] GameObject coinSprite;
        [SerializeField] GameObject lifeSprite;
        [SerializeField] GameObject bombSprite;
        [SerializeField] GameObject singleBlockSprite;
        [SerializeField] GameObject rotateSprite;
        [SerializeField] RectTransform CoinsRewardPlacement;

        private Sprite[] animationSprite = new Sprite[3];

        /// <summary>
        /// Prepares daily reward for the day.
        /// </summary>
        public void SetReward(int day, int currentRewardDay, AllDayRewards reward)
        {
            for(int i=0;i < 3; i++)
            {
                animationSprite[i] = null;
            }
            txtDay.SetFormattedTextWithTag("txtDay_FR", day.ToString(), "");
            if(reward.allCoinRewards > 0)
            {
                coinSprite.gameObject.SetActive(true);
                animationSprite[0] = coinSprite.GetComponent<Image>().sprite;
                coinSprite.GetComponentInChildren<TextMeshProUGUI>().text = reward.allCoinRewards.ToString();
            }
            else
            {
                if(reward.allLifeRewards > 0)
                {
                    coinSprite.gameObject.SetActive(false);
                    lifeSprite.gameObject.SetActive(true);
                    animationSprite[0] = lifeSprite.GetComponent<Image>().sprite;
                    lifeSprite.GetComponentInChildren<TextMeshProUGUI>().text = reward.allLifeRewards.ToString();
                }
                else
                {
                    coinSprite.gameObject.SetActive(false);
                    lifeSprite.gameObject.SetActive(false);
                    bombSprite.gameObject.SetActive(reward.allBombRewards > 0);
                    singleBlockSprite.gameObject.SetActive(reward.allSingleBlockRewards > 0);
                    rotateSprite.gameObject.SetActive(reward.allRotateRewards > 0);
                    int i = 0;
                    if (reward.allBombRewards > 0)
                    {
                        animationSprite[i] = bombSprite.GetComponent<Image>().sprite;
                        i++;
                    }
                    if (reward.allSingleBlockRewards > 0)
                    {
                        animationSprite[i] = singleBlockSprite.GetComponent<Image>().sprite;
                        i++;
                    }
                    if(reward.allRotateRewards > 0)
                    {
                        animationSprite[i] = rotateSprite.GetComponent<Image>().sprite;
                    }
                }
            }

            if (day <= currentRewardDay)
            {
                gameObject.GetComponent<Image>().sprite = ThemeManager.Instance.GetBlockSpriteWithTag("DailyRewardCollected");

                if (day < currentRewardDay)
                {

                }
                else
                {
                    Invoke("AnimateAndProcessRewards", 1F);
                    DailyRewardManager.Instance.SaveCollectRewardInfo();
                }
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = ThemeManager.Instance.GetBlockSpriteWithTag("DailyRewardNotCollected");
            }
            coinAmount = reward.allCoinRewards;
            lifeAmount = reward.allLifeRewards;
            rotateAmount = reward.allRotateRewards;
            singleBlockAmount = reward.allSingleBlockRewards;
            bombAmount = reward.allBombRewards;
        }

        /// <summary>
        /// Gives reward with animation.
        /// </summary>
        void AnimateAndProcessRewards()
        {
            UIController.Instance.PlayAddCoinsAnimationAtPosition(CoinsRewardPlacement.position, UIController.Instance.ShopButtonCoinsIcon.position, 0, animationSprite);
            CurrencyManager.Instance.AddCoins(coinAmount);
            CurrencyManager.Instance.SavePackValues(lifeAmount, rotateAmount, bombAmount, singleBlockAmount);
        }
    }
}
