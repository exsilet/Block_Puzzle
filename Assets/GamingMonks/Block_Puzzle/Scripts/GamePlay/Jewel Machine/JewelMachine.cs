using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using GamingMonks;

public class JewelMachine : MonoBehaviour
{
    // list of blocks present in this jewel machine 
    private List<Block> m_blocks;

    // jewel counter value  
    private int m_jewelCounter = 0;

    // jewel machine block position
    private Vector3 m_machinePosition;

    // delay for the coroutine
    private WaitForSeconds m_waitForToMoveBlocks;
    private WaitForSeconds m_waitForToFillBlocks;
    [SerializeField] private GameObject gemUnderMachine;
    /// <summary>
    /// creates delay objects for the coroutine 
    /// </summary>
    private void OnEnable()
    {
        m_waitForToMoveBlocks = new WaitForSeconds(0.6f);
        m_waitForToFillBlocks = new WaitForSeconds(0.2f);
    }


    public void Initialize(int _coloumId, int machineTarget, int _jewelCounter)
    {
        m_jewelCounter = _jewelCounter;
        m_blocks = new List<Block>();
        m_blocks = GamePlay.Instance.GetEntirColumn(_coloumId);
        m_machinePosition = m_blocks[0].transform.position;
        m_blocks.RemoveAt(0);
        StartMachine(machineTarget);
    }
  
    private void StartMachine(int gemsTarget)
    {
        SpawnMoverBlock();
        InitializeMoverBlock(gemsTarget);
        PlaceMoverBlockOnBlocks();
    }

    /// <summary>
    /// set mover block with gems and simple blocks 
    /// </summary>
    /// <param name="gemsTarget"></param>
    private void InitializeMoverBlock(int gemsTarget)
    {
        int counter = 0;
        while (counter < gemsTarget)
        {
            int randomNumber = Random.Range(0, m_blocks.Count - 1);
            if (!m_blocks[randomNumber].isFilled && !m_blocks[randomNumber].jewelMoverBlock.isFilled)
            {
                m_blocks[randomNumber].jewelMoverBlock.PlaceJewel(m_jewelCounter, m_machinePosition);
            }
            counter++;
        }

        for (int i = 0; i < m_blocks.Count; i++)
        {
            if (!m_blocks[i].jewelMoverBlock.isFilled && !m_blocks[i].isFilled)
            {
                m_blocks[i].jewelMoverBlock.PlaceBlock(m_machinePosition);
            }
        }
    }

