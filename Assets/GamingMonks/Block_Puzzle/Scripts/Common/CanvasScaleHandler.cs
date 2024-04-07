using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	public class CanvasScaleHandler : MonoBehaviour 
	{
		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		private void Awake() {

			float screenAspect = 0.0F;

			if(Screen.height > Screen.width) {
				screenAspect = (((float) Screen.height) / ((float) Screen.width));
			} else {
				screenAspect = (((float) Screen.width) / ((float) Screen.height));			
			}

			if(screenAspect < 1.75F) {
				GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0F;
			} else {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5F;
            }
		}
	}
}