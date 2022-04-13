using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using UnityEngine.EventSystems;

[System.Serializable]
public class ModFilesManager : Editor
{

    public string modsSettingsPath = "Assets/Resources/SubModulesData/";
    public string modsResourcesPath = "Assets/Resources/Data/";
    public string settingsAsset = "Assets/Settings/BDT_settings.asset";
    [SerializeField]
    public ModuleReceiver module;
    [SerializeField]
    public ModFiles asset;
    [SerializeField]
    Dictionary<string, string> configs;

    Dictionary<Faction, string> fac_Assets;
    Dictionary<Culture, string> cult_Assets;
    Dictionary<Hero, string> hero_Assets;
    Dictionary<Kingdom, string> kingd_Assets;
    Dictionary<NPCCharacter, string> npc_Assets;
    Dictionary<Settlement, string> settl_Assets;
    Dictionary<Item, string> item_Assets;
    Dictionary<PartyTemplate, string> PT_Assets;
    Dictionary<TranslationString, string> TS_Assets;
    Dictionary<TranslationString, string> TS_lang_Assets;
    Dictionary<ModLanguage, string> lang_Assets;
    Dictionary<EquipmentSet, string> eqp_Roster_Assets;
    Dictionary<EquipmentSet, string> eqp_Main_Assets;
    Dictionary<EquipmentSet, string> equipmentSets;
    Dictionary<Equipment, string> equipments;

    public void CreateLoadDictionaries()
    {
        fac_Assets = new Dictionary<Faction, string>();
        cult_Assets = new Dictionary<Culture, string>();
        hero_Assets = new Dictionary<Hero, string>();
        kingd_Assets = new Dictionary<Kingdom, string>();
        npc_Assets = new Dictionary<NPCCharacter, string>();
        settl_Assets = new Dictionary<Settlement, string>();
        item_Assets = new Dictionary<Item, string>();
        PT_Assets = new Dictionary<PartyTemplate, string>();
        TS_Assets = new Dictionary<TranslationString, string>();
        TS_lang_Assets = new Dictionary<TranslationString, string>();
        lang_Assets = new Dictionary<ModLanguage, string>();
        eqp_Roster_Assets = new Dictionary<EquipmentSet, string>();
        eqp_Main_Assets = new Dictionary<EquipmentSet, string>();
        equipments = new Dictionary<Equipment, string>();
        equipmentSets = new Dictionary<EquipmentSet, string>();

    }

