using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.EventSystems;

public class NPCEditor : EditorWindow
{

    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<NPCCharacter>> charList;
    public Dictionary<string, List<NPCCharacter>> charSearchBackup;
    public Dictionary<string, List<NPCCharacter>> charSearch;
    public Dictionary<string, bool> charListBool;
    public Dictionary<string, bool> charListBoolBackup;
    public List<Hero> heroLinksList;
    bool is_Input_now;
    string searchInput;

    bool show_BasicTroop = true;
    bool show_Mercenary = true;
    bool show_Companion = true;
    bool show_Hero = true;
    bool show_Template = true;
    bool show_chld_Template = true;
    bool show_other = true;
    bool show_male = true;
    bool show_female = true;

    //
    bool show_HeroLink = false;
    bool sort_ByHeroFaction = false;

    //

    Color basicTroopCol;
    GUIStyle basicTroopStyle;
    Color mercenaryCol;
    GUIStyle mercenaryStyle;
    Color companionCol;
    GUIStyle companionColStyle;
    Color heroCol;
    GUIStyle heroStyle;
    Color templateCol;
    GUIStyle templateStyle;
    Color chldTmplCol;
    GUIStyle chldTmplStyle;


    // 
    bool sortBy_HeroFaction;

    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string dataPath = "Assets/Resources/Data/";

    BDTSettings settingsAsset;

