using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks.UITween
{    
    /// <summary>
    /// UI Tween Managers.
    /// </summary>
    public class GMTweenManager : MonoBehaviour
    {
        // Anchor X.
        public static FloatTweener AnchorX(RectTransform rectT,  float fromValue, float toValue, float duration) {
            FloatTweener tweener = rectT.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.AnchorX, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Anchor Y.
        public static FloatTweener AnchorY(RectTransform rectT,  float fromValue, float toValue, float duration) {
            FloatTweener tweener = rectT.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.AnchorY, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Anchor Z.
        public static FloatTweener AnchorZ(RectTransform rectT,  float fromValue, float toValue, float duration) {
            FloatTweener tweener = rectT.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.AnchorZ, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Vector2 Tween.
        public static Vector2Tweener AnchoredPosition(RectTransform rectT, Vector2 fromValue, Vector2 toValue, float duration)
        {
            Vector2Tweener tweener = rectT.gameObject.AddComponent<Vector2Tweener>();
            tweener.SetTweenParams(AnimationType.AnchoredPosition, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Vector3 Tween.
        public static Vector3Tweener AnchoredPosition3D(RectTransform rectT, Vector3 fromValue, Vector3 toValue, float duration)
        {
            Vector3Tweener tweener = rectT.gameObject.AddComponent<Vector3Tweener>();
            tweener.SetTweenParams(AnimationType.AnchoredPosition3D, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // PositionX Tween.
        public static FloatTweener PositionX(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.PositionX, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local PositionX Tween.
        public static FloatTweener LocalPositionX(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalPositionX, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // PositionY Tween.
        public static FloatTweener PositionY(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.PositionY, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local PositionY Tween.
        public static FloatTweener LocalPositionY(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalPositionY, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // PositionZ Tween.
        public static FloatTweener PositionZ(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.PositionZ, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local PositionZ Tween.
        public static FloatTweener LocalPositionZ(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalPositionZ, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Position Tween.
        public static Vector3Tweener Position(Transform transform, Vector3 fromValue, Vector3 toValue, float duration)
        {
            Vector3Tweener tweener = transform.gameObject.AddComponent<Vector3Tweener>();
            tweener.SetTweenParams(AnimationType.Position, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local Position Tween.
        public static Vector3Tweener LocalPosition(Transform transform, Vector3 fromValue, Vector3 toValue, float duration)
        {
            Vector3Tweener tweener = transform.gameObject.AddComponent<Vector3Tweener>();
            tweener.SetTweenParams(AnimationType.LocalPosition, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local RotationX Tween.
        public static FloatTweener LocalRotationToX(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalRotationToX, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local RotationY Tween.
        public static FloatTweener LocalRotationToY(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalRotationToY, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local RotationZ Tween.
        public static FloatTweener LocalRotationToZ(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalRotationToZ, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local RotationTo Tween.
        public static Vector3Tweener LocalRotationTo(Transform transform, Vector3 fromValue, Vector3 toValue, float duration)
        {
            Vector3Tweener tweener = transform.gameObject.AddComponent<Vector3Tweener>();
            tweener.SetTweenParams(AnimationType.LocalRotationTo, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local ScaleX Tween.
        public static FloatTweener LocalScaleX(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalScaleX, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local ScaleY Tween.
        public static FloatTweener LocalScaleY(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalScaleY, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local ScaleZ Tween.
        public static FloatTweener LocalScaleZ(Transform transform, float fromValue, float toValue, float duration)
        {
            FloatTweener tweener = transform.gameObject.AddComponent<FloatTweener>();
            tweener.SetTweenParams(AnimationType.LocalScaleZ, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Local Scale Tween.
        public static Vector3Tweener LocalScale(Transform transform, Vector3 fromValue, Vector3 toValue, float duration)
        {
            Vector3Tweener tweener = transform.gameObject.AddComponent<Vector3Tweener>();
            tweener.SetTweenParams(AnimationType.LocalScale, fromValue, toValue, duration);
            tweener.Play();
            return tweener;
        }

        // Sprite Alpha.
        public static SpriteAlpha SetAlpha(SpriteRenderer spriteR, float endValue, float time) {
            SpriteAlpha tweener = spriteR.gameObject.AddComponent<SpriteAlpha>();
            tweener.SetTweenParams(AnimationType.Color, spriteR.color.a, endValue, time);
            tweener.Play();
            return tweener;
        }

        // Image Alpha
        public static ImageAlpha SetAlpha(Image image, float endValue, float time) {
            ImageAlpha tweener = image.gameObject.AddComponent<ImageAlpha>();
            tweener.SetTweenParams(AnimationType.Color, image.color.a, endValue, time);
            tweener.Play();
            return tweener;
        }

        // Text Color
        public static TextAlpha SetAlpha(Text image, float endValue, float time) {
            TextAlpha tweener = image.gameObject.AddComponent<TextAlpha>();
            tweener.SetTweenParams(AnimationType.Color, image.color.a, endValue, time);
            tweener.Play();
            return tweener;
        }

        // CanvasGroup Alpha
        public static CanvasGroupAlpha SetAlpha(CanvasGroup canvasGroup, float endValue, float time) {
            CanvasGroupAlpha tweener = canvasGroup.gameObject.AddComponent<CanvasGroupAlpha>();
            tweener.SetTweenParams(AnimationType.Color, canvasGroup.alpha, endValue, time);
            tweener.Play();
            return tweener;
        }


        // Sprite Color
        public static SpriteColor SetColor(SpriteRenderer spriteR, Color endValue, float time) {
            SpriteColor tweener = spriteR.gameObject.AddComponent<SpriteColor>();
            tweener.SetTweenParams(AnimationType.Color, spriteR.color, endValue, time);
            tweener.Play();
            return tweener;
        }

        // Image Color
        public static ImageColor SetColor(Image image, Color endValue, float time) {
            ImageColor tweener = image.gameObject.AddComponent<ImageColor>();
            tweener.SetTweenParams(AnimationType.Color, image.color, endValue, time);
            tweener.Play();
            return tweener;
        }

        // Text Color
        public static TextColor SetColor(Text text, Color endValue, float time) {
            TextColor tweener = text.gameObject.AddComponent<TextColor>();
            tweener.SetTweenParams(AnimationType.Color, text.color, endValue, time);
            tweener.Play();
            return tweener;
        }

        // Image Fill Amount
        public static ImageFillAmount FillAmount(Image image, float endValue, float time) {
            ImageFillAmount tweener = image.gameObject.AddComponent<ImageFillAmount>();
            tweener.SetTweenParams(AnimationType.Color, image.fillAmount, endValue, time);
            tweener.Play();
            return tweener;
        }
    }
}
