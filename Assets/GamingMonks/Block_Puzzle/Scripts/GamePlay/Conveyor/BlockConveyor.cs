using System.Collections.Generic;
using UnityEngine;
using GamingMonks;
using System.Threading.Tasks;

public class BlockConveyor
{
    // conveyor present on this block
    public Block conveyorBlock { get; private set; }

    // next block reference - on which block conveyor moves filled block
    private Block m_nextBlock;

    private ConveyorType m_conveyorType;
    private int m_rowId = 0;
    private int m_columId = 0;
    public bool hasMoved { get; private set; }

   /// <summary>
   /// Sets the properties of conveyor block 
   /// </summary>
   /// <param name="blockConveyor"></param>
    public void SetBlockConveyor(BlockConveyors blockConveyor)
    {
        m_rowId = (int)blockConveyor.position.x;
        m_columId = (int)blockConveyor.position.y;
        m_conveyorType = blockConveyor.conveyorType;

        switch(m_conveyorType)
        {
            case ConveyorType.Right:
                SetBlockReferenceFromRow(m_rowId, true);
                break;

            case ConveyorType.Left:
                SetBlockReferenceFromRow(m_rowId, false);
                break;

            case ConveyorType.Up:
                SetBlockReferenceFromColum(m_columId, false);
                break;

            case ConveyorType.Down:
                SetBlockReferenceFromColum(m_columId, true);
                break;
        }

        SetConveyorBlockSprite();
        SpawnMoverBlock();
        ConveyorBeltController.Instance.conveyorBlocks.Add(conveyorBlock);
    }

    /// <summary>
    /// Moves the block on the m_nextBlock if it is filled
    /// </summary>
    public void MoveBlock()
    {
        if(!hasMoved && CanMove())
        {
            hasMoved = true;
            conveyorBlock.conveyorMoverBlock.gameObject.SetActive(true);
            conveyorBlock.conveyorMoverBlock.SetConveyorBlock();
            conveyorBlock.conveyorMoverBlock.Move(m_nextBlock);
            SetHasMoved(false);
        }
    }

    /// <summary>
    /// Checks if any row or coloum is filled by the conveyor blocks after moving
    /// </summary>
    public void CheckForRowOrColumClear()
    {
        if (conveyorBlock.isFilled)
        {
            if (conveyorBlock.spriteType == SpriteType.Magnet)
            {
                Magnet.Instance.CheckForRowOrColoumClear(m_rowId, m_columId);
            }

            if (!ConveyorBeltController.Instance.completedRows.Contains(m_rowId))
            {
                if (GamePlay.Instance.IsRowCompleted(m_rowId))
                {
                    ConveyorBeltController.Instance.completedRows.Add(m_rowId);
                }
            }

            if (!ConveyorBeltController.Instance.completedColumns.Contains(m_columId))
            {
                if (GamePlay.Instance.IsColumnCompleted(m_columId))
                {
                    ConveyorBeltController.Instance.completedColumns.Add(m_columId);
                }
            }
        }
    }

    /// <summary>
    /// Checks if block conveyor can move
    /// </summary>
    /// <returns> true/false </returns>
    private bool CanMove()
    {
        return (conveyorBlock.isFilled && !m_nextBlock.isFilled);
    }

    /// <summary>
    /// Changes Blocks sprite with the conveyor sprite
    /// </summary>
    private void SetConveyorBlockSprite()
    {
        conveyorBlock.ChangeConveyorSprite(GetSpriteTag());
    }

    /// <summary>
    /// Instantiate Conveyor Mover block
    /// </summary>
    private void SpawnMoverBlock()
    {
        ConveyorMoverBlock moverBlock = GameObject.Instantiate<ConveyorMoverBlock>(ConveyorBeltController.Instance.conveyorMoverBlockPrefab,
                                                                    ConveyorBeltController.Instance.transform);
        moverBlock.transform.position = conveyorBlock.transform.position;
        //moverBlock.m_block = conveyorBlock;
        moverBlock.SetBlockReference(conveyorBlock);
        conveyorBlock.conveyorMoverBlock = moverBlock;
        GamePlay.Instance.instantiatedGameObjects.Add(moverBlock.gameObject);
    }

    /// <summary>
    /// Set references of the conveyorBlock and m_nextBlock
    /// </summary>
    /// <param name="rowId"> conveyor block rowId </param>
    /// <param name="isNextBlock"> true - next block / false - first block</param>
    private void SetBlockReferenceFromRow(int rowId, bool isNextBlock)
    {
        List<Block> blocks = GamePlay.Instance.GetEntireRow(rowId);
        for(int i = 0; i < blocks.Count; i++)
        {
            Block b = blocks[i];
            if (b.ColumnId == m_columId && b.RowId == m_rowId)
            {
                conveyorBlock = b;
                if(isNextBlock && (i+1) < blocks.Count)
                {
                    m_nextBlock = blocks[i + 1];
                    break;
                }
                else
                {
                    m_nextBlock = blocks[i - 1];
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Set references of the conveyorBlock and m_nextBlock
    /// </summary>
    /// <param name="columId"> conveyor block columId </param>
    /// <param name="isNextBlock">true - next block / false - first block </param>
    private void SetBlockReferenceFromColum(int columId, bool isNextBlock)
    {
        List<Block> blocks = GamePlay.Instance.GetEntirColumn(columId);
        for (int i = 0; i < blocks.Count; i++)
        {
            Block b = blocks[i];
            if (b.ColumnId == m_columId && b.RowId == m_rowId)
            {
                conveyorBlock = b;
                if (isNextBlock && (i + 1) < blocks.Count)
                {
                    m_nextBlock = blocks[i + 1];
                    break;
                }
                else
                {
                    m_nextBlock = blocks[i - 1];
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Set spriteTag base on conveyor type
    /// </summary>
    /// <returns> spriteTag </returns>
    private string GetSpriteTag()
    {
        string spriteTag = "";
        switch(m_conveyorType)
        {
            case ConveyorType.Right :
                spriteTag = "ConveyorRight";
                break;

            case ConveyorType.Left:
                spriteTag = "ConveyorLeft";
                break;

            case ConveyorType.Up:
                spriteTag = "ConveyorUp";
                break;

            case ConveyorType.Down:
                spriteTag = "ConveyorDown";
                break;
        }
        return spriteTag;
    }

    private async void SetHasMoved(bool status)
    {
        await Task.Delay(System.TimeSpan.FromSeconds(0.3));
        hasMoved = status;
    }
}
