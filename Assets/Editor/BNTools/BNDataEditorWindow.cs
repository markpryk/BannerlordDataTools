using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using XNode;
using System.Data;
using System.Windows;



// [CustomEditor(typeof(BNDataEditorWindow))]
[CanEditMultipleObjects]
[System.Serializable]
public class BNDataEditorWindow : EditorWindow
{
    bool DebugOptions;
    bool groupEnabled;
    [SerializeField]
    public List<ModuleReceiver> modsList;

    [SerializeField]
    public ModuleReceiver currentMod;
    ModFiles currentModFiles;
    public UnityEngine.Object source;
    string[] _modChoices;
    // string path = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/";
    string path = "Assets/Modules/";
    string configPath = "Assets/Settings/BDT_settings.asset";
    // string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";

    BDTSettings settingsAsset;

    [MenuItem("BNDataTools/BNDataEditor")]


    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(BNDataEditorWindow), false, "Bannerlord Data Tools", true);
    }
    public void OnEnable()
    {

        if (settingsAsset == null)
        {
            if (System.IO.File.Exists(configPath))
            {
                settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));

                string[] ModulesData = Directory.GetFiles(modsSettingsPath, "*.asset");

                //foreach (var mod in ModulesData)
                //{
                //    ModuleReceiver loadMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));
                //    //LoadModProjectData(loadMod);
                //}

          

                EditorUtility.SetDirty(settingsAsset);
            }
            else
            {
                Debug.Log("BDT settings dont exist");
            }


        }

        FullDataBaseRefresh();

        if (currentMod != null)
            EditorUtility.SetDirty(currentMod);
    }

    void OnGUI()
    {
        if (settingsAsset == null)
        {
            if (System.IO.File.Exists(configPath))
            {
                settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));

                string[] ModulesData = Directory.GetFiles(modsSettingsPath, "*.asset");

                //foreach (var mod in ModulesData)
                //{
                //    ModuleReceiver loadMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));
                //    LoadModProjectData(loadMod);
                //}
               
                EditorUtility.SetDirty(settingsAsset);
            }

            FullDataBaseRefresh();

            if (currentMod != null)
                EditorUtility.SetDirty(currentMod);

        }

        if (settingsAsset != null && settingsAsset.load_a && settingsAsset.load_b)
        {
            // Load From SETTINGS
            if (currentMod != null && settingsAsset.currentModule != "" && settingsAsset.currentModule != "none")
            {
                string asset = modsSettingsPath + settingsAsset.currentModule + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    ModuleReceiver dpdMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(asset, typeof(ModuleReceiver));
                    source = dpdMod;
                    currentMod = (ModuleReceiver)source;
                }
                else
                {
                    source = null;
                }
            }

            path = settingsAsset.BNModulesPath;


            GUIStyle style = new GUIStyle();
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
            style = EditorStyles.helpBox;
            style.fontSize = 18;

            // GUILayout.Label("BN Data Tools", EditorStyles.helpBox, GUILayout.Width(132), GUILayout.Height(28));

            EditorGUILayout.HelpBox("Data Tools", MessageType.Info);

            // GUILayout.Space(10);
            Color col = new Color(0.5f, 0.5f, 0.5f, 0.05f);
            DrawUILine(col, 1, 12);

            EditorGUILayout.LabelField("Import:", EditorStyles.boldLabel);
            // button Bar
            GUILayout.Space(8);

            if (GUILayout.Button("Import Module"))
            {
                CreateSubModuleData();
                FullDataBaseRefresh();
            }
            // EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawUILine(col, 3, 12);
            GUILayout.Space(10);

            CheckCurrentModuleReciver(style);

            if (currentMod != null)
            {
                settingsAsset.currentModule = currentMod.id;
                CurrentModData();

            }
            else
            {
                settingsAsset.currentModule = "none";
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Data Tools", MessageType.Info);

            // GUILayout.Space(10);
            Color col = new Color(0.5f, 0.5f, 0.5f, 0.05f);
            DrawUILine(col, 1, 12);
            EditorGUILayout.HelpBox(" Setup tool Settings First \n BNData Tools -> BDT Settings", MessageType.Error);
        }

    }

    public void LoadModProjectData(ModuleReceiver modToLoad)
    {

        string path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/" + modToLoad.id + "_Config.asset";
        if (System.IO.File.Exists(path))
        {
            ModFiles data = (ModFiles)AssetDatabase.LoadAssetAtPath(path, typeof(ModFiles));
            modToLoad.modFilesData = data;
            EditorUtility.SetDirty(data);
        }

        if (modToLoad.modFilesData != null)
        {
            // load asset configs
            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "kng_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {

                KingdomsData kngData = (KingdomsData)AssetDatabase.LoadAssetAtPath(path, typeof(KingdomsData));
                modToLoad.modFilesData.kingdomsData = kngData;
                EditorUtility.SetDirty(kngData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/Kingdoms";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.kingdomsData.kingdoms = new List<Kingdom>();
                    string[] kngAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in kngAssetFiles)
                    {
                        Kingdom kng = (Kingdom)AssetDatabase.LoadAssetAtPath(file, typeof(Kingdom));
                        if (kng)
                        {
                            modToLoad.modFilesData.kingdomsData.kingdoms.Add(kng);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "fac_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                FactionsData facData = (FactionsData)AssetDatabase.LoadAssetAtPath(path, typeof(FactionsData));
                modToLoad.modFilesData.factionsData = facData;
                EditorUtility.SetDirty(facData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/Factions";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.factionsData.factions = new List<Faction>();
                    string[] facAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in facAssetFiles)
                    {
                        Faction fac = (Faction)AssetDatabase.LoadAssetAtPath(file, typeof(Faction));
                        if (fac)
                        {
                            modToLoad.modFilesData.factionsData.factions.Add(fac);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "sett_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                SettlementsData settData = (SettlementsData)AssetDatabase.LoadAssetAtPath(path, typeof(SettlementsData));
                modToLoad.modFilesData.settlementsData = settData;
                EditorUtility.SetDirty(settData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/Settlements";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.settlementsData.settlements = new List<Settlement>();
                    string[] facAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in facAssetFiles)
                    {
                        Settlement sett = (Settlement)AssetDatabase.LoadAssetAtPath(file, typeof(Settlement));
                        if (sett)
                        {
                            modToLoad.modFilesData.settlementsData.settlements.Add(sett);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "npc_" + modToLoad.id + ".asset";

            if (System.IO.File.Exists(path))
            {
                NPCCharactersData npcData = (NPCCharactersData)AssetDatabase.LoadAssetAtPath(path, typeof(NPCCharactersData));
                modToLoad.modFilesData.npcChrData = npcData;
                EditorUtility.SetDirty(npcData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/NPC";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.npcChrData.NPCCharacters = new List<NPCCharacter>();
                    string[] npcAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in npcAssetFiles)
                    {
                        NPCCharacter npc = (NPCCharacter)AssetDatabase.LoadAssetAtPath(file, typeof(NPCCharacter));
                        if (npc != null)
                        {
                            modToLoad.modFilesData.npcChrData.NPCCharacters.Add(npc);
                        }
                    }
                }

                // Load Templates
                path = "Assets/Resources/Data/" + modToLoad.id + "/_Templates/NPCtemplates";
                if (AssetDatabase.IsValidFolder(path))
                {
                    //modToLoad.modFilesData.npcChrData.NPCCharacters = new List<NPCCharacter>();
                    string[] npcAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in npcAssetFiles)
                    {
                        NPCCharacter npc = (NPCCharacter)AssetDatabase.LoadAssetAtPath(file, typeof(NPCCharacter));
                        if (npc != null)
                        {
                            modToLoad.modFilesData.npcChrData.NPCCharacters.Add(npc);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "cult_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                CulturesData cultData = (CulturesData)AssetDatabase.LoadAssetAtPath(path, typeof(CulturesData));
                modToLoad.modFilesData.culturesData = cultData;
                EditorUtility.SetDirty(cultData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/Cultures";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.culturesData.cultures = new List<Culture>();
                    string[] cultAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in cultAssetFiles)
                    {
                        Culture cult = (Culture)AssetDatabase.LoadAssetAtPath(file, typeof(Culture));
                        if (cult)
                        {
                            modToLoad.modFilesData.culturesData.cultures.Add(cult);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "PT_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                PartyTemplatesData PTdata = (PartyTemplatesData)AssetDatabase.LoadAssetAtPath(path, typeof(PartyTemplatesData));
                modToLoad.modFilesData.PTdata = PTdata;
                EditorUtility.SetDirty(PTdata);

                path = "Assets/Resources/Data/" + modToLoad.id + "/PartyTemplates";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.PTdata.partyTemplates = new List<PartyTemplate>();
                    string[] cultAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in cultAssetFiles)
                    {
                        PartyTemplate pt = (PartyTemplate)AssetDatabase.LoadAssetAtPath(file, typeof(PartyTemplate));
                        if (pt)
                        {
                            modToLoad.modFilesData.PTdata.partyTemplates.Add(pt);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "hero_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                HeroesData heroData = (HeroesData)AssetDatabase.LoadAssetAtPath(path, typeof(HeroesData));
                modToLoad.modFilesData.heroesData = heroData;
                EditorUtility.SetDirty(heroData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/Heroes";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.heroesData.heroes = new List<Hero>();
                    string[] cultAssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in cultAssetFiles)
                    {
                        Hero hero = (Hero)AssetDatabase.LoadAssetAtPath(file, typeof(Hero));
                        if (hero)
                        {
                            modToLoad.modFilesData.heroesData.heroes.Add(hero);
                        }
                    }
                }
            }

            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "language_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                LanguagesData langData = (LanguagesData)AssetDatabase.LoadAssetAtPath(path, typeof(LanguagesData));
                modToLoad.modFilesData.languagesData = langData;
                EditorUtility.SetDirty(langData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/TranslationData/_Languages";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.languagesData.languages = new List<ModLanguage>();
                    string[] assetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in assetFiles)
                    {
                        ModLanguage lang = (ModLanguage)AssetDatabase.LoadAssetAtPath(file, typeof(ModLanguage));
                        if (lang)
                        {
                            modToLoad.modFilesData.languagesData.languages.Add(lang);
                        }
                    }
                }
            }

            // Load Items
            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "item_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                ItemsData itemData = (ItemsData)AssetDatabase.LoadAssetAtPath(path, typeof(ItemsData));
                modToLoad.modFilesData.itemsData = itemData;
                EditorUtility.SetDirty(itemData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/Items";
                if (AssetDatabase.IsValidFolder(path))
                {
                    modToLoad.modFilesData.itemsData.items = new List<Item>();
                    string[] assetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in assetFiles)
                    {
                        Item item = (Item)AssetDatabase.LoadAssetAtPath(file, typeof(Item));
                        if (item)
                        {
                            modToLoad.modFilesData.itemsData.items.Add(item);
                        }
                    }
                }
            }
            // Load Equipment Sets
            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "equipment_set_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                EquipmentSetData eqp_set_Data = (EquipmentSetData)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSetData));
                modToLoad.modFilesData.equipmentSetData = eqp_set_Data;
                EditorUtility.SetDirty(eqp_set_Data);

                modToLoad.modFilesData.equipmentSetData.equipmentSets = new List<EquipmentSet>();

                // rosters
                path = "Assets/Resources/Data/" + modToLoad.id + "/NPC/Equipment/EquipmentRosters";
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] eqp_roster_AssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in eqp_roster_AssetFiles)
                    {
                        EquipmentSet eqp = (EquipmentSet)AssetDatabase.LoadAssetAtPath(file, typeof(EquipmentSet));
                        if (eqp)
                        {
                            modToLoad.modFilesData.equipmentSetData.equipmentSets.Add(eqp);
                        }
                    }
                }
                // main
                path = "Assets/Resources/Data/" + modToLoad.id + "/NPC/Equipment/EquipmentMain";
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] eqp_roster_AssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in eqp_roster_AssetFiles)
                    {
                        EquipmentSet eqp = (EquipmentSet)AssetDatabase.LoadAssetAtPath(file, typeof(EquipmentSet));
                        if (eqp)
                        {
                            modToLoad.modFilesData.equipmentSetData.equipmentSets.Add(eqp);
                        }
                    }
                }
                // Equip Sets
                path = "Assets/Resources/Data/" + modToLoad.id + "/Sets/Equipments/EqpSets";
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] eqp_roster_AssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in eqp_roster_AssetFiles)
                    {
                        EquipmentSet eqp = (EquipmentSet)AssetDatabase.LoadAssetAtPath(file, typeof(EquipmentSet));
                        if (eqp)
                        {
                            modToLoad.modFilesData.equipmentSetData.equipmentSets.Add(eqp);
                        }
                    }
                }
            }

            // Load Equipments
            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "equipments_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                EquipmentData eqp_Data = (EquipmentData)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentData));
                modToLoad.modFilesData.equipmentsData = eqp_Data;
                EditorUtility.SetDirty(eqp_Data);

                modToLoad.modFilesData.equipmentsData.equipmentSets = new List<Equipment>();

                // rosters
                path = "Assets/Resources/Data/" + modToLoad.id + "/Sets/Equipments";
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] eqp_roster_AssetFiles = Directory.GetFiles(path, "*.asset");
                    foreach (string file in eqp_roster_AssetFiles)
                    {
                        Equipment eqp = (Equipment)AssetDatabase.LoadAssetAtPath(file, typeof(Equipment));
                        if (eqp)
                        {
                            modToLoad.modFilesData.equipmentsData.equipmentSets.Add(eqp);
                        }
                    }
                }
            }

            // Lenguages
            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "translation_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                TranslationData transData = (TranslationData)AssetDatabase.LoadAssetAtPath(path, typeof(TranslationData));
                modToLoad.modFilesData.translationData = transData;
                EditorUtility.SetDirty(transData);

                path = "Assets/Resources/Data/" + modToLoad.id + "/TranslationData";

                string[] directories = Directory.GetDirectories("Assets/Resources/Data/" + modToLoad.id + "/TranslationData");
                modToLoad.modFilesData.translationData.translationStrings = new List<TranslationString>();

                foreach (var dir in directories)
                {
                    string lenguageFolderName = Path.GetFileName(dir);

                    if (lenguageFolderName == "_Languages")
                    {

                        string[] langDirs = Directory.GetDirectories(dir);

                        foreach (var Ldirs in langDirs)
                        {
                            if (AssetDatabase.IsValidFolder(Ldirs))
                            {
                                string[] assetFiles = Directory.GetFiles(Ldirs, "*.asset");
                                foreach (string file in assetFiles)
                                {
                                    TranslationString trans = (TranslationString)AssetDatabase.LoadAssetAtPath(file, typeof(TranslationString));
                                    if (trans)
                                    {
                                        modToLoad.modFilesData.translationData.translationStrings.Add(trans);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        if (AssetDatabase.IsValidFolder(dir))
                        {
                            string[] assetFiles = Directory.GetFiles(dir, "*.asset");
                            foreach (string file in assetFiles)
                            {
                                TranslationString trans = (TranslationString)AssetDatabase.LoadAssetAtPath(file, typeof(TranslationString));
                                if (trans)
                                {
                                    modToLoad.modFilesData.translationData.translationStrings.Add(trans);
                                }
                            }
                        }
                    }

                }
            }

            // Export Settings
            path = "Assets/Resources/SubModulesData/" + modToLoad.id + "/ModDataConfigs/" + "export_" + modToLoad.id + ".asset";
            if (System.IO.File.Exists(path))
            {
                ExportSettings exportSett = (ExportSettings)AssetDatabase.LoadAssetAtPath(path, typeof(ExportSettings));
                modToLoad.modFilesData.exportSettings = exportSett;
            }

        }
    }

    void CurrentModData()
    {
        if (currentMod.modFilesData != null)
        {
            string kingdName;
            string factName;
            string settName;
            string npcName;
            string cultName;
            string PTname;
            string heroName;
            string itemName;
            string equipmentName;

            if (currentMod.modFilesData.kingdomsData != null)
            {
                kingdName = currentMod.modFilesData.kingdomsData.kingdoms.Count.ToString() + " - Kingdoms";
            }
            else
            {
                kingdName = " - Kingdoms";
            }

            if (currentMod.modFilesData.factionsData != null)
            {
                factName = currentMod.modFilesData.factionsData.factions.Count.ToString() + " - Factions";
            }
            else
            {
                factName = " - Factions";
            }

            if (currentMod.modFilesData.settlementsData != null)
            {
                settName = currentMod.modFilesData.settlementsData.settlements.Count.ToString() + " - Settlements";
            }
            else
            {
                settName = " - Settlements";
            }

            if (currentMod.modFilesData.npcChrData != null)
            {
                npcName = currentMod.modFilesData.npcChrData.NPCCharacters.Count.ToString() + " - NPC";
            }
            else
            {
                npcName = " - NPC";
            }

            if (currentMod.modFilesData.culturesData != null)
            {
                cultName = currentMod.modFilesData.culturesData.cultures.Count.ToString() + " - Cultures";
            }
            else
            {
                cultName = " - Cultures";
            }

            if (currentMod.modFilesData.PTdata != null)
            {
                PTname = currentMod.modFilesData.PTdata.partyTemplates.Count.ToString() + " - Party Templates";
            }
            else
            {
                PTname = " - Party Templates";
            }

            if (currentMod.modFilesData.heroesData != null)
            {
                heroName = currentMod.modFilesData.heroesData.heroes.Count.ToString() + " - Heroes";
            }
            else
            {
                heroName = " - Heroes";
            }

            if (currentMod.modFilesData.itemsData != null)
            {
                itemName = currentMod.modFilesData.itemsData.items.Count.ToString() + " - Items";
            }
            else
            {
                itemName = " - Items";
            }

            if (currentMod.modFilesData.itemsData != null)
            {
                equipmentName = currentMod.modFilesData.equipmentsData.equipmentSets.Count.ToString() + " - Equipments";
            }
            else
            {
                equipmentName = " - Equipemnts";
            }



            GUILayout.Space(-6);
            Color col = new Color(0.5f, 0.5f, 0.5f, 0.1f);
            DrawUILine(col, 1, 12);

            col = new Color(0.5f, 0.5f, 0.5f, 0.05f);

            EditorGUILayout.LabelField("Edit Project Data:", EditorStyles.boldLabel);
            // button Bar
            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(cultName, GUILayout.Width(128), GUILayout.Height(32)))
            {
                CulturesEditor cultEditor = (CulturesEditor)EditorWindow.GetWindow(typeof(CulturesEditor), false, "Cultures Editor", true);
                cultEditor.loadedMod = currentMod;
                cultEditor.CheckAndResort();
            }

            if (GUILayout.Button(kingdName, GUILayout.Width(128), GUILayout.Height(32)))
            {
                KingdomsEditor kingdomesEditor = (KingdomsEditor)EditorWindow.GetWindow(typeof(KingdomsEditor), false, "Kingdoms Editor", true);
                kingdomesEditor.loadedMod = currentMod;
                kingdomesEditor.CheckAndResort();
            }


            if (GUILayout.Button(factName, GUILayout.Width(128), GUILayout.Height(32)))
            {

                FactionEditor factionsEditor = (FactionEditor)EditorWindow.GetWindow(typeof(FactionEditor), false, "Factions Editor", true);
                factionsEditor.loadedMod = currentMod;
                factionsEditor.CheckAndResort();
                // factionsEditor.ShowWindow();

            }

            if (GUILayout.Button(settName, GUILayout.Width(128), GUILayout.Height(32)))
            {
                SettlementsEditor settlementsEditor = (SettlementsEditor)EditorWindow.GetWindow(typeof(SettlementsEditor), false, "Settlements Editor", true);
                settlementsEditor.loadedMod = currentMod;
                settlementsEditor.CheckAndResort();
            }

            EditorGUILayout.EndHorizontal();
            DrawUILine(col, 1, 6);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(npcName, GUILayout.Width(128), GUILayout.Height(32)))
            {
                NPCEditor npcEditor = (NPCEditor)EditorWindow.GetWindow(typeof(NPCEditor), false, "Characters Editor", true);
                npcEditor.loadedMod = currentMod;
                npcEditor.CheckAndResort();
            }


            if (GUILayout.Button(itemName, GUILayout.Width(128), GUILayout.Height(32)))
            {
                ItemsEditor item_editor = (ItemsEditor)EditorWindow.GetWindow(typeof(ItemsEditor), false, "Items Editor", true);
                item_editor.loadedMod = currentMod;
                item_editor.CheckAndResort();
            }

            if (GUILayout.Button(equipmentName, GUILayout.Width(128), GUILayout.Height(32)))
            {
                EquipmentsEditor eqp_editor = (EquipmentsEditor)EditorWindow.GetWindow(typeof(EquipmentsEditor), false, "Equipemnts Editor", true);
                eqp_editor.loadedMod = currentMod;
                eqp_editor.CheckAndResort();
            }

            if (GUILayout.Button(PTname, GUILayout.Width(128), GUILayout.Height(32)))
            {
                PartyTemplatesEditor pt_editor = (PartyTemplatesEditor)EditorWindow.GetWindow(typeof(PartyTemplatesEditor), false, "Party Templates Editor", true);
                pt_editor.loadedMod = currentMod;
                pt_editor.CheckAndResort();
            }

            // if (GUILayout.Button("Heroes", GUILayout.Width(128)))
            // {

            // }

            EditorGUILayout.EndHorizontal();

            DrawUILine(col, 3, 12);

            // EditorGUILayout.LabelField("Save & Export:", EditorStyles.boldLabel);
            // button Bar
            // GUILayout.Space(8);
            if (GUILayout.Button("Module Settings"))
            {
                ModuleSettingsEditor mse = (ModuleSettingsEditor)EditorWindow.GetWindow(typeof(ModuleSettingsEditor), true, "Module Settings");
            }

            GUILayout.Space(8);
            if (GUILayout.Button("Export Data to Bannerlord Xml"))
            {
                BNDataExporter exporter = (BNDataExporter)EditorWindow.GetWindow(typeof(BNDataExporter), true, "Export Data");
                exporter.module = currentMod.id;
                exporter.exported_Mod = currentMod;
            }

            GUILayout.Space(-12);
            DrawUILine(col, 6, 24);

            //Debug refresh data base
            //if (GUILayout.Button("Refresh DataBase"))
            //{
            //    FullDataBaseRefresh();

            //    Debug.Log("BDT - REFRESH DATA BASE");
            //}

            //DrawUILine(col, 3, 6);

            if (GUILayout.Button("Delete Module"))
            {
                if (EditorUtility.DisplayDialog($"Deleting {currentMod.id} mod?",
               "Are you sure you want to delete " + currentMod.id
               + " mod and all data in BDT tools?", "Yes remove", "Do Not Remove"))
                {
                    string dataPath = $"Assets/Resources/Data/{currentMod.id}/";
                    string sub_dataPath = $"Assets/Resources/SubModulesData/{currentMod.id}/";
                    string mod_asset = $"Assets/Resources/SubModulesData/{currentMod.id}.asset";

                    FileUtil.DeleteFileOrDirectory(dataPath);
                    //Directory.Delete(dataPath);
                    FileUtil.DeleteFileOrDirectory(sub_dataPath);
                    //Directory.Delete(sub_dataPath);
                    File.Delete(mod_asset);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

            }
            DrawUILine(col, 2, 4);

            /// DEBUG
            //DrawDebugOptions();

        }
    }

    private void FullDataBaseRefresh()
    {
        var path = "Assets/Resources/SubModulesData";
        if (AssetDatabase.IsValidFolder(path))
        {
            //modToLoad.modFilesData.npcChrData.NPCCharacters = new List<NPCCharacter>();
            string[] mod_assets = Directory.GetFiles(path, "*.asset");
            foreach (string mod_name in mod_assets)
            {
                ModuleReceiver mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod_name, typeof(ModuleReceiver));

                if (mod != null)
                {
                    //Debug.Log(currentMod.id);
                    EditorUtility.SetDirty(mod);
                    LoadModProjectData(mod);
                    //  AssetDatabase.Refresh();
                }
            }
        }
    }

    private void DrawDebugOptions()
    {

        // EditorGUILayout.LabelField("DEBUG OPTIONS:", EditorStyles.boldLabel);
        DebugOptions = EditorGUILayout.Toggle("DEBUG OPTIONS:", DebugOptions);
        GUILayout.Space(8);
        if (DebugOptions)
        {
            // if (GUILayout.Button("Import Item Definitions"))
            // {
            //     var itemTypes = new List<string>();
            //     var crafTemplates = new List<string>();
            //     // Search Items
            //     string[] directories = Directory.GetDirectories(settingsAsset.BNModulesPath + "SandBoxCore/ModuleData");

            //     foreach (var dir in directories)
            //     {
            //         string[] XMLfiles = Directory.GetFiles(dir, "*.XML");
            //         // Read module
            //         foreach (string file in XMLfiles)
            //         {
            //             // Debug.Log(file);
            //             XmlDocument Doc = new XmlDocument();
            //             // UTF 8 - 16
            //             StreamReader reader = new StreamReader(file);
            //             Doc.Load(reader);
            //             reader.Close();

            //             XmlElement Root = Doc.DocumentElement;
            //             XmlNodeList XNL = Root.ChildNodes;
            //             foreach (XmlNode node in Root.ChildNodes)
            //             {
            //                 if (node.Name != "#comment" && Root.Name != "base")
            //                 {
            //                     if (node.LocalName == "Item")
            //                     {
            //                         if (node.Attributes["Type"] != null)
            //                         {
            //                             if (!itemTypes.Contains(node.Attributes["Type"].Value))
            //                             {
            //                                 itemTypes.Add(node.Attributes["Type"].Value);
            //                             }
            //                         }
            //                     }
            //                     else if (node.LocalName == "CraftedItem")
            //                     {
            //                         if (!crafTemplates.Contains(node.Attributes["crafting_template"].Value))
            //                         {
            //                             crafTemplates.Add(node.Attributes["crafting_template"].Value);
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }

            //     settingsAsset.ItemTypesDefinitions = new string[itemTypes.Count];
            //     settingsAsset.CraftingTemplatesDefinitions = new string[crafTemplates.Count];

            //     int i = 0;
            //     foreach (var type in itemTypes)
            //     {
            //         settingsAsset.ItemTypesDefinitions[i] = type;
            //         i++;
            //     }

            //     i = 0;
            //     foreach (var template in crafTemplates)
            //     {
            //         settingsAsset.CraftingTemplatesDefinitions[i] = template;
            //         i++;
            //     }

            //     settingsAsset.ItemTypesDefinitions[4] = "TwoHandedWeapon";
            // }

            if (GUILayout.Button("Load Item Definitions Assets"))
            {


                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";
                string[] itemTypesAssets = Directory.GetFiles("Assets/Settings/Definitions/ItemTypes", "*.asset");

                settingsAsset.ItemTypesDefinitions = new List<ItemType>();

                foreach (var asset in itemTypesAssets)
                {
                    var assetLoaded = (ItemType)AssetDatabase.LoadAssetAtPath(asset, typeof(ItemType));
                    settingsAsset.ItemTypesDefinitions.Add(assetLoaded);

                }


            }
            if (GUILayout.Button("Show Equip Flags"))
            {


                var itemTypes = new List<ItemModifier>();
                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";
                string itemModifiers = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBoxCore/ModuleData/sandboxcore_equipment_sets.xml";

                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(itemModifiers);
                Doc.Load(reader);
                reader.Close();

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XNL = Root.ChildNodes;
                foreach (XmlNode node in Root.ChildNodes)
                {
                    if (node.LocalName == "EquipmentRoster")
                    {
                        foreach (XmlNode SubNode in node.ChildNodes)
                        {
                            if (SubNode.LocalName == "Flags")
                            {
                                foreach (XmlAttribute flag in SubNode.Attributes)
                                {
                                    Debug.Log(flag.Name);
                                }
                            }
                        }
                    }

                }


            }
            if (GUILayout.Button("Import Item Modifiers"))
            {

                var itemTypes = new List<ItemModifier>();
                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";
                string itemModifiers = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/Native/ModuleData/item_modifiers_groups.xml";

                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(itemModifiers);
                Doc.Load(reader);
                reader.Close();

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XNL = Root.ChildNodes;
                foreach (XmlNode node in Root.ChildNodes)
                {

                    if (node.LocalName == "ItemModifierGroup")
                    {
                        if (node.Attributes["id"] != null)
                        {
                            bool contains = false;
                            foreach (var modif in itemTypes)
                            {
                                if (modif.itemModifierGroup_id == node.Attributes["id"].Value)
                                {
                                    contains = true;
                                }
                            }

                            if (contains == false)
                            {
                                ItemModifier modifier = new ItemModifier();

                                modifier.itemModifierGroup_id = node.Attributes["id"].Value;

                                if (node.Attributes["item_type"] != null)
                                {

                                    modifier.itemModifierGroup_item_type = node.Attributes["item_type"].Value;
                                }

                                int M_count = 0;
                                foreach (XmlNode modifiersNode in node.ChildNodes)
                                {
                                    M_count++;
                                }

                                modifier.itemModifiers_id = new string[M_count];

                                int i = 0;
                                foreach (XmlNode modifiersNode in node.ChildNodes)
                                {
                                    modifier.itemModifiers_id[i] = modifiersNode.Attributes["id"].Value;
                                    i++;
                                }

                                itemTypes.Add(modifier);
                            }
                        }
                    }

                }

                foreach (ItemModifier modifierData in itemTypes)
                {
                    path = "Assets/Settings/Definitions/ItemModifierGroups/" + modifierData.itemModifierGroup_id + ".asset";
                    EditorUtility.SetDirty(modifierData);
                    AssetDatabase.CreateAsset(modifierData, path);
                    // AssetDatabase.SaveAssets();
                }


            }
            if (GUILayout.Button("Import Pieces From Native"))
            {
                var craftPieces = new List<CraftingPiece>();
                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";
                string itemModifiers = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/Native/ModuleData/crafting_pieces.xml";

                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(itemModifiers);
                Doc.Load(reader);
                reader.Close();

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XNL = Root.ChildNodes;
                foreach (XmlNode node in Root.ChildNodes)
                {

                    if (node.LocalName == "CraftingPiece")
                    {
                        if (node.Attributes["id"] != null)
                        {
                            bool contains = false;
                            foreach (var piece in craftPieces)
                            {
                                if (piece.ID == node.Attributes["id"].Value)
                                {
                                    contains = true;
                                }
                            }

                            if (contains == false)
                            {
                                CraftingPiece piece = new CraftingPiece();

                                piece.ID = node.Attributes["id"].Value;

                                if (node.Attributes["name"] != null)
                                {

                                    piece.craftName = node.Attributes["name"].Value;
                                }

                                if (node.Attributes["piece_type"] != null)
                                {
                                    piece.piece_type = node.Attributes["piece_type"].Value;
                                }

                                // int M_count = 0;
                                // foreach (XmlNode modifiersNode in node.ChildNodes)
                                // {
                                //     M_count++;
                                // }

                                // modifier.itemModifiers_id = new string[M_count];

                                // int i = 0;
                                // foreach (XmlNode modifiersNode in node.ChildNodes)
                                // {
                                //     modifier.itemModifiers_id[i] = modifiersNode.Attributes["id"].Value;
                                //     i++;
                                // }

                                craftPieces.Add(piece);
                            }
                        }
                    }

                }

                foreach (CraftingPiece pieceData in craftPieces)
                {
                    path = "Assets/Settings/Definitions/CraftingPieces/" + pieceData.ID + ".asset";
                    EditorUtility.SetDirty(pieceData);
                    AssetDatabase.CreateAsset(pieceData, path);
                    AssetDatabase.SaveAssets();
                }
            }
            if (GUILayout.Button("Load Pieces Settings"))
            {
                string[] pieceAssets = Directory.GetFiles("Assets/Settings/Definitions/CraftingPieces", "*.asset");

                settingsAsset.NativePiecesDefinitions = new CraftingPiece[pieceAssets.Length];

                int i = 0;
                foreach (var asset in pieceAssets)
                {
                    CraftingPiece piece = (CraftingPiece)AssetDatabase.LoadAssetAtPath(asset, typeof(CraftingPiece));

                    settingsAsset.NativePiecesDefinitions[i] = piece;
                    i++;
                }

                var definitions = new List<String>();
                foreach (var asset in pieceAssets)
                {
                    CraftingPiece piece = (CraftingPiece)AssetDatabase.LoadAssetAtPath(asset, typeof(CraftingPiece));

                    if (!definitions.Contains(piece.piece_type))
                    {
                        definitions.Add(piece.piece_type);
                    }
                }

                settingsAsset.PieceTypesDefinitions = new string[definitions.Count + 1];

                settingsAsset.PieceTypesDefinitions[0] = "none";
                i = 1;
                foreach (var type in definitions)
                {
                    settingsAsset.PieceTypesDefinitions[i] = type;
                    i++;
                }
            }

            if (GUILayout.Button("Load Skills From NPC"))
            {
                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";

                List<string> listSkills = new List<string>();

                string pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/lords.xml";
                ImportSkillsFromPath(listSkills, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBoxCore/ModuleData/spnpccharacters.xml";
                ImportSkillsFromPath(listSkills, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBoxCore/ModuleData/spnpccharactertemplates.xml";
                ImportSkillsFromPath(listSkills, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/companions.xml";
                ImportSkillsFromPath(listSkills, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/bandits.xml";
                ImportSkillsFromPath(listSkills, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/caravans.xml";
                ImportSkillsFromPath(listSkills, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/StoryMode/ModuleData/storymode_characters.xml";
                ImportSkillsFromPath(listSkills, pathTrait);


                settingsAsset.SkillsDefinitions = new string[listSkills.Count];
                int i = 0;
                foreach (var skill in listSkills)
                {
                    settingsAsset.SkillsDefinitions[i] = skill;
                    i++;
                }
            }
            if (GUILayout.Button("Load Traits From NPC"))
            {
                List<string> listTraits = new List<string>();

                string pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/lords.xml";
                ImportTraitsFromPath(listTraits, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBoxCore/ModuleData/spnpccharacters.xml";
                ImportTraitsFromPath(listTraits, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBoxCore/ModuleData/spnpccharactertemplates.xml";
                ImportTraitsFromPath(listTraits, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/companions.xml";
                ImportTraitsFromPath(listTraits, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/bandits.xml";
                ImportTraitsFromPath(listTraits, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/caravans.xml";
                ImportTraitsFromPath(listTraits, pathTrait);
                pathTrait = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/StoryMode/ModuleData/storymode_characters.xml";
                ImportTraitsFromPath(listTraits, pathTrait);

                settingsAsset.TraitsDefinitions = new string[listTraits.Count];
                int i = 0;
                foreach (var trait in listTraits)
                {
                    settingsAsset.TraitsDefinitions[i] = trait;
                    i++;
                }
            }
            if (GUILayout.Button("Load Equip Slots From NPC"))
            {
                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";
                string lordsNPC = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/lords.xml";

                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(lordsNPC);
                Doc.Load(reader);
                reader.Close();

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XNL = Root.ChildNodes;

                List<string> eqpSlots = new List<string>();

                foreach (XmlNode node in Root.ChildNodes)
                {
                    if (node.LocalName == "NPCCharacter")
                    {
                        foreach (XmlNode nodeSkill in node.ChildNodes)
                        {
                            if (nodeSkill.LocalName == "equipmentSet")
                            {

                                foreach (XmlNode skill in nodeSkill.ChildNodes)
                                {
                                    if (skill.LocalName == "equipment")
                                    {
                                        if (!eqpSlots.Contains(skill.Attributes["slot"].Value))
                                            eqpSlots.Add(skill.Attributes["slot"].Value);

                                        // Debug.Log(skill.Attributes["id"].Value);
                                    }
                                }
                            }
                        }
                    }
                }

                settingsAsset.EquipmentSlotsDefinitions = new string[eqpSlots.Count];
                int i = 0;
                foreach (var slot in eqpSlots)
                {
                    settingsAsset.EquipmentSlotsDefinitions[i] = slot;
                    i++;
                }
            }


            if (GUILayout.Button("Refresh DataBase"))
            {

                // string path = "Assets/Settings/Definitions/ItemTypes" + def + ".asset";
                // string lordsNPC = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData/bandits.xml";
                // string[] npcXmls = Directory.GetFiles("E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/SandBox/ModuleData", "*.xml"); ;

                // foreach (var lordsNPC in npcXmls)
                // {
                //     XmlDocument Doc = new XmlDocument();
                //     // UTF 8 - 16
                //     StreamReader reader = new StreamReader(lordsNPC);
                //     Doc.Load(reader);
                //     reader.Close();

                //     XmlElement Root = Doc.DocumentElement;
                //     XmlNodeList XNL = Root.ChildNodes;

                //     List<string> eqpSlots = new List<string>();

                //     foreach (XmlNode node in Root.ChildNodes)
                //     {
                //         if (node.LocalName == "Kingdom")
                //         {
                //             foreach (XmlNode nodeChild in node.ChildNodes)
                //             {
                //                 if (nodeChild.LocalName == "policies")
                //                     foreach (XmlNode nodePolicie in nodeChild.ChildNodes)
                //                     {
                //                         Debug.Log(nodePolicie.Attributes["id"].Value);

                //                     }
                //             }

                //         }
                //     }

                // }

                //Debug.Log(currentMod.id);
                EditorUtility.SetDirty(currentMod);
                LoadModProjectData(currentMod);
                //  AssetDatabase.Refresh();

                Debug.Log("BDT - REFRESH DATA BASE");

            }
            if (GUILayout.Button("Map Icons to Zero"))
            {
                string pathScene = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/CrusadesCleaner/SceneObj/Main_map/scene.xscene";


                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(pathScene);
                StreamWriter writer = new StreamWriter("E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/Crusades/SceneObj/Main_map/scene.xml");

                Doc.Load(reader);

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XNL = Root.ChildNodes;

                List<string> eqpSlots = new List<string>();

                foreach (XmlNode node in Root.ChildNodes)
                {
                    if (node.LocalName == "entities")
                    {
                        foreach (XmlNode nodeChild in node.ChildNodes)
                        {
                            if (nodeChild.LocalName == "game_entity" && nodeChild.ChildNodes[0].Attributes["position"] != null)
                            {
                                var vec3 = StringToVector3(nodeChild.ChildNodes[0].Attributes["position"].Value);
                                vec3 = new Vector3(vec3.x, vec3.y, 0.0f);

                                var val = vec3.ToString().Replace("(", "");
                                val = val.Replace(")", "");
                                nodeChild.ChildNodes[0].Attributes["position"].Value = val;

                            }
                        }

                    }
                }

                Doc.Save(writer);
                writer.Close();
                reader.Close();


            }

            if (GUILayout.Button("Load Banner Editor Data"))
            {
                var path = "Assets/Settings/BannersEditor/BannerEditor_settings.asset";
                var bannerSettings = (BannerEditorSettings)AssetDatabase.LoadAssetAtPath(path, typeof(BannerEditorSettings));
                EditorUtility.SetDirty(bannerSettings);

                string[] groupDirs = Directory.GetDirectories("Assets/Settings/BannersEditor/Resources/sprites");

                var spritesList = new List<string>();
                var IDList = new List<string>();

                for (int i = 0; i < groupDirs.Length; i++)
                {
                    string[] spritesTex = Directory.GetFiles(groupDirs[i], "*.png");

                    foreach (string tex in spritesTex)
                    {
                        spritesList.Add(tex);

                        var id = Path.GetFileName(tex).Replace("ico_banner_", "");
                        id = id.Replace(".png", "");

                        IDList.Add(id);
                    }
                }

                bannerSettings.sprites = new string[spritesList.Count];
                bannerSettings.sprite_IDs = new string[IDList.Count];

                for (int i = 0; i < spritesList.Count; i++)
                {
                    bannerSettings.sprites[i] = spritesList[i];
                    bannerSettings.sprite_IDs[i] = IDList[i];
                }

            }

            if (GUILayout.Button("Read Banner editor Colors Pattern"))
            {

                var path = "Assets/Settings/BannersEditor/BannerEditor_settings.asset";
                var bannerSettings = (BannerEditorSettings)AssetDatabase.LoadAssetAtPath(path, typeof(BannerEditorSettings));
                EditorUtility.SetDirty(bannerSettings);

                var patternPath = "Assets/Settings/BannersEditor/Resources/colorsPattern.png";


                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(patternPath, typeof(Texture2D));
                // Texture2D texCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);

                // RGBA32 texture format data layout exactly matches Color32 struct
                var data = texture.GetRawTextureData<Color32>();

                // fill texture data with a simple pattern
                Color32 orange = new Color32(255, 165, 0, 255);
                Color32 teal = new Color32(0, 128, 128, 255);
                int index = 0;

                var columns = 6;
                var rows = 28;
                var columnsPixel = 73;
                var rowsPixel = 70;

                Debug.Log(texture.width);
                Debug.Log(texture.height);

                var colors = new Dictionary<int, Color>();
                for (int column = 0; column < columns; column++)
                {
                    rowsPixel = 70;
                    for (int row = 0; row < rows; row++)
                    {
                        var col = texture.GetPixel(rowsPixel, texture.height - columnsPixel);

                        Debug.Log(index);
                        colors.Add(index, col);

                        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(col) + ">████████████</color> = " + col);

                        rowsPixel = rowsPixel + 70;

                        index++;

                        if (index == 158)
                        {
                            break;
                        }
                    }


                    columnsPixel = columnsPixel + 73;

                }

                bannerSettings.colors = new string[colors.Count];
                bannerSettings.color_IDs = new string[colors.Count];

                int i = 0;
                foreach (var color in colors)
                {
                    bannerSettings.colors[i] = "#" + ColorUtility.ToHtmlStringRGB(color.Value);
                    bannerSettings.color_IDs[i] = color.Key.ToString();
                    i++;
                }

            }
        }
      
    }



    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
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

    void CheckCurrentModuleReciver(GUIStyle styleModInfo)
    {


        if (currentMod == null)
        {
            styleModInfo.fontSize = 12;
            EditorGUILayout.LabelField("Module Don't Loaded", "Load Project from list ↓", styleModInfo);
            // source = null;
            source = EditorGUILayout.ObjectField(source, typeof(ModuleReceiver), false);
            GUILayout.Space(5);
            settingsAsset.currentModule = "";
        }
        else
        {

            styleModInfo.fontSize = 12;
            EditorGUILayout.LabelField("Current Module", currentMod.moduleName + " - Version: " + currentMod.version, styleModInfo);

            // EditorGUILayout.ObjectField(currMod, typeof(ModuleReceiver), new GUIContent(" "), GUILayout.Height(20));
            source = EditorGUILayout.ObjectField(source, typeof(ModuleReceiver), false);

            currentMod = (ModuleReceiver)source;
            GUILayout.Space(5);

            if (currentMod != null)
            {
                //if (currentMod.modDependencies != null)
                //{
                //    foreach (var dependency in currentMod.modDependencies)
                //    {
                //        string asset = modsSettingsPath + dependency + ".asset";

                //        if (System.IO.File.Exists(asset))
                //        {
                //            ModuleReceiver dpdMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(asset, typeof(ModuleReceiver));

                //            if (dpdMod.modFilesData == null)
                //            {
                //                LoadModProjectData(dpdMod);
                //            }
                //        }

                //    }
                //}

                FullDataBaseRefresh();

                currentMod = (ModuleReceiver)source;
                settingsAsset.currentModule = currentMod.id;

                if (currentMod.modFilesData == null)
                {
                    //LoadModProjectData(currentMod);
                    FullDataBaseRefresh();
                }
            }

        }

        if (source != null && currentMod == null)
        {
            currentMod = (ModuleReceiver)source;
        }
    }

    void CreateSubModuleData()
    {
        modsList = new List<ModuleReceiver>();

        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] fileInfo = info.GetFiles();

        string[] dir = Directory.GetDirectories(path);

        // foreach (var s in dir)
        // {
        //     Debug.Log(dir[i]);
        //     i++;
        // }
        int i = 0;
        foreach (var directory in dir)
        {

            if (System.IO.File.Exists(dir[i] + "/SubModule.xml"))
            {
                // Debug.Log("Is Module " + dir[i]);
                ModuleReceiver mod = ScriptableObject.CreateInstance<ModuleReceiver>();

                mod.path = dir[i];
                ReadXmlModule(mod);
            }

            i++;

            _modChoices = new string[modsList.Count];

            int choiceIndex = 0;

            foreach (var mod in modsList)
            {
                _modChoices[choiceIndex] = mod.id;
                choiceIndex++;
            }
            // Debug.Log(mods);


            ModListPopup listPopup = (ModListPopup)EditorWindow.GetWindow(typeof(ModListPopup), true, "Import Data");
            listPopup.options = _modChoices;
            listPopup.modList = modsList;
            listPopup.DataWindow = this;
            listPopup.Show();


        }
    }


    public void ReadXmlModule(ModuleReceiver mod)
    {
        XmlDocument Doc = new XmlDocument();
        Doc.Load(mod.path + "/SubModule.xml");

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XML = Root.ChildNodes;
        List<string> depend = new List<string>();

        // Debug.Log(Root.ChildNodes.Count);

        foreach (XmlNode node in Root.ChildNodes)
        {

            if (node.Name == "Name")
            {

                mod.moduleName = node.Attributes["value"].Value;
            }
            if (node.Name == "Id")
            {
                mod.id = node.Attributes["value"].Value;
            }
            if (node.Name == "Version")
            {
                mod.version = node.Attributes["value"].Value;
            }

            if (node.Name == "DependedModules")
            {
                foreach (XmlNode depNode in node.ChildNodes)
                {
                    if (depNode.Attributes != null)
                    {
                        // if (depNode.Attributes["Id"].Value != "Native")
                        // {
                        var IDvalue = depNode.Attributes["Id"].Value;
                        string modPath = path + IDvalue;

                        ModuleReceiver dependMod = ScriptableObject.CreateInstance<ModuleReceiver>();

                        dependMod.path = modPath;

                        depend.Add(IDvalue);

                    }
                    // }

                }


            }

        }
        // add module to list

        modsList.Add(mod);

        mod.modDependencies = new string[depend.Count];

        int i = 0;
        foreach (var dependMod in depend)
        {
            mod.modDependencies[i] = dependMod;
            i++;

        }

    }

    public string[] ModChoices(string[] list)
    {
        return _modChoices;
    }

    public void ImportTraitsFromPath(List<string> listTraits, string path)
    {
        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(path);
        Doc.Load(reader);
        reader.Close();

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;


        foreach (XmlNode node in Root.ChildNodes)
        {
            if (node.LocalName == "NPCCharacter")
            {
                foreach (XmlNode nodeSkill in node.ChildNodes)
                {
                    if (nodeSkill.LocalName == "Traits")
                    {

                        foreach (XmlNode skill in nodeSkill.ChildNodes)
                        {
                            if (skill.LocalName == "Trait")
                            {
                                if (!listTraits.Contains(skill.Attributes["id"].Value))
                                    listTraits.Add(skill.Attributes["id"].Value);

                                // Debug.Log(skill.Attributes["id"].Value);
                            }
                        }
                    }
                }
            }
        }
    }
    public void ImportSkillsFromPath(List<string> listSkills, string path)
    {
        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(path);
        Doc.Load(reader);
        reader.Close();

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;

        foreach (XmlNode node in Root.ChildNodes)
        {
            if (node.LocalName == "NPCCharacter")
            {
                foreach (XmlNode nodeSkill in node.ChildNodes)
                {
                    if (nodeSkill.LocalName == "skills")
                    {

                        foreach (XmlNode skill in nodeSkill.ChildNodes)
                        {
                            if (skill.LocalName == "skill")
                            {
                                if (!listSkills.Contains(skill.Attributes["id"].Value))
                                    listSkills.Add(skill.Attributes["id"].Value);

                                // Debug.Log(skill.Attributes["id"].Value);
                            }
                        }
                    }
                }
            }
        }
    }

}