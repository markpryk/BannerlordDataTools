using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class DocGenerator : EditorWindow
{
    bool init = false;
    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    BDTSettings settingsAsset;
    ModuleReceiver currMod;

    public string path = "E:/Modding/Projects/Bannerlord/CWE_DLC/Info";

    [MenuItem("BNDataTools/DocGen")]
    static void Init()
    {
        var window = GetWindow<DocGenerator>("SDF Texture Generator");
        window.position = new Rect(512, 256, 256, 350);
        window.Show();
    }
    void OnGUI()
    {
        InitializeModData();

        path = EditorGUILayout.TextField(path);

        if (GUILayout.Button("Create doc"))
        {
            var dir = $"{path}/Docs";

            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            var docDir = Directory.CreateDirectory(dir);

            foreach (var kngd_data in currMod.modFilesData.kingdomsData.kingdoms)
            {
                string soloName = kngd_data.kingdomName;
                RemoveTSString(ref soloName);

                var k_dir = $"{dir}/{soloName}/";
                Directory.CreateDirectory(k_dir);
                var facs_dir = Directory.CreateDirectory(k_dir + "/Factions");

                foreach (var fac_data in currMod.modFilesData.factionsData.factions)
                {
                    if (kngd_data.id == fac_data.super_faction.Replace("Kingdom.", ""))
                    {
                        soloName = fac_data.factionName;
                        RemoveTSString(ref soloName);

                        var f_dir = $"{facs_dir}/{soloName}";
                        Directory.CreateDirectory(f_dir);
                        var settls_dir = Directory.CreateDirectory(f_dir + "/Locations");

                        string description = $"{f_dir}/{soloName}_Info.txt";

                        StreamWriter writer_fac = new StreamWriter(description, true);

                        writer_fac.WriteLine($"Faction - {soloName}");
                        writer_fac.WriteLine();
                        writer_fac.WriteLine($"Fiefs:");


                        foreach (var settl_data in GetModByName("CrusadesCoreCleaner").modFilesData.settlementsData.settlements)
                        {
                            if (settl_data.isTown || settl_data.isCastle)
                                if (fac_data.id == settl_data.owner.Replace("Faction.", ""))
                                {

                                    soloName = settl_data.settlementName;
                                    RemoveTSString(ref soloName);

                                    var s_dir = $"{settls_dir}/{soloName}";
                                    Directory.CreateDirectory(s_dir);
                                    var villa_dir = Directory.CreateDirectory(s_dir + "/Villages");

                                    description = $"{s_dir}/{soloName}_Info.txt";

                                    var writer_settl = new StreamWriter(description, true);

                                    if (settl_data.isTown)
                                    {
                                        writer_settl.WriteLine($"Town - {soloName}");
                                        writer_fac.WriteLine($"Town - {soloName}");
                                    }
                                    else
                                    {
                                        writer_settl.WriteLine($"Castle - {soloName}");
                                        writer_fac.WriteLine($"Castle - {soloName}");
                                    }

                                    writer_settl.WriteLine();
                                    writer_settl.WriteLine($"prosperity - {settl_data.prosperity}");
                                    writer_settl.WriteLine();
                                    writer_settl.WriteLine($"Fiefs:");

                                    foreach (var village_data in GetModByName("CrusadesCoreCleaner").modFilesData.settlementsData.settlements)
                                    {
                                        if (village_data.isVillage)
                                            if (settl_data.id == village_data.CMP_bound.Replace("Settlement.", ""))
                                            {

                                                soloName = village_data.settlementName;
                                                RemoveTSString(ref soloName);

                                                var v_dir = $"{villa_dir}/{soloName}";
                                                Directory.CreateDirectory(v_dir);

                                                description = $"{v_dir}/{soloName}_Info.txt";

                                                var writer = new StreamWriter(description, true);

                                                writer.WriteLine($"Village - {soloName}");
                                                writer_fac.WriteLine($"Village - {soloName}");
                                                writer.WriteLine();
                                                writer.WriteLine($"Production type - {village_data.CMP_villageType.Replace("VillageType.", "")}");

                                                writer.Close();

                                                writer_settl.WriteLine($"Village - {soloName}");

                                            }
                                    }

                                    writer_settl.Close();
                                    writer_fac.WriteLine();

                                }
                        }
                        writer_fac.Close();
                    }
                }

            }
        }

    }

    bool GetSettlementOwner(string owner)
    {
        //owner = owner.Replace("Faction.", "");
        //foreach (var kingdom in GetModByName("Crusades").modFilesData.kingdomsData.kingdoms)
        //{
        //    if (kingdom.id == super_fac)
        //        return true;
        //}

        return false;

    }
    private void InitializeModData()
    {
        if (!init)
        {

            if (settingsAsset == null)
            {
                if (System.IO.File.Exists(configPath))
                {
                    settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));
                }
                else
                {
                    Debug.Log("BDT settings dont exist");
                }
            }

            currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));

            EditorUtility.SetDirty(settingsAsset);
            EditorUtility.SetDirty(currMod);

            init = true;

        }
    }

    ModuleReceiver GetModByName(string modName)
    {
        var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + modName + ".asset", typeof(ModuleReceiver));
        return mod;
    }
    private static void RemoveTSString(ref string inputString)
    {
        var TS_Name = inputString;
        Regex regex = new Regex("{=(.*)}");
        if (regex != null)
        {
            var v = regex.Match(TS_Name);
            string s = v.Groups[1].ToString();
            TS_Name = "{=" + s + "}";
        }

        if (TS_Name != "" && TS_Name != "{=}")
        {
            inputString = inputString.Replace(TS_Name, "");
        }
    }
}