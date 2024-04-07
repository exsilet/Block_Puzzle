using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    public class RotatePowerUp : Singleton<RotatePowerUp>
    {
        
        public Button PanelButton;
        public GameObject rotateSprite;
        public GameObject valueSprite;
        public GameObject cancelSprite;
        public GameObject freeWithCoinSprite;

        private int m_remainingRotateValue = 0;
        private bool m_isActive = false;
        private int m_purchaseWithCoinAmount = 0;
        private bool m_isUsedByInventory = false;

        float[] rotations = new float[3];
        public List<GameObject> rotationItems;

        private void OnEnable()
        {
            CurrencyManager.Instance.OnValuesChanged += OnRotateUpValuesChanged;
            Initialize();
        }

        private void OnDisable()
        {
            CurrencyManager.Instance.OnValuesChanged -= OnRotateUpValuesChanged;
        }

        private void Initialize()
        {
            m_purchaseWithCoinAmount = GamePlay.Instance.appSettings.rotatePowerUpCost;
            m_remainingRotateValue = PlayerPrefs.GetInt("currentRotatePowerUp");
            valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_remainingRotateValue.ToString();
            freeWithCoinSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_purchaseWithCoinAmount.ToString();
            rotateSprite.SetActive(true);
            valueSprite.SetActive(m_remainingRotateValue > 0);
            freeWithCoinSprite.SetActive(m_remainingRotateValue <= 0);
            cancelSprite.SetActive(false);
            m_isActive = false;
        }

        public void ResetRotatePowerUp()
        {
            for (int i = 0; i < PowerUpsController.Instance.m_blockShapesController.AllShapeContainer.Count; i++)
            {
                ShapeContainer shapeContainer = PowerUpsController.Instance.m_blockShapesController.AllShapeContainer[i];
                if (shapeContainer.blockShape != null)
                {
                    rotationItems[i].gameObject.SetActive(false);
                }
            }

            Initialize();
        }

        public bool IsActive
        {
            get
            {
                return m_isActive;
            }
        }

        public void OnRotateUpButton()
        {
            //if (InputManager.Instance.canInput() && m_remainingRotateValue > 0)
            if(CanUse())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                cancelSprite.SetActive(!m_isActive);
                valueSprite.SetActive(m_isActive);
                rotateSprite.SetActive(m_isActive);
                List<ShapeContainer> allShapeContainers = new List<ShapeContainer>();
                GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
                if (!m_isActive)
                {
                    UIController.Instance.OpenPowerUpContextPanel(PowerUp.RotatePowerUpIcon);
                    rotations = GamePlay.Instance.blockShapeController.GetAllBlockShapeRotations();
                    allShapeContainers.AddRange(PowerUpsController.Instance.m_blockShapesController.AllShapeContainer);
                    
                    for (int i = 0; i < allShapeContainers.Count; i++)
                    {
                        ShapeContainer shapeContainer = allShapeContainers[i];
                        if (shapeContainer.blockShape != null)
                        {
                            rotationItems[i].gameObject.SetActive(true);
                            rotationItems[i].transform.position = shapeContainer.transform.position;
                        }
                    }
                    
                    GamePlayUI.Instance.currentModeSettings.allowRotation = true;
                    PowerUpsController.Instance.SetButtonInteractable(PanelButton);
                    m_isActive = true;
                }
                else
                {
                    DeactivateRotateOnUsed(false);
                }
            }
        }

        private bool CanUse()
        {
            if (InputManager.Instance.canInput())
            {
                if (m_remainingRotateValue > 0)
                {
                    m_isUsedByInventory = true;
                    return true;
                }

                if (m_purchaseWithCoinAmount <= CurrencyManager.Instance.GetCurrentCoinsBalance())
                {
                    return true;
                }
            }
            PowerUpsController.Instance.OpenShopScreen();
            return false;
        }

        public void DeactivateRotateOnUsed(bool isUsed)
        {
            cancelSprite.SetActive(!m_isActive);
            valueSprite.SetActive(m_remainingRotateValue > 0);
            freeWithCoinSprite.SetActive(m_remainingRotateValue <= 0);
            rotateSprite.SetActive(m_isActive);
            UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
            for (int i = 0; i < rotationItems.Count; i++)
            {
                rotationItems[i].gameObject.SetActive(false);
            }
            GamePlayUI.Instance.currentModeSettings.allowRotation = false;
            GamePlay.Instance.blockShapeController.SetAllBlockShapeRotations(rotations);
            m_isActive = false;

            //if (m_remainingRotateValue > 0 && isUsed)
            if (m_isUsedByInventory && isUsed)
            { 
                m_remainingRotateValue--;
                valueSprite.SetActive(m_remainingRotateValue > 0);
                valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = m_remainingRotateValue.ToString();
                PlayerPrefs.SetInt("currentRotatePowerUp", m_remainingRotateValue);
                freeWithCoinSprite.SetActive(m_remainingRotateValue <= 0);
                AnalyticsManager.Instance.RotatePowerUpEvent(m_isUsedByInventory, m_purchaseWithCoinAmount);
                //FBManeger.Instance.PowerUpsUsed("Rotate");
                PowerUpsController.Instance.PowerUpsUsedCount++;
                m_isUsedByInventory = false;
                CurrencyManager.Instance.UpdatePowerUps();
            }
            else if( isUsed)
            {
                CurrencyManager.Instance.DeductCoins(m_purchaseWithCoinAmount);
                CurrencyManager.Instance.UpdatePowerUps();
            }
            PowerUpsController.Instance.SetAllButtonsActive();
        }

        private void OnRotateUpValuesChanged(int currentHealth, int currentRotatePowerUp, int currentBombPowerUp, int currentIndividualBlocksPowerUp)
        {
            m_remainingRotateValue = currentRotatePowerUp;
            valueSprite.GetComponentInChildren<TextMeshProUGUI>().text = currentRotatePowerUp.ToString();
            valueSprite.SetActive(m_remainingRotateValue > 0);
            freeWithCoinSprite.SetActive(m_remainingRotateValue <= 0);
            cancelSprite.SetActive(false);
            m_isActive = false;
        }
    }
}


