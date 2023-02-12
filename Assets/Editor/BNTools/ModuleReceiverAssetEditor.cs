using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModuleReceiver))]
public class ModuleReceiverAssetEditor : Editor
{
    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";

    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";

    ModuleReceiver module;

    string[] moduleCategory_options = new string[] { "Singleplayer", "Multiplayer" };
    int moduleCategory_index;
    string[] dependencies_options;
    int dependencies_index;

    bool showDependenciesEditor;
    bool showSubModulesEditor;

    List<string> existingModules;
    List<string> dirNames;
    string[] dirs;

    float originLabelWidth = EditorGUIUtility.labelWidth;
    Vector2 textDimensions;
    GUIStyle titleStyle;

    bool styleSetup = false;
    public void OnEnable()
    {
        module = (ModuleReceiver)target;
        EditorUtility.SetDirty(module);

        if (module.Dependencies == null)
            module.Dependencies = new List<ModuleReceiver.Dependency>();
        if (module.SubModules == null)
            module.SubModules = new List<ModuleReceiver.SubModule>();

        ModuleCreatorEditor.GetModuleDirectoryData(ref dirs, ref existingModules, ref dirNames);

        dependencies_options = new string[existingModules.Count];
        for (int i = 0; i < existingModules.Count; i++)
        {
            dependencies_options[i] = existingModules[i];
        }

        module.UpdateInternalDependencies();
        RefreshDependenciesOptions();

    }

    public override void OnInspectorGUI()
    {
        if (!styleSetup)
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 16;

            Color newCol;
            ColorUtility.TryParseHtmlString("#fbb034", out newCol);
            titleStyle.normal.textColor = newCol;
            styleSetup = true;
        }

        EditorGUILayout.LabelField("SubModule Settings", EditorStyles.boldLabel);
        DrawUILine(colUILine, 3, 12);

        EditorGUILayout.LabelField("Module ID: ");
        EditorGUI.BeginDisabledGroup(true);
        module.id = EditorGUILayout.TextField(module.id);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Module Name: ");
        module.moduleName = EditorGUILayout.TextField(module.moduleName);

        EditorGUILayout.LabelField("Module Version: ");
        module.version = EditorGUILayout.TextField(module.version);

        DrawUILine(colUILine, 1, 4);

        GUILabelLenght("Default Module: ");
        module.defaultModule = EditorGUILayout.Toggle("Default Module: ", module.defaultModule);
        GUILabelLenghtEnd();

        DrawUILine(colUILine, 1, 4);

        EditorGUILayout.LabelField("Module Category: ", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        moduleCategory_index = EditorGUILayout.Popup(moduleCategory_index, moduleCategory_options, GUILayout.Width(128));
        module.moduleCategory = moduleCategory_options[moduleCategory_index];
        DrawUILine(colUILine, 3, 12);

        DrawDependenciesEditor();
        DrawUILine(colUILine, 3, 12);
        DrawSubModulesEditor();
        DrawUILine(colUILine, 3, 12);

    }

