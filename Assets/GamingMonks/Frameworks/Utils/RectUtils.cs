using UnityEngine;

namespace GamingMonks.Utils
{
    /// <summary>
    /// Extention class Rect.
    /// </summary>
    public static class RectUtils
    {
        // Replace X value of Rect with given value.
        public static Rect WithNewX(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        // Replace Y value of Rect with given value.
        public static Rect WithNewY(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        // Replace Width value of Rect with given value.
        public static Rect WithNewWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        // Replace Height of Rect with given value.
        public static Rect WithNewHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }
    }
}
