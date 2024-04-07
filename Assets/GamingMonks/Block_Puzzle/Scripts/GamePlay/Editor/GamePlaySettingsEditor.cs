using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using GamingMonks.Tutorial;

namespace GamingMonks
{
    [CustomEditor(typeof(GamePlaySettings))]
    public class GamePlaySettingsEditor : CustomInspectorHelper
    {
        private bool cache = false;
        GamePlaySettings gamePlaySettings;
        SerializedProperty standardBlockShapesInfo;
        SerializedProperty advancedBlockShapesInfo;

        SerializedProperty tutorialModeSettings;
        SerializedProperty classicModeSettings;
        SerializedProperty timeModeSettings;
        SerializedProperty blastModeSettings;
        SerializedProperty advancedModeSettings;
        SerializedProperty blockSpriteColorTags;

        GUIStyle labelStyle;
        GUIStyle inputStyle;
        GUIStyle popupStyle;

        readonly string[] boardSizes = new string[] { "0X0", "1X1", "2X2", "3X3", "4X4", "5X5", "6X6", "7X7", "8X8", "9X9", "10X10", "11X11", "12X12" };


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!cache)
            {
                gamePlaySettings = (GamePlaySettings)target;

                standardBlockShapesInfo = serializedObject.FindProperty("standardBlockShapesInfo");
                advancedBlockShapesInfo = serializedObject.FindProperty("advancedBlockShapesInfo");
                blockSpriteColorTags = serializedObject.FindProperty("blockSpriteColorTags");

                tutorialModeSettings = serializedObject.FindProperty("tutorialModeSettings");
                classicModeSettings = serializedObject.FindProperty("classicModeSettings");
                timeModeSettings = serializedObject.FindProperty("timeModeSettings");
                blastModeSettings = serializedObject.FindProperty("blastModeSettings");
                advancedModeSettings = serializedObject.FindProperty("advancedModeSettings");

                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontStyle = FontStyle.Normal;

                inputStyle = new GUIStyle(GUI.skin.textField);
                inputStyle.fontStyle = FontStyle.Bold;
                inputStyle.alignment = TextAnchor.MiddleCenter;

                popupStyle = EditorStyles.popup;
                popupStyle.alignment = TextAnchor.MiddleCenter;
                cache = true;
            }

