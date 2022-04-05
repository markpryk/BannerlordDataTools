using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using XNode;
using XNodeEditor;

public class SettlementsEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<Settlement>> settList;
    public Dictionary<string, List<Settlement>> settSearchBackup;
    public Dictionary<string, List<Settlement>> settSearch;
    public Dictionary<string, bool> settListBool;
    public Dictionary<string, bool> settListBoolBackup;
    public List<Settlement> modifyList;
    public Dictionary<string, Settlement[]> boundLinksDic;
    public Dictionary<string, Settlement[]> tradeBoundLinksDic;

    bool is_Input_now;
    string searchInput;

    bool show_town = true;
    bool show_castle = true;
    bool show_village = true;
    bool show_hideout = true;
    bool show_other = true;
    bool show_boundVillages = false;
    bool show_tradeBoundVillages = false;

    //
    // List<Settlement> bounds;
    // List<Settlement> tradeBounds;
    bool show_villages_loaded;

    //

    Color townCol;
    GUIStyle townStyle;
    Color castleCol;
    GUIStyle castleStyle;
    Color villageCol;
    GUIStyle villageStyle;
    Color hideoutCol;
    GUIStyle hideoutStyle;

    // 

    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string dataPath = "Assets/Resources/Data/";

    BDTSettings settingsAsset;

    Event currEvent;

    string[] currEventSelection = new string[2];

    void OnFocus()
    {
        if (loadedMod == null || loadedMod.modFilesData == null)
        {
            // Debug.Log(null);
            EditorWindow.GetWindow(typeof(BNDataEditorWindow));
        }
        else
        {
            CheckAndResort();
        }
    }


    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SettlementsEditor));
    }

    public void OnEnable()
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
            loadedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));

        }



        sortBy_options[0] = "Name";
        sortBy_options[1] = "ID";
        sortBy_options[2] = "Faction";


        settList = new Dictionary<string, List<Settlement>>();
        settSearchBackup = new Dictionary<string, List<Settlement>>();
        settSearch = new Dictionary<string, List<Settlement>>();
        settListBool = new Dictionary<string, bool>();
        settListBoolBackup = new Dictionary<string, bool>();

        boundLinksDic = new Dictionary<string, Settlement[]>();
        tradeBoundLinksDic = new Dictionary<string, Settlement[]>();

        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            CheckAndResort();

        }

    }

    void OnGUI()
    {

        townStyle = new GUIStyle(EditorStyles.boldLabel);
        castleStyle = new GUIStyle(EditorStyles.boldLabel);
        villageStyle = new GUIStyle(EditorStyles.boldLabel);
        hideoutStyle = new GUIStyle(EditorStyles.boldLabel);

        ColorUtility.TryParseHtmlString("#f65314", out townCol);
        ColorUtility.TryParseHtmlString("#00bce4", out castleCol);
        ColorUtility.TryParseHtmlString("#7ac143", out villageCol);
        ColorUtility.TryParseHtmlString("#c89a58", out hideoutCol);

        townStyle.normal.textColor = townCol;
        castleStyle.normal.textColor = castleCol;
        hideoutStyle.normal.textColor = hideoutCol;
        villageStyle.normal.textColor = villageCol;



        var headerLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newColLabel;
        ColorUtility.TryParseHtmlString("#fdd666", out newColLabel);

        headerLabelStyle.normal.textColor = newColLabel;
        headerLabelStyle.fontSize = 24;
        headerLabelStyle.fontStyle = FontStyle.Bold;

        currEvent = Event.current;

        if (!currEvent.shift)
        {
            currEventSelection[0] = "";
            currEventSelection[1] = "";

        }

        if (settingsAsset.currentModule != loadedMod.id)
        {
            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                loadedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
            }

            CheckAndResort();

        }

        if (loadedMod)
        {
            var settl = loadedMod.modFilesData.settlementsData.settlements;
            GUILayout.Space(16);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Sort By:", EditorStyles.boldLabel);
            sortBy_index = EditorGUILayout.Popup(sortBy_index, sortBy_options, GUILayout.Width(128));

            GUILayout.Space(4);

            EditorGUILayout.LabelField("Filter:", EditorStyles.boldLabel);

            searchInput = EditorGUILayout.TextField(searchInput, EditorStyles.toolbarSearchField, GUILayout.Width(256));
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {
                show_town = true;
                show_castle = true;
                show_hideout = true;
                show_village = true;
                show_other = true;

                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                show_town = false;
                show_castle = false;
                show_hideout = false;
                show_village = false;
                show_other = false;
                CheckAndResort();
                this.Repaint();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawUILine(colUILine, 1, 4);


            EditorGUILayout.BeginHorizontal();

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_town, "Towns");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_castle, "Castles");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_hideout, "Hideouts");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_village, "Villages");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_other, "Other");

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            DrawUILine(colUILine, 1, 4);

            if (sortBy_index == 2)
            {

                if (show_boundVillages)
                {
                    EditorGUILayout.BeginHorizontal();

                    CreateEditorToggle(ref show_boundVillages, "Show bounds");

                    DrawUILineVertical(colUILine, 1, 1, 16);
                    CreateEditorToggle(ref show_tradeBoundVillages, "Show trade bounds");
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    CreateEditorToggle(ref show_boundVillages, "Show bounds");

                }
            }
            else
            {
                show_boundVillages = false;
                show_villages_loaded = false;
                show_tradeBoundVillages = false;
            }

            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("check");

                CheckAndResort();

            }

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Settlements", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create Settlement", GUILayout.Width(128)))
            {

                if (SEM == null)
                {
                    SettlementsEditorManager assetMng = (SettlementsEditorManager)ScriptableObject.CreateInstance(typeof(SettlementsEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 5;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "settl_template";
                    assetMng.assetName_new = "settl_template";
                    assetMng.assetID_new = "settl_template_ID";
                }
                else
                {
                    SettlementsEditorManager assetMng = SEM;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 5;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "settl_template";
                    assetMng.assetName_new = "settl_template";
                    assetMng.assetID_new = "settl_template";

                }

                // assetMng.CopyAssetAsOverride();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify Settlement", GUILayout.Width(128)))
            {

                var dic = new List<Settlement>();

                for (int i = 0; i < settListBool.Count; i++)
                {
                    if (settListBool.ToArray()[i].Value == true)
                    {
                        foreach (var facGrp in settList)
                        {
                            foreach (var facInList in facGrp.Value)
                            {
                                if (settListBool.ToArray()[i].Key == facInList.id)
                                {
                                    dic.Add(facInList);
                                }
                            }
                        }
                    }

                }

                if (SEM == null)
                {
                    SettlementsEditorManager assetMng = (SettlementsEditorManager)ScriptableObject.CreateInstance(typeof(SettlementsEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 5;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "settl_template";
                    assetMng.assetName_new = "settl_template";
                    assetMng.assetID_new = "settl_template_ID";

                    assetMng.modifyDic = new List<Settlement>(dic);
                }
                else
                {
                    SettlementsEditorManager assetMng = SEM;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 5;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "settl_template";
                    assetMng.assetName_new = "settl_template";
                    assetMng.assetID_new = "settl_template";

                    assetMng.modifyDic = new List<Settlement>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[settListBool.Keys.Count];

                int i = 0;
                foreach (var ck in settListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < settListBool.Keys.Count; i3++)
                {
                    settListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[settListBool.Keys.Count];

                int i = 0;
                foreach (var ck in settListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < settListBool.Keys.Count; i3++)
                {
                    settListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);

            if (loadedMod.modFilesData.settlementsData.settlements.Count != 0)
            {
                if (GUILayout.Button("World Positions Editor", GUILayout.Width(160)))
                {
                    WorldMapPositionsManager WMPM = (WorldMapPositionsManager)EditorWindow.GetWindow(typeof(WorldMapPositionsManager), true, "Module Settings");

                    if (File.Exists(loadedMod.W_Map_Texture))
                    {
                        WMPM.texture = (Texture2D)AssetDatabase.LoadAssetAtPath(loadedMod.W_Map_Texture, typeof(Texture2D));
                    }
                    else
                    {
                        WMPM.texture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/worldMap_default.png", typeof(Texture2D));
                    }

                    WMPM.W_cells_x = loadedMod.W_X_Size;
                    WMPM.W_cells_y = loadedMod.W_Y_Size;
                    WMPM.W_cell_meter = loadedMod.W_SingleNodeSize;

                }
            }

            if (is_Input_now)
            {

                settSearchBackup = new Dictionary<string, List<Settlement>>();

                foreach (var key in settList)
                {
                    var tempSettList = new List<Settlement>();

                    foreach (var settlInList in key.Value)
                    {

                        if (sortBy_index == 0)
                        {
                            string soloName = settlInList.settlementName;

                            RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempSettList.Add(settlInList);
                                }
                            }
                        }
                        else if (sortBy_index == 1)
                        {
                            if (searchInput != null)
                            {
                                if (settlInList.id.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempSettList.Add(settlInList);
                                }
                            }
                        }
                        else if (sortBy_index == 2)
                        {
                            string soloName = settlInList.owner.Replace("Faction.", "");

                            // RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempSettList.Add(settlInList);
                                }
                            }
                        }
                    }
                    settSearchBackup.Add(key.Key, tempSettList);
                }
                settSearch = new Dictionary<string, List<Settlement>>(settSearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(settSearch);
            }
            else
            {
                DrawList(settList);
            }




        }
        else
        {
            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                loadedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
            }
        }

    }

    // ! CHECK
    public void CheckAndResort()
    {
        var settlData = loadedMod.modFilesData.settlementsData.settlements;

        if (settListBool != null && settListBool.Count != 0)
        {
            settListBoolBackup = settListBool;
        }

        if (show_town || show_castle || show_hideout || show_village || show_other)
        {
            // EditorGUILayout.BeginHorizontal();
            SortByMethod(settlData);
        }
        else
        {
            // SortByMethodNoneBool(npcData);
            settList = new Dictionary<string, List<Settlement>>();
        }

        if (searchInput != "" && is_Input_now == false)
        {
            is_Input_now = true;
        }

        if (show_boundVillages)
        {
            CheckSettlementsData();

            SortByMethod(settlData);
        }

        if (settList.Count != 0)
        {
            settListBool = new Dictionary<string, bool>();
            foreach (var list in settList)
            {
                foreach (var npc in list.Value)
                {
                    if (!settListBool.ContainsKey(npc.id))
                    {
                        settListBool.Add(npc.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (settListBoolBackup != null && settListBool != null && settListBoolBackup.Count != 0 && settListBool.Count != 0)
        {
            for (int i = 0; i < settListBool.Count; i++)
            {

                if (settListBoolBackup.ContainsKey(settListBool.ToArray()[i].Key))
                {
                    var b = false;
                    settListBoolBackup.TryGetValue(settListBool.ToArray()[i].Key, out b);
                    settListBool[settListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<Settlement>> settlListInput)
    {
        var originLabelWidth = EditorGUIUtility.labelWidth;
        var tagLabelStyle = new GUIStyle(EditorStyles.helpBox);



        Color newCol;
        ColorUtility.TryParseHtmlString("#da5a47", out newCol);

        tagLabelStyle.normal.textColor = newCol;
        tagLabelStyle.fontSize = 24;
        tagLabelStyle.fontStyle = FontStyle.Bold;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));

        if (settlListInput != null && settlListInput.Count != 0)
        {

            foreach (var settl_list in settlListInput)
            {
                if (settl_list.Value.Count != 0)
                {
                    var chrUP = settl_list.Key.ToCharArray();
                    var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    DrawUILine(colUILine, 1, 12);


                    if (sortBy_index == 0 || sortBy_index == 1)
                    {
                        EditorGUILayout.LabelField(chrUPstr, tagLabelStyle);
                    }
                    else if (sortBy_index == 2)
                    {

                        for (int i_f = 0; i_f < loadedMod.modFilesData.factionsData.factions.Count; i_f++)
                        {
                            // Debug.Log(currMod.modFilesData.factionsData.factions[i_f].name);

                            if (settl_list.Key == "None")
                            {
                                EditorGUILayout.LabelField("None", tagLabelStyle);
                                break;
                            }
                            else if (loadedMod.modFilesData.factionsData.factions[i_f].id == settl_list.Key)
                            {
                                var nm = loadedMod.modFilesData.factionsData.factions[i_f].factionName;

                                RemoveTSString(ref nm);
                                EditorGUILayout.LabelField(nm, tagLabelStyle);
                                break;
                            }
                        }

                        // EditorGUILayout.LabelField(settl_list.Key, tagLabelStyle);
                    }


                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_sett = 0;
                    foreach (var settlEntity in settl_list.Value)
                    {

                        var settlement = settl_list.Value[i_sett];

                        var soloName = "";
                        if (sortBy_index == 0 || sortBy_index == 2)
                        {
                            soloName = settlement.settlementName;
                            RemoveTSString(ref soloName);

                            if (soloName == "")
                            {
                                soloName = settlement.id;
                            }
                        }
                        if (sortBy_index == 1)
                        {
                            soloName = settlement.id;
                        }

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        if (settlement.isTown)
                        {
                            EditorGUILayout.LabelField(soloName, townStyle);
                        }
                        else if (settlement.isCastle)
                        {
                            EditorGUILayout.LabelField(soloName, castleStyle);
                        }


                        else if (settlement.isVillage)
                        {
                            EditorGUILayout.LabelField(soloName, villageStyle);


                        }
                        else if (settlement.isHideout)
                        {
                            EditorGUILayout.LabelField(soloName, hideoutStyle);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(soloName, EditorStyles.label);
                        }


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-128);

                        EditorGUI.BeginChangeCheck();

                        if (settListBool.ContainsKey(settlement.id))
                        {
                            settListBool[settlement.id] = EditorGUILayout.Toggle(settListBool[settlement.id], GUILayout.ExpandWidth(false));

                        }

                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = settlement.id;
                                }
                                else
                                {
                                    currEventSelection[1] = settlement.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (settListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < settListBool.Count; i3++)
                                        {
                                            if (settListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (settListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < settListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                settListBool[settListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }
                            }
                        }




                        EditorGUIUtility.labelWidth = originLabelWidth;

                        // EditorGUILayout.Space(-64);

                        EditorGUILayout.ObjectField(settlement, typeof(Settlement), true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(4);

                        DrawUILine(colUILine, 1, 4);

                        if (show_boundVillages)
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(512));
                            EditorGUILayout.LabelField("", EditorStyles.label);
                            EditorGUILayout.LabelField("Bounds:", EditorStyles.miniBoldLabel);

                            EditorGUILayout.EndHorizontal();

                            foreach (var boundStack in boundLinksDic)
                            {
                                if (boundStack.Key == settlement.id)
                                {
                                    int i_S = 0;
                                    // Debug.Log(boundStack.Value.Length);

                                    foreach (var bound in boundStack.Value)
                                    {

                                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));
                                        EditorGUILayout.LabelField("", EditorStyles.label);
                                        EditorGUILayout.ObjectField(bound, typeof(Settlement), true);

                                        EditorGUILayout.EndHorizontal();

                                        i_S++;
                                    }

                                }

                            }

                            if (show_tradeBoundVillages)
                            {

                                EditorGUILayout.BeginHorizontal(GUILayout.Width(512));
                                EditorGUILayout.LabelField("", EditorStyles.label);
                                EditorGUILayout.LabelField("Trade Bounds:", EditorStyles.miniBoldLabel);

                                EditorGUILayout.EndHorizontal();

                                foreach (var boundStack in tradeBoundLinksDic)
                                {
                                    if (boundStack.Key == settlement.id)
                                    {
                                        int i_S = 0;
                                        // Debug.Log(boundStack.Value.Length);

                                        foreach (var bound in boundStack.Value)
                                        {

                                            EditorGUILayout.BeginHorizontal(GUILayout.Width(512));
                                            EditorGUILayout.LabelField("", EditorStyles.label);
                                            EditorGUILayout.ObjectField(bound, typeof(Settlement), true);

                                            EditorGUILayout.EndHorizontal();

                                            i_S++;
                                        }

                                    }

                                }
                            }


                        }

                        // if (!isSettlementChecked && !settlement.isVillage)
                        // {
                        //     EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        //     EditorGUILayout.LabelField("", EditorStyles.label);
                        //     EditorGUILayout.HelpBox("This NPC is Hero, but not contains Hero reference Data in this module and his dependencies.", MessageType.Warning);
                        //     EditorGUILayout.EndHorizontal();

                        //     EditorGUILayout.BeginHorizontal(GUILayout.Width(512));
                        //     EditorGUILayout.LabelField("", EditorStyles.label);

                        //     if (GUILayout.Button("Create Hero Reference"))
                        //     {


                        //         Hero heroAsset = ScriptableObject.CreateInstance<Hero>();

                        //         heroAsset.moduleID = settingsAsset.currentModule;
                        //         heroAsset.id = settlement.id;

                        //         var path = dataPath + settingsAsset.currentModule + "/Heroes/" + heroAsset.id + ".asset";

                        //         var contains = false;

                        //         Debug.Log(heroAsset.id);

                        //         for (int i2 = 0; i2 < loadedMod.modFilesData.heroesData.heroes.Count; i2++)
                        //         {
                        //             if (loadedMod.modFilesData.heroesData.heroes[i2] != null)
                        //             {
                        //                 if (loadedMod.modFilesData.heroesData.heroes[i2].id == heroAsset.id)
                        //                 {
                        //                     contains = true;
                        //                     break;
                        //                 }
                        //             }
                        //         }

                        //         if (!contains)
                        //         {
                        //             AssetDatabase.CreateAsset(heroAsset, path);
                        //             AssetDatabase.SaveAssets();

                        //             var heroLoad = (Hero)AssetDatabase.LoadAssetAtPath(path, typeof(Hero));
                        //             loadedMod.modFilesData.heroesData.heroes.Add(heroLoad);
                        //         }

                        //         AssetDatabase.Refresh();

                        //         CheckAndResort();
                        //     }
                        //     EditorGUILayout.EndHorizontal();



                        // }




                        EditorGUILayout.Space(4);
                        DrawUILine(colUILine, 1, 4);
                        DrawUILine(colUILine, 4, 8);

                        i_sett++;
                    }

                }

            }
        }
        EditorGUILayout.EndScrollView();

        // Repaint();


    }



    public void SortByMethod(List<Settlement> settInputList)
    {
        this.settList = new Dictionary<string, List<Settlement>>();


        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var settl in alpb)
            {
                var newList = new List<Settlement>();
                var chrUP = settl.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var settlement in settInputList)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = settlement.settlementName;

                        if (soloName == "")
                        {
                            soloName = settlement.id;
                        }
                    }
                    if (sortBy_index == 1)
                    {
                        soloName = settlement.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();
                    if (settl != "other")
                    {
                        if (soloFirstChar == settl || soloFirstChar == chrUPstr)
                        {
                            if (settlement.isCastle)
                            {
                                CheckShowBool(newList, settlement, settlement.isCastle, show_castle);

                            }
                            else if (settlement.isTown)
                            {
                                CheckShowBool(newList, settlement, settlement.isTown, show_town);

                            }
                            else if (settlement.isHideout)
                            {
                                CheckShowBool(newList, settlement, settlement.isHideout, show_hideout);

                            }
                            else if (settlement.isVillage)
                            {
                                CheckShowBool(newList, settlement, settlement.isVillage, show_village);

                            }
                            else
                            {
                                if (show_other)
                                {
                                    newList.Add(settlement);
                                }
                            }

                        }
                    }
                }
                settList.Add(settl, newList);
            }

        }


        if (sortBy_index == 2)
        {
            foreach (var settl in settInputList)
            {
                if (settl.owner != null && settl.owner != "" && settl.owner.Contains("Faction."))
                {
                    // var keyName = settl.owner.Replace("Faction.", "");
                    var keyName = settl.owner.Replace("Faction.", "");
                    var newList = new List<Settlement>();

                    if (!settList.Keys.Contains(keyName))
                    {
                        settList.Add(keyName, newList);
                    }
                }
                else
                {

                    var newList = new List<Settlement>();

                    if (!settList.Keys.Contains("None"))
                    {
                        settList.Add("None", newList);
                    }
                }

            }

            foreach (var dic in settList)
            {
                foreach (var S in settInputList)
                {
                    var ow = "None";

                    if (S.owner != null && S.owner != "")
                        ow = S.owner.Replace("Faction.", "");

                    if (ow == dic.Key)
                    {
                        if (S.isTown)
                        {
                            CheckShowBool(dic.Value, S, S.isTown, show_town);
                        }
                        else if (S.isCastle)
                        {
                            CheckShowBool(dic.Value, S, S.isCastle, show_castle);

                        }
                        else if (S.isHideout)
                        {
                            CheckShowBool(dic.Value, S, S.isHideout, show_hideout);

                        }
                        else if (S.isVillage)
                        {
                            CheckShowBool(dic.Value, S, S.isVillage, show_village);
                        }
                        else
                        {
                            if (show_other)
                            {
                                dic.Value.Add(S);
                            }
                        }
                    }
                }
            }

            // for (int i_bound = 0; i_bound < boundLinksDic.Count; i_bound++)
            // {
            //     var boundList = boundLinksDic.ToArray()[i_bound];

            //     foreach (var bound in boundList.Value)
            //     {
            //         var newList = new List<Settlement>();

            //         if (!settList.Keys.Contains(bound.CMP_bound.Replace("Settlement.", "")))
            //             settList.Add(bound.CMP_bound.Replace("Settlement.", ""), newList);
            //     }


            //     for (int i_bound2 = 0; i_bound2 < boundList.Value.Length; i_bound2++)
            //     {
            //         foreach (var dic in settInputList)
            //         {

            //             for (int i_S = 0; i_S < settInputList.Count; i_S++)
            //             {
            //                 if (settInputList[i_S].id == boundList.Value[i_bound2].id)
            //                 {
            //                     if (boundList.Value[i_bound2].CMP_bound.Replace("Settlement.", "") == dic.id)
            //                     {
            //                         CheckShowBool(settInputList, settInputList[i_S], settInputList[i_S].isVillage, show_boundVillages);
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // }
        }
    }

    private void CheckSettlementsData()
    {
        boundLinksDic = new Dictionary<string, Settlement[]>();
        // var heroData = loadedMod.modFilesData.heroesData.heroes;
        var settlData = loadedMod.modFilesData.settlementsData.settlements;

        for (int i_root = 0; i_root < settList.Count; i_root++)
        {
            var boundRoot = settList.ToArray()[i_root].Value;
            foreach (var settInKey in boundRoot)
            {
                Settlement[] Slist = new Settlement[0];
                foreach (var settlement in settlData)
                {
                    if (settlement != null)
                    {
                        if (settInKey.id == settlement.CMP_bound.Replace("Settlement.", ""))
                        {
                            Settlement _bound = null;
                            var refAsset = $"Settlement.{settlement.id}";
                            GetSettlemetnAsset(ref refAsset, ref _bound);

                            if (!Slist.Contains(_bound))
                            {
                                var templist = Slist;
                                Slist = new Settlement[Slist.Count() + 1];

                                for (int i_temp = 0; i_temp < templist.Count(); i_temp++)
                                {
                                    Slist[i_temp] = templist[i_temp];
                                }

                                Slist[Slist.Count() - 1] = _bound;
                            }

                        }
                    }
                }
                boundLinksDic.Add(settInKey.id, Slist);
            }

        }

        // Trade Bounds
        tradeBoundLinksDic = new Dictionary<string, Settlement[]>();

        for (int i_root = 0; i_root < settList.Count; i_root++)
        {
            var boundRoot = settList.ToArray()[i_root].Value;
            foreach (var settInKey in boundRoot)
            {
                Settlement[] Slist = new Settlement[0];
                foreach (var settlement in settlData)
                {
                    if (settlement != null)
                    {
                        if (settInKey.id == settlement.CMP_trade_bound.Replace("Settlement.", ""))
                        {
                            Settlement _bound = null;
                            var refAsset = $"Settlement.{settlement.id}";
                            GetSettlemetnAsset(ref refAsset, ref _bound);

                            if (!Slist.Contains(_bound))
                            {
                                var templist = Slist;
                                Slist = new Settlement[Slist.Count() + 1];

                                for (int i_temp = 0; i_temp < templist.Count(); i_temp++)
                                {
                                    Slist[i_temp] = templist[i_temp];
                                }

                                Slist[Slist.Count() - 1] = _bound;
                            }

                        }
                    }
                }
                tradeBoundLinksDic.Add(settInKey.id, Slist);
            }

        }



    }


    private void GetSettlemetnAsset(ref string settlementLink, ref Settlement settlementAsset)
    {
        // Face Key Template template
        // 
        if (settlementLink != null && settlementLink != "")
        {
            if (settlementLink.Contains("Settlement."))
            {
                // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");

                string dataName = settlementLink.Replace("Settlement.", "");

                string assetPath = dataPath + loadedMod.id + "/Settlements/" + dataName + ".asset";
                string assetPathShort = "/Settlements/" + dataName + ".asset";


                if (System.IO.File.Exists(assetPath))
                {
                    settlementAsset = (Settlement)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Settlement));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + loadedMod.id + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + assetPathShort;

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                settlementAsset = (Settlement)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Settlement));
                                break;
                            }
                            else
                            {
                                settlementAsset = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (settlementAsset == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == loadedMod.id)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                            settlementAsset = (Settlement)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Settlement));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (settlementAsset == null)
                        {

                            Debug.Log("Settlement " + dataName + " - Not EXIST in" + " ' " + loadedMod.id + " ' " + "resources, and they dependencies.");

                        }
                    }

                }
            }
        }
    }

    public void SortByMethodNoneBool(List<Settlement> settlInput)
    {
        settList = new Dictionary<string, List<Settlement>>();

        // Debug.Log(sortBy_index);
        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Settlement>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var fac in settlInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = fac.settlementName;

                        if (soloName == "")
                        {
                            soloName = fac.id;
                        }
                    }
                    if (sortBy_index == 1)
                    {
                        soloName = fac.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();

                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {
                        newList.Add(fac);
                    }
                }
                settList.Add(character, newList);
            }


            foreach (var npcChar in settlInput)
            {
                bool contains = false;
                foreach (var facL in settList)
                {
                    if (facL.Value.Contains(npcChar))
                    {
                        contains = true;
                        break;

                    }
                }

                if (!contains)
                {
                    settList["other"].Add(npcChar);
                }

            }
        }

        if (sortBy_index == 2)
        {
            foreach (var sett in settlInput)
            {
                if (sett.owner != "")
                {
                    var keyName = sett.owner.Replace("Faction.", "");
                    var newList = new List<Settlement>();

                    if (!settList.Keys.Contains(keyName))
                    {
                        settList.Add(keyName, newList);
                    }
                }
                else
                {
                    var newList = new List<Settlement>();

                    if (!settList.Keys.Contains("None"))
                    {
                        settList.Add("None", newList);
                    }
                }

            }


            foreach (var dic in settList)
            {
                foreach (var sett in settlInput)
                {
                    if (sett.owner.Replace("Faction.", "") == dic.Key)
                    {
                        dic.Value.Add(sett);
                    }
                }

            }
        }

    }

    public void GetHero(string hero, string modName, ref Hero heroOutput)
    {
        bool contains = false;

        if (hero != null && hero != "")
        {
            if (hero.Contains("Hero."))
            {
                string dataName = hero.Replace("Hero.", "");
                string asset = dataPath + modName + "/Heroes/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    heroOutput = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
                    contains = true;
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + modName + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                heroOutput = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                break;
                            }
                            else
                            {
                                contains = false;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (contains == false)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == modName)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            heroOutput = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (contains == false)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + modName + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }
    }

    private void CheckShowBool(List<Settlement> newList, Settlement settl, bool settlParam, bool boolParam)
    {
        if (settlParam)
        {
            if (boolParam == true)
            {
                newList.Add(settl);
            }

        }
    }
    private static void CreateEditorToggle(ref bool toggleBool, string label)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        toggleBool = EditorGUILayout.Toggle(label, toggleBool);

        EditorGUIUtility.labelWidth = originLabelWidth;

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

    public static SettlementsEditorManager SEM
    {
        get { return EditorWindow.GetWindow<SettlementsEditorManager>(); }
    }

    public bool IsValueBetween(double testValue, double bound1, double bound2)
    {
        return (testValue >= Math.Min(bound1, bound2) && testValue <= Math.Max(bound1, bound2));
    }


    // UI DRAW TOOLS
    public static void DrawUILineVerticalLarge(Color color, int spaceX)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(3));
        r.height = 44;
        r.x += spaceX;
        r.y -= 10;
        r.height += 32;
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

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}

