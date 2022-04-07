using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Linq; //Needed for XDocument
using System.Text.RegularExpressions;
using System.Linq;



public class BNDataExporter : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string module;
    public ModuleReceiver exported_Mod;
    // public string export_path = "Assets/Output/";
    public string export_path = "";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string configPath = "Assets/Settings/BDT_settings.asset";
    string dataPath = "Assets/Resources/Data/";


    // string dataPath = "Assets/Resources/Data/";

    BDTSettings settingsAsset;


    void Init()
    {
        EditorWindow window = GetWindow(typeof(BNDataExporter));
        window.Show();
    }

    public void OnEnable()
    {

        if (settingsAsset == null)
        {
            if (System.IO.File.Exists(configPath))
            {
                settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));

                string[] ModulesData = Directory.GetFiles(modsSettingsPath, "*.asset");
            }
            else
            {
                Debug.Log("BDT settings dont exist");
            }
        }
    }



    void OnGUI()
    {


        export_path = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/ModuleData/";


        EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);

        exported_Mod.version = EditorGUILayout.TextField("Version", exported_Mod.version);

        DrawUILine(colUILine, 3, 12);

        var originLabelWidth = EditorGUIUtility.labelWidth;

        EditorGUILayout.LabelField("Output XML File Names", EditorStyles.boldLabel);

        ExportSettings expSettings = exported_Mod.modFilesData.exportSettings;

        expSettings.Culture_xml_name = EditorGUILayout.TextField("Cultures", expSettings.Culture_xml_name);
        expSettings.Faction_xml_name = EditorGUILayout.TextField("Factions", expSettings.Faction_xml_name);
        expSettings.Kingdom_xml_name = EditorGUILayout.TextField("Kingdoms", expSettings.Kingdom_xml_name);

        expSettings.Hero_xml_name = EditorGUILayout.TextField("Heroes", expSettings.Hero_xml_name);
        expSettings.NPCCharacter_xml_name = EditorGUILayout.TextField("NPCCharacters", expSettings.NPCCharacter_xml_name);

        expSettings.Item_xml_name = EditorGUILayout.TextField("Items", expSettings.Item_xml_name);
        expSettings.PartyTemplate_xml_name = EditorGUILayout.TextField("Party Templates", expSettings.PartyTemplate_xml_name);
        expSettings.Settlement_xml_name = EditorGUILayout.TextField("Settlements", expSettings.Settlement_xml_name);
        expSettings.EquipmentSet_xml_name = EditorGUILayout.TextField("EquipmentSets", expSettings.EquipmentSet_xml_name);

        DrawUILine(colUILine, 3, 12);

        var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Remove duplicates if exist   "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        expSettings.checkOverrides = EditorGUILayout.Toggle("Remove Duplicates if Exist", expSettings.checkOverrides, GUILayout.Width(256));

        //textDimensions = GUI.skin.label.CalcSize(new GUIContent("Create BackUp  "));
        //EditorGUIUtility.labelWidth = textDimensions.x;

        expSettings.createBackUp = EditorGUILayout.Toggle("Create BackUp", expSettings.createBackUp, GUILayout.Width(256));

        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.LabelField("Main Map .xscene Settings", EditorStyles.boldLabel);

        if (File.Exists($"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/scene.xscene"))
        {

            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Create entities for unexisted settlements   "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            expSettings.createEntities = EditorGUILayout.Toggle("Create entities for inexistent settlements", expSettings.createEntities, GUILayout.Width(256));
            // expSettings.createEntities = EditorGUILayout.Toggle("Remove entities if not exist data", expSettings.createEntities, GUILayout.Width(256));
            expSettings.exportDataToScene = EditorGUILayout.Toggle("Update Settlements positions", expSettings.exportDataToScene, GUILayout.Width(256));


            if (exported_Mod.W_HeightMap_Texture != "")
            {

                expSettings.createHigtMapData = EditorGUILayout.Toggle("Setup Z position acording HeightMap", expSettings.createHigtMapData, GUILayout.Width(256));
            }
            else
            {
                //DrawUILine(colUILine, 3, 12);
                EditorGUILayout.HelpBox("Hightmap not assigned in module settings. You need assign heightmap to export settlements Z position.", MessageType.Warning);
                expSettings.createHigtMapData = false;
            }

            if (!expSettings.createHigtMapData)
            {
                expSettings.settlToZero = EditorGUILayout.Toggle("Reset settlements Z position to Zero", expSettings.settlToZero, GUILayout.Width(256));

            }
            else
            {
                expSettings.settlToZero = false;
            }


            EditorGUIUtility.labelWidth = originLabelWidth;

        }
        else
        {
            EditorGUILayout.HelpBox("/Main_map/scene.xscene not exist in " + exported_Mod.id + " Folder. \n You need main map scene to activate .xscene export settings.", MessageType.Warning);
        }

        DrawUILine(colUILine, 3, 12);

        // Disable Core modules
        if (exported_Mod.id == "Sandbox" || exported_Mod.id == "SandBoxCore")
        {
            EditorGUILayout.HelpBox("You can not export SandBox or SandBoxCore modules. To prevent possible issues with core files.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Export"))
            {
                var created_backup = false;
                if (expSettings.createBackUp)
                    CreateBackUpData(ref created_backup);
                else
                    created_backup = true;

                if (created_backup)
                {
                    if (expSettings.checkOverrides)
                        ReadDirectoryXML();

                    if (expSettings.createEntities)
                        AddInexistentMapData();

                    // if (!Directory.Exists(export_path + exported_Mod.id))
                    // {
                    //     Directory.CreateDirectory(export_path + exported_Mod.id);
                    // }

                    if (exported_Mod.modFilesData.factionsData.factions.Count != 0)
                        WriteFactionAssets();
                    if (exported_Mod.modFilesData.culturesData.cultures.Count != 0)
                        WriteCulturesAssets();
                    if (exported_Mod.modFilesData.heroesData.heroes.Count != 0)
                        WriteHeroAsset();
                    if (exported_Mod.modFilesData.itemsData.items.Count != 0)
                        WriteItemAsset();
                    if (exported_Mod.modFilesData.kingdomsData.kingdoms.Count != 0)
                        WriteKingdomAsset();
                    if (exported_Mod.modFilesData.npcChrData.NPCCharacters.Count != 0)
                        WriteNPCAsset();
                    if (exported_Mod.modFilesData.PTdata.partyTemplates.Count != 0)
                        WritePartyTemplateAsset();
                    if (exported_Mod.modFilesData.settlementsData.settlements.Count != 0)
                        WriteSettlementAsset();
                    if (exported_Mod.modFilesData.equipmentsData.equipmentSets.Count != 0)
                        WriteEquipmentAsset();
                    if (exported_Mod.modFilesData.translationData.translationStrings.Count != 0)
                        WriteTranslationStrings();


                    if (expSettings.exportDataToScene)
                        WriteSettlementsPositions();

                    if (expSettings.settlToZero)
                        ResetSettlementsZ();


                    //write version data to submodule.xml
                    var sub_modules_path = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SubModule.xml";

                    if (File.Exists(sub_modules_path))
                    {
                        XmlDocument xmlDoc = new XmlDocument();

                        xmlDoc.Load(sub_modules_path);

                        foreach (XmlNode node in xmlDoc.ChildNodes[0])
                        {
                            if (node.LocalName == "Version")
                            {
                                node.Attributes["value"].Value = exported_Mod.version;
                                break;
                            }

                        }

                        xmlDoc.Save(sub_modules_path);
                    }
                    else
                    {
                        Debug.LogWarning("SubModule.xml not exist for  " + exported_Mod.id);
                    }

                    //AssetDatabase.Refresh();
                    this.Close();
                }
            }
        }
    }

    private void ResetSettlementsZ()
    {
        string pathScene = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/scene.xscene";

        var tempName = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/TEMP_SCENE_NAME.xscene";

        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(pathScene);
        StreamWriter writer = new StreamWriter(tempName);

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
        File.Replace(tempName, pathScene, "TEMP_SCENE_NAME_BCKP");
    }

    private void WriteSettlementsPositions()
    {

        string pathScene = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/scene.xscene";
        var tempName = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/TEMP_SCENE_NAME.xscene";

        //if(File.Exists(tempName))
        //File.Delete(tempName);

        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(pathScene);
        StreamWriter writer = new StreamWriter(tempName);

        Doc.Load(reader);

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;

        List<string> eqpSlots = new List<string>();

        for (int i = 0; i < exported_Mod.modFilesData.settlementsData.settlements.Count; i++)
        {

            var SList = exported_Mod.modFilesData.settlementsData.settlements;
            foreach (XmlNode node in Root.ChildNodes)
            {
                if (node.LocalName == "entities")
                {
                    foreach (XmlNode nodeChild in node.ChildNodes)
                    {
                        if (nodeChild.Attributes["name"] != null)
                        {

                            if (nodeChild.LocalName == "game_entity" && nodeChild.Attributes["name"].Value == SList[i].id && nodeChild.ChildNodes[0].Attributes["position"] != null)
                            {
                                var vec3 = StringToVector3(nodeChild.ChildNodes[0].Attributes["position"].Value);

                                if (exported_Mod.modFilesData.exportSettings.createHigtMapData)
                                {
                                    float Z = GetHightMapData(vec3).z;
                                    vec3 = new Vector3(float.Parse(SList[i].posX), float.Parse(SList[i].posY), Z);
                                }
                                else
                                    vec3 = new Vector3(float.Parse(SList[i].posX), float.Parse(SList[i].posY), vec3.z);


                                var val = vec3.ToString().Replace("(", "");
                                val = val.Replace(")", "");
                                nodeChild.ChildNodes[0].Attributes["position"].Value = val;

                                // Debug.Log(nodeChild.Attributes["name"].Value);

                            }
                        }
                    }

                }
            }

        }

        Doc.Save(writer);
        writer.Close();
        reader.Close();
        File.Replace(tempName, pathScene, "TEMP_SCENE_NAME_BCKP");
    }



    public Vector3 GetHightMapData(Vector3 inputPos)
    {
        //var pos = new Vector3(0, 0, 0);
        Texture2D heightmap = (Texture2D)AssetDatabase.LoadAssetAtPath(exported_Mod.W_HeightMap_Texture, typeof(Texture2D));

        var world_H = exported_Mod.W_Y_Size * exported_Mod.W_SingleNodeSize;
        var world_X = exported_Mod.W_X_Size * exported_Mod.W_SingleNodeSize;

        var heightRange = 0.0f;
        if (exported_Mod.W_min_Height < 0)
            heightRange = exported_Mod.W_max_Height + exported_Mod.W_min_Height;
        else
            heightRange = exported_Mod.W_max_Height - exported_Mod.W_min_Height;

        Vector3 size = new Vector3(world_X, heightRange, world_H);

        int x = Mathf.FloorToInt(inputPos.x / size.x * heightmap.width);
        int z = Mathf.FloorToInt(inputPos.y / size.z * heightmap.height);
        Vector3 pos = inputPos;
        pos.z = heightmap.GetPixel(x, z).grayscale * size.y;

        return pos;
    }

    void CreateBackUpData(ref bool isCreated)
    {
        //string[] backup_ids = Directory.GetDirectories($"{settingsAsset.BNModulesPath}_BDT_BackUPs/{exported_Mod.id}/");

        int backupID = exported_Mod.BackUp_Count + 1;

        string backUp_path_source = export_path;
        string backUp_path_dest = $"{settingsAsset.BNModulesPath}_BDT_BackUPs/{exported_Mod.id}/BackUp_{exported_Mod.version}_{backupID}/";

        var xScene_source = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/scene.xscene";
        var xScene_dest = backUp_path_dest + "scene.xscene";

        if (Directory.Exists(backUp_path_dest))
        {
            // backUp_path_dest = $"{settingsAsset.BNModulesPath}_BDT_BackUPs/{exported_Mod.id}/BackUp_{exported_Mod.version}_{backupID}_{backupID}/";
            // xScene_dest = backUp_path_dest + "scene.xscene";

            Debug.LogError($" Same backup folder already exist: {backUp_path_dest}, rename it first");
            isCreated = false;
            return;

            //if (Directory.Exists(backUp_path_dest))
            //{

            //    Debug.LogError($" Same backup folder already exist: {backUp_path_dest}, rename it first");
            //    isCreated = false;
            //    return;
            //}
        }

        FileUtil.CopyFileOrDirectory(backUp_path_source, backUp_path_dest);
        FileUtil.CopyFileOrDirectory(xScene_source, xScene_dest);

        isCreated = true;

        EditorUtility.SetDirty(exported_Mod);
        exported_Mod.BackUp_Count = backupID;
    }

    void AddInexistentMapData()
    {
        List<string> existed_comp_list = new List<string>();
        List<XmlNode> added_comp_list = new List<XmlNode>();

        var xScene_source = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/scene.xscene";
        var tempName = $"{settingsAsset.BNModulesPath}{exported_Mod.id}/SceneObj/Main_map/TEMP_SCENE_NAME.xscene";

        var templates = SttlNodeTemplates();

        //if(File.Exists(tempName))
        //File.Delete(tempName);

        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(xScene_source);
        StreamWriter writer = new StreamWriter(tempName);

        Doc.Load(reader);

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;

        foreach (XmlNode node in Root.ChildNodes)
        {
            ReadXsceneComponents(node, existed_comp_list);
        }

        foreach (var settl in exported_Mod.modFilesData.settlementsData.settlements)
        {
            if (settl.id != "" && !existed_comp_list.Contains(settl.id))
            {
                if (settl.isTown)
                {
                    var id = 0;
                    AppendNodeToXscene(id, templates, Root, settl);
                }
                else if (settl.isCastle)
                {
                    var id = 1;
                    AppendNodeToXscene(id, templates, Root, settl);
                }
                if (settl.isVillage)
                {
                    var id = 2;
                    AppendNodeToXscene(id, templates, Root, settl);
                }
                if (settl.isHideout)
                {
                    var id = 3;
                    AppendNodeToXscene(id, templates, Root, settl);
                }
            }
        }

        Doc.Save(writer);
        writer.Close();
        reader.Close();
        File.Replace(tempName, xScene_source, "TEMP_SCENE_NAME_BCKP");
    }

    private void AppendNodeToXscene(int id, XmlNode[] templates, XmlElement Root, Settlement settl)
    {
        var vec3 = StringToVector3(templates[id].ChildNodes[0].Attributes["position"].Value);

        if (exported_Mod.modFilesData.exportSettings.createHigtMapData)
        {
            float Z = GetHightMapData(vec3).z;
            vec3 = new Vector3(float.Parse(settl.posX), float.Parse(settl.posY), Z);
        }
        else
            vec3 = new Vector3(float.Parse(settl.posX), float.Parse(settl.posY), vec3.z);

        var val = vec3.ToString().Replace("(", "");
        val = val.Replace(")", "");
        templates[id].ChildNodes[0].Attributes["position"].Value = val;
        templates[id].Attributes["name"].Value = settl.id;
        XmlNode importNode = Root.OwnerDocument.ImportNode(templates[id], true);

        Root.ChildNodes[3].AppendChild(importNode);

    }

    //private void DebugSettlTemplatesAtributeID(XmlNode input_node)
    //{
    //    if (input_node.ChildNodes.Count != 0)
    //    {
    //        foreach (XmlNode node_chld in input_node.ChildNodes)
    //        {
    //            if (node_chld.LocalName != "#comment")
    //            {
    //                if (input_node.LocalName == "components" && node_chld.LocalName == "meta_mesh_component")
    //                {
    //                    if (node_chld.Attributes["name"] != null && node_chld.Attributes["name"].Value != "")
    //                    {
    //                        Debug.Log(node_chld)
    //                    }
    //                }

    //                DebugSettlTemplatesAtributeID(node_chld);
    //            }
    //        }
    //    }
    //}

    public XmlNode[] SttlNodeTemplates()
    {
        var settlement_Templates = "Assets/Settings/Definitions/MapSettlementsTemplates.xml";

        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(settlement_Templates);
        Doc.Load(reader);
        reader.Close();

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;
        var nodeList = new XmlNode[4];

        foreach (XmlNode node_chld in Root.ChildNodes)
        {
            if (node_chld.LocalName == "Town")
            {
                nodeList[0] = node_chld.FirstChild;
            }
            else if (node_chld.LocalName == "Castle")
            {
                nodeList[1] = node_chld.FirstChild;
            }
            else if (node_chld.LocalName == "Village")
            {
                nodeList[2] = node_chld.FirstChild;
            }
            else if (node_chld.LocalName == "Hideout")
            {
                nodeList[3] = node_chld.FirstChild;
            }
        }

        return nodeList;
    }

    void ReadXsceneComponents(XmlNode input_node, List<string> component_names_list)
    {

        if (input_node.ChildNodes.Count != 0)
        {
            foreach (XmlNode node_chld in input_node.ChildNodes)
            {
                if (node_chld.LocalName != "#comment")
                {
                    if (input_node.LocalName == "entities" && node_chld.LocalName == "game_entity")
                    {
                        if (node_chld.Attributes["name"] != null && node_chld.Attributes["name"].Value != "")
                        {
                            component_names_list.Add(node_chld.Attributes["name"].Value);
                        }
                    }

                    ReadXsceneComponents(node_chld, component_names_list);
                }
            }
        }
    }

    void ReadDirectoryXML()
    {
        string module_data_path = settingsAsset.BNModulesPath + exported_Mod.id + "/ModuleData/";

        string[] files = Directory.GetFiles(module_data_path);

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
        var nodeList = new List<XmlNode>();

        // TODO
        /// BodyProperties - SkillSets - Projects - BannerIconData - item_holsters - ItemModifiers - ItemModifierGroups - map_icons
        var data_types = new string[]
        {
            "NPCCharacters",
            "Heroes",
            "partyTemplates",
            "EquipmentRosters",
            "Settlements",
            "Factions",
            "Kingdoms",
            "SPCultures",
            "Items"
        };

        if (data_types.Contains(Root.LocalName))
        {
            ReadNodeTree(Root, nodeList);
        }

        foreach (var nodes in nodeList)
        {
            Doc.DocumentElement.RemoveChild(nodes);
        }

        Doc.Save(read_path);

    }

    void ReadNodeTree(XmlNode input_node, List<XmlNode> list)
    {

        if (input_node.ChildNodes.Count != 0)
        {
            foreach (XmlNode node_chld in input_node.ChildNodes)
            {
                if (node_chld.LocalName != "#comment")
                {
                    ReadAndOverrideExisted(input_node, node_chld, list);
                    ReadNodeTree(node_chld, list);
                }
            }
        }
    }

    public void ReadAndOverrideExisted(XmlNode mainNode, XmlNode childNode, List<XmlNode> list)
    {
        foreach (var asset in exported_Mod.modFilesData.factionsData.factions)
        {
            OverrideNodeValues(list, mainNode, childNode, "Factions", "Faction", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.npcChrData.NPCCharacters)
        {
            OverrideNodeValues(list, mainNode, childNode, "NPCCharacters", "NPCCharacter", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.heroesData.heroes)
        {
            OverrideNodeValues(list, mainNode, childNode, "Heroes", "Hero", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.PTdata.partyTemplates)
        {
            OverrideNodeValues(list, mainNode, childNode, "partyTemplates", "MBPartyTemplate", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.equipmentsData.equipmentSets)
        {
            OverrideNodeValues(list, mainNode, childNode, "EquipmentRosters", "EquipmentRoster", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.settlementsData.settlements)
        {
            OverrideNodeValues(list, mainNode, childNode, "Settlements", "Settlement", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.kingdomsData.kingdoms)
        {
            OverrideNodeValues(list, mainNode, childNode, "Kingdoms", "Kingdom", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.culturesData.cultures)
        {
            OverrideNodeValues(list, mainNode, childNode, "SPCultures", "Culture", "id", asset.id);
        }
        foreach (var asset in exported_Mod.modFilesData.itemsData.items)
        {
            OverrideNodeValues(list, mainNode, childNode, "Items", "Item", "id", asset.id);
        }
    }

    public void OverrideNodeValues(List<XmlNode> list, XmlNode mainNode, XmlNode childNode, string main_naming, string child_naming, string attribute_item, string attribute_id)
    {

        if (mainNode.LocalName == main_naming && childNode.LocalName == child_naming)
        {

            if (childNode.Attributes[attribute_item] != null && childNode.Attributes[attribute_item].Value != "")
            {
                var value = childNode.Attributes[attribute_item].Value;

                if (value == attribute_id)
                {
                    list.Add(childNode);
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
    void WriteTranslationStrings()
    {

        // var langPath = export_path + exported_Mod.id + "/Languages";
        var langPath = export_path + "Languages";

        if (!Directory.Exists(langPath))
        {
            Directory.CreateDirectory(langPath);
        }

        var ModLanguages = new Dictionary<string, string>();
        foreach (var translation in exported_Mod.modFilesData.translationData.translationStrings)
        {
            if (translation != null && translation.lenguage_short_Tag != null && translation.lenguage_short_Tag != "")
            {
                // Debug.Log(TS.lenguage_short_Tag);
                if (!ModLanguages.ContainsKey(translation.lenguage_short_Tag))
                {

                    ModLanguages.Add(translation.lenguage_short_Tag, translation.lenguageTag);
                }
            }
        }



        foreach (var language in ModLanguages)
        {

            if (language.Key != "")
            {
                List<TranslationString> Culture = new List<TranslationString>();
                List<TranslationString> Faction = new List<TranslationString>();
                List<TranslationString> Hero = new List<TranslationString>();
                List<TranslationString> Kingdom = new List<TranslationString>();
                List<TranslationString> NPCCharacter = new List<TranslationString>();
                List<TranslationString> PartyTemplate = new List<TranslationString>();
                List<TranslationString> Settlement = new List<TranslationString>();
                List<TranslationString> Item = new List<TranslationString>();

                var currLangPath = langPath + "/" + language.Key;

                if (!Directory.Exists(currLangPath))
                {
                    Directory.CreateDirectory(currLangPath);
                }

                foreach (var TS in exported_Mod.modFilesData.translationData.translationStrings)
                {
                    if (TS != null && TS.lenguage_short_Tag == language.Key)
                    {
                        if (TS.TSObjectType == "Culture")
                        {
                            if (!Culture.Contains(TS))
                            {
                                Culture.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "Faction")
                        {
                            if (!Faction.Contains(TS))
                            {
                                Faction.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "Hero")
                        {
                            if (!Hero.Contains(TS))
                            {
                                Hero.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "Kingdom")
                        {
                            if (!Kingdom.Contains(TS))
                            {
                                Kingdom.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "NPCCharacter")
                        {
                            if (!NPCCharacter.Contains(TS))
                            {
                                NPCCharacter.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "PartyTemplate")
                        {
                            if (!PartyTemplate.Contains(TS))
                            {
                                PartyTemplate.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "Settlement")
                        {
                            if (!Settlement.Contains(TS))
                            {
                                Settlement.Add(TS);
                            }
                        }
                        else if (TS.TSObjectType == "Item")
                        {
                            if (!Item.Contains(TS))
                            {
                                Item.Add(TS);
                            }
                        }
                    }
                }

                var translationPrefix = language.Key + "_" + exported_Mod.modFilesData.exportSettings.translationXml_Tag;


                var xmlFileName = exported_Mod.modFilesData.exportSettings.Culture_xml_name;
                var outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, Faction, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.Faction_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, Culture, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.Hero_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, Hero, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.Kingdom_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, Kingdom, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.NPCCharacter_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, NPCCharacter, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.PartyTemplate_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, PartyTemplate, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.Settlement_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, Settlement, outputPath);

                xmlFileName = exported_Mod.modFilesData.exportSettings.Item_xml_name;
                outputPath = currLangPath + "/" + translationPrefix + xmlFileName;
                WriteLanguagesTS(language.Value, Item, outputPath);
            }
        }
        //UnityEditor.AssetDatabase.Refresh();
    }

    private void WriteLanguagesTS(string langID, List<TranslationString> TSList, string exportPath)
    {
        if (TSList.Count != 0)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = false
            };

            // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
            string tFile = exportPath;

            XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
            BNXmlWriter.WriteStartDocument();
            BNXmlWriter.WriteStartElement("base");
            BNXmlWriter.WriteStartElement("tags");

            BNXmlWriter.WriteStartElement("tag");

            CheckAndWriteAttribute(BNXmlWriter, "language", langID);

            BNXmlWriter.WriteEndElement();

            BNXmlWriter.WriteFullEndElement();

            BNXmlWriter.WriteStartElement("strings");

            foreach (TranslationString TS in TSList)
            {
                if (TS != null)
                {
                    // Debug.Log(TS.id);

                    BNXmlWriter.WriteStartElement("string");
                    var text = TS.stringText;
                    if (TS.translationPluralString != "")
                    {
                        text = TS.stringText + "{@Plural}" + TS.translationPluralString + "{\\@}";
                    }

                    CheckAndWriteAttribute(BNXmlWriter, "id", TS.stringTranslationID);
                    CheckAndWriteAttribute(BNXmlWriter, "text", text);

                    BNXmlWriter.WriteEndElement();
                }
            }

            BNXmlWriter.WriteFullEndElement();
            BNXmlWriter.WriteFullEndElement();
            BNXmlWriter.WriteEndDocument();
            BNXmlWriter.Flush();
            BNXmlWriter.Close();

        }
    }

    string GetFullNameToWrite(string WorkString)
    {

        if (WorkString.Contains("{@Plural}"))
        {
            var translationString = WorkString;

            Regex regex = new Regex("{=(.*)}");

            var v = regex.Match(translationString);
            string s = v.Groups[1].ToString();

            translationString = "{=" + s + "}";
            foreach (var ts in exported_Mod.modFilesData.translationData.translationStrings)
            {
                if (ts.id == translationString && ts.translationPluralString != "")
                {
                    var naming = WorkString + "{@Plural}" + ts.translationPluralString + "{\\@}";
                    return naming;
                }
            }
        }

        return WorkString;
    }
    void WriteFactionAssets()
    {

        var path = export_path + exported_Mod.modFilesData.exportSettings.Faction_xml_name;
        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.Faction_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = true
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("Factions");

        foreach (Faction fac in exported_Mod.modFilesData.factionsData.factions)
        {
            BNXmlWriter.WriteStartElement("Faction");

            CheckAndWriteAttribute(BNXmlWriter, "id", fac.id);
            CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(fac.factionName));
            CheckAndWriteAttribute(BNXmlWriter, "is_minor_faction", fac.is_minor_faction);
            CheckAndWriteAttribute(BNXmlWriter, "is_outlaw", fac.is_outlaw);
            CheckAndWriteAttribute(BNXmlWriter, "is_mafia", fac.is_mafia);
            CheckAndWriteAttribute(BNXmlWriter, "is_bandit", fac.is_bandit);
            CheckAndWriteAttribute(BNXmlWriter, "initial_posX", fac.initial_posX);
            CheckAndWriteAttribute(BNXmlWriter, "initial_posY", fac.initial_posY);
            CheckAndWriteAttribute(BNXmlWriter, "owner", fac.owner);
            CheckAndWriteAttribute(BNXmlWriter, "super_faction", fac.super_faction);
            CheckAndWriteAttribute(BNXmlWriter, "banner_key", fac.banner_key);
            CheckAndWriteAttribute(BNXmlWriter, "label_color", fac.label_color);
            CheckAndWriteAttribute(BNXmlWriter, "color", fac.color);
            CheckAndWriteAttribute(BNXmlWriter, "color2", fac.color2);
            CheckAndWriteAttribute(BNXmlWriter, "alternative_color", fac.alternative_color);
            CheckAndWriteAttribute(BNXmlWriter, "alternative_color2", fac.alternative_color2);
            CheckAndWriteAttribute(BNXmlWriter, "culture", fac.culture);
            CheckAndWriteAttribute(BNXmlWriter, "settlement_banner_mesh", fac.settlement_banner_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "default_party_template", fac.default_party_template);
            CheckAndWriteAttribute(BNXmlWriter, "tier", fac.tier);
            CheckAndWriteAttribute(BNXmlWriter, "text", fac.text);

            // update 1.7.2
            CheckAndWriteAttribute(BNXmlWriter, "is_clan_type_mercenary", fac.is_clan_type_mercenary);
            CheckAndWriteAttribute(BNXmlWriter, "short_name", fac.short_name);
            CheckAndWriteAttribute(BNXmlWriter, "is_sect", fac.is_sect);
            CheckAndWriteAttribute(BNXmlWriter, "is_nomad", fac.is_nomad);

            WriteMinorFactiontemplates(BNXmlWriter, "minor_faction_character_templates", fac.minor_faction_character_templates);

            BNXmlWriter.WriteFullEndElement();
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();


    }
    void WriteCulturesAssets()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.Culture_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.Culture_xml_name;

        //if (!File.Exists(path))
        //{
        //    XmlDocument doc = new XmlDocument();

        //    //xml declaration is recommended, but not mandatory
        //    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

        //    //create the root element
        //    XmlElement root = doc.DocumentElement;
        //    doc.InsertBefore(xmlDeclaration, root);

        //    //string.Empty makes cleaner code
        //    XmlElement element1 = doc.CreateElement(string.Empty, "Mainbody", string.Empty);
        //    doc.AppendChild(element1);

        //    doc.Save(path);
        //}

        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("SPCultures");

        foreach (Culture cult in exported_Mod.modFilesData.culturesData.cultures)
        {
            if (cult != null)
            {
                BNXmlWriter.WriteStartElement("Culture");

                CheckAndWriteAttribute(BNXmlWriter, "id", cult.id);
                CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(cult.cultureName));
                CheckAndWriteAttribute(BNXmlWriter, "is_main_culture", cult.is_main_culture);
                CheckAndWriteAttribute(BNXmlWriter, "color", cult.color);
                CheckAndWriteAttribute(BNXmlWriter, "color2", cult.color2);
                CheckAndWriteAttribute(BNXmlWriter, "elite_basic_troop", cult.elite_basic_troop);
                CheckAndWriteAttribute(BNXmlWriter, "basic_troop", cult.basic_troop);
                CheckAndWriteAttribute(BNXmlWriter, "melee_militia_troop", cult.melee_militia_troop);
                CheckAndWriteAttribute(BNXmlWriter, "ranged_militia_troop", cult.ranged_militia_troop);
                CheckAndWriteAttribute(BNXmlWriter, "melee_elite_militia_troop", cult.melee_elite_militia_troop);
                CheckAndWriteAttribute(BNXmlWriter, "ranged_elite_militia_troop", cult.ranged_elite_militia_troop);
                CheckAndWriteAttribute(BNXmlWriter, "can_have_settlement", cult.can_have_settlement);
                CheckAndWriteAttribute(BNXmlWriter, "town_edge_number", cult.town_edge_number);
                CheckAndWriteAttribute(BNXmlWriter, "villager_party_template", cult.villager_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "default_party_template", cult.default_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "caravan_party_template", cult.caravan_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "elite_caravan_party_template", cult.elite_caravan_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "militia_party_template", cult.militia_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "rebels_party_template", cult.rebels_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "prosperity_bonus", cult.prosperity_bonus);
                CheckAndWriteAttribute(BNXmlWriter, "encounter_background_mesh", cult.encounter_background_mesh);
                CheckAndWriteAttribute(BNXmlWriter, "default_face_key", cult.default_face_key);
                CheckAndWriteAttribute(BNXmlWriter, "text", cult.text);
                CheckAndWriteAttribute(BNXmlWriter, "tournament_master", cult.tournament_master);
                CheckAndWriteAttribute(BNXmlWriter, "villager", cult.villager);
                CheckAndWriteAttribute(BNXmlWriter, "caravan_master", cult.caravan_master);
                CheckAndWriteAttribute(BNXmlWriter, "armed_trader", cult.armed_trader);
                CheckAndWriteAttribute(BNXmlWriter, "caravan_guard", cult.caravan_guard);
                CheckAndWriteAttribute(BNXmlWriter, "veteran_caravan_guard", cult.veteran_caravan_guard);
                CheckAndWriteAttribute(BNXmlWriter, "duel_preset", cult.duel_preset);
                CheckAndWriteAttribute(BNXmlWriter, "prison_guard", cult.prison_guard);
                CheckAndWriteAttribute(BNXmlWriter, "guard", cult.guard);
                CheckAndWriteAttribute(BNXmlWriter, "steward", cult.steward);
                CheckAndWriteAttribute(BNXmlWriter, "blacksmith", cult.blacksmith);
                CheckAndWriteAttribute(BNXmlWriter, "weaponsmith", cult.weaponsmith);
                CheckAndWriteAttribute(BNXmlWriter, "townswoman", cult.townswoman);
                CheckAndWriteAttribute(BNXmlWriter, "townswoman_infant", cult.townswoman_infant);
                CheckAndWriteAttribute(BNXmlWriter, "townswoman_child", cult.townswoman_child);
                CheckAndWriteAttribute(BNXmlWriter, "townswoman_teenager", cult.townswoman_teenager);
                CheckAndWriteAttribute(BNXmlWriter, "townsman", cult.townsman);
                CheckAndWriteAttribute(BNXmlWriter, "townsman_infant", cult.townsman_infant);
                CheckAndWriteAttribute(BNXmlWriter, "townsman_child", cult.townsman_child);
                CheckAndWriteAttribute(BNXmlWriter, "village_woman", cult.village_woman);
                CheckAndWriteAttribute(BNXmlWriter, "villager_male_child", cult.villager_male_child);
                CheckAndWriteAttribute(BNXmlWriter, "villager_male_teenager", cult.villager_male_teenager);
                CheckAndWriteAttribute(BNXmlWriter, "villager_female_child", cult.villager_female_child);
                CheckAndWriteAttribute(BNXmlWriter, "villager_female_teenager", cult.villager_female_teenager);
                CheckAndWriteAttribute(BNXmlWriter, "townsman_teenager", cult.townsman_teenager);
                CheckAndWriteAttribute(BNXmlWriter, "ransom_broker", cult.ransom_broker);
                CheckAndWriteAttribute(BNXmlWriter, "gangleader_bodyguard", cult.gangleader_bodyguard);
                CheckAndWriteAttribute(BNXmlWriter, "merchant_notary", cult.merchant_notary);
                CheckAndWriteAttribute(BNXmlWriter, "artisan_notary", cult.artisan_notary);
                CheckAndWriteAttribute(BNXmlWriter, "preacher_notary", cult.preacher_notary);
                CheckAndWriteAttribute(BNXmlWriter, "rural_notable_notary", cult.rural_notable_notary);
                CheckAndWriteAttribute(BNXmlWriter, "shop_worker", cult.shop_worker);
                CheckAndWriteAttribute(BNXmlWriter, "tavernkeeper", cult.tavernkeeper);
                CheckAndWriteAttribute(BNXmlWriter, "taverngamehost", cult.taverngamehost);
                CheckAndWriteAttribute(BNXmlWriter, "musician", cult.musician);
                CheckAndWriteAttribute(BNXmlWriter, "tavern_wench", cult.tavern_wench);
                CheckAndWriteAttribute(BNXmlWriter, "armorer", cult.armorer);
                CheckAndWriteAttribute(BNXmlWriter, "horseMerchant", cult.horseMerchant);
                CheckAndWriteAttribute(BNXmlWriter, "barber", cult.barber);
                CheckAndWriteAttribute(BNXmlWriter, "merchant", cult.merchant);
                CheckAndWriteAttribute(BNXmlWriter, "beggar", cult.beggar);
                CheckAndWriteAttribute(BNXmlWriter, "female_beggar", cult.female_beggar);
                CheckAndWriteAttribute(BNXmlWriter, "female_dancer", cult.female_dancer);
                CheckAndWriteAttribute(BNXmlWriter, "gear_practice_dummy", cult.gear_practice_dummy);
                CheckAndWriteAttribute(BNXmlWriter, "weapon_practice_stage_1", cult.weapon_practice_stage_1);
                CheckAndWriteAttribute(BNXmlWriter, "weapon_practice_stage_2", cult.weapon_practice_stage_2);
                CheckAndWriteAttribute(BNXmlWriter, "weapon_practice_stage_3", cult.weapon_practice_stage_3);
                CheckAndWriteAttribute(BNXmlWriter, "gear_dummy", cult.gear_dummy);
                CheckAndWriteAttribute(BNXmlWriter, "board_game_type", cult.board_game_type);


                CheckAndWriteAttribute(BNXmlWriter, "vassal_reward_party_template", cult.vassal_reward_party_template);
                CheckAndWriteAttribute(BNXmlWriter, "faction_banner_key", cult.faction_banner_key);
                CheckAndWriteAttribute(BNXmlWriter, "basic_mercenary_troop", cult.basic_mercenary_troop);
                CheckAndWriteAttribute(BNXmlWriter, "militia_bonus", cult.militia_bonus);
                CheckAndWriteAttribute(BNXmlWriter, "is_bandit", cult.is_bandit);
                CheckAndWriteAttribute(BNXmlWriter, "bandit_chief", cult.bandit_chief);
                CheckAndWriteAttribute(BNXmlWriter, "bandit_raider", cult.bandit_raider);
                CheckAndWriteAttribute(BNXmlWriter, "bandit_bandit", cult.bandit_bandit);
                CheckAndWriteAttribute(BNXmlWriter, "bandit_boss", cult.bandit_boss);
                CheckAndWriteAttribute(BNXmlWriter, "bandit_boss_party_template", cult.bandit_boss_party_template);

                WriteVassalRewards(BNXmlWriter, "vassal_reward_items", cult.reward_item_id);

                WriteNamesNodes(BNXmlWriter, "male_names", cult.male_names);
                WriteNamesNodes(BNXmlWriter, "female_names", cult.female_names);
                WriteNamesNodes(BNXmlWriter, "clan_names", cult.clan_names);

                WriteCulturalFeats(BNXmlWriter, "cultural_feats", cult.cultural_feat_id);
                WritePossibleClanIcons(BNXmlWriter, "possible_clan_banner_icon_ids", cult.banner_icon_id);

                // 1.72 update 
                WriteNPCTemplatesNodes(BNXmlWriter, "child_character_templates", cult.child_character_templates);
                WriteNPCTemplatesNodes(BNXmlWriter, "notable_and_wanderer_templates", cult.notable_and_wanderer_templates);
                WriteNPCTemplatesNodes(BNXmlWriter, "lord_templates", cult.lord_templates);
                WriteNPCTemplatesNodes(BNXmlWriter, "rebellion_hero_templates", cult.rebellion_hero_templates);

                WriteNPCTemplatesNodes(BNXmlWriter, "tournament_team_templates_one_participant", cult.TTT_one_participants);
                WriteNPCTemplatesNodes(BNXmlWriter, "tournament_team_templates_two_participant", cult.TTT_two_participants);
                WriteNPCTemplatesNodes(BNXmlWriter, "tournament_team_templates_four_participant", cult.TTT_four_participants);








                BNXmlWriter.WriteFullEndElement();
            }
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();


    }

    void WriteHeroAsset()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.Hero_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.Hero_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("Heroes");

        foreach (Hero hero in exported_Mod.modFilesData.heroesData.heroes)
        {
            if (hero != null)
            {
                BNXmlWriter.WriteStartElement("Hero");

                CheckAndWriteAttribute(BNXmlWriter, "id", hero.id);
                CheckAndWriteAttribute(BNXmlWriter, "alive", hero.alive);
                CheckAndWriteAttribute(BNXmlWriter, "is_noble", hero.is_noble);
                CheckAndWriteAttribute(BNXmlWriter, "faction", hero.faction);
                CheckAndWriteAttribute(BNXmlWriter, "banner_key", hero.banner_key);
                CheckAndWriteAttribute(BNXmlWriter, "father", hero.father);
                CheckAndWriteAttribute(BNXmlWriter, "mother", hero.mother);
                CheckAndWriteAttribute(BNXmlWriter, "spouse", hero.spouse);
                CheckAndWriteAttribute(BNXmlWriter, "text", hero.text);

                BNXmlWriter.WriteFullEndElement();
            }
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();


    }
    void WriteItemAsset()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.Item_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.Item_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("Items");

        foreach (Item item in exported_Mod.modFilesData.itemsData.items)
        {

            if (item.IsCraftedItem)
            {
                BNXmlWriter.WriteStartElement("CraftedItem");

                CheckAndWriteAttribute(BNXmlWriter, "id", item.id);
                CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(item.itemName));
                CheckAndWriteAttribute(BNXmlWriter, "crafting_template", item.CT_crafting_template);
                CheckAndWriteAttribute(BNXmlWriter, "is_merchandise", item.is_merchandise);
                CheckAndWriteAttribute(BNXmlWriter, "culture", item.culture);

                // 1.7.2 update
                if (item.CT_has_modifier == "true")
                {
                    CheckAndWriteAttribute(BNXmlWriter, "has_modifier", item.CT_has_modifier);
                    CheckAndWriteAttribute(BNXmlWriter, "item_modifier_group", item.WPN_item_modifier_group);
                }

                BNXmlWriter.WriteFullEndElement();

            }
            else
            {
                BNXmlWriter.WriteStartElement("Item");

                CheckAndWriteAttribute(BNXmlWriter, "id", item.id);
                CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(item.itemName));
                CheckAndWriteAttribute(BNXmlWriter, "mesh", item.mesh);
                CheckAndWriteAttribute(BNXmlWriter, "culture", item.culture);

                CheckAndWriteAttribute(BNXmlWriter, "is_merchandise", item.is_merchandise);

                CheckZeroWriteAttribute(BNXmlWriter, "value", item.value);

                CheckAndWriteAttribute(BNXmlWriter, "item_category", item.item_category);
                CheckAndWriteAttribute(BNXmlWriter, "weight", item.weight);
                CheckAndWriteAttribute(BNXmlWriter, "difficulty", item.difficulty);
                CheckAndWriteAttribute(BNXmlWriter, "appearance", item.appearance);
                CheckAndWriteAttribute(BNXmlWriter, "Type", item.Type);

                CheckAndWriteAttribute(BNXmlWriter, "subtype", item.subtype);
                CheckAndWriteAttribute(BNXmlWriter, "lod_atlas_index", item.lod_atlas_index);
                CheckAndWriteAttribute(BNXmlWriter, "recalculate_body", item.recalculate_body);
                CheckAndWriteAttribute(BNXmlWriter, "multiplayer_item", item.multiplayer_item);
                CheckAndWriteAttribute(BNXmlWriter, "using_tableau", item.using_tableau);

                CheckAndWriteAttribute(BNXmlWriter, "IsFood", item.IsFood);
                CheckAndWriteAttribute(BNXmlWriter, "body_name", item.body_name);
                CheckAndWriteAttribute(BNXmlWriter, "shield_body_name", item.shield_body_name);
                CheckAndWriteAttribute(BNXmlWriter, "flying_mesh", item.flying_mesh);
                CheckAndWriteAttribute(BNXmlWriter, "AmmoOffset", item.AmmoOffset);
                CheckAndWriteAttribute(BNXmlWriter, "prefab", item.prefab);
                CheckAndWriteAttribute(BNXmlWriter, "item_holsters", item.item_holsters);
                CheckAndWriteAttribute(BNXmlWriter, "holster_body_name", item.holster_body_name);
                CheckAndWriteAttribute(BNXmlWriter, "holster_mesh", item.holster_mesh);
                CheckAndWriteAttribute(BNXmlWriter, "holster_mesh_with_weapon", item.holster_mesh_with_weapon);
                CheckAndWriteAttribute(BNXmlWriter, "has_lower_holster_priority", item.has_lower_holster_priority);
                CheckAndWriteAttribute(BNXmlWriter, "holster_position_shift", item.holster_position_shift);

                BNXmlWriter.WriteStartElement("ItemComponent");

                if (item.IsWeapon)
                {

                    BNXmlWriter.WriteStartElement("Weapon");

                    //WEAPONS
                    CheckAndWriteAttribute(BNXmlWriter, "weapon_class", item.WPN_weapon_class);
                    CheckAndWriteAttribute(BNXmlWriter, "thrust_speed", item.WPN_thrust_speed);
                    CheckAndWriteAttribute(BNXmlWriter, "thrust_damage", item.WPN_thrust_damage);
                    CheckAndWriteAttribute(BNXmlWriter, "thrust_damage_type", item.WPN_thrust_damage_type);
                    CheckAndWriteAttribute(BNXmlWriter, "speed_rating", item.WPN_speed_rating);
                    CheckAndWriteAttribute(BNXmlWriter, "physics_material", item.WPN_physics_material);
                    CheckAndWriteAttribute(BNXmlWriter, "item_usage", item.WPN_item_usage);
                    CheckAndWriteAttribute(BNXmlWriter, "position", item.WPN_position);
                    CheckAndWriteAttribute(BNXmlWriter, "rotation", item.WPN_rotation);
                    CheckAndWriteAttribute(BNXmlWriter, "weapon_length", item.WPN_weapon_length);
                    CheckAndWriteAttribute(BNXmlWriter, "center_of_mass", item.WPN_center_of_mass);
                    CheckAndWriteAttribute(BNXmlWriter, "hit_points", item.WPN_hit_points);
                    CheckAndWriteAttribute(BNXmlWriter, "weapon_balance", item.WPN_weapon_balance);
                    CheckAndWriteAttribute(BNXmlWriter, "missile_speed", item.WPN_missile_speed);
                    CheckAndWriteAttribute(BNXmlWriter, "swing_damage", item.WPN_swing_damage);
                    CheckAndWriteAttribute(BNXmlWriter, "swing_damage_type", item.WPN_swing_damage_type);
                    CheckAndWriteAttribute(BNXmlWriter, "ammo_class", item.WPN_ammo_class);
                    CheckAndWriteAttribute(BNXmlWriter, "ammo_limit", item.WPN_ammo_limit);
                    CheckAndWriteAttribute(BNXmlWriter, "accuracy", item.WPN_accuracy);
                    CheckAndWriteAttribute(BNXmlWriter, "flying_sound_code", item.WPN_flying_sound_code);
                    CheckAndWriteAttribute(BNXmlWriter, "rotation_speed", item.WPN_rotation_speed);
                    CheckAndWriteAttribute(BNXmlWriter, "trail_particle_name", item.WPN_trail_particle_name);
                    CheckAndWriteAttribute(BNXmlWriter, "stack_amount", item.WPN_stack_amount);

                    CheckAndWriteAttribute(BNXmlWriter, "passby_sound_code", item.WPN_passby_sound_code);
                    CheckAndWriteAttribute(BNXmlWriter, "sticking_rotation", item.WPN_sticking_rotation);
                    CheckAndWriteAttribute(BNXmlWriter, "sticking_position", item.WPN_sticking_position);

                    CheckAndWriteAttribute(BNXmlWriter, "body_armor", item.ARMOR_body_armor);

                    // update 1.7.2
                    CheckAndWriteAttribute(BNXmlWriter, "reload_phase_count", item.WPN_reload_phase_count);
                    CheckAndWriteAttribute(BNXmlWriter, "item_modifier_group", item.WPN_item_modifier_group);

                    BNXmlWriter.WriteStartElement("WeaponFlags");

                    CheckAndWriteAttribute(BNXmlWriter, "RangedWeapon", item.WPN_FLG_RangedWeapon);
                    CheckAndWriteAttribute(BNXmlWriter, "HasString", item.WPN_FLG_HasString);
                    CheckAndWriteAttribute(BNXmlWriter, "CantReloadOnHorseback", item.WPN_FLG_CantReloadOnHorseback);
                    CheckAndWriteAttribute(BNXmlWriter, "NotUsableWithOneHand", item.WPN_FLG_NotUsableWithOneHand);
                    CheckAndWriteAttribute(BNXmlWriter, "TwoHandIdleOnMount", item.WPN_FLG_TwoHandIdleOnMount);
                    CheckAndWriteAttribute(BNXmlWriter, "Consumable", item.WPN_FLG_Consumable);
                    CheckAndWriteAttribute(BNXmlWriter, "AutoReload", item.WPN_FLG_AutoReload);
                    CheckAndWriteAttribute(BNXmlWriter, "UseHandAsThrowBase", item.WPN_FLG_UseHandAsThrowBase);
                    CheckAndWriteAttribute(BNXmlWriter, "AmmoSticksWhenShot", item.WPN_FLG_AmmoSticksWhenShot);
                    CheckAndWriteAttribute(BNXmlWriter, "AmmoBreaksOnBounceBack", item.WPN_FLG_AmmoBreaksOnBounceBack);
                    CheckAndWriteAttribute(BNXmlWriter, "AttachAmmoToVisual", item.WPN_FLG_AttachAmmoToVisual);
                    CheckAndWriteAttribute(BNXmlWriter, "CanBlockRanged", item.WPN_FLG_CanBlockRanged);
                    CheckAndWriteAttribute(BNXmlWriter, "HasHitPoints", item.WPN_FLG_HasHitPoints);
                    CheckAndWriteAttribute(BNXmlWriter, "MeleeWeapon", item.WPN_FLG_MeleeWeapon);
                    CheckAndWriteAttribute(BNXmlWriter, "PenaltyWithShield", item.WPN_FLG_PenaltyWithShield);
                    CheckAndWriteAttribute(BNXmlWriter, "WideGrip", item.WPN_FLG_WideGrip);

                    CheckAndWriteAttribute(BNXmlWriter, "StringHeldByHand", item.WPN_FLG_StringHeldByHand);
                    CheckAndWriteAttribute(BNXmlWriter, "UnloadWhenSheathed", item.WPN_FLG_UnloadWhenSheathed);
                    CheckAndWriteAttribute(BNXmlWriter, "CanPenetrateShield", item.WPN_FLG_CanPenetrateShield);
                    CheckAndWriteAttribute(BNXmlWriter, "MultiplePenetration", item.WPN_FLG_MultiplePenetration);
                    CheckAndWriteAttribute(BNXmlWriter, "Burning", item.WPN_FLG_Burning);
                    CheckAndWriteAttribute(BNXmlWriter, "LeavesTrail", item.WPN_FLG_LeavesTrail);
                    CheckAndWriteAttribute(BNXmlWriter, "CanKnockDown", item.WPN_FLG_CanKnockDown);
                    CheckAndWriteAttribute(BNXmlWriter, "MissileWithPhysics", item.WPN_FLG_MissileWithPhysics);
                    CheckAndWriteAttribute(BNXmlWriter, "AmmoCanBreakOnBounceBack", item.WPN_FLG_AmmoCanBreakOnBounceBack);
                    CheckAndWriteAttribute(BNXmlWriter, "AffectsArea", item.WPN_FLG_AffectsArea);
                    CheckAndWriteAttribute(BNXmlWriter, "AffectsAreaBig", item.WPN_FLG_AffectsAreaBig);

                    BNXmlWriter.WriteFullEndElement();

                    BNXmlWriter.WriteFullEndElement();


                }
                else if (item.IsArmor)
                {
                    BNXmlWriter.WriteStartElement("Armor");

                    // ARM ARMOR
                    CheckAndWriteAttribute(BNXmlWriter, "arm_armor", item.ARMOR_arm_armor);
                    CheckAndWriteAttribute(BNXmlWriter, "covers_hands", item.ARMOR_covers_hands);
                    CheckAndWriteAttribute(BNXmlWriter, "modifier_group", item.ARMOR_modifier_group);
                    CheckAndWriteAttribute(BNXmlWriter, "material_type", item.ARMOR_material_type);
                    CheckAndWriteAttribute(BNXmlWriter, "family_type", item.ARMOR_family_type);

                    // BODY ARMOR

                    CheckAndWriteAttribute(BNXmlWriter, "body_armor", item.ARMOR_body_armor);
                    CheckAndWriteAttribute(BNXmlWriter, "leg_armor", item.ARMOR_leg_armor);
                    CheckAndWriteAttribute(BNXmlWriter, "has_gender_variations", item.ARMOR_has_gender_variations);
                    CheckAndWriteAttribute(BNXmlWriter, "covers_body", item.ARMOR_covers_body);
                    CheckAndWriteAttribute(BNXmlWriter, "body_mesh_type", item.ARMOR_body_mesh_type);
                    CheckAndWriteAttribute(BNXmlWriter, "head_armor", item.ARMOR_head_armor);
                    CheckAndWriteAttribute(BNXmlWriter, "hair_cover_type", item.ARMOR_hair_cover_type);
                    CheckAndWriteAttribute(BNXmlWriter, "beard_cover_type", item.ARMOR_beard_cover_type);
                    CheckAndWriteAttribute(BNXmlWriter, "covers_legs", item.ARMOR_covers_legs);

                    // ARMOR UPDATE

                    CheckAndWriteAttribute(BNXmlWriter, "mane_cover_type", item.ARMOR_mane_cover_type);
                    CheckAndWriteAttribute(BNXmlWriter, "maneuver_bonus", item.ARMOR_maneuver_bonus);
                    CheckAndWriteAttribute(BNXmlWriter, "speed_bonus", item.ARMOR_speed_bonus);
                    CheckAndWriteAttribute(BNXmlWriter, "charge_bonus", item.ARMOR_charge_bonus);
                    CheckAndWriteAttribute(BNXmlWriter, "reins_mesh", item.ARMOR_reins_mesh);
                    CheckAndWriteAttribute(BNXmlWriter, "covers_head", item.ARMOR_covers_head);
                    BNXmlWriter.WriteFullEndElement();

                }
                else if (item.IsHorse)
                {
                    BNXmlWriter.WriteStartElement("Horse");

                    CheckAndWriteAttribute(BNXmlWriter, "monster", item.HRS_monster);
                    CheckAndWriteAttribute(BNXmlWriter, "maneuver", item.HRS_maneuver);
                    CheckAndWriteAttribute(BNXmlWriter, "speed", item.HRS_speed);
                    CheckAndWriteAttribute(BNXmlWriter, "charge_damage", item.HRS_charge_damage);
                    CheckAndWriteAttribute(BNXmlWriter, "body_length", item.HRS_body_length);
                    CheckAndWriteAttribute(BNXmlWriter, "is_mountable", item.HRS_is_mountable);
                    CheckAndWriteAttribute(BNXmlWriter, "extra_health", item.HRS_extra_health);
                    CheckAndWriteAttribute(BNXmlWriter, "skeleton_scale", item.HRS_skeleton_scale);
                    CheckAndWriteAttribute(BNXmlWriter, "modifier_group", item.ARMOR_modifier_group);

                    // ITEM HORSES & OTHER UPDATE
                    CheckAndWriteAttribute(BNXmlWriter, "is_pack_animal", item.HRS_is_pack_animal);
                    CheckAndWriteAttribute(BNXmlWriter, "mane_mesh", item.HRS_mane_mesh);

                    BNXmlWriter.WriteStartElement("AdditionalMeshes");

                    if (item.ADD_Mesh_affected_by_cover != "" && item.ADD_Mesh_affected_by_cover != "false")
                    {
                        if (item.ADD_cover_Mesh_name != "")
                        {
                            BNXmlWriter.WriteStartElement("Mesh");
                            CheckAndWriteAttribute(BNXmlWriter, "name", item.ADD_cover_Mesh_name);
                            CheckAndWriteAttribute(BNXmlWriter, "affected_by_cover", item.ADD_Mesh_affected_by_cover);
                            BNXmlWriter.WriteFullEndElement();
                        }
                        if (item.ADD_Mesh != "")
                        {
                            BNXmlWriter.WriteStartElement("Mesh");
                            CheckAndWriteAttribute(BNXmlWriter, "name", item.ADD_Mesh);
                            BNXmlWriter.WriteFullEndElement();
                        }
                        BNXmlWriter.WriteFullEndElement();


                        BNXmlWriter.WriteStartElement("Materials");

                        int i = 0;
                        foreach (var mat in item.ADD_mat_name)
                        {
                            BNXmlWriter.WriteStartElement("Material");

                            CheckAndWriteAttribute(BNXmlWriter, "name", item.ADD_mat_name[i]);

                            if (item.aDD_mat_meshMult_a[i] != "")
                            {
                                BNXmlWriter.WriteStartElement("MeshMultipliers");

                                BNXmlWriter.WriteStartElement("MeshMultiplier");
                                CheckAndWriteAttribute(BNXmlWriter, "mesh_multiplier", item.aDD_mat_meshMult_a[i]);
                                CheckAndWriteAttribute(BNXmlWriter, "percentage", item.aDD_mat_meshMult_a_prc[i]);
                                BNXmlWriter.WriteFullEndElement();

                                if (item.aDD_mat_meshMult_b[i] != "")
                                {
                                    BNXmlWriter.WriteStartElement("MeshMultiplier");
                                    CheckAndWriteAttribute(BNXmlWriter, "mesh_multiplier", item.aDD_mat_meshMult_b[i]);
                                    CheckAndWriteAttribute(BNXmlWriter, "percentage", item.aDD_mat_meshMult_b_prc[i]);
                                    BNXmlWriter.WriteFullEndElement();
                                }

                                BNXmlWriter.WriteFullEndElement();
                            }

                            BNXmlWriter.WriteFullEndElement();
                            i++;
                        }
                    }

                    BNXmlWriter.WriteFullEndElement();


                    BNXmlWriter.WriteFullEndElement();

                }
                else if (item.IsTrade)
                {
                    BNXmlWriter.WriteStartElement("Trade");

                    CheckAndWriteAttribute(BNXmlWriter, "morale_bonus", item.TRADE_morale_bonus);
                    BNXmlWriter.WriteFullEndElement();

                }
                BNXmlWriter.WriteFullEndElement();

                BNXmlWriter.WriteStartElement("Flags");

                CheckAndWriteAttribute(BNXmlWriter, "DropOnWeaponChange", item.FLG_DropOnWeaponChange);
                CheckAndWriteAttribute(BNXmlWriter, "ForceAttachOffHandPrimaryItemBone", item.FLG_ForceAttachOffHandPrimaryItemBone);
                CheckAndWriteAttribute(BNXmlWriter, "HeldInOffHand", item.FLG_HeldInOffHand);
                CheckAndWriteAttribute(BNXmlWriter, "HasToBeHeldUp", item.FLG_HasToBeHeldUp);
                CheckAndWriteAttribute(BNXmlWriter, "WoodenParry", item.FLG_WoodenParry);
                CheckAndWriteAttribute(BNXmlWriter, "DoNotScaleBodyAccordingToWeaponLength", item.FLG_DoNotScaleBodyAccordingToWeaponLength);
                CheckAndWriteAttribute(BNXmlWriter, "QuickFadeOut", item.FLG_QuickFadeOut);
                CheckAndWriteAttribute(BNXmlWriter, "CannotBePickedUp", item.FLG_CannotBePickedUp);
                CheckAndWriteAttribute(BNXmlWriter, "DropOnAnyAction", item.FLG_DropOnAnyAction);
                CheckAndWriteAttribute(BNXmlWriter, "Civilian", item.FLG_Civilian);
                CheckAndWriteAttribute(BNXmlWriter, "UseTeamColor", item.FLG_UseTeamColor);
                CheckAndWriteAttribute(BNXmlWriter, "ForceAttachOffHandSecondaryItemBone", item.FLG_ForceAttachOffHandSecondaryItemBone);
                CheckAndWriteAttribute(BNXmlWriter, "DoesNotHideChest", item.FLG_DoesNotHideChest);

                BNXmlWriter.WriteFullEndElement();

                BNXmlWriter.WriteFullEndElement();

            }

        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        //  UnityEditor.AssetDatabase.Refresh();


    }


    void WriteKingdomAsset()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.Kingdom_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.Kingdom_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("Kingdoms");

        foreach (Kingdom kingd in exported_Mod.modFilesData.kingdomsData.kingdoms)
        {
            BNXmlWriter.WriteStartElement("Kingdom");

            CheckAndWriteAttribute(BNXmlWriter, "id", kingd.id);
            CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(kingd.kingdomName));

            CheckAndWriteAttribute(BNXmlWriter, "owner", kingd.owner);
            CheckAndWriteAttribute(BNXmlWriter, "banner_key", kingd.banner_key);
            CheckAndWriteAttribute(BNXmlWriter, "primary_banner_color", kingd.primary_banner_color);
            CheckAndWriteAttribute(BNXmlWriter, "secondary_banner_color", kingd.secondary_banner_color);
            CheckAndWriteAttribute(BNXmlWriter, "label_color", kingd.label_color);
            CheckAndWriteAttribute(BNXmlWriter, "color", kingd.color);
            CheckAndWriteAttribute(BNXmlWriter, "color2", kingd.color2);
            CheckAndWriteAttribute(BNXmlWriter, "alternative_color", kingd.alternative_color);
            CheckAndWriteAttribute(BNXmlWriter, "alternative_color2", kingd.alternative_color2);
            CheckAndWriteAttribute(BNXmlWriter, "culture", kingd.culture);
            CheckAndWriteAttribute(BNXmlWriter, "settlement_banner_mesh", kingd.settlement_banner_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "flag_mesh", kingd.flag_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "short_name", kingd.short_name);
            CheckAndWriteAttribute(BNXmlWriter, "title", kingd.title);
            CheckAndWriteAttribute(BNXmlWriter, "ruler_title", kingd.ruler_title);
            CheckAndWriteAttribute(BNXmlWriter, "text", kingd.text);

            BNXmlWriter.WriteStartElement("relationships");

            int i = 0;
            foreach (var rel in kingd.relationships)
            {
                if (rel != "")
                {
                    BNXmlWriter.WriteStartElement("relationship");

                    CheckAndWriteAttribute(BNXmlWriter, "kingdom", kingd.relationships[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "value", kingd.relationValues[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "isAtWar", kingd.relationsAtWar[i]);

                    BNXmlWriter.WriteFullEndElement();

                }
                i++;
            }


            BNXmlWriter.WriteFullEndElement();
            BNXmlWriter.WriteStartElement("policies");

            i = 0;
            foreach (var pol in kingd.policies)
            {
                if (pol != "")
                {
                    BNXmlWriter.WriteStartElement("policy");

                    CheckAndWriteAttribute(BNXmlWriter, "id", kingd.policies[i]);

                    BNXmlWriter.WriteFullEndElement();

                }
                i++;
            }


            BNXmlWriter.WriteFullEndElement();

            BNXmlWriter.WriteFullEndElement();
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();


    }

    void WriteNPCAsset()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.NPCCharacter_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.NPCCharacter_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("NPCCharacters");

        foreach (NPCCharacter npc in exported_Mod.modFilesData.npcChrData.NPCCharacters)
        {
            if (npc != null)
            {
                BNXmlWriter.WriteStartElement("NPCCharacter");

                CheckAndWriteAttribute(BNXmlWriter, "id", npc.id);
                CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(npc.npcName));
                CheckAndWriteAttribute(BNXmlWriter, "is_female", npc.is_female);
                CheckAndWriteAttribute(BNXmlWriter, "is_hero", npc.is_hero);
                CheckAndWriteAttribute(BNXmlWriter, "is_companion", npc.is_companion);
                CheckAndWriteAttribute(BNXmlWriter, "is_mercenary", npc.is_mercenary);
                CheckAndWriteAttribute(BNXmlWriter, "is_child_template", npc.is_child_template);
                CheckAndWriteAttribute(BNXmlWriter, "is_template", npc.is_template);
                CheckAndWriteAttribute(BNXmlWriter, "age", npc.age);
                CheckAndWriteAttribute(BNXmlWriter, "voice", npc.voice);
                CheckAndWriteAttribute(BNXmlWriter, "culture", npc.culture);
                CheckAndWriteAttribute(BNXmlWriter, "default_group", npc.default_group);

                CheckAndWriteAttribute(BNXmlWriter, "level", npc.level);
                CheckAndWriteAttribute(BNXmlWriter, "occupation", npc.occupation);

                CheckAndWriteAttribute(BNXmlWriter, "banner_symbol_mesh_name", npc.banner_symbol_mesh_name);
                CheckAndWriteAttribute(BNXmlWriter, "banner_symbol_color", npc.banner_symbol_color);

                CheckAndWriteAttribute(BNXmlWriter, "skill_template", npc.skill_template);
                CheckAndWriteAttribute(BNXmlWriter, "battleTemplate", npc.battleTemplate);
                CheckAndWriteAttribute(BNXmlWriter, "civilianTemplate", npc.civilianTemplate);
                CheckAndWriteAttribute(BNXmlWriter, "upgrade_requires", npc.upgrade_requires);
                CheckAndWriteAttribute(BNXmlWriter, "is_basic_troop", npc.is_basic_troop);
                CheckAndWriteAttribute(BNXmlWriter, "formation_position_preference", npc.formation_position_preference);

                // update 1.7.2
                CheckAndWriteAttribute(BNXmlWriter, "face_mesh_cache", npc.face_mesh_cache);
                CheckAndWriteAttribute(BNXmlWriter, "is_hidden_encyclopedia", npc.is_hidden_encyclopedia);
                CheckAndWriteAttribute(BNXmlWriter, "is_obsolete", npc.is_obsolete);

                BNXmlWriter.WriteStartElement("face");
                if (npc.BP_version != "" && npc.BP_version != "0")
                {
                    BNXmlWriter.WriteStartElement("BodyProperties");

                    CheckAndWriteAttribute(BNXmlWriter, "version", npc.BP_version);
                    CheckAndWriteAttribute(BNXmlWriter, "age", npc.BP_age);
                    CheckAndWriteAttribute(BNXmlWriter, "weight", npc.BP_weight);

                    CheckAndWriteAttribute(BNXmlWriter, "build", npc.BP_build);
                    CheckAndWriteAttribute(BNXmlWriter, "key", npc.BP_key);

                    BNXmlWriter.WriteFullEndElement();
                }
                if (npc.Max_BP_version != "" && npc.Max_BP_version != "0")
                {
                    BNXmlWriter.WriteStartElement("BodyPropertiesMax");

                    CheckAndWriteAttribute(BNXmlWriter, "version", npc.Max_BP_version);
                    CheckAndWriteAttribute(BNXmlWriter, "age", npc.Max_BP_age);
                    CheckAndWriteAttribute(BNXmlWriter, "weight", npc.Max_BP_weight);

                    CheckAndWriteAttribute(BNXmlWriter, "build", npc.Max_BP_build);
                    CheckAndWriteAttribute(BNXmlWriter, "key", npc.Max_BP_key);

                    BNXmlWriter.WriteFullEndElement();
                }
                if (npc.face_key_template != "")
                {
                    BNXmlWriter.WriteStartElement("face_key_template");

                    CheckAndWriteAttribute(BNXmlWriter, "value", npc.face_key_template);

                    BNXmlWriter.WriteFullEndElement();
                }

                // update 1.7.2
                WriteHairBodyTags(BNXmlWriter, "hair_tags", npc.hair_tag);
                WriteBeardBodyTags(BNXmlWriter, "beard_tags", npc.beard_tag);

                BNXmlWriter.WriteFullEndElement();

                BNXmlWriter.WriteStartElement("skills");
                int i = 0;
                foreach (var val in npc.skills)
                {
                    if (val != "")
                    {
                        BNXmlWriter.WriteStartElement("skill");

                        CheckAndWriteAttribute(BNXmlWriter, "id", npc.skills[i]);
                        CheckAndWriteAttribute(BNXmlWriter, "value", npc.skillValues[i]);

                        BNXmlWriter.WriteFullEndElement();

                    }
                    i++;
                }
                BNXmlWriter.WriteFullEndElement();

                BNXmlWriter.WriteStartElement("Traits");
                i = 0;
                foreach (var val in npc.traits)
                {
                    if (val != "")
                    {
                        BNXmlWriter.WriteStartElement("Trait");

                        CheckAndWriteAttribute(BNXmlWriter, "id", npc.traits[i]);
                        CheckAndWriteAttribute(BNXmlWriter, "value", npc.traitValues[i]);

                        BNXmlWriter.WriteFullEndElement();

                    }
                    i++;
                }
                BNXmlWriter.WriteFullEndElement();

                // Write Items - Rosters - Sets

                if (npc.equipment_Main != "" || npc.equipment_Roster.Length != 0 || npc.equipment_Set.Length != 0)
                {
                    BNXmlWriter.WriteStartElement("Equipments");

                    if (npc.equipment_Roster.Length != 0)
                    {

                        foreach (string roster in npc.equipment_Roster)
                        {
                            BNXmlWriter.WriteStartElement("EquipmentRoster");

                            var equip_roster = GetEquipmentSetAssetNPC(npc, roster, 1);

                            if (equip_roster.IsCivilianEquip)
                            {
                                CheckAndWriteAttribute(BNXmlWriter, "civilian", "true");
                            }

                            WriteMainEquipmentNPC(BNXmlWriter, equip_roster);

                            BNXmlWriter.WriteFullEndElement();

                        }


                    }

                    if (npc.equipment_Set.Length != 0)
                    {

                        foreach (var set in npc.equipment_Set)
                        {
                            if (set != "")
                            {

                                BNXmlWriter.WriteStartElement("EquipmentSet");

                                var equip_set = GetEquipmentAssetNPC(npc, set);

                                CheckAndWriteAttribute(BNXmlWriter, "id", set);

                                if (equip_set.IsCivilianTemplate)
                                {
                                    CheckAndWriteAttribute(BNXmlWriter, "civilian", "true");
                                }

                                BNXmlWriter.WriteFullEndElement();
                            }

                        }

                    }

                    if (npc.equipment_Main != "")
                    {
                        var equip = GetEquipmentSetAssetNPC(npc, npc.equipment_Main, 0);

                        WriteMainEquipmentNPC(BNXmlWriter, equip);

                    }

                    BNXmlWriter.WriteFullEndElement();

                }

                // WriteEquipmentSets(BNXmlWriter, npc);

                BNXmlWriter.WriteStartElement("upgrade_targets");
                i = 0;
                foreach (var val in npc.upgrade_targets)
                {
                    if (val != "")
                    {
                        BNXmlWriter.WriteStartElement("upgrade_target");

                        CheckAndWriteAttribute(BNXmlWriter, "id", npc.upgrade_targets[i]);

                        BNXmlWriter.WriteFullEndElement();

                    }
                    i++;
                }
                BNXmlWriter.WriteFullEndElement();

                if (npc.COMP_Companion != "")
                {
                    BNXmlWriter.WriteStartElement("Components");
                    BNXmlWriter.WriteStartElement("Companion");

                    CheckAndWriteAttribute(BNXmlWriter, "id", npc.COMP_Companion);

                    BNXmlWriter.WriteFullEndElement();
                    BNXmlWriter.WriteFullEndElement();

                }

                BNXmlWriter.WriteFullEndElement();
            }
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        //  UnityEditor.AssetDatabase.Refresh();

    }

    void WriteEquipmentAsset()
    {

        var path = export_path + exported_Mod.modFilesData.exportSettings.EquipmentSet_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("EquipmentRosters");

        foreach (Equipment equip in exported_Mod.modFilesData.equipmentsData.equipmentSets)
        {

            if (equip != null)
            {
                BNXmlWriter.WriteStartElement("EquipmentRoster");

                CheckAndWriteAttribute(BNXmlWriter, "id", equip.id);

                if (equip.IsCivilianTemplate)
                {
                    CheckAndWriteAttribute(BNXmlWriter, "civilian", "true");
                }
                CheckAndWriteAttribute(BNXmlWriter, "culture", equip.culture);


                foreach (var set in equip.eqpSetID)
                {
                    if (set != "")
                    {
                        BNXmlWriter.WriteStartElement("EquipmentSet");

                        var equip_set = GetEquipmentSetAsset(equip, set, 2);

                        if (equip_set.IsCivilianEquip)
                        {
                            CheckAndWriteAttribute(BNXmlWriter, "civilian", "true");
                        }

                        WriteMainEquipment(BNXmlWriter, equip_set);

                        BNXmlWriter.WriteFullEndElement();

                    }
                }

                BNXmlWriter.WriteStartElement("Flags");

                //var equip_set = GetEquipmentAsset(equip, equip.id);

                WriteEquipmentFlags(BNXmlWriter, equip);

                //BNXmlWriter.WriteFullEndElement();
                BNXmlWriter.WriteEndElement();

            }

            BNXmlWriter.WriteEndElement();
        }


        BNXmlWriter.WriteEndElement();

        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();

    }

    private static void WriteEquipmentFlags(XmlWriter BNXmlWriter, Equipment equip)
    {
        if (equip.IsEquipmentTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsEquipmentTemplate", "true");
        }
        if (equip.IsNobleTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsNobleTemplate", "true");
        }
        if (equip.IsMediumTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsMediumTemplate", "true");
        }
        if (equip.IsHeavyTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsHeavyTemplate", "true");
        }
        if (equip.IsFlamboyantTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsFlamboyantTemplate", "true");
        }
        if (equip.IsStoicTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsStoicTemplate", "true");
        }
        if (equip.IsNomadTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsNomadTemplate", "true");
        }
        if (equip.IsWoodlandTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsWoodlandTemplate", "true");
        }
        if (equip.IsFemaleTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsFemaleTemplate", "true");
        }
        if (equip.IsCivilianTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsCivilianTemplate", "true");
        }
        if (equip.IsCombatantTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsCombatantTemplate", "true");
        }
        if (equip.IsNoncombatantTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsNoncombatantTemplate", "true");
        }
        if (equip.IsChildTemplate)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsChildTemplate", "true");
        }
        if (equip.IsWandererEquipment)
        {
            CheckAndWriteAttribute(BNXmlWriter, "IsWandererEquipment", "true");
        }
    }

    // Types
    // 0 = main
    // 1 = roster
    // 2 = template
    EquipmentSet GetEquipmentSetAssetNPC(NPCCharacter npc, string dataName, int type)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {

            var equipmentSet = new EquipmentSet();
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
                                foreach (var data in iSDependencyOfMod.modFilesData.equipmentSetData.equipmentSets)
                                {
                                    if (data != null)
                                        if (data.EquipmentSetID == dataName)
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

            return equipmentSet;

        }
        return null;

    }

    Equipment GetEquipmentAssetNPC(NPCCharacter npc, string dataName)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {

            Equipment equipment = new Equipment();
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

            return equipment;
        }

        return null;
    }

    EquipmentSet GetEquipmentSetAsset(Equipment eqp, string dataName, int type)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {

            var equipmentSet = new EquipmentSet();
            string assetPath = "";
            string assetPathShort = "";

            switch (type)
            {
                case 0:
                    assetPath = dataPath + eqp.moduleID + "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                    assetPathShort = "/NPC/Equipment/EquipmentMain/" + dataName + ".asset";
                    break;
                case 1:
                    assetPath = dataPath + eqp.moduleID + "/NPC/Equipment/EquipmentRosters/" + dataName + ".asset";
                    assetPathShort = "/NPC/Equipment/EquipmentRosters/" + dataName + ".asset";
                    break;
                case 2:
                    assetPath = dataPath + eqp.moduleID + "/Sets/Equipments/EqpSets/" + dataName + ".asset";
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
                string modSett = modsSettingsPath + eqp.moduleID + ".asset";
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
                            if (depend == eqp.moduleID)
                            {
                                foreach (var data in iSDependencyOfMod.modFilesData.equipmentSetData.equipmentSets)
                                {
                                    if (data != null)
                                        if (data.EquipmentSetID == dataName)
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

                        Debug.Log("EquipmentSet " + dataName + " - Not EXIST in" + " ' " + eqp.moduleID + " ' " + "resources, and they dependencies.");

                    }
                }

            }

            return equipmentSet;

        }
        return null;

    }
    Equipment GetEquipmentAsset(Equipment eqp, string dataName)
    {
        // Face Key Template template
        // 
        if (dataName != null && dataName != "")
        {

            Equipment equipment = new Equipment();
            string assetPath = dataPath + eqp.moduleID + "/Sets/Equipments/" + dataName + ".asset";
            string assetPathShort = "/Sets/Equipments/" + dataName + ".asset";


            if (System.IO.File.Exists(assetPath))
            {
                equipment = (Equipment)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Equipment));
            }
            else
            {
                // SEARCH IN DEPENDENCIES
                string modSett = modsSettingsPath + eqp.moduleID + ".asset";
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
                            if (depend == eqp.moduleID)
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

                        Debug.Log("Equipment " + dataName + " - Not EXIST in" + " ' " + eqp.moduleID + " ' " + "resources, and they dependencies.");

                    }
                }

            }

            return equipment;
        }

        return null;
    }
    void WriteMainEquipmentNPC(XmlWriter BNXmlWriter, EquipmentSet equip)
    {
        if (equip.eqp_Item0 != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item0");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Item0);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Item1 != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item1");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Item1);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Item2 != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item2");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Item2);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Item3 != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item3");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Item3);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Head != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Head");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Head);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Body != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Body");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Body);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Cape != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Cape");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Cape);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Gloves != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Gloves");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Gloves);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Leg != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Leg");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Leg);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Horse != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Horse");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_Horse);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_HorseHarness != "")
        {
            BNXmlWriter.WriteStartElement("equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "HorseHarness");
            CheckAndWriteAttribute(BNXmlWriter, "id", equip.eqp_HorseHarness);

            BNXmlWriter.WriteFullEndElement();

        }
    }
    void WriteMainEquipment(XmlWriter BNXmlWriter, EquipmentSet equip)
    {


        if (equip.eqp_Item0 != "")
        {
            var item_id = equip.eqp_Item0;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item0");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Item1 != "")
        {
            var item_id = equip.eqp_Item1;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item1");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Item2 != "")
        {
            var item_id = equip.eqp_Item2;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item2");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Item3 != "")
        {
            var item_id = equip.eqp_Item3;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Item3");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Head != "")
        {
            var item_id = equip.eqp_Head;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Head");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Body != "")
        {
            var item_id = equip.eqp_Body;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Body");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Cape != "")
        {
            var item_id = equip.eqp_Cape;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Cape");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Gloves != "")
        {
            var item_id = equip.eqp_Gloves;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Gloves");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Leg != "")
        {
            var item_id = equip.eqp_Leg;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Leg");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_Horse != "")
        {
            var item_id = equip.eqp_Horse;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "Horse");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
        if (equip.eqp_HorseHarness != "")
        {
            var item_id = equip.eqp_HorseHarness;
            if (!item_id.Contains("Item."))
                item_id = "Item." + item_id;

            BNXmlWriter.WriteStartElement("Equipment");

            CheckAndWriteAttribute(BNXmlWriter, "slot", "HorseHarness");
            CheckAndWriteAttribute(BNXmlWriter, "id", item_id);

            BNXmlWriter.WriteFullEndElement();

        }
    }


    // private static void WriteEquipmentSets(XmlWriter BNXmlWriter, NPCCharacter npc)
    // {
    //     int i = 0;

    //     foreach (var npcSet in npc.IsCivilianEquip)
    //     {
    //         BNXmlWriter.WriteStartElement("equipmentSet");

    //         if (npc.IsCivilianEquip[i] == true)
    //         {
    //             CheckAndWriteAttribute(BNXmlWriter, "civilian", "true");
    //         }

    //         if (npc.eqp_Item0[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Item0");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Item0[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Item1[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Item1");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Item1[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Item2[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Item2");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Item2[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Item3[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Item3");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Item3[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Head[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Head");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Head[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Gloves[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Gloves");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Gloves[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Body[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Body");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Body[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Leg[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Leg");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Leg[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Cape[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Cape");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Cape[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_Horse[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "Horse");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_Item0[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }
    //         if (npc.eqp_HorseHarness[i] != "")
    //         {
    //             BNXmlWriter.WriteStartElement("equipment");
    //             CheckAndWriteAttribute(BNXmlWriter, "slot", "HorseHarness");
    //             CheckAndWriteAttribute(BNXmlWriter, "id", npc.eqp_HorseHarness[i]);
    //             BNXmlWriter.WriteEndElement();

    //         }

    //         BNXmlWriter.WriteFullEndElement();

    //         i++;
    //     }



    // }

    void WritePartyTemplateAsset()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.PartyTemplate_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.PartyTemplate_xml_name;


        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("partyTemplates");

        foreach (PartyTemplate PT in exported_Mod.modFilesData.PTdata.partyTemplates)
        {
            BNXmlWriter.WriteStartElement("MBPartyTemplate");

            CheckAndWriteAttribute(BNXmlWriter, "id", PT.id);

            if (PT.PTS_troop.Length != 0)
            {
                BNXmlWriter.WriteStartElement("stacks");

                int i = 0;

                foreach (var npcSet in PT.PTS_troop)
                {
                    if (PT.PTS_troop[i] != "")
                    {
                        BNXmlWriter.WriteStartElement("PartyTemplateStack");
                        CheckAndWriteAttribute(BNXmlWriter, "min_value", PT.PTS_min_value[i]);
                        CheckAndWriteAttribute(BNXmlWriter, "max_value", PT.PTS_max_value[i]);
                        CheckAndWriteAttribute(BNXmlWriter, "troop", PT.PTS_troop[i]);
                        BNXmlWriter.WriteEndElement();

                    }
                    i++;
                }
                BNXmlWriter.WriteEndElement();
            }

            BNXmlWriter.WriteFullEndElement();
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();


    }
    void WriteSettlementAsset()
    {

        // var path = export_path + exported_Mod.id + "/" + exported_Mod.modFilesData.exportSettings.Settlement_xml_name;
        var path = export_path + exported_Mod.modFilesData.exportSettings.Settlement_xml_name;

        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false
        };

        // string tFile = AssetDatabase.GenerateUniqueAssetPath(path);
        string tFile = path;

        XmlWriter BNXmlWriter = XmlWriter.Create(tFile, xmlWriterSettings);
        BNXmlWriter.WriteStartDocument();
        BNXmlWriter.WriteStartElement("Settlements");

        foreach (Settlement settl in exported_Mod.modFilesData.settlementsData.settlements)
        {
            BNXmlWriter.WriteStartElement("Settlement");

            CheckAndWriteAttribute(BNXmlWriter, "id", settl.id);
            CheckAndWriteAttribute(BNXmlWriter, "name", GetFullNameToWrite(settl.settlementName));

            CheckAndWriteAttribute(BNXmlWriter, "culture", settl.culture);
            CheckAndWriteAttribute(BNXmlWriter, "owner", settl.owner);
            CheckAndWriteAttribute(BNXmlWriter, "prosperity", settl.prosperity);
            CheckAndWriteAttribute(BNXmlWriter, "posX", settl.posX);
            CheckAndWriteAttribute(BNXmlWriter, "posY", settl.posY);
            CheckAndWriteAttribute(BNXmlWriter, "gate_posX", settl.gate_posX);
            CheckAndWriteAttribute(BNXmlWriter, "gate_posY", settl.gate_posY);

            CheckAndWriteAttribute(BNXmlWriter, "text", settl.text);

            // BNXmlWriter.WriteFullEndElement();

            WriteSettlementComponents(BNXmlWriter, settl);

            if (settl.AREA_name.Length != 0)
            {
                BNXmlWriter.WriteStartElement("CommonAreas");

                int i = 0;

                foreach (var area in settl.AREA_name)
                {
                    if (settl.AREA_name[i] != "")
                    {
                        BNXmlWriter.WriteStartElement("Area");
                        CheckAndWriteAttribute(BNXmlWriter, "type", settl.AREA_type[i]);
                        CheckAndWriteAttribute(BNXmlWriter, "name", settl.AREA_name[i]);
                        BNXmlWriter.WriteEndElement();

                    }
                    i++;
                }
                BNXmlWriter.WriteEndElement();
            }


            BNXmlWriter.WriteFullEndElement();
        }

        BNXmlWriter.WriteEndElement();
        BNXmlWriter.WriteEndDocument();
        BNXmlWriter.Flush();
        BNXmlWriter.Close();

        // UnityEditor.AssetDatabase.Refresh();


    }

    private static void WriteSettlementComponents(XmlWriter BNXmlWriter, Settlement settlement)
    {
        BNXmlWriter.WriteStartElement("Components");

        if (settlement.isTown)
        {
            BNXmlWriter.WriteStartElement("Town");

            CheckAndWriteAttribute(BNXmlWriter, "id", settlement.CMP_id);
            CheckAndWriteAttribute(BNXmlWriter, "is_castle", "false");

            // CheckAndWriteAttribute(BNXmlWriter, "villageType", settlement.CMP_villageType);
            // CheckAndWriteAttribute(BNXmlWriter, "hearth", settlement.CMP_hearth);
            // CheckAndWriteAttribute(BNXmlWriter, "trade_bound", settlement.CMP_trade_bound);
            // CheckAndWriteAttribute(BNXmlWriter, "bound", settlement.CMP_bound);
            CheckAndWriteAttribute(BNXmlWriter, "level", settlement.CMP_Level);

            CheckAndWriteAttribute(BNXmlWriter, "background_crop_position", settlement.CMP_background_crop_position);
            CheckAndWriteAttribute(BNXmlWriter, "background_mesh", settlement.CMP_background_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "wait_mesh", settlement.CMP_wait_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "castle_background_mesh", settlement.CMP_castle_background_mesh);

            CheckAndWriteAttribute(BNXmlWriter, "map_icon", settlement.CMP_map_icon);
            CheckAndWriteAttribute(BNXmlWriter, "scene_name", settlement.CMP_scene_name);
            CheckAndWriteAttribute(BNXmlWriter, "gate_rotation", settlement.CMP_gate_rotation);

            BNXmlWriter.WriteEndElement();

        }
        else if (settlement.isCastle)
        {
            BNXmlWriter.WriteStartElement("Town");

            CheckAndWriteAttribute(BNXmlWriter, "id", settlement.CMP_id);

            CheckAndWriteAttribute(BNXmlWriter, "is_castle", "true");

            // CheckAndWriteAttribute(BNXmlWriter, "villageType", settlement.CMP_villageType);
            // CheckAndWriteAttribute(BNXmlWriter, "hearth", settlement.CMP_hearth);
            // CheckAndWriteAttribute(BNXmlWriter, "trade_bound", settlement.CMP_trade_bound);
            // CheckAndWriteAttribute(BNXmlWriter, "bound", settlement.CMP_bound);
            CheckAndWriteAttribute(BNXmlWriter, "level", settlement.CMP_Level);
            CheckAndWriteAttribute(BNXmlWriter, "background_crop_position", settlement.CMP_background_crop_position);
            CheckAndWriteAttribute(BNXmlWriter, "background_mesh", settlement.CMP_background_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "wait_mesh", settlement.CMP_wait_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "castle_background_mesh", settlement.CMP_castle_background_mesh);

            CheckAndWriteAttribute(BNXmlWriter, "map_icon", settlement.CMP_map_icon);
            CheckAndWriteAttribute(BNXmlWriter, "scene_name", settlement.CMP_scene_name);
            CheckAndWriteAttribute(BNXmlWriter, "gate_rotation", settlement.CMP_gate_rotation);

            BNXmlWriter.WriteEndElement();

        }
        else if (settlement.isVillage)
        {
            BNXmlWriter.WriteStartElement("Village");

            CheckAndWriteAttribute(BNXmlWriter, "id", settlement.CMP_id);

            CheckAndWriteAttribute(BNXmlWriter, "village_type", settlement.CMP_villageType);
            CheckAndWriteAttribute(BNXmlWriter, "hearth", settlement.CMP_hearth);
            CheckAndWriteAttribute(BNXmlWriter, "trade_bound", settlement.CMP_trade_bound);
            CheckAndWriteAttribute(BNXmlWriter, "bound", settlement.CMP_bound);
            CheckAndWriteAttribute(BNXmlWriter, "background_crop_position", settlement.CMP_background_crop_position);
            CheckAndWriteAttribute(BNXmlWriter, "background_mesh", settlement.CMP_background_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "wait_mesh", settlement.CMP_wait_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "castle_background_mesh", settlement.CMP_castle_background_mesh);

            CheckAndWriteAttribute(BNXmlWriter, "map_icon", settlement.CMP_map_icon);
            CheckAndWriteAttribute(BNXmlWriter, "scene_name", settlement.CMP_scene_name);
            CheckAndWriteAttribute(BNXmlWriter, "gate_rotation", settlement.CMP_gate_rotation);

            BNXmlWriter.WriteEndElement();

        }
        else if (settlement.isHideout)
        {
            BNXmlWriter.WriteStartElement("Hideout");

            CheckAndWriteAttribute(BNXmlWriter, "id", settlement.CMP_id);

            //CheckAndWriteAttribute(BNXmlWriter, "village_type", settlement.CMP_villageType);
            //CheckAndWriteAttribute(BNXmlWriter, "hearth", settlement.CMP_hearth);
            //CheckAndWriteAttribute(BNXmlWriter, "trade_bound", settlement.CMP_trade_bound);
            //CheckAndWriteAttribute(BNXmlWriter, "bound", settlement.CMP_bound);
            CheckAndWriteAttribute(BNXmlWriter, "background_crop_position", settlement.CMP_background_crop_position);
            CheckAndWriteAttribute(BNXmlWriter, "background_mesh", settlement.CMP_background_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "wait_mesh", settlement.CMP_wait_mesh);
            CheckAndWriteAttribute(BNXmlWriter, "castle_background_mesh", settlement.CMP_castle_background_mesh);

            CheckAndWriteAttribute(BNXmlWriter, "map_icon", settlement.CMP_map_icon);
            CheckAndWriteAttribute(BNXmlWriter, "scene_name", settlement.CMP_scene_name);
            CheckAndWriteAttribute(BNXmlWriter, "gate_rotation", settlement.CMP_gate_rotation);

            CheckAndWriteAttribute(BNXmlWriter, "type", "Hideout");


            BNXmlWriter.WriteEndElement();
        }

        BNXmlWriter.WriteFullEndElement();

        if (!settlement.isHideout)
        {
            BNXmlWriter.WriteStartElement("Locations");
            CheckAndWriteAttribute(BNXmlWriter, "complex_template", settlement.LOC_complex_template);

            int i = 0;
            foreach (var loc in settlement.LOC_id)
            {
                BNXmlWriter.WriteStartElement("Location");

                if (settlement.isVillage)
                {
                    CheckAndWriteAttribute(BNXmlWriter, "id", settlement.LOC_id[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "scene_name", settlement.LOC_scn[i]);

                    // temp
                    if (settlement.LOC_max_prosperity.Length >= i)
                    {
                        CheckZeroWriteAttribute(BNXmlWriter, "max_prosperity", settlement.LOC_max_prosperity[i]);
                    }
                }
                else
                {
                    CheckAndWriteAttribute(BNXmlWriter, "id", settlement.LOC_id[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "scene_name", settlement.LOC_scn[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "scene_name_1", settlement.LOC_scn_1[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "scene_name_2", settlement.LOC_scn_2[i]);
                    CheckAndWriteAttribute(BNXmlWriter, "scene_name_3", settlement.LOC_scn_3[i]);
                }


                BNXmlWriter.WriteEndElement();

                i++;
            }

            BNXmlWriter.WriteFullEndElement();

        }

    }


    private static void CheckAndWriteAttribute(XmlWriter BNXmlWriter, string attributeName, string attributeValue)
    {
        if (attributeValue != "" && attributeValue != "none")
        {
            BNXmlWriter.WriteAttributeString(attributeName, attributeValue);
        }
    }
    private static void CheckZeroWriteAttribute(XmlWriter BNXmlWriter, string attributeName, string attributeValue)
    {
        if (attributeValue != "" && attributeValue != "none" & attributeValue != "0")
        {
            BNXmlWriter.WriteAttributeString(attributeName, attributeValue);
        }
    }
    private static void WriteNamesNodes(XmlWriter BNXmlWriter, string blockName, string[] namesContainer)
    {
        if (namesContainer != null && namesContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in namesContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("name");
                    BNXmlWriter.WriteAttributeString("name", namesContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }
    private static void WriteNPCTemplatesNodes(XmlWriter BNXmlWriter, string blockName, string[] tournamentContainer)
    {
        if (tournamentContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in tournamentContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("template");
                    BNXmlWriter.WriteAttributeString("name", tournamentContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }


    private static void WriteVassalRewards(XmlWriter BNXmlWriter, string blockName, string[] itemsContainer)
    {
        if (itemsContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in itemsContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("item");
                    BNXmlWriter.WriteAttributeString("id", itemsContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }

    private static void WriteHairBodyTags(XmlWriter BNXmlWriter, string blockName, string[] itemsContainer)
    {
        if (itemsContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in itemsContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("hair_tag");
                    BNXmlWriter.WriteAttributeString("name", itemsContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }

    private static void WriteBeardBodyTags(XmlWriter BNXmlWriter, string blockName, string[] itemsContainer)
    {
        if (itemsContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in itemsContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("beard_tag");
                    BNXmlWriter.WriteAttributeString("name", itemsContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }

    private static void WriteCulturalFeats(XmlWriter BNXmlWriter, string blockName, string[] dataContainer)
    {
        if (dataContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in dataContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("feat");
                    BNXmlWriter.WriteAttributeString("id", dataContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }

    private static void WriteMinorFactiontemplates(XmlWriter BNXmlWriter, string blockName, string[] dataContainer)
    {
        if (dataContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in dataContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("template");
                    BNXmlWriter.WriteAttributeString("id", dataContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

    }

    private static void WritePossibleClanIcons(XmlWriter BNXmlWriter, string blockName, string[] dataContainer)
    {
        if (dataContainer.Length != 0)
        {
            BNXmlWriter.WriteStartElement(blockName);

            int i = 0;
            foreach (var name_value in dataContainer)
            {
                // check if none name
                if (name_value != "")
                {
                    BNXmlWriter.WriteStartElement("icon");
                    BNXmlWriter.WriteAttributeString("id", dataContainer[i]);
                    BNXmlWriter.WriteEndElement();
                }
                i++;
            }

            BNXmlWriter.WriteEndElement();
        }

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
