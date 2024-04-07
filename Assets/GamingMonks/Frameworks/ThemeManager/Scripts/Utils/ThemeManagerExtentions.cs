using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    /// <summary>
    /// Theme manager extentions to make theme releted call easy.
    /// </summary>
    public static class ThemeManagerExtentions
    {
        // Extention to set color of image with given tag.
        public static void SetColorWithThemeId(this Image img, string colorTag) {
            if(ThemeManager.Instance.UIThemeEnabled) {
                img.color = ThemeManager.Instance.GetThemeColorWithTag(colorTag);
            }
        }

        // Extention to set color of sprite renderer with given tag.
        public static void SetColorWithThemeId(this SpriteRenderer renderer, string colorTag) {   
            if(ThemeManager.Instance.UIThemeEnabled) {
                renderer.color = ThemeManager.Instance.GetThemeColorWithTag(colorTag); 
            }
        }

        // Extention to set color of text with given tag.
        public static void SetColorWithThemeId(this Text txt, string colorTag) {
            if(ThemeManager.Instance.UIThemeEnabled) {
                txt.color = ThemeManager.Instance.GetThemeColorWithTag(colorTag); 
            }
        }

        // Extention to set color of textmesh with given tag.
        public static void SetColorWithThemeId(this TextMesh txt, string colorTag) {
            if(ThemeManager.Instance.UIThemeEnabled) {
                txt.color = ThemeManager.Instance.GetThemeColorWithTag(colorTag); 
            }
        }

        // Extention to set sprite of image with given tag.
        public static void SetSpriteWithThemeId(this Image img, string spriteTag) {
            if(ThemeManager.Instance.UIThemeEnabled) {
                img.sprite = ThemeManager.Instance.GetThemeSpriteWithTag(spriteTag); 
            }
        }

        // Extention to set sprite of sprite renderer with given tag.
        public static void SetSpriteWithThemeId(this SpriteRenderer renderer, string spriteTag) {
            if(ThemeManager.Instance.UIThemeEnabled) {
                renderer.sprite = ThemeManager.Instance.GetThemeSpriteWithTag(spriteTag);
            }
        }
    }
}
