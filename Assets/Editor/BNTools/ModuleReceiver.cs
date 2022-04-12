using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ModuleReceiver : ScriptableObject
{
    [SerializeField]
    public string path;
    [SerializeField]
    public string id;
    [SerializeField]
    public string moduleName;
    [SerializeField]
    public string version;
    [SerializeField]
    public ModFiles modFilesData;

    public string[] modDependencies;

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


}
