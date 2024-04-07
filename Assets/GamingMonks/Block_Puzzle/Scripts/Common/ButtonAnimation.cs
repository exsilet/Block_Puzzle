using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	/// <summary>
	/// Add this script component to any UI Button element to animate on button click event. 
	/// This script will use Animator attached to the button.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class ButtonAnimation : MonoBehaviour 
	{
		[SerializeField] bool doAnimate = true;
		Button thisButton;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake()
		{
			thisButton = GetComponent<Button>();
			if(GetComponent<Animator>() == null) {
				doAnimate = false;
			}
		}

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start()
		{
			thisButton.onClick.AddListener(()=> {
				if(doAnimate) {
					thisButton.GetComponent<Animator>().SetTrigger("Press");
				}
				UIFeedback.Instance.PlayButtonPressEffect();
			});
		}
	}
}