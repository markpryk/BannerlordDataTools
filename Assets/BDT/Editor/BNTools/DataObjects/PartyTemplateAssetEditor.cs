using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

[CustomEditor(typeof(PartyTemplate))]
public class PartyTemplateAssetEditor : Editor
{
    string dataPath = "Assets/BDT/Resources/Data/";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    // string folder = "NPCTranslationData";
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    //
    public NPCCharacter[] StackTroops;

    //
    PartyTemplate PTemplate;
    bool showStacksEditor = true;


    bool isDependency = false;
    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";
    public void OnEnable()
    {
        PTemplate = (PartyTemplate)target;
        EditorUtility.SetDirty(PTemplate);


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

        if (PTemplate.PTS_troop != null && PTemplate.PTS_troop.Length > 0)
        {
            StackTroops = new NPCCharacter[PTemplate.PTS_troop.Length];
        }
        else
        {
            StackTroops = new NPCCharacter[0];
        }
    }
    public override void OnInspectorGUI()
    {
        if (settingsAsset.currentModule != PTemplate.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == PTemplate.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + PTemplate.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + PTemplate.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }

            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + PTemplate.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + PTemplate.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + PTemplate.moduleID + ".asset", typeof(ModuleReceiver));
                    window.source = mod;
                    window.currentMod = mod;
                    settingsAsset.currentModule = mod.id;

                    window.Repaint();
                    this.Repaint();

