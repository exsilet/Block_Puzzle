using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace GamingMonks
{
    public class LevelSettingsMenu
    {
        #region LevelSettings
        [MenuItem("GamingMonks/Level Settings", false, 25)]
        public static void GenerateGameSettingsScriptable()
        {
            string assetPath = "Assets/GamingMonks/Resources";
            string assetName = "LevelSettings.asset";

            LevelSO asset;

            if (!System.IO.Directory.Exists(assetPath))
            {
                System.IO.Directory.CreateDirectory(assetPath);
            }

            if (System.IO.File.Exists(assetPath + "/" + assetName))
            {
                asset = (LevelSO)(Resources.Load(System.IO.Path.GetFileNameWithoutExtension(assetName)));
            }
            else
            {
                asset = ScriptableObject.CreateInstance<LevelSO>();
                AssetDatabase.CreateAsset(asset, assetPath + "/" + assetName);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        #endregion
    }
}