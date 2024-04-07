using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GamingMonks
{
    public class LevelSelectionBtn : MonoBehaviour
    {
        public int levelNumber;
        public Image thisImage;
        public TextMeshProUGUI levelText;
        public GameObject lockSprite;

        /// <summary>
        /// Setup level and Image sprite for the button.
        /// </summary>
        public void SetupButton(int level, Sprite levelSprite, bool isLocked, Material textMaterial)
        {
            levelNumber = level;
            levelText.text = levelNumber.ToString();
            this.gameObject.name = "Level "+ levelNumber;
            levelText.fontSharedMaterial = textMaterial;
            ChangeButtonSprite(levelSprite);
            lockSprite.SetActive(isLocked);
        }

        /// <summary>
        /// On level btn pressed.
        /// </summary>
        public void OnLevelBtnPressed()
        {
            if (InputManager.Instance.canInput())
            {
                if (PlayerPrefs.GetInt("currentEnergy") > 0)
                {
                    UIFeedback.Instance.PlayButtonPressEffect();
                    AdmobManager.Instance.isRewardedShown = false;
                    UIController.Instance.LoadGoalScreenFromLevelSelection(levelNumber);
                }
                else
                {
                    UIController.Instance.OpenLivesPanel();
                }
            }
        }

        /// <summary>
        /// Change button sprite based on if its locked, unclocked and incomplete or unlocked and completed
        /// </summary>
        public void ChangeButtonSprite(Sprite levelSprite)
        {
            thisImage.sprite = levelSprite;
        }

    }
}
