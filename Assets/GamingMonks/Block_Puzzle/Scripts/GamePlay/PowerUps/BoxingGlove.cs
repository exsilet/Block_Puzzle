using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

namespace GamingMonks
{
    public class BoxingGlove : Singleton<BoxingGlove>, IPointerDownHandler, IBeginDragHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Animator m_animator;
        [SerializeField] private Image m_gloveImage;
        [SerializeField] private BlockShapesController m_blockShapesController;
        [SerializeField] private GameObject cancelSprite;
        [SerializeField] private Button m_gloveButton;
        //[SerializeField] private TextMeshProUGUI m_levelText;
        [SerializeField] private RectTransform m_gloveHolder;

        private bool m_canClear = false;
        private Vector2 m_initaialPosition;
        private List<int> highlightRows = new List<int>();
        private List<Block> m_colliderDisabledBlock = new List<Block>();

        private float m_rayCastMultiplier = 0.2f;
        private int m_level;
        private int m_totalRowCleared;
        private int m_level1RowClearedCount = 5;
        private int m_level2RowClearedCount = 10;
        private int m_level3RowClearedCount = 15;

        [Header("Progress Bar")]
        [SerializeField] private Slider m_progressBarSlider;
        private Coroutine m_filledBarCorountine;
        private float m_timeToFillProgressBar = 0.5f;
        private bool m_isActive;
        private bool m_isBoxingGloveAlreadyActive = false;
        private void OnEnable()
        {
            //gameObject.GetComponent<Canvas>().sortingOrder = -1;
        }

        private void Start()
        {
            m_initaialPosition = transform.localPosition;
            ResetBoxingGlove();
        }

        public bool IsBoxingGloveAlreadyActivated()
        {
            return m_isBoxingGloveAlreadyActive;
        }

        public void OnBoxingGloveButtonPress()
        {
            GamePlay.Instance.blockShapeController.ResetAllShapesPosition();
            if (m_level > 0)
            {
                m_isBoxingGloveAlreadyActive = true;
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                GamePlay.Instance.blockShapeController.ToggleBlockShapeContainer(m_isActive);
                cancelSprite.SetActive(!m_isActive);
                if (!m_isActive)
                {
                    gameObject.GetComponent<Canvas>().sortingOrder = 1;
                    UIController.Instance.OpenPowerUpContextPanel((PowerUp)Enum.Parse(typeof(PowerUp),
                        PowerUp.BoxingGlove.ToString() + m_level.ToString()));
                    transform.position = GamePlay.Instance.blockShapeController.GetMiddleBlockShapePosition().position;
                    PowerUpsController.Instance.SetButtonInteractable(m_gloveButton);
                    m_isActive = true;
                }
                else
                {
                    if (TargetController.Instance.isAllTargetsCollected)
                    {
                        return;
                    }
                    else
                    {
                        if (GamePlay.Instance.maxMovesAllowed <= 0 && GamePlay.Instance.movingKitesList.Count <= 0 && !TargetController.Instance.isAllTargetsCollected)
                        {
                            GamePlay.Instance.maxMovesAllowed = 0;
                            GamePlayUI.Instance.TryRescueGame(GameOverReason.OUT_OF_MOVE);
                        }
                    }
                    
                    UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
                    ResetGloveTransform();
                    PowerUpsController.Instance.SetAllButtonsActive();
                    m_isActive = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_level > 0 && m_isActive)
            {
                transform.localScale = Vector3.one * 1.5f;
                transform.position = this.transform.position + new Vector3(0, 1, 0);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(m_level > 0 && m_isActive)
            {
                gameObject.GetComponent<Canvas>().sortingOrder = 4;
                highlightRows.Clear();
                EnableAllCollider();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_level > 0 && m_isActive)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);
                pos = pos + new Vector2(-0.25f, 1);
                this.transform.position = pos;
                HighLightBlock();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_level > 0 && m_isActive)
            {
                DiableAllCollider();
                StopHighlighting();
                if (m_canClear)
                {
                    gameObject.GetComponent<Canvas>().sortingOrder = 1;
                    m_isBoxingGloveAlreadyActive = false;
                    transform.position = new Vector3(-3f, transform.position.y, 0);
                    StartCoroutine(PlayPunchAnimation());
                    ClearRows();
                    PowerUpsController.Instance.PowerUpsUsedCount++;
                    AnalyticsManager.Instance.BoxingGlovePowerUpEvent();
                    //FBManeger.Instance.PowerUpsUsed("Boxing_Glove");
                    m_canClear = false;

                    cancelSprite.SetActive(false);
                    if (UIController.Instance.topPanelWithModeContext.settingsPopUp.activeSelf)
                    {
                        UIController.Instance.topPanelWithModeContext.settingsPopUp.SetActive(false);
                    }
                    GamePlay.Instance.blockShapeController.ToggleBlockShapeContainer(true);
                    UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
                    PowerUpsController.Instance.SetAllButtonsActive();
                    StartCoroutine(CheckForOutOfMove());                  
                }
                else
                {
                    transform.position = GamePlay.Instance.blockShapeController.GetMiddleBlockShapePosition().position;
                    transform.localScale = Vector3.one;
                }
            }
        }