                    // Debug.Log("Switched to mod: " + window.currentMod);
                }

            }


            if (isDependency)
            {
                if (GUILayout.Button("Create Override Asset"))
                {
                    if (ADM_Instance == null)
                    {
                        AssetsDataManager assetMng = new AssetsDataManager();
                        assetMng.windowStateID = 1;
                        assetMng.objID = 6;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = PTemplate;

                        assetMng.assetName_org = PTemplate.id;
                        assetMng.assetName_new = PTemplate.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 6;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = PTemplate;

                        assetMng.assetName_org = PTemplate.id;
                        assetMng.assetName_new = PTemplate.id;
                    }

                    // assetMng.CopyAssetAsOverride();
                }
            }
            GUILayout.EndHorizontal();
            DrawUILine(colUILine, 3, 12);

        }
        else
        {
            isDependency = false;
        }

        EditorGUI.BeginDisabledGroup(isDependency);

        var labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);

        EditorGUI.BeginDisabledGroup(true);
        PTemplate.id = EditorGUILayout.TextField("Party Template ID", PTemplate.id);

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {
            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 6;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = PTemplate;

                assetMng.assetName_org = PTemplate.id;
                assetMng.assetName_new = PTemplate.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 6;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = PTemplate;

                assetMng.assetName_org = PTemplate.id;
                assetMng.assetName_new = PTemplate.id;
            }
        }
        DrawUILine(colUILine, 3, 12);

        DrawUpgradeTargetsEditor();

    }

    void DrawUpgradeTargetsEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#FFB900", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#66ccff", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Party Stacks: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showStacksEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Party Template Stacks Editor";
        }

        showStacksEditor = EditorGUILayout.Foldout(showStacksEditor, showEditorLabel, hiderStyle);

        if (showStacksEditor)
        {



            EditorGUILayout.LabelField("Party Template Stacks Editor", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(4);
            DrawUILine(colUILine, 1, 6);


            if (GUILayout.Button((new GUIContent("Add Stack", "Add new Party Template Stack")), buttonStyle, GUILayout.Width(128)))
            {

                //int i2 = 0;

                // disable stack creation if not filled by object
                //if (PTemplate.PTS_troop != null && PTemplate.PTS_troop.Length != 0 )
                //{
                //    if (PTemplate.PTS_troop[PTemplate.PTS_troop.Length - 1] == "")
                //    {
                //        return;
                //    }
                //}

                //var troopsValues = new Dictionary<string, string>();
                //var listMaxVal = new List<string>();

                //i2 = 0;
                //foreach (string troop in PTemplate.PTS_troop)
                //{
                //    if (!troopsValues.ContainsKey(PTemplate.PTS_troop[i2]))
                //    {
                //        troopsValues.Add(PTemplate.PTS_troop[i2], PTemplate.PTS_min_value[i2]);
                //        listMaxVal.Add(PTemplate.PTS_max_value[i2]);

                //        i2++;
                //    }

                //}

                //int indexVal = troopsValues.Count + 1;

                //PTemplate.PTS_troop = new string[indexVal];
                //PTemplate.PTS_min_value = new string[indexVal];
                //PTemplate.PTS_max_value = new string[indexVal];

                //i2 = 0;
                //foreach (var element in troopsValues)
                //{
                //    PTemplate.PTS_troop[i2] = element.Key;
                //    PTemplate.PTS_min_value[i2] = element.Value;
                //    PTemplate.PTS_max_value[i2] = listMaxVal[i2];

                //    i2++;
                //}

                //PTemplate.PTS_troop[PTemplate.PTS_troop.Length - 1] = "";
                //PTemplate.PTS_min_value[PTemplate.PTS_min_value.Length - 1] = "0";
                //PTemplate.PTS_max_value[PTemplate.PTS_max_value.Length - 1] = "0";

                //StackTroops = new NPCCharacter[PTemplate.PTS_troop.Length];

                if (PTemplate.PTS_troop == null)
                    PTemplate.PTS_troop = new string[0];

                if (PTemplate.PTS_min_value == null)
                    PTemplate.PTS_min_value = new string[0];

                if (PTemplate.PTS_max_value == null)
                    PTemplate.PTS_max_value = new string[0];

                var temp = new string[PTemplate.PTS_troop.Length + 1];
                PTemplate.PTS_troop.CopyTo(temp, 0);
                PTemplate.PTS_troop = temp;

                PTemplate.PTS_troop[PTemplate.PTS_troop.Length - 1] = "";

                temp = new string[PTemplate.PTS_min_value.Length + 1];
                PTemplate.PTS_min_value.CopyTo(temp, 0);
                PTemplate.PTS_min_value = temp;

                PTemplate.PTS_min_value[PTemplate.PTS_min_value.Length - 1] = "0";

                temp = new string[PTemplate.PTS_max_value.Length + 1];
                PTemplate.PTS_max_value.CopyTo(temp, 0);
                PTemplate.PTS_max_value = temp;

                PTemplate.PTS_max_value[PTemplate.PTS_max_value.Length - 1] = "0";

                StackTroops = new NPCCharacter[PTemplate.PTS_troop.Length];

                return;
            }


            if (PTemplate.PTS_troop != null && PTemplate.PTS_troop.Length > 0)
            {

                int i = 0;
                foreach (var targetAsset in PTemplate.PTS_troop)
                {
                    // Debug.Log(UpgradeTargets.Length);
                    GetNPCAsset(ref PTemplate.PTS_troop[i], ref StackTroops[i], false);
                    if (StackTroops[i] == null)
                    {
                        GetNPCAsset(ref PTemplate.PTS_troop[i], ref StackTroops[i], true);
                    }
                    i++;
                }

                DrawUILine(colUILine, 3, 12);

                i = 0;
                foreach (var targetAsset in PTemplate.PTS_troop)
                {

                    // EditorGUILayout.LabelField("Upgrade Target:", EditorStyles.label);


                    ColorUtility.TryParseHtmlString("#F25022", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 11;
                    EditorGUILayout.LabelField("Stack - " + i, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);
                    ColorUtility.TryParseHtmlString("#FF9900", out newCol);
                    titleStyle.normal.textColor = newCol;

                    titleStyle.fontSize = 12;

                    string nameLabel = "None";
                    if (StackTroops[i] != null)
                    {
                        nameLabel = StackTroops[i].npcName;
                    }

                    RemoveTSString(ref nameLabel);

                    EditorGUILayout.LabelField(nameLabel, titleStyle, GUILayout.ExpandWidth(true));
                    // EditorGUILayout.Space(8);

                    EditorGUILayout.BeginHorizontal();
                    object npcTargetField = EditorGUILayout.ObjectField(StackTroops[i], typeof(NPCCharacter), true, GUILayout.MaxWidth(320));
                    StackTroops[i] = (NPCCharacter)npcTargetField;

                    if (StackTroops[i] != null)
                    {
                        PTemplate.PTS_troop[i] = "NPCCharacter." + StackTroops[i].id;
                    }
                    else
                    {
                        PTemplate.PTS_troop[i] = "";
                    }

                    buttonStyle.hover.textColor = Color.red;

                    if (GUILayout.Button((new GUIContent("X", "Remove Party Stack")), buttonStyle, GUILayout.Width(32)))
                    {
                        //var troopsValues = new Dictionary<string, string>();
                        //var listMaxVal = new List<string>();

                        //PTemplate.PTS_troop[i] = "remove";

                        //int i2 = 0;
                        //foreach (string trg in PTemplate.PTS_troop)
                        //{
                        //    if (trg != "remove")
                        //    {
                        //        troopsValues.Add(PTemplate.PTS_troop[i2], PTemplate.PTS_min_value[i2]);
                        //        listMaxVal.Add(PTemplate.PTS_max_value[i2]);

                        //    }
                        //    i2++;
                        //}

                        //PTemplate.PTS_troop = new string[troopsValues.Count];

                        //i2 = 0;
                        //foreach (var obj in troopsValues)
                        //{
                        //    PTemplate.PTS_troop[i2] = obj.Key;
                        //    PTemplate.PTS_min_value[i2] = obj.Value;
                        //    PTemplate.PTS_max_value[i2] = listMaxVal[i2];
                        //    i2++;
                        //}

                        //StackTroops = new NPCCharacter[PTemplate.PTS_troop.Length];

                        var count = PTemplate.PTS_troop.Length - 1;
                        var pt_troop = new string[count];
                        var pt_min_val = new string[count];
                        var pt_max_val = new string[count];

                        int i2 = 0;
                        int i3 = 0;
                        foreach (string trg in PTemplate.PTS_troop)
                        {
                            if (i3 != i)
                            {
                                pt_troop[i2] = PTemplate.PTS_troop[i3];
                                pt_min_val[i2] = PTemplate.PTS_min_value[i3];
                                pt_max_val[i2] = PTemplate.PTS_max_value[i3];
                                i2++;
                            }
                            i3++;
                        }

                        PTemplate.PTS_troop = pt_troop;
                        PTemplate.PTS_min_value = pt_min_val;
                        PTemplate.PTS_max_value = pt_max_val;

                        StackTroops = new NPCCharacter[PTemplate.PTS_troop.Length];

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    CreateIntAttribute(ref PTemplate.PTS_min_value[i], "Min Value:");
                    DrawUILineVertical(colUILine, 1, 1, 16);
                    CreateIntAttribute(ref PTemplate.PTS_max_value[i], "Max Value:");
                    if (EditorGUI.EndChangeCheck())
                    {
                        int val_max;
                        int.TryParse(PTemplate.PTS_max_value[i], out val_max);
                        int val_min;
                        int.TryParse(PTemplate.PTS_min_value[i], out val_min);

                        if (val_max <= val_min)
                            PTemplate.PTS_max_value[i] = val_min.ToString();

                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(4);



                    DrawUILine(colUILine, 1, 4);
                    i++;
                }
            }
        }
    }

    void CreateIntAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        int val;
        int.TryParse(attribute, out val);

        val = EditorGUILayout.IntField(label, val, GUILayout.MaxWidth(162));

        if (val < 0)
            val = 0;

        attribute = val.ToString();

    }
    private void GetNPCAsset(ref string npcLink, ref NPCCharacter npcCharacter, bool npcTemplates)
    {
        // Face Key Template template
        // 
        if (npcLink != null && npcLink != "")
        {
            if (npcLink.Contains("NPCCharacter."))
            {
                // string[] assetFiles = Directory.GetFiles(dataPath + npc.moduleName + "/_Templates/NPCtemplates/", "*.asset");

                string dataName = npcLink.Replace("NPCCharacter.", "");

                string assetPath;
                string assetPathShort;

                if (npcTemplates)
                {
                    assetPath = dataPath + PTemplate.moduleID + "/_Templates/NPCtemplates/" + dataName + ".asset";
                    assetPathShort = "/_Templates/NPCtemplates/" + dataName + ".asset";
                }
                else
                {
                    assetPath = dataPath + PTemplate.moduleID + "/NPC/" + dataName + ".asset";
                    assetPathShort = "/NPC/" + dataName + ".asset";
                }

                if (System.IO.File.Exists(assetPath))
                {
                    npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(NPCCharacter));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + PTemplate.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + assetPathShort;

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
                                break;
                            }
                            else
                            {
                                npcCharacter = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (npcCharacter == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == PTemplate.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.npcChrData.NPCCharacters)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + assetPathShort;
                                            npcCharacter = (NPCCharacter)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(NPCCharacter));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (npcCharacter == null)
                        {
                            if (npcTemplates)
                            {
                                Debug.Log("NPCCharacter " + dataName + " - Not EXIST in" + " ' " + PTemplate.moduleID + " ' " + "resources, and they dependencies.");
                            }
                        }
                    }

                }
            }
        }
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
    public static AssetsDataManager ADM_Instance
    {
        get { return EditorWindow.GetWindow<AssetsDataManager>(); }
    }


    // UI DRAW TOOLS
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
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
    public static void DrawUILineVerticalLarge(Color color, int spaceX)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(3));
        r.height = 44;
        r.x += spaceX;
        r.y -= 10;
        r.height += 32;
        EditorGUI.DrawRect(r, color);
    }
}
