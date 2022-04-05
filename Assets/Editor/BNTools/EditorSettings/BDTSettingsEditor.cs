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
using System.Threading.Tasks;
 using System.Reflection;

public class BDTSettingsEditor : EditorWindow
{
    /// BN DATA 
    //bool write_debug_data_log = false;
    //bool assign_definitions = true;

    /// <summary>
    /// BD Settings
    /// </summary>
    string path = "Assets/Modules/";
    string configPath = "Assets/Settings/BDT_settings.asset";
    // string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";

    BDTSettings settingsAsset;

    // Temp Lists
    Dictionary<string, List<string>> debug_data_dic;
    List<CraftingPiece> CT_Pieces;
    Dictionary<string, List<string>> CPX_loc;
    Dictionary<string, List<string>> CPX_area;

    // UI
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    GUIStyle headerLabelStyle;
    Color newColLabel;

    [MenuItem("BNDataTools/BDT Settings")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(BDTSettingsEditor), false, "Bannerlord Data Tools - Settings", true);
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
                settingsAsset = new BDTSettings();
                AssetDatabase.CreateAsset(settingsAsset, configPath);
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                Debug.Log("BDT settings created");
            }

            EditorUtility.SetDirty(settingsAsset);
        }

        headerLabelStyle = new GUIStyle(EditorStyles.helpBox);
        ColorUtility.TryParseHtmlString("#fbb034", out newColLabel);

        headerLabelStyle.normal.textColor = newColLabel;
        headerLabelStyle.fontSize = 22;
        headerLabelStyle.fontStyle = FontStyle.Bold;
    }

    void OnGUI()
    {

        DrawUILine(colUILine, 1, 4);

        EditorGUILayout.LabelField("BDT - Settings", headerLabelStyle);
        // GUILayout.Space(2);
        DrawUILine(colUILine, 1, 4);


        if (Directory.Exists(settingsAsset.BNModulesPath + "Native"))
        {
            EditorGUILayout.HelpBox(" Path Validated ✔ ", MessageType.None);

            EditorGUILayout.LabelField("Game Modules Path", EditorStyles.boldLabel);

            settingsAsset.BNModulesPath = EditorGUILayout.TextField(settingsAsset.BNModulesPath);
            settingsAsset.load_a = true;

            DrawUILine(colUILine, 1, 4);

        }
        else
        {
            EditorGUILayout.LabelField("Game Modules Path", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Enter your modules path.\n Example: \n ../Mount & Blade II Bannerlord/Modules/", MessageType.Warning);

            settingsAsset.BNModulesPath = EditorGUILayout.TextField(settingsAsset.BNModulesPath);
            settingsAsset.load_a = false;
        }

        if (settingsAsset.load_a)
        {
            if (!settingsAsset.load_b)
            {
                EditorGUILayout.HelpBox("Import necessary Native data to Work", MessageType.Error);

                if (GUILayout.Button("Import Data"))
                {
                    ImportBannerlordNativeDefinitions(false);
                }
            }

            if (settingsAsset.load_b)
            {
                EditorGUILayout.HelpBox(" Native Data Loaded ✔ ", MessageType.None);
                DrawUILine(colUILine, 1, 4);

                if (GUILayout.Button("Load BDT Layout"))
                {
                    UnityEditor.EditorUtility.LoadWindowLayout("Assets/Settings/BDT_UnityLayout.wlt");
                }
            }

        }

    }

    private void ImportBannerlordNativeDefinitions(bool write_debug_data_log)
    {
        string[] native_modules = new string[] { "Native", "SandBox", "SandBoxCore" };
        string debug_path = "Assets/debug_readed_data.txt";

        EditorUtility.SetDirty(settingsAsset);

        debug_data_dic = new Dictionary<string, List<string>>();
        CT_Pieces = new List<CraftingPiece>();
        ResetDefinitions();

        // Settlement types - and complex templates
        CreateSettlementsDefinitions();

        // Read all Definitions
        for (int mod_index = 0; mod_index < native_modules.Length; mod_index++)
        {
            string module_data_path = settingsAsset.BNModulesPath + native_modules[mod_index] + "/ModuleData/";

            string[] files = Directory.GetFiles(module_data_path);
            string[] dirs = Directory.GetDirectories(module_data_path);

            ReadDirectoryXML(files);

            foreach (var dir in dirs)
            {
                var directory = dir.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string[] dir_files = Directory.GetFiles(directory);
                ReadDirectoryXML(dir_files);

            }
        }
        // write complex - area  / location
        WriteSettlementsComplexData();

        // Lenguage
        // EN - ES
        LoadLenguages(native_modules);

        // Equipment Solts Definitions
        CreateEquipSlotsDefinitions();

        // ItemTypes (internal)
        CreateInternalItemTypes();

        var cp_path = "Assets/Settings/Definitions/CraftingPieces/";

        if (Directory.Exists(cp_path))
        {
            FileUtil.DeleteFileOrDirectory(cp_path);
            Directory.CreateDirectory(cp_path);
        }
        else
        {
            Directory.CreateDirectory(cp_path);
        }

        settingsAsset.NativePiecesDefinitions = new CraftingPiece[CT_Pieces.Count];
        var index = 0;
        foreach (var cp in CT_Pieces)
        {
            var full_cp_path = cp_path + cp.ID + ".asset";
            AssetDatabase.CreateAsset(cp, full_cp_path);
            //AssetDatabase.SaveAssets();
            settingsAsset.NativePiecesDefinitions[index] = cp;
            index++;
        }


        // debug log data
        if (write_debug_data_log)
        {
            StreamWriter writer = new StreamWriter(debug_path, false);

            foreach (var cat in debug_data_dic)
            {
                //Debug.Log(cat.Key);
                writer.WriteLine("Category  -  " + cat.Key + " - " + cat.Value.Count.ToString());
                writer.WriteLine();
                foreach (var attr in cat.Value)
                {
                    //Debug.Log(cat.Key + " ---> " + attr);
                    writer.WriteLine(cat.Key + " ---> " + attr);
                }

                writer.WriteLine("-------------------------------------");


            }

            writer.Close();
        }

        settingsAsset.load_b = true;

        //AssetDatabase.SaveAssetIfDirty(settingsAsset);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public void WriteSettlementsComplexData()
    {
        EditorUtility.SetDirty(settingsAsset.LocationComplexTemplateDefinitions[0]);
        EditorUtility.SetDirty(settingsAsset.LocationComplexTemplateDefinitions[1]);
        EditorUtility.SetDirty(settingsAsset.LocationComplexTemplateDefinitions[2]);
        EditorUtility.SetDirty(settingsAsset.LocationComplexTemplateDefinitions[3]);

        foreach (var val in CPX_loc)
        {
            if (val.Key == "town_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[0].locationsComplexID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[0].locationsComplexID[idx] = lst;
                    idx++;
                }
            }
            else if (val.Key == "castle_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[1].locationsComplexID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[1].locationsComplexID[idx] = lst;
                    idx++;
                }
            }
            else if (val.Key == "village_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[2].locationsComplexID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[2].locationsComplexID[idx] = lst;
                    idx++;
                }
            }
            else if (val.Key == "hideout_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[3].locationsComplexID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[3].locationsComplexID[idx] = lst;
                    idx++;
                }
            }

        }

        foreach (var val in CPX_area)
        {
            if (val.Key == "town_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[0].locationAreasID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[0].locationAreasID[idx] = lst;
                    idx++;
                }

            }
            else if (val.Key == "castle_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[1].locationAreasID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[1].locationAreasID[idx] = lst;
                    idx++;
                }
            }
            else if (val.Key == "village_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[2].locationAreasID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[2].locationAreasID[idx] = lst;
                    idx++;
                }
            }
            else if (val.Key == "hideout_complex")
            {
                settingsAsset.LocationComplexTemplateDefinitions[3].locationAreasID = new string[val.Value.Count];
                int idx = 0;
                foreach (var lst in val.Value)
                {
                    settingsAsset.LocationComplexTemplateDefinitions[3].locationAreasID[idx] = lst;
                    idx++;
                }
            }

        }

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

    }

    public void LoadLenguages(string[] native_modules)
    {
        var langs = new Dictionary<string, ModLanguage>();
        for (int mod_index = 0; mod_index < native_modules.Length; mod_index++)
        {
            string module_data_path = settingsAsset.BNModulesPath + native_modules[mod_index] + "/ModuleData/Languages/";

            if (Directory.Exists(module_data_path))
                LoadLenguageXML(module_data_path, langs);
        }

        var lang_path = "Assets/Settings/Definitions/Languages/";

        if (Directory.Exists(lang_path))
        {
            FileUtil.DeleteFileOrDirectory(lang_path);
            Directory.CreateDirectory(lang_path);
        }
        else
        {
            Directory.CreateDirectory(lang_path);
        }

        settingsAsset.LanguagesDefinitions = new ModLanguage[langs.Count];
        var index = 0;
        foreach (var lang in langs)
        {
            var full_lang_path = lang_path + lang.Key + ".asset";
            AssetDatabase.CreateAsset(lang.Value, full_lang_path);
            settingsAsset.LanguagesDefinitions[index] = lang.Value;
            index++;
        }

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public void LoadLenguageXML(string module_data_path, Dictionary<string, ModLanguage> dic)
    {

        string[] files = Directory.GetFiles(module_data_path);
        string[] dirs = Directory.GetDirectories(module_data_path);

        foreach (var file in files)
        {
            if (file.Contains(".xml"))
            {
                var dir = file.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                LoadLanguageFromFolder(files, "EN", dic, dir);
            }
        }


        foreach (var dir in dirs)
        {
            string[] dir_files = Directory.GetFiles(dir);
            foreach (var file in dir_files)
            {
                if (file.Contains(".xml"))
                {
                    var directory = file.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    var lang_id = new DirectoryInfo(System.IO.Path.GetDirectoryName(directory)).Name;
                    LoadLanguageFromFolder(dir_files, lang_id, dic, directory);
                }
            }

        }
    }

    public void LoadLanguageFromFolder(string[] files, string lang_id, Dictionary<string, ModLanguage> dic, string directory_path)
    {
        if (!dic.ContainsKey(lang_id))
        {
            var lang_data = new ModLanguage();
            lang_data.languageID = lang_id;
            foreach (var file in files)
            {
                if (file.Contains(".xml"))
                {

                    XmlDocument Doc = new XmlDocument();
                    // UTF 8 - 16
                    StreamReader reader = new StreamReader(directory_path);
                    Doc.Load(reader);
                    reader.Close();

                    XmlElement Root = Doc.DocumentElement;
                    XmlNodeList XNL = Root.ChildNodes;

                    if ("base" == Root.LocalName)
                    {
                        if (Root.ChildNodes.Count != 0)
                        {
                            foreach (XmlNode node_chld in Root.ChildNodes)
                            {
                                if (node_chld.LocalName != "#comment" && node_chld.LocalName == "tags")
                                {

                                    if (node_chld.ChildNodes[0].Attributes["language"] != null && node_chld.ChildNodes[0].Attributes["language"].Value != "")
                                    {
                                        var attribute = node_chld.ChildNodes[0].Attributes["language"].Value;
                                        lang_data.languageName = attribute;
                                        dic[lang_id] = lang_data;

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

    }

    void ReadDirectoryXML(string[] files)
    {
        foreach (var file in files)
        {
            if (file.Contains(".xml"))
            {
                var dir = file.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                //Debug.Log(dir);
                ReadBannerlordXML(dir);
            }
        }
    }
    void ReadBannerlordXML(string read_path)
    {

        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(read_path);
        Doc.Load(reader);
        reader.Close();

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;

        // TODO
        /// BodyProperties - SkillSets - Projects - BannerIconData - item_holsters - ItemModifiers - ItemModifierGroups - map_icons
        var data_types = new string[]
        {
            "NPCCharacters",
            "Heroes",
            "LocationComplexTemplates",
            "partyTemplates",
            "EquipmentRosters",
            "Settlements",
            "Factions",
            "Kingdoms",
            "SPCultures",
            "CraftingPieces",
            "CraftingTemplates",
            "Items"
        };

        // All existed root nodes - debug
        //Debug.Log(node.LocalName);

        if (data_types.Contains(Root.LocalName))
        {
            ReadNodeTree(Root);
        }
    }

    void ReadNodeTree(XmlNode input_node)
    {

        if (!debug_data_dic.ContainsKey(input_node.LocalName))
            debug_data_dic.Add(input_node.LocalName, new List<string>());

        foreach (XmlAttribute attr in input_node.Attributes)
        {
            if (!debug_data_dic[input_node.LocalName].Contains(attr.Name))
                debug_data_dic[input_node.LocalName].Add(attr.Name);
        }

        if (input_node.ChildNodes.Count != 0)
        {
            foreach (XmlNode node_chld in input_node.ChildNodes)
            {
                if (node_chld.LocalName != "#comment")
                {
                    ReadBannerlordDefinitions(input_node, node_chld);
                    ReadNodeTree(node_chld);
                }
            }
        }


    }


    public void ReadBannerlordDefinitions(XmlNode mainNode, XmlNode childNode)
    {

        /// DEFINITIONS
        // Definition Type
        // Example
        ///

        // read Crafting Pieces Data
        ReadCraftingPieces(mainNode, childNode, "CraftingPieces", "CraftingPiece");

        // read location complexes
        ReadLocationComplexes(mainNode, childNode, "LocationComplexTemplate", "Location");

        // read location complexes
        ReadLocationComplexes(mainNode, childNode, "Locations", "Location");

        // read location Areas
        ReadLocationAreas(mainNode, childNode, "Settlement", "Components");

        // Policies
        // policy_senate
        ReadAttributeValue(mainNode, childNode, "default_policies", "policy", "id", ref settingsAsset.PoliciesDefinitions, "", "");

        // Village Type
        // none - sheep_farm
        ReadAttributeValue(mainNode, childNode, "Components", "Village", "village_type", ref settingsAsset.VillagesTypeDefinitions, "VillageType.", "none");

        // NPC Voice 
        // none - softspoken
        ReadAttributeValue(mainNode, childNode, "NPCCharacters", "NPCCharacter", "voice", ref settingsAsset.NPCVoiceDefinitions, "", "none");

        // NPC Occupations
        // none - Judge
        ReadAttributeValue(mainNode, childNode, "NPCCharacters", "NPCCharacter", "occupation", ref settingsAsset.NPCOccupationDefinitions, "", "none");

        // NPC Default Groups
        // none - Infantry
        ReadAttributeValue(mainNode, childNode, "NPCCharacters", "NPCCharacter", "default_group", ref settingsAsset.NPCDefaultGroupsDefinitions, "", "none");

        // Formation Position Preference
        // none - Back - Front
        ReadAttributeValue(mainNode, childNode, "NPCCharacters", "NPCCharacter", "formation_position_preference", ref settingsAsset.FormationPosPrefDefinitions, "", "none");

        // Skills
        // Engineering
        ReadAttributeValue(mainNode, childNode, "skills", "skill", "id", ref settingsAsset.SkillsDefinitions, "", "");

        // Traits
        // Valor
        ReadAttributeValue(mainNode, childNode, "Traits", "Trait", "id", ref settingsAsset.TraitsDefinitions, "", "");

        // Weapon Class
        // none - LargeShield - TwoHandedPolearm
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "weapon_class", ref settingsAsset.WeaponClassDefinitions, "", "none");

        // SubTypes
        // none - two_handed_wpn
        ReadAttributeValue(mainNode, childNode, "Items", "Item", "subtype", ref settingsAsset.SubTypesDefinitions, "", "none");

        // Item Category
        // none - grape - fish
        ReadAttributeValue(mainNode, childNode, "Items", "Item", "item_category", ref settingsAsset.ItemCategoryDefinitions, "", "none");

        // Modifier Group
        // none - cloth - cloth_unarmored
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "modifier_group", ref settingsAsset.ModifierGroupDefinitions, "", "none");

        // Material Type
        // none - Cloth - Plate
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "material_type", ref settingsAsset.MaterialTypesDefinitions, "", "none");

        // Hair Beard Cover Types
        // none - all - type1
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "hair_cover_type", ref settingsAsset.HairBeardCoverTypesDefinitions, "", "none");
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "beard_cover_type", ref settingsAsset.HairBeardCoverTypesDefinitions, "", "none");

        // Physics Materials
        // none - wood_weapon - burning_ballista
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "physics_material", ref settingsAsset.PhysicsMatsDefinitions, "", "none");

        // Thrust Damage Type
        // none - Blunt - Pierce
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "thrust_damage_type", ref settingsAsset.ThurstDamageTypesDefinitions, "", "none");

        // Swing Damage Type
        // none - Blunt
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "swing_damage_type", ref settingsAsset.SwingDamageTypesDefinitions, "", "none");

        // Item Usage
        // none - torch - polearm_block_swing_thrust
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "item_usage", ref settingsAsset.ItemUsageDefinitions, "", "none");

        // Body Mesh Type
        // none - shoulders 
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "body_mesh_type", ref settingsAsset.BodyMeshTypesDefinitions, "", "none");

        // Monsters
        // none - goose - mule_unmountable
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Horse", "monster", ref settingsAsset.MonstersDefinitions, "Monster.", "none");

        // Horse Family Types
        // none - 1 - 2
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "family_type", ref settingsAsset.HorseFamilyTypes, "", "none");

        // Horse Skeleton Type
        // none - battania_horse
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Horse", "skeleton_scale", ref settingsAsset.HorseSkeletonTypes, "", "none");

        // Mane Cover
        // none - all
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Armor", "mane_cover_type", ref settingsAsset.ManeCoverTypes, "", "none");

        // Ammo Class
        // none - Bolt
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "ammo_class", ref settingsAsset.AmmoClassesDefinitions, "", "none");

        // Crafting Templates 
        // none - TwoHandedMace - Pike
        ReadAttributeValue(mainNode, childNode, "Items", "CraftedItem", "crafting_template", ref settingsAsset.CraftingTemplatesDefinitions, "", "none");

        // Piece Type
        // Blade - Handle
        ReadAttributeValue(mainNode, childNode, "Pieces", "Piece", "Type", ref settingsAsset.PieceTypesDefinitions, "", "");

        // Cultural Feats
        //  
        ReadAttributeValue(mainNode, childNode, "cultural_feats", "feat", "id", ref settingsAsset.CulturalFeatsDefinitions, "", "");

        // Item Modifier
        // 
        ReadAttributeValue(mainNode, childNode, "ItemComponent", "Weapon", "item_modifier_group", ref settingsAsset.ItemModifierGroupDefinitions, "", "none");

        // Hair Tag
        // 
        ReadAttributeValue(mainNode, childNode, "hair_tags", "hair_tag", "name", ref settingsAsset.HairTagDefinitions, "", "");

        // Beard Tag
        // 
        ReadAttributeValue(mainNode, childNode, "beard_tags", "beard_tag", "name", ref settingsAsset.BeardTagDefinitions, "", "");

    }
    public void CreateInternalItemTypes()
    {
        /////////////////////////// !Item Types
        // none - Arrows - Goods
        //ReadAttributeValue(mainNode, childNode, "Items", "Item", "Type", settingsAsset.ItemTypesDefinitions, "", "none");

        var item_types = new string[]
        {
            "none",
            "Banner",
            "OneHandedWeapon",
            "TwoHandedWeapon",
            "Polearm",
            "Shield",
            "Thrown",
            "Arrows",
            "Bow",
            "Crossbow",
            "Bolts",
            "BodyArmor",
            "Cape",
            "HandArmor",
            "HeadArmor",
            "LegArmor",
             "Horse",
            "HorseHarness",
            "Animal",
            "Goods",
        };

        var item_types_definitions = new string[]
        {
            "none",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "weapon",
            "armor",
            "armor",
            "armor",
            "armor",
            "armor",
            "horse",
            "armor",
            "horse",
            "trade",
        };


        var internal_item_types = new List<ItemType>();

        for (int i = 0; i < item_types.Length; i++)
        {
            var type = new ItemType();

            type.itemTypeID = item_types_definitions[i];
            type.itemTypeName = item_types[i];

            internal_item_types.Add(type);
        }

        var it_path = "Assets/Settings/Definitions/ItemTypes/";

        if (Directory.Exists(it_path))
        {
            FileUtil.DeleteFileOrDirectory(it_path);
            Directory.CreateDirectory(it_path);
        }
        else
        {
            Directory.CreateDirectory(it_path);
        }


        settingsAsset.ItemTypesDefinitions = internal_item_types;
        var index = 0;
        foreach (var type in settingsAsset.ItemTypesDefinitions)
        {
            var full_itp_path = it_path + type.itemTypeName + ".asset";
            AssetDatabase.CreateAsset(type, full_itp_path);
            index++;
        }

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public void CreateEquipSlotsDefinitions()
    {
        // Equipment Slots
        // Item0 - Item1 - Item2 - Item3 - Body - Leg - Cape - Horse - HorseHarness - Gloves - Head

        var equip_slots = new string[]
        {
            "Item0",
            "Item1",
            "Item2",
            "Item3",
            "Body",
            "Leg",
            "Cape",
            "Horse",
            "HorseHarness",
            "Gloves",
            "Head",
        };

        settingsAsset.EquipmentSlotsDefinitions = new string[equip_slots.Length];

        for (int i = 0; i < equip_slots.Length; i++)
        {
            settingsAsset.EquipmentSlotsDefinitions[i] = equip_slots[i];
        }

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

    }

    public void CreateSettlementsDefinitions()
    {

        // Settlements Types
        // Town - Village

        // LocationComplexTemplate

        var settlement_types = new string[]
        {
            "Town",
            "Castle",
            "Village",
            "Hideout",
        };

        var complex_types = new string[]
        {
            "town_complex",
            "castle_complex",
            "village_complex",
            "hideout_complex",
        };

        settingsAsset.SettlementsTypeDefinitions = new string[settlement_types.Length];

        for (int i = 0; i < settlement_types.Length; i++)
        {
            settingsAsset.SettlementsTypeDefinitions[i] = settlement_types[i];
        }

        var complex_path = "Assets/Settings/Definitions/LocationComplexTemplate/";

        if (Directory.Exists(complex_path))
        {
            FileUtil.DeleteFileOrDirectory(complex_path);
            Directory.CreateDirectory(complex_path);
        }
        else
        {
            Directory.CreateDirectory(complex_path);
        }

        settingsAsset.LocationComplexTemplateDefinitions = new LocationComplexTemplate[complex_types.Length];
        CPX_loc = new Dictionary<string, List<string>>();
        CPX_area = new Dictionary<string, List<string>>();

        for (int i = 0; i < complex_types.Length; i++)
        {
            var complex = new LocationComplexTemplate();
            EditorUtility.SetDirty(complex);

            complex.complexID = complex_types[i];
            var full_complex_path = complex_path + complex.complexID + ".asset";
            AssetDatabase.CreateAsset(complex, full_complex_path);

            settingsAsset.LocationComplexTemplateDefinitions[i] = complex;

            CPX_loc.Add(complex_types[i], new List<string>());
            CPX_area.Add(complex_types[i], new List<string>());
        }

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public void ReadLocationComplexes(XmlNode mainNode, XmlNode childNode, string main_naming, string child_naming)
    {

        if (mainNode.LocalName == main_naming && childNode.LocalName == child_naming)
        {
            if (mainNode.Attributes["id"] != null && mainNode.Attributes["id"].Value != "")
            {
                var complex_val = mainNode.Attributes["id"].Value;

                if (complex_val.Contains("LocationComplexTemplate."))
                    complex_val = complex_val.Replace("LocationComplexTemplate.", "");
                if (complex_val == "town_complex")
                {
                    if (childNode.Attributes["id"] != null && childNode.Attributes["id"].Value != "")
                    {
                        var loc_id = childNode.Attributes["id"].Value;
                        if (!CPX_loc[complex_val].Contains(loc_id))
                        {
                            CPX_loc[complex_val].Add(loc_id);
                        }
                    }
                }
                else if (complex_val == "castle_complex")
                {
                    if (childNode.Attributes["id"] != null && childNode.Attributes["id"].Value != "")
                    {
                        var loc_id = childNode.Attributes["id"].Value;
                        if (!CPX_loc[complex_val].Contains(loc_id))
                        {
                            CPX_loc[complex_val].Add(loc_id);
                        }
                    }
                }
                else if (complex_val == "village_complex")
                {
                    if (childNode.Attributes["id"] != null && childNode.Attributes["id"].Value != "")
                    {
                        var loc_id = childNode.Attributes["id"].Value;
                        if (!CPX_loc[complex_val].Contains(loc_id))
                        {
                            CPX_loc[complex_val].Add(loc_id);
                        }
                    }
                }
                else if (complex_val == "hideout_complex")
                {
                    if (childNode.Attributes["id"] != null && childNode.Attributes["id"].Value != "")
                    {
                        var loc_id = childNode.Attributes["id"].Value;
                        if (!CPX_loc[complex_val].Contains(loc_id))
                        {
                            CPX_loc[complex_val].Add(loc_id);
                        }
                    }
                }
            }
        }
    }

    public void ReadLocationAreas(XmlNode mainNode, XmlNode childNode, string main_naming, string child_naming)
    {

        if (mainNode.LocalName == main_naming && childNode.LocalName == child_naming)
        {

            if (childNode.ChildNodes[0] != null)
            {
                if (childNode.ChildNodes[0].LocalName == "Town")
                {
                    if (childNode.ChildNodes[0].Attributes["is_castle"] != null && childNode.ChildNodes[0].Attributes["is_castle"].Value == "true")
                    {
                        foreach (XmlNode area_node in mainNode)
                        {
                            if (area_node.LocalName == "CommonAreas")
                            {
                                foreach (XmlNode area in area_node)
                                {
                                    if (area.Attributes["type"] != null && area.Attributes["type"].Value != "")
                                    {
                                        var area_val = area.Attributes["type"].Value;
                                        if (!CPX_area["castle_complex"].Contains(area_val))
                                        {
                                            CPX_area["castle_complex"].Add(area_val);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (XmlNode area_node in mainNode)
                        {
                            if (area_node.LocalName == "CommonAreas")
                            {
                                foreach (XmlNode area in area_node)
                                {
                                    if (area.Attributes["type"] != null && area.Attributes["type"].Value != "")
                                    {
                                        var area_val = area.Attributes["type"].Value;
                                        if (!CPX_area["town_complex"].Contains(area_val))
                                        {
                                            CPX_area["town_complex"].Add(area_val);
                                        }


                                    }
                                }
                            }
                        }

                    }

                }
                else if (childNode.ChildNodes[0].LocalName == "Village")
                {
                    foreach (XmlNode area_node in mainNode)
                    {
                        if (area_node.LocalName == "CommonAreas")
                        {
                            foreach (XmlNode area in area_node)
                            {
                                if (area.Attributes["type"] != null && area.Attributes["type"].Value != "")
                                {
                                    var area_val = area.Attributes["type"].Value;
                                    if (!CPX_area["village_complex"].Contains(area_val))
                                    {
                                        CPX_area["village_complex"].Add(area_val);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (childNode.ChildNodes[0].LocalName == "Hideout")
                {
                    foreach (XmlNode area_node in mainNode)
                    {
                        if (area_node.LocalName == "CommonAreas")
                        {
                            foreach (XmlNode area in area_node)
                            {
                                if (area.Attributes["type"] != null && area.Attributes["type"].Value != "")
                                {
                                    var area_val = area.Attributes["type"].Value;
                                    if (!CPX_area["hideout_complex"].Contains(area_val))
                                    {
                                        CPX_area["hideout_complex"].Add(area_val);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
    public void ReadCraftingPieces(XmlNode mainNode, XmlNode childNode, string main_naming, string child_naming)
    {

        if (mainNode.LocalName == main_naming && childNode.LocalName == child_naming)
        {
            var cp = new CraftingPiece();
            if (childNode.Attributes["id"] != null && childNode.Attributes["id"].Value != "")
            {
                cp.ID = childNode.Attributes["id"].Value;
            }
            if (childNode.Attributes["name"] != null && childNode.Attributes["name"].Value != "")
            {
                cp.craftName = childNode.Attributes["name"].Value;

            }
            if (childNode.Attributes["piece_type"] != null && childNode.Attributes["piece_type"].Value != "")
            {
                cp.piece_type = childNode.Attributes["piece_type"].Value;

            }
            CT_Pieces.Add(cp);
        }
    }

    public void ReadAttributeValue(XmlNode mainNode, XmlNode childNode, string main_naming, string child_naming, string attribute_item, ref string[] definitions_array, string exclude_link, string zeroAttribute)
    {
        if (zeroAttribute != "" && !definitions_array.Contains(zeroAttribute))
        {
            definitions_array = new string[1];
            definitions_array[0] = zeroAttribute;

        }

        if (mainNode.LocalName == main_naming && childNode.LocalName == child_naming)
        {

            if (childNode.Attributes[attribute_item] != null && childNode.Attributes[attribute_item].Value != "")
            {
                var attribute = childNode.Attributes[attribute_item].Value;

                if (exclude_link != "" && attribute.Contains(exclude_link))
                    attribute = attribute.Replace(exclude_link, "");

                string str = CheckFirstCharacter(attribute);

                //Debug.Log(str);

                if (!definitions_array.Contains(attribute) && !definitions_array.Contains(str))
                {
                    var temp = new string[definitions_array.Length + 1];
                    definitions_array.CopyTo(temp, 0);
                    definitions_array = temp;

                    definitions_array[definitions_array.Length - 1] = attribute;
                    //Debug.Log(attribute);
                }
            }
        }
    }

    private static string CheckFirstCharacter(string attribute)
    {
        var upper = attribute.ToCharArray()[0];
        upper = Char.ToUpper(upper);
        var temp_rest = attribute.ToCharArray();
        var rest = new char[temp_rest.Length - 1];
        for (int i_char = 0; i_char < temp_rest.Length; i_char++)
        {
            if (i_char != 0)
                rest[i_char - 1] = temp_rest[i_char];
        }
        var str = upper + new string(rest);
        return str;
    }

    public void ResetDefinitions()
    {
        // Policies
        // policy_senate
        settingsAsset.PoliciesDefinitions = new string[0];

        // Village Type
        // none - sheep_farm
        settingsAsset.VillagesTypeDefinitions = new string[0];

        // NPC Voice 
        // none - softspoken
        settingsAsset.NPCVoiceDefinitions = new string[0];

        // NPC Occupations
        // none - Judge
        settingsAsset.NPCOccupationDefinitions = new string[0];

        // NPC Default Groups
        // none - Infantry
        settingsAsset.NPCDefaultGroupsDefinitions = new string[0];

        // Formation Position Preference
        // none - Back - Front
        settingsAsset.FormationPosPrefDefinitions = new string[0];

        // Skills
        // Engineering
        settingsAsset.SkillsDefinitions = new string[0];

        // Traits
        // Valor
        settingsAsset.TraitsDefinitions = new string[0];

        // Weapon Class
        // none - LargeShield - TwoHandedPolearm
        settingsAsset.WeaponClassDefinitions = new string[0];

        // SubTypes
        // none - two_handed_wpn
        settingsAsset.SubTypesDefinitions = new string[0];

        // Item Category
        // none - grape - fish
        settingsAsset.ItemCategoryDefinitions = new string[0];

        // Modifier Group
        // none - cloth - cloth_unarmored
        settingsAsset.ModifierGroupDefinitions = new string[0];

        // Material Type
        // none - Cloth - Plate
        settingsAsset.MaterialTypesDefinitions = new string[0];

        // Hair Beard Cover Types
        // none - all - type1
        settingsAsset.HairBeardCoverTypesDefinitions = new string[0];

        // Physics Materials
        // none - wood_weapon - burning_ballista
        settingsAsset.PhysicsMatsDefinitions = new string[0];

        // Thrust Damage Type
        // none - Blunt - Pierce
        settingsAsset.ThurstDamageTypesDefinitions = new string[0];

        // Swing Damage Type
        // none - Blunt
        settingsAsset.SwingDamageTypesDefinitions = new string[0];

        // Item Usage
        // none - torch - polearm_block_swing_thrust
        settingsAsset.ItemUsageDefinitions = new string[0];

        // Body Mesh Type
        // none - shoulders 
        settingsAsset.BodyMeshTypesDefinitions = new string[0];

        // Monsters
        // none - goose - mule_unmountable
        settingsAsset.MonstersDefinitions = new string[0];

        // Horse Family Types
        // none - 1 - 2
        settingsAsset.HorseFamilyTypes = new string[0];

        // Horse Skeleton Type
        // none - battania_horse
        settingsAsset.HorseSkeletonTypes = new string[0];

        // Mane Cover
        // none - all
        settingsAsset.ManeCoverTypes = new string[0];

        // Ammo Class
        // none - Bolt
        settingsAsset.AmmoClassesDefinitions = new string[0];

        // Crafting Templates 
        // none - TwoHandedMace - Pike
        settingsAsset.CraftingTemplatesDefinitions = new string[0];

        // Piece Type
        // Blade - Handle
        settingsAsset.PieceTypesDefinitions = new string[0];

        // Cultural Feats
        //  
        settingsAsset.CulturalFeatsDefinitions = new string[0];

        // Item Modifier
        // 
        settingsAsset.ItemModifierGroupDefinitions = new string[0];

        // Hair Tag
        // 
        settingsAsset.HairTagDefinitions = new string[0];

        // Beard Tag
        // 
        settingsAsset.BeardTagDefinitions = new string[0];
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

