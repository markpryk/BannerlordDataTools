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

public class FactionEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<Faction>> facList;
    public Dictionary<string, List<Faction>> facSearchBackup;
    public Dictionary<string, List<Faction>> facSearch;
    public Dictionary<string, bool> facListBool;
    public Dictionary<string, bool> facListBoolBackup;
    public List<Faction> modifyList;


    bool is_Input_now;
    string searchInput;

    bool show_bandit = true;
    bool show_mafia = true;
    bool show_outlaw = true;
    bool show_minor = true;
    bool show_other = true;

    //

    Color banditCol;
    GUIStyle banditStyle;
    Color mafiaCol;
    GUIStyle mafiaStyle;
    Color minorFactionCol;
    GUIStyle minorFactionStyle;
    Color outlawCol;
    GUIStyle outlawStyle;

    // 

    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string dataPath = "Assets/Resources/Data/";

    BDTSettings settingsAsset;

    Event currEvent;
    // ModuleReceiver currMod;

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
        EditorWindow.GetWindow(typeof(FactionEditor));
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
        sortBy_options[2] = "Kingdom";


        facList = new Dictionary<string, List<Faction>>();
        facSearchBackup = new Dictionary<string, List<Faction>>();
        facSearch = new Dictionary<string, List<Faction>>();
        facListBool = new Dictionary<string, bool>();
        facListBoolBackup = new Dictionary<string, bool>();


        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            CheckAndResort();

        }

    }

    void OnGUI()
    {

        banditStyle = new GUIStyle(EditorStyles.boldLabel);
        mafiaStyle = new GUIStyle(EditorStyles.boldLabel);
        minorFactionStyle = new GUIStyle(EditorStyles.boldLabel);
        outlawStyle = new GUIStyle(EditorStyles.boldLabel);

        ColorUtility.TryParseHtmlString("#629aa9", out banditCol);
        ColorUtility.TryParseHtmlString("#aea400", out mafiaCol);
        ColorUtility.TryParseHtmlString("#ffa500", out outlawCol);
        ColorUtility.TryParseHtmlString("#ffc2e5", out minorFactionCol);

        banditStyle.normal.textColor = banditCol;
        mafiaStyle.normal.textColor = mafiaCol;
        outlawStyle.normal.textColor = outlawCol;
        minorFactionStyle.normal.textColor = minorFactionCol;

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
                show_bandit = true;
                show_mafia = true;
                show_minor = true;
                show_outlaw = true;
                show_other = true;

                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                show_bandit = false;
                show_mafia = false;
                show_minor = false;
                show_outlaw = false;
                show_other = false;
                CheckAndResort();
                this.Repaint();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawUILine(colUILine, 1, 4);


            EditorGUILayout.BeginHorizontal();

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_bandit, "Bandit");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_mafia, "Mafia");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_minor, "Minor Faction");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_outlaw, "Outlaw");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_other, "Other");

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            DrawUILine(colUILine, 1, 4);

            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("check");

                CheckAndResort();

            }

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Factions", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create Faction", GUILayout.Width(128)))
            {

                if (FAC_EDITOR_Manager == null)
                {
                    FactionsEditorManager assetMng = (FactionsEditorManager)ScriptableObject.CreateInstance(typeof(FactionsEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "fac_template";
                    assetMng.assetName_new = "fac_template";
                    assetMng.assetID_new = "fac_template_ID";
                }
                else
                {
                    FactionsEditorManager assetMng = FAC_EDITOR_Manager;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "fac_template";
                    assetMng.assetName_new = "fac_template";
                    assetMng.assetID_new = "fac_template";

                }

                // assetMng.CopyAssetAsOverride();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify Faction", GUILayout.Width(128)))
            {

                var dic = new List<Faction>();

                for (int i = 0; i < facListBool.Count; i++)
                {
                    if (facListBool.ToArray()[i].Value == true)
                    {
                        foreach (var facGrp in facList)
                        {
                            foreach (var facInList in facGrp.Value)
                            {
                                if (facListBool.ToArray()[i].Key == facInList.id)
                                {
                                    dic.Add(facInList);
                                }
                            }
                        }
                    }

                }

                if (FAC_EDITOR_Manager == null)
                {
                    FactionsEditorManager assetMng = (FactionsEditorManager)ScriptableObject.CreateInstance(typeof(FactionsEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "fac_template";
                    assetMng.assetName_new = "fac_template";
                    assetMng.assetID_new = "fac_template_ID";

                    assetMng.modifyDic = new List<Faction>(dic);
                }
                else
                {
                    FactionsEditorManager assetMng = FAC_EDITOR_Manager;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "fac_template";
                    assetMng.assetName_new = "fac_template";
                    assetMng.assetID_new = "fac_template";

                    assetMng.modifyDic = new List<Faction>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[facListBool.Keys.Count];

                int i = 0;
                foreach (var ck in facListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < facListBool.Keys.Count; i3++)
                {
                    facListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[facListBool.Keys.Count];

                int i = 0;
                foreach (var ck in facListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < facListBool.Keys.Count; i3++)
                {
                    facListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);


            if (is_Input_now)
            {

                facSearchBackup = new Dictionary<string, List<Faction>>();

                foreach (var key in facList)
                {
                    var tempFacList = new List<Faction>();

                    foreach (var facInList in key.Value)
                    {

                        if (sortBy_index == 0)
                        {
                            string soloName = facInList.factionName;

                            RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(facInList);
                                }
                            }
                        }
                        else if (sortBy_index == 1)
                        {
                            if (searchInput != null)
                            {
                                if (facInList.id.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(facInList);
                                }
                            }
                        }
                        else if (sortBy_index == 2)
                        {
                            string soloName = facInList.super_faction.Replace("Kingdom.", "");

                            // RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(facInList);
                                }
                            }
                        }
                    }
                    facSearchBackup.Add(key.Key, tempFacList);
                }
                facSearch = new Dictionary<string, List<Faction>>(facSearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(facSearch);
            }
            else
            {
                DrawList(facList);
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
        var facData = loadedMod.modFilesData.factionsData.factions;

        if (facListBool != null && facListBool.Count != 0)
        {
            facListBoolBackup = facListBool;
        }

        if (show_bandit || show_mafia || show_minor || show_outlaw || show_other)
        {
            // EditorGUILayout.BeginHorizontal();
            SortByMethod(facData);
        }
        else
        {
            // SortByMethodNoneBool(npcData);
            facList = new Dictionary<string, List<Faction>>();
        }

        if (searchInput != "" && is_Input_now == false)
        {
            is_Input_now = true;
        }

        // if (show_HeroLink && show_Hero)
        // {
        //     CheckHeroesData();

        //     SortByMethod(npcData);
        // }

        if (facList.Count != 0)
        {
            facListBool = new Dictionary<string, bool>();
            foreach (var list in facList)
            {
                foreach (var npc in list.Value)
                {
                    if (!facListBool.ContainsKey(npc.id))
                    {
                        facListBool.Add(npc.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (facListBoolBackup != null && facListBool != null && facListBoolBackup.Count != 0 && facListBool.Count != 0)
        {
            for (int i = 0; i < facListBool.Count; i++)
            {

                if (facListBoolBackup.ContainsKey(facListBool.ToArray()[i].Key))
                {
                    var b = false;
                    facListBoolBackup.TryGetValue(facListBool.ToArray()[i].Key, out b);
                    facListBool[facListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<Faction>> facListInput)
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
                    else if (sortBy_index == 2)
                    {
                        for (int i_f = 0; i_f < loadedMod.modFilesData.kingdomsData.kingdoms.Count; i_f++)
                        {
                            // Debug.Log(currMod.modFilesData.factionsData.factions[i_f].name);

                            if (loadedMod.modFilesData.kingdomsData.kingdoms[i_f].id == fac_list.Key)
                            {
                                var nm = loadedMod.modFilesData.kingdomsData.kingdoms[i_f].kingdomName;

                                RemoveTSString(ref nm);
                                EditorGUILayout.LabelField(nm, tagLabelStyle);
                                break;
                            }
                        }
                    }


                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_FAC = 0;
                    foreach (var facChar in fac_list.Value)
                    {

                        var fac = fac_list.Value[i_FAC];

                        var soloName = "";
                        if (sortBy_index == 0 || sortBy_index == 2)
                        {
                            soloName = fac.factionName;
                            RemoveTSString(ref soloName);

                            if (soloName == "")
                            {
                                soloName = fac.id;
                            }
                        }
                        if (sortBy_index == 1)
                        {
                            soloName = fac.id;
                        }

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        // if (fac.is_bandit == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, banditStyle);
                        // }
                        // else if (fac.is_mafia == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, mafiaStyle);
                        // }


                        // else if (fac.is_minor_faction == "true")
                        // {
                        //     Color titleColor;
                        //     ColorUtility.TryParseHtmlString(fac._BannerColorA, out titleColor);
                        //     minorFactionStyle.normal.textColor = titleColor;

                        //     EditorGUILayout.LabelField(soloName, minorFactionStyle);
                        // }
                        // else if (fac.is_outlaw == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, outlawStyle);
                        // }
                        // else
                        // {

                        if (fac._BannerColorB != "")
                        {
                            Color titleColor;
                            ColorUtility.TryParseHtmlString(fac._BannerColorB, out titleColor);
                            minorFactionStyle.normal.textColor = titleColor * 2;
                            EditorGUILayout.LabelField(soloName, minorFactionStyle);

                        }
                        else
                        {
                            EditorGUILayout.LabelField(soloName, EditorStyles.label);

                        }

                        // }


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-128);

                        EditorGUI.BeginChangeCheck();
                        if (facListBool.ContainsKey(fac.id))
                        {
                            facListBool[fac.id] = EditorGUILayout.Toggle(facListBool[fac.id], GUILayout.ExpandWidth(false));
                        }
                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = fac.id;
                                }
                                else
                                {
                                    currEventSelection[1] = fac.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (facListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < facListBool.Count; i3++)
                                        {
                                            if (facListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (facListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < facListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                facListBool[facListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }




                            }

                        }


                        EditorGUIUtility.labelWidth = originLabelWidth;

                        // EditorGUILayout.Space(-64);

                        EditorGUILayout.ObjectField(fac, typeof(Faction), true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(4);
                        DrawUILine(colUILine, 1, 4);

                        if (fac.is_bandit != "true" && fac.is_outlaw != "true" && fac.is_mafia != "true" && fac.is_minor_faction != "true")
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.ExpandWidth(false));
                            GUILayout.Space(32);

                            // ! _NODES EDITOR
                            if (GUILayout.Button("Edit Family Tree", GUILayout.Width(192)))
                            {
                                EditFamilyTree(fac);

                                //CheckData
                                //var charData = new List<NPCCharacter>();
                                //var heroData = new List<Hero>();
                                //var facData = new List<Faction>();

                                //CheckClanMixTree(fac, charData, heroData, facData);
                            }
                            EditorGUILayout.EndHorizontal();

                        }

                        EditorGUILayout.BeginVertical();
                        // GUILayout.Space(16);
                        DrawUILine(colUILine, 4, 12);
                        EditorGUILayout.EndVertical();


                        i_FAC++;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        // Repaint();


    }
    private void CheckClanMixTree(Faction fac, List<NPCCharacter> npc_list, List<Hero> hero_list, List<Faction> faction_list)
    {


        foreach (var n in loadedMod.modFilesData.npcChrData.NPCCharacters)
        {
            if (n.is_hero == "true")
            {
                npc_list.Add(n);
            }
        }

        foreach (var mod in loadedMod.modDependencies)
        {
            ModuleReceiver dpd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

            if (dpd_mod != null)
            {
                foreach (var n in dpd_mod.modFilesData.npcChrData.NPCCharacters)
                {
                    if (n.is_hero == "true")
                    {
                        npc_list.Add(n);
                    }
                }
            }
        }

        foreach (var h in loadedMod.modFilesData.heroesData.heroes)
        {
            hero_list.Add(h);
        }

        foreach (var mod in loadedMod.modDependencies)
        {
            ModuleReceiver dpd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

            if (dpd_mod != null)
            {
                foreach (var h in dpd_mod.modFilesData.heroesData.heroes)
                {
                    hero_list.Add(h);
                }
            }
        }

        foreach (var f in loadedMod.modFilesData.factionsData.factions)
        {
            faction_list.Add(f);
        }

        foreach (var mod in loadedMod.modDependencies)
        {
            ModuleReceiver dpd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

            if (dpd_mod != null)
            {
                foreach (var f in dpd_mod.modFilesData.factionsData.factions)
                {
                    faction_list.Add(f);
                }
            }
        }

       // AssetDatabase.Refresh();


        foreach (var hero in hero_list)
        {
            hero.isMixedClans = false;
            hero.mixedFather = "";
            hero.mixedFather_fac = "";
            hero.mixedFather_redactable = false;
            hero.mixedMother = "";
            hero.mixedMother_fac = "";
            hero.mixedMother_redactable = false;
            hero.mixedSpouse = "";
            hero.mixedSpouse_fac = "";
            hero.mixedSpouse_redactable = false;
        }

        //AssetDatabase.Refresh();


        foreach (var hero in hero_list)
        {
            if (hero != null && hero.father != null && hero.mother != null && hero.spouse != null)
            {
                if (hero.father != "")
                {
                    var nm = hero.father.Replace("Hero.", "");

                    foreach (var father_hero in hero_list)
                    {
                        if (father_hero.id == nm)
                        {
                            if (father_hero.faction.Replace("Faction.", "") != fac.id && hero.faction.Replace("Faction.", "") == fac.id)
                            {
                                father_hero.isMixedClans = true;
                            }
                        }
                    }
                }

                if (hero.mother != "")
                {
                    var nm = hero.mother.Replace("Hero.", "");

                    foreach (var mother_hero in hero_list)
                    {
                        if (mother_hero.id == nm && mother_hero != hero)
                        {
                            if (mother_hero.faction.Replace("Faction.", "") != fac.id && hero.faction.Replace("Faction.", "") == fac.id)
                            {
                                mother_hero.isMixedClans = true;
                            }
                        }
                    }


                }

                if (hero.spouse != "")
                {
                    var nm = hero.spouse.Replace("Hero.", "");

                    foreach (var spouse_hero in hero_list)
                    {
                        if (spouse_hero.id == nm)
                        {
                            if (spouse_hero.faction.Replace("Faction.", "") != fac.id && hero.faction.Replace("Faction.", "") == fac.id)
                            {
                                spouse_hero.isMixedClans = true;
                            }
                        }
                    }
                }
            }
        }

        foreach (var hero in hero_list)
        {
            if (hero != null)
            {
                if (hero.isMixedClans)
                {
                    if (hero.father != "")
                    {
                        var nm = hero.father.Replace("Hero.", "");

                        foreach (var father_npc in npc_list)
                        {
                            if (father_npc.id == nm && father_npc != hero)
                            {

                                var npc_name_str = father_npc.npcName;
                                RemoveTSString(ref npc_name_str);
                                hero.mixedFather = npc_name_str;

                                foreach (var father_hero in hero_list)
                                {
                                    if (father_hero.id == nm)
                                    {
                                        if (father_hero.faction.Replace("Faction.", "") != fac.id)
                                        {
                                            hero.mixedFather_fac = father_hero.faction.Replace("Faction.", "");
                                        }
                                        else
                                        {
                                            hero.mixedFather_redactable = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        hero.mixedFather_redactable = true;
                    }


                    if (hero.mother != "")
                    {
                        var nm = hero.mother.Replace("Hero.", "");

                        foreach (var mother_npc in npc_list)
                        {
                            if (mother_npc.id == nm && mother_npc != hero)
                            {

                                var npc_name_str = mother_npc.npcName;
                                RemoveTSString(ref npc_name_str);
                                hero.mixedMother = npc_name_str;

                                foreach (var mother_hero in hero_list)
                                {
                                    if (mother_hero.id == nm)
                                    {

                                        if (mother_hero.faction.Replace("Faction.", "") != fac.id)
                                        {
                                            hero.mixedMother_fac = mother_hero.faction.Replace("Faction.", "");
                                        }
                                        else
                                        {
                                            hero.mixedMother_redactable = true;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        hero.mixedMother_redactable = true;
                    }


                    if (hero.spouse != "")
                    {
                        var nm = hero.spouse.Replace("Hero.", "");

                        foreach (var spouse_npc in npc_list)
                        {
                            if (spouse_npc.id == nm && spouse_npc != hero)
                            {

                                var npc_name_str = spouse_npc.npcName;
                                RemoveTSString(ref npc_name_str);
                                hero.mixedSpouse = npc_name_str;

                                foreach (var spouse_hero in hero_list)
                                {
                                    if (spouse_hero.id == nm)
                                    {

                                        if (spouse_hero.faction.Replace("Faction.", "") != fac.id)
                                        {
                                            hero.mixedSpouse_fac = spouse_hero.faction.Replace("Faction.", "");

                                        }
                                        else
                                        {
                                            hero.mixedSpouse_redactable = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        hero.mixedSpouse_redactable = true;
                    }
                }
            }
        }

        foreach (var faction_hero in hero_list)
        {
            if (faction_hero != null)
            {
                if (faction_hero.faction.Replace("Faction.", "") == fac.id)
                {
                    foreach (var parent_hero in hero_list)
                    {
                        if (parent_hero.isMixedClans && parent_hero != faction_hero)
                        {
                            if (faction_hero.father.Replace("Hero.", "") == parent_hero.id)
                            {
                                faction_hero.mixedFather = parent_hero.id;
                            }
                            if (faction_hero.mother.Replace("Hero.", "") == parent_hero.id)
                            {
                                faction_hero.mixedMother = parent_hero.id;
                            }
                            if (faction_hero.spouse.Replace("Hero.", "") == parent_hero.id)
                            {
                                faction_hero.mixedSpouse = parent_hero.id;
                            }
                        }
                    }
                }
            }
        }

        foreach (var faction_hero in hero_list)
        {
            if (faction_hero != null)
            {
                if (faction_hero.isMixedClans)
                    foreach (var faction in faction_list)
                    {
                        if (faction.id == faction_hero.mixedFather_fac)
                        {
                            var fac_name_str = faction.factionName;
                            RemoveTSString(ref fac_name_str);
                            faction_hero.mixedFather_fac = fac_name_str;

                        }
                        if (faction.id == faction_hero.mixedMother_fac)
                        {
                            var fac_name_str = faction.factionName;
                            RemoveTSString(ref fac_name_str);
                            faction_hero.mixedMother_fac = fac_name_str;

                        }
                        if (faction.id == faction_hero.mixedSpouse_fac)
                        {
                            var fac_name_str = faction.factionName;
                            RemoveTSString(ref fac_name_str);
                            faction_hero.mixedSpouse_fac = fac_name_str;
                        }
                    }
            }
        }
        //AssetDatabase.Refresh();
    }

    private void EditFamilyTree(Faction fac)
    {
        var charData = new List<NPCCharacter>();
        var heroData = new List<Hero>();
        var facData = new List<Faction>();

        CheckClanMixTree(fac, charData, heroData, facData);

        var nodesGraphPath = "Assets/Settings/NodeGraphs/FamilyTreeGraphEditor.asset";
        FamilyTreeGraph tree = (FamilyTreeGraph)AssetDatabase.LoadAssetAtPath(nodesGraphPath, typeof(FamilyTreeGraph));
        // tree = (FamilyTreeGraph)ScriptableObject.CreateInstance(typeof(FamilyTreeGraph));

        tree.nodes = new List<Node>();
        tree.mainFaction = fac;
        tree.heroData = heroData;
        tree.charData = charData;
        tree.facData = facData;

        bool heroSwitch = false;

        int generations = 6;
        Dictionary<Hero, Node>[] gens = new Dictionary<Hero, Node>[generations];
        int[] pos_x = new int[generations];
        int[] pos_y = new int[generations];

        //var heroData = loadedMod.modFilesData.heroesData.heroes;
        //var charData = loadedMod.modFilesData.npcChrData.NPCCharacters;

        // int i = 0;
        NodeEditorWindow w = XNodeEditor.NodeEditorWindow.Open(tree as XNode.NodeGraph);
        var title = fac.factionName;
        RemoveTSString(ref title);
        w.title = title;

        DrawHeaderNode(fac, tree, 0, -340);

        for (int i = 0; i < generations; i++)
        {
            gens[i] = new Dictionary<Hero, Node>();

            if (i == 0)
            {

                foreach (var hero in heroData)
                {
                    if (hero != null)
                    {
                        if (hero.faction.Replace("Faction.", "") == fac.id)
                        {
                            //Debug.Log(hero.faction.Replace("Faction.", ""));
                            //Debug.Log(hero.id);
                            if (hero.father == "" && hero.mother == "")
                            {
                                if (hero.spouse != "")
                                {
                                    Hero heroLoad = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
                                    GetHero(hero.spouse, fac.moduleID, ref heroLoad);

                                    if (heroLoad.father == "" && heroLoad.mother == "")
                                    {
                                        // Debug.Log("ROOT");

                                        DrawNode(fac, tree, pos_x[i], pos_y[i], charData, hero, ref gens[i]);
                                        DrawNode(fac, tree, pos_x[i] + 332, pos_y[i], charData, heroLoad, ref gens[i]);
                                        pos_x[i] = pos_x[i] + 1440 + 332;
                                    }
                                }
                                {
                                    //Debug.Log(hero.id);

                                    // Debug.Log("ROOT NO SPOUSE");
                                    DrawNode(fac, tree, pos_x[i], pos_y[i], charData, hero, ref gens[i]);
                                    pos_x[i] = pos_x[i] + 512;
                                }

                            }
                            else
                            {
                                //Debug.Log(hero.id);

                                // Debug.Log("ROOT NO SPOUSE");
                                DrawNode(fac, tree, pos_x[i], pos_y[i], charData, hero, ref gens[i]);
                                pos_x[i] = pos_x[i] + 512;
                            }

                        }
                    }
                }
            }
            else
            {
                pos_y[i] = pos_y[i - 1] + 512;



                foreach (var genHero in gens[i - 1])
                {
                    if (genHero.Key.faction.Replace("Faction.", "") == fac.id)
                    {
                        heroSwitch = true;
                        foreach (var hero in heroData)
                        {
                            if (hero != null)
                            {
                                if (hero.faction.Replace("Faction.", "") == fac.id)
                                {
                                    //Debug.Log(hero.id);

                                    if (hero.father != "" || hero.mother != "")
                                    {

                                        if (hero.father.Replace("Hero.", "") == genHero.Key.id || hero.mother.Replace("Hero.", "") == genHero.Key.id)
                                        {
                                            // if (test % 2 == 0) // Is even

                                            int mult = 0;

                                            if (i % 2 == 1)
                                            {
                                                mult = (i * 4) * 256;
                                            }

                                            if (heroSwitch)
                                            {

                                                int xVal = int.Parse(genHero.Value.position.x.ToString());
                                                // DrawNode(fac, tree, xVal, pos_y[i], charData, hero, ref gens[i]);
                                                heroSwitch = false;

                                                if (hero.spouse != "")
                                                {
                                                    Hero heroLoad = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
                                                    GetHero(hero.spouse, fac.moduleID, ref heroLoad);

                                                    DrawNode(fac, tree, xVal - 840, pos_y[i], charData, hero, ref gens[i]);
                                                    DrawNode(fac, tree, xVal - 840 + 320, pos_y[i], charData, heroLoad, ref gens[i]);

                                                    pos_x[i] = xVal - 840 + 512 + 320 + mult;

                                                }
                                                else
                                                {
                                                    // Debug.Log("ROOT NO SPOUSE");
                                                    DrawNode(fac, tree, xVal, pos_y[i], charData, hero, ref gens[i]);
                                                    pos_x[i] = pos_x[i] + 512 + mult;
                                                }
                                            }
                                            else
                                            {
                                                //var val = gens.ToArray()[i].ToArray()[gens.ToArray()[i].Count - 1];
                                                //int xVal = 0;
                                                //if (val is KeyValuePair)
                                                //    xVal = int.Parse(gens.ToArray()[i].ToArray()[gens.ToArray()[i].Count - 1].Value.position.x.ToString());
                                                var xVal = 512 + mult;

                                                if (hero.spouse != "")
                                                {
                                                    Hero heroLoad = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
                                                    GetHero(hero.spouse, fac.moduleID, ref heroLoad);

                                                    DrawNode(fac, tree, xVal, pos_y[i], charData, hero, ref gens[i]);
                                                    DrawNode(fac, tree, xVal + 320, pos_y[i], charData, heroLoad, ref gens[i]);
                                                    pos_x[i] = xVal + 512 + 320 + mult;

                                                }
                                                else
                                                {
                                                    DrawNode(fac, tree, xVal, pos_y[i], charData, hero, ref gens[i]);
                                                    pos_x[i] = pos_x[i] + 512 + mult;

                                                    heroSwitch = false;
                                                }
                                            }
                                        }


                                    }
                                }

                            }
                        }
                    }
                }

                foreach (var hero in heroData)
                {
                    if (hero != null)
                    {
                        if (hero.faction.Replace("Faction.", "") == fac.id)
                        {
                            foreach (var parent_hero in heroData)
                            {
                                if (parent_hero.isMixedClans && parent_hero != hero)
                                {
                                    if (hero.mixedFather == parent_hero.id)
                                    {
                                        DrawNode(fac, tree, pos_x[i], pos_y[i], charData, parent_hero, ref gens[i]);
                                        pos_x[i] = pos_x[i] + 512;
                                    }
                                    if (hero.mixedMother == parent_hero.id)
                                    {
                                        DrawNode(fac, tree, pos_x[i], pos_y[i], charData, parent_hero, ref gens[i]);
                                        pos_x[i] = pos_x[i] + 512;
                                    }
                                    if (hero.mixedSpouse == parent_hero.id)
                                    {
                                        DrawNode(fac, tree, pos_x[i], pos_y[i], charData, parent_hero, ref gens[i]);
                                        pos_x[i] = pos_x[i] + 512;
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        // w.Home(); // Focus selected node


        // w.graph.nodes.Sort();
        // w.graphEditor.nod.Sort();


        foreach (Node m_node in tree.nodes)
        {
            var node = m_node as FamilyTreeNPC;

            if (node != null)
            {
                foreach (Node m_nodeParent in tree.nodes)
                {
                    var nodeParent = m_nodeParent as FamilyTreeNPC;

                    if (nodeParent != null)
                    {
                        if (nodeParent.spouse != null && nodeParent.spouse.id == node.ID)
                        {
                            var rel_a_Out = node.GetOutputPort("Out");
                            var rel_b_In = nodeParent.GetInputPort("S");
                            rel_a_Out.Connect(rel_b_In);
                        }
                        if (nodeParent.father != null && nodeParent.father.id == node.ID)
                        {
                            var rel_a_Out = node.GetOutputPort("Out");
                            var rel_b_In = nodeParent.GetInputPort("F");
                            rel_a_Out.Connect(rel_b_In);
                        }

                        if (nodeParent.mother != null && nodeParent.mother.id == node.ID)
                        {
                            var rel_a_Out = node.GetOutputPort("Out");
                            var rel_b_In = nodeParent.GetInputPort("M");
                            rel_a_Out.Connect(rel_b_In);
                        }
                    }
                }
            }
        }



        for (int i = 0; i < generations; i++)
        {
            for (int i2 = 0; i2 < gens[i].Count; i2++)
            {
                foreach (Node m_node in tree.nodes)
                {
                    var node = m_node as FamilyTreeNPC;

                    if (node != null)
                    {
                        if (gens[i].ToArray()[i2].Key.id == node.ID)
                        {
                            int stackCount = 0;
                            int offset = 1536;
                            Vector2 prevPosStatic = new Vector2(0, 0);
                            int prevStackOffset = 0;

                            foreach (Node m_nodeParent in tree.nodes)
                            {
                                var nodeParent = m_nodeParent as FamilyTreeNPC;

                                if (nodeParent != null)
                                {
                                    if (nodeParent.father != null && nodeParent.father.id == node.ID)
                                    {
                                        stackCount++;
                                    }
                                    else
                                    {
                                        if (nodeParent.mother != null && nodeParent.mother.id == node.ID)
                                        {
                                            stackCount++;

                                        }
                                    }

                                }
                            }

                            var stackOffset = (offset * stackCount) / 4;

                            node.position = new Vector2(node.position.x - prevPosStatic.x + prevStackOffset, node.position.y);

                            Vector2 tempPosStatic = new Vector2(node.position.x + prevStackOffset, node.position.y);
                            Vector2 tempPosDynamic = NodeNextRow(node, stackOffset);


                            foreach (Node m_nodeParent in tree.nodes)
                            {
                                var nodeParent = m_nodeParent as FamilyTreeNPC;

                                if (nodeParent != null)
                                {
                                    if (nodeParent.father != null && nodeParent.father.id == node.ID)
                                    {

                                        nodeParent.position = new Vector2(tempPosDynamic.x, tempPosDynamic.y);
                                        tempPosDynamic = new Vector2(tempPosDynamic.x + offset, tempPosDynamic.y);

                                    }
                                    else
                                    {
                                        if (nodeParent.mother != null && nodeParent.mother.id == node.ID)
                                        {
                                            // Debug.Log(nodeParent.ID);

                                            nodeParent.position = new Vector2(tempPosDynamic.x, tempPosDynamic.y);
                                            tempPosDynamic = new Vector2(tempPosDynamic.x + offset, tempPosDynamic.y);

                                        }
                                    }


                                    if (nodeParent.spouse != null && nodeParent.spouse.id == node.ID)
                                    {
                                        nodeParent.position = new Vector2(tempPosStatic.x - 352, tempPosStatic.y);


                                    }

                                }
                                prevStackOffset = stackOffset;
                                prevPosStatic = node.position;
                            }
                        }
                    }
                }
            }
        }
    }

    private static Vector2 NodeNextRow(FamilyTreeNPC node, int offset)
    {
        return new Vector2(node.position.x - offset, node.position.y + 512);
    }

    private void DrawNode(Faction fac, FamilyTreeGraph tree, int posX, int posY, List<NPCCharacter> charData, Hero hero, ref Dictionary<Hero, Node> heroList)
    {
        NPCCharacter character = null;

        foreach (var charNPC in charData)
        {
            if (charNPC.id == hero.id)
            {
                character = charNPC;
            }
        }

        FamilyTreeNPC treeNPC = (FamilyTreeNPC)ScriptableObject.CreateInstance(typeof(FamilyTreeNPC));

        treeNPC.position = new Vector2(posX, posY);
        // var nodeTitleName = character.npcName;
        // RemoveTSString(ref nodeTitleName);

        // treeNPC.name = nodeTitleName;

        treeNPC.heroAsset = hero;
        treeNPC.npcAsset = character;
        treeNPC.ID = hero.id;
        // treeNPC.Age = int.Parse(character.age);

        // if (character.is_female == "true")
        // {
        //     treeNPC.isFemale = true;
        // }

        if (hero.father != "")
        {
            //Debug.Log(hero.father);
            //Debug.Log(fac.moduleID);

            Hero heroLoad = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
            GetHero(hero.father, fac.moduleID, ref heroLoad);
            // treeNPC.name_Father = heroLoad.id;
            treeNPC.father = heroLoad;
        }
        if (hero.mother != "")
        {
            Hero heroLoad = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
            GetHero(hero.mother, fac.moduleID, ref heroLoad);
            // treeNPC.name_Mother = heroLoad.id;
            treeNPC.mother = heroLoad;
        }
        if (hero.spouse != "")
        {
            Hero heroLoad = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
            GetHero(hero.spouse, fac.moduleID, ref heroLoad);
            // treeNPC.name_Spouse = heroLoad.id;
            treeNPC.spouse = heroLoad;
        }

        var contains = false;
        foreach (Node m_node in tree.nodes)
        {
            var node = m_node as FamilyTreeNPC;

            if (node != null)
                if (node.ID == treeNPC.ID)
                {
                    contains = true;
                }

        }


        if (!contains)
        {
            tree.nodes.Add(treeNPC);
            heroList.Add(hero, treeNPC);
        }
    }
    private void DrawHeaderNode(Faction fac, FamilyTreeGraph tree, int posX, int posY)
    {

        FamilyTreeHeader header = (FamilyTreeHeader)ScriptableObject.CreateInstance(typeof(FamilyTreeHeader));
        if(fac.Node_X != 0 && fac.Node_Y != 0)
        {
            header.position = new Vector2(fac.Node_X, fac.Node_Y);
        }
        else
        {
            header.position = new Vector2(posX, posY);
        }

        header.header_faction = fac;

        tree.nodes.Add(header);
    }

    public void SortByMethod(List<Faction> facListInput)
    {
        this.facList = new Dictionary<string, List<Faction>>();


        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Faction>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var fac in facListInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = fac.factionName;

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
                    if (character != "other")
                    {
                        if (soloFirstChar == character || soloFirstChar == chrUPstr)
                        {
                            if (fac.is_mafia == "true")
                            {
                                CheckShowBool(newList, fac, ref fac.is_mafia, ref show_mafia);

                            }
                            else if (fac.is_bandit == "true")
                            {
                                CheckShowBool(newList, fac, ref fac.is_bandit, ref show_bandit);

                            }
                            else if (fac.is_minor_faction == "true")
                            {
                                CheckShowBool(newList, fac, ref fac.is_minor_faction, ref show_minor);

                            }
                            else if (fac.is_outlaw == "true")
                            {
                                CheckShowBool(newList, fac, ref fac.is_outlaw, ref show_outlaw);

                            }
                            else
                            {
                                if (show_other)
                                {
                                    newList.Add(fac);
                                }
                            }

                        }
                    }
                }
                facList.Add(character, newList);
            }

        }


        if (sortBy_index == 2)
        {
            foreach (var fac in facListInput)
            {
                if (fac.super_faction != "" && fac.super_faction.Contains("Kingdom."))
                {
                    var keyName = fac.super_faction.Replace("Kingdom.", "");
                    var newList = new List<Faction>();

                    if (!facList.Keys.Contains(keyName))
                    {
                        facList.Add(keyName, newList);
                    }
                }
                else
                {
                    var newList = new List<Faction>();

                    if (!facList.Keys.Contains("None"))
                    {
                        facList.Add("None", newList);
                    }
                }

            }

            foreach (var dic in facList)
            {
                foreach (var fac in facListInput)
                {
                    if (fac.super_faction.Replace("Kingdom.", "") == dic.Key)
                    {
                        if (fac.is_bandit == "true")
                        {
                            CheckShowBool(dic.Value, fac, ref fac.is_bandit, ref show_bandit);

                        }
                        else if (fac.is_mafia == "true")
                        {
                            CheckShowBool(dic.Value, fac, ref fac.is_mafia, ref show_mafia);

                        }
                        else if (fac.is_minor_faction == "true")
                        {
                            CheckShowBool(dic.Value, fac, ref fac.is_minor_faction, ref show_minor);

                        }
                        else if (fac.is_outlaw == "true")
                        {
                            CheckShowBool(dic.Value, fac, ref fac.is_outlaw, ref show_outlaw);

                        }
                        else
                        {
                            if (show_other)
                            {
                                dic.Value.Add(fac);
                            }
                        }
                    }
                }
            }

        }

    }


    public void SortByMethodNoneBool(List<Faction> facInput)
    {
        facList = new Dictionary<string, List<Faction>>();

        // Debug.Log(sortBy_index);
        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Faction>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var fac in facInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = fac.factionName;

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
                facList.Add(character, newList);
            }


            foreach (var npcChar in facInput)
            {
                bool contains = false;
                foreach (var facL in facList)
                {
                    if (facL.Value.Contains(npcChar))
                    {
                        contains = true;
                        break;

                    }
                }

                if (!contains)
                {
                    facList["other"].Add(npcChar);
                }

            }
        }

        if (sortBy_index == 2)
        {
            foreach (var fac in facInput)
            {
                if (fac.super_faction != "")
                {
                    var keyName = fac.super_faction.Replace("Kingdom.", "");
                    var newList = new List<Faction>();

                    if (!facList.Keys.Contains(keyName))
                    {
                        facList.Add(keyName, newList);
                    }
                }
                else
                {
                    var newList = new List<Faction>();

                    if (!facList.Keys.Contains("None"))
                    {
                        facList.Add("None", newList);
                    }
                }

            }


            foreach (var dic in facList)
            {
                foreach (var fac in facInput)
                {
                    if (fac.super_faction.Replace("Kingdom.", "") == dic.Key)
                    {
                        dic.Value.Add(fac);
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

                        if (contains == false && heroOutput == null)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + modName + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }
    }

    private void CheckShowBool(List<Faction> newList, Faction fac, ref string facParam, ref bool faBoolParam)
    {
        if (facParam == "true")
        {
            if (faBoolParam != false)
            {
                newList.Add(fac);
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

    public static FactionsEditorManager FAC_EDITOR_Manager
    {
        get { return EditorWindow.GetWindow<FactionsEditorManager>(); }
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

