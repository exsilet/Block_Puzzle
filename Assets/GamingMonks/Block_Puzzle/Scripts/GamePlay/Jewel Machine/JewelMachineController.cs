using System.Collections.Generic;
using GamingMonks;
using System.Threading.Tasks;
using UnityEngine;

public class JewelMachineController : Singleton<JewelMachineController>
{
    // Prefab of Jewel Machine
    [SerializeField] private JewelMachine jewelMachinePrefab;

    // prefab of Jewel Mover Block 
    [SerializeField] public JewelMoverBlock jewelMoverBlockPrefab;

    // prefab of Jewel
    public Jewel jewelPrefab;

    // Holding all the Jewel machine present on the grid 
    private List<JewelMachine> jewelMachines;

    // Holds Jewel Machine ColoumID 
    private List<int> machineColoumId = new List<int>();

    // Jewel Count to spwan on the grid ON game start
    private int jewelSpwanCount = 0;

    public bool isJewelMachineHasInitialized = false;

    public bool isMoving { get; private set; }
    private void OnEnable()
    {
        GamePlay.Instance.SetJewelMachineStatus(true);
        GamePlay.Instance.OnRowColoumCleared += FillBlocks;
    }

    private void OnDisable()
    {
        GamePlay.Instance.OnRowColoumCleared -= FillBlocks;
    }

    public void ResetMachine()
    {
        foreach(JewelMachine machine in jewelMachines)
        {
            Destroy(machine.gameObject);
        }
        GamePlay.Instance.SetJewelMachineStatus(false);
        jewelSpwanCount = 0;
        jewelMachines.Clear();
        machineColoumId.Clear();
    }

    /// <summary>
    /// Stores jewel machine coloum ids
    /// </summary>
    /// <param name="coloumId"> jewel machine coloum Id</param>
    public void AddJewelMachineColoumId(int coloumId)
    {
        machineColoumId.Add(coloumId);
    }

    /// <summary>
    /// Initialise jewel machines
    /// </summary>
    /// <param name="jewelCounter"> counter value of jewel </param>
    /// <param name="_jewelSpwanCount"> count to spwan jewel at level start</param>
    public void StartJewelMachine(int jewelCounter, int _jewelSpwanCount)
    {
        jewelMachines = new List<JewelMachine>();
        jewelSpwanCount = _jewelSpwanCount;
        int machineTarget = GetTargetForEachMachine();
        for (int i = 0; i < machineColoumId.Count; i++)
        {
            JewelMachine machine = Instantiate(jewelMachinePrefab, transform);
            machine.Initialize(machineColoumId[i], machineTarget, jewelCounter);
            jewelMachines.Add(machine);
            GamePlay.Instance.instantiatedGameObjects.Add(machine.gameObject);
        }
    }

    public bool CheckJewelMachineInitialized()
    {
        return isJewelMachineHasInitialized;
    }
  
    /// <summary>
    /// Checks the condition for the gems to fill and call the fill gems function of the machine
    /// After filling gems, calls the simple block filling function
    /// </summary>
    public async void FillBlocks()
    {
        isMoving = true;
        await Task.Delay(System.TimeSpan.FromSeconds(0.4f));
        foreach (JewelMachine machine in jewelMachines)
        {
            machine.MoveAllBlocks();
        }

        await Task.Delay(System.TimeSpan.FromSeconds(0.6f));

        if (CanFillGems())
        {
            int gemsFilled = 0;
            int gemsToFill = TotalGemsToBeFilled();
            while (gemsFilled <= gemsToFill)
            {
                foreach (JewelMachine machine in jewelMachines)
                {
                    StartCoroutine(machine.FillGem());
                    gemsFilled++;
                    if(gemsFilled >= gemsToFill)
                    {
                        break;
                    }
                }
            }
        }

        await Task.Delay(System.TimeSpan.FromSeconds(0.4f));

        FillBlock();

        await Task.Delay(System.TimeSpan.FromSeconds(0.4f));
        isMoving = false;
        GamePlay.Instance.blockShapeController.CheckAllShapesCanbePlaced();
    }

    /// <summary>
    /// Fill blocks of the machine by simple blocks
    /// </summary>
    private void FillBlock()
    {
        foreach (JewelMachine machine in jewelMachines)
        {
            StartCoroutine(machine.FillBlocks());
        }
    }

    /// <summary>
    /// Checks for the gems can place on the jewel machine
    /// </summary>
    /// <returns> can gems place </returns>
    public bool CanFillGems()
    {
        int jewelTarget = TargetController.Instance.GetRemainingTarget(SpriteType.ColouredJewel);
        int gemsPresent = GetTotalGemsPresent();
        if (gemsPresent < jewelTarget && gemsPresent < jewelSpwanCount)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculates total gems to be filled
    /// </summary>
    /// <returns> gems count to place </returns>
    private int TotalGemsToBeFilled()
    {
        //int jewelTarget = TargetController.Instance.GetRemainingTarget(SpriteType.ColouredJewel);
        return jewelSpwanCount - GetTotalGemsPresent();
    }

   /// <summary>
   /// Calculates total total gems present on the grid
   /// </summary>
   /// <returns> total gems Present on grid </returns>
    private int GetTotalGemsPresent()
    {
        int gemsPresent = 0;
        foreach (JewelMachine machine in jewelMachines)
        {
            gemsPresent += machine.GetPresentGemCount();
        }
        return gemsPresent;
    }

    /// <summary>
    /// Calculates targets for the machine
    /// </summary>
    /// <returns> gems spawn target for the machine </returns>
    private int GetTargetForEachMachine()
    {
        //jewelTarget = TargetController.Instance.GetRemainingTarget(SpriteType.ColouredJewel);
        return jewelSpwanCount / machineColoumId.Count;
    }
}
