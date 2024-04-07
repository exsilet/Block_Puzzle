using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks.Localization
{
    public static class LocalizationExtentions
    {
        // Sets localized text with given tag to the Text.
        public static void SetTextWithTag(this TextMeshProUGUI txt, string tag)
        {
            txt.text = LocalizationManager.Instance.GetTextWithTag(tag);
        }

        public static void SetFormattedTextWithTag(this TextMeshProUGUI txt, string tag, string value1 = "", string value2 = "")
        {
            string localizedText = LocalizationManager.Instance.GetTextWithTag(tag);
            if (value1 != "" && value2 != "")
            {
                txt.text = string.Format(localizedText, value1, value2);
            }
            else
            {
                if (value1 != "")
                {
                    txt.text = string.Format(localizedText, value1);
                }
                else if (value2 != "")
                {
                    txt.text = string.Format(localizedText, value2);
                }
                else
                {
                    txt.text = localizedText;
                }
            }
        }

        public static void SetFormattedTextWithTag(this TextMeshProUGUI txt, string tag, object value1)
        {
            string localizedText = LocalizationManager.Instance.GetTextWithTag(tag);
            txt.text = string.Format(localizedText, value1.ToString());
        }
    }
}
