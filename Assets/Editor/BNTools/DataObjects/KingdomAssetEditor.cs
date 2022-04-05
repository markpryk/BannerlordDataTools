using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

[CustomEditor(typeof(Kingdom))]
public class KingdomAssetEditor : Editor
{

    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string folder = "KingdomsTranslationData";

    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    Vector2 textScrollPos;

    //
    public string[] policies_options;
    public int policies_index;
    //
    string _changed_Kingdom;
    Kingdom _changed_OLD_Kingdom;
    bool _IS_changed_Kingdom;
    //
    bool showRelationshipsEditor;
    bool showPoliciesEditor;

    //
    bool[] IsAtWar_Bool;

    Kingdom[] KingdomRelation;
    //

    string soloName;
    string soloText;

    string soloTitle;
    string soloRulerTitle;
    string soloShortName;

    string nameTranslationString;
    string textTranslationString;
    string soloTitleTS;
    string soloRulerTitleTS;
    string soloShortNameTS;
    TranslationString translationStringName;
    TranslationString translationStringDescription;
    TranslationString TS_soloTitle;
    TranslationString TS_soloRulerTitle;
    TranslationString TS_soloShortName;
    Kingdom kingd;
    public Hero kingdomOwner;
    public Culture cultureIs;

    Color primaryBannerColor;

    Color secondaryBannerColor;

    Color labelColor;

    Color color_a;

    Color color_b;
    Color alt_color_a;

    Color alt_color_b;


    bool isDependency = false;
    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";

    public void OnEnable()
    {
        kingd = (Kingdom)target;
        EditorUtility.SetDirty(kingd);

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

        if (kingd.relationships != null && kingd.relationships.Length > 0)
            KingdomRelation = new Kingdom[kingd.relationships.Length];

        if (kingd.relationsAtWar != null && kingd.relationsAtWar.Length > 0)
        {
            IsAtWar_Bool = new bool[kingd.relationsAtWar.Length];
            int i = 0;

            foreach (var rel in kingd.relationsAtWar)
            {
                if (rel == "true")
                {
                    IsAtWar_Bool[i] = true;
                }
                else
                {
                    IsAtWar_Bool[i] = false;
                }
                i++;
            }
        }

        CreatePoliciesOptions(ref policies_options, ref policies_index, settingsAsset.PoliciesDefinitions);


    }

    public override void OnInspectorGUI()
    {

        if (settingsAsset.currentModule != kingd.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependencies)
                {
                    if (depend == kingd.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + kingd.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + kingd.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + kingd.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + kingd.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + kingd.moduleID + ".asset", typeof(ModuleReceiver));
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
                        assetMng.objID = 7;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = kingd;

                        assetMng.assetName_org = kingd.id;
                        assetMng.assetName_new = kingd.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 1;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = kingd;

                        assetMng.assetName_org = kingd.id;
                        assetMng.assetName_new = kingd.id;
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
        labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);

        EditorGUI.BeginDisabledGroup(true);
        kingd.id = EditorGUILayout.TextField("Kingdom ID", kingd.id);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {
            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 7;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = kingd;

                assetMng.assetName_org = kingd.id;
                assetMng.assetName_new = kingd.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 1;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = kingd;

                assetMng.assetName_org = kingd.id;
                assetMng.assetName_new = kingd.id;
            }
        }

        // kingd.kingdomName = EditorGUILayout.TextField("Kingdom Name", kingd.kingdomName);

        DrawUILine(colUILine, 3, 12);

        SetLabelFieldTS(ref kingd.kingdomName, ref soloName, ref nameTranslationString, folder, translationStringName, "Kingdom Name:", kingd.moduleID, kingd, 1, kingd.id);

        DrawUILine(colUILine, 3, 12);
        // -------------- Kingdom Name END

        // kingd.culture = EditorGUILayout.TextField("Culture:", kingd.culture);

