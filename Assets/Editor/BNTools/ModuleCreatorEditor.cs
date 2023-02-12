using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using XNode;
using System.Data;
using System.Windows;

[Serializable]
public class ModuleCreatorEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settings;
    List<string> existingModules;
    List<string> dirNames;
    string[] dirs;

    public BNDataEditorWindow DataWindow;

    string[] dependencies_options;
    int dependencies_index;
    string[] moduleCategory_options = new string[] { "Singleplayer", "Multiplayer" };
    int moduleCategory_index;

    bool showDependenciesEditor;
    Vector2 scrollPos;




    string modID;
    string modName;
    string modV = "v0.0.1";
    bool defaultModule = true;
    List<ModuleReceiver.Dependency> _dependencies;

    GUIStyle titleStyle;
    public ModuleCreatorEditor(BNDataEditorWindow BNEditor)
    {
        var window = (ModuleCreatorEditor)GetWindow(typeof(ModuleCreatorEditor), true, "Module Creator");

        if (window.position.x == 0 && window.position.y == 0)
            window.position = new Rect(window.position.x, window.position.y, 260, 700);

        DataWindow = BNEditor;
        window.Show();
    }

    private void OnEnable()
    {
        settings = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));
        EditorUtility.SetDirty(settings);

        GetModuleDirectoryData(ref dirs, ref existingModules, ref dirNames);

        _dependencies = new List<ModuleReceiver.Dependency>();

        dependencies_options = new string[existingModules.Count];
        for (int i = 0; i < existingModules.Count; i++)
        {
            dependencies_options[i] = existingModules[i];
        }

        titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        Color newCol;
        ColorUtility.TryParseHtmlString("#fbb034", out newCol);
        titleStyle.normal.textColor = newCol;
    }

    public static void GetModuleDirectoryData(ref string[] Directories, ref List<string> ExistingModules, ref List<string> DirectoryNames)
    {
        string cfgPath = "Assets/Settings/BDT_settings.asset";
        var settings_asset = (BDTSettings)AssetDatabase.LoadAssetAtPath(cfgPath, typeof(BDTSettings));
        Directories = Directory.GetDirectories(settings_asset.BNModulesPath);

        //Debug.Log(settings_asset.BNModulesPath);

        ExistingModules = new List<string>();
        DirectoryNames = new List<string>();

        for (int i = 0; i < Directories.Length; i++)
        {
            var dirName = Directories[i].Replace(Path.GetDirectoryName(Directories[i]) + "/", "");

            if (File.Exists(Directories[i] + "/SubModule.xml"))
            {
                ExistingModules.Add(dirName);

            }

            DirectoryNames.Add(dirName);
        }
    }

    void OnGUI()
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Module Creator", titleStyle);

        //DrawUILine(colUILine, 1, 4);

        //if (string.IsNullOrWhiteSpace(modID))
        //    EditorGUILayout.HelpBox($"Enter the new module ID.", MessageType.Warning);

        DrawUILine(colUILine, 1, 6);

        EditorGUILayout.LabelField("ID:", EditorStyles.boldLabel);
        modID = EditorGUILayout.TextField(modID);

        EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
        modName = EditorGUILayout.TextField(modName);

        EditorGUILayout.LabelField("Version:", EditorStyles.boldLabel);
        modV = EditorGUILayout.TextField(modV);

        if (!Char.IsLetter(modV[0]))
            EditorGUILayout.HelpBox($"Your version need to start with a letter, to avoid errors at launcher loading.", MessageType.Error);

        DrawUILine(colUILine, 1, 4);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Default Module:", EditorStyles.boldLabel, GUILayout.Width(100));
        defaultModule = EditorGUILayout.Toggle(defaultModule);
        EditorGUILayout.EndHorizontal();
        DrawUILine(colUILine, 1, 4);

        EditorGUILayout.LabelField("Module Category:", EditorStyles.boldLabel);
        moduleCategory_index = EditorGUILayout.Popup(moduleCategory_index, moduleCategory_options, GUILayout.Width(128));
        DrawUILine(colUILine, 3, 12);

        DrawDependenciesEditor();
        DrawUILine(colUILine, 3, 12);

        if (!string.IsNullOrWhiteSpace(modID) && !dirNames.Contains(modID))
        {
            if (GUILayout.Button("Create Module"))
            {
                CreateSubModuleDirecotries();
                CreateSubModuleXML();
                CreateSubModuleBDTData();
                this.Close();
            }
        }
        else if (!string.IsNullOrWhiteSpace(modID))
            EditorGUILayout.HelpBox($"This directory name ({modID}) \nalready exist in Modules directory.", MessageType.Error);

    }

    private void DrawDependenciesEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        Color newCol;
        ColorUtility.TryParseHtmlString("#84bd00", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#8ec06c", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        var originDimensions = EditorGUIUtility.labelWidth;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Dependencies: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showDependenciesEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "Dependencies Editor";
        }

        showDependenciesEditor = EditorGUILayout.Foldout(showDependenciesEditor, showEditorLabel, hiderStyle);

        if (showDependenciesEditor)
        {

            EditorGUILayout.LabelField("Dependencies Editor", titleStyle, GUILayout.ExpandWidth(true));

            if (_dependencies.Count < existingModules.Count)
            {
                DrawUILine(colUILine, 3, 12);
                //EditorGUILayout.BeginHorizontal();
                dependencies_index = EditorGUILayout.Popup("", dependencies_index, dependencies_options, GUILayout.Width(180));

                DrawUILine(colUILine, 1, 6);

                if (GUILayout.Button((new GUIContent("Add Dependency", "Add selected Dependency for this Module")), buttonStyle, GUILayout.Width(128)))
                {
                    var dpd = new ModuleReceiver.Dependency(dependencies_options[dependencies_index], GetModuleVersion(dependencies_options[dependencies_index]), false);
                    _dependencies.Add(dpd);
                    RefreshDependenciesOptions();
                }

                //EditorGUILayout.EndHorizontal();
                //EditorGUILayout.Space(4);
            }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            if (_dependencies != null && _dependencies.Count > 0)
            {
                titleStyle.fontSize = 13;
                titleStyle.normal.textColor = newCol2;

                var scrollPosW = 100 * _dependencies.Count;
                if (scrollPosW > 256)
                    scrollPosW = 256;

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(260), GUILayout.Height(scrollPosW));

                for (int i = 0; i < _dependencies.Count; i++)
                {
                    DrawUILine(colUILine, 3, 12);
                    EditorGUILayout.LabelField(_dependencies[i].DependedModule, titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.LabelField("Version:");
                    var vers = EditorGUILayout.TextField(_dependencies[i].DependentVersion, GUILayout.Width(128));
                    _dependencies[i] = new ModuleReceiver.Dependency(_dependencies[i].DependedModule, vers, _dependencies[i].Optional);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Optional", GUILayout.Width(48));
                    var opt = EditorGUILayout.Toggle(_dependencies[i].Optional, GUILayout.Width(42));
                    _dependencies[i] = new ModuleReceiver.Dependency(_dependencies[i].DependedModule, _dependencies[i].DependentVersion, opt);

                    if (GUILayout.Button((new GUIContent("X", "Remove Dependency")), buttonStyle, GUILayout.Width(32)))
                    {
                        _dependencies.Remove(_dependencies[i]);
                        RefreshDependenciesOptions();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndScrollView();

            }
        }
    }

    private void RefreshDependenciesOptions()
    {
        List<string> options = new List<string>();
        for (int i_exist = 0; i_exist < existingModules.Count; i_exist++)
        {
            if (!ModuleReceiver.Dependency.ContainsID(existingModules[i_exist], _dependencies))
                options.Add(existingModules[i_exist]);
        }
        dependencies_options = options.ToArray();
        dependencies_index = 0;
    }

    private void CreateSubModuleBDTData()
    {
        string modulePath = settings.BNModulesPath + modID;
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

        ModuleReceiver asset = CreateInstance<ModuleReceiver>();
        asset.id = modID;
        asset.moduleName = modName;
        asset.version = modV;
        asset.path = modulePath;
        asset.modDependenciesInternal = new string[_dependencies.Count];

        for (int i = 0; i < _dependencies.Count; i++)
        {
            asset.modDependenciesInternal[i] = _dependencies[i].DependedModule;
        }

        AssetDatabase.CreateAsset(asset, headModulePath + asset.id + ".asset");

        bool import = false;
        var mfm = new ModFilesManager(asset, false, ref import, ref import, ref import, ref import, ref import, ref import, ref import, ref import, ref import);
        //mfm.IsCreationProcces = true;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        DataWindow.currentMod = asset;
        DataWindow.source = asset;

        EditorUtility.ClearProgressBar();
    }
    private void CreateSubModuleDirecotries()
    {
        var moduleDirectory = settings.BNModulesPath + modID + "/";
        var bin = Directory.CreateDirectory(moduleDirectory + "bin");

        Directory.CreateDirectory(bin + "/Win64_Shipping_Client");
        Directory.CreateDirectory(bin + "/Win64_Shipping_wEditor");

        Directory.CreateDirectory(moduleDirectory + "Assets");
        Directory.CreateDirectory(moduleDirectory + "AssetSources");
        Directory.CreateDirectory(moduleDirectory + "ModuleData");
        Directory.CreateDirectory(moduleDirectory + "NavMeshPrefabs");
        Directory.CreateDirectory(moduleDirectory + "Prefabs");
        Directory.CreateDirectory(moduleDirectory + "SceneObj");
        Directory.CreateDirectory(moduleDirectory + "GUI");
    }
    private void CreateSubModuleXML()
    {
        var moduleDirectory = settings.BNModulesPath + modID;
        Directory.CreateDirectory(moduleDirectory);

        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmlDeclaration, root);

        XmlElement modNode = doc.CreateElement(string.Empty, "Module", string.Empty);
        doc.AppendChild(modNode);

        XmlElement nameNode = doc.CreateElement(string.Empty, "Name", string.Empty);
        if (string.IsNullOrWhiteSpace(modName))
            modName = modID;
        nameNode.SetAttribute("value", modName);
        modNode.AppendChild(nameNode);

        XmlElement idNode = doc.CreateElement(string.Empty, "Id", string.Empty);
        idNode.SetAttribute("value", modID);
        modNode.AppendChild(idNode);

        XmlElement versionNode = doc.CreateElement(string.Empty, "Version", string.Empty);
        versionNode.SetAttribute("value", modV);
        modNode.AppendChild(versionNode);

        XmlElement defaultModuleNode = doc.CreateElement(string.Empty, "DefaultModule", string.Empty);
        defaultModuleNode.SetAttribute("value", defaultModule.ToString().ToLower());
        modNode.AppendChild(defaultModuleNode);

        XmlElement ModuleCategoryNode = doc.CreateElement(string.Empty, "ModuleCategory", string.Empty);
        ModuleCategoryNode.SetAttribute("value", moduleCategory_options[moduleCategory_index]);
        modNode.AppendChild(ModuleCategoryNode);

        XmlElement officialNode = doc.CreateElement(string.Empty, "Official", string.Empty);
        officialNode.SetAttribute("value", "false");
        modNode.AppendChild(officialNode);

        XmlElement dependedModulesNode = doc.CreateElement(string.Empty, "DependedModules", string.Empty);
        modNode.AppendChild(dependedModulesNode);

        foreach (var dpd in _dependencies)
        {
            XmlElement dependency = doc.CreateElement(string.Empty, "DependedModule", string.Empty);
            dependency.SetAttribute("Id", dpd.DependedModule);
            dependency.SetAttribute("DependentVersion", dpd.DependentVersion);
            dependency.SetAttribute("Optional", dpd.Optional.ToString().ToLower());
            dependedModulesNode.AppendChild(dependency);
        }

        XmlElement SubModulesNode = doc.CreateElement(string.Empty, "SubModules", string.Empty);
        modNode.AppendChild(SubModulesNode);

        XmlElement xmls = doc.CreateElement(string.Empty, "Xmls", string.Empty);
        modNode.AppendChild(xmls);

        doc.Save($"{moduleDirectory}/SubModule.xml");
    }
    // UI DRAW TOOLS

    public static string GetModuleVersion(string modName)
    {
        string cfgPath = "Assets/Settings/BDT_settings.asset";
        var settings_asset = (BDTSettings)AssetDatabase.LoadAssetAtPath(cfgPath, typeof(BDTSettings));
        var path = settings_asset.BNModulesPath + modName + "/SubModule.xml";
        string version = "0.0.0.0";

        XmlDocument Doc = new XmlDocument();
        StreamReader reader = new StreamReader(path);

        Doc.Load(reader);

        XmlElement Root = Doc.DocumentElement;

        foreach (XmlNode node in Root.ChildNodes)
        {
            if (node.Name == "Version")
            {
                version = node.Attributes["value"].Value;
            }
        }
        reader.Close();

        return version;
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
