using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[Serializable]
public class ModuleReceiver : ScriptableObject
{
    public string id;
    public string moduleName;
    public string version;
    public bool defaultModule;
    public string moduleCategory;
    public bool official;

    public string path;
    public ModFiles modFilesData;

    public string[] modDependenciesInternal;
    public List<Dependency> Dependencies;
    public List<SubModule> SubModules;

    public bool load_xscene;

    public string world_map_xscene_id = "Main_map";

    public int W_X_Size;
    public int W_Y_Size;
    public int W_SingleNodeSize;
    public float W_max_Height;
    public float W_min_Height;
    public string W_Map_Texture;
    public string W_HeightMap_Texture;

    public int BackUp_Count = 0;

    [Serializable]
    public struct SubModule
    {
        public string Name;
        public string DLLName;
        public string SubModuleClassType;
        public List<SubModuleTag> Tags;
        public bool ShowTags;
        public SubModule(string name, string dllName, string subModuleClassType, List<SubModuleTag> tags, bool showTags)
        {
            this.Name = name;
            this.DLLName = dllName;
            this.SubModuleClassType = subModuleClassType;
            this.Tags = new List<SubModuleTag>(tags);
            this.ShowTags = showTags;
        }
    }

    [Serializable]
    public struct SubModuleTag
    {
        public string Key;
        public string Value;

        public SubModuleTag(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    [Serializable]
    public struct Dependency
    {
        public string DependedModule;
        public string DependentVersion;
        public bool Optional;
        public Dependency(string dependedModule, string dependentVersion, bool optional)
        {
            this.DependedModule = dependedModule;
            this.DependentVersion = dependentVersion;
            this.Optional = optional;
        }
        public static bool ContainsID(string id, List<Dependency> container)
        {
            foreach (var dpd in container)
                if (dpd.DependedModule == id)
                    return true;

            return false;
        }
    }
    public void UpdateInternalDependencies()
    {
        if (Dependencies != null && Dependencies.Count > 0)
        {
            modDependenciesInternal = new string[Dependencies.Count];
            for (int i = 0; i < Dependencies.Count; i++)
                modDependenciesInternal[i] = Dependencies[i].DependedModule;
        }
        else
            modDependenciesInternal = new string[0];

    }

}