        // CULTURE
        if (kingd.culture != null && kingd.culture != "")
        {
            if (kingd.culture.Contains("Culture."))
            {
                string dataName = kingd.culture.Replace("Culture.", "");
                string asset = dataPath + kingd.moduleID + "/Cultures/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(asset, typeof(Culture));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + kingd.moduleID + ".asset";
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
                                if (depend == kingd.moduleID)
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
                            Debug.Log("Culture " + dataName + " - Not EXIST in" + " ' " + kingd.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }

            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Kingdom Culture:", EditorStyles.label);
        object cultureField = EditorGUILayout.ObjectField(cultureIs, typeof(Culture), true);
        cultureIs = (Culture)cultureField;

        if (cultureIs != null)
        {
            kingd.culture = "Culture." + cultureIs.id;
        }
        else
        {
            kingd.culture = "";
        }
        GUILayout.EndHorizontal();

        //CULTURE END

        // owner
        if (kingd.owner != null && kingd.owner != "")
        {
            if (kingd.owner.Contains("Hero."))
            {
                string dataName = kingd.owner.Replace("Hero.", "");
                string asset = dataPath + kingd.moduleID + "/Heroes/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    kingdomOwner = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + kingd.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                kingdomOwner = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                break;
                            }
                            else
                            {
                                kingdomOwner = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (kingdomOwner == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == kingd.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            kingdomOwner = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingdomOwner == null)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + kingd.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Kingdom Owner:", EditorStyles.label);
        object ownerField = EditorGUILayout.ObjectField(kingdomOwner, typeof(Hero), true);
        kingdomOwner = (Hero)ownerField;

        if (kingdomOwner != null)
        {
            kingd.owner = "Hero." + kingdomOwner.id;
        }
        else
        {
            kingd.owner = "";
        }
        GUILayout.EndHorizontal();

        // flag mesh
        kingd.flag_mesh = EditorGUILayout.TextField("Flag Mesh:", kingd.flag_mesh);

        DrawUILine(colUILine, 3, 12);

        // banner key
        var originLabelWidth = EditorGUIUtility.labelWidth;
        Vector2 textDimensions;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Banner Key:    "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        kingd.banner_key = EditorGUILayout.TextField("Banner Key:", kingd.banner_key);

        EditorGUIUtility.labelWidth = originLabelWidth;

        GUILayout.Space(4);

        if (GUILayout.Button("Edit Banner", GUILayout.Width(80)))
        {
            if (BANNER_EDITOR_Instance == null)
            {
                BannerEditor assetMng = (BannerEditor)ScriptableObject.CreateInstance(typeof(BannerEditor));
                assetMng.bannerKey = kingd.banner_key;
                //assetMng.ReadBannerKey();
                assetMng.inputKingdom = kingd;
                assetMng.inputFaction = null;
                assetMng.inputCulture = null;
            }
            else
            {
                BannerEditor assetMng = BANNER_EDITOR_Instance;
                assetMng.bannerKey = kingd.banner_key;
                //assetMng.ReadBannerKey();
                assetMng.inputKingdom = kingd;
                assetMng.inputFaction = null;
                assetMng.inputCulture = null;
            }
        }

        DrawUILine(colUILine, 3, 12);


        GUILayout.Space(4);

        // Banner Colors
        // color banner primary
        // labelStyle = new GUIStyle(EditorStyles.boldLabel);

        if (kingd.primary_banner_color != null && kingd.primary_banner_color != "")
        {
            if (kingd.primary_banner_color.Contains("0xff"))
            {
                kingd.primary_banner_color = kingd.primary_banner_color.Replace("0xff", "FF");
                // Debug.Log(kingd.primary_banner_color);
            }
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + kingd.primary_banner_color, out primaryBannerColor);
            labelStyle.normal.textColor = new Color(primaryBannerColor.r, primaryBannerColor.g, primaryBannerColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Primary Banner Color:", labelStyle);
            primaryBannerColor = EditorGUILayout.ColorField(primaryBannerColor);
            GUILayout.EndHorizontal();
            kingd.primary_banner_color = ColorUtility.ToHtmlStringRGBA(primaryBannerColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (primaryBannerColor == new Color(0, 0, 0, 0))
            {
                kingd.primary_banner_color = "";
            }
        }
        else
        {
            if (primaryBannerColor == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Primary Banner Color: (Unused)", labelStyle);
                primaryBannerColor = EditorGUILayout.ColorField(primaryBannerColor);
                GUILayout.EndHorizontal();
                kingd.primary_banner_color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(primaryBannerColor.r, primaryBannerColor.g, primaryBannerColor.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Primary Banner Color:", labelStyle);
                primaryBannerColor = EditorGUILayout.ColorField(primaryBannerColor);
                GUILayout.EndHorizontal();
                kingd.primary_banner_color = ColorUtility.ToHtmlStringRGBA(primaryBannerColor);
            }
        }

        // color banner Seconadry
        labelStyle = new GUIStyle(EditorStyles.label);

        if (kingd.secondary_banner_color != null && kingd.secondary_banner_color != "")
        {

            if (kingd.secondary_banner_color.Contains("0xff"))
            {
                kingd.secondary_banner_color = kingd.secondary_banner_color.Replace("0xff", "FF");
                // Debug.Log(kingd.secondary_banner_color);
            }

            labelStyle = new GUIStyle(EditorStyles.boldLabel);

            ColorUtility.TryParseHtmlString("#" + kingd.secondary_banner_color, out secondaryBannerColor);
            labelStyle.normal.textColor = new Color(secondaryBannerColor.r, secondaryBannerColor.g, secondaryBannerColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Secondary Banner Color:", labelStyle);
            secondaryBannerColor = EditorGUILayout.ColorField(secondaryBannerColor);
            GUILayout.EndHorizontal();
            kingd.secondary_banner_color = ColorUtility.ToHtmlStringRGBA(secondaryBannerColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (secondaryBannerColor == new Color(0, 0, 0, 0))
            {
                kingd.secondary_banner_color = "";
            }
        }
        else
        {
            if (secondaryBannerColor == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Secondary Banner Color 2: (Unused)", labelStyle);
                secondaryBannerColor = EditorGUILayout.ColorField(secondaryBannerColor);
                GUILayout.EndHorizontal();
                kingd.secondary_banner_color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(secondaryBannerColor.r, secondaryBannerColor.g, secondaryBannerColor.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Secondary Banner Color 2:", labelStyle);
                secondaryBannerColor = EditorGUILayout.ColorField(secondaryBannerColor);
                GUILayout.EndHorizontal();
                kingd.secondary_banner_color = ColorUtility.ToHtmlStringRGBA(secondaryBannerColor);
            }
        }

        // color labels
        labelStyle = new GUIStyle(EditorStyles.label);

        if (kingd.label_color != null && kingd.label_color != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + kingd.label_color, out labelColor);
            labelStyle.normal.textColor = new Color(labelColor.r, labelColor.g, labelColor.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Label Color:", labelStyle);
            labelColor = EditorGUILayout.ColorField(labelColor);
            GUILayout.EndHorizontal();
            kingd.label_color = ColorUtility.ToHtmlStringRGBA(labelColor);
            // Debug.Log("Color Parsed: " + labelColor);

            if (labelColor == new Color(0, 0, 0, 0))
            {
                kingd.label_color = "";
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
                kingd.label_color = "";
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
                kingd.label_color = ColorUtility.ToHtmlStringRGBA(labelColor);
            }
        }

        // color A
        labelStyle = new GUIStyle(EditorStyles.label);

        if (kingd.color != null && kingd.color != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + kingd.color, out color_a);
            labelStyle.normal.textColor = new Color(color_a.r, color_a.g, color_a.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Kingdom Color 1:", labelStyle);
            color_a = EditorGUILayout.ColorField(color_a);
            GUILayout.EndHorizontal();
            kingd.color = ColorUtility.ToHtmlStringRGBA(color_a);
            // Debug.Log("Color Parsed: " + labelColor);

            if (color_a == new Color(0, 0, 0, 0))
            {
                kingd.color = "";
            }
        }
        else
        {
            if (color_a == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Kingdom Color 1: (Unused)", labelStyle);
                color_a = EditorGUILayout.ColorField(color_a);
                GUILayout.EndHorizontal();
                kingd.color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(color_a.r, color_a.g, color_a.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Kingdom Color 1:", labelStyle);
                color_a = EditorGUILayout.ColorField(color_a);
                GUILayout.EndHorizontal();
                kingd.color = ColorUtility.ToHtmlStringRGBA(color_a);
            }
        }

        // color B
        labelStyle = new GUIStyle(EditorStyles.label);

        if (kingd.color2 != null && kingd.color2 != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + kingd.color2, out color_b);
            labelStyle.normal.textColor = new Color(color_b.r, color_b.g, color_b.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Kingdom Color 2:", labelStyle);
            color_b = EditorGUILayout.ColorField(color_b);
            GUILayout.EndHorizontal();
            kingd.color2 = ColorUtility.ToHtmlStringRGBA(color_b);
            // Debug.Log("Color Parsed: " + labelColor);

            if (color_b == new Color(0, 0, 0, 0))
            {
                kingd.color2 = "";
            }
        }
        else
        {
            if (color_b == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Kingdom Color 2: (Unused)", labelStyle);
                color_b = EditorGUILayout.ColorField(color_b);
                GUILayout.EndHorizontal();
                kingd.color2 = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(color_b.r, color_b.g, color_b.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Kingdom Color 2:", labelStyle);
                color_b = EditorGUILayout.ColorField(color_b);
                GUILayout.EndHorizontal();
                kingd.color2 = ColorUtility.ToHtmlStringRGBA(color_b);
            }
        }

        // alternative Color A
        labelStyle = new GUIStyle(EditorStyles.label);

        if (kingd.alternative_color != null && kingd.alternative_color != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + kingd.alternative_color, out alt_color_a);
            labelStyle.normal.textColor = new Color(alt_color_a.r, alt_color_a.g, alt_color_a.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alternative Kingdom Color 1:", labelStyle);
            alt_color_a = EditorGUILayout.ColorField(alt_color_a);
            GUILayout.EndHorizontal();
            kingd.alternative_color = ColorUtility.ToHtmlStringRGBA(alt_color_a);
            // Debug.Log("Color Parsed: " + labelColor);

            if (alt_color_a == new Color(0, 0, 0, 0))
            {
                kingd.alternative_color = "";
            }
        }
        else
        {
            if (alt_color_a == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Kingdom Color 1: (Unused)", labelStyle);
                alt_color_a = EditorGUILayout.ColorField(alt_color_a);
                GUILayout.EndHorizontal();
                kingd.alternative_color = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(alt_color_a.r, alt_color_a.g, alt_color_a.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Kingdom Color 1:", labelStyle);
                alt_color_a = EditorGUILayout.ColorField(alt_color_a);
                GUILayout.EndHorizontal();
                kingd.alternative_color = ColorUtility.ToHtmlStringRGBA(alt_color_a);
            }
        }

        // alternative Color B
        labelStyle = new GUIStyle(EditorStyles.label);

        if (kingd.alternative_color2 != null && kingd.alternative_color2 != "")
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            ColorUtility.TryParseHtmlString("#" + kingd.alternative_color2, out alt_color_b);
            labelStyle.normal.textColor = new Color(alt_color_b.r, alt_color_b.g, alt_color_b.b, 1);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alternative Kingdom Color 1:", labelStyle);
            alt_color_b = EditorGUILayout.ColorField(alt_color_b);
            GUILayout.EndHorizontal();
            kingd.alternative_color2 = ColorUtility.ToHtmlStringRGBA(alt_color_b);
            // Debug.Log("Color Parsed: " + labelColor);

            if (alt_color_b == new Color(0, 0, 0, 0))
            {
                kingd.alternative_color2 = "";
            }
        }
        else
        {
            if (alt_color_b == new Color(0, 0, 0, 0))
            {
                // Debug.Log("Color Unexist " + labelColor);
                labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Kingdom Color 1: (Unused)", labelStyle);
                alt_color_b = EditorGUILayout.ColorField(alt_color_b);
                GUILayout.EndHorizontal();
                kingd.alternative_color2 = "";
            }
            else
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                // Debug.Log("Color Assigned Mannualy " + labelColor);
                labelStyle.normal.textColor = new Color(alt_color_b.r, alt_color_b.g, alt_color_b.b, 1);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alternative Kingdom Color 1:", labelStyle);
                alt_color_b = EditorGUILayout.ColorField(alt_color_b);
                GUILayout.EndHorizontal();
                kingd.alternative_color2 = ColorUtility.ToHtmlStringRGBA(alt_color_b);
            }
        }

        labelStyle = new GUIStyle(EditorStyles.label);
        /// COLORS END

        // kingd.short_name = EditorGUILayout.TextField("Short Name:", kingd.short_name);

        // Kingdom SHORT Name & translationString tag
        DrawUILine(colUILine, 3, 12);

        // object ID
        // ---------
        // 1 kingdom name
        // 11 kingdom shortName
        // 12 kingdom title
        // 13 kingdom rulerTitle
        // 14 kingdom text

        SetLabelFieldTS(ref kingd.short_name, ref soloShortName, ref soloShortNameTS, folder, TS_soloShortName, "Kingdom Short Name:", kingd.moduleID, kingd, 11, kingd.id);
        DrawUILine(colUILine, 3, 12);

        SetLabelFieldTS(ref kingd.title, ref soloTitle, ref soloTitleTS, folder, TS_soloTitle, "Kingdom Title:", kingd.moduleID, kingd, 12, kingd.id);
        DrawUILine(colUILine, 3, 12);

        SetLabelFieldTS(ref kingd.ruler_title, ref soloRulerTitle, ref soloRulerTitleTS, folder, TS_soloRulerTitle, "Ruler Title:", kingd.moduleID, kingd, 13, kingd.id);
        DrawUILine(colUILine, 3, 12);

        DrawRelationshipsEditor();

        DrawUILine(colUILine, 3, 12);

        DrawPoliciesEditor();

        DrawUILine(colUILine, 3, 12);


        SetTextFieldTS(ref kingd.text, ref soloText, ref textTranslationString, folder, translationStringDescription, "Kingdom Description Text:", kingd.moduleID, kingd, 14, kingd.id);
        // DrawUILine(colUILine, 3, 12);

    }

    void DrawRelationshipsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
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

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Relationships: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showRelationshipsEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Relationships Editor";
        }

        showRelationshipsEditor = EditorGUILayout.Foldout(showRelationshipsEditor, showEditorLabel, hiderStyle);

        if (showRelationshipsEditor)
        {

            EditorGUILayout.LabelField("Relationships Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            // if (kingd.relationships == null)
            // {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((new GUIContent("Add Relation", "Add relation between Kingdoms")), buttonStyle, GUILayout.Width(128)))
            {
                //int i2 = 0;

                //if (kingd.relationships.Length != 0)
                //{
                //    if (kingd.relationships[kingd.relationships.Length - 1] == "")
                //    {
                //        return;
                //    }
                //}
                //var objects = new Dictionary<string, string>();
                //var bools = new List<string>();

                //i2 = 0;
                //foreach (string relationAsset in kingd.relationships)
                //{

                //    objects.Add(relationAsset, kingd.relationValues[i2]);
                //    bools.Add(kingd.relationsAtWar[i2]);

                //    i2++;
                //}

                //kingd.relationships = new string[objects.Count + 1];
                //kingd.relationValues = new string[objects.Count + 1];
                //kingd.relationsAtWar = new string[objects.Count + 1];

                //i2 = 0;
                //foreach (var obj in objects)
                //{
                //    kingd.relationValues[i2] = obj.Value;
                //    kingd.relationships[i2] = obj.Key;
                //    kingd.relationsAtWar[i2] = bools.ToArray()[i2];
                //    i2++;
                //}

                //kingd.relationships[kingd.relationships.Length - 1] = "";
                //kingd.relationValues[kingd.relationValues.Length - 1] = "0";
                //kingd.relationsAtWar[kingd.relationsAtWar.Length - 1] = "false";

                //KingdomRelation = new Kingdom[kingd.relationships.Length];
                //IsAtWar_Bool = new bool[kingd.relationsAtWar.Length];

                //i2 = 0;
                //if (kingd.relationsAtWar.Length != 0)
                //{
                //    foreach (var rel in kingd.relationsAtWar)
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

                var temp = new string[kingd.relationships.Length + 1];
                kingd.relationships.CopyTo(temp, 0);
                kingd.relationships = temp;

                kingd.relationships[kingd.relationships.Length - 1] = "";

                temp = new string[kingd.relationValues.Length + 1];
                kingd.relationValues.CopyTo(temp, 0);
                kingd.relationValues = temp;

                kingd.relationValues[kingd.relationValues.Length - 1] = "0";

                temp = new string[kingd.relationsAtWar.Length + 1];
                kingd.relationsAtWar.CopyTo(temp, 0);
                kingd.relationsAtWar = temp;

                kingd.relationsAtWar[kingd.relationsAtWar.Length - 1] = "false";

                KingdomRelation = new Kingdom[kingd.relationships.Length];
                IsAtWar_Bool = new bool[kingd.relationsAtWar.Length];

                if (kingd.relationsAtWar != null && kingd.relationsAtWar.Length != 0)
                {
                    int i2 = 0;
                    foreach (var rel in kingd.relationsAtWar)
                    {
                        if (rel == "true")
                        {
                            IsAtWar_Bool[i2] = true;
                        }
                        else
                        {
                            IsAtWar_Bool[i2] = false;
                        }
                        i2++;
                    }
                }

                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            // DrawUILine(colUILine, 3, 12);
            // }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            int i = 0;
            if (kingd.relationships.Length != 0)
            {
                foreach (var relation in kingd.relationships)
                {
                    GetKingdomAsset(ref kingd.relationships[i], ref KingdomRelation[i]);
                    i++;
                }

                CheckLinkedData();

                i = 0;
                foreach (var relation in kingd.relationships)
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

                    CreateAttributeToggle(ref IsAtWar_Bool[i], ref kingd.relationsAtWar[i], " Is At War");

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
                        kingd.relationships[i] = "Kingdom." + KingdomRelation[i].id;
                    }
                    else
                    {
                        kingd.relationships[i] = "";
                    }

                    EditorGUI.BeginChangeCheck();

                    CreateIntAttribute(ref kingd.relationValues[i], "Relation Value:");

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
                        // Remove in dependencies

                        //int i2 = 0;
                        //foreach (var rel in KingdomRelation[i].relationships)
                        //{
                        //    if (rel == "Kingdom." + kingd.id)
                        //    {
                        //        var objectsDPD = new Dictionary<string, string>();
                        //        var boolsDPD = new List<string>();
                        //        KingdomRelation[i].relationships[i2] = "remove";

                        //        i2 = 0;
                        //        foreach (string relationAsset in KingdomRelation[i].relationships)
                        //        {
                        //            if (relationAsset != "remove")
                        //            {
                        //                objectsDPD.Add(relationAsset, kingd.relationValues[i2]);
                        //                boolsDPD.Add(kingd.relationsAtWar[i2]);
                        //            }
                        //            i2++;
                        //        }

                        //        KingdomRelation[i].relationships = new string[objectsDPD.Count];
                        //        KingdomRelation[i].relationValues = new string[objectsDPD.Count];
                        //        KingdomRelation[i].relationsAtWar = new string[objectsDPD.Count];

                        //        i2 = 0;
                        //        foreach (var obj in objectsDPD)
                        //        {
                        //            KingdomRelation[i].relationValues[i2] = obj.Value;
                        //            KingdomRelation[i].relationships[i2] = obj.Key;
                        //            KingdomRelation[i].relationsAtWar[i2] = boolsDPD.ToArray()[i2];
                        //            i2++;
                        //        }
                        //    }
                        //    i2++;
                        //}

                        //var objects = new Dictionary<string, string>();
                        //var bools = new List<string>();
                        //kingd.relationships[i] = "remove";

                        //i2 = 0;
                        //foreach (string relationAsset in kingd.relationships)
                        //{
                        //    if (relationAsset != "remove")
                        //    {
                        //        objects.Add(relationAsset, kingd.relationValues[i2]);
                        //        bools.Add(kingd.relationsAtWar[i2]);
                        //    }
                        //    i2++;
                        //}

                        //kingd.relationships = new string[objects.Count];
                        //kingd.relationValues = new string[objects.Count];
                        //kingd.relationsAtWar = new string[objects.Count];

                        //i2 = 0;
                        //foreach (var obj in objects)
                        //{
                        //    kingd.relationValues[i2] = obj.Value;
                        //    kingd.relationships[i2] = obj.Key;
                        //    kingd.relationsAtWar[i2] = bools.ToArray()[i2];
                        //    i2++;
                        //}

                        if (KingdomRelation[i] != null)
                        {
                            foreach (var rel in KingdomRelation[i].relationships)
                            {
                                if (rel != null && rel == "Kingdom." + kingd.id)
                                {
                                    var dpd_count = KingdomRelation[i].relationships.Length - 1;
                                    var dpd_temp_rel = new string[dpd_count];
                                    var dpd_temp_val = new string[dpd_count];
                                    var dpd_temp_war = new string[dpd_count];

                                    int dpd_i2 = 0;
                                    var dpd_i3 = 0;
                                    foreach (string trg in KingdomRelation[i].relationships)
                                    {
                                        if (dpd_i3 != i)
                                        {
                                            dpd_temp_rel[dpd_i2] = KingdomRelation[i].relationships[dpd_i3];
                                            dpd_temp_val[dpd_i2] = KingdomRelation[i].relationValues[dpd_i3];
                                            dpd_temp_war[dpd_i2] = KingdomRelation[i].relationsAtWar[dpd_i3];
                                            dpd_i2++;
                                        }
                                        dpd_i3++;
                                    }

                                    KingdomRelation[i].relationships = dpd_temp_rel;
                                    KingdomRelation[i].relationValues = dpd_temp_val;
                                    KingdomRelation[i].relationsAtWar = dpd_temp_war;
                                }
                            }
                        }

                        var count = kingd.relationships.Length - 1;
                        var temp_rel = new string[count];
                        var temp_val = new string[count];
                        var temp_war = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in kingd.relationships)
                        {
                            if (i3 != i)
                            {
                                temp_rel[i2] = kingd.relationships[i3];
                                temp_val[i2] = kingd.relationValues[i3];
                                temp_war[i2] = kingd.relationsAtWar[i3];
                                i2++;
                            }
                            i3++;
                        }

                        kingd.relationships = temp_rel;
                        kingd.relationValues = temp_val;
                        kingd.relationsAtWar = temp_war;

                        KingdomRelation = new Kingdom[kingd.relationships.Length];
                        IsAtWar_Bool = new bool[kingd.relationsAtWar.Length];

                        if (kingd.relationsAtWar != null && kingd.relationsAtWar.Length != 0)
                        {
                            i2 = 0;
                            foreach (var rel in kingd.relationsAtWar)
                            {
                                if (rel == "true")
                                {
                                    IsAtWar_Bool[i2] = true;
                                }
                                else
                                {
                                    IsAtWar_Bool[i2] = false;
                                }
                                i2++;
                            }
                        }

                        return;

                    }



                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }

    private void CheckLinkedData()
    {
        // Load Liked Data

        if (_IS_changed_Kingdom)
        {
            int i2 = 0;

            foreach (var rel in KingdomRelation)
            {
                if (rel != null && rel.id == _changed_Kingdom)
                {

                    bool existingKingdom = false;
                    foreach (string relationAsset in rel.relationships)
                    {
                        if (relationAsset.Contains("Kingdom." + kingd.id))
                        {
                            existingKingdom = true;
                        }
                    }
                    // Debug.Log(existingKingdom);

                    if (!existingKingdom)
                    {
                        var objectsDic = new Dictionary<string, string>();
                        var boolsList = new List<string>();

                        var i3 = 0;
                        foreach (string relationAsset in rel.relationships)
                        {
                            // Debug.Log(relationAsset);
                            if(!objectsDic.ContainsKey(rel.relationships[i3]))
                            {
                            objectsDic.Add(rel.relationships[i3], rel.relationValues[i3]);
                            boolsList.Add(rel.relationsAtWar[i3]);

                            i3++;
                            }
                        }

                        // foreach (var val in objectsDic.Keys)
                        // {
                        //     Debug.Log(val);
                        // }

                        rel.relationships = new string[objectsDic.Count + 1];
                        rel.relationValues = new string[objectsDic.Count + 1];
                        rel.relationsAtWar = new string[boolsList.Count + 1];

                        i3 = 0;
                        foreach (var obj in objectsDic)
                        {
                            rel.relationValues[i3] = obj.Value;
                            rel.relationships[i3] = obj.Key;
                            rel.relationsAtWar[i3] = boolsList.ToArray()[i3];
                            i3++;
                        }

                        rel.relationships[rel.relationships.Length - 1] = "Kingdom." + kingd.id;

                        i3 = 0;
                        foreach (var kingRel in kingd.relationships)
                        {
                            if (kingRel == "Kingdom." + rel.id)
                            {
                                rel.relationValues[rel.relationValues.Length - 1] = kingd.relationValues[i3];
                                rel.relationsAtWar[rel.relationsAtWar.Length - 1] = kingd.relationsAtWar[i3];
                            }
                            i3++;
                        }

                    }
                    else
                    {
                        int i3 = 0;
                        foreach (string relationAsset in rel.relationships)
                        {
                            if (relationAsset == "Kingdom." + kingd.id)
                            {
                                var i4 = 0;
                                foreach (var kingRel in kingd.relationships)
                                {
                                    if (kingRel == "Kingdom." + rel.id)
                                    {
                                        rel.relationValues[i3] = kingd.relationValues[i4];
                                        rel.relationsAtWar[i3] = kingd.relationsAtWar[i4];
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
                    foreach (var rel in _changed_OLD_Kingdom.relationships)
                    {
                        if (rel == "Kingdom." + kingd.id)
                        {
                            var objects = new Dictionary<string, string>();
                            var bools = new List<string>();
                            _changed_OLD_Kingdom.relationships[i2] = "remove";

                            i2 = 0;
                            foreach (string relationAsset in _changed_OLD_Kingdom.relationships)
                            {
                                if (relationAsset != "remove")
                                {
                                    objects.Add(relationAsset, kingd.relationValues[i2]);
                                    bools.Add(kingd.relationsAtWar[i2]);
                                }
                                i2++;
                            }

                            _changed_OLD_Kingdom.relationships = new string[objects.Count];
                            _changed_OLD_Kingdom.relationValues = new string[objects.Count];
                            _changed_OLD_Kingdom.relationsAtWar = new string[objects.Count];

                            i2 = 0;
                            foreach (var obj in objects)
                            {
                                _changed_OLD_Kingdom.relationValues[i2] = obj.Value;
                                _changed_OLD_Kingdom.relationships[i2] = obj.Key;
                                _changed_OLD_Kingdom.relationsAtWar[i2] = bools.ToArray()[i2];
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


                assetPath = dataPath + kingd.moduleID + "/Kingdoms/" + dataName + ".asset";
                assetPathShort = "/Kingdoms/" + dataName + ".asset";


                if (System.IO.File.Exists(assetPath))
                {
                    kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Kingdom));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + kingd.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependencies)
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

                            foreach (var depend in iSDependencyOfMod.modDependencies)
                            {
                                if (depend == kingd.moduleID)
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

                            Debug.Log("Kingdom " + dataName + " - Not EXIST in" + " ' " + kingd.moduleID + " ' " + "resources, and they dependencies.");

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

            if (TS_Name != "" && TS_Name != "{=}" && TS_Name != "{=*}" && TS_Name != "{=*}{=*}")
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
                                if (depend == kingd.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + kingd.moduleID + " ' " + "resources, and they dependencies.");

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

            if (TS_Name != "" && TS_Name != "{=}" && TS_Name != "{=*}")
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
                                if (depend == kingd.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + kingd.moduleID + " ' " + "resources, and they dependencies.");
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

    void DrawPoliciesEditor()
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

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Policies: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showPoliciesEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Policies Editor";
        }

        showPoliciesEditor = EditorGUILayout.Foldout(showPoliciesEditor, showEditorLabel, hiderStyle);

        if (showPoliciesEditor)
        {

            EditorGUILayout.LabelField("Policies Editor", titleStyle, GUILayout.ExpandWidth(true));
            DrawUILine(colUILine, 3, 12);


            if (kingd.policies == null || kingd.policies.Length < settingsAsset.PoliciesDefinitions.Length)
            {
                EditorGUILayout.BeginHorizontal();

                policies_index = EditorGUILayout.Popup("Policies:", policies_index, policies_options, GUILayout.Width(320));
                // kingd.policies[i] = skills_options[skills_index];


                // DrawUILine(colUILine, 3, 12);
                if (GUILayout.Button((new GUIContent("Add Policy", "Add selected Policies for this Kingdom")), buttonStyle, GUILayout.Width(128)))
                {

                    var objects = new List<string>();

                    int i2 = 0;
                    foreach (string skillAsset in kingd.policies)
                    {
                        objects.Add(skillAsset);
                        i2++;
                    }

                    int indexVal = objects.Count + 1;

                    kingd.policies = new string[indexVal];

                    i2 = 0;
                    foreach (var element in objects)
                    {
                        kingd.policies[i2] = element;
                        i2++;
                    }

                    kingd.policies[kingd.policies.Length - 1] = "policy_" + policies_options[policies_index];

                    CreatePoliciesOptions(ref policies_options, ref policies_index, settingsAsset.PoliciesDefinitions);
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
            if (kingd.policies.Length != 0)
            {
                foreach (var skill in kingd.policies)
                {
                    titleStyle.fontSize = 13;
                    titleStyle.normal.textColor = newCol2;

                    string soloPolicyName = kingd.policies[i].Replace("policy_", "");
                    EditorGUILayout.LabelField("Policy - " + soloPolicyName, titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button((new GUIContent("X", "Remove Policy")), buttonStyle, GUILayout.Width(32)))
                    {
                        var objects = new List<string>();
                        kingd.policies[i] = "remove";

                        int i2 = 0;
                        foreach (string skillAsset in kingd.policies)
                        {
                            if (skillAsset != "remove")
                            {
                                objects.Add(skillAsset);
                            }
                            i2++;
                        }

                        kingd.policies = new string[objects.Count];

                        i2 = 0;
                        foreach (var obj in objects)
                        {
                            kingd.policies[i2] = obj;
                            i2++;
                        }
                        CreatePoliciesOptions(ref policies_options, ref policies_index, settingsAsset.PoliciesDefinitions);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawUILine(colUILine, 3, 12);
                    i++;
                }
            }
        }
    }

    private void CreatePoliciesOptions(ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS


        var listOptionsAll = new List<string>();
        var listOptionsLoaded = new List<string>();

        foreach (var data in definitions)
        {
            string soloPolicyName = data.Replace("policy_", "");
            listOptionsAll.Add(soloPolicyName);

        }

        foreach (var dataPolice in kingd.policies)
        {
            string soloPolicyName = dataPolice.Replace("policy_", "");
            listOptionsLoaded.Add(soloPolicyName);
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

}
