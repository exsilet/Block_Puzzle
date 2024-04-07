using GamingMonks.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GamingMonks
{
    public class PowerUpContextPanel : MonoBehaviour
    {
        public Image panelInfoImage;
        public TextMeshProUGUI panelText;

        public void SetPanelContext(PowerUp powerUpName)
        {
            panelInfoImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(powerUpName.ToString());
            panelText.text = LocalizationManager.Instance.GetTextWithTag(powerUpName.ToString());
        }
    }
}


