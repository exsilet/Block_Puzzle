using UnityEngine;
using UnityEngine.UI;
using GamingMonks;
using TMPro;

public class Target : MonoBehaviour
{
    public SpriteType spriteType { get; private set; }
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Animator animator;
    [SerializeField] public Image image;
    [SerializeField] public GameObject tickMark;
    private int target = 0;

    public void Initialize(int target, SpriteType spriteType)
    {
        this.spriteType = spriteType;
        this.target = target;
        targetText.gameObject.SetActive(true);
        tickMark.gameObject.SetActive(false);
        UpdateSprite();
        UpdateTargetText();
        SetSpriteType(spriteType);
        if(spriteType == SpriteType.MusicalNode)
        {
            GamePlay.Instance.musicNoteCount = target;
        }
        if(spriteType == SpriteType.Bird)
        {
            GamePlay.Instance.birdCount = target;
        }
    }

    private void SetSpriteType(SpriteType spriteType)
    {
        switch(spriteType)
        {
            case SpriteType.Bird:
                this.spriteType = SpriteType.Hat;
                break;

            default:
                this.spriteType = spriteType;
                break;
        }
    }

    private void UpdateSprite()
    {
        if (spriteType == SpriteType.MusicalNode)
        {
            image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.MusicalNodeGoal.ToString());
        }
        else
        {
            image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
        }
    }

    public void UpdateTargetCount()
    {
        target--;
        target = Mathf.Max(0, target);
        if(target <= 0)
        {
            TargetController.Instance.AddCompletedTarget(spriteType);
        }
    }

    private void UpdateTargetText()
    {
        targetText.text = target.ToString();
    }

    public void PlayTargetShakeEffect()
    {
        UpdateTargetText();
        PlayShakeAnimation();
        if(target <= 0)
        {
            tickMark.gameObject.SetActive(true);
            targetText.gameObject.SetActive(false);
        }
    }

    public int GetRemainingTarget()
    {
        return target;
    }

    private void PlayShakeAnimation()
    {
        animator.SetTrigger("Shake");
    }
}
