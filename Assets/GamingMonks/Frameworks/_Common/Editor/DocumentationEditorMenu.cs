using UnityEngine;
using UnityEditor;

public static class DocumentationEditorMenu 
{    
    static string gameDocName = "https://drive.google.com/file/d/1frPjYQr-Z9C1AVfqBuXVOL8kVg4mzZJx/view?usp=sharing";

    [MenuItem("GamingMonks/Documentation/Game Setup", false, 00)]
    public static void OpenGameSettingDocumentation() {
        Application.OpenURL(gameDocName);
    }
}
