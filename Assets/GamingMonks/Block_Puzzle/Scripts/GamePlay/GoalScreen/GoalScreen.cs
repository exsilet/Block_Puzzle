using GamingMonks;
using GamingMonks.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GoalScreen : MonoBehaviour
{
    public int goalLevel = 0;
    [SerializeField] private GameObject goalPrefab;
    public Transform goalHolder;
    public TextMeshProUGUI levelText;
    LevelSO levelSettings;


    private void Awake()
    {
        if (levelSettings == null)
        {
            levelSettings = (LevelSO)Resources.Load("LevelSettings");
        }
    }

    public void SetGoals(int levelNumber)
    {
        goalLevel = levelNumber;
        levelText.text = LocalizationManager.Instance.GetTextWithTag("txtLevel") + goalLevel.ToString();
        LevelGoal[] levelGoals = levelSettings.Levels[GamePlayUI.Instance.gamePlaySettings.levelToLoadInfo[levelNumber-1] - 1].Goal;
        foreach (Transform goalChild in goalHolder)
        {
            Destroy(goalChild.gameObject);
        }
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, gameObject.transform);
            goal.GetComponent<Image>().sprite = ThemeManager.Instance.GetBlockSpriteWithTag(levelGoals[i].spriteType.ToString());
            goal.transform.SetParent(goalHolder, false);
            TextMeshProUGUI t = goal.transform.GetComponentInChildren<TextMeshProUGUI>();
            //t.transform.localPosition = Vector2.zero;
            //t.enabled = true;
            t.text = levelSettings.Levels[GamePlayUI.Instance.gamePlaySettings.levelToLoadInfo[levelNumber-1] - 1].Goal[i].target.ToString();
        }

    }

    public void OnPlay()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            this.gameObject.Deactivate();
            UIController.Instance.LoadGamePlayFromLevelSelection(GameMode.Level, goalLevel);
        }
        //AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetFiveExtraCoins);
    }

    public void OnCancel()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            this.gameObject.Deactivate();
        }
    }
}
