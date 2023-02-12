using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

[CustomEditor(typeof(Settlement))]
public class SettlementAssetEditor : Editor
{

    string dataPath = "Assets/BDT/Resources/Data/";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    string folder = "SettlementsTranslationData";
    //
    public string[] settlemetsType_options;
    public int settlementsType_index = 0;

    public string[] villageType_options;
    public int villageType_index = 0;

    public string[] areaType_options;
    public int areaType_index = 0;

    public string[] location_options;
    public int location_index = 0;
    //

    bool showLocEditor;
    bool showAreasEditor;
    //

    Settlement _bound;
    Settlement _trade_bound;
    //
    string soloName;
    string soloText;
    string[] soloAreaName = new string[3];
    string[] areaTranslationString = new string[3];
    string nameTranslationString;
    string textTranslationString;
    TranslationString[] translationStringArea = new TranslationString[3];
    TranslationString translationStringName;
    TranslationString translationStringDescription;
    Vector2 textScrollPos;
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    Settlement settl;
    public Faction settlementOwner;
    public Culture cultureIs;
    Color primaryBannerColor;

    Color secondaryBannerColor;

    Color labelColor;

    Color color_a;

    Color color_b;
    Color alt_color_a;

    Color alt_color_b;

    bool isDependency = false;
    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";

    /// <summary>
    /// 
    /// Areas / Locations - Definitions
    /// 
    /// Town:
    /// 0 = Backstreet
    /// 1 = Clearing
    /// 2 = Waterfront
    /// 
    /// Castle:
    /// -- NotDefined
    /// 
    /// Village:
    /// 0 = Pasture
    /// 1 = Thicket
    /// 2 = Bog
    /// 
    /// </summary>



