using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingMonks
{
    public class MagicHat : MonoBehaviour
    {
        //public ParticleSystem hatParticle;
        private void Awake()
        {
            //hatParticle.transform.localPosition = transform.position;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            rectTransform.sizeDelta = new Vector2(size, size);
        }
    }
}
