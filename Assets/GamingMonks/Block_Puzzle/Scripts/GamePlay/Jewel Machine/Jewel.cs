using UnityEngine;
using TMPro;
using GamingMonks;
using GamingMonks.UITween;
using UnityEngine.UI;

public class Jewel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_txtCounter;
    [SerializeField] private Image m_jewelImage;

    private int m_remainingCounter = 9;
    private int m_counter = 0;
    private bool m_flag = true;
    private Block m_parentBlock = null;

    private void Awake()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
        rectTransform.sizeDelta = new Vector2(size-6, size-6);
    }
  
    private void OnEnable()
    {
        m_jewelImage.material.SetFloat("_GrayscaleAmount", 0);
        GamePlayUI.OnShapePlacedEvent += GamePlayUI_OnShapePlacedEvent;
    }

   
    private void OnDisable()
    {
        GamePlayUI.OnShapePlacedEvent -= GamePlayUI_OnShapePlacedEvent;
    }

    /// <summary>
    /// Sets jewel counter
    /// </summary>
    /// <param name="remainCounter"></param>
    public void SetCounter(int remainCounter)
    {
        m_remainingCounter = m_counter = remainCounter;
        m_txtCounter.text = remainCounter.ToString();
    }

    public void SetParentBlock(Block block)
    {
        m_parentBlock = block;
    }

    /// <summary>
    /// will get called every time on shape placed
    /// reduces counter on jewel
    /// Changes the sprite of this.jewel and spritetype if remaining counter less than 1
    /// </summary>
    private void GamePlayUI_OnShapePlacedEvent()
    {
        if (m_parentBlock.spriteType == SpriteType.ColouredJewel || m_parentBlock.spriteType == SpriteType.UnColouredJewel || 
            m_parentBlock.spriteType == SpriteType.Bubble && m_parentBlock.secondarySpriteType == SpriteType.ColouredJewel ||
            m_parentBlock.spriteType == SpriteType.Bubble && m_parentBlock.secondarySpriteType == SpriteType.UnColouredJewel)
        {
            m_remainingCounter -= 1;
            m_txtCounter.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
            {
                m_txtCounter.text = m_remainingCounter.ToString();
                m_txtCounter.transform.LocalScale(Vector3.one, 0.25F).OnComplete(() =>
                {
                    if (m_remainingCounter < 1)
                    {
                        m_remainingCounter = m_counter;
                        m_txtCounter.text = m_remainingCounter.ToString();
                        if (m_flag)
                        {
                            if(m_parentBlock.spriteType == SpriteType.Bubble)
                            {
                                m_parentBlock.secondarySpriteType = SpriteType.UnColouredJewel;
                            } 
                            else
                            {
                                m_parentBlock.SetBlockSpriteType(SpriteType.UnColouredJewel);
                            }
                            GamePlay.Instance.blockers.Remove(m_parentBlock);
                            m_jewelImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.UnColouredJewel.ToString());
                            //m_jewelImage.color = Color.white;
                            //m_jewelImage.color = Color.gray;
                            m_flag = false;
                        }
                        else
                        {
                            m_flag = true;
                            if (m_parentBlock.spriteType == SpriteType.Bubble)
                            {
                                m_parentBlock.secondarySpriteType = SpriteType.ColouredJewel;
                            }
                            else
                            {
                                m_parentBlock.SetBlockSpriteType(SpriteType.ColouredJewel);
                            }
                            
                            GamePlay.Instance.blockers.Add(m_parentBlock);
                            m_jewelImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.ColouredJewel.ToString());
                        }
                    }
                });
            });
        }
    }
}
