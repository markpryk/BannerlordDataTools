using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PartyTemplatesData : ScriptableObject
{
  [SerializeField]
  public List<PartyTemplate> partyTemplates;
}