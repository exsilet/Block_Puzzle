using UnityEngine;

namespace GamingMonks.UITween
{    
    /// <summary>
    /// UI Tween Ease calculations.
    /// </summary>
    public class GMTweenUtility : MonoBehaviour
    {
        public static float EaseOut(float t)
        {
            return 1.0f - (1.0f - t) * (1.0f - t) * (1.0f - t);
        }
        
        public static float EaseIn(float t)
        {
            return t * t * t;
        }
    }
}
