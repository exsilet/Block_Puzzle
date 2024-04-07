using GamingMonks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PowerUpsController : Singleton<PowerUpsController>
{
    public Button[] powerUpButtons;
    private int m_NumberOfTimesPowerUpsUse;
    [SerializeField] public BlockShapesController m_blockShapesController;

    public void SetButtonInteractable(Button ButtonPanel)
    {
        for (int i = 0; i < powerUpButtons.Length; i++)
        {
            if (powerUpButtons[i] == ButtonPanel)
            {
                powerUpButtons[i].interactable = true;
            }
            else
            {
                powerUpButtons[i].interactable = false;
            }
        }
    }

    public void SetAllButtonsActive()
    {
        foreach(Button button in powerUpButtons)
        {
            button.interactable = true;
        }
    }

    public void SetAllButtonsDeActive()
    {
        foreach(Button button in powerUpButtons)
        {
            button.interactable = false;
        }
    }
    
    public void OpenShopScreen()
    {
        InputManager.Instance.DisableTouchForDelay();
        GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
        UIController.Instance.OpenShopScreen();
    }

    public int PowerUpsUsedCount
    {
        get
        {
            return m_NumberOfTimesPowerUpsUse;
        }

        set
        {
            m_NumberOfTimesPowerUpsUse = value;
        }
    }

    public async void CheckBlockShapesCanPlaced()
    {
        InputManager.Instance.DisableTouchForDelay(0.5f);
        await Task.Delay(System.TimeSpan.FromSeconds(0.5f));
        m_blockShapesController.CheckAllShapesCanbePlaced();
    }

    public void Reset()
    {
        PowerUpsUsedCount = 0;
        m_NumberOfTimesPowerUpsUse = 0;
        BoxingGlove.Instance.SetSortingOrder();
        BoxingGlove.Instance.ResetBoxingGlove();
        BombPowerUps.Instance.ResetBombPowerUp();
        SingleBlockPowerUp.Instance.Initialize();
        RotatePowerUp.Instance.ResetRotatePowerUp();
        SetAllButtonsActive();
    }

}

[System.Serializable]
public enum PowerUp
{
    RotatePowerUpIcon,
    BombPowerUpIcon,
    SinglePowerUpIcon,
    BoxingGlove,
    BoxingGlove1,
    BoxingGlove2,
    BoxingGlove3
}
