using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
	/// <summary>
	/// This script component will animate the new added score and will deappear after 1 second.
	/// </summary>
    public class ScoreAnimator : MonoBehaviour
    {	
        #pragma warning disable 0649
		// Animating text.
        [SerializeField] Text txtAnimatingScore;

		// Animator controller for the text.
        [SerializeField] Animator scoereAnim;
        #pragma warning restore 0649

		// Plays animation effect with given score amount.
        public void Animate(int score)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            mousePos.y += 1;
            transform.position = mousePos;

            txtAnimatingScore.text = score.ToString();
            scoereAnim.SetTrigger("Animate");
        }
    }
}