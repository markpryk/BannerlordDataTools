using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

// [CanEditMultipleObjects]
[CustomEditor(typeof(NPCCharacter))]
public class NPCAssetEditor : Editor
{

    string dataPath = "Assets/Resources/Data/";
    string folder = "NPCTranslationData";

    //
    public string[] skills_options = new string[18];
    public int skills_index;
    public string[] traits_options = new string[18];
    public int traits_index;
    public string[] upgdateRequieres_options;
    public int upgdateRequieres_index;
    public string[] formationPosPref_options;
    public int formationPosPref_index;
    public string[] defaultGroups_options;
    public int defaultGroups_index;
    public string[] occupation_options;
    public int occupation_index;
    public string[] voice_options;
    public int voice_index;

    public bool[] roster_index_bool;

    //
    bool showSkillsEditor;
    bool showTraitsEditor;
    bool showEquipmentEditor;
    bool showUpgradeTargetsEditor;
    bool show_equip;
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

    //
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    string soloName;
    string nameTranslationString;
    TranslationString translationStringName;
    NPCCharacter npc;

    bool isBasicTroopBtn;
    bool isFemaleBtn;
    bool isHeroBtn;
    bool isCompanionBtn;
    bool isMercenaryBtn;
    bool isChildTmplBtn;
    bool isTmplBtn;
    public NPCCharacter skillTemplate;
    public NPCCharacter civilianTemplate;
    public NPCCharacter battleTemplate;
    public NPCCharacter FaceKeyTemplate;
    public NPCCharacter[] UpgradeTargets;
    public Culture cultureIs;
    public EquipmentSet mainEquipment;
    public EquipmentSet[] rosterEquipment;
    public Equipment[] Equip;
    public string[] equip_editor_options;
    public int equip_editor_index;
    Color bannerSymbolColor;

    // Update 1.7.2
    bool face_mesh_cacheBtn;
    bool is_hidden_encyclopediaBtn;
    bool is_obsoleteBtn;

    bool showHairTags;
    public string[] hair_tag_options;
    public int hair_tag_index;

    bool showBeardTags;
    public string[] beard_tag_options;
    public int beard_tag_index;

    bool isDependency = false;
    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";

    public void OnEnable()
    {
        npc = (NPCCharacter)target;
        EditorUtility.SetDirty(npc);

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


        CreateSkillsOptions(ref skills_options, ref skills_index, settingsAsset.SkillsDefinitions);
        CreateTraisOptions(ref traits_options, ref traits_index, settingsAsset.TraitsDefinitions);

        mainEquipment = new EquipmentSet();

        if (npc.equipment_Roster != null && npc.equipment_Roster.Length > 0)
            rosterEquipment = new EquipmentSet[npc.equipment_Roster.Length];

        if (npc.upgrade_targets != null && npc.upgrade_targets.Length != 0)
            UpgradeTargets = new NPCCharacter[npc.upgrade_targets.Length];

        upgdateRequieres_options = new string[settingsAsset.ItemCategoryDefinitions.Length];

        int i = 0;
        foreach (var category in settingsAsset.ItemCategoryDefinitions)
        {
            upgdateRequieres_options[i] = category;
            if (("ItemCategory." + upgdateRequieres_options[i]) == npc.upgrade_requires)
            {
                upgdateRequieres_index = i;
            }
            i++;
        }

        formationPosPref_options = new string[settingsAsset.FormationPosPrefDefinitions.Length];

        i = 0;
        foreach (var category in settingsAsset.FormationPosPrefDefinitions)
        {
            formationPosPref_options[i] = category;
            if ((formationPosPref_options[i]) == npc.formation_position_preference)
            {
                formationPosPref_index = i;
            }
            i++;
        }

        defaultGroups_options = new string[settingsAsset.NPCDefaultGroupsDefinitions.Length];

        i = 0;
        foreach (var category in settingsAsset.NPCDefaultGroupsDefinitions)
        {
            defaultGroups_options[i] = category;
            if ((defaultGroups_options[i]) == npc.default_group)
            {
                defaultGroups_index = i;
            }
            i++;
        }

        occupation_options = new string[settingsAsset.NPCOccupationDefinitions.Length];

        i = 0;
        foreach (var category in settingsAsset.NPCOccupationDefinitions)
        {
            occupation_options[i] = category;
            if ((occupation_options[i]) == npc.occupation)
            {
                occupation_index = i;
            }
            i++;
        }

        voice_options = new string[settingsAsset.NPCVoiceDefinitions.Length];

        i = 0;
        foreach (var category in settingsAsset.NPCVoiceDefinitions)
        {
            voice_options[i] = category;
            if ((voice_options[i]) == npc.voice)
            {
                voice_index = i;
            }
            i++;
        }

        equip_editor_options = new string[3];
        equip_editor_options[0] = "Main Equipment";
        equip_editor_options[1] = "Main Equipment Roster";
        equip_editor_options[2] = "Equipment Sets";

        equip_editor_index = 2;

        CreateHairsOptions(ref hair_tag_options, ref hair_tag_index, settingsAsset.HairTagDefinitions);
        CreateBeardTagOptions(ref beard_tag_options, ref beard_tag_index, settingsAsset.BeardTagDefinitions);


        if (npc.equipment_Set != null && npc.equipment_Set.Length > 0)
            Equip = new Equipment[npc.equipment_Set.Length];
        else
            Equip = new Equipment[0];

        if (rosterEquipment != null && rosterEquipment.Length > 0)
            roster_index_bool = new bool[rosterEquipment.Length];

    }

    private void ResetItemSlots()
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

