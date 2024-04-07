using UnityEngine;
using System;

namespace GamingMonks.UITween
{
    public class FloatTweener : GMTweenBehaviour
    {
        float fromValue = 0;
        float toValue = 0;

        // Returns Interpolation for the given ease type.
        public override object GetValue(float t)
        {
            return Mathf.LerpUnclamped(fromValue, toValue, t);
        }

        // Start Tween after given delay.
        public FloatTweener SetDelay(float _delay)
        {
            delay = _delay;
            return this;
        }

        // Set given ease type for the tween.
        public FloatTweener SetEase(Ease ease)
        {
            easeType = ease;
            return this;
        }

        // Set given loop type for the tween.
        public FloatTweener SetLoop(int _loopCount, LoopType _loopType = LoopType.PingPong)
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
        public FloatTweener SetAnimation(AnimationCurve curve)
        {
            easeType = Ease.Custom;
            animationCurve = curve;
            return this;
        }

        // Invokes Tween complete event callback.
        public FloatTweener OnComplete(Action onComplete)
        {
            OnCompleteDeletegate = onComplete;
            return this;
        }

        // Invokes Tween loop complete event callback.
        public FloatTweener OnLoopComplete(Action<int> onLoopComplete)
        {
            OnLoopCompleteDelegate = onLoopComplete;
            return this;
        }

        // Set values to object property.
        public override void SetValues(object _fromValue, object _toValue)
        {
            fromValue = ((float)_fromValue);
            toValue = ((float)_toValue);
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