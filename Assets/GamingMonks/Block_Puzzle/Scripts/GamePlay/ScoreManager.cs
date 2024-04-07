using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamingMonks.UITween;
using TMPro;

namespace GamingMonks
{
    /// <summary>
    /// Handled the game score.
    /// </summary>
	public class ScoreManager : MonoBehaviour
    {
        #pragma warning disable 0649
        // Text displays score.
        [SerializeField] private TextMeshProUGUI txtScore;

        // Text displays best score for selected mode.
        [SerializeField] private TextMeshProUGUI txtBestScore;
        #pragma warning restore 0649

        int Score = 0;
        int blockScore = 0;
        int singleLineBreakScore = 0;
        int multiLineScoreMultiplier = 0;
        int bestScore = 0;

        // Yield instruction for the score countet iterations.
        WaitForSeconds scoreIterationWait = new WaitForSeconds(0.02F);

        #pragma warning disable 0649
        [SerializeField] private ScoreAnimator scoreAnimator;
        #pragma warning restore 0649
        

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        void OnEnable()
        {
            /// Registers game status callbacks.
            GamePlayUI.OnGameStartedEvent += GamePlayUI_OnGameStartedEvent;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            /// Unregisters game status callbacks.
            GamePlayUI.OnGameStartedEvent -= GamePlayUI_OnGameStartedEvent;
        }

        /// <summary>
        /// Set best score onn game start. 
        /// </summary>
        private void GamePlayUI_OnGameStartedEvent(GameMode currentGameMode)
        {
            #region score data to local members
            blockScore = GamePlayUI.Instance.blockScore;
            singleLineBreakScore = GamePlayUI.Instance.singleLineBreakScore;
            multiLineScoreMultiplier = GamePlayUI.Instance.multiLineScoreMultiplier;
            #endregion

            if (GamePlayUI.Instance.progressData != null)
            {
                Score += GamePlayUI.Instance.progressData.score;
            }
            txtScore.text = Score.ToString("N0");
            bestScore = ProfileManager.Instance.GetBestScore(GamePlayUI.Instance.currentGameMode);
            txtBestScore.text = bestScore.ToString("N0");
        }

        /// <summary>
        /// Adds score based on calculation and bonus.
        /// </summary>
        public void AddScore(int linesCleared, int clearedBlocks)
        {
            int scorePerLine = singleLineBreakScore + ((linesCleared - 1) * multiLineScoreMultiplier);
            int scoreToAdd = ((linesCleared * scorePerLine) + (clearedBlocks * blockScore));

            int oldScore = Score;
            Score += scoreToAdd;

            PlayerPrefs.SetInt("rescueScreenScore", Score);

            StartCoroutine(SetScore(oldScore, Score));
            scoreAnimator.Animate(scoreToAdd);

            if (Score > bestScore)
            {
                ProfileManager.Instance.SetBestScore(Score, GamePlayUI.Instance.currentGameMode);
                StartCoroutine(SetHighScore(bestScore, Score));
                bestScore = Score;
            }
        }


        void ShowScoreAnimation() {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            mousePos.z = 0;
            scoreAnimator.transform.position = mousePos;
            // txtAnimatedText.text = "+" + 100.ToString ();
            
        }

        /// <summary>
        /// Returns score for the current game mode.
        /// </summary>
        public int GetScore()
        {
            return Score;
        }

        /// <summary>
        /// Set score with countetr animatio effect.
        /// </summary>
        IEnumerator SetScore(int lastScore, int currentScore)
        {
            int IterationSize = (currentScore - lastScore) / 10;
            txtScore.transform.LocalScale(Vector3.one * 1.2F, 0.2F).OnComplete(() =>
            {
                txtScore.transform.LocalScale(Vector3.one, 0.2F);
            });

            for (int index = 1; index < 10; index++)
            {
                lastScore += IterationSize;
                txtScore.text = lastScore.ToString("N0");
                AudioController.Instance.PlayClipLow(AudioController.Instance.addScoreSoundChord, 0.15F);
                yield return scoreIterationWait;
            }
            txtScore.text = currentScore.ToString("N0");
        }

        IEnumerator SetHighScore(int lastScore, int currentScore)
        {
            int IterationSize = (currentScore - lastScore) / 10;
            txtBestScore.transform.LocalScale(Vector3.one * 1.2F, 0.2F).OnComplete(() =>
            {
                txtBestScore.transform.LocalScale(Vector3.one, 0.2F);
            });

            for (int index = 1; index < 10; index++)
            {
                lastScore += IterationSize;
                txtBestScore.text = lastScore.ToString("N0");
                AudioController.Instance.PlayClipLow(AudioController.Instance.addScoreSoundChord, 0.15F);
                yield return scoreIterationWait;
            }
            txtBestScore.text = currentScore.ToString("N0");
        }

        /// <summary>
        /// Resets score on game over, game quit.
        /// </summary>
        public void ResetGame()
        {
            txtScore.text = "0";
            txtBestScore.text = "0";
            Score = 0;
        }
    }
}