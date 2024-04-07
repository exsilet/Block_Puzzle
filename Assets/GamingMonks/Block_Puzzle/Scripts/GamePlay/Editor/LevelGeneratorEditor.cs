using GamingMonks;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelSO))]
public class LevelGeneratorEditor : CustomInspectorHelper
{
    private bool cache = false;
    LevelSO targetlevel;
    SerializedProperty levels;

    GUIStyle labelStyle;
    GUIStyle inputStyle;
    GUIStyle popupStyle;


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!cache)
        {
            targetlevel = (LevelSO)target;
            levels = serializedObject.FindProperty("Levels");

            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Normal;

            inputStyle = new GUIStyle(GUI.skin.textField);
            inputStyle.fontStyle = FontStyle.Bold;
            inputStyle.alignment = TextAnchor.MiddleCenter;

            popupStyle = EditorStyles.popup;
            popupStyle.alignment = TextAnchor.MiddleCenter;
            cache = true;
        }

        DrawLevelsInfo();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetlevel);
    }

    private void DrawLevelsInfo()
    {
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        inputStyle.fontStyle = FontStyle.Bold;
        inputStyle.alignment = TextAnchor.MiddleCenter;

        bool isExpanded = BeginFoldoutBox("Level Settings");
        int indentLevel = EditorGUI.indentLevel;

        if (isExpanded)
        {
            EditorGUI.indentLevel = 1;
            DrawLevel();
        }
        EndBox();
        EditorGUI.indentLevel = indentLevel;
    }
    private void DrawLevel()
    {
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        inputStyle.fontStyle = FontStyle.Bold;
        inputStyle.alignment = TextAnchor.MiddleCenter;

        if (levels != null)
        {
            if (levels.arraySize > 0)
            {
                for (int i = 0; i < levels.arraySize; i++)
                {
                    BeginBox();

                    EditorGUILayout.BeginHorizontal();
                    bool isLevelExpanded = BeginSimpleFoldoutBox("Level " + (i + 1));
                    //EditorGUILayout.PropertyField(levels.GetArrayElementAtIndex(i), new GUIContent("Level " + (i + 1)), GUILayout.Width(120));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        levels.InsertArrayElementAtIndex(i);
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        levels.DeleteArrayElementAtIndex(i);
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    if (isLevelExpanded)
                    {
                        EditorGUI.indentLevel = 2;
                        EditorGUILayout.Space();
                        DrawMode(i);
                        EditorGUILayout.Space();
                        DrawSpriteInfo(i);
                        EditorGUILayout.Space();
                        DrawStandardBlockShape(i);
                        EditorGUILayout.Space();
                        DrawAdvancedBlockShape(i);
                        EditorGUILayout.Space();
                        DrawFixedBlockShape(i);
                        EditorGUILayout.Space();
                        DrawGoal(i);
                        EditorGUILayout.Space();
                        DrawSpecialBlockShape(i);
                        EditorGUILayout.Space();
                        DrawConveyorBelts(i);
                        EditorGUILayout.Space();
                        DrawJewelMachine(i);
                        EditorGUILayout.Space();
                        DrawBlockerStick(i);
                        EditorGUILayout.Space();
                        DrawBombs(i);
                        EditorGUILayout.Space();
                        DrawGuide(i);
                        EditorGUILayout.Space();
                        DrawInitialGrid(i);
                        
                    }
                    EndBox();
                }

            }
        }

        if (DrawAddElementButton("Add Level"))
        {
            levels.arraySize += 1;
        }
    }

    private void DrawMode(int index)
    {
        BeginBox();
        bool isModeExpanded = BeginSimpleFoldoutBox("Mode");

        if (isModeExpanded)
        {
            DrawLine();
            EditorGUI.indentLevel = 3;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Game Mode", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].Mode.GameMode = (GameMode)EditorGUILayout.EnumPopup(targetlevel.Levels[index].Mode.GameMode, popupStyle);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);

            bool isGameModeSettingsExpanded = BeginSimpleFoldoutBox("Game Mode Settings");
            if (isGameModeSettingsExpanded)
            {
                DrawGameModeSettings(index);
            }
        }

        EndBox();

    }
    private void DrawGameModeSettings(int index)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Mode Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.modeEnabled = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.modeEnabled, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        if(targetlevel.Levels[index].rows.Length > 0)
        {
            GUI.backgroundColor = Color.grey;
            EditorGUILayout.EnumPopup(targetlevel.Levels[index].Mode.GameModeSettings.boardSize, popupStyle);
            EditorGUILayout.HelpBox("If you want to change the board size then first clear the Grid !", MessageType.Info, true);
            GUI.backgroundColor = Color.white;
        }
        else
        {
            EditorGUI.indentLevel = 3;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Board Size", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].Mode.GameModeSettings.boardSize = (BoardSize)EditorGUILayout.EnumPopup(targetlevel.Levels[index].Mode.GameModeSettings.boardSize, popupStyle);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Block Size", EditorStyles.boldLabel, GUILayout.MaxWidth(200));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.blockSize = EditorGUILayout.FloatField(targetlevel.Levels[index].Mode.GameModeSettings.blockSize);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Block Space", EditorStyles.boldLabel, GUILayout.MaxWidth(200));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.blockSpace = EditorGUILayout.FloatField(targetlevel.Levels[index].Mode.GameModeSettings.blockSpace);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Maximum Moves Allowed", EditorStyles.boldLabel, GUILayout.MaxWidth(200));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.maxMoves = EditorGUILayout.IntField(targetlevel.Levels[index].Mode.GameModeSettings.maxMoves);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Standard Shapes Allowed", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.standardShapeAllowed = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.standardShapeAllowed, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Advanced Shapes Allowed", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.advancedShapeAllowed = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.advancedShapeAllowed, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select Shapes Color from this Level", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.pickShapeColorsFromThisLevel = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.pickShapeColorsFromThisLevel, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select Shapes from this Level", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.pickShapeFromThisLevel = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.pickShapeFromThisLevel, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Allow Rotation", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.allowRotation = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.allowRotation, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Always Keep Filled", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.alwaysKeepFilled = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.alwaysKeepFilled, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Shape Inactive Size", EditorStyles.boldLabel, GUILayout.MaxWidth(190));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.shapeInactiveSize = EditorGUILayout.FloatField(targetlevel.Levels[index].Mode.GameModeSettings.shapeInactiveSize);
        EditorGUILayout.EndHorizontal();

        // GUILayout.Space(3);
        //
        // EditorGUILayout.BeginHorizontal();
        // EditorGUILayout.LabelField("Shape Drag Position Offset", EditorStyles.boldLabel, GUILayout.MaxWidth(190));
        // EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        // targetlevel.Levels[index].Mode.GameModeSettings.shapeDragPositionOffset = EditorGUILayout.FloatField(targetlevel.Levels[index].Mode.GameModeSettings.shapeDragPositionOffset);
        // EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Allow Rescue Game", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.allowRescueGame = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.allowRescueGame, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Progress", EditorStyles.boldLabel, GUILayout.MaxWidth(260));
        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
        targetlevel.Levels[index].Mode.GameModeSettings.saveProgress = EditorGUILayout.Toggle(targetlevel.Levels[index].Mode.GameModeSettings.saveProgress, EditorStyles.toggle);
        EditorGUILayout.EndHorizontal();

    }
    private void DrawSpecialBlockShape(int index)
    {
        BeginBox();
        bool isSpecialBlockShapeExpanded = BeginSimpleFoldoutBox("Special Block Shape");

        if (isSpecialBlockShapeExpanded)
        {

            if (targetlevel.Levels[index].SpecialBlockShape.Length > 0)
            {
                DrawLine();
                for (int i = 0; i < targetlevel.Levels[index].SpecialBlockShape.Length; i++)
                {
                    BeginBox();

                    EditorGUILayout.BeginHorizontal();
                    bool isSpecialBlockShapeItemExpanded = BeginSimpleFoldoutBox("Special Block Shape " + (i + 1));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        List<SpecialBlock> specialBlockShapeList = new List<SpecialBlock>(targetlevel.Levels[index].SpecialBlockShape);
                        specialBlockShapeList.Insert(i, new SpecialBlock());
                        targetlevel.Levels[index].SpecialBlockShape = specialBlockShapeList.ToArray();
                        Debug.Log(targetlevel.Levels[index].SpecialBlockShape.Length);
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        List<SpecialBlock> specialBlockShapeList = new List<SpecialBlock>(targetlevel.Levels[index].SpecialBlockShape);
                        specialBlockShapeList.RemoveAt(i);
                        targetlevel.Levels[index].SpecialBlockShape = specialBlockShapeList.ToArray();
                        Debug.Log(targetlevel.Levels[index].SpecialBlockShape.Length);
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    if (isSpecialBlockShapeItemExpanded)
                    {
                        EditorGUI.indentLevel = 3;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Allow Special Block Shape", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].SpecialBlockShape[i].allowSpecialBlockShape = EditorGUILayout.Toggle(targetlevel.Levels[index].SpecialBlockShape[i].allowSpecialBlockShape, EditorStyles.toggle);
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(3);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Probability", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].SpecialBlockShape[i].probability = EditorGUILayout.IntField(targetlevel.Levels[index].SpecialBlockShape[i].probability);
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(3);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Sprite Type", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].SpecialBlockShape[i].spriteType = (SpriteType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].SpecialBlockShape[i].spriteType, popupStyle, GUILayout.MaxWidth(250));
                        EditorGUILayout.EndHorizontal();
                    }
                    EndBox();
                }

            }

            //EditorGUI.indentLevel = 0;

            if (DrawAddElementButton("Add Element"))
            {
                List<SpecialBlock> specialBlockShapeList = new List<SpecialBlock>(targetlevel.Levels[index].SpecialBlockShape);
                specialBlockShapeList.Add(new SpecialBlock());
                targetlevel.Levels[index].SpecialBlockShape = specialBlockShapeList.ToArray();
            }
        }
        EndBox();
    }
    private void DrawConveyorBelts(int index)
    {
        BeginBox();
        bool isConveyorBeltsExpanded = BeginSimpleFoldoutBox("Conveyor Belts");
        if (isConveyorBeltsExpanded)
        {
            DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].ConveyorBelts.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.enabled, EditorStyles.toggle);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);

            if (targetlevel.Levels[index].ConveyorBelts.enabled)
            {
                bool isCircularConveyorExpanded = BeginSimpleFoldoutBox("Circular Conveyor");
                if (isCircularConveyorExpanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    targetlevel.Levels[index].ConveyorBelts.CircularConveyor.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.CircularConveyor.enabled, EditorStyles.toggle);
                    EditorGUILayout.EndHorizontal();
                    if (targetlevel.Levels[index].ConveyorBelts.CircularConveyor.enabled)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Length", EditorStyles.boldLabel, GUILayout.MaxWidth(150));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].ConveyorBelts.CircularConveyor.length = EditorGUILayout.IntSlider(targetlevel.Levels[index].ConveyorBelts.CircularConveyor.length, 1, 10);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Height", EditorStyles.boldLabel, GUILayout.MaxWidth(150));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].ConveyorBelts.CircularConveyor.height = EditorGUILayout.IntSlider(targetlevel.Levels[index].ConveyorBelts.CircularConveyor.height, 1, 10);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Starting Position", EditorStyles.boldLabel, GUILayout.MaxWidth(150));
                        //EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        var x = EditorGUILayout.FloatField(targetlevel.Levels[index].ConveyorBelts.CircularConveyor.startPosition.x);
                        var y = EditorGUILayout.FloatField(targetlevel.Levels[index].ConveyorBelts.CircularConveyor.startPosition.y);
                        targetlevel.Levels[index].ConveyorBelts.CircularConveyor.startPosition = new Vector2(x, y);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                bool isLinearConveyorExpanded = BeginSimpleFoldoutBox("Linear Conveyor");
                if (isLinearConveyorExpanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    targetlevel.Levels[index].ConveyorBelts.LinearConveyor.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.enabled, EditorStyles.toggle);
                    EditorGUILayout.EndHorizontal();
                    if (targetlevel.Levels[index].ConveyorBelts.LinearConveyor.enabled)
                    {
                        //locations
                        bool isPositionsExpanded = BeginSimpleFoldoutBox("Positions");
                        if (isPositionsExpanded)
                        {
                            if (targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations.Length > 0)
                            {
                                BeginBox();
                                for (int i = 0; i < targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations.Length; i++)
                                {
                                    BeginBox();
                                    EditorGUILayout.BeginHorizontal();
                                    bool isPositionsItemExpanded = BeginSimpleFoldoutBox("Position " + (i + 1));
                                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                                    {
                                        List<ConveyorLocation> positionsList = new List<ConveyorLocation>(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations);
                                        positionsList.Insert(i, new ConveyorLocation());
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations = positionsList.ToArray();
                                    }

                                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                                    {
                                        List<ConveyorLocation> positionShapeList = new List<ConveyorLocation>(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations);
                                        positionShapeList.RemoveAt(i);
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations = positionShapeList.ToArray();
                                        return;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    GUILayout.Space(3);
                                    if (isPositionsItemExpanded)
                                    {
                                        EditorGUI.indentLevel = 4;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("On Row", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].onRow = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].onRow, EditorStyles.toggle);
                                        EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);

                                        EditorGUI.indentLevel = 4;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("On Column", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].onColoum = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].onColoum, EditorStyles.toggle);
                                        EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);

                                        EditorGUI.indentLevel = 4;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Reverse", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].reverse = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].reverse, EditorStyles.toggle);
                                        EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Conveyor Width", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].conveyorWidth = EditorGUILayout.IntSlider(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].conveyorWidth, 1, 2);
                                        EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);

                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Position On Grid", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                        targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].PositionOnGrid = EditorGUILayout.IntSlider(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations[i].PositionOnGrid, 0, 9);
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EndBox();
                                }
                                EndBox();
                            }
                            if (DrawAddElementButton("Add Element"))
                            {
                                List<ConveyorLocation> positionsList = new List<ConveyorLocation>(targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations);
                                positionsList.Add(new ConveyorLocation());
                                targetlevel.Levels[index].ConveyorBelts.LinearConveyor.Locations = positionsList.ToArray();
                            }

                        }
                    }
                }

                bool isSingleBlockConveyorExpanded = BeginSimpleFoldoutBox("Single Block Conveyor");
                if (isSingleBlockConveyorExpanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.enabled, EditorStyles.toggle);
                    EditorGUILayout.EndHorizontal();
                    if (targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.enabled)
                    {
                        bool isPositionsExpanded = BeginSimpleFoldoutBox("Positions");
                        if (isPositionsExpanded)
                        {
                            if (targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors.Length > 0)
                            {
                                BeginBox();
                                for (int i = 0; i < targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors.Length; i++)
                                {
                                    BeginBox();
                                    EditorGUILayout.BeginHorizontal();
                                    bool isPositionsItemExpanded = BeginSimpleFoldoutBox("Position " + (i + 1));
                                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                                    {
                                        List<BlockConveyors> positionsList = new List<BlockConveyors>(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors);
                                        positionsList.Insert(i, new BlockConveyors());
                                        targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors = positionsList.ToArray();
                                    }

                                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                                    {
                                        List<BlockConveyors> positionShapeList = new List<BlockConveyors>(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors);
                                        positionShapeList.RemoveAt(i);
                                        targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors = positionShapeList.ToArray();
                                        return;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    GUILayout.Space(3);
                                    if (isPositionsItemExpanded)
                                    {
                                        EditorGUI.indentLevel = 4;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Position", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                                        var x = EditorGUILayout.FloatField(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors[i].position.x);
                                        var y = EditorGUILayout.FloatField(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors[i].position.y);
                                        targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors[i].position = new Vector2(x, y);
                                        EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);

                                        EditorGUI.indentLevel = 4;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Conveyor Type", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                                        targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors[i].conveyorType = (ConveyorType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors[i].conveyorType, popupStyle);
                                        EditorGUILayout.EndHorizontal();

                                        //GUILayout.Space(3);

                                    }
                                    EndBox();
                                }
                                EndBox();
                            }
                            if (DrawAddElementButton("Add Element"))
                            {
                                List<BlockConveyors> positionsList = new List<BlockConveyors>(targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors);
                                positionsList.Add(new BlockConveyors());
                                targetlevel.Levels[index].ConveyorBelts.SingleBlockConveyor.blockConveyors = positionsList.ToArray();
                            }

                        }
                    }
                }
            }
            // GUI.backgroundColor = Color.grey;
            // EditorGUILayout.HelpBox("Only Even Values supported!", MessageType.Info, true);
            // GUI.backgroundColor = Color.white;
        }
        EndBox();
    }

    private void DrawSpriteInfo(int index)
    {
        BeginBox();
        bool isSpriteInfoExpanded = BeginSimpleFoldoutBox("Sprite Colours");

        if (isSpriteInfoExpanded)
        {
            EditorGUI.indentLevel = 1;
            if (targetlevel.Levels[index].spriteInfo.Length > 0)
            {
                BeginBox();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index", labelStyle, GUILayout.Width(50));
                //EditorGUILayout.LabelField("Shape Prefab", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Sprite Tag", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Prob.", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                EditorGUILayout.EndHorizontal();

                DrawLine();
                for (int i = 0; i < targetlevel.Levels[index].spriteInfo.Length; i++)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(50));

                    //EditorGUI.indentLevel = 3;
                    //targetlevel.Levels[index].spriteInfo[i].blockShape = (GameObject)EditorGUILayout.ObjectField(targetlevel.Levels[index].standardBlockShapesInfo[i].blockShape, typeof(GameObject), false, GUILayout.Width(120));

                    targetlevel.Levels[index].spriteInfo[i].spriteTag = EditorGUILayout.TextField(targetlevel.Levels[index].spriteInfo[i].spriteTag, inputStyle, GUILayout.Width(120));

                    targetlevel.Levels[index].spriteInfo[i].probability = EditorGUILayout.IntField(targetlevel.Levels[index].spriteInfo[i].probability, inputStyle, GUILayout.Width(120));

                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        List<SpriteInfo> standardBlockShapeList = new List<SpriteInfo>(targetlevel.Levels[index].spriteInfo);
                        standardBlockShapeList.Insert(i, new SpriteInfo());
                        targetlevel.Levels[index].spriteInfo = standardBlockShapeList.ToArray();
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        List<SpriteInfo> standardBlockShapeList = new List<SpriteInfo>(targetlevel.Levels[index].spriteInfo);
                        standardBlockShapeList.RemoveAt(i);
                        targetlevel.Levels[index].spriteInfo = standardBlockShapeList.ToArray();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);

                }
                EndBox();

            }

            if (DrawAddElementButton("Add Element"))
            {
                List<SpriteInfo> standardBlockShapeList = new List<SpriteInfo>(targetlevel.Levels[index].spriteInfo);
                standardBlockShapeList.Add(new SpriteInfo());
                targetlevel.Levels[index].spriteInfo = standardBlockShapeList.ToArray();
            }
        }
        EndBox();
    }

    private void DrawStandardBlockShape(int index)
    {
        BeginBox();
        bool isStandardBlockShapeExpanded = BeginSimpleFoldoutBox("Standard Block Shape");

        if (isStandardBlockShapeExpanded)
        {
            EditorGUI.indentLevel = 1;
            if (targetlevel.Levels[index].standardBlockShapesInfo.Length > 0)
            {
                BeginBox();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField("Shape Prefab", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Sprite Tag", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Prob.", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                EditorGUILayout.EndHorizontal();

                DrawLine();
                for (int i = 0; i < targetlevel.Levels[index].standardBlockShapesInfo.Length; i++)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(50));

                    //EditorGUI.indentLevel = 3;
                    targetlevel.Levels[index].standardBlockShapesInfo[i].blockShape = (GameObject)EditorGUILayout.ObjectField(targetlevel.Levels[index].standardBlockShapesInfo[i].blockShape, typeof(GameObject), false, GUILayout.Width(120));

                    targetlevel.Levels[index].standardBlockShapesInfo[i].blockSpriteTag = EditorGUILayout.TextField(targetlevel.Levels[index].standardBlockShapesInfo[i].blockSpriteTag, inputStyle, GUILayout.Width(120));

                    targetlevel.Levels[index].standardBlockShapesInfo[i].spawnProbability = EditorGUILayout.IntField(targetlevel.Levels[index].standardBlockShapesInfo[i].spawnProbability, inputStyle, GUILayout.Width(120));

                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        List<BlockShapeInfo> standardBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].standardBlockShapesInfo);
                        standardBlockShapeList.Insert(i, new BlockShapeInfo());
                        targetlevel.Levels[index].standardBlockShapesInfo = standardBlockShapeList.ToArray();
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        List<BlockShapeInfo> standardBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].standardBlockShapesInfo);
                        standardBlockShapeList.RemoveAt(i);
                        targetlevel.Levels[index].standardBlockShapesInfo = standardBlockShapeList.ToArray();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);

                }
                EndBox();

            }

            if (DrawAddElementButton("Add Element"))
            {
                List<BlockShapeInfo> standardBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].standardBlockShapesInfo);
                standardBlockShapeList.Add(new BlockShapeInfo());
                targetlevel.Levels[index].standardBlockShapesInfo = standardBlockShapeList.ToArray();
            }
        }
        EndBox();
    }
    private void DrawAdvancedBlockShape(int index)
    {
        BeginBox();
        bool isAdvancedBlockShapeExpanded = BeginSimpleFoldoutBox("Advance Block Shape");

        if (isAdvancedBlockShapeExpanded)
        {
            EditorGUI.indentLevel = 1;
            if (targetlevel.Levels[index].advancedBlockShapesInfo.Length > 0)
            {
                BeginBox();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField("Shape Prefab", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Sprite Tag", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Prob.", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                EditorGUILayout.EndHorizontal();

                DrawLine();
                for (int i = 0; i < targetlevel.Levels[index].advancedBlockShapesInfo.Length; i++)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(50));

                    //EditorGUI.indentLevel = 3;
                    targetlevel.Levels[index].advancedBlockShapesInfo[i].blockShape = (GameObject)EditorGUILayout.ObjectField(targetlevel.Levels[index].advancedBlockShapesInfo[i].blockShape, typeof(GameObject), false, GUILayout.Width(120));

                    targetlevel.Levels[index].advancedBlockShapesInfo[i].blockSpriteTag = EditorGUILayout.TextField(targetlevel.Levels[index].advancedBlockShapesInfo[i].blockSpriteTag, inputStyle, GUILayout.Width(120));

                    targetlevel.Levels[index].advancedBlockShapesInfo[i].spawnProbability = EditorGUILayout.IntField(targetlevel.Levels[index].advancedBlockShapesInfo[i].spawnProbability, inputStyle, GUILayout.Width(120));

                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        List<BlockShapeInfo> fixedBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].advancedBlockShapesInfo);
                        fixedBlockShapeList.Insert(i, new BlockShapeInfo());
                        targetlevel.Levels[index].advancedBlockShapesInfo = fixedBlockShapeList.ToArray();
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        List<BlockShapeInfo> fixedBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].advancedBlockShapesInfo);
                        fixedBlockShapeList.RemoveAt(i);
                        targetlevel.Levels[index].advancedBlockShapesInfo = fixedBlockShapeList.ToArray();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);

                }
                EndBox();

            }

            if (DrawAddElementButton("Add Element"))
            {
                List<BlockShapeInfo> advancedBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].advancedBlockShapesInfo);
                advancedBlockShapeList.Add(new BlockShapeInfo());
                targetlevel.Levels[index].advancedBlockShapesInfo = advancedBlockShapeList.ToArray();
            }
        }
        EndBox();
    }
    private void DrawFixedBlockShape(int index)
    {
        BeginBox();
        bool isFixedBlockShapeExpanded = BeginSimpleFoldoutBox("Fixed Block Shape");

        if (isFixedBlockShapeExpanded)
        {
            EditorGUI.indentLevel = 1;
            if (targetlevel.Levels[index].FixedBlockShape.Length > 0)
            {
                BeginBox();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index", labelStyle, GUILayout.Width(50));
                EditorGUILayout.LabelField("Shape Prefab", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Sprite Tag", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("Rotation", labelStyle, GUILayout.Width(120));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                EditorGUILayout.EndHorizontal();

                DrawLine();
                for (int i = 0; i < targetlevel.Levels[index].FixedBlockShape.Length; i++)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField((i + 1).ToString(), labelStyle, GUILayout.Width(50));

                    //EditorGUI.indentLevel = 3;
                    targetlevel.Levels[index].FixedBlockShape[i].blockShape = (GameObject)EditorGUILayout.ObjectField(targetlevel.Levels[index].FixedBlockShape[i].blockShape, typeof(GameObject), false, GUILayout.Width(120));

                    targetlevel.Levels[index].FixedBlockShape[i].blockSpriteTag = EditorGUILayout.TextField(targetlevel.Levels[index].FixedBlockShape[i].blockSpriteTag, inputStyle, GUILayout.Width(120));

                    targetlevel.Levels[index].FixedBlockShape[i].spawnProbability = EditorGUILayout.IntField(targetlevel.Levels[index].FixedBlockShape[i].spawnProbability, inputStyle, GUILayout.Width(120));

                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        List<BlockShapeInfo> fixedBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].FixedBlockShape);
                        fixedBlockShapeList.Insert(i, new BlockShapeInfo());
                        targetlevel.Levels[index].FixedBlockShape = fixedBlockShapeList.ToArray();
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        List<BlockShapeInfo> fixedBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].FixedBlockShape);
                        fixedBlockShapeList.RemoveAt(i);
                        targetlevel.Levels[index].FixedBlockShape = fixedBlockShapeList.ToArray();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);

                }
                EndBox();

            }

            if (DrawAddElementButton("Add Element"))
            {
                List<BlockShapeInfo> fixedBlockShapeList = new List<BlockShapeInfo>(targetlevel.Levels[index].FixedBlockShape);
                fixedBlockShapeList.Add(new BlockShapeInfo());
                targetlevel.Levels[index].FixedBlockShape = fixedBlockShapeList.ToArray();
            }
        }
        EndBox();
    }
    private void DrawGoal(int index)
    {
        BeginBox();
        bool isGoalExpanded = BeginSimpleFoldoutBox("Goal");
        if (isGoalExpanded)
        {
            if (targetlevel.Levels[index].Goal.Length > 0)
            {
                DrawLine();
                for (int i = 0; i < targetlevel.Levels[index].Goal.Length; i++)
                {
                    BeginBox();

                    EditorGUILayout.BeginHorizontal();
                    bool isGoalItemExpanded = BeginSimpleFoldoutBox("Goal " + (i + 1));
                    EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                    {
                        List<LevelGoal> goalList = new List<LevelGoal>(targetlevel.Levels[index].Goal);
                        goalList.Insert(i, new LevelGoal());
                        targetlevel.Levels[index].Goal = goalList.ToArray();
                    }

                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                    {
                        List<LevelGoal> goalList = new List<LevelGoal>(targetlevel.Levels[index].Goal);
                        goalList.RemoveAt(i);
                        targetlevel.Levels[index].Goal = goalList.ToArray();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    if (isGoalItemExpanded)
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Sprite Type", EditorStyles.boldLabel, GUILayout.MaxWidth(140));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].Goal[i].spriteType = (SpriteType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].Goal[i].spriteType, popupStyle, GUILayout.MaxWidth(280));
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(3);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Target", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        targetlevel.Levels[index].Goal[i].target = EditorGUILayout.IntField(targetlevel.Levels[index].Goal[i].target);
                        EditorGUILayout.EndHorizontal();
                    }
                    EndBox();
                }

            }

            if (DrawAddElementButton("Add Element"))
            {
                List<LevelGoal> goalList = new List<LevelGoal>(targetlevel.Levels[index].Goal);
                goalList.Add(new LevelGoal());
                targetlevel.Levels[index].Goal = goalList.ToArray();
            }
        }
        EndBox();
    }
    private void DrawJewelMachine(int index)
    {
        BeginBox();
        bool isJewelMachineExpanded = BeginSimpleFoldoutBox("Jewel Machine");
        if (isJewelMachineExpanded)
        {
            DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].JewelMachine.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].JewelMachine.enabled, EditorStyles.toggle);
            EditorGUILayout.EndHorizontal();
            if (targetlevel.Levels[index].JewelMachine.enabled)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Counter", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].JewelMachine.counter = EditorGUILayout.IntField(targetlevel.Levels[index].JewelMachine.counter);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Gems to Spawn", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].JewelMachine.GemsToSpwan = EditorGUILayout.IntField(targetlevel.Levels[index].JewelMachine.GemsToSpwan);
                EditorGUILayout.EndHorizontal();
            }
        }
        EndBox();
    }
    private void DrawBlockerStick(int index)
    {
        BeginBox();
        bool isBlockerStickExpanded = BeginSimpleFoldoutBox("Blocker Sticker");
        if (isBlockerStickExpanded)
        {
            DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enabled", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].BlockerStick.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].BlockerStick.enabled, EditorStyles.toggle);
            EditorGUILayout.EndHorizontal();
            if (targetlevel.Levels[index].BlockerStick.enabled)
            {
                if (targetlevel.Levels[index].BlockerStick.Stick.Length > 0)
                {
                    for (int i = 0; i < targetlevel.Levels[index].BlockerStick.Stick.Length; i++)
                    {
                        BeginBox();

                        EditorGUILayout.BeginHorizontal();
                        bool isBlockerStickItemExpanded = BeginSimpleFoldoutBox("Blocker Stick " + (i + 1));
                        EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                        if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
                        {
                            List<Stick> blockerStickList = new List<Stick>(targetlevel.Levels[index].BlockerStick.Stick);
                            blockerStickList.Insert(i, new Stick());
                            targetlevel.Levels[index].BlockerStick.Stick = blockerStickList.ToArray();
                        }

                        if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
                        {
                            List<Stick> blockerStickList = new List<Stick>(targetlevel.Levels[index].BlockerStick.Stick);
                            blockerStickList.RemoveAt(i);
                            targetlevel.Levels[index].BlockerStick.Stick = blockerStickList.ToArray();
                            return;
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(3);
                        if (isBlockerStickItemExpanded)
                        {
                            EditorGUI.indentLevel = 3;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("On Vertical", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                            targetlevel.Levels[index].BlockerStick.Stick[i].onVertical = EditorGUILayout.Toggle(targetlevel.Levels[index].BlockerStick.Stick[i].onVertical, EditorStyles.toggle);
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(3);

                            EditorGUI.indentLevel = 3;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("On Horizontal", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                            targetlevel.Levels[index].BlockerStick.Stick[i].onHorrizontal = EditorGUILayout.Toggle(targetlevel.Levels[index].BlockerStick.Stick[i].onHorrizontal, EditorStyles.toggle);
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(3);

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Length", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                            targetlevel.Levels[index].BlockerStick.Stick[i].length = EditorGUILayout.IntField(targetlevel.Levels[index].BlockerStick.Stick[i].length);
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(3);

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Starting Position", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
                            //EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                            var x = EditorGUILayout.FloatField(targetlevel.Levels[index].BlockerStick.Stick[i].startingPosition.x);
                            var y = EditorGUILayout.FloatField(targetlevel.Levels[index].BlockerStick.Stick[i].startingPosition.y);
                            targetlevel.Levels[index].BlockerStick.Stick[i].startingPosition = new Vector2(x, y);
                            EditorGUILayout.EndHorizontal();
                        }
                        EndBox();
                    }
                }
                if (DrawAddElementButton("Add Element"))
                {
                    List<Stick> blockerStickList = new List<Stick>(targetlevel.Levels[index].BlockerStick.Stick);
                    blockerStickList.Add(new Stick());
                    targetlevel.Levels[index].BlockerStick.Stick = blockerStickList.ToArray();

                }
            }

        }
        EndBox();
    }

    private void DrawBombs(int index)
    {
        BeginBox();
        bool isBombExpanded = BeginSimpleFoldoutBox("Bomb");
        if (isBombExpanded)
        {
            DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Allow Bomb", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].allowBombs = EditorGUILayout.Toggle(targetlevel.Levels[index].allowBombs, EditorStyles.toggle);
            EditorGUILayout.EndHorizontal();

            if (targetlevel.Levels[index].allowBombs)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Bomb Type", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].BombType = (BombType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].BombType, popupStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Balloon Bomb and Ice Machine Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Remaining Counter", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].remainingcounter = EditorGUILayout.IntField(targetlevel.Levels[index].remainingcounter);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Balloon bomb place after how many moves", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add Bombs after Moves", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].addBombAfterMoves = EditorGUILayout.IntField(targetlevel.Levels[index].addBombAfterMoves);
                EditorGUILayout.EndHorizontal();
            }
        }
        EndBox();
    }
    
    private void DrawGuide(int index)
    {
        BeginBox();
        bool isGuideExpanded = BeginSimpleFoldoutBox("Guide Screen");
        if (isGuideExpanded)
        {
            DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enable Guide Screen", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
            EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
            targetlevel.Levels[index].guide.enabled = EditorGUILayout.Toggle(targetlevel.Levels[index].guide.enabled, EditorStyles.toggle);
            EditorGUILayout.EndHorizontal();

            if (targetlevel.Levels[index].guide.enabled)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Guide Sprite", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].guide.guideSprite = (SpriteType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].guide.guideSprite, popupStyle);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Guide Message", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("", labelStyle, GUILayout.MinWidth(0));
                targetlevel.Levels[index].guide.guideMessage = EditorGUILayout.TextField(targetlevel.Levels[index].guide.guideMessage, inputStyle, GUILayout.Width(120));
                EditorGUILayout.EndHorizontal();
            }
        }
        EndBox();
    }
    
    private void DrawInitialGrid(int index)
    {

        EditorGUI.indentLevel = 2;

        #region Format
        GUIStyle tableStyle = new GUIStyle();
        tableStyle.margin.left = 40;


        GUIStyle headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 30;

        GUIStyle columnStyle = new GUIStyle();
        columnStyle.fixedWidth = 65;

        GUIStyle rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;

        GUIStyle rowHeaderStyle = new GUIStyle();
        rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

        GUIStyle columnHeaderStyle = new GUIStyle();
        columnHeaderStyle.fixedWidth = 30;
        columnHeaderStyle.fixedHeight = 25f;

        GUIStyle columnLabelStyle = new GUIStyle();
        columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
        columnLabelStyle.alignment = TextAnchor.MiddleCenter;
        columnLabelStyle.fontStyle = FontStyle.Bold;

        GUIStyle cornerLabelStyle = new GUIStyle();
        cornerLabelStyle.fixedWidth = 30;
        cornerLabelStyle.alignment = TextAnchor.MiddleLeft;
        cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
        cornerLabelStyle.fontSize = 14;
        cornerLabelStyle.padding.top = -5;
        cornerLabelStyle.padding.left = -60;

        GUIStyle rowLabelStyle = new GUIStyle();
        rowLabelStyle.fixedWidth = 25;
        rowLabelStyle.alignment = TextAnchor.MiddleRight;
        rowLabelStyle.fontStyle = FontStyle.Bold;

        #endregion

        bool isGridExpanded = BeginSimpleFoldoutBox("Game Grid");
        if (isGridExpanded)
        {

            if (targetlevel.Levels[index].rows.Length > 0)
            {
                int gridSize = (int)targetlevel.Levels[index].Mode.GameModeSettings.boardSize;
                //EditorGUI.indentLevel = 4;
                BeginBox();
                EditorGUILayout.BeginHorizontal(tableStyle);
                for (int i = -1; i < gridSize; i++)
                {
                    if (targetlevel.Levels[index].rows[0].coloum.Length > 0)
                    {
                        EditorGUILayout.BeginVertical((i == -1) ? headerColumnStyle : columnStyle);

                        for (int j = -1; j < gridSize; j++)
                        {

                            if (i == -1 && j == -1)
                            {
                                EditorGUILayout.BeginVertical(rowHeaderStyle);
                                EditorGUILayout.LabelField("[" + gridSize + "X" + gridSize + "]", cornerLabelStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (i == -1)
                            {
                                
                                EditorGUILayout.BeginVertical(columnHeaderStyle);
                                EditorGUILayout.LabelField("["+j.ToString()+"] "+"Sprite Type", rowLabelStyle);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical(columnHeaderStyle);
                                EditorGUILayout.LabelField("Second Sprite", rowLabelStyle);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical(columnHeaderStyle);
                                EditorGUILayout.LabelField("Has Stages", rowLabelStyle);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical(columnHeaderStyle);
                                EditorGUILayout.LabelField("Stages", rowLabelStyle);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical(columnHeaderStyle);
                                EditorGUILayout.LabelField("Counter", rowLabelStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (j == -1)
                            {
                                EditorGUILayout.BeginVertical(rowHeaderStyle);
                                EditorGUILayout.LabelField("[" + i.ToString() + "] ", columnLabelStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                            if (i >= 0 && j >= 0)
                            {
                                EditorGUILayout.BeginVertical(rowStyle);
                                targetlevel.Levels[index].rows[j].coloum[i].spriteType = (SpriteType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].rows[j].coloum[i].spriteType, popupStyle, GUILayout.Width(80));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical(rowStyle);
                                targetlevel.Levels[index].rows[j].coloum[i].secondarySpriteType = (SpriteType)EditorGUILayout.EnumPopup(targetlevel.Levels[index].rows[j].coloum[i].secondarySpriteType, popupStyle, GUILayout.Width(80));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical(rowStyle);
                                targetlevel.Levels[index].rows[j].coloum[i].hasStages = EditorGUILayout.Toggle(targetlevel.Levels[index].rows[j].coloum[i].hasStages);
                                EditorGUILayout.EndHorizontal();
                                if (targetlevel.Levels[index].rows[j].coloum[i].hasStages)
                                {
                                    EditorGUILayout.BeginVertical(rowStyle);
                                    targetlevel.Levels[index].rows[j].coloum[i].stage = EditorGUILayout.IntField(targetlevel.Levels[index].rows[j].coloum[i].stage, GUILayout.Width(50));
                                    EditorGUILayout.EndHorizontal();
                                }
                                else
                                {
                                    EditorGUILayout.BeginVertical(columnHeaderStyle);
                                    EditorGUILayout.LabelField("", rowLabelStyle);
                                    EditorGUILayout.EndHorizontal();
                                }
                                var spriteType = targetlevel.Levels[index].rows[j].coloum[i].spriteType;
                                var secondarySpriteType = targetlevel.Levels[index].rows[j].coloum[i].secondarySpriteType;
                                
                                if (spriteType == SpriteType.BalloonBomb || spriteType == SpriteType.IceBomb || spriteType == SpriteType.IceMachine 
                                    || secondarySpriteType == SpriteType.BalloonBomb || secondarySpriteType == SpriteType.IceBomb)
                                {
                                    EditorGUILayout.BeginVertical(rowStyle);
                                    targetlevel.Levels[index].rows[j].coloum[i].remainingCounter = EditorGUILayout.IntField(targetlevel.Levels[index].rows[j].coloum[i].remainingCounter, GUILayout.Width(50));
                                    EditorGUILayout.EndHorizontal();
                                }
                                else
                                {
                                    EditorGUILayout.BeginVertical(columnHeaderStyle);
                                    EditorGUILayout.LabelField("", rowLabelStyle);
                                    EditorGUILayout.EndHorizontal();
                                }
                            }

                        }
                        EditorGUILayout.EndVertical();
                    }

                }
                EditorGUILayout.EndHorizontal();
                EndBox();
                
            }

            if (DrawAddElementButton("Refresh Grid"))
            {

                int gridSize = (int)targetlevel.Levels[index].Mode.GameModeSettings.boardSize;
                targetlevel.Levels[index].rows = new Row[gridSize];
                for (int i = 0; i < gridSize; i++)
                {
                    targetlevel.Levels[index].rows[i] = new Row();
                }
                for (int i = 0; i < gridSize; i++)
                {
                    Debug.Log(i);
                    targetlevel.Levels[index].rows[i].coloum = new BlockSprite[gridSize];
                }
            }

            if (DrawAddElementButton("Clear"))
            {
                List<Row> rowList = new List<Row>(targetlevel.Levels[index].rows);
                rowList.Clear(); ;
                targetlevel.Levels[index].rows = rowList.ToArray();
                if (targetlevel.Levels[index].rows.Length > 0)
                {
                    List<BlockSprite> columnList = new List<BlockSprite>(targetlevel.Levels[index].rows[0].coloum);
                    for (int i = 0; i < 8; i++)
                    {
                        columnList.Clear();
                    }
                    targetlevel.Levels[index].rows[targetlevel.Levels[index].rows.Length - 1].coloum = columnList.ToArray();
                }

            }
            GUI.backgroundColor = Color.grey;
            EditorGUILayout.HelpBox("Clear Before Changing Grid Size And then Refresh.", MessageType.Info, true);
            GUI.backgroundColor = Color.white;
        }
    }
}