            EditorGUILayout.Space();
            DrawAllModeSettings();
            EditorGUILayout.Space();
            DrawBlockShapesInfo();
            EditorGUILayout.Space();
            DrawRewardSettings();
            EditorGUILayout.Space();
            DrawScoreSettings();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(gamePlaySettings);
        }

        void DrawAllModeSettings()
        {
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            bool isExpanded = BeginFoldoutBox("All Modes Settings");
            int indentLevel = EditorGUI.indentLevel;

            if (isExpanded)
            {
                if (tutorialModeSettings != null)
                {
                    DrawTutorialSettings(tutorialModeSettings, "Tutorial Settings");
                }
                else
                {
                    tutorialModeSettings = serializedObject.FindProperty("tutorialModeSettings");
                }

                if (classicModeSettings != null)
                {
                    DrawGameModeSettings(classicModeSettings, "Classic Mode");
                }
                else
                {
                    classicModeSettings = serializedObject.FindProperty("classicModeSettings");
                }

                if (timeModeSettings != null)
                {
                    DrawGameModeSettings(timeModeSettings, "Time Mode");
                }
                else
                {
                    timeModeSettings = serializedObject.FindProperty("timeModeSettings");
                }

                if (blastModeSettings != null)
                {
                    DrawGameModeSettings(blastModeSettings, "Blast Mode");
                }
                else
                {
                    blastModeSettings = serializedObject.FindProperty("blastModeSettings");
                }

                if (advancedModeSettings != null)
                {
                    DrawGameModeSettings(advancedModeSettings, "Advanced Mode");
                }
                else
                {
                    advancedModeSettings = serializedObject.FindProperty("advancedModeSettings");
                }

                DrawLevelModeSettings();

            }
            EndBox();
        }

        private void DrawLevelModeSettings()
        {
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            BeginBox();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            bool isExpanded = BeginSimpleFoldoutBox("Level Mode");
            if (isExpanded)
            {
                BeginBox();
                EditorGUILayout.BeginHorizontal();
                bool isLevelModeGlobalBlockSettingsExpanded = BeginSimpleFoldoutBox("Global Level Mode Block Settings");
                EditorGUILayout.LabelField("", GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                if(isLevelModeGlobalBlockSettingsExpanded)
                {
                    BeginBox();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Block Size", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    gamePlaySettings.globalLevelModeBlockSize = EditorGUILayout.FloatField(gamePlaySettings.globalLevelModeBlockSize);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(3);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Block Space", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    gamePlaySettings.globalLevelModeBlockSpace = EditorGUILayout.FloatField(gamePlaySettings.globalLevelModeBlockSpace);
                    EditorGUILayout.EndHorizontal();
                    
                    GUILayout.Space(3);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Shape Drag Position Offset", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    gamePlaySettings.globalLevelShapeDragPositionOffset = EditorGUILayout.FloatField(gamePlaySettings.globalLevelShapeDragPositionOffset);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Max moves Offset", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    gamePlaySettings.globalLevelModeMaxMovesOffset = EditorGUILayout.IntField(gamePlaySettings.globalLevelModeMaxMovesOffset);
                    EditorGUILayout.EndHorizontal();
                    EndBox();
                }
                GUILayout.Space(5);
                DrawLine();
                EditorGUILayout.BeginHorizontal();
                bool isBlockLevelInfoExpanded = BeginSimpleFoldoutBox("Level Block Shapes Info");
                EditorGUILayout.LabelField("", GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                if (isBlockLevelInfoExpanded)
                {
                    if (gamePlaySettings.blockShapeLevelInfo.Length > 0)
                    {

                        for (int i = 0; i < gamePlaySettings.blockShapeLevelInfo.Length; i++)
                        {
                            BeginBox();
                            EditorGUILayout.BeginHorizontal();
                            bool isBlockLevelRangeExpanded = BeginSimpleFoldoutBox("Level Range " + (i+1)+":");
                            EditorGUILayout.LabelField(gamePlaySettings.blockShapeLevelInfo[i].minLevel + "-" + gamePlaySettings.blockShapeLevelInfo[i].maxLevel, labelStyle, GUILayout.Width(50));
                            EditorGUILayout.LabelField("", GUILayout.Width(5));

                            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                            {
                                List<BlockShapeLevelInfo> blockShapeList = new List<BlockShapeLevelInfo>(gamePlaySettings.blockShapeLevelInfo);
                                blockShapeList.Insert(i, new BlockShapeLevelInfo());
                                gamePlaySettings.blockShapeLevelInfo = blockShapeList.ToArray();
                            }

                            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                            {
                                List<BlockShapeLevelInfo> blockShapeList = new List<BlockShapeLevelInfo>(gamePlaySettings.blockShapeLevelInfo);
                                blockShapeList.RemoveAt(i);
                                gamePlaySettings.blockShapeLevelInfo = blockShapeList.ToArray();
                                return;
                            }
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(3);
                            if (isBlockLevelRangeExpanded)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Index : ", labelStyle, GUILayout.Width(60));
                                EditorGUILayout.LabelField("Min Level : ", labelStyle, GUILayout.Width(100));
                                EditorGUILayout.LabelField("Max Level : ", labelStyle, GUILayout.Width(100));
                                EditorGUILayout.EndHorizontal();

                                DrawLine();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(60));
                                gamePlaySettings.blockShapeLevelInfo[i].minLevel = EditorGUILayout.IntField(gamePlaySettings.blockShapeLevelInfo[i].minLevel, inputStyle, GUILayout.Width(100));
                                gamePlaySettings.blockShapeLevelInfo[i].maxLevel = EditorGUILayout.IntField(gamePlaySettings.blockShapeLevelInfo[i].maxLevel, inputStyle, GUILayout.Width(100));
                                EditorGUILayout.EndHorizontal();
                                BeginBox();
                                EditorGUILayout.BeginHorizontal();
                                bool isBlockLevelListExpanded = BeginSimpleFoldoutBox("Block Shape Level List " + (i + 1));
                                EditorGUILayout.LabelField("", GUILayout.Width(5));
                                EditorGUILayout.EndHorizontal();
                                if (isBlockLevelListExpanded)
                                {
                                    if(gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList != null)
                                    {
                                        if (gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList.Length > 0)
                                        {
                                            GUILayout.Space(5);
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("Index : ", labelStyle, GUILayout.Width(120));
                                            EditorGUILayout.LabelField("Block Shape : ", labelStyle, GUILayout.Width(120));
                                            EditorGUILayout.LabelField("Probability : ", labelStyle, GUILayout.Width(120));
                                            EditorGUILayout.EndHorizontal();

                                            DrawLine();

                                            for (int j = 0; j < gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList.Length; j++)
                                            {
                                                EditorGUILayout.BeginHorizontal();
                                                EditorGUILayout.LabelField((j + 1).ToString(), labelStyle, GUILayout.Width(120));
                                                gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList[j].blockShape = (GameObject)EditorGUILayout.ObjectField(gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList[j].blockShape, typeof(GameObject), false, GUILayout.Width(120));
                                                gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList[j].spawnProbability = EditorGUILayout.FloatField(gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList[j].spawnProbability, inputStyle, GUILayout.Width(120));
                                                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                                if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                                                {
                                                    List<BlockShapeLevelListInfo> blockShapeList = new List<BlockShapeLevelListInfo>(gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList);
                                                    blockShapeList.Insert(j, new BlockShapeLevelListInfo());
                                                    gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList = blockShapeList.ToArray();
                                                }

                                                if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                                                {
                                                    List<BlockShapeLevelListInfo> blockShapeList = new List<BlockShapeLevelListInfo>(gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList);
                                                    blockShapeList.RemoveAt(j);
                                                    gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList = blockShapeList.ToArray();
                                                    return;
                                                }
                                                EditorGUILayout.EndHorizontal();
                                                GUILayout.Space(3);

                                            }
                                        }
                                    }
                                    if (DrawAddElementButton("Add Block Shape Element"))
                                    {
                                        if (gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList == null)
                                        {
                                            gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList = new BlockShapeLevelListInfo[0];
                                        }
                                        List<BlockShapeLevelListInfo> blockShapeList = new List<BlockShapeLevelListInfo>(gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList);
                                        blockShapeList.Add(new BlockShapeLevelListInfo());
                                        gamePlaySettings.blockShapeLevelInfo[i].blockShapeInfoList = blockShapeList.ToArray();
                                    }
                                }EndBox();
                            }
                            EndBox();
                        }
                        
                    }
                        
                    
                    if (DrawAddElementButton("Add Element"))
                    {

                        List<BlockShapeLevelInfo> blockShapeList = new List<BlockShapeLevelInfo>(gamePlaySettings.blockShapeLevelInfo);
                        blockShapeList.Add(new BlockShapeLevelInfo());
                        gamePlaySettings.blockShapeLevelInfo = blockShapeList.ToArray();
                    }
                }
                GUILayout.Space(5);
                DrawLine();
                EditorGUILayout.BeginHorizontal();
                bool isBlockSpriteColorsExpanded = BeginSimpleFoldoutBox("Block Sprite Colors");
                EditorGUILayout.LabelField("", GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                if (isBlockSpriteColorsExpanded)
                {
                    if (gamePlaySettings.blockSpriteColorTags.Length > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Index", labelStyle, GUILayout.Width(60));
                        EditorGUILayout.LabelField("Sprite Tag", labelStyle, GUILayout.Width(85));
                        EditorGUILayout.EndHorizontal();

                        DrawLine();

                        for (int i = 0; i < gamePlaySettings.blockSpriteColorTags.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(60));

                            gamePlaySettings.blockSpriteColorTags[i] = EditorGUILayout.TextField(gamePlaySettings.blockSpriteColorTags[i],
                                                                                            inputStyle, GUILayout.Width(80));

                            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                            {
                                List<string> blockShapeList = new List<string>(gamePlaySettings.blockSpriteColorTags);
                                blockShapeList.Insert(i, "");
                                gamePlaySettings.blockSpriteColorTags = blockShapeList.ToArray();
                            }

                            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                            {
                                List<string> blockShapeList = new List<string>(gamePlaySettings.blockSpriteColorTags);
                                blockShapeList.RemoveAt(i);
                                gamePlaySettings.blockSpriteColorTags = blockShapeList.ToArray();
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(3);
                        }

                    }
                    if (DrawAddElementButton("Add Element"))
                    {
                        List<string> blockShapeList = new List<string>(gamePlaySettings.blockSpriteColorTags);
                        blockShapeList.Add("");
                        gamePlaySettings.blockSpriteColorTags = blockShapeList.ToArray();
                    }

                }  
                
                GUILayout.Space(5);
                DrawLine();
                EditorGUILayout.BeginHorizontal();
                bool isLevelToLoadInfoExpanded = BeginSimpleFoldoutBox("Level To Load Info");
                EditorGUILayout.LabelField("", GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                if (isLevelToLoadInfoExpanded)
                {
                    if (gamePlaySettings.levelToLoadInfo != null)
                    {
                         if (gamePlaySettings.levelToLoadInfo.Length > 0)
                         {
                             EditorGUILayout.BeginHorizontal();
                             EditorGUILayout.LabelField("Level Number", labelStyle, GUILayout.Width(120));
                             EditorGUILayout.LabelField("Level To Load", labelStyle, GUILayout.Width(120));
                             EditorGUILayout.EndHorizontal();

                             DrawLine();

                             for (int i = 0; i < gamePlaySettings.levelToLoadInfo.Length; i++)
                             {
                                 EditorGUILayout.BeginHorizontal();

                                 EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(60));

                                 gamePlaySettings.levelToLoadInfo[i] = EditorGUILayout.IntField(gamePlaySettings.levelToLoadInfo[i],
                                     inputStyle, GUILayout.Width(80));

                                 EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                                 if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                                 {
                                     List<int> levelToLoadList = new List<int>(gamePlaySettings.levelToLoadInfo);
                                     levelToLoadList.Insert(i, 1);
                                     gamePlaySettings.levelToLoadInfo = levelToLoadList.ToArray();
                                 }

                                 if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                                 {
                                     List<int> levelToLoadList = new List<int>(gamePlaySettings.levelToLoadInfo);
                                     levelToLoadList.RemoveAt(i);
                                     gamePlaySettings.levelToLoadInfo = levelToLoadList.ToArray();
                                     return;
                                 }

                                 EditorGUILayout.EndHorizontal();
                                 GUILayout.Space(3);
                             }

                         }
                    }
                     
                    if (DrawAddElementButton("Add Element"))
                    {
                        List<int> levelToLoadList = new List<int>(gamePlaySettings.levelToLoadInfo);
                        levelToLoadList.Add(1);
                        gamePlaySettings.levelToLoadInfo = levelToLoadList.ToArray();
                    }

                }
                EndBox();
            }
            EndBox();
            EditorGUI.indentLevel = indentLevel;
        }

        void DrawTutorialSettings(SerializedProperty modeSettings, string modeName)
        {
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            BeginBox();

            SerializedProperty modeEnabled = modeSettings.FindPropertyRelative("modeEnabled");

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            bool isExpanded = BeginSimpleFoldoutBox(modeName);
            //EditorGUILayout.EndHorizontal();

            modeEnabled.boolValue = EditorGUILayout.Toggle(modeEnabled.boolValue, GUILayout.Width(30));
            EditorGUILayout.LabelField("", GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();

            if (isExpanded)
            {
                labelStyle.fontStyle = FontStyle.Normal;

                GUI.enabled = modeEnabled.boolValue;

                DrawLine();

                EditorGUILayout.BeginHorizontal();
                SerializedProperty blockSize = modeSettings.FindPropertyRelative("blockSize");
                EditorGUILayout.LabelField("Block Size : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                blockSize.floatValue = EditorGUILayout.FloatField(blockSize.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty blockSpace = modeSettings.FindPropertyRelative("blockSpace");
                EditorGUILayout.LabelField("Space B/W Blocks : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                blockSpace.floatValue = EditorGUILayout.FloatField(blockSpace.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();

                DrawLine();

                DrawBoldLabel("Block Shape Settings : ");
                GUILayout.Space(3);
                EditorGUILayout.BeginHorizontal();
                SerializedProperty allowRotation = modeSettings.FindPropertyRelative("allowRotation");
                EditorGUILayout.LabelField("Allow Rotation : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                allowRotation.boolValue = EditorGUILayout.Toggle(allowRotation.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty alwaysKeepFilled = modeSettings.FindPropertyRelative("alwaysKeepFilled");
                EditorGUILayout.LabelField("Always Keep Filled : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                alwaysKeepFilled.boolValue = EditorGUILayout.Toggle(alwaysKeepFilled.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty shapeInactiveSize = modeSettings.FindPropertyRelative("shapeInactiveSize");
                EditorGUILayout.LabelField("Inactive Size : ", labelStyle, GUILayout.Width(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                shapeInactiveSize.floatValue = EditorGUILayout.FloatField(shapeInactiveSize.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty shapeDragPositionOffset = modeSettings.FindPropertyRelative("shapeDragPositionOffset");
                EditorGUILayout.LabelField("Drag Position Offset : ", labelStyle, GUILayout.Width(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                shapeDragPositionOffset.floatValue = EditorGUILayout.FloatField(shapeDragPositionOffset.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                if (modeName == "Time Mode")
                {
                    DrawTimeModeAdditionalSettings();
                }

                if (modeName == "Blast Mode")
                {
                    DrawBlastModeAdditionalSettings();
                }
                //EndBox();
                GUI.enabled = true;
            }
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EndBox();
            EditorGUI.indentLevel = indentLevel;
        }

        void DrawGameModeSettings(SerializedProperty modeSettings, string modeName)
        {
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            BeginBox();

            SerializedProperty modeEnabled = modeSettings.FindPropertyRelative("modeEnabled");

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            bool isExpanded = BeginSimpleFoldoutBox(modeName);

            modeEnabled.boolValue = EditorGUILayout.Toggle(modeEnabled.boolValue, GUILayout.Width(30));
            EditorGUILayout.LabelField("", GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();

            if (isExpanded)
            {
                labelStyle.fontStyle = FontStyle.Normal;

                GUI.enabled = modeEnabled.boolValue;

                DrawLine();
                EditorGUILayout.BeginHorizontal();
                SerializedProperty boardSize = modeSettings.FindPropertyRelative("boardSize");
                EditorGUILayout.LabelField("Board Size : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                boardSize.intValue = EditorGUILayout.Popup(boardSize.intValue, boardSizes, popupStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty blockSize = modeSettings.FindPropertyRelative("blockSize");
                EditorGUILayout.LabelField("Block Size : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                blockSize.floatValue = EditorGUILayout.FloatField(blockSize.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty blockSpace = modeSettings.FindPropertyRelative("blockSpace");
                EditorGUILayout.LabelField("Space B/W Blocks : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                blockSpace.floatValue = EditorGUILayout.FloatField(blockSpace.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();

                DrawLine();

                SerializedProperty standardShapeAllowed = modeSettings.FindPropertyRelative("standardShapeAllowed");
                SerializedProperty advancedShapeAllowed = modeSettings.FindPropertyRelative("advancedShapeAllowed");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Standard Shapes : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                standardShapeAllowed.boolValue = EditorGUILayout.Toggle(standardShapeAllowed.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                if (!standardShapeAllowed.boolValue && !advancedShapeAllowed.boolValue)
                {
                    advancedShapeAllowed.boolValue = true;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Advanced Shapes : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                advancedShapeAllowed.boolValue = EditorGUILayout.Toggle(advancedShapeAllowed.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                DrawLine();


                if (!standardShapeAllowed.boolValue && !advancedShapeAllowed.boolValue)
                {
                    standardShapeAllowed.boolValue = true;
                }

                DrawBoldLabel("Block Shape Settings : ");
                GUILayout.Space(3);
                EditorGUILayout.BeginHorizontal();
                SerializedProperty allowRotation = modeSettings.FindPropertyRelative("allowRotation");
                EditorGUILayout.LabelField("Allow Rotation : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                allowRotation.boolValue = EditorGUILayout.Toggle(allowRotation.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty alwaysKeepFilled = modeSettings.FindPropertyRelative("alwaysKeepFilled");
                EditorGUILayout.LabelField("Always Keep Filled : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                alwaysKeepFilled.boolValue = EditorGUILayout.Toggle(alwaysKeepFilled.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty shapeInactiveSize = modeSettings.FindPropertyRelative("shapeInactiveSize");
                EditorGUILayout.LabelField("Inactive Size : ", labelStyle, GUILayout.Width(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                shapeInactiveSize.floatValue = EditorGUILayout.FloatField(shapeInactiveSize.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty shapeDragPositionOffset = modeSettings.FindPropertyRelative("shapeDragPositionOffset");
                EditorGUILayout.LabelField("Drag Position Offset : ", labelStyle, GUILayout.Width(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                shapeDragPositionOffset.floatValue = EditorGUILayout.FloatField(shapeDragPositionOffset.floatValue, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                DrawLine();
                DrawBoldLabel("Additional Settings : ");
                GUILayout.Space(3);
                EditorGUILayout.BeginHorizontal();
                SerializedProperty allowRescueGame = modeSettings.FindPropertyRelative("allowRescueGame");
                EditorGUILayout.LabelField("Allow Rescue : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                allowRescueGame.boolValue = EditorGUILayout.Toggle(allowRescueGame.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty saveProgress = modeSettings.FindPropertyRelative("saveProgress");
                EditorGUILayout.LabelField("Save Progress : ", labelStyle, GUILayout.MaxWidth(130));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                saveProgress.boolValue = EditorGUILayout.Toggle(saveProgress.boolValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);

                if (modeName == "Time Mode")
                {
                    DrawTimeModeAdditionalSettings();
                }

                if (modeName == "Blast Mode")
                {
                    DrawBlastModeAdditionalSettings();
                }
                //EndBox();
                GUI.enabled = true;
            }
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EndBox();
            EditorGUI.indentLevel = indentLevel;
        }


        void DrawTimeModeAdditionalSettings()
        {
            DrawLine();
            DrawBoldLabel("Timer Settings : ");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Timer (Sec.) : ", labelStyle, GUILayout.Width(170));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            gamePlaySettings.initialTime = EditorGUILayout.IntField(gamePlaySettings.initialTime, inputStyle, GUILayout.Width(70));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Seconds On Line Break : ", labelStyle, GUILayout.Width(170));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            gamePlaySettings.addSecondsOnLineBreak = EditorGUILayout.IntField(gamePlaySettings.addSecondsOnLineBreak, inputStyle, GUILayout.Width(70));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);
        }

        void DrawBlastModeAdditionalSettings()
        {
            DrawLine();
            DrawBoldLabel("Bomb Placement Settings : ");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Counter : ", labelStyle, GUILayout.Width(170));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            gamePlaySettings.blastModeCounter = EditorGUILayout.IntField(gamePlaySettings.blastModeCounter, inputStyle, GUILayout.Width(70));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Bomb After Moves : ", labelStyle, GUILayout.Width(170));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            gamePlaySettings.addBombAfterMoves = EditorGUILayout.IntField(gamePlaySettings.addBombAfterMoves, inputStyle, GUILayout.Width(70));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);
        }

        void DrawBlockShapesInfo()
        {
            labelStyle.fontStyle = FontStyle.Bold;

            bool isExpanded = BeginFoldoutBox("GamePlay Block Shapes Settings : ");
            int indentLevel = EditorGUI.indentLevel;

            bool isStandardShapesExpanded = false;
            bool isAdvanceShapesExpanded = false;

            if (isExpanded)
            {
                if (standardBlockShapesInfo != null)
                {
                    isStandardShapesExpanded = BeginFoldoutBox("Standard Shapes");

                    if (isStandardShapesExpanded)
                    {
                        DrawShapeInfo(standardBlockShapesInfo);
                    }
                    EndBox();
                }
                else
                {
                    standardBlockShapesInfo = serializedObject.FindProperty("standardBlockShapesInfo");
                }

                if (advancedBlockShapesInfo != null)
                {
                    isAdvanceShapesExpanded = BeginFoldoutBox("Advance Shapes");

                    if (isAdvanceShapesExpanded)
                    {
                        DrawShapeInfo(advancedBlockShapesInfo);
                    }
                    EndBox();
                }
                else
                {
                    advancedBlockShapesInfo = serializedObject.FindProperty("advancedBlockShapesInfo");
                }

                if (isStandardShapesExpanded || isAdvanceShapesExpanded)
                {
                    GUI.backgroundColor = Color.grey;
                    EditorGUILayout.HelpBox("Prob. Is Probability of how often shape will spawn during game. Value Should be 1 to 10.", MessageType.Info, true);
                    GUI.backgroundColor = Color.white;
                }
            }
            EndBox();
            EditorGUI.indentLevel = indentLevel;
        }

        void DrawShapeInfo(SerializedProperty shapeInfo)
        {
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            inputStyle.fontStyle = FontStyle.Bold;
            inputStyle.alignment = TextAnchor.MiddleCenter;

            if (shapeInfo.arraySize > 0)
            {
                BeginBox();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField("Shape Prefab", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Sprite Tag", labelStyle, GUILayout.Width(80));
                EditorGUILayout.LabelField("Prob.", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                EditorGUILayout.EndHorizontal();


                DrawLine();

                for (int i = 0; i < shapeInfo.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(50));

                    SerializedProperty blockShape = shapeInfo.GetArrayElementAtIndex(i).FindPropertyRelative("blockShape");
                    blockShape.objectReferenceValue = EditorGUILayout.ObjectField(blockShape.objectReferenceValue, typeof(GameObject), false, GUILayout.Width(120));

                    SerializedProperty blockSpriteTag = shapeInfo.GetArrayElementAtIndex(i).FindPropertyRelative("blockSpriteTag");
                    blockSpriteTag.stringValue = EditorGUILayout.TextField(blockSpriteTag.stringValue, inputStyle, GUILayout.Width(80));

                    SerializedProperty spawnProbability = shapeInfo.GetArrayElementAtIndex(i).FindPropertyRelative("spawnProbability");
                    spawnProbability.intValue = EditorGUILayout.IntField(spawnProbability.intValue, inputStyle, GUILayout.Width(50));


                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        shapeInfo.InsertArrayElementAtIndex(i);
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        shapeInfo.DeleteArrayElementAtIndex(i);
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
                EndBox();
            }

            //EditorGUI.indentLevel = 0;

            if (DrawAddElementButton("Add Element"))
            {
                shapeInfo.arraySize += 1;
            }
        }

        void DrawRewardSettings()
        {
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            bool isExpanded = BeginFoldoutBox("Reward Settings");
            int indentLevel = EditorGUI.indentLevel;

            if (isExpanded)
            {
                EditorGUI.indentLevel = 1;

                GUILayout.Space(5);
                labelStyle.fontStyle = FontStyle.Normal;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Reward On Game Over ? ", labelStyle, GUILayout.Width(200));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                gamePlaySettings.rewardOnGameOver = EditorGUILayout.Toggle(gamePlaySettings.rewardOnGameOver, GUILayout.Width(30));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();

                if (gamePlaySettings.rewardOnGameOver)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Give Fixed Reward ? ", labelStyle, GUILayout.Width(200));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    gamePlaySettings.giveFixedReward = EditorGUILayout.Toggle(gamePlaySettings.giveFixedReward, GUILayout.Width(30));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                    EditorGUILayout.EndHorizontal();

                    if (gamePlaySettings.giveFixedReward)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Game Over Reward (COINS) : ", labelStyle, GUILayout.Width(200));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        gamePlaySettings.fixedRewardAmount = EditorGUILayout.IntField(gamePlaySettings.fixedRewardAmount, inputStyle, GUILayout.Width(70));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Reward/Line Complete (COINS) : ", labelStyle, GUILayout.Width(200));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        gamePlaySettings.rewardPerLineCompleted = EditorGUILayout.FloatField(gamePlaySettings.rewardPerLineCompleted, inputStyle, GUILayout.Width(70));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EndBox();
            EditorGUI.indentLevel = indentLevel;
        }

        void DrawScoreSettings()
        {
            labelStyle.fontStyle = FontStyle.Bold;
            bool isExpanded = BeginFoldoutBox("Score Settings");
            int indentLevel = EditorGUI.indentLevel;

            if (isExpanded)
            {
                EditorGUI.indentLevel = 1;

                GUILayout.Space(5);
                labelStyle.fontStyle = FontStyle.Normal;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Score/Block Remove : ", labelStyle, GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                gamePlaySettings.blockScore = EditorGUILayout.IntField(gamePlaySettings.blockScore, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Line Break Score : ", labelStyle, GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                gamePlaySettings.singleLineBreakScore = EditorGUILayout.IntField(gamePlaySettings.singleLineBreakScore, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Extra Bonus/Line (Multiline) : ", labelStyle, GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                gamePlaySettings.multiLineScoreMultiplier = EditorGUILayout.IntField(gamePlaySettings.multiLineScoreMultiplier, inputStyle, GUILayout.Width(70));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.Width(5));
                EditorGUILayout.EndHorizontal();
            }
            EndBox();
            EditorGUI.indentLevel = indentLevel;
        }
    }
}