using UnityEngine;
using System;

namespace GamingMonks.UITween
{
    /// <summary>
    /// Vector 3 Type Tweener
    /// </summary>
    public class Vector3Tweener : GMTweenBehaviour
    {
        Vector3 fromValue = Vector3.zero;
        Vector3 toValue = Vector3.zero;

        // Returns Interpolation for the given ease type.
        public override object GetValue(float t)
        {
            return new Vector3(Mathf.LerpUnclamped(fromValue.x, toValue.x, t), Mathf.LerpUnclamped(fromValue.y, toValue.y, t), Mathf.LerpUnclamped(fromValue.z, toValue.z, t));
        }       

        // Start Tween after given delay.
        public Vector3Tweener SetDelay(float _delay)
        {
            delay = _delay;
            return this;
        }

        // Set given ease type for the tween.
        public Vector3Tweener SetEase(Ease ease)
        {
            easeType = ease;
            return this;
        }

        // Set given loop type for the tween.
        public Vector3Tweener SetLoop(int _loopCount, LoopType _loopType = LoopType.PingPong)
        {
            loopCount = _loopCount;
            loopType = _loopType;

            if (_loopType == LoopType.PingPong && (_loopCount > 1))
            {
                loopCount = (_loopCount * 2);
            }
            return this;
        }

        // Set given animation curve for the tween.
        public Vector3Tweener SetAnimation(AnimationCurve curve)
        {
            easeType = Ease.Custom;
            animationCurve = curve;
            return this;
        }

        // Invokes Tween complete event callback.
        public Vector3Tweener OnComplete(Action onComplete)
        {
            OnCompleteDeletegate = onComplete;
            return this;
        }

        // Invokes Tween loop complete event callback.
        public Vector3Tweener OnLoopComplete(Action<int> onLoopComplete)
        {
            OnLoopCompleteDelegate = onLoopComplete;
            return this;
        }

        // Set values to object property.
        public override void SetValues(object _fromValue, object _toValue)
        {
            fromValue = ((Vector3)_fromValue);
            toValue = ((Vector3)_toValue);
        }

        public override object GetStartPoint()
        {
            return fromValue;
        }

        public override object GetEndPoint()
        {
            return toValue;
        }
    }
}