using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Settlement : ScriptableObject
{
    public string moduleID;
    [SerializeField]
    public string XmlFileName;
    [SerializeField]
    public string id;
    [SerializeField]
    public string settlementName;

    public string culture;
    public string type;
    public string owner;
    public string prosperity;
    public string posX;
    public string posY;
    public string gate_posX;
    public string gate_posY;

    public string text;

    // SETTLEMENT COMPONENTS

    public bool isVillage;
    public bool isCastle;
    public bool isTown;
    public bool isHideout;

    public string CMP_id;

    // Village
    public string CMP_villageType;
    public string CMP_Level;
    public string CMP_hearth;
    public string CMP_trade_bound;
    public string CMP_bound;
    public string CMP_background_crop_position;
    public string CMP_background_mesh;
    public string CMP_wait_mesh;
    public string CMP_castle_background_mesh;

    //HIDEOUT
    public string CMP_map_icon;
    public string CMP_scene_name;
    public string CMP_gate_rotation;

    // SETTLEMENT LOCATIONS

    public string LOC_complex_template;
    public string[] LOC_id;
    public string[] LOC_max_prosperity;
    public string[] LOC_scn;
    public string[] LOC_scn_1;
    public string[] LOC_scn_2;
    public string[] LOC_scn_3;

    // SETTLEMENT LOCATIONS

    public string[] AREA_type;
    public string[] AREA_name;

    // WORLD MAP
    public bool WM_Undefined;

}
