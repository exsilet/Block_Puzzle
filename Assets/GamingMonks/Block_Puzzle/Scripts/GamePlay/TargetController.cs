using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamingMonks;
using GamingMonks.UITween;
using System.Collections;
using UnityEngine.SceneManagement;

public class TargetController : Singleton<TargetController>
{
    [SerializeField] private Target targetPrefab;
    [SerializeField] private GameObject destroyingBlockPrefab;
    [SerializeField] public Transform particleParent;
    [SerializeField] private Transform destroyingBlockParent;
    [SerializeField] private ParticleSystem m_targetCollectEffect;
    [SerializeField] private ParticleSystem m_bubbleTargetCollectEffect;
    public bool canCollectAnyEmptyBlock = false;
    public ParticleSystem particle;
    public int totalRocketGlobal = 0;
    public bool isAllTargetsCollected = false;

    private List<Target> targets = new List<Target>();
    private List<Target> completedTargets = new List<Target>();

    public void InitializeTargets(LevelGoal[] goals)
    {
        for (int i = 0; i < goals.Length; i++)
        {
            Target target = Instantiate<Target>(targetPrefab, gameObject.transform);
            target.Initialize(goals[i].target, goals[i].spriteType);
            targets.Add(target);
            if (goals[i].spriteType == SpriteType.AllColourBlock)
            {
                canCollectAnyEmptyBlock = true;
            }
        }
    }

    public void UpdateTarget(Transform block, SpriteType spriteType)
    {
        foreach(Target target in targets)
        {
            if(spriteType == target.spriteType)
            {
                if (GamePlay.Instance.jewelMachineEnabled)
                {
                   target.UpdateTargetCount();
                }
                
                if (targets.Count <= 0)
                {
                    // When we're about to win the game disable touche to avoid unwanted actions.
                    isAllTargetsCollected = true;
                    InputManager.Instance.DisableTouch();                  
                    //GamePlayUI.Instance.isGameWon = true;
                }
                
                SpawnDestroyingBlock(block.gameObject.transform, target, spriteType);
                break;
            }
        }
    }

    public bool IstargetsRemaining()
    {
        if (targets.Count > 0)
        {
            return true;
        }
        return false;
    }

    public int GetRemainingTarget(SpriteType spriteType)
    {
        foreach (Target target in targets)
        {
            if (spriteType == target.spriteType)
            {
               return target.GetRemainingTarget();
            }
        }
        return -1;
    }

