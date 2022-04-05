using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Faction : ScriptableObject
{

    public string moduleID;
    [SerializeField]
    public string XmlFileName;
    [SerializeField]
    public string id;
    public string _BannerColorA;
    public string _BannerColorB;

    public string factionName;

    public string is_minor_faction;
    public string is_mafia;
    public string is_outlaw;
    public string is_bandit;

    // UPDATES 1.7.2
    public string is_clan_type_mercenary;
    public string short_name;
    public string is_sect;
    public string is_nomad;

    public string[] minor_faction_character_templates;
    //Category  -  minor_faction_character_templates - 0
    // template ---> id
    // template ---> name

    public string initial_posX;
    public string initial_posY;
    public string owner;

    public string super_faction;
    public string banner_key;

    [SerializeField]
    public string label_color;
    [SerializeField]
    public string color;
    [SerializeField]
    public string color2;
    [SerializeField]
    public string alternative_color;
    [SerializeField]
    public string alternative_color2;

    public string culture;
    public string settlement_banner_mesh;
    public string default_party_template;
    public string tier;
    public string text;

    // internal
    public float Node_X;
    public float Node_Y;


}
