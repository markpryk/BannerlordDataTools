using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationComplexTemplate : ScriptableObject
{
    [SerializeField]
    public string complexID;
    [SerializeField]
    public string[] locationsComplexID;
    [SerializeField]
    public string[] locationAreasID;

}
