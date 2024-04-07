using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace GamingMonks
{	
	public class GamePlaySettingsEditorMenu
	{
		#region AppSetting
        [MenuItem("GamingMonks/GamePlay Settings", false, 11)]
        public static void GenerateGameSettingsScriptable()
        {
            string assetPath = "Assets/GamingMonks/Resources";
            string assetName = "GamePlaySettings.asset";

            GamePlaySettings asset;

            if (!System.IO.Directory.Exists(assetPath)) {
                System.IO.Directory.CreateDirectory(assetPath);
            }

            if (System.IO.File.Exists(assetPath + "/"+ assetName))  {
                asset = (GamePlaySettings)(Resources.Load(System.IO.Path.GetFileNameWithoutExtension(assetName)));
            }
            else   {
                asset = ScriptableObject.CreateInstance<GamePlaySettings>();
                AssetDatabase.CreateAsset(asset, assetPath + "/" + assetName);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        #endregion
	}
}
