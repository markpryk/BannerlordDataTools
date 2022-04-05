using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ScriptableObject
{
    public string id;
    public string moduleID;
    public string culture;

    public string[] eqpSetID;

    
    //flags
    public bool IsEquipmentTemplate;
    public bool IsNobleTemplate;
    public bool IsMediumTemplate;
    public bool IsHeavyTemplate;
    public bool IsFlamboyantTemplate;
    public bool IsStoicTemplate;
    public bool IsNomadTemplate;
    public bool IsWoodlandTemplate;
    public bool IsFemaleTemplate;
    public bool IsCivilianTemplate;
    public bool IsCombatantTemplate;
    public bool IsNoncombatantTemplate;

    public bool IsChildTemplate;
    public bool IsWandererEquipment;

}
