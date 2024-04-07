using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GamingMonks;
using GamingMonks.Features;
using GamingMonks.Utils;

public class ConveyorMoverBlock : MonoBehaviour
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
    private bool m_hasJewel = false;
    private bool m_hasBalloonBomb = false;
    private bool m_hasIceBomb = false;
    private bool m_hasDiamond = false;
    private bool m_isPresentInBlockers = false;
    private bool m_hasIceMachine = false;

    private SpriteType m_spriteType = SpriteType.Empty;
    private SpriteType m_secondarySpriteType = SpriteType.Empty;

    private Jewel m_jewel = null;
    private BalloonBomb m_balloonBomb = null;
    private IceBomb m_iceBomb = null;
    private Diamond m_diaMond = null;
    private Block m_block = null;
    private MusicalPlayer m_MusicalPlayer;
    private MagicHat m_MagicHat;
    private IceMachine m_IceMachine = null;

    private WaitForSeconds m_waitForToMove = new WaitForSeconds(0.01f);

    private void Awake()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
        rectTransform.sizeDelta = new Vector2(size, size);
    }

    private void OnEnable()
    {
        m_initialPosition = m_block.gameObject.transform.position;
        transform.position = m_block.gameObject.transform.position;
    }

    private void OnDisable()
    {
        ResetConveyorBlock();
    }

    /// <summary>
    /// Sets all properties of the Block which is about to move on This MoverBlock
    /// </summary>
    public void SetConveyorBlock()
    {
        m_blockImage.enabled = true;
        m_blockImage.sprite = m_block.blockImage.sprite;
        //m_blockImage.color = m_block.blockImage.color.WithNewA(1);
        //m_blockImage.color.WithNewA(1);
        m_blockImage.color = m_block.blockImage.color;

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
            //m_blockLayerImage1.color.WithNewA(1);
            m_blockLayerImage1.color = m_block.blockImageLayer1.color;
        }

        if (m_block.blockImageLayer2.enabled && m_block.spriteType != SpriteType.Bubble)
        {
            m_enabled2 = true;
            m_blockLayerImage2.enabled = true;
            m_blockLayerImage2.sprite = m_block.blockImageLayer2.sprite;
            //m_blockLayerImage2.color.WithNewA(1);
            m_blockLayerImage2.color = m_block.blockImageLayer2.color;
        }

        if(m_block.hasJewel)
        {
            m_hasJewel = true;
            m_jewel = m_block.jewel;
            m_jewel.transform.SetParent(m_blockImage.transform);
            m_jewel.transform.localPosition = Vector3.zero;
            m_jewel.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }
           
        if(m_block.isBalloonBomb)
        {
            m_hasBalloonBomb = true;
            m_balloonBomb = m_block.thisBalloonBomb;
            m_balloonBomb.transform.SetParent(m_blockImage.transform);
            m_balloonBomb.transform.localPosition = Vector3.zero;
            m_balloonBomb.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }

        if(m_block.isIceBomb)
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
            m_blockImage.enabled = false;
            m_diaMond = m_block.diamond;
            m_diaMond.ClearDiamond();
        }

        if (m_block.thisMusicalPlayer != null)
        {
            m_MusicalPlayer = m_block.thisMusicalPlayer;
            m_MusicalPlayer.transform.SetParent(m_blockImage.transform);
            m_MusicalPlayer.transform.localPosition = Vector3.zero;
            m_MusicalPlayer.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }

        if (m_block.thisMagicHat != null)
        {
            m_MagicHat = m_block.thisMagicHat;
            m_MagicHat.transform.SetParent(m_blockImage.transform);
            m_MagicHat.transform.localPosition = Vector3.zero;
            m_MagicHat.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }

        if (m_block.isIceMachine)
        {
            m_hasIceMachine = true;
            m_IceMachine = m_block.thisIceMachine;
            m_IceMachine.transform.SetParent(m_blockImage.transform);
            m_IceMachine.transform.localPosition = Vector3.zero;
            m_IceMachine.transform.localScale = Vector3.one;
            m_blockImage.enabled = false;
        }

        if (GamePlay.Instance.blockers.Contains(m_block))
        {
            m_isPresentInBlockers = true;
            GamePlay.Instance.blockers.Remove(m_block);
        }
    }

    /// <summary>
    /// Moves this Mover block to the Destination block
    /// will get called only for BlockConveyor
    /// </summary>
    /// <param name="destination"> destination block </param>
    public void Move(Block destination)
    {
        StartCoroutine(MoveBlock(destination));
    }

    /// <summary>
    /// Moves this Mover block till the destination
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    public IEnumerator MoveBlock(Block destination)
    {
        m_block.ResetBlock();
        float time = 0;
        Vector3 pos = transform.position;
        //original time = 0.20
        while (time < 0.15f)
        {
            //yield return m_waitForToMove;
            yield return null;
            time += Time.deltaTime;
            //transform.position = Vector3.Lerp(transform.position, destination.transform.position, time);
            transform.position = Vector3.Lerp(pos, destination.transform.position, time / 0.15f);
        }

        PlaceBlock(destination);
    }

   /// <summary>
   /// Places this Mover block on Destination block
   /// </summary>
   /// <param name="destination"> this Mover block will get place on destination block</param>
    private void PlaceBlock(Block destination)
    {
        destination.hasStages = m_hasStages;
        destination.stage = m_stage;

        if (m_enabled1 == true)
        {
            destination.blockImageLayer1.enabled = true;
            destination.blockImageLayer1.sprite = m_blockLayerImage1.sprite;
            destination.blockImageLayer1.color = destination.blockImageLayer1.color.WithNewA(1);
        }

        if (m_enabled2 == true)
        {
            destination.blockImageLayer2.enabled = true;
            destination.blockImageLayer2.sprite = m_blockLayerImage2.sprite;
            destination.blockImageLayer2.color = destination.blockImageLayer2.color.WithNewA(1);
        }

        if(m_hasJewel)
        {
            destination.hasJewel = true;
            m_jewel.transform.SetParent(null);
            destination.blockImage.enabled = false;
            m_jewel.transform.SetParent(destination.blockImage.transform);
            destination.jewel = m_jewel;
            destination.jewel.transform.localScale = Vector3.one;
            destination.jewel.transform.localPosition = Vector3.zero;
            destination.isFilled = true;
            destination.isAvailable = false;
            destination.thisCollider.enabled = false;
            //destination.jewel.m_parentBlock = destination;
            destination.jewel.SetParentBlock(destination);
            if(m_spriteType == SpriteType.ColouredJewel)
                GamePlay.Instance.blockers.Add(destination);
        } 
        else if(m_hasBalloonBomb)
        {
            if (m_balloonBomb != null)
            {
                destination.isBalloonBomb = true;
                m_balloonBomb.transform.SetParent(null);
                destination.blockImage.enabled = false;
                m_balloonBomb.transform.SetParent(destination.blockImage.transform);
                destination.thisBalloonBomb = m_balloonBomb;
                destination.thisBalloonBomb.transform.localScale = Vector3.one;
                destination.thisBalloonBomb.transform.localPosition = Vector3.zero;
                destination.isFilled = true;
                destination.isAvailable = false;
                destination.thisCollider.enabled = false;
                destination.thisBalloonBomb.block = destination;
            }
        }
        else if (m_hasIceBomb) 
        {
            if (m_iceBomb != null)
            {
                destination.isIceBomb = true;
                m_iceBomb.transform.SetParent(null);
                destination.blockImage.enabled = false;
                m_iceBomb.transform.SetParent(destination.blockImage.transform);
                destination.thisIceBomb = m_iceBomb;
                destination.thisIceBomb.transform.localScale = Vector3.one;
                destination.thisIceBomb.transform.localPosition = Vector3.zero;
                destination.isFilled = true;
                destination.isAvailable = false;
                destination.thisCollider.enabled = false;
                destination.thisIceBomb.block = destination;
            }
        }
        else if (m_hasDiamond)
        {
            if (m_diaMond != null)
            {
                m_diaMond.currentBlock = destination;
                destination.diamond = m_diaMond;
                destination.diamond.PlaceDiamond();
                destination.blockImage.enabled = false;
            }
        }

        else if (m_hasIceMachine)
        {
            if (m_IceMachine != null)
            {
                destination.thisIceMachine = m_IceMachine;
                //destination.thisIceMachine.block = destination;
                m_IceMachine.block = destination;
                destination.isIceMachine = true;
                destination.blockImage.enabled = false;
                m_IceMachine.transform.SetParent(destination.blockImage.transform);
                destination.thisIceMachine.transform.localScale = Vector3.one;
                destination.thisIceMachine.transform.localPosition = Vector3.zero;
                destination.thisIceMachine = m_IceMachine;
                destination.PlaceBlock(m_spriteType);
            }
        }

        else if (m_MagicHat != null)
        {
            destination.blockImage.enabled = false;
            m_MagicHat.transform.SetParent(destination.blockImage.transform);
            //destination.thisMagicHat.transform.localScale = Vector3.one;
            //destination.thisMagicHat.transform.localPosition = Vector3.zero;
            destination.thisMagicHat = m_MagicHat;
            destination.PlaceBlock(m_spriteType);
        }

        else if (m_MusicalPlayer != null) 
        {
            destination.blockImage.enabled = false;
            m_MusicalPlayer.transform.SetParent(destination.blockImage.transform);
            destination.thisMusicalPlayer = m_MusicalPlayer;
            destination.PlaceBlock(m_spriteType);
        }
        else
        {
            destination.PlaceBlock(m_blockImage.sprite, m_spriteTag);
        }

        if (m_spriteType == SpriteType.Bubble && destination.spriteType == SpriteType.Bubble)
        {
            destination.secondarySpriteType = m_secondarySpriteType;
        }
        else if(destination.spriteType != SpriteType.Bubble && m_spriteType == SpriteType.Bubble)
        {
            destination.SetBlockSpriteType(m_secondarySpriteType);
            destination.secondarySpriteType = SpriteType.Empty;
        }
        else if (m_spriteType == SpriteType.Magnet && destination.spriteType == SpriteType.Bubble)
        {
            destination.spriteType = SpriteType.MagnetWithBubble;
        }
        else if (m_spriteType != SpriteType.Bubble && destination.spriteType == SpriteType.Bubble)
        {
            destination.secondarySpriteType = m_spriteType;
        }
        else
        {
            destination.spriteType = m_spriteType;
            destination.secondarySpriteType = m_secondarySpriteType;
        }
        
        if(m_isPresentInBlockers)
        {
            GamePlay.Instance.blockers.Add(destination);
        }

        gameObject.SetActive(false);
    }
   
    /// <summary>
    /// Resets all properties
    /// </summary>
    private void ResetConveyorBlock()
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
        m_MagicHat = null;
        m_MusicalPlayer = null;
        m_IceMachine = null;
        m_hasIceMachine = false;
    }

    /// <summary>
    /// Sets the reference of block
    /// </summary>
    /// <param name="block"></param>
    public void SetBlockReference(Block block)
    {
        m_block = block;
    }
}
