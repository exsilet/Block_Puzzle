using GamingMonks.Localization;
using UnityEngine;
using UnityEngine.UI;
using GamingMonks.UITween;

namespace GamingMonks
{
	public class InGameMessage : MonoBehaviour 
	{
		public AnimationCurve animationCurve;
		public GameObject messageView;
		public Text txtMessageText;

		public void ShowMessage(GameOverReason reason) 
		{
			messageView.transform.localScale = Vector3.zero;
			txtMessageText.text = GetRescueReason(reason);

			messageView.gameObject.SetActive(true);
			messageView.transform.LocalScale(Vector3.one, 0.2F).SetAnimation(animationCurve).OnComplete(()=> {
				messageView.transform.LocalScale(Vector3.zero, 0.2F).SetAnimation(animationCurve).SetDelay(1F);
			});
		}

		public string GetRescueReason(GameOverReason reason)
        {
            switch (reason)
            {
                case GameOverReason.GRID_FILLED:
                    return LocalizationManager.Instance.GetTextWithTag("txtGameOver_gridfull");

                case GameOverReason.BOMB_BLAST:
                    return LocalizationManager.Instance.GetTextWithTag("txtGameOver_bombexplode");

                case GameOverReason.TIME_OVER:
                    return LocalizationManager.Instance.GetTextWithTag("txtGameOver_timeover");

                case GameOverReason.OUT_OF_MOVE:
	                return LocalizationManager.Instance.GetTextWithTag("txtGameOver_outofmove");
                
				default:
					return LocalizationManager.Instance.GetTextWithTag("txtGameOver_gridfull");
            }
        }
	}
}

