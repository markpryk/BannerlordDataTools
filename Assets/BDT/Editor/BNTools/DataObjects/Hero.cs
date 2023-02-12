using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Hero : ScriptableObject
{
    public string moduleID;
    [SerializeField]
    public string XmlFileName;
    [SerializeField]
    public string id;
    public string alive;
    //public string is_noble;
    public string faction;
    public string banner_key;
    public string father;
    public string mother;
    public string spouse;
    public string text;

    public string preferred_upgrade_formation;

    // internal
    public bool isMixedClans;
    public string mixedFather;
    public string mixedFather_fac;
    public bool mixedFather_redactable;
    public string mixedMother;
    public string mixedMother_fac;
    public bool mixedMother_redactable;
    public string mixedSpouse;
    public string mixedSpouse_fac;
    public bool mixedSpouse_redactable;

    public float node_X;
    public float node_Y;
}
