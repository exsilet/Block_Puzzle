using UnityEngine;

namespace GamingMonks.Utils
{    
    /// <summary>
    /// Extention class Rect Transform.
    /// </summary>
    public static class RectTransfromUtils 
    {
        // Set width of rect transform as given.
        public static void SetNewWidth(this RectTransform rect, float width) {
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }

        // Set height of rect transform as given.
        public static void SetNewHeight(this RectTransform rect, float height) {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }
    }   
}