    public void CreateModSettings(ref bool imp_cult, ref bool imp_kgd, ref bool imp_fac, ref bool imp_hero, ref bool imp_npc, ref bool imp_item, ref bool imp_eqp, ref bool imp_pt, ref bool imp_settl)
    {
        if (module != null)
        {

            configs = new Dictionary<string, string>();
            asset = ScriptableObject.CreateInstance<ModFiles>();
            asset.mod = module;
            asset.BNResourcesPath = module.path + "/ModuleData";

            asset.modSettingsPath = modsSettingsPath + module.id;
            asset.modResourcesPath = modsResourcesPath + module.id;

            module.modFilesData = asset;

            CreateDirectories(asset);

            AssetDatabase.CreateAsset(asset, asset.modSettingsPath + "/" + module.id + "_Config.asset");
            //AssetDatabase.SaveAssets();

            // Kingdoms Data List
            KingdomsData kingdomConfig = ScriptableObject.CreateInstance<KingdomsData>();
            string ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "kng_" + module.id + ".asset";

            if (kingdomConfig.kingdoms == null)
            {
                kingdomConfig.kingdoms = new List<Kingdom>();
                asset.kingdomsData = kingdomConfig;
            }

            AssetDatabase.CreateAsset(kingdomConfig, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            configs.Add("kingdoms", ModDataConfigsPath);

            // Factions Data List
            FactionsData factionsConfig = ScriptableObject.CreateInstance<FactionsData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "fac_" + module.id + ".asset";

            if (factionsConfig.factions == null)
            {
                factionsConfig.factions = new List<Faction>();
                asset.factionsData = factionsConfig;
            }

            AssetDatabase.CreateAsset(factionsConfig, ModDataConfigsPath);
            //  AssetDatabase.SaveAssets();

            configs.Add("factions", ModDataConfigsPath);

            // Settlements Data List
            SettlementsData settlementsConfig = ScriptableObject.CreateInstance<SettlementsData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "sett_" + module.id + ".asset";

            if (settlementsConfig.settlements == null)
            {
                settlementsConfig.settlements = new List<Settlement>();
                asset.settlementsData = settlementsConfig;
            }

            AssetDatabase.CreateAsset(settlementsConfig, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            configs.Add("settlements", ModDataConfigsPath);

            // NPC Data List
            NPCCharactersData npcChrConfig = ScriptableObject.CreateInstance<NPCCharactersData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "npc_" + module.id + ".asset";

            if (npcChrConfig.NPCCharacters == null)
            {
                npcChrConfig.NPCCharacters = new List<NPCCharacter>();
                asset.npcChrData = npcChrConfig;
            }

            AssetDatabase.CreateAsset(npcChrConfig, ModDataConfigsPath);
            //  AssetDatabase.SaveAssets();

            configs.Add("NPCCharacters", ModDataConfigsPath);

            // Cultures Data List
            CulturesData culturesConfig = ScriptableObject.CreateInstance<CulturesData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "cult_" + module.id + ".asset";

            if (culturesConfig.cultures == null)
            {
                culturesConfig.cultures = new List<Culture>();
                asset.culturesData = culturesConfig;
            }

            AssetDatabase.CreateAsset(culturesConfig, ModDataConfigsPath);
            //   AssetDatabase.SaveAssets();

            configs.Add("cultures", ModDataConfigsPath);

            // PartyTemplates Data List
            PartyTemplatesData PTconfig = ScriptableObject.CreateInstance<PartyTemplatesData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "PT_" + module.id + ".asset";

            if (PTconfig.partyTemplates == null)
            {
                PTconfig.partyTemplates = new List<PartyTemplate>();
                asset.PTdata = PTconfig;
            }

            AssetDatabase.CreateAsset(PTconfig, ModDataConfigsPath);
            //   AssetDatabase.SaveAssets();

            configs.Add("partyTemplates", ModDataConfigsPath);

            // Heroes Data List
            HeroesData heroesConfig = ScriptableObject.CreateInstance<HeroesData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "hero_" + module.id + ".asset";

            if (heroesConfig.heroes == null)
            {
                heroesConfig.heroes = new List<Hero>();
                asset.heroesData = heroesConfig;
            }

            AssetDatabase.CreateAsset(heroesConfig, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            configs.Add("heroes", ModDataConfigsPath);

            // Translation Data List
            TranslationData translationConfig = ScriptableObject.CreateInstance<TranslationData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "translation_" + module.id + ".asset";

            if (translationConfig.translationStrings == null)
            {
                translationConfig.translationStrings = new List<TranslationString>();
                asset.translationData = translationConfig;
            }

            AssetDatabase.CreateAsset(translationConfig, ModDataConfigsPath);
            //AssetDatabase.SaveAssets();

            configs.Add("translationStrings", ModDataConfigsPath);

            // Languages Data List
            LanguagesData languagesConfig = ScriptableObject.CreateInstance<LanguagesData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "language_" + module.id + ".asset";

            if (languagesConfig.languages == null)
            {
                languagesConfig.languages = new List<ModLanguage>();
                asset.languagesData = languagesConfig;
            }

            AssetDatabase.CreateAsset(languagesConfig, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            configs.Add("languages", ModDataConfigsPath);

            // Items Data List
            ItemsData itemsConfig = ScriptableObject.CreateInstance<ItemsData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "item_" + module.id + ".asset";

            if (itemsConfig.items == null)
            {
                itemsConfig.items = new List<Item>();
                asset.itemsData = itemsConfig;
            }

            AssetDatabase.CreateAsset(itemsConfig, ModDataConfigsPath);
            //  AssetDatabase.SaveAssets();

            configs.Add("items", ModDataConfigsPath);

            // Sets Data List
            EquipmentSetData eqpSetConfig = ScriptableObject.CreateInstance<EquipmentSetData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "equipment_set_" + module.id + ".asset";

            if (eqpSetConfig.equipmentSets == null)
            {
                eqpSetConfig.equipmentSets = new List<EquipmentSet>();
                asset.equipmentSetData = eqpSetConfig;
            }

            AssetDatabase.CreateAsset(eqpSetConfig, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            configs.Add("equipmentSets", ModDataConfigsPath);
            // Sets Data List
            EquipmentData eqpConfig = ScriptableObject.CreateInstance<EquipmentData>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "equipments_" + module.id + ".asset";

            if (eqpConfig.equipmentSets == null)
            {
                eqpConfig.equipmentSets = new List<Equipment>();
                asset.equipmentsData = eqpConfig;
            }

            AssetDatabase.CreateAsset(eqpConfig, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            configs.Add("equipments", ModDataConfigsPath);

            // ExportConfigs
            ExportSettings exportConfigs = ScriptableObject.CreateInstance<ExportSettings>();
            ModDataConfigsPath = asset.modSettingsPath + "/ModDataConfigs/" + "export_" + module.id + ".asset";

            AssetDatabase.CreateAsset(exportConfigs, ModDataConfigsPath);
            // AssetDatabase.SaveAssets();

            asset.exportSettings = exportConfigs;
            configs.Add("export", ModDataConfigsPath);


            ReadModuleXml(asset, ref imp_cult, ref imp_kgd, ref imp_fac, ref imp_hero, ref imp_npc, ref imp_item, ref imp_eqp, ref imp_pt, ref imp_settl);


        }
    }

    static void CreateDirectories(ModFiles modFilesAsset)
    {
        if (!Directory.Exists(modFilesAsset.modSettingsPath))
        {
            Directory.CreateDirectory(modFilesAsset.modSettingsPath);
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath);
        }

        if (!Directory.Exists(modFilesAsset.modSettingsPath + "/ModDataConfigs"))
        {
            Directory.CreateDirectory(modFilesAsset.modSettingsPath + "/ModDataConfigs");
        }

        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Kingdoms"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Kingdoms");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Factions"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Factions");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Settlements"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Settlements");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/NPC"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/NPC");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/NPC/Equipment/EquipmentRosters");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/NPC/Equipment/EquipmentMain");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Cultures"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Cultures");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/PartyTemplates"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/PartyTemplates");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Heroes"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Heroes");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Items"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Items");
        }


        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/_Templates"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/_Templates/NPCtemplates");
        }
        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/Sets"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Sets/Equipments");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Sets/Equipments/EqpSets");

            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/Sets/SkillSets");
        }

        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/TranslationData"))
        {
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/FactionsTranslationData");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/KingdomsTranslationData");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/NPCTranslationData");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/HeroesTranslationData");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/CulturesTranslationData");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/SettlementsTranslationData");
            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/ItemsTranslationData");
            // Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/---TranslationData");
        }
        // if (!Directory.Exists(modFilesAsset.modResourcesPath + "/TranslationData"))
        // {

        // }

    }

    public void CreateKingdomAssets(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Kingdom kingdomAsset = ScriptableObject.CreateInstance<Kingdom>();

        kingdomAsset.moduleID = modFilesAsset.mod.id;

        kingdomAsset.id = node.Attributes["id"].Value;

        if (node.Attributes["name"] != null)
        {
            // faction name & translationString tag
            CreateTSData(node, modFilesAsset, "name", "KingdomsTranslationData", TS_Assets, "Kingdom");
            kingdomAsset.kingdomName = node.Attributes["name"].Value;
        }


        if (node.Attributes["owner"] != null)
        {
            kingdomAsset.owner = node.Attributes["owner"].Value;
        }
        if (node.Attributes["banner_key"] != null)
        {
            kingdomAsset.banner_key = node.Attributes["banner_key"].Value;
        }
        if (node.Attributes["primary_banner_color"] != null)
        {
            kingdomAsset.primary_banner_color = node.Attributes["primary_banner_color"].Value;
        }
        if (node.Attributes["secondary_banner_color"] != null)
        {
            kingdomAsset.secondary_banner_color = node.Attributes["secondary_banner_color"].Value;
        }
        if (node.Attributes["label_color"] != null)
        {
            kingdomAsset.label_color = node.Attributes["label_color"].Value;
        }
        if (node.Attributes["color"] != null)
        {
            kingdomAsset.color = node.Attributes["color"].Value;
        }
        if (node.Attributes["color2"] != null)
        {
            kingdomAsset.color2 = node.Attributes["color2"].Value;
        }
        if (node.Attributes["alternative_color"] != null)
        {
            kingdomAsset.alternative_color = node.Attributes["alternative_color"].Value;
        }
        if (node.Attributes["alternative_color2"] != null)
        {
            kingdomAsset.alternative_color2 = node.Attributes["alternative_color2"].Value;
        }
        if (node.Attributes["culture"] != null)
        {
            kingdomAsset.culture = node.Attributes["culture"].Value;
        }
        if (node.Attributes["settlement_banner_mesh"] != null)
        {
            kingdomAsset.settlement_banner_mesh = node.Attributes["settlement_banner_mesh"].Value;
        }
        if (node.Attributes["flag_mesh"] != null)
        {
            kingdomAsset.flag_mesh = node.Attributes["flag_mesh"].Value;
        }
        if (node.Attributes["short_name"] != null)
        {
            // kingdom shortName & translationString tag
            CreateTSData(node, modFilesAsset, "short_name", "KingdomsTranslationData", TS_Assets, "Kingdom");
            kingdomAsset.short_name = node.Attributes["short_name"].Value;
        }
        if (node.Attributes["title"] != null)
        {
            // kingdom title & translationString tag
            CreateTSData(node, modFilesAsset, "title", "KingdomsTranslationData", TS_Assets, "Kingdom");
            kingdomAsset.title = node.Attributes["title"].Value;
        }
        if (node.Attributes["ruler_title"] != null)
        {
            // kingdom ruler title & translationString tag
            CreateTSData(node, modFilesAsset, "ruler_title", "KingdomsTranslationData", TS_Assets, "Kingdom");
            kingdomAsset.ruler_title = node.Attributes["ruler_title"].Value;
        }
        if (node.Attributes["text"] != null)
        {
            // kingdom text & translationString tag
            CreateTSData(node, modFilesAsset, "text", "KingdomsTranslationData", TS_Assets, "Kingdom");
            kingdomAsset.text = node.Attributes["text"].Value;
        }

        // RELATIONSHIPS

        foreach (XmlNode relNode in node.ChildNodes)
        {
            if (relNode.LocalName == "relationships")
            {
                int i = 0;
                kingdomAsset.relationships = new string[relNode.ChildNodes.Count];
                kingdomAsset.relationValues = new string[relNode.ChildNodes.Count];
                kingdomAsset.relationsAtWar = new string[relNode.ChildNodes.Count];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.Attributes["kingdom"] != null)
                    {
                        kingdomAsset.relationships[i] = childRelation.Attributes["kingdom"].Value;
                    }
                    if (childRelation.Attributes["value"] != null)
                    {
                        kingdomAsset.relationValues[i] = childRelation.Attributes["value"].Value;
                    }
                    if (childRelation.Attributes["isAtWar"] != null)
                    {
                        kingdomAsset.relationsAtWar[i] = childRelation.Attributes["isAtWar"].Value;
                    }

                    i++;
                }
            }

            // POLICIES
            if (relNode.LocalName == "policies")
            {
                int i = 0;
                kingdomAsset.policies = new string[relNode.ChildNodes.Count];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.Attributes["id"] != null)
                    {
                        kingdomAsset.policies[i] = childRelation.Attributes["id"].Value;
                    }
                    i++;
                }
            }



        }




        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        kingdomAsset.XmlFileName = xmlFileName;

        // modFilesAsset.kingdomsData.kingdoms.Add(kingdomAsset);

        path = modFilesAsset.modResourcesPath + "/Kingdoms/" + node.Attributes["id"].Value + ".asset";

        kingd_Assets.Add(kingdomAsset, path);
        // AssetDatabase.CreateAsset(kingdomAsset, path);
        // AssetDatabase.SaveAssets();
    }

    public void CreateFactionAssets(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Faction factionAsset = ScriptableObject.CreateInstance<Faction>();

        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);

        factionAsset.moduleID = modFilesAsset.mod.id;

        factionAsset.id = node.Attributes["id"].Value;

        if (node.Attributes["name"] != null)
        {
            // faction name & translationString tag
            CreateTSData(node, modFilesAsset, "name", "FactionsTranslationData", TS_Assets, "Faction");
            factionAsset.factionName = node.Attributes["name"].Value;
        }


        if (node.Attributes["super_faction"] != null)
        {
            factionAsset.super_faction = node.Attributes["super_faction"].Value;
        }
        if (node.Attributes["owner"] != null)
        {
            factionAsset.owner = node.Attributes["owner"].Value;
        }
        if (node.Attributes["banner_key"] != null)
        {

            factionAsset.banner_key = node.Attributes["banner_key"].Value;

            var bKey = factionAsset.banner_key;
            string[] characters = new string[bKey.Length];
            var layerIterCount = 0;

            var BSpath = "Assets/Settings/BannersEditor/BannerEditor_settings.asset";
            BannerEditorSettings bannerSettings = (BannerEditorSettings)AssetDatabase.LoadAssetAtPath(BSpath, typeof(BannerEditorSettings));

            for (int i = 0; i < bKey.Length; i++)
            {
                characters[i] = System.Convert.ToString(bKey[i]);

                // var layer = layerList[layerList.Count - 1];
                // Debug.Log(layer.name);

                if (characters[i] == ".")
                {
                    layerIterCount++;
                }
                else
                {
                    switch (layerIterCount)
                    {
                        case 1:
                            factionAsset._BannerColorA = factionAsset._BannerColorA + characters[i];
                            break;
                        case 2:
                            factionAsset._BannerColorB = factionAsset._BannerColorB + characters[i];
                            break;
                            // default:
                            //     Debug.LogError("Banner Key Import Error");
                            //     break;
                    }
                }

                if (layerIterCount == 3)
                {
                    break;
                }
            }

            var col_ID = 0;
            foreach (var colorID in bannerSettings.color_IDs)
            {

                if (colorID == factionAsset._BannerColorB)
                {
                    factionAsset._BannerColorB = bannerSettings.colors[col_ID];
                }

                if (colorID == factionAsset._BannerColorA)
                {
                    factionAsset._BannerColorA = bannerSettings.colors[col_ID];
                }

                col_ID++;
            }
        }

        if (node.Attributes["is_minor_faction"] != null)
        {
            factionAsset.is_minor_faction = node.Attributes["is_minor_faction"].Value;
        }
        if (node.Attributes["is_outlaw"] != null)
        {
            factionAsset.is_outlaw = node.Attributes["is_outlaw"].Value;
        }
        if (node.Attributes["is_mafia"] != null)
        {
            factionAsset.is_mafia = node.Attributes["is_mafia"].Value;
        }
        if (node.Attributes["is_bandit"] != null)
        {
            factionAsset.is_bandit = node.Attributes["is_bandit"].Value;
        }
        if (node.Attributes["label_color"] != null)
        {
            factionAsset.label_color = node.Attributes["label_color"].Value;
        }
        if (node.Attributes["color"] != null)
        {
            factionAsset.color = node.Attributes["color"].Value;
        }
        if (node.Attributes["color2"] != null)
        {
            factionAsset.color2 = node.Attributes["color2"].Value;
        }
        if (node.Attributes["alternative_color"] != null)
        {
            factionAsset.alternative_color = node.Attributes["alternative_color"].Value;
        }
        if (node.Attributes["alternative_color2"] != null)
        {
            factionAsset.alternative_color2 = node.Attributes["alternative_color2"].Value;
        }
        if (node.Attributes["culture"] != null)
        {
            factionAsset.culture = node.Attributes["culture"].Value;
        }
        if (node.Attributes["settlement_banner_mesh"] != null)
        {
            factionAsset.settlement_banner_mesh = node.Attributes["settlement_banner_mesh"].Value;
        }
        if (node.Attributes["tier"] != null)
        {
            factionAsset.tier = node.Attributes["tier"].Value;
        }

        // party templates

        if (node.Attributes["default_party_template"] != null)
        {
            factionAsset.default_party_template = node.Attributes["default_party_template"].Value;
        }

        // Spawn coords
        if (node.Attributes["initial_posX"] != null)
        {
            factionAsset.initial_posX = node.Attributes["initial_posX"].Value;
        }
        if (node.Attributes["initial_posY"] != null)
        {
            factionAsset.initial_posY = node.Attributes["initial_posY"].Value;
        }

        // text description

        if (node.Attributes["text"] != null)
        {
            CreateTSData(node, modFilesAsset, "text", "FactionsTranslationData", TS_Assets, "Faction");

            factionAsset.text = node.Attributes["text"].Value;
        }

        // Update 1.7.2
        if (node.Attributes["short_name"] != null)
        {
            CreateTSData(node, modFilesAsset, "short_name", "FactionsTranslationData", TS_Assets, "Faction");

            factionAsset.short_name = node.Attributes["short_name"].Value;
        }
        if (node.Attributes["is_clan_type_mercenary"] != null)
        {
            factionAsset.is_clan_type_mercenary = node.Attributes["is_clan_type_mercenary"].Value;
        }
        if (node.Attributes["is_sect"] != null)
        {
            factionAsset.is_sect = node.Attributes["is_sect"].Value;
        }
        if (node.Attributes["is_nomad"] != null)
        {
            factionAsset.is_nomad = node.Attributes["is_nomad"].Value;
        }

        foreach (XmlNode relNode in node.ChildNodes)
        {
            if (relNode.LocalName == "minor_faction_character_templates")
            {
                int i = 0;
                factionAsset.minor_faction_character_templates = new string[relNode.ChildNodes.Count];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.Attributes["id"] != null)
                    {
                        factionAsset.minor_faction_character_templates[i] = childRelation.Attributes["id"].Value;
                    }
                    i++;
                }
            }
        }


        factionAsset.XmlFileName = xmlFileName;

        // modFilesAsset.factionsData.factions.Add(factionAsset);

        path = modFilesAsset.modResourcesPath + "/Factions/" + node.Attributes["id"].Value + ".asset";

        fac_Assets.Add(factionAsset, path);
        // AssetDatabase.CreateAsset(factionAsset, path);
        // AssetDatabase.SaveAssets();
    }

    public void CreateSettlementAssets(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Settlement settlementAsset = ScriptableObject.CreateInstance<Settlement>();

        settlementAsset.moduleID = modFilesAsset.mod.id;



        if (node.Attributes["id"] != null)
        {
            settlementAsset.id = node.Attributes["id"].Value;
        }

        if (node.Attributes["name"] != null)
        {
            // Settlement name & translationString tag
            CreateTSData(node, modFilesAsset, "name", "SettlementsTranslationData", TS_Assets, "Settlement");
            settlementAsset.settlementName = node.Attributes["name"].Value;
        }

        if (node.Attributes["owner"] != null)
        {
            settlementAsset.owner = node.Attributes["owner"].Value;
        }
        if (node.Attributes["culture"] != null)
        {
            settlementAsset.culture = node.Attributes["culture"].Value;
        }
        if (node.Attributes["type"] != null)
        {
            if (node.Attributes["type"].Value == "Hideout")
            {
                settlementAsset.isHideout = true;
            }

            settlementAsset.type = node.Attributes["type"].Value;
        }
        if (node.Attributes["prosperity"] != null)
        {
            settlementAsset.prosperity = node.Attributes["prosperity"].Value;
        }
        if (node.Attributes["posX"] != null)
        {
            settlementAsset.posX = node.Attributes["posX"].Value;
        }
        if (node.Attributes["posY"] != null)
        {
            settlementAsset.posY = node.Attributes["posY"].Value;
        }
        if (node.Attributes["gate_posX"] != null)
        {
            settlementAsset.gate_posX = node.Attributes["gate_posX"].Value;
        }
        if (node.Attributes["gate_posY"] != null)
        {
            settlementAsset.gate_posY = node.Attributes["gate_posY"].Value;
        }
        // text description

        if (node.Attributes["text"] != null)
        {

            // settlement text & translationString tag
            CreateTSData(node, modFilesAsset, "text", "SettlementsTranslationData", TS_Assets, "Settlement");
            settlementAsset.text = node.Attributes["text"].Value;
        }

        foreach (XmlNode relNode in node.ChildNodes)
        {

            // COMPONENT
            if (relNode.LocalName == "Components")
            {

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "Town" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["is_castle"] == null || childRelation.Attributes["is_castle"].Value == "false")
                        {
                            settlementAsset.isTown = true;

                            if (childRelation.Attributes["id"] != null)
                            {
                                settlementAsset.CMP_id = childRelation.Attributes["id"].Value;
                            }
                            if (childRelation.Attributes["village_type"] != null)
                            {
                                settlementAsset.CMP_villageType = childRelation.Attributes["village_type"].Value;
                            }
                            if (childRelation.Attributes["hearth"] != null)
                            {
                                settlementAsset.CMP_hearth = childRelation.Attributes["hearth"].Value;
                            }
                            if (childRelation.Attributes["trade_bound"] != null)
                            {
                                settlementAsset.CMP_trade_bound = childRelation.Attributes["trade_bound"].Value;
                            }
                            if (childRelation.Attributes["bound"] != null)
                            {
                                settlementAsset.CMP_bound = childRelation.Attributes["bound"].Value;
                            }
                            if (childRelation.Attributes["background_crop_position"] != null)
                            {
                                settlementAsset.CMP_background_crop_position = childRelation.Attributes["background_crop_position"].Value;
                            }
                            if (childRelation.Attributes["background_mesh"] != null)
                            {
                                settlementAsset.CMP_background_mesh = childRelation.Attributes["background_mesh"].Value;
                            }
                            if (childRelation.Attributes["wait_mesh"] != null)
                            {
                                settlementAsset.CMP_wait_mesh = childRelation.Attributes["wait_mesh"].Value;
                            }
                            if (childRelation.Attributes["castle_background_mesh"] != null)
                            {
                                settlementAsset.CMP_castle_background_mesh = childRelation.Attributes["castle_background_mesh"].Value;
                            }
                            if (childRelation.Attributes["scene_name"] != null)
                            {
                                settlementAsset.CMP_scene_name = childRelation.Attributes["scene_name"].Value;
                            }
                            if (childRelation.Attributes["map_icon"] != null)
                            {
                                settlementAsset.CMP_map_icon = childRelation.Attributes["map_icon"].Value;
                            }
                            if (childRelation.Attributes["gate_rotation"] != null)
                            {
                                settlementAsset.CMP_gate_rotation = childRelation.Attributes["gate_rotation"].Value;
                            }
                            if (childRelation.Attributes["level"] != null)
                            {
                                settlementAsset.CMP_Level = childRelation.Attributes["level"].Value;
                            }

                        }
                        else
                        {
                            if (childRelation.Attributes["is_castle"].Value == "true")
                            {
                                settlementAsset.isCastle = true;

                                if (childRelation.Attributes["id"] != null)
                                {
                                    settlementAsset.CMP_id = childRelation.Attributes["id"].Value;
                                }
                                if (childRelation.Attributes["village_type"] != null)
                                {
                                    settlementAsset.CMP_villageType = childRelation.Attributes["village_type"].Value;
                                }
                                if (childRelation.Attributes["hearth"] != null)
                                {
                                    settlementAsset.CMP_hearth = childRelation.Attributes["hearth"].Value;
                                }
                                if (childRelation.Attributes["trade_bound"] != null)
                                {
                                    settlementAsset.CMP_trade_bound = childRelation.Attributes["trade_bound"].Value;
                                }
                                if (childRelation.Attributes["bound"] != null)
                                {
                                    settlementAsset.CMP_bound = childRelation.Attributes["bound"].Value;
                                }
                                if (childRelation.Attributes["background_crop_position"] != null)
                                {
                                    settlementAsset.CMP_background_crop_position = childRelation.Attributes["background_crop_position"].Value;
                                }
                                if (childRelation.Attributes["background_mesh"] != null)
                                {
                                    settlementAsset.CMP_background_mesh = childRelation.Attributes["background_mesh"].Value;
                                }
                                if (childRelation.Attributes["wait_mesh"] != null)
                                {
                                    settlementAsset.CMP_wait_mesh = childRelation.Attributes["wait_mesh"].Value;
                                }
                                if (childRelation.Attributes["castle_background_mesh"] != null)
                                {
                                    settlementAsset.CMP_castle_background_mesh = childRelation.Attributes["castle_background_mesh"].Value;
                                }
                                if (childRelation.Attributes["scene_name"] != null)
                                {
                                    settlementAsset.CMP_scene_name = childRelation.Attributes["scene_name"].Value;
                                }
                                if (childRelation.Attributes["map_icon"] != null)
                                {
                                    settlementAsset.CMP_map_icon = childRelation.Attributes["map_icon"].Value;
                                }
                                if (childRelation.Attributes["gate_rotation"] != null)
                                {
                                    settlementAsset.CMP_gate_rotation = childRelation.Attributes["gate_rotation"].Value;
                                }
                                if (childRelation.Attributes["level"] != null)
                                {
                                    settlementAsset.CMP_Level = childRelation.Attributes["level"].Value;
                                }
                            }
                        }
                    }

                    if (childRelation.LocalName == "Village" && childRelation.Attributes["id"] != null)
                    {
                        settlementAsset.isVillage = true;

                        if (childRelation.Attributes["id"] != null)
                        {
                            settlementAsset.CMP_id = childRelation.Attributes["id"].Value;
                        }
                        if (childRelation.Attributes["village_type"] != null)
                        {
                            settlementAsset.CMP_villageType = childRelation.Attributes["village_type"].Value;
                        }
                        if (childRelation.Attributes["hearth"] != null)
                        {
                            settlementAsset.CMP_hearth = childRelation.Attributes["hearth"].Value;
                        }
                        if (childRelation.Attributes["trade_bound"] != null)
                        {
                            settlementAsset.CMP_trade_bound = childRelation.Attributes["trade_bound"].Value;
                        }
                        if (childRelation.Attributes["bound"] != null)
                        {
                            settlementAsset.CMP_bound = childRelation.Attributes["bound"].Value;
                        }
                        if (childRelation.Attributes["background_crop_position"] != null)
                        {
                            settlementAsset.CMP_background_crop_position = childRelation.Attributes["background_crop_position"].Value;
                        }
                        if (childRelation.Attributes["background_mesh"] != null)
                        {
                            settlementAsset.CMP_background_mesh = childRelation.Attributes["background_mesh"].Value;
                        }
                        if (childRelation.Attributes["wait_mesh"] != null)
                        {
                            settlementAsset.CMP_wait_mesh = childRelation.Attributes["wait_mesh"].Value;
                        }
                        if (childRelation.Attributes["castle_background_mesh"] != null)
                        {
                            settlementAsset.CMP_castle_background_mesh = childRelation.Attributes["castle_background_mesh"].Value;
                        }
                        if (childRelation.Attributes["scene_name"] != null)
                        {
                            settlementAsset.CMP_scene_name = childRelation.Attributes["scene_name"].Value;
                        }
                        if (childRelation.Attributes["map_icon"] != null)
                        {
                            settlementAsset.CMP_map_icon = childRelation.Attributes["map_icon"].Value;
                        }
                        if (childRelation.Attributes["gate_rotation"] != null)
                        {
                            settlementAsset.CMP_gate_rotation = childRelation.Attributes["gate_rotation"].Value;
                        }
                        if (childRelation.Attributes["level"] != null)
                        {
                            settlementAsset.CMP_Level = childRelation.Attributes["level"].Value;
                        }
                    }

                    if (childRelation.LocalName == "Hideout" && childRelation.Attributes["id"] != null)
                    {
                        settlementAsset.isHideout = true;

                        if (childRelation.Attributes["id"] != null)
                        {
                            settlementAsset.CMP_id = childRelation.Attributes["id"].Value;
                        }
                        if (childRelation.Attributes["village_type"] != null)
                        {
                            settlementAsset.CMP_villageType = childRelation.Attributes["village_type"].Value;
                        }
                        if (childRelation.Attributes["hearth"] != null)
                        {
                            settlementAsset.CMP_hearth = childRelation.Attributes["hearth"].Value;
                        }
                        if (childRelation.Attributes["trade_bound"] != null)
                        {
                            settlementAsset.CMP_trade_bound = childRelation.Attributes["trade_bound"].Value;
                        }
                        if (childRelation.Attributes["bound"] != null)
                        {
                            settlementAsset.CMP_bound = childRelation.Attributes["bound"].Value;
                        }
                        if (childRelation.Attributes["background_crop_position"] != null)
                        {
                            settlementAsset.CMP_background_crop_position = childRelation.Attributes["background_crop_position"].Value;
                        }
                        if (childRelation.Attributes["background_mesh"] != null)
                        {
                            settlementAsset.CMP_background_mesh = childRelation.Attributes["background_mesh"].Value;
                        }
                        if (childRelation.Attributes["wait_mesh"] != null)
                        {
                            settlementAsset.CMP_wait_mesh = childRelation.Attributes["wait_mesh"].Value;
                        }
                        if (childRelation.Attributes["castle_background_mesh"] != null)
                        {
                            settlementAsset.CMP_castle_background_mesh = childRelation.Attributes["castle_background_mesh"].Value;
                        }
                        if (childRelation.Attributes["scene_name"] != null)
                        {
                            settlementAsset.CMP_scene_name = childRelation.Attributes["scene_name"].Value;
                        }
                        if (childRelation.Attributes["map_icon"] != null)
                        {
                            settlementAsset.CMP_map_icon = childRelation.Attributes["map_icon"].Value;
                        }
                        if (childRelation.Attributes["gate_rotation"] != null)
                        {
                            settlementAsset.CMP_gate_rotation = childRelation.Attributes["gate_rotation"].Value;
                        }
                    }
                }
            }
            // LOCATIONS
            if (relNode.LocalName == "Locations")
            {

                if (relNode.Attributes["complex_template"] != null)
                {
                    settlementAsset.LOC_complex_template = relNode.Attributes["complex_template"].Value;
                }


                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                settlementAsset.LOC_id = new string[countArray];
                settlementAsset.LOC_scn = new string[countArray];
                settlementAsset.LOC_scn_1 = new string[countArray];
                settlementAsset.LOC_scn_2 = new string[countArray];
                settlementAsset.LOC_scn_3 = new string[countArray];
                settlementAsset.LOC_max_prosperity = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "Location" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            settlementAsset.LOC_id[i] = childRelation.Attributes["id"].Value;
                        }
                        if (childRelation.Attributes["scene_name"] != null)
                        {
                            settlementAsset.LOC_scn[i] = childRelation.Attributes["scene_name"].Value;
                        }
                        if (childRelation.Attributes["max_prosperity"] != null)
                        {
                            settlementAsset.LOC_max_prosperity[i] = childRelation.Attributes["max_prosperity"].Value;
                        }
                        if (childRelation.Attributes["scene_name_1"] != null)
                        {
                            settlementAsset.LOC_scn_1[i] = childRelation.Attributes["scene_name_1"].Value;
                        }
                        if (childRelation.Attributes["scene_name_2"] != null)
                        {
                            settlementAsset.LOC_scn_2[i] = childRelation.Attributes["scene_name_2"].Value;
                        }
                        if (childRelation.Attributes["scene_name_3"] != null)
                        {
                            settlementAsset.LOC_scn_3[i] = childRelation.Attributes["scene_name_3"].Value;
                        }

                        i++;
                    }
                }
            }
            // AREAS
            if (relNode.LocalName == "CommonAreas")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                settlementAsset.AREA_type = new string[countArray];
                settlementAsset.AREA_name = new string[countArray];


                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "Area" && childRelation.Attributes["type"] != null)
                    {
                        if (childRelation.Attributes["type"] != null)
                        {
                            settlementAsset.AREA_type[i] = childRelation.Attributes["type"].Value;
                        }
                        if (childRelation.Attributes["name"] != null)
                        {
                            CreateTSData(childRelation, modFilesAsset, "name", "SettlementsTranslationData", TS_Assets, "Settlement");
                            settlementAsset.AREA_name[i] = childRelation.Attributes["name"].Value;
                        }


                        i++;
                    }
                }
            }
        }

        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        settlementAsset.XmlFileName = xmlFileName;

        // modFilesAsset.settlementsData.settlements.Add(settlementAsset);

        path = modFilesAsset.modResourcesPath + "/Settlements/" + node.Attributes["id"].Value + ".asset";

        settl_Assets.Add(settlementAsset, path);
        // AssetDatabase.CreateAsset(settlementAsset, path);
        // AssetDatabase.SaveAssets();
    }

    public void CreateNPCAsset(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        var equipCount = 0;
        var dicEquips = new Dictionary<int, Dictionary<string, string>>();
        var dicIDs = new List<bool>();

        NPCCharacter npcAsset = ScriptableObject.CreateInstance<NPCCharacter>();

        npcAsset.moduleID = modFilesAsset.mod.id;

        npcAsset.id = node.Attributes["id"].Value;

        if (node.Attributes["name"] != null)
        {
            CreateTSData(node, modFilesAsset, "name", "NPCTranslationData", TS_Assets, "NPC");
            npcAsset.npcName = node.Attributes["name"].Value;
        }

        if (node.Attributes["default_group"] != null)
        {
            npcAsset.default_group = node.Attributes["default_group"].Value;
        }
        if (node.Attributes["is_hero"] != null)
        {
            npcAsset.is_hero = node.Attributes["is_hero"].Value;
        }
        if (node.Attributes["age"] != null)
        {
            npcAsset.age = node.Attributes["age"].Value;
        }
        if (node.Attributes["is_female"] != null)
        {
            npcAsset.is_female = node.Attributes["is_female"].Value;
        }
        if (node.Attributes["culture"] != null)
        {
            npcAsset.culture = node.Attributes["culture"].Value;
        }
        if (node.Attributes["npcName"] != null)
        {
            npcAsset.npcName = node.Attributes["npcName"].Value;
        }
        if (node.Attributes["banner_symbol_mesh_name"] != null)
        {
            npcAsset.banner_symbol_mesh_name = node.Attributes["banner_symbol_mesh_name"].Value;
        }
        if (node.Attributes["occupation"] != null)
        {
            npcAsset.occupation = node.Attributes["occupation"].Value;
        }
        if (node.Attributes["banner_symbol_color"] != null)
        {
            npcAsset.banner_symbol_color = node.Attributes["banner_symbol_color"].Value;
        }
        if (node.Attributes["voice"] != null)
        {
            npcAsset.voice = node.Attributes["voice"].Value;
        }
        if (node.Attributes["is_template"] != null)
        {
            npcAsset.is_template = node.Attributes["is_template"].Value;
        }
        if (node.Attributes["battleTemplate"] != null)
        {
            npcAsset.battleTemplate = node.Attributes["battleTemplate"].Value;
        }
        if (node.Attributes["civilianTemplate"] != null)
        {
            npcAsset.civilianTemplate = node.Attributes["civilianTemplate"].Value;
        }
        if (node.Attributes["is_child_template"] != null)
        {
            npcAsset.is_child_template = node.Attributes["is_child_template"].Value;
        }
        if (node.Attributes["level"] != null)
        {
            npcAsset.level = node.Attributes["level"].Value;
        }
        if (node.Attributes["is_companion"] != null)
        {
            npcAsset.is_companion = node.Attributes["is_companion"].Value;
        }
        if (node.Attributes["is_mercenary"] != null)
        {
            npcAsset.is_mercenary = node.Attributes["is_mercenary"].Value;
        }
        if (node.Attributes["skill_template"] != null)
        {
            npcAsset.skill_template = node.Attributes["skill_template"].Value;
        }
        if (node.Attributes["upgrade_requires"] != null)
        {
            npcAsset.upgrade_requires = node.Attributes["upgrade_requires"].Value;
        }
        if (node.Attributes["is_basic_troop"] != null)
        {
            npcAsset.is_basic_troop = node.Attributes["is_basic_troop"].Value;
        }
        if (node.Attributes["formation_position_preference"] != null)
        {
            npcAsset.formation_position_preference = node.Attributes["formation_position_preference"].Value;
        }

        // Update 1.7.2
        if (node.Attributes["face_mesh_cache"] != null)
        {
            npcAsset.face_mesh_cache = node.Attributes["face_mesh_cache"].Value;
        }
        if (node.Attributes["is_hidden_encyclopedia"] != null)
        {
            npcAsset.is_hidden_encyclopedia = node.Attributes["is_hidden_encyclopedia"].Value;
        }
        if (node.Attributes["is_obsolete"] != null)
        {
            npcAsset.is_obsolete = node.Attributes["is_obsolete"].Value;
        }

        //SUBNODES
        //EQUIPS
        foreach (XmlNode chld in node.ChildNodes)
        {
            if (chld.LocalName == "Equipments")
            {
                CreateEquipmentSetNPC(chld, modFilesAsset, xmlFile, npcAsset);
            }
        }

        foreach (XmlNode relNode in node.ChildNodes)
        {
            // Body Properties
            if (relNode.LocalName == "face")
            {
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "BodyProperties" && childRelation.Attributes["version"] != null)
                    {
                        if (childRelation.Attributes["version"] != null)
                        {
                            npcAsset.BP_version = childRelation.Attributes["version"].Value;
                        }
                        if (childRelation.Attributes["age"] != null)
                        {
                            npcAsset.BP_age = childRelation.Attributes["age"].Value;
                        }
                        if (childRelation.Attributes["weight"] != null)
                        {
                            npcAsset.BP_weight = childRelation.Attributes["weight"].Value;
                        }
                        if (childRelation.Attributes["build"] != null)
                        {
                            npcAsset.BP_build = childRelation.Attributes["build"].Value;
                        }
                        if (childRelation.Attributes["key"] != null)
                        {
                            npcAsset.BP_key = childRelation.Attributes["key"].Value;
                        }

                    }

                    if (childRelation.LocalName == "BodyPropertiesMax" && childRelation.Attributes["version"] != null)
                    {
                        if (childRelation.Attributes["version"] != null)
                        {
                            npcAsset.Max_BP_version = childRelation.Attributes["version"].Value;
                        }
                        if (childRelation.Attributes["age"] != null)
                        {
                            npcAsset.Max_BP_age = childRelation.Attributes["age"].Value;
                        }
                        if (childRelation.Attributes["weight"] != null)
                        {
                            npcAsset.Max_BP_weight = childRelation.Attributes["weight"].Value;
                        }
                        if (childRelation.Attributes["build"] != null)
                        {
                            npcAsset.Max_BP_build = childRelation.Attributes["build"].Value;
                        }
                        if (childRelation.Attributes["key"] != null)
                        {
                            npcAsset.Max_BP_key = childRelation.Attributes["key"].Value;
                        }
                        npcAsset._Has_Max_BP = true;
                    }

                    if (childRelation.LocalName == "face_key_template" && childRelation.Attributes["value"] != null)
                    {
                        if (childRelation.Attributes["value"] != null)
                        {
                            npcAsset.face_key_template = childRelation.Attributes["value"].Value;
                        }
                    }

                    // Update 1.7.2
                    if (childRelation.LocalName == "hair_tags")
                    {

                        int countArray = 0;
                        foreach (XmlNode rel in relNode.ChildNodes)
                        {
                            if (rel.LocalName != "#comment")
                            {
                                countArray++;
                            }
                        }

                        int i = 0;
                        npcAsset.hair_tag = new string[countArray];

                        foreach (XmlNode rel in relNode.ChildNodes)
                        {
                            if (rel.LocalName == "hair_tag" && rel.Attributes["name"] != null)
                            {
                                if (rel.Attributes["name"] != null)
                                {
                                    npcAsset.hair_tag[i] = rel.Attributes["name"].Value;
                                }
                                i++;
                            }
                        }
                    }

                    if (childRelation.LocalName == "beard_tags")
                    {

                        int countArray = 0;
                        foreach (XmlNode rel in relNode.ChildNodes)
                        {
                            if (rel.LocalName != "#comment")
                            {
                                countArray++;
                            }
                        }

                        int i = 0;
                        npcAsset.beard_tag = new string[countArray];

                        foreach (XmlNode rel in relNode.ChildNodes)
                        {
                            if (rel.LocalName == "beard_tag" && rel.Attributes["name"] != null)
                            {
                                if (rel.Attributes["name"] != null)
                                {
                                    npcAsset.hair_tag[i] = rel.Attributes["name"].Value;
                                }
                                i++;
                            }
                        }
                    }
                }
            }
            // SKILLS
            if (relNode.LocalName == "skills")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                npcAsset.skills = new string[countArray];
                npcAsset.skillValues = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "skill" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            npcAsset.skills[i] = childRelation.Attributes["id"].Value;
                        }
                        if (childRelation.Attributes["value"] != null)
                        {
                            npcAsset.skillValues[i] = childRelation.Attributes["value"].Value;
                        }

                        i++;
                    }
                }
            }
            // UPGRADE TARGETS
            if (relNode.LocalName == "upgrade_targets")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                npcAsset.upgrade_targets = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "upgrade_target" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            npcAsset.upgrade_targets[i] = childRelation.Attributes["id"].Value;
                        }
                        i++;
                    }
                }
            }

            // COMPONENTS
            if (relNode.LocalName == "Components")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                // npcAsset.COMP_Companion = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "Companion" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            npcAsset.COMP_Companion = childRelation.Attributes["id"].Value;
                        }
                        i++;
                    }
                }
            }

            // TRAITS
            if (relNode.LocalName == "Traits")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                npcAsset.traits = new string[countArray];
                npcAsset.traitValues = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "Trait" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            npcAsset.traits[i] = childRelation.Attributes["id"].Value;
                        }
                        if (childRelation.Attributes["value"] != null)
                        {
                            npcAsset.traitValues[i] = childRelation.Attributes["value"].Value;
                        }

                        i++;
                    }
                }
            }
            // FEATS
            if (relNode.LocalName == "feats")
            {
                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                npcAsset.feats = new string[countArray];
                npcAsset.featValues = new string[countArray];


                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "feat" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            npcAsset.feats[i] = childRelation.Attributes["id"].Value;
                        }
                        if (childRelation.Attributes["value"] != null)
                        {
                            npcAsset.featValues[i] = childRelation.Attributes["value"].Value;
                        }

                        i++;
                    }
                }
            }

            //     // EQUIPMENTS
            //     if (relNode.LocalName == "Equipments")
            //     {
            //         foreach (XmlNode eqp_RST in relNode.ChildNodes)
            //         {
            //             if (eqp_RST.LocalName == "EquipmentRoster")
            //             {
            //                 if (eqp_RST.Attributes["civilian"] == null || eqp_RST.Attributes["civilian"].Value != "true")
            //                 {
            //                     var dicValues = new Dictionary<string, string>();

            //                     foreach (XmlNode childRelation in eqp_RST.ChildNodes)
            //                     {
            //                         if (childRelation.LocalName == "equipment" && childRelation.Attributes["slot"] != null)
            //                         {

            //                             if (childRelation.Attributes["slot"].Value == "Item0")
            //                             {
            //                                 if (dicValues.ContainsKey("Item0"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item0"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item0", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Item1")
            //                             {
            //                                 if (dicValues.ContainsKey("Item1"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item1"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item1", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Item2")
            //                             {
            //                                 if (dicValues.ContainsKey("Item2"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item2"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item2", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Item3")
            //                             {
            //                                 if (dicValues.ContainsKey("Item3"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item3"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item3", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Head")
            //                             {
            //                                 if (dicValues.ContainsKey("Head"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Head"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Head", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Gloves")
            //                             {
            //                                 if (dicValues.ContainsKey("Gloves"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Gloves"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Gloves", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Body")
            //                             {
            //                                 if (dicValues.ContainsKey("Body"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Body"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Body", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Leg")
            //                             {
            //                                 if (dicValues.ContainsKey("Leg"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Leg"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Leg", childRelation.Attributes["id"].Value);
            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Cape")
            //                             {
            //                                 if (dicValues.ContainsKey("Cape"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Cape"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Cape", childRelation.Attributes["id"].Value);
            //                                 }

            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Horse")
            //                             {
            //                                 if (dicValues.ContainsKey("Horse"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Horse"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Horse", childRelation.Attributes["id"].Value);
            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "HorseHarness")
            //                             {
            //                                 if (dicValues.ContainsKey("HorseHarness"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["HorseHarness"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("HorseHarness", childRelation.Attributes["id"].Value);
            //                                 }
            //                             }
            //                         }
            //                     }

            //                     dicIDs.Add(false);
            //                     dicEquips.Add(equipCount, dicValues);
            //                 }
            //                 else
            //                 {
            //                     var dicValues = new Dictionary<string, string>();

            //                     foreach (XmlNode childRelation in eqp_RST.ChildNodes)
            //                     {
            //                         if (childRelation.LocalName == "equipment" && childRelation.Attributes["slot"] != null)
            //                         {
            //                             if (childRelation.Attributes["slot"].Value == "Item0")
            //                             {
            //                                 if (dicValues.ContainsKey("Item0"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item0"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item0", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Item1")
            //                             {
            //                                 if (dicValues.ContainsKey("Item1"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item1"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item1", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Item2")
            //                             {
            //                                 if (dicValues.ContainsKey("Item2"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item2"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item2", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Item3")
            //                             {
            //                                 if (dicValues.ContainsKey("Item3"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Item3"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Item3", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Head")
            //                             {
            //                                 if (dicValues.ContainsKey("Head"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Head"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Head", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Gloves")
            //                             {
            //                                 if (dicValues.ContainsKey("Gloves"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Gloves"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Gloves", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Body")
            //                             {
            //                                 if (dicValues.ContainsKey("Body"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Body"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Body", childRelation.Attributes["id"].Value);

            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Leg")
            //                             {
            //                                 if (dicValues.ContainsKey("Leg"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Leg"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Leg", childRelation.Attributes["id"].Value);
            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Cape")
            //                             {
            //                                 if (dicValues.ContainsKey("Cape"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Cape"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Cape", childRelation.Attributes["id"].Value);
            //                                 }

            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "Horse")
            //                             {
            //                                 if (dicValues.ContainsKey("Horse"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["Horse"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("Horse", childRelation.Attributes["id"].Value);
            //                                 }
            //                             }
            //                             else if (childRelation.Attributes["slot"].Value == "HorseHarness")
            //                             {
            //                                 if (dicValues.ContainsKey("HorseHarness"))
            //                                 {
            //                                     // Debug.Log(childRelation.Attributes["id"].Value);
            //                                     dicValues["HorseHarness"] = childRelation.Attributes["id"].Value;
            //                                 }
            //                                 else
            //                                 {
            //                                     dicValues.Add("HorseHarness", childRelation.Attributes["id"].Value);
            //                                 }
            //                             }
            //                         }

            //                     }

            //                     dicIDs.Add(true);
            //                     dicEquips.Add(equipCount, dicValues);
            //                 }
            //                 equipCount++;
            //             }
            //         }


            //         npcAsset.EquipmentRoster.eqpSetList.eqp_Item0 = new string[dicEquips.Count];
            //         npcAsset.eqp_Item1 = new string[dicEquips.Count];
            //         npcAsset.eqp_Item2 = new string[dicEquips.Count];
            //         npcAsset.eqp_Item3 = new string[dicEquips.Count];
            //         npcAsset.eqp_Head = new string[dicEquips.Count];
            //         npcAsset.eqp_Gloves = new string[dicEquips.Count];
            //         npcAsset.eqp_Body = new string[dicEquips.Count];
            //         npcAsset.eqp_Leg = new string[dicEquips.Count];
            //         npcAsset.eqp_Cape = new string[dicEquips.Count];
            //         npcAsset.eqp_Horse = new string[dicEquips.Count];
            //         npcAsset.eqp_HorseHarness = new string[dicEquips.Count];

            //         npcAsset.IsCivilianEquip = new bool[dicEquips.Count];

            //         int i2 = 0;
            //         foreach (var equipDictionary in dicEquips)
            //         {
            //             npcAsset.IsCivilianEquip[i2] = dicIDs.ToArray()[i2];

            //             foreach (var equip in equipDictionary.Value)
            //             {
            //                 if (equip.Key == "Item0")
            //                 {
            //                     npcAsset.eqp_Item0[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Item1")
            //                 {
            //                     npcAsset.eqp_Item1[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Item2")
            //                 {
            //                     npcAsset.eqp_Item2[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Item3")
            //                 {
            //                     npcAsset.eqp_Item3[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Head")
            //                 {
            //                     npcAsset.eqp_Head[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Gloves")
            //                 {
            //                     npcAsset.eqp_Gloves[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Body")
            //                 {
            //                     npcAsset.eqp_Body[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Leg")
            //                 {
            //                     npcAsset.eqp_Leg[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Cape")
            //                 {
            //                     npcAsset.eqp_Cape[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "Horse")
            //                 {
            //                     npcAsset.eqp_Horse[i2] = equip.Value;

            //                 }
            //                 else if (equip.Key == "HorseHarness")
            //                 {
            //                     npcAsset.eqp_HorseHarness[i2] = equip.Value;

            //                 }

            //             }
            //             i2++;
            //         }
            //     }


        }


        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        npcAsset.XmlFileName = xmlFileName;

        // modFilesAsset.npcChrData.NPCCharacters.Add(npcAsset);

        if (npcAsset.is_template == "true" || npcAsset.is_child_template == "true")
        {
            path = modFilesAsset.modResourcesPath + "/_Templates/NPCtemplates/" + node.Attributes["id"].Value + ".asset";
            // AssetDatabase.CreateAsset(npcAsset, path);
            // AssetDatabase.SaveAssets();
        }
        else
        {
            path = modFilesAsset.modResourcesPath + "/NPC/" + node.Attributes["id"].Value + ".asset";
            // AssetDatabase.CreateAsset(npcAsset, path);
            // AssetDatabase.SaveAssets();
        }

        npc_Assets.Add(npcAsset, path);
        // AssetDatabase.CreateAsset(npcAsset, path);
        // AssetDatabase.SaveAssets();
    }

    public void CreateCultureAsset(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Culture cultAsset = ScriptableObject.CreateInstance<Culture>();

        cultAsset.moduleID = modFilesAsset.mod.id;

        cultAsset.id = node.Attributes["id"].Value;

        if (node.Attributes["name"] != null)
        {
            CreateTSData(node, modFilesAsset, "name", "CulturesTranslationData", TS_Assets, "Culture");
            cultAsset.cultureName = node.Attributes["name"].Value;
        }

        if (node.Attributes["is_main_culture"] != null)
        {
            cultAsset.is_main_culture = node.Attributes["is_main_culture"].Value;
        }
        if (node.Attributes["color"] != null)
        {
            cultAsset.color = node.Attributes["color"].Value;
        }
        if (node.Attributes["color2"] != null)
        {
            cultAsset.color2 = node.Attributes["color2"].Value;
        }
        if (node.Attributes["elite_basic_troop"] != null)
        {
            cultAsset.elite_basic_troop = node.Attributes["elite_basic_troop"].Value;
        }
        if (node.Attributes["basic_troop"] != null)
        {
            cultAsset.basic_troop = node.Attributes["basic_troop"].Value;
        }
        if (node.Attributes["melee_militia_troop"] != null)
        {
            cultAsset.melee_militia_troop = node.Attributes["melee_militia_troop"].Value;
        }
        if (node.Attributes["ranged_militia_troop"] != null)
        {
            cultAsset.ranged_militia_troop = node.Attributes["ranged_militia_troop"].Value;
        }
        if (node.Attributes["melee_elite_militia_troop"] != null)
        {
            cultAsset.melee_elite_militia_troop = node.Attributes["melee_elite_militia_troop"].Value;
        }
        if (node.Attributes["ranged_elite_militia_troop"] != null)
        {
            cultAsset.ranged_elite_militia_troop = node.Attributes["ranged_elite_militia_troop"].Value;
        }
        if (node.Attributes["can_have_settlement"] != null)
        {
            cultAsset.can_have_settlement = node.Attributes["can_have_settlement"].Value;
        }
        if (node.Attributes["town_edge_number"] != null)
        {
            cultAsset.town_edge_number = node.Attributes["town_edge_number"].Value;
        }
        if (node.Attributes["villager_party_template"] != null)
        {
            cultAsset.villager_party_template = node.Attributes["villager_party_template"].Value;
        }
        if (node.Attributes["default_party_template"] != null)
        {
            cultAsset.default_party_template = node.Attributes["default_party_template"].Value;
        }
        if (node.Attributes["caravan_party_template"] != null)
        {
            cultAsset.caravan_party_template = node.Attributes["caravan_party_template"].Value;
        }
        if (node.Attributes["elite_caravan_party_template"] != null)
        {
            cultAsset.elite_caravan_party_template = node.Attributes["elite_caravan_party_template"].Value;
        }
        if (node.Attributes["militia_party_template"] != null)
        {
            cultAsset.militia_party_template = node.Attributes["militia_party_template"].Value;
        }
        if (node.Attributes["rebels_party_template"] != null)
        {
            cultAsset.rebels_party_template = node.Attributes["rebels_party_template"].Value;
        }
        if (node.Attributes["prosperity_bonus"] != null)
        {
            cultAsset.prosperity_bonus = node.Attributes["prosperity_bonus"].Value;
        }
        if (node.Attributes["encounter_background_mesh"] != null)
        {
            cultAsset.encounter_background_mesh = node.Attributes["encounter_background_mesh"].Value;
        }
        if (node.Attributes["default_face_key"] != null)
        {
            cultAsset.default_face_key = node.Attributes["default_face_key"].Value;
        }

        if (node.Attributes["text"] != null)
        {
            CreateTSData(node, modFilesAsset, "text", "CulturesTranslationData", TS_Assets, "Culture");

            cultAsset.text = node.Attributes["text"].Value;
        }

        if (node.Attributes["tournament_master"] != null)
        {
            cultAsset.tournament_master = node.Attributes["tournament_master"].Value;
        }
        if (node.Attributes["villager"] != null)
        {
            cultAsset.villager = node.Attributes["villager"].Value;
        }
        if (node.Attributes["caravan_master"] != null)
        {
            cultAsset.caravan_master = node.Attributes["caravan_master"].Value;
        }
        if (node.Attributes["armed_trader"] != null)
        {
            cultAsset.armed_trader = node.Attributes["armed_trader"].Value;
        }
        if (node.Attributes["caravan_guard"] != null)
        {
            cultAsset.caravan_guard = node.Attributes["caravan_guard"].Value;
        }
        if (node.Attributes["veteran_caravan_guard"] != null)
        {
            cultAsset.veteran_caravan_guard = node.Attributes["veteran_caravan_guard"].Value;
        }
        if (node.Attributes["duel_preset"] != null)
        {
            cultAsset.duel_preset = node.Attributes["duel_preset"].Value;
        }
        if (node.Attributes["prison_guard"] != null)
        {
            cultAsset.prison_guard = node.Attributes["prison_guard"].Value;
        }
        if (node.Attributes["guard"] != null)
        {
            cultAsset.guard = node.Attributes["guard"].Value;
        }
        if (node.Attributes["steward"] != null)
        {
            cultAsset.steward = node.Attributes["steward"].Value;
        }
        if (node.Attributes["blacksmith"] != null)
        {
            cultAsset.blacksmith = node.Attributes["blacksmith"].Value;
        }
        if (node.Attributes["weaponsmith"] != null)
        {
            cultAsset.weaponsmith = node.Attributes["weaponsmith"].Value;
        }
        if (node.Attributes["townswoman"] != null)
        {
            cultAsset.townswoman = node.Attributes["townswoman"].Value;
        }
        if (node.Attributes["townswoman_infant"] != null)
        {
            cultAsset.townswoman_infant = node.Attributes["townswoman_infant"].Value;
        }
        if (node.Attributes["townswoman_child"] != null)
        {
            cultAsset.townswoman_child = node.Attributes["townswoman_child"].Value;
        }
        if (node.Attributes["townswoman_teenager"] != null)
        {
            cultAsset.townswoman_teenager = node.Attributes["townswoman_teenager"].Value;
        }
        if (node.Attributes["townsman"] != null)
        {
            cultAsset.townsman = node.Attributes["townsman"].Value;
        }
        if (node.Attributes["townsman_infant"] != null)
        {
            cultAsset.townsman_infant = node.Attributes["townsman_infant"].Value;
        }
        if (node.Attributes["townsman_child"] != null)
        {
            cultAsset.townsman_child = node.Attributes["townsman_child"].Value;
        }
        if (node.Attributes["village_woman"] != null)
        {
            cultAsset.village_woman = node.Attributes["village_woman"].Value;
        }
        if (node.Attributes["villager_male_child"] != null)
        {
            cultAsset.villager_male_child = node.Attributes["villager_male_child"].Value;
        }
        if (node.Attributes["villager_male_teenager"] != null)
        {
            cultAsset.villager_male_teenager = node.Attributes["villager_male_teenager"].Value;
        }
        if (node.Attributes["villager_female_child"] != null)
        {
            cultAsset.villager_female_child = node.Attributes["villager_female_child"].Value;
        }
        if (node.Attributes["villager_female_teenager"] != null)
        {
            cultAsset.villager_female_teenager = node.Attributes["villager_female_teenager"].Value;
        }
        if (node.Attributes["townsman_teenager"] != null)
        {
            cultAsset.townsman_teenager = node.Attributes["townsman_teenager"].Value;
        }
        if (node.Attributes["ransom_broker"] != null)
        {
            cultAsset.ransom_broker = node.Attributes["ransom_broker"].Value;
        }
        if (node.Attributes["gangleader_bodyguard"] != null)
        {
            cultAsset.gangleader_bodyguard = node.Attributes["gangleader_bodyguard"].Value;
        }
        if (node.Attributes["merchant_notary"] != null)
        {
            cultAsset.merchant_notary = node.Attributes["merchant_notary"].Value;
        }
        if (node.Attributes["artisan_notary"] != null)
        {
            cultAsset.artisan_notary = node.Attributes["artisan_notary"].Value;
        }
        if (node.Attributes["preacher_notary"] != null)
        {
            cultAsset.preacher_notary = node.Attributes["preacher_notary"].Value;
        }
        if (node.Attributes["rural_notable_notary"] != null)
        {
            cultAsset.rural_notable_notary = node.Attributes["rural_notable_notary"].Value;
        }
        if (node.Attributes["shop_worker"] != null)
        {
            cultAsset.shop_worker = node.Attributes["shop_worker"].Value;
        }
        if (node.Attributes["tavernkeeper"] != null)
        {
            cultAsset.tavernkeeper = node.Attributes["tavernkeeper"].Value;
        }
        if (node.Attributes["taverngamehost"] != null)
        {
            cultAsset.taverngamehost = node.Attributes["taverngamehost"].Value;
        }
        if (node.Attributes["musician"] != null)
        {
            cultAsset.musician = node.Attributes["musician"].Value;
        }
        if (node.Attributes["tavern_wench"] != null)
        {
            cultAsset.tavern_wench = node.Attributes["tavern_wench"].Value;
        }
        if (node.Attributes["armorer"] != null)
        {
            cultAsset.armorer = node.Attributes["armorer"].Value;
        }
        if (node.Attributes["horseMerchant"] != null)
        {
            cultAsset.horseMerchant = node.Attributes["horseMerchant"].Value;
        }
        if (node.Attributes["barber"] != null)
        {
            cultAsset.barber = node.Attributes["barber"].Value;
        }
        if (node.Attributes["merchant"] != null)
        {
            cultAsset.merchant = node.Attributes["merchant"].Value;
        }
        if (node.Attributes["beggar"] != null)
        {
            cultAsset.beggar = node.Attributes["beggar"].Value;
        }
        if (node.Attributes["female_beggar"] != null)
        {
            cultAsset.female_beggar = node.Attributes["female_beggar"].Value;
        }
        if (node.Attributes["female_dancer"] != null)
        {
            cultAsset.female_dancer = node.Attributes["female_dancer"].Value;
        }
        if (node.Attributes["gear_practice_dummy"] != null)
        {
            cultAsset.gear_practice_dummy = node.Attributes["gear_practice_dummy"].Value;
        }
        if (node.Attributes["weapon_practice_stage_1"] != null)
        {
            cultAsset.weapon_practice_stage_1 = node.Attributes["weapon_practice_stage_1"].Value;
        }
        if (node.Attributes["weapon_practice_stage_2"] != null)
        {
            cultAsset.weapon_practice_stage_2 = node.Attributes["weapon_practice_stage_2"].Value;
        }
        if (node.Attributes["weapon_practice_stage_3"] != null)
        {
            cultAsset.weapon_practice_stage_3 = node.Attributes["weapon_practice_stage_3"].Value;
        }
        if (node.Attributes["gear_dummy"] != null)
        {
            cultAsset.gear_dummy = node.Attributes["gear_dummy"].Value;
        }
        if (node.Attributes["board_game_type"] != null)
        {
            cultAsset.board_game_type = node.Attributes["board_game_type"].Value;
        }

        // Update 1.7.2
        if (node.Attributes["vassal_reward_party_template"] != null)
        {
            cultAsset.vassal_reward_party_template = node.Attributes["vassal_reward_party_template"].Value;
        }
        if (node.Attributes["faction_banner_key"] != null)
        {
            cultAsset.faction_banner_key = node.Attributes["faction_banner_key"].Value;
        }
        if (node.Attributes["basic_mercenary_troop"] != null)
        {
            cultAsset.basic_mercenary_troop = node.Attributes["basic_mercenary_troop"].Value;
        }
        if (node.Attributes["militia_bonus"] != null)
        {
            cultAsset.militia_bonus = node.Attributes["militia_bonus"].Value;
        }
        if (node.Attributes["is_bandit"] != null)
        {
            cultAsset.is_bandit = node.Attributes["is_bandit"].Value;
        }
        if (node.Attributes["bandit_chief"] != null)
        {
            cultAsset.bandit_chief = node.Attributes["bandit_chief"].Value;
        }
        if (node.Attributes["bandit_raider"] != null)
        {
            cultAsset.bandit_raider = node.Attributes["bandit_raider"].Value;
        }
        if (node.Attributes["bandit_bandit"] != null)
        {
            cultAsset.bandit_bandit = node.Attributes["bandit_bandit"].Value;
        }
        if (node.Attributes["bandit_boss"] != null)
        {
            cultAsset.bandit_boss = node.Attributes["bandit_boss"].Value;
        }
        if (node.Attributes["bandit_boss_party_template"] != null)
        {
            cultAsset.bandit_boss_party_template = node.Attributes["bandit_boss_party_template"].Value;
        }


        foreach (XmlNode relNode in node.ChildNodes)
        {

            // MaleName
            if (relNode.LocalName == "male_names")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.male_names = new string[countArray];


                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "name" && childRelation.Attributes["name"] != null)
                    {
                        if (childRelation.Attributes["name"] != null)
                        {
                            CreateTSData(childRelation, modFilesAsset, "name", "CulturesTranslationData", TS_Assets, "Culture");
                            cultAsset.male_names[i] = childRelation.Attributes["name"].Value;
                        }

                        i++;
                    }
                }
            }

            // FemaleNames
            if (relNode.LocalName == "female_names")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.female_names = new string[countArray];


                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "name" && childRelation.Attributes["name"] != null)
                    {
                        if (childRelation.Attributes["name"] != null)
                        {
                            CreateTSData(childRelation, modFilesAsset, "name", "CulturesTranslationData", TS_Assets, "Culture");
                            cultAsset.female_names[i] = childRelation.Attributes["name"].Value;
                        }

                        i++;
                    }
                }
            }
            // ClanNames
            if (relNode.LocalName == "clan_names")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.clan_names = new string[countArray];


                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "name" && childRelation.Attributes["name"] != null)
                    {
                        if (childRelation.Attributes["name"] != null)
                        {
                            CreateTSData(childRelation, modFilesAsset, "name", "CulturesTranslationData", TS_Assets, "Culture");
                            cultAsset.clan_names[i] = childRelation.Attributes["name"].Value;
                        }

                        i++;
                    }
                }
            }

            WriteNPCTemplate(relNode, ref cultAsset.child_character_templates, "child_character_templates");
            WriteNPCTemplate(relNode, ref cultAsset.notable_and_wanderer_templates, "notable_and_wanderer_templates");
            WriteNPCTemplate(relNode, ref cultAsset.lord_templates, "lord_templates");
            WriteNPCTemplate(relNode, ref cultAsset.rebellion_hero_templates, "rebellion_hero_templates");

            // Tournament Templates
            if (relNode.LocalName == "tournament_team_templates_one_participant")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.TTT_one_participants = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "template" && childRelation.Attributes["name"] != null)
                    {
                        if (childRelation.Attributes["name"] != null)
                        {
                            cultAsset.TTT_one_participants[i] = childRelation.Attributes["name"].Value;
                        }

                        i++;
                    }
                }
            }

            if (relNode.LocalName == "tournament_team_templates_two_participant")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.TTT_two_participants = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "template" && childRelation.Attributes["name"] != null)
                    {
                        if (childRelation.Attributes["name"] != null)
                        {
                            cultAsset.TTT_two_participants[i] = childRelation.Attributes["name"].Value;
                        }

                        i++;
                    }
                }
            }

            if (relNode.LocalName == "tournament_team_templates_four_participant")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.TTT_four_participants = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "template" && childRelation.Attributes["name"] != null)
                    {
                        if (childRelation.Attributes["name"] != null)
                        {
                            cultAsset.TTT_four_participants[i] = childRelation.Attributes["name"].Value;
                        }

                        i++;
                    }
                }
            }

            // Update 1.7.2
            if (relNode.LocalName == "possible_clan_banner_icon_ids")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.banner_icon_id = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "icon" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            cultAsset.banner_icon_id[i] = childRelation.Attributes["id"].Value;
                        }

                        i++;
                    }
                }
            }

            if (relNode.LocalName == "vassal_reward_items")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.reward_item_id = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "item" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            cultAsset.reward_item_id[i] = childRelation.Attributes["id"].Value;
                        }

                        i++;
                    }
                }
            }

            if (relNode.LocalName == "cultural_feats")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                cultAsset.cultural_feat_id = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "feat" && childRelation.Attributes["id"] != null)
                    {
                        if (childRelation.Attributes["id"] != null)
                        {
                            cultAsset.cultural_feat_id[i] = childRelation.Attributes["id"].Value;
                        }

                        i++;
                    }
                }
            }
        }


        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        cultAsset.XmlFileName = xmlFileName;

        // modFilesAsset.culturesData.cultures.Add(cultAsset);

        path = modFilesAsset.modResourcesPath + "/Cultures/" + node.Attributes["id"].Value + ".asset";

        cult_Assets.Add(cultAsset, path);
        // AssetDatabase.CreateAsset(cultAsset, path);
        // AssetDatabase.SaveAssets();
    }

    private static void WriteNPCTemplate(XmlNode relNode, ref string[] string_array, string templatesID)
    {
        if (relNode.LocalName == templatesID)
        {
            //Debug.Log(templatesID);
            int countArray = 0;
            foreach (XmlNode childRelation in relNode.ChildNodes)
            {
                if (childRelation.LocalName != "#comment")
                {
                    countArray++;
                }
            }

            int i = 0;
            string_array = new string[countArray];

            foreach (XmlNode childRelation in relNode.ChildNodes)
            {
                if (childRelation.LocalName == "template" && childRelation.Attributes["name"] != null)
                {
                    if (childRelation.Attributes["name"] != null)
                    {
                        string_array[i] = childRelation.Attributes["name"].Value;
                    }

                    i++;
                }
            }
        }
    }

    public void CreatePTAsset(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        PartyTemplate PTasset = ScriptableObject.CreateInstance<PartyTemplate>();

        PTasset.moduleID = modFilesAsset.mod.id;

        if (node.Attributes["id"] != null)
        {
            PTasset.id = node.Attributes["id"].Value; ;
        }


        foreach (XmlNode relNode in node.ChildNodes)
        {

            // SKILLS
            if (relNode.LocalName == "stacks")
            {

                int countArray = 0;
                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName != "#comment")
                    {
                        countArray++;
                    }
                }

                int i = 0;
                PTasset.PTS_troop = new string[countArray];
                PTasset.PTS_min_value = new string[countArray];
                PTasset.PTS_max_value = new string[countArray];

                foreach (XmlNode childRelation in relNode.ChildNodes)
                {
                    if (childRelation.LocalName == "PartyTemplateStack" && childRelation.Attributes["troop"] != null)
                    {
                        if (childRelation.Attributes["troop"] != null)
                        {
                            PTasset.PTS_troop[i] = childRelation.Attributes["troop"].Value;
                        }
                        if (childRelation.Attributes["min_value"] != null)
                        {
                            PTasset.PTS_min_value[i] = childRelation.Attributes["min_value"].Value;
                        }
                        if (childRelation.Attributes["max_value"] != null)
                        {
                            PTasset.PTS_max_value[i] = childRelation.Attributes["max_value"].Value;
                        }

                        i++;
                    }
                }
            }
        }

        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        PTasset.XmlFileName = xmlFileName;

        // modFilesAsset.PTdata.partyTemplates.Add(PTasset);

        path = modFilesAsset.modResourcesPath + "/PartyTemplates/" + node.Attributes["id"].Value + ".asset";

        PT_Assets.Add(PTasset, path);
        // AssetDatabase.CreateAsset(PTasset, path);
        // AssetDatabase.SaveAssets();
    }
    public void CreateItemAsset(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Item itemAsset = ScriptableObject.CreateInstance<Item>();

        itemAsset.moduleID = modFilesAsset.mod.id;

        if (node.LocalName == "Item")
        {
            // Debug.Log("Item");
            CheckAssignAtribute(node, ref itemAsset.id, "id");

            if (node.Attributes["name"] != null)
            {
                CreateTSData(node, modFilesAsset, "name", "ItemsTranslationData", TS_Assets, "Item");
                itemAsset.itemName = GetSingleNameFromString(node.Attributes["name"].Value);
            }

            CheckAssignAtribute(node, ref itemAsset.mesh, "mesh");
            CheckAssignAtribute(node, ref itemAsset.culture, "culture");
            CheckAssignAtribute(node, ref itemAsset.weight, "weight");
            CheckAssignAtribute(node, ref itemAsset.difficulty, "difficulty");
            CheckAssignAtribute(node, ref itemAsset.appearance, "appearance");
            CheckAssignAtribute(node, ref itemAsset.Type, "Type");
            CheckAssignAtribute(node, ref itemAsset.is_merchandise, "is_merchandise");
            CheckAssignAtribute(node, ref itemAsset.value, "value");
            CheckAssignAtribute(node, ref itemAsset.item_category, "item_category");

            // UPD

            CheckAssignAtribute(node, ref itemAsset.lod_atlas_index, "lod_atlas_index");
            CheckAssignAtribute(node, ref itemAsset.subtype, "subtype");
            CheckAssignAtribute(node, ref itemAsset.multiplayer_item, "multiplayer_item");
            CheckAssignAtribute(node, ref itemAsset.IsFood, "IsFood");
            CheckAssignAtribute(node, ref itemAsset.body_name, "body_name");
            CheckAssignAtribute(node, ref itemAsset.recalculate_body, "recalculate_body");
            CheckAssignAtribute(node, ref itemAsset.using_tableau, "using_tableau");
            CheckAssignAtribute(node, ref itemAsset.item_holsters, "item_holsters");
            CheckAssignAtribute(node, ref itemAsset.shield_body_name, "shield_body_name");
            CheckAssignAtribute(node, ref itemAsset.has_lower_holster_priority, "has_lower_holster_priority");
            CheckAssignAtribute(node, ref itemAsset.holster_position_shift, "holster_position_shift");
            CheckAssignAtribute(node, ref itemAsset.prefab, "prefab");
            CheckAssignAtribute(node, ref itemAsset.AmmoOffset, "AmmoOffset");
            CheckAssignAtribute(node, ref itemAsset.holster_body_name, "holster_body_name");
            CheckAssignAtribute(node, ref itemAsset.holster_mesh, "holster_mesh");
            CheckAssignAtribute(node, ref itemAsset.holster_mesh_with_weapon, "holster_mesh_with_weapon");
            CheckAssignAtribute(node, ref itemAsset.flying_mesh, "flying_mesh");

            foreach (XmlNode relNode in node.ChildNodes)
            {

                // ITEM COMPONENT
                if (relNode.LocalName == "ItemComponent")
                {
                    foreach (XmlNode childRelation in relNode.ChildNodes)
                    {

                        if (childRelation.LocalName != "#comment")
                        {
                            if (childRelation.LocalName == "Weapon")
                            {
                                itemAsset.IsWeapon = true;

                                foreach (XmlNode wpn_Node in childRelation.ChildNodes)
                                {
                                    if (wpn_Node.LocalName == "WeaponFlags")
                                    {
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_RangedWeapon, "RangedWeapon");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_HasString, "HasString");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_CantReloadOnHorseback, "CantReloadOnHorseback");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_NotUsableWithOneHand, "NotUsableWithOneHand");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_TwoHandIdleOnMount, "TwoHandIdleOnMount");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AutoReload, "AutoReload");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_Consumable, "Consumable");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_UseHandAsThrowBase, "UseHandAsThrowBase");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AmmoSticksWhenShot, "AmmoSticksWhenShot");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AmmoBreaksOnBounceBack, "AmmoBreaksOnBounceBack");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AttachAmmoToVisual, "AttachAmmoToVisual");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_CanBlockRanged, "CanBlockRanged");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_HasHitPoints, "HasHitPoints");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_MeleeWeapon, "MeleeWeapon");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_PenaltyWithShield, "PenaltyWithShield");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_WideGrip, "WideGrip");

                                        /// UPD
                                        /// 
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_StringHeldByHand, "StringHeldByHand");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_UnloadWhenSheathed, "UnloadWhenSheathed");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_CanPenetrateShield, "CanPenetrateShield");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_MultiplePenetration, "MultiplePenetration");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_Burning, "Burning");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_LeavesTrail, "LeavesTrail");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_CanKnockDown, "CanKnockDown");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_MissileWithPhysics, "MissileWithPhysics");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AmmoCanBreakOnBounceBack, "AmmoCanBreakOnBounceBack");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AffectsArea, "AffectsArea");
                                        CheckAssignAtribute(wpn_Node, ref itemAsset.WPN_FLG_AffectsAreaBig, "AffectsAreaBig");

                                    }
                                }

                            }
                            else if (childRelation.LocalName == "Armor")
                            {
                                itemAsset.IsArmor = true;
                            }
                            else if (childRelation.LocalName == "Horse")
                            {
                                itemAsset.IsHorse = true;
                            }

                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_arm_armor, "arm_armor");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_covers_hands, "covers_hands");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_modifier_group, "modifier_group");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_material_type, "material_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_family_type, "family_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_body_armor, "body_armor");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_leg_armor, "leg_armor");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_has_gender_variations, "has_gender_variations");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_covers_body, "covers_body");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_body_mesh_type, "body_mesh_type");

                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_head_armor, "head_armor");

                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_hair_cover_type, "hair_cover_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_beard_cover_type, "beard_cover_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_covers_legs, "covers_legs");


                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_weapon_class, "weapon_class");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_thrust_speed, "thrust_speed");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_thrust_damage, "thrust_damage");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_thrust_damage_type, "thrust_damage_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_speed_rating, "speed_rating");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_physics_material, "physics_material");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_item_usage, "item_usage");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_position, "position");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_rotation, "rotation");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_weapon_length, "weapon_length");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_center_of_mass, "center_of_mass");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_hit_points, "hit_points");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_weapon_balance, "weapon_balance");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_missile_speed, "missile_speed");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_swing_damage, "swing_damage");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_swing_damage_type, "swing_damage_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_ammo_class, "ammo_class");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_ammo_limit, "ammo_limit");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_accuracy, "accuracy");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_flying_sound_code, "flying_sound_code");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_rotation_speed, "rotation_speed");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_trail_particle_name, "trail_particle_name");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_stack_amount, "stack_amount");


                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_monster, "monster");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_maneuver, "maneuver");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_speed, "speed");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_charge_damage, "charge_damage");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_body_length, "body_length");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_is_mountable, "is_mountable");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_extra_health, "extra_health");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_skeleton_scale, "skeleton_scale");

                            // UPD

                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_mane_cover_type, "mane_cover_type");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_maneuver_bonus, "maneuver_bonus");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_speed_bonus, "speed_bonus");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_charge_bonus, "charge_bonus");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_reins_mesh, "reins_mesh");
                            CheckAssignAtribute(childRelation, ref itemAsset.ARMOR_covers_head, "covers_head");

                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_passby_sound_code, "passby_sound_code");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_sticking_rotation, "sticking_rotation");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_sticking_position, "sticking_position");

                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_is_pack_animal, "is_pack_animal");
                            CheckAssignAtribute(childRelation, ref itemAsset.HRS_mane_mesh, "mane_mesh");

                            CheckAssignAtribute(childRelation, ref itemAsset.TRADE_morale_bonus, "morale_bonus");


                            // pdate 1.7.2
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_reload_phase_count, "reload_phase_count");
                            CheckAssignAtribute(childRelation, ref itemAsset.WPN_item_modifier_group, "item_modifier_group");



                            foreach (XmlNode componentNode in childRelation.ChildNodes)
                            {

                                // AdditionalMeshes
                                if (componentNode.LocalName == "AdditionalMeshes")
                                {

                                    foreach (XmlNode childMeshNode in componentNode.ChildNodes)
                                    {
                                        if (childMeshNode.LocalName == "Mesh")
                                        {
                                            if (childMeshNode.Attributes["affected_by_cover"] != null)
                                            {
                                                CheckAssignAtribute(childMeshNode, ref itemAsset.ADD_cover_Mesh_name, "name");
                                                CheckAssignAtribute(childMeshNode, ref itemAsset.ADD_Mesh_affected_by_cover, "affected_by_cover");
                                            }
                                            else
                                            {
                                                CheckAssignAtribute(childMeshNode, ref itemAsset.ADD_Mesh, "name");
                                            }
                                        }


                                    }
                                }
                                // Material
                                if (componentNode.LocalName == "Materials")
                                {
                                    int countArray = 0;
                                    foreach (XmlNode chldCount in componentNode.ChildNodes)
                                    {
                                        if (chldCount.LocalName != "#comment")
                                        {
                                            countArray++;
                                        }
                                    }

                                    int i = 0;
                                    itemAsset.ADD_mat_name = new string[countArray];

                                    itemAsset.aDD_mat_meshMult_a = new string[countArray];
                                    itemAsset.aDD_mat_meshMult_a_prc = new string[countArray];
                                    itemAsset.aDD_mat_meshMult_b = new string[countArray];
                                    itemAsset.aDD_mat_meshMult_b_prc = new string[countArray];

                                    foreach (XmlNode matNodeChild in componentNode.ChildNodes)
                                    {
                                        if (matNodeChild.LocalName == "Material")
                                        {
                                            CheckAssignAtribute(matNodeChild, ref itemAsset.ADD_mat_name[i], "name");

                                            foreach (XmlNode matGroupNodes in matNodeChild.ChildNodes)
                                            {
                                                if (matGroupNodes.LocalName == "MeshMultipliers")
                                                {
                                                    int multAdded = 0;
                                                    foreach (XmlNode matNodes in matGroupNodes.ChildNodes)
                                                    {
                                                        if (matNodes.LocalName == "MeshMultiplier")
                                                        {
                                                            if (multAdded == 0)
                                                            {
                                                                CheckAssignAtribute(matNodes, ref itemAsset.aDD_mat_meshMult_a[i], "mesh_multiplier");
                                                                CheckAssignAtribute(matNodes, ref itemAsset.aDD_mat_meshMult_a_prc[i], "percentage");
                                                                multAdded = 1;
                                                            }
                                                            else if (multAdded == 1)
                                                            {
                                                                CheckAssignAtribute(matNodes, ref itemAsset.aDD_mat_meshMult_b[i], "mesh_multiplier");
                                                                CheckAssignAtribute(matNodes, ref itemAsset.aDD_mat_meshMult_b_prc[i], "percentage");
                                                                multAdded = 2;
                                                            }
                                                            else
                                                            {
                                                                Debug.Log("Nodes Container Filled !!!");
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            i++;
                                        }
                                    }
                                }

                            }

                        }


                    }

                }

                // FLAGS
                if (relNode.LocalName == "Flags")
                {
                    // Debug.Log(relNode.LocalName);
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_DropOnWeaponChange, "DropOnWeaponChange");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_ForceAttachOffHandPrimaryItemBone, "ForceAttachOffHandPrimaryItemBone");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_HeldInOffHand, "HeldInOffHand");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_HasToBeHeldUp, "HasToBeHeldUp");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_WoodenParry, "WoodenParry");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_DoNotScaleBodyAccordingToWeaponLength, "DoNotScaleBodyAccordingToWeaponLength");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_QuickFadeOut, "QuickFadeOut");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_CannotBePickedUp, "CannotBePickedUp");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_DropOnAnyAction, "DropOnAnyAction");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_Civilian, "Civilian");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_UseTeamColor, "UseTeamColor");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_ForceAttachOffHandSecondaryItemBone, "ForceAttachOffHandSecondaryItemBone");
                    CheckAssignAtribute(relNode, ref itemAsset.FLG_DoesNotHideChest, "DoesNotHideChest");
                }

            }

        }

        if (node.LocalName == "CraftedItem")
        {
            // Debug.Log("CraftedItem");
            itemAsset.IsCraftedItem = true;
            CheckAssignAtribute(node, ref itemAsset.id, "id");

            if (node.Attributes["name"] != null)
            {
                CreateTSData(node, modFilesAsset, "name", "ItemsTranslationData", TS_Assets, "Item");

                itemAsset.itemName = GetSingleNameFromString(node.Attributes["name"].Value);
            }

            CheckAssignAtribute(node, ref itemAsset.CT_crafting_template, "crafting_template");
            CheckAssignAtribute(node, ref itemAsset.culture, "culture");
            CheckAssignAtribute(node, ref itemAsset.is_merchandise, "is_merchandise");

            // Update 1.7.2
            CheckAssignAtribute(node, ref itemAsset.CT_has_modifier, "has_modifier");

            foreach (XmlNode relNode in node.ChildNodes)
            {

                // Pieces
                if (relNode.LocalName == "Pieces")
                {
                    int countArray = 0;
                    foreach (XmlNode chldCount in relNode.ChildNodes)
                    {
                        if (chldCount.LocalName != "#comment")
                        {
                            countArray++;
                        }
                    }


                    int i = 0;
                    itemAsset.CT_pieces_id = new string[countArray];
                    itemAsset.CT_pieces_Type = new string[countArray];
                    itemAsset.CT_pieces_scale_factor = new string[countArray];

                    foreach (XmlNode childPieceNode in relNode.ChildNodes)
                    {
                        if (childPieceNode.LocalName == "Piece")
                        {
                            CheckAssignAtribute(childPieceNode, ref itemAsset.CT_pieces_id[i], "id");
                            CheckAssignAtribute(childPieceNode, ref itemAsset.CT_pieces_Type[i], "Type");
                            CheckAssignAtribute(childPieceNode, ref itemAsset.CT_pieces_scale_factor[i], "scale_factor");

                            i++;
                        }


                    }
                }
            }

        }

        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        itemAsset.XmlFileName = xmlFileName;


        // var randomID = Math.Abs(0.5 - UnityEngine.Random.value) * 1000;
        // itemAsset.id = "MISSED_ID_" + randomID;


        path = modFilesAsset.modResourcesPath + "/Items/" + itemAsset.id + ".asset";

        if (itemAsset.id != null && itemAsset.id != "")
        {
            item_Assets.Add(itemAsset, path);
        }
        // AssetDatabase.CreateAsset(PTasset, path);
        // AssetDatabase.SaveAssets();
    }

    private static void CheckAssignAtribute(XmlNode node, ref string parameter, string atrrName)
    {
        if (node.Attributes[atrrName] != null)
        {
            parameter = node.Attributes[atrrName].Value;
        }
    }

    private static void CheckAssignAtributeBool(XmlNode node, ref bool parameter, string atrrName)
    {
        if (node.Attributes[atrrName] != null)
        {
            if (node.Attributes[atrrName].Value == "true")
                parameter = true;
        }
    }

    public void CreateHeroAsset(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Hero heroAsset = ScriptableObject.CreateInstance<Hero>();

        heroAsset.moduleID = modFilesAsset.mod.id;

        heroAsset.id = node.Attributes["id"].Value;

        if (node.Attributes["alive"] != null)
        {
            heroAsset.alive = node.Attributes["alive"].Value;
        }
        if (node.Attributes["is_noble"] != null)
        {
            heroAsset.is_noble = node.Attributes["is_noble"].Value;
        }
        if (node.Attributes["faction"] != null)
        {
            heroAsset.faction = node.Attributes["faction"].Value;
        }
        if (node.Attributes["banner_key"] != null)
        {
            heroAsset.banner_key = node.Attributes["banner_key"].Value;
        }
        if (node.Attributes["father"] != null)
        {
            heroAsset.father = node.Attributes["father"].Value;
        }
        if (node.Attributes["mother"] != null)
        {
            heroAsset.mother = node.Attributes["mother"].Value;
        }
        if (node.Attributes["spouse"] != null)
        {
            heroAsset.spouse = node.Attributes["spouse"].Value;
        }
        // text description

        if (node.Attributes["text"] != null)
        {
            CreateTSData(node, modFilesAsset, "text", "HeroesTranslationData", TS_Assets, "Heroe");
            heroAsset.text = node.Attributes["text"].Value;
        }

        string fullPath = Path.GetFullPath(xmlFile).TrimEnd(Path.DirectorySeparatorChar);
        string xmlFileName = Path.GetFileName(fullPath);
        heroAsset.XmlFileName = xmlFileName;

        // modFilesAsset.heroesData.heroes.Add(heroAsset);

        path = modFilesAsset.modResourcesPath + "/Heroes/" + node.Attributes["id"].Value + ".asset";

        hero_Assets.Add(heroAsset, path);
        // AssetDatabase.CreateAsset(heroAsset, path);
        // AssetDatabase.SaveAssets();
    }
    public void CreateEquipments(XmlNode node, ModFiles modFilesAsset, string xmlFile)
    {
        string path;

        Equipment equipAsset = ScriptableObject.CreateInstance<Equipment>();
        var eqpSetList = new List<string>();

        equipAsset.moduleID = modFilesAsset.mod.id;

        equipAsset.id = node.Attributes["id"].Value;

        if (node.Attributes["culture"] != null)
        {
            equipAsset.culture = node.Attributes["culture"].Value;
        }

        int i = 0;
        foreach (XmlNode equipSetNode in node.ChildNodes)
        {

            if (equipSetNode.LocalName == "EquipmentSet")
            {
                EquipmentSet equipSet_asset = GetNodeEquipmentSlots(equipSetNode, "Equipment");
                equipSet_asset.EquipmentSetID = equipAsset.id;

                CheckAssignAtributeBool(equipSetNode, ref equipSet_asset.IsCivilianEquip, "civilian");

                path = modFilesAsset.modResourcesPath + "/Sets/Equipments/EqpSets/" + equipAsset.id + "_" + i + ".asset";

                equipmentSets.Add(equipSet_asset, path);
                eqpSetList.Add(equipAsset.id + "_" + i);
                i++;
            }

            if (equipSetNode.LocalName == "Flags")
            {
                foreach (XmlAttribute flag in equipSetNode.Attributes)
                {
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsEquipmentTemplate, "IsEquipmentTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsNobleTemplate, "IsNobleTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsMediumTemplate, "IsMediumTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsHeavyTemplate, "IsHeavyTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsFlamboyantTemplate, "IsFlamboyantTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsStoicTemplate, "IsStoicTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsNomadTemplate, "IsNomadTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsWoodlandTemplate, "IsWoodlandTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsFemaleTemplate, "IsFemaleTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsCivilianTemplate, "IsCivilianTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsCombatantTemplate, "IsCombatantTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsNoncombatantTemplate, "IsNoncombatantTemplate");

                    // Update 1.7.2
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsChildTemplate, "IsChildTemplate");
                    CheckAssignAtributeBool(equipSetNode, ref equipAsset.IsWandererEquipment, "IsWandererEquipment");

                }
            }
        }

        equipAsset.eqpSetID = new string[eqpSetList.Count];

        i = 0;
        foreach (var eqpSet in eqpSetList)
        {
            equipAsset.eqpSetID[i] = eqpSet;
            i++;
        }

        path = modFilesAsset.modResourcesPath + "/Sets/Equipments/" + equipAsset.id + ".asset";

        equipments.Add(equipAsset, path);


    }

    public void CreateEquipmentSetNPC(XmlNode EquipmentNode, ModFiles modFilesAsset, string xmlFile, NPCCharacter npc)
    {
        var sets_links_ids = new List<string>();
        var sets_links_civ_ids = new List<bool>();
        var roster_id_count = 0;
        var roster_ids = new List<string>();

        foreach (XmlNode chld in EquipmentNode.ChildNodes)
        {
            if (chld.LocalName == "EquipmentRoster" && chld.ChildNodes.Count > 0)
            {
                var path = modFilesAsset.modResourcesPath + "/NPC/Equipment/EquipmentRosters/" + "eqp_roster_" + npc.id + "_" + roster_id_count + ".asset";

                if (!eqp_Roster_Assets.ContainsValue(path))
                {
                    // npc.equipment_Roster = "eqp_roster_" + npc.id;

                    roster_ids.Add("eqp_roster_" + npc.id + "_" + roster_id_count);

                    EquipmentSet equip = GetNodeEquipmentSlots(chld, "equipment");
                    equip._is_roster = true;
                    equip.EquipmentSetID = npc.id;
                    equip.moduleID = npc.moduleID;

                    CheckAssignAtributeBool(chld, ref equip.IsCivilianEquip, "civilian");

                    eqp_Roster_Assets.Add(equip, path);
                }

                roster_id_count++;
            }

            if (chld.LocalName == "equipment" && chld.Attributes["slot"] != null)
            {
                var path = modFilesAsset.modResourcesPath + "/NPC/Equipment/EquipmentMain/" + "eqp_main_" + npc.id + ".asset";

                if (!eqp_Main_Assets.ContainsValue(path))
                {
                    npc.equipment_Main = "eqp_main_" + npc.id;
                    EquipmentSet equip = GetNodeEquipmentSlots(EquipmentNode, "equipment");
                    equip._is_main = true;
                    equip.EquipmentSetID = npc.id;
                    equip.moduleID = npc.moduleID;

                    eqp_Main_Assets.Add(equip, path);
                }
            }

            if (chld.LocalName == "EquipmentSet" && chld.Attributes["id"] != null)
            {
                sets_links_ids.Add(chld.Attributes["id"].Value);

                if (chld.Attributes["civilian"] != null)
                {
                    sets_links_civ_ids.Add(true);
                }
                else
                {
                    sets_links_civ_ids.Add(false);
                }
            }

        }

        npc.equipment_Roster = new string[roster_ids.Count];

        int i_roster = 0;
        foreach (var roster in roster_ids)
        {
            npc.equipment_Roster[i_roster] = roster;
            i_roster++;
        }

        npc.equipment_Set = new string[sets_links_ids.Count];

        int i = 0;
        foreach (var link in sets_links_ids)
        {
            npc.equipment_Set[i] = link;
            i++;
        }

        npc.equipment_Set_civilian_flag = new bool[sets_links_civ_ids.Count];

        i = 0;
        foreach (var civ_link in sets_links_civ_ids)
        {
            npc.equipment_Set_civilian_flag[i] = civ_link;
            i++;
        }

    }

    EquipmentSet GetNodeEquipmentSlots(XmlNode eqpSetNode, string slotID)
    {
        EquipmentSet set = ScriptableObject.CreateInstance<EquipmentSet>();
        foreach (XmlNode childRelation in eqpSetNode.ChildNodes)
        {
            if (childRelation.LocalName == slotID && childRelation.Attributes["slot"] != null)
            {
                if (childRelation.Attributes["slot"].Value == "Item0")
                {
                    set.eqp_Item0 = childRelation.Attributes["id"].Value;
                }
                else if (childRelation.Attributes["slot"].Value == "Item1")
                {
                    set.eqp_Item1 = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Item2")
                {
                    set.eqp_Item2 = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Item3")
                {
                    set.eqp_Item3 = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Head")
                {
                    set.eqp_Head = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Gloves")
                {
                    set.eqp_Gloves = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Body")
                {
                    set.eqp_Body = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Leg")
                {
                    set.eqp_Leg = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "Cape")
                {
                    set.eqp_Cape = childRelation.Attributes["id"].Value;


                }
                else if (childRelation.Attributes["slot"].Value == "Horse")
                {
                    set.eqp_Horse = childRelation.Attributes["id"].Value;

                }
                else if (childRelation.Attributes["slot"].Value == "HorseHarness")
                {
                    set.eqp_HorseHarness = childRelation.Attributes["id"].Value;

                }
            }

        }
        return set;
    }

    public void ReadModuleXml(ModFiles modFilesAsset, ref bool imp_cult, ref bool imp_kgd, ref bool imp_fac, ref bool imp_hero, ref bool imp_npc, ref bool imp_item, ref bool imp_eqp, ref bool imp_pt, ref bool imp_settl)
    {
        string[] XMLfiles = Directory.GetFiles(modFilesAsset.BNResourcesPath, "*.XML");

        // Read module
        foreach (string file in XMLfiles)
        {
            XmlDocument Doc = new XmlDocument();
            // UTF 8 - 16
            StreamReader reader = new StreamReader(file);
            Doc.Load(reader);
            reader.Close();

            XmlElement Root = Doc.DocumentElement;
            XmlNodeList XNL = Root.ChildNodes;


            // Debug.Log(Root.ChildNodes.Count);

            foreach (XmlNode node in Root.ChildNodes)
            {
                //Debug.Log(node.Name);
                if (node.Name == "Kingdom" && imp_kgd)
                {
                    CreateKingdomAssets(node, modFilesAsset, file);
                }

                if (node.Name == "Faction" && imp_fac)
                {
                    CreateFactionAssets(node, modFilesAsset, file);
                }

                if (node.Name == "Settlement" && imp_settl)
                {
                    // Debug.Log("Settlement");
                    CreateSettlementAssets(node, modFilesAsset, file);
                }

                if (node.Name == "NPCCharacter" && imp_npc)
                {
                    // Debug.Log("Settlement");
                    CreateNPCAsset(node, modFilesAsset, file);
                }
                if (node.Name == "Culture" && imp_cult)
                {
                    //Debug.Log(node.Name);
                    CreateCultureAsset(node, modFilesAsset, file);
                }
                if (node.Name == "MBPartyTemplate" && imp_pt)
                {
                    // Debug.Log("Settlement");
                    CreatePTAsset(node, modFilesAsset, file);
                }
                if (node.Name == "Hero" && imp_hero)
                {
                    // Debug.Log("Settlement");
                    CreateHeroAsset(node, modFilesAsset, file);
                }
                if (node.Name == "EquipmentRoster" && imp_eqp)
                {
                    // Debug.Log("Settlement");
                    CreateEquipments(node, modFilesAsset, file);
                }
                if (node.Name == "Item" || node.Name == "CraftedItem" && imp_eqp)
                {
                    CreateItemAsset(node, modFilesAsset, file);
                }
            }
        }

        // search in folders
        string[] directories = Directory.GetDirectories(modFilesAsset.BNResourcesPath);

        foreach (var dir in directories)
        {
            XMLfiles = Directory.GetFiles(dir, "*.XML");
            // Read module
            foreach (string file in XMLfiles)
            {
                // Debug.Log(file);
                XmlDocument Doc = new XmlDocument();
                // UTF 8 - 16
                StreamReader reader = new StreamReader(file);
                Doc.Load(reader);
                reader.Close();

                XmlElement Root = Doc.DocumentElement;
                XmlNodeList XNL = Root.ChildNodes;
                foreach (XmlNode node in Root.ChildNodes)
                {
                    if (node.Name == "Kingdom" && imp_kgd)
                    {
                        CreateKingdomAssets(node, modFilesAsset, file);
                    }

                    if (node.Name == "Faction" && imp_fac)
                    {
                        CreateFactionAssets(node, modFilesAsset, file);
                    }

                    if (node.Name == "Settlement" && imp_settl)
                    {
                        // Debug.Log("Settlement");
                        CreateSettlementAssets(node, modFilesAsset, file);
                    }

                    if (node.Name == "NPCCharacter" && imp_npc)
                    {
                        // Debug.Log("Settlement");
                        CreateNPCAsset(node, modFilesAsset, file);
                    }
                    if (node.Name == "Culture" && imp_cult)
                    {
                        //Debug.Log(node.Name);
                        CreateCultureAsset(node, modFilesAsset, file);
                    }
                    if (node.Name == "MBPartyTemplate" && imp_pt)
                    {
                        // Debug.Log("Settlement");
                        CreatePTAsset(node, modFilesAsset, file);
                    }
                    if (node.Name == "Hero" && imp_hero)
                    {
                        // Debug.Log("Settlement");
                        CreateHeroAsset(node, modFilesAsset, file);
                    }
                    if (node.Name == "EquipmentRoster" && imp_eqp)
                    {
                        // Debug.Log("Settlement");
                        CreateEquipments(node, modFilesAsset, file);
                    }
                    if (node.Name == "Item" || node.Name == "CraftedItem" && imp_eqp)
                    {
                        CreateItemAsset(node, modFilesAsset, file);
                    }
                }
            }
        }

        // Debug.Log("Kingdom " + kingd_Assets.Count);
        // Debug.Log("Faction " + fac_Assets.Count);
        // Debug.Log("Settlement " + settl_Assets.Count);
        // Debug.Log("NPCCharacter " + npc_Assets.Count);
        // Debug.Log("Culture " + cult_Assets.Count);
        // Debug.Log("MBPartyTemplate " + PT_Assets.Count);
        // Debug.Log("Hero " + hero_Assets.Count);
        // Debug.Log("Translation Strings " + TS_Assets.Count);
        // Debug.Log("Items " + item_Assets.Count);
        //#
        //CreateLanguages(modFilesAsset);
        //CheckSettingsLanguages(modFilesAsset);

        CreateAssetsFile(modFilesAsset);

        // CreateLanguagesData(modFilesAsset);

        // Debug.Log("Languages Translation Strings " + TS_lang_Assets.Count);
        // CreateLangAssetsFile(modFilesAsset);

        // CREATE TRANSLATION DATA for deafault language (eng)
        // CreateLanguagesData(modFilesAsset);

        //#
        //CopyLanguages(modFilesAsset);

    }

    //check settings
    public void CheckSettingsLanguages(ModFiles modFilesAsset)
    {
        BDTSettings settings = (BDTSettings)AssetDatabase.LoadAssetAtPath(settingsAsset, typeof(BDTSettings));

        foreach (ModLanguage lang in settings.LanguagesDefinitions)
        {
            string path = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/";
            string langFolderPath = path + lang.languageID;

            if (!Directory.Exists(langFolderPath))
            {
                Directory.CreateDirectory(langFolderPath);

                var assetPath = AssetDatabase.GetAssetPath(lang);
                var assetPathNew = path + Path.GetFileName(assetPath);
                AssetDatabase.CopyAsset(assetPath, assetPathNew);

                ModLanguage ML = (ModLanguage)AssetDatabase.LoadAssetAtPath(assetPathNew, typeof(ModLanguage));
                // Debug.Log(ML.languageID);

                modFilesAsset.languagesData.languages.Add(ML);
            }
        }

    }

    // CREATE FILE ASSETS
    public void CreateAssetsFile(ModFiles modFilesAsset)
    {
        foreach (Kingdom kingd in kingd_Assets.Keys)
        {
            AssetDatabase.CreateAsset(kingd, kingd_Assets[kingd]);
            modFilesAsset.kingdomsData.kingdoms.Add(kingd);
        }
        foreach (Faction fac in fac_Assets.Keys)
        {
            AssetDatabase.CreateAsset(fac, fac_Assets[fac]);
            modFilesAsset.factionsData.factions.Add(fac);
        }
        foreach (Settlement settl in settl_Assets.Keys)
        {
            AssetDatabase.CreateAsset(settl, settl_Assets[settl]);
            modFilesAsset.settlementsData.settlements.Add(settl);
        }
        foreach (NPCCharacter npc in npc_Assets.Keys)
        {
            AssetDatabase.CreateAsset(npc, npc_Assets[npc]);
            modFilesAsset.npcChrData.NPCCharacters.Add(npc);
        }
        foreach (Culture cult in cult_Assets.Keys)
        {
            AssetDatabase.CreateAsset(cult, cult_Assets[cult]);
            modFilesAsset.culturesData.cultures.Add(cult);
        }
        foreach (PartyTemplate PT in PT_Assets.Keys)
        {
            AssetDatabase.CreateAsset(PT, PT_Assets[PT]);
            modFilesAsset.PTdata.partyTemplates.Add(PT);
        }
        foreach (Hero hero in hero_Assets.Keys)
        {
            AssetDatabase.CreateAsset(hero, hero_Assets[hero]);
            modFilesAsset.heroesData.heroes.Add(hero);
        }
        foreach (EquipmentSet equip_roster in eqp_Roster_Assets.Keys)
        {
            AssetDatabase.CreateAsset(equip_roster, eqp_Roster_Assets[equip_roster]);
            modFilesAsset.equipmentSetData.equipmentSets.Add(equip_roster);
        }
        foreach (EquipmentSet equip_main in eqp_Main_Assets.Keys)
        {
            AssetDatabase.CreateAsset(equip_main, eqp_Main_Assets[equip_main]);
            modFilesAsset.equipmentSetData.equipmentSets.Add(equip_main);
        }
        foreach (EquipmentSet equipSet in equipmentSets.Keys)
        {
            AssetDatabase.CreateAsset(equipSet, equipmentSets[equipSet]);
            modFilesAsset.equipmentSetData.equipmentSets.Add(equipSet);
        }
        foreach (Equipment equip in equipments.Keys)
        {
            AssetDatabase.CreateAsset(equip, equipments[equip]);
            modFilesAsset.equipmentsData.equipmentSets.Add(equip);
        }

        for (int i = 0; i < TS_Assets.Count; i++)
        {
            var TS = TS_Assets.ToArray()[i].Key;

            if (TS.id.Contains("*"))
            {
                TS.id = TS.id.Replace("*", "");
            }

            if (TS.id != "" && TS.id != "{=}" && TS.id != "{=}{=}")
            {

                if (TS_Assets[TS].Contains("*"))
                {
                    TS_Assets[TS] = TS_Assets[TS].Replace("*", "");
                }

                AssetDatabase.CreateAsset(TS, TS_Assets[TS]);
                modFilesAsset.translationData.translationStrings.Add(TS);
            }
        }

        foreach (Item item in item_Assets.Keys)
        {
            AssetDatabase.CreateAsset(item, item_Assets[item]);
            modFilesAsset.itemsData.items.Add(item);
        }
        foreach (ModLanguage language in lang_Assets.Keys)
        {
            AssetDatabase.CreateAsset(language, lang_Assets[language]);
            modFilesAsset.languagesData.languages.Add(language);
        }
        AssetDatabase.SaveAssets();
    }
    // public void CreateLangAssetsFile(ModFiles modFilesAsset)
    // {
    //     foreach (TranslationString TS in TS_lang_Assets.Keys)
    //     {
    //         AssetDatabase.CreateAsset(TS, TS_lang_Assets[TS]);
    //         modFilesAsset.translationData.translationStrings.Add(TS);
    //     }
    //     AssetDatabase.SaveAssets();
    // }
    public void CopyLanguages(ModFiles modFilesAsset)
    {
        // string[] engXMLfiles = Directory.GetFiles(modFilesAsset.BNResourcesPath + "/Languages", "*.XML");

        string dirFrom = modFilesAsset.BNResourcesPath + "/Languages";
        string dirTo = modFilesAsset.modSettingsPath + "/Languages";

        if (System.IO.Directory.Exists(dirFrom))
        {
            if (System.IO.Directory.Exists(dirTo))
            {
                FileUtil.DeleteFileOrDirectory(dirTo + ".meta");
                FileUtil.DeleteFileOrDirectory(dirTo);
                FileUtil.CopyFileOrDirectory(dirFrom, dirTo);
            }
            else
            {
                FileUtil.CopyFileOrDirectory(dirFrom, dirTo);
            }
        }
    }


    public void CreateLanguages(ModFiles modFilesAsset)
    {
        if (System.IO.Directory.Exists(modFilesAsset.BNResourcesPath + "/Languages"))
        {


            string[] directories = Directory.GetDirectories(modFilesAsset.BNResourcesPath + "/Languages");
            foreach (var dir in directories)
            {
                string lenguageFolderName = Path.GetFileName(dir);
                string path = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName;
                Directory.CreateDirectory(path);

                string langAssetPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName + ".asset";

                ModLanguage languageAsset = ScriptableObject.CreateInstance<ModLanguage>();

                languageAsset.languageID = lenguageFolderName;
                languageAsset.languageName = "";

                string[] XMLfiles = Directory.GetFiles(dir, "*.XML");
                // get lenguage files
                foreach (string lenguageFile in XMLfiles)
                {
                    if (languageAsset.languageName == "")
                    {
                        XmlDocument Doc = new XmlDocument();
                        StreamReader reader = new StreamReader(lenguageFile);
                        Doc.Load(reader);
                        reader.Close();

                        XmlElement Root = Doc.DocumentElement;
                        XmlNodeList XML = Root.ChildNodes;

                        foreach (XmlNode langNode in XML)
                        {

                            if (langNode.Name == "tags")
                            {
                                languageAsset.languageName = langNode.FirstChild.Attributes["language"].Value;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                }

                lang_Assets.Add(languageAsset, langAssetPath);
            }

            string engFolderName = "EN";
            string engPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + engFolderName;

            Directory.CreateDirectory(engPath);

            string[] engXMLfiles = Directory.GetFiles(modFilesAsset.BNResourcesPath + "/Languages", "*.XML");
            string engAssetPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + engFolderName + ".asset";

            ModLanguage engLanguageAsset = ScriptableObject.CreateInstance<ModLanguage>();

            engLanguageAsset.languageID = engFolderName;
            engLanguageAsset.languageName = "";

            foreach (string lenguageFile in engXMLfiles)
            {
                if (engLanguageAsset.languageName == "")
                {
                    XmlDocument Doc = new XmlDocument();
                    StreamReader reader = new StreamReader(lenguageFile);
                    Doc.Load(reader);
                    reader.Close();

                    XmlElement Root = Doc.DocumentElement;
                    XmlNodeList XML = Root.ChildNodes;

                    foreach (XmlNode langNode in XML)
                    {
                        if (langNode.Name == "tags")
                        {
                            engLanguageAsset.languageName = langNode.FirstChild.Attributes["language"].Value;
                        }
                    }
                }
                else
                {
                    break;
                }

            }

            lang_Assets.Add(engLanguageAsset, engAssetPath);
        }
        else
        {
            ModLanguage engLanguageAsset = ScriptableObject.CreateInstance<ModLanguage>();
            string engAssetPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/EN.asset";
            string engPath = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/EN";

            Directory.CreateDirectory(engPath);

            engLanguageAsset.languageID = "EN";
            engLanguageAsset.languageName = "English";

            lang_Assets.Add(engLanguageAsset, engAssetPath);
        }

    }

    public void CreateLanguagesData(ModFiles modFilesAsset)
    {
        string[] engXMLfiles = Directory.GetFiles(modFilesAsset.BNResourcesPath + "/Languages", "*.XML");
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
            string lenguageFolderName = "ENG";
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
                            var soloID = idVal;
                            idVal = "{=" + idVal + "}";

                            // if translation string exist in the project DATA
                            foreach (TranslationString prjString in modFilesAsset.translationData.translationStrings)
                            {

                                if (idVal == prjString.id)
                                {
                                    // Debug.Log(idVal);
                                    TranslationString translationAsset = ScriptableObject.CreateInstance<TranslationString>();

                                    translationAsset.id = modFilesAsset.mod.id;
                                    translationAsset.id = idVal;
                                    translationAsset.stringTranslationID = soloID;
                                    translationAsset.lenguageTag = langTag;
                                    translationAsset.stringText = nodeChild.Attributes["text"].Value;
                                    translationAsset.editorType = "Imported";
                                    translationAsset.lenguage_short_Tag = lenguageFolderName;

                                    string fullPath = Path.GetFullPath(lenguageFile).TrimEnd(Path.DirectorySeparatorChar);
                                    string xmlFileName = Path.GetFileName(fullPath);
                                    translationAsset.XmlFileName = xmlFileName;

                                    modFilesAsset.translationData.translationStrings.Add(translationAsset);


                                    string path = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName + "/" + lenguageFolderName + "_{=" + nodeChild.Attributes["id"].Value + "}" + ".asset";

                                    if (!Directory.Exists(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName))
                                    {
                                        Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName);
                                    }

                                    TS_lang_Assets.Add(translationAsset, path);
                                    // AssetDatabase.CreateAsset(translationAsset, path);
                                    // AssetDatabase.SaveAssets();
                                    // Debug.Log(translationAsset);
                                    break;
                                }
                            }
                        }


                    }

                }

            }

            // Debug.Log(modFilesAsset.translationData.translationStrings.Count);
            // foreach (TranslationString prjString in modFilesAsset.translationData.translationStrings)
            // {
            // }


        }

        // Load Another Languages

        string[] lenguageDirectories = Directory.GetDirectories(modFilesAsset.BNResourcesPath + "/Languages");

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
                                var soloID = idVal;
                                idVal = "{=" + idVal + "}";

                                // if translation string exist in the project DATA
                                foreach (TranslationString prjString in modFilesAsset.translationData.translationStrings)
                                {

                                    if (idVal == prjString.id)
                                    {
                                        // Debug.Log(idVal);
                                        TranslationString translationAsset = ScriptableObject.CreateInstance<TranslationString>();

                                        translationAsset.moduleID = modFilesAsset.mod.id;
                                        translationAsset.id = idVal;
                                        translationAsset.stringTranslationID = soloID;
                                        translationAsset.lenguageTag = langTag;
                                        translationAsset.stringText = nodeChild.Attributes["text"].Value;
                                        translationAsset.editorType = "Imported";
                                        translationAsset.lenguage_short_Tag = lenguageFolderName;

                                        string fullPath = Path.GetFullPath(lenguageFile).TrimEnd(Path.DirectorySeparatorChar);
                                        string xmlFileName = Path.GetFileName(fullPath);
                                        translationAsset.XmlFileName = xmlFileName;

                                        modFilesAsset.translationData.translationStrings.Add(translationAsset);


                                        string path = modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName + "/" + lenguageFolderName + "_{=" + nodeChild.Attributes["id"].Value + "}" + ".asset";

                                        if (!Directory.Exists(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName))
                                        {
                                            Directory.CreateDirectory(modFilesAsset.modResourcesPath + "/TranslationData/_Languages/" + lenguageFolderName);
                                        }

                                        TS_lang_Assets.Add(translationAsset, path);
                                        // AssetDatabase.CreateAsset(translationAsset, path);
                                        // AssetDatabase.SaveAssets();
                                        // Debug.Log(translationAsset);
                                        break;
                                    }
                                }
                            }


                        }

                    }

                }

                // Debug.Log(modFilesAsset.translationData.translationStrings.Count);
                // foreach (TranslationString prjString in modFilesAsset.translationData.translationStrings)
                // {
                // }


            }
        }
    }
    private static void CreateTSData(XmlNode node, ModFiles modFilesAsset, string attr, string dataType, Dictionary<TranslationString, string> dic, string objID)
    {
        // NPC name & translationString tag
        if (node.Attributes[attr].Value != "")
        {

            var WorkString = node.Attributes[attr].Value;

            var plural_string = "";
            var soloName = WorkString;
            var translationString = WorkString;
            var soloID = "";

            if (WorkString.Contains("{@Plural}"))
            {
                plural_string = WorkString;
                // TS TAG - example: plural
                Regex regex_tag = new Regex(@"{@Plural}(.*){\\@}");


                var v_plu = regex_tag.Match(plural_string);
                string p_string = v_plu.Groups[1].ToString();
                // Debug.Log(p_string);

                plural_string = p_string;

                translationString = translationString.Replace("{@Plural}" + plural_string + "{\\@}", "");

                Regex regex = new Regex("{=(.*)}");

                var v = regex.Match(translationString);
                string s = v.Groups[1].ToString();
                soloID = s;

                translationString = "{=" + s + "}";

                // Debug.Log(WorkString);

            }
            else
            {

                Regex regex = new Regex("{=(.*)}");

                var v = regex.Match(translationString);
                string s = v.Groups[1].ToString();
                soloID = s;

                translationString = "{=" + s + "}";
            }


            if (translationString != "" && translationString != "{=}" && translationString != "{=*}" && translationString != "{=*}{=*}")
            {
                TranslationString translationAsset = ScriptableObject.CreateInstance<TranslationString>();

                translationAsset.moduleID = modFilesAsset.mod.id;
                translationAsset.id = translationString;
                soloName = soloName.Replace(translationString, "");
                soloName = soloName.Replace("{@Plural}" + plural_string + "{\\@}", "");
                translationAsset.stringTranslationID = soloID;
                translationAsset.translationPluralString = plural_string;
                translationAsset.stringText = soloName;
                translationAsset.editorType = "Imported";
                translationAsset.TSObjectType = objID;

                string transPath = modFilesAsset.modResourcesPath + "/TranslationData/" + dataType + "/" + translationAsset.id + ".asset";
                dic.Add(translationAsset, transPath);


                // modFilesAsset.translationData.translationStrings.Add(translationAsset);
                // AssetDatabase.CreateAsset(translationAsset, transPath);
                // AssetDatabase.SaveAssets();
            }
        }
    }

    string GetSingleNameFromString(string WorkString)
    {
        var plural_string = WorkString;
        var soloName = WorkString;
        var translationString = WorkString;
        var soloID = "";

        if (WorkString.Contains("{@Plural}"))
        {

            // TS TAG - example: plural
            Regex regex_tag = new Regex(@"{@Plural}(.*){\\@}");


            var v_plu = regex_tag.Match(plural_string);
            string p_string = v_plu.Groups[1].ToString();
            // Debug.Log(p_string);

            plural_string = p_string;

            translationString = translationString.Replace("{@Plural}" + plural_string + "{\\@}", "");

            Regex regex = new Regex("{=(.*)}");

            var v = regex.Match(translationString);
            string s = v.Groups[1].ToString();
            soloID = s;

            translationString = "{=" + s + "}";

            // Debug.Log(WorkString);

        }
        else
        {

            Regex regex = new Regex("{=(.*)}");

            var v = regex.Match(translationString);
            string s = v.Groups[1].ToString();
            soloID = s;

            translationString = "{=" + s + "}";
        }

        // soloName = soloName.Replace(translationString, "");
        soloName = soloName.Replace("{@Plural}" + plural_string + "{\\@}", "");

        return soloName;

    }
}

