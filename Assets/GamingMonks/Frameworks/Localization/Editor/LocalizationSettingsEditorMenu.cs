using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GamingMonks.Utils;

namespace GamingMonks.Localization
{    
    public class LocalizationSettingsEditorMenu : MonoBehaviour
    {
       	#region LocalizationSettings
        [MenuItem("GamingMonks/Localization Settings", false, 15)]
        public static void GenerateLocalizationSettings()
        {
            string assetPath = "Assets/GamingMonks/Resources";
            string assetName = "LocalizationSettings.asset";

            LocalizationSettings asset;

            if (!System.IO.Directory.Exists(assetPath))
            {
                System.IO.Directory.CreateDirectory(assetPath);
            }

            if (System.IO.File.Exists(assetPath + "/"+ assetName))
            {
                asset = (LocalizationSettings)(Resources.Load(System.IO.Path.GetFileNameWithoutExtension(assetName)));
            }
            else
            {
                asset = ScriptableObject.CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(asset, assetPath + "/" + assetName);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    	#endregion
    }
}
