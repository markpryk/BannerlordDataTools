using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// Creates an instance of a primitive depending on the option selected by the user.
public class ModListPopup : EditorWindow
{
    // string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    public string[] options;
    public int index = 0;

    public BNDataEditorWindow DataWindow;
    public List<ModuleReceiver> modList;
    Dictionary<string, bool> isSelectedDPD;
    Vector2 textDimensions;

    // public List<ModuleReceiver> modDependencies;
    // [MenuItem("Examples/Editor GUILayout Popup usage")]

    public bool _showImportOptions;

    public bool _cultures = true;
    public bool _kingdoms = true;
    public bool _factions = true;
    public bool _heroes = true;
    public bool _NPC = true;
    public bool _items = true;
    public bool _equip = true;
    public bool _settlement = true;
    public bool _pt = true;

    private bool _alreadyImported;
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(ModListPopup));
        window.Show();
    }

    void OnGUI()
    {

        if (isSelectedDPD == null)
        {
            isSelectedDPD = new Dictionary<string, bool>();
            foreach (var mod in modList)
            {
                isSelectedDPD.Add(mod.id, true);
            }
        }

        DrawUILine(colUILine, 3, 12);
        EditorGUILayout.LabelField("Import Module:", EditorStyles.boldLabel);
        index = EditorGUILayout.Popup(index, options);

        string doString = "Import Module";
        var originLabelWidth = EditorGUIUtility.labelWidth;
        textDimensions = GUI.skin.label.CalcSize(new GUIContent("--"));
        EditorGUIUtility.labelWidth = textDimensions.x;

        if (File.Exists(modsSettingsPath + options[index] + ".asset"))
        {
            EditorGUILayout.Space(2);
            // Debug.Log(options[index]);
            ModuleReceiver selectedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + options[index] + ".asset", typeof(ModuleReceiver));

            GUIStyle styleB = new GUIStyle(EditorStyles.miniLabel);
            styleB.normal.textColor = new Color32(102, 255, 102, 255);
            styleB.alignment = TextAnchor.UpperCenter;
            GUIStyle styleD = new GUIStyle(EditorStyles.miniLabel);
            styleD.normal.textColor = new Color32(255, 153, 51, 255);
            styleD.alignment = TextAnchor.UpperCenter;

            var versionUpdate = false;

            CheckVersion(modList[index].version, selectedMod.version, ref versionUpdate);

            if (versionUpdate)
            {
                string msg = "(Update avitable to version: " + modList[index].version + ")";
                EditorGUILayout.BeginHorizontal();
                // EditorGUILayout.LabelField(modList[index].version, styleA, GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField(msg, styleB);
                // GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                doString = "Update Module";

            }
            else
            {
                string msg = "(Already Imported) " + selectedMod.version;
                EditorGUILayout.BeginHorizontal();

                // EditorGUILayout.LabelField(selectedMod.id, styleC, GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField(msg, styleD);

                EditorGUILayout.EndHorizontal();

                doString = "Reimport Module";

                _alreadyImported = true;
            }
        }
        DrawUILine(colUILine, 3, 4);

        // DEPENDENCIES
        EditorGUILayout.LabelField("Depended Modules:", EditorStyles.boldLabel);

        foreach (string dependency in modList[index].modDependenciesInternal)
        {

            if (dependency == "Native")
            {



                GUIStyle styleA = new GUIStyle(EditorStyles.miniBoldLabel);
                GUIStyle styleB = new GUIStyle(EditorStyles.miniLabel);
                styleA.normal.textColor = Color.gray;
                styleB.normal.textColor = Color.gray;


                string msg = "| (Import locked by deafult) ";
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Toggle(false, GUILayout.Width(16));
                EditorGUILayout.LabelField(dependency, styleA, GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField(msg, styleB);
                // GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

            }
            else
            {
                if (System.IO.File.Exists(modsSettingsPath + dependency + ".asset"))
                {
                    ModuleReceiver dependMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + dependency + ".asset", typeof(ModuleReceiver));

                    foreach (var mod in modList)
                    {
                        if (mod.id == dependency)
                        {

                            GUIStyle styleA = new GUIStyle(EditorStyles.miniBoldLabel);
                            GUIStyle styleB = new GUIStyle(EditorStyles.miniLabel);

                            if (isSelectedDPD[dependency])
                            {
                                styleA.normal.textColor = new Color32(102, 255, 102, 255);
                                styleB.normal.textColor = new Color32(102, 255, 102, 255);
                            }


                            GUIStyle styleC = new GUIStyle(EditorStyles.miniBoldLabel);
                            GUIStyle styleD = new GUIStyle(EditorStyles.miniLabel);
                            styleC.normal.textColor = new Color32(255, 153, 51, 255);
                            styleD.normal.textColor = new Color32(255, 153, 51, 255);

                            var versionUpdate = false;

                            CheckVersion(mod.version, dependMod.version, ref versionUpdate);

                            if (versionUpdate)
                            {
                                string msg = "| (Update avitable to version) " + mod.version;
                                EditorGUILayout.BeginHorizontal();
                                isSelectedDPD[mod.id] = EditorGUILayout.Toggle(isSelectedDPD[mod.id], GUILayout.Width(16));
                                EditorGUILayout.LabelField(mod.id, styleA, GUILayout.ExpandWidth(false));
                                EditorGUILayout.LabelField(msg, styleB);

                                EditorGUILayout.EndHorizontal();

                            }
                            else
                            {
                                string msg = "| (Already Imported) " + dependMod.version;
                                EditorGUILayout.BeginHorizontal();
                                isSelectedDPD[dependMod.id] = EditorGUILayout.Toggle(isSelectedDPD[dependMod.id], GUILayout.Width(16));
                                EditorGUILayout.LabelField(dependMod.id, styleC, GUILayout.ExpandWidth(false));
                                EditorGUILayout.LabelField(msg, styleD);

                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                }
                else
                {

                    GUIStyle styleA = new GUIStyle(EditorStyles.miniBoldLabel);
                    GUIStyle styleB = new GUIStyle(EditorStyles.miniLabel);

                    if (isSelectedDPD[dependency])
                    {
                        styleA.normal.textColor = new Color32(51, 204, 51, 255);
                        styleB.normal.textColor = new Color32(51, 204, 51, 255);
                    }


                    string V = "";
                    foreach (var mod in modList)
                    {
                        if (mod.id == dependency)
                            V = mod.version;
                    }

                    string msg = "| " + V;
                    EditorGUILayout.BeginHorizontal();
                    isSelectedDPD[dependency] = EditorGUILayout.Toggle(isSelectedDPD[dependency], GUILayout.Width(16));
                    EditorGUILayout.LabelField(dependency, styleA, GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField(msg, styleB);

                    EditorGUILayout.EndHorizontal();

                    // EditorGUILayout.LabelField(dependency, EditorStyles.miniLabel);
                    // EditorGUIUtility.labelWidth = originLabelWidth;

                }
                // EditorGUIUtility.labelWidth = originLabelWidth;
            }
        }

        DrawUILine(colUILine, 3, 12);

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;

        var showEditorLabel = "Hide";
        if (!_showImportOptions)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Import Options";
        }

        _showImportOptions = EditorGUILayout.Foldout(_showImportOptions, showEditorLabel, hiderStyle);


        if (_showImportOptions)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Party Templates  "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            _cultures = EditorGUILayout.Toggle("Cultures", _cultures);
            _equip = EditorGUILayout.Toggle("Equipments", _equip);
            _factions = EditorGUILayout.Toggle("Factions", _factions);
            _heroes = EditorGUILayout.Toggle("Heroes", _heroes);
            _items = EditorGUILayout.Toggle("Items", _items);
            _kingdoms = EditorGUILayout.Toggle("Kingdoms", _kingdoms);
            _NPC = EditorGUILayout.Toggle("NPCCharacters", _NPC);
            _pt = EditorGUILayout.Toggle("Party Templates", _pt);
            _settlement = EditorGUILayout.Toggle("Settlements", _settlement);

        }
        EditorGUIUtility.labelWidth = originLabelWidth;
        DrawUILine(colUILine, 3, 12);

        // Debug.Log("Index on GUI" + index);
        if (GUILayout.Button(doString))
        {
            ImportData();
            // AssetDatabase.Refresh();
        }
    }

    void ImportData()
    {
        if(_alreadyImported)
        {
            if (EditorUtility.DisplayDialog($"Current module {modList[index].id} is already imported", "You can reimport it, but: \n" +
                                           "Warning!!! All old data related to this mod will be deleted.",
                                           "Yes, Just remove all and reimport", "Cancel"))
            {
                CleanModData(modList[index].id);
            }
            else
            {
                return;
            }
        }

        ModuleReceiver asset = CreateInstance<ModuleReceiver>();
        asset = modList[index];

        string headModulePath = "Assets/Resources/SubModulesData/";
        string headModuleResourcesPath = "Assets/Resources/Data/";

        if (!Directory.Exists(headModulePath))
        {
            Directory.CreateDirectory(headModulePath);
        }

        if (!Directory.Exists(headModuleResourcesPath))
        {
            Directory.CreateDirectory(headModuleResourcesPath);
        }

        //var mod_asset_path = headModulePath + asset.id + ".asset";
        //if (!File.Exists(mod_asset_path))
        AssetDatabase.CreateAsset(asset, headModulePath + asset.id + ".asset");
        // AssetDatabase.SaveAssets();

        DataWindow.currentMod = asset;
        DataWindow.source = asset;

        var mfg = new ModFilesManager(asset, true, ref _cultures, ref _kingdoms, ref _factions, ref _heroes, ref _NPC, ref _items, ref _equip, ref _pt, ref _settlement);

        foreach (string dependency in modList[index].modDependenciesInternal)
        {

            if (dependency != "Native")
            {
                ModuleReceiver DPDasset = CreateInstance<ModuleReceiver>();

                foreach (var mod in modList)
                {

                    if (mod.id == dependency && isSelectedDPD[mod.id] == true)
                    {
                        DPDasset = mod;

                        string dpdModPath = headModulePath;
                        string dpdModResourcesPath = headModuleResourcesPath;

                        if (!Directory.Exists(dpdModPath))
                        {
                            Directory.CreateDirectory(dpdModPath);
                        }

                        if (!Directory.Exists(dpdModResourcesPath))
                        {
                            Directory.CreateDirectory(dpdModResourcesPath);
                        }

                        AssetDatabase.CreateAsset(DPDasset, dpdModPath + DPDasset.id + ".asset");
                        //  AssetDatabase.SaveAssets();
                        // DataWindow.currentMod = DPDasset;
                        // DataWindow.source = DPDasset;

                        ModFilesManager dpdFileManager = new ModFilesManager(DPDasset, true, ref _cultures, ref _kingdoms, ref _factions, ref _heroes, ref _NPC, ref _items, ref _equip, ref _pt, ref _settlement);
                    }

                }
            }
        }

        this.Close();

        EditorUtility.LoadWindowLayout("Assets/Settings/BDT_UnityLayout.wlt");
    }

    static void CheckVersion(string v1, string v2, ref bool isGreater)
    {
        // // string v1 = "1.23.56.1487";
        // // string v2 = "1.24.55.487";

        // string version1 = new string(v1);
        // string version2 = new string(v2);

        var result = v1.CompareTo(v2);

        if (result > 0)
        {
            // Debug.Log("Importing V is greater");
            isGreater = true;
        }
        else if (result < 0)
        {
            // Debug.Log("Existing V is greater");
        }
        else
        {
            // Debug.Log("Versions are equal");
        }
        return;

    }

    private void CleanModData(string modID)
    {
        string dataPath = $"Assets/Resources/Data/{modID}";
        string sub_dataPath = $"Assets/Resources/SubModulesData/{modID}";
        string sub_dataPathConfigMeta = $"Assets/Resources/SubModulesData/{modID}.asset.meta";
        string mod_asset = $"Assets/Resources/SubModulesData/{modID}.asset";

        if (File.Exists(mod_asset))
            File.Delete(mod_asset);

        if (Directory.Exists(dataPath))
            Directory.Delete(dataPath, true);
        if (Directory.Exists(sub_dataPath))
            Directory.Delete(sub_dataPath, true);

        if (File.Exists(dataPath + ".meta"))
            File.Delete(dataPath + ".meta");
        if (File.Exists(sub_dataPath + ".meta"))
            File.Delete(sub_dataPath + ".meta");
        if (File.Exists(sub_dataPathConfigMeta))
            File.Delete(sub_dataPathConfigMeta);

        //AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
}