using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[System.Serializable]
public class FactionsData : ScriptableObject
{
  [SerializeField]  
  public List<Faction> factions;
}

