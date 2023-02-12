using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

public class TpacReader : EditorWindow
{
    public const uint TPAC_MAGIC_NUMBER = 0x43415054;

    string tx_type_guid = "43415054-0002-0000-93ab-9e3851ec814d";
    string color_tx_guid = "Assets/BDT/Editor/BNTools/Utils/TpacSource/TpacSources/g_tex.tpac";
    public byte[] color_tx_bytes = { 156, 118, 97, 213, 213, 143, 169, 75, 175, 120, 14, 30, 65, 15, 7, 171 };
    string nm_tx_guid = "ecebfabe-e899-332a-0100-000077010000";
    string spec_tx_guid = "72f66eb0-f723-e174-0100-00007b010000";
    string hm_tx_guid = "615018b8-f76a-bdb2-0100-00007f010000";

    bool init = false;
    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    BDTSettings settingsAsset;
    ModuleReceiver currMod;

    public string path = "E:/Modding/Projects/Bannerlord/CWE_DLC/Info";


    [MenuItem("BNDataTools/TpacReader")]
    static void Init()
    {
        var window = GetWindow<TpacReader>("SDF Texture Generator");
        window.position = new Rect(512, 256, 256, 350);
        window.Show();
    }
    void OnGUI()
    {
        //InitializeModData();

        path = EditorGUILayout.TextField(path);

        if (GUILayout.Button("Read Tpac"))
        {

            //string tx_type_guid = "43415054-0002-0000-93ab-9e3851ec814d";
            //string color_tx_guid = "ff40de90-6d7e-8a10-0100-00007d010000";
            //string nm_tx_guid = "ecebfabe-e899-332a-0100-000077010000";
            //string spec_tx_guid = "72f66eb0-f723-e174-0100-00007b010000";
            //string hm_tx_guid = "615018b8-f76a-bdb2-0100-00007f010000";

            //FileInfo file = new FileInfo(color_tx_guid);
            //var stream = Utils.OpenBinaryReader(file);

            //byte[] str_new = Encoding.UTF8.GetBytes("PEDRO");
            var src = File.ReadAllBytes(path);
            var src_tx = File.ReadAllBytes(color_tx_guid);

            //Debug.Log(Encoding.UTF8.GetString(str));
            var bites = (byte[])RandomBytesArray();
            //var matPath = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/CrusadesCore/Assets/BDT/mats/mat_test_mtl.tpac";
            var texPath = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/CrusadesCore/Assets/BDT/mats/color_test_texture.tpac";
            var finaltexPathOrg = "$BASE/Modules/CrusadesCore/AssetSources/mats/color_tx.png";
            var finaltexPath = "$BASE/Modules/CrusadesCore/AssetSources/mats/color_texture.png";
            var finaltexPathTpac = "$BASE/Modules/CrusadesCore/Assets/BDT/mats/color_texture_tex.tpac";
            var name = "color_texture.tpac";
            //File.WriteAllBytes(matPath, ReplaceBytes(src, color_tx_bytes, bites));
            //File.WriteAllBytes(texPath, ReplaceBytes(src_tx, color_tx_bytes, bites));
            //Encoding.UTF8.GetString(ReplaceBytes(src, color_tx_bytes, str_new));  

            //var tex_path_source = Encoding.UTF8.GetBytes("$BASE/Modules/CrusadesCore/AssetSources/mats/color_tx.png");
            //var tex_path = Encoding.UTF8.GetBytes("$BASE/Modules/CrusadesCore/AssetSources/materials/color_test.png");

            //src_tx = File.ReadAllBytes(texPath);

            //File.WriteAllBytes(texPath, ReplaceBytes(src_tx, tex_path_source, tex_path));

            src_tx = File.ReadAllBytes(color_tx_guid);

            var nameSource = Encoding.ASCII.GetBytes("color_tx");
            var texName = Encoding.ASCII.GetBytes("color_texture");

            int unicode = (finaltexPathTpac.Length) + 1;
            int unicodetwo = (name.Length);
            char character = (char)unicode;
            string text = character.ToString();

            var num = Encoding.ASCII.GetBytes(text);

            Debug.Log(text);

            var uni = finaltexPath.Length + unicode + unicodetwo;
            var chr = (char)uni;
            var txto = chr.ToString();

            Debug.Log(txto);
            var numtwo = Encoding.ASCII.GetBytes(txto);


            //File.WriteAllBytes(texPath, ReplaceBytes(src_tx, tex_path_source, tex_path));

            //src_tx = File.ReadAllBytes(texPath);

            File.WriteAllBytes(texPath, ReplaceBytesByIndex(src_tx, 28, numtwo));

            src_tx = File.ReadAllBytes(texPath);

            File.WriteAllBytes(texPath, ReplaceBytesByIndex(src_tx, 116, num));

            src_tx = File.ReadAllBytes(texPath);

            File.WriteAllBytes(texPath, ReplaceBytes(src_tx, nameSource, texName));

            nameSource = Encoding.ASCII.GetBytes(finaltexPathOrg);
            texName = Encoding.ASCII.GetBytes(finaltexPath);

            src_tx = File.ReadAllBytes(texPath);

            File.WriteAllBytes(texPath, ReplaceBytes(src_tx, nameSource, texName));

            //src = File.ReadAllBytes(matPath);

            //byte[] str = Encoding.UTF8.GetBytes("new_material");
            //byte[] str_new = Encoding.UTF8.GetBytes("mat_test");

            //File.WriteAllBytes(matPath, ReplaceBytes(src, str, str_new));

        }

        if (GUILayout.Button("Read Write Tpac"))
        {

            byte[] str = Encoding.UTF8.GetBytes("test_material");
            byte[] str_new = Encoding.UTF8.GetBytes("bla_bla_bla");
            var src = File.ReadAllBytes(path);
            File.WriteAllBytes(path, ReplaceBytes(src, str, str_new));

        }

        if (GUILayout.Button("Read Tpac Asset"))
        {

            //byte[] byts = { 206, 176, 140, 251, 169, 7, 120, 12, 220, 164, 138, 45, 10, 120, 195, 242 };
            byte[] byts_asset = { 108, 117, 59, 10, 193, 44, 103, 25 };
            byte[] byts_path = {
                36, 66, 65 ,83, 69, 47 ,77 ,111 ,100 ,117, 108 ,101, 115, 47, 67 ,114,
                117, 115, 97, 100, 101, 115, 67, 111, 114, 101, 47 ,65, 115 ,115 ,101 ,116,
                83 ,111 ,117, 114 ,99 ,101 ,115, 47, 109, 97, 116, 115 ,47, 99 ,111, 108,
                111, 114, 95, 116, 120, 46, 112 ,110 ,103};
            byte[] byts_name = { 99, 111, 108, 111, 114, 95, 116 ,120, 171 };
            int byts_pathLenghtPointer = 116;
            int byts_nameLenghtPointer = 28;
            byte[] bytsZero = { 0, 0, 0, 0, 0, 0, 0, 0 };

            var texPath = "E:/Games/SteamLibrary/steamapps/common/Mount & Blade II Bannerlord/Modules/CrusadesCore/Assets/BDT/g_tex.tpac";
            var texPathBin = "$BASE/Modules/CrusadesCore/AssetSources//g.png";

            var texName = Encoding.ASCII.GetBytes("g");
            var src_tx = File.ReadAllBytes(path);
            src_tx = ReplaceBytes(src_tx, byts_asset, bytsZero);

            char character = (char)texPathBin.Length;
            string unicodeChar = character.ToString();

            var num = Encoding.ASCII.GetBytes(unicodeChar);

            src_tx = ReplaceBytes(src_tx, byts_path, Encoding.UTF8.GetBytes(texPathBin));
            src_tx = ReplaceBytesByIndex(src_tx, byts_pathLenghtPointer, num);

            byte[] bytN = {  160 };

            src_tx = ReplaceBytes(src_tx, byts_name, texName);
            src_tx = ReplaceBytesByIndex(src_tx, 77, bytN);


            //var uni = unicode;
            //var chr = (char)uni;
            //var txt_lenght_name = chr.ToString();

            //var num_two = Encoding.ASCII.GetBytes(txt_lenght_name);

            byte[] bytZ = { 107 };

            src_tx = ReplaceBytesByIndex(src_tx, byts_nameLenghtPointer, bytZ);

            File.WriteAllBytes(texPath, src_tx);

        }




    }