    public void OnEnable()
    {
        settl = (Settlement)target;
        EditorUtility.SetDirty(settl);

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

        settlemetsType_options = new string[settingsAsset.SettlementsTypeDefinitions.Length];

        int i = 0;
        foreach (var category in settingsAsset.SettlementsTypeDefinitions)
        {
            settlemetsType_options[i] = category;

            if (settl.isTown)
            {
                if ((settlemetsType_options[i]) == "Town")
                {
                    settlementsType_index = i;
                }
            }
            else if (settl.isCastle)
            {
                if ((settlemetsType_options[i]) == "Castle")
                {
                    settlementsType_index = i;
                }
            }
            else if (settl.isVillage)
            {
                if ((settlemetsType_options[i]) == "Village")
                {
                    settlementsType_index = i;
                }
            }
            else if (settl.isHideout)
            {
                if ((settlemetsType_options[i]) == "Hideout")
                {
                    settlementsType_index = i;
                }
            }

            i++;
        }


        villageType_options = new string[settingsAsset.VillagesTypeDefinitions.Length];

        i = 0;
        foreach (var category in settingsAsset.VillagesTypeDefinitions)
        {
            villageType_options[i] = category;
            if ("VillageType." + villageType_options[i] == settl.CMP_villageType)
            {
                villageType_index = i;
            }
            i++;
        }

        if (settl.isTown)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.town_complex";
        }
        else if (settl.isCastle)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.castle_complex";
        }
        else if (settl.isVillage)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.village_complex";
        }

        //LoadNewLocComplex();
    }

    public override void OnInspectorGUI()
    {

        if (settingsAsset.currentModule != settl.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == settl.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + settl.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + settl.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + settl.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + settl.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settl.moduleID + ".asset", typeof(ModuleReceiver));
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
                        assetMng.objID = 5;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = settl;

                        assetMng.assetName_org = settl.id;
                        assetMng.assetName_new = settl.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 5;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = settl;

                        assetMng.assetName_org = settl.id;
                        assetMng.assetName_new = settl.id;
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


        var labelStyle = new GUIStyle(EditorStyles.label);

        EditorGUI.BeginDisabledGroup(true);
        settl.id = EditorGUILayout.TextField("Settlement ID:", settl.id);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {
            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 5;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = settl;

                assetMng.assetName_org = settl.id;
                assetMng.assetName_new = settl.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 5;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = settl;

                assetMng.assetName_org = settl.id;
                assetMng.assetName_new = settl.id;
            }
        }
        //DrawUILine(colUILine, 3, 12);

        // settl.settlementName = EditorGUILayout.TextField("Settlement Name:", settl.settlementName);

        // settlement name & translationString tag
        DrawUILine(colUILine, 3, 12);

        SetLabelFieldTS(ref settl.settlementName, ref soloName, ref nameTranslationString, folder, translationStringName, "Settlement Name:", settl.moduleID, settl, 5, settl.id);

        DrawUILine(colUILine, 3, 12);
        // -------------- settlement Name END

        //# TYPE
        EditorGUILayout.LabelField("Settlement Type:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        // EditorGUILayout.Space(2);

        var chanagedIndex = false;
        EditorGUI.BeginChangeCheck();
        settlementsType_index = EditorGUILayout.Popup(settlementsType_index, settlemetsType_options, GUILayout.Width(128));
        if (EditorGUI.EndChangeCheck())
        {
            chanagedIndex = true;
        }

        if (settlemetsType_options[settlementsType_index] == "Town")
        {
            settl.isTown = true;
            settl.isCastle = false;
            settl.isVillage = false;
            settl.isHideout = false;

            settl.type = "";
        }
        else if (settlemetsType_options[settlementsType_index] == "Castle")
        {
            settl.isTown = false;
            settl.isCastle = true;
            settl.isVillage = false;
            settl.isHideout = false;
            settl.type = "";
        }
        else if (settlemetsType_options[settlementsType_index] == "Village")
        {
            settl.isTown = false;
            settl.isCastle = false;
            settl.isVillage = true;
            settl.isHideout = false;
            settl.type = "";
        }
        else if (settlemetsType_options[settlementsType_index] == "Hideout")
        {
            settl.isTown = false;
            settl.isCastle = false;
            settl.isVillage = false;
            settl.isHideout = true;
            settl.type = "Hideout";
        }
        DrawUILine(colUILine, 3, 12);

        // SETTLEMENT PROSPERITY
        var originLabelWidth = EditorGUIUtility.labelWidth;
        Vector2 textDimensions;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Prosperity:"));
        EditorGUIUtility.labelWidth = textDimensions.x + 64;
        // Clan Tier
        // fac.tier = EditorGUILayout.TextField("Faction Tier:", fac.tier);

        // if hideout / hide unused node parameters 
        if (settlemetsType_options[settlementsType_index] != "Hideout")
        {
            int prosperityValue;

            int.TryParse(settl.prosperity, out prosperityValue);

            prosperityValue = EditorGUILayout.IntField("Prosperity:", prosperityValue, GUILayout.Width(200));

            settl.prosperity = prosperityValue.ToString();

            EditorGUIUtility.labelWidth = originLabelWidth;

            DrawUILine(colUILine, 3, 12);
        }

        // settl.culture = EditorGUILayout.TextField("Culture:", settl.culture);
        // CULTURE
        if (settl.culture != null && settl.culture != "")
        {
            if (settl.culture.Contains("Culture."))
            {
                string dataName = settl.culture.Replace("Culture.", "");
                string asset = dataPath + settl.moduleID + "/Cultures/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(asset, typeof(Culture));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + settl.moduleID + ".asset";
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
                                if (depend == settl.moduleID)
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
                            Debug.Log("Culture " + dataName + " - Not EXIST in" + " ' " + settl.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }
            }
        }


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Settlement Culture:", EditorStyles.label);
        object cultureField = EditorGUILayout.ObjectField(cultureIs, typeof(Culture), true);
        cultureIs = (Culture)cultureField;

        if (cultureIs != null)
        {
            settl.culture = "Culture." + cultureIs.id;
        }
        else
        {
            settl.culture = "";
        }
        GUILayout.EndHorizontal();

        //CULTURE END

        // owner
        if (settl.owner != null && settl.owner != "")
        {
            if (settl.owner.Contains("Faction."))
            {
                string dataName = settl.owner.Replace("Faction.", "");
                string asset = dataPath + settl.moduleID + "/Factions/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    settlementOwner = (Faction)AssetDatabase.LoadAssetAtPath(asset, typeof(Faction));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + settl.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Factions/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                settlementOwner = (Faction)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Faction));
                                break;
                            }
                            else
                            {
                                settlementOwner = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (settlementOwner == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == settl.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.factionsData.factions)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Factions/" + dataName + ".asset";
                                            settlementOwner = (Faction)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Faction));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (settlementOwner == null)
                        {
                            Debug.Log("Faction " + dataName + " - Not EXIST in" + " ' " + settl.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        // if hideout / hide unused node parameters 
        if (settlemetsType_options[settlementsType_index] != "Hideout")
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Settlement Owner's Faction:", EditorStyles.label);
            object ownerField = EditorGUILayout.ObjectField(settlementOwner, typeof(Faction), true);
            settlementOwner = (Faction)ownerField;

            if (settlementOwner != null)
            {
                settl.owner = "Faction." + settlementOwner.id;
            }
            else
            {
                settl.owner = "";
            }
            GUILayout.EndHorizontal();
        }

        // settl.prosperity = EditorGUILayout.TextField("Prosperity:", settl.prosperity);


        /// SETTLEMENT TYPE
        // textDimensions = GUI.skin.label.CalcSize(new GUIContent("Settlement Type:         "));
        // EditorGUIUtility.labelWidth = textDimensions.x;
        // settl.type = EditorGUILayout.TextField("Settlement Type:", settl.type, GUILayout.MaxWidth(256));

        // EditorGUIUtility.labelWidth = originLabelWidth;

        DrawUILine(colUILine, 3, 12);


        /// SETTLEMENT POSITION
        /// 
        EditorGUILayout.LabelField("Settlement Position:", EditorStyles.boldLabel);

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("World Position X            "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        float posValueX;

        float.TryParse(settl.posX, out posValueX);

        posValueX = EditorGUILayout.FloatField("World Position X", posValueX, GUILayout.MaxWidth(256));

        settl.posX = posValueX.ToString();

        float posValueY;

        float.TryParse(settl.posY, out posValueY);

        posValueY = EditorGUILayout.FloatField("World Position Y", posValueY, GUILayout.MaxWidth(256));

        settl.posY = posValueY.ToString();

        // settl.posX = EditorGUILayout.TextField("World Position X:", settl.posX);
        // settl.posY = EditorGUILayout.TextField("World Position Y:", settl.posY);
        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Settlement Gate Position:", EditorStyles.boldLabel);

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("World Position X            "));
        EditorGUIUtility.labelWidth = textDimensions.x;
        float posGateValueX;

        float.TryParse(settl.gate_posX, out posGateValueX);

        posGateValueX = EditorGUILayout.FloatField("Gate World Position X", posGateValueX, GUILayout.MaxWidth(256));

        settl.gate_posX = posGateValueX.ToString();

        float posGateValueY;

        float.TryParse(settl.gate_posY, out posGateValueY);

        posGateValueY = EditorGUILayout.FloatField("Gate World Position Y", posGateValueY, GUILayout.MaxWidth(256));

        settl.gate_posY = posGateValueY.ToString();

        EditorGUIUtility.labelWidth = originLabelWidth;
        // settl.gate_posX = EditorGUILayout.TextField("Gate Position X:", settl.gate_posX);
        // settl.gate_posY = EditorGUILayout.TextField("Gate Position Y:", settl.gate_posY);

        DrawUILine(colUILine, 3, 12);

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Component ID: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        settl.CMP_id = EditorGUILayout.TextField("Component ID: ", settl.CMP_id);

        DrawUILine(colUILine, 3, 12);
        CreateIntAttribute(ref settl.CMP_Level, "Level:");

        // if village / Show :Village: node parameters 
        if (settlemetsType_options[settlementsType_index] == "Village")
        {
            DrawUILine(colUILine, 3, 12);

            EditorGUILayout.LabelField("Village Type:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            // EditorGUILayout.Space(2);
            villageType_index = EditorGUILayout.Popup(villageType_index, villageType_options, GUILayout.Width(128));
            settl.CMP_villageType = "VillageType." + villageType_options[villageType_index];

            DrawUILine(colUILine, 3, 12);
            CreateIntAttribute(ref settl.CMP_hearth, "Hearth:");

            DrawUILine(colUILine, 3, 12);

            GetSettlemetnAsset(ref settl.CMP_bound, ref _bound);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bound:", EditorStyles.label);
            object boundField = EditorGUILayout.ObjectField(_bound, typeof(Settlement), true);
            _bound = (Settlement)boundField;

            if (_bound != null)
            {
                settl.CMP_bound = "Settlement." + _bound.id;
            }
            else
            {
                settl.CMP_bound = "";
            }
            GUILayout.EndHorizontal();

            DrawUILine(colUILine, 1, 6);

            GetSettlemetnAsset(ref settl.CMP_trade_bound, ref _trade_bound);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Trade Bound:", EditorStyles.label);
            object TradeBoundField = EditorGUILayout.ObjectField(_trade_bound, typeof(Settlement), true);
            _trade_bound = (Settlement)TradeBoundField;

            if (_trade_bound != null)
            {
                settl.CMP_trade_bound = "Settlement." + _trade_bound.id;
            }
            else
            {
                settl.CMP_trade_bound = "";
            }
            GUILayout.EndHorizontal();
        }

        DrawUILine(colUILine, 3, 12);


        // DrawUILineVertical(colUILine, 1, 1, 16);
        CreateFloatAttribute(ref settl.CMP_background_crop_position, "Background Crop Position:", 220);
        settl.CMP_background_mesh = EditorGUILayout.TextField("Background Mesh: ", settl.CMP_background_mesh);
        settl.CMP_wait_mesh = EditorGUILayout.TextField("Wait Mesh: ", settl.CMP_wait_mesh);
        settl.CMP_castle_background_mesh = EditorGUILayout.TextField("Castle Background Mesh: ", settl.CMP_castle_background_mesh);

        DrawUILine(colUILine, 3, 12);
        CreateFloatAttribute(ref settl.CMP_gate_rotation, "Gate Rotation:", 220);

        settl.CMP_map_icon = EditorGUILayout.TextField("Map Icon: ", settl.CMP_map_icon);
        settl.CMP_scene_name = EditorGUILayout.TextField("Scene Name: ", settl.CMP_scene_name);

        DrawUILine(colUILine, 3, 12);

        if (settl.isTown)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.town_complex";
        }
        else if (settl.isCastle)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.castle_complex";
        }
        else if (settl.isVillage)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.village_complex";
        }
        else if (settl.isHideout)
        {
            settl.LOC_complex_template = "LocationComplexTemplate.hideout_complex";
        }

        Color newCol;
        ColorUtility.TryParseHtmlString("#ffbb00", out newCol);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        titleStyle.normal.textColor = newCol;
        titleStyle.fontSize = 16;

        var complexSoloName = settl.LOC_complex_template.Replace("LocationComplexTemplate.", "");

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Scene 1:  "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide - Locations Editor";
        if (!showLocEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Locations Editor";
        }
        else
        {
            hiderStyle.fontSize = 10;
        }


        EditorGUI.BeginChangeCheck();
        showLocEditor = EditorGUILayout.Foldout(showLocEditor, showEditorLabel, hiderStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if (showLocEditor)
            {
                RefreshLocations();
            }

        }

        if (showLocEditor)
        {

            // ColorUtility.TryParseHtmlString("#f65314", out newCol);
            // titleStyle.normal.textColor = newCol;
            if (settl.isTown)
            {
                EditorGUILayout.LabelField("Complex Template - Town", titleStyle);
            }
            else if (settl.isCastle)
            {
                EditorGUILayout.LabelField("Complex Template - Castle", titleStyle);
            }
            else if (settl.isVillage)
            {
                EditorGUILayout.LabelField("Complex Template - Village", titleStyle);
            }

            DrawUILine(colUILine, 3, 12);

            ColorUtility.TryParseHtmlString("#f65314", out newCol);
            titleStyle.normal.textColor = newCol;

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.green;

            if (location_options != null && location_options.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();

                textDimensions = GUI.skin.label.CalcSize(new GUIContent("Locations: "));
                EditorGUIUtility.labelWidth = textDimensions.x;

                areaType_index = EditorGUILayout.Popup("Locations:", areaType_index, location_options, GUILayout.Width(160));

                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Location", "Add selected Location for this Settlement")), buttonStyle, GUILayout.Width(128)))
                {
                    if (settl.LOC_scn == null)
                        settl.LOC_scn = new string[0];

                    if (settl.LOC_scn_1 == null)
                        settl.LOC_scn_1 = new string[0];

                    if (settl.LOC_scn_2 == null)
                        settl.LOC_scn_2 = new string[0];

                    if (settl.LOC_scn_3 == null)
                        settl.LOC_scn_3 = new string[0];

                    if (settl.LOC_max_prosperity == null)
                        settl.LOC_max_prosperity = new string[0];

                    if (settl.LOC_id == null)
                        settl.LOC_id = new string[0];

                    var temp = new string[settl.LOC_id.Length + 1];
                    settl.LOC_id.CopyTo(temp, 0);
                    settl.LOC_id = temp;

                    settl.LOC_id[settl.LOC_id.Length - 1] = location_options[areaType_index];

                    temp = new string[settl.LOC_scn.Length + 1];
                    settl.LOC_scn.CopyTo(temp, 0);
                    settl.LOC_scn = temp;

                    settl.LOC_scn[settl.LOC_scn.Length - 1] = "";

                    temp = new string[settl.LOC_scn_1.Length + 1];
                    settl.LOC_scn_1.CopyTo(temp, 0);
                    settl.LOC_scn_1 = temp;

                    settl.LOC_scn_1[settl.LOC_scn_1.Length - 1] = "";

                    temp = new string[settl.LOC_scn_2.Length + 1];
                    settl.LOC_scn_2.CopyTo(temp, 0);
                    settl.LOC_scn_2 = temp;

                    settl.LOC_scn_2[settl.LOC_scn_2.Length - 1] = "";

                    temp = new string[settl.LOC_scn_3.Length + 1];
                    settl.LOC_scn_3.CopyTo(temp, 0);
                    settl.LOC_scn_3 = temp;

                    settl.LOC_scn_3[settl.LOC_scn_3.Length - 1] = "";

                    temp = new string[settl.LOC_max_prosperity.Length + 1];
                    settl.LOC_max_prosperity.CopyTo(temp, 0);
                    settl.LOC_max_prosperity = temp;

                    settl.LOC_max_prosperity[settl.LOC_max_prosperity.Length - 1] = "";

                    RefreshLocations();

                    return;
                }

                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 3, 12);
            }

            foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
            {
                if (complex.complexID == complexSoloName)
                {
                    if (complex.locationsComplexID != null && complex.locationsComplexID.Length > 0)
                    {
                        foreach (var complexLocation in complex.locationsComplexID)
                        {
                            if (settl.LOC_id == null)
                                settl.LOC_id = new string[0];

                            int i = 0;
                            foreach (var settlLoc in settl.LOC_id)
                            {
                                if (complexLocation == settlLoc)
                                {
                                    EditorGUILayout.LabelField("Location - " + settlLoc, titleStyle);

                                    EditorGUILayout.Space(2);
                                    EditorGUILayout.BeginHorizontal();

                                    buttonStyle.fontStyle = FontStyle.Bold;
                                    buttonStyle.hover.textColor = Color.red;

                                    if (GUILayout.Button((new GUIContent("X", "Remove Location")), buttonStyle, GUILayout.Width(32)))
                                    {

                                        var count = settl.LOC_id.Length - 1;
                                        var temp_id = new string[count];
                                        var temp_scn_0 = new string[count];
                                        var temp_scn_1 = new string[count];
                                        var temp_scn_2 = new string[count];
                                        var temp_scn_3 = new string[count];
                                        var temp_max_prosperity = new string[count];

                                        int i2 = 0;
                                        int i3 = 0;
                                        foreach (string trg in settl.LOC_id)
                                        {
                                            if (i3 != i)
                                            {
                                                temp_id[i2] = settl.LOC_id[i3];
                                                temp_scn_0[i2] = settl.LOC_scn[i3];
                                                temp_scn_1[i2] = settl.LOC_scn_1[i3];
                                                temp_scn_2[i2] = settl.LOC_scn_2[i3];
                                                temp_scn_3[i2] = settl.LOC_scn_3[i3];
                                                temp_max_prosperity[i2] = settl.LOC_max_prosperity[i3];
                                                i2++;
                                            }
                                            i3++;
                                        }

                                        settl.LOC_id = temp_id;
                                        settl.LOC_scn = temp_scn_0;
                                        settl.LOC_scn_1 = temp_scn_1;
                                        settl.LOC_scn_2 = temp_scn_2;
                                        settl.LOC_scn_3 = temp_scn_3;
                                        settl.LOC_max_prosperity = temp_max_prosperity;

                                        RefreshLocations();
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    DrawUILine(colUILine, 3, 12);

                                    if (settl.isVillage)
                                    {
                                        CreateIntAttribute(ref settl.LOC_max_prosperity[i], "Max Prosperity:");
                                    }
                                    DrawUILine(colUILine, 1, 3);

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space(-50);
                                    EditorGUILayout.BeginVertical();

                                    settl.LOC_scn[i] = EditorGUILayout.TextField("Scene 0: ", settl.LOC_scn[i], GUILayout.MaxWidth(360));
                                    settl.LOC_scn_1[i] = EditorGUILayout.TextField("Scene 1: ", settl.LOC_scn_1[i], GUILayout.MaxWidth(360));
                                    settl.LOC_scn_2[i] = EditorGUILayout.TextField("Scene 2: ", settl.LOC_scn_2[i], GUILayout.MaxWidth(360));
                                    settl.LOC_scn_3[i] = EditorGUILayout.TextField("Scene 3: ", settl.LOC_scn_3[i], GUILayout.MaxWidth(360));
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();

                                    DrawUILine(colUILine, 3, 12);

                                }
                                i++;
                            }
                        }

                    }
                    else
                    {
                        EditorGUILayout.HelpBox(settlemetsType_options[settlementsType_index] + " settlement type don't contains Location definitions", MessageType.Warning);
                        DrawUILine(colUILine, 3, 12);
                    }
                }


            }

        }
        else
        {
            DrawUILine(colUILine, 3, 12);
        }

        EditorGUIUtility.labelWidth = originLabelWidth;


        // AREAS EDITOR
        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Area Name:  "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        ColorUtility.TryParseHtmlString("#7cbb00", out newCol);
        hiderStyle.normal.textColor = newCol;

        showEditorLabel = "Hide - Areas Editor";
        if (!showAreasEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Areas Editor";
        }
        else
        {
            hiderStyle.fontSize = 10;
        }

        EditorGUI.BeginChangeCheck();
        showAreasEditor = EditorGUILayout.Foldout(showAreasEditor, showEditorLabel, hiderStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if (showAreasEditor)
            {
                RefreshAreas();
            }

        }

        complexSoloName = settl.LOC_complex_template.Replace("LocationComplexTemplate.", "");

        if (showAreasEditor)
        {
            ColorUtility.TryParseHtmlString("#7cbb00", out newCol);
            titleStyle.normal.textColor = newCol;

            // EditorGUILayout.LabelField("Areas Editor", titleStyle);

            // ColorUtility.TryParseHtmlString("#f65314", out newCol);
            // titleStyle.normal.textColor = newCol;
            if (settl.isTown)
            {
                EditorGUILayout.LabelField("Areas Editor - Town", titleStyle);
            }
            else if (settl.isCastle)
            {
                EditorGUILayout.LabelField("Areas - Castle", titleStyle);
            }
            else if (settl.isVillage)
            {
                EditorGUILayout.LabelField("Areas Editor - Village", titleStyle);
            }

            DrawUILine(colUILine, 3, 12);

            ColorUtility.TryParseHtmlString("#279b37", out newCol);
            titleStyle.normal.textColor = newCol;
            titleStyle.fontSize = 14;

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.green;

            if (areaType_options != null && areaType_options.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();

                textDimensions = GUI.skin.label.CalcSize(new GUIContent("Areas: "));
                EditorGUIUtility.labelWidth = textDimensions.x;

                areaType_index = EditorGUILayout.Popup("Areas:", areaType_index, areaType_options, GUILayout.Width(160));

                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Area", "Add selected Areas for this Settlement")), buttonStyle, GUILayout.Width(128)))
                {

                    if (settl.AREA_type == null)
                        settl.AREA_type = new string[0];

                    if (settl.AREA_name == null)
                        settl.AREA_name = new string[0];

                    if (soloAreaName == null)
                        soloAreaName = new string[0];

                    var temp = new string[settl.AREA_type.Length + 1];
                    settl.AREA_type.CopyTo(temp, 0);
                    settl.AREA_type = temp;

                    settl.AREA_type[settl.AREA_type.Length - 1] = areaType_options[areaType_index];

                    temp = new string[settl.AREA_name.Length + 1];
                    settl.AREA_name.CopyTo(temp, 0);
                    settl.AREA_name = temp;

                    settl.AREA_name[settl.AREA_name.Length - 1] = "";

                    temp = new string[soloAreaName.Length + 1];
                    soloAreaName.CopyTo(temp, 0);
                    soloAreaName = temp;

                    soloAreaName[soloAreaName.Length - 1] = "";

                    RefreshAreas();

                    return;
                }

                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 3, 12);
            }

            foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
            {
                if (complex.complexID == complexSoloName)
                {
                    if (complex.locationAreasID != null && complex.locationAreasID.Length > 0)
                    {
                        foreach (var complexAreas in complex.locationAreasID)
                        {
                            if (settl.AREA_type == null)
                                settl.AREA_type = new string[0];

                            int i = 0;
                            foreach (var areaType in settl.AREA_type)
                            {
                                if (complexAreas == areaType)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.BeginVertical();


                                    EditorGUILayout.LabelField("Area Type - " + areaType, titleStyle);

                                    EditorGUILayout.Space(2);
                                    EditorGUILayout.BeginHorizontal();

                                    buttonStyle.fontStyle = FontStyle.Bold;
                                    buttonStyle.hover.textColor = Color.red;

                                    if (GUILayout.Button((new GUIContent("X", "Remove Area")), buttonStyle, GUILayout.Width(32)))
                                    {

                                        var count = settl.AREA_type.Length - 1;
                                        var temp_type = new string[count];
                                        var temp_name = new string[count];
                                        var temp_solo_name = new string[count];

                                        int i2 = 0;
                                        int i3 = 0;
                                        foreach (string trg in settl.AREA_type)
                                        {
                                            if (i3 != i)
                                            {

                                                temp_type[i2] = settl.AREA_type[i3];
                                                temp_name[i2] = settl.AREA_name[i3];
                                                temp_solo_name[i2] = soloAreaName[i3];
                                                i2++;
                                            }
                                            i3++;
                                        }

                                        settl.AREA_type = temp_type;
                                        settl.AREA_name = temp_name;
                                        soloAreaName = temp_solo_name;

                                        RefreshAreas();
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    DrawUILine(colUILine, 3, 12);

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space(16);
                                    EditorGUILayout.BeginVertical();

                                    // DrawUILine(colUILine, 3, 12);

                                    SetLabelAreaFieldTS(ref settl.AREA_name[i], ref soloAreaName[i], ref areaTranslationString[i], folder, translationStringArea[i], "Area Name:", settl.moduleID, settl, 52, settl.id, i);

                                    DrawUILine(colUILine, 3, 12);
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.EndHorizontal();

                                    // DrawUILine(colUILine, 3, 12);

                                }
                                i++;
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(settlemetsType_options[settlementsType_index] + " settlement type don't contains Area definitions", MessageType.Warning);
                        //DrawUILine(colUILine, 3, 12);
                    }
                }
            }

        }
        // else
        // {
        DrawUILine(colUILine, 3, 12);
        // }
        EditorGUILayout.Space(4);

        // DrawUILine(colUILine, 3, 12);

        // __Return

        // ? SETTLEMENT DESCRIPTION TEXT
        // DrawUILine(colUILine, 3, 12);
        // 5 settlement name
        // 51 settlement text
        SetTextFieldTS(ref settl.text, ref soloText, ref textTranslationString, folder, translationStringDescription, "Settlement Description Text:", settl.moduleID, settl, 51, settl.id);

        if (chanagedIndex)
        {
            LoadNewLocComplex();
            chanagedIndex = false;
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

                string assetPath = dataPath + settl.moduleID + "/Settlements/" + dataName + ".asset";
                string assetPathShort = "/Settlements/" + dataName + ".asset";


                if (System.IO.File.Exists(assetPath))
                {
                    settlementAsset = (Settlement)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Settlement));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + settl.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
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

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == settl.moduleID)
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

                            Debug.Log("Settlement " + dataName + " - Not EXIST in" + " ' " + settl.moduleID + " ' " + "resources, and they dependencies.");

                        }
                    }

                }
            }
        }
    }

    private void SetLabelAreaFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel, int areaID)
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
                                if (depend == settl.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + settl.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Area Name: "));
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
            transEditor.areaID = areaID;
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
                                if (depend == settl.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + settl.moduleID + " ' " + "resources, and they dependencies.");
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

        // if (soloString == null || soloString == "")
        // {
        //     EditorGUILayout.HelpBox(typeName + " field is empty", MessageType.Warning);
        // }


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
                                if (depend == settl.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + settl.moduleID + " ' " + "resources, and they dependencies.");
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

    void CreateIntAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        int val;
        int.TryParse(attribute, out val);
        val = EditorGUILayout.IntField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

    }
    void CreateFloatAttribute(ref string attribute, string label, int weight)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        float val;
        float.TryParse(attribute, out val);
        val = EditorGUILayout.FloatField(label, val, GUILayout.MaxWidth(weight));
        attribute = val.ToString();

    }

    private void LoadNewLocComplex()
    {
        int complex_ID = 0;
        foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
        {
            // Debug.Log(settl.LOC_complex_template);

            if (complex_ID == settlementsType_index)
            {
                settl.LOC_complex_template = "LocationComplexTemplate." + complex.complexID;

                settl.LOC_id = new string[0];
                settl.LOC_scn = new string[0];
                settl.LOC_scn_1 = new string[0];
                settl.LOC_scn_2 = new string[0];
                settl.LOC_scn_3 = new string[0];

                settl.AREA_name = new string[0];
                settl.AREA_type = new string[0];
                soloAreaName = new string[0];

                //settl.LOC_id = new string[complex.locationsComplexID.Length];
                //settl.LOC_scn = new string[complex.locationsComplexID.Length];
                //settl.LOC_scn_1 = new string[complex.locationsComplexID.Length];
                //settl.LOC_scn_2 = new string[complex.locationsComplexID.Length];
                //settl.LOC_scn_3 = new string[complex.locationsComplexID.Length];

                //int i = 0;
                //foreach (var ID in complex.locationsComplexID)
                //{
                //    settl.LOC_id[i] = ID;
                //    i++;
                //}

            }
            complex_ID++;
        }

        RefreshLocations();
        RefreshAreas();
    }

    public void RefreshAreas()
    {
        var temp = new List<string>();

        if (settl.AREA_type != null && settl.AREA_type.Length > 0)
        {
            foreach (var areaType in settl.AREA_type)
            {
                if (areaType != "")
                    temp.Add(areaType);
            }
        }

        var complexSoloName = settl.LOC_complex_template.Replace("LocationComplexTemplate.", "");
        //Debug.Log(temp.Count);
        if (temp.Count > 0)
        {
            foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
            {
                if (complex.complexID == complexSoloName)
                {
                    var val = complex.locationAreasID.Length - temp.Count;
                    //if (val < 0)
                    //    val = 0;

                    areaType_options = new string[val];
                    //Debug.Log(areaType_options.Length);
                    areaType_index = 0;
                    int index = 0;
                    foreach (var complexArea in complex.locationAreasID)
                    {
                        if (!temp.Contains(complexArea))
                        {
                            //Debug.Log(index);
                            areaType_options[index] = complexArea;
                            index++;

                        }
                    }

                }

            }
        }
        else
        {
            foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
            {
                if (complex.complexID == complexSoloName)
                {
                    areaType_options = new string[complex.locationAreasID.Length];
                    areaType_index = 0;
                    int index = 0;
                    foreach (var complexArea in complex.locationAreasID)
                    {
                        areaType_options[index] = complexArea;
                        index++;
                    }

                }
            }
        }
    }

    public void RefreshLocations()
    {
        var temp = new List<string>();

        if (settl.LOC_id != null && settl.LOC_id.Length > 0)
        {
            foreach (var areaType in settl.LOC_id)
            {
                if (areaType != "")
                    temp.Add(areaType);
            }
        }

        var complexSoloName = settl.LOC_complex_template.Replace("LocationComplexTemplate.", "");
        //Debug.Log(temp.Count);
        if (temp.Count > 0)
        {
            foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
            {
                if (complex.complexID == complexSoloName)
                {
                    var val = complex.locationsComplexID.Length - temp.Count;
                    //if (val < 0)
                    //    val = 0;

                    location_options = new string[val];
                    //Debug.Log(areaType_options.Length);
                    location_index = 0;
                    int index = 0;
                    foreach (var complexID in complex.locationsComplexID)
                    {
                        if (!temp.Contains(complexID))
                        {
                            //Debug.Log(index);
                            location_options[index] = complexID;
                            index++;

                        }
                    }

                }

            }
        }
        else
        {
            foreach (var complex in settingsAsset.LocationComplexTemplateDefinitions)
            {
                if (complex.complexID == complexSoloName)
                {
                    location_options = new string[complex.locationsComplexID.Length];
                    location_index = 0;
                    int index = 0;
                    foreach (var complexID in complex.locationsComplexID)
                    {
                        location_options[index] = complexID;
                        index++;
                    }

                }
            }
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

}
