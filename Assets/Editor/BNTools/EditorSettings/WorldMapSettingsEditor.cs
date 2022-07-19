using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using UnityEngine.EventSystems;

public class WorldMapSettingsEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    BDTSettings settingsAsset;
    ModuleReceiver currMod;
    Texture2D tex2D;
    Texture2D tex2D_H;
    bool init;

    int w_x;
    int w_y;
    int w_tileSize;
    float w_max_H;
    float w_min_H;


    static void Init()
    {
        var window = GetWindow<WorldMapSettingsEditor>("World Map Settings");
        window.position = new Rect(0, 0, 1024, 1024);
        window.Show();


    }

    void OnGUI()
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

            if (currMod.load_xscene)
            {
                var xScene_file = $"{settingsAsset.BNModulesPath}{currMod.id}/SceneObj/{currMod.world_map_xscene_id}/scene.xscene";

                if (File.Exists(xScene_file))
                    ReadXscene(xScene_file);
            }
        }

        if (currMod != null)
        {
            var originLabelWidth = EditorGUIUtility.labelWidth;

            var titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 16;

            EditorGUILayout.Space(4);

            if (File.Exists(currMod.W_Map_Texture))
            {
                tex2D = (Texture2D)AssetDatabase.LoadAssetAtPath(currMod.W_Map_Texture, typeof(Texture2D));
            }

            if (File.Exists(currMod.W_HeightMap_Texture))
            {
                tex2D_H = (Texture2D)AssetDatabase.LoadAssetAtPath(currMod.W_HeightMap_Texture, typeof(Texture2D));
            }

            EditorGUILayout.LabelField("World Map Properties", titleStyle);

            DrawUILine(colUILine, 3, 12);

            EditorGUILayout.LabelField("  World Map Texture", EditorStyles.boldLabel);
            EditorGUILayout.Space(-20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            tex2D = TextureField("", tex2D);

            if (EditorGUI.EndChangeCheck())
            {
                currMod.W_Map_Texture = AssetDatabase.GetAssetPath(tex2D);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawUILine(colUILine, 2, 6);

            EditorGUILayout.LabelField("  HeightMap Texture", EditorStyles.boldLabel);
            EditorGUILayout.Space(-20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            tex2D_H = TextureField("", tex2D_H);

            if (EditorGUI.EndChangeCheck())
            {
                currMod.W_HeightMap_Texture = AssetDatabase.GetAssetPath(tex2D_H);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawUILine(colUILine, 2, 6);

            currMod.world_map_xscene_id = EditorGUILayout.TextField("  Main Map name:", currMod.world_map_xscene_id);

            DrawUILine(colUILine, 2, 6);

            var xScene_file = $"{settingsAsset.BNModulesPath}{currMod.id}/SceneObj/{currMod.world_map_xscene_id}/scene.xscene";

            if (File.Exists(xScene_file))
            {
                EditorGUI.BeginChangeCheck();

                DrawUILine(colUILine, 2, 6);
                EditorGUILayout.LabelField("  Load Values from .xscene ", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                currMod.load_xscene = EditorGUILayout.Toggle(currMod.load_xscene);
                GUILayout.EndHorizontal();

                DrawUILine(colUILine, 2, 6);

                if (EditorGUI.EndChangeCheck())
                {
                    if (currMod.load_xscene == true)
                    {
                        ReadXscene(xScene_file);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(currMod.world_map_xscene_id+"/scene.xscene not exist in " + currMod.id + " Folder. \n You need main map scene to load terain data from module.", MessageType.Warning);
            }

            DrawUILine(colUILine, 3, 12);

            EditorGUI.BeginDisabledGroup(currMod.load_xscene);
            EditorGUILayout.LabelField("  Terrain Properties", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("  X "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            GUILayout.BeginHorizontal();
            currMod.W_X_Size = EditorGUILayout.IntField("  X", currMod.W_X_Size, GUILayout.Width(64));
            currMod.W_Y_Size = EditorGUILayout.IntField("  Y", currMod.W_Y_Size, GUILayout.Width(64));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            textDimensions = GUI.skin.label.CalcSize(new GUIContent("  Single Node Size "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            currMod.W_SingleNodeSize = EditorGUILayout.IntField("  Single Node Size", currMod.W_SingleNodeSize, GUILayout.Width(160));
            EditorGUIUtility.labelWidth = originLabelWidth;

            DrawUILine(colUILine, 2, 6);

            EditorGUILayout.LabelField("  Height Properties", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            textDimensions = GUI.skin.label.CalcSize(new GUIContent("  max "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            GUILayout.BeginHorizontal();
            currMod.W_max_Height = EditorGUILayout.FloatField("  max", currMod.W_max_Height, GUILayout.Width(128));
            currMod.W_min_Height = EditorGUILayout.FloatField("  min", currMod.W_min_Height, GUILayout.Width(128));
            GUILayout.EndHorizontal();

            DrawUILine(colUILine, 2, 6);
            EditorGUI.EndDisabledGroup();

        }

    }

    private void ReadXscene(string Xscene)
    {
        XmlDocument Doc = new XmlDocument();
        // UTF 8 - 16
        StreamReader reader = new StreamReader(Xscene);

        Doc.Load(reader);

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;

        foreach (XmlNode node in Root.ChildNodes)
        {
            if (node.LocalName == "terrain")
            {
                if (node.Attributes["node_dimension_x"] != null)
                {
                    currMod.W_X_Size = int.Parse(node.Attributes["node_dimension_x"].Value);
                }
                if (node.Attributes["node_dimension_y"] != null)
                {
                    currMod.W_Y_Size = int.Parse(node.Attributes["node_dimension_y"].Value);

                }
                if (node.Attributes["node_size"] != null)
                {
                    currMod.W_SingleNodeSize = (int)float.Parse(node.Attributes["node_size"].Value);

                }
                if (node.Attributes["min_height"] != null)
                {
                    currMod.W_min_Height = float.Parse(node.Attributes["min_height"].Value);

                }
                if (node.Attributes["max_height"] != null)
                {
                    currMod.W_max_Height = float.Parse(node.Attributes["max_height"].Value);

                }
            }
        }
        reader.Close();
    }

    // UI DRAW TOOLS

    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
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
