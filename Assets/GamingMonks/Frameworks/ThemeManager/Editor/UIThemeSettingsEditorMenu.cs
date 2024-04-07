using UnityEngine;
using UnityEditor;

namespace GamingMonks
{
    public class UIThemeSettingsEditorMenu : MonoBehaviour
    {
        #region MobileAdsSetting
        [MenuItem("GamingMonks/UI Theme Settings", false, 16)]
        public static void GenerateUIThemeSettingsScriptable()
        {
            string assetPath = "Assets/GamingMonks/Resources";
            string assetName = "UIThemeSettings.asset";

            UIThemeSettings asset;

            if (!System.IO.Directory.Exists(assetPath)) {
                System.IO.Directory.CreateDirectory(assetPath);
            }

            if (System.IO.File.Exists(assetPath + "/" + assetName)) {
                asset = (UIThemeSettings)(Resources.Load(System.IO.Path.GetFileNameWithoutExtension(assetName)));
            }
            else
            {
                asset = ScriptableObject.CreateInstance<UIThemeSettings>();
                AssetDatabase.CreateAsset(asset, assetPath + "/" + assetName);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        #endregion
    }
}