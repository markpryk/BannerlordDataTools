using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class AssetsDataManager : EditorWindow
{

    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public BDTSettings bdt_settings;
    public int windowStateID;

    public bool preserveTSLinks;
    public bool overrideAsset;
    public bool renameHeroNPC = true;


    // ? object ID
    // ? ---------
    // -1 culture 
    // 1 kingdom
    // 2 faction
    // 3 NPC
    // 4 hero 
    // 5 settlement name
    // 6 partyTemplate
    // 7 item
    // 8 equipment
    public int objID;
    public object obj;

    public string assetName_org;
    public string assetName_new;

    // 

    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

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
                Debug.Log("BDT settings dont exist");
            }
        }

        if (obj != null)
            EditorUtility.SetDirty(obj as ScriptableObject);
    }
    void OnGUI()
    {

        if (windowStateID == 1)
        {
            //asset Copy and override popup 
            CopyAndOverride();
        }

        else if (windowStateID == 2)
        {
            //asset rename

            EditorUtility.SetDirty(obj as UnityEngine.Object);
            //AssetDatabase.Refresh(); 
            RenameID();
        }

        // else if (windowStateID == 21)
        // {
        //     //asset modify
        //     ModifySelectedAssets();
        // }

    }

    private void RenameID()
    {
        this.title = "Rename Asset";

        EditorGUILayout.HelpBox("Rename Asset ID, and override links to this one if they exists.", MessageType.Info);

        DrawUILine(colUILine, 2, 8);

        if (obj != null)
        {
            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

            //CULTURES 
            if (objID == -1)
            {
                Culture cult = obj as Culture;

                string originModuleID = cult.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);

                var contains = false;

                foreach (var data in currentMod.modFilesData.culturesData.cultures)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("Rename Asset"))
                    {

                        //Culture copiedCult = cult;

                        cult.id = assetName_new;
                        var renamePath = AssetDatabase.GetAssetPath(cult);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        //if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        //{
                        //    currentMod.modFilesData.culturesData.cultures.Add(copiedCult);

                        // OVERRIDE
                        CulturesReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        CulturesReplaceID(depd_mod);
                        //}
                        //}
                        //EditorUtility.SetDirty(cult);
                        EditorWindow.GetWindow<CulturesEditor>();
                        //AssetDatabase.Refresh();
						
                        this.Close();

                    }
                }

                //EditorGUILayout.EndHorizontal();
            }

            else if (objID == 1)
            {
                Kingdom kingd = obj as Kingdom;

                string originModuleID = kingd.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);

                DrawUILine(colUILine, 2, 8);

                EditorGUILayout.BeginHorizontal();

                var contains = false;

                foreach (var data in currentMod.modFilesData.kingdomsData.kingdoms)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("Rename Asset"))
                    {

                        //Kingdom copiedKingd = kingd;
                        //Debug.Log(assetName_new);
                        kingd.id = assetName_new;
                        //Debug.Log(kingd.id);
                        var renamePath = AssetDatabase.GetAssetPath(kingd);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        //if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        //{

                        //    currentMod.modFilesData.kingdomsData.kingdoms.Add(copiedKingd);

                        // OVERRIDE

                        KingdomReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        KingdomReplaceID(depd_mod);
                        //}


                        //}
                        //EditorUtility.SetDirty(kingd);
                        EditorWindow.GetWindow<KingdomsEditor>();
                        //AssetDatabase.Refresh();

                        this.Close();

                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // FACTIONS
            else if (objID == 2)
            {
                Faction fac = obj as Faction;

                string originModuleID = fac.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);

                DrawUILine(colUILine, 2, 8);

                var contains = false;

                foreach (var data in currentMod.modFilesData.factionsData.factions)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("Rename Asset"))
                    {

                        //Faction copiedFac = fac;

                        fac.id = assetName_new;

                        var renamePath = AssetDatabase.GetAssetPath(fac);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        FactionReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        FactionReplaceID(depd_mod);
                        //}
                        //EditorUtility.SetDirty(fac);
                        EditorWindow.GetWindow<FactionEditor>();
                        //AssetDatabase.Refresh();

                        this.Close();

                    }
                }

            }
            else if (objID == 3)
            {
                NPCCharacter npc = obj as NPCCharacter;

                string originModuleID = npc.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);

                if (npc.is_hero == "true")
                {
                    renameHeroNPC = EditorGUILayout.Toggle("Rename Hero ID also", renameHeroNPC);
                    EditorGUILayout.Space(4);
                }
                else
                {
                    renameHeroNPC = false;
                }

                DrawUILine(colUILine, 2, 8);

                var contains = false;

                foreach (var data in currentMod.modFilesData.npcChrData.NPCCharacters)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("Rename Asset"))
                    {

                        for (int rst = 0; rst < npc.equipment_Roster.Length; rst++)
                        {
                            foreach (var roster_set in currentMod.modFilesData.equipmentSetData.equipmentSets)
                            {
                                if (roster_set.name == npc.equipment_Roster[rst])
                                {
                                    EditorUtility.SetDirty(roster_set);
                                    roster_set.EquipmentSetID = assetName_new;
                                    var rp = AssetDatabase.GetAssetPath(roster_set);
                                    var naming = npc.equipment_Roster[rst].Replace(npc.id, assetName_new);
                                    npc.equipment_Roster[rst] = naming;
                                    AssetDatabase.RenameAsset(rp, naming);
                                }
                            }

                        }

                        if (npc.equipment_Main != "")
                        {
                            foreach (var roster_set in currentMod.modFilesData.equipmentSetData.equipmentSets)
                            {
                                if (roster_set.name == npc.equipment_Main)
                                {
                                    EditorUtility.SetDirty(roster_set);
                                    roster_set.EquipmentSetID = assetName_new;
                                    var rp = AssetDatabase.GetAssetPath(roster_set);
                                    var naming = npc.equipment_Main.Replace(npc.id, assetName_new);
                                    npc.equipment_Main = naming;
                                    AssetDatabase.RenameAsset(rp, naming);
                                }
                            }
                        }

                        npc.id = assetName_new;

                        var renamePath = AssetDatabase.GetAssetPath(npc);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        NPCReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        NPCReplaceID(depd_mod);
                        //}

                        //EditorUtility.SetDirty(npc);
                        EditorWindow.GetWindow<NPCEditor>();
                       // AssetDatabase.Refresh();

                        this.Close();

                    }
                }
            }

            // HERO
            else if (objID == 4)
            {
                Hero hero = obj as Hero;

                string originModuleID = hero.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);

                renameHeroNPC = EditorGUILayout.Toggle("Rename NPC ID also", renameHeroNPC);
                EditorGUILayout.Space(4);

                DrawUILine(colUILine, 2, 8);

                var contains = false;

                foreach (var data in currentMod.modFilesData.heroesData.heroes)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename Asset"))
                    {

                        //Hero copiedHero = hero;

                        hero.id = assetName_new;

                        var renamePath = AssetDatabase.GetAssetPath(hero);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        //if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        //{
                        // currentMod.modFilesData.heroesData.heroes.Add(copiedHero);


                        // OVERRIDE
                        HeroReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        HeroReplaceID(depd_mod);
                        //}

                        //EditorUtility.SetDirty(hero);
                        EditorWindow.GetWindow<NPCEditor>();
                       // AssetDatabase.Refresh();

                        this.Close();

                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // Settlement
            else if (objID == 5)
            {
                Settlement settl = obj as Settlement;

                string originModuleID = settl.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);

                DrawUILine(colUILine, 2, 8);

                var contains = false;

                foreach (var data in currentMod.modFilesData.settlementsData.settlements)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename Asset"))
                    {
                        //Settlement copiedSettl = settl;

                        settl.id = assetName_new;
                        var renamePath = AssetDatabase.GetAssetPath(settl);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        //if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        //{
                        //    currentMod.modFilesData.settlementsData.settlements.Add(copiedSettl);

                        // OVERRIDE
                        SettlementReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        SettlementReplaceID(depd_mod);
                        //}

                        //}
                        //EditorUtility.SetDirty(settl);
                        EditorWindow.GetWindow<SettlementsEditor>();
                        //AssetDatabase.Refresh();

                        this.Close();

                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // Party Template
            else if (objID == 6)
            {
                PartyTemplate PT = obj as PartyTemplate;

                string originModuleID = PT.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);

                DrawUILine(colUILine, 2, 8);

                var contains = false;

                foreach (var data in currentMod.modFilesData.PTdata.partyTemplates)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename Asset"))
                    {

                        //PartyTemplate copiedPT = PT;

                        PT.id = assetName_new;
                        var renamePath = AssetDatabase.GetAssetPath(PT);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        //if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        //{

                        //currentMod.modFilesData.PTdata.partyTemplates.Add(copiedPT);

                        // OVERRIDE
                        PartyTemplateReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        PartyTemplateReplaceID(depd_mod);
                        //}

                        //}
                        //EditorUtility.SetDirty(PT);
                        EditorWindow.GetWindow<PartyTemplatesEditor>();
                       // AssetDatabase.Refresh();

                        this.Close();

                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // ITEMS
            else if (objID == 7)
            {
                Item item = obj as Item;

                string originModuleID = item.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);

                EditorGUILayout.Space(4);
                DrawUILine(colUILine, 2, 8);

                var contains = false;

                foreach (var data in currentMod.modFilesData.itemsData.items)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }
                EditorGUILayout.BeginHorizontal();

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("Rename Asset"))
                    {


                        //Item copiedItem = item;

                        item.id = assetName_new;
                        var renamePath = AssetDatabase.GetAssetPath(item);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        //if (preserveTSLinks == false)
                        //{
                        //    DontPreserveStringLinks(ref item.itemName);
                        //}

                        //if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        //{
                        //    currentMod.modFilesData.itemsData.items.Add(copiedItem);

                        //    // OVERRIDE
                        ItemReplaceID(currentMod);

                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        ItemReplaceID(depd_mod);
                        //}

                        //}
                        //EditorUtility.SetDirty(item);
                        EditorWindow.GetWindow<ItemsEditor>();
                      //  AssetDatabase.Refresh();

                        this.Close();

                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // EQUIPMENTS
            else if (objID == 8)
            {
                Equipment equip = obj as Equipment;

                string originModuleID = equip.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);

                EditorGUILayout.Space(4);
                DrawUILine(colUILine, 2, 8);

                var contains = false;
                foreach (var data in currentMod.modFilesData.itemsData.items)
                {
                    if (data.id == assetName_new)
                    {
                        contains = true;
                        break;
                    }
                }

                if (contains)
                {
                    EditorGUILayout.HelpBox($"This ID already exist in {currentMod.id} module. Change it, if you want rename.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename Asset"))
                    {
                        //Equipment copiedEquipemnt = equip;

                        for (int rst = 0; rst < equip.eqpSetID.Length; rst++)
                        {
                            foreach (var roster_set in currentMod.modFilesData.equipmentSetData.equipmentSets)
                            {
                                if (roster_set.name == equip.eqpSetID[rst])
                                {
                                    EditorUtility.SetDirty(roster_set);
                                    roster_set.EquipmentSetID = assetName_new;
                                    var rp = AssetDatabase.GetAssetPath(roster_set);
                                    var naming = equip.eqpSetID[rst].Replace(equip.id, assetName_new);
                                    equip.eqpSetID[rst] = naming;
                                    AssetDatabase.RenameAsset(rp, naming);
                                }
                            }

                        }

                        equip.id = assetName_new;
                        var renamePath = AssetDatabase.GetAssetPath(equip);
                        AssetDatabase.RenameAsset(renamePath, assetName_new);

                        EquipmentReplaceID(currentMod);
                        //foreach (var mod in currentMod.modDependencies)
                        //{
                        //    ModuleReceiver depd_mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + mod + ".asset", typeof(ModuleReceiver));

                        //    if (depd_mod != null)
                        //        EquipmentReplaceID(depd_mod);
                        //}

                        //EditorUtility.SetDirty(equip);
                        EditorWindow.GetWindow<EquipmentsEditor>();
                      //  AssetDatabase.Refresh();

                        this.Close();

                    }
                }
                EditorGUILayout.EndHorizontal();
            }

        }



    }

    private void EquipmentReplaceID(ModuleReceiver currentMod)
    {
        foreach (var character in currentMod.modFilesData.npcChrData.NPCCharacters)
        {
            for (int index = 0; index < character.equipment_Set.Length; index++)
            {
                OverrideAsset(ref character.equipment_Set[index], "", assetName_org, assetName_new);
            }
        }
    }

    private void ItemReplaceID(ModuleReceiver currentMod)
    {
        foreach (var set in currentMod.modFilesData.equipmentSetData.equipmentSets)
        {
            OverrideAsset(ref set.eqp_Body, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Cape, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Gloves, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Head, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Horse, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_HorseHarness, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Item0, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Item1, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Item2, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Item3, "Item.", assetName_org, assetName_new);
            OverrideAsset(ref set.eqp_Leg, "Item.", assetName_org, assetName_new);
        }
    }

    private void PartyTemplateReplaceID(ModuleReceiver currentMod)
    {
        foreach (var cult in currentMod.modFilesData.culturesData.cultures)
        {
            OverrideAsset(ref cult.default_party_template, "PartyTemplate.", assetName_org, assetName_new);
            OverrideAsset(ref cult.villager_party_template, "PartyTemplate.", assetName_org, assetName_new);
            OverrideAsset(ref cult.elite_caravan_party_template, "PartyTemplate.", assetName_org, assetName_new);
            OverrideAsset(ref cult.elite_caravan_party_template, "PartyTemplate.", assetName_org, assetName_new);
            OverrideAsset(ref cult.militia_party_template, "PartyTemplate.", assetName_org, assetName_new);
            OverrideAsset(ref cult.rebels_party_template, "PartyTemplate.", assetName_org, assetName_new);
        }

        foreach (var fac in currentMod.modFilesData.factionsData.factions)
        {
            OverrideAsset(ref fac.default_party_template, "PartyTemplate.", assetName_org, assetName_new);

        }
    }

    private void SettlementReplaceID(ModuleReceiver currentMod)
    {
        foreach (var settlement in currentMod.modFilesData.settlementsData.settlements)
        {
            OverrideAsset(ref settlement.CMP_bound, "Settlement.", assetName_org, assetName_new);
            OverrideAsset(ref settlement.CMP_trade_bound, "Settlement.", assetName_org, assetName_new);
        }
    }

    private void HeroReplaceID(ModuleReceiver currentMod)
    {
        foreach (var fac in currentMod.modFilesData.factionsData.factions)
        {
            OverrideAsset(ref fac.owner, "Hero.", assetName_org, assetName_new);
        }


        foreach (var heroChar in currentMod.modFilesData.heroesData.heroes)
        {
            OverrideAsset(ref heroChar.father, "Hero.", assetName_org, assetName_new);
            OverrideAsset(ref heroChar.mother, "Hero.", assetName_org, assetName_new);
            OverrideAsset(ref heroChar.spouse, "Hero.", assetName_org, assetName_new);
        }

        foreach (var kingd in currentMod.modFilesData.kingdomsData.kingdoms)
        {
            OverrideAsset(ref kingd.owner, "Hero.", assetName_org, assetName_new);
        }

        if (renameHeroNPC)
        {
            foreach (var npcCharacter in currentMod.modFilesData.npcChrData.NPCCharacters)
            {
                if (npcCharacter.id == assetName_org)
                {
                    npcCharacter.id = assetName_new;
                    // Debug.Log("rename Path");
                    var renamePathNPC = AssetDatabase.GetAssetPath(npcCharacter);
                    AssetDatabase.RenameAsset(renamePathNPC, assetName_new);
                    break;
                }
            }
        }
    }

    private void NPCReplaceID(ModuleReceiver currentMod)
    {
        foreach (var cult in currentMod.modFilesData.culturesData.cultures)
        {
            // search in battle presets
            OverrideAsset(ref cult.elite_basic_troop, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.basic_troop, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.melee_militia_troop, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.ranged_militia_troop, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.melee_elite_militia_troop, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.ranged_elite_militia_troop, "NPCCharacter.", assetName_org, assetName_new);

            // search in civilian presets
            OverrideAsset(ref cult.tournament_master, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.villager, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.caravan_master, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.armed_trader, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.caravan_guard, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.veteran_caravan_guard, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.duel_preset, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.prison_guard, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.guard, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.steward, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.blacksmith, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.weaponsmith, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townswoman, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townswoman_infant, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townswoman_child, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townswoman_teenager, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townsman, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townsman_infant, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townsman_child, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.village_woman, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.villager_male_child, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.villager_male_teenager, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.villager_female_child, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.villager_female_teenager, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.townsman_teenager, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.ransom_broker, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.gangleader_bodyguard, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.merchant_notary, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.artisan_notary, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.preacher_notary, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.rural_notable_notary, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.shop_worker, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.tavernkeeper, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.taverngamehost, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.musician, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.tavern_wench, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.armorer, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.horseMerchant, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.barber, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.merchant, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.beggar, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.female_beggar, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.female_dancer, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.gear_practice_dummy, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.weapon_practice_stage_1, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.weapon_practice_stage_2, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.weapon_practice_stage_3, "NPCCharacter.", assetName_org, assetName_new);
            OverrideAsset(ref cult.gear_dummy, "NPCCharacter.", assetName_org, assetName_new);

            //}

            foreach (var npcChar in currentMod.modFilesData.npcChrData.NPCCharacters)
            {
                OverrideAsset(ref npcChar.skill_template, "NPCCharacter.", assetName_org, assetName_new);
                OverrideAsset(ref npcChar.battleTemplate, "NPCCharacter.", assetName_org, assetName_new);
                OverrideAsset(ref npcChar.civilianTemplate, "NPCCharacter.", assetName_org, assetName_new);
            }

            if (renameHeroNPC)
            {
                foreach (var heroCharacter in currentMod.modFilesData.heroesData.heroes)
                {
                    if (heroCharacter.id == assetName_org)
                    {
                        heroCharacter.id = assetName_new;
                        // Debug.Log("rename Path");
                        var renamePathNPC = AssetDatabase.GetAssetPath(heroCharacter);
                        AssetDatabase.RenameAsset(renamePathNPC, assetName_new);
                        break;
                    }
                }
            }


        }
    }

    private void FactionReplaceID(ModuleReceiver currentMod)
    {
        foreach (var hero in currentMod.modFilesData.heroesData.heroes)
        {
            OverrideAsset(ref hero.faction, "Faction.", assetName_org, assetName_new);
        }

        foreach (var settl in currentMod.modFilesData.settlementsData.settlements)
        {
            OverrideAsset(ref settl.owner, "Faction.", assetName_org, assetName_new);
        }
    }

    private void CulturesReplaceID(ModuleReceiver currentMod)
    {
        foreach (var fac in currentMod.modFilesData.factionsData.factions)
        {

            OverrideAsset(ref fac.culture, "Culture.", assetName_org, assetName_new);
        }

        foreach (var item in currentMod.modFilesData.itemsData.items)
        {

            OverrideAsset(ref item.culture, "Culture.", assetName_org, assetName_new);
        }

        foreach (var kngd in currentMod.modFilesData.kingdomsData.kingdoms)
        {

            OverrideAsset(ref kngd.culture, "Culture.", assetName_org, assetName_new);
        }

        foreach (var npc in currentMod.modFilesData.npcChrData.NPCCharacters)
        {

            OverrideAsset(ref npc.culture, "Culture.", assetName_org, assetName_new);
        }

        foreach (var settl in currentMod.modFilesData.settlementsData.settlements)
        {

            OverrideAsset(ref settl.culture, "Culture.", assetName_org, assetName_new);
        }
    }

    private void KingdomReplaceID(ModuleReceiver currentMod)
    {
        foreach (var fac in currentMod.modFilesData.factionsData.factions)
        {
            OverrideAsset(ref fac.super_faction, "Kingdom.", assetName_org, assetName_new);
        }

        foreach (var kgd in currentMod.modFilesData.kingdomsData.kingdoms)
        {
            for (int kingdom_id = 0; kingdom_id < kgd.relationships.Length; kingdom_id++)
            {
                OverrideAsset(ref kgd.relationships[kingdom_id], "Kingdom.", assetName_org, assetName_new);
            }
        }
    }

    private void CopyAndOverride()
    {
        this.title = "Copy Asset";

        EditorGUILayout.HelpBox("Copy asset to current " + "(" + bdt_settings.currentModule + ")" + " module, and override links to this one if they exists.", MessageType.Info);

        DrawUILine(colUILine, 2, 8);

        if (obj != null)
        {

            //CULTURES 
            if (objID == -1)
            {
                Culture cult = obj as Culture;

                string originModuleID = cult.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 2, 8);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(cult);
                        var newPath = dataPath + bdt_settings.currentModule + "/Cultures/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Culture copiedCult = (Culture)AssetDatabase.LoadAssetAtPath(newPath, typeof(Culture));

                        copiedCult.id = assetName_new;
                        copiedCult.moduleID = bdt_settings.currentModule;

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.culturesData.cultures.Add(copiedCult);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(cult);
                        var newPath = dataPath + bdt_settings.currentModule + "/Cultures/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Culture copiedCult = (Culture)AssetDatabase.LoadAssetAtPath(newPath, typeof(Culture));

                        copiedCult.id = assetName_new;
                        copiedCult.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedCult.cultureName);
                            DontPreserveStringLinks(ref copiedCult.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.culturesData.cultures.Add(copiedCult);

                            // OVERRIDE
                            foreach (var fac in currentMod.modFilesData.factionsData.factions)
                            {

                                OverrideAsset(ref fac.culture, "Culture.", assetName_org, assetName_new);
                            }

                            foreach (var item in currentMod.modFilesData.itemsData.items)
                            {

                                OverrideAsset(ref item.culture, "Culture.", assetName_org, assetName_new);
                            }

                            foreach (var kngd in currentMod.modFilesData.kingdomsData.kingdoms)
                            {

                                OverrideAsset(ref kngd.culture, "Culture.", assetName_org, assetName_new);
                            }

                            foreach (var npc in currentMod.modFilesData.npcChrData.NPCCharacters)
                            {

                                OverrideAsset(ref npc.culture, "Culture.", assetName_org, assetName_new);
                            }

                            foreach (var settl in currentMod.modFilesData.settlementsData.settlements)
                            {

                                OverrideAsset(ref settl.culture, "Culture.", assetName_org, assetName_new);
                            }
                        }
                        this.Close();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            else if (objID == 1)
            {
                Kingdom kingd = obj as Kingdom;

                string originModuleID = kingd.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 2, 8);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(kingd);
                        var newPath = dataPath + bdt_settings.currentModule + "/Kingdoms/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Kingdom copiedKingd = (Kingdom)AssetDatabase.LoadAssetAtPath(newPath, typeof(Kingdom));

                        copiedKingd.id = assetName_new;
                        copiedKingd.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedKingd.kingdomName);
                            DontPreserveStringLinks(ref copiedKingd.text);
                            DontPreserveStringLinks(ref copiedKingd.short_name);
                            DontPreserveStringLinks(ref copiedKingd.title);
                            DontPreserveStringLinks(ref copiedKingd.ruler_title);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.kingdomsData.kingdoms.Add(copiedKingd);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(kingd);
                        var newPath = dataPath + bdt_settings.currentModule + "/Kingdoms/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Kingdom copiedKingd = (Kingdom)AssetDatabase.LoadAssetAtPath(newPath, typeof(Kingdom));

                        copiedKingd.id = assetName_new;
                        copiedKingd.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedKingd.kingdomName);
                            DontPreserveStringLinks(ref copiedKingd.text);
                            DontPreserveStringLinks(ref copiedKingd.short_name);
                            DontPreserveStringLinks(ref copiedKingd.title);
                            DontPreserveStringLinks(ref copiedKingd.ruler_title);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.kingdomsData.kingdoms.Add(copiedKingd);

                            // OVERRIDE
                            foreach (var fac in currentMod.modFilesData.factionsData.factions)
                            {
                                OverrideAsset(ref fac.super_faction, "Kingdom.", assetName_org, assetName_new);
                            }

                        }
                        this.Close();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // FACTIONS
            else if (objID == 2)
            {
                Faction fac = obj as Faction;

                string originModuleID = fac.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 2, 8);


                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(fac);
                        var newPath = dataPath + bdt_settings.currentModule + "/Factions/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Faction copiedFac = (Faction)AssetDatabase.LoadAssetAtPath(newPath, typeof(Faction));

                        copiedFac.id = assetName_new;
                        copiedFac.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedFac.factionName);
                            DontPreserveStringLinks(ref copiedFac.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.factionsData.factions.Add(copiedFac);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(fac);
                        var newPath = dataPath + bdt_settings.currentModule + "/Factions/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Faction copiedFac = (Faction)AssetDatabase.LoadAssetAtPath(newPath, typeof(Faction));

                        copiedFac.id = assetName_new;
                        copiedFac.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedFac.factionName);
                            DontPreserveStringLinks(ref copiedFac.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.factionsData.factions.Add(copiedFac);

                            // OVERRIDE
                            foreach (var hero in currentMod.modFilesData.heroesData.heroes)
                            {
                                OverrideAsset(ref hero.faction, "Faction.", assetName_org, assetName_new);
                            }

                            foreach (var settl in currentMod.modFilesData.settlementsData.settlements)
                            {
                                OverrideAsset(ref settl.owner, "Faction.", assetName_org, assetName_new);
                            }
                        }
                        this.Close();
                    }
                }


            }
            else if (objID == 3)
            {
                NPCCharacter npc = obj as NPCCharacter;

                string originModuleID = npc.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 2, 8);
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {

                        var originPath = AssetDatabase.GetAssetPath(npc);
                        var newPath = dataPath + bdt_settings.currentModule + "/NPC/" + npc.id + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        NPCCharacter copiedNPC = (NPCCharacter)AssetDatabase.LoadAssetAtPath(newPath, typeof(NPCCharacter));

                        copiedNPC.id = assetName_new;
                        copiedNPC.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedNPC.npcName);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.npcChrData.NPCCharacters.Add(copiedNPC);
                        }

                        this.Close();

                    }
                    else
                    {

                        var originPath = AssetDatabase.GetAssetPath(npc);
                        var newPath = dataPath + bdt_settings.currentModule + "/NPC/" + npc.id + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        NPCCharacter copiedNPC = (NPCCharacter)AssetDatabase.LoadAssetAtPath(newPath, typeof(NPCCharacter));

                        copiedNPC.id = assetName_new;
                        copiedNPC.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedNPC.npcName);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            foreach (var cult in currentMod.modFilesData.culturesData.cultures)
                            {
                                // search in battle presets
                                OverrideAsset(ref cult.elite_basic_troop, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.basic_troop, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.melee_militia_troop, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.ranged_militia_troop, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.melee_elite_militia_troop, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.ranged_elite_militia_troop, "NPCCharacter.", assetName_org, assetName_new);

                                // search in civilian presets
                                OverrideAsset(ref cult.tournament_master, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.villager, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.caravan_master, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.armed_trader, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.caravan_guard, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.veteran_caravan_guard, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.duel_preset, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.prison_guard, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.guard, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.steward, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.blacksmith, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.weaponsmith, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townswoman, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townswoman_infant, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townswoman_child, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townswoman_teenager, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townsman, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townsman_infant, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townsman_child, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.village_woman, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.villager_male_child, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.villager_male_teenager, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.villager_female_child, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.villager_female_teenager, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.townsman_teenager, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.ransom_broker, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.gangleader_bodyguard, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.merchant_notary, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.artisan_notary, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.preacher_notary, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.rural_notable_notary, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.shop_worker, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.tavernkeeper, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.taverngamehost, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.musician, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.tavern_wench, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.armorer, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.horseMerchant, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.barber, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.merchant, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.beggar, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.female_beggar, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.female_dancer, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.gear_practice_dummy, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.weapon_practice_stage_1, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.weapon_practice_stage_2, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.weapon_practice_stage_3, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.gear_dummy, "NPCCharacter.", assetName_org, assetName_new);

                            }

                            foreach (var npcChar in currentMod.modFilesData.npcChrData.NPCCharacters)
                            {
                                OverrideAsset(ref npcChar.skill_template, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref npcChar.battleTemplate, "NPCCharacter.", assetName_org, assetName_new);
                                OverrideAsset(ref npcChar.civilianTemplate, "NPCCharacter.", assetName_org, assetName_new);
                            }
                        }
                        this.Close();
                    }
                }
            }

            // HERO
            else if (objID == 4)
            {
                Hero hero = obj as Hero;

                string originModuleID = hero.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 2, 8);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(hero);
                        var newPath = dataPath + bdt_settings.currentModule + "/Heroes/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Hero copiedHero = (Hero)AssetDatabase.LoadAssetAtPath(newPath, typeof(Hero));

                        copiedHero.id = assetName_new;
                        copiedHero.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedHero.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.heroesData.heroes.Add(copiedHero);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(hero);
                        var newPath = dataPath + bdt_settings.currentModule + "/Heroes/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Hero copiedHero = (Hero)AssetDatabase.LoadAssetAtPath(newPath, typeof(Hero));

                        copiedHero.id = assetName_new;
                        copiedHero.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedHero.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.heroesData.heroes.Add(copiedHero);

                            if (currentMod.modFilesData.factionsData.factions.Count != 0)
                            {
                                // OVERRIDE
                                foreach (var fac in currentMod.modFilesData.factionsData.factions)
                                {
                                    OverrideAsset(ref fac.owner, "Hero.", assetName_org, assetName_new);
                                }
                            }

                            foreach (var heroChar in currentMod.modFilesData.heroesData.heroes)
                            {
                                OverrideAsset(ref heroChar.father, "Hero.", assetName_org, assetName_new);
                                OverrideAsset(ref heroChar.mother, "Hero.", assetName_org, assetName_new);
                                OverrideAsset(ref heroChar.spouse, "Hero.", assetName_org, assetName_new);
                            }

                            foreach (var kingd in currentMod.modFilesData.kingdomsData.kingdoms)
                            {
                                OverrideAsset(ref kingd.owner, "Hero.", assetName_org, assetName_new);
                            }
                        }
                        this.Close();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // Settlement
            else if (objID == 5)
            {
                Settlement settl = obj as Settlement;

                string originModuleID = settl.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                DrawUILine(colUILine, 2, 8);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(settl);
                        var newPath = dataPath + bdt_settings.currentModule + "/Settlements/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Settlement copiedSettl = (Settlement)AssetDatabase.LoadAssetAtPath(newPath, typeof(Settlement));

                        copiedSettl.id = assetName_new;
                        copiedSettl.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedSettl.settlementName);
                            DontPreserveStringLinks(ref copiedSettl.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.settlementsData.settlements.Add(copiedSettl);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(settl);
                        var newPath = dataPath + bdt_settings.currentModule + "/Settlements/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Settlement copiedSettl = (Settlement)AssetDatabase.LoadAssetAtPath(newPath, typeof(Settlement));

                        copiedSettl.id = assetName_new;
                        copiedSettl.moduleID = bdt_settings.currentModule;


                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedSettl.settlementName);
                            DontPreserveStringLinks(ref copiedSettl.text);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.settlementsData.settlements.Add(copiedSettl);

                            // OVERRIDE
                            // foreach (var fac in currentMod.modFilesData.factionsData.factions)
                            // {
                            //     OverrideAsset(ref fac.owner, "Hero.", assetName_org, assetName_new);
                            // }

                        }
                        this.Close();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // Party Template
            else if (objID == 6)
            {
                PartyTemplate PT = obj as PartyTemplate;

                string originModuleID = PT.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);

                preserveTSLinks = false;

                // EditorGUILayout.Space(4);
                // EditorGUILayout.BeginHorizontal();
                // EditorGUILayout.LabelField("Preserve Translation Strings Links:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                // EditorGUILayout.Space(-32);
                // preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                // EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                DrawUILine(colUILine, 2, 8);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(PT);
                        var newPath = dataPath + bdt_settings.currentModule + "/PartyTemplates/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        PartyTemplate copiedPT = (PartyTemplate)AssetDatabase.LoadAssetAtPath(newPath, typeof(PartyTemplate));

                        copiedPT.id = assetName_new;
                        copiedPT.moduleID = bdt_settings.currentModule;

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.PTdata.partyTemplates.Add(copiedPT);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(PT);
                        var newPath = dataPath + bdt_settings.currentModule + "/PartyTemplates/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        PartyTemplate copiedPT = (PartyTemplate)AssetDatabase.LoadAssetAtPath(newPath, typeof(PartyTemplate));

                        copiedPT.id = assetName_new;
                        copiedPT.moduleID = bdt_settings.currentModule;

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.PTdata.partyTemplates.Add(copiedPT);

                            // OVERRIDE
                            foreach (var cult in currentMod.modFilesData.culturesData.cultures)
                            {
                                OverrideAsset(ref cult.default_party_template, "PartyTemplate.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.villager_party_template, "PartyTemplate.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.elite_caravan_party_template, "PartyTemplate.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.elite_caravan_party_template, "PartyTemplate.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.militia_party_template, "PartyTemplate.", assetName_org, assetName_new);
                                OverrideAsset(ref cult.rebels_party_template, "PartyTemplate.", assetName_org, assetName_new);
                            }

                            foreach (var fac in currentMod.modFilesData.factionsData.factions)
                            {
                                OverrideAsset(ref fac.default_party_template, "PartyTemplate.", assetName_org, assetName_new);

                            }

                        }
                        this.Close();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // ITEMS
            else if (objID == 7)
            {
                Item item = obj as Item;

                string originModuleID = item.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                DrawUILine(colUILine, 2, 8);


                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(item);
                        var newPath = dataPath + bdt_settings.currentModule + "/Items/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Item copiedItem = (Item)AssetDatabase.LoadAssetAtPath(newPath, typeof(Item));

                        copiedItem.id = assetName_new;
                        copiedItem.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedItem.itemName);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.itemsData.items.Add(copiedItem);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(item);
                        var newPath = dataPath + bdt_settings.currentModule + "/Items/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Item copiedItem = (Item)AssetDatabase.LoadAssetAtPath(newPath, typeof(Item));

                        copiedItem.id = assetName_new;
                        copiedItem.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedItem.itemName);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.itemsData.items.Add(copiedItem);

                            // OVERRIDE
                            // foreach (var cult in currentMod.modFilesData.culturesData.cultures)
                            // {

                            // }



                        }
                        this.Close();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // EQUIPMENTS
            else if (objID == 8)
            {
                Equipment equip = obj as Equipment;

                string originModuleID = equip.moduleID;

                assetName_new = EditorGUILayout.TextField("New Asset ID", assetName_new);

                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override asset at copy:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.Space(-60);
                overrideAsset = EditorGUILayout.Toggle(overrideAsset, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                //EditorGUILayout.Space(4);
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField("Preserve Translation Keys:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                //EditorGUILayout.Space(-60);
                //preserveTSLinks = EditorGUILayout.Toggle(preserveTSLinks, GUILayout.ExpandWidth(false));
                //EditorGUILayout.EndHorizontal();

                DrawUILine(colUILine, 2, 8);


                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Asset"))
                {
                    if (overrideAsset == false)
                    {
                        var originPath = AssetDatabase.GetAssetPath(equip);
                        var newPath = dataPath + bdt_settings.currentModule + "/Sets/Equipments/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Equipment copiedEquipment = (Equipment)AssetDatabase.LoadAssetAtPath(newPath, typeof(Equipment));

                        copiedEquipment.id = assetName_new;
                        copiedEquipment.moduleID = bdt_settings.currentModule;

                        //if (preserveTSLinks == false)
                        //{
                        //    DontPreserveStringLinks(ref copiedEquipment.itemName);
                        //}

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.equipmentsData.equipmentSets.Add(copiedEquipment);
                        }

                        this.Close();
                    }
                    else
                    {
                        var originPath = AssetDatabase.GetAssetPath(equip);
                        var newPath = dataPath + bdt_settings.currentModule + "/Sets/Equipments/" + assetName_new + ".asset";

                        AssetDatabase.CopyAsset(originPath, newPath);

                        Item copiedItem = (Item)AssetDatabase.LoadAssetAtPath(newPath, typeof(Item));

                        copiedItem.id = assetName_new;
                        copiedItem.moduleID = bdt_settings.currentModule;

                        if (preserveTSLinks == false)
                        {
                            DontPreserveStringLinks(ref copiedItem.itemName);
                        }

                        if (System.IO.File.Exists(modsSettingsPath + bdt_settings.currentModule + ".asset"))
                        {
                            ModuleReceiver currentMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + bdt_settings.currentModule + ".asset", typeof(ModuleReceiver));

                            currentMod.modFilesData.itemsData.items.Add(copiedItem);

                            // OVERRIDE
                            // foreach (var cult in currentMod.modFilesData.culturesData.cultures)
                            // {

                            // }



                        }
                        this.Close();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

        }

    }

    static void DontPreserveStringLinks(ref string stringTS)
    {
        var soloString = stringTS;
        var TS_Name = stringTS;
        Regex regex = new Regex("{=(.*)}");
        if (regex != null)
        {
            var v = regex.Match(TS_Name);
            string s = v.Groups[1].ToString();
            TS_Name = "{=" + s + "}";
        }
        stringTS = soloString.Replace(TS_Name, "");
    }

    private void OverrideAsset(ref string assetString, string assetType, string orgID, string newID)
    {
        if (assetString.Contains(orgID))
        {
            assetString = assetType + newID;
        }
    }

    private static void CreateEditorToggle(ref bool toggleBool, string label)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        toggleBool = EditorGUILayout.Toggle(label, toggleBool);

        EditorGUIUtility.labelWidth = originLabelWidth;

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
    void CreateFloatAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        float val;
        float.TryParse(attribute, out val);
        val = EditorGUILayout.FloatField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

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
