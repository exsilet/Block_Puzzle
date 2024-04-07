using System.Collections;
using GamingMonks.Utils;
using UnityEngine;

namespace GamingMonks
{
    public class Kite : MonoBehaviour
    {
        [Header("Kite Speed Settings")]
        #region Kite Speed Settings
        [SerializeField] private float timeParameter;
        [SerializeField] private float speedModifier;
        [SerializeField] private float turnSpeed;
        #endregion

        #region Kite Path Transforms
        [Space(5)]
        [Tooltip("Target path for kite In child of KITE")]
        [SerializeField] private Transform initialTransform;
        [SerializeField] private Transform midTransform;
        [SerializeField] private Transform targetTransform;
        #endregion

        #region Bools
        private bool coroutineAllowed; 
        [SerializeField] private bool noBlockers = false;
        #endregion

        #region Private Fields
        private RectTransform rectTransform;
        private Vector2 kitePos;
        #endregion

        IEnumerator Start()
        {           
            rectTransform = GetComponent<RectTransform>();
            timeParameter = 0f;

            if (!GamePlay.Instance.isBoardReady || JewelMachineController.Instance.isMoving)
            {
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
            // coroutineAllowed = true;
            StartCoroutine(GoByTheRoute());
        }

        // private void Update()
        // {
        //     // if (coroutineAllowed)
        //     // {
        //     //     StartCoroutine(GoByTheRoute());
        //     // }
        // }

        /// <summary>
        /// Kite will follow the given path
        /// </summary>
        /// <returns></returns>
        IEnumerator GoByTheRoute()
        {
            //yield return new WaitForSeconds(0.5f);
            if (GamePlay.Instance.blockers.Count > 0)
            {
                // int i = Random.Range(0, GamePlay.Instance.blockers.Count);
                GamePlay.Instance.blockers.Shuffle();
                Block targetBlock = GamePlay.Instance.blockers[0];
                
                SpriteType targetSpriteType = targetBlock.spriteType;
                if (targetSpriteType != SpriteType.MusicalPlayer && targetSpriteType != SpriteType.Panda &&
                    targetSpriteType != SpriteType.Ice && targetSpriteType != SpriteType.Hat)
                {
                    GamePlay.Instance.blockers.RemoveAt(0);
                }
                
                targetTransform = targetBlock.transform;
            }
            else
            {
                noBlockers = true;
                Block targetIsFilledBlock = GamePlay.Instance.GetRandomIsFilledBlock();
                if(targetIsFilledBlock == null)
                {
                    Destroy(gameObject);
                    GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);
                    // yield break;
                } 
                else
                {
                    targetTransform = targetIsFilledBlock.transform;
                }
            }
           
            // coroutineAllowed = false;
            midTransform.position = new Vector2(transform.position.x + (-1f), transform.position.y + 1.5f);
            Vector2 p0 = transform.position;
            Vector2 p1 = midTransform.position;
            Vector2 p2 = targetTransform.position;

            while (timeParameter < 1)
            {

                timeParameter += Time.deltaTime * speedModifier;
                kitePos = Mathf.Pow(1 - timeParameter, 2) * p0 +
                    3 * Mathf.Pow(1 - timeParameter, 1) * timeParameter * p1 +
                    //3 *(1-tParam)* Mathf.Pow(tParam, 2) * p2 +
                    Mathf.Pow(timeParameter, 3) * p2;

                Vector2 a = kitePos - (Vector2)transform.position;
                float angle = Mathf.Atan2(a.y, a.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
                transform.position = kitePos;
                yield return new WaitForEndOfFrame();
            }
            
            rectTransform.pivot = new Vector2(2f, 0.5f);
           
            if (!noBlockers)
            {
                Block b = targetTransform.GetComponent<Block>();
                b.isHitByKite = true;

                //Destroy(gameObject);
                //GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);

                if (b.hasMilkShop)
                {
                    b.milkShop.CollectMilkBottle();
                    //yield break;
                }
                else
                {
                    b.ClearBlock();
                }
                // GamePlay.Instance.blockers.Remove(b.gameObject);
            }
            else
            {
                Block b = targetTransform.gameObject.GetComponent<Block>();
                b.ClearBlock();
                //Destroy(gameObject);
                //GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);
            }

            if(GamePlay.Instance.jewelMachineEnabled)
            {
                JewelMachineController.Instance.FillBlocks();
            }
            
            GamePlay.Instance.movingKitesList.Remove(gameObject);
            GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);

            if ( GamePlay.Instance.maxMovesAllowed <= 0 && GamePlay.Instance.movingKitesList.Count <= 0)
            {
                GamePlay.Instance.maxMovesAllowed = 0;
                GamePlay.Instance.blockShapeController.DisableAllBlockShapeContainerInput();
                GamePlay.Instance.CheckOutOfMove();
            }
            
            GamePlay.Instance.blockShapeController.CheckAllShapesCanbePlaced();
            Destroy(gameObject);
            //if (GamePlay.Instance.movingKitesList.Count > 0)
            //{
            //}
        }
    }
}