    /// <summary>
    /// Place the Mover Block on the block
    /// </summary>
    private async void PlaceMoverBlockOnBlocks()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(0.3f));
        InputManager.Instance.DisableTouch();
        for (int i = m_blocks.Count - 1; i >= 0; i--)
        {
            if(!m_blocks[i].isFilled)
            {
                m_blocks[i].jewelMoverBlock.SetDestination(m_blocks[i]);
                m_blocks[i].jewelMoverBlock.gameObject.SetActive(true);
                await Task.Delay(System.TimeSpan.FromSeconds(0.10f));
            }
        }
        InputManager.Instance.EnableTouch();
        JewelMachineController.Instance.isJewelMachineHasInitialized = true;
    }

    /// <summary>
    /// Instantiate Mover block 
    /// Sets the Mover-block reference in Block.cs and Block reference in  JewelMoverBlock.cs
    /// </summary>
    private void SpawnMoverBlock()
    {
        foreach(Block block in m_blocks)
        {
            JewelMoverBlock moverBlock = Instantiate(JewelMachineController.Instance.jewelMoverBlockPrefab, JewelMachineController.Instance.transform);
            moverBlock.SetReferenceBlock(block);
            moverBlock.transform.position = block.transform.position;
            block.jewelMoverBlock = moverBlock;
            //gemUnderMachine.transform.LocalScale(new Vector3(0.2F, 0.2F, 0.2F), 0.5F).OnComplete(() => {
                //gemUnderMachine.transform.LocalScale(Vector3.one, 0.5F);
            //});
                //GamePlay.Instance.instantiatedGameObjects.Add(moverBlock.gameObject);
        }
    }

    /// <summary>
    /// Returns total gems present in the coloum
    /// </summary>
    /// <returns></returns>
    public int GetPresentGemCount()
    {
        int gemPresent = 0;
        foreach (Block block in m_blocks)
        {
            if (block.hasJewel || (block.jewelMoverBlock != null && block.jewelMoverBlock.m_hasJewel))
            {
                gemPresent++;
            }
        }
        return gemPresent;
    }

    public IEnumerator FillBlocks()
    {
        //MoveAllBlocks();
        //yield return m_waitForToMoveBlocks;
        yield return StartCoroutine(FillEmptyBlock());
    }

    public IEnumerator FillGem()
    {
        //MoveAllBlocks();
        //yield return m_waitForToMoveBlocks;
        yield return StartCoroutine(FillGemAtEmptyBlock());
    }

    /// <summary>
    /// Moves all the block downward
    /// </summary>
    public void MoveAllBlocks()
    {
        int j = GetEmptyBlockIndex();
        int i = GetFilledBlockAfterEmpty(j);
        
        while( i >= 0 && j > 0)
        {
            if(m_blocks[i].isFilled && !m_blocks[i].isAvailable && !m_blocks[j].isFilled)
            {
                MoveBlock(m_blocks[i], m_blocks[j]);
            }
            i--;
            j--;
        }
    }

    /// <summary>
    /// Returns filled block index after empty block
    /// </summary>
    /// <param name="emptyBlockIndex"> </param>
    /// <returns></returns>
    private int GetFilledBlockAfterEmpty(int emptyBlockIndex)
    {
        for(int i = emptyBlockIndex-1; i >= 0; i--)
        {
            if(m_blocks[i].isFilled && !m_blocks[i].isAvailable)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Fills the Gem at the empty block
    /// </summary>
    private IEnumerator  FillGemAtEmptyBlock()
    {
        if (JewelMachineController.Instance.CanFillGems())
        {
            int index = GetEmptyBlockIndex();
            if (index > -1)
            {
                if (!m_blocks[index].isFilled)
                {
                    m_blocks[index].jewelMoverBlock.PlaceJewel(m_jewelCounter, m_machinePosition);
                    m_blocks[index].jewelMoverBlock.SetDestination(m_blocks[index]);
                    m_blocks[index].jewelMoverBlock.gameObject.SetActive(true);

                    yield return m_waitForToFillBlocks;
                }
            }
        }
    }

    /// <summary>
    /// Fills all the empty block by simple block
    /// </summary>
    /// <returns></returns>
    public IEnumerator FillEmptyBlock()
    {
        int index = GetEmptyBlockIndex();
        if (index > -1)
        {
            for (int i = index; i >= 0; i--)
            {
                if (!m_blocks[i].isFilled)
                {
                    m_blocks[i].jewelMoverBlock.PlaceBlock(m_machinePosition);
                    m_blocks[i].jewelMoverBlock.SetDestination(m_blocks[i]);
                    m_blocks[i].jewelMoverBlock.gameObject.SetActive(true);

                    yield return m_waitForToFillBlocks;
                }
            }
        }
    }

    /// <summary>
    /// Retuns empty block index from the jewel machine coloum
    /// </summary>
    /// <returns></returns>
    public int GetEmptyBlockIndex()
    {
        for (int i = m_blocks.Count - 1; i >= 0; i--)
        {
            if (!m_blocks[i].isFilled && m_blocks[i].isAvailable && m_blocks[i].jewelMoverBlock != null && !m_blocks[i].jewelMoverBlock.isFilled)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Moves moving block on the destination block
    /// </summary>
    /// <param name="movingBlock"> Moving block</param>
    /// <param name="destinationBlock"> the block on moving block will place</param>
    private void MoveBlock(Block movingBlock, Block destinationBlock)
    {
        movingBlock.jewelMoverBlock.SetJewelMoverBlock();
        movingBlock.jewelMoverBlock.SetDestination(destinationBlock);
        movingBlock.jewelMoverBlock.gameObject.SetActive(true);
    }
}
