using UnityEngine;
using UnityEngine.Serialization;

namespace GamingMonks
{
	/// <summary>
	/// Scriptable for ad settings.
	/// </summary>
	public class AppSettings : ScriptableObject
	{
		#region CommonSettings
		
		// Android store like google, amazon, samsung etc.
		public int currentAndroidStore = 0;

		// Privacy policy url.
		public string privacyPolicyURL;

		// Support url need to be enabled and need to show in settings screen or not.
		public bool enableSupportURL = true;

		// Support url is support is enabled.
		public string supportURL;

		// Apple Id to nevigate to store.
		public string appleID;
		
		#endregion


		#region ReviewSettings

		// Review popup should be enabled or not.
		public bool showReviewPopupOnLaunch = true;

		// At which launch count review request should be made. enter numbers seperated by comma.
		public string reviewPopupAppLaunchCount;

		// Review popup should be enabled or not.
		public bool showReviewPopupOnGameOver = true;

		// At which launch count review request should be made. enter numbers seperated by comma.
		public string reviewPopupGameOverCount;

		// Navigate to store for review if user selected minimum star from review popup.
		public float minRatingToNavigateToStore = 4.5F;

		// Apple native store review request should be made instead of popup.
		public bool showAppleStoreReviewPopupOniOS = true;

		// Should show review popup if already rated.
		public bool neverShowReviewPopupIfRated = true;

		// Store revire url amazon store.
		public string amazonReviewURL = "";

		// Store revire url samsung store.
		public string samsungReviewURL = "";
		
		#endregion

		// Vibration permission should be added to android manifest.
		public bool enableVibrations = true;
		
		#region InventorySettings

		// Default amount of coins at starting of game.
		public int defaultCoinAmount = 240;
		
		// Free coins reward on watching rewarded video.
		public int watchAdsRewardCoin = 35;

		// Coins amount to rescue game.
		public int rescueGameCoinsCost = 35;
		
		// Free life reward on watching rewarded video.
		public int watchAdsRewardLife = 1;
		
		// Free extra moves reward on watching rewarded video.
		public int watchAdsRewardMoves = 6;
		
		// Free extra time reward for time mode on watching rewarded video.
		public float watchAdsRewardTime = 30;
		
		// Coins amount to purchase life.
		public int lifeCost = 50;
		
		// Coins amount to purchase Rotate powerUp.
		public int rotatePowerUpCost = 50;
		
		// Coins amount to purchase singleBlock powerUp.
		public int singleBlockPowerUpCost = 50;
		
		// Coins amount to purchase bomb PowerUp.
		public int bombPowerUpCost = 50;
		#endregion

		#region DailyRewards

		// Daily reward should be enabled or not.
		public bool useDailyRewards = true;

		// Scriptable instance for daily reward settings.
		public DailyRewardSettings dailyRewardsSettings;
		
		#endregion
	}
}	

[System.Serializable]
public class DailyRewardSettings {
	
	// Reward on each days. Add as many days as you want with reward amount. Will keep repeating.
	public AllDayRewards[] allDayRewards;

}

[System.Serializable]
public class AllDayRewards
{
    public int allCoinRewards;
    public int allLifeRewards;
    public int allRotateRewards;
    public int allSingleBlockRewards;
    public int allBombRewards;
}

