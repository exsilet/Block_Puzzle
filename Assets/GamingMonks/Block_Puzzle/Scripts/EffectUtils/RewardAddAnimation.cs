using System.Collections;
using System.Collections.Generic;
using GamingMonks.HapticFeedback;
using UnityEngine;
using GamingMonks.UITween;
using UnityEngine.UI;

namespace GamingMonks
{
    /// <summary>
    /// This script generates coins adding/deducting effect while any change any coins balance.
    /// </summary>
    public class RewardAddAnimation : MonoBehaviour
    {   
        [SerializeField] List<RectTransform> allElements;
        [SerializeField] List<Image> allSprites;
        [SerializeField] Image coinSpriteTemplate;
        Vector3 elementMovePosition = Vector3.zero;

        /// Starts Animation.
        public void PlayCoinsBalanceUpdateAnimation(Vector3 toPosition, float delay, Sprite[] sprite) {
            elementMovePosition = toPosition;
            int count = 0;
            if(sprite != null)
            {
                foreach (Sprite imgSprite in sprite)
                {
                    if (imgSprite != null)
                    {
                        count++;
                    }
                }
            }

            int range = 0;
            if (count == 0)
            {
                for(int i = 0; i< allSprites.Count; i++)
                {
                    allSprites[i].sprite = coinSpriteTemplate.sprite;
                }
                
            }
            else if (count == 1)
            {
                for (int i = 0; i < allSprites.Count; i++)
                {
                    allSprites[i].sprite = sprite[0];
                }
            }
            else if (count == 2)
            {
                //2
                range = 10 / 2;
                for (int i = 0; i < allSprites.Count; i++)
                {
                    if(i< range)
                    {
                        allSprites[i].sprite = sprite[0];
                    }
                    else
                    {
                        allSprites[i].sprite = sprite[1];
                    }
                }
            }
            else if (count == 3)
            {
                //3
                range = 10 / 3;
                for (int i = 0; i < allSprites.Count; i++)
                {
                    if (i < range)
                    {
                        allSprites[i].sprite = sprite[0];
                    }
                    else if(i >= range && i < range * 2)
                    {
                        allSprites[i].sprite = sprite[1];
                    }
                    else
                    {
                        allSprites[i].sprite = sprite[2];
                    }
                }
            }
            StartCoroutine(PlayAddRewardAnimationCoroutine(delay));
        }

        /// Plays animations and iterated all coin images.
        IEnumerator PlayAddRewardAnimationCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            int iterations = 10;
            for (int i = 0; i < iterations; i++)
            {
                allElements[i].Position(elementMovePosition, 0.5F).OnComplete(() =>
                {
                    if (ProfileManager.Instance.IsVibrationEnabled)
                    {
                        HapticFeedbackGenerator.Haptic(HapticFeedback.FeedbackType.LightImpact);
                    }
                });
                yield return new WaitForSeconds(0.05F);
                if (ProfileManager.Instance.IsVibrationEnabled)
                {
                    HapticFeedbackGenerator.Haptic(HapticFeedback.FeedbackType.LightImpact);
                }
            }
            Invoke("DestroyAnim", 0.5F);
        }

        void DestroyAnim()
        {
            Destroy(gameObject);
        }
    }
}
