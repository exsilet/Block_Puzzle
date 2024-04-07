using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using GamingMonks;


public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private bool _isInitialized = false;

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            _isInitialized = true;
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
        }
        catch (ConsentCheckException e)
        {
            // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
            _isInitialized = false;
            Debug.Log(e);
        }
    }

    public void LevelEvent(int levelNumber,bool hasWon , string status)
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        parameter.Add("LevelNumber", levelNumber);
        parameter.Add("LevelStatus", status);
        parameter.Add("NumberOfMoves", GamePlay.Instance.NumberOfMoves);
        parameter.Add("NumberOfPowerUpsUsed", PowerUpsController.Instance.PowerUpsUsedCount);
        parameter.Add("TimeToCompleteLevel", GamePlayUI.Instance.GetTimeToCompleteLevel());

        if (hasWon)
        {
            parameter.Add("NumberOfAttemptToWin", GameProgressTracker.Instance.GetFreqToWinLevel(GamePlayUI.Instance.level));
        }
        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Level", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void GameLaunchEvent()
    {
        if(!PlayerPrefs.HasKey("FirstGameLauch"))
        {
            if (_isInitialized)
            {
                AnalyticsService.Instance.CustomData("First_Game_Lauch", null);
            }
            PlayerPrefs.SetInt("FirstGameLauch", 1);
        }
    }

    public void TutorialStartedEvent()
    {
        if(_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Tutorial_Started", null);
        }
    }

    public void TutorialEndedEvent()
    {
        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Tutorial_Ended", null);
        }
    }

    public void GameWinEvent()
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }
        parameter.Add("NumberOfMoves", GamePlay.Instance.NumberOfMoves);

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Game_win", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void GameLooseEvent()
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }
        parameter.Add("NumberOfMoves", GamePlay.Instance.NumberOfMoves);

        if(_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Game_lose", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void GamePlayedEvent()
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        GameMode gameMode = GamePlayUI.Instance.currentGameMode;
        string gameModeName = gameMode.ToString();

        parameter.Add("GameMode", gameModeName);
        parameter.Add("NumberOfTimesModePlayed", GameProgressTracker.Instance.GetGameModePlayedCount(gameMode));

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Game_played", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void OutOfMoveEvent()
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        parameter.Add("NumberOfMoves", GamePlay.Instance.NumberOfMoves);

        int count = 0;
        foreach(ShapeContainer shapeContainer in GamePlay.Instance.blockShapeController.AllShapeContainer)
        {
            if(shapeContainer.blockShape != null)
            {
                count++;
                if(count > 1)
                {
                    break;
                }
            }
        }

        if(count > 1)
        {
            int index = 0;
            foreach (ShapeContainer shapeContainer in GamePlay.Instance.blockShapeController.AllShapeContainer)
            {
                index++;
                if (shapeContainer.blockShape != null)
                {
                    string parameterName = "ShapeInHand" + index;
                    string data = shapeContainer.blockShape.BlockShapeType + " Rotated by " + shapeContainer.blockShape.Rotation;
                    parameter.Add(parameterName, data);
                }
            }
        }

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Out_of_move", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void ContinueGameEvent(bool continuedByAd, int spendCoin)
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }

        if (continuedByAd) {
            parameter.Add("ContinuedBy", "Ad");
        } else {
            parameter.Add("ContinuedBy", "Coin");
            parameter.Add("SpendCoin", spendCoin);
        }

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Continued_game", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void RotatePowerUpEvent(bool isUsedByInventory, int spendCoin )
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }

        if (isUsedByInventory) {
            parameter.Add("UsedBy", "Inventory");
        } else {
            parameter.Add("UsedBy", "Coin"); ;
            parameter.Add("SpendCoin", spendCoin);
        }

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Rotate_powerup_used", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void SingleBlockPowerUpEvent(bool isUsedByInventory, int spendCoin)
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }

        if (isUsedByInventory) {
            parameter.Add("UsedBy", "Inventory"); ;
        } else {
            parameter.Add("UsedBy", "Coin"); ;
            parameter.Add("SpendCoin", spendCoin);
        }

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("SingleBlock_powerup_used", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void BombPowerUpEvent(bool isUsedByInventory, int spendCoin)
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }

        if (isUsedByInventory) {
            parameter.Add("UsedBy", "Inventory"); ;
        } else {
            parameter.Add("UsedBy", "Coin"); ;
            parameter.Add("SpendCoin", spendCoin);
        }

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("Bomb_powerup_used", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }

    public void BoxingGlovePowerUpEvent()
    {
        Dictionary<string, object> parameter = new Dictionary<string, object>();

        string gameModeName = GamePlayUI.Instance.currentGameMode.ToString();
        parameter.Add("GameMode", gameModeName);

        if (GamePlayUI.Instance.currentGameMode == GameMode.Level)
        {
            parameter.Add("LevelNumber", GamePlayUI.Instance.level);
        }
        parameter.Add("UsedBy", "Line clear");

        if (_isInitialized)
        {
            AnalyticsService.Instance.CustomData("BoxingGlove_powerup_used", parameter);
        }
        // AnalyticsService.Instance.Flush();
    }
}
