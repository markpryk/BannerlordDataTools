using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

[CustomEditor(typeof(Faction))]
public class FactionAssetEditor : Editor
{

    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string folder = "FactionsTranslationData";

    Faction fac;
    string soloName;
    string soloText;
    string nameTranslationString;
    string textTranslationString;
    TranslationString translationStringName;
    TranslationString translationStringDescription;

    public bool is_minor_faction;
    public bool is_mafia;
    public bool is_outlaw;
    public bool is_bandit;

    public Hero factionOwner;
    public Kingdom superFaction;

    public PartyTemplate partyTemplate;
    public Culture cultureIs;

    // update 1.7.2

    public string solo_short_name;
    string solo_short_name_TS;
    TranslationString translationStringShortName;

    public bool is_clan_type_mercenary;
    public bool is_sect;
    public bool is_nomad;

    // update 1.8.0
    public bool is_noble;

    bool showMinorFacTemplates;
    public NPCCharacter[] minor_fac_char_templates;

    // Relations
    bool showRelationshipsEditor;

    Kingdom[] KingdomRelation;
    //bool[] IsAtWar_Bool;

    string _changed_Kingdom;
    Kingdom _changed_OLD_Kingdom;
    bool _IS_changed_Kingdom;

    Faction[] FactionRelation;
    bool showFactionsRelationshipsEditor;

    string _changed_Faction;
    Faction _changed_OLD_Faction;
    bool _IS_changed_Faction;

    // Relations - End

    Color labelColor;

    Color color_a;

    Color color_b;
    Color alt_color_a;

    Color alt_color_b;

    Vector2 textScrollPos;
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    bool isDependency = false;
    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";
    public void OnEnable()
    {
        fac = (Faction)target;
        EditorUtility.SetDirty(fac);

        if (fac.minor_faction_character_templates != null && fac.minor_faction_character_templates.Length > 0)
            minor_fac_char_templates = new NPCCharacter[fac.minor_faction_character_templates.Length];
        else
            minor_fac_char_templates = new NPCCharacter[0];

        if (fac.fac_relationships != null && fac.fac_relationships.Length > 0)
            FactionRelation = new Faction[fac.fac_relationships.Length];

        if (fac.relationships != null && fac.relationships.Length > 0)
            KingdomRelation = new Kingdom[fac.relationships.Length];
        //else
        //    FactionRelation = new Faction[0];

    }

    public override void OnInspectorGUI()
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

        if (settingsAsset.currentModule != fac.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == fac.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + fac.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + fac.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + fac.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + fac.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + fac.moduleID + ".asset", typeof(ModuleReceiver));
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
                        assetMng.objID = 2;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = fac;

                        assetMng.assetName_org = fac.id;
                        assetMng.assetName_new = fac.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 2;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = fac;

                        assetMng.assetName_org = fac.id;
                        assetMng.assetName_new = fac.id;
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

        // fac.XmlFileName = EditorGUILayout.TextField("Xml File name", fac.XmlFileName);