    private void DrawSubModulesEditor()
    {
        Vector2 textDimensions;
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.hover.textColor = Color.green;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;

        Color newCol;
        ColorUtility.TryParseHtmlString("#f7931e", out newCol);
        Color newCol2;
        ColorUtility.TryParseHtmlString("#fa9f1e", out newCol2);
        titleStyle.normal.textColor = newCol;

        GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);
        hiderStyle.fontSize = 10;
        hiderStyle.normal.textColor = newCol;

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("SubModules: "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        var showEditorLabel = "Hide";
        if (!showSubModulesEditor)
        {
            hiderStyle.fontSize = 16;
            showEditorLabel = "SubModules Editor";
        }

        showSubModulesEditor = EditorGUILayout.Foldout(showSubModulesEditor, showEditorLabel, hiderStyle);

        if (showSubModulesEditor)
        {

            EditorGUILayout.LabelField("SubModules Editor", titleStyle, GUILayout.ExpandWidth(true));

            DrawUILine(colUILine, 1, 6);

            if (GUILayout.Button((new GUIContent("Add SubModule", "Add SubModule Data for this Module")), buttonStyle, GUILayout.Width(128)))
            {
                var subMod = new ModuleReceiver.SubModule("NewSubModule", "", "", new List<ModuleReceiver.SubModuleTag>(), false);
                module.SubModules.Add(subMod);
            }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            if (module.SubModules != null && module.SubModules.Count > 0)
            {
                titleStyle.fontSize = 13;
                titleStyle.normal.textColor = newCol2;

                for (int i = 0; i < module.SubModules.Count; i++)
                {
                    DrawUILine(colUILine, 3, 12);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(module.SubModules[i].Name, titleStyle, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button((new GUIContent("X", "Remove SubModule Data")), buttonStyle, GUILayout.Width(32), GUILayout.ExpandWidth(false)))
                    {
                        module.SubModules.Remove(module.SubModules[i]);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Name:");
                    var nm = EditorGUILayout.TextField(module.SubModules[i].Name, GUILayout.Width(128));

                    DrawUILine(colUILine, 1, 4);


                    EditorGUILayout.LabelField("DLL Name:");
                    var dll = EditorGUILayout.TextField(module.SubModules[i].DLLName, GUILayout.Width(128));

                    EditorGUILayout.Space(4);

                    if (!string.IsNullOrWhiteSpace(dll) && new FileInfo(dll).Extension != ".dll")
                    {
                        EditorGUILayout.HelpBox($"Add a *.dll extension at the end", MessageType.Info);
                    }

                    DrawUILine(colUILine, 1, 4);

                    EditorGUILayout.LabelField("SubModule Class Type:");
                    var subModClass = EditorGUILayout.TextField(module.SubModules[i].SubModuleClassType, GUILayout.Width(256));

                    var tagsNew = new List<ModuleReceiver.SubModuleTag>();
                    DrawUILine(colUILine, 1, 4);

                    var showTagEditor = module.SubModules[i].ShowTags;

                    if (GUILayout.Button((new GUIContent("Add Tag", "Add tag Data for this SubModule")), buttonStyle, GUILayout.Width(128)))
                    {
                        tagsNew.Add(new ModuleReceiver.SubModuleTag("", ""));
                        showTagEditor = true;
                    }
                    DrawUILine(colUILine, 1, 4);

                    if (!showTagEditor)
                    {
                        hiderStyle.fontSize = 12;
                        showEditorLabel = "SubModule Tags";
                    }
                    else
                    {
                        showEditorLabel = "Hide tags";
                        hiderStyle.fontSize = 10;
                    }

                    showTagEditor = EditorGUILayout.Foldout(showTagEditor, showEditorLabel, hiderStyle);

                    if (showTagEditor)
                    {
                        if (module.SubModules[i].Tags.Count > 0)
                            for (int tagID = 0; tagID < module.SubModules[i].Tags.Count; tagID++)
                            {
                                DrawUILine(colUILine, 1, 3);

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($"Tag {tagID + 1} |", titleStyle, GUILayout.Width(52));

                                EditorGUILayout.LabelField("Key:", GUILayout.Width(32));
                                var key = EditorGUILayout.TextField(module.SubModules[i].Tags.ElementAt(tagID).Key, GUILayout.Width(132));

                                EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                                var val = EditorGUILayout.TextField(module.SubModules[i].Tags.ElementAt(tagID).Value, GUILayout.Width(80));

                                if (GUILayout.Button((new GUIContent("X", "Remove tag")), buttonStyle, GUILayout.Width(32), GUILayout.ExpandWidth(false)))
                                {
                                    module.SubModules[i].Tags.Remove(module.SubModules[i].Tags.ElementAt(tagID));
                                    return;
                                }

                                EditorGUILayout.EndHorizontal();

                                tagsNew.Add(new ModuleReceiver.SubModuleTag(key, val));
                            }
                        else
                            EditorGUILayout.HelpBox($"No tags in this SubModule Data", MessageType.Info);
                    }

                    if (!showTagEditor)
                    {
                        module.SubModules[i] = new ModuleReceiver.SubModule(nm, dll, subModClass, module.SubModules[i].Tags, showTagEditor);
                    }
                    else
                    {
                        module.SubModules[i] = new ModuleReceiver.SubModule(nm, dll, subModClass, tagsNew, showTagEditor);
                    }
                }
            }
        }
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

            if (module.Dependencies.Count < existingModules.Count - 1)
            {
                DrawUILine(colUILine, 3, 12);

                //EditorGUILayout.BeginHorizontal();
                dependencies_index = EditorGUILayout.Popup(dependencies_index, dependencies_options, GUILayout.Width(180));

                DrawUILine(colUILine, 1, 6);

                if (GUILayout.Button((new GUIContent("Add Dependency", "Add selected Dependency for this Module")), buttonStyle, GUILayout.Width(128)))
                {
                    var dpd = new ModuleReceiver.Dependency(dependencies_options[dependencies_index], ModuleCreatorEditor.GetModuleVersion(dependencies_options[dependencies_index]), false);
                    module.Dependencies.Add(dpd);
                    module.UpdateInternalDependencies();
                    RefreshDependenciesOptions();
                }

                //EditorGUILayout.EndHorizontal();
                //EditorGUILayout.Space(4);
            }

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.hover.textColor = Color.red;

            if (module.Dependencies != null && module.Dependencies.Count > 0)
            {
                titleStyle.fontSize = 13;
                titleStyle.normal.textColor = newCol2;

                for (int i = 0; i < module.Dependencies.Count; i++)
                {
                    DrawUILine(colUILine, 3, 12);
                    EditorGUILayout.LabelField(module.Dependencies[i].DependedModule, titleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.LabelField("Version:");
                    var vers = EditorGUILayout.TextField(module.Dependencies[i].DependentVersion, GUILayout.Width(128));
                    module.Dependencies[i] = new ModuleReceiver.Dependency(module.Dependencies[i].DependedModule, vers, module.Dependencies[i].Optional);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Optional", GUILayout.Width(48));
                    var opt = EditorGUILayout.Toggle(module.Dependencies[i].Optional, GUILayout.Width(42));
                    module.Dependencies[i] = new ModuleReceiver.Dependency(module.Dependencies[i].DependedModule, module.Dependencies[i].DependentVersion, opt);

                    if (GUILayout.Button((new GUIContent("X", "Remove Dependency")), buttonStyle, GUILayout.Width(32)))
                    {
                        module.Dependencies.Remove(module.Dependencies[i]);
                        module.UpdateInternalDependencies();
                        RefreshDependenciesOptions();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                }

            }
        }
    }

    private void RefreshDependenciesOptions()
    {
        List<string> options = new List<string>();
        for (int i_exist = 0; i_exist < existingModules.Count; i_exist++)
        {
            if (!ModuleReceiver.Dependency.ContainsID(existingModules[i_exist], module.Dependencies) && existingModules[i_exist] != module.id)
                options.Add(existingModules[i_exist]);
        }
        dependencies_options = options.ToArray();
        dependencies_index = 0;
    }


    private void GUILabelLenght(string stringLenght)
    {
        textDimensions = GUI.skin.label.CalcSize(new GUIContent(stringLenght));
        EditorGUIUtility.labelWidth = textDimensions.x;
    }

    private void GUILabelLenghtEnd()
    {
        EditorGUIUtility.labelWidth = originLabelWidth;
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
