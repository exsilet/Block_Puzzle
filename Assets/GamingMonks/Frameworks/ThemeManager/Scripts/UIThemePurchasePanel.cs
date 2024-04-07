using GamingMonks;
using GamingMonks.Localization;
using GamingMonks.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    public class UIThemePurchasePanel : MonoBehaviour
    {
        ThemeConfig currentSelectedTheme;

#pragma warning disable 0649
        [SerializeField] TextMeshProUGUI txtThemeName;
        [SerializeField] Image imgBlockSample;
        //[SerializeField] GameObject imgPanelUnlockedFilter;
        [SerializeField] Button btnUnlock;
        [SerializeField] Button btnSelect;
        [SerializeField] Button btnActive;
        //[SerializeField] TextMeshProUGUI txtUnlockPrice;
        [SerializeField] Text txtUnlockPrice;
        //[SerializeField] TextMeshProUGUI txtUnlockText;
        public string themeName;
#pragma warning restore 0649

        bool isActiveTheme = false;
        bool isUnlockedTheme = false;

        //private UnityEngine.Purchasing.Product thisProduct;
        [SerializeField] private IAPProductID themeProductID;

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            ThemeManager.OnThemeChangedEvent += OnThemeChanged;
            if (currentSelectedTheme != null)
            {
                if ((PlayerPrefs.GetInt("ThemeUnlockStatus_" + currentSelectedTheme.themeIAPID, 0) == 1))
                {
                    UpdateUIAfterUnlock();
                }
            }

            UITheme currentUITheme = ThemeManager.Instance.GetCurrentTheme();

            if (currentUITheme != null)
            {
                UIThemeSettings uiThemeSettings = (UIThemeSettings)Resources.Load("UIThemeSettings");
                foreach (ThemeConfig setting in uiThemeSettings.allThemeConfigs)
                {
                    if (setting.isEnabled && setting.themeIAPID == themeProductID && setting.uiTheme != null)
                    {
                        SetTheme(setting);
                    }
                }
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            ThemeManager.OnThemeChangedEvent -= OnThemeChanged;
        }

        private void SetTheme(ThemeConfig setting)
        {
            currentSelectedTheme = setting;
            txtThemeName.text = setting.themeName;
            imgBlockSample.sprite = setting.demoSprite;
            //txtUnlockPrice.text = setting.unlockCost.ToString();
            txtUnlockPrice.text = GetPriceText();

            bool unlockStatus = false;
            /*
            if (setting.defaultStatus == 1)
            {
                PlayerPrefs.SetInt("ThemeUnlockStatus_" + setting.themeName, 1);
            }
            */
            if ((PlayerPrefs.GetInt("ThemeUnlockStatus_" + setting.themeIAPID, 0) == 1) || setting.themeIAPID == IAPProductID.Other)
            {
                unlockStatus = true;
            }
            isUnlockedTheme = unlockStatus;
            isActiveTheme = currentSelectedTheme.themeName.Equals(ThemeManager.Instance.GetCurrentThemeName());
            if (!unlockStatus)
            {
                btnUnlock.gameObject.SetActive(true);
            }

            else
            {
                UpdateUIAfterUnlock();
            }

        }

        /// <summary>
        /// Theme selection button click listener.
        /// </summary>
        public void OnThemeButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                if (PlayerPrefs.HasKey("ThemeUnlockStatus_" + currentSelectedTheme.themeIAPID) || currentSelectedTheme.themeIAPID == IAPProductID.Other)
                {
                    ThemeManager.Instance.SetTheme(currentSelectedTheme);
                }
                else
                {
                    PurchaseTheme();
                }
            }
        }

        private void PurchaseTheme()
        {
            /*
            if (CurrencyManager.Instance.DeductGems(currentSelectedTheme.unlockCost))
            {
                UIController.Instance.PlayDeductGemsAnimation(btnUnlock.gameObject.transform.position, 0.1F);
                PlayerPrefs.SetInt("ThemeUnlockStatus_" + currentSelectedTheme.themeName, 1);
                isUnlockedTheme = true;
                //purchase successfull screen/failed screen
                Invoke("UpdateUIAfterUnlock", 1.2F);
            }
            else
            {
                //UIController.Instance.shopScreen.gameObject.Activate();
            }
            */
            IAPManagers.Instance.PurchaseProduct(currentSelectedTheme.themeIAPID.ToString());
        }

        public void UnlockTheme()
        {
            PlayerPrefs.SetInt("ThemeUnlockStatus_" + themeProductID, 1);
            isUnlockedTheme = true;
            Invoke("UpdateUIAfterUnlock", 1.2F);
        }

        private void UpdateUIAfterUnlock()
        {
            if (isActiveTheme)
            {
                btnActive.gameObject.SetActive(true);
                btnUnlock.gameObject.SetActive(false);
                btnSelect.gameObject.SetActive(false);
            }
            else
            {
                btnActive.gameObject.SetActive(false);
                btnUnlock.gameObject.SetActive(false);
                btnSelect.gameObject.SetActive(true);
            }

            //txtUnlockText.gameObject.SetActive(true);
            //txtUnlockText.text = LocalizationManager.Instance.GetTextWithTag("txtThemeUnlocked");
            //imgPanelUnlockedFilter.gameObject.SetActive(true);
        }
        void OnThemeChanged(string themeName)
        {
            if (isUnlockedTheme)
            {
                if (currentSelectedTheme.themeName.Equals(themeName))
                {
                    if (!isActiveTheme)
                    {
                        //imgBorder.SetActive(true);
                        isActiveTheme = true;
                        btnActive.gameObject.SetActive(true);
                        btnSelect.gameObject.SetActive(false);
                        btnUnlock.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (isActiveTheme)
                    {
                        //imgBorder.SetActive(false);
                        btnActive.gameObject.SetActive(false);
                        btnSelect.gameObject.SetActive(true);
                        btnUnlock.gameObject.SetActive(false);
                    }
                    isActiveTheme = false;
                }
            }
        }

        private string GetPriceText()
        {
            if (IAPManagers.Instance.hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product thisProduct = IAPManagers.Instance.GetProduct(currentSelectedTheme.themeIAPID.ToString());
                if (thisProduct != null)
                {
                      return thisProduct.metadata.localizedPriceString;
                }
            }
            
            return IAPManagers.Instance.GetDefaultPrice(currentSelectedTheme.themeIAPID);
        }

        public IAPProductID ThemeProductID
        {
            get { return themeProductID; }
        }

    }

}
