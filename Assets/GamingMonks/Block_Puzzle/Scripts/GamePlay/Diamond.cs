using System.Collections;
using System.Collections.Generic;
using GamingMonks;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    private bool canDrop;
    public Block currentBlock = null;
    public Block belowBlock = null;
    private int rowSize;

    //private void OnEnable()
    //{
    //    GamePlayUI.Instance.OnBlockDestroyEvent += CheckDiamondCanBeDrop;
    //}
    //private void OnDisable()
    //{
    //    GamePlayUI.Instance.OnBlockDestroyEvent -= CheckDiamondCanBeDrop;
    //}

    private void Awake()
    {
        transform.localScale = Vector3.one * ((0.88f * GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize) / 90);
    }

    private void Start()
    {
        BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();
        rowSize = (int)boardSize;
        if (GamePlay.Instance.blockers.Contains(currentBlock))
        {
            GamePlay.Instance.blockers.Remove(currentBlock);
        }
    }

    private void Update()
    {
        if (!GamePlay.Instance.isBoardReady)
        {
            return;
        }
        else
        {
            List<Block> lowerRow = null;
            if (currentBlock != null)
            {
                lowerRow = GamePlay.Instance.allRows[currentBlock.RowId + 1];
            }

            Block lowerBlock = null;
            if(lowerRow != null)
                lowerBlock = lowerRow[currentBlock.ColumnId];

            if (lowerBlock != null)
            {
                belowBlock = lowerBlock;
                if (lowerBlock.isFilled || lowerBlock.hasBlockerStick || lowerBlock.hasDiamond)
                {
                    if (GamePlay.Instance.blockers.Contains(lowerBlock))
                        return;
                    GamePlay.Instance.blockers.Add(lowerBlock);
                }
                else
                {
                    StartCoroutine(Drop(lowerBlock));
                }
            }
        }
    }

    public void Initialize(Block block)
    {
        currentBlock = block;
        PlaceDiamond();
    }
    
    // private void CheckDiamondCanBeDrop()
    // {
    //     StartCoroutine(Drop());
    // }

    private IEnumerator Drop(Block belowDiamondBlock)
    {
        // yield return new WaitForSeconds(.2f);
        // var row = GamePlay.Instance.allRows[currentBlock.RowId+1];
        // Block block = row[currentBlock.ColumnId];
        // if (block.isFilled)
        // {
        //     yield break;
        // }
        if (GamePlay.Instance.blockers.Contains(belowDiamondBlock))
        {
            GamePlay.Instance.blockers.Remove(belowDiamondBlock);
        }
        
        float time = 0;
        while (time < 0.20f)
        {
            yield return new WaitForSeconds(0.01f);
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, belowDiamondBlock.transform.position, time);
        }
        
        ClearDiamond();
        currentBlock = belowDiamondBlock;
        PlaceDiamond();
        

        if (currentBlock.RowId == rowSize-1)
        {
            TargetController.Instance.UpdateTarget(currentBlock.transform, SpriteType.Diamond);
            ClearDiamond();
            Destroy(gameObject);
            yield break;
        }

        // StartCoroutine(Drop());
    }
    
    public void PlaceDiamond()
    {
        currentBlock.thisCollider.enabled = false;
        currentBlock.isFilled = false;
        currentBlock.isAvailable = false; 
        currentBlock.hasDiamond = true;
        currentBlock.diamond = this;
        transform.position = currentBlock.transform.position;
        currentBlock.assignedSpriteTag = SpriteType.Diamond.ToString();
        currentBlock.SetBlockSpriteType(SpriteType.Diamond);
    }

    public void ClearDiamond()
    {
        currentBlock.thisCollider.enabled = true;
        currentBlock.isFilled = false;
        currentBlock.isAvailable = true; 
        currentBlock.hasDiamond = false;
        currentBlock.diamond = null;
        currentBlock.assignedSpriteTag = currentBlock.defaultSpriteTag;
        currentBlock.SetBlockSpriteType(SpriteType.Empty);
        
        if (GamePlay.Instance.blockers.Contains(belowBlock))
        {
            GamePlay.Instance.blockers.Remove(belowBlock);
        }
    }
}