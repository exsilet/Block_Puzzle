using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    public class SingleBlockPowerUp : Singleton<SingleBlockPowerUp>
    {
        public Button PanelButton;
        public BlockShape singleBlockPrefab;
        public GameObject singleBlockSprite;
        public GameObject valueSprite;
        public GameObject cancelSprite;
        public GameObject freeWithCoinSprite;

        private int m_remainingSingleBlock = 0;
        private bool m_isActive = false;
        private int m_purchaseWithCoinAmount = 100;
        private bool m_isUsedByInventory;

        //ShapeInfo[] originalBlockShapes = new ShapeInfo[3];
        ShapeData[] originalBlockShapes = new ShapeData[3];
        string[] spriteTags = new string[3];

        private void OnEnable()
        {
            CurrencyManager.Instance.OnValuesChanged += OnSingleBlockPowerUpValuesChanged;
            Initialize();
        }

        private void OnDisable()
        {
            CurrencyManager.Instance.OnValuesChanged -= OnSingleBlockPowerUpValuesChanged;
        }

        public void Initialize()
        {
            m_purchaseWithCoinAmount = GamePlay.Instance.appSettings.singleBlockPowerUpCost;
            m_remainingSingleBlock = PlayerPrefs.GetInt("currentSingleBlockPowerUp");
            valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_remainingSingleBlock.ToString();
            freeWithCoinSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_purchaseWithCoinAmount.ToString();
            singleBlockSprite.SetActive(true);
            valueSprite.SetActive(m_remainingSingleBlock > 0);
            freeWithCoinSprite.SetActive(m_remainingSingleBlock <= 0);
            cancelSprite.SetActive(false);
            m_isActive = false;
        }

        public bool IsActive
        {
            get
            {
                return m_isActive;
            }
        }

        public void OnSingleBlockButton()
        {
            //if (InputManager.Instance.canInput() && m_remainingSingleBlock > 0)
            if (CanUse())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                cancelSprite.SetActive(!m_isActive);
                valueSprite.SetActive(m_isActive);
                singleBlockSprite.SetActive(m_isActive);
                if (!m_isActive)
                {
                    //UIController.Instance.powerUpContextPanel.Activate();
                    UIController.Instance.OpenPowerUpContextPanel(PowerUp.SinglePowerUpIcon);
                    spriteTags = GamePlay.Instance.blockShapeController.GetAllSpriteTags();
                    originalBlockShapes = GamePlay.Instance.blockShapeController.GetCurrentShapesInfoWithSpecialSpriteType();
                    GamePlay.Instance.blockShapeController.SetSingleBlock(spriteTags);
                    PowerUpsController.Instance.SetButtonInteractable(PanelButton);
                    m_isActive = true;
                }
                else
                {
                    DeactivateSingleBlockPowerUpOnUsed(false);
                }
            }
        }

        private bool CanUse()
        {
            if (InputManager.Instance.canInput())
            {
                if (m_remainingSingleBlock > 0)
                {
                    m_isUsedByInventory = true;
                    return true;
                }

                if (m_purchaseWithCoinAmount <= CurrencyManager.Instance.GetCurrentCoinsBalance())
                {
                    return true;
                }
            }
            //PowerUpsController.Instance.OpenShopScreen();
            return false;
        }

        public void DeactivateSingleBlockPowerUpOnUsed(bool isUsed)
        {
            cancelSprite.SetActive(!m_isActive);
            valueSprite.SetActive(m_remainingSingleBlock > 0);
            freeWithCoinSprite.SetActive(m_remainingSingleBlock <= 0);
            singleBlockSprite.SetActive(m_isActive);
            UIController.Instance.powerUpContextPanel.gameObject.Deactivate();

            //if (m_remainingSingleBlock > 0 && isUsed)
            if(m_isUsedByInventory && isUsed)
            {
                m_remainingSingleBlock--;
                valueSprite.SetActive(m_remainingSingleBlock > 0);
                valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_remainingSingleBlock.ToString();
                PlayerPrefs.SetInt("currentSingleBlockPowerUp", m_remainingSingleBlock);
                freeWithCoinSprite.SetActive(m_remainingSingleBlock <= 0);
                AnalyticsManager.Instance.SingleBlockPowerUpEvent(m_isUsedByInventory, m_purchaseWithCoinAmount);
                //FBManeger.Instance.PowerUpsUsed("Single_Block");
                PowerUpsController.Instance.PowerUpsUsedCount++;
                m_isUsedByInventory = false;
                CurrencyManager.Instance.UpdatePowerUps();
            }
            else if(isUsed)
            {
                CurrencyManager.Instance.DeductCoins(m_purchaseWithCoinAmount);
                CurrencyManager.Instance.UpdatePowerUps();
            }
            else
            {
                GamePlay.Instance.blockShapeController.RevertSingleBlock(originalBlockShapes, spriteTags);
            }
            m_isActive = false;

            PowerUpsController.Instance.SetAllButtonsActive();
            PowerUpsController.Instance.CheckBlockShapesCanPlaced();
        }

        public void OnSingleBlockPowerUpValuesChanged(int currentHealth, int currentRotatePowerUp, int currentBombPowerUp, int currentSingleBlockPowerUp)
        {
            m_remainingSingleBlock = currentSingleBlockPowerUp;
            valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = currentSingleBlockPowerUp.ToString();
            valueSprite.SetActive(m_remainingSingleBlock > 0);
            freeWithCoinSprite.SetActive(m_remainingSingleBlock <= 0);
            cancelSprite.SetActive(false);
            m_isActive = false;
        }
    }

    public class ShapeData
    {
        public bool isAdvanceShape = false;
        public string shapeName;
        public float shapeRotation;
        public bool isSpecialShape;
        public SpriteType spriteType;
        public int specialBlockIndex;

        // Class constructor with required parameters.
        public ShapeData(bool _isAdvanceShape, string _shapeName, float _shapeRotation, bool _isSpecialShape, SpriteType _spriteType, int _specialBlockIndex)
        {
            isAdvanceShape = _isAdvanceShape;
            shapeName = _shapeName;
            shapeRotation = _shapeRotation;
            isSpecialShape = _isSpecialShape;
            spriteType = _spriteType;
            specialBlockIndex = _specialBlockIndex;
        }
    }
}

