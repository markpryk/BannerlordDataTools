using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PartyTemplate : ScriptableObject
{
    public string moduleID;
    [SerializeField]
    public string XmlFileName;
    [SerializeField]
    public string id;

    public string[] PTS_troop;
    public string[] PTS_min_value;
    public string[] PTS_max_value;
    

}
