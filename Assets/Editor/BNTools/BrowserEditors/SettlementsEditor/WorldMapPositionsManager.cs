using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using UnityEngine.EventSystems;


class WorldMapPositionsManager : EditorWindow
{
    Color colUILine = new Color(0.13f, 0.09f, 0.00f, 0.63f);

    public Texture2D ico_castle;
    public Texture2D texture;
    Texture2D invertedTexture;
    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string dataPath = "Assets/Resources/Data/";
    public Vector2 scrollPosMap = Vector2.zero;
    public Vector2 scrollPosFacList = Vector2.zero;
    float facScrollSize = 0;
    public float resValue = 0.15f;
    public float W_res = 1024;
    public int W_cells_x = 16;
    public int W_cells_y = 16;
    public int W_cell_meter = 100;

    ///
    public float ico_sizes = 0.5f;
    public Color colorBoundCurve = Color.black;
    public bool drawCurves = false;

    // public int W_cell_res = 256;
    public Vector2 WorldDimension;

    bool isLoaded = false;
    List<Settlement> SList;
    Dictionary<Faction, Kingdom> FacDic;
    List<Kingdom> kingdList = new List<Kingdom>();
    Dictionary<Settlement, Faction> SFacBoundList;
    Dictionary<Faction, WorldMapViewer> facViewer;
    Dictionary<Kingdom, WorldMapViewer> kngdViewer;
    Color[] SColorA;
    Color[] SColorB;


    bool _Factions = true;
    bool _Kingdoms = false;
    // bool showTrade = true;
    bool[] _FactionsHide;
    bool[] _KingdomsHide;
    //string[] current_fac_ids;
    bool show_town = true;
    Vector2 show_town_size = new Vector2(0.5f, 1.5f);
    Color town_button_color;
    bool show_castle = true;
    Vector2 show_castle_size;
    Color castle_button_color;
    bool show_hideout = true;
    bool show_other = true;

    Vector2 show_hideout_size;
    Color hideout_button_color;

    bool show_village = true;
    Vector2 show_village_size;
    Color village_button_color;


    bool buttonPressed;
    string draggingID;

    BDTSettings settingsAsset;
    ModuleReceiver currMod;

    bool loadSettings = false;

    [MenuItem("Examples/World Editor")]
    static void Init()
    {
        var window = GetWindow<WorldMapPositionsManager>("World Editor");
        window.position = new Rect(0, 0, 1024, 1024);
        window.Show();

    }

    // static void AddCursorRectExample()
    // {
    //     MouseCursorExample window =
    //         EditorWindow.GetWindowWithRect<MouseCursorExample>(new Rect(0, 0, 180, 80));
    //     window.Show();
    // }

