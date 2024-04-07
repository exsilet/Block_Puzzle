using UnityEngine;
using UnityEngine.UI;
using GamingMonks;
using GamingMonks.Utils;

public class JewelMoverBlock : MonoBehaviour
{
    [SerializeField] private Image m_blockImage;
    [SerializeField] private Image m_blockLayerImage1;
    [SerializeField] private Image m_blockLayerImage2;

    private Vector3 m_initialPosition;
    private bool m_hasStages = false;
    private int m_stage = 0;
    private string m_spriteTag = "";

    private bool m_enabled1 = false;
    private bool m_enabled2 = false;
    public bool m_hasJewel { get; private set; }
    private bool m_hasBalloonBomb = false;
    private bool m_hasIceBomb = false;
    private bool m_hasDiamond = false;
    private bool m_isPresentInBlockers = false;

    private SpriteType m_spriteType = SpriteType.Empty;
    private SpriteType m_secondarySpriteType = SpriteType.Empty;

    private Jewel m_jewel = null;
    private BalloonBomb m_balloonBomb = null;
    private IceBomb m_iceBomb = null;
    private Diamond m_diaMond = null;
    private Block m_block = null;

    public bool isFilled { get; private set; }
    [SerializeField] private float m_speed;
    private Block m_destination;
    private Vector3 m_direction;
    private float m_distance;
    private float m_timer;

