using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class NPCEditorManager : EditorWindow
{
    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    public int existingStackCount;

    public BDTSettings bdt_settings;
    public int windowStateID;

    public bool preserveTSLinks;
    public bool overrideAsset;


    // ? object ID
    // ? ---------
    // -1 culture 
    // 1 Settlement
    // 2 Settlement
    // 3 NPC
    // 4 hero 
    // 5 settlement name
    // 6 partyTemplate
    // 7 item
    // 8 Equipemnt
    public int objID;
    public int startStack;
    public object obj;

    public string prefix = "NPC_";

    // 
    public bool NPC_is_Hero;
    public bool NPC_is_female;
    public bool NPC_is_Template;

    public string assetName_org;
    public string assetName_new;
    public string assetID_new;


    //
    public bool Bool_A = true;
    public bool Bool_B;
    public bool Bool_C;
    public bool Bool_D;
    public bool contains_Bool = false;

    //
    public int stack_count;

    //

    public string[] A_options;
    public int A_index;
    public string[] B_options;
    public int B_index;
    //
    public bool useObjectRef;
    public bool createHeroData;
    public bool assignHeroFaction;
    // public bool override;

    public object object_reference_A;
    public object object_reference_B;
    public object object_Faction_ref;

    public Dictionary<NPCCharacter, Hero> modifyDic;
    //

    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    private bool _checkAtStart;
    public void OnEnable()
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

        _checkAtStart = false;
        //Bool_C = true;

        if (windowStateID == 2)
        {
            foreach (var data in modifyDic)
            {
                EditorUtility.SetDirty(data.Key);
                EditorUtility.SetDirty(data.Value);
            }
        }
    }
    void OnGUI()
    {

        if (windowStateID == 1)
        {
            //asset Create
            CreateAsset();
        }

        else if (windowStateID == 2)
        {
            //asset modify
            ModifySelectedAssets();
        }

    }

    private void ModifySelectedAssets()
    {

        if (modifyDic.Count == 0)
        {
            EditorGUILayout.HelpBox("Zero Character Selected, " + "\n" + "select at least one Character before edit it.", MessageType.Error);

        }
        else
        {
            A_options = new string[3];
            A_options[0] = "Edit";
            A_options[1] = "Copy";
            A_options[2] = "Remove";


            if (B_options == null)
            {
                string[] mods = Directory.GetFiles("Assets/Resources/SubModulesData", "*.asset");

                B_options = new string[mods.Length];

                int i = 0;
                foreach (var mod in mods)
                {
                    var sharedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                    if (sharedMod.id != settingsAsset.currentModule)
                    {
                        B_options[i] = sharedMod.id;
                        i++;

                    }
                }
            }

            ModuleReceiver currMod;
            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
            }
            else
            {
                return;
            }

            if (_checkAtStart == false)
            {
                CheckContains(currMod);
            }

            this.title = "Modify Selected Assets";

            EditorGUILayout.HelpBox("Modify selected Character Assets.", MessageType.Info);

            DrawUILine(colUILine, 2, 8);

            EditorGUILayout.LabelField("Modify Type:", EditorStyles.boldLabel);
            A_index = EditorGUILayout.Popup(A_index, A_options, GUILayout.Width(128));
            EditorGUILayout.Space(4);



            EditorGUILayout.LabelField($"{modifyDic.Count} Selected objects", EditorStyles.helpBox);

            EditorGUILayout.Space(4);

            // EDIT
            if (A_index == 0)
            {
                WindowModeEdit(currMod);

            }

            // COPY 
            if (A_index == 1)
            {
                WindowModeCopy(currMod);
            }

            // REMOVE
            if (A_index == 2)
            {

                if (GUILayout.Button("Remove Selected"))
                {

                    foreach (var dic in modifyDic)
                    {
                        var asstPath = AssetDatabase.GetAssetPath(dic.Key);

                        if (dic.Key.equipment_Roster != null && dic.Key.equipment_Roster.Length > 0)
                        {
                            foreach (var roster in dic.Key.equipment_Roster)
                            {
                                var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentRosters/" + roster + ".asset";
                                EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));

                                currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set);
                                AssetDatabase.DeleteAsset(path);

                            }
                        }

                        if (dic.Key.equipment_Main != "")
                        {
                            var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentMain/" + dic.Key.equipment_Main + ".asset";
                            EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));

                            currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set);
                            AssetDatabase.DeleteAsset(path);
                        }

                        currMod.modFilesData.npcChrData.NPCCharacters.Remove(dic.Key);
                        AssetDatabase.DeleteAsset(asstPath);

                        // Debug.Log(dic.Value.id);

                        if (dic.Value != null)
                        {
                            asstPath = AssetDatabase.GetAssetPath(dic.Value);

                            currMod.modFilesData.heroesData.heroes.Remove(dic.Value);
                            AssetDatabase.DeleteAsset(asstPath);
                        }
                    }


                    //AssetDatabase.Refresh();
                    EditorWindow.GetWindow(typeof(NPCEditor));
                    this.Close();
                }

            }
        }

    }

    private void WindowModeEdit(ModuleReceiver currMod)
    {
        CreateEditorToggle(ref Bool_C, "Edit as Copy");
        EditorGUILayout.Space(4);
        // GUILayout.Space(4);
        DrawUILine(colUILine, 2, 8);


        EditorGUILayout.LabelField("Modify Stack ID:", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        assetID_new = EditorGUILayout.TextField("New ID:", assetID_new);

        if (modifyDic.Count > 1)
            startStack = EditorGUILayout.IntField("Stack ID num:", startStack);

        if (startStack < 0)
            startStack = 0;

        if (EditorGUI.EndChangeCheck())
        {
            contains_Bool = false;

            for (int i2 = 0; i2 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i2++)
            {
                for (int i3 = 0; i3 < modifyDic.Count; i3++)
                {
                    var assetName = "";

                    if (modifyDic.Count == 1)
                    {
                        assetName = assetID_new;
                    }
                    else
                    {
                        assetName = assetID_new + "_" + (startStack + i3 + 1);
                    }

                    // Debug.Log(assetName);
                    if (currMod.modFilesData.npcChrData.NPCCharacters[i2].id == assetName)
                    {
                        contains_Bool = true;
                        // break;
                        // existingStackCount = existingStackCount + 1;
                    }
                }
            }
        }
        DrawUILine(colUILine, 2, 8);

        if (modifyDic.Count != 1)
        {
            EditorGUILayout.LabelField("Name preview:", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("(name + count)", EditorStyles.miniLabel);
            EditorGUILayout.LabelField(assetID_new + "_" + startStack, EditorStyles.miniBoldLabel);
            DrawUILine(colUILine, 2, 8);
        }

        string createAssetsLabel = "Edit Asset";

        // EditorGUILayout.Space(4);

        if (contains_Bool)
        {
            if (modifyDic.Count == 1)
            {

                EditorGUILayout.HelpBox($"{assetID_new} - asset already exist in {settingsAsset.currentModule} module, do you want to override it?", MessageType.Warning);
                createAssetsLabel = "Override Asset";

            }
            else
            {
                EditorGUILayout.HelpBox($"{existingStackCount} - Assets with this names already exist in {settingsAsset.currentModule} module, do you want to do?", MessageType.Warning);

                CreateEditorToggle(ref Bool_A, "Add to existing Stack");
                if (Bool_A)
                {
                    Bool_B = false;
                    createAssetsLabel = "Edit Assets";

                }
                CreateEditorToggle(ref Bool_B, "Override if exist");
                if (Bool_B)
                {
                    Bool_A = false;
                    createAssetsLabel = "Edit & Override Assets";
                }
            }

            if (Bool_A || Bool_B)
            {
                if (GUILayout.Button(createAssetsLabel))
                {
                    Bool_D = false;
                    CheckContains(currMod);
                    ApplyEditAsset(currMod);
                    //CreateEditAssetTest(currMod);
                }
            }

        }
        else
        {
            if (GUILayout.Button(createAssetsLabel))
            {
                Bool_D = false;
                CheckContains(currMod);
                ApplyEditAsset(currMod);
                //CreateEditAssetTest(currMod);
            }
        }
    }

    private void WindowModeCopy(ModuleReceiver currMod)
    {
        Bool_C = true;

        EditorGUILayout.LabelField("Copy to module:", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);
        B_index = EditorGUILayout.Popup(B_index, B_options, GUILayout.Width(128));
        EditorGUILayout.Space(4);

        if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
        {
            // currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + B_options[B_index] + ".asset", typeof(ModuleReceiver));

            string modPath = modsSettingsPath + B_options[B_index] + ".asset";
            var destinationMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modPath, typeof(ModuleReceiver));

            EditorGUI.BeginChangeCheck();

            CreateEditorToggle(ref Bool_D, "Create new ID at Copy");
            if (EditorGUI.EndChangeCheck())
            {
                //if (Bool_D)
                //{
                //    assetID_new = assetName_new;
                //}

                CheckContains(destinationMod);
            }

            EditorGUILayout.Space(4);

            if (Bool_D)
            {
                DrawUILine(colUILine, 2, 8);
                // EditorGUILayout.LabelField("Modify Stack ID:", EditorStyles.boldLabel);

                //EditorGUI.BeginChangeCheck();

                var idLabel = "New ID:";

                if (modifyDic.Count != 1)
                {
                    idLabel = "New Stack ID:";
                }

                assetID_new = EditorGUILayout.TextField(idLabel, assetID_new);


                if (modifyDic.Count > 1)
                    startStack = EditorGUILayout.IntField("Stack ID num:", startStack);

                if (startStack < 0)
                    startStack = 0;


                //if (EditorGUI.EndChangeCheck())
                //{
                CheckContains(destinationMod);
                //}
                DrawUILine(colUILine, 2, 8);

                if (modifyDic.Count != 1)
                {
                    EditorGUILayout.LabelField("Name preview:", EditorStyles.miniBoldLabel);
                    EditorGUILayout.LabelField("(Name + count)", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField(assetID_new + "_" + startStack, EditorStyles.miniBoldLabel);
                    DrawUILine(colUILine, 2, 8);
                }
            }
            else
            {
                if (!Bool_D)
                {
                    assetName_new = assetID_new;
                    assetID_new = modifyDic.ToArray()[0].Key.id;
                }

                CheckContains(destinationMod);
            }

            string createAssetsLabel = "Copy Asset";

            // EditorGUILayout.Space(4);

            //Debug.Log(contains_Bool);


            if (contains_Bool)
            {
                if (modifyDic.Count == 1)
                {

                    var naming = assetID_new;
                    if (!Bool_D)
                    {
                        naming = modifyDic.ToArray()[0].Key.id;
                    }

                    EditorGUILayout.HelpBox($"{naming} - asset already exist in {B_options[B_index]} module, do you want to override it?", MessageType.Warning);
                    createAssetsLabel = "Copy & Override Asset";

                }
                else
                {
                    if (Bool_D)
                    {

                        EditorGUILayout.HelpBox($"{existingStackCount} - Assets with this names already exist in {B_options[B_index]} module, do you want to do?", MessageType.Warning);

                        CreateEditorToggle(ref Bool_A, "Add to existing Stack");
                        if (Bool_A)
                        {
                            Bool_B = false;
                            createAssetsLabel = "Copy Assets";

                        }
                        CreateEditorToggle(ref Bool_B, "Override if exist");
                        if (Bool_B)
                        {
                            Bool_A = false;
                            createAssetsLabel = "Copy & Override Assets";
                        }
                    }
                    else
                    {
                        Bool_A = false;
                        Bool_B = true;

                        EditorGUILayout.HelpBox($"{existingStackCount} - Assets with this names already exist in {B_options[B_index]} module, do you want to override exisited?", MessageType.Warning);
                        createAssetsLabel = "Copy & Override Assets";

                    }

                }
            }

            if (Bool_D)
            {
                if (Bool_A || Bool_B)
                {
                    if (GUILayout.Button(createAssetsLabel))
                    {
                        //Debug.Log("done");
                        //CreateEditAssetTest(currMod);
                        ApplyCopyAsset(destinationMod);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(createAssetsLabel))
                {
                    //Debug.Log("done");
                    //CreateEditAssetTest(currMod);
                    ApplyCopyAsset(destinationMod);
                }
            }

        }

    }

    private void CheckContains(ModuleReceiver currMod)
    {
        contains_Bool = false;

        for (int i2 = 0; i2 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i2++)
        {
            for (int i3 = 0; i3 < modifyDic.Count; i3++)
            {
                var assetName = "";

                if (A_index == 0)
                {
                    if (modifyDic.Count == 1)
                    {
                        assetName = assetID_new;
                    }
                    else
                    {
                        assetName = assetID_new + "_" + (startStack + i3);
                    }
                }
                else if (A_index == 1)
                {
                    if (Bool_D)
                    {
                        if (modifyDic.Count == 1)
                        {
                            assetName = assetID_new;
                        }
                        else
                        {
                            assetName = assetID_new + "_" + (startStack + i3);
                        }
                    }
                    else
                    {
                        assetName = assetID_new;
                    }
                }

                if (currMod.modFilesData.npcChrData.NPCCharacters[i2].id == assetName)
                {

                    contains_Bool = true;
                    // break;
                    // existingStackCount = existingStackCount + 1;
                }
            }

            if (contains_Bool)
            {


                existingStackCount = 0;
                var assetName = "";

                for (int i3 = 0; i3 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i3++)
                {
                    var npcCharObj = currMod.modFilesData.npcChrData.NPCCharacters[i3];

                    for (int i4 = 0; i4 < modifyDic.Count; i4++)
                    {

                        if (A_index == 0)
                        {
                            assetName = assetID_new + "_" + (startStack + i4);
                            if (npcCharObj.id == assetName)
                            {
                                existingStackCount++;
                            }
                        }
                        else if (A_index == 1)
                        {

                            if (Bool_D)
                            {
                                assetName = assetID_new + "_" + (startStack + i4);
                                if (npcCharObj.id == assetName)
                                {
                                    existingStackCount++;
                                }
                            }
                            else
                            {
                                assetName = assetID_new;
                                if (npcCharObj.id == assetName)
                                {
                                    existingStackCount++;
                                }
                            }

                        }



                    }
                }

                // Debug.Log(existingStackCount);
            }

        }
    }

    int CheckStackSize(ModuleReceiver currMod)
    {

        int stack = 0;
        var assetName = assetID_new + "_";

        for (int i3 = 0; i3 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i3++)
        {
            var npcCharObj = currMod.modFilesData.npcChrData.NPCCharacters[i3];

            if (modifyDic.Count == 1)
            {
                if (npcCharObj.id.Contains(assetName))
                {
                    if (npcCharObj.id.ToCharArray().Length == assetName.ToCharArray().Length)
                    {
                        var num = System.Int32.Parse(npcCharObj.id.Replace(assetName, ""));

                        if (num > stack)
                        {
                            stack = num;
                        }

                    }
                }
            }
            else
            {
                if (npcCharObj.id.Contains(assetName))
                {
                    if (npcCharObj.id.ToCharArray().Length == assetName.ToCharArray().Length + 1 || npcCharObj.id.ToCharArray().Length == assetName.ToCharArray().Length + 2 || npcCharObj.id.ToCharArray().Length == assetName.ToCharArray().Length + 3)
                    {
                        var num = System.Int32.Parse(npcCharObj.id.Replace(assetName, ""));

                        if (num > stack)
                        {
                            stack = num;
                        }
                    }
                }
            }

        }

        //Debug.Log(stack);

        return stack;
    }

    // WIP:
    void ApplyEditAsset(ModuleReceiver currMod)
    {

        if (modifyDic.Count > 1)
        {
            var i_exist = CheckStackSize(currMod) + 1;
            //Debug.Log(i_exist);
            var i_solo = startStack;


            for (int index = 0; index < modifyDic.Count; index++)
            {
                if (contains_Bool)
                {
                    if (Bool_A)
                    {
                        var naming = assetID_new + $"_{i_exist}";
                        var dic = modifyDic.ToArray()[index];
                        var oldNaming = dic.Key.id;

                        AssetAsCopy(currMod, dic, naming);

                        if (!Bool_C)
                        {
                            RemoveOnOverride(currMod, oldNaming);
                        }
                    }
                    else if (Bool_B)
                    {

                        var naming = assetID_new + $"_{i_solo}";
                        var dic = modifyDic.ToArray()[index];
                        var oldNaming = dic.Key.id;
                        OverrideIfExist(currMod, dic, naming);
                        //Debug.Log(naming);
                        //Debug.Log(oldNaming);
                        if (!Bool_C && oldNaming != naming)
                        {
                            RemoveOnOverride(currMod, oldNaming);
                        }

                    }
                }
                else
                {

                    if (Bool_C)
                    {
                        var naming = assetID_new + $"_{i_solo}";
                        var dic = modifyDic.ToArray()[index];
                        AssetAsCopy(currMod, dic, naming);
                    }
                    else
                    {
                        var naming = assetID_new + $"_{i_solo}";
                        var dic = modifyDic.ToArray()[index];
                        EditCurrentAssets(dic, naming);
                    }

                }

                i_exist++;
                i_solo++;
            }
        }
        else
        {
            if (contains_Bool)
            {

                var naming = assetID_new;
                var dic = modifyDic.ToArray()[0];
                var oldNaming = dic.Key.id;
                OverrideIfExist(currMod, dic, naming);

                if (!Bool_C)
                {
                    RemoveOnOverride(currMod, oldNaming);
                }

            }
            else
            {

                if (Bool_C)
                {

                    var naming = assetID_new;
                    var dic = modifyDic.ToArray()[0];
                    AssetAsCopy(currMod, dic, naming);
                }
                else
                {
                    Debug.Log("WORK");

                    var naming = assetID_new;
                    var dic = modifyDic.ToArray()[0];
                    RemoveOnOverride(currMod, naming);
                    EditCurrentAssets(dic, naming);
                }

            }

        }

    


       // AssetDatabase.Refresh();
        EditorWindow.GetWindow(typeof(NPCEditor));
        this.Close();

    }
    void ApplyCopyAsset(ModuleReceiver currMod)
    {

        //string modPath = "Assets/Resources/SubModulesData/" + B_options[B_index] + ".asset";
        //var destinationMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modPath, typeof(ModuleReceiver));

        //if (Bool_D)
        //{
        //    Bool_A = true;
        //    //Bool_B = false;
        //}
        //else
        //{
        //    Bool_A = false;
        //    //Bool_B = true;
        //}



        if (modifyDic.Count > 1)
        {
            var i_exist = CheckStackSize(currMod) + 1;
            //Debug.Log(i_exist);
            var i_solo = startStack;

            for (int index = 0; index < modifyDic.Count; index++)
            {
                if (contains_Bool)
                {
                    if (Bool_A)
                    {

                        var naming = assetID_new + $"_{i_exist}";

                        if (!Bool_D)
                        {
                            naming = modifyDic.ToArray()[index].Key.id;
                        }

                        var dic = modifyDic.ToArray()[index];

                        AssetAsCopy(currMod, dic, naming);
                        //Debug.Log("BOOL A:" + Bool_A);

                    }
                    else if (Bool_B)
                    {
                        var naming = assetID_new + $"_{i_solo}";

                        if (!Bool_D)
                        {
                            naming = modifyDic.ToArray()[index].Key.id;
                        }

                        var dic = modifyDic.ToArray()[index];
                        OverrideIfExist(currMod, dic, naming);
                        Debug.Log(naming);
                        //Debug.Log(oldNaming);
                        //Debug.Log("BOOL B:" + Bool_B);


                    }

                }
                else
                {

                    var naming = assetID_new + $"_{i_solo}";

                    if (!Bool_D)
                    {
                        naming = modifyDic.ToArray()[index].Key.id;
                    }

                    var dic = modifyDic.ToArray()[index];

                    AssetAsCopy(currMod, dic, naming);

                }

                i_exist++;
                i_solo++;
            }
        }
        else
        {

            if (contains_Bool)
            {

                var naming = assetID_new;
                var dic = modifyDic.ToArray()[0];
                var oldNaming = dic.Key.id;
                OverrideIfExist(currMod, dic, naming);

            }
            else
            {

                var naming = assetID_new;

                if (!Bool_D)
                {
                    naming = modifyDic.ToArray()[0].Key.id;
                }

                var dic = modifyDic.ToArray()[0];
                var oldNaming = dic.Key.id;

                AssetAsCopy(currMod, dic, naming);

            }

        }

       // AssetDatabase.Refresh();
        EditorWindow.GetWindow(typeof(NPCEditor));
        this.Close();
    }
    void AssetAsCopy(ModuleReceiver currMod, KeyValuePair<NPCCharacter, Hero> dic, string naming)
    {

        //Debug.Log("AssetAsCopy");

        NPCCharacter data = Object.Instantiate(dic.Key);

        if (data.equipment_Roster != null && data.equipment_Roster.Length > 0)
        {
            int id_num = 0;
            foreach (var roster in data.equipment_Roster)
            {
                //Debug.Log("work");
                var new_naming = "eqp_roster_" + naming + "_" + id_num;
                var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentRosters/" + roster + ".asset";
                var created_path = dataPath + currMod.id + "/NPC/Equipment/EquipmentRosters/" + new_naming + ".asset";

                EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));
                EquipmentSet created_set = Object.Instantiate(set);
                created_set.moduleID = currMod.id;
                created_set.EquipmentSetID = naming;

				EditorUtility.SetDirty (created_set);
                AssetDatabase.CreateAsset(created_set, created_path);
                //AssetDatabase.SaveAssets();

                currMod.modFilesData.equipmentSetData.equipmentSets.Add(created_set);

                id_num++;
            }
        }

        if (data.equipment_Main != "")
        {
            var new_naming = "eqp_main_" + naming;
            var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentMain/" + data.equipment_Main + ".asset";
            var created_path = dataPath + currMod.id + "/NPC/Equipment/EquipmentMain/" + new_naming + ".asset";

            EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));
            EquipmentSet created_set = Object.Instantiate(set);
            created_set.moduleID = currMod.id;
            created_set.EquipmentSetID = naming;

			EditorUtility.SetDirty (created_set);
            AssetDatabase.CreateAsset(created_set, created_path);
            //AssetDatabase.SaveAssets();

            currMod.modFilesData.equipmentSetData.equipmentSets.Add(created_set);
        }

        var asstPath = AssetDatabase.GetAssetPath(dic.Key);
        var name = Path.GetFileName(asstPath);
        asstPath = asstPath.Replace(name, naming + ".asset");

        asstPath = asstPath.Replace(dic.Key.moduleID, currMod.id);

        data.id = naming;

        if (data.npcName == "" || data.npcName == null)

        {
            data.npcName = naming;
        }

        data.moduleID = currMod.id;

		EditorUtility.SetDirty (data);
        AssetDatabase.CreateAsset(data, asstPath);
        //AssetDatabase.SaveAssets();

        currMod.modFilesData.npcChrData.NPCCharacters.Add(data);

        if (dic.Value != null)
        {
            Hero hero = Object.Instantiate(dic.Value);

            asstPath = AssetDatabase.GetAssetPath(dic.Value);

            name = Path.GetFileName(asstPath);
            asstPath = asstPath.Replace(name, naming + ".asset");
            asstPath = asstPath.Replace(dic.Value.moduleID, currMod.id);

            hero.id = naming;
            hero.moduleID = currMod.id;
			
			EditorUtility.SetDirty (hero);
            AssetDatabase.CreateAsset(hero, asstPath);
            //AssetDatabase.SaveAssets();

            currMod.modFilesData.heroesData.heroes.Add(hero);

        }

        ReassignRosters(data, naming);

    }

    void OverrideIfExist(ModuleReceiver currMod, KeyValuePair<NPCCharacter, Hero> dic, string naming)
    {
        //Debug.Log("OverrideIfExist");

        NPCCharacter data = Object.Instantiate(dic.Key);

        if (data.equipment_Roster != null && data.equipment_Roster.Length > 0)
        {
            var id_num = 0;
            foreach (var roster in data.equipment_Roster)
            {
                var new_naming = "eqp_roster_" + naming + "_" + id_num;
                var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentRosters/" + roster + ".asset";
                var created_path = dataPath + currMod.id + "/NPC/Equipment/EquipmentRosters/" + new_naming + ".asset";

                EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));
                EquipmentSet created_set = Object.Instantiate(set);
                created_set.moduleID = currMod.id;
                created_set.EquipmentSetID = naming;

				EditorUtility.SetDirty (created_set);
                AssetDatabase.CreateAsset(created_set, created_path);
                //AssetDatabase.SaveAssets();

                currMod.modFilesData.equipmentSetData.equipmentSets.Add(created_set);
                
                //currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set);
                //AssetDatabase.DeleteAsset(path);

                id_num++;
            }
        }

        if (data.equipment_Main != "")
        {
            var new_naming = "eqp_main_" + naming;
            var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentMain/" + data.equipment_Main + ".asset";
            var created_path = dataPath + currMod.id + "/NPC/Equipment/EquipmentMain/" + new_naming + ".asset";

            EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));
            EquipmentSet created_set = Object.Instantiate(set);
            created_set.moduleID = currMod.id;
            created_set.EquipmentSetID = naming;

			EditorUtility.SetDirty (created_set);
            AssetDatabase.CreateAsset(created_set, created_path);
            //AssetDatabase.SaveAssets();

            currMod.modFilesData.equipmentSetData.equipmentSets.Add(created_set);

            //currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set);
            //AssetDatabase.DeleteAsset(path);
        }

        var asstPath = AssetDatabase.GetAssetPath(dic.Key);
        var name = Path.GetFileName(asstPath);
        asstPath = asstPath.Replace(name, naming + ".asset");

        asstPath = asstPath.Replace(dic.Key.moduleID, currMod.id);

        data.id = naming;

        if (data.npcName == "" || data.npcName == null)
        {
            data.npcName = naming;
        }

        data.moduleID = currMod.id;

        ReassignRosters(data, naming);

        RemoveOnOverride(currMod, naming);
		
		EditorUtility.SetDirty (data);
        AssetDatabase.CreateAsset(data, asstPath);
        //AssetDatabase.SaveAssets();

        currMod.modFilesData.npcChrData.NPCCharacters.Add(data);

        if (dic.Value != null)
        {
            Hero hero = Object.Instantiate(dic.Value);

            asstPath = AssetDatabase.GetAssetPath(dic.Value);

            name = Path.GetFileName(asstPath);
            asstPath = asstPath.Replace(name, naming + ".asset");
            asstPath = asstPath.Replace(dic.Value.moduleID, currMod.id);

            hero.id = naming;

			EditorUtility.SetDirty (hero);
            AssetDatabase.CreateAsset(hero, asstPath);
            //AssetDatabase.SaveAssets();

            currMod.modFilesData.heroesData.heroes.Add(hero);

        }

    }

    private void RemoveOnOverride(ModuleReceiver currMod, string nameID)
    {
        //Debug.Log("RemoveOnOverride");
        for (int index2 = 0; index2 < currMod.modFilesData.npcChrData.NPCCharacters.Count; index2++)
        {
            var data = currMod.modFilesData.npcChrData.NPCCharacters;
            if (data[index2].id == nameID)
            {
                if (data[index2].equipment_Roster != null && data[index2].equipment_Roster.Length > 0)
                {
                    int id_num = 0;
                    foreach (var roster in data[index2].equipment_Roster)
                    {
                        var RemPath = dataPath + currMod.id + "/NPC/Equipment/EquipmentRosters/" + roster + ".asset";

                        foreach (var set_eqp in currMod.modFilesData.equipmentSetData.equipmentSets)
                        {

                            if ("eqp_roster_" + set_eqp.EquipmentSetID + "_" + id_num == roster)
                            {
                                currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set_eqp);
                                AssetDatabase.DeleteAsset(RemPath);
                                break;
                            }
                        }
                        id_num++;
                    }
                }

                if (data[index2].equipment_Main != "")
                {

                    var RemPath = dataPath + currMod.id + "/NPC/Equipment/EquipmentMain/" + data[index2].equipment_Main + ".asset";

                    foreach (var set_eqp in currMod.modFilesData.equipmentSetData.equipmentSets)
                    {
                        if ("eqp_main_" + set_eqp.EquipmentSetID == data[index2].equipment_Main)
                        {
                            currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set_eqp);
                            AssetDatabase.DeleteAsset(RemPath);
                            break;
                        }
                    }

                }

                var path = AssetDatabase.GetAssetPath(data[index2]);

                currMod.modFilesData.npcChrData.NPCCharacters.Remove(data[index2]);
                AssetDatabase.DeleteAsset(path);
                // return;

            }
        }

        for (int index2 = 0; index2 < currMod.modFilesData.heroesData.heroes.Count; index2++)
        {
            var hero = currMod.modFilesData.heroesData.heroes;
            if (hero[index2].id == nameID)
            {
                // overrideAsset = npc[index];

                var RemPath = AssetDatabase.GetAssetPath(hero[index2]);

                currMod.modFilesData.heroesData.heroes.Remove(hero[index2]);
                AssetDatabase.DeleteAsset(RemPath);
                // return;
            }
        }
    }
    
    private void RemoveOnOverrideRosters(ModuleReceiver currMod, string nameID)
    {
        //Debug.Log("RemoveOnOverride");
        for (int index2 = 0; index2 < currMod.modFilesData.npcChrData.NPCCharacters.Count; index2++)
        {
            var data = currMod.modFilesData.npcChrData.NPCCharacters;
            if (data[index2].id == nameID)
            {
                if (data[index2].equipment_Roster != null && data[index2].equipment_Roster.Length > 0)
                {
                    int id_num = 0;
                    foreach (var roster in data[index2].equipment_Roster)
                    {
                        var RemPath = dataPath + currMod.id + "/NPC/Equipment/EquipmentRosters/" + roster + ".asset";

                        foreach (var set_eqp in currMod.modFilesData.equipmentSetData.equipmentSets)
                        {

                            if ("eqp_roster_" + set_eqp.EquipmentSetID + "_" + id_num == roster)
                            {
                                currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set_eqp);
                                AssetDatabase.DeleteAsset(RemPath);
                                break;
                            }
                        }
                        id_num++;
                    }
                }

                if (data[index2].equipment_Main != "")
                {

                    var RemPath = dataPath + currMod.id + "/NPC/Equipment/EquipmentMain/" + data[index2].equipment_Main + ".asset";

                    foreach (var set_eqp in currMod.modFilesData.equipmentSetData.equipmentSets)
                    {
                        if ("eqp_main_" + set_eqp.EquipmentSetID == data[index2].equipment_Main)
                        {
                            currMod.modFilesData.equipmentSetData.equipmentSets.Remove(set_eqp);
                            AssetDatabase.DeleteAsset(RemPath);
                            break;
                        }
                    }

                }
            }
        }

        for (int index2 = 0; index2 < currMod.modFilesData.heroesData.heroes.Count; index2++)
        {
            var hero = currMod.modFilesData.heroesData.heroes;
            if (hero[index2].id == nameID)
            {
                // overrideAsset = npc[index];

                var RemPath = AssetDatabase.GetAssetPath(hero[index2]);

                currMod.modFilesData.heroesData.heroes.Remove(hero[index2]);
                AssetDatabase.DeleteAsset(RemPath);
                // return;
            }
        }
    }

    private void EditCurrentAssets(KeyValuePair<NPCCharacter, Hero> dic, string naming)
    {

        //Debug.Log("EditCurrentAssets");
        if (dic.Value != null)
        {
            var h_Path = AssetDatabase.GetAssetPath(dic.Value);
            AssetDatabase.RenameAsset(h_Path, naming);
            dic.Value.id = naming;
        }

        if (dic.Key.equipment_Roster != null && dic.Key.equipment_Roster.Length > 0)
        {
            int id_num = 0;
            foreach (var roster in dic.Key.equipment_Roster)
            {
                var path_rst = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentRosters/" + roster + ".asset";
                //Debug.Log(path);
                EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path_rst, typeof(EquipmentSet));
                var new_naming = "eqp_roster_" + naming + "_" + id_num;

                AssetDatabase.RenameAsset(path_rst, new_naming);
                set.EquipmentSetID = naming;
                set.name = new_naming;

                id_num++;
            }
        }

        if (dic.Key.equipment_Main != "")
        {
            var path = dataPath + dic.Key.moduleID + "/NPC/Equipment/EquipmentMain/" + dic.Key.equipment_Main + ".asset";
            EquipmentSet set = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentSet));
            var new_naming = "eqp_main_" + naming;

            AssetDatabase.RenameAsset(path, new_naming);
            set.EquipmentSetID = naming;
            set.name = new_naming;

        }

        var asstPath = AssetDatabase.GetAssetPath(dic.Key);
        AssetDatabase.RenameAsset(asstPath, naming);
        dic.Key.id = naming;
        dic.Key.name = naming;

        ReassignRosters(dic.Key, naming);
    }

    private static void ReassignRosters(NPCCharacter data, string naming )
    {

        if (data.equipment_Roster != null && data.equipment_Roster.Length > 0)
        {
            for (int rst = 0; rst < data.equipment_Roster.Length; rst++)
            {
                data.equipment_Roster[rst] = "eqp_roster_" + naming + "_" + rst;
                //Debug.Log(dic.Key.equipment_Roster[rst]);
            }
        }

        if (data.equipment_Main != "")
        {
            data.equipment_Main = "eqp_main_" + naming;
        }

      
    }

    private void CreateAsset()
    {

        A_options = new string[2];
        A_options[0] = "Solo";
        A_options[1] = "Stack";

        ModuleReceiver currMod;
        if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
        {
            currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
        }
        else
        {
            return;
        }

        this.title = "Create Character Asset";

        EditorGUILayout.HelpBox("Create Character Asset for " + "(" + bdt_settings.currentModule + ")" + " module.", MessageType.Info);

        DrawUILine(colUILine, 2, 8);

        EditorGUILayout.LabelField("Create:", EditorStyles.boldLabel);
        A_index = EditorGUILayout.Popup(A_index, A_options, GUILayout.Width(128));

        if (A_index == 1)
        {
            DrawUILine(colUILine, 1, 3);
            EditorGUILayout.Space(4);

            var originLabelWidth = EditorGUIUtility.labelWidth;
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Stack Count: "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            stack_count = EditorGUILayout.IntField("Stack Count:", stack_count, GUILayout.MaxWidth(162));

            startStack = EditorGUILayout.IntField("Stack ID num:", startStack, GUILayout.MaxWidth(128));

            if (startStack < 0)
                startStack = 0;

            if (stack_count < 2)
            {
                stack_count = 2;
            }
            else if (stack_count > 99)
            {
                stack_count = 99;
            }


            EditorGUIUtility.labelWidth = originLabelWidth;
        }
        else
        {
            stack_count = 1;
            startStack = 0;
        }

        EditorGUILayout.Space(4);

        CreateEditorToggle(ref useObjectRef, "Use Object Reference");

        if (useObjectRef)
        {
            EditorGUILayout.Space(4);

            NPCCharacter eqpRef = (NPCCharacter)object_reference_A;
            object_reference_A = EditorGUILayout.ObjectField(eqpRef, typeof(NPCCharacter), true);
            // object_reference_A = npcRef;
        }

        EditorGUILayout.Space(4);

        NPCCharacter refOBJ = (NPCCharacter)object_reference_A;

        if ((NPC_is_Hero == true && useObjectRef == false) || (useObjectRef == true && refOBJ != null && refOBJ.is_hero == "true" && refOBJ.COMP_Companion != "true"))
        {
            EditorGUILayout.Space(4);

            CreateEditorToggle(ref createHeroData, "Create Hero Data");

            if (createHeroData)
            {
                EditorGUILayout.Space(4);

                CreateEditorToggle(ref assignHeroFaction, "Assign Hero Faction");

                if (assignHeroFaction)
                {

                    EditorGUILayout.Space(4);

                    Faction facRef = (Faction)object_Faction_ref;
                    object_Faction_ref = EditorGUILayout.ObjectField(facRef, typeof(Faction), true);

                    // Faction refOBJ = (Faction)object_reference_A;

                }

            }

        }


        GUILayout.Space(4);
        DrawUILine(colUILine, 2, 8);


        assetID_new = EditorGUILayout.TextField("Character ID:", assetID_new);

        int stackExist = 0;
        contains_Bool = false;

        for (int i2 = 0; i2 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i2++)
        {
            for (int i3 = 0; i3 < stack_count; i3++)
            {
                var assetName = "";

                if (A_index == 0)
                {
                    assetName = assetID_new;
                }
                else if (A_index == 1)
                {
                    assetName = assetID_new + "_" + (startStack + i3 + 1);
                }

                // Debug.Log(assetName);
                if (currMod.modFilesData.npcChrData.NPCCharacters[i2].id == assetName)
                {
                    contains_Bool = true;
                    break;
                    // existingStackCount = existingStackCount + 1;
                }
            }

            if (contains_Bool)
                break;
        }

        if (contains_Bool)
        {
            //existingStackCount = 0;
            var assetName = assetID_new + "_";

            for (int i3 = 0; i3 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i3++)
            {
                var facObj = currMod.modFilesData.npcChrData.NPCCharacters[i3];

                if (A_index == 0)
                {
                    if (facObj.id.Contains(assetName))
                    {
                        if (facObj.id.ToCharArray().Length == assetName.ToCharArray().Length)
                        {
                            //existingStackCount++;

                            var num = System.Int32.Parse(facObj.id.Replace(assetName, ""));

                            if (num > stackExist)
                            {
                                stackExist = num;
                            }
                        }
                    }
                }
                else if (A_index == 1)
                {
                    if (facObj.id.Contains(assetName))
                    {
                        if (facObj.id.ToCharArray().Length == assetName.ToCharArray().Length + 1 || facObj.id.ToCharArray().Length == assetName.ToCharArray().Length + 2 || facObj.id.ToCharArray().Length == assetName.ToCharArray().Length + 3)
                        {
                            //existingStackCount++;

                            var num = System.Int32.Parse(facObj.id.Replace(assetName, ""));

                            if (num > stackExist)
                            {
                                stackExist = num;
                            }
                        }
                    }
                }
            }


            // Debug.Log(existingStackCount);
        }

        DrawUILine(colUILine, 2, 8);

        if (A_index != 0)
        {
            EditorGUILayout.LabelField("Name preview:", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("(name + count)", EditorStyles.miniLabel);
            EditorGUILayout.LabelField(assetID_new + "_" + (startStack + stack_count), EditorStyles.miniBoldLabel);
            DrawUILine(colUILine, 2, 8);
        }


        NPCCharacter eqp = (NPCCharacter)ScriptableObject.CreateInstance(typeof(NPCCharacter));

        // if (A_index != 0)
        // {
        //     if (!useObjectRef)
        //     {
        //         EditorGUILayout.BeginHorizontal();

        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_basic_troop, "Is Basic Troop");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_Mercenary, "Is Mercenary");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_Hero, "Is Hero");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_Companion, "Is Companion");
        //         GUILayout.FlexibleSpace();

        //         EditorGUILayout.EndHorizontal();

        //         DrawUILine(colUILine, 2, 8);

        //         EditorGUILayout.BeginHorizontal();

        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_Template, "Is Template");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_Chld_template, "Is Child Template");
        //         // DrawUILineVertical(colUILine, 1, 1, 16);
        //         // CreateEditorToggle(ref NPC_is_male, "Is Male");
        //         DrawUILineVertical(colUILine, 1, 1, 16);
        //         CreateEditorToggle(ref NPC_is_female, "Is Female");

        //         GUILayout.FlexibleSpace();

        //         EditorGUILayout.EndHorizontal();

        //         DrawUILine(colUILine, 2, 8);
        //     }

        // }
        // else
        // {

        if (!useObjectRef)
        {

            //BOOLS DEFINITIONS

            EditorGUILayout.BeginHorizontal();

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref NPC_is_Hero, "Is Hero");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref NPC_is_Template, "Is Template");

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref NPC_is_female, "Is Female");

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            DrawUILine(colUILine, 2, 8);
        }

        // }

        if (contains_Bool)
        {
            existingStackCount = 0;
            var nm = "";

            for (int i3 = 0; i3 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i3++)
            {
                var npcCharObj = currMod.modFilesData.npcChrData.NPCCharacters[i3];

                for (int i4 = 0; i4 < stack_count; i4++)
                {

                    if (A_index == 1)
                    {
                        nm = assetID_new + "_" + (startStack + i4 + 1);
                        //Debug.Log(nm);

                        if (npcCharObj.id == nm)
                        {
                            existingStackCount++;
                        }
                    }
                }
            }
        }

        string createAssetsLabel = "Create Asset";

        // EditorGUILayout.Space(4);

        if (contains_Bool)
        {
            if (A_index == 0)
            {
                EditorGUILayout.HelpBox($"{assetID_new} - asset already exist in {settingsAsset.currentModule} module, do you want to override it?", MessageType.Warning);
                createAssetsLabel = "Override Asset";

            }
            else if (A_index == 1)
            {
                EditorGUILayout.HelpBox($"{existingStackCount} - Assets with this names already exist in {settingsAsset.currentModule} module, do you want to do?", MessageType.Warning);

                CreateEditorToggle(ref Bool_A, "Add to existing Stack");
                if (Bool_A)
                {
                    Bool_B = false;
                    createAssetsLabel = "Create Assets";

                }
                CreateEditorToggle(ref Bool_B, "Override if exist");
                if (Bool_B)
                {
                    Bool_A = false;
                    createAssetsLabel = "Create & Override Assets";
                }
            }
        }

        EditorGUILayout.Space(4);
        var isSelectedRef = true;

        if (useObjectRef == true)
        {
            if (object_reference_A == null)
            {
                isSelectedRef = false;
            }
        }

        //Debug.Log(isSelectedRef);
        if (isSelectedRef)
        {
            if (GUILayout.Button(createAssetsLabel))
            {

                if (A_index == 0)
                {
                    stack_count = 1;
                }


                for (int i = 0; i < stack_count; i++)
                {

                    var assetName = assetID_new + "_" + (startStack + i);

                    if (Bool_A)
                    {
                        assetName = assetID_new + "_" + (stackExist + i + 1);
                    }

                    if (A_index == 0)
                    {
                        assetName = assetID_new;
                    }


                    if (contains_Bool)
                    {
                        RemoveOnOverrideRosters(currMod,assetName);
                    }


                    NPCCharacter asset = ScriptableObject.CreateInstance<NPCCharacter>();
                    string initial_module_id = "";

                    if (useObjectRef)
                    {
                        if (object_reference_A != null)
                        {
                            NPCCharacter npcRef = (NPCCharacter)object_reference_A;
                            asset = ScriptableObject.Instantiate(npcRef);
                            initial_module_id = asset.moduleID;
                        }
                        else
                        {
                            Debug.Log("Ref object Null!");
                            return;
                        }

                    }

                    asset.moduleID = settingsAsset.currentModule;
                    // npcAsset.name = assetName;
                    asset.id = assetName;

                    if (asset.npcName == "" || asset.npcName == null)
                    {
                        asset.npcName = assetName;
                    }

                    if (!useObjectRef)
                    {

                        //BOOLS CHECK

                        asset.occupation = "NotAssigned";

                        if (NPC_is_Hero)
                        {
                            asset.is_hero = "true";
                        }
                        else if (NPC_is_female)
                        {
                            asset.is_female = "true";
                        }
                        else if (NPC_is_Template)
                        {
                            asset.is_template = "true";
                        }

                    }

                    string path;

                    if (asset.is_template == "true" || asset.is_child_template == "true")
                    {
						EditorUtility.SetDirty (asset);
                        path = dataPath + currMod.id + "/_Templates/NPCtemplates/" + assetName + ".asset";
                        AssetDatabase.CreateAsset(asset, path);
                        //AssetDatabase.SaveAssets();
                    }
                    else
                    {
						EditorUtility.SetDirty (asset);
                        path = dataPath + currMod.id + "/NPC/" + assetName + ".asset";
                        AssetDatabase.CreateAsset(asset, path);
                        //AssetDatabase.SaveAssets();
                    }


                    contains_Bool = false;
                    for (int i2 = 0; i2 < currMod.modFilesData.npcChrData.NPCCharacters.Count; i2++)
                    {
                        if (currMod.modFilesData.npcChrData.NPCCharacters[i2].id == asset.id)
                        {
                            var eqpLoad = (NPCCharacter)AssetDatabase.LoadAssetAtPath(path, typeof(NPCCharacter));

                            currMod.modFilesData.npcChrData.NPCCharacters[i2] = eqpLoad;
                            contains_Bool = true;
                            break;
                        }
                    }

                    if (!contains_Bool)
                    {
                        var itemLoad = (NPCCharacter)AssetDatabase.LoadAssetAtPath(path, typeof(NPCCharacter));
                        currMod.modFilesData.npcChrData.NPCCharacters.Add(itemLoad);
                    }

                  
                    // create sets
                    if (asset.equipment_Roster != null && asset.equipment_Roster.Length > 0)
                    {
                        for (int i_id = 0; i_id < asset.equipment_Roster.Length; i_id++)
                        {

                            EquipmentSet set = new EquipmentSet();

                            if (useObjectRef)
                            {
                                var path_sets_init = dataPath + initial_module_id + "/NPC/Equipment/EquipmentRosters/" + asset.equipment_Roster[i_id] + ".asset";
                                //Debug.Log(path_sets_init);
                                set = Object.Instantiate((EquipmentSet)AssetDatabase.LoadAssetAtPath(path_sets_init, typeof(EquipmentSet)));
                            }
                           

                            asset.equipment_Roster[i_id] = "eqp_roster_" + assetName + "_" + i_id;
                            var path_sets = dataPath + currMod.id + "/NPC/Equipment/EquipmentRosters/" + "eqp_roster_" + assetName + "_" + i_id + ".asset";

                            set.EquipmentSetID = assetName;
                            set.moduleID = settingsAsset.currentModule;
                            set._is_roster = true;

								EditorUtility.SetDirty (set);
                            AssetDatabase.CreateAsset(set, path_sets);
                            //AssetDatabase.SaveAssets();

                            var eqpLoad = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path_sets, typeof(EquipmentSet));

                            if (!currMod.modFilesData.equipmentSetData.equipmentSets.Contains(eqpLoad))
                                currMod.modFilesData.equipmentSetData.equipmentSets.Add(eqpLoad);

                        }
                    }

                    //Debug.Log(asset.equipment_Main);
                    if (asset.equipment_Main != null  && asset.equipment_Main != "")
                    {

                        EquipmentSet set = new EquipmentSet();

                        if (useObjectRef)
                        {
                            var path_sets_init = dataPath + initial_module_id + "/NPC/Equipment/EquipmentMain/" + asset.equipment_Main + ".asset";

                            set = Object.Instantiate((EquipmentSet)AssetDatabase.LoadAssetAtPath(path_sets_init, typeof(EquipmentSet)));
                        }
                        

                        asset.equipment_Main = "eqp_main_" + assetName;
                        var path_sets = dataPath + currMod.id + "/NPC/Equipment/EquipmentMain/" + "eqp_main_" + assetName + ".asset";

                        set.EquipmentSetID = assetName;
                        set.moduleID = settingsAsset.currentModule;
                        set._is_main = true;

						EditorUtility.SetDirty (set);
                        AssetDatabase.CreateAsset(set, path_sets);
                        //AssetDatabase.SaveAssets();

                        var eqpLoad = (EquipmentSet)AssetDatabase.LoadAssetAtPath(path_sets, typeof(EquipmentSet));

                        if (!currMod.modFilesData.equipmentSetData.equipmentSets.Contains(eqpLoad))
                            currMod.modFilesData.equipmentSetData.equipmentSets.Add(eqpLoad);

                    }

                    ReassignRosters(asset, assetName);

                    /// create heroes

                    if (createHeroData)
                    {
                        Hero heroAsset = ScriptableObject.CreateInstance<Hero>();

                        heroAsset.moduleID = settingsAsset.currentModule;
                        heroAsset.id = assetName;

                        if (assignHeroFaction && object_Faction_ref != null)
                        {
                            Faction facH = (Faction)object_Faction_ref;
                            heroAsset.faction = "Faction." + facH.id;

                        }

                        path = dataPath + currMod.id + "/Heroes/" + assetName + ".asset";

                        contains_Bool = false;

                        // Debug.Log(heroAsset.id);

                        for (int i2 = 0; i2 < currMod.modFilesData.heroesData.heroes.Count; i2++)
                        {
                            if (currMod.modFilesData.heroesData.heroes[i2] != null)
                            {
                                if (currMod.modFilesData.heroesData.heroes[i2].id == heroAsset.id)
                                {
                                    contains_Bool = true;
                                    break;
                                }
                            }
                        }

                        if (!contains_Bool)
                        {
							EditorUtility.SetDirty (heroAsset);
                            AssetDatabase.CreateAsset(heroAsset, path);
                           // AssetDatabase.SaveAssets();

                            var heroLoad = (Hero)AssetDatabase.LoadAssetAtPath(path, typeof(Hero));
                            currMod.modFilesData.heroesData.heroes.Add(heroLoad);
                        }

                    }

                }

               // AssetDatabase.Refresh();
                EditorWindow.GetWindow(typeof(NPCEditor));
                this.Close();
            }
        }

    }

    private static void CreateEditorToggle(ref bool toggleBool, string label)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        toggleBool = EditorGUILayout.Toggle(label, toggleBool);

        EditorGUIUtility.labelWidth = originLabelWidth;

    }

    void CreateIntAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        int val;
        int.TryParse(attribute, out val);
        val = EditorGUILayout.IntField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

    }
    void CreateFloatAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        float val;
        float.TryParse(attribute, out val);
        val = EditorGUILayout.FloatField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

    }
    // UI DRAW TOOLS
    public static void DrawUILineVerticalLarge(Color color, int spaceX)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(3));
        r.height = 44;
        r.x += spaceX;
        r.y -= 10;
        r.height += 32;
        EditorGUI.DrawRect(r, color);
    }
    public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10, int lenght = 4)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
        r.height = thickness;
        r.x += padding / 2;
        r.y -= 2;
        r.height += 6 + lenght;
        EditorGUI.DrawRect(r, color);
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}

