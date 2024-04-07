using UnityEngine;
using UnityEditor;
using System;

public static class EditorMenuUtils 
{
    [MenuItem("GamingMonks/Misc./Clear All PlayerPrefs")]
    public static void ClearAllPlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("GamingMonks/Misc./Clear Define Symbol EditorPrefs")]
    public static void ClearDefineSymbolEditorPrefs() {
        EditorPrefs.DeleteKey("userRemoved_" + "UnityIAP");
        EditorPrefs.DeleteKey("userRemoved_" + "UnityAds");
        EditorPrefs.DeleteKey("userRemoved_" + "Admob");
        EditorPrefs.DeleteKey("userRemoved_" + "IronSource");
        EditorPrefs.DeleteKey("userRemoved_" + "AppLovinMax");
        EditorPrefs.DeleteKey("userRemoved_" + "Vungle");
    }

    #region CaptureScreenshot
    [MenuItem("GamingMonks/Misc./Capture Screenshot/1X")]
    private static void Capture1XScreenshot()
    {
        CaptureScreenshot(1);
    }

    [MenuItem("GamingMonks/Misc./Capture Screenshot/2X")]
    private static void Capture2XScreenshot()
    {
        CaptureScreenshot(2);
    }

    [MenuItem("GamingMonks/Misc./Capture Screenshot/3X")]
    private static void Capture3XScreenshot()
    {
        CaptureScreenshot(3);
    }

    public static void CaptureScreenshot(int supersize)
    {
        string imgName = "IMG-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") + ".png";
        ScreenCapture.CaptureScreenshot((Application.dataPath + "/" + imgName), supersize);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
    #endregion
}