        EditorGUI.BeginDisabledGroup(true);
        fac.id = EditorGUILayout.TextField("Faction ID", fac.id);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {

            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 2;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = fac;

                assetMng.assetName_org = fac.id;
                assetMng.assetName_new = fac.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 2;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = fac;

                assetMng.assetName_org = fac.id;
                assetMng.assetName_new = fac.id;
            }

        }


        // color labels
        var labelStyle = new GUIStyle(EditorStyles.label);

        // 2 faction name
        // 21 faction text
        // 22 faction short name
        // faction name & translationString tag
        DrawUILine(colUILine, 3, 12);

        SetLabelFieldTS(ref fac.factionName, ref soloName, ref nameTranslationString, folder, translationStringName, "Faction Name:", fac.moduleID, fac, 2, fac.id);
        GUILayout.Space(6);
        SetLabelFieldTS(ref fac.short_name, ref solo_short_name, ref solo_short_name_TS, folder, translationStringShortName, "Short Name:", fac.moduleID, fac, 22, fac.id);

        DrawUILine(colUILine, 3, 12);
        // -------------- Faction Name END


        // DrawUILine(colUILine, 3, 12);

        var originLabelWidth = EditorGUIUtility.labelWidth;
        Vector2 textDimensions;
        // is Minor faction

        GUILayout.BeginHorizontal();
        CreateToggleFaction(ref fac.is_minor_faction, ref is_minor_faction, "Is Minor Faction: ");
        DrawUILineVertical(colUILine, 1, 1, 16);

        CreateToggleFaction(ref fac.is_outlaw, ref is_outlaw, "Is Outlaw: ");
        DrawUILineVertical(colUILine, 1, 1, 16);

        CreateToggleFaction(ref fac.is_bandit, ref is_bandit, "Is Bandit: ");
        DrawUILineVertical(colUILine, 1, 1, 16);

        CreateToggleFaction(ref fac.is_mafia, ref is_mafia, "Is Mafia: ");
        GUILayout.EndHorizontal();
        DrawUILine(colUILine, 3, 12);
        GUILayout.BeginHorizontal();
        CreateToggleFaction(ref fac.is_clan_type_mercenary, ref is_clan_type_mercenary, "Is Clan Type Mercenary: ");
        DrawUILineVertical(colUILine, 1, 1, 16);

        CreateToggleFaction(ref fac.is_sect, ref is_sect, "Is Sect: ");
        DrawUILineVertical(colUILine, 1, 1, 16);

        CreateToggleFaction(ref fac.is_nomad, ref is_nomad, "Is Nomad: ");

        DrawUILineVertical(colUILine, 1, 1, 16);

        CreateToggleFaction(ref fac.is_noble, ref is_noble, "Is Noble: ");

        GUILayout.EndHorizontal();

        EditorGUILayout.Space(4);


        // Initial world position
        if (fac.is_minor_faction == "true")
        {
            DrawUILine(colUILine, 3, 12);

            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Initial World Position X         "));
            EditorGUIUtility.labelWidth = textDimensions.x;
            float posValueX;

            float.TryParse(fac.initial_posX, out posValueX);

            posValueX = EditorGUILayout.FloatField("Initial World Position X", posValueX, GUILayout.MaxWidth(288));

            fac.initial_posX = posValueX.ToString();

            float posValueY;

            float.TryParse(fac.initial_posY, out posValueY);

            posValueY = EditorGUILayout.FloatField("Initial World Position Y", posValueY, GUILayout.MaxWidth(288));

            fac.initial_posY = posValueY.ToString();

            // EditorGUIUtility.labelWidth = originLabelWidth;

            // fac.initial_posX = EditorGUILayout.TextField("Initial World Position X", fac.initial_posX);
            // fac.initial_posY = EditorGUILayout.TextField("Initial World Position Y", fac.initial_posY); 
        }


        DrawUILine(colUILine, 3, 12);

        EditorGUIUtility.labelWidth = originLabelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Faction Tier:    "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        // Clan Tier
        // fac.tier = EditorGUILayout.TextField("Faction Tier:", fac.tier);

        int tierValue;

        int.TryParse(fac.tier, out tierValue);

        tierValue = EditorGUILayout.IntField("Faction Tier:", tierValue, GUILayout.Width(128));

        fac.tier = tierValue.ToString();

        EditorGUIUtility.labelWidth = originLabelWidth;

        //Culture
        // fac.culture = EditorGUILayout.TextField("Faction Culture:", fac.culture);
        // CULTURE
        if (fac.culture != null && fac.culture != "")
        {
            if (fac.culture.Contains("Culture."))
            {
                string dataName = fac.culture.Replace("Culture.", "");
                string asset = dataPath + fac.moduleID + "/Cultures/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(asset, typeof(Culture));
                }
                else
                {

                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + fac.moduleID + ".asset";
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
                                if (depend == fac.moduleID)
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
                            Debug.Log("Culture " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");

                        }

                    }
                }
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Faction Culture:", EditorStyles.label);
        object cultureField = EditorGUILayout.ObjectField(cultureIs, typeof(Culture), true);
        cultureIs = (Culture)cultureField;

        if (cultureIs != null)
        {
            fac.culture = "Culture." + cultureIs.id;
        }
        else
        {
            fac.culture = "";
        }
        GUILayout.EndHorizontal();

        //CULTURE END

        // owner
        if (fac.owner != null && fac.owner != "")
        {
            if (fac.owner.Contains("Hero."))
            {

                string dataName = fac.owner.Replace("Hero.", "");
                string asset = dataPath + fac.moduleID + "/Heroes/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    factionOwner = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + fac.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                factionOwner = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                break;
                            }
                            else
                            {
                                factionOwner = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (factionOwner == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == fac.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            factionOwner = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (factionOwner == null)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");

                        }

                    }
                }

            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Faction Owner:", EditorStyles.label);
        object ownerField = EditorGUILayout.ObjectField(factionOwner, typeof(Hero), true);
        factionOwner = (Hero)ownerField;
        if (factionOwner != null)
        {
            fac.owner = "Hero." + factionOwner.id;
        }
        else
        {
            fac.owner = "";
        }
        GUILayout.EndHorizontal();

        // Super Faction

        if (fac.super_faction != null && fac.super_faction != "")
        {
            if (fac.super_faction.Contains("Kingdom."))
            {

                string dataName = fac.super_faction.Replace("Kingdom.", "");
                string asset = dataPath + fac.moduleID + "/Kingdoms/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    superFaction = (Kingdom)AssetDatabase.LoadAssetAtPath(asset, typeof(Kingdom));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + fac.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Kingdoms/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                superFaction = (Kingdom)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Kingdom));
                                break;
                            }
                            else
                            {
                                superFaction = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (superFaction == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == fac.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.kingdomsData.kingdoms)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Kingdoms/" + dataName + ".asset";
                                            superFaction = (Kingdom)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Kingdom));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (superFaction == null)
                        {
                            Debug.Log("Kingdom " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }

            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Kingdom:", EditorStyles.label);
        object superFacField = EditorGUILayout.ObjectField(superFaction, typeof(Kingdom), true);
        superFaction = (Kingdom)superFacField;
        if (superFaction != null)
        {
            fac.super_faction = "Kingdom." + superFaction.id;
        }
        else
        {
            fac.super_faction = "";
        }
        GUILayout.EndHorizontal();

        // fac.default_party_template = EditorGUILayout.TextField("Default PartyTemplate:", fac.default_party_template);

        // Party Template
        if (fac.default_party_template != null && fac.default_party_template != "")
        {
            if (fac.default_party_template.Contains("PartyTemplate."))
            {
                string dataName = fac.default_party_template.Replace("PartyTemplate.", "");
                string asset = dataPath + fac.moduleID + "/PartyTemplates/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    partyTemplate = (PartyTemplate)AssetDatabase.LoadAssetAtPath(asset, typeof(PartyTemplate));
                }
                else
                {
                    string modSett = modsSettingsPath + fac.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/PartyTemplates/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                partyTemplate = (PartyTemplate)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(PartyTemplate));
                                break;
                            }
                            else
                            {
                                partyTemplate = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (partyTemplate == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == fac.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.PTdata.partyTemplates)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/PartyTemplates/" + dataName + ".asset";
                                            partyTemplate = (PartyTemplate)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(PartyTemplate));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (partyTemplate == null)
                        {
                            Debug.Log("PartyTemplate " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Default Party Template:", EditorStyles.label);
        object PTfield = EditorGUILayout.ObjectField(partyTemplate, typeof(PartyTemplate), true);
        partyTemplate = (PartyTemplate)PTfield;

        if (partyTemplate != null)
        {
            fac.default_party_template = "PartyTemplate." + partyTemplate.id;
        }
        else
        {
            fac.default_party_template = "";
        }
        GUILayout.EndHorizontal();

        DrawUILine(colUILine, 2, 12);

        // settlement banner mesh
        fac.settlement_banner_mesh = EditorGUILayout.TextField("Settlement Banner Mesh:", fac.settlement_banner_mesh);
       
        DrawUILine(colUILine, 2, 12);

        fac.flag_mesh = EditorGUILayout.TextField("Flag Mesh:", fac.flag_mesh);


        DrawUILine(colUILine, 3, 12);

        // banner key

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Banner Key:    "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        fac.banner_key = EditorGUILayout.TextField("Banner Key:", fac.banner_key);

        EditorGUIUtility.labelWidth = originLabelWidth;

        GUILayout.Space(4);

        if (GUILayout.Button("Edit Banner", GUILayout.Width(80)))
        {
            if (BANNER_EDITOR_Instance == null)
            {
                BannerEditor assetMng = (BannerEditor)ScriptableObject.CreateInstance(typeof(BannerEditor));
                assetMng.bannerKey = fac.banner_key;
                //assetMng.ReadBannerKey();
                assetMng.inputNPC = null;
                assetMng.inputKingdom = null;
                assetMng.inputFaction = fac;
                assetMng.inputCulture = null;
            }
            else
            {
                BannerEditor assetMng = BANNER_EDITOR_Instance;
                assetMng.bannerKey = fac.banner_key;
                //assetMng.ReadBannerKey();
                assetMng.inputNPC = null;
                assetMng.inputKingdom = null;
                assetMng.inputFaction = fac;
                assetMng.inputCulture = null;
            }
        }

        DrawUILine(colUILine, 3, 12);

        /// COLORS START

        if (fac.label_color != null && fac.label_color != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + fac.label_color, out labelColor);
            labelStyle.normal.textColor = new Color(labelColor.r, labelColor.g, labelColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Label Color:", labelStyle);
            labelColor = EditorGUILayout.ColorField(labelColor);
            GUILayout.EndHorizontal();
            fac.label_color = ColorUtility.ToHtmlStringRGBA(labelColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (labelColor == new Color(0, 0, 0, 0))
            {
                fac.label_color = "";
            }
        }
        else
        {
            if (labelColor == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Label Color: (Unused)", labelStyle);
                labelColor = EditorGUILayout.ColorField(labelColor);
                GUILayout.EndHorizontal();
                fac.label_color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(labelColor.r, labelColor.g, labelColor.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Label Color:", labelStyle);
                labelColor = EditorGUILayout.ColorField(labelColor);
                GUILayout.EndHorizontal();
                fac.label_color = ColorUtility.ToHtmlStringRGBA(labelColor);
            }
        }

        // color A
        labelStyle = new GUIStyle(EditorStyles.label);

        if (fac.color != null && fac.color != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + fac.color, out color_a);
            labelStyle.normal.textColor = new Color(color_a.r, color_a.g, color_a.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Faction Color 1:", labelStyle);
            color_a = EditorGUILayout.ColorField(color_a);
            GUILayout.EndHorizontal();
            fac.color = ColorUtility.ToHtmlStringRGBA(color_a);
            // Debug.Log("Color Parsed: " + labelColor);

            if (color_a == new Color(0, 0, 0, 0))
            {
                fac.color = "";
            }
        }
        else
        {
            if (color_a == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Faction Color 1: (Unused)", labelStyle);
                color_a = EditorGUILayout.ColorField(color_a);
                GUILayout.EndHorizontal();
                fac.color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(color_a.r, color_a.g, color_a.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Faction Color 1:", labelStyle);
                color_a = EditorGUILayout.ColorField(color_a);
                GUILayout.EndHorizontal();
                fac.color = ColorUtility.ToHtmlStringRGBA(color_a);
            }
        }

        // color B
        labelStyle = new GUIStyle(EditorStyles.label);

        if (fac.color2 != null && fac.color2 != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + fac.color2, out color_b);
            labelStyle.normal.textColor = new Color(color_b.r, color_b.g, color_b.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Faction Color 2:", labelStyle);
            color_b = EditorGUILayout.ColorField(color_b);
            GUILayout.EndHorizontal();
            fac.color2 = ColorUtility.ToHtmlStringRGBA(color_b);
            // Debug.Log("Color Parsed: " + labelColor);

            if (color_b == new Color(0, 0, 0, 0))
            {
                fac.color2 = "";
            }
        }
        else
        {
            if (color_b == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Faction Color 2: (Unused)", labelStyle);
                color_b = EditorGUILayout.ColorField(color_b);
                GUILayout.EndHorizontal();
                fac.color2 = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(color_b.r, color_b.g, color_b.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Faction Color 2:", labelStyle);
                color_b = EditorGUILayout.ColorField(color_b);
                GUILayout.EndHorizontal();
                fac.color2 = ColorUtility.ToHtmlStringRGBA(color_b);
            }
        }

        // alternative Color A
        labelStyle = new GUIStyle(EditorStyles.label);

        if (fac.alternative_color != null && fac.alternative_color != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + fac.alternative_color, out alt_color_a);
            labelStyle.normal.textColor = new Color(alt_color_a.r, alt_color_a.g, alt_color_a.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alternative Faction Color 1:", labelStyle);
            alt_color_a = EditorGUILayout.ColorField(alt_color_a);
            GUILayout.EndHorizontal();
            fac.alternative_color = ColorUtility.ToHtmlStringRGBA(alt_color_a);
            // Debug.Log("Color Parsed: " + labelColor);

            if (alt_color_a == new Color(0, 0, 0, 0))
            {
                fac.alternative_color = "";
            }
        }
        else
        {
            if (alt_color_a == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Faction Color 1: (Unused)", labelStyle);
                alt_color_a = EditorGUILayout.ColorField(alt_color_a);
                GUILayout.EndHorizontal();
                fac.alternative_color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(alt_color_a.r, alt_color_a.g, alt_color_a.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Faction Color 1:", labelStyle);
                alt_color_a = EditorGUILayout.ColorField(alt_color_a);
                GUILayout.EndHorizontal();
                fac.alternative_color = ColorUtility.ToHtmlStringRGBA(alt_color_a);
            }
        }

        // alternative Color B
        labelStyle = new GUIStyle(EditorStyles.label);

        if (fac.alternative_color2 != null && fac.alternative_color2 != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + fac.alternative_color2, out alt_color_b);
            labelStyle.normal.textColor = new Color(alt_color_b.r, alt_color_b.g, alt_color_b.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alternative Faction Color 1:", labelStyle);
            alt_color_b = EditorGUILayout.ColorField(alt_color_b);
            GUILayout.EndHorizontal();
            fac.alternative_color2 = ColorUtility.ToHtmlStringRGBA(alt_color_b);
            // Debug.Log("Color Parsed: " + labelColor);

            if (alt_color_b == new Color(0, 0, 0, 0))
            {
                fac.alternative_color2 = "";
            }
        }
        else
        {
            if (alt_color_b == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Faction Color 1: (Unused)", labelStyle);
                alt_color_b = EditorGUILayout.ColorField(alt_color_b);
                GUILayout.EndHorizontal();
                fac.alternative_color2 = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(alt_color_b.r, alt_color_b.g, alt_color_b.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Faction Color 1:", labelStyle);
                alt_color_b = EditorGUILayout.ColorField(alt_color_b);
                GUILayout.EndHorizontal();
                fac.alternative_color2 = ColorUtility.ToHtmlStringRGBA(alt_color_b);
            }
        }

        labelStyle = new GUIStyle(EditorStyles.label);
        /// COLORS END
        /// 

        // Relations
        DrawUILine(colUILine, 3, 12);
        DrawKingdomRelationshipsEditor();
        DrawUILine(colUILine, 3, 12);
        DrawFactionRelationshipsEditor();

        if (is_minor_faction)
            DrawMinorTemplatesEditor();

        DrawUILine(colUILine, 3, 12);

        // 2 faction name
        // 21 faction text
        // 22 faction short name

        SetTextFieldTS(ref fac.text, ref soloText, ref textTranslationString, folder, translationStringDescription, "Faction Description Text:", fac.moduleID, fac, 21, fac.id);
        DrawUILine(colUILine, 3, 12);

        // EditorGUILayout.EndToggleGroup();
        // EditorGUI.EndDisabledGroup();
    }
    void DrawKingdomRelationshipsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#faae40", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#f38020", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Relationships: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showRelationshipsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Kingdoms Relationships";
        }

        showRelationshipsEditor = EditorGUILayout.Foldout(showRelationshipsEditor, showEditorLabel, hiderStyle);

        if (showRelationshipsEditor)
        {

            EditorGUILayout.LabelField("Kingdoms Relationships", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            // if (kingd.relationships == null)
            // {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((new GUIContent("Add Relation", "Add relation between Kingdoms")), buttonStyle, GUILayout.Width(128)))
            {
                if (fac.relationships == null)
                    fac.relationships = new string[0];

                if (fac.relationValues == null)
                    fac.relationValues = new string[0];

                var temp = new string[fac.relationships.Length + 1];
                fac.relationships.CopyTo(temp, 0);
                fac.relationships = temp;

                fac.relationships[fac.relationships.Length - 1] = "";

                temp = new string[fac.relationValues.Length + 1];
                fac.relationValues.CopyTo(temp, 0);
                fac.relationValues = temp;

                fac.relationValues[fac.relationValues.Length - 1] = "0";

                //temp = new string[fac.relationsAtWar.Length + 1];
                //fac.relationsAtWar.CopyTo(temp, 0);
                //fac.relationsAtWar = temp;

                //fac.relationsAtWar[fac.relationsAtWar.Length - 1] = "false";

                KingdomRelation = new Kingdom[fac.relationships.Length];
                //IsAtWar_Bool = new bool[fac.relationsAtWar.Length];

                //if (fac.relationsAtWar != null && fac.relationsAtWar.Length != 0)
                //{
                //    int i2 = 0;
                //    foreach (var rel in fac.relationsAtWar)
                //    {
                //        if (rel == "true")
                //        {
                //            IsAtWar_Bool[i2] = true;
                //        }
                //        else
                //        {
                //            IsAtWar_Bool[i2] = false;
                //        }
                //        i2++;
                //    }
                //}

                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            // DrawUILine(colUILine, 3, 12);
            // }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (fac.relationships != null && fac.relationships.Length != 0)
            {
                foreach (var relation in fac.relationships)
                {
                    GetKingdomAsset(ref fac.relationships[i], ref KingdomRelation[i]);
                    i++;
                }

                CheckKingdomLinkedData();

                i = 0;
                foreach (var relation in fac.relationships)
                {

                    if (KingdomRelation[i] != null)
                    {
                        _changed_OLD_Kingdom = KingdomRelation[i];
                    }
                    else
                    {
                        _changed_OLD_Kingdom = null;
                    }

                    // GetKingdomAsset(ref kingd.relationships[i], ref KingdomRelation[i]);

                    // ColorUtility.TryParseHtmlString("#F65314", out newCol2);
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    // GetKingdomAsset(ref kingd.relationships[i], ref KingdomRelation[i]);


                    if (KingdomRelation[i] != null)
                    {
                        ColorUtility.TryParseHtmlString("#" + KingdomRelation[i].color, out newCol2);
                        titleStyle.normal.textColor = new Color(newCol2.r, newCol2.g, newCol2.b, 1);

                        string kingdom_soloName = KingdomRelation[i].kingdomName;
                        RemoveTSString(ref kingdom_soloName);
                        EditorGUILayout.LabelField(kingdom_soloName, titleStyle, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        ColorUtility.TryParseHtmlString("#6a737b", out newCol2);
                        titleStyle.normal.textColor = newCol2;

                        EditorGUILayout.LabelField("None", titleStyle, GUILayout.ExpandWidth(true));
                    }

                    EditorGUILayout.Space(2);

                    EditorGUI.BeginChangeCheck();

                    //CreateAttributeToggle(ref IsAtWar_Bool[i], ref fac.relationsAtWar[i], " Is At War");

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (KingdomRelation[i] != null)
                        {
                            _changed_Kingdom = KingdomRelation[i].id;
                            _IS_changed_Kingdom = true;
                        }

                    }
                    EditorGUILayout.Space(3);


                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Kingdom: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    // EditorGUILayout.LabelField("Kingdom:", EditorStyles.label, GUILayout.ExpandWidth(false));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    object kingdRelField = EditorGUILayout.ObjectField(KingdomRelation[i], typeof(Kingdom), true, GUILayout.MaxWidth(192));
                    KingdomRelation[i] = (Kingdom)kingdRelField;

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (KingdomRelation[i] != null)
                        {
                            _changed_Kingdom = KingdomRelation[i].id;
                            _IS_changed_Kingdom = true;
                        }

                    }

                    if (KingdomRelation[i] != null)
                    {
                        fac.relationships[i] = "Kingdom." + KingdomRelation[i].id;
                    }
                    else
                    {
                        fac.relationships[i] = "";
                    }

                    EditorGUI.BeginChangeCheck();

                    CreateIntAttribute(ref fac.relationValues[i], "Relation Value:");

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (KingdomRelation[i] != null)
                        {
                            _changed_Kingdom = KingdomRelation[i].id;
                            _IS_changed_Kingdom = true;
                        }

                    }

                    // EditorGUILayout.Space();

                    if (GUILayout.Button((new GUIContent("X", "Remove Relation")), buttonStyle, GUILayout.Width(32)))
                    {
                        if (KingdomRelation[i] != null)
                        {
                            foreach (var rel in KingdomRelation[i].fac_relationships)
                            {
                                if (rel != null && rel == "Faction." + fac.id)
                                {
                                    var dpd_count = 0;

                                    if (KingdomRelation[i].fac_relationships.Length != 0)
                                        dpd_count = KingdomRelation[i].fac_relationships.Length - 1;

                                    var dpd_temp_rel = new string[dpd_count];
                                    var dpd_temp_val = new string[dpd_count];

                                    int dpd_i2 = 0;
                                    var dpd_i3 = 0;
                                    foreach (string trg in KingdomRelation[i].fac_relationships)
                                    {
                                        if (trg != "Faction." + fac.id)
                                        {
                                            dpd_temp_rel[dpd_i2] = KingdomRelation[i].fac_relationships[dpd_i3];
                                            dpd_temp_val[dpd_i2] = KingdomRelation[i].fac_relationValues[dpd_i3];
                                            dpd_i2++;
                                        }
                                        dpd_i3++;
                                    }

                                    KingdomRelation[i].fac_relationships = dpd_temp_rel;
                                    KingdomRelation[i].fac_relationValues = dpd_temp_val;
                                }
                            }
                        }

                        var count = fac.relationships.Length - 1;
                        var temp_rel = new string[count];
                        var temp_val = new string[count];
                        var temp_war = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in fac.relationships)
                        {
                            if (i3 != i)
                            {
                                temp_rel[i2] = fac.relationships[i3];
                                temp_val[i2] = fac.relationValues[i3];
                                //temp_war[i2] = fac.relationsAtWar[i3];
                                i2++;
                            }
                            i3++;
                        }

                        fac.relationships = temp_rel;
                        fac.relationValues = temp_val;
                        //fac.relationsAtWar = temp_war;

                        KingdomRelation = new Kingdom[fac.relationships.Length];
                        //IsAtWar_Bool = new bool[fac.relationsAtWar.Length];

                        //if (fac.relationsAtWar != null && fac.relationsAtWar.Length != 0)
                        //{
                        //    i2 = 0;
                        //    foreach (var rel in fac.relationsAtWar)
                        //    {
                        //        if (rel == "true")
                        //        {
                        //            IsAtWar_Bool[i2] = true;
                        //        }
                        //        else
                        //        {
                        //            IsAtWar_Bool[i2] = false;
                        //        }
                        //        i2++;
                        //    }
                        //}

                        return;

                    }



                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }

    private void CheckKingdomLinkedData()
    {
        // Load Liked Data

        if (_IS_changed_Kingdom)
        {
            int i2 = 0;

            foreach (var rel in KingdomRelation)
            {
                if (rel != null && rel.id == _changed_Kingdom)
                {
                        bool existingFaction = false;
                        foreach (string relationAsset in rel.fac_relationships)
                        {
                            if (relationAsset.Contains("Faction." + fac.id))
                            {
                                existingFaction = true;
                            }
                        }
                        // Debug.Log(existingKingdom);

                        if (!existingFaction)
                        {

                            var objectsDic = new Dictionary<string, string>();
                            var boolsList = new List<string>();

                            var i3 = 0;
                            foreach (string relationAsset in rel.fac_relationships)
                            {
                                // Debug.Log(relationAsset);
                                if (!objectsDic.ContainsKey(rel.fac_relationships[i3]))
                                {
                                    objectsDic.Add(rel.fac_relationships[i3], rel.fac_relationValues[i3]);
                                    i3++;
                                }
                            }

                            rel.fac_relationships = new string[objectsDic.Count + 1];
                            rel.fac_relationValues = new string[objectsDic.Count + 1];

                            i3 = 0;
                            foreach (var obj in objectsDic)
                            {
                                rel.fac_relationValues[i3] = obj.Value;
                                rel.fac_relationships[i3] = obj.Key;
                                i3++;
                            }

                            rel.fac_relationships[rel.fac_relationships.Length - 1] = "Faction." + fac.id;

                            i3 = 0;
                            foreach (var facRel in fac.fac_relationships)
                            {
                                if (facRel == "Kingdom." + rel.id)
                                {
                                    rel.fac_relationValues[rel.fac_relationValues.Length - 1] = fac.relationValues[i3];
                                }
                                i3++;
                            }


                        }
                        else
                        {
                            int i3 = 0;
                            foreach (string relationAsset in rel.fac_relationships)
                            {
                                if (relationAsset == "Faction." + fac.id)
                                {
                                    var i4 = 0;
                                    foreach (var facRel in fac.relationships)
                                    {
                                        if (facRel == "Kingdom." + rel.id)
                                        {
                                            rel.fac_relationValues[i3] = fac.relationValues[i4];
                                        }
                                        i4++;
                                    }
                                }
                                i3++;
                            }

                        }


                    }
                    i2++;
                }

                i2 = 0;
                if (_changed_OLD_Kingdom != null)
                {
                    // string oldKingdomLabel = "Kingdom." + _changed_OLD_Kingdom.id;
                    // GetKingdomAsset(ref oldKingdomLabel, ref _changed_OLD_Kingdom);
                    // Debug.Log(_changed_OLD_Kingdom.kingdomName);


                    bool oldKingdom = false;
                    foreach (var rel in KingdomRelation)
                    {
                        if (rel.id == _changed_OLD_Kingdom.id)
                        {
                            oldKingdom = true;
                        }
                        i2++;
                    }

                    if (!oldKingdom)
                    {
                        i2 = 0;
                        foreach (var rel in _changed_OLD_Kingdom.fac_relationships)
                        {
                            if (rel == "Faction." + fac.id)
                            {
                                var objects = new Dictionary<string, string>();
                            _changed_OLD_Kingdom.fac_relationships[i2] = "remove";

                                i2 = 0;
                                foreach (string relationAsset in _changed_OLD_Kingdom.fac_relationships)
                                {
                                    if (relationAsset != "remove")
                                    {
                                        objects.Add(relationAsset, _changed_OLD_Kingdom.fac_relationValues[i2]);
                                    }
                                    i2++;
                                }

                            _changed_OLD_Kingdom.fac_relationships = new string[objects.Count];
                            _changed_OLD_Kingdom.fac_relationValues = new string[objects.Count];

                                i2 = 0;
                                foreach (var obj in objects)
                                {
                                _changed_OLD_Kingdom.fac_relationValues[i2] = obj.Value;
                                _changed_OLD_Kingdom.fac_relationships[i2] = obj.Key;
                                    i2++;
                                }
                            }
                            i2++;
                        }
                    }

                    // Debug.Log(_changed_OLD_Kingdom.id);
                }


                _changed_Kingdom = "";
            _IS_changed_Kingdom = false;
        }
    }

    private void GetKingdomAsset(ref string kingdomLink, ref Kingdom kingdom)
    {
        // Face Key Template template
        // 
        if (kingdomLink != null && kingdomLink != "")
        {
            if (kingdomLink.Contains("Kingdom."))
            {
                // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");

                string dataName = kingdomLink.Replace("Kingdom.", "");

                string assetPath;
                string assetPathShort;


                assetPath = dataPath + fac.moduleID + "/Kingdoms/" + dataName + ".asset";
                assetPathShort = "/Kingdoms/" + dataName + ".asset";


                if (System.IO.File.Exists(assetPath))
                {
                    kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Kingdom));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + fac.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + assetPathShort;

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Kingdom));
                                break;
                            }
                            else
                            {
                                kingdom = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (kingdom == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == fac.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                            kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Kingdom));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingdom == null)
                        {

                            Debug.Log("Kingdom " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");

                        }
                    }

                }
            }
        }
    }
    private void CreateAttributeToggle(ref bool attrBool, ref string attr, string toggleLabel)
    {
        attrBool = GUILayout.Toggle(attrBool, toggleLabel);

        if (attrBool)
        {
            attr = "true";
        }
        else
        {
            attr = "false";
        }
    }
    void DrawFactionRelationshipsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#00d2f3", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#00aaff", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Relationships: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showFactionsRelationshipsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Factions Relationships";
        }

        showFactionsRelationshipsEditor = EditorGUILayout.Foldout(showFactionsRelationshipsEditor, showEditorLabel, hiderStyle);

        if (showFactionsRelationshipsEditor)
        {

            EditorGUILayout.LabelField("Factions Relationships", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            // if (kingd.relationships == null)
            // {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((new GUIContent("Add Relation", "Add relation between Factions")), buttonStyle, GUILayout.Width(128)))
            {
                if (fac.fac_relationships == null)
                    fac.fac_relationships = new string[0];

                if (fac.fac_relationValues == null)
                    fac.fac_relationValues = new string[0];

                var temp = new string[fac.fac_relationships.Length + 1];
                fac.fac_relationships.CopyTo(temp, 0);
                fac.fac_relationships = temp;

                fac.fac_relationships[fac.fac_relationships.Length - 1] = "";

                temp = new string[fac.fac_relationValues.Length + 1];
                fac.fac_relationValues.CopyTo(temp, 0);
                fac.fac_relationValues = temp;

                fac.fac_relationValues[fac.fac_relationValues.Length - 1] = "0";

                FactionRelation = new Faction[fac.fac_relationships.Length];

                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            // DrawUILine(colUILine, 3, 12);
            // }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (fac.fac_relationships != null && fac.fac_relationships.Length != 0)
            {
                foreach (var relation in fac.fac_relationships)
                {
                    //Debug.Log(relation);
                    GetFactionAsset(ref fac.fac_relationships[i], ref FactionRelation[i]);
                    i++;
                }

                CheckFactionLinkedData();

                i = 0;
                foreach (var relation in fac.fac_relationships)
                {

                    if (FactionRelation[i] != null)
                    {
                        _changed_OLD_Faction = FactionRelation[i];
                    }
                    else
                    {
                        _changed_OLD_Faction = null;
                    }

                    // GetKingdomAsset(ref kingd.relationships[i], ref KingdomRelation[i]);

                    // ColorUtility.TryParseHtmlString("#F65314", out newCol2);
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    // GetKingdomAsset(ref kingd.relationships[i], ref KingdomRelation[i]);


                    if (FactionRelation[i] != null)
                    {
                        ColorUtility.TryParseHtmlString("#" + FactionRelation[i].color, out newCol2);
                        //titleStyle.normal.textColor = new Color(newCol2.r, newCol2.g, newCol2.b, 1);

                        string kingdom_soloName = FactionRelation[i].factionName;
                        RemoveTSString(ref kingdom_soloName);
                        EditorGUILayout.LabelField(kingdom_soloName, titleStyle, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        ColorUtility.TryParseHtmlString("#6a737b", out newCol2);
                        titleStyle.normal.textColor = newCol2;

                        EditorGUILayout.LabelField("None", titleStyle, GUILayout.ExpandWidth(true));
                    }

                    EditorGUILayout.Space(2);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Faction: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    // EditorGUILayout.LabelField("Kingdom:", EditorStyles.label, GUILayout.ExpandWidth(false));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    FactionRelation[i] = (Faction)EditorGUILayout.ObjectField(FactionRelation[i], typeof(Faction), true, GUILayout.MaxWidth(192));

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (FactionRelation[i] != null)
                        {
                            if (FactionRelation[i].id != fac.id)
                                _changed_Faction = FactionRelation[i].id;
                            else
                            {
                                FactionRelation[i] = null;
                                _changed_Faction = "";
                            }

                            _IS_changed_Faction = true;
                        }

                    }

                    if (FactionRelation[i] != null)
                    {
                        fac.fac_relationships[i] = "Faction." + FactionRelation[i].id;
                    }
                    else
                    {
                        fac.fac_relationships[i] = "";
                    }

                    EditorGUI.BeginChangeCheck();

                    CreateIntAttribute(ref fac.fac_relationValues[i], "Relation Value:");

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (FactionRelation[i] != null)
                        {
                            _changed_Faction = FactionRelation[i].id;
                            _IS_changed_Faction = true;
                        }

                    }

                    // EditorGUILayout.Space();

                    if (GUILayout.Button((new GUIContent("X", "Remove Relation")), buttonStyle, GUILayout.Width(32)))
                    {

                        if (FactionRelation[i] != null)
                        {
                            foreach (var rel in FactionRelation[i].fac_relationships)
                            {
                                if (rel != null && rel == "Faction." + fac.id)
                                {
                                    var dpd_count = 0;

                                    if (FactionRelation[i].fac_relationships.Length != 0)
                                        dpd_count = FactionRelation[i].fac_relationships.Length - 1;

                                    var dpd_temp_rel = new string[dpd_count];
                                    var dpd_temp_val = new string[dpd_count];

                                    int dpd_i2 = 0;
                                    var dpd_i3 = 0;
                                    foreach (string trg in FactionRelation[i].fac_relationships)
                                    {
                                        if (trg != "Faction." + fac.id)
                                        {
                                            dpd_temp_rel[dpd_i2] = FactionRelation[i].fac_relationships[dpd_i3];
                                            dpd_temp_val[dpd_i2] = FactionRelation[i].fac_relationValues[dpd_i3];
                                            dpd_i2++;
                                        }
                                        dpd_i3++;
                                    }

                                    FactionRelation[i].fac_relationships = dpd_temp_rel;
                                    FactionRelation[i].fac_relationValues = dpd_temp_val;
                                }
                            }
                        }

                        var count = 0;

                        if (fac.fac_relationships.Length != 0)
                            count = fac.fac_relationships.Length - 1;

                        var temp_rel = new string[count];
                        var temp_val = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in fac.fac_relationships)
                        {
                            if (i3 != i)
                            {
                                temp_rel[i2] = fac.fac_relationships[i3];
                                temp_val[i2] = fac.fac_relationValues[i3];
                                i2++;
                            }
                            i3++;
                        }

                        fac.fac_relationships = temp_rel;
                        fac.fac_relationValues = temp_val;

                        FactionRelation = new Faction[fac.fac_relationships.Length];

                        return;

                    }



                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }

    private void CheckFactionLinkedData()
    {
        // Load Liked Data

        if (_IS_changed_Faction)
        {
            int i2 = 0;

            foreach (var rel in FactionRelation)
            {
                if (rel != null && rel.id == _changed_Faction)
                {
                    bool existingFaction = false;
                    foreach (string relationAsset in rel.fac_relationships)
                    {
                        if (relationAsset.Contains("Faction." + fac.id))
                        {
                            existingFaction = true;
                        }
                    }
                    // Debug.Log(existingKingdom);


                    if (!existingFaction)
                    {

                        var objectsDic = new Dictionary<string, string>();
                        var boolsList = new List<string>();

                        var i3 = 0;
                        foreach (string relationAsset in rel.fac_relationships)
                        {
                            // Debug.Log(relationAsset);
                            if (!objectsDic.ContainsKey(rel.fac_relationships[i3]))
                            {
                                objectsDic.Add(rel.fac_relationships[i3], rel.fac_relationValues[i3]);
                                i3++;
                            }
                        }

                        rel.fac_relationships = new string[objectsDic.Count + 1];
                        rel.fac_relationValues = new string[objectsDic.Count + 1];

                        i3 = 0;
                        foreach (var obj in objectsDic)
                        {
                            rel.fac_relationValues[i3] = obj.Value;
                            rel.fac_relationships[i3] = obj.Key;
                            i3++;
                        }

                        rel.fac_relationships[rel.fac_relationships.Length - 1] = "Faction." + fac.id;

                        i3 = 0;
                        foreach (var facRel in fac.fac_relationships)
                        {
                            if (facRel == "Faction." + rel.id)
                            {
                                rel.fac_relationValues[rel.fac_relationValues.Length - 1] = fac.fac_relationValues[i3];
                            }
                            i3++;
                        }


                    }
                    else
                    {
                        int i3 = 0;
                        foreach (string relationAsset in rel.fac_relationships)
                        {
                            if (relationAsset == "Faction." + fac.id)
                            {
                                var i4 = 0;
                                foreach (var facRel in fac.fac_relationships)
                                {
                                    if (facRel == "Faction." + rel.id)
                                    {
                                        rel.fac_relationValues[i3] = fac.fac_relationValues[i4];
                                    }
                                    i4++;
                                }
                            }
                            i3++;
                        }

                    }


                }
                i2++;
            }

            i2 = 0;
            if (_changed_OLD_Faction != null)
            {
                // string oldKingdomLabel = "Kingdom." + _changed_OLD_Kingdom.id;
                // GetKingdomAsset(ref oldKingdomLabel, ref _changed_OLD_Kingdom);
                // Debug.Log(_changed_OLD_Kingdom.kingdomName);


                bool oldKingdom = false;
                foreach (var rel in FactionRelation)
                {
                    if (rel.id == _changed_OLD_Faction.id)
                    {
                        oldKingdom = true;
                    }
                    i2++;
                }

                if (!oldKingdom)
                {
                    i2 = 0;
                    foreach (var rel in _changed_OLD_Faction.fac_relationships)
                    {
                        if (rel == "Faction." + fac.id)
                        {
                            var objects = new Dictionary<string, string>();
                            _changed_OLD_Faction.fac_relationships[i2] = "remove";

                            i2 = 0;
                            foreach (string relationAsset in _changed_OLD_Faction.fac_relationships)
                            {
                                if (relationAsset != "remove")
                                {
                                    objects.Add(relationAsset, _changed_OLD_Faction.fac_relationValues[i2]);
                                }
                                i2++;
                            }

                            _changed_OLD_Faction.fac_relationships = new string[objects.Count];
                            _changed_OLD_Faction.fac_relationValues = new string[objects.Count];

                            i2 = 0;
                            foreach (var obj in objects)
                            {
                                _changed_OLD_Faction.fac_relationValues[i2] = obj.Value;
                                _changed_OLD_Faction.fac_relationships[i2] = obj.Key;
                                i2++;
                            }
                        }
                        i2++;
                    }
                }

                // Debug.Log(_changed_OLD_Kingdom.id);
            }


            _changed_Faction = "";
            _IS_changed_Faction = false;
        }
    }
    private void GetFactionAsset(ref string factionLink, ref Faction faction)
    {
        // 
        if (factionLink != null && factionLink != "")
        {
            if (factionLink.Contains("Faction."))
            {
                // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");

                string dataName = factionLink.Replace("Faction.", "");

                string assetPath;
                string assetPathShort;


                assetPath = dataPath + fac.moduleID + "/Factions/" + dataName + ".asset";
                assetPathShort = "/Factions/" + dataName + ".asset";


                if (System.IO.File.Exists(assetPath))
                {
                    faction = (Faction)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Faction));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + fac.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + assetPathShort;

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                faction = (Faction)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Faction));
                                break;
                            }
                            else
                            {
                                faction = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (faction == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == fac.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                            faction = (Faction)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Faction));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (faction == null)
                        {

                            Debug.Log("Faction " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");

                        }
                    }

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
    private void DrawMinorTemplatesEditor()
    {
        DrawUILine(colUILine, 3, 12);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        ColorUtility.TryParseHtmlString("#00a78e", out newCol);

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;
        titleStyle.normal.textColor = newCol;

        var showEditorLabel = "Hide";
        if (!showMinorFacTemplates)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Minor Faction Character Templates";
        }

        showMinorFacTemplates = EditorGUILayout.Foldout(showMinorFacTemplates, showEditorLabel, hiderStyle);

        if (showMinorFacTemplates)
        {
            EditorGUILayout.LabelField("Minor Faction Character Templates", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);
            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Template", "Add new minor faction template to this Faction")), buttonStyle, GUILayout.Width(128)))
            {

                var temp = new string[1];
                if (fac.minor_faction_character_templates != null && fac.minor_faction_character_templates.Length > 0)
                    temp = new string[fac.minor_faction_character_templates.Length + 1];

                fac.minor_faction_character_templates.CopyTo(temp, 0);
                fac.minor_faction_character_templates = temp;

                fac.minor_faction_character_templates[fac.minor_faction_character_templates.Length - 1] = "";

                minor_fac_char_templates = new NPCCharacter[fac.minor_faction_character_templates.Length];

                return;
            }


            if (fac.minor_faction_character_templates != null && fac.minor_faction_character_templates.Length > 0)
            {

                int i = 0;
                foreach (var targetAsset in fac.minor_faction_character_templates)
                {
                    //Debug.Log(UpgradeTargets.Length);
                    GetNPCAsset(ref fac.minor_faction_character_templates[i], ref minor_fac_char_templates[i], false);
                    if (minor_fac_char_templates[i] == null)
                    {
                        GetNPCAsset(ref fac.minor_faction_character_templates[i], ref minor_fac_char_templates[i], true);
                    }

                    i++;
                }

                DrawUILine(colUILine, 3, 12);

                i = 0;
                foreach (var targetAsset in fac.minor_faction_character_templates)
                {

                    // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


                    ColorUtility.TryParseHtmlString("#F25022", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 11;
                    EditorGUILayout.LabelField("Character Template - " + i, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                    ColorUtility.TryParseHtmlString("#FF9900", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 12;

                    string nameLabel = "None";
                    if (minor_fac_char_templates[i] != null)
                    {
                        nameLabel = minor_fac_char_templates[i].npcName;
                    }

                    RemoveTSString(ref nameLabel);

                    EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);

                    EditorGUILayout.BeginHorizontal();
                    minor_fac_char_templates[i] = (NPCCharacter)EditorGUILayout.ObjectField(minor_fac_char_templates[i], typeof(NPCCharacter), true, GUILayout.MaxWidth(320));

                    if (minor_fac_char_templates[i] != null)
                    {
                        fac.minor_faction_character_templates[i] = "NPCCharacter." + minor_fac_char_templates[i].id;
                    }
                    else
                    {
                        fac.minor_faction_character_templates[i] = "";
                    }

                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("X", "Remove Template")), buttonStyle, GUILayout.Width(32)))
                    {
                        var count = fac.minor_faction_character_templates.Length - 1;
                        var pt_troop = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in fac.minor_faction_character_templates)
                        {
                            if (i3 != i)
                            {
                                pt_troop[i2] = fac.minor_faction_character_templates[i3];

                                i2++;
                            }
                            i3++;
                        }

                        fac.minor_faction_character_templates = pt_troop;

                        minor_fac_char_templates = new NPCCharacter[fac.minor_faction_character_templates.Length];

                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    DrawUILine(colUILine, 1, 4);
                    i++;
                }
            }
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
    private void GetNPCAsset(ref string npcLink, ref NPCCharacter npcCharacter, bool npcTemplates)
    {
        // Face Key Template template
        // 
        if (npcLink != null && npcLink != "")
        {
            string dataName = npcLink;

            if (npcLink.Contains("NPCCharacter."))
            {
                // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");
                dataName = npcLink.Replace("NPCCharacter.", "");
            }

            string assetPath;
            string assetPathShort;

            if (npcTemplates)
            {
                assetPath = dataPath + fac.moduleID + "/_Templates/NPCtemplates/" + dataName + ".asset";
                assetPathShort = "/_Templates/NPCtemplates/" + dataName + ".asset";
            }
            else
            {
                assetPath = dataPath + fac.moduleID + "/NPC/" + dataName + ".asset";
                assetPathShort = "/NPC/" + dataName + ".asset";
            }

            if (System.IO.File.Exists(assetPath))
            {
                npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(NPCCharacter));
            }
            else
            {
                // SEARCH IN DEPENDENCIES
                string modSett = modsSettingsPath + fac.moduleID + ".asset";
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
                            if (depend == fac.moduleID)
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
                            Debug.Log("NPCCharacter " + dataName + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }

            }
        }
    }

    private void CreateToggleFaction(ref string toggleVal, ref bool boolObject, string label)
    {
        if (toggleVal == "true")
        {
            boolObject = true;
        }
        else
        {
            boolObject = false;
        }

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
        EditorGUIUtility.labelWidth = textDimensions.x;

        EditorGUILayout.PrefixLabel(label);
        boolObject = EditorGUILayout.Toggle(boolObject, EditorStyles.toggle);

        if (boolObject)
        {
            toggleVal = "true";
        }
        else
        {
            toggleVal = "";
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
                                if (depend == fac.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");
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
                                if (depend == fac.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + fac.moduleID + " ' " + "resources, and they dependencies.");
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
