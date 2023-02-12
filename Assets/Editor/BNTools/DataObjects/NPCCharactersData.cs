using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[System.Serializable]
public class NPCCharactersData : ScriptableObject
{
  [SerializeField]
  public List<NPCCharacter> NPCCharacters;
}
