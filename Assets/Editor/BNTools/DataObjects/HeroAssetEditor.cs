using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

[CustomEditor(typeof(Hero))]
public class HeroAssetEditor : Editor
{

    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string folder = "HeroesTranslationData";
    Hero hero;
    public Culture cultureIs;

    Color primaryBannerColor;
    public bool isAliveBtn = true;
    public bool isNobleBtn;

    public Faction heroFaction;
    public Hero heroFather;
    public Hero heroMother;
    public Hero heroSpouse;
    string soloText;
    string textTranslationString;
    TranslationString translationStringDescription;

    // Update 1.8.0
    public string[] preferredUpgradeFormation_options;
    public int preferredUpgradeFormation_index;

    Vector2 textScrollPos;

    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    bool isDependency = false;
    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";

    public void OnEnable()
    {
        hero = (Hero)target;
        EditorUtility.SetDirty(hero);

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

        if (settingsAsset.currentModule != hero.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == hero.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + hero.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + hero.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + hero.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + hero.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + hero.moduleID + ".asset", typeof(ModuleReceiver));
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
                        assetMng.objID = 4;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = hero;

                        assetMng.assetName_org = hero.id;
                        assetMng.assetName_new = hero.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 4;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = hero;

                        assetMng.assetName_org = hero.id;
                        assetMng.assetName_new = hero.id;
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

        preferredUpgradeFormation_options = new string[settingsAsset.PreferredUpgradeFormationDefinitions.Length];

        int i = 0;
        foreach (var category in settingsAsset.PreferredUpgradeFormationDefinitions)
        {
            preferredUpgradeFormation_options[i] = category;
            if ((preferredUpgradeFormation_options[i]) == hero.preferred_upgrade_formation)
            {
                preferredUpgradeFormation_index = i;
            }
            i++;
        }

        EditorGUI.BeginDisabledGroup(isDependency);


        EditorGUI.BeginDisabledGroup(true);
        hero.id = EditorGUILayout.TextField("Hero ID:", hero.id);

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {

            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 4;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = hero;

                assetMng.assetName_org = hero.id;
                assetMng.assetName_new = hero.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 4;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = hero;

                assetMng.assetName_org = hero.id;
                assetMng.assetName_new = hero.id;
            }

        }

        DrawUILine(colUILine, 3, 12);


        var originLabelWidth = EditorGUIUtility.labelWidth;
        Vector2 textDimensions;
        EditorGUILayout.BeginHorizontal();

        // IS Alive

        if (hero.alive != null)
        {
            if (hero.alive == "false")
            {
                isAliveBtn = false;
            }
            else
            {
                isAliveBtn = true;
            }
        }


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Alive:"));
        EditorGUIUtility.labelWidth = textDimensions.x;

        isAliveBtn = EditorGUILayout.Toggle("Alive:", isAliveBtn);
        DrawUILineVertical(colUILine, 1, 1, 16);
        // EditorGUILayout.Space(-5);



        if (isAliveBtn)
        {
            hero.alive = "true";
        }
        else
        {
            hero.alive = "false";
        }

        // IS NOBLE (removed in 1.8.0)
        //if (hero.is_noble != null)
        //{
        //    if (hero.is_noble == "true")
        //    {
        //        isNobleBtn = true;
        //    }
        //    else
        //    {
        //        isNobleBtn = false;
        //    }
        //}
        //textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Noble:"));
        //EditorGUIUtility.labelWidth = textDimensions.x;

        //isNobleBtn = EditorGUILayout.Toggle("Is Noble:", isNobleBtn);
        //DrawUILineVertical(colUILine, 1, 1, 16);
        //// EditorGUILayout.Space(-5);


        //if (isNobleBtn)
        //{
        //    hero.is_noble = "true";
        //}
        //else
        //{
        //    hero.is_noble = "false";
        //}


