using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "NPCCharacter", menuName = "ModulesData/NPCCharacter", order = 1)]
public class NPCCharacter : ScriptableObject
{
    public string XmlFileName;
    public string moduleID;
    public string id;
    public string npcName;
    public string is_female;
    public string is_hero;
    public string is_companion;
    public string is_mercenary;
    public string is_child_template;
    public string is_template;
    public string age;
    public string voice;
    public string culture;
    public string default_group;

    public string level;
    public string occupation;

    public string banner_symbol_mesh_name;
    public string banner_symbol_color;

    public string skill_template;
    public string battleTemplate;
    public string civilianTemplate;


    //BodyProperties

    public string BP_version;
    public string BP_age;
    public string BP_weight;

    public string BP_build;
    public string BP_key;

    //BodyPropertiesMAX
    public bool _Has_Max_BP;
    public string Max_BP_version;
    public string Max_BP_age;
    public string Max_BP_weight;

    public string Max_BP_build;
    public string Max_BP_key;

    // SKILLS

    public string[] skills;
    public string[] skillValues;
    // TRAITS

    public string[] traits;
    public string[] traitValues;
    // FEATS

    public string[] feats;
    public string[] featValues;

    /// Equipment Sets

    public string[] equipment_Roster;
    public string equipment_Main;
    public string[] equipment_Set;
    public bool[] equipment_Set_civilian_flag;


    //
    public string upgrade_requires;
    public string[] upgrade_targets;
    public string is_basic_troop;
    public string formation_position_preference;
    //
    public string COMP_Companion;
    public string face_key_template;


    //UPDATE
    public string face_mesh_cache;
    public string is_hidden_encyclopedia;
    public string is_obsolete;

    // hair_tag ---> name
    // beard_tag ---> name
    public string[] hair_tag;
    public string[] beard_tag;

}
