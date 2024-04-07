using UnityEngine;

namespace GamingMonks
{
	/// <summary>
	/// This script component can be added to any canvas to handle safe area or notch.
	/// </summary>
	public class CanvasSafeAreaHandler : MonoBehaviour 
	{
		[SerializeField] Vector2 offsetMin = Vector2.zero;
		[SerializeField] Vector2 offsetMax = Vector2.zero;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>		
		private void Awake() {
			float bottomSafeArea = Screen.safeArea.y;
			float topSafeArea = Screen.height - ( bottomSafeArea + Screen.safeArea.height);

			if(topSafeArea > 0) {
				GetComponent<RectTransform>().offsetMin = offsetMin;
				GetComponent<RectTransform>().offsetMax = offsetMax;
			}
		}
	}
}