    Event currEvent;
    string[] currEventSelection = new string[2];


    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(NPCEditor));

    }
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
        // currEvent = Event.current;


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
            // BNDataEditorWindow editorWindow = (BNDataEditorWindow)ScriptableObject.CreateInstance(typeof(BNDataEditorWindow));
            // editorWindow.LoadModProjectData(ref loadedMod);
        }



        sortBy_options[0] = "Name";
        sortBy_options[1] = "ID";
        sortBy_options[2] = "Occupation";


        charList = new Dictionary<string, List<NPCCharacter>>();
        charSearchBackup = new Dictionary<string, List<NPCCharacter>>();
        charSearch = new Dictionary<string, List<NPCCharacter>>();
        heroLinksList = new List<Hero>();
        charListBool = new Dictionary<string, bool>();
        charListBoolBackup = new Dictionary<string, bool>();


        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            // var npc = loadedMod.modFilesData.npcChrData.NPCCharacters;
            CheckAndResort();
            // Debug.Log("enabale");

        }
        // else
        // {
        //     EditorWindow.GetWindow(typeof(BNDataEditorWindow));
        //     Debug.Log("disable");

        // }

    }

    void OnGUI()
    {
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



        basicTroopStyle = new GUIStyle(EditorStyles.boldLabel);
        mercenaryStyle = new GUIStyle(EditorStyles.boldLabel);
        companionColStyle = new GUIStyle(EditorStyles.boldLabel);
        heroStyle = new GUIStyle(EditorStyles.boldLabel);
        templateStyle = new GUIStyle(EditorStyles.boldLabel);
        chldTmplStyle = new GUIStyle(EditorStyles.boldLabel);

        ColorUtility.TryParseHtmlString("#629aa9", out basicTroopCol);
        ColorUtility.TryParseHtmlString("#aea400", out mercenaryCol);
        ColorUtility.TryParseHtmlString("#ffa500", out heroCol);
        ColorUtility.TryParseHtmlString("#ffc2e5", out companionCol);
        ColorUtility.TryParseHtmlString("#00ad45", out templateCol);
        ColorUtility.TryParseHtmlString("#5ecc62", out chldTmplCol);

        basicTroopStyle.normal.textColor = basicTroopCol;
        mercenaryStyle.normal.textColor = mercenaryCol;
        heroStyle.normal.textColor = heroCol;
        companionColStyle.normal.textColor = companionCol;
        templateStyle.normal.textColor = templateCol;
        chldTmplStyle.normal.textColor = chldTmplCol;



        var headerLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newColLabel;
        ColorUtility.TryParseHtmlString("#fdd666", out newColLabel);

        headerLabelStyle.normal.textColor = newColLabel;
        headerLabelStyle.fontSize = 24;
        headerLabelStyle.fontStyle = FontStyle.Bold;

        if (loadedMod)
        {
            var npc = loadedMod.modFilesData.npcChrData.NPCCharacters;
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
                show_BasicTroop = true;
                show_Mercenary = true;
                show_Companion = true;
                show_Hero = true;
                show_Template = true;
                show_chld_Template = true;
                show_other = true;
                show_male = true;
                show_female = true;
                show_HeroLink = true;
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                show_BasicTroop = false;
                show_Mercenary = false;
                show_Companion = false;
                show_Hero = false;
                show_Template = false;
                show_chld_Template = false;
                show_other = false;
                show_male = false;
                show_female = false;
                show_HeroLink = false;
                CheckAndResort();
                this.Repaint();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawUILine(colUILine, 1, 4);


            EditorGUILayout.BeginHorizontal();

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_BasicTroop, "Basic Troop");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_Mercenary, "Mercenary");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_Hero, "Hero");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_Companion, "Companion");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_Template, "Template");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_chld_Template, "Child Template");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_other, "Other");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_male, "Male");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_female, "Female");

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            DrawUILine(colUILine, 1, 4);

            EditorGUILayout.BeginHorizontal();

            if (!show_Hero)
            {
                show_HeroLink = false;
                sort_ByHeroFaction = false;
            }
            else
            {
                CreateEditorToggle(ref show_HeroLink, "Show Hero Links");
                if (!show_HeroLink)
                {
                    sort_ByHeroFaction = false;
                }
                else
                {
                    DrawUILineVertical(colUILine, 1, 1, 16);
                    CreateEditorToggle(ref sort_ByHeroFaction, "Sort by Faction (only heroes)");
                    GUILayout.FlexibleSpace();

                }
            }

            EditorGUILayout.EndHorizontal();



            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("check");

                CheckAndResort();

            }

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("NPC Characters & Heroes", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create NPC", GUILayout.Width(128)))
            {

                CheckHeroesData();

                if (NPCH_Manager == null)
                {
                    NPCEditorManager assetMng = (NPCEditorManager)ScriptableObject.CreateInstance(typeof(NPCEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 3;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "npc_template";
                    assetMng.assetName_new = "npc_template";
                    assetMng.assetID_new = "npc_template_ID";
                }
                else
                {
                    NPCEditorManager assetMng = NPCH_Manager;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 3;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "npc_template";
                    assetMng.assetName_new = "npc_template";
                    assetMng.assetID_new = "npc_template";

                }

                // assetMng.CopyAssetAsOverride();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify selected", GUILayout.Width(128)))
            {
                CheckHeroesData();
                var dic = new Dictionary<NPCCharacter, Hero>();

                for (int i = 0; i < charListBool.Count; i++)
                {
                    if (charListBool.ToArray()[i].Value == true)
                    {
                        foreach (var npcGrp in charList)
                        {
                            foreach (var npcInList in npcGrp.Value)
                            {
                                if (npcInList.id == charListBool.ToArray()[i].Key)
                                {
                                    var add = false;
                                    foreach (var heroInList in heroLinksList)
                                    {
                                        if (heroInList.id == charListBool.ToArray()[i].Key)
                                        {
                                            dic.Add(npcInList, heroInList);
                                            add = true;
                                        }
                                    }

                                    if (!add)
                                    {
                                        dic.Add(npcInList, null);
                                    }
                                }
                            }
                        }
                    }

                }

                if (NPCH_Manager == null)
                {
                    NPCEditorManager assetMng = (NPCEditorManager)ScriptableObject.CreateInstance(typeof(NPCEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 3;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "npc_template";
                    assetMng.assetName_new = "npc_template";
                    assetMng.assetID_new = "npc_template_ID";

                    assetMng.modifyDic = new Dictionary<NPCCharacter, Hero>(dic);
                }
                else
                {
                    NPCEditorManager assetMng = NPCH_Manager;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 3;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "npc_template";
                    assetMng.assetName_new = "npc_template";
                    assetMng.assetID_new = "npc_template";

                    assetMng.modifyDic = new Dictionary<NPCCharacter, Hero>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[charListBool.Keys.Count];

                int i = 0;
                foreach (var ck in charListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < charListBool.Keys.Count; i3++)
                {
                    charListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[charListBool.Keys.Count];

                int i = 0;
                foreach (var ck in charListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < charListBool.Keys.Count; i3++)
                {
                    charListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);


            if (is_Input_now)
            {

                charSearchBackup = new Dictionary<string, List<NPCCharacter>>();

                foreach (var key in charList)
                {
                    var tempCharList = new List<NPCCharacter>();

                    foreach (var npcInList in key.Value)
                    {

                        if (sortBy_index == 0 || sortBy_index == 2)
                        {
                            string soloName = npcInList.npcName;

                            RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempCharList.Add(npcInList);
                                }
                            }
                        }
                        else if (sortBy_index == 1)
                        {
                            if (searchInput != null)
                            {
                                if (npcInList.id.Contains(searchInput))
                                {
                                    tempCharList.Add(npcInList);
                                }
                            }
                        }
                    }
                    charSearchBackup.Add(key.Key, tempCharList);
                }
                charSearch = new Dictionary<string, List<NPCCharacter>>(charSearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(charSearch);
            }
            else
            {
                DrawList(charList);
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

    private void SearchHeroAsset(ref Hero hero, ref string heroID)
    {
        if (heroID != null && heroID != "")
        {
            string dataName = heroID;
            string asset = dataPath + settingsAsset.currentModule + "/Heroes/" + dataName + ".asset";

            if (System.IO.File.Exists(asset))
            {
                hero = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
            }
            else
            {
                // SEARCH IN DEPENDENCIES
                string modSett = modsSettingsPath + settingsAsset.currentModule + ".asset";
                ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                foreach (string dpdMod in currMod.modDependenciesInternal)
                {
                    string dpdPath = modsSettingsPath + dpdMod + ".asset";

                    if (System.IO.File.Exists(dpdPath))
                    {
                        string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                        if (System.IO.File.Exists(dpdAsset))
                        {
                            hero = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                            break;
                        }
                        else
                        {
                            hero = null;
                        }

                    }
                }

                //Check is dependency OF
                if (hero == null)
                {
                    string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                    foreach (string mod in mods)
                    {
                        ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                        foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                        {
                            if (depend == settingsAsset.currentModule)
                            {
                                foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                {
                                    if (data != null)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            hero = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (hero == null)
                    {
                        // Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + settingsAsset.currentModule + " ' " + "resources, and they dependencies.");
                    }
                }
            }

        }

        // GUILayout.BeginHorizontal();
        // EditorGUILayout.LabelField("Kingdom Owner:", EditorStyles.label);
        // object ownerField = EditorGUILayout.ObjectField(hero, typeof(Hero), true);
        // hero = (Hero)ownerField;

        // if (kingdomOwner != null)
        // {
        //     kingd.owner = "Hero." + kingdomOwner.id;
        // }
        // else
        // {
        //     kingd.owner = "";
        // }
    }

    // ! CHECK
    public void CheckAndResort()
    {
        var npcData = loadedMod.modFilesData.npcChrData.NPCCharacters;

        if (charListBool != null && charListBool.Count != 0)
        {
            charListBoolBackup = charListBool;
        }

        if (show_BasicTroop || show_chld_Template || show_Companion || show_Hero || show_Mercenary || show_Template || show_other || show_female || show_male)
        {
            // EditorGUILayout.BeginHorizontal();
            SortByMethod(npcData);
        }
        else
        {
            // SortByMethodNoneBool(npcData);
            charList = new Dictionary<string, List<NPCCharacter>>();
        }

        if (searchInput != "" && is_Input_now == false)
        {
            is_Input_now = true;
        }

        if (show_HeroLink && show_Hero)
        {
            CheckHeroesData();

            SortByMethod(npcData);
        }

        if (charList.Count != 0)
        {
            charListBool = new Dictionary<string, bool>();
            foreach (var list in charList)
            {
                foreach (var npc in list.Value)
                {
                    if (!charListBool.ContainsKey(npc.id))
                    {
                        charListBool.Add(npc.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (charListBoolBackup != null && charListBool != null && charListBoolBackup.Count != 0 && charListBool.Count != 0)
        {
            for (int i = 0; i < charListBool.Count; i++)
            {

                if (charListBoolBackup.ContainsKey(charListBool.ToArray()[i].Key))
                {
                    var b = false;
                    charListBoolBackup.TryGetValue(charListBool.ToArray()[i].Key, out b);
                    charListBool[charListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<NPCCharacter>> charListInput)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var tagLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newCol;
        ColorUtility.TryParseHtmlString("#da5a47", out newCol);

        tagLabelStyle.normal.textColor = newCol;
        tagLabelStyle.fontSize = 24;
        tagLabelStyle.fontStyle = FontStyle.Bold;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));

        if (charListInput != null && charListInput.Count != 0)
        {

            foreach (var npcDic in charListInput)
            {
                if (npcDic.Value.Count != 0)
                {
                    var chrUPstr = "Unknown";

                    if (npcDic.Key != "")
                    {
                        var chrUP = npcDic.Key.ToCharArray();
                        chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    }
                    else
                    {

                    }

                    DrawUILine(colUILine, 1, 12);

                    if (!sort_ByHeroFaction)
                    {

                        if (sortBy_index == 0 || sortBy_index == 1)
                        {
                            EditorGUILayout.LabelField(chrUPstr, tagLabelStyle);
                        }
                        else if (sortBy_index == 2)
                        {
                            EditorGUILayout.LabelField(npcDic.Key, tagLabelStyle);
                        }
                    }
                    else
                    {

                        EditorGUILayout.LabelField(npcDic.Key, tagLabelStyle);
                    }

                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_NPC = 0;
                    foreach (var npcChar in npcDic.Value)
                    {

                        var npcCharacter = npcDic.Value[i_NPC];

                        var soloName = "";
                        if (sortBy_index == 0 || sortBy_index == 2)
                        {
                            soloName = npcCharacter.npcName;
                            RemoveTSString(ref soloName);

                            if (soloName == "")
                            {
                                soloName = npcCharacter.id;
                            }

                        }
                        if (sortBy_index == 1)
                        {
                            soloName = npcCharacter.id;
                        }

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        if (npcCharacter.is_basic_troop == "true")
                        {
                            EditorGUILayout.LabelField(soloName, basicTroopStyle);


                        }
                        else if (npcCharacter.is_mercenary == "true")
                        {
                            EditorGUILayout.LabelField(soloName, mercenaryStyle);


                        }
                        else if (npcCharacter.is_hero == "true")
                        {
                            if (npcCharacter.is_companion == "true")
                            {
                                EditorGUILayout.LabelField(soloName, companionColStyle);

                            }
                            else
                            {
                                EditorGUILayout.LabelField(soloName, heroStyle);


                            }
                        }

                        else if (npcCharacter.is_template == "true")
                        {
                            EditorGUILayout.LabelField(soloName, templateStyle);


                        }
                        else if (npcCharacter.is_child_template == "true")
                        {
                            EditorGUILayout.LabelField(soloName, chldTmplStyle);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(soloName, EditorStyles.label);
                        }


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-128);

                        EditorGUI.BeginChangeCheck();
                        if (charListBool.ContainsKey(npcCharacter.id))
                        {
                            charListBool[npcCharacter.id] = EditorGUILayout.Toggle(charListBool[npcCharacter.id], GUILayout.ExpandWidth(false));

                        }

                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = npcCharacter.id;
                                }
                                else
                                {
                                    currEventSelection[1] = npcCharacter.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (charListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < charListBool.Count; i3++)
                                        {
                                            if (charListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (charListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < charListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                charListBool[charListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }




                            }

                        }


                        EditorGUIUtility.labelWidth = originLabelWidth;

                        // EditorGUILayout.Space(-64);

                        EditorGUILayout.ObjectField(npcCharacter, typeof(NPCCharacter), true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(4);

                        DrawUILine(colUILine, 1, 4);

                        if (show_HeroLink)
                        {

                            bool isHeroChecked = false;
                            for (int i = 0; i < heroLinksList.Count; i++)
                            {
                                if (heroLinksList[i].id == npcCharacter.id)
                                {
                                    EditorGUILayout.BeginHorizontal(GUILayout.Width(512));


                                    EditorGUILayout.LabelField("", EditorStyles.label);
                                    EditorGUILayout.ObjectField(heroLinksList[i], typeof(Hero), true);

                                    EditorGUILayout.EndHorizontal();
                                    isHeroChecked = true;

                                    break;
                                }
                            }


                            if (isHeroChecked == false && npcCharacter.is_hero == "true" && npcCharacter.is_companion != "true")
                            {
                                EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                                EditorGUILayout.LabelField("", EditorStyles.label);
                                EditorGUILayout.HelpBox("This NPC is Hero, but not contains Hero reference Data in this module and his dependencies.", MessageType.Warning);
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal(GUILayout.Width(512));
                                EditorGUILayout.LabelField("", EditorStyles.label);

                                if (GUILayout.Button("Create Hero Reference"))
                                {


                                    Hero heroAsset = ScriptableObject.CreateInstance<Hero>();

                                    heroAsset.moduleID = settingsAsset.currentModule;
                                    heroAsset.id = npcCharacter.id;

                                    var path = dataPath + settingsAsset.currentModule + "/Heroes/" + heroAsset.id + ".asset";

                                    var contains = false;

                                    //Debug.Log(heroAsset.id);

                                    for (int i2 = 0; i2 < loadedMod.modFilesData.heroesData.heroes.Count; i2++)
                                    {
                                        if (loadedMod.modFilesData.heroesData.heroes[i2] != null)
                                        {
                                            if (loadedMod.modFilesData.heroesData.heroes[i2].id == heroAsset.id)
                                            {
                                                contains = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!contains)
                                    {
										EditorUtility.SetDirty (heroAsset);
                                        AssetDatabase.CreateAsset(heroAsset, path);
                                        //AssetDatabase.SaveAssets();

                                        var heroLoad = (Hero)AssetDatabase.LoadAssetAtPath(path, typeof(Hero));
                                        loadedMod.modFilesData.heroesData.heroes.Add(heroLoad);
                                    }

                                    //AssetDatabase.Refresh();

                                    CheckAndResort();
                                }
                                EditorGUILayout.EndHorizontal();



                            }


                        }

                        EditorGUILayout.BeginVertical();
                        // GUILayout.Space(16);
                        DrawUILine(colUILine, 4, 12);
                        EditorGUILayout.EndVertical();


                        i_NPC++;
                    }

                }

            }
        }
        EditorGUILayout.EndScrollView();

        // Repaint();


    }

    public void SortByMethod(List<NPCCharacter> npc)
    {
        charList = new Dictionary<string, List<NPCCharacter>>();

        if (!sort_ByHeroFaction)
        {
            if (sortBy_index == 0 || sortBy_index == 1)
            {
                foreach (var character in alpb)
                {
                    var newList = new List<NPCCharacter>();
                    var chrUP = character.ToCharArray();
                    var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    foreach (var npcChar in npc)
                    {
                        var soloName = "";
                        if (sortBy_index == 0)
                        {
                            soloName = npcChar.npcName;

                            if (soloName == "")
                            {
                                soloName = npcChar.id;
                            }
                        }
                        if (sortBy_index == 1)
                        {
                            soloName = npcChar.id;
                        }

                        RemoveTSString(ref soloName);

                        var soloFirstChar = soloName.ToCharArray()[0].ToString();
                        if (character != "other")
                        {
                            if (soloFirstChar == character || soloFirstChar == chrUPstr)
                            {
                                if (npcChar.is_basic_troop == "true")
                                {
                                    CheckShowBool(newList, npcChar, ref npcChar.is_basic_troop, ref show_BasicTroop);

                                }
                                else if (npcChar.is_mercenary == "true")
                                {
                                    CheckShowBool(newList, npcChar, ref npcChar.is_mercenary, ref show_Mercenary);

                                }
                                else if (npcChar.is_hero == "true")
                                {
                                    if (npcChar.is_companion == "true")
                                    {
                                        CheckShowBool(newList, npcChar, ref npcChar.is_companion, ref show_Companion);
                                    }
                                    else
                                    {
                                        CheckShowBool(newList, npcChar, ref npcChar.is_hero, ref show_Hero);

                                    }
                                }

                                else if (npcChar.is_template == "true")
                                {
                                    CheckShowBool(newList, npcChar, ref npcChar.is_template, ref show_Template);

                                }
                                else if (npcChar.is_child_template == "true")
                                {
                                    CheckShowBool(newList, npcChar, ref npcChar.is_child_template, ref show_chld_Template);

                                }
                                else
                                {
                                    if (show_other)
                                    {
                                        // if (character == "other")
                                        if (npcChar.is_female == "true")
                                        {
                                            if (show_female != false)
                                            {
                                                newList.Add(npcChar);

                                            }
                                        }
                                        else
                                        {
                                            if (show_male != false)
                                            {
                                                newList.Add(npcChar);

                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    charList.Add(character, newList);
                }


                // foreach (var npcChar in npc)
                // {
                //     bool contains = false;
                //     foreach (var caharL in charList)
                //     {
                //         if (caharL.Value.Contains(npcChar))
                //         {
                //             contains = true;
                //             break;

                //         }
                //     }

                //     if (!contains)
                //     {
                //         charList["other"].Add(npcChar);
                //     }

                // }
            }


            if (sortBy_index == 2)
            {
                foreach (var occupation in settingsAsset.NPCOccupationDefinitions)
                {
                    var newList = new List<NPCCharacter>();
                    charList.Add(occupation, newList);
                }

                foreach (var dic in charList)
                {
                    foreach (var npcChar in npc)
                    {
                        if (npcChar.occupation == dic.Key)
                        {
                            if (npcChar.is_basic_troop == "true")
                            {
                                CheckShowBool(dic.Value, npcChar, ref npcChar.is_basic_troop, ref show_BasicTroop);

                            }
                            else if (npcChar.is_mercenary == "true")
                            {
                                CheckShowBool(dic.Value, npcChar, ref npcChar.is_mercenary, ref show_Mercenary);

                            }
                            else if (npcChar.is_hero == "true")
                            {
                                if (npcChar.is_companion == "true")
                                {
                                    CheckShowBool(dic.Value, npcChar, ref npcChar.is_companion, ref show_Companion);
                                }
                                else
                                {
                                    CheckShowBool(dic.Value, npcChar, ref npcChar.is_hero, ref show_Hero);

                                }
                            }

                            else if (npcChar.is_template == "true")
                            {
                                CheckShowBool(dic.Value, npcChar, ref npcChar.is_template, ref show_Template);

                            }
                            else if (npcChar.is_child_template == "true")
                            {
                                CheckShowBool(dic.Value, npcChar, ref npcChar.is_child_template, ref show_chld_Template);

                            }
                            else
                            {
                                if (show_other)
                                {
                                    // if (character == "other")
                                    if (npcChar.is_female == "true")
                                    {
                                        if (show_female != false)
                                        {
                                            dic.Value.Add(npcChar);

                                        }
                                    }
                                    else
                                    {
                                        if (show_male != false)
                                        {
                                            dic.Value.Add(npcChar);

                                        }
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

            foreach (var hero in heroLinksList)
            {
                if (hero.id != null)
                {
                    var newList = new List<NPCCharacter>();
                    if (!charList.Keys.Contains(hero.faction.Replace("Faction.", "")))
                    {
                        charList.Add(hero.faction.Replace("Faction.", ""), newList);
                    }
                }
            }


            foreach (var hero in heroLinksList)
            {
                foreach (var dic in charList)
                {
                    foreach (var npcChar in npc)
                    {
                        if (npcChar.id == hero.id)
                        {
                            if (hero.faction.Replace("Faction.", "") == dic.Key)
                            {
                                CheckShowBool(dic.Value, npcChar, ref npcChar.is_hero, ref show_Hero);
                            }
                        }
                    }
                }
            }

        }
    }

    private void CheckHeroesData()
    {
        heroLinksList = new List<Hero>();
        // var heroData = loadedMod.modFilesData.heroesData.heroes;
        var npcData = loadedMod.modFilesData.npcChrData.NPCCharacters;

        foreach (var npcCharacter in npcData)
        {
            var hero = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
            SearchHeroAsset(ref hero, ref npcCharacter.id);
            // GUILayout.BeginHorizontal();

            if (hero != null)
            {
                heroLinksList.Add(hero);
            }
        }
    }

    public void SortByMethodNoneBool(List<NPCCharacter> npc)
    {
        charList = new Dictionary<string, List<NPCCharacter>>();

        // Debug.Log(sortBy_index);
        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<NPCCharacter>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var npcChar in npc)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = npcChar.npcName;

                        if (soloName == "")
                        {
                            soloName = npcChar.id;
                        }
                    }
                    if (sortBy_index == 1)
                    {
                        soloName = npcChar.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();

                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {
                        newList.Add(npcChar);
                    }
                }
                charList.Add(character, newList);
            }


            foreach (var npcChar in npc)
            {
                bool contains = false;
                foreach (var caharL in charList)
                {
                    if (caharL.Value.Contains(npcChar))
                    {
                        contains = true;
                        break;

                    }
                }

                if (!contains)
                {
                    charList["other"].Add(npcChar);
                }

            }
        }

        if (sortBy_index == 2)
        {
            foreach (var occupation in settingsAsset.NPCOccupationDefinitions)
            {
                var newList = new List<NPCCharacter>();
                charList.Add(occupation, newList);
            }

            foreach (var dic in charList)
            {
                foreach (var npcChar in npc)
                {
                    if (npcChar.occupation == dic.Key)
                    {
                        dic.Value.Add(npcChar);
                    }
                }

            }
        }

    }

    private void CheckShowBool(List<NPCCharacter> newList, NPCCharacter npcChar, ref string npcParameter, ref bool boolParameter)
    {
        if (npcParameter == "true")
        {
            if (boolParameter != false)
            {

                if (npcChar.is_female == "true")
                {
                    if (show_female != false)
                    {
                        newList.Add(npcChar);

                    }
                }
                else
                {
                    if (show_male != false)
                    {
                        newList.Add(npcChar);

                    }
                }

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

    public static NPCEditorManager NPCH_Manager
    {
        get { return EditorWindow.GetWindow<NPCEditorManager>(); }
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
