using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GamingMonks;
using UnityEngine.Serialization;

public class ConveyorBeltController : Singleton<ConveyorBeltController>
{
    // Conveyor Prefab
    [SerializeField] private GameObject conveyorBeltPrefab;

    // Conveyor Mover Block prefab
    [SerializeField] public ConveyorMoverBlock conveyorMoverBlockPrefab;

    // references of conveyors
    private CircularConveyorBelt CircularConveyorBelt;
    private List<LinearConveyorBelt> LinearConveyorHolder;
    private List<BlockConveyor> BlockConveyors;

    public List<int> completedRows = new List<int>();
    public List<int> completedColumns = new List<int>();

    // All the blocks on which conveyors are present.
    public List<Block> conveyorBlocks = new List<Block>();
    
    [Tooltip("Time in seconds after which conveyors should highlight after level starts.")]
    [SerializeField] private float m_Time = 1f;
    
    [Tooltip("Repeat conveyor highlight every n seconds.")]
    [SerializeField] private float m_RepeatRate = 3f;
    
    private void OnEnable()
    {
        GamePlayUI.OnShapePlacedEvent += MoveConveyorBelt;
        
        if(!IsInvoking(nameof(StartHighlightingConveyorBlocks))) {
            InvokeRepeating(nameof(StartHighlightingConveyorBlocks), m_Time, m_RepeatRate);
        }
    }

    private void OnDisable()
    {
        GamePlayUI.OnShapePlacedEvent -= MoveConveyorBelt;
        ResetConveyorBelts();
        
        if(IsInvoking(nameof(StartHighlightingConveyorBlocks))) {
            CancelInvoke(nameof(StartHighlightingConveyorBlocks));
        }
        conveyorBlocks.Clear();
    }

    private void StartHighlightingConveyorBlocks()
    {
        //if (!GamePlay.Instance.isBoardReady)
        //{
        //    return;
        //}
        //else
        //{
            if (conveyorBlocks.Count > 0)
            {
                foreach (Block block in conveyorBlocks)
                {
                    StartCoroutine(block.HighlightConveyor());
                }
            }
        //}
    }

    public void SetConveyor()
    {
        ConveyorBelts ConveyorBelts = GamePlayUI.Instance.currentLevel.ConveyorBelts;

        if(ConveyorBelts.enabled)
        {
            if(ConveyorBelts.CircularConveyor.enabled)
            {
                SpawnCircularConveyor(ConveyorBelts.CircularConveyor);
            }

            if (ConveyorBelts.LinearConveyor.enabled)
            {
                LinearConveyorHolder = new List<LinearConveyorBelt>();
                ConveyorLocation[] conveyorBelts = ConveyorBelts.LinearConveyor.Locations;
                for (int i = 0; i < conveyorBelts.Length; i++)
                {
                    SpawnLinearConveyor(conveyorBelts[i]);
                }
            }

            if(ConveyorBelts.SingleBlockConveyor.enabled)
            {
                BlockConveyors = new List<BlockConveyor>();
                BlockConveyors[] blockConveyors = ConveyorBelts.SingleBlockConveyor.blockConveyors;
                for(int i = 0; i < blockConveyors.Length; i++)
                {
                    SpawnBlockConveyor(blockConveyors[i]);
                }
            }
        }
    }

    private void SpawnCircularConveyor(CircularConveyor circularConveyor)
    {
        CircularConveyorBelt = Instantiate(conveyorBeltPrefab, transform).GetComponent<CircularConveyorBelt>();
        CircularConveyorBelt.SetConveyor(circularConveyor);
        GamePlay.Instance.instantiatedGameObjects.Add(CircularConveyorBelt.gameObject);
    }

    private void SpawnLinearConveyor(ConveyorLocation location)
    {
        LinearConveyorBelt linearConveyorBelt = Instantiate(conveyorBeltPrefab, transform).GetComponent<LinearConveyorBelt>();
        linearConveyorBelt.SetConveyor(location);
        LinearConveyorHolder.Add(linearConveyorBelt);
        GamePlay.Instance.instantiatedGameObjects.Add(linearConveyorBelt.gameObject);
    }

    private void SpawnBlockConveyor(BlockConveyors blockConveyor)
    {
        BlockConveyor conveyor = new BlockConveyor();
        conveyor.SetBlockConveyor(blockConveyor);
        BlockConveyors.Add(conveyor);
    }

    public async void MoveConveyorBelt()
    {
        InputManager.Instance.DisableTouch();
        GamePlay.Instance.isBoardReady = false;
        await Task.Delay(System.TimeSpan.FromSeconds(0.6f));

        if (CircularConveyorBelt != null)
        {
            CircularConveyorBelt.MoveConveyor();
        }

        if (LinearConveyorHolder != null && LinearConveyorHolder.Count > 0)
        {
            for (int i = 0; i < LinearConveyorHolder.Count; i++)
            {
                LinearConveyorHolder[i].MoveConveyorBelt();
            }
        }

        if(BlockConveyors != null && BlockConveyors.Count > 0)
        {
            foreach(BlockConveyor conveyor in BlockConveyors)
            {
                // StartCoroutine(conveyor.conveyorBlock.HighlightConveyor());
                conveyor.MoveBlock();
            }

            foreach (BlockConveyor conveyor in BlockConveyors)
            {
                if(!conveyor.hasMoved)
                {
                    conveyor.MoveBlock();
                }
            }
        }

        //await Task.Delay(System.TimeSpan.FromSeconds(1f));
        await Task.Delay(System.TimeSpan.FromSeconds(0.2f));

        if (CircularConveyorBelt != null)
        {
            CheckRowOrColoumClearForCircular();
        }

        if (LinearConveyorHolder != null && LinearConveyorHolder.Count > 0)
        {
            CheckRowOrColoumClearForLinear();
        }

        if (BlockConveyors != null && BlockConveyors.Count > 0)
        {
            CheckRowOrColoumClearForBlockCony();
        }

        ClearBlocks();
        GamePlay.Instance.isBoardReady = true;
        GamePlay.Instance.blockShapeController.CheckAllShapesCanbePlaced();
        InputManager.Instance.EnableTouch();
    }

    private void CheckRowOrColoumClearForCircular()
    {
        CircularConveyorBelt.CheckForRowOrColoumClear();
    }

    private void CheckRowOrColoumClearForLinear()
    {
        for (int i = 0; i < LinearConveyorHolder.Count; i++)
        {
            LinearConveyorHolder[i].CheckRowOrColoumClear();
        }
    }

    private void CheckRowOrColoumClearForBlockCony()
    {
        foreach (BlockConveyor conveyor in BlockConveyors)
        {
            conveyor.CheckForRowOrColumClear();
        }
    }

    public void ResetConveyorBelts()
    {
        if(LinearConveyorHolder != null)
        {
            LinearConveyorHolder.Clear();
        }

        if(BlockConveyors != null)
        {
            BlockConveyors.Clear();
        }

        CircularConveyorBelt = null;
    }

    private void ClearBlocks()
    {
        if (completedRows.Count > 0)
        {
            GamePlay.Instance.ClearRows(completedRows);
            AudioController.Instance.PlayLineBreakSound(completedRows.Count);
        }

        if (completedColumns.Count > 0)
        {
            GamePlay.Instance.ClearColumns(completedColumns);
            AudioController.Instance.PlayLineBreakSound(completedColumns.Count);
        }
        completedColumns.Clear();
        completedRows.Clear();  
    }
}