    //private bool callback(int package, int packageCount, string fileName, bool completed)
    //{
    //    Debug.Log(fileName);
    //    //throw new NotImplementedException();
    //    return true;
    //}

    private object RandomBytesArray()
    {
        var array_lenght = 16;

        byte[] array = new byte[array_lenght];

        int len = array_lenght - 1;

        for (int i = len; i >= 0; i--)
        {
            array[i] = 0x20;
        }

        return array;
    }
    public int FindBytes(byte[] src, byte[] find)
    {
        int index = -1;
        int matchIndex = 0;
        // handle the complete source array
        for (int i = 0; i < src.Length; i++)
        {
            if (src[i] == find[matchIndex])
            {
                if (matchIndex == (find.Length - 1))
                {
                    index = i - matchIndex;
                    break;
                }
                matchIndex++;
            }
            else if (src[i] == find[0])
            {
                matchIndex = 1;
            }
            else
            {
                matchIndex = 0;
            }

        }
        return index;
    }

    public byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
    {
        byte[] dst = null;
        int index = FindBytes(src, search);
        if (index >= 0)
        {
            dst = new byte[src.Length - search.Length + repl.Length];
            // before found array
            Buffer.BlockCopy(src, 0, dst, 0, index);
            // repl copy
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            // rest of src array
            Buffer.BlockCopy(
                src,
                index + search.Length,
                dst,
                index + repl.Length,
                src.Length - (index + search.Length));
        }
        return dst;
    }

    public byte[] ReplaceBytesByIndex(byte[] src, int index, byte[] repl)
    {
        byte[] dst = null;

        if (index >= 0)
        {
            dst = new byte[src.Length - 1 + repl.Length];
            // before found array
            Buffer.BlockCopy(src, 0, dst, 0, index);
            // repl copy
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            // rest of src array
            Buffer.BlockCopy(
                src,
                index + 1,
                dst,
                index + repl.Length,
                src.Length - (index + 1));
        }
        return dst;
    }

}

