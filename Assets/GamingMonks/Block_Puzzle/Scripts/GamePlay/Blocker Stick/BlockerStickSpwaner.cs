using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamingMonks;
using GamingMonks.Utils;

public class BlockerStickSpwaner : Singleton<BlockerStickSpwaner>
{
    public BlockerStick VerticalBlockerStickPrefab;
    public BlockerStick HorizontalBlockerStickPrefab;

    private List<BlockerStick> verticalBlockerSticks = new List<BlockerStick>();
    private List<BlockerStick> horizontalBlockerSticks = new List<BlockerStick>();

    private float blockHeight;

    public void SpawnBlockerStick()
    {
        GamePlay.Instance.SetBlockerStickStatus(true);

        BlockerSticks blockerStick = GamePlayUI.Instance.currentLevel.BlockerStick;
        blockHeight = GamePlayUI.Instance.currentLevel.Mode.GameModeSettings.blockSize;
        if(blockerStick.enabled)
        {
            Stick[] sticks = blockerStick.Stick;
            for(int i = 0; i < sticks.Length; i++)
            {
                if (sticks[i].onVertical)
                {
                    SpawnVerticalBlockerStick(sticks[i]);
                }

                if(sticks[i].onHorrizontal)
                {
                    SpawnHorizontalBlockerStick(sticks[i]);
                }
            }
        }
    }

    private void SpawnVerticalBlockerStick(Stick stick)
    {
        int x = (int)stick.startingPosition.x;
        int y = (int)stick.startingPosition.y;
        List<Block> blocks = GamePlay.Instance.GetEntirColumn(y);
        List<Block> nextBlocks = GamePlay.Instance.GetEntirColumn(y + 1);
        int counter = 0;

        for (int i = x; i < blocks.Count; i++)
        {
            if (counter < stick.length)
            {
                Vector3 pos = (blocks[i].transform.position + nextBlocks[i].transform.position) / 2;
                BlockerStick verticalStick = Instantiate(VerticalBlockerStickPrefab, pos, VerticalBlockerStickPrefab.transform.rotation, transform);
                verticalStick.GetComponent<RectTransform>().SetNewWidth(blockHeight + 15);
                verticalStick.rowId = blocks[i].RowId;
                verticalBlockerSticks.Add(verticalStick);
               
                counter++;
                GamePlay.Instance.instantiatedGameObjects.Add(verticalStick.gameObject);
                continue;
            }
            break;
        }
    }

    private void SpawnHorizontalBlockerStick(Stick stick)
    {
        int x = (int)stick.startingPosition.x;
        int y = (int)stick.startingPosition.y;
        List<Block> blocks = GamePlay.Instance.GetEntireRow(x);
        int counter = 0;
        
        for (int i = y; i < blocks.Count; i++)
        {
            if (counter < stick.length)
            {
                Vector3 pos = (blocks[i].transform.position);
                BlockerStick horizontalStick = Instantiate(HorizontalBlockerStickPrefab, pos, Quaternion.identity, transform);
                horizontalStick.GetComponent<RectTransform>().SetNewWidth(blockHeight + 15);
                horizontalStick.transform.localPosition += new Vector3(0,blockHeight / 2 , 0);
                horizontalStick.coloumId = blocks[i].ColumnId;
                horizontalBlockerSticks.Add(horizontalStick);
                horizontalStick.stickBlock = blocks[i];
                blocks[i].hasBlockerStick = true;
                counter++;
                GamePlay.Instance.instantiatedGameObjects.Add(horizontalStick.gameObject);
                continue;
            }
            break;
        }
    }

    public IEnumerator DestroyVerticalBlockerStick(int rowId)
    {
        yield return new WaitForSeconds(0.1f);

        if (verticalBlockerSticks.Count > 0)
        {
            for(int i = 0; i < verticalBlockerSticks.Count; i++)
            {
                if (verticalBlockerSticks[i].rowId == rowId)
                {
                    GamePlay.Instance.instantiatedGameObjects.Remove(verticalBlockerSticks[i].gameObject);
                    Destroy(verticalBlockerSticks[i].gameObject);
                    verticalBlockerSticks.RemoveAt(i);
                    i -= 1; 
                }
            }
        }
    }

    public IEnumerator DestroyHorizontalBlockerStick(int columnId)
    {
        yield return new WaitForSeconds(0.1f);

        if (horizontalBlockerSticks.Count > 0)
        {
            for (int i = 0; i < horizontalBlockerSticks.Count; i++)
            {
                if (horizontalBlockerSticks[i].coloumId == columnId)
                {
                    GamePlay.Instance.instantiatedGameObjects.Remove(horizontalBlockerSticks[i].gameObject);
                    horizontalBlockerSticks[i].stickBlock.hasBlockerStick = false;
                    Destroy(horizontalBlockerSticks[i].gameObject);
                    horizontalBlockerSticks.RemoveAt(i);
                    i -= 1;
                }
            }
        }
    }

    public void ResetBlockerStick()
    {
        GamePlay.Instance.SetBlockerStickStatus(false);

        foreach(BlockerStick blockerStick in verticalBlockerSticks)
        {
            Destroy(blockerStick.gameObject);
        }

        foreach (BlockerStick blockerStick in horizontalBlockerSticks)
        {
            Destroy(blockerStick.gameObject);
        }

        verticalBlockerSticks.Clear();
        horizontalBlockerSticks.Clear();
    }
}