    public void AddCompletedTarget(SpriteType spriteType)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            if(targets[i].spriteType == spriteType)
            {
                completedTargets.Add(targets[i]);
                targets.RemoveAt(i);
                i--;
            }
        }
    }

    private void SpawnDestroyingBlock(Transform destroyingBlock, Target target, SpriteType spriteType)
    {
        GameObject targetInstance = null;
        switch (spriteType)
        {
            case SpriteType.Bubble:
                //targetInstance = Instantiate(m_targetCollectEffect, destroyingBlock.position, Quaternion.identity, destroyingBlockParent).gameObject;
                targetInstance = Instantiate(m_bubbleTargetCollectEffect, destroyingBlock.position, Quaternion.identity, destroyingBlockParent).gameObject;
                //Move(targetInstance.transform, target);
                StartCoroutine(MoveTowardsTarget(targetInstance.transform, target));
                break;

            case SpriteType.Hat:
                targetInstance = Instantiate(destroyingBlockPrefab, destroyingBlock.position, Quaternion.identity, destroyingBlockParent);
                targetInstance.GetComponent<Image>().sprite = target.image.sprite;
                targetInstance.transform.localScale = Vector3.zero;
                //targetInstance.transform.localScale = Vector3.one * 1.5f;
                Animator animator = targetInstance.GetComponent<Animator>();
                animator.enabled = true;
                animator.SetTrigger("Fly");
                targetInstance.transform.LocalScale(Vector3.one * 1.5F, 0.5F).SetEase(Ease.EaseOut).OnComplete(() =>
                {
                    Move(targetInstance.transform, target);
                });
                break;

            case SpriteType.Ice or SpriteType.AllColourBlock or SpriteType.Red or SpriteType.Yellow or SpriteType.Blue or 
                    SpriteType.Green or SpriteType.Cyan or SpriteType.Purple or SpriteType.Pink :
                targetInstance = Instantiate(m_targetCollectEffect, destroyingBlock.position, Quaternion.identity, particleParent).gameObject;
                Move(targetInstance.transform, target);
                break;

            default:
                targetInstance = Instantiate(destroyingBlockPrefab, destroyingBlock.position, Quaternion.identity, destroyingBlockParent);
                targetInstance.GetComponent<Image>().sprite = target.image.sprite;
                targetInstance.transform.LocalScale(Vector3.one * 1.3f, .2F).SetEase(Ease.EaseOut).OnComplete(() =>
                {
                    //StartCoroutine(MoveTowardsTarget(targetInstance.transform, target));
                    Move(targetInstance.transform, target);
                });
                break;
        }
    }

     IEnumerator OnWinCoroutine()
     {
        if(!GamePlayUI.Instance.isGameWon)
        {
            GamePlayUI.Instance.isGameWon = true;
            yield return new WaitForSeconds(0.8f);
            GamePlayUI.Instance.OnGameWin();
            InputManager.Instance.EnableTouch();
        }
        yield return null;
     }

    private IEnumerator MoveTowardsTarget(Transform destroyingBlock, Target target)
    {
        /*
        float timeToMove = 0.5f;
        float timeElapced = 0;
        Vector3 initialPosition = destroyingBlock.position;
        while (timeElapced < timeToMove)
        {
            yield return null;
            timeElapced += Time.deltaTime;
            destroyingBlock.position = Vector3.Lerp(initialPosition, target.transform.position, timeElapced / timeToMove);
        }
        */

        float speed = 3f;
        Vector3 direction = (target.transform.position - destroyingBlock.position).normalized;
        float distance = 1;
        while (distance > 0.8f)
        {
            yield return null;
            distance = Vector3.Distance(destroyingBlock.position, target.transform.position);
            destroyingBlock.Translate(direction * Time.deltaTime * speed);
        }

        if (!GamePlay.Instance.jewelMachineEnabled)
        {
            target.UpdateTargetCount();
        }
        target.PlayTargetShakeEffect();
        Destroy(destroyingBlock.gameObject);
        if (targets.Count <= 0)
        {
            isAllTargetsCollected = true;
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "LimitedMoves")
            {
                StartCoroutine(SpawnRockets());
            }
            else
            {
                StartCoroutine(OnWinCoroutine());
            }
            //StartCoroutine(OnWinCoroutine());
        }
    }

    private void Move(Transform destroyingBlock, Target target)
    {
        if (!GamePlay.Instance.jewelMachineEnabled)
        {
            target.UpdateTargetCount();
        }
        if (targets.Count <= 0)
        {
            isAllTargetsCollected = true;
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "LimitedMoves")
            {
                StartCoroutine(SpawnRockets());
            }
            else
            {
                StartCoroutine(OnWinCoroutine());
            }
            //StartCoroutine(OnWinCoroutine());
        }
        destroyingBlock.Position(target.gameObject.transform.position, 0.8F).SetEase(Ease.EaseIn).OnComplete(() =>
        {
            target.PlayTargetShakeEffect();
            Destroy(destroyingBlock.gameObject);
        });
    }

    IEnumerator SpawnRockets()
    {
        for (int i = 0; i < GamePlay.Instance.allShapes.Count; i++)
        {
           
            int shapeCount = GamePlay.Instance.allShapes[i].Count;
            int counter = 0;
            for (int j = 0; j < GamePlay.Instance.allShapes[i].Count; j++)
            {
                if (!GamePlay.Instance.allShapes[i][j].isFilled)
                {
                    counter++;
                }
            }
            if (counter == shapeCount)
            {
                GamePlay.Instance.allShapes.RemoveAt(i);
                i--;
            }
        }

        

        int totalRockets = GamePlay.Instance.allShapes.Count;
        Debug.Log("All Shape:" + GamePlay.Instance.allShapes.Count);
        if (totalRockets > 10)
        {
            totalRockets = 10;
        }
        if (totalRockets > 0 || GamePlay.Instance.GetFilledBlockForIceMachine() != null)
        {
            
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);
            UIController.Instance.movesCountTxt.gameObject.SetActive(false);
            UIController.Instance.movesText.gameObject.SetActive(false);
            UIController.Instance.boomrangHUD.SetActive(true);
            InputManager.Instance.DisableTouch();

            for (int i = 0; i < totalRockets; i++)
            {
                GameObject rocket = Instantiate(GamePlay.Instance.rocket, UIController.Instance.boomrangHUD.transform.position + new Vector3(0, -0.15f, 0), GamePlay.Instance.rocket.transform.rotation,
                                    GamePlay.Instance.instantiatedGameObjectsParent);
                GamePlay.Instance.instantiatedGameObjects.Add(rocket);
                GamePlay.Instance.movingRocketsList.Add(rocket);
                rocket.transform.localScale = Vector3.one;
                yield return waitForSeconds;
            }

            if (totalRockets <= 2)
            {
                totalRocketGlobal = 2 - totalRockets;
                for (int i = 0; i < 2 - totalRockets; i++)
                {
                    GameObject rocket = Instantiate(GamePlay.Instance.rocket, UIController.Instance.boomrangHUD.transform.position + new Vector3(0, -0.15f, 0), GamePlay.Instance.rocket.transform.rotation,
                                        GamePlay.Instance.instantiatedGameObjectsParent);
                    GamePlay.Instance.instantiatedGameObjects.Add(rocket);
                    GamePlay.Instance.movingRocketsList.Add(rocket);
                    rocket.transform.localScale = Vector3.one;
                    yield return waitForSeconds;
                }
            }
            StartCoroutine(UIController.Instance.DeactivateBoomRangHUD());
            yield return new WaitForSeconds(2.6f);
            StartCoroutine(OnWinCoroutine());
        }
        else
        {
            StartCoroutine(OnWinCoroutine());
        }
    }

    public void DestroyTargetsOnReloadLevel()
    {
        if(targets != null)
        {
            foreach (Target target in targets)
            {
                Destroy(target.gameObject);
            }
            targets.Clear();
        }

        if(completedTargets != null)
        {
            foreach (Target target in completedTargets)
            {
                Destroy(target.gameObject);
            }
            completedTargets.Clear();
        }

        isAllTargetsCollected = false;
    }
}