    private void Awake()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
        rectTransform.sizeDelta = new Vector2(size, size);
    }

    private void OnEnable()
    {
        m_initialPosition = m_block.gameObject.transform.position;
    }

    private void OnDisable()
    {
        ResetMoverBlock();
    }

    /// <summary>
    /// Sets all properties of the Block which is about to move
    /// </summary>
    public void SetJewelMoverBlock()
    {
        if(m_block.blockImage.sprite != null)
        {
            m_blockImage.enabled = true;
            m_blockImage.sprite = m_block.blockImage.sprite;
            m_blockImage.color = m_blockImage.color.WithNewA(1);
        }
       
        m_spriteTag = m_block.spriteType == SpriteType.Empty ? m_block.assignedSpriteTag : m_block.spriteType.ToString();

        if (m_block.spriteType == SpriteType.MagnetWithBubble)
        {
            m_spriteType = SpriteType.Magnet;
            m_secondarySpriteType = SpriteType.Bubble;
        }
        else
        {
            m_spriteType = m_block.spriteType;
            m_secondarySpriteType = m_block.secondarySpriteType;
        }

        m_hasStages = m_block.hasStages;
        m_stage = m_block.stage;

        if (m_block.blockImageLayer1.enabled == true)
        {
            m_enabled1 = true;
            m_blockLayerImage1.enabled = true;
            m_blockLayerImage1.sprite = m_block.blockImageLayer1.sprite;
            m_blockLayerImage1.color.WithNewA(1);
        }

        if (m_block.blockImageLayer2.enabled && m_block.spriteType != SpriteType.Bubble)
        {
            m_enabled2 = true;
            m_blockLayerImage2.enabled = true;
            m_blockLayerImage2.sprite = m_block.blockImageLayer2.sprite;
            m_blockLayerImage2.color.WithNewA(1);
        }
       
        if (m_block.hasJewel)
        {
            m_hasJewel = true;
            m_jewel = m_block.jewel;
            m_jewel.transform.SetParent(m_blockImage.transform);
            m_jewel.transform.localPosition = Vector3.zero;
            m_jewel.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }

        if (m_block.isBalloonBomb)
        {
            m_hasBalloonBomb = true;
            m_balloonBomb = m_block.thisBalloonBomb;
            m_balloonBomb.transform.SetParent(m_blockImage.transform);
            m_balloonBomb.transform.localPosition = Vector3.zero;
            m_balloonBomb.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }

        if (m_block.isIceBomb)
        {
            if (m_block.thisIceBomb != null)
            {
                m_hasIceBomb = true;
                m_iceBomb = m_block.thisIceBomb;
                m_iceBomb.transform.SetParent(m_blockImage.transform);
                m_iceBomb.transform.localPosition = Vector3.zero;
                m_iceBomb.transform.localScale = Vector3.one;
                m_blockImage.enabled = false;
            }
        }

        if (m_block.hasDiamond)
        {
            m_hasDiamond = true;
            m_diaMond = m_block.diamond;
            m_diaMond.ClearDiamond();
            m_diaMond.transform.localPosition = Vector3.zero;
        }

        if (GamePlay.Instance.blockers.Contains(m_block))
        {
            m_isPresentInBlockers = true;
            GamePlay.Instance.blockers.Remove(m_block);
        }
    }

   /// <summary>
   /// Sets the destination for the movement
   /// </summary>
   /// <param name="destination"> Destination block </param>
    public void SetDestination(Block destination)
    {
        m_block.ResetBlock();
        this.m_destination = destination;
    }

    /// <summary>
    /// Sets reference of block 
    /// (this Mover block moves the block - reference)
    /// </summary>
    /// <param name="block"> mover block moves this.block </param>
    public void SetReferenceBlock(Block block)
    {
        m_block = block;
    }
   
    /// <summary>
    /// Moves Mover Block to destination
    /// </summary>
    private void Update()
    {
        m_distance = Vector3.Distance(transform.position, m_destination.transform.position);
        if (m_distance > 0.1f)
        {
            m_direction = (m_destination.transform.position - transform.position).normalized;
            transform.Translate(m_direction * m_speed * Time.deltaTime);

            m_timer += Time.deltaTime;
            if(m_timer > 0.1)
            {
                transform.localScale = Vector3.one - new Vector3(0.1f, 0.1f, 0);
            }
        } 
        else
        {
            PlaceBlock();
        }
    }

    /// <summary>
    /// Places this Mover block on the destination block and Sets all the properties  
    /// </summary>
    private void PlaceBlock()
    {
        m_destination.hasStages = m_hasStages;
        m_destination.stage = m_stage;

        if (m_enabled1 == true)
        {
            m_destination.blockImageLayer1.enabled = true;
            m_destination.blockImageLayer1.sprite = m_blockLayerImage1.sprite;
            m_destination.blockImageLayer1.color = m_destination.blockImageLayer1.color.WithNewA(1);
        }

        if (m_enabled2 == true)
        {
            m_destination.blockImageLayer2.enabled = true;
            m_destination.blockImageLayer2.sprite = m_blockLayerImage2.sprite;
            m_destination.blockImageLayer2.color = m_destination.blockImageLayer2.color.WithNewA(1);
        }

        if (m_hasJewel)
        {
            m_destination.hasJewel = true;
            m_jewel.transform.SetParent(null);
            m_destination.blockImage.enabled = false;
            m_jewel.transform.SetParent(m_destination.blockImage.transform);
            m_destination.jewel = m_jewel;
            m_destination.jewel.transform.localScale = Vector3.one;
            m_destination.jewel.transform.localPosition = Vector3.zero;
            m_destination.isFilled = true;
            m_destination.isAvailable = false;
            m_destination.thisCollider.enabled = false;
            //m_destination.jewel.m_parentBlock = m_destination;
            m_destination.jewel.SetParentBlock(m_destination);
            if (m_spriteType == SpriteType.ColouredJewel)
                GamePlay.Instance.blockers.Add(m_destination);
        }
        else if (m_hasBalloonBomb)
        {
            if (m_balloonBomb != null)
            {
                m_destination.isBalloonBomb = true;
                m_balloonBomb.transform.SetParent(null);
                m_destination.blockImage.enabled = false;
                m_balloonBomb.transform.SetParent(m_destination.blockImage.transform);
                m_destination.thisBalloonBomb = m_balloonBomb;
                m_destination.thisBalloonBomb.transform.localScale = Vector3.one;
                m_destination.thisBalloonBomb.transform.localPosition = Vector3.zero;
                m_destination.isFilled = true;
                m_destination.isAvailable = false;
                m_destination.thisCollider.enabled = false;
                m_destination.thisBalloonBomb.block = m_destination;
            }
        }
        else if (m_hasIceBomb)
        {
            if (m_iceBomb != null)
            {
                m_destination.isIceBomb = true;
                m_iceBomb.transform.SetParent(null);
                m_destination.blockImage.enabled = false;
                m_iceBomb.transform.SetParent(m_destination.blockImage.transform);
                m_destination.thisIceBomb = m_iceBomb;
                m_destination.thisIceBomb.transform.localScale = Vector3.one;
                m_destination.thisIceBomb.transform.localPosition = Vector3.zero;
                m_destination.isFilled = true;
                m_destination.isAvailable = false;
                m_destination.thisCollider.enabled = false;
                m_destination.thisIceBomb.block = m_destination;
            }
        }
        else if (m_hasDiamond)
        {
            if (m_diaMond != null)
            {
                m_diaMond.currentBlock = m_destination;
                m_destination.diamond = m_diaMond;
                m_destination.diamond.PlaceDiamond();
                m_destination.blockImage.enabled = false;
            }
        }
        else
        {
            m_destination.PlaceBlock(m_blockImage.sprite, m_spriteTag);
        }

        if (m_spriteType == SpriteType.Bubble && m_destination.spriteType == SpriteType.Bubble)
        {
            m_destination.secondarySpriteType = m_secondarySpriteType;
        }
        else if (m_destination.spriteType != SpriteType.Bubble && m_spriteType == SpriteType.Bubble)
        {
            m_destination.SetBlockSpriteType(m_secondarySpriteType);
            m_destination.secondarySpriteType = SpriteType.Empty;
        }
        else if (m_spriteType == SpriteType.Magnet && m_destination.spriteType == SpriteType.Bubble)
        {
            m_destination.spriteType = SpriteType.MagnetWithBubble;
        }
        else if (m_spriteType != SpriteType.Bubble && m_destination.spriteType == SpriteType.Bubble)
        {
            m_destination.secondarySpriteType = m_spriteType;
        }
        else
        {
            m_destination.spriteType = m_spriteType;
            m_destination.secondarySpriteType = m_secondarySpriteType;
        }

        if (m_isPresentInBlockers)
        {
            GamePlay.Instance.blockers.Add(m_destination);
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// resets all the properties
    /// </summary>
    public void ResetMoverBlock()
    {
        m_hasStages = false;
        m_stage = 0;
        m_enabled1 = false;
        m_enabled2 = false;
        m_spriteType = SpriteType.Empty;
        m_secondarySpriteType = SpriteType.Empty;
        m_spriteTag = "";
        m_blockImage.enabled = false;
        m_blockLayerImage1.enabled = false;
        m_blockLayerImage2.enabled = false;
        m_jewel = null;
        m_hasJewel = false;
        transform.position = m_initialPosition;

        m_hasIceBomb = false;
        m_hasBalloonBomb = false;
        m_balloonBomb = null;
        m_iceBomb = null;
        m_hasDiamond = false;
        m_isPresentInBlockers = false;
        m_diaMond = null;
        isFilled = false;
        m_destination = null;

        if(m_blockImage.transform.childCount > 0)
        {
            for(int i = 0; i < m_blockImage.transform.childCount; i++ )
            {
                Destroy(m_blockImage.transform.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// Places jewel on this moves block
    /// </summary>
    /// <param name="jewelCounter"> Counter on Jewel</param>
    /// <param name="position"> Mover block Position</param>
    public void PlaceJewel(int jewelCounter, Vector3 position)
    {
        this.transform.position = position;
        isFilled = true;
        m_jewel = Instantiate(JewelMachineController.Instance.jewelPrefab, m_blockImage.transform);
        m_blockImage.enabled = false;
        m_jewel.transform.localPosition = Vector3.zero;
        m_jewel.SetCounter(jewelCounter);
        m_jewel.gameObject.SetActive(true);
        m_hasJewel = true;
        if (m_block.spriteType == SpriteType.Bubble)
        {
            m_spriteType = SpriteType.Bubble;
            m_secondarySpriteType = SpriteType.ColouredJewel;
        }
        else
        {
            m_spriteType = SpriteType.ColouredJewel;
        }
    }

    /// <summary>
    /// Places the simple block on this Mover block
    /// </summary>
    /// <param name="machinePosition"> Mover block Position</param>
    public void PlaceBlock(Vector3 machinePosition)
    {
        string spriteTag = GamePlay.Instance.blockShapeController.GetSpriteTag();
        transform.position = machinePosition;
        this.m_spriteTag = spriteTag;
        m_blockImage.enabled = true;
        m_blockImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteTag);
        m_blockImage.color = m_blockImage.color.WithNewA(1);

        if (m_block.spriteType == SpriteType.Bubble)
        {
            m_spriteType = SpriteType.Bubble;
        }
        else
        {
            m_spriteType = SpriteType.Empty;
        }
    }
}