    void OnGUI()
    {

        if (loadSettings == false)
        {
            if (settingsAsset == null)
            {
                if (System.IO.File.Exists(configPath))
                {
                    settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));
                }
                else
                {
                    Debug.Log("BDT settings dont exist");
                }
            }

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));

            }
        }

        // W_res = EditorGUILayout.IntField("World Resolution", W_res);

        if (texture)
        {
            var scrollViewRect = new Rect(448, 64, 1440, 904);
            if (scrollViewRect.Contains(Event.current.mousePosition))
            {

                if (Event.current.button == 2 && Event.current.type == EventType.MouseDrag)
                {
                    // EditorGUIUtility.AddCursorRect(scrollViewRect, MouseCursor.Orbit);

                    scrollPosMap += -Event.current.delta;
                    Event.current.Use();
                }
            }
            scrollPosMap = GUI.BeginScrollView(scrollViewRect, scrollPosMap, new Rect(0, 0, W_res, W_res));

            EditorGUI.DrawPreviewTexture(new Rect(0, 0, W_res, W_res), texture);
            // EditorGUILayout.Toggle(false);

            if (!isLoaded)
            {
                GetMapSize();
            }
            else
            {
                DrawSettlements();
            }
        }

        // End the scroll view that we began above.
        GUI.EndScrollView();

        var menuRect = new Rect(64, 64, 320, 820);

        Color colMenu;
        ColorUtility.TryParseHtmlString("#404041", out colMenu);

        EditorGUI.DrawRect(menuRect, colMenu);
        GUI.BeginGroup(menuRect);


        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("World Map Scale", EditorStyles.boldLabel);
        resValue = EditorGUILayout.Slider(resValue, 0.15f, 2, GUILayout.Width(300));

        if (EditorGUI.EndChangeCheck())
        {
            W_res = 1024 * (resValue * 5);
            // scrollPosition = new Vector2(1440 / 2, 904 / 2);
        }

        EditorGUILayout.Space(4);

        var originLabelWidth = EditorGUIUtility.labelWidth;

        EditorGUILayout.LabelField("Icon Sizes", EditorStyles.boldLabel);
        ico_sizes = EditorGUILayout.Slider(ico_sizes, 0, 1, GUILayout.Width(300));

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Bounds Curves "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        EditorGUILayout.LabelField("Bounds Curves", EditorStyles.boldLabel);
        drawCurves = EditorGUILayout.Toggle("Visualize", drawCurves);
        colorBoundCurve = EditorGUILayout.ColorField("Curves Color", colorBoundCurve, GUILayout.Width(160));

        // EditorGUILayout.Space(32);
        DrawUILine(colUILine, 3, 12);


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Towns "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        EditorGUILayout.BeginHorizontal();
        show_town = EditorGUILayout.Toggle("Towns", show_town, GUILayout.ExpandWidth(false));
        DrawUILineVertical(colUILine, 1, 1, 16);

        show_castle = EditorGUILayout.Toggle("Castles", show_castle, GUILayout.ExpandWidth(false));
        DrawUILineVertical(colUILine, 1, 1, 16);

        show_village = EditorGUILayout.Toggle("Villages", show_village, GUILayout.ExpandWidth(false));
        DrawUILineVertical(colUILine, 1, 1, 16);

        show_hideout = EditorGUILayout.Toggle("Hideouts", show_hideout, GUILayout.ExpandWidth(false));
        //DrawUILineVertical(colUILine, 1, 6, 16);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(4);

        show_other = EditorGUILayout.Toggle("Other", show_other, GUILayout.ExpandWidth(false));

        EditorGUIUtility.labelWidth = originLabelWidth;

        DrawUILine(colUILine, 3, 12);

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Kingdoms "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(16);

        EditorGUI.BeginChangeCheck();

        DrawUILineVertical(colUILine, 1, 1, 16);
        _Kingdoms = EditorGUILayout.Toggle("Kingdoms", _Kingdoms, EditorStyles.radioButton, GUILayout.ExpandWidth(false));
        DrawUILineVertical(colUILine, 1, 1, 16);

        if (EditorGUI.EndChangeCheck())
        {
            if (_Kingdoms)
            {
                _Factions = false;
            }
            else
            {
                _Factions = true;
            }

            if (_Factions)
            {
                for (int i = 0; i < _FactionsHide.Length; i++)
                {

                    _FactionsHide[i] = false;

                }
                facScrollSize = FacDic.Count * 24 + 24;

            }
            else
            {
                for (int i = 0; i < _KingdomsHide.Length; i++)
                {

                    _KingdomsHide[i] = false;

                }

                facScrollSize = kingdList.Count * 24 + 24;

            }
        }

        EditorGUI.BeginChangeCheck();

        _Factions = EditorGUILayout.Toggle("Factions", _Factions, EditorStyles.radioButton, GUILayout.ExpandWidth(false));
        DrawUILineVertical(colUILine, 1, 1, 16);

        if (EditorGUI.EndChangeCheck())
        {
            if (_Factions)
            {
                _Kingdoms = false;
            }
            else
            {
                _Kingdoms = true;
            }

            if (_Factions)
            {
                for (int i = 0; i < _FactionsHide.Length; i++)
                {

                    _FactionsHide[i] = false;

                }
                facScrollSize = FacDic.Count * 24 + 24;

            }
            else
            {
                for (int i = 0; i < _KingdomsHide.Length; i++)
                {

                    _KingdomsHide[i] = false;

                }

                facScrollSize = kingdList.Count * 24 + 24;

            }

        }



        EditorGUIUtility.labelWidth = originLabelWidth;

        if (GUILayout.Button("All", GUILayout.Width(50)))
        {
            if (_Factions)
            {
                foreach (var facV in facViewer)
                {
                    facV.Value.show_towns = true;
                    facV.Value.show_castles = true;
                    facV.Value.show_villages = true;
                }
            }
            else
            {
                foreach (var kgdV in kngdViewer)
                {
                    kgdV.Value.show_towns = true;
                    kgdV.Value.show_castles = true;
                    kgdV.Value.show_villages = true;
                }
            }
        }

        if (GUILayout.Button("None", GUILayout.Width(50)))
        {
            if (_Factions)
            {
                foreach (var facV in facViewer)
                {
                    facV.Value.show_towns = false;
                    facV.Value.show_castles = false;
                    facV.Value.show_villages = false;
                }
            }
            else
            {
                foreach (var kgdV in kngdViewer)
                {
                    kgdV.Value.show_towns = false;
                    kgdV.Value.show_castles = false;
                    kgdV.Value.show_villages = false;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        var factionsRect = new Rect(16, 256, 288, 512);

        ColorUtility.TryParseHtmlString("#282828", out colMenu);

        EditorGUI.DrawRect(factionsRect, colMenu);

        scrollPosFacList = GUI.BeginScrollView(factionsRect, scrollPosFacList, new Rect(16, 128, 256, facScrollSize));

        if (_Factions)
        {
            int step = 0;
            int indexPos = 0;
            int foldoutSize = 128;
            foreach (var factionDic in FacDic)
            {

                EditorGUI.BeginChangeCheck();

                _FactionsHide[indexPos] = EditorGUI.Foldout(new Rect(16, 128 + step, 128, 32), _FactionsHide[indexPos], factionDic.Key.id);

                if (EditorGUI.EndChangeCheck())
                {
                    if (_FactionsHide[indexPos])
                    {

                        // var delSize = false;
                        for (int i = 0; i < _FactionsHide.Length; i++)
                        {
                            if (i != indexPos && _FactionsHide[i])
                            {
                                _FactionsHide[i] = false;
                                facScrollSize = facScrollSize - foldoutSize;
                            }
                        }
                        facScrollSize = facScrollSize + foldoutSize;
                    }
                    else
                    {
                        facScrollSize = facScrollSize - foldoutSize;
                    }

                }

                if (_FactionsHide[indexPos])
                {
                    var soloName = factionDic.Key.factionName;

                    RemoveTSString(ref soloName);


                    ColorUtility.TryParseHtmlString("#f38020", out colMenu);
                    var guiLabel = new GUIStyle(EditorStyles.boldLabel);
                    guiLabel.normal.textColor = colMenu;

                    EditorGUI.LabelField(new Rect(32, 124 + step + 24, 256, 32), soloName, guiLabel);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Towns   "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    facViewer[factionDic.Key].show_towns = EditorGUI.Toggle(new Rect(32, 122 + step + 56, 32, 32), "Towns", facViewer[factionDic.Key].show_towns);


                    facViewer[factionDic.Key].show_castles = EditorGUI.Toggle(new Rect(32, 122 + step + 80, 32, 32), "Castles", facViewer[factionDic.Key].show_castles);


                    facViewer[factionDic.Key].show_villages = EditorGUI.Toggle(new Rect(32, 122 + step + 104, 32, 32), "Villages", facViewer[factionDic.Key].show_villages);

                    EditorGUI.LabelField(new Rect(124, 122 + step + 56, 128, 32), "Show Bounds Curves");

                    facViewer[factionDic.Key].show_bounds = EditorGUI.Toggle(new Rect(256, 122 + step + 56, 32, 32), "", facViewer[factionDic.Key].show_bounds);

                    EditorGUIUtility.labelWidth = textDimensions.x;

                    EditorGUI.DrawRect(new Rect(0, 128 + step + 132, 256, 1), Color.grey);

                    step = step + foldoutSize;
                }
                else
                {
                    step = step + 24;
                }

                indexPos++;
            }
        }
        else
        {
            int step = 0;
            int indexPos = 0;
            int foldoutSize = 128;
            foreach (var kngd in kingdList)
            {

                EditorGUI.BeginChangeCheck();

                _KingdomsHide[indexPos] = EditorGUI.Foldout(new Rect(16, 124 + step, 128, 32), _KingdomsHide[indexPos], kngd.id);

                if (EditorGUI.EndChangeCheck())
                {
                    if (_KingdomsHide[indexPos])
                    {

                        // var delSize = false;
                        for (int i = 0; i < _KingdomsHide.Length; i++)
                        {
                            if (i != indexPos && _KingdomsHide[i])
                            {
                                _KingdomsHide[i] = false;
                                facScrollSize = facScrollSize - foldoutSize;
                            }
                        }
                        facScrollSize = facScrollSize + foldoutSize;
                    }
                    else
                    {
                        facScrollSize = facScrollSize - foldoutSize;
                    }

                }

                if (_KingdomsHide[indexPos])
                {

                    var soloName = kngd.kingdomName;

                    RemoveTSString(ref soloName);

                    ColorUtility.TryParseHtmlString("#f38020", out colMenu);
                    var guiLabel = new GUIStyle(EditorStyles.boldLabel);
                    guiLabel.normal.textColor = colMenu;

                    EditorGUI.LabelField(new Rect(32, 128 + step + 24, 256, 32), soloName, guiLabel);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Towns   "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    kngdViewer[kngd].show_towns = EditorGUI.Toggle(new Rect(32, 122 + step + 56, 32, 32), "Towns", kngdViewer[kngd].show_towns);


                    kngdViewer[kngd].show_castles = EditorGUI.Toggle(new Rect(32, 122 + step + 80, 32, 32), "Castles", kngdViewer[kngd].show_castles);


                    kngdViewer[kngd].show_villages = EditorGUI.Toggle(new Rect(32, 122 + step + 104, 32, 32), "Villages", kngdViewer[kngd].show_villages);

                    EditorGUIUtility.labelWidth = originLabelWidth;

                    EditorGUI.LabelField(new Rect(124, 122 + step + 56, 128, 32), "Show Bounds Curves");

                    kngdViewer[kngd].show_bounds = EditorGUI.Toggle(new Rect(256, 122 + step + 56, 32, 32), "", kngdViewer[kngd].show_bounds);

                    EditorGUI.DrawRect(new Rect(0, 123 + step + 132, 256, 1), Color.grey);

                    step = step + foldoutSize;

                }
                else
                {
                    step = step + 24;
                }

                indexPos++;
            }
        }
        GUI.EndScrollView();

        EditorGUILayout.Space(524);

        DrawUILine(colUILine, 3, 12);


        GUI.EndGroup();

    }

    public void DrawSettlements()
    {
        var originCol = GUI.color;

        if (drawCurves)
        {
            for (int i = 0; i < SList.Count; i++)
            {
                if (show_village && SList[i].isVillage)
                {
                    if (_Factions)
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                if (facViewer[fac.Value].show_villages)
                                {
                                    if (facViewer[fac.Value].show_bounds)
                                    {
                                        var X = float.Parse(SList[i].posX);
                                        var Y = -float.Parse(SList[i].posY);

                                        var multValX = W_res / WorldDimension.x;
                                        var multValY = W_res / WorldDimension.y;
                                        X = X * multValX;
                                        Y = Y * multValY;

                                        float i_size = 32 * ico_sizes;
                                        var buttonRect = new Rect(X - (i_size / 2) - 2, Y + W_res - (i_size / 2), i_size, i_size);

                                        foreach (var bound in SList)
                                        {
                                            if (bound.id == SList[i].CMP_bound.Replace("Settlement.", ""))
                                            {
                                                if (bound.isTown && show_town || bound.isCastle && show_castle)
                                                {
                                                    var X_target = float.Parse(bound.posX);
                                                    var Y_target = -float.Parse(bound.posY);

                                                    X_target = X_target * multValX;
                                                    Y_target = Y_target * multValY;

                                                    i_size = 32 * ico_sizes;
                                                    var buttonRect_target = new Rect(X_target - (i_size / 2), Y_target + W_res - (i_size / 2), i_size, i_size);

                                                    DrawNodeCurve(buttonRect, buttonRect_target);

                                                    // CURVES

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                foreach (var kngd in kingdList)
                                {
                                    if (fac.Value.super_faction.Replace("Kingdom.", "") == kngd.id)
                                    {
                                        if (kngdViewer[kngd].show_castles)
                                        {
                                            if (kngdViewer[kngd].show_bounds)
                                            {

                                                foreach (var bound in SList)
                                                {
                                                    if (bound.id == SList[i].CMP_bound.Replace("Settlement.", ""))
                                                    {
                                                        if (bound.isTown && show_town || bound.isCastle && show_castle)
                                                        {
                                                            var X = float.Parse(SList[i].posX);
                                                            var Y = -float.Parse(SList[i].posY);

                                                            var multValX = W_res / WorldDimension.x;
                                                            var multValY = W_res / WorldDimension.y;
                                                            X = X * multValX;
                                                            Y = Y * multValY;

                                                            float i_size = 32 * ico_sizes;
                                                            var buttonRect = new Rect(X - (i_size / 2) - 2, Y + W_res - (i_size / 2), i_size, i_size);

                                                            var X_target = float.Parse(bound.posX);
                                                            var Y_target = -float.Parse(bound.posY);

                                                            X_target = X_target * multValX;
                                                            Y_target = Y_target * multValY;

                                                            i_size = 32 * ico_sizes;
                                                            var buttonRect_target = new Rect(X_target - (i_size / 2), Y_target + W_res - (i_size / 2), i_size, i_size);

                                                            DrawNodeCurve(buttonRect, buttonRect_target);

                                                            // CURVES

                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < SList.Count; i++)
        {

            if (!SList[i].WM_Undefined)
            {
                if (show_town && SList[i].isTown)
                {
                    if (_Factions)
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                if (facViewer[fac.Value].show_towns)
                                {
                                    DrawSettlement(i);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                foreach (var kngd in kingdList)
                                {
                                    if (fac.Value.super_faction.Replace("Kingdom.", "") == kngd.id)
                                    {
                                        if (kngdViewer[kngd].show_towns)
                                        {
                                            DrawSettlement(i);
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    // DrawSettlement(i);
                }
                if (show_castle && SList[i].isCastle)
                {
                    if (_Factions)
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                if (facViewer[fac.Value].show_castles)
                                {
                                    DrawSettlement(i);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                foreach (var kngd in kingdList)
                                {
                                    if (fac.Value.super_faction.Replace("Kingdom.", "") == kngd.id)
                                    {
                                        if (kngdViewer[kngd].show_castles)
                                        {
                                            DrawSettlement(i);
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }

                }

                if (show_village && SList[i].isVillage)
                {
                    if (_Factions)
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                if (facViewer[fac.Value].show_villages)
                                {
                                    DrawSettlement(i);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var fac in SFacBoundList)
                        {
                            if (fac.Key.id == SList[i].id)
                            {
                                foreach (var kngd in kingdList)
                                {
                                    if (fac.Value.super_faction.Replace("Kingdom.", "") == kngd.id)
                                    {
                                        if (kngdViewer[kngd].show_villages)
                                        {
                                            DrawSettlement(i);
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }

                }


                if (show_hideout && SList[i].isHideout)
                {
                    DrawSettlement(i);
                }
            }
            else if (show_other)
            {
                DrawSettlement(i);
            }



            GUI.color = originCol;


        }
    }



    private void DrawSettlement(int i)
    {

        var guiBGTextureOrg = GUI.skin.button.normal.background;

        float X = 512;
        float Y = 512;
        if (SList[i].posX != "")
            X = float.Parse(SList[i].posX);

        if (SList[i].posY != "")
            Y = -float.Parse(SList[i].posY);

        var multValX = W_res / WorldDimension.x;
        var multValY = W_res / WorldDimension.y;
        X = X * multValX;
        Y = Y * multValY;

        float i_size = 32 * ico_sizes;
        var buttonRect = new Rect(X, Y + W_res, i_size, i_size);
        var moverButtonRect = new Rect(buttonRect.x - (i_size / 2), buttonRect.y - (i_size / 2), buttonRect.width + 4, buttonRect.height + 4);

        if (moverButtonRect.Contains(Event.current.mousePosition))
        {
            // Debug.Log(true);

            if (Event.current.type == EventType.MouseDown)
            {
                buttonPressed = true;
                draggingID = SList[i].id;
                Selection.activeObject = SList[i];
            }
            if (Event.current.type == EventType.MouseUp)
            {
                buttonPressed = false;
                draggingID = "";
            }

            //EditorGUIUtility.PingObject(SList[i]);
            
        }
        if (buttonPressed && Event.current.type == EventType.MouseDrag)
        {
            if (draggingID == SList[i].id)
            {
                buttonRect.x += Event.current.delta.x;
                buttonRect.y += Event.current.delta.y;

                var x_conv = buttonRect.x / multValX;
                var y_conv = buttonRect.y - W_res;
                y_conv = y_conv / multValY;

                SList[i].posX = x_conv.ToString();
                SList[i].posY = (-y_conv).ToString();

                //EditorGUILayout.LabelField($"({SList[i].posX},{SList[i].posY}", EditorStyles.miniBoldLabel);
                // Debug.Log($" X {x_conv.ToString()} Y {(-y_conv).ToString()}");
            }

        }

        var buttonRectStroke = new Rect(0, 0, 0, 0);
        if (!SList[i].WM_Undefined)
        {
            if (show_town && SList[i].isTown)
            {

                GUI.color = SColorA[i] * 4;
                buttonRectStroke = new Rect(buttonRect.x - (i_size / 2), buttonRect.y - (i_size / 2), buttonRect.width + 4, buttonRect.height + 4);
                GUI.Button(buttonRectStroke, new GUIContent(""));

                GUI.color = SColorB[i] * 4;

                GUI.skin.button.normal.background = ico_castle;
            }
            if (show_castle && SList[i].isCastle)
            {
                i_size = 22 * ico_sizes;
                buttonRect = new Rect(X, Y + W_res, i_size, i_size);

                GUI.color = SColorA[i] * 3;
                buttonRectStroke = new Rect(buttonRect.x - (i_size / 2), buttonRect.y - (i_size / 2), buttonRect.width + 4, buttonRect.height + 4);
                GUI.Button(buttonRectStroke, new GUIContent(""));


                GUI.color = SColorB[i] * 3;

            }
            if (show_village && SList[i].isVillage)
            {
                i_size = 14 * ico_sizes;
                buttonRect = new Rect(X, Y + W_res, i_size, i_size);

                GUI.color = SColorA[i] * 2;
                buttonRectStroke = new Rect(buttonRect.x - (i_size / 2), buttonRect.y - (i_size / 2), buttonRect.width + 4, buttonRect.height + 4);
                GUI.Button(buttonRectStroke, new GUIContent(""));

                GUI.color = SColorB[i] * 2;
            }
            if (show_hideout && SList[i].isHideout)
            {
                i_size = 16 * ico_sizes;
                buttonRect = new Rect(X, Y + W_res, i_size, i_size);
                GUI.color = Color.red * 3;
            }
        }
        else if(show_other)
        {
            i_size = 32 * ico_sizes;
            buttonRect = new Rect(X, Y + W_res, i_size, i_size);
            GUI.color = Color.gray * 3;
        }

        var soloName = SList[i].settlementName;
        RemoveTSString(ref soloName);
        GUI.Button(new Rect(buttonRect.x - (i_size / 2) - (buttonRect.x - buttonRect.x - 2), buttonRect.y - (i_size / 2) - (buttonRect.y - buttonRect.y - 2), buttonRect.width, buttonRect.height), new GUIContent("", soloName));

        GUI.skin.button.normal.background = guiBGTextureOrg;


    }

    private static void RemoveTSString(ref string inputString)
    {
        var TS_Name = inputString;
        Regex regex = new Regex("{=(.*)}");
        if (regex != null)
        {
            var v = regex.Match(TS_Name);
            string s = v.Groups[1].ToString();
            TS_Name = "{=" + s + "}";
        }

        if (TS_Name != "" && TS_Name != "{=}")
        {
            inputString = inputString.Replace(TS_Name, "");
        }
    }
    public void GetMapSize()
    {


        WorldDimension = new Vector2();
        WorldDimension.x = W_cells_x * W_cell_meter;
        WorldDimension.y = W_cells_y * W_cell_meter;

        //string[] assets = Directory.GetFiles(path, "*.asset");

        SList = new List<Settlement>();
        kingdList = new List<Kingdom>();
        SFacBoundList = new Dictionary<Settlement, Faction>();
        FacDic = new Dictionary<Faction, Kingdom>();
        foreach (var asst in currMod.modFilesData.settlementsData.settlements)
        {
            //var loadedAsst = (Settlement)AssetDatabase.LoadAssetAtPath(asst, typeof(Settlement));

            //Debug.Log(loadedAsst.id);

            SList.Add(asst);

        }

        SColorA = new Color[SList.Count];
        SColorB = new Color[SList.Count];

        int i = 0;

        foreach (var sett in SList)
        {
            for (int i_c = 0; i_c < currMod.modFilesData.factionsData.factions.Count; i_c++)
            {
                if (sett.owner != "" && sett.CMP_bound == "")
                {
                    if (currMod.modFilesData.factionsData.factions[i_c].id == sett.owner.Replace("Faction.", ""))
                    {
                        Color cl;
                        Color clB;
                        ColorUtility.TryParseHtmlString(currMod.modFilesData.factionsData.factions[i_c]._BannerColorA, out cl);
                        ColorUtility.TryParseHtmlString(currMod.modFilesData.factionsData.factions[i_c]._BannerColorB, out clB);

                        SColorA[i] = cl;
                        SColorB[i] = clB;

                        // if (sett.isCastle)
                        // {
                        //     Debug.Log("Castle");
                        // }
                        // if (sett.isTown)
                        // {
                        //     Debug.Log("Town");
                        // }

                        if (!SFacBoundList.ContainsKey(sett))
                        {
                            SFacBoundList.Add(sett, currMod.modFilesData.factionsData.factions[i_c]);
                        }
                        break;
                    }
                }
                else
                {
                    for (int i_ow = 0; i_ow < SList.Count; i_ow++)
                    {
                        if (sett.CMP_bound.Replace("Settlement.", "") == SList[i_ow].id)
                        {
                            for (int i_c2 = 0; i_c2 < currMod.modFilesData.factionsData.factions.Count; i_c2++)
                            {
                                if (currMod.modFilesData.factionsData.factions[i_c2].id == SList[i_ow].owner.Replace("Faction.", ""))
                                {
                                    Color cl;
                                    Color clB;
                                    ColorUtility.TryParseHtmlString(currMod.modFilesData.factionsData.factions[i_c2]._BannerColorA, out cl);
                                    ColorUtility.TryParseHtmlString(currMod.modFilesData.factionsData.factions[i_c2]._BannerColorB, out clB);

                                    SColorA[i] = cl;
                                    SColorB[i] = clB;

                                    // Debug.Log("Village");

                                    if (!SFacBoundList.ContainsKey(sett))
                                    {
                                        SFacBoundList.Add(sett, currMod.modFilesData.factionsData.factions[i_c2]);
                                    }

                                    break;
                                }
                            }
                        }
                    }


                }
            }
            i++;
        }


        foreach (var fac in SFacBoundList)
        {
            if (!FacDic.ContainsKey(fac.Value))
            {
                bool added = false;
                foreach (var kngd in currMod.modFilesData.kingdomsData.kingdoms)
                {
                    if (fac.Value.super_faction == "Kingdom." + kngd.id)
                    {
                        FacDic.Add(fac.Value, kngd);

                        //Debug.Log(fac.Value.id);

                        if (!kingdList.Contains(kngd))
                        {
                            kingdList.Add(kngd);
                        }
                        added = true;
                    }

                }

                if (!added)
                {
                    //Debug.Log(fac.Key.id);

                    FacDic.Add(fac.Value, new Kingdom());
                }
            }
        }

        _FactionsHide = new bool[FacDic.Count];
        _KingdomsHide = new bool[kingdList.Count];

        facViewer = new Dictionary<Faction, WorldMapViewer>();
        foreach (var facInList in FacDic)
        {
            var viewer = (WorldMapViewer)ScriptableObject.CreateInstance(typeof(WorldMapViewer));
            facViewer.Add(facInList.Key, viewer);
        }

        kngdViewer = new Dictionary<Kingdom, WorldMapViewer>();
        foreach (var kngdInList in kingdList)
        {
            var viewer = (WorldMapViewer)ScriptableObject.CreateInstance(typeof(WorldMapViewer));
            kngdViewer.Add(kngdInList, viewer);
        }

        if (_Factions)
        {
            facScrollSize = FacDic.Count * 24 + 24;
        }
        else
        {
            facScrollSize = kingdList.Count * 24 + 24;
        }

        var current_fac_ids = new string[currMod.modFilesData.factionsData.factions.Count];

        var index = 0;
        foreach (var faction in currMod.modFilesData.factionsData.factions)
        {
            current_fac_ids[index] = faction.id;
            index++;
        }

        var current_settl_ids = new string[SList.Count];

        index = 0;
        foreach (var settlement in SList)
        {
            current_settl_ids[index] = settlement.id;
            index++;
        }

        foreach (var settlement in SList)
        {
            EditorUtility.SetDirty(settlement);
            settlement.WM_Undefined = false;

            if (settlement.owner == "" || !current_fac_ids.Contains(settlement.owner.Replace("Faction.", "")))
            {
                if(settlement.isVillage)
                {
                    var bound = settlement.CMP_bound.Replace("Settlement.", "");
                    //Debug.Log(bound);
                    if (bound == "" || !current_settl_ids.Contains(bound))
                    {
                        settlement.WM_Undefined = true;
                    }
                }
                else if (!settlement.isHideout)
                {
                    settlement.WM_Undefined = true;
                }
                //Debug.Log(settlement.owner.Replace("Faction.", ""));
            }
        }

        ColorUtility.TryParseHtmlString("#464646", out colUILine);

        isLoaded = true;
    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        DrawNodeCurve(start, end, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
    }

    void DrawNodeCurve(Rect start, Rect end, Vector2 vStartPercentage, Vector2 vEndPercentage)
    {
        Vector3 startPos = new Vector3(start.x + start.width * vStartPercentage.x, start.y + start.height * vStartPercentage.y, 0);
        Vector3 endPos = new Vector3(end.x + end.width * vEndPercentage.x, end.y + end.height * vEndPercentage.y, 0);
        Vector3 startTan = startPos + Vector3.right * (-50 + 100 * vStartPercentage.x) + Vector3.up * (-50 + 100 * vStartPercentage.y);
        Vector3 endTan = endPos + Vector3.right * (-50 + 100 * vEndPercentage.x) + Vector3.up * (-50 + 100 * vEndPercentage.y);
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, colorBoundCurve, null, 2);
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10, int lenght = 4)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
        r.height = thickness;
        r.x += padding / 2;
        r.y -= 2;
        r.height += 6 + lenght;
        EditorGUI.DrawRect(r, color);
    }


}