        // Hero Faction
        if (hero.faction != null && hero.faction != "")
        {
            if (hero.faction.Contains("Faction."))
            {
                string dataName = hero.faction.Replace("Faction.", "");
                string asset = dataPath + hero.moduleID + "/Factions/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    heroFaction = (Faction)AssetDatabase.LoadAssetAtPath(asset, typeof(Faction));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + hero.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Factions/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                heroFaction = (Faction)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Faction));
                                break;
                            }
                            else
                            {
                                heroFaction = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (heroFaction == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == hero.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.factionsData.factions)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Factions/" + dataName + ".asset";
                                            heroFaction = (Faction)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Faction));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (heroFaction == null)
                        {
                            Debug.Log("Faction " + dataName + " - Not EXIST in" + " ' " + hero.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Hero Faction:"));
        EditorGUIUtility.labelWidth = textDimensions.x;

        EditorGUILayout.LabelField("Hero Faction:", EditorStyles.label);

        EditorGUILayout.Space(-32);
        heroFaction = (Faction)EditorGUILayout.ObjectField(heroFaction, typeof(Faction), true, GUILayout.Width(256));
        //heroFaction = (Faction)factionField;

        if (heroFaction != null)
        {
            hero.faction = "Faction." + heroFaction.id;
        }
        else
        {
            hero.faction = "";
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = originLabelWidth;

        EditorGUILayout.Space(8);



        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Banner Key:    "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        hero.banner_key = EditorGUILayout.TextField("Banner Key:", hero.banner_key);

        EditorGUIUtility.labelWidth = originLabelWidth;

        // Family 
        DrawUILine(colUILine, 3, 12);
        EditorGUILayout.LabelField("Hero Family:", EditorStyles.boldLabel);
        // Father
        if (hero.father != null && hero.father != "")
        {
            if (hero.father.Contains("Hero."))
            {
                string dataName = hero.father.Replace("Hero.", "");
                string asset = dataPath + hero.moduleID + "/Heroes/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    heroFather = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + hero.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                heroFather = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                break;
                            }
                            else
                            {
                                heroFather = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (heroFather == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == hero.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            heroFather = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (heroFather == null)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + hero.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }
        else
        {
            heroFather = null;
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Father:", EditorStyles.label);

        heroFather = (Hero)EditorGUILayout.ObjectField(heroFather, typeof(Hero), true);
        //heroFather = (Hero)fatherField;

        if (heroFather != null)
        {
            hero.father = "Hero." + heroFather.id;
        }
        else
        {
            hero.father = "";
        }
        GUILayout.EndHorizontal();



        // Mother
        if (hero.mother != null && hero.mother != "")
        {
            if (hero.mother.Contains("Hero."))
            {
                string dataName = hero.mother.Replace("Hero.", "");
                string asset = dataPath + hero.moduleID + "/Heroes/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    heroMother = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + hero.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                heroMother = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                break;
                            }
                            else
                            {
                                heroMother = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (heroMother == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == hero.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            heroMother = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (heroMother == null)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + hero.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }
        else
        {
            heroMother = null;
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Mother:", EditorStyles.label);
        heroMother = (Hero)EditorGUILayout.ObjectField(heroMother, typeof(Hero), true);
        //heroMother = (Hero)motherField;

        if (heroMother != null)
        {
            hero.mother = "Hero." + heroMother.id;
        }
        else
        {
            hero.mother = "";
        }
        GUILayout.EndHorizontal();


        // Spouse
        if (hero.spouse != null && hero.spouse != "")
        {
            if (hero.spouse.Contains("Hero."))
            {
                string dataName = hero.spouse.Replace("Hero.", "");
                string asset = dataPath + hero.moduleID + "/Heroes/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    heroSpouse = (Hero)AssetDatabase.LoadAssetAtPath(asset, typeof(Hero));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + hero.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Heroes/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                heroSpouse = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                break;
                            }
                            else
                            {
                                heroSpouse = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (heroSpouse == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == hero.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.heroesData.heroes)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Heroes/" + dataName + ".asset";
                                            heroSpouse = (Hero)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Hero));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (heroSpouse == null)
                        {
                            Debug.Log("Hero " + dataName + " - Not EXIST in" + " ' " + hero.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }
        else
        {
            heroSpouse = null;
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Spouse:", EditorStyles.label);
        heroSpouse = (Hero)EditorGUILayout.ObjectField(heroSpouse, typeof(Hero), true);
        //heroSpouse = (Hero)spouseField;

        if (heroSpouse != null)
        {
            hero.spouse = "Hero." + heroSpouse.id;

            // var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));

            // foreach (var hero in currModSettings.modFilesData.heroesData.heroes)
            // {
            //     if (hero.id == heroSpouse.id)
            //     {
            //         heroSpouse.spouse = "Hero." + hero.id;
            //     }
            // }
        }
        else
        {
            hero.spouse = "";
            // heroSpouse.spouse = "";
        }
        GUILayout.EndHorizontal();
        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.LabelField("Preferred Upgrade Formation:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        // EditorGUILayout.Space(2);

        preferredUpgradeFormation_index = EditorGUILayout.Popup(preferredUpgradeFormation_index, preferredUpgradeFormation_options, GUILayout.Width(128));
        hero.preferred_upgrade_formation = preferredUpgradeFormation_options[preferredUpgradeFormation_index];

        DrawUILine(colUILine, 1, 6);

        // 4 hero text
        SetTextFieldTS(ref hero.text, ref soloText, ref textTranslationString, folder, translationStringDescription, "Hero Description Text:", hero.moduleID, hero, 4, hero.id);

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
                                if (depend == hero.moduleID)
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
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + hero.moduleID + " ' " + "resources, and they dependencies.");
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