        IEnumerator CheckForOutOfMove()
        {
            yield return new WaitForSeconds(1f);
            if (TargetController.Instance.isAllTargetsCollected)
            {
                yield break;
            }
            else
            {
                if (GamePlay.Instance.maxMovesAllowed <= 0 && GamePlay.Instance.movingKitesList.Count <= 0 && !TargetController.Instance.isAllTargetsCollected)
                {
                    GamePlay.Instance.maxMovesAllowed = 0;
                    GamePlayUI.Instance.TryRescueGame(GameOverReason.OUT_OF_MOVE);
                }
            }
        }

        private void ResetGloveTransform()
        {
            //gameObject.GetComponent<Canvas>().sortingOrder = 1;
            transform.localPosition = m_initaialPosition;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Will get called on level complete
        /// </summary>
        public void ResetBoxingGlove()
        {
            m_progressBarSlider.minValue = 0;
            m_progressBarSlider.maxValue = m_level3RowClearedCount;
            m_progressBarSlider.value = 0;

            if(m_filledBarCorountine != null)
            {
                StopCoroutine(m_filledBarCorountine);
                m_filledBarCorountine = null;
            }

            m_level = 0;
            m_totalRowCleared = 0;
            ResetGloveTransform();
            UpdateBoxingGloveSprite();
            //m_levelText.text = "";
            cancelSprite.SetActive(false);
            m_isActive = false;
        }

        public void SetSortingOrder()
        {
            gameObject.GetComponent<Canvas>().sortingOrder = 1;
        }

        public bool CanBoxingGloveUse
        {
            get 
            {
                return (m_level > 0) && !m_isActive;
            }
        }

        public void OnRowCleared(int rowCleared)
        {
            UpdateRowCleared(rowCleared);
            UpdateLevel();

            if(m_filledBarCorountine != null)
            {
                StopCoroutine(m_filledBarCorountine);
                m_filledBarCorountine = null;
            }
            m_filledBarCorountine = StartCoroutine(UpdateProgressBar());
        }

        private void UpdateRowCleared(int rowcleared)
        {
            m_totalRowCleared += rowcleared;
            if(m_totalRowCleared >= m_level3RowClearedCount)
            {
                m_totalRowCleared = m_level3RowClearedCount;
            }

            if(m_totalRowCleared < 0)
            {
                m_totalRowCleared = 0;
            }
        }

        private IEnumerator PlayGloveSpriteChangeEffect()
        {
            yield return StartCoroutine(ChangeScale(m_gloveHolder, Vector3.one * 0.7f, 0.20f));
            UpdateBoxingGloveSprite();
            yield return StartCoroutine(ChangeScale(m_gloveHolder, Vector3.one, 0.20f));
        }

        private void UpdateBoxingGloveSprite()
        {
            m_gloveImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag("BoxingGlove" + m_level);
        }

        private IEnumerator ChangeScale(Transform Object, Vector3 a, float time)
        {
            float timeElapced = 0;
            while (timeElapced < time)
            {
                Object.localScale = Vector3.Lerp(Object.localScale, a, timeElapced / time);
                timeElapced += Time.deltaTime;
                yield return null;
            }
            Object.localScale = a;
        }

        private IEnumerator UpdateProgressBar()
        {
            float time = 0;
            while(time < m_timeToFillProgressBar)
            {
                time += Time.deltaTime;
                m_progressBarSlider.value = Mathf.Lerp(m_progressBarSlider.value, m_totalRowCleared, time / m_timeToFillProgressBar);
                yield return null;
            }
            m_progressBarSlider.value = m_totalRowCleared;
        }

        private void UpdateLevel()
        {
            int temp = m_level;
            if (m_totalRowCleared >= m_level3RowClearedCount)
            {
                m_level = 3;
            }
            else if (m_totalRowCleared >= m_level2RowClearedCount && m_totalRowCleared < m_level3RowClearedCount)
            {
                m_level = 2;
            }
            else if (m_totalRowCleared >= m_level1RowClearedCount && m_totalRowCleared < m_level2RowClearedCount)
            {
                m_level = 1;
            }
            else
            {
                m_level = 0;
            }

            if(temp != m_level)
            {
                OnBoxingGloveLevelChanged();
            }
        }

        private void OnBoxingGloveLevelChanged()
        {
            StartCoroutine(PlayGloveSpriteChangeEffect());

            //if (m_level > 0)
            //{
            //    m_levelText.text = "Lv." + m_level.ToString();
            //}
            //else
            //{
            //    m_levelText.text = "";
            //}
        }

        private void OnBoxingGloveUsed()
        {
            if (m_level == 1)
            {
                UpdateRowCleared(-1 * m_level1RowClearedCount);
            }
            else if (m_level == 2)
            {
                UpdateRowCleared(-1 * m_level2RowClearedCount);
            }
            else if (m_level == 3)
            {
                UpdateRowCleared(-1 * m_level3RowClearedCount);
            }

            UpdateLevel();
            m_isActive = false;
            if (m_filledBarCorountine != null)
            {
                StopCoroutine(m_filledBarCorountine);
                m_filledBarCorountine = null;
            }
            m_filledBarCorountine = StartCoroutine(UpdateProgressBar());
        }

        private void ClearRows()
        {
            foreach (int rowID in highlightRows)
            {
                foreach(Block block in GamePlay.Instance.GetEntireRow(rowID))
                {
                    block.isHitByBoxingGlove = true;
                }

                GamePlay.Instance.ClearRow(rowID);

                if (GamePlayUI.Instance.currentGameMode == GameMode.Level && GamePlayUI.Instance.currentLevel.BlockerStick.enabled)
                {
                    StartCoroutine(BlockerStickSpwaner.Instance.DestroyVerticalBlockerStick(rowID));
                }
            }
        }

        IEnumerator PlayPunchAnimation()
        {
            m_animator.enabled = true;
            m_animator.SetTrigger("Punch_Lev" + m_level);
            yield return new WaitForSeconds(0.6f);
            ResetGloveTransform();
            m_animator.enabled = false;
            OnBoxingGloveUsed();
            //PowerUpsController.Instance.CheckBlockShapesCanPlaced();
        }

        private void EnableAllCollider()
        {
            foreach (List<Block> row in GamePlay.Instance.allRows)
            {
                foreach (Block block in row)
                {
                    block.isHitByBoxingGlove = false;
                    if (block.isFilled)
                    {
                        block.thisCollider.enabled = true;
                        m_colliderDisabledBlock.Add(block);
                    }
                }
            }
        }

        private void DiableAllCollider()
        {
            /*
        foreach (Block block in colliderDisabledBlock)
        {
            block.thisCollider.enabled = false;
        }
        */


            foreach (List<Block> row in GamePlay.Instance.allRows)
            {
                foreach (Block block in row)
                {
                    if (block.isFilled || block.hasDiamond)
                    {
                        block.thisCollider.enabled = false;
                        //colliderDisabledBlock.Add(block);
                    }
                }
            }
        }

        private void HighLightBlock()
        {
            //Collider2D[] collidedBlocks = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(this.transform.localScale.x / 3, this.transform.localScale.y / 3), 0);
            Collider2D[] collidedBlocks = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(m_rayCastMultiplier * m_level, m_rayCastMultiplier * m_level), 0);
            if (collidedBlocks.Length <= 0)
            {
                m_canClear = false;
                StopHighlighting();
                return;
            }

            for (int i = 0; i < collidedBlocks.Length; i++)
            {
                Block block = collidedBlocks[i].GetComponent<Block>();
                if (block != null)
                {
                    int rowId = block.RowId;

                    if (!highlightRows.Contains(rowId))
                    {
                        highlightRows.Add(rowId);
                        if (highlightRows.Count > m_level)
                        {
                            while (highlightRows.Count > m_level)
                            {
                                StopHighlightingRow(highlightRows[0]);
                                highlightRows.RemoveAt(0);
                            }
                        }
                    }
                }
            }

            foreach (int row in highlightRows)
            {
                HighlightingRow(row);
            }
            m_canClear = true;
        }

        private void StopHighlightingRow(int rowId)
        {
            foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
            {
                rowBlock.Reset();
            }
        }

        private void HighlightingRow(int rowId)
        {
            foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
            {
                rowBlock.Highlight();
            }
        }

        private void StopHighlighting()
        {
            foreach (int rowId in highlightRows)
            {
                StopHighlightingRow(rowId);
            }
        }
    }
}
