using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
[System.Serializable]

[CustomEditor(typeof(Culture))]
public class CultureAssetEditor : Editor
{
    string dataPath = "Assets/Resources/Data/";
    string folder = "CulturesTranslationData";
    //
    bool showMaleNamesEditor;
    bool showFemaleNamesEditor;
    bool showClanNamesEditor;
    bool showTournamentTemplatesEditor;
    bool showTournamentEditorOne;
    bool showTournamentEditorTwo;
    bool showTournamentEditorFour;

    //
    public NPCCharacter[] TTT_NPC_one;
    public NPCCharacter[] TTT_NPC_two;
    public NPCCharacter[] TTT_NPC_four;

    bool show_chld_template;
    public NPCCharacter[] child_character_templates;
    bool show_notable_and_wanderer_templates;

    public NPCCharacter[] notable_and_wanderer_templates;
    bool show_lord_templates;

    public NPCCharacter[] lord_templates;
    bool show_rebellion_hero_templates;

    public NPCCharacter[] rebellion_hero_templates;
    //

    string[] m_soloName = new string[3];
    string[] m_name_TranslationString = new string[3];
    TranslationString[] translationStringMale_Names = new TranslationString[3];
    string[] f_soloName = new string[3];
    string[] f_name_TranslationString = new string[3];
    TranslationString[] translationStringFemale_Names = new TranslationString[3];
    string[] cln_soloName = new string[3];
    string[] cln_name_TranslationString = new string[3];
    TranslationString[] translationStringClan_Names = new TranslationString[3];


    string soloName;
    string soloText;
    string nameTranslationString;
    string textTranslationString;
    TranslationString translationStringName;
    TranslationString translationStringDescription;
    Vector2 textScrollPos;
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    bool showNPC_Battle;
    bool showPT;
    bool showNPC_Civillian;
    public Text defFaceKey;

    Color primaryColor;
    Color secondaryBannerColor;
    Color col;
    Color col_opened;
    Color col_active;

    Culture cult;

    bool isMainCulture;
    bool canHaveSettlements;
    Color color_a;

    Color color_b;

    // Battle Presets
    public NPCCharacter elite_basic_troop;
    public NPCCharacter basic_troop;
    public NPCCharacter melee_militia_troop;
    public NPCCharacter ranged_militia_troop;
    public NPCCharacter melee_elite_militia_troop;
    public NPCCharacter ranged_elite_militia_troop;

    // PartyTemplates Presets
    public PartyTemplate villager_party_template;
    public PartyTemplate default_party_template;
    public PartyTemplate caravan_party_template;
    public PartyTemplate elite_caravan_party_template;
    public PartyTemplate militia_party_template;
    public PartyTemplate rebels_party_template;

    // Civilian Presets
    public NPCCharacter tournament_master;
    public NPCCharacter villager;
    public NPCCharacter caravan_master;
    public NPCCharacter armed_trader;
    public NPCCharacter caravan_guard;
    public NPCCharacter veteran_caravan_guard;
    public NPCCharacter duel_preset;
    public NPCCharacter prison_guard;
    public NPCCharacter guard;
    public NPCCharacter steward;
    public NPCCharacter blacksmith;
    public NPCCharacter weaponsmith;
    public NPCCharacter townswoman;
    public NPCCharacter townswoman_infant;
    public NPCCharacter townswoman_child;
    public NPCCharacter townswoman_teenager;
    public NPCCharacter townsman;
    public NPCCharacter townsman_infant;
    public NPCCharacter townsman_child;
    public NPCCharacter village_woman;
    public NPCCharacter villager_male_child;
    public NPCCharacter villager_male_teenager;
    public NPCCharacter villager_female_child;
    public NPCCharacter villager_female_teenager;
    public NPCCharacter townsman_teenager;
    public NPCCharacter ransom_broker;
    public NPCCharacter gangleader_bodyguard;
    public NPCCharacter merchant_notary;
    public NPCCharacter artisan_notary;
    public NPCCharacter preacher_notary;
    public NPCCharacter rural_notable_notary;
    public NPCCharacter shop_worker;
    public NPCCharacter tavernkeeper;
    public NPCCharacter taverngamehost;
    public NPCCharacter musician;
    public NPCCharacter tavern_wench;
    public NPCCharacter armorer;
    public NPCCharacter horseMerchant;
    public NPCCharacter barber;
    public NPCCharacter merchant;
    public NPCCharacter beggar;
    public NPCCharacter female_beggar;
    public NPCCharacter female_dancer;
    public NPCCharacter gear_practice_dummy;
    public NPCCharacter weapon_practice_stage_1;
    public NPCCharacter weapon_practice_stage_2;
    public NPCCharacter weapon_practice_stage_3;
    public NPCCharacter gear_dummy;

    // Update 1.7.2
    public NPCCharacter basic_mercenary_troop;
    public NPCCharacter bandit_chief;
    public NPCCharacter bandit_raider;
    public NPCCharacter bandit_bandit;
    public NPCCharacter bandit_boss;

    public PartyTemplate vassal_reward_party_template;
    public PartyTemplate bandit_boss_party_template;

    //Update 1.8.0
    public Equipment default_battle_equipment_roster;
    public Equipment default_civilian_equipment_roster;

    //public string faction_banner_key;
    public bool is_bandit;
    public int militia_bonus;

    bool showRewardItems;
    public Item[] vassal_reward_items;

    //  public string[] cultural_feats;
    bool showCulturalFeatsEditor;
    public string[] cultural_feats_options;
    public int cultural_feats_index;

    bool showMapIconsID;
    public int[] possible_clan_banner_icon_ids;


    bool isDependency = false;
    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";

    public void OnEnable()
    {
        cult = (Culture)target;
        EditorUtility.SetDirty(cult);


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


        if (cult.male_names != null && cult.male_names.Length > 0)
        {
            m_soloName = new string[cult.male_names.Length];
            m_name_TranslationString = new string[cult.male_names.Length];
            translationStringMale_Names = new TranslationString[cult.male_names.Length];
        }

        if (cult.female_names != null && cult.female_names.Length > 0)
        {
            f_soloName = new string[cult.female_names.Length];
            f_name_TranslationString = new string[cult.female_names.Length];
            translationStringFemale_Names = new TranslationString[cult.female_names.Length];
        }

        if (cult.clan_names != null && cult.clan_names.Length > 0)
        {
            cln_soloName = new string[cult.clan_names.Length];
            cln_name_TranslationString = new string[cult.clan_names.Length];
            translationStringClan_Names = new TranslationString[cult.clan_names.Length];
        }

        if (cult.TTT_one_participants != null && cult.TTT_one_participants.Length > 0)
        {
            TTT_NPC_one = new NPCCharacter[cult.TTT_one_participants.Length];
            TTT_NPC_two = new NPCCharacter[cult.TTT_two_participants.Length];
            TTT_NPC_four = new NPCCharacter[cult.TTT_four_participants.Length];
        }

        //if (cult.child_character_templates != null && cult.child_character_templates.Length > 0)
        //{
        //    child_character_templates = new NPCCharacter[cult.child_character_templates.Length];
        //} 
        if (cult.notable_and_wanderer_templates != null && cult.notable_and_wanderer_templates.Length > 0)
        {
            notable_and_wanderer_templates = new NPCCharacter[cult.notable_and_wanderer_templates.Length];
        }
        if (cult.lord_templates != null && cult.lord_templates.Length > 0)
        {
            lord_templates = new NPCCharacter[cult.lord_templates.Length];
        }
        if (cult.rebellion_hero_templates != null && cult.rebellion_hero_templates.Length > 0)
        {
            rebellion_hero_templates = new NPCCharacter[cult.rebellion_hero_templates.Length];
        }

        CreateFeatsOptions(ref cultural_feats_options, ref cultural_feats_index, settingsAsset.CulturalFeatsDefinitions);

        if (cult.reward_item_id != null && cult.reward_item_id.Length > 0)
            vassal_reward_items = new Item[cult.reward_item_id.Length];
        else
            vassal_reward_items = new Item[0];

        if (cult.banner_icon_id != null && cult.banner_icon_id.Length > 0)
            possible_clan_banner_icon_ids = new int[cult.banner_icon_id.Length];
        else
            possible_clan_banner_icon_ids = new int[0];

    }
    public override void OnInspectorGUI()
    {



        if (settingsAsset.currentModule != cult.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == cult.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + cult.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + cult.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + cult.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + cult.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + cult.moduleID + ".asset", typeof(ModuleReceiver));
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
                        assetMng.objID = -1;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = cult;

                        assetMng.assetName_org = cult.id;
                        assetMng.assetName_new = cult.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = -1;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = cult;

                        assetMng.assetName_org = cult.id;
                        assetMng.assetName_new = cult.id;
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

        GUIStyle foldoutHeader = new GUIStyle(EditorStyles.foldoutHeader);
        GUIStyle headerBoldStyle = new GUIStyle(EditorStyles.boldLabel);
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);

