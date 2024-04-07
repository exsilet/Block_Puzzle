using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GamingMonks
{
    public class LevelSelectionScreen : MonoBehaviour, IEndDragHandler, IDragHandler
    {
        #pragma warning disable 0649
        [SerializeField] Button m_btnLeftScroll;
        [SerializeField] Button m_btnRightScroll;
        #pragma warning restore 0649

        [Header("Swipe Settings")]
        [SerializeField] private GameObject m_pageContainer;
        [SerializeField] private CanvasScaler m_canvasScaler;
        public Vector3 panelLocation;
        public float percentThreshold = 0.2f;
        public float easing = 0.5f;
        public int totalPages = 3;
        public int currentPage = 1;
        public int totalButtons = 25;

        [SerializeField] Material m_lockedMaterial;
        [SerializeField] Material m_unlockedMaterial;

        [SerializeField] LevelSelectionBtn[] m_levelBtnList;
        [SerializeField] Image[] m_pageIndicators;

        LevelSO m_levelSO;

        void Awake()
        {
            if (m_levelSO == null)
            {
                m_levelSO = (LevelSO)Resources.Load("LevelSettings");
            }
            if (!m_btnLeftScroll)
            {
                m_btnLeftScroll.gameObject.SetActive(false);
            }
            if (!m_btnRightScroll)
            {
                m_btnRightScroll.gameObject.SetActive(false);
            }
            //Change Page size for different resolutions.
            for (int i = 0; i < m_pageContainer.transform.childCount; i++)
            {
                UnityEngine.Transform childs = m_pageContainer.transform.GetChild(i);
                RectTransform childRects = childs.GetComponent<RectTransform>();
                childRects.sizeDelta = new Vector2(m_canvasScaler.referenceResolution.x, Screen.height);
                childRects.localPosition = new Vector3(i * m_canvasScaler.referenceResolution.x, 0, 0);
            }
        }

        private void OnEnable()
        {
            // if (!AdmobManager.Instance.GetInterstitialAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadInterstitialAd();
            // }
            panelLocation = m_pageContainer.transform.localPosition;
            UIController.Instance.topPanelWithHearts.gameObject.SetActive(true);
            
            // If powerUp context panel is still active then disable it before opening level selection screen.
            if (UIController.Instance.powerUpContextPanel.gameObject.activeSelf)
            {
                UIController.Instance.powerUpContextPanel.gameObject.Deactivate();
            }
            // If powerUp context panel is still active then disable it before opening level selection screen.
            if (UIController.Instance.gameRetryAndQuitScreen.gameObject.activeSelf)
            {
                UIController.Instance.gameRetryAndQuitScreen.gameObject.Deactivate();
            }
            
            
            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
            {
                PlayerPrefs.SetInt("CurrentLevel", 1);
            }

            RefreshScreen();
            MoveToCurrentPanel();
        }

        public void OnDrag(PointerEventData data)
        {
            float difference = Camera.main.ScreenToWorldPoint(data.pressPosition).x - Camera.main.ScreenToWorldPoint(data.position).x; ;
            m_pageContainer.transform.localPosition = panelLocation - new Vector3(difference, 0, 0);
        }

        public void OnEndDrag(PointerEventData data)
        {
            float percentage = Camera.main.ScreenToWorldPoint(data.pressPosition).x - Camera.main.ScreenToWorldPoint(data.position).x / Screen.width;
            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector3 newLocation = panelLocation;
                if (percentage > 0 && currentPage < totalPages)
                {
                    currentPage++;
                    newLocation += new Vector3(-m_canvasScaler.referenceResolution.x, 0, 0);
                }
                else if (percentage < 0 && currentPage > 1)
                {
                    currentPage--;
                    newLocation += new Vector3(m_canvasScaler.referenceResolution.x, 0, 0);
                }
                StartCoroutine(SmoothMove(m_pageContainer.transform.localPosition, newLocation, easing));
                panelLocation = newLocation;
            }
            else
            {
                StartCoroutine(SmoothMove(m_pageContainer.transform.localPosition, panelLocation, easing));
            }
            PageIndicatorSetup();
        }

        IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
        {
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                m_pageContainer.transform.localPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        /// <summary>
        /// Action on pressing Left Scroll button on level selection screen.
        /// </summary>
        public void OnLeftScrollButtonPressed()
        {
            Vector3 newLocation = panelLocation;
            if (currentPage > 1)
            {
                currentPage--;
                newLocation += new Vector3(m_canvasScaler.referenceResolution.x, 0, 0);
            }
            StartCoroutine(SmoothMove(m_pageContainer.transform.localPosition, newLocation, easing));
            panelLocation = newLocation;
            RefreshScreen();
            PageIndicatorSetup();
        }

        /// <summary>
        /// Action on pressing Right Scroll button on level selection screen.
        /// </summary>
        public void OnRightScrollButtonPressed()
        {
            Vector3 newLocation = panelLocation;
            if (currentPage < totalPages)
            {
                currentPage++;
                newLocation += new Vector3(-m_canvasScaler.referenceResolution.x, 0, 0);
            }
            StartCoroutine(SmoothMove(m_pageContainer.transform.localPosition, newLocation, easing));
            panelLocation = newLocation;
            RefreshScreen();
            PageIndicatorSetup();
        }

        /// <summary>
        /// Move Page to the Page which has the Current Level.
        /// </summary>
        public void MoveToCurrentPanel()
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
            Vector3 newLocation = panelLocation - new Vector3(-m_canvasScaler.referenceResolution.x * (currentPage - 1), 0, 0);
            for (int i = 1; i <= totalPages; i++)
            {              
                if(((currentLevel) > (totalButtons * (i - 1))) && ((currentLevel) <= (totalButtons * i)))
                {
                    currentPage = i ;
                    newLocation += new Vector3(-m_canvasScaler.referenceResolution.x * (i - 1), 0, 0);
                }            
            }
            StartCoroutine(SmoothMove(m_pageContainer.transform.localPosition, newLocation, easing));
            panelLocation = newLocation;
            //RefreshScreen();
            PageIndicatorSetup();
        }

        /// <summary>
        /// Sets up the page indicator for the button panels.
        /// </summary>
        private void PageIndicatorSetup()
        {
            int pageNumber = currentPage;
            if (pageNumber > 5 && pageNumber <= 10)
            {
                pageNumber -= 5;
            }
            else if (pageNumber > 10 && pageNumber <= 15)
            {
                pageNumber -= 10;
            }
            else
            {
                //take normal page number.
            }
            for (int i = 0; i < m_pageIndicators.Length; i++)
            {
                if (i == pageNumber - 1)
                {
                    m_pageIndicators[i].sprite = ThemeManager.Instance.GetBlockSpriteWithTag("CurrentPage");
                }
                else
                {
                    m_pageIndicators[i].sprite = ThemeManager.Instance.GetBlockSpriteWithTag("RemainingPages");
                }
            }
            ToggleScrollButtons();
        }

        /// <summary>
        /// Refresh the level selection scrren.
        /// </summary>
        public void RefreshScreen()
        {
            //int totalLevels = levelSO.Levels.Length;
            //totalPages = (int)(totalLevels / levelBtnList.Length);
            //int index = currentPage * levelBtnList.Length;
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
            for (int i = 0; i < m_levelBtnList.Length; i++)
            {
                if (i < currentLevel - 1)
                {
                    m_levelBtnList[i].GetComponent<Button>().enabled = true;
                    m_levelBtnList[i].SetupButton(i + 1, ThemeManager.Instance.GetBlockSpriteWithTag("UnlockedAndCompleted"), false, m_unlockedMaterial);
                }
                else if (i == currentLevel - 1)
                {
                    m_levelBtnList[i].GetComponent<Button>().enabled = true;
                    m_levelBtnList[i].SetupButton(i + 1, ThemeManager.Instance.GetBlockSpriteWithTag("UnlockedAndIncompleted"), false, m_unlockedMaterial);
                }
                else if (i > currentLevel - 1)
                {
                    m_levelBtnList[i].GetComponent<Button>().enabled = false;
                    m_levelBtnList[i].SetupButton(i + 1, ThemeManager.Instance.GetBlockSpriteWithTag("LockedLevel"), true, m_lockedMaterial);
                }
            }
            ToggleScrollButtons();
        }


        public void ToggleScrollButtons()
        {
            m_btnLeftScroll.gameObject.SetActive(currentPage > 1);
            m_btnRightScroll.gameObject.SetActive(currentPage < totalPages);
        }

        public void GetFiveExtraCoinsOnLevelSelectionScreenBtnPressed()
        {
            // if (!AdmobManager.Instance.GetInterstitialAd().IsLoaded())
            // {
            //     AdmobManager.Instance.RequestAndLoadInterstitialAd();
            // }
            AdmobManager.Instance.ShowRewardedAd(RewardAdType.GetFreeExtraCoinsFromLevelSelectionPanel);
        }
    }
}

