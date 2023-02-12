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

public class EquipmentsEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    //public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<Equipment>> equipment_List;
    public Dictionary<string, List<Equipment>> equipment_SearchBackup;
    public Dictionary<string, List<Equipment>> equipment_Search;
    public Dictionary<string, bool> equipment_ListBool;
    public Dictionary<string, bool> equipment_ListBoolBackup;
    public List<Equipment> modifyList;


    bool is_Input_now;
    string searchInput;

    public bool show_IsEquipmentTemplate = true;
    public bool show_IsNobleTemplate = true;
    public bool show_IsMediumTemplate = true;
    public bool show_IsHeavyTemplate = true;
    public bool show_IsFlamboyantTemplate = true;
    public bool show_IsStoicTemplate = true;
    public bool show_IsNomadTemplate = true;
    public bool show_IsWoodlandTemplate = true;
    public bool show_IsFemaleTemplate = true;
    public bool show_IsCivilianTemplate = true;
    public bool show_IsCombatantTemplate = true;
    public bool show_IsNoncombatantTemplate = true;
    //TODO add 1.8.0 browser filters flags 
    public bool show_other = true;

    //

    Color armorCol;
    GUIStyle armorStyle;
    Color weaponCol;
    GUIStyle weaponStyle;
    Color tradeCol;
    GUIStyle tradeStyle;
    Color horseCol;
    GUIStyle horseStyle;

    // 

    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    // string dataPath = "Assets/BDT/Resources/Data/";

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
        EditorWindow.GetWindow(typeof(EquipmentsEditor));
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


        equipment_List = new Dictionary<string, List<Equipment>>();
        equipment_SearchBackup = new Dictionary<string, List<Equipment>>();
        equipment_Search = new Dictionary<string, List<Equipment>>();
        equipment_ListBool = new Dictionary<string, bool>();
        equipment_ListBoolBackup = new Dictionary<string, bool>();


        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            CheckAndResort();

        }

    }

    void OnGUI()
    {

        //armorStyle = new GUIStyle(EditorStyles.boldLabel);
        //weaponStyle = new GUIStyle(EditorStyles.boldLabel);
        //tradeStyle = new GUIStyle(EditorStyles.boldLabel);
        //horseStyle = new GUIStyle(EditorStyles.boldLabel);

        //ColorUtility.TryParseHtmlString("#629aa9", out armorCol);
        //ColorUtility.TryParseHtmlString("#aea400", out weaponCol);
        //ColorUtility.TryParseHtmlString("#ffa500", out horseCol);
        //ColorUtility.TryParseHtmlString("#ffc2e5", out tradeCol);

        //armorStyle.normal.textColor = armorCol;
        //weaponStyle.normal.textColor = weaponCol;
        //horseStyle.normal.textColor = horseCol;
        //tradeStyle.normal.textColor = tradeCol;



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

            //EditorGUILayout.LabelField("Sort By:", EditorStyles.boldLabel);
            //sortBy_index = EditorGUILayout.Popup(sortBy_index, sortBy_options, GUILayout.Width(128));

            //GUILayout.Space(4);

            EditorGUILayout.LabelField("Filter:", EditorStyles.boldLabel);

            searchInput = EditorGUILayout.TextField(searchInput, EditorStyles.toolbarSearchField, GUILayout.Width(256));
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {
                show_IsEquipmentTemplate = true;
                show_IsNobleTemplate = true;
                show_IsMediumTemplate = true;
                show_IsHeavyTemplate = true;
                show_IsFlamboyantTemplate = true;
                show_IsStoicTemplate = true;
                show_IsNomadTemplate = true;
                show_IsWoodlandTemplate = true;
                show_IsFemaleTemplate = true;
                show_IsCivilianTemplate = true;
                show_IsCombatantTemplate = true;
                show_IsNoncombatantTemplate = true;
                show_other = true;

                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                show_IsEquipmentTemplate = false;
                show_IsNobleTemplate = false;
                show_IsMediumTemplate = false;
                show_IsHeavyTemplate = false;
                show_IsFlamboyantTemplate = false;
                show_IsStoicTemplate = false;
                show_IsNomadTemplate = false;
                show_IsWoodlandTemplate = false;
                show_IsFemaleTemplate = false;
                show_IsCivilianTemplate = false;
                show_IsCombatantTemplate = false;
                show_IsNoncombatantTemplate = false;
                show_other = false;


                CheckAndResort();
                this.Repaint();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawUILine(colUILine, 1, 4);


            EditorGUILayout.BeginHorizontal();

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsEquipmentTemplate, "EquipmentTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsNobleTemplate, "NobleTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsMediumTemplate, "MediumTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsHeavyTemplate, "HeavyTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            CreateEditorToggle(ref show_IsFlamboyantTemplate, "FlamboyantTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsStoicTemplate, "StoicTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsNomadTemplate, "NomadTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsWoodlandTemplate, "WoodlandTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            CreateEditorToggle(ref show_IsFemaleTemplate, "FemaleTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsCivilianTemplate, "CivilianTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsCombatantTemplate, "CombatantTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_IsNoncombatantTemplate, "NoncombatantTemplate");
            DrawUILineVertical(colUILine, 1, 1, 16);

            EditorGUILayout.EndHorizontal();

            CreateEditorToggle(ref show_other, "Other");


            GUILayout.FlexibleSpace();

            DrawUILine(colUILine, 1, 4);

            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("check");

                CheckAndResort();

            }

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Equipments", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create equipemnts", GUILayout.Width(128)))
            {
                //! TODO
                if (EQUIPMENT_EDITOR_Manager == null)
                {
                    EquipmentsEditorManager assetMng = (EquipmentsEditorManager)ScriptableObject.CreateInstance(typeof(EquipmentsEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 8;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "equipmentSet_template";
                    assetMng.assetName_new = "equipmentSet_template";
                    assetMng.assetID_new = "equipmentSet_template_ID";
                }
                else
                {
                    EquipmentsEditorManager assetMng = EQUIPMENT_EDITOR_Manager;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 8;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "equipmentSet_template";
                    assetMng.assetName_new = "equipmentSet_template";
                    assetMng.assetID_new = "equipmentSet_template_ID";

                }


            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify Equipment", GUILayout.Width(128)))
            {

                var dic = new List<Equipment>();

                for (int i = 0; i < equipment_ListBool.Count; i++)
                {
                    if (equipment_ListBool.ToArray()[i].Value == true)
                    {
                        foreach (var facGrp in equipment_List)
                        {
                            foreach (var equipment_InList in facGrp.Value)
                            {
                                if (equipment_ListBool.ToArray()[i].Key == equipment_InList.id)
                                {
                                    dic.Add(equipment_InList);
                                }
                            }
                        }
                    }

                }

                if (EQUIPMENT_EDITOR_Manager == null)
                {
                    EquipmentsEditorManager assetMng = (EquipmentsEditorManager)ScriptableObject.CreateInstance(typeof(EquipmentsEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 8;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "equipmentSet_template";
                    assetMng.assetName_new = "equipmentSet_template";
                    assetMng.assetID_new = "equipmentSet_template_ID";

                    assetMng.modifyDic = new List<Equipment>(dic);
                }
                else
                {
                    EquipmentsEditorManager assetMng = EQUIPMENT_EDITOR_Manager;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 8;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "equipmentSet_template";
                    assetMng.assetName_new = "equipmentSet_template";
                    assetMng.assetID_new = "equipmentSet_template_ID";

                    assetMng.modifyDic = new List<Equipment>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[equipment_ListBool.Keys.Count];

                int i = 0;
                foreach (var ck in equipment_ListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < equipment_ListBool.Keys.Count; i3++)
                {
                    equipment_ListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[equipment_ListBool.Keys.Count];

                int i = 0;
                foreach (var ck in equipment_ListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < equipment_ListBool.Keys.Count; i3++)
                {
                    equipment_ListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);


            if (is_Input_now)
            {

                equipment_SearchBackup = new Dictionary<string, List<Equipment>>();

                foreach (var key in equipment_List)
                {
                    var tempFacList = new List<Equipment>();

                    foreach (var equip_InList in key.Value)
                    {


                        if (searchInput != null)
                        {
                            if (equip_InList.id.ToLower().Contains(searchInput.ToLower()))
                            {
                                tempFacList.Add(equip_InList);
                            }
                        }

                    }
                    equipment_SearchBackup.Add(key.Key, tempFacList);
                }
                equipment_Search = new Dictionary<string, List<Equipment>>(equipment_SearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(equipment_Search);
            }
            else
            {
                DrawList(equipment_List);
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
        var equipData = loadedMod.modFilesData.equipmentsData.equipmentSets;

        if (equipment_ListBool != null && equipment_ListBool.Count != 0)
        {
            equipment_ListBoolBackup = equipment_ListBool;
        }

        if (show_IsEquipmentTemplate || show_IsNobleTemplate || show_IsMediumTemplate || show_IsHeavyTemplate || show_IsFlamboyantTemplate || show_IsStoicTemplate
            || show_IsNomadTemplate || show_IsWoodlandTemplate || show_IsFemaleTemplate || show_IsCivilianTemplate || show_IsCombatantTemplate || show_IsNoncombatantTemplate || show_other)
        {
            // EditorGUILayout.BeginHorizontal();
            SortByMethod(equipData);
        }
        else
        {
            // SortByMethodNoneBool(itemData);
            equipment_List = new Dictionary<string, List<Equipment>>();
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

        if (equipment_List.Count != 0)
        {
            equipment_ListBool = new Dictionary<string, bool>();
            foreach (var list in equipment_List)
            {
                foreach (var eqp in list.Value)
                {
                    if (!equipment_ListBool.ContainsKey(eqp.id))
                    {
                        equipment_ListBool.Add(eqp.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (equipment_ListBoolBackup != null && equipment_ListBool != null && equipment_ListBoolBackup.Count != 0 && equipment_ListBool.Count != 0)
        {
            for (int i = 0; i < equipment_ListBool.Count; i++)
            {

                if (equipment_ListBoolBackup.ContainsKey(equipment_ListBool.ToArray()[i].Key))
                {
                    var b = false;
                    equipment_ListBoolBackup.TryGetValue(equipment_ListBool.ToArray()[i].Key, out b);
                    equipment_ListBool[equipment_ListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<Equipment>> eqpListInput)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var tagLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newCol;
        ColorUtility.TryParseHtmlString("#da5a47", out newCol);

        tagLabelStyle.normal.textColor = newCol;
        tagLabelStyle.fontSize = 24;
        tagLabelStyle.fontStyle = FontStyle.Bold;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));

        if (eqpListInput != null && eqpListInput.Count != 0)
        {

            foreach (var eqp_list in eqpListInput)
            {
                if (eqp_list.Value.Count != 0)
                {
                    var chrUP = eqp_list.Key.ToCharArray();
                    var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    DrawUILine(colUILine, 1, 12);



                    EditorGUILayout.LabelField(chrUPstr, tagLabelStyle);



                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_FAC = 0;
                    foreach (var facChar in eqp_list.Value)
                    {

                        var eqp = eqp_list.Value[i_FAC];

                        var soloName = "";

                        soloName = eqp.id;


                        EditorGUILayout.BeginHorizontal(GUILayout.Width(840));

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

                        // if (item._BannerColorB != "")
                        // {
                        //     Color titleColor;
                        //     ColorUtility.TryParseHtmlString(item._BannerColorB, out titleColor);
                        //     minorFactionStyle.normal.textColor = titleColor * 2;
                        //     EditorGUILayout.LabelField(soloName, minorFactionStyle);

                        // }
                        // else
                        // {
                        EditorGUILayout.LabelField(soloName, EditorStyles.label);

                        // }

                        // }


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-320);

                        EditorGUI.BeginChangeCheck();

                        if (equipment_ListBool.ContainsKey(eqp.id))
                        {
                            equipment_ListBool[eqp.id] = EditorGUILayout.Toggle(equipment_ListBool[eqp.id], GUILayout.ExpandWidth(false));

                        }

                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = eqp.id;
                                }
                                else
                                {
                                    currEventSelection[1] = eqp.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (equipment_ListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < equipment_ListBool.Count; i3++)
                                        {
                                            if (equipment_ListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (equipment_ListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < equipment_ListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                equipment_ListBool[equipment_ListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }




                            }

                        }


                        EditorGUIUtility.labelWidth = originLabelWidth;

                        // EditorGUILayout.Space(-64);

                        EditorGUILayout.ObjectField(eqp, typeof(Equipment), true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(4);
                        DrawUILine(colUILine, 1, 4);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.ExpandWidth(false));
                        GUILayout.Space(32);


                        EditorGUILayout.EndHorizontal();

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

    public void SortByMethod(List<Equipment> equipList)
    {
        this.equipment_List = new Dictionary<string, List<Equipment>>();

        foreach (var character in alpb)
        {
            var newList = new List<Equipment>();
            var chrUP = character.ToCharArray();
            var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

            foreach (var equip in equipList)
            {
                var soloName = "";

                soloName = equip.id;

                //RemoveTSString(ref soloName);

                var soloFirstChar = "";
                if (soloName != "")
                {
                    soloFirstChar = soloName.ToCharArray()[0].ToString();
                }

                if (character != "other")
                {
                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {

                        //                    public bool show_IsEquipmentTemplate;
                        //public bool show_IsNobleTemplate;
                        //public bool show_IsMediumTemplate;
                        //public bool show_IsHeavyTemplate;
                        //public bool show_IsFlamboyantTemplate;
                        //public bool show_IsStoicTemplate;
                        //public bool show_IsNomadTemplate;
                        //public bool show_IsWoodlandTemplate;
                        //public bool show_IsFemaleTemplate;
                        //public bool show_IsCivilianTemplate;
                        //public bool show_IsCombatantTemplate;
                        //public bool show_IsNoncombatantTemplate;
                        //public bool show_other;

                        if (equip.IsEquipmentTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsEquipmentTemplate, ref show_IsEquipmentTemplate);
                        }
                        if (equip.IsNobleTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsNobleTemplate, ref show_IsNobleTemplate);

                        }
                        if (equip.IsMediumTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsMediumTemplate, ref show_IsMediumTemplate);

                        }
                        if (equip.IsHeavyTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsHeavyTemplate, ref show_IsHeavyTemplate);

                        }
                        if (equip.IsFlamboyantTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsFlamboyantTemplate, ref show_IsFlamboyantTemplate);

                        }
                        if (equip.IsStoicTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsStoicTemplate, ref show_IsStoicTemplate);

                        }
                        if (equip.IsNomadTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsNomadTemplate, ref show_IsNomadTemplate);

                        }
                        if (equip.IsWoodlandTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsWoodlandTemplate, ref show_IsWoodlandTemplate);

                        }
                        if (equip.IsFemaleTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsFemaleTemplate, ref show_IsFemaleTemplate);

                        }
                        if (equip.IsCivilianTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsCivilianTemplate, ref show_IsCivilianTemplate);

                        }
                        if (equip.IsCombatantTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsCombatantTemplate, ref show_IsCombatantTemplate);

                        }
                        if (equip.IsNoncombatantTemplate)
                        {
                            CheckShowBool(newList, equip, ref equip.IsNoncombatantTemplate, ref show_IsNoncombatantTemplate);

                        }



                        if (show_other)
                        {
                            if (!newList.Contains(equip))
                                newList.Add(equip);
                        }

                    }

                }

            }
            equipment_List.Add(character, newList);
        }




    }


    public void SortByMethodNoneBool(List<Equipment> equipInput)
    {
        equipment_List = new Dictionary<string, List<Equipment>>();

        // Debug.Log(sortBy_index);
        //if (sortBy_index == 0 || sortBy_index == 1)
        //{
        foreach (var character in alpb)
        {
            var newList = new List<Equipment>();
            var chrUP = character.ToCharArray();
            var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

            foreach (var equip in equipInput)
            {
                var soloName = "";
                soloName = equip.id;

                RemoveTSString(ref soloName);

                var soloFirstChar = soloName.ToCharArray()[0].ToString();

                if (soloFirstChar == character || soloFirstChar == chrUPstr)
                {
                    if (!equip.IsCivilianTemplate && !equip.IsCombatantTemplate && !equip.IsEquipmentTemplate && !equip.IsFemaleTemplate
                        && !equip.IsFlamboyantTemplate && !equip.IsHeavyTemplate && !equip.IsMediumTemplate && !equip.IsNobleTemplate
                        && !equip.IsNomadTemplate && !equip.IsNoncombatantTemplate && !equip.IsStoicTemplate && !equip.IsWoodlandTemplate)
                    {
                        newList.Add(equip);
                    }
                }
            }
            equipment_List.Add(character, newList);
        }


        // foreach (var npcChar in itemInput)
        // {
        //     bool contains = false;
        //     foreach (var item in item_List)
        //     {
        //         if (item.Value.Contains(npcChar))
        //         {
        //             contains = true;
        //             break;

        //         }
        //     }

        //     if (!contains)
        //     {
        //         item_List["other"].Add(npcChar);
        //     }

        // }
        //}

    }

    private void CheckShowBool(List<Equipment> newList, Equipment equip, ref bool facParam, ref bool faBoolParam)
    {
        if (facParam)
        {
            if (faBoolParam != false)
            {
                if (!newList.Contains(equip))
                    newList.Add(equip);
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

    public static EquipmentsEditorManager EQUIPMENT_EDITOR_Manager
    {
        get { return EditorWindow.GetWindow<EquipmentsEditorManager>(); }
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

