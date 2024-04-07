using UnityEngine;

namespace GamingMonks.Ads
{
	/// <summary>
	/// Ironsource Ads configuration.
	/// </summary>
	public class IronSourceAdsSettings : ScriptableObject 
	{	
		#pragma warning disable 0649
		// Android App Id.
		[SerializeField] string appId_android;
		
		// Apple App Id.
		[SerializeField] string appId_iOS;

		// Banner ad position.
		[SerializeField] BannerAdPosition bannerAdPosition;

		// Banner ad bg color.
		[SerializeField] string bannerBGColor;
        #pragma warning restore 0649

		public string GetAppId() {
			#if UNITY_ANDROID
			return appId_android;
			#elif UNITY_IOS
			return appId_iOS;
			#else 
			return "";
			#endif
		}

		#if HB_IRONSOURCE
		// Returns banner ad position.
		public IronSourceBannerPosition GetBannerPosition() 
		{
			IronSourceBannerPosition position = IronSourceBannerPosition.BOTTOM;
			switch(bannerAdPosition) 
			{
				case BannerAdPosition.TOP_RIGHT:
				case BannerAdPosition.TOP_CENTER:
				case BannerAdPosition.TOP_LEFT:
					position = IronSourceBannerPosition.TOP;
				break;
					
				case BannerAdPosition.CENTER:
				case BannerAdPosition.BOTTOM_RIGHT:
				case BannerAdPosition.BOTTOM_CENTER:
				case BannerAdPosition.BOTTOM_LEFT:
					position = IronSourceBannerPosition.BOTTOM;
				break;
			}
			return position;
		}
		#endif

		// Banner ad bg color.
		public string GetBannerBgColor() {
			return bannerBGColor;
		}
	}
}
