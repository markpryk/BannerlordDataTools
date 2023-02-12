using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

// [CanEditMultipleObjects]
[CustomEditor(typeof(Equipment))]
public class EquipmentAssetEditor : Editor
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    string dataPath = "Assets/BDT/Resources/Data/";

    public Culture cultureIs;
    bool showEquipmentEditor;

    public string[] _eqpSetID;

    public bool[] eqpSetID_bool;

    //
    Item slot_Item0;
    Item slot_Item1;
    Item slot_Item2;
    Item slot_Item3;
    Item slot_Head;
    Item slot_Gloves;
    Item slot_Body;
    Item slot_Leg;
    Item slot_Cape;
    Item slot_Horse;
    Item slot_HorseHarness;

    //flags
    public bool _IsEquipmentTemplate_btn;
    public bool _IsNobleTemplate_btn;
    public bool _IsMediumTemplate_btn;
    public bool _IsHeavyTemplate_btn;
    public bool _IsFlamboyantTemplate_btn;
    public bool _IsStoicTemplate_btn;
    public bool _IsNomadTemplate_btn;
    public bool _IsWoodlandTemplate_btn;
    public bool _IsFemaleTemplate_btn;
    public bool _IsCivilianTemplate_btn;
    public bool _IsCombatantTemplate_btn;
    public bool _IsNoncombatantTemplate_btn;

    // update 1.7.2
    public bool _IsWandererEquipment_btn;
    public bool _IsChildTemplate_btn;

    // update 1.8.0
    public bool _IsGentryEquipment_btn;
    public bool _IsRebelHeroEquipment_btn;
    public bool _IsTeenagerEquipmentTemplate_btn;

    Equipment equip;

    bool _showEquipmentFlags;

    bool isDependency = false;
    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";


    private void OnEnable()
    {
        equip = (Equipment)target;
        EditorUtility.SetDirty(equip);

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

        if (equip.eqpSetID != null && equip.eqpSetID.Length > 0)
            eqpSetID_bool = new bool[equip.eqpSetID.Length];

    }
    public override void OnInspectorGUI()
    {

        if (settingsAsset.currentModule != equip.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == equip.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + equip.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + equip.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + equip.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + equip.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + equip.moduleID + ".asset", typeof(ModuleReceiver));
                    window.source = mod;
                    window.currentMod = mod;
                    settingsAsset.currentModule = mod.id;

                    window.Repaint();
                    this.Repaint();

                    // Debug.Log("Switched to mod: " + window.currentMod);
                }

            }


            if (isDependency)
            {
                if (GUILayout.Button("Create Override Asset"))
                {
                    if (ADM_Instance == null)
                    {
                        AssetsDataManager assetMng = new AssetsDataManager();
                        assetMng.windowStateID = 1;
                        assetMng.objID = 8;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = equip;

                        assetMng.assetName_org = equip.id;
                        assetMng.assetName_new = equip.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 8;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = equip;

                        assetMng.assetName_org = equip.id;
                        assetMng.assetName_new = equip.id;
                    }

                    // assetMng.CopyAssetAsOverride();
                }
            }
            GUILayout.EndHorizontal();
            DrawUILine(colUILine, 3, 12);

        }
        else
        {
            isDependency = false;
        }

        EditorGUI.BeginDisabledGroup(isDependency);

        EditorGUI.BeginDisabledGroup(true);
        equip.id = EditorGUILayout.TextField("Equipment ID", equip.id);

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {
            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 8;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = equip;

                assetMng.assetName_org = equip.id;
                assetMng.assetName_new = equip.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 8;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = equip;

                assetMng.assetName_org = equip.id;
                assetMng.assetName_new = equip.id;
            }
        }


        // npc.npcName = EditorGUILayout.TextField("NPC Name", npc.npcName);

        // NPC name & translationString tag
        DrawUILine(colUILine, 3, 12);

        var originLabelWidth = EditorGUIUtility.labelWidth;

        GUILayout.BeginHorizontal();

        // CULTURE
        if (equip.culture != null && equip.culture != "")
        {
            if (equip.culture.Contains("Culture."))
            {
                string dataName = equip.culture.Replace("Culture.", "");
                string asset = dataPath + equip.moduleID + "/Cultures/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(asset, typeof(Culture));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + equip.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Cultures/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Culture));
                                break;
                            }
                            else
                            {
                                cultureIs = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (cultureIs == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == equip.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.culturesData.cultures)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Cultures/" + dataName + ".asset";
                                            cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Culture));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (cultureIs == null)
                        {
                            Debug.Log("Culture " + dataName + " - Not EXIST in" + " ' " + equip.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }
            }
        }

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Culture:"));
        EditorGUIUtility.labelWidth = textDimensions.x - 32;

        EditorGUILayout.LabelField("Culture:", EditorStyles.label);
        object cultureField = EditorGUILayout.ObjectField(cultureIs, typeof(Culture), true, GUILayout.MaxWidth(192));
        cultureIs = (Culture)cultureField;

        if (cultureIs != null)
        {
            equip.culture = "Culture." + cultureIs.id;
        }
        else
        {
            equip.culture = "";
        }


        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = originLabelWidth;
        EditorGUILayout.EndHorizontal();
        //GUILayout.Space(8);
        // EditorGUI.EndDisabledGroup();
        DrawUILine(colUILine, 3, 12);

        //CULTURE END

        Color newCol;
        ColorUtility.TryParseHtmlString("#ff4f81", out newCol);

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;


         textDimensions = GUI.skin.label.CalcSize(new GUIContent("Equipment Flags: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!_showEquipmentFlags)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Equipment Flags";
        }

        _showEquipmentFlags = EditorGUILayout.Foldout(_showEquipmentFlags, showEditorLabel, hiderStyle);

        if (_showEquipmentFlags)
        {
            // BOOLEANS NPC

            // is civilian template
            DrawEquipmentFlagBool(ref equip.IsCivilianTemplate, ref _IsCivilianTemplate_btn, "IsCivilianTemplate");
            // is noble template
            DrawEquipmentFlagBool(ref equip.IsNobleTemplate, ref _IsNobleTemplate_btn, "IsNobleTemplate");
            // is medium template
            DrawEquipmentFlagBool(ref equip.IsMediumTemplate, ref _IsMediumTemplate_btn, "IsMediumTemplate");
            // is heavy template
            DrawEquipmentFlagBool(ref equip.IsHeavyTemplate, ref _IsHeavyTemplate_btn, "IsHeavyTemplate");
            // is Flamboyant Template
            DrawEquipmentFlagBool(ref equip.IsFlamboyantTemplate, ref _IsFlamboyantTemplate_btn, "IsFlamboyantTemplate");
            // is stoic template
            DrawEquipmentFlagBool(ref equip.IsStoicTemplate, ref _IsStoicTemplate_btn, "IsStoicTemplate");

            // is nomad template
            DrawEquipmentFlagBool(ref equip.IsNomadTemplate, ref _IsNomadTemplate_btn, "IsNomadTemplate");

            // IsChildTemplate
            DrawEquipmentFlagBool(ref equip.IsChildEquipmentTemplate, ref _IsChildTemplate_btn, "IsChildEquipmentTemplate");
            // IsWandererEquipment
            DrawEquipmentFlagBool(ref equip.IsWandererEquipment, ref _IsWandererEquipment_btn, "IsWandererEquipment");
            // IsGentryEquipment
            DrawEquipmentFlagBool(ref equip.IsGentryEquipment, ref _IsGentryEquipment_btn, "IsGentryEquipment");

            // IsRebelHeroEquipment
            DrawEquipmentFlagBool(ref equip.IsRebelHeroEquipment, ref _IsRebelHeroEquipment_btn, "IsRebelHeroEquipment");

            // IsTeenagerEquipmentTemplate
            DrawEquipmentFlagBool(ref equip.IsTeenagerEquipmentTemplate, ref _IsTeenagerEquipmentTemplate_btn, "IsTeenagerEquipmentTemplate");

            // IsEquipmentTemplate
            DrawEquipmentFlagBool(ref equip.IsEquipmentTemplate, ref _IsEquipmentTemplate_btn, "IsEquipmentTemplate");

            // IsWoodlandTemplate
            DrawEquipmentFlagBool(ref equip.IsWoodlandTemplate, ref _IsWoodlandTemplate_btn, "IsWoodlandTemplate");

            // IsFemaleTemplate
            DrawEquipmentFlagBool(ref equip.IsFemaleTemplate, ref _IsFemaleTemplate_btn, "IsFemaleTemplate");


            // IsEquipmentTemplate
            DrawEquipmentFlagBool(ref equip.IsCombatantTemplate, ref _IsCombatantTemplate_btn, "IsCombatantTemplate");

            // IsWoodlandTemplate
            DrawEquipmentFlagBool(ref equip.IsNoncombatantTemplate, ref _IsNoncombatantTemplate_btn, "IsNoncombatantTemplate");
            EditorGUIUtility.labelWidth = originLabelWidth;


            ///
            ///----- End toggles 
        }
        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.Space(4);

        //Draw Body Editor

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        //Color newCol;
        ColorUtility.TryParseHtmlString("#aca095", out newCol);
        titleStyle.normal.textColor = newCol;

        //GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

         showEditorLabel = "Hide - Equipment Editor";
        if (!showEquipmentEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Equipment Editor";
        }
        else
        {
            hiderStyle.fontSize = 10;
        }

        showEquipmentEditor = EditorGUILayout.Foldout(showEquipmentEditor, showEditorLabel, hiderStyle);

        if (showEquipmentEditor)
        {

            if (equip.eqpSetID != null && equip.eqpSetID.Length != 0)
            {
                int eqp_set_id = 0;
                hiderStyle = new GUIStyle(EditorStyles.foldout);

                foreach (var equip_set in equip.eqpSetID)
                {
                    // is civ
                    var path = dataPath + equip.moduleID + "/Sets/Equipments/EqpSets/" + equip_set + ".asset";
                    EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));

                    if (set != null)
                        EditorUtility.SetDirty(set);

                    showEditorLabel = "Hide";

                    if (!eqpSetID_bool[eqp_set_id])
                    {
                        hiderStyle.fontSize = 16;
                        showEditorLabel = "Roster Equipment - " + eqp_set_id;
                    }
                    else
                    {
                        hiderStyle.fontSize = 8;
                    }

                    DrawUILine(colUILine, 2, 4);

                    eqpSetID_bool[eqp_set_id] = EditorGUILayout.Foldout(eqpSetID_bool[eqp_set_id], showEditorLabel, hiderStyle);

                    if (eqpSetID_bool[eqp_set_id])
                    {
                        ResetItemSlots();

                        GetEquipmentSetAsset(ref equip.eqpSetID[eqp_set_id], ref set);

                        if (set != null)
                            EditorUtility.SetDirty(set);

                        Color savedColorObj = EditorStyles.objectField.normal.textColor;

                        EditorGUILayout.BeginHorizontal();

                        if (set.IsCivilianEquip)
                        {
                            ColorUtility.TryParseHtmlString("#7CBB00", out newCol);
                            titleStyle.normal.textColor = newCol;

                            titleStyle.fontSize = 14;
                            EditorGUILayout.LabelField("NPC Civilian Roster equipment - " + eqp_set_id, titleStyle, GUILayout.ExpandWidth(true));
                            // EditorGUILayout.Space(8);
                        }
                        else
                        {
                            ColorUtility.TryParseHtmlString("#ef5956", out newCol);
                            titleStyle.normal.textColor = newCol;

                            titleStyle.fontSize = 14;
                            EditorGUILayout.LabelField("NPC Battles Roster equipment - " + eqp_set_id, titleStyle, GUILayout.ExpandWidth(true));
                            // EditorGUILayout.Space(8);
                        }


                        EditorGUILayout.EndHorizontal();

                        DrawUILine(colUILine, 2, 4);

                        originLabelWidth = EditorGUIUtility.labelWidth;
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Civilian" + " "));

                        EditorGUIUtility.labelWidth = textDimensions.x;

                        set.IsCivilianEquip = EditorGUILayout.Toggle("Is Civilian", set.IsCivilianEquip);

                        EditorGUIUtility.labelWidth = originLabelWidth;

                        DrawUILine(colUILine, 2, 4);

                        EditorGUILayout.Space(3);
                        DrawUILine(colUILine, 2, 4);

                        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);

                        buttonStyle.fontStyle = FontStyle.Bold;
                        buttonStyle.hover.textColor = Color.red;

                        if (GUILayout.Button((new GUIContent("Remove", "Remove current equipment roster from current NPC node")), buttonStyle, GUILayout.Width(64)))
                        {
                            var deleteNaming = equip.eqpSetID[eqp_set_id];

                            equip.eqpSetID[eqp_set_id] = "REMOVE_TEMP";

                            var templist = new List<string>();

                            int i = 0;

                            foreach (var str in equip.eqpSetID)
                            {
                                if (str != "REMOVE_TEMP")
                                {
                                    templist.Add(str);
                                }
                                i++;

                            }

                            var del_path = dataPath + equip.moduleID + "/Sets/Equipments/EqpSets/" + deleteNaming + ".asset";
                            EquipmentSet del_set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(del_path, typeof(EquipmentSet));

                            var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + equip.moduleID + ".asset", typeof(ModuleReceiver));

                            mod.modFilesData.equipmentSetData.equipmentSets.Remove(del_set);

                            AssetDatabase.DeleteAsset(del_path);

                            equip.eqpSetID = new string[templist.Count];

                            for (i = 0; i < templist.Count; i++)
                            {
                                var ren_path = dataPath + equip.moduleID + "/Sets/Equipments/EqpSets/" + templist[i] + ".asset";
                                EquipmentSet ren_set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(ren_path, typeof(EquipmentSet));

                                var new_naming = equip.id + "_" + i;
                                AssetDatabase.RenameAsset(ren_path, new_naming);
                                ren_set.name = new_naming;

                                equip.eqpSetID[i] = new_naming;

                            }

                            // AssetDatabase.Refresh();

                            return;
                        }

                        EditorGUILayout.Space(4);

                        DrawUILine(colUILine, 2, 4);

                        EditorGUILayout.Space(8);

                        ColorUtility.TryParseHtmlString("#66757F", out newCol);
                        titleStyle.normal.textColor = newCol;


                        EditorGUILayout.LabelField("Weapon Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(16);
                        EditorGUILayout.BeginVertical();

                        DrawEquipmentSlot(ref set.eqp_Item0, ref slot_Item0, "Item0", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Item1, ref slot_Item1, "Item1", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Item2, ref slot_Item2, "Item2", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Item3, ref slot_Item3, "Item3", ref set.IsCivilianEquip);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 6);

                        EditorGUILayout.LabelField("Armor Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(16);
                        EditorGUILayout.BeginVertical();
                        DrawEquipmentSlot(ref set.eqp_Head, ref slot_Head, "Head", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Gloves, ref slot_Gloves, "Gloves", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Body, ref slot_Body, "Body", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Leg, ref slot_Leg, "Leg", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_Cape, ref slot_Cape, "Cape", ref set.IsCivilianEquip);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 6);

                        EditorGUILayout.LabelField("Horse Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(0);
                        EditorGUILayout.BeginVertical();
                        DrawEquipmentSlot(ref set.eqp_Horse, ref slot_Horse, "Horse", ref set.IsCivilianEquip);
                        DrawEquipmentSlot(ref set.eqp_HorseHarness, ref slot_HorseHarness, "HorseHarness", ref set.IsCivilianEquip);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 6);

                        EditorGUILayout.Space(4);

                    }

                    eqp_set_id++;


                }

                DrawUILine(colUILine, 2, 4);

                EditorGUILayout.Space(4);
                if (GUILayout.Button((new GUIContent("Add Equipment Roster", "Create Equipment Roster nodes in NPC Equipment root node")), GUILayout.Width(180)))
                {
                    var path_set = dataPath + equip.moduleID + "/Sets/Equipments/EqpSets/" + equip.id + "_" + eqp_set_id + ".asset";

                    EquipmentSet eqp_set = ScriptableObject.CreateInstance<EquipmentSet>();
                    eqp_set.moduleID = equip.moduleID;
                    eqp_set.EquipmentSetID = equip.id;

                    AssetDatabase.CreateAsset(eqp_set, path_set);

                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + equip.moduleID + ".asset", typeof(ModuleReceiver));
                    mod.modFilesData.equipmentSetData.equipmentSets.Add(eqp_set);

                    var eqp_rst_list = new List<string>();

                    for (int rst_id = 0; rst_id < equip.eqpSetID.Length; rst_id++)
                    {
                        eqp_rst_list.Add(equip.eqpSetID[rst_id]);
                    }

                    eqp_rst_list.Add(equip.id + "_" + eqp_set_id);

                    equip.eqpSetID = new string[eqp_rst_list.Count];

                    for (int rst_id = 0; rst_id < eqp_rst_list.Count; rst_id++)
                    {
                        equip.eqpSetID[rst_id] = equip.id + "_" + rst_id;
                    }

                    eqpSetID_bool = new bool[eqp_rst_list.Count];

                }
            }
            else
            {
                EditorGUILayout.HelpBox("This EQUIP is empty, want to add?", MessageType.Warning);
                EditorGUILayout.Space(4);
                if (GUILayout.Button((new GUIContent("Add Equipment Roster node", "Create Equipment Roster nodes in NPC Equipment root node")), GUILayout.Width(180)))
                {
                    var path_set = dataPath + equip.moduleID + "/Sets/Equipments/EqpSets/" + equip.id + "_0" + ".asset";

                    EquipmentSet eqp_set = ScriptableObject.CreateInstance<EquipmentSet>();
                    eqp_set._is_roster = true;
                    AssetDatabase.CreateAsset(eqp_set, path_set);

                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + equip.moduleID + ".asset", typeof(ModuleReceiver));
                    mod.modFilesData.equipmentSetData.equipmentSets.Add(eqp_set);

                    equip.eqpSetID = new string[1];
                    equip.eqpSetID[0] = equip.id + "_0";

                    eqpSetID_bool = new bool[1];

                }
            }
        }

        void ResetItemSlots()
        {
            slot_Item0 = null;
            slot_Item1 = null;
            slot_Item2 = null;
            slot_Item3 = null;
            slot_Body = null;
            slot_Cape = null;
            slot_Leg = null;
            slot_Gloves = null;
            slot_Head = null;
            slot_Horse = null;
            slot_HorseHarness = null;
        }


        // private void DrawEquipmentSetsEditor(ref string showEditorLabel, ref GUIStyle hiderStyle, ref GUIStyle titleStyle, ref Color newCol)
        // {
        //     GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        //     buttonStyle.fontStyle = FontStyle.Bold;
        //     buttonStyle.hover.textColor = Color.green;

        //     EditorGUILayout.BeginHorizontal();
        //     if (GUILayout.Button((new GUIContent("Add Equipment", "Add new equipment set")), buttonStyle, GUILayout.Width(128)))
        //     {
        //         var templist = new List<string>();
        //         var templistBool = new List<bool>();

        //         int i = 0;

        //         foreach (var str in npc.equipment_Set)
        //         {
        //             templist.Add(str);
        //             templistBool.Add(npc.equipment_Set_civilian_flag[i]);
        //             i++;
        //         }

        //         npc.equipment_Set = new string[templist.Count + 1];
        //         npc.equipment_Set_civilian_flag = new bool[templist.Count + 1];

        //         i = 0;
        //         foreach (var str in templist)
        //         {
        //             npc.equipment_Set[i] = str;
        //             npc.equipment_Set_civilian_flag[i] = templistBool[i];
        //             i++;
        //         }

        //         npc.equipment_Set[npc.equipment_Set.Length - 1] = "";
        //         npc.equipment_Set_civilian_flag[npc.equipment_Set_civilian_flag.Length - 1] = false;

        //         Equip = new Equipment[templist.Count + 1];
        //     }
        //     DrawUILineVertical(colUILine, 1, 1, 16);

        //     EditorGUILayout.EndHorizontal();

        //     DrawUILine(colUILine, 3, 12);

        //     if (npc.equipment_Set.Length != 0)
        //     {
        //         int eqp_i = 0;

        //         foreach (var equip in npc.equipment_Set)
        //         {
        //             GetEquipmentAsset(ref npc.equipment_Set[eqp_i], ref Equip[eqp_i]);

        //             Color savedColorObj = EditorStyles.objectField.normal.textColor;


        //             EditorGUILayout.BeginHorizontal();

        //             if (!npc.equipment_Set_civilian_flag[eqp_i])
        //             {
        //                 ColorUtility.TryParseHtmlString("#fdb94e", out newCol);
        //                 titleStyle.normal.textColor = newCol;

        //                 titleStyle.fontSize = 13;
        //                 EditorGUILayout.LabelField("Equipment " + eqp_i + " - Battle ", titleStyle, GUILayout.ExpandWidth(true));
        //                 // EditorGUILayout.Space(8);
        //             }
        //             else
        //             {
        //                 ColorUtility.TryParseHtmlString("#0af167", out newCol);
        //                 titleStyle.normal.textColor = newCol;

        //                 titleStyle.fontSize = 13;
        //                 EditorGUILayout.LabelField("Equipment " + eqp_i + " - Civilian ", titleStyle, GUILayout.ExpandWidth(true));
        //                 // EditorGUILayout.Space(8);
        //             }
        //             EditorGUILayout.EndHorizontal();
        //             EditorGUILayout.Space(4);

        //             buttonStyle.fontStyle = FontStyle.Bold;
        //             buttonStyle.hover.textColor = Color.red;

        //             if (GUILayout.Button((new GUIContent("Remove", "Remove current equipment from current NPC node")), buttonStyle, GUILayout.Width(64)))
        //             {
        //                 npc.equipment_Set[eqp_i] = "REMOVE_TEMP";

        //                 var templist = new List<string>();
        //                 var templistBool = new List<bool>();

        //                 int i = 0;

        //                 foreach (var str in npc.equipment_Set)
        //                 {
        //                     if (str != "REMOVE_TEMP")
        //                     {
        //                         templist.Add(str);
        //                         templistBool.Add(npc.equipment_Set_civilian_flag[i]);
        //                     }
        //                     i++;

        //                 }

        //                 npc.equipment_Set = new string[templist.Count];
        //                 npc.equipment_Set_civilian_flag = new bool[templist.Count];

        //                 i = 0;
        //                 foreach (var str in templist)
        //                 {
        //                     npc.equipment_Set[i] = str;
        //                     npc.equipment_Set_civilian_flag[i] = templistBool[i];
        //                     i++;
        //                 }

        //             }

        //             EditorGUILayout.Space(4);
        //             // npc.equipment_Set_civilian_flag[eqp_i] = GUILayout.Toggle(npc.equipment_Set_civilian_flag[eqp_i], "Is Civilian Equipment");
        //             // DrawUILine(colUILine, 1, 12);
        //             // EditorGUILayout.Space(8);


        //             ColorUtility.TryParseHtmlString("#66757F", out newCol);
        //             titleStyle.normal.textColor = newCol;
        //             titleStyle.fontSize = 12;


        //             GUILayout.BeginHorizontal();
        //             EditorGUILayout.LabelField("Equipment Roster:", titleStyle);
        //             object eqp = EditorGUILayout.ObjectField(Equip[eqp_i], typeof(Equipment), true);
        //             Equip[eqp_i] = (Equipment)eqp;

        //             if (Equip[eqp_i] != null)
        //             {
        //                 npc.equipment_Set[eqp_i] = Equip[eqp_i].ID;
        //                 npc.equipment_Set_civilian_flag[eqp_i] = Equip[eqp_i].IsCivilianTemplate;
        //             }
        //             else
        //             {
        //                 npc.equipment_Set[eqp_i] = "";
        //                 npc.equipment_Set_civilian_flag[eqp_i] = false;
        //             }

        //             GUILayout.EndHorizontal();

        //             // DrawUILine(colUILine, 3, 6);

        //             EditorGUILayout.Space(4);
        //             eqp_i++;
        //             DrawUILine(colUILine, 2, 4);
        //             EditorStyles.objectField.normal.textColor = savedColorObj;
        //             // }
        //         }
        //     }
        // }

        // private void DrawBodyEditor(GUIStyle titleStyle, bool hasMaxBP)
        // {
        //     Vector2 textDimensions;
        //     EditorGUILayout.LabelField("Body Properties:", titleStyle, GUILayout.ExpandWidth(true));
        //     EditorGUILayout.Space(2);
        //     textDimensions = GUI.skin.label.CalcSize(new GUIContent("Has Max Body Properties"));
        //     EditorGUIUtility.labelWidth = textDimensions.x;
        //     npc._Has_Max_BP = EditorGUILayout.Toggle("Has Max Body Properties", npc._Has_Max_BP);
        //     DrawUILine(colUILine, 1, 3);

        //     // npc.face_key_template = EditorGUILayout.TextField("Face Key Template: ", npc.face_key_template);

        //     GetNPCAsset(ref npc.face_key_template, ref FaceKeyTemplate, false);
        //     if (FaceKeyTemplate == null)
        //     {
        //         GetNPCAsset(ref npc.face_key_template, ref FaceKeyTemplate, true);
        //     }

        //     GUILayout.BeginHorizontal();
        //     EditorGUILayout.LabelField("Face Key Template:", EditorStyles.label);
        //     object faceKeyTemplateField = EditorGUILayout.ObjectField(FaceKeyTemplate, typeof(NPCCharacter), true);
        //     FaceKeyTemplate = (NPCCharacter)faceKeyTemplateField;

        //     if (FaceKeyTemplate != null)
        //     {
        //         npc.face_key_template = "NPCCharacter." + FaceKeyTemplate.id;
        //     }
        //     else
        //     {
        //         npc.face_key_template = "";
        //     }
        //     GUILayout.EndHorizontal();

        //     DrawUILine(colUILine, 1, 3);
        //     EditorGUILayout.Space(2);

        //     EditorGUILayout.BeginHorizontal();
        //     CreateIntAttribute(ref npc.BP_version, "Version:");
        //     DrawUILineVertical(colUILine, 1, 1, 16);
        //     CreateFloatAttribute(ref npc.BP_age, "Age:");
        //     DrawUILineVertical(colUILine, 1, 1, 16);
        //     CreateFloatAttribute(ref npc.BP_weight, "Weight:");
        //     DrawUILineVertical(colUILine, 1, 1, 16);
        //     CreateFloatAttribute(ref npc.BP_build, "Build:");
        //     EditorGUILayout.EndHorizontal();
        //     DrawUILine(colUILine, 1, 6);

        //     textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Key: "));
        //     EditorGUIUtility.labelWidth = textDimensions.x;

        //     npc.BP_key = EditorGUILayout.TextField("Body Key: ", npc.BP_key);

        //     EditorGUILayout.Space(2);

        //     if (hasMaxBP)
        //     {
        //         DrawUILine(colUILine, 3, 12);
        //         EditorGUILayout.LabelField("Body Properties Max:", titleStyle, GUILayout.ExpandWidth(true));
        //         EditorGUILayout.Space(2);

        //         EditorGUILayout.BeginHorizontal();
        //         CreateIntAttribute(ref npc.Max_BP_version, "Version:");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateFloatAttribute(ref npc.Max_BP_age, "Age:");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateFloatAttribute(ref npc.Max_BP_weight, "Weight:");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateFloatAttribute(ref npc.Max_BP_build, "Build:");
        //         EditorGUILayout.EndHorizontal();
        //         DrawUILine(colUILine, 1, 6);

        //         textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Key: "));
        //         EditorGUIUtility.labelWidth = textDimensions.x;

        //         npc.Max_BP_key = EditorGUILayout.TextField("Body Key: ", npc.BP_key);

        //         EditorGUILayout.Space(2);
        //     }

        // }

        // private void GetNPCAsset(ref string npcLink, ref NPCCharacter npcCharacter, bool npcTemplates)
        // {
        //     // Face Key Template template
        //     // 
        //     if (npcLink != null && npcLink != "")
        //     {
        //         if (npcLink.Contains("NPCCharacter."))
        //         {
        //             // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");

        //             string dataName = npcLink.Replace("NPCCharacter.", "");

        //             string assetPath;
        //             string assetPathShort;

        //             if (npcTemplates)
        //             {
        //                 assetPath = dataPath + npc.moduleID + "/_Templates/NPCtemplates/" + dataName + ".asset";
        //                 assetPathShort = "/_Templates/NPCtemplates/" + dataName + ".asset";
        //             }
        //             else
        //             {
        //                 assetPath = dataPath + npc.moduleID + "/NPC/" + dataName + ".asset";
        //                 assetPathShort = "/NPC/" + dataName + ".asset";
        //             }

        //             if (System.IO.File.Exists(assetPath))
        //             {
        //                 npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(NPCCharacter));
        //             }
        //             else
        //             {
        //                 // SEARCH IN DEPENDENCIES
        //                 string modSett = modsSettingsPath + npc.moduleID + ".asset";
        //                 ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

        //                 foreach (string dpdMod in currMod.modDependencies)
        //                 {
        //                     string dpdPath = modsSettingsPath + dpdMod + ".asset";

        //                     if (System.IO.File.Exists(dpdPath))
        //                     {
        //                         string dpdAsset = dataPath + dpdMod + assetPathShort;

        //                         if (System.IO.File.Exists(dpdAsset))
        //                         {
        //                             npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
        //                             break;
        //                         }
        //                         else
        //                         {
        //                             npcCharacter = null;
        //                         }

        //                     }
        //                 }

        //                 //Check is dependency OF
        //                 if (npcCharacter == null)
        //                 {
        //                     string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

        //                     foreach (string mod in mods)
        //                     {
        //                         ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

        //                         foreach (var depend in iSDependencyOfMod.modDependencies)
        //                         {
        //                             if (depend == npc.moduleID)
        //                             {
        //                                 foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
        //                                 {
        //                                     if (data != null)
        //                                         if (data.id == dataName)
        //                                         {
        //                                             string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
        //                                             npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
        //                                             break;
        //                                         }
        //                                 }
        //                             }
        //                         }
        //                     }

        //                     if (npcCharacter == null)
        //                     {
        //                         if (npcTemplates)
        //                         {
        //                             Debug.Log("NPCCharacter " + dataName + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");
        //                         }
        //                     }
        //                 }

        //             }
        //         }
        //     }
        // }

        // // Types
        // // 0 = main
        // // 1 = roster
        // // 2 = template
        void GetEquipmentSetAsset(ref string dataName, ref EquipmentSet equipmentSet)
        {
            // Face Key Template template
            // 
            if (dataName != null && dataName != "")
            {

                string assetPath = "";
                string assetPathShort = "";


                assetPath = dataPath + equip.moduleID + "/Sets/Equipments/EqpSets/" + dataName + ".asset";
                assetPathShort = "/Sets/Equipments/EqpSets/" + dataName + ".asset";



                if (System.IO.File.Exists(assetPath))
                {
                    equipmentSet = (EquipmentSet)AssetDatabase.LoadAssetAtPath(assetPath, typeof(EquipmentSet));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + equip.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + assetPathShort;

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                equipmentSet = (EquipmentSet)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(EquipmentSet));
                                break;
                            }
                            else
                            {
                                equipmentSet = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (equipmentSet == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == equip.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data != null)
                                            if (data.id == dataName)
                                            {
                                                string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                                equipmentSet = (EquipmentSet)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(EquipmentSet));
                                                break;
                                            }
                                    }
                                }
                            }
                        }

                        if (equipmentSet == null)
                        {

                            Debug.Log("EquipmentSet " + dataName + " - Not EXIST in" + " ' " + equip.moduleID + " ' " + "resources, and they dependencies.");

                        }
                    }

                }

            }
        }
        // private void GetEquipmentAsset(ref string dataName, ref Equipment equipment)
        // {
        //     // Face Key Template template
        //     // 
        //     if (dataName != null && dataName != "")
        //     {


        //         string assetPath = dataPath + npc.moduleID + "/Sets/Equipments/" + dataName + ".asset";
        //         string assetPathShort = "/Sets/Equipments/" + dataName + ".asset";


        //         if (System.IO.File.Exists(assetPath))
        //         {
        //             equipment = (Equipment)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Equipment));
        //         }
        //         else
        //         {
        //             // SEARCH IN DEPENDENCIES
        //             string modSett = modsSettingsPath + npc.moduleID + ".asset";
        //             ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

        //             foreach (string dpdMod in currMod.modDependencies)
        //             {
        //                 string dpdPath = modsSettingsPath + dpdMod + ".asset";

        //                 if (System.IO.File.Exists(dpdPath))
        //                 {
        //                     string dpdAsset = dataPath + dpdMod + assetPathShort;

        //                     if (System.IO.File.Exists(dpdAsset))
        //                     {
        //                         equipment = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Equipment));
        //                         break;
        //                     }
        //                     else
        //                     {
        //                         equipment = null;
        //                     }

        //                 }
        //             }

        //             //Check is dependency OF
        //             if (equipment == null)
        //             {
        //                 string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

        //                 foreach (string mod in mods)
        //                 {
        //                     ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

        //                     foreach (var depend in iSDependencyOfMod.modDependencies)
        //                     {
        //                         if (depend == npc.moduleID)
        //                         {
        //                             foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
        //                             {
        //                                 if (data != null)
        //                                     if (data.id == dataName)
        //                                     {
        //                                         string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
        //                                         equipment = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Equipment));
        //                                         break;
        //                                     }
        //                             }
        //                         }
        //                     }
        //                 }

        //                 if (equipment == null)
        //                 {

        //                     Debug.Log("Equipment " + dataName + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");

        //                 }
        //             }

        //         }

        //     }
        // }

        // // private void LoadSavedEquip(ref List<Dictionary<string, string>> objects, ref List<bool> bools)
        // // {
        // //     int i2 = 0;
        // //     foreach (var obj in objects)
        // //     {
        // //         objects.ToArray()[i2].TryGetValue("Item0", out npc.eqp_Item0[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Item1", out npc.eqp_Item1[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Item2", out npc.eqp_Item2[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Item3", out npc.eqp_Item3[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Body", out npc.eqp_Body[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Cape", out npc.eqp_Cape[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Leg", out npc.eqp_Leg[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Gloves", out npc.eqp_Gloves[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Head", out npc.eqp_Head[i2]);
        // //         objects.ToArray()[i2].TryGetValue("Horse", out npc.eqp_Horse[i2]);
        // //         objects.ToArray()[i2].TryGetValue("HorseHarness", out npc.eqp_HorseHarness[i2]);

        // //         npc.IsCivilianEquip[i2] = bools.ToArray()[i2];
        // //         i2++;
        // //     }
        // // }

        // // private void SaveRemovedEquip(ref List<Dictionary<string, string>> obj, ref string[] slotArray, ref List<bool> bools)
        // // {
        // //     int i2 = 0;

        // //     foreach (string slot in slotArray)
        // //     {
        // //         if (slot != "TEMP_REMOVE_SLOT_DATA")
        // //         {
        // //             var dict = new Dictionary<string, string>();

        // //             dict.Add("Item0", npc.eqp_Item0[i2]);
        // //             dict.Add("Item1", npc.eqp_Item1[i2]);
        // //             dict.Add("Item2", npc.eqp_Item2[i2]);
        // //             dict.Add("Item3", npc.eqp_Item3[i2]);
        // //             dict.Add("Body", npc.eqp_Body[i2]);
        // //             dict.Add("Cape", npc.eqp_Cape[i2]);
        // //             dict.Add("Leg", npc.eqp_Leg[i2]);
        // //             dict.Add("Gloves", npc.eqp_Gloves[i2]);
        // //             dict.Add("Head", npc.eqp_Head[i2]);
        // //             dict.Add("Horse", npc.eqp_Horse[i2]);
        // //             dict.Add("HorseHarness", npc.eqp_HorseHarness[i2]);

        // //             obj.Add(dict);
        // //             bools.Add(npc.IsCivilianEquip[i2]);
        // //             i2++;
        // //         }
        // //     }


        // // }

        void DrawEquipmentSlot(ref string Slot, ref Item item, string slotID, ref bool isCivilian)
        {

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.green;

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            // GUIStyle objField = new GUIStyle(EditorStyles.objectField);

            Color newCol2;
            ColorUtility.TryParseHtmlString("#F65314", out newCol2);


            var originDimensions = EditorGUIUtility.labelWidth;



            CheckItemSlot(ref Slot, ref item);


            titleStyle.fontSize = 12;
            titleStyle.normal.textColor = newCol2;

            if (item == null)
            {
                ColorUtility.TryParseHtmlString("#878f99", out newCol2);

                titleStyle.fontSize = 12;
                titleStyle.fontStyle = FontStyle.Normal;
                titleStyle.normal.textColor = newCol2;

                ColorUtility.TryParseHtmlString("#66757F", out newCol2);
                EditorStyles.objectField.normal.textColor = newCol2;
            }
            else
            {
                if (isCivilian)
                {
                    ColorUtility.TryParseHtmlString("#7CBB00", out newCol2);
                    titleStyle.normal.textColor = newCol2;
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#ef5956", out newCol2);
                    titleStyle.normal.textColor = newCol2;
                }
            }


            // ColorUtility.TryParseHtmlString("#F65314", out newCol2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (!isCivilian)
            {
                EditorGUILayout.LabelField("Slot - " + slotID, titleStyle, GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.LabelField("CivilianSlot - " + slotID, titleStyle, GUILayout.ExpandWidth(true));
            }
            if (item != null)
            {
                string soloName = item.itemName;

                if (soloName == null)
                {

                }
                else
                {
                    RemoveTSString(ref soloName);
                }


                if (isCivilian)
                {

                    ColorUtility.TryParseHtmlString("#34A853", out newCol2);
                    titleStyle.normal.textColor = newCol2;
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#f38654", out newCol2);
                    titleStyle.normal.textColor = newCol2;
                }
                EditorGUILayout.LabelField(soloName, titleStyle, GUILayout.ExpandWidth(true));
            }

            if (item != null)
            {
                switch (slotID)
                {
                    case "Item0":
                    case "Item1":
                    case "Item2":
                    case "Item3":
                        {
                            if (!item.IsWeapon)
                            {
                                if (!item.IsCraftedItem)
                                {
                                    item = null;
                                }
                            }


                        }
                        break;

                    case "Head":
                    case "Gloves":
                    case "Body":
                    case "Leg":
                    case "Cape":
                    case "HorseHarness":
                        {
                            if (!item.IsArmor)
                            {
                                item = null;
                            }
                        }
                        break;

                    case "Horse":
                        {
                            if (!item.IsHorse)
                            {
                                item = null;
                            }
                        }
                        break;

                    default:
                        {
                            item = null;
                        }
                        break;
                }
            }

            object itemObj = EditorGUILayout.ObjectField(item, typeof(Item), true, GUILayout.Width(256));
            EditorGUILayout.EndVertical();



            WriteSlotData(out Slot, out item, itemObj);

            EditorGUILayout.EndHorizontal();
            // WriteSlotData(out Slot, out item, itemObj);



            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(-2048);
            DrawUILine(colUILine, 1, 4);
            EditorGUILayout.EndHorizontal();



        }



        // // private void DrawEquipmentSlotDouble(ref string Slot, ref string civ_Slot, ref Item item, ref Item civ_item, string slotID, bool isCivilian)
        // // {
        // //     GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        // //     buttonStyle.fontStyle = FontStyle.Bold;
        // //     buttonStyle.hover.textColor = Color.green;

        // //     GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        // //     // GUIStyle objField = new GUIStyle(EditorStyles.objectField);

        // //     Color newCol2;
        // //     ColorUtility.TryParseHtmlString("#F65314", out newCol2);
        // //     Color savedColorObj = EditorStyles.objectField.normal.textColor;


        // //     var originDimensions = EditorGUIUtility.labelWidth;

        // //     CheckItemSlot(ref Slot, ref item);


        // //     titleStyle.fontSize = 12;
        // //     titleStyle.normal.textColor = newCol2;



        // //     if (item == null)
        // //     {
        // //         ColorUtility.TryParseHtmlString("#878f99", out newCol2);

        // //         titleStyle.fontSize = 12;
        // //         titleStyle.fontStyle = FontStyle.Normal;
        // //         titleStyle.normal.textColor = newCol2;

        // //         ColorUtility.TryParseHtmlString("#66757F", out newCol2);
        // //         EditorStyles.objectField.normal.textColor = newCol2;
        // //     }
        // //     else
        // //     {
        // //         ColorUtility.TryParseHtmlString("#F7F7F7", out newCol2);
        // //         EditorStyles.objectField.normal.textColor = newCol2;
        // //     }

        // //     // ColorUtility.TryParseHtmlString("#F65314", out newCol2);
        // //     EditorGUILayout.BeginHorizontal();
        // //     EditorGUILayout.BeginVertical();
        // //     EditorGUILayout.LabelField("Slot - " + slotID, titleStyle, GUILayout.ExpandWidth(true));
        // //     if (item != null)
        // //     {
        // //         string soloName = item.itemName;
        // //         RemoveTSString(ref soloName);

        // //         ColorUtility.TryParseHtmlString("#FFBB00", out newCol2);
        // //         titleStyle.normal.textColor = newCol2;

        // //         EditorGUILayout.LabelField(soloName, titleStyle, GUILayout.ExpandWidth(true));
        // //     }
        // //     object itemObj = EditorGUILayout.ObjectField(item, typeof(Item), true, GUILayout.Width(256));
        // //     EditorGUILayout.EndVertical();

        // //     EditorStyles.objectField.normal.textColor = savedColorObj;
        // //     if (isCivilian)
        // //     {
        // //         Color newCol3;
        // //         ColorUtility.TryParseHtmlString("#7CBB00", out newCol3);

        // //         GUIStyle titleStyle2 = new GUIStyle(EditorStyles.boldLabel);

        // //         titleStyle2.fontSize = 12;
        // //         titleStyle2.normal.textColor = newCol3;

        // //         CheckItemSlot(ref civ_Slot, ref civ_item);
        // //         if (civ_item == null)
        // //         {
        // //             ColorUtility.TryParseHtmlString("#878f99", out newCol2);
        // //             titleStyle2.fontSize = 12;
        // //             titleStyle2.fontStyle = FontStyle.Normal;
        // //             titleStyle2.normal.textColor = newCol2;

        // //             ColorUtility.TryParseHtmlString("#66757F", out newCol2);
        // //             EditorStyles.objectField.normal.textColor = newCol2;
        // //         }
        // //         else
        // //         {
        // //             ColorUtility.TryParseHtmlString("#F7F7F7", out newCol2);
        // //             EditorStyles.objectField.normal.textColor = newCol2;
        // //         }

        // //         DrawUILineVerticalLarge(colUILine, 16);
        // //         EditorGUILayout.Space(32);
        // //         EditorGUILayout.BeginVertical();
        // //         EditorGUILayout.LabelField("Civilian Slot - " + slotID, titleStyle2, GUILayout.ExpandWidth(true));
        // //         if (civ_item != null)
        // //         {
        // //             string soloName = civ_item.itemName;
        // //             RemoveTSString(ref soloName);

        // //             ColorUtility.TryParseHtmlString("#34A853", out newCol2);
        // //             titleStyle2.normal.textColor = newCol2;

        // //             EditorGUILayout.LabelField(soloName, titleStyle2, GUILayout.ExpandWidth(true));
        // //         }
        // //         object civ_itemObj = EditorGUILayout.ObjectField(civ_item, typeof(Item), true, GUILayout.Width(256));
        // //         EditorGUILayout.EndVertical();
        // //         WriteSlotData(out civ_Slot, out civ_item, civ_itemObj);

        // //         EditorStyles.objectField.normal.textColor = savedColorObj;
        // //     }

        // //     EditorGUILayout.EndHorizontal();
        // //     WriteSlotData(out Slot, out item, itemObj);


        // //     DrawUILine(colUILine, 3, 12);
        // // }

        void RemoveTSString(ref string inputString)
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

        void WriteSlotData(out string Slot, out Item item, object itemObj)
        {
            item = (Item)itemObj;

            if (item != null)
            {
                if (item.id != "")
                {
                    Slot = "Item." + item.id;
                }
                else
                {
                    Slot = "";
                }
            }
            else
            {
                Slot = "";
            }
        }

        void CheckItemSlot(ref string Slot, ref Item item)
        {
            if (Slot != null && Slot != "")
            {

                Slot = Slot.Replace("Item.", "");


                string modSettings = modsSettingsPath + equip.moduleID + ".asset";
                ModuleReceiver module = null;

                if (System.IO.File.Exists(modSettings))
                {
                    module = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSettings, typeof(ModuleReceiver));

                    foreach (Item itemAsset in module.modFilesData.itemsData.items)
                    {

                        if (itemAsset.id == Slot)
                        {
                            item = itemAsset;
                            break;
                        }
                    }

                    if (item == null)
                    {
                        foreach (var dependency in module.modDependenciesInternal)
                        {
                            modSettings = modsSettingsPath + dependency + ".asset";

                            if (System.IO.File.Exists(modSettings))
                            {
                                module = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSettings, typeof(ModuleReceiver));

                                foreach (Item itemAsset in module.modFilesData.itemsData.items)
                                {
                                    if (itemAsset.id == Slot)
                                    {
                                        // Debug.Log(itemAsset.id);
                                        item = itemAsset;
                                        break;
                                    }
                                }
                            }

                        }
                    }

                    if (item == null)
                    {

                        string[] AllModSettings = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (var mod in AllModSettings)
                        {
                            module = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            if (module.id != equip.moduleID)
                            {
                                foreach (var modDPD in module.modDependenciesInternal)
                                {
                                    if (modDPD == equip.moduleID)
                                    {
                                        if (module.modFilesData != null)
                                        {
                                            foreach (Item itemAsset in module.modFilesData.itemsData.items)
                                            {
                                                if (itemAsset.id == Slot)
                                                {
                                                    // Debug.Log(itemAsset.id);
                                                    item = itemAsset;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogWarning("Need to load ModFilesData");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (item == null)
                    {
                        Debug.Log("No Item Loaded " + Slot);
                    }
                }
            }
        }
    }

    private void DrawEquipmentFlagBool(ref bool equipmentFlag, ref bool equipmentFlagView, string equipmentFlagName)
    {
        if (equipmentFlag == true)
        {
            equipmentFlagView = true;
        }
        else
        {
            equipmentFlagView = false;
        }


        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent("IsTeenagerEquipmentTemplate "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        equipmentFlagView = EditorGUILayout.Toggle(equipmentFlagName, equipmentFlagView);

        if (equipmentFlagView)
        {
            equipmentFlag = true;
        }
        else
        {
            equipmentFlag = false;
        }

        DrawUILine(colUILine, 1, 2);

    }

    // void DrawUpgradeTargetsEditor()
    // {
    //     Vector2 textDimensions;
    //     GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
    //     buttonStyle.fontStyle = FontStyle.Bold;
    //     buttonStyle.hover.textColor = Color.green;

    //     GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
    //     titleStyle.fontSize = 16;

    //     Color newCol;
    //     ColorUtility.TryParseHtmlString("#FFB900", out newCol);
    //     Color newCol2;
    //     ColorUtility.TryParseHtmlString("#66ccff", out newCol2);
    //     titleStyle.normal.textColor = newCol;

    //     GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
    //     hiderStyle.fontSize = 10;
    //     hiderStyle.normal.textColor = newCol;

    //     var originDimensions = EditorGUIUtility.labelWidth;

    //     textDimensions = GUI.skin.label.CalcSize(new GUIContent("Upgrade: "));
    //     EditorGUIUtility.labelWidth = textDimensions.x;

    //     var showEditorLabel = "Hide";
    //     if (!showUpgradeTargetsEditor)
    //     {
    //         hiderStyle.fontSize = 16;
    //         showEditorLabel = "Upgrade Editor";
    //     }

    //     showUpgradeTargetsEditor = EditorGUILayout.Foldout(showUpgradeTargetsEditor, showEditorLabel, hiderStyle);

    //     if (showUpgradeTargetsEditor)
    //     {
    //         EditorGUILayout.LabelField("Upgrade Editor", titleStyle, GUILayout.ExpandWidth(true));
    //         EditorGUILayout.Space(4);


    //         EditorGUILayout.LabelField("Requiered item for Update:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
    //         // EditorGUILayout.Space(2);

    //         upgdateRequieres_index = EditorGUILayout.Popup(upgdateRequieres_index, upgdateRequieres_options, GUILayout.Width(128));
    //         npc.upgrade_requires = "ItemCategory." + upgdateRequieres_options[upgdateRequieres_index];

    //         DrawUILine(colUILine, 1, 6);


    //         if (GUILayout.Button((new GUIContent("Add Target", "Add NPC upgrade target ")), buttonStyle, GUILayout.Width(128)))
    //         {
    //             var objects = new Dictionary<string, string>();

    //             int i2 = 0;
    //             foreach (string trg in npc.upgrade_targets)
    //             {
    //                 if (!objects.ContainsKey(trg))
    //                 {
    //                     objects.Add(trg, npc.upgrade_targets[i2]);
    //                     i2++;
    //                 }

    //             }

    //             int indexVal = objects.Count + 1;

    //             npc.upgrade_targets = new string[indexVal];

    //             i2 = 0;
    //             foreach (var element in objects)
    //             {
    //                 npc.upgrade_targets[i2] = element.Key;
    //                 i2++;
    //             }

    //             npc.upgrade_targets[npc.upgrade_targets.Length - 1] = "";

    //             UpgradeTargets = new NPCCharacter[npc.upgrade_targets.Length];

    //         }
    //         DrawUILine(colUILine, 3, 12);

    //         int i = 0;

    //         if (npc.upgrade_targets != null && npc.upgrade_targets.Length != 0)
    //         {
    //             foreach (var targetAsset in npc.upgrade_targets)
    //             {
    //                 // Debug.Log(UpgradeTargets.Length);
    //                 GetNPCAsset(ref npc.upgrade_targets[i], ref UpgradeTargets[i], false);
    //                 if (UpgradeTargets[i] == null)
    //                 {
    //                     GetNPCAsset(ref npc.upgrade_targets[i], ref UpgradeTargets[i], true);
    //                 }

    //                 // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


    //                 ColorUtility.TryParseHtmlString("#F25022", out newCol);
    //                 titleStyle.normal.textColor = newCol;

    //                 titleStyle.fontSize = 11;
    //                 EditorGUILayout.LabelField("Upgrade Target " + i, titleStyle, GUILayout.ExpandWidth(true));
    //                 // EditorGUILayout.Space(8);
    //                 ColorUtility.TryParseHtmlString("#FF9900", out newCol);
    //                 titleStyle.normal.textColor = newCol;

    //                 titleStyle.fontSize = 12;

    //                 string nameLabel = "None";
    //                 if (UpgradeTargets[i] != null)
    //                 {
    //                     nameLabel = UpgradeTargets[i].npcName;
    //                 }

    //                 RemoveTSString(ref nameLabel);

    //                 EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
    //                 // EditorGUILayout.Space(8);

    //                 EditorGUILayout.BeginHorizontal();
    //                 object UpgradeTargetField = EditorGUILayout.ObjectField(UpgradeTargets[i], typeof(NPCCharacter), true, GUILayout.MaxWidth(320));

    //                 buttonStyle.hover.textColor = Color.red;

    //                 if (GUILayout.Button((new GUIContent("X", "Remove Upgrade Target")), buttonStyle, GUILayout.Width(32)))
    //                 {
    //                     var objects = new Dictionary<string, string>();
    //                     npc.upgrade_targets[i] = "remove";

    //                     int i2 = 0;
    //                     foreach (string trg in npc.upgrade_targets)
    //                     {
    //                         if (trg != "remove")
    //                         {
    //                             objects.Add(trg, npc.upgrade_targets[i2]);
    //                         }
    //                         i2++;
    //                     }

    //                     npc.upgrade_targets = new string[objects.Count];

    //                     i2 = 0;
    //                     foreach (var obj in objects)
    //                     {
    //                         npc.upgrade_targets[i2] = obj.Key;
    //                         i2++;
    //                     }

    //                     UpgradeTargets = new NPCCharacter[npc.upgrade_targets.Length];

    //                 }

    //                 EditorGUILayout.EndHorizontal();

    //                 UpgradeTargets[i] = (NPCCharacter)UpgradeTargetField;

    //                 if (UpgradeTargets[i] != null)
    //                 {
    //                     npc.upgrade_targets[i] = "NPCCharacter." + UpgradeTargets[i].id;
    //                 }
    //                 else
    //                 {
    //                     npc.upgrade_targets[i] = "";
    //                 }
    //                 DrawUILine(colUILine, 1, 4);
    //                 i++;
    //             }
    //         }

    //     }
    // }
    // void DrawSkillsEditor()
    // {
    //     Vector2 textDimensions;
    //     GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
    //     buttonStyle.fontStyle = FontStyle.Bold;
    //     buttonStyle.hover.textColor = Color.green;

    //     GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
    //     titleStyle.fontSize = 16;

    //     Color newCol;
    //     ColorUtility.TryParseHtmlString("#33ccff", out newCol);
    //     Color newCol2;
    //     ColorUtility.TryParseHtmlString("#66ccff", out newCol2);
    //     titleStyle.normal.textColor = newCol;

    //     GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
    //     hiderStyle.fontSize = 10;
    //     hiderStyle.normal.textColor = newCol;

    //     var originDimensions = EditorGUIUtility.labelWidth;

    //     textDimensions = GUI.skin.label.CalcSize(new GUIContent("Skills: "));
    //     EditorGUIUtility.labelWidth = textDimensions.x;

    //     var showEditorLabel = "Hide";
    //     if (!showSkillsEditor)
    //     {
    //         hiderStyle.fontSize = 16;
    //         showEditorLabel = "Skills Editor";
    //     }

    //     showSkillsEditor = EditorGUILayout.Foldout(showSkillsEditor, showEditorLabel, hiderStyle);

    //     if (showSkillsEditor)
    //     {

    //         EditorGUILayout.LabelField("Skills Editor", titleStyle, GUILayout.ExpandWidth(true));
    //         DrawUILine(colUILine, 3, 12);


    //         if (npc.skills == null || npc.skills.Length < settingsAsset.SkillsDefinitions.Length)
    //         {
    //             EditorGUILayout.BeginHorizontal();

    //             skills_index = EditorGUILayout.Popup("Skills:", skills_index, skills_options, GUILayout.Width(192));
    //             // npc.skills[i] = skills_options[skills_index];


    //             // DrawUILine(colUILine, 3, 12);
    //             if (GUILayout.Button((new GUIContent("Add Skill", "Add selected Skill for this NPC Character")), buttonStyle, GUILayout.Width(128)))
    //             {

    //                 var objects = new Dictionary<string, string>();

    //                 int i2 = 0;
    //                 foreach (string skillAsset in npc.skills)
    //                 {
    //                     objects.Add(skillAsset, npc.skillValues[i2]);
    //                     i2++;
    //                 }

    //                 int indexVal = objects.Count + 1;

    //                 npc.skills = new string[indexVal];
    //                 npc.skillValues = new string[indexVal];

    //                 i2 = 0;
    //                 foreach (var element in objects)
    //                 {
    //                     npc.skills[i2] = element.Key;
    //                     npc.skillValues[i2] = element.Value;
    //                     i2++;
    //                 }

    //                 npc.skills[npc.skills.Length - 1] = skills_options[skills_index];
    //                 npc.skillValues[npc.skillValues.Length - 1] = "0";

    //                 CreateSkillsOptions(ref skills_options, ref skills_index, settingsAsset.SkillsDefinitions);
    //             }

    //             EditorGUILayout.EndHorizontal();

    //             EditorGUILayout.Space(4);
    //             // DrawUILine(colUILine, 3, 12);
    //         }

    //         buttonStyle.fontStyle = FontStyle.Bold;
    //         buttonStyle.hover.textColor = Color.red;

    //         int i = 0;
    //         if (npc.skills != null && npc.skills.Length != 0)
    //         {
    //             foreach (var skill in npc.skills)
    //             {
    //                 titleStyle.fontSize = 13;
    //                 titleStyle.normal.textColor = newCol2;

    //                 EditorGUILayout.LabelField(npc.skills[i], titleStyle, GUILayout.ExpandWidth(true));

    //                 EditorGUILayout.BeginHorizontal();
    //                 CreateIntAttribute(ref npc.skillValues[i], "Skill Value:");

    //                 if (GUILayout.Button((new GUIContent("X", "Remove Skill")), buttonStyle, GUILayout.Width(32)))
    //                 {
    //                     var objects = new Dictionary<string, string>();
    //                     npc.skills[i] = "remove";
    //                     npc.skillValues[i] = "";

    //                     int i2 = 0;
    //                     foreach (string skillAsset in npc.skills)
    //                     {
    //                         if (skillAsset != "remove")
    //                         {
    //                             objects.Add(skillAsset, npc.skillValues[i2]);
    //                         }
    //                         i2++;
    //                     }

    //                     npc.skills = new string[objects.Count];
    //                     npc.skillValues = new string[objects.Count];

    //                     i2 = 0;
    //                     foreach (var obj in objects)
    //                     {
    //                         npc.skillValues[i2] = obj.Value;
    //                         npc.skills[i2] = obj.Key;
    //                         i2++;
    //                     }
    //                     CreateSkillsOptions(ref skills_options, ref skills_index, settingsAsset.SkillsDefinitions);
    //                 }

    //                 EditorGUILayout.EndHorizontal();

    //                 DrawUILine(colUILine, 3, 12);
    //                 i++;
    //             }
    //         }
    //     }
    // }
    // void DrawTraitsEditor()
    // {
    //     Vector2 textDimensions;
    //     GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
    //     buttonStyle.fontStyle = FontStyle.Bold;
    //     buttonStyle.hover.textColor = Color.green;

    //     GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
    //     titleStyle.fontSize = 16;

    //     Color newCol;
    //     ColorUtility.TryParseHtmlString("#ff9933", out newCol);
    //     Color newCol2;
    //     ColorUtility.TryParseHtmlString("#ff9966", out newCol2);
    //     titleStyle.normal.textColor = newCol;

    //     GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
    //     hiderStyle.fontSize = 10;
    //     hiderStyle.normal.textColor = newCol;

    //     var originDimensions = EditorGUIUtility.labelWidth;

    //     textDimensions = GUI.skin.label.CalcSize(new GUIContent("Traits: "));
    //     EditorGUIUtility.labelWidth = textDimensions.x;

    //     var showEditorLabel = "Hide";
    //     if (!showTraitsEditor)
    //     {
    //         hiderStyle.fontSize = 16;
    //         showEditorLabel = "Traits Editor";
    //     }

    //     showTraitsEditor = EditorGUILayout.Foldout(showTraitsEditor, showEditorLabel, hiderStyle);

    //     if (showTraitsEditor)
    //     {

    //         EditorGUILayout.LabelField("Traits Editor", titleStyle, GUILayout.ExpandWidth(true));
    //         DrawUILine(colUILine, 3, 12);


    //         if (npc.traits == null || npc.traits.Length < settingsAsset.TraitsDefinitions.Length)
    //         {
    //             EditorGUILayout.BeginHorizontal();

    //             traits_index = EditorGUILayout.Popup("Traits:", traits_index, traits_options, GUILayout.Width(192));
    //             // npc.skills[i] = skills_options[skills_index];


    //             // DrawUILine(colUILine, 3, 12);
    //             if (GUILayout.Button((new GUIContent("Add Trait", "Add selected Trait for this NPC Character")), buttonStyle, GUILayout.Width(128)))
    //             {

    //                 var objects = new Dictionary<string, string>();

    //                 int i2 = 0;
    //                 foreach (string traitAsset in npc.traits)
    //                 {
    //                     objects.Add(traitAsset, npc.traitValues[i2]);
    //                     i2++;
    //                 }

    //                 int indexVal = objects.Count + 1;

    //                 npc.traits = new string[indexVal];
    //                 npc.traitValues = new string[indexVal];

    //                 i2 = 0;
    //                 foreach (var element in objects)
    //                 {
    //                     npc.traits[i2] = element.Key;
    //                     npc.traitValues[i2] = element.Value;
    //                     i2++;
    //                 }

    //                 npc.traits[npc.traits.Length - 1] = traits_options[traits_index];
    //                 npc.traitValues[npc.traitValues.Length - 1] = "0";

    //                 CreateTraisOptions(ref traits_options, ref traits_index, settingsAsset.TraitsDefinitions);

    //             }

    //             EditorGUILayout.EndHorizontal();

    //             EditorGUILayout.Space(4);
    //             DrawUILine(colUILine, 3, 12);
    //         }

    //         buttonStyle.fontStyle = FontStyle.Bold;
    //         buttonStyle.hover.textColor = Color.red;

    //         int i = 0;
    //         if (npc.traits != null && npc.traits.Length != 0)
    //         {
    //             foreach (var trait in npc.traits)
    //             {

    //                 titleStyle.fontSize = 13;
    //                 titleStyle.normal.textColor = newCol2;

    //                 EditorGUILayout.LabelField(npc.traits[i], titleStyle, GUILayout.ExpandWidth(true));

    //                 EditorGUILayout.BeginHorizontal();
    //                 CreateIntAttribute(ref npc.traitValues[i], "Trait Value:");

    //                 if (GUILayout.Button((new GUIContent("X", "Remove Trait")), buttonStyle, GUILayout.Width(32)))
    //                 {
    //                     var objects = new Dictionary<string, string>();
    //                     npc.traits[i] = "remove";
    //                     npc.traitValues[i] = "";

    //                     int i2 = 0;
    //                     foreach (string traitAsset in npc.traits)
    //                     {
    //                         if (traitAsset != "remove")
    //                         {
    //                             objects.Add(traitAsset, npc.traitValues[i2]);
    //                         }
    //                         i2++;
    //                     }

    //                     npc.traits = new string[objects.Count];
    //                     npc.traitValues = new string[objects.Count];

    //                     i2 = 0;
    //                     foreach (var obj in objects)
    //                     {
    //                         npc.traits[i2] = obj.Key;
    //                         npc.traitValues[i2] = obj.Value;
    //                         i2++;
    //                     }
    //                     CreateTraisOptions(ref traits_options, ref traits_index, settingsAsset.TraitsDefinitions);

    //                 }

    //                 EditorGUILayout.EndHorizontal();

    //                 DrawUILine(colUILine, 3, 12);
    //                 i++;
    //             }
    //         }
    //     }
    // }

    // void CreateIntAttribute(ref string attribute, string label)
    // {

    //     Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
    //     EditorGUIUtility.labelWidth = textDimensions.x;

    //     int val;
    //     int.TryParse(attribute, out val);
    //     val = EditorGUILayout.IntField(label, val, GUILayout.MaxWidth(162));
    //     attribute = val.ToString();

    // }
    // void CreateFloatAttribute(ref string attribute, string label)
    // {

    //     Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
    //     EditorGUIUtility.labelWidth = textDimensions.x;

    //     float val;
    //     float.TryParse(attribute, out val);
    //     val = EditorGUILayout.FloatField(label, val, GUILayout.MaxWidth(162));
    //     attribute = val.ToString();

    // }
    // private void CreateSkillsOptions(ref string[] options, ref int index, string[] definitions)
    // {
    //     //WPN CLASS
    //     if (npc != null)
    //     {

    //         var listOptionsAll = new List<string>();
    //         var listOptionsLoaded = new List<string>();

    //         foreach (var data in definitions)
    //         {
    //             listOptionsAll.Add(data);

    //         }

    //         if (npc.skills != null && npc.skills.Length != 0)
    //         {
    //             foreach (var dataSkill in npc.skills)
    //             {
    //                 listOptionsLoaded.Add(dataSkill);
    //             }
    //         }

    //         foreach (var option in listOptionsLoaded)
    //         {
    //             if (listOptionsAll.Contains(option))
    //             {
    //                 listOptionsAll.Remove(option);
    //             }
    //         }

    //         // listOptionsAll.Add(typeString);

    //         options = new string[listOptionsAll.Count];

    //         int i = 0;
    //         foreach (var element in listOptionsAll)
    //         {
    //             options[i] = element;
    //             i++;
    //             // Debug.Log(element);
    //         }

    //         index = 0;
    //         // i = 0;
    //         // foreach (var type in options)
    //         // {
    //         //     if (type == typeString)
    //         //     {
    //         //         // Debug.Log("");
    //         //         index = i;
    //         //     }
    //         //     i++;
    //         // }
    //     }
    // }
    // private void CreateTraisOptions(ref string[] options, ref int index, string[] definitions)
    // {
    //     //WPN CLASS


    //     var listOptionsAll = new List<string>();
    //     var listOptionsLoaded = new List<string>();

    //     foreach (var data in definitions)
    //     {
    //         listOptionsAll.Add(data);

    //     }

    //     if (npc.traits != null && npc.traits.Length != 0)
    //     {
    //         foreach (var DataTrait in npc.traits)
    //         {
    //             listOptionsLoaded.Add(DataTrait);
    //         }
    //     }

    //     foreach (var option in listOptionsLoaded)
    //     {
    //         if (listOptionsAll.Contains(option))
    //         {
    //             listOptionsAll.Remove(option);
    //         }
    //     }

    //     options = new string[listOptionsAll.Count];

    //     int i = 0;
    //     foreach (var element in listOptionsAll)
    //     {
    //         options[i] = element;
    //         i++;

    //     }

    //     index = 0;

    // }
    // private void CreateOptions(ref string typeString, ref string[] options, ref int index, string[] definitions)
    // {
    //     //WPN CLASS
    //     options = new string[definitions.Length];

    //     int i = 0;
    //     foreach (var data in definitions)
    //     {
    //         options[i] = data;
    //         // Debug.Log("");
    //         i++;
    //     }

    //     i = 0;
    //     foreach (var type in options)
    //     {
    //         if (type == typeString)
    //         {
    //             // Debug.Log("");
    //             index = i;
    //         }
    //         i++;
    //     }
    // }

    // private void SetLabelFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel)
    // {

    //     bool isDependencyTS = false;
    //     var labelStyle = new GUIStyle(EditorStyles.label);
    //     if (soloString == null || soloString == "")
    //     {
    //         EditorGUILayout.HelpBox(typeName + " field is empty", MessageType.Error);
    //     }


    //     if (name != null && inputString != null && inputString != "")
    //     {
    //         soloString = inputString;
    //         TS_Name = inputString;
    //         Regex regex = new Regex("{=(.*)}");
    //         if (regex != null)
    //         {
    //             var v = regex.Match(TS_Name);
    //             string s = v.Groups[1].ToString();
    //             TS_Name = "{=" + s + "}";
    //         }

    //         if (TS_Name != "" && TS_Name != "{=}")
    //         {
    //             soloString = soloString.Replace(TS_Name, "");

    //             string TSasset = (dataPath + moduleID + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset");

    //             if (System.IO.File.Exists(TSasset))
    //             {
    //                 TS = (TranslationString)AssetDatabase.LoadAssetAtPath(TSasset, typeof(TranslationString));
    //             }
    //             else
    //             {

    //                 // SEARCH IN DEPENDENCIES
    //                 string modSett = modsSettingsPath + moduleID + ".asset";

    //                 ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

    //                 foreach (string dpdMod in currMod.modDependencies)
    //                 {
    //                     string dpdPath = modsSettingsPath + dpdMod + ".asset";

    //                     if (System.IO.File.Exists(dpdPath))
    //                     {

    //                         string dpdTSAsset = dataPath + dpdMod + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";

    //                         if (System.IO.File.Exists(dpdTSAsset))
    //                         {
    //                             TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdTSAsset, typeof(TranslationString));
    //                             isDependencyTS = true;
    //                             break;
    //                         }
    //                         else
    //                         {
    //                             TS = null;
    //                         }

    //                     }
    //                 }

    //                 //Check is dependency OF
    //                 if (TS == null)
    //                 {
    //                     string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

    //                     foreach (string mod in mods)
    //                     {
    //                         ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

    //                         foreach (var depend in iSDependencyOfMod.modDependencies)
    //                         {
    //                             if (depend == npc.moduleID)
    //                             {
    //                                 foreach (var data in iSDependencyOfMod.modFilesData.translationData.translationStrings)
    //                                 {
    //                                     if (data.id == TS_Name)
    //                                     {
    //                                         string dpdTSAsset = dataPath + iSDependencyOfMod.id + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";
    //                                         TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdTSAsset, typeof(TranslationString));
    //                                         break;
    //                                     }
    //                                 }
    //                             }
    //                         }
    //                     }

    //                     if (TS == null)
    //                     {
    //                         Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");
    //                     }

    //                 }
    //             }
    //         }
    //     }

    //     soloString = EditorGUILayout.TextField(typeName, soloString);

    //     // Draw UI - & Edit Translation String 
    //     EditorGUILayout.BeginHorizontal();
    //     // translationStringID = EditorGUILayout.TextField("Translation String", translationStringID);

    //     if (TS == null)
    //     {
    //         labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
    //         EditorGUILayout.LabelField("Translation String: (Unused)", labelStyle);
    //     }
    //     else
    //     {
    //         EditorGUILayout.LabelField("Translation String:", EditorStyles.label);
    //     }


    //     EditorGUILayout.ObjectField(TS, typeof(TranslationString), true);

    //     if (GUILayout.Button("Edit Translation"))
    //     {

    //         TranslationEditor transEditor = (TranslationEditor)EditorWindow.GetWindow(typeof(TranslationEditor), true, "Translation Editor");
    //         transEditor.defaultData = TS;
    //         transEditor.module = moduleID;
    //         transEditor.obj = obj;
    //         transEditor.objectID = objID;
    //         transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - )";

    //         transEditor.isDependency = isDependencyTS;

    //         transEditor.SearchStrings();
    //         transEditor.Show();
    //     }
    //     EditorGUILayout.EndHorizontal();
    //     // DrawUILine(colUILine, 3, 12);


    //     // Solo Name Check
    //     if (soloString != null && soloString != "")
    //     {
    //         if (TS_Name != "" && TS_Name != "{=}")
    //         {
    //             inputString = TS_Name + soloString;
    //         }
    //         else
    //         {
    //             inputString = soloString;
    //         }


    //     }

    //     if (soloString != null && soloString.Length <= 2)
    //     {
    //         // soloText = "";
    //         // translationString = "";
    //         if (TS != null)
    //         {
    //             inputString = TS.name + soloString;
    //         }
    //         else
    //         {
    //             inputString = soloString;
    //         }

    //     }

    //     if (TS != null && isDependencyTS == false)
    //     {
    //         TS.stringText = soloString;
    //     }

    // }

    /// Check Assets
    /// 

    public static AssetsDataManager ADM_Instance
    {
        get { return EditorWindow.GetWindow<AssetsDataManager>(); }
    }

    // UI DRAW TOOLS
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
    public static void DrawUILineVerticalLarge(Color color, int spaceX)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(3));
        r.height = 44;
        r.x += spaceX;
        r.y -= 10;
        r.height += 32;
        EditorGUI.DrawRect(r, color);
    }
}