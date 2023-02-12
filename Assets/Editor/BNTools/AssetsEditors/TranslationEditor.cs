using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

[System.Serializable]
public class TranslationEditor : EditorWindow
{

    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    public string module;
    public TranslationString defaultData;

    public TranslationString currentTranslationData;
    public Dictionary<string, TranslationString> translationsData;

    public UnityEngine.Object obj;
    public int objectID;
    public int areaID;
    public int nameID;
    public string translationLabel;
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    Vector2 textScrollPos_A;
    Vector2 textScrollPos_B;
    public int index = 0;
    public string[] options;
    public bool isDependency;


    public int ts_editor_index = 0;
    string[] ts_editor_options = new string[] { "Default", "Plural" };


    public GUIStyle idStyle;


    void OnGUI()
    {

        if (translationsData != null)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField(translationLabel, EditorStyles.centeredGreyMiniLabel);
            idStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            idStyle.fontSize = 16;
            // idStyle.fontStyle = EditorStyles.centeredGreyMiniLabel.fontStyle;

            if (options == null)
            {
                options = new string[translationsData.Keys.Count - 1];

                int i = 0;
                foreach (string language in translationsData.Keys)
                {
                    if (language != "English")
                    {
                        options[i] = language;
                        i++;
                    }

                    
                }
            }



            /// If translation String Dont Exist
            if (defaultData != null)
            {
                EditorUtility.SetDirty(defaultData);
                if (!isDependency)
                {


                    DrawUILine(colUILine, 3, 12);
                    GUILayout.BeginHorizontal();
                    if (ts_editor_index == 0)
                        EditorGUILayout.SelectableLabel("String: " + defaultData.id, idStyle);
                    else if (ts_editor_index == 1)
                        EditorGUILayout.SelectableLabel("String: {@Plural}", idStyle);
                    GUILayout.Space(128);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Select Language: ", EditorStyles.boldLabel);
                    GUILayout.Space(-48);
                    index = EditorGUILayout.Popup(index, options, GUILayout.Width(96));
                    GUILayout.Space(-64);
                    GUILayout.EndHorizontal();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();

                    var originLabelWidth = EditorGUIUtility.labelWidth;
                    var textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Edit:  "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    EditorGUILayout.PrefixLabel(" Edit: ", EditorStyles.boldLabel);

                    EditorGUIUtility.labelWidth = originLabelWidth;
                    //GUILayout.Space(-48);
                    ts_editor_index = EditorGUILayout.Popup(ts_editor_index, ts_editor_options, GUILayout.Width(96));
                    GUILayout.Space(-64);
                    GUILayout.EndHorizontal();
                    DrawUILine(colUILine, 3, 12);

                    GUILayout.BeginHorizontal();
                    if (ts_editor_index == 0)
                        EditorGUILayout.LabelField("Default Text:", EditorStyles.boldLabel);
                    if (ts_editor_index == 1)
                        EditorGUILayout.LabelField("Plural Text:", EditorStyles.boldLabel);
                    if (currentTranslationData != null)
                    {
                        EditorGUILayout.LabelField("Translation Text:", EditorStyles.boldLabel);
                    }
                    // else
                    // {
                    //     EditorGUILayout.LabelField("WARINING:", EditorStyles.foldout);
                    // }

                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();


                    textScrollPos_A = EditorGUILayout.BeginScrollView(textScrollPos_A, GUILayout.Height(256), GUILayout.Width(320));

                    // draw default text
                    GUILayout.BeginVertical();
                    if (ts_editor_index == 0)
                        defaultData.stringText = GUILayout.TextArea(defaultData.stringText);
                    if (ts_editor_index == 1)
                        defaultData.translationPluralString = GUILayout.TextArea(defaultData.translationPluralString);


                    GUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();

                    DrawUILineVertical(colUILine, 1, 1, 16);


                    textScrollPos_B = EditorGUILayout.BeginScrollView(textScrollPos_B, GUILayout.Height(256), GUILayout.Width(320));
                    // draw translation text
                    GUILayout.BeginVertical();

                    translationsData.TryGetValue(options[index], out currentTranslationData);

                    if (currentTranslationData != null)
                        EditorUtility.SetDirty(currentTranslationData);

                    if (defaultData.langImported == false)
                    {
                        if (GUILayout.Button("Import Existing Languages Strings"))
                        {

                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                            string defaultPath = "Assets/Resources/SubModulesData/" + module + "/Languages";


                            if (Directory.Exists(defaultPath))
                            {
                                // Debug.Log($"Load: {defaultPath}");

                                LoadTSFromLanguageData(module);
                            }

                            foreach (var mod in currentMod.modDependenciesInternal)
                            {
                                if (System.IO.File.Exists(modsSettingsPath + mod + ".asset"))
                                {
                                    defaultPath = "Assets/Resources/SubModulesData/" + mod + "/Languages";

                                    if (Directory.Exists(defaultPath))
                                    {
                                        LoadTSFromLanguageData(mod);
                                    }

                                }

                            }

                            string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                            foreach (string mod in mods)
                            {
                                ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                                foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                                {
                                    if (depend == module)
                                    {
                                        defaultPath = "Assets/Resources/SubModulesData/" + iSDependencyOfMod.id + "/Languages";

                                        if (Directory.Exists(defaultPath))
                                        {
                                            LoadTSFromLanguageData(iSDependencyOfMod.id);
                                        }

                                    }
                                }
                            }

                            defaultData.langImported = true;
                        }
                    }
                    else
                    {
                        if (currentTranslationData != null)
                        {
                            if (ts_editor_index == 0)
                                currentTranslationData.stringText = GUILayout.TextArea(currentTranslationData.stringText);
                            if (ts_editor_index == 1)
                                currentTranslationData.translationPluralString = GUILayout.TextArea(currentTranslationData.translationPluralString);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox(options[index] + " language dont contains translation for this string", MessageType.Warning);
                            // GUILayout.Label(options[index] + " language dont contains translation for this string", EditorStyles.centeredGreyMiniLabel);
                            // GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Generate Translation String"))
                            {

                                ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                var langFolderName = "UNKLOWN";
                                var langName = "UNKLOWN";
                                foreach (var lang in currentMod.modFilesData.languagesData.languages)
                                {
                                    if (lang.languageName == options[index])
                                    {
                                        langFolderName = lang.languageID;
                                        langName = lang.languageName;
                                    }
                                }

                                CreateTSData(ref currentMod.modFilesData, langName, langFolderName);

                                isDependency = false;
                            }
                            // EditorGUILayout.EndHorizontal();

                        }
                    }

                    GUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    DrawUILine(colUILine, 3, 12);
                    EditorGUILayout.HelpBox("This string " + defaultData.id + " is used as dependency." + "\n"
                    + "To edit translations, copy and override this string to current (" + module + ") module." + "\n" +
                    "If is a copy asset, Generate new Translation String (Recomended), this string can be used by another asset", MessageType.Warning);

                    defaultData.stringTranslationID = "";

                    // ? object ID
                    // ? ---------
                    // 0 culture name
                    // -1 culture text
                    // -2 culture maleNames
                    // -3 culture femaleNames
                    // -4 culture clanNames
                    // 1 kingdom name
                    // 11 kingdom shortName
                    // 12 kingdom title
                    // 13 kingdom rulerTitle
                    // 14 kingdom text
                    // 2 faction name
                    // 21 faction text
                    // 22 faction text
                    // 3 NPC
                    // 4 hero text
                    // 5 settlement name
                    // 51 settlement text
                    // 52 settlement Area Name
                    // 6 partyTemplate
                    // 7 item
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Generate Translation Key"))
                    {
                        if (obj != null)
                        {

                            if (objectID == 0)
                            {
                                Culture cult = obj as Culture;
                                TranslationKeyGenerator(ref cult.cultureName, cult.moduleID, "CulturesTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == -1)
                            {
                                Culture cult = obj as Culture;
                                TranslationKeyGenerator(ref cult.text, cult.moduleID, "CulturesTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == -2)
                            {
                                Culture cult = obj as Culture;
                                TranslationKeyGenerator(ref cult.male_names[nameID], cult.moduleID, "CulturesTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == -3)
                            {
                                Culture cult = obj as Culture;
                                TranslationKeyGenerator(ref cult.female_names[nameID], cult.moduleID, "CulturesTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == -4)
                            {
                                Culture cult = obj as Culture;
                                TranslationKeyGenerator(ref cult.clan_names[nameID], cult.moduleID, "CulturesTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 1)
                            {
                                Kingdom kingd = obj as Kingdom;
                                TranslationKeyGenerator(ref kingd.kingdomName, kingd.moduleID, "KingdomsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 11)
                            {
                                Kingdom kingd = obj as Kingdom;
                                TranslationKeyGenerator(ref kingd.short_name, kingd.moduleID, "KingdomsTranslationData", true);

                                isDependency = false;
                            }
                            else if (objectID == 12)
                            {
                                Kingdom kingd = obj as Kingdom;
                                TranslationKeyGenerator(ref kingd.title, kingd.moduleID, "KingdomsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 13)
                            {
                                Kingdom kingd = obj as Kingdom;
                                TranslationKeyGenerator(ref kingd.ruler_title, kingd.moduleID, "KingdomsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 14)
                            {
                                Kingdom kingd = obj as Kingdom;
                                TranslationKeyGenerator(ref kingd.text, kingd.moduleID, "KingdomsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 2)
                            {
                                Faction fac = obj as Faction;
                                TranslationKeyGenerator(ref fac.factionName, fac.moduleID, "FactionsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 21)
                            {
                                Faction fac = obj as Faction;
                                TranslationKeyGenerator(ref fac.text, fac.moduleID, "FactionsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 22)
                            {
                                Faction fac = obj as Faction;
                                TranslationKeyGenerator(ref fac.short_name, fac.moduleID, "FactionsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 3)
                            {
                                NPCCharacter npc = obj as NPCCharacter;
                                TranslationKeyGenerator(ref npc.npcName, npc.moduleID, "NPCTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 4)
                            {
                                Hero heroObj = obj as Hero;
                                TranslationKeyGenerator(ref heroObj.text, heroObj.moduleID, "HeroesTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 5)
                            {
                                Settlement sett = obj as Settlement;
                                TranslationKeyGenerator(ref sett.settlementName, sett.moduleID, "SettlementsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 51)
                            {
                                Settlement sett = obj as Settlement;
                                Debug.Log(sett.moduleID);
                                TranslationKeyGenerator(ref sett.text, sett.moduleID, "SettlementsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 52)
                            {
                                Settlement sett = obj as Settlement;
                                Debug.Log(sett.moduleID);
                                TranslationKeyGenerator(ref sett.AREA_name[areaID], sett.moduleID, "SettlementsTranslationData", true);
                                isDependency = false;
                            }
                            else if (objectID == 7)
                            {
                                Item item = obj as Item;
                                Debug.Log(item.moduleID);
                                TranslationKeyGenerator(ref item.itemName, item.moduleID, "ItemsTranslationData", true);
                                isDependency = false;
                            }
                        }
                    }

                    if (GUILayout.Button("Copy & Override Translation Asset"))
                    {
                        if (obj != null)
                        {
                            if (objectID == 0 || objectID == -1)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/CulturesTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                            else if (objectID == 5 || objectID == 51)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/SettlementsTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                            else if (objectID == 1 || objectID == 11 || objectID == 12 || objectID == 13 || objectID == 14)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/KingdomsTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                            else if (objectID == 2 || objectID == 21 || objectID == 22)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/FactionsTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                            else if (objectID == 3)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/NPCTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                            else if (objectID == 4)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/HeroesTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                            else if (objectID == 7)
                            {
                                var originPath = AssetDatabase.GetAssetPath(defaultData);
                                var newPath = dataPath + module + "/TranslationData/ItemsTranslationData/" + defaultData.id + ".asset";

                                AssetDatabase.CopyAsset(originPath, newPath);

                                TranslationString cultTS = (TranslationString)AssetDatabase.LoadAssetAtPath(newPath, typeof(TranslationString));

                                cultTS.moduleID = module;

                                if (System.IO.File.Exists(modsSettingsPath + module + ".asset"))
                                {
                                    ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + module + ".asset", typeof(ModuleReceiver));

                                    currentMod.modFilesData.translationData.translationStrings.Add(cultTS);
                                    defaultData = cultTS;
                                    isDependency = false;
                                }

                            }
                        }

                    }
                    EditorGUILayout.EndHorizontal();
                }

            }

            // ? object ID
            // ? ---------
            // 0 culture name
            // -1 culture text
            // 1 kingdom name
            // 11 kingdom shortName
            // 12 kingdom title
            // 13 kingdom rulerTitle
            // 14 kingdom text
            // 2 faction name
            // 21 faction text
            // 3 NPC
            // 4 hero text
            // 5 settlement name
            // 51 settlement text
            // 6 partyTemplate
            // 7 item

            if (obj != null)
            {
                if (objectID == 0)
                {
                    Culture cult = obj as Culture;
                    if (defaultData != null)
                    {
                        cult.cultureName = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref cult.cultureName, cult.moduleID, "CulturesTranslationData", false);

                    }
                }
                else if (objectID == -1)
                {
                    Culture cult = obj as Culture;
                    if (defaultData != null)
                    {
                        cult.text = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref cult.text, cult.moduleID, "CulturesTranslationData", false);

                    }
                }
                else if (objectID == -2)
                {
                    Culture cult = obj as Culture;
                    if (defaultData != null)
                    {
                        cult.male_names[nameID] = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref cult.male_names[nameID], cult.moduleID, "CulturesTranslationData", false);

                    }
                }
                else if (objectID == -3)
                {
                    Culture cult = obj as Culture;
                    if (defaultData != null)
                    {
                        cult.female_names[nameID] = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref cult.female_names[nameID], cult.moduleID, "CulturesTranslationData", false);

                    }
                }
                else if (objectID == -4)
                {
                    Culture cult = obj as Culture;
                    if (defaultData != null)
                    {
                        cult.clan_names[nameID] = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref cult.clan_names[nameID], cult.moduleID, "CulturesTranslationData", false);

                    }
                }
                else if (objectID == 1)
                {
                    Kingdom kingd = obj as Kingdom;
                    if (defaultData != null)
                    {
                        kingd.kingdomName = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref kingd.kingdomName, kingd.moduleID, "KingdomsTranslationData", false);

                    }
                }
                else if (objectID == 11)
                {
                    Kingdom kingd = obj as Kingdom;
                    if (defaultData != null)
                    {
                        kingd.short_name = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref kingd.short_name, kingd.moduleID, "KingdomsTranslationData", false);
                    }
                }
                else if (objectID == 12)
                {
                    Kingdom kingd = obj as Kingdom;
                    if (defaultData != null)
                    {
                        kingd.title = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref kingd.title, kingd.moduleID, "KingdomsTranslationData", false);
                    }
                }
                else if (objectID == 13)
                {
                    Kingdom kingd = obj as Kingdom;
                    if (defaultData != null)
                    {
                        kingd.ruler_title = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref kingd.ruler_title, kingd.moduleID, "KingdomsTranslationData", false);
                    }
                }
                else if (objectID == 14)
                {
                    Kingdom kingd = obj as Kingdom;
                    if (defaultData != null)
                    {
                        kingd.text = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref kingd.text, kingd.moduleID, "KingdomsTranslationData", false);
                    }

                }
                else if (objectID == 2)
                {
                    Faction fac = obj as Faction;
                    if (defaultData != null)
                    {
                        fac.factionName = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref fac.factionName, fac.moduleID, "FactionsTranslationData", false);
                    }

                }
                else if (objectID == 21)
                {
                    Faction fac = obj as Faction;
                    if (defaultData != null)
                    {
                        fac.text = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref fac.text, fac.moduleID, "FactionsTranslationData", false);
                    }
                }
                else if (objectID == 22)
                {
                    Faction fac = obj as Faction;
                    if (defaultData != null)
                    {
                        fac.short_name = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref fac.short_name, fac.moduleID, "FactionsTranslationData", false);
                    }
                }
                else if (objectID == 3)
                {
                    NPCCharacter npc = obj as NPCCharacter;
                    if (defaultData != null)
                    {
                        npc.npcName = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref npc.npcName, npc.moduleID, "NPCTranslationData", false);
                    }
                }
                else if (objectID == 4)
                {
                    Hero heroObj = obj as Hero;

                    if (defaultData != null)
                    {
                        heroObj.text = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref heroObj.text, heroObj.moduleID, "HeroesTranslationData", false);
                    }
                }
                else if (objectID == 5)
                {
                    Settlement sett = obj as Settlement;
                    if (defaultData != null)
                    {
                        sett.settlementName = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref sett.settlementName, sett.moduleID, "SettlementsTranslationData", false);
                    }
                }
                else if (objectID == 51)
                {
                    Settlement sett = obj as Settlement;
                    if (defaultData != null)
                    {
                        sett.text = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref sett.text, sett.moduleID, "SettlementsTranslationData", false);
                    }
                }
                else if (objectID == 52)
                {
                    Settlement sett = obj as Settlement;
                    if (defaultData != null)
                    {
                        sett.AREA_name[areaID] = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref sett.AREA_name[areaID], sett.moduleID, "SettlementsTranslationData", false);
                    }
                }
                else if (objectID == 7)
                {
                    Item item = obj as Item;
                    if (defaultData != null)
                    {
                        item.itemName = defaultData.id + defaultData.stringText;
                    }
                    else
                    {
                        TranslationKeyGenerator(ref item.itemName, item.moduleID, "ItemsTranslationData", false);
                    }

                }
            }
        }
    }

    private void TranslationKeyGenerator(ref string stringTS, string modName, string TSfoldeName, bool GenDataBool)
    {

        DrawUILine(colUILine, 3, 12);

        var soloString = stringTS;
        var TS_Name = stringTS;
        Regex regex = new Regex("{=(.*)}");
        if (regex != null)
        {
            var v = regex.Match(TS_Name);
            string s = v.Groups[1].ToString();
            TS_Name = "{=" + s + "}";
        }
        soloString = soloString.Replace(TS_Name, "");

        EditorGUILayout.HelpBox("This string dont contain translation Key", MessageType.Warning);

        //  Generate translation key
        // ! Left to check if key exist

        if (GUILayout.Button("Generate Translation Key") || GenDataBool == true)
        {
            TranslationKeyGenerator gen = new TranslationKeyGenerator();
            var key = gen.GenerateKey();

            TranslationString keyString = new TranslationString();
            keyString.stringTranslationID = key;
            keyString.id = "{=" + key + "}";
            keyString.stringText = soloString;
            keyString.editorType = "Created";

            string configPath = "Assets/Resources/SubModulesData/" + modName + "/" + modName + "_Config.asset";
            if (System.IO.File.Exists(configPath))
            {
                ModFiles data = (ModFiles)AssetDatabase.LoadAssetAtPath(configPath, typeof(ModFiles));
                data.translationData.translationStrings.Add(keyString);

                string transPath = data.modResourcesPath + "/TranslationData/" + TSfoldeName + "/" + keyString.id + ".asset";
                EditorUtility.SetDirty(keyString);
                AssetDatabase.CreateAsset(keyString, transPath);
                //AssetDatabase.SaveAssets();

                defaultData = keyString;
                stringTS = keyString.id + soloString;

                SearchStrings();

                options = new string[translationsData.Keys.Count];

                int i = 0;
                foreach (string language in translationsData.Keys)
                {
                    options[i] = language;
                    i++;
                }

            }



        }
        DrawUILine(colUILine, 3, 12);
        EditorGUILayout.LabelField("Default Text:", EditorStyles.boldLabel);
        stringTS = GUILayout.TextField(soloString);
    }

    public void SearchStrings()
    {
        translationsData = new Dictionary<string, TranslationString>();

        string configPath = "Assets/Resources/SubModulesData/" + module + "/" + module + "_Config.asset";

        ModFiles modFilesAsset = (ModFiles)AssetDatabase.LoadAssetAtPath(configPath, typeof(ModFiles));

        // Debug.Log(modFilesAsset.translationData.translationStrings.Count);

        // string[] transDataFile = Directory.GetFiles(dataPath + module + "/TranslationData/_Languages", "*.asset");
        if (defaultData != null)
        {
            string[] lenguageDirectories = Directory.GetDirectories(dataPath + module + "/TranslationData/_Languages");
            string[] XMLfiles = Directory.GetFiles(dataPath + module + "/TranslationData/_Languages", "*.asset");

            foreach (ModLanguage lang in modFilesAsset.languagesData.languages)
            {
                string[] TS_files = Directory.GetFiles(dataPath + module + "/TranslationData/_Languages/" + lang.languageID, "*.asset");

                translationsData.Add(lang.languageName, null);

                // string[] transFiles = Directory.GetFiles(dir, "*.asset");
                foreach (var file in TS_files)
                {
                    TranslationString trans = (TranslationString)AssetDatabase.LoadAssetAtPath(file, typeof(TranslationString));

                    if (trans.name.Contains(defaultData.stringTranslationID))
                    {
                        if (translationsData.ContainsKey(trans.lenguageTag))
                        {
                            translationsData[trans.lenguageTag] = trans;
                        }
                        else
                        {
                            translationsData.Add(trans.lenguageTag, trans);
                        }

                    }
                    else
                    {
                        if (!translationsData.ContainsKey(trans.lenguageTag))
                        {
                            translationsData.Add(trans.lenguageTag, null);
                        }
                    }
                }
            }

        }
    }

    public void LoadTSFromLanguageData(string modName)
    {
        string engPath = "Assets/Resources/SubModulesData/" + modName + "/Languages";
        string configPath = "Assets/Resources/SubModulesData/" + module + "/" + module + "_Config.asset";
        ModFiles modFilesAsset = (ModFiles)AssetDatabase.LoadAssetAtPath(configPath, typeof(ModFiles));

        string[] engXMLfiles = Directory.GetFiles(engPath, "*.XML");
        // get lenguage files

        foreach (string lenguageFile in engXMLfiles)
        {

            XmlDocument Doc = new XmlDocument();
            // UTF 8 - 16
            StreamReader reader = new StreamReader(lenguageFile);
            Doc.Load(reader);
            reader.Close();

            XmlElement Root = Doc.DocumentElement;
            XmlNodeList XML = Root.ChildNodes;
            // List<string> languageTags = new List<string>();

            // Debug.Log(Root.ChildNodes.Count);
            string lenguageFolderName = "EN";
            string langTag = "";

            foreach (XmlNode langNode in Root.ChildNodes)
            {
                //Debug.Log(langNode.Name);

                if (langNode.Name == "tags")
                {
                    langTag = langNode.FirstChild.Attributes["language"].Value;
                }

                if (langNode.Name == "strings" && langTag != "")
                {

                    foreach (XmlNode nodeChild in langNode.ChildNodes)
                    {

                        if (nodeChild.Name == "string" && nodeChild.Attributes["id"] != null)
                        {
                            var idVal = nodeChild.Attributes["id"].Value;


                            // if translation string exist in the project DATA

                            if (idVal == defaultData.stringTranslationID)
                            {
                                // Debug.Log(idVal);
                                TranslationString translationAsset = ScriptableObject.CreateInstance<TranslationString>();

                                translationAsset.moduleID = module;
                                translationAsset.id = "{=" + idVal + "}";
                                translationAsset.stringTranslationID = idVal;
                                translationAsset.lenguageTag = langTag;
                                //translationAsset.stringText = nodeChild.Attributes["text"].Value;
                                translationAsset.editorType = "Imported";
                                translationAsset.lenguage_short_Tag = lenguageFolderName;

                                if (nodeChild.Attributes["text"].Value.Contains("{@Plural}"))
                                {
                                    var text = nodeChild.Attributes["text"].Value;
                                    var ts = nodeChild.Attributes["text"].Value;
                                    var plural_string = text;

                                    // TS TAG - example: plural
                                    Regex regex_tag = new Regex(@"{@Plural}(.*){\\@}");


                                    var v_plu = regex_tag.Match(plural_string);
                                    string p_string = v_plu.Groups[1].ToString();
                                    // Debug.Log(p_string);

                                    plural_string = p_string;

                                    ts = ts.Replace("{@Plural}" + plural_string + "{\\@}", "");

                                    Regex regex = new Regex("{=(.*)}");

                                    var v = regex.Match(ts);
                                    string s = v.Groups[1].ToString();
                                    //soloID = s;

                                    ts = "{=" + s + "}";

                                    text = text.Replace(ts, "");
                                    text = text.Replace("{@Plural}" + plural_string + "{\\@}", "");

                                    translationAsset.stringText = text;
                                    translationAsset.translationPluralString = p_string;
                                }
                                else
                                {
                                    translationAsset.stringText = nodeChild.Attributes["text"].Value;
                                    translationAsset.translationPluralString = "";
                                }

                                string fullPath = Path.GetFullPath(lenguageFile).TrimEnd(Path.DirectorySeparatorChar);
                                string xmlFileName = Path.GetFileName(fullPath);
                                translationAsset.XmlFileName = xmlFileName;

                                modFilesAsset.translationData.translationStrings.Add(translationAsset);


                                string path = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName + "/" + lenguageFolderName + "_{=" + nodeChild.Attributes["id"].Value + "}" + ".asset";

                                if (!Directory.Exists(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName))
                                {
                                    Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName);
                                }

                                // TS_lang_Assets.Add(translationAsset, path);
                                AssetDatabase.CreateAsset(translationAsset, path);
                                EditorUtility.SetDirty(translationAsset);
                                //AssetDatabase.SaveAssets();
                                // Debug.Log(translationAsset);
                                break;
                            }

                        }

                    }

                }

            }

        }

        // Load Another Languages

        string[] lenguageDirectories = Directory.GetDirectories("Assets/Resources/SubModulesData/" + modName + "/Languages");

        foreach (string dir in lenguageDirectories)
        {
            string[] langXMLFiles = Directory.GetFiles(dir, "*.XML");

            foreach (string lenguageFile in langXMLFiles)
            {

                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(lenguageFile);
                Doc.Load(reader);
                reader.Close();


                // Debug.Log(lenguageFile);

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XML = Root.ChildNodes;
                // List<string> languageTags = new List<string>();

                // Debug.Log(Root.ChildNodes.Count);
                string lenguageFolderName = Path.GetFileName(dir);
                string langTag = "";

                foreach (XmlNode langNode in Root.ChildNodes)
                {
                    // Debug.Log(langNode.Name);
                    // string path;

                    if (langNode.Name == "tags")
                    {
                        langTag = langNode.FirstChild.Attributes["language"].Value;
                    }

                    if (langNode.Name == "strings" && langTag != "")
                    {
                        foreach (XmlNode nodeChild in langNode.ChildNodes)
                        {

                            if (nodeChild.Name == "string" && nodeChild.Attributes["id"] != null)
                            {
                                var idVal = nodeChild.Attributes["id"].Value;

                                // var soloID = idVal;
                                // idVal = "{=" + idVal + "}";

                                // if translation string exist in the project DATA

                                if (idVal == defaultData.stringTranslationID)
                                {
                                    //Debug.Log(idVal);
                                    TranslationString translationAsset = CreateInstance<TranslationString>();

                                    translationAsset.moduleID = module;
                                    translationAsset.id = "{=" + idVal + "}";
                                    translationAsset.stringTranslationID = idVal;
                                    translationAsset.lenguageTag = langTag;
                                    //translationAsset.stringText = nodeChild.Attributes["text"].Value;
                                    translationAsset.editorType = "Imported";
                                    translationAsset.lenguage_short_Tag = lenguageFolderName;


                                    if (nodeChild.Attributes["text"].Value.Contains("{@Plural}"))
                                    {
                                        var text = nodeChild.Attributes["text"].Value;
                                        var ts = nodeChild.Attributes["text"].Value;
                                        var plural_string = text;

                                        // TS TAG - example: plural
                                        Regex regex_tag = new Regex(@"{@Plural}(.*){\\@}");


                                        var v_plu = regex_tag.Match(plural_string);
                                        string p_string = v_plu.Groups[1].ToString();
                                        // Debug.Log(p_string);

                                        plural_string = p_string;

                                        ts = ts.Replace("{@Plural}" + plural_string + "{\\@}", "");

                                        Regex regex = new Regex("{=(.*)}");

                                        var v = regex.Match(ts);
                                        string s = v.Groups[1].ToString();
                                        //soloID = s;

                                        ts = "{=" + s + "}";

                                        text = text.Replace(ts, "");
                                        text = text.Replace("{@Plural}" + plural_string + "{\\@}", "");

                                        translationAsset.stringText = text;
                                        translationAsset.translationPluralString = p_string;
                                    }
                                    else
                                    {
                                        translationAsset.stringText = nodeChild.Attributes["text"].Value;
                                        translationAsset.translationPluralString = "";
                                    }

                                    string fullPath = Path.GetFullPath(lenguageFile).TrimEnd(Path.DirectorySeparatorChar);
                                    string xmlFileName = Path.GetFileName(fullPath);
                                    translationAsset.XmlFileName = xmlFileName;

                                    modFilesAsset.translationData.translationStrings.Add(translationAsset);


                                    string path = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName + "/" + lenguageFolderName + "_{=" + nodeChild.Attributes["id"].Value + "}" + ".asset";

                                    if (!Directory.Exists(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName))
                                    {
                                        Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName);
                                    }

                                    // TS_lang_Assets.Add(translationAsset, path);
                                    AssetDatabase.CreateAsset(translationAsset, path);
                                    EditorUtility.SetDirty(translationAsset);
                                    //AssetDatabase.SaveAssets();
                                    //Debug.Log(path);
                                    break;
                                }

                            }


                        }

                    }

                }

            }
        }

        SearchStrings();
    }

    public void CreateTSData(ref ModFiles modFilesAsset, string langName, string langFolderName)
    {

        TranslationString translationAsset = ScriptableObject.CreateInstance<TranslationString>();

        translationAsset.moduleID = modFilesAsset.mod.id;
        translationAsset.id = defaultData.id;
        translationAsset.lenguageTag = langName;
        translationAsset.lenguage_short_Tag = langFolderName;
        translationAsset.langImported = true;
        translationAsset.stringTranslationID = GetTSID(defaultData.id);
        translationAsset.stringText = defaultData.stringText;
        translationAsset.editorType = "Created";

        translationAsset.TSObjectType = GetObjectIDName();

        // string transPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + "/" + translationAsset.id + ".asset";
        string transPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + langFolderName + "/" + langFolderName + "_" + translationAsset.id + ".asset";
        EditorUtility.SetDirty(translationAsset);
        AssetDatabase.CreateAsset(translationAsset, transPath);
        //AssetDatabase.SaveAssets();

        TranslationString ts_loaded = (TranslationString)AssetDatabase.LoadAssetAtPath(transPath, typeof(TranslationString));

        modFilesAsset.translationData.translationStrings.Add(ts_loaded);
        currentTranslationData = ts_loaded;

        SearchStrings();
    }

    private string GetTSID(string inputString)
    {
        var TS_Name = inputString;
        Regex regex = new Regex("{=(.*)}");

        var v = regex.Match(TS_Name);
        string s = v.Groups[1].ToString();
        TS_Name = s;

        return s;

    }
    private string GetObjectIDName()
    {
        if (objectID == 0)
        {
            return "Culture";
        }
        else if (objectID == -1)
        {
            return "Culture";
        }
        else if (objectID == -2)
        {
            return "Culture";
        }
        else if (objectID == -3)
        {
            return "Culture";
        }
        else if (objectID == -4)
        {
            return "Culture";
        }
        else if (objectID == 1)
        {
            return "Kingdom";
        }
        else if (objectID == 11)
        {
            return "Kingdom";
        }
        else if (objectID == 12)
        {
            return "Kingdom";
        }
        else if (objectID == 13)
        {
            return "Kingdom";
        }
        else if (objectID == 14)
        {
            return "Kingdom";
        }
        else if (objectID == 2)
        {
            return "Faction";
        }
        else if (objectID == 21)
        {
            return "Faction";
        }
        else if (objectID == 22)
        {
            return "Faction";
        }
        else if (objectID == 3)
        {
            return "NPC";
        }
        else if (objectID == 4)
        {
            return "Hero";
        }
        else if (objectID == 5)
        {
            return "Settlement";
        }
        else if (objectID == 51)
        {
            return "Settlement";
        }
        else if (objectID == 52)
        {
            return "Settlement";
        }
        else if (objectID == 7)
        {
            return "Item";

        }

        return "";

    }

    // UI DRAW TOOLS
    public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10, int lenght = 4)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
        r.height = thickness;
        r.x += padding;
        r.y -= 2;
        r.height += 512 + lenght;
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
