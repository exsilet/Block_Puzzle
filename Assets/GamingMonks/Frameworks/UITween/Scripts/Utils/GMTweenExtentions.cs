using UnityEngine;
using UnityEngine.UI;

namespace  GamingMonks.UITween
{
    /// <summary>
    /// Extentions for the tweens classes.
    /// </summary>
    public static class GMTweenExtentions
    {
        //Rect Transform
        public static FloatTweener AnchorX(this RectTransform rectT, float endValue, float time) {
            return GMTweenManager.AnchorX(rectT, rectT.anchoredPosition.x, endValue, time);
        }

        public static FloatTweener AnchorY(this RectTransform rectT, float endValue, float time)
        {
            return GMTweenManager.AnchorY(rectT, rectT.anchoredPosition.y, endValue, time);
        }

        public static FloatTweener AnchorZ(this RectTransform rectT, float endValue, float time)
        {
            return GMTweenManager.AnchorZ(rectT, rectT.anchoredPosition3D.z, endValue, time);
        }

        public static Vector2Tweener AnchoredPosition(this RectTransform rectT, Vector2 endValue, float time)
        {
            return GMTweenManager.AnchoredPosition(rectT, rectT.anchoredPosition, endValue, time);
        }

        public static Vector3Tweener AnchoredPosition3D(this RectTransform rectT, Vector3 endValue, float time)
        {
            return GMTweenManager.AnchoredPosition3D(rectT, rectT.anchoredPosition3D, endValue, time);
        }

        //PositionX
        public static FloatTweener PositionX(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.PositionX(transform, transform.position.x, endValue, time);
        }

        //LocalPositionX
        public static FloatTweener LocalPositionX(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalPositionX(transform, transform.localPosition.x, endValue, time);
        }

        //PositionY
        public static FloatTweener PositionY(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.PositionY(transform, transform.position.y, endValue, time);
        }

        //LocalPositionY
        public static FloatTweener LocalPositionY(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalPositionY(transform, transform.localPosition.y, endValue, time);
        }

        //PositionZ
        public static FloatTweener PositionZ(this Transform transform, float endValue, float time)
        {   
            return GMTweenManager.PositionZ(transform, transform.position.z, endValue, time);
        }

        //LocalPositionZ
        public static FloatTweener LocalPositionZ(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalPositionZ(transform, transform.localPosition.z, endValue, time);
        }

        //Position
        public static Vector3Tweener Position(this Transform transform, Vector3 endValue, float time)
        {
            return GMTweenManager.Position(transform, transform.position, endValue, time);
        }

        //LocalPosition
        public static Vector3Tweener LocalPosition(this Transform transform, Vector3 endValue, float time)
        {
            return GMTweenManager.LocalPosition(transform, transform.localPosition, endValue, time);
        }


        //LocalRotationX
        public static FloatTweener LocalRotationToX(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalRotationToX(transform, transform.localEulerAngles.x, endValue, time);
        }

        //LocalRotationY
        public static FloatTweener LocalRotationToY(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalRotationToY(transform, transform.localEulerAngles.y, endValue, time);
        }

        //LocalRotationZ
        public static FloatTweener LocalRotationToZ(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalRotationToZ(transform, transform.localEulerAngles.z, endValue, time);
        }


        //LocalRotation
        public static Vector3Tweener LocalRotationTo(this Transform transform, Vector3 endValue, float time)
        {
            return GMTweenManager.LocalRotationTo(transform, transform.localEulerAngles, endValue, time);
        }


        //LocalScaleX
        public static FloatTweener LocalScaleX(this RectTransform rectT, float endValue, float time)
        {
            return GMTweenManager.LocalScaleX(rectT, rectT.localScale.x, endValue, time);
        }


        //LocalScaleX
        public static FloatTweener LocalScaleX(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalScaleX(transform, transform.localScale.x, endValue, time);
        }

        //LocalScaleY
        public static FloatTweener LocalScaleY(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalScaleY(transform, transform.localScale.y, endValue, time);
        }

         //LocalScaleZ
        public static FloatTweener LocalScaleZ(this Transform transform, float endValue, float time)
        {
            return GMTweenManager.LocalScaleZ(transform, transform.localScale.z, endValue, time);
        }
         //LocalScaleZ
        public static Vector3Tweener LocalScale(this Transform transform, Vector3 endValue, float time)
        {
            return GMTweenManager.LocalScale(transform, transform.localScale, endValue, time);
        }


        //Sprite Alpha
        public static SpriteAlpha SetAlpha(this SpriteRenderer spriteR, float endValue, float time) {
            return GMTweenManager.SetAlpha(spriteR, endValue, time);
        }

        //Image Alpha
        public static ImageAlpha SetAlpha(this Image image, float endValue, float time) {
            return GMTweenManager.SetAlpha(image, endValue, time);
        }

         //Text Alpha
        public static TextAlpha SetAlpha(this Text txt, float endValue, float time) {
            return GMTweenManager.SetAlpha(txt, endValue, time);
        }

        //CanvasGroup Alpha
        public static CanvasGroupAlpha SetAlpha(this CanvasGroup canvasGroup, float endValue, float time) {
            return GMTweenManager.SetAlpha(canvasGroup, endValue, time);
        }

        //Sprite Color
        public static SpriteColor SetColor(this SpriteRenderer spriteR, Color endValue, float time) {
           return GMTweenManager.SetColor(spriteR, endValue, time);
        }

        //Image Color
        public static ImageColor SetColor(this Image image, Color endValue, float time) {
            return GMTweenManager.SetColor(image, endValue, time);
        }

        //Image FillAmount
        public static ImageFillAmount FillAmount(this Image image, float endValue, float time) {
            return GMTweenManager.FillAmount(image, endValue, time);
        }

        //Image Color
        public static TextColor SetColor(this Text txt, Color endValue, float time) {
            return GMTweenManager.SetColor(txt, endValue, time);
        }

        // Pauses the tween.

        public static void Pause(this Transform transform) {
            transform.SendMessage("Pause");
        }

        public static void Pause(this Image image) {
           image.SendMessage("Pause");
        }

        public static void Pause(this SpriteRenderer spriteR) {
           spriteR.SendMessage("Pause");
        }

        public static void Pause(this CanvasGroup canvasGroup) {
           canvasGroup.SendMessage("Pause");
        }

        // Resumes the tween.

        public static void Resume(this Transform transform) {
            transform.SendMessage("Resume");
        }

        public static void Resume(this Image image) {
           image.SendMessage("Resume");
        }

        public static void Resume(this SpriteRenderer spriteR) {
           spriteR.SendMessage("Resume");
        }

        public static void Resume(this CanvasGroup canvasGroup) {
           canvasGroup.SendMessage("Resume");
        }
        
        
    }
}
