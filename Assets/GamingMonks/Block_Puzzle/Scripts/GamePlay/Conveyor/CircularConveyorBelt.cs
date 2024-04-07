using System.Collections.Generic;
using UnityEngine;
using GamingMonks;

public class CircularConveyorBelt : MonoBehaviour
{
    // list of conveyor blocks 
    private List<Block> m_blockList = new List<Block>();


    /// <summary>
    /// Adds conveyor blocks references
    /// changes blocks sprites
    /// </summary>
    /// <param name="circularConveyor"></param>
    public void SetConveyor(CircularConveyor circularConveyor)
    {
        
        int x = (int)circularConveyor.startPosition.x;
        int y = (int)circularConveyor.startPosition.y;

        List<Block> tempList = GamePlay.Instance.GetEntireRow(x);
        string spriteTag = "ConveyorRight";
        for (int i = y; i < circularConveyor.length + y - 1; i++)
        {
            m_blockList.Add(tempList[i]);
            tempList[i].ChangeConveyorSprite(spriteTag);
        }

        //tempList.Clear();
        tempList = GamePlay.Instance.GetEntirColumn(circularConveyor.length + y - 1);
        spriteTag = "ConveyorDown";
        for (int i = x ; i < circularConveyor.height + x - 1; i++)
        {
            m_blockList.Add(tempList[i]);
            tempList[i].ChangeConveyorSprite(spriteTag);
        }

        //tempList.Clear();
        tempList = GamePlay.Instance.GetEntireRow(x + circularConveyor.height - 1);
        spriteTag = "ConveyorLeft";
        for (int i = y + circularConveyor.length - 1; i > y; i--)
        {
            m_blockList.Add(tempList[i]);
            tempList[i].ChangeConveyorSprite(spriteTag);
        }
      
        tempList = GamePlay.Instance.GetEntirColumn(y);
        spriteTag = "ConveyorUp";
        for (int i = x + circularConveyor.height - 1; i > x; i--)
        {
            m_blockList.Add(tempList[i]);
            tempList[i].ChangeConveyorSprite(spriteTag);
        }
        
        ConveyorBeltController.Instance.conveyorBlocks.AddRange(m_blockList);
        SpawnMoverBlock();
    }
   
    /// <summary>
    /// Instantiates conveyor mover blocks
    /// </summary>
    private void SpawnMoverBlock()
    {
        foreach (Block block in m_blockList)
        {
            block.conveyorMoverBlock = GameObject.Instantiate<ConveyorMoverBlock>(ConveyorBeltController.Instance.conveyorMoverBlockPrefab, ConveyorBeltController.Instance.transform);
            block.conveyorMoverBlock.transform.position = block.transform.position;
            block.conveyorMoverBlock.SetBlockReference(block);
        }
    }

    /// <summary>
    /// Check for if row / coloum is filled by the conveyor block
    /// </summary>
    public void CheckForRowOrColoumClear()
    {
        foreach (Block block in m_blockList)
        {
            if (block.isFilled)
            {
                int rowId = block.RowId;
                int columnId = block.ColumnId;
                
                if (block.spriteType is SpriteType.Magnet or SpriteType.MagnetWithBubble )
                {
                    Magnet.Instance.CheckForRowOrColoumClear(rowId, columnId);
                }
              
                if (!ConveyorBeltController.Instance.completedRows.Contains(rowId))
                {
                    if (GamePlay.Instance.IsRowCompleted(rowId))
                    {
                        ConveyorBeltController.Instance.completedRows.Add(rowId);
                    }
                }

                if (!ConveyorBeltController.Instance.completedColumns.Contains(columnId))
                {
                    if (GamePlay.Instance.IsColumnCompleted(columnId))
                    {
                        ConveyorBeltController.Instance.completedColumns.Add(columnId);
                    }
                }

            }
        }
    }

    /// <summary>
    /// Moves Conveyor
    /// </summary>
    public void MoveConveyor()
    {
        bool isFilled = false;
        for (int i = m_blockList.Count - 1; i >= 0; i--)
        {
            Block block = m_blockList[i];
            // StartCoroutine(block.HighlightConveyor());
            if (i == m_blockList.Count - 1)
            {
                if (block.isFilled)
                {
                    isFilled = true;
                    block.conveyorMoverBlock.gameObject.SetActive(true);
                    block.conveyorMoverBlock.SetConveyorBlock();
                    block.ResetBlock();
                }
            }
            else if (i == 0)
            {
                if (block.isFilled)
                {
                    MoveBlock(block, m_blockList[i + 1], false);
                }
                
                if(isFilled)
                {
                    MoveBlock(m_blockList[m_blockList.Count - 1], block, true);
                }
            }
            else
            {
                if (block.isFilled)
                {
                    MoveBlock(block, m_blockList[i + 1], false);
                }
            }
        }
    }

    /// <summary>
    /// Moves moving-block to the destination-block
    /// </summary>
    /// <param name="movingBlock"> moving block </param>
    /// <param name="destinationBlock"> on this moving block will place </param>
    /// <param name="isConveyorMoverBlockSet"> mover block already set</param>
    private void MoveBlock(Block movingBlock, Block destinationBlock, bool isConveyorMoverBlockSet)
    {
        if(isConveyorMoverBlockSet)
        {
            StartCoroutine(movingBlock.conveyorMoverBlock.MoveBlock(destinationBlock));
        }
        else
        {
            movingBlock.conveyorMoverBlock.gameObject.SetActive(true);
            movingBlock.conveyorMoverBlock.SetConveyorBlock();
            StartCoroutine(movingBlock.conveyorMoverBlock.MoveBlock(destinationBlock));
        }
    }
}
