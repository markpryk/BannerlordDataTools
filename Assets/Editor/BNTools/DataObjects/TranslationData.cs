using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[System.Serializable]
public class TranslationData : ScriptableObject
{
  [SerializeField]
  // public List<TranslationString> lenguagesList;
  public List<TranslationString> translationStrings;
}