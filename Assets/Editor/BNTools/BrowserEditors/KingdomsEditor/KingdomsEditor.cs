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

public class KingdomsEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<Kingdom>> kngdList;
    public Dictionary<string, List<Kingdom>> kngdSearchBackup;
    public Dictionary<string, List<Kingdom>> kngdSearch;
    public Dictionary<string, bool> kngdListBool;
    public Dictionary<string, bool> kngdListBoolBackup;
    public List<Kingdom> modifyList;


    bool is_Input_now;
    string searchInput;

    // bool show_main_cultures = true;
    // bool show_other = true;

    //

    // Color banditCol;
    // GUIStyle banditStyle;

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
        EditorWindow.GetWindow(typeof(KingdomsEditor));
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


        kngdList = new Dictionary<string, List<Kingdom>>();
        kngdSearchBackup = new Dictionary<string, List<Kingdom>>();
        kngdSearch = new Dictionary<string, List<Kingdom>>();
        kngdListBool = new Dictionary<string, bool>();
        kngdListBoolBackup = new Dictionary<string, bool>();


        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            CheckAndResort();

        }

    }

    void OnGUI()
    {
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
            var npc = loadedMod.modFilesData.kingdomsData.kingdoms;
            GUILayout.Space(16);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Sort By:", EditorStyles.boldLabel);
            sortBy_index = EditorGUILayout.Popup(sortBy_index, sortBy_options, GUILayout.Width(128));

            GUILayout.Space(4);

            EditorGUILayout.LabelField("Filter:", EditorStyles.boldLabel);

            searchInput = EditorGUILayout.TextField(searchInput, EditorStyles.toolbarSearchField, GUILayout.Width(256));
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            // if (GUILayout.Button("All", GUILayout.Width(48)))
            // {
            //     show_main_cultures = true;

            //     CheckAndResort();

            // }
            // DrawUILineVertical(colUILine, 1, 1, 16);

            // if (GUILayout.Button("None", GUILayout.Width(48)))
            // {
            //     show_main_cultures = false;
            //     CheckAndResort();
            //     this.Repaint();
            // }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawUILine(colUILine, 1, 4);


            // EditorGUILayout.BeginHorizontal();

            // CreateEditorToggle(ref show_main_cultures, "Main Cultures");
            // DrawUILineVertical(colUILine, 1, 1, 16);
            // CreateEditorToggle(ref show_other, "Others");

            // GUILayout.FlexibleSpace();

            // EditorGUILayout.EndHorizontal();
            // DrawUILine(colUILine, 1, 4);

            // if (EditorGUI.EndChangeCheck())
            // {
            //     // Debug.Log("check");

            //     CheckAndResort();

            // }

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Kingdoms", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create Kingdom", GUILayout.Width(128)))
            {

                if (KNGD_EDIT_MANAGER == null)
                {
                    KingdomsEditorManager assetMng = (KingdomsEditorManager)ScriptableObject.CreateInstance(typeof(KingdomsEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 1;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "kingd_template";
                    assetMng.assetName_new = "kingd_template";
                    assetMng.assetID_new = "kingd_template_ID";
                }
                else
                {
                    KingdomsEditorManager assetMng = KNGD_EDIT_MANAGER;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 1;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "kingd_template";
                    assetMng.assetName_new = "kingd_template";
                    assetMng.assetID_new = "kingd_template";

                }

                // assetMng.CopyAssetAsOverride();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify Kingdoms", GUILayout.Width(128)))
            {

                var dic = new List<Kingdom>();

                for (int i = 0; i < kngdListBool.Count; i++)
                {
                    if (kngdListBool.ToArray()[i].Value == true)
                    {
                        foreach (var facGrp in kngdList)
                        {
                            foreach (var facInList in facGrp.Value)
                            {
                                if (kngdListBool.ToArray()[i].Key == facInList.id)
                                {
                                    dic.Add(facInList);
                                }
                            }
                        }
                    }

                }

                if (KNGD_EDIT_MANAGER == null)
                {
                    KingdomsEditorManager assetMng = (KingdomsEditorManager)ScriptableObject.CreateInstance(typeof(KingdomsEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 1;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "kingd_template";
                    assetMng.assetName_new = "kingd_template";
                    assetMng.assetID_new = "kingd_template_ID";

                    assetMng.modifyDic = new List<Kingdom>(dic);
                }
                else
                {
                    KingdomsEditorManager assetMng = KNGD_EDIT_MANAGER;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 1;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "kingd_template";
                    assetMng.assetName_new = "kingd_template";
                    assetMng.assetID_new = "kingd_template";

                    assetMng.modifyDic = new List<Kingdom>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[kngdListBool.Keys.Count];

                int i = 0;
                foreach (var ck in kngdListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < kngdListBool.Keys.Count; i3++)
                {
                    kngdListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[kngdListBool.Keys.Count];

                int i = 0;
                foreach (var ck in kngdListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < kngdListBool.Keys.Count; i3++)
                {
                    kngdListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);


            if (is_Input_now)
            {

                kngdSearchBackup = new Dictionary<string, List<Kingdom>>();

                foreach (var key in kngdList)
                {
                    var tempFacList = new List<Kingdom>();

                    foreach (var kingdInList in key.Value)
                    {

                        if (sortBy_index == 0)
                        {
                            string soloName = kingdInList.kingdomName;

                            RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(kingdInList);
                                }
                            }
                        }
                        else if (sortBy_index == 1)
                        {
                            if (searchInput != null)
                            {
                                if (kingdInList.id.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(kingdInList);
                                }
                            }
                        }

                    }
                    kngdSearchBackup.Add(key.Key, tempFacList);
                }
                kngdSearch = new Dictionary<string, List<Kingdom>>(kngdSearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(kngdSearch);
            }
            else
            {
                DrawList(kngdList);
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
        var kngdData = loadedMod.modFilesData.kingdomsData.kingdoms;

        if (kngdListBool != null && kngdListBool.Count != 0)
        {
            kngdListBoolBackup = kngdListBool;
        }

        // if (show_main_cultures || show_other)
        // {
        //     // EditorGUILayout.BeginHorizontal();
        //     SortByMethod(cultData);
        // }
        // else
        // {
        //     // SortByMethodNoneBool(npcData);
        //     kngdList = new Dictionary<string, List<Kingdom>>();
        // }

        SortByMethod(kngdData);

        if (searchInput != "" && is_Input_now == false)
        {
            is_Input_now = true;
        }

        // if (show_HeroLink && show_Hero)
        // {
        //     CheckHeroesData();

        //     SortByMethod(npcData);
        // }

        if (kngdList.Count != 0)
        {
            kngdListBool = new Dictionary<string, bool>();
            foreach (var list in kngdList)
            {
                foreach (var npc in list.Value)
                {
                    if (!kngdListBool.ContainsKey(npc.id))
                    {
                        kngdListBool.Add(npc.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (kngdListBoolBackup != null && kngdListBool != null && kngdListBoolBackup.Count != 0 && kngdListBool.Count != 0)
        {
            for (int i = 0; i < kngdListBool.Count; i++)
            {

                if (kngdListBoolBackup.ContainsKey(kngdListBool.ToArray()[i].Key))
                {
                    var b = false;
                    kngdListBoolBackup.TryGetValue(kngdListBool.ToArray()[i].Key, out b);
                    kngdListBool[kngdListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<Kingdom>> facListInput)
    {
        var originLabelWidth = EditorGUIUtility.labelWidth;
        var tagLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newCol;
        ColorUtility.TryParseHtmlString("#da5a47", out newCol);

        tagLabelStyle.normal.textColor = newCol;
        tagLabelStyle.fontSize = 24;
        tagLabelStyle.fontStyle = FontStyle.Bold;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));

        if (facListInput != null && facListInput.Count != 0)
        {

            foreach (var fac_list in facListInput)
            {
                if (fac_list.Value.Count != 0)
                {
                    var chrUP = fac_list.Key.ToCharArray();
                    var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    DrawUILine(colUILine, 1, 12);


                    if (sortBy_index == 0 || sortBy_index == 1)
                    {
                        EditorGUILayout.LabelField(chrUPstr, tagLabelStyle);
                    }
                    // else if (sortBy_index == 2)
                    // {
                    //     EditorGUILayout.LabelField(fac_list.Key, tagLabelStyle);
                    // }


                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_cult = 0;
                    foreach (var facChar in fac_list.Value)
                    {

                        var kingd = fac_list.Value[i_cult];

                        var soloName = "";
                        if (sortBy_index == 0 || sortBy_index == 2)
                        {
                            soloName = kingd.kingdomName;
                            RemoveTSString(ref soloName);

                            if (soloName == "")
                            {
                                soloName = kingd.id;
                            }
                        }
                        if (sortBy_index == 1)
                        {
                            soloName = kingd.id;
                        }

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        // if (kingd.is_main_culture == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, banditStyle);
                        // }
                        // else
                        // {
                            EditorGUILayout.LabelField(soloName, EditorStyles.label);
                        // }


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-128);

                        EditorGUI.BeginChangeCheck();
                        kngdListBool[kingd.id] = EditorGUILayout.Toggle(kngdListBool[kingd.id], GUILayout.ExpandWidth(false));

                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = kingd.id;
                                }
                                else
                                {
                                    currEventSelection[1] = kingd.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (kngdListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < kngdListBool.Count; i3++)
                                        {
                                            if (kngdListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (kngdListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < kngdListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                kngdListBool[kngdListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }




                            }

                        }


                        EditorGUIUtility.labelWidth = originLabelWidth;

                        // EditorGUILayout.Space(-64);

                        EditorGUILayout.ObjectField(kingd, typeof(Faction), true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(4);
                        DrawUILine(colUILine, 1, 4);

                        EditorGUILayout.BeginVertical();
                        // GUILayout.Space(16);
                        DrawUILine(colUILine, 4, 12);
                        EditorGUILayout.EndVertical();


                        i_cult++;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        // Repaint();


    }

    public void SortByMethod(List<Kingdom> kngdListInput)
    {
        this.kngdList = new Dictionary<string, List<Kingdom>>();


        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Kingdom>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var kingd in kngdListInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = kingd.kingdomName;

                        if (soloName == "")
                        {
                            soloName = kingd.id;
                        }

                    }
                    if (sortBy_index == 1)
                    {
                        soloName = kingd.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();
                    if (character != "other")
                    {
                        if (soloFirstChar == character || soloFirstChar == chrUPstr)
                        {
                            // if (kingd.is_main_culture == "true")
                            // {
                            //     CheckShowBool(newList, kingd, ref kingd.is_main_culture, ref show_main_cultures);

                            // }
                            // else
                            // {
                            //     if (show_other)
                            //     {
                                    newList.Add(kingd);
                            //     }
                            // }

                        }
                    }
                }
                kngdList.Add(character, newList);
            }

        }

    }


    public void SortByMethodNoneBool(List<Kingdom> kngdInput)
    {
        kngdList = new Dictionary<string, List<Kingdom>>();

        // Debug.Log(sortBy_index);
        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Kingdom>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var kngd in kngdInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = kngd.kingdomName;

                        if (soloName == "")
                        {
                            soloName = kngd.id;
                        }
                    }
                    if (sortBy_index == 1)
                    {
                        soloName = kngd.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();

                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {
                        newList.Add(kngd);
                    }
                }
                kngdList.Add(character, newList);
            }


            foreach (var kngd in kngdInput)
            {
                bool contains = false;
                foreach (var facL in kngdList)
                {
                    if (facL.Value.Contains(kngd))
                    {
                        contains = true;
                        break;

                    }
                }

                if (!contains)
                {
                    kngdList["other"].Add(kngd);
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

                    foreach (string dpdMod in currMod.modDependenciesInternal)
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

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
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

    private void CheckShowBool(List<Kingdom> newList, Kingdom kngd, ref string facParam, ref bool faBoolParam)
    {
        if (facParam == "true")
        {
            if (faBoolParam != false)
            {
                newList.Add(kngd);
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

    public static KingdomsEditorManager KNGD_EDIT_MANAGER
    {
        get { return EditorWindow.GetWindow<KingdomsEditorManager>(); }
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

