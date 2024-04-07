using System.Collections.Generic;
using UnityEngine;
using GamingMonks;

public class LinearConveyorBelt : MonoBehaviour
{
    private List<Block> m_blockList1;
    private List<Block> m_blockList2;
    private int m_conveyorWidth = 0;

    /// <summary>
    /// Adds conveyor blocks references
    /// changes blocks sprites
    /// </summary>
    public void SetConveyor(ConveyorLocation location)
    {
        string spriteTag;
        m_conveyorWidth = location.conveyorWidth;

        if (m_conveyorWidth == 1)
        {
            m_blockList1 = new List<Block>();

            if (location.onRow)
            {
                if (location.reverse)
                {
                    List<Block> tempBlocks = GamePlay.Instance.GetEntireRow(location.PositionOnGrid);
                    AddElementsToListInReverse(tempBlocks, m_blockList1);
                    spriteTag = "ConveyorLeft";
                }
                else
                {
                    m_blockList1 = GamePlay.Instance.GetEntireRow(location.PositionOnGrid);
                    spriteTag = "ConveyorRight";
                }
            }
            else
            {
                if (location.reverse)
                {
                    List<Block> tempBlocks = GamePlay.Instance.GetEntirColumn(location.PositionOnGrid);
                    AddElementsToListInReverse(tempBlocks, m_blockList1);
                    spriteTag = "ConveyorUp";
                }
                else
                {
                    m_blockList1 = GamePlay.Instance.GetEntirColumn(location.PositionOnGrid);
                    spriteTag = "ConveyorDown";
                }
            }
            ConveyorBeltController.Instance.conveyorBlocks.AddRange(m_blockList1);
            SetConveyorSprite(m_blockList1, spriteTag);
            return;
        }

        if (m_conveyorWidth == 2)
        {
            m_blockList1 = new List<Block>();
            m_blockList2 = new List<Block>();

            int index = location.PositionOnGrid;

            if (location.onRow)
            {
                if (location.reverse)
                {
                    List<Block> tempBlocks = GamePlay.Instance.GetEntireRow(index);
                    AddElementsToListInReverse(tempBlocks, m_blockList1);

                    tempBlocks = GamePlay.Instance.GetEntireRow(index + 1);
                    AddElementsToListInReverse(tempBlocks, m_blockList2);
                    spriteTag = "ConveyorLeft";
                }
                else
                {
                    m_blockList1 = GamePlay.Instance.GetEntireRow(index);
                    m_blockList2 = GamePlay.Instance.GetEntireRow(index + 1);
                    spriteTag = "ConveyorRight";
                }
            }
            else
            {
                if (location.reverse)
                {
                    List<Block> tempBlocks = GamePlay.Instance.GetEntirColumn(index);
                    AddElementsToListInReverse(tempBlocks, m_blockList1);

                    tempBlocks = GamePlay.Instance.GetEntirColumn(index + 1);
                    AddElementsToListInReverse(tempBlocks, m_blockList2);
                    spriteTag = "ConveyorUp";
                }
                else
                {
                    m_blockList1 = GamePlay.Instance.GetEntirColumn(index);
                    m_blockList2 = GamePlay.Instance.GetEntirColumn(index + 1);
                    spriteTag = "ConveyorDown";
                }
            }
            ConveyorBeltController.Instance.conveyorBlocks.AddRange(m_blockList1);
            ConveyorBeltController.Instance.conveyorBlocks.AddRange(m_blockList2);
            SetConveyorSprite(m_blockList1, spriteTag);
            SetConveyorSprite(m_blockList2, spriteTag);
        }
    }

    private void AddElementsToListInReverse(List<Block> Blocks, List<Block> StoredIn)
    {
        for (int i = Blocks.Count - 1; i >= 0; i--)
        {
            StoredIn.Add(Blocks[i]);
        }
    }

    /// <summary>
    /// Sets conveyor block sprite and Instantiates conveyor Mover blocks
    /// </summary>
    private void SetConveyorSprite(List<Block> Blocks, string _spriteTag)
    {
        foreach (Block block in Blocks)
        {
            block.ChangeConveyorSprite(_spriteTag);
            block.conveyorMoverBlock = GameObject.Instantiate<ConveyorMoverBlock>(ConveyorBeltController.Instance.conveyorMoverBlockPrefab,
                                                                    ConveyorBeltController.Instance.transform);
            block.conveyorMoverBlock.transform.position = block.transform.position;
            block.conveyorMoverBlock.SetBlockReference(block);
            GamePlay.Instance.instantiatedGameObjects.Add(block.conveyorMoverBlock.gameObject);
        }
    }

    public void MoveConveyorBelt()
    {
        if (m_conveyorWidth == 1)
        {
            MoveConveyor(m_blockList1);
        }
        else
        {
            MoveConveyor(m_blockList1);
            MoveConveyor(m_blockList2);
        }
    }

    public void CheckRowOrColoumClear()
    {
        if (m_conveyorWidth == 1)
        {
            CheckForRowOrColumnClear(m_blockList1);
        }
        else
        {
            CheckForRowOrColumnClear(m_blockList1);
            CheckForRowOrColumnClear(m_blockList2);
        }
    }

    /// <summary>
    /// Check for if row / coloum is filled by the conveyor block
    /// </summary>
    private void CheckForRowOrColumnClear(List<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            if (block.isFilled)
            {
                int rowId = block.RowId;
                int columnId = block.ColumnId;
                
                if (block.spriteType == SpriteType.Magnet)
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
    private void MoveConveyor(List<Block> Blocks)
    {
        bool isFilled = false;
        for (int i = Blocks.Count - 1; i >= 0; i--)
        {
            Block block = Blocks[i];
            
            // StartCoroutine(block.HighlightConveyor());
            
            if (i == Blocks.Count - 1)
            {
                if (block.isFilled || block.hasDiamond)
                {
                    isFilled = true;
                    block.conveyorMoverBlock.gameObject.SetActive(true);
                    block.conveyorMoverBlock.SetConveyorBlock();
                    block.ResetBlock();
                }
            }
            else if (i == 0)
            {
                if (block.isFilled || block.hasDiamond)
                {
                    MoveBlock(block, Blocks[i + 1],false);
                }
               
                if (isFilled)
                {
                    MoveBlock(Blocks[Blocks.Count - 1], block, true);
                }
            }
            else
            {
                if (block.isFilled || block.hasDiamond)
                {
                   MoveBlock(block, Blocks[i + 1], false);
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
        if (isConveyorMoverBlockSet)
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