    public override void OnInspectorGUI()
    {

        if (settingsAsset.currentModule != npc.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependencies)
                {
                    if (depend == npc.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + npc.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + npc.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + npc.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + npc.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + npc.moduleID + ".asset", typeof(ModuleReceiver));
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
                        assetMng.objID = 3;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = npc;

                        assetMng.assetName_org = npc.id;
                        assetMng.assetName_new = npc.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 3;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = npc;

                        assetMng.assetName_org = npc.id;
                        assetMng.assetName_new = npc.id;
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
        npc.id = EditorGUILayout.TextField("NPC ID", npc.id);

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {
            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 3;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = npc;

                assetMng.assetName_org = npc.id;
                assetMng.assetName_new = npc.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 3;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = npc;

                assetMng.assetName_org = npc.id;
                assetMng.assetName_new = npc.id;
            }
        }


        // npc.npcName = EditorGUILayout.TextField("NPC Name", npc.npcName);

        // NPC name & translationString tag
        DrawUILine(colUILine, 3, 12);

        // 3 NPC
        SetLabelFieldTS(ref npc.npcName, ref soloName, ref nameTranslationString, folder, translationStringName, "NPC Name:", npc.moduleID, npc, 3, npc.id);

        DrawUILine(colUILine, 3, 12);


        // BOOLEANS NPC

        EditorGUILayout.BeginHorizontal();
        var originLabelWidth = EditorGUIUtility.labelWidth;

        // is female
        if (npc.is_female != null)
        {
            if (npc.is_female == "true")
            {
                isFemaleBtn = true;
            }
            else
            {
                isFemaleBtn = false;
            }
        }


        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Female"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isFemaleBtn = EditorGUILayout.Toggle("Is Female", isFemaleBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (isFemaleBtn)
        {
            npc.is_female = "true";
        }
        else
        {
            npc.is_female = "false";
        }

        // is Basic Troop
        if (npc.is_basic_troop != null)
        {
            if (npc.is_basic_troop == "true")
            {
                isBasicTroopBtn = true;
            }
            else
            {
                isBasicTroopBtn = false;
            }
        }


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Basic Troop"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isBasicTroopBtn = EditorGUILayout.Toggle("Is Basic Troop", isBasicTroopBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (isBasicTroopBtn)
        {
            npc.is_basic_troop = "true";
        }
        else
        {
            npc.is_basic_troop = "false";
        }

        // is hero
        if (npc.is_hero != null)
        {
            if (npc.is_hero == "true")
            {
                isHeroBtn = true;
            }
            else
            {
                isHeroBtn = false;
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Hero"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isHeroBtn = EditorGUILayout.Toggle("Is Hero", isHeroBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (isHeroBtn)
        {
            npc.is_hero = "true";
        }
        else
        {
            npc.is_hero = "false";
        }

        // is companion
        if (npc.is_companion != null)
        {
            if (npc.is_companion == "true")
            {
                isCompanionBtn = true;
            }
            else
            {
                isCompanionBtn = false;
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Companion"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isCompanionBtn = EditorGUILayout.Toggle("Is Companion", isCompanionBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (isCompanionBtn)
        {
            npc.is_companion = "true";
        }
        else
        {
            npc.is_companion = "false";
        }
        EditorGUILayout.EndHorizontal();

        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.BeginHorizontal();
        // is mercenary
        if (npc.is_mercenary != null)
        {
            if (npc.is_mercenary == "true")
            {
                isMercenaryBtn = true;
            }
            else
            {
                isMercenaryBtn = false;
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Mercenary"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isMercenaryBtn = EditorGUILayout.Toggle("Is Mercenary", isMercenaryBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (isMercenaryBtn)
        {
            npc.is_mercenary = "true";
        }
        else
        {
            npc.is_mercenary = "false";
        }

        // is template
        if (npc.is_template != null)
        {
            if (npc.is_template == "true")
            {
                isTmplBtn = true;
            }
            else
            {
                isTmplBtn = false;
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Template"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isTmplBtn = EditorGUILayout.Toggle("Is Template", isTmplBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (isTmplBtn)
        {
            npc.is_template = "true";
        }
        else
        {
            npc.is_template = "false";
        }

        // is child template
        if (npc.is_child_template != null)
        {
            if (npc.is_child_template == "true")
            {
                isChildTmplBtn = true;
            }
            else
            {
                isChildTmplBtn = false;
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Child Template"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        isChildTmplBtn = EditorGUILayout.Toggle("Is Child Template", isChildTmplBtn);
        // DrawUILineVertical(colUILine, 1, 1, 16);
        // EditorGUILayout.Space(-5);

        if (isChildTmplBtn)
        {
            npc.is_child_template = "true";
        }
        else
        {
            npc.is_child_template = "false";
        }

        EditorGUIUtility.labelWidth = originLabelWidth;

        EditorGUILayout.EndHorizontal();
        DrawUILine(colUILine, 3, 12);
        GUILayout.BeginHorizontal();

        // face cache
        if (npc.face_mesh_cache == "true")
        {
            face_mesh_cacheBtn = true;
        }
        else
        {
            face_mesh_cacheBtn = false;
        }


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Face Mesh Cache "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        face_mesh_cacheBtn = EditorGUILayout.Toggle("Face Mesh Cache", face_mesh_cacheBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (face_mesh_cacheBtn)
        {
            npc.face_mesh_cache = "true";
        }
        else
        {
            npc.face_mesh_cache = "false";
        }

        // is_hidden_encyclopedia
        if (npc.is_hidden_encyclopedia == "true")
        {
            is_hidden_encyclopediaBtn = true;
        }
        else
        {
            is_hidden_encyclopediaBtn = false;
        }


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Hidden Encyclopedia "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        is_hidden_encyclopediaBtn = EditorGUILayout.Toggle("Is Hidden Encyclopedia", is_hidden_encyclopediaBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (is_hidden_encyclopediaBtn)
        {
            npc.is_hidden_encyclopedia = "true";
        }
        else
        {
            npc.is_hidden_encyclopedia = "false";
        }

        // is_obsolete
        if (npc.is_obsolete == "true")
        {
            is_obsoleteBtn = true;
        }
        else
        {
            is_obsoleteBtn = false;
        }


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Obsolete "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        is_obsoleteBtn = EditorGUILayout.Toggle("Is Obsolete", is_obsoleteBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        EditorGUILayout.Space(-5);

        if (is_obsoleteBtn)
        {
            npc.is_obsolete = "true";
        }
        else
        {
            npc.is_obsolete = "false";
        }

        EditorGUILayout.EndHorizontal();
        DrawUILine(colUILine, 3, 12);
        ///
        ///----- End toggles 


        GUILayout.BeginHorizontal();
        // AGE
        // npc.age = EditorGUILayout.TextField("Age", npc.age);
        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Age:         "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        int ageValue;

        int.TryParse(npc.age, out ageValue);

        ageValue = EditorGUILayout.IntField("Age:", ageValue, GUILayout.MaxWidth(96));

        npc.age = ageValue.ToString();

        EditorGUIUtility.labelWidth = originLabelWidth;

        GUILayout.Space(16);
        DrawUILineVertical(colUILine, 1, 1, 16);
        GUILayout.Space(16);
        /// age end


        // NPC Age
        // npc.level = EditorGUILayout.TextField("Level", npc.level);

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Level:       "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        int levelValue;

        int.TryParse(npc.level, out levelValue);

        levelValue = EditorGUILayout.IntField("Level:", levelValue, GUILayout.MaxWidth(96));

        npc.level = levelValue.ToString();

        EditorGUIUtility.labelWidth = originLabelWidth;

        GUILayout.Space(16);
        DrawUILineVertical(colUILine, 1, 1, 16);
        GUILayout.Space(16);

        // CULTURE
        if (npc.culture != null && npc.culture != "")
        {
            if (npc.culture.Contains("Culture."))
            {
                string dataName = npc.culture.Replace("Culture.", "");
                string asset = dataPath + npc.moduleID + "/Cultures/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(asset, typeof(Culture));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + npc.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
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

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == npc.moduleID)
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
                            Debug.Log("Culture " + dataName + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Culture:"));
        EditorGUIUtility.labelWidth = textDimensions.x - 32;

        EditorGUILayout.LabelField("Culture:", EditorStyles.label);
        object cultureField = EditorGUILayout.ObjectField(cultureIs, typeof(Culture), true, GUILayout.MaxWidth(192));
        cultureIs = (Culture)cultureField;

        if (cultureIs != null)
        {
            npc.culture = "Culture." + cultureIs.id;
        }
        else
        {
            npc.culture = "";
        }

        //CULTURE END
        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = originLabelWidth;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(8);
        DrawUILine(colUILine, 1, 4);

        EditorGUILayout.BeginHorizontal();

        textDimensions = GUI.skin.label.CalcSize(new GUIContent(":"));
        EditorGUIUtility.labelWidth = textDimensions.x;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Default Group:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        // EditorGUILayout.Space(2);
        defaultGroups_index = EditorGUILayout.Popup(defaultGroups_index, defaultGroups_options, GUILayout.Width(128));
        npc.default_group = defaultGroups_options[defaultGroups_index];
        EditorGUILayout.EndVertical();
        // DrawUILineVerticalLarge(colUILine, 1);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Occupation:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        // EditorGUILayout.Space(2);
        occupation_index = EditorGUILayout.Popup(occupation_index, occupation_options, GUILayout.Width(128));
        npc.occupation = occupation_options[occupation_index];
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Voice:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        // EditorGUILayout.Space(2);
        voice_index = EditorGUILayout.Popup(voice_index, voice_options, GUILayout.Width(128));
        npc.voice = voice_options[voice_index];
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originLabelWidth;

        if (npc.is_companion == "true")
        {

            DrawUILine(colUILine, 1, 6);
            EditorGUILayout.Space(2);
            npc.COMP_Companion = EditorGUILayout.TextField("Companion component ID:", npc.COMP_Companion);
        }


        // npc.default_group = EditorGUILayout.TextField("Default Group", npc.default_group);

        // npc.occupation = EditorGUILayout.TextField("Occupation", npc.occupation);

        // npc.voice = EditorGUILayout.TextField("Voice", npc.voice);

        DrawUILine(colUILine, 3, 12);

        // banner mesh
        npc.banner_symbol_mesh_name = EditorGUILayout.TextField("Banner symbol mesh name:", npc.banner_symbol_mesh_name);

        // Banner Colors
        // color banner primary
        var labelStyle = new GUIStyle(EditorStyles.boldLabel);

        if (npc.banner_symbol_color != null && npc.banner_symbol_color != "")
        {

            // FIX 0xff in color code (need to research)
            if (npc.banner_symbol_color.Contains("0xff"))
            {
                npc.banner_symbol_color = npc.banner_symbol_color.Replace("0xff", "FF");
                // Debug.Log(kingd.primary_banner_color);
            }

            ColorUtility.TryParseHtmlString("#" + npc.banner_symbol_color, out bannerSymbolColor);
            labelStyle.normal.textColor = new Color(bannerSymbolColor.r, bannerSymbolColor.g, bannerSymbolColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Banner Symbol Color:", labelStyle);
            bannerSymbolColor = EditorGUILayout.ColorField(bannerSymbolColor);
            GUILayout.EndHorizontal();
            npc.banner_symbol_color = ColorUtility.ToHtmlStringRGBA(bannerSymbolColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (bannerSymbolColor == new Color(0, 0, 0, 0))
            {
                npc.banner_symbol_color = "";
            }
        }
        else
        {
            if (bannerSymbolColor == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Banner Symbol Color: (Unused)", labelStyle);
                bannerSymbolColor = EditorGUILayout.ColorField(bannerSymbolColor);
                GUILayout.EndHorizontal();
                npc.banner_symbol_color = "";

                labelStyle = new GUIStyle(EditorStyles.boldLabel);
            }
            else
            {
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(bannerSymbolColor.r, bannerSymbolColor.g, bannerSymbolColor.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Banner Symbol Color:", labelStyle);
                bannerSymbolColor = EditorGUILayout.ColorField(bannerSymbolColor);
                GUILayout.EndHorizontal();
                npc.banner_symbol_color = ColorUtility.ToHtmlStringRGBA(bannerSymbolColor);
            }
        }

        DrawUILine(colUILine, 3, 12);

        // textDimensions = GUI.skin.label.CalcSize(new GUIContent("Formation Position Preference: "));
        // EditorGUIUtility.labelWidth = textDimensions.x;

        // npc.formation_position_preference = EditorGUILayout.TextField("Formation Position Preference:", npc.formation_position_preference);

        EditorGUILayout.LabelField("Formation Position Preference:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        // EditorGUILayout.Space(2);

        formationPosPref_index = EditorGUILayout.Popup(formationPosPref_index, formationPosPref_options, GUILayout.Width(128));
        npc.formation_position_preference = formationPosPref_options[formationPosPref_index];

        DrawUILine(colUILine, 1, 6);

        // SKill Template
        GetNPCAsset(ref npc.skill_template, ref skillTemplate, false);
        if (skillTemplate == null)
        {
            GetNPCAsset(ref npc.skill_template, ref skillTemplate, true);
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Skill Template:", EditorStyles.label);
        object skillTempField = EditorGUILayout.ObjectField(skillTemplate, typeof(NPCCharacter), true);
        skillTemplate = (NPCCharacter)skillTempField;

        if (skillTemplate != null)
        {
            npc.skill_template = "NPCCharacter." + skillTemplate.id;
        }
        else
        {
            npc.skill_template = "";
        }
        GUILayout.EndHorizontal();

        // Civilian Template template
        // 
        GetNPCAsset(ref npc.civilianTemplate, ref civilianTemplate, false);
        if (civilianTemplate == null)
        {
            GetNPCAsset(ref npc.civilianTemplate, ref civilianTemplate, true);
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Civilian Template:", EditorStyles.label);
        object civTempField = EditorGUILayout.ObjectField(civilianTemplate, typeof(NPCCharacter), true);
        civilianTemplate = (NPCCharacter)civTempField;

        if (civilianTemplate != null)
        {
            npc.civilianTemplate = "NPCCharacter." + civilianTemplate.id;
        }
        else
        {
            npc.civilianTemplate = "";
        }
        GUILayout.EndHorizontal();

        // Battle Template 
        // 
        GetNPCAsset(ref npc.battleTemplate, ref battleTemplate, false);
        if (battleTemplate == null)
        {
            GetNPCAsset(ref npc.battleTemplate, ref battleTemplate, true);
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Battle Template:", EditorStyles.label);
        object battTempField = EditorGUILayout.ObjectField(battleTemplate, typeof(NPCCharacter), true);
        battleTemplate = (NPCCharacter)battTempField;

        if (battleTemplate != null)
        {
            npc.battleTemplate = "NPCCharacter." + battleTemplate.id;
        }
        else
        {
            npc.battleTemplate = "";
        }
        GUILayout.EndHorizontal();
        // EditorGUI.EndDisabledGroup();
        DrawUILine(colUILine, 3, 12);



        //Draw Body Editor

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        Color newCol;
        ColorUtility.TryParseHtmlString("#aca095", out newCol);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        DrawBodyEditor(titleStyle, npc._Has_Max_BP);

        DrawUILine(colUILine, 3, 12);


        ColorUtility.TryParseHtmlString("#EA4335", out newCol);
        titleStyle.normal.textColor = newCol;
        titleStyle.fontSize = 16;

        hiderStyle.fontSize = 12;
        hiderStyle.normal.textColor = newCol;

        DrawHairTagsEditor();
        DrawUILine(colUILine, 3, 12);

        DrawBeardTagsEditor();
        DrawUILine(colUILine, 3, 12);

        DrawUpgradeTargetsEditor();

        DrawUILine(colUILine, 3, 12);

        DrawSkillsEditor();

        if (npc.skills != null && npc.skills.Length == 0 || showSkillsEditor == false)
        {
            DrawUILine(colUILine, 3, 12);
        }
        EditorGUILayout.Space(4);

        DrawTraitsEditor();

        if (!showTraitsEditor)
        {
            DrawUILine(colUILine, 3, 12);
        }
        EditorGUILayout.Space(4);

        var showEditorLabel = "Hide";
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

            EditorGUILayout.LabelField("Equipment Editor", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(2);
            // npc.IsCivilianEquip[0] = GUILayout.Toggle(npc.IsCivilianEquip[0], "Has Civilian Equipment");

            EditorGUILayout.BeginHorizontal();
            equip_editor_index = EditorGUILayout.Popup(equip_editor_index, equip_editor_options, GUILayout.Width(160));
            EditorGUILayout.EndHorizontal();
            DrawUILine(colUILine, 3, 12);


            switch (equip_editor_index)
            {
                case 0:

                    if (npc.equipment_Main != "")
                    {
                        ResetItemSlots();

                        GetEquipmentSetAsset(ref npc.equipment_Main, ref mainEquipment, 0);

                        if (mainEquipment != null)
                            EditorUtility.SetDirty(mainEquipment);

                        Color savedColorObj = EditorStyles.objectField.normal.textColor;

                        EditorGUILayout.BeginHorizontal();


                        ColorUtility.TryParseHtmlString("#0af167", out newCol);
                        titleStyle.normal.textColor = newCol;

                        titleStyle.fontSize = 14;
                        EditorGUILayout.LabelField("NPC Main Equipment", titleStyle, GUILayout.ExpandWidth(true));
                        // EditorGUILayout.Space(8);

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(8);

                        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);

                        buttonStyle.fontStyle = FontStyle.Bold;
                        buttonStyle.hover.textColor = Color.red;

                        if (GUILayout.Button((new GUIContent("Remove", "Remove Main equipment from current NPC node")), buttonStyle, GUILayout.Width(64)))
                        {

                            var del_path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentMain/" + npc.equipment_Main + ".asset";
                            EquipmentSet del_set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(del_path, typeof(EquipmentSet));

                            var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + npc.moduleID + ".asset", typeof(ModuleReceiver));
                            mod.modFilesData.equipmentSetData.equipmentSets.Remove(del_set);

                            npc.equipment_Main = "";

                            AssetDatabase.DeleteAsset(del_path);
                           // AssetDatabase.Refresh();

                            return;
                        }

                        DrawUILine(colUILine, 3, 12);

                        ColorUtility.TryParseHtmlString("#66757F", out newCol);
                        titleStyle.normal.textColor = newCol;


                        EditorGUILayout.LabelField("Weapon Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(16);
                        EditorGUILayout.BeginVertical();

                        DrawEquipmentSlot(ref mainEquipment.eqp_Item0, ref slot_Item0, "Item0", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Item1, ref slot_Item1, "Item1", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Item2, ref slot_Item2, "Item2", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Item3, ref slot_Item3, "Item3", ref mainEquipment.IsCivilianEquip);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 6);

                        EditorGUILayout.LabelField("Armor Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(16);
                        EditorGUILayout.BeginVertical();
                        DrawEquipmentSlot(ref mainEquipment.eqp_Head, ref slot_Head, "Head", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Gloves, ref slot_Gloves, "Gloves", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Body, ref slot_Body, "Body", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Leg, ref slot_Leg, "Leg", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_Cape, ref slot_Cape, "Cape", ref mainEquipment.IsCivilianEquip);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 6);

                        EditorGUILayout.LabelField("Horse Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(0);
                        EditorGUILayout.BeginVertical();
                        DrawEquipmentSlot(ref mainEquipment.eqp_Horse, ref slot_Horse, "Horse", ref mainEquipment.IsCivilianEquip);
                        DrawEquipmentSlot(ref mainEquipment.eqp_HorseHarness, ref slot_HorseHarness, "HorseHarness", ref mainEquipment.IsCivilianEquip);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 6);

                        EditorGUILayout.Space(4);

                        DrawUILine(colUILine, 2, 4);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("This NPC not contain equipment in main node, want to add?", MessageType.Warning);
                        EditorGUILayout.Space(4);
                        if (GUILayout.Button((new GUIContent("Add Main Equipment Nodes", "Create item nodes in NPC Equipment root node")), GUILayout.Width(180)))
                        {
                            var path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentMain/" + "eqp_main_" + npc.id + ".asset";

                            EquipmentSet equip = ScriptableObject.CreateInstance<EquipmentSet>();
                            equip._is_main = true;
                            AssetDatabase.CreateAsset(equip, path);

                            var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + npc.moduleID + ".asset", typeof(ModuleReceiver));
                            mod.modFilesData.equipmentSetData.equipmentSets.Add(equip);


                            npc.equipment_Main = "eqp_main_" + npc.id;


                        }
                    }

                    break;
                case 1:

                    DrawEquipmentRosters(ref titleStyle, ref newCol);
                    break;
                case 2:
                    DrawEquipmentSetsEditor(ref titleStyle, ref newCol);
                    break;
                default:
                    Debug.LogError("No Equipment editor Index");
                    break;
            }

        }
    }
    void DrawHairTagsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#ff4f81", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#ffaaaa", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Hair Tags Editor: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showHairTags)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Hair Tags Editor";
        }

        showHairTags = EditorGUILayout.Foldout(showHairTags, showEditorLabel, hiderStyle);

        if (showHairTags)
        {

            EditorGUILayout.LabelField("Hair Tags Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            if (npc.hair_tag == null || npc.hair_tag.Length < settingsAsset.HairTagDefinitions.Length)
            {
                EditorGUILayout.BeginHorizontal();

                hair_tag_index = EditorGUILayout.Popup("Hair Tags:", hair_tag_index, hair_tag_options, GUILayout.Width(320));
                // kingd.policies[i] = skills_options[skills_index];


                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Hair Tag", "Add selected hair tag to this Character")), buttonStyle, GUILayout.Width(128)))
                {

                    var objects = new List<string>();

                    int i2 = 0;
                    foreach (string tag in npc.hair_tag)
                    {
                        objects.Add(tag);
                        i2++;
                    }

                    int indexVal = objects.Count + 1;

                    npc.hair_tag = new string[indexVal];

                    i2 = 0;
                    foreach (var element in objects)
                    {
                        npc.hair_tag[i2] = element;
                        i2++;
                    }

                    npc.hair_tag[npc.hair_tag.Length - 1] = hair_tag_options[hair_tag_index];

                    CreateHairsOptions(ref hair_tag_options, ref hair_tag_index, settingsAsset.HairTagDefinitions);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                // DrawUILine(colUILine, 3, 12);
            }

            DrawUILine(colUILine, 1, 4);
            EditorGUILayout.Space(4);


            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (npc.hair_tag.Length != 0)
            {
                foreach (var tag in npc.hair_tag)
                {
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    //string soloPolicyName = cult.cultural_feat_id[i].Replace("policy_", "");
                    EditorGUILayout.LabelField(" Tag - " + npc.hair_tag[i], titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button((new GUIContent("X", "Remove Tag")), buttonStyle, GUILayout.Width(32)))
                    {
                        var objects = new List<string>();
                        npc.hair_tag[i] = "remove";

                        int i2 = 0;
                        foreach (string set in npc.hair_tag)
                        {
                            if (set != "remove")
                            {
                                objects.Add(set);
                            }
                            i2++;
                        }

                        npc.hair_tag = new string[objects.Count];

                        i2 = 0;
                        foreach (var obj in objects)
                        {
                            npc.hair_tag[i2] = obj;
                            i2++;
                        }
                        CreateHairsOptions(ref hair_tag_options, ref hair_tag_index, settingsAsset.HairTagDefinitions);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }
    private void CreateHairsOptions(ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS


        var listOptionsAll = new List<string>();
        var listOptionsLoaded = new List<string>();

        foreach (var data in definitions)
        {
            //string soloPolicyName = data.Replace("policy_", "");
            listOptionsAll.Add(data);

        }

        if (npc.hair_tag != null && npc.hair_tag.Length > 0)
        {
            foreach (var dataTrait in npc.hair_tag)
            {
                //string soloPolicyName = dataPolice.Replace("policy_", "");
                listOptionsLoaded.Add(dataTrait);
            }
        }


        foreach (var option in listOptionsLoaded)
        {
            if (listOptionsAll.Contains(option))
            {
                listOptionsAll.Remove(option);
            }
        }

        // listOptionsAll.Add(typeString);

        options = new string[listOptionsAll.Count];

        int i = 0;
        foreach (var element in listOptionsAll)
        {
            options[i] = element;
            i++;
            // Debug.Log(element);
        }

        index = 0;
        // i = 0;
        // foreach (var type in options)
        // {
        //     if (type == typeString)
        //     {
        //         // Debug.Log("");
        //         index = i;
        //     }
        //     i++;
        // }
    }

    void DrawBeardTagsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#49c0b6", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#00a0af", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Beard Tags Editor: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showBeardTags)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Beard Tags Editor";
        }

        showBeardTags = EditorGUILayout.Foldout(showBeardTags, showEditorLabel, hiderStyle);

        if (showBeardTags)
        {

            EditorGUILayout.LabelField("Beard Tags Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            if (npc.beard_tag == null || npc.beard_tag.Length < settingsAsset.BeardTagDefinitions.Length)
            {
                EditorGUILayout.BeginHorizontal();

                beard_tag_index = EditorGUILayout.Popup("Beard Tags:", beard_tag_index, beard_tag_options, GUILayout.Width(320));
                // kingd.policies[i] = skills_options[skills_index];


                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Beard Tag", "Add selected Beard Tag to this Character")), buttonStyle, GUILayout.Width(128)))
                {

                    var objects = new List<string>();

                    int i2 = 0;
                    foreach (string feat in npc.beard_tag)
                    {
                        objects.Add(feat);
                        i2++;
                    }

                    int indexVal = objects.Count + 1;

                    npc.beard_tag = new string[indexVal];

                    i2 = 0;
                    foreach (var element in objects)
                    {
                        npc.beard_tag[i2] = element;
                        i2++;
                    }

                    npc.beard_tag[npc.beard_tag.Length - 1] = beard_tag_options[beard_tag_index];

                    CreateBeardTagOptions(ref beard_tag_options, ref beard_tag_index, settingsAsset.BeardTagDefinitions);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                // DrawUILine(colUILine, 3, 12);
            }

            DrawUILine(colUILine, 1, 4);
            EditorGUILayout.Space(4);


            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (npc.beard_tag.Length != 0)
            {
                foreach (var feat in npc.beard_tag)
                {
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    //string soloPolicyName = cult.cultural_feat_id[i].Replace("policy_", "");
                    EditorGUILayout.LabelField("Tag - " + npc.beard_tag[i], titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button((new GUIContent("X", "Remove Tag")), buttonStyle, GUILayout.Width(32)))
                    {
                        var objects = new List<string>();
                        npc.beard_tag[i] = "remove";

                        int i2 = 0;
                        foreach (string skillAsset in npc.beard_tag)
                        {
                            if (skillAsset != "remove")
                            {
                                objects.Add(skillAsset);
                            }
                            i2++;
                        }

                        npc.beard_tag = new string[objects.Count];

                        i2 = 0;
                        foreach (var obj in objects)
                        {
                            npc.beard_tag[i2] = obj;
                            i2++;
                        }
                        CreateBeardTagOptions(ref beard_tag_options, ref beard_tag_index, settingsAsset.BeardTagDefinitions);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }
    private void CreateBeardTagOptions(ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS


        var listOptionsAll = new List<string>();
        var listOptionsLoaded = new List<string>();

        foreach (var data in definitions)
        {
            //string soloPolicyName = data.Replace("policy_", "");
            listOptionsAll.Add(data);

        }

        if (npc.beard_tag != null && npc.beard_tag.Length > 0)
        {
            foreach (var dataTrait in npc.beard_tag)
            {
                //string soloPolicyName = dataPolice.Replace("policy_", "");
                listOptionsLoaded.Add(dataTrait);
            }
        }


        foreach (var option in listOptionsLoaded)
        {
            if (listOptionsAll.Contains(option))
            {
                listOptionsAll.Remove(option);
            }
        }

        // listOptionsAll.Add(typeString);

        options = new string[listOptionsAll.Count];

        int i = 0;
        foreach (var element in listOptionsAll)
        {
            options[i] = element;
            i++;
            // Debug.Log(element);
        }

        index = 0;
        // i = 0;
        // foreach (var type in options)
        // {
        //     if (type == typeString)
        //     {
        //         // Debug.Log("");
        //         index = i;
        //     }
        //     i++;
        // }
    }
    void DrawEquipmentRosters(ref GUIStyle titleStyle, ref Color newCol)
    {

        if (npc.equipment_Roster.Length != 0)
        {
            int eqp_rost_i = 0;
            GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);

            foreach (var equip_rost in npc.equipment_Roster)
            {
                var showEditorLabel = "Hide";

                if (!roster_index_bool[eqp_rost_i])
                {
                    hiderStyle.fontSize = 16;
                    showEditorLabel = "Roster Equipment - " + eqp_rost_i;
                }
                else
                {
                    hiderStyle.fontSize = 8;
                }

                DrawUILine(colUILine, 2, 4);

                roster_index_bool[eqp_rost_i] = EditorGUILayout.Foldout(roster_index_bool[eqp_rost_i], showEditorLabel, hiderStyle);

                if (roster_index_bool[eqp_rost_i])
                {
                    ResetItemSlots();

                    GetEquipmentSetAsset(ref npc.equipment_Roster[eqp_rost_i], ref rosterEquipment[eqp_rost_i], 1);

                    if (rosterEquipment != null)
                        EditorUtility.SetDirty(rosterEquipment[eqp_rost_i]);

                    Color savedColorObj = EditorStyles.objectField.normal.textColor;

                    EditorGUILayout.BeginHorizontal();

                    if (rosterEquipment[eqp_rost_i].IsCivilianEquip)
                    {
                        ColorUtility.TryParseHtmlString("#7CBB00", out newCol);
                        titleStyle.normal.textColor = newCol;

                        titleStyle.fontSize = 14;
                        EditorGUILayout.LabelField("NPC Civilian Roster equipment - " + eqp_rost_i, titleStyle, GUILayout.ExpandWidth(true));
                        // EditorGUILayout.Space(8);
                    }
                    else
                    {
                        ColorUtility.TryParseHtmlString("#ef5956", out newCol);
                        titleStyle.normal.textColor = newCol;

                        titleStyle.fontSize = 14;
                        EditorGUILayout.LabelField("NPC Battles Roster equipment - " + eqp_rost_i, titleStyle, GUILayout.ExpandWidth(true));
                        // EditorGUILayout.Space(8);
                    }


                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 2, 4);
                    // is civ
                    var path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentRosters/" + "eqp_roster_" + npc.id + "_" + eqp_rost_i + ".asset";
                    EquipmentSet equip = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));

                    var originLabelWidth = EditorGUIUtility.labelWidth;
                    var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Civilian" + " "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    equip.IsCivilianEquip = EditorGUILayout.Toggle("Is Civilian", equip.IsCivilianEquip);

                    EditorGUIUtility.labelWidth = originLabelWidth;

                    EditorGUILayout.Space(3);
                    DrawUILine(colUILine, 2, 4);

                    GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);

                    buttonStyle.fontStyle = FontStyle.Bold;
                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("Remove", "Remove current equipment roster from current NPC node")), buttonStyle, GUILayout.Width(64)))
                    {

                        var deleteNaming = npc.equipment_Roster[eqp_rost_i];

                        npc.equipment_Roster[eqp_rost_i] = "REMOVE_TEMP";

                        var templist = new List<string>();

                        int i = 0;

                        foreach (var str in npc.equipment_Roster)
                        {
                            if (str != "REMOVE_TEMP")
                            {
                                templist.Add(str);
                            }
                            i++;

                        }

                        npc.equipment_Roster = new string[templist.Count];

                        i = 0;
                        foreach (var str in templist)
                        {
                            npc.equipment_Roster[i] = str;
                            i++;
                        }
                        //Debug.Log(deleteNaming);
                        var del_path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentRosters/" + deleteNaming + ".asset";
                        EquipmentSet del_set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(del_path, typeof(EquipmentSet));

                        var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + npc.moduleID + ".asset", typeof(ModuleReceiver));

                        mod.modFilesData.equipmentSetData.equipmentSets.Remove(del_set);

                        AssetDatabase.DeleteAsset(del_path);

                        npc.equipment_Roster = new string[templist.Count];

                        for (i = 0; i < templist.Count; i++)
                        {
                            var ren_path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentRosters/" + templist[i] + ".asset";
                            EquipmentSet ren_set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(ren_path, typeof(EquipmentSet));

                            var new_naming = "eqp_roster_" + npc.id + "_" + i;
                            AssetDatabase.RenameAsset(ren_path, new_naming);
                            ren_set.name = new_naming;

                            npc.equipment_Roster[i] = new_naming;

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

                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Item0, ref slot_Item0, "Item0", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Item1, ref slot_Item1, "Item1", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Item2, ref slot_Item2, "Item2", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Item3, ref slot_Item3, "Item3", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    DrawUILine(colUILine, 3, 6);

                    EditorGUILayout.LabelField("Armor Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                    EditorGUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(16);
                    EditorGUILayout.BeginVertical();
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Head, ref slot_Head, "Head", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Gloves, ref slot_Gloves, "Gloves", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Body, ref slot_Body, "Body", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Leg, ref slot_Leg, "Leg", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Cape, ref slot_Cape, "Cape", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    DrawUILine(colUILine, 3, 6);

                    EditorGUILayout.LabelField("Horse Slots: ", titleStyle, GUILayout.ExpandWidth(true));
                    EditorGUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(0);
                    EditorGUILayout.BeginVertical();
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_Horse, ref slot_Horse, "Horse", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    DrawEquipmentSlot(ref rosterEquipment[eqp_rost_i].eqp_HorseHarness, ref slot_HorseHarness, "HorseHarness", ref rosterEquipment[eqp_rost_i].IsCivilianEquip);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    DrawUILine(colUILine, 3, 6);

                    EditorGUILayout.Space(4);

                }

                eqp_rost_i++;

            }

            DrawUILine(colUILine, 2, 4);

            EditorGUILayout.Space(4);
            if (GUILayout.Button((new GUIContent("Add Equipment Roster", "Create Equipment Roster nodes in NPC Equipment root node")), GUILayout.Width(180)))
            {
                var path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentRosters/" + "eqp_roster_" + npc.id + "_" + eqp_rost_i + ".asset";

                EquipmentSet equip = ScriptableObject.CreateInstance<EquipmentSet>();
                equip._is_roster = true;
                AssetDatabase.CreateAsset(equip, path);

                var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + npc.moduleID + ".asset", typeof(ModuleReceiver));
                mod.modFilesData.equipmentSetData.equipmentSets.Add(equip);

                var eqp_rst_list = new List<string>();

                for (int rst_id = 0; rst_id < npc.equipment_Roster.Length; rst_id++)
                {
                    eqp_rst_list.Add(npc.equipment_Roster[rst_id]);
                }

                eqp_rst_list.Add("eqp_roster_" + npc.id + "_" + eqp_rost_i);

                npc.equipment_Roster = new string[eqp_rst_list.Count];

                for (int rst_id = 0; rst_id < eqp_rst_list.Count; rst_id++)
                {
                    npc.equipment_Roster[rst_id] = "eqp_roster_" + npc.id + "_" + rst_id;
                }

                roster_index_bool = new bool[eqp_rst_list.Count];
                rosterEquipment = new EquipmentSet[eqp_rst_list.Count];
            }
        }
        else
        {
            EditorGUILayout.HelpBox("This NPC not contain Equipment Roster in main node, want to add?", MessageType.Warning);
            EditorGUILayout.Space(4);
            if (GUILayout.Button((new GUIContent("Add Equipment Roster node", "Create Equipment Roster nodes in NPC Equipment root node")), GUILayout.Width(180)))
            {
                var path = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentRosters/" + "eqp_roster_" + npc.id + "_0" + ".asset";

                EquipmentSet equip = ScriptableObject.CreateInstance<EquipmentSet>();
                equip._is_roster = true;
                AssetDatabase.CreateAsset(equip, path);

                var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + npc.moduleID + ".asset", typeof(ModuleReceiver));
                mod.modFilesData.equipmentSetData.equipmentSets.Add(equip);

                npc.equipment_Roster = new string[1];
                npc.equipment_Roster[0] = "eqp_roster_" + npc.id + "_0";

                roster_index_bool = new bool[1];
                rosterEquipment = new EquipmentSet[1];


            }
        }



    }

    private void DrawEquipmentSetsEditor(ref GUIStyle titleStyle, ref Color newCol)
    {
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button((new GUIContent("Add Equipment", "Add new equipment set")), buttonStyle, GUILayout.Width(128)))
        {
            //var templist = new List<string>();
            //var templistBool = new List<bool>();

            //int i = 0;

            //foreach (var str in npc.equipment_Set)
            //{
            //    templist.Add(str);
            //    templistBool.Add(npc.equipment_Set_civilian_flag[i]);
            //    i++;
            //}

            //npc.equipment_Set = new string[templist.Count + 1];
            //npc.equipment_Set_civilian_flag = new bool[templist.Count + 1];

            //i = 0;
            //foreach (var str in templist)
            //{
            //    npc.equipment_Set[i] = str;
            //    npc.equipment_Set_civilian_flag[i] = templistBool[i];
            //    i++;
            //}

            //npc.equipment_Set[npc.equipment_Set.Length - 1] = "";
            //npc.equipment_Set_civilian_flag[npc.equipment_Set_civilian_flag.Length - 1] = false;


            var temp = new string[npc.equipment_Set.Length + 1];
            npc.equipment_Set.CopyTo(temp, 0);
            npc.equipment_Set = temp;

            npc.equipment_Set[npc.equipment_Set.Length - 1] = "";

            var temp_bool = new bool[npc.equipment_Set_civilian_flag.Length + 1];
            npc.equipment_Set_civilian_flag.CopyTo(temp_bool, 0);
            npc.equipment_Set_civilian_flag = temp_bool;

            npc.equipment_Set_civilian_flag[npc.equipment_Set_civilian_flag.Length - 1] = false;

            Equip = new Equipment[npc.equipment_Set.Length];
        }
        DrawUILineVertical(colUILine, 1, 1, 16);

        EditorGUILayout.EndHorizontal();

        DrawUILine(colUILine, 3, 12);

        if (npc.equipment_Set.Length != 0)
        {
            int eqp_i = 0;

            foreach (var equip in npc.equipment_Set)
            {
                GetEquipmentAsset(ref npc.equipment_Set[eqp_i], ref Equip[eqp_i]);

                Color savedColorObj = EditorStyles.objectField.normal.textColor;


                EditorGUILayout.BeginHorizontal();

                if (!npc.equipment_Set_civilian_flag[eqp_i])
                {
                    ColorUtility.TryParseHtmlString("#fdb94e", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 13;
                    EditorGUILayout.LabelField("Equipment " + eqp_i + " - Battle ", titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#0af167", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 13;
                    EditorGUILayout.LabelField("Equipment " + eqp_i + " - Civilian ", titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(4);

                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.hover.textColor = Color.red;

                if (GUILayout.Button((new GUIContent("Remove", "Remove current equipment from current NPC node")), buttonStyle, GUILayout.Width(64)))
                {
                    //npc.equipment_Set[eqp_i] = "REMOVE_TEMP";

                    //var templist = new List<string>();
                    //var templistBool = new List<bool>();

                    //int i = 0;

                    //foreach (var str in npc.equipment_Set)
                    //{
                    //    if (str != "REMOVE_TEMP")
                    //    {
                    //        templist.Add(str);
                    //        templistBool.Add(npc.equipment_Set_civilian_flag[i]);
                    //    }
                    //    i++;

                    //}

                    //npc.equipment_Set = new string[templist.Count];
                    //npc.equipment_Set_civilian_flag = new bool[templist.Count];

                    //i = 0;
                    //foreach (var str in templist)
                    //{
                    //    npc.equipment_Set[i] = str;
                    //    npc.equipment_Set_civilian_flag[i] = templistBool[i];
                    //    i++;
                    //}

                    var count = npc.equipment_Set.Length - 1;
                    var eqp_set = new string[count];
                    var eqp_set_value = new bool[count];

                    int i2 = 0;
                    int i3 = 0;
                    foreach (string trg in npc.equipment_Set)
                    {
                        if (i3 != eqp_i)
                        {
                            eqp_set[i2] = npc.equipment_Set[i3];
                            eqp_set_value[i2] = npc.equipment_Set_civilian_flag[i3];
                            i2++;
                        }
                        i3++;
                    }

                    npc.equipment_Set = eqp_set;
                    npc.equipment_Set_civilian_flag = eqp_set_value;

                    Equip = new Equipment[npc.equipment_Set.Length];

                    return;

                }

                EditorGUILayout.Space(4);
                // npc.equipment_Set_civilian_flag[eqp_i] = GUILayout.Toggle(npc.equipment_Set_civilian_flag[eqp_i], "Is Civilian Equipment");
                // DrawUILine(colUILine, 1, 12);
                // EditorGUILayout.Space(8);


                ColorUtility.TryParseHtmlString("#66757F", out newCol);
                titleStyle.normal.textColor = newCol;
                titleStyle.fontSize = 12;


                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Equipment Roster:", titleStyle);
                object eqp = EditorGUILayout.ObjectField(Equip[eqp_i], typeof(Equipment), true);
                Equip[eqp_i] = (Equipment)eqp;

                if (Equip[eqp_i] != null)
                {
                    npc.equipment_Set[eqp_i] = Equip[eqp_i].id;
                    npc.equipment_Set_civilian_flag[eqp_i] = Equip[eqp_i].IsCivilianTemplate;
                }
                else
                {
                    npc.equipment_Set[eqp_i] = "";
                    npc.equipment_Set_civilian_flag[eqp_i] = false;
                }

                GUILayout.EndHorizontal();

                // DrawUILine(colUILine, 3, 6);

                EditorGUILayout.Space(4);
                eqp_i++;
                DrawUILine(colUILine, 2, 4);
                EditorStyles.objectField.normal.textColor = savedColorObj;
                // }
            }
        }
    }

    private void DrawBodyEditor(GUIStyle titleStyle, bool hasMaxBP)
    {
        Vector2 textDimensions;
        EditorGUILayout.LabelField("Body Properties:", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(2);
        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Has Max Body Properties"));
        EditorGUIUtility.labelWidth = textDimensions.x;
        npc._Has_Max_BP = EditorGUILayout.Toggle("Has Max Body Properties", npc._Has_Max_BP);
        DrawUILine(colUILine, 1, 3);

        // npc.face_key_template = EditorGUILayout.TextField("Face Key Template: ", npc.face_key_template);

        GetNPCAsset(ref npc.face_key_template, ref FaceKeyTemplate, false);
        if (FaceKeyTemplate == null)
        {
            GetNPCAsset(ref npc.face_key_template, ref FaceKeyTemplate, true);
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Face Key Template:", EditorStyles.label);
        object faceKeyTemplateField = EditorGUILayout.ObjectField(FaceKeyTemplate, typeof(NPCCharacter), true);
        FaceKeyTemplate = (NPCCharacter)faceKeyTemplateField;

        if (FaceKeyTemplate != null)
        {
            npc.face_key_template = "NPCCharacter." + FaceKeyTemplate.id;
        }
        else
        {
            npc.face_key_template = "";
        }
        GUILayout.EndHorizontal();

        DrawUILine(colUILine, 1, 3);
        EditorGUILayout.Space(2);

        EditorGUILayout.BeginHorizontal();
        CreateIntAttribute(ref npc.BP_version, "Version:");
        DrawUILineVertical(colUILine, 1, 1, 16);
        CreateFloatAttribute(ref npc.BP_age, "Age:");
        DrawUILineVertical(colUILine, 1, 1, 16);
        CreateFloatAttribute(ref npc.BP_weight, "Weight:");
        DrawUILineVertical(colUILine, 1, 1, 16);
        CreateFloatAttribute(ref npc.BP_build, "Build:");
        EditorGUILayout.EndHorizontal();
        DrawUILine(colUILine, 1, 6);

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Key: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        npc.BP_key = EditorGUILayout.TextField("Body Key: ", npc.BP_key);

        EditorGUILayout.Space(2);

        if (hasMaxBP)
        {
            DrawUILine(colUILine, 3, 12);
            EditorGUILayout.LabelField("Body Properties Max:", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            CreateIntAttribute(ref npc.Max_BP_version, "Version:");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateFloatAttribute(ref npc.Max_BP_age, "Age:");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateFloatAttribute(ref npc.Max_BP_weight, "Weight:");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateFloatAttribute(ref npc.Max_BP_build, "Build:");
            EditorGUILayout.EndHorizontal();
            DrawUILine(colUILine, 1, 6);

            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Key: "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            npc.Max_BP_key = EditorGUILayout.TextField("Body Key: ", npc.BP_key);

            EditorGUILayout.Space(2);
        }

    }

    private void GetNPCAsset(ref string npcLink, ref NPCCharacter npcCharacter, bool npcTemplates)
    {
        // Face Key Template template
        // 
        if (npcLink != null && npcLink != "")
        {
            if (npcLink.Contains("NPCCharacter."))
            {
                // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");

                string dataName = npcLink.Replace("NPCCharacter.", "");

                string assetPath;
                string assetPathShort;

                if (npcTemplates)
                {
                    assetPath = dataPath + npc.moduleID + "/_Templates/NPCtemplates/" + dataName + ".asset";
                    assetPathShort = "/_Templates/NPCtemplates/" + dataName + ".asset";
                }
                else
                {
                    assetPath = dataPath + npc.moduleID + "/NPC/" + dataName + ".asset";
                    assetPathShort = "/NPC/" + dataName + ".asset";
                }

                if (System.IO.File.Exists(assetPath))
                {
                    npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(NPCCharacter));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + npc.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + assetPathShort;

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
                                break;
                            }
                            else
                            {
                                npcCharacter = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (npcCharacter == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == npc.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data != null)
                                            if (data.id == dataName)
                                            {
                                                string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                                npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
                                                break;
                                            }
                                    }
                                }
                            }
                        }

                        if (npcCharacter == null)
                        {
                            if (npcTemplates)
                            {
                                Debug.Log("NPCCharacter " + dataName + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");
                            }
                        }
                    }

                }
            }
        }
    }

    // Types
    // 0 = main
    // 1 = roster
    // 2 = template
    private void GetEquipmentSetAsset(ref string dataName, ref EquipmentSet equipmentSet, int type)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {

            string assetPath = "";
            string assetPathShort = "";

            switch (type)
            {
                case 0:
                    assetPath = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                    assetPathShort = "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                    break;
                case 1:
                    assetPath = dataPath + npc.moduleID + "/NPC/Equipment/EquipmentRosters/" + dataName + ".asset";
                    assetPathShort = "/NPC/Equipment/EquipmentRosters/" + dataName + ".asset";
                    break;
                case 2:
                    assetPath = dataPath + npc.moduleID + "/Sets/Equipments/EqpSets/" + dataName + ".asset";
                    assetPathShort = "/Sets/Equipments/EqpSets/" + dataName + ".asset";
                    break;
                default:
                    Debug.Log("No Equipment type assigned");
                    break;
            }


            if (System.IO.File.Exists(assetPath))
            {
                equipmentSet = (EquipmentSet)AssetDatabase.LoadAssetAtPath(assetPath, typeof(EquipmentSet));
            }
            else
            {
                // SEARCH IN DEPENDENCIES
                string modSett = modsSettingsPath + npc.moduleID + ".asset";
                ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                foreach (string dpdMod in currMod.modDependencies)
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

                        foreach (var depend in iSDependencyOfMod.modDependencies)
                        {
                            if (depend == npc.moduleID)
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

                        Debug.Log("EquipmentSet " + dataName + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");

                    }
                }

            }

        }
    }
    private void GetEquipmentAsset(ref string dataName, ref Equipment equipment)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {


            string assetPath = dataPath + npc.moduleID + "/Sets/Equipments/" + dataName + ".asset";
            string assetPathShort = "/Sets/Equipments/" + dataName + ".asset";


            if (System.IO.File.Exists(assetPath))
            {
                equipment = (Equipment)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Equipment));
            }
            else
            {
                // SEARCH IN DEPENDENCIES
                string modSett = modsSettingsPath + npc.moduleID + ".asset";
                ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                foreach (string dpdMod in currMod.modDependencies)
                {
                    string dpdPath = modsSettingsPath + dpdMod + ".asset";

                    if (System.IO.File.Exists(dpdPath))
                    {
                        string dpdAsset = dataPath + dpdMod + assetPathShort;

                        if (System.IO.File.Exists(dpdAsset))
                        {
                            equipment = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Equipment));
                            break;
                        }
                        else
                        {
                            equipment = null;
                        }

                    }
                }

                //Check is dependency OF
                if (equipment == null)
                {
                    string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                    foreach (string mod in mods)
                    {
                        ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                        foreach (var depend in iSDependencyOfMod.modDependencies)
                        {
                            if (depend == npc.moduleID)
                            {
                                foreach (var data in iSDependencyOfMod.modFilesData.equipmentsData.equipmentSets)
                                {
                                    if (data != null)
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                            equipment = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Equipment));
                                            break;
                                        }
                                }
                            }
                        }
                    }

                    if (equipment == null)
                    {

                        Debug.Log("Equipment " + dataName + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");

                    }
                }

            }

        }
    }

    // private void LoadSavedEquip(ref List<Dictionary<string, string>> objects, ref List<bool> bools)
    // {
    //     int i2 = 0;
    //     foreach (var obj in objects)
    //     {
    //         objects.ToArray()[i2].TryGetValue("Item0", out npc.eqp_Item0[i2]);
    //         objects.ToArray()[i2].TryGetValue("Item1", out npc.eqp_Item1[i2]);
    //         objects.ToArray()[i2].TryGetValue("Item2", out npc.eqp_Item2[i2]);
    //         objects.ToArray()[i2].TryGetValue("Item3", out npc.eqp_Item3[i2]);
    //         objects.ToArray()[i2].TryGetValue("Body", out npc.eqp_Body[i2]);
    //         objects.ToArray()[i2].TryGetValue("Cape", out npc.eqp_Cape[i2]);
    //         objects.ToArray()[i2].TryGetValue("Leg", out npc.eqp_Leg[i2]);
    //         objects.ToArray()[i2].TryGetValue("Gloves", out npc.eqp_Gloves[i2]);
    //         objects.ToArray()[i2].TryGetValue("Head", out npc.eqp_Head[i2]);
    //         objects.ToArray()[i2].TryGetValue("Horse", out npc.eqp_Horse[i2]);
    //         objects.ToArray()[i2].TryGetValue("HorseHarness", out npc.eqp_HorseHarness[i2]);

    //         npc.IsCivilianEquip[i2] = bools.ToArray()[i2];
    //         i2++;
    //     }
    // }

    // private void SaveRemovedEquip(ref List<Dictionary<string, string>> obj, ref string[] slotArray, ref List<bool> bools)
    // {
    //     int i2 = 0;

    //     foreach (string slot in slotArray)
    //     {
    //         if (slot != "TEMP_REMOVE_SLOT_DATA")
    //         {
    //             var dict = new Dictionary<string, string>();

    //             dict.Add("Item0", npc.eqp_Item0[i2]);
    //             dict.Add("Item1", npc.eqp_Item1[i2]);
    //             dict.Add("Item2", npc.eqp_Item2[i2]);
    //             dict.Add("Item3", npc.eqp_Item3[i2]);
    //             dict.Add("Body", npc.eqp_Body[i2]);
    //             dict.Add("Cape", npc.eqp_Cape[i2]);
    //             dict.Add("Leg", npc.eqp_Leg[i2]);
    //             dict.Add("Gloves", npc.eqp_Gloves[i2]);
    //             dict.Add("Head", npc.eqp_Head[i2]);
    //             dict.Add("Horse", npc.eqp_Horse[i2]);
    //             dict.Add("HorseHarness", npc.eqp_HorseHarness[i2]);

    //             obj.Add(dict);
    //             bools.Add(npc.IsCivilianEquip[i2]);
    //             i2++;
    //         }
    //     }


    // }

    public void DrawEquipmentSlot(ref string Slot, ref Item item, string slotID, ref bool isCivilian)
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



    // private void DrawEquipmentSlotDouble(ref string Slot, ref string civ_Slot, ref Item item, ref Item civ_item, string slotID, bool isCivilian)
    // {
    //     GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
    //     buttonStyle.fontStyle = FontStyle.Bold;
    //     buttonStyle.hover.textColor = Color.green;

    //     GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
    //     // GUIStyle objField = new GUIStyle(EditorStyles.objectField);

    //     Color newCol2;
    //     ColorUtility.TryParseHtmlString("#F65314", out newCol2);
    //     Color savedColorObj = EditorStyles.objectField.normal.textColor;


    //     var originDimensions = EditorGUIUtility.labelWidth;

    //     CheckItemSlot(ref Slot, ref item);


    //     titleStyle.fontSize = 12;
    //     titleStyle.normal.textColor = newCol2;



    //     if (item == null)
    //     {
    //         ColorUtility.TryParseHtmlString("#878f99", out newCol2);

    //         titleStyle.fontSize = 12;
    //         titleStyle.fontStyle = FontStyle.Normal;
    //         titleStyle.normal.textColor = newCol2;

    //         ColorUtility.TryParseHtmlString("#66757F", out newCol2);
    //         EditorStyles.objectField.normal.textColor = newCol2;
    //     }
    //     else
    //     {
    //         ColorUtility.TryParseHtmlString("#F7F7F7", out newCol2);
    //         EditorStyles.objectField.normal.textColor = newCol2;
    //     }

    //     // ColorUtility.TryParseHtmlString("#F65314", out newCol2);
    //     EditorGUILayout.BeginHorizontal();
    //     EditorGUILayout.BeginVertical();
    //     EditorGUILayout.LabelField("Slot - " + slotID, titleStyle, GUILayout.ExpandWidth(true));
    //     if (item != null)
    //     {
    //         string soloName = item.itemName;
    //         RemoveTSString(ref soloName);

    //         ColorUtility.TryParseHtmlString("#FFBB00", out newCol2);
    //         titleStyle.normal.textColor = newCol2;

    //         EditorGUILayout.LabelField(soloName, titleStyle, GUILayout.ExpandWidth(true));
    //     }
    //     object itemObj = EditorGUILayout.ObjectField(item, typeof(Item), true, GUILayout.Width(256));
    //     EditorGUILayout.EndVertical();

    //     EditorStyles.objectField.normal.textColor = savedColorObj;
    //     if (isCivilian)
    //     {
    //         Color newCol3;
    //         ColorUtility.TryParseHtmlString("#7CBB00", out newCol3);

    //         GUIStyle titleStyle2 = new GUIStyle(EditorStyles.boldLabel);

    //         titleStyle2.fontSize = 12;
    //         titleStyle2.normal.textColor = newCol3;

    //         CheckItemSlot(ref civ_Slot, ref civ_item);
    //         if (civ_item == null)
    //         {
    //             ColorUtility.TryParseHtmlString("#878f99", out newCol2);
    //             titleStyle2.fontSize = 12;
    //             titleStyle2.fontStyle = FontStyle.Normal;
    //             titleStyle2.normal.textColor = newCol2;

    //             ColorUtility.TryParseHtmlString("#66757F", out newCol2);
    //             EditorStyles.objectField.normal.textColor = newCol2;
    //         }
    //         else
    //         {
    //             ColorUtility.TryParseHtmlString("#F7F7F7", out newCol2);
    //             EditorStyles.objectField.normal.textColor = newCol2;
    //         }

    //         DrawUILineVerticalLarge(colUILine, 16);
    //         EditorGUILayout.Space(32);
    //         EditorGUILayout.BeginVertical();
    //         EditorGUILayout.LabelField("Civilian Slot - " + slotID, titleStyle2, GUILayout.ExpandWidth(true));
    //         if (civ_item != null)
    //         {
    //             string soloName = civ_item.itemName;
    //             RemoveTSString(ref soloName);

    //             ColorUtility.TryParseHtmlString("#34A853", out newCol2);
    //             titleStyle2.normal.textColor = newCol2;

    //             EditorGUILayout.LabelField(soloName, titleStyle2, GUILayout.ExpandWidth(true));
    //         }
    //         object civ_itemObj = EditorGUILayout.ObjectField(civ_item, typeof(Item), true, GUILayout.Width(256));
    //         EditorGUILayout.EndVertical();
    //         WriteSlotData(out civ_Slot, out civ_item, civ_itemObj);

    //         EditorStyles.objectField.normal.textColor = savedColorObj;
    //     }

    //     EditorGUILayout.EndHorizontal();
    //     WriteSlotData(out Slot, out item, itemObj);


    //     DrawUILine(colUILine, 3, 12);
    // }

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

    private static void WriteSlotData(out string Slot, out Item item, object itemObj)
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

    private void CheckItemSlot(ref string Slot, ref Item item)
    {
        if (Slot != null && Slot != "")
        {

            Slot = Slot.Replace("Item.", "");


            string modSettings = modsSettingsPath + npc.moduleID + ".asset";
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
                    foreach (var dependency in module.modDependencies)
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

                        if (module.id != npc.moduleID)
                        {
                            foreach (var modDPD in module.modDependencies)
                            {
                                if (modDPD == npc.moduleID)
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

    void DrawUpgradeTargetsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#FFB900", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#66ccff", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Upgrade: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showUpgradeTargetsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Upgrade Editor";
        }

        showUpgradeTargetsEditor = EditorGUILayout.Foldout(showUpgradeTargetsEditor, showEditorLabel, hiderStyle);

        if (showUpgradeTargetsEditor)
        {
            EditorGUILayout.LabelField("Upgrade Editor", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);


            EditorGUILayout.LabelField("Requiered item for Update:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            // EditorGUILayout.Space(2);

            upgdateRequieres_index = EditorGUILayout.Popup(upgdateRequieres_index, upgdateRequieres_options, GUILayout.Width(128));
            npc.upgrade_requires = "ItemCategory." + upgdateRequieres_options[upgdateRequieres_index];

            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Target", "Add NPC upgrade target ")), buttonStyle, GUILayout.Width(128)))
            {
                //var objects = new Dictionary<string, string>();

                //int i2 = 0;
                //foreach (string trg in npc.upgrade_targets)
                //{
                //    if (!objects.ContainsKey(trg))
                //    {
                //        objects.Add(trg, npc.upgrade_targets[i2]);
                //        i2++;
                //    }

                //}

                //int indexVal = objects.Count + 1;

                //npc.upgrade_targets = new string[indexVal];

                //i2 = 0;
                //foreach (var element in objects)
                //{
                //    npc.upgrade_targets[i2] = element.Key;
                //    i2++;
                //}

                var temp = new string[npc.upgrade_targets.Length + 1];
                npc.upgrade_targets.CopyTo(temp, 0);
                npc.upgrade_targets = temp;

                npc.upgrade_targets[npc.upgrade_targets.Length - 1] = "";

                UpgradeTargets = new NPCCharacter[npc.upgrade_targets.Length];

            }
            DrawUILine(colUILine, 3, 12);

            int i = 0;

            if (npc.upgrade_targets != null && npc.upgrade_targets.Length != 0)
            {
                foreach (var targetAsset in npc.upgrade_targets)
                {
                    // Debug.Log(UpgradeTargets.Length);
                    GetNPCAsset(ref npc.upgrade_targets[i], ref UpgradeTargets[i], false);
                    if (UpgradeTargets[i] == null)
                    {
                        GetNPCAsset(ref npc.upgrade_targets[i], ref UpgradeTargets[i], true);
                    }

                    // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


                    ColorUtility.TryParseHtmlString("#F25022", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 11;
                    EditorGUILayout.LabelField("Upgrade Target " + i, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                    ColorUtility.TryParseHtmlString("#FF9900", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 12;

                    string nameLabel = "None";
                    if (UpgradeTargets[i] != null)
                    {
                        nameLabel = UpgradeTargets[i].npcName;
                    }

                    RemoveTSString(ref nameLabel);

                    EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);

                    EditorGUILayout.BeginHorizontal();
                    object UpgradeTargetField = EditorGUILayout.ObjectField(UpgradeTargets[i], typeof(NPCCharacter), true, GUILayout.MaxWidth(320));

                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("X", "Remove Upgrade Target")), buttonStyle, GUILayout.Width(32)))
                    {
                        //var objects = new Dictionary<string, string>();
                        //npc.upgrade_targets[i] = "remove";

                        //int i2 = 0;
                        //foreach (string trg in npc.upgrade_targets)
                        //{
                        //    if (trg != "remove")
                        //    {
                        //        objects.Add(trg, npc.upgrade_targets[i2]);
                        //    }
                        //    i2++;
                        //}

                        //npc.upgrade_targets = new string[objects.Count];

                        //i2 = 0;
                        //foreach (var obj in objects)
                        //{
                        //    npc.upgrade_targets[i2] = obj.Key;
                        //    i2++;
                        //}

                        var temp = new string[npc.upgrade_targets.Length - 1];

                        int i2 = 0;
                        var i3 = 0;
                        foreach (string trg in npc.upgrade_targets)
                        {
                            if (i3 != i)
                            {
                                temp[i2] = npc.upgrade_targets[i3];
                                i2++;
                            }
                            i3++;
                        }

                        npc.upgrade_targets = temp;
                        UpgradeTargets = new NPCCharacter[npc.upgrade_targets.Length];

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    UpgradeTargets[i] = (NPCCharacter)UpgradeTargetField;

                    if (UpgradeTargets[i] != null)
                    {
                        npc.upgrade_targets[i] = "NPCCharacter." + UpgradeTargets[i].id;
                    }
                    else
                    {
                        npc.upgrade_targets[i] = "";
                    }
                    DrawUILine(colUILine, 1, 4);
                    i++;
                }
            }

        }
    }
    void DrawSkillsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#33ccff", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#66ccff", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Skills: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showSkillsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Skills Editor";
        }

        showSkillsEditor = EditorGUILayout.Foldout(showSkillsEditor, showEditorLabel, hiderStyle);

        if (showSkillsEditor)
        {

            EditorGUILayout.LabelField("Skills Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            if (npc.skills == null || npc.skills.Length < settingsAsset.SkillsDefinitions.Length)
            {
                EditorGUILayout.BeginHorizontal();

                skills_index = EditorGUILayout.Popup("Skills:", skills_index, skills_options, GUILayout.Width(192));
                // npc.skills[i] = skills_options[skills_index];


                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Skill", "Add selected Skill for this NPC Character")), buttonStyle, GUILayout.Width(128)))
                {

                    var objects = new Dictionary<string, string>();

                    int i2 = 0;
                    foreach (string skillAsset in npc.skills)
                    {
                        objects.Add(skillAsset, npc.skillValues[i2]);
                        i2++;
                    }

                    int indexVal = objects.Count + 1;

                    npc.skills = new string[indexVal];
                    npc.skillValues = new string[indexVal];

                    i2 = 0;
                    foreach (var element in objects)
                    {
                        npc.skills[i2] = element.Key;
                        npc.skillValues[i2] = element.Value;
                        i2++;
                    }

                    npc.skills[npc.skills.Length - 1] = skills_options[skills_index];
                    npc.skillValues[npc.skillValues.Length - 1] = "0";

                    CreateSkillsOptions(ref skills_options, ref skills_index, settingsAsset.SkillsDefinitions);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                // DrawUILine(colUILine, 3, 12);
            }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (npc.skills != null && npc.skills.Length != 0)
            {
                foreach (var skill in npc.skills)
                {
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    EditorGUILayout.LabelField(npc.skills[i], titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.BeginHorizontal();
                    CreateIntAttribute(ref npc.skillValues[i], "Skill Value:");

                    if (GUILayout.Button((new GUIContent("X", "Remove Skill")), buttonStyle, GUILayout.Width(32)))
                    {
                        var objects = new Dictionary<string, string>();
                        npc.skills[i] = "remove";
                        npc.skillValues[i] = "";

                        int i2 = 0;
                        foreach (string skillAsset in npc.skills)
                        {
                            if (skillAsset != "remove")
                            {
                                objects.Add(skillAsset, npc.skillValues[i2]);
                            }
                            i2++;
                        }

                        npc.skills = new string[objects.Count];
                        npc.skillValues = new string[objects.Count];

                        i2 = 0;
                        foreach (var obj in objects)
                        {
                            npc.skillValues[i2] = obj.Value;
                            npc.skills[i2] = obj.Key;
                            i2++;
                        }
                        CreateSkillsOptions(ref skills_options, ref skills_index, settingsAsset.SkillsDefinitions);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }
    void DrawTraitsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#ff9933", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#ff9966", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Traits: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showTraitsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Traits Editor";
        }

        showTraitsEditor = EditorGUILayout.Foldout(showTraitsEditor, showEditorLabel, hiderStyle);

        if (showTraitsEditor)
        {
            
            EditorGUILayout.LabelField("Traits Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);

            // Check 18+
            if (int.Parse(npc.age) < 18)
            {
                EditorGUILayout.HelpBox("You can assign traits only for 18+, in other case traits for this NPC will by ignored at export.", MessageType.Error);
                DrawUILine(colUILine, 3, 12);
            }

            if (npc.traits == null || npc.traits.Length < settingsAsset.TraitsDefinitions.Length)
            {
                EditorGUILayout.BeginHorizontal();

                traits_index = EditorGUILayout.Popup("Traits:", traits_index, traits_options, GUILayout.Width(192));
                // npc.skills[i] = skills_options[skills_index];


                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Trait", "Add selected Trait for this NPC Character")), buttonStyle, GUILayout.Width(128)))
                {

                    var objects = new Dictionary<string, string>();

                    int i2 = 0;
                    foreach (string traitAsset in npc.traits)
                    {
                        objects.Add(traitAsset, npc.traitValues[i2]);
                        i2++;
                    }

                    int indexVal = objects.Count + 1;

                    npc.traits = new string[indexVal];
                    npc.traitValues = new string[indexVal];

                    i2 = 0;
                    foreach (var element in objects)
                    {
                        npc.traits[i2] = element.Key;
                        npc.traitValues[i2] = element.Value;
                        i2++;
                    }

                    npc.traits[npc.traits.Length - 1] = traits_options[traits_index];
                    npc.traitValues[npc.traitValues.Length - 1] = "0";

                    CreateTraisOptions(ref traits_options, ref traits_index, settingsAsset.TraitsDefinitions);

                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                DrawUILine(colUILine, 3, 12);
            }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (npc.traits != null && npc.traits.Length != 0)
            {
                foreach (var trait in npc.traits)
                {

                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    EditorGUILayout.LabelField(npc.traits[i], titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.BeginHorizontal();
                    CreateIntAttribute(ref npc.traitValues[i], "Trait Value:");

                    if (GUILayout.Button((new GUIContent("X", "Remove Trait")), buttonStyle, GUILayout.Width(32)))
                    {
                        var objects = new Dictionary<string, string>();
                        npc.traits[i] = "remove";
                        npc.traitValues[i] = "";

                        int i2 = 0;
                        foreach (string traitAsset in npc.traits)
                        {
                            if (traitAsset != "remove")
                            {
                                objects.Add(traitAsset, npc.traitValues[i2]);
                            }
                            i2++;
                        }

                        npc.traits = new string[objects.Count];
                        npc.traitValues = new string[objects.Count];

                        i2 = 0;
                        foreach (var obj in objects)
                        {
                            npc.traits[i2] = obj.Key;
                            npc.traitValues[i2] = obj.Value;
                            i2++;
                        }
                        CreateTraisOptions(ref traits_options, ref traits_index, settingsAsset.TraitsDefinitions);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }

    void CreateIntAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        int val;
        int.TryParse(attribute, out val);
        val = EditorGUILayout.IntField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

    }
    void CreateFloatAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        float val;
        float.TryParse(attribute, out val);
        val = EditorGUILayout.FloatField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

    }
    private void CreateSkillsOptions(ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS
        if (npc != null)
        {

            var listOptionsAll = new List<string>();
            var listOptionsLoaded = new List<string>();

            foreach (var data in definitions)
            {
                listOptionsAll.Add(data);

            }

            if (npc.skills != null && npc.skills.Length != 0)
            {
                foreach (var dataSkill in npc.skills)
                {
                    listOptionsLoaded.Add(dataSkill);
                }
            }

            foreach (var option in listOptionsLoaded)
            {
                if (listOptionsAll.Contains(option))
                {
                    listOptionsAll.Remove(option);
                }
            }

            // listOptionsAll.Add(typeString);

            options = new string[listOptionsAll.Count];

            int i = 0;
            foreach (var element in listOptionsAll)
            {
                options[i] = element;
                i++;
                // Debug.Log(element);
            }

            index = 0;
            // i = 0;
            // foreach (var type in options)
            // {
            //     if (type == typeString)
            //     {
            //         // Debug.Log("");
            //         index = i;
            //     }
            //     i++;
            // }
        }
    }
    private void CreateTraisOptions(ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS


        var listOptionsAll = new List<string>();
        var listOptionsLoaded = new List<string>();

        foreach (var data in definitions)
        {
            listOptionsAll.Add(data);

        }

        if (npc.traits != null && npc.traits.Length != 0)
        {
            foreach (var DataTrait in npc.traits)
            {
                listOptionsLoaded.Add(DataTrait);
            }
        }

        foreach (var option in listOptionsLoaded)
        {
            if (listOptionsAll.Contains(option))
            {
                listOptionsAll.Remove(option);
            }
        }

        options = new string[listOptionsAll.Count];

        int i = 0;
        foreach (var element in listOptionsAll)
        {
            options[i] = element;
            i++;

        }

        index = 0;

    }
    private void CreateOptions(ref string typeString, ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS
        options = new string[definitions.Length];

        int i = 0;
        foreach (var data in definitions)
        {
            options[i] = data;
            // Debug.Log("");
            i++;
        }

        i = 0;
        foreach (var type in options)
        {
            if (type == typeString)
            {
                // Debug.Log("");
                index = i;
            }
            i++;
        }
    }

    private void SetLabelFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel)
    {

        bool isDependencyTS = false;
        var labelStyle = new GUIStyle(EditorStyles.label);
        if (soloString == null || soloString == "")
        {
            EditorGUILayout.HelpBox(typeName + " field is empty", MessageType.Error);
        }


        if (name != null && inputString != null && inputString != "")
        {
            soloString = inputString;
            TS_Name = inputString;
            Regex regex = new Regex("{=(.*)}");
            if (regex != null)
            {
                var v = regex.Match(TS_Name);
                string s = v.Groups[1].ToString();
                TS_Name = "{=" + s + "}";
            }

            if (TS_Name != "" && TS_Name != "{=}")
            {
                soloString = soloString.Replace(TS_Name, "");

                string TSasset = (dataPath + moduleID + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset");

                if (System.IO.File.Exists(TSasset))
                {
                    TS = (TranslationString)AssetDatabase.LoadAssetAtPath(TSasset, typeof(TranslationString));
                }
                else
                {

                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + moduleID + ".asset";

                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {

                            string dpdTSAsset = dataPath + dpdMod + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";

                            if (System.IO.File.Exists(dpdTSAsset))
                            {
                                TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdTSAsset, typeof(TranslationString));
                                isDependencyTS = true;
                                break;
                            }
                            else
                            {
                                TS = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (TS == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == npc.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.translationData.translationStrings)
                                    {
                                        if (data.id == TS_Name)
                                        {
                                            string dpdTSAsset = dataPath + iSDependencyOfMod.id + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";
                                            TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdTSAsset, typeof(TranslationString));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (TS == null)
                        {
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + npc.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        soloString = EditorGUILayout.TextField(typeName, soloString);

        // Draw UI - & Edit Translation String 
        EditorGUILayout.BeginHorizontal();
        // translationStringID = EditorGUILayout.TextField("Translation String", translationStringID);

        if (TS == null)
        {
            labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
            EditorGUILayout.LabelField("Translation String: (Unused)", labelStyle);
        }
        else
        {
            EditorGUILayout.LabelField("Translation String:", EditorStyles.label);
        }


        EditorGUILayout.ObjectField(TS, typeof(TranslationString), true);

        if (GUILayout.Button("Edit Translation"))
        {

            TranslationEditor transEditor = (TranslationEditor)EditorWindow.GetWindow(typeof(TranslationEditor), true, "Translation Editor");
            transEditor.defaultData = TS;
            transEditor.module = moduleID;
            transEditor.obj = obj;
            transEditor.objectID = objID;
            transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - )";

            transEditor.isDependency = isDependencyTS;

            transEditor.SearchStrings();
            transEditor.Show();
        }
        EditorGUILayout.EndHorizontal();
        // DrawUILine(colUILine, 3, 12);


        // Solo Name Check
        if (soloString != null && soloString != "")
        {
            if (TS_Name != "" && TS_Name != "{=}")
            {
                inputString = TS_Name + soloString;
            }
            else
            {
                inputString = soloString;
            }


        }

        if (soloString != null && soloString.Length <= 2)
        {
            // soloText = "";
            // translationString = "";
            if (TS != null)
            {
                inputString = TS.name + soloString;
            }
            else
            {
                inputString = soloString;
            }

        }

        if (TS != null && isDependencyTS == false)
        {
            TS.stringText = soloString;
        }

    }

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