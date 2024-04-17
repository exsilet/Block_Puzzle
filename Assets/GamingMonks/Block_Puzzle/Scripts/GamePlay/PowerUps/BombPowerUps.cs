using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GamingMonks;
using GamingMonks.Feedbacks;
using TMPro;
using UnityEngine.UI;

public class BombPowerUps : Singleton<BombPowerUps>, IPointerDownHandler, IBeginDragHandler, IPointerUpHandler, IDragHandler
{
    public Button panelButton;
    public GameObject valueSprite;
    public GameObject cancelSprite;
    public GameObject freeWithCoinSprite;
    public ParticleSystem bombExplosion;

    private int m_remaingBombFromInventory = 0;
    private bool m_canClear = false;
    private bool m_isActive = false;
    private bool m_canUse = false;
    private int m_purchaseWithCoinAmount = 0;
    private bool m_isUsedByInventory;

    private Vector2 m_pointerClickOffset = new Vector2(0, 1);
    private List<int> highlightRows = new List<int>();
    private List<int> highlightColoum = new List<int>();

    private List<Block> colliderDisabledBlock = new List<Block>();

    private void OnEnable()
    {
        //gameObject.GetComponent<Canvas>().sortingOrder = -1;
        CurrencyManager.Instance.OnValuesChanged += OnBombUpValuesChanged;
        Initialize();
    }

    private void OnDisable()
    {
        CurrencyManager.Instance.OnValuesChanged -= OnBombUpValuesChanged;
    }

    private void Initialize()
    {
        m_purchaseWithCoinAmount = GamePlay.Instance.appSettings.bombPowerUpCost;
        m_remaingBombFromInventory = PlayerPrefs.GetInt("currentBombPowerUp");
        valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_remaingBombFromInventory.ToString();
        freeWithCoinSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_purchaseWithCoinAmount.ToString();
        valueSprite.SetActive(m_remaingBombFromInventory > 0);
        freeWithCoinSprite.SetActive(m_remaingBombFromInventory <= 0);
        cancelSprite.SetActive(false);
        m_isActive = false;
        transform.localPosition = Vector3.zero;
        cancelSprite.SetActive(false);
    }

    public void OnPowerUpButton()
    {
        if (CanUse())
        {
            m_canUse = true;
            InputManager.Instance.DisableTouchForDelay();
            UIFeedback.Instance.PlayButtonPressEffect();
            GamePlay.Instance.blockShapeController.ToggleBlockShapeContainer(m_isActive);
            cancelSprite.SetActive(!m_isActive);
            valueSprite.SetActive(m_isActive);
            if (!m_isActive)
            {
                UIController.Instance.OpenPowerUpContextPanel(PowerUp.BombPowerUpIcon);
                transform.position = GamePlay.Instance.blockShapeController.GetMiddleBlockShapePosition().position;
                this.transform.localScale = Vector3.one * 1.5f;
                PowerUpsController.Instance.SetButtonInteractable(panelButton);
                m_isActive = true;
            }
            else
            {
                UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
                this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                PowerUpsController.Instance.SetAllButtonsActive();
                m_isActive = false;
                ResetBombPowerUp();
            }
        }
    }

    private bool CanUse()
    {
        if (InputManager.Instance.canInput())
        {
            if(m_remaingBombFromInventory > 0)
            {
                m_isUsedByInventory = true;
                return true;
            }

            if(m_purchaseWithCoinAmount <= CurrencyManager.Instance.GetCurrentCoinsBalance())
            {
                return true;
            }
        }
        //PowerUpsController.Instance.OpenShopScreen();
        return false;
    }
    
    private void OnBombUpValuesChanged(int currentHealth, int currentRotatePowerUp, int currentBombPowerUp, int currentIndividualBlocksPowerUp)
    {
        m_remaingBombFromInventory = currentBombPowerUp;
        valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = currentBombPowerUp.ToString();
        valueSprite.SetActive(m_remaingBombFromInventory > 0);
        freeWithCoinSprite.SetActive(m_remaingBombFromInventory <= 0);
        freeWithCoinSprite.SetActive(m_remaingBombFromInventory <= 0);
        cancelSprite.SetActive(false);
        m_isActive = false;
        this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(m_canUse && m_isActive)
        {
            gameObject.GetComponent<Canvas>().sortingOrder = 4;
            EnableAllCollider();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_canUse && m_isActive)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            pos += m_pointerClickOffset;
            transform.position = pos;
            HighLightBlock();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_canUse && m_isActive)
        {
            Vector2 offsetPosition = transform.position;
            offsetPosition += m_pointerClickOffset;
            transform.position = offsetPosition;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (m_canUse && m_isActive)
        {
            gameObject.GetComponent<Canvas>().sortingOrder = 1;
            bombExplosion.transform.position = transform.position;
            transform.position = GamePlay.Instance.blockShapeController.GetMiddleBlockShapePosition().position;
            DiableAllCollider();
            StopHighlighting();
            if (m_canClear)
            {
                bombExplosion.Play();
                this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                cancelSprite.SetActive(false);
                valueSprite.SetActive(m_remaingBombFromInventory > 0);

                CheckForMilkShop();
                //m_canClear = false;
                highlightRows.Clear();
                highlightColoum.Clear();
                if (m_isUsedByInventory)
                {
                    m_remaingBombFromInventory--;
                    valueSprite.SetActive(m_remaingBombFromInventory > 0);
                    valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_remaingBombFromInventory.ToString();
                    PlayerPrefs.SetInt("currentBombPowerUp", m_remaingBombFromInventory);
                    freeWithCoinSprite.SetActive(m_remaingBombFromInventory <= 0);
                    CurrencyManager.Instance.UpdatePowerUps();
                }
                else
                {
                    CurrencyManager.Instance.DeductCoins(m_purchaseWithCoinAmount);
                    CurrencyManager.Instance.UpdatePowerUps();
                }
                if (UIController.Instance.topPanelWithModeContext.settingsPopUp.activeSelf)
                {
                    UIController.Instance.topPanelWithModeContext.settingsPopUp.SetActive(false);
                }
                GamePlay.Instance.blockShapeController.ToggleBlockShapeContainer(true);
                UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
                PowerUpsController.Instance.SetAllButtonsActive();
                AnalyticsManager.Instance.BombPowerUpEvent(m_isUsedByInventory, m_purchaseWithCoinAmount);
                //FBManeger.Instance.PowerUpsUsed("Bomb");
                PowerUpsController.Instance.PowerUpsUsedCount++;
                ResetBombPowerUp();
            }
        }
    }

