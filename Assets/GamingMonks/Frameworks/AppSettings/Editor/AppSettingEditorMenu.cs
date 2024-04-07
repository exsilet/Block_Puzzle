using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GamingMonks
{
    public class AppSettingEditorMenu : MonoBehaviour
    {
        #region AppSetting
        [MenuItem("GamingMonks/App Settings",false, 12)]
        public static void GenerateAppSettingsScriptable()
        {
            string assetPath = "Assets/GamingMonks/Resources";
            string assetName = "AppSettings.asset";

            AppSettings asset;

            if (!System.IO.Directory.Exists(assetPath)) {
                System.IO.Directory.CreateDirectory(assetPath);
            }

            if (System.IO.File.Exists(assetPath + "/"+ assetName))  {
                asset = (AppSettings)(Resources.Load(System.IO.Path.GetFileNameWithoutExtension(assetName)));
            }
            else   {
                asset = ScriptableObject.CreateInstance<AppSettings>();
                AssetDatabase.CreateAsset(asset, assetPath + "/" + assetName);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        #endregion
    }
}
