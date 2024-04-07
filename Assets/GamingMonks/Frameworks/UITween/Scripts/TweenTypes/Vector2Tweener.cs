using UnityEngine;
using System;

namespace GamingMonks.UITween
{
    /// <summary>
    /// Vector 2 Type Tweener
    /// </summary>
    public class Vector2Tweener : GMTweenBehaviour
    {
        Vector2 fromValue = Vector3.zero;
        Vector2 toValue = Vector3.zero;

        // Returns Interpolation for the given ease type.
        public override object GetValue(float t)
        {
            return new Vector2(Mathf.LerpUnclamped(fromValue.x, toValue.x, t), Mathf.LerpUnclamped(fromValue.y, toValue.y, t));
        }

        // Start Tween after given delay.
        public Vector2Tweener SetDelay(float _delay)
        {
            delay = _delay;
            return this;
        }   

        // Set given ease type for the tween.
        public Vector2Tweener SetEase(Ease ease)
        {
            easeType = ease;
            return this;
        }

        // Set given loop type for the tween.
        public Vector2Tweener SetLoop(int _loopCount, LoopType _loopType = LoopType.PingPong)
        {
            loopCount = _loopCount;
            loopType = _loopType;

            if (_loopType == LoopType.PingPong && (_loopCount > 1))
            {
                _loopCount = (_loopCount * 2);
            }
            return this;
        }

        // Set given animation curve for the tween.
        public Vector2Tweener SetAnimation(AnimationCurve curve)
        {
            easeType = Ease.Custom;
            animationCurve = curve;
            return this;
        }

        // Invokes Tween complete event callback.
        public Vector2Tweener OnComplete(Action onComplete)
        {
            OnCompleteDeletegate = onComplete;
            return this;
        }

        // Invokes Tween loop complete event callback.
        public Vector2Tweener OnLoopComplete(Action<int> onLoopComplete)
        {
            OnLoopCompleteDelegate = onLoopComplete;
            return this;
        }

        // Set values to object property.
        public override void SetValues(object _fromValue, object _toValue)
        {
            fromValue = ((Vector2)_fromValue);
            toValue = ((Vector2)_toValue);
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