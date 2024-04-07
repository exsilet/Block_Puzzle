using UnityEngine;

namespace GamingMonks
{
    public class GemsRewardAnimationPlacer : MonoBehaviour
    {
        RewardAddAnimation rewardAddAnimation;
        
        [SerializeField] float animationDelay;

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            InputManager.Instance.DisableTouchForDelay(1F);
            GameObject rewardAnim = (GameObject)Instantiate(Resources.Load("RewardAnimation")) as GameObject;
            rewardAnim.transform.SetParent(transform);
            rewardAnim.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            rewardAnim.transform.localScale = Vector3.one;
            rewardAddAnimation = rewardAnim.GetComponent<RewardAddAnimation>();

            Invoke("ShowAddRewardAnimation", (animationDelay +  0.2F));
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            if (rewardAddAnimation != null)
            {
                Destroy(rewardAddAnimation.gameObject);
            }
        }

        void ShowAddRewardAnimation()
        {
            rewardAddAnimation.PlayCoinsBalanceUpdateAnimation(UIController.Instance.ShopButtonCoinsIcon.position,0, null);
            
        }
    }
}
