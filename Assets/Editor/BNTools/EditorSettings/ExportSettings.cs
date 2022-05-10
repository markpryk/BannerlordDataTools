using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExportSettings : ScriptableObject
{
    public string translationXml_Tag = "std_";
    public string Culture_xml_name = "spcultures.xml";
    public string Faction_xml_name = "spclans.xml";
    public string Hero_xml_name = "heroes.xml";
    public string Kingdom_xml_name = "spkingdoms.xml";
    public string NPCCharacter_xml_name = "spnpccharacters.xml";
    public string PartyTemplate_xml_name = "partyTemplates.xml";
    public string Settlement_xml_name = "settlements.xml";
    public string Item_xml_name = "items.xml";
    public string EquipmentSet_xml_name = "equipment_sets.xml";

    public bool export_cult = true;
    public bool export_fac = true;
    public bool export_hero = true;
    public bool export_kingd = true;
    public bool export_npc = true;
    public bool export_pt = true;
    public bool export_settl = true;
    public bool export_item = true;
    public bool export_equip = true;

    public bool checkOverrides;
    public bool createBackUp;

    public bool exportDataToScene;
    public bool createEntities;
    public bool centerIconCapsules;
    public bool createHigtMapData;
    public bool settlToZero;
}