    public void ResetBombPowerUp()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = 1;
        m_isUsedByInventory = false;
        m_canUse = false;
        m_canClear = false;
        m_isActive = false;
        transform.localScale = Vector3.one;
        Initialize();
    }

    private void CheckForMilkShop()
    {
        foreach (int rowID in highlightRows)
        {
            List<Block> row = GamePlay.Instance.GetEntireRow(rowID);

            foreach (Block block in row)
            {
                block.isHitByBoxingGlove = true;
            }

            GamePlay.Instance.ClearRow(rowID);

            if (GamePlayUI.Instance.currentGameMode == GameMode.Level && GamePlayUI.Instance.currentLevel.BlockerStick.enabled)
            {
                StartCoroutine(BlockerStickSpwaner.Instance.DestroyVerticalBlockerStick(rowID));
            }
        }

        foreach (int columnID in highlightColoum)
        {
            List<Block> column = GamePlay.Instance.GetEntirColumn(columnID);

            foreach (Block block in column)
            {
                block.isHitByBoxingGlove = true;
            }

            GamePlay.Instance.ClearColoum(columnID);

            if (GamePlayUI.Instance.currentGameMode == GameMode.Level && GamePlayUI.Instance.currentLevel.BlockerStick.enabled)
            {
                StartCoroutine(BlockerStickSpwaner.Instance.DestroyHorizontalBlockerStick(columnID));
            }
        }
    }

    private void EnableAllCollider()
    {
        foreach(List<Block> row in GamePlay.Instance.allRows)
        {
            foreach(Block block in row)
            {
                if(block.isFilled && !block.hasDiamond)
                {
                    block.thisCollider.enabled = true;
                    colliderDisabledBlock.Add(block);
                }
            }
        } 
    }

    private void DiableAllCollider()
    {
        foreach(Block block in colliderDisabledBlock)
        {
            block.thisCollider.enabled = false;
        }
        colliderDisabledBlock.Clear();
    }

    private void HighLightBlock()
    {
        Collider2D[] collidedBlocks = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(this.transform.localScale.x / 4, this.transform.localScale.x / 4), 0);
        
        if(collidedBlocks.Length <= 0)
        {
            m_canClear = false;
            StopHighlighting();
            return;
        }

        for(int i = 0; i < collidedBlocks.Length; i++)
        {
            Block block = collidedBlocks[i].GetComponent<Block>();
            if (block != null)
            {
                int rowId = block.RowId;
                int colId = block.ColumnId;

                if(!highlightRows.Contains(rowId))
                {
                    highlightRows.Add(rowId);
                    if (highlightRows.Count > 2)
                    {
                        while(highlightRows.Count > 2)
                        {
                            StopHighlightingRow(highlightRows[0]);
                            highlightRows.RemoveAt(0);
                        }
                    }
                } 

                if (!highlightColoum.Contains(colId))
                {
                    highlightColoum.Add(colId);
                    if (highlightColoum.Count > 2)
                    {
                        while (highlightColoum.Count > 2)
                        {
                            StopHighlightingColoum(highlightColoum[0]);
                            highlightColoum.RemoveAt(0);
                        }
                    }
                }
            }
        }

        foreach (int row in highlightRows)
        {
            HighlightingRow(row);
        }

        foreach (int col in highlightColoum)
        {
            HighlightingColoum(col);
        }
        m_canClear = true;
    }

    private void StopHighlightingRow(int rowId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
        {
            rowBlock.Reset();
        }
    }

    private void StopHighlightingColoum(int colId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntirColumn(colId))
        {
            rowBlock.Reset();
        }
    }

    private void HighlightingRow(int rowId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
        {
            rowBlock.Highlight();
        }
    }

    private void HighlightingColoum(int colId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntirColumn(colId))
        {
            rowBlock.Highlight();
        }
    }

    private void StopHighlighting()
    {
        foreach(int rowId in highlightRows)
        {
            StopHighlightingRow(rowId);
        }

        foreach (int colId in highlightColoum)
        {
            StopHighlightingColoum(colId);
        }
    }
}
