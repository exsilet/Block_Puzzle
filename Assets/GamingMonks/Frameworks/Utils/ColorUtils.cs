﻿using UnityEngine;

namespace GamingMonks.Utils
{
    /// <summary>
    /// Extention class for color related methods.
    /// </summary>
    public static class ColorUtils
    {
        // Replaces color with new R.
        public static Color WithNewR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        // Replaces color with new G.
        public static Color WithNewG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        // Replaces color with new B.
        public static Color WithNewB(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        // Replaces color with new A.
        public static Color WithNewA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        // Replaces color with new R.
        public static Color WithNewR255(this Color color, float r)
        {
            color.r = r / 255f;
            return color;
        }

        // Replaces color with new G.
        public static Color WithNewG255(this Color color, float g)
        {
            color.g = g / 255f;
            return color;
        }

        // Replaces color with new B.
        public static Color WithNewB255(this Color color, float b)
        {
            color.b = b / 255f;
            return color;
        }

        // Replaces color with new A.
        public static Color WithNewA255(this Color color, float a)
        {
            color.a = a / 255f;
            return color;
        }

        // Converts hex value to color.
        public static Color GetColorFromHexa(string hexa) {
            Color newColor;
            ColorUtility.TryParseHtmlString(hexa, out newColor);
            return newColor;
        }
    }
}
