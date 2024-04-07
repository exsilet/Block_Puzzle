using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
namespace GamingMonks
{
    public class GuidScreenForMoves : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private GameObject continueText;
        [SerializeField] private Button cancelButton;

        private void OnEnable()
        {
            cancelButton.interactable = false;
            InputManager.Instance.DisableTouch();
            movesText.text = GamePlay.Instance.maxMovesAllowed.ToString();
            StartCoroutine(ContinueTextAppearCoroutine());
        }

        public void OnCancleButtonPressed()
        {
            continueText.Deactivate();
            gameObject.Deactivate();
        }

        IEnumerator ContinueTextAppearCoroutine()
        {
            yield return new WaitForSecondsRealtime(3f);
            continueText.Activate();
            InputManager.Instance.EnableTouch();
            cancelButton.interactable = true;
        }
    }
}
