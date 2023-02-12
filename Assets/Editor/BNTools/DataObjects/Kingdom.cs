using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Kingdom : ScriptableObject
{
    public string moduleID;
    [SerializeField]
    public string XmlFileName;
    [SerializeField]
    public string id;
    [SerializeField]
    public string kingdomName;

    public string owner;
    public string banner_key;
    public string primary_banner_color;
    public string secondary_banner_color;
    public string label_color;
    public string color;
    public string color2;
    public string alternative_color;
    public string alternative_color2;
    public string culture;
    public string settlement_banner_mesh;
    public string flag_mesh;
    public string short_name;
    public string title;
    public string ruler_title;
    public string text;

    /// Kingdom Relations

    public string[] relationships;
    public string[] relationValues;
    public string[] relationsAtWar;

    public string[] fac_relationships;
    public string[] fac_relationValues;
    /// Policies

    public string[] policies;
}
