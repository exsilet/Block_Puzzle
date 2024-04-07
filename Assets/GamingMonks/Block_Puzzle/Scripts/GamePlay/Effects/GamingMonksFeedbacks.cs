using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamingMonks.Feedbacks
{
    /// <summary>
    /// This class will be responsible for playing Feel effects & feedbacks.
    /// </summary>
    public class GamingMonksFeedbacks : Singleton<GamingMonksFeedbacks>
    {
        [System.NonSerialized] public GameObject currentBlockBreakEffect;

        [SerializeField] private List<ParticleSystem> m_BlockBreakEffects;

        public void PrepareBlockBreakEffect()
        {
            int randomVal = Random.Range(0, m_BlockBreakEffects.Count);
            currentBlockBreakEffect = m_BlockBreakEffects[randomVal].gameObject;
        }
       
    }
}