        EditorGUI.BeginDisabledGroup(true);
        cult.id = EditorGUILayout.TextField("Culture ID", cult.id);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {

            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = -1;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = cult;

                assetMng.assetName_org = cult.id;
                assetMng.assetName_new = cult.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = -1;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = cult;

                assetMng.assetName_org = cult.id;
                assetMng.assetName_new = cult.id;
            }

        }

        // faction name & translationString tag
        DrawUILine(colUILine, 3, 12);


        // object ID
        //---
        // 0 culture name
        // -1 culture text

        SetLabelFieldTS(ref cult.cultureName, ref soloName, ref nameTranslationString, folder, translationStringName, "Culture Name", cult.moduleID, cult, 0, cult.id);
        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.BeginHorizontal();
        var originLabelWidth = EditorGUIUtility.labelWidth;
        // is main culture
        if (cult.is_main_culture != null)
        {
            if (cult.is_main_culture == "true")
            {
                isMainCulture = true;
            }
            else
            {
                isMainCulture = false;
            }
        }

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Main Culture   "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        isMainCulture = EditorGUILayout.Toggle("Is Main Culture", isMainCulture);
        DrawUILineVertical(colUILine, 1, 1, 16);


        if (isMainCulture)
        {
            cult.is_main_culture = "true";
        }
        else
        {
            cult.is_main_culture = "false";
        }

        if (cult.can_have_settlement != null)
        {
            if (cult.can_have_settlement == "true")
            {
                canHaveSettlements = true;
            }
            else
            {
                canHaveSettlements = false;
            }
        }

        // is bandit
        if (cult.is_bandit != null)
        {
            if (cult.is_bandit == "true")
            {
                is_bandit = true;
            }
            else
            {
                is_bandit = false;
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Bandit   "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        is_bandit = EditorGUILayout.Toggle("Is Bandit", is_bandit);
        DrawUILineVertical(colUILine, 1, 1, 16);


        if (is_bandit)
        {
            cult.is_bandit = "true";
        }
        else
        {
            cult.is_bandit = "false";
        }

        // can have settlements

        if (cult.can_have_settlement != null)
        {
            if (cult.can_have_settlement == "true")
            {
                canHaveSettlements = true;
            }
            else
            {
                canHaveSettlements = false;
            }
        }


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Can Have Settlements   "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        canHaveSettlements = EditorGUILayout.Toggle("Can Have Settlements", canHaveSettlements);
        DrawUILineVertical(colUILine, 1, 1, 16);


        if (canHaveSettlements)
        {
            cult.can_have_settlement = "true";
        }
        else
        {
            cult.can_have_settlement = "false";
        }

        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originLabelWidth;

        GUILayout.Space(4);
        DrawUILine(colUILine, 1, 6);
        GUILayout.Space(4);

        // Town Edge Number
        GUILayout.BeginHorizontal();

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Town Edge Number:   "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        int TENValue;

        int.TryParse(cult.town_edge_number, out TENValue);

        TENValue = EditorGUILayout.IntField("Town Edge Number:", TENValue, GUILayout.MaxWidth(192));

        cult.town_edge_number = TENValue.ToString();
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originLabelWidth;
        /// Town Edge Number end
        GUILayout.Space(4);
        // Prosperity Bonus
        GUILayout.BeginHorizontal();

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Prosperity Bonus:   "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        int ProsPValue;

        int.TryParse(cult.prosperity_bonus, out ProsPValue);

        ProsPValue = EditorGUILayout.IntField("Prosperity Bonus:", ProsPValue, GUILayout.MaxWidth(192));

        cult.prosperity_bonus = ProsPValue.ToString();
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originLabelWidth;
        /// Prosperity Bonus end

        // Militia Bonus
        GUILayout.BeginHorizontal();

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Militia Bonus:    "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        int militBonus;
        int.TryParse(cult.militia_bonus, out militBonus);

        militBonus = EditorGUILayout.IntField("Militia Bonus:", militBonus, GUILayout.MaxWidth(192));

        cult.militia_bonus = militBonus.ToString();
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originLabelWidth;
        /// Militia Bonus end 

        /// Game Type & BG Mesh
        GUILayout.Space(4);
        cult.encounter_background_mesh = EditorGUILayout.TextField("Encounter Background Mesh:", cult.encounter_background_mesh);
        cult.board_game_type = EditorGUILayout.TextField("Board Game Type:", cult.board_game_type);

        DrawUILine(colUILine, 3, 12);

        // Culture Colors
        // COLOR A

        EditorGUILayout.LabelField("Culture Colors:", EditorStyles.boldLabel);
        if (cult.color != null && cult.color != "")
        {
            if (cult.color.Contains("0xff"))
            {
                cult.color = cult.color.Replace("0xff", "FF");
                // Debug.Log(cult.color);
            }
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + cult.color, out primaryColor);
            labelStyle.normal.textColor = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Primary Color:", labelStyle);
            primaryColor = EditorGUILayout.ColorField(primaryColor);
            GUILayout.EndHorizontal();
            cult.color = ColorUtility.ToHtmlStringRGBA(primaryColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (primaryColor == new Color(0, 0, 0, 0))
            {
                cult.color = "";
            }
        }
        else
        {
            if (primaryColor == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Primary Color: (Unused)", labelStyle);
                primaryColor = EditorGUILayout.ColorField(primaryColor);
                GUILayout.EndHorizontal();
                cult.color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Primary Color:", labelStyle);
                primaryColor = EditorGUILayout.ColorField(primaryColor);
                GUILayout.EndHorizontal();
                cult.color = ColorUtility.ToHtmlStringRGBA(primaryColor);
            }
        }

        // COLOR B

        if (cult.color2 != null && cult.color2 != "")
        {

            if (cult.color2.Contains("0xff"))
            {
                cult.color2 = cult.color2.Replace("0xff", "FF");
                // Debug.Log(cult.color2);
            }

            labelStyle = new GUIStyle(EditorStyles.boldLabel);

            ColorUtility.TryParseHtmlString("#" + cult.color2, out secondaryBannerColor);
            labelStyle.normal.textColor = new Color(secondaryBannerColor.r, secondaryBannerColor.g, secondaryBannerColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Secondary Color:", labelStyle);
            secondaryBannerColor = EditorGUILayout.ColorField(secondaryBannerColor);
            GUILayout.EndHorizontal();
            cult.color2 = ColorUtility.ToHtmlStringRGBA(secondaryBannerColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (secondaryBannerColor == new Color(0, 0, 0, 0))
            {
                cult.color2 = "";
            }
        }
        else
        {
            if (secondaryBannerColor == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Secondary Color: (Unused)", labelStyle);
                secondaryBannerColor = EditorGUILayout.ColorField(secondaryBannerColor);
                GUILayout.EndHorizontal();
                cult.color2 = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(secondaryBannerColor.r, secondaryBannerColor.g, secondaryBannerColor.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Secondary Color:", labelStyle);
                secondaryBannerColor = EditorGUILayout.ColorField(secondaryBannerColor);
                GUILayout.EndHorizontal();
                cult.color2 = ColorUtility.ToHtmlStringRGBA(secondaryBannerColor);
            }
        }
        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.LabelField("Faction Banner Key:", EditorStyles.boldLabel);
        cult.faction_banner_key = EditorGUILayout.TextField(cult.faction_banner_key);

        GUILayout.Space(4);

        if (GUILayout.Button("Edit Banner", GUILayout.Width(80)))
        {
            if (BANNER_EDITOR_Instance == null)
            {
                BannerEditor assetMng = (BannerEditor)CreateInstance(typeof(BannerEditor));
                assetMng.bannerKey = cult.faction_banner_key;
                //assetMng.ReadBannerKey();
                assetMng.inputNPC = null;
                assetMng.inputKingdom = null;
                assetMng.inputFaction = null;
                assetMng.inputCulture = cult;
            }
            else
            {
                BannerEditor assetMng = BANNER_EDITOR_Instance;
                assetMng.bannerKey = cult.faction_banner_key;
                //assetMng.ReadBannerKey();
                assetMng.inputNPC = null;
                assetMng.inputKingdom = null;
                assetMng.inputFaction = null;
                assetMng.inputCulture = cult;
            }
        }

        DrawUILine(colUILine, 3, 12);

        // DEFAULT FACE KEY

        EditorGUILayout.LabelField("Default Face Key:", EditorStyles.boldLabel);
        cult.default_face_key = EditorGUILayout.TextField(cult.default_face_key);

        DrawUILine(colUILine, 3, 12);

        //// Assign Equipments (WIP) - Temporary unused
        ///
        ////ColorUtility.TryParseHtmlString("#ffc168", out col);
        //ColorUtility.TryParseHtmlString("#ff9933", out col_opened);

        //headerBoldStyle.normal.textColor = col;
        //headerBoldStyle.onNormal.textColor = col_opened;
        //EditorGUILayout.LabelField("Default Equipments", headerBoldStyle);
        //DrawUILine(colUILine, 1, 6);
        //AssignEquipments(ref cult.default_battle_equipment_roster, default_battle_equipment_roster, "Default Battle Equipment Roster:");
        //AssignEquipments(ref cult.default_civilian_equipment_roster, default_civilian_equipment_roster, "Default Civilian Equipment Roster:");

        //// AssignBattleNPC
        //DrawUILine(colUILine, 3, 12);

        // Assign party templates
        ColorUtility.TryParseHtmlString("#01b636", out col);
        ColorUtility.TryParseHtmlString("#3bd16f", out col_opened);

        foldoutHeader.normal.textColor = col;
        foldoutHeader.onNormal.textColor = col_opened;

        showPT = EditorGUILayout.Foldout(showPT, "Party Templates", foldoutHeader);

        if (showPT)
        {
            // EditorGUILayout.LabelField("Party Templates:", EditorStyles.boldLabel);
            GUILayout.Space(4);
            AssignPT(ref cult.default_party_template, default_party_template, "Deafult Party Template");
            AssignPT(ref cult.villager_party_template, villager_party_template, "Villager Party Template");
            AssignPT(ref cult.caravan_party_template, caravan_party_template, "Caravan Party Template");
            AssignPT(ref cult.elite_caravan_party_template, elite_caravan_party_template, "Elite Caravan Party Template");
            AssignPT(ref cult.militia_party_template, militia_party_template, "Militia Party Template");
            AssignPT(ref cult.rebels_party_template, rebels_party_template, "Rebels Party Template");

            // Update 1.7.2
            AssignPT(ref cult.vassal_reward_party_template, vassal_reward_party_template, "Vassal Reward Party Template");
            AssignPT(ref cult.bandit_boss_party_template, bandit_boss_party_template, "Bandit Boss Party Template");
        }
        DrawUILine(colUILine, 3, 12);

        ColorUtility.TryParseHtmlString("#C72C41", out col);
        ColorUtility.TryParseHtmlString("#EE4540", out col_opened);

        foldoutHeader.normal.textColor = col;
        foldoutHeader.onNormal.textColor = col_opened;


        showNPC_Battle = EditorGUILayout.Foldout(showNPC_Battle, "Battle NPC", foldoutHeader);
        if (showNPC_Battle)
        {
            // DrawUILine(colUILine, 1, 4);
            // EditorGUILayout.LabelField("NPC Battle Templates:", headerBoldStyle);
            GUILayout.Space(8);


            AssignNPC(ref cult.basic_troop, basic_troop, "Basic troop:");
            AssignNPC(ref cult.elite_basic_troop, elite_basic_troop, "Elite basic troop:");
            AssignNPC(ref cult.melee_militia_troop, melee_militia_troop, "Melee militia troop:");
            AssignNPC(ref cult.ranged_militia_troop, ranged_militia_troop, "Ranged militia troop:");
            AssignNPC(ref cult.melee_elite_militia_troop, melee_elite_militia_troop, "Melee Elite militia troop:");
            AssignNPC(ref cult.ranged_elite_militia_troop, ranged_elite_militia_troop, "Ranged Elite militia troop:");


        }
        DrawUILine(colUILine, 3, 12);

        ColorUtility.TryParseHtmlString("#4b86b4", out col);
        ColorUtility.TryParseHtmlString("#63ace5", out col_opened);

        foldoutHeader.normal.textColor = col;
        foldoutHeader.onNormal.textColor = col_opened;

        showNPC_Civillian = EditorGUILayout.Foldout(showNPC_Civillian, "Civilian NPC", foldoutHeader);
        if (showNPC_Civillian)
        {
            ///Civilian Templates
            /// 

            // DrawUILine(colUILine, 1, 4);
            // EditorGUILayout.LabelField("NPC Civilian Templates:", headerBoldStyle);
            GUILayout.Space(8);

            AssignNPC(ref cult.townsman, townsman, "Townsman:");
            AssignNPC(ref cult.townsman_child, townsman_child, "Townsman child:");
            AssignNPC(ref cult.townsman_infant, townsman_infant, "Townsman infant:");
            AssignNPC(ref cult.townsman_teenager, townsman_teenager, "Townsman teenager:");

            AssignNPC(ref cult.townswoman, townswoman, "Townswoman:");
            AssignNPC(ref cult.townswoman_child, townswoman_child, "Townswoman child:");
            AssignNPC(ref cult.townswoman_infant, townswoman_infant, "Townswoman infant:");
            AssignNPC(ref cult.townswoman_teenager, townswoman_teenager, "Townswoman teenager:");

            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.villager, villager, "Villager:");
            AssignNPC(ref cult.villager_male_child, villager_male_child, "Villager male child:");
            AssignNPC(ref cult.villager_male_teenager, villager_male_teenager, "Villager male teenager:");

            AssignNPC(ref cult.village_woman, village_woman, "Village woman:");
            AssignNPC(ref cult.villager_female_child, villager_female_child, "Villager female child:");
            AssignNPC(ref cult.villager_female_teenager, villager_female_teenager, "Villager female teenager:");
            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.armed_trader, armed_trader, "Armed Trader:");
            AssignNPC(ref cult.caravan_master, caravan_master, "Caravan Master:");
            AssignNPC(ref cult.caravan_guard, caravan_guard, "Caravan Guard:");
            AssignNPC(ref cult.veteran_caravan_guard, veteran_caravan_guard, "Veteran Caravan Guard:");

            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.tournament_master, tournament_master, "Tournament Master:");
            AssignNPC(ref cult.duel_preset, duel_preset, "Duel Preset:");
            AssignNPC(ref cult.gear_dummy, gear_dummy, "Gear dummy:");
            AssignNPC(ref cult.gear_practice_dummy, gear_practice_dummy, "Gear practice dummy:");
            AssignNPC(ref cult.weapon_practice_stage_1, weapon_practice_stage_1, "Weapon practice stage 1:");
            AssignNPC(ref cult.weapon_practice_stage_2, weapon_practice_stage_2, "Weapon practice stage 3:");
            AssignNPC(ref cult.weapon_practice_stage_3, weapon_practice_stage_3, "Weapon practice stage 3:");

            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.merchant, merchant, "Merchant:");
            AssignNPC(ref cult.merchant_notary, merchant_notary, "Merchant notary:");
            AssignNPC(ref cult.horseMerchant, horseMerchant, "Horse merchant:");
            AssignNPC(ref cult.armorer, armorer, "Armorer:");
            AssignNPC(ref cult.blacksmith, blacksmith, "Blacksmith:");
            AssignNPC(ref cult.weaponsmith, weaponsmith, "Weaponsmith:");
            AssignNPC(ref cult.artisan_notary, artisan_notary, "Artisan notary:");
            AssignNPC(ref cult.preacher_notary, preacher_notary, "Preacher notary:");

            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.tavernkeeper, tavernkeeper, "Tavernkeeper:");
            AssignNPC(ref cult.taverngamehost, taverngamehost, "Taverngamehost:");
            AssignNPC(ref cult.tavern_wench, tavern_wench, "Tavern wench:");
            AssignNPC(ref cult.musician, musician, "Musician:");
            AssignNPC(ref cult.female_dancer, female_dancer, "Female dancer:");
            AssignNPC(ref cult.ransom_broker, ransom_broker, "Ransom broker:");

            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.prison_guard, prison_guard, "Prision Guard:");
            AssignNPC(ref cult.guard, guard, "Guard:");
            AssignNPC(ref cult.steward, steward, "Steward:");
            AssignNPC(ref cult.gangleader_bodyguard, gangleader_bodyguard, "Gangleader bodyguard:");
            AssignNPC(ref cult.rural_notable_notary, rural_notable_notary, "Rural notable notary:");
            AssignNPC(ref cult.shop_worker, shop_worker, "Shop worker:");
            AssignNPC(ref cult.barber, barber, "Barber:");
            AssignNPC(ref cult.beggar, beggar, "Beggar:");
            AssignNPC(ref cult.female_beggar, female_beggar, "Female beggar:");

            // Update 1.7.2
            DrawUILine(colUILine, 1, 6);

            AssignNPC(ref cult.basic_mercenary_troop, basic_mercenary_troop, "Basic Mercenary Troop:");
            AssignNPC(ref cult.bandit_boss, bandit_boss, "Bandit Boss:");
            AssignNPC(ref cult.bandit_chief, bandit_chief, "Bandit Chief:");
            AssignNPC(ref cult.bandit_bandit, bandit_bandit, "Bandit:");
            AssignNPC(ref cult.bandit_raider, bandit_raider, "Bandit Raider:");


        }

        DrawUILine(colUILine, 3, 12);

        DrawNamesEditor(ref cult.male_names, ref showMaleNamesEditor, ref m_soloName, ref m_name_TranslationString, ref translationStringMale_Names, 0);

        DrawUILine(colUILine, 3, 12);
        DrawNamesEditor(ref cult.female_names, ref showFemaleNamesEditor, ref f_soloName, ref f_name_TranslationString, ref translationStringFemale_Names, 1);

        DrawUILine(colUILine, 3, 12);
        DrawNamesEditor(ref cult.clan_names, ref showClanNamesEditor, ref cln_soloName, ref cln_name_TranslationString, ref translationStringClan_Names, 2);

        DrawUILine(colUILine, 3, 12);

        //DrawNPCTemplatesEditor(ref show_chld_template,ref cult.child_character_templates, ref child_character_templates, "Child Character", "#00bce4", "#0cb9c1");
        //DrawUILine(colUILine, 3, 12);

        DrawNPCTemplatesEditor(ref show_notable_and_wanderer_templates, ref cult.notable_and_wanderer_templates, ref notable_and_wanderer_templates, "Notable and Wanderer", "#00c16e", "#00a78e");
        DrawUILine(colUILine, 3, 12);

        DrawNPCTemplatesEditor(ref show_lord_templates, ref cult.lord_templates, ref lord_templates, "Lord", "#ffc845", "#f48924");
        DrawUILine(colUILine, 3, 12);

        DrawNPCTemplatesEditor(ref show_rebellion_hero_templates, ref cult.rebellion_hero_templates, ref rebellion_hero_templates, "Rebellion Hero", "#ff4f81", "#ff6c5f");

        DrawUILine(colUILine, 3, 12);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#5cc3e8", out newCol);

        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var showEditorLabel = "Hide";
        if (!showTournamentTemplatesEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Tournament Team Templates";

        }

        showTournamentTemplatesEditor = EditorGUILayout.Foldout(showTournamentTemplatesEditor, showEditorLabel, hiderStyle);

        if (showTournamentTemplatesEditor)
        {

            EditorGUILayout.LabelField("Tournament Team Templates", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);

            DrawUILine(colUILine, 1, 6);

            DrawTournamentTemplatesEditor(ref cult.TTT_one_participants, ref TTT_NPC_one, ref showTournamentEditorOne, 0);

            DrawUILine(colUILine, 3, 12);
            DrawTournamentTemplatesEditor(ref cult.TTT_two_participants, ref TTT_NPC_two, ref showTournamentEditorTwo, 1);

            DrawUILine(colUILine, 3, 12);
            DrawTournamentTemplatesEditor(ref cult.TTT_four_participants, ref TTT_NPC_four, ref showTournamentEditorFour, 2);

        }
        DrawUILine(colUILine, 3, 12);

        DrawCulturalFeatsEditor();

        DrawUILine(colUILine, 3, 12);

        DrawRewardItemsEditor(titleStyle, newCol);

        DrawUILine(colUILine, 3, 12);
        DrawMapIconsID(titleStyle, newCol);

        DrawUILine(colUILine, 3, 12);

        // object ID
        //---
        // 0 culture name
        // -1 culture text
        SetTextFieldTS(ref cult.text, ref soloText, ref textTranslationString, folder, translationStringDescription, "Culture Description Text:", cult.moduleID, cult, -1, cult.id);


    }

    void DrawNPCTemplatesEditor(ref bool show_template_editor, ref string[] templates_array, ref NPCCharacter[] characters, string editor_name, string colorA, string colorB)
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString(colorA, out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString(colorB, out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent(editor_name));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!show_template_editor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = editor_name + " Templates";
        }

        show_template_editor = EditorGUILayout.Foldout(show_template_editor, showEditorLabel, hiderStyle);

        if (show_template_editor)
        {



            EditorGUILayout.LabelField(editor_name + " Templates", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);
            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Template", "Add new " + editor_name + " template")), buttonStyle, GUILayout.Width(128)))
            {

                if (templates_array == null)
                    templates_array = new string[0];

                var temp = new string[templates_array.Length + 1];
                templates_array.CopyTo(temp, 0);
                templates_array = temp;

                templates_array[templates_array.Length - 1] = "";

                characters = new NPCCharacter[templates_array.Length];

                return;
            }


            if (templates_array != null && templates_array.Length > 0)
            {

                int i = 0;
                foreach (var targetAsset in templates_array)
                {
                    //Debug.Log(targetAsset);
                    GetNPCAsset(ref templates_array[i], ref characters[i], false);
                    if (characters[i] == null)
                    {
                        GetNPCAsset(ref templates_array[i], ref characters[i], true);
                    }
                    i++;
                }

                DrawUILine(colUILine, 3, 12);

                i = 0;
                foreach (var targetAsset in templates_array)
                {

                    // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


                    ColorUtility.TryParseHtmlString("#F25022", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 11;
                    EditorGUILayout.LabelField(editor_name + " - " + i, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                    ColorUtility.TryParseHtmlString("#FF9900", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 12;

                    string nameLabel = "None";
                    if (characters[i] != null)
                    {
                        nameLabel = characters[i].npcName;
                    }

                    RemoveTSString(ref nameLabel);

                    EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);

                    //EditorGUILayout.BeginHorizontal();
                    object npcTargetField = EditorGUILayout.ObjectField(characters[i], typeof(NPCCharacter), true, GUILayout.MaxWidth(320));
                    characters[i] = (NPCCharacter)npcTargetField;

                    if (characters[i] != null)
                    {
                        templates_array[i] = "NPCCharacter." + characters[i].id;
                    }
                    else
                    {
                        templates_array[i] = "";
                    }

                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("X", "Remove Template")), buttonStyle, GUILayout.Width(32)))
                    {

                        var count = templates_array.Length - 1;
                        var pt_troop = new string[count];


                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in templates_array)
                        {
                            if (i3 != i)
                            {
                                pt_troop[i2] = templates_array[i3];
                                i2++;
                            }
                            i3++;
                        }

                        templates_array = pt_troop;

                        characters = new NPCCharacter[templates_array.Length];

                        return;
                    }

                    DrawUILine(colUILine, 1, 4);
                    i++;
                }
            }
        }
    }
    void DrawTournamentTemplatesEditor(ref string[] tournamentTemplates, ref NPCCharacter[] npcObj, ref bool TTT_show, int editorID)
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        Color newCol;
        ColorUtility.TryParseHtmlString("#79ceb8", out newCol);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Participant: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!TTT_show)
        {
            hiderStyle.fontSize = 14;
            if (editorID == 0)
            {
                showEditorLabel = "One Participant";
            }
            else if (editorID == 1)
            {
                ColorUtility.TryParseHtmlString("#FFB900", out newCol);
                titleStyle.normal.textColor = newCol;

                showEditorLabel = "Two Participant";
            }
            else if (editorID == 2)
            {
                ColorUtility.TryParseHtmlString("#FFB900", out newCol);
                titleStyle.normal.textColor = newCol;
                showEditorLabel = "Four Participant";
            }
        }

        TTT_show = EditorGUILayout.Foldout(TTT_show, showEditorLabel, hiderStyle);

        if (TTT_show)
        {

            if (editorID == 0)
            {
                EditorGUILayout.LabelField("One Participant", titleStyle, GUILayout.ExpandWidth(true));
            }
            else if (editorID == 1)
            {
                EditorGUILayout.LabelField("Two Participant", titleStyle, GUILayout.ExpandWidth(true));

            }
            else if (editorID == 2)
            {
                EditorGUILayout.LabelField("Four Participant", titleStyle, GUILayout.ExpandWidth(true));

            }
            EditorGUILayout.Space(4);

            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Template", "Add tournament template participant")), buttonStyle, GUILayout.Width(128)))
            {
                var objects = new List<string>();

                int i2 = 0;
                foreach (string tmpl in tournamentTemplates)
                {
                    objects.Add(tmpl);
                    i2++;
                }

                int indexVal = objects.Count + 1;

                tournamentTemplates = new string[indexVal];

                i2 = 0;
                foreach (var element in objects)
                {
                    tournamentTemplates[i2] = element;
                    i2++;
                }

                tournamentTemplates[tournamentTemplates.Length - 1] = "";

                npcObj = new NPCCharacter[tournamentTemplates.Length];

            }
            DrawUILine(colUILine, 3, 12);

            if (tournamentTemplates == null)
                tournamentTemplates = new string[0];

            int i = 0;
            foreach (var targetAsset in tournamentTemplates)
            {
                // Debug.Log(npcObj.Length);
                GetNPCAsset(ref tournamentTemplates[i], ref npcObj[i], false);
                if (npcObj[i] == null)
                {
                    GetNPCAsset(ref tournamentTemplates[i], ref npcObj[i], true);
                }

                // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


                ColorUtility.TryParseHtmlString("#6a9c84", out newCol);
                titleStyle.normal.textColor = newCol;

                titleStyle.fontSize = 11;

                if (editorID == 0)
                {
                    EditorGUILayout.LabelField("One Participants Template -  " + (i + 1), titleStyle, GUILayout.ExpandWidth(true));
                }
                else if (editorID == 1)
                {
                    titleStyle.normal.textColor = newCol;
                    EditorGUILayout.LabelField("Two Participants Template -  " + (i + 1), titleStyle, GUILayout.ExpandWidth(true));

                }
                else if (editorID == 2)
                {
                    titleStyle.normal.textColor = newCol;
                    EditorGUILayout.LabelField("Four Participants Template -  " + (i + 1), titleStyle, GUILayout.ExpandWidth(true));

                }

                // EditorGUILayout.Space(8);
                ColorUtility.TryParseHtmlString("#acc236", out newCol);
                titleStyle.normal.textColor = newCol;

                titleStyle.fontSize = 12;

                string nameLabel = "None";
                if (npcObj[i] != null)
                {
                    nameLabel = npcObj[i].npcName;
                }

                RemoveTSString(ref nameLabel);

                EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
                // EditorGUILayout.Space(8);

                EditorGUILayout.BeginHorizontal();
                object UpgradeTargetField = EditorGUILayout.ObjectField(npcObj[i], typeof(NPCCharacter), true, GUILayout.MaxWidth(320));
                npcObj[i] = (NPCCharacter)UpgradeTargetField;

                if (npcObj[i] != null)
                {
                    tournamentTemplates[i] = "NPCCharacter." + npcObj[i].id;
                }
                else
                {
                    tournamentTemplates[i] = "";
                }


                buttonStyle.hover.textColor = Color.red;

                if (GUILayout.Button((new GUIContent("X", $"Remove {tournamentTemplates[i]} Participant Template")), buttonStyle, GUILayout.Width(32)))
                {
                    var objects = new List<string>();
                    tournamentTemplates[i] = "remove";

                    int i2 = 0;
                    foreach (string tmpl in tournamentTemplates)
                    {
                        if (tmpl != "remove")
                        {

                            objects.Add(tmpl);
                        }
                        i2++;
                    }

                    tournamentTemplates = new string[objects.Count];

                    i2 = 0;
                    foreach (var obj in objects)
                    {
                        tournamentTemplates[i2] = obj;
                        i2++;
                    }

                    npcObj = new NPCCharacter[tournamentTemplates.Length];

                    return;
                }

                EditorGUILayout.EndHorizontal();


                DrawUILine(colUILine, 1, 4);
                i++;
            }

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
                    assetPath = dataPath + cult.moduleID + "/_Templates/NPCtemplates/" + dataName + ".asset";
                    assetPathShort = "/_Templates/NPCtemplates/" + dataName + ".asset";
                }
                else
                {
                    assetPath = dataPath + cult.moduleID + "/NPC/" + dataName + ".asset";
                    assetPathShort = "/NPC/" + dataName + ".asset";
                }

                if (System.IO.File.Exists(assetPath))
                {
                    npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(NPCCharacter));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + cult.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
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

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
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
                                Debug.Log("NPCCharacter " + dataName + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");
                            }
                        }
                    }

                }
            }
        }
    }

    private void DrawNamesEditor(ref string[] names_container, ref bool showEditor, ref string[] obj_soloName, ref string[] name_TranslationString, ref TranslationString[] translationString_Names, int editor_type)
    {


        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        Color newCol;
        ColorUtility.TryParseHtmlString("#ffbb00", out newCol);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        titleStyle.normal.textColor = newCol;
        titleStyle.fontSize = 16;

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Male Name:  "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        ColorUtility.TryParseHtmlString("#00c16e", out newCol);
        hiderStyle.normal.textColor = newCol;

        var showEditorLabel = "Hide";
        if (!showEditor)
        {
            hiderStyle.fontSize = 16;
            if (editor_type == 0)
            {
                showEditorLabel = "Male Names Editor";
            }
            else if (editor_type == 1)
            {
                ColorUtility.TryParseHtmlString("#fd5c63", out newCol);
                hiderStyle.normal.textColor = newCol;
                showEditorLabel = "Female Names Editor";
            }
            else if (editor_type == 2)
            {
                ColorUtility.TryParseHtmlString("#fdbd10", out newCol);
                hiderStyle.normal.textColor = newCol;
                showEditorLabel = "Clan Names Editor";
            }
        }
        else
        {
            hiderStyle.fontSize = 10;
        }

        showEditor = EditorGUILayout.Foldout(showEditor, showEditorLabel, hiderStyle);

        // var SoloName = settl.LOC_complex_template.Replace("LocationComplexTemplate.", "");

        if (showEditor)
        {
            ColorUtility.TryParseHtmlString("#00c16e", out newCol);
            titleStyle.normal.textColor = newCol;


            if (editor_type == 0)
            {
                // ColorUtility.TryParseHtmlString("#00c16e", out newCol);
                // titleStyle.normal.textColor = newCol;
                showEditorLabel = "Male Names Editor";
            }
            else if (editor_type == 1)
            {
                ColorUtility.TryParseHtmlString("#fd5c63", out newCol);
                titleStyle.normal.textColor = newCol;
                showEditorLabel = "Female Names Editor";
            }
            else if (editor_type == 2)
            {
                ColorUtility.TryParseHtmlString("#fdbd10", out newCol);
                titleStyle.normal.textColor = newCol;
                showEditorLabel = "Clan Names Editor";
            }

            EditorGUILayout.LabelField(showEditorLabel, titleStyle);
            EditorGUILayout.Space(4);

            if (GUILayout.Button((new GUIContent("Add Name", "Add selected Name for this Culture")), buttonStyle, GUILayout.Width(128)))
            {
                int i2 = 0;

                // if (names_container.Length != 0)
                // {
                //     if (names_container[names_container.Length - 1] == "")
                //     {
                //         return;
                //     }
                // }

                var objects = new List<string>();

                i2 = 0;
                foreach (string nameAsset in names_container)
                {
                    objects.Add(nameAsset);
                    i2++;
                }

                int indexVal = objects.Count + 1;

                names_container = new string[indexVal];

                i2 = 0;
                foreach (var element in objects)
                {
                    names_container[i2] = element;
                    i2++;
                }

                names_container[names_container.Length - 1] = "";

                obj_soloName = new string[names_container.Length];
                name_TranslationString = new string[names_container.Length];
                translationString_Names = new TranslationString[names_container.Length];

            }


            DrawUILine(colUILine, 3, 12);


            titleStyle.fontSize = 12;

            if (names_container == null)
                names_container = new string[0];

            int i = 0;
            foreach (var _Name in names_container)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();

                string soloName = _Name;
                RemoveTSString(ref soloName);

                ColorUtility.TryParseHtmlString("#00c16e", out newCol);
                titleStyle.normal.textColor = newCol;
                titleStyle.fontSize = 11;

                if (soloName == "")
                {
                    ColorUtility.TryParseHtmlString("#a3a3a3", out newCol);
                    titleStyle.normal.textColor = newCol;
                    soloName = "None";
                }


                EditorGUILayout.BeginHorizontal();

                if (editor_type == 0)
                {
                    // ColorUtility.TryParseHtmlString("#1c79c0", out newCol);
                    // titleStyle.normal.textColor = newCol;
                    EditorGUILayout.LabelField("Male Name - " + (i + 1), titleStyle);
                }
                else if (editor_type == 1)
                {
                    if (soloName != "None")
                    {
                        ColorUtility.TryParseHtmlString("#fd5c63", out newCol);
                        titleStyle.normal.textColor = newCol;
                    }

                    EditorGUILayout.LabelField("Female Name - " + (i + 1), titleStyle);

                }
                else if (editor_type == 2)
                {
                    if (soloName != "None")
                    {
                        ColorUtility.TryParseHtmlString("#fdbd10", out newCol);
                        titleStyle.normal.textColor = newCol;
                    }
                    EditorGUILayout.LabelField("Clan Name - " + (i + 1), titleStyle);
                }
                else
                {
                    EditorGUILayout.LabelField("UNKLOWN Name - " + (i + 1), titleStyle);
                }



                if (GUILayout.Button((new GUIContent("X", $"Remove {soloName}")), buttonStyle, GUILayout.Width(32)))
                {
                    // Remove in dependencies



                    var names = new List<string>();
                    names_container[i] = "remove";

                    int i2 = 0;
                    foreach (string namesCult in names_container)
                    {
                        if (namesCult != "remove")
                        {
                            names.Add(names_container[i2]);
                        }
                        i2++;
                    }

                    names_container = new string[names.Count];

                    i2 = 0;
                    foreach (var obj in names)
                    {
                        names_container[i2] = names.ToArray()[i2];
                        i2++;
                    }

                    obj_soloName = new string[names_container.Length];
                    name_TranslationString = new string[names_container.Length];
                    translationString_Names = new TranslationString[names_container.Length];

                    return;
                }
                EditorGUILayout.EndHorizontal();


                titleStyle.fontSize = 12;

                ColorUtility.TryParseHtmlString("#fb8a2e", out newCol);
                titleStyle.normal.textColor = newCol;

                if (soloName == "None")
                {
                    ColorUtility.TryParseHtmlString("#505050", out newCol);
                    titleStyle.normal.textColor = newCol;
                    // soloName = "None";
                }

                if (editor_type == 0)
                {
                    // ColorUtility.TryParseHtmlString("#1c79c0", out newCol);
                    // titleStyle.normal.textColor = newCol;
                    EditorGUILayout.LabelField(soloName, titleStyle);
                }
                else if (editor_type == 1)
                {
                    if (soloName != "None")
                    {
                        ColorUtility.TryParseHtmlString("#61b3de", out newCol);
                        titleStyle.normal.textColor = newCol;
                    }
                    EditorGUILayout.LabelField(soloName, titleStyle);
                }
                else if (editor_type == 2)
                {
                    if (soloName != "None")
                    {
                        ColorUtility.TryParseHtmlString("#8ba753", out newCol);
                        titleStyle.normal.textColor = newCol;
                    }
                    EditorGUILayout.LabelField(soloName, titleStyle);
                }

                DrawUILine(colUILine, 1, 6);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();

                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.hover.textColor = Color.red;

                int ID = 123454321;
                if (editor_type == 0)
                {
                    ID = -2;
                }
                else if (editor_type == 1)
                {
                    ID = -3;

                }
                else if (editor_type == 2)
                {
                    ID = -4;
                }

                SetLabelCultureNamesFieldTS(ref names_container[i], ref obj_soloName[i], ref name_TranslationString[i], folder, translationString_Names[i], "Name:", cult.moduleID, cult, ID, cult.id, i);

                DrawUILine(colUILine, 3, 12);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                // DrawUILine(colUILine, 3, 12);


                i++;
            }




        }
    }
    private void DrawMapIconsID(GUIStyle titleStyle, Color newCol)
    {
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        ColorUtility.TryParseHtmlString("#ffc168", out newCol);

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;
        titleStyle.normal.textColor = newCol;

        var showEditorLabel = "Hide";
        if (!showMapIconsID)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Possible Clan Banner Icons";
        }

        showMapIconsID = EditorGUILayout.Foldout(showMapIconsID, showEditorLabel, hiderStyle);

        if (showMapIconsID)
        {
            EditorGUILayout.LabelField("Possible Clan Banner Icons", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);
            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Icon", "Add new possible clan banner icon to this Culture")), buttonStyle, GUILayout.Width(128)))
            {
                if (cult.banner_icon_id == null)
                    cult.banner_icon_id = new string[0];

                var temp = new string[cult.banner_icon_id.Length + 1];
                cult.banner_icon_id.CopyTo(temp, 0);
                cult.banner_icon_id = temp;

                cult.banner_icon_id[cult.banner_icon_id.Length - 1] = "";

                possible_clan_banner_icon_ids = new int[cult.banner_icon_id.Length];

                return;
            }


            if (cult.banner_icon_id != null && cult.banner_icon_id.Length > 0)
            {

                int i = 0;
                foreach (var targetAsset in cult.banner_icon_id)
                {
                    int.TryParse(targetAsset, out possible_clan_banner_icon_ids[i]);
                    i++;
                }

                DrawUILine(colUILine, 3, 12);

                i = 0;
                foreach (var targetAsset in cult.banner_icon_id)
                {

                    // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);
                    ColorUtility.TryParseHtmlString("#F25022", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 11;
                    EditorGUILayout.LabelField("Icon - " + i, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                    ColorUtility.TryParseHtmlString("#FF9900", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 12;

                    //EditorGUILayout.LabelField("Icon", titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);

                    EditorGUILayout.BeginHorizontal();
                    possible_clan_banner_icon_ids[i] = EditorGUILayout.IntField("ID:", possible_clan_banner_icon_ids[i], GUILayout.MaxWidth(192));

                    cult.banner_icon_id[i] = possible_clan_banner_icon_ids[i].ToString();

                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("X", "Remove Icon")), buttonStyle, GUILayout.Width(32)))
                    {
                        var count = cult.banner_icon_id.Length - 1;
                        var pt_troop = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in cult.banner_icon_id)
                        {
                            if (i3 != i)
                            {
                                pt_troop[i2] = cult.banner_icon_id[i3];

                                i2++;
                            }
                            i3++;
                        }

                        cult.banner_icon_id = pt_troop;

                        possible_clan_banner_icon_ids = new int[cult.banner_icon_id.Length];

                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    DrawUILine(colUILine, 1, 4);
                    i++;
                }
            }
        }

    }
    private void DrawRewardItemsEditor(GUIStyle titleStyle, Color newCol)
    {
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        ColorUtility.TryParseHtmlString("#00a78e", out newCol);

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;
        titleStyle.normal.textColor = newCol;

        var showEditorLabel = "Hide";
        if (!showRewardItems)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Vassal Reward Items Editor";
        }

        showRewardItems = EditorGUILayout.Foldout(showRewardItems, showEditorLabel, hiderStyle);

        if (showRewardItems)
        {
            EditorGUILayout.LabelField("Vassal Reward Items Editor", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);
            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Reward item", "Add new reward item to this Culture")), buttonStyle, GUILayout.Width(128)))
            {
                if (cult.reward_item_id == null)
                    cult.reward_item_id = new string[0];

                var temp = new string[cult.reward_item_id.Length + 1];
                cult.reward_item_id.CopyTo(temp, 0);
                cult.reward_item_id = temp;

                cult.reward_item_id[cult.reward_item_id.Length - 1] = "";

                vassal_reward_items = new Item[cult.reward_item_id.Length];

                return;
            }


            if (cult.reward_item_id != null && cult.reward_item_id.Length > 0)
            {

                int i = 0;
                foreach (var targetAsset in cult.reward_item_id)
                {
                    // Debug.Log(UpgradeTargets.Length);
                    GetItemAsset(ref cult.reward_item_id[i], ref vassal_reward_items[i]);
                    //if (vassal_reward_items[i] != null)
                    //{
                    //    GetItemAsset(ref cult.reward_item_id[i], ref vassal_reward_items[i]);
                    //}

                    i++;
                }

                DrawUILine(colUILine, 3, 12);

                i = 0;
                foreach (var targetAsset in cult.reward_item_id)
                {

                    // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


                    ColorUtility.TryParseHtmlString("#F25022", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 11;
                    EditorGUILayout.LabelField("Reward Item - " + i, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                    ColorUtility.TryParseHtmlString("#FF9900", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 12;

                    string nameLabel = "None";
                    if (vassal_reward_items[i] != null)
                    {
                        nameLabel = vassal_reward_items[i].itemName;
                    }

                    RemoveTSString(ref nameLabel);

                    EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);

                    EditorGUILayout.BeginHorizontal();
                    vassal_reward_items[i] = (Item)EditorGUILayout.ObjectField(vassal_reward_items[i], typeof(Item), true, GUILayout.MaxWidth(320));

                    if (vassal_reward_items[i] != null)
                    {
                        cult.reward_item_id[i] = "Item." + vassal_reward_items[i].id;
                    }
                    else
                    {
                        cult.reward_item_id[i] = "";
                    }

                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("X", "Remove Item")), buttonStyle, GUILayout.Width(32)))
                    {
                        var count = cult.reward_item_id.Length - 1;
                        var pt_troop = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in cult.reward_item_id)
                        {
                            if (i3 != i)
                            {
                                pt_troop[i2] = cult.reward_item_id[i3];

                                i2++;
                            }
                            i3++;
                        }

                        cult.reward_item_id = pt_troop;

                        vassal_reward_items = new Item[cult.reward_item_id.Length];

                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    DrawUILine(colUILine, 1, 4);
                    i++;
                }
            }
        }

    }

    private void GetItemAsset(ref string dataName, ref Item item)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {
            if (dataName.Contains("Item."))
                dataName = dataName.Replace("Item.", "");

            string assetPath = dataPath + cult.moduleID + "/Items/" + dataName + ".asset";
            string assetPathShort = "/Items/" + dataName + ".asset";


            if (System.IO.File.Exists(assetPath))
            {
                item = (Item)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Item));
            }
            else
            {
                // SEARCH IN DEPENDENCIES
                string modSett = modsSettingsPath + cult.moduleID + ".asset";
                ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                foreach (string dpdMod in currMod.modDependenciesInternal)
                {
                    string dpdPath = modsSettingsPath + dpdMod + ".asset";

                    if (System.IO.File.Exists(dpdPath))
                    {
                        string dpdAsset = dataPath + dpdMod + assetPathShort;

                        if (System.IO.File.Exists(dpdAsset))
                        {
                            item = (Item)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Item));
                            break;
                        }
                        else
                        {
                            item = null;
                        }

                    }
                }

                //Check is dependency OF
                if (item == null)
                {
                    string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                    foreach (string mod in mods)
                    {
                        ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                        foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                        {
                            if (depend == cult.moduleID)
                            {
                                foreach (var data in iSDependencyOfMod.modFilesData.itemsData.items)
                                {
                                    if (data != null)
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                            item = (Item)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Item));
                                            break;
                                        }
                                }
                            }
                        }
                    }

                    if (item == null)
                    {

                        Debug.Log("Item " + dataName + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");

                    }
                }

            }

        }
    }


    void DrawCulturalFeatsEditor()
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

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Cultural Feats: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showCulturalFeatsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Cultural Feats Editor";
        }

        showCulturalFeatsEditor = EditorGUILayout.Foldout(showCulturalFeatsEditor, showEditorLabel, hiderStyle);

        if (showCulturalFeatsEditor)
        {

            EditorGUILayout.LabelField("Cultural Feats Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            if (cult.cultural_feat_id == null || cult.cultural_feat_id.Length < settingsAsset.CulturalFeatsDefinitions.Length)
            {
                EditorGUILayout.BeginHorizontal();

                cultural_feats_index = EditorGUILayout.Popup("Feats:", cultural_feats_index, cultural_feats_options, GUILayout.Width(320));
                // kingd.policies[i] = skills_options[skills_index];


                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Cultural Feat", "Add selected Feat for this Culture")), buttonStyle, GUILayout.Width(128)))
                {
                    if (cult.cultural_feat_id == null)
                        cult.cultural_feat_id = new string[0];

                    var objects = new List<string>();

                    int i2 = 0;
                    foreach (string feat in cult.cultural_feat_id)
                    {
                        objects.Add(feat);
                        i2++;
                    }

                    int indexVal = objects.Count + 1;

                    cult.cultural_feat_id = new string[indexVal];

                    i2 = 0;
                    foreach (var element in objects)
                    {
                        cult.cultural_feat_id[i2] = element;
                        i2++;
                    }

                    cult.cultural_feat_id[cult.cultural_feat_id.Length - 1] = cultural_feats_options[cultural_feats_index];

                    CreateFeatsOptions(ref cultural_feats_options, ref cultural_feats_index, settingsAsset.CulturalFeatsDefinitions);
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
            if (cult.cultural_feat_id != null && cult.cultural_feat_id.Length != 0)
            {
                foreach (var feat in cult.cultural_feat_id)
                {
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    //string soloPolicyName = cult.cultural_feat_id[i].Replace("policy_", "");
                    EditorGUILayout.LabelField("Feat - " + cult.cultural_feat_id[i], titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button((new GUIContent("X", "Remove Feat")), buttonStyle, GUILayout.Width(32)))
                    {
                        var objects = new List<string>();
                        cult.cultural_feat_id[i] = "remove";

                        int i2 = 0;
                        foreach (string skillAsset in cult.cultural_feat_id)
                        {
                            if (skillAsset != "remove")
                            {
                                objects.Add(skillAsset);
                            }
                            i2++;
                        }

                        cult.cultural_feat_id = new string[objects.Count];

                        i2 = 0;
                        foreach (var obj in objects)
                        {
                            cult.cultural_feat_id[i2] = obj;
                            i2++;
                        }
                        CreateFeatsOptions(ref cultural_feats_options, ref cultural_feats_index, settingsAsset.CulturalFeatsDefinitions);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }
    private void CreateFeatsOptions(ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS


        var listOptionsAll = new List<string>();
        var listOptionsLoaded = new List<string>();

        foreach (var data in definitions)
        {
            //string soloPolicyName = data.Replace("policy_", "");
            listOptionsAll.Add(data);

        }

        if (cult.cultural_feat_id != null && cult.cultural_feat_id.Length > 0)
        {
            foreach (var dataTrait in cult.cultural_feat_id)
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
    private void SetLabelCultureNamesFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel, int nameID)
    {

        bool isDependencyTS = false;
        var labelStyle = new GUIStyle(EditorStyles.label);
        if (soloString == null || soloString == "")
        {
            EditorGUILayout.HelpBox($"Warning: Name field is empty.", MessageType.Warning);
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

                    foreach (string dpdMod in currMod.modDependenciesInternal)
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

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Name: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        soloString = EditorGUILayout.TextField(typeName, soloString);

        // Draw UI - & Edit Translation String 
        EditorGUILayout.BeginHorizontal();
        // translationStringID = EditorGUILayout.TextField("Translation String", translationStringID);

        if (TS == null)
        {
            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Translation String: (Unused)  "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
            EditorGUILayout.LabelField("Translation String: (Unused)", labelStyle);
        }
        else
        {
            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Translation String:  "));
            EditorGUIUtility.labelWidth = textDimensions.x;

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
            transEditor.nameID = nameID;

            if (objID == -2)
            {
                transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - Male Name - )";
            }
            if (objID == -3)
            {
                transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - Female Name - )";
            }
            if (objID == -4)
            {
                transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - Clan Name - )";
            }
            // transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - )";

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
    private void AssignPT(ref string inputString, PartyTemplate PT, string fieldLabel)
    {
        // Party Template
        if (inputString != null && inputString != "")
        {
            if (inputString.Contains("PartyTemplate."))
            {
                string dataName = inputString.Replace("PartyTemplate.", "");
                string asset = dataPath + cult.moduleID + "/PartyTemplates/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    PT = (PartyTemplate)AssetDatabase.LoadAssetAtPath(asset, typeof(PartyTemplate));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    // Debug.Log(cult.moduleID);
                    string modSett = modsSettingsPath + cult.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/PartyTemplates/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                PT = (PartyTemplate)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(PartyTemplate));
                                break;
                            }
                            else
                            {
                                PT = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (PT == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
                                {
                                    if (iSDependencyOfMod.modFilesData != null)
                                    {
                                        foreach (var data in iSDependencyOfMod.modFilesData.PTdata.partyTemplates)
                                        {
                                            if (data.id == dataName)
                                            {
                                                string dpdAsset = dataPath + iSDependencyOfMod.id + "/PartyTemplates/" + dataName + ".asset";
                                                PT = (PartyTemplate)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(PartyTemplate));
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        if (PT == null)
                        {
                            Debug.Log("PartyTemplate " + dataName + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");

                        }

                    }
                }

            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(fieldLabel, EditorStyles.label);
        object PTfield = EditorGUILayout.ObjectField(PT, typeof(PartyTemplate), true);
        PT = (PartyTemplate)PTfield;

        if (PT != null)
        {
            inputString = "PartyTemplate." + PT.id;
        }
        else
        {
            inputString = "";
        }
        GUILayout.EndHorizontal();
    }

    private void SetLabelFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel)
    {

        bool isDependencyTS = false;
        var labelStyle = new GUIStyle(EditorStyles.label);
        if (soloString == null || soloString == "")
        {
            EditorGUILayout.HelpBox(typeName + " field is empty", MessageType.Warning);
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

                    foreach (string dpdMod in currMod.modDependenciesInternal)
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

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.translationData.translationStrings)
                                    {
                                        if (data.id == TS_Name)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";
                                            TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(TranslationString));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (TS == null)
                        {
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");

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
    private void SetTextFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel)
    {
        bool isDependencyTS = false;
        var labelStyle = new GUIStyle(EditorStyles.label);

        EditorGUILayout.LabelField(typeName, EditorStyles.boldLabel);

        if (soloString == null || soloString == "")
        {
            EditorGUILayout.HelpBox(typeName + " field is empty", MessageType.Warning);
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

                    foreach (string dpdMod in currMod.modDependenciesInternal)
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

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.translationData.translationStrings)
                                    {
                                        if (data.id == TS_Name)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";
                                            TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(TranslationString));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (TS == null)
                        {
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");

                        }

                    }
                }
            }
        }

        // soloString = EditorGUILayout.TextField(typeName, soloString);

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

        textScrollPos = EditorGUILayout.BeginScrollView(textScrollPos, GUILayout.Height(128));
        // draw text
        GUILayout.BeginVertical();
        soloString = GUILayout.TextArea(soloString);
        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();


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

    public void AssignNPC(ref string npc, NPCCharacter npcObject, string fieldName)
    {


        if (npc != null && npc != "" && npcObject == null)
        {
            // npcObject = new NPCCharacter();
            if (npc.Contains("NPCCharacter."))
            {
                string dataName = npc.Replace("NPCCharacter.", "");
                string asset = dataPath + cult.moduleID + "/NPC/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    npcObject = (NPCCharacter)AssetDatabase.LoadAssetAtPath(asset, typeof(NPCCharacter));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + cult.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/NPC/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                npcObject = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
                                break;
                            }
                            else
                            {
                                npcObject = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (npcObject == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/NPC/" + dataName + ".asset";
                                            npcObject = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (npcObject == null)
                        {
                            Debug.Log("NPCCharacter " + dataName + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }

            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(fieldName, EditorStyles.label);
        object field = EditorGUILayout.ObjectField(npcObject, typeof(NPCCharacter), true);
        npcObject = (NPCCharacter)field;

        if (npcObject != null)
        {
            npc = "NPCCharacter." + npcObject.id;
        }
        else
        {
            npc = "";

        }
        GUILayout.EndHorizontal();
    }

    public void AssignEquipments(ref string equip, Equipment equipObject, string fieldName)
    {

        if (equip != null && equip != "" && equipObject == null)
        {
            // npcObject = new NPCCharacter();
            if (equip.Contains("EquipmentRoster."))
            {

                string dataName = equip.Replace("EquipmentRoster.", "");
                string asset = dataPath + cult.moduleID + "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                string assetPathSets = dataPath + cult.moduleID + "/Sets/Equipments/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    equipObject = (Equipment)AssetDatabase.LoadAssetAtPath(asset, typeof(Equipment));
                }
                else if (System.IO.File.Exists(assetPathSets))
                {
                    equipObject = (Equipment)AssetDatabase.LoadAssetAtPath(assetPathSets, typeof(Equipment));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + cult.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                            string dpdAssetSets = dataPath + dpdMod + "/Sets/Equipments/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                equipObject = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Equipment));
                                break;
                            }
                            else if (System.IO.File.Exists(dpdAssetSets))
                            {
                                equipObject = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAssetSets, typeof(Equipment));
                            }
                            else
                            {
                                equipObject = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (equipObject == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == cult.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                                            string dpdAssetSets = dataPath + iSDependencyOfMod.id + "/Sets/Equipments/" + dataName + ".asset";

                                            if (System.IO.File.Exists(dpdAsset))
                                            {
                                                equipObject = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Equipment));
                                                break;
                                            }
                                            else if (System.IO.File.Exists(dpdAssetSets))
                                            {
                                                equipObject = (Equipment)AssetDatabase.LoadAssetAtPath(dpdAssetSets, typeof(Equipment));
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (equipObject == null)
                        {
                            Debug.Log("Equipment " + dataName + " - Not EXIST in" + " ' " + cult.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }

            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(fieldName, EditorStyles.label);
        equipObject = (Equipment)EditorGUILayout.ObjectField(equipObject, typeof(Equipment), true);
        //equipObject = (Equipment)field;

        if (equipObject != null)
        {
            equip = "EquipmentRoster." + equipObject.id;
        }
        else
        {
            equip = "";

        }
        GUILayout.EndHorizontal();
    }
    public static AssetsDataManager ADM_Instance
    {
        get { return EditorWindow.GetWindow<AssetsDataManager>(); }
    }

    public static BannerEditor BANNER_EDITOR_Instance
    {
        get { return EditorWindow.GetWindow<BannerEditor>(); }
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
}
