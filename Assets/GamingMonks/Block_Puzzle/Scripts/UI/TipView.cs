using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	public class TipView : MonoBehaviour 
	{
		#pragma warning disable 0649
		[SerializeField] RectTransform tipContent;
		[SerializeField] Text txtTip;
		#pragma warning restore 0649

		public void ShowTipAtPosition(Vector2 tipPosition, Vector2 anchor, string tipText) {
			if(IsInvoking("HideTip")) {
				CancelInvoke("HideTip");
			}
			tipContent.anchorMax = anchor;
			tipContent.anchorMin = anchor;
			tipContent.pivot = anchor;
			tipContent.anchoredPosition = tipPosition;
			txtTip.text = tipText;
		}

		public void ShowTipAtPosition(Vector2 tipPosition, Vector2 anchor, string tipText, float duration) {
			if(IsInvoking("HideTip")) {
				CancelInvoke("HideTip");
			}
			
			tipContent.anchorMax = anchor;
			tipContent.anchorMin = anchor;
			tipContent.pivot = anchor;
			tipContent.anchoredPosition = tipPosition;
			txtTip.text = tipText;

			if(!IsInvoking("HideTip")) {
				Invoke("HideTip", duration);
			}
		}


		void HideTip() {
			gameObject.Deactivate();
		}
	}
}

