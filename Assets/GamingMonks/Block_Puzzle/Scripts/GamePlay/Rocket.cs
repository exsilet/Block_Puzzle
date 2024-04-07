using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingMonks
{
    public class Rocket : MonoBehaviour
    {
        [Header("Rocket Speed Settings")]
        #region Rocket Speed Settings
        [SerializeField] private float timeParameter;
        [SerializeField] private float speedModifier;
        [SerializeField] private float turnSpeed;
        #endregion

        #region Rocket Path Transforms
        [Space(5)]
        [Tooltip("Target path for rocket In child of ROCKET")]
        [SerializeField] private Transform initialTransform;
        [SerializeField] private Transform midTransform;
        [SerializeField] private Transform targetTransform;
        #endregion

        #region Private Fields
        private RectTransform rectTransform;
        private Vector2 rocketPos;
        #endregion

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            timeParameter = 0f;
            UIFeedback.Instance.PlayBoomerangMovingEffect();
            //yield return new WaitForSeconds(1f);
            StartCoroutine(GoByTheRoute());
        }

        /// <summary>
        /// Rocket will follow the given path
        /// </summary>
        /// <returns></returns>
        IEnumerator GoByTheRoute()
        {          
            List<Block> targetIsFilledBlockList = GamePlay.Instance.GetRandomIsFilledBlockForRocket();
            if(targetIsFilledBlockList.Count > 0)
            {
                int randomNumber = UnityEngine.Random.Range(0, targetIsFilledBlockList.Count);
                Block targetIsFilledBlock = targetIsFilledBlockList[randomNumber];
                if (targetIsFilledBlock == null)
                {
                    GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);
                    Destroy(gameObject);
                }
                else
                {
                    targetTransform = targetIsFilledBlock.transform;
                }
            }


            List<Vector3> rocketPosList = new List<Vector3>() { new Vector3(-3f, -3f, 0), new Vector3(-5f, 2, 0), new Vector3(2f, -5f, 0) };
            midTransform.position = rocketPosList[GamePlay.Instance.rocketPositionIndex];
            GamePlay.Instance.rocketPositionIndex++;
            if (GamePlay.Instance.rocketPositionIndex == 3)
            {
                GamePlay.Instance.rocketPositionIndex = 0;
            }

            Vector2 p0 = transform.position;
            Vector2 p1 = midTransform.position;
            Vector2 p2 = targetTransform.position;

            while (timeParameter < 1)
            {
                timeParameter += Time.deltaTime * speedModifier;
                rocketPos = Mathf.Pow(1 - timeParameter, 3) * p0 +
                    3 * Mathf.Pow(1 - timeParameter, 2) * timeParameter * p1+
                    //3 * Mathf.Pow(1- timeParameter, 2) * timeParameter * timeParameter * p1_2 +
                    Mathf.Pow(timeParameter, 3) * p2;

                //Vector2 a = rocketPos - (Vector2)transform.position;
                //float angle = Mathf.Atan2(a.y, a.x) * Mathf.Rad2Deg;
                //Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
                transform.Rotate(0, 0, -30f);
                transform.position = rocketPos;
                yield return new WaitForEndOfFrame();
            }

            rectTransform.pivot = new Vector2(2f, 0.5f);

            foreach(Block b in targetIsFilledBlockList)
            {
                if (b.isFilled)
                {
                    b.ClearBlock();
                }
            }
            GamePlay.Instance.movingRocketsList.Remove(gameObject);
            if (GamePlay.Instance.movingRocketsList.Count == 0)
            {
                foreach (List<Block> blocks in GamePlay.Instance.allRows)
                {
                    foreach (Block block in blocks)
                    {
                        if (block.isFilled)
                        {
                            block.ClearBlock();
                        }
                    }
                }
            }
            if (GamePlay.Instance.movingRocketsList.Count <= 0)
            {
                UIFeedback.Instance.StopAudio();
            }
            Destroy(gameObject);
        }

    }

}

