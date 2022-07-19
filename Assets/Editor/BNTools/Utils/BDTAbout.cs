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

public class BDTAbout : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    GUIStyle headerLabelStyle;
    Color newColLabel;

    bool settingRefresh = false;
    [MenuItem("BNDataTools/About")]
    static void OnEnable()
    {
        var window = GetWindow<BDTAbout>("About");
        window.position = new Rect(512, 256, 280, 236);
        window.Show();


    }
    void OnGUI()
    {
        if (!settingRefresh)
        {
            settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));
            EditorUtility.SetDirty(settingsAsset);

            headerLabelStyle = new GUIStyle(EditorStyles.helpBox);
            ColorUtility.TryParseHtmlString("#fbb034", out newColLabel);

            headerLabelStyle.normal.textColor = newColLabel;
            headerLabelStyle.fontSize = 22;
            headerLabelStyle.fontStyle = FontStyle.Bold;
            settingRefresh = true;
        }

        DrawUILine(colUILine, 1, 4);
        EditorGUILayout.LabelField("Bannerlord Data Tools", headerLabelStyle);
        // GUILayout.Space(2);
        DrawUILine(colUILine, 1, 4);
        EditorGUILayout.HelpBox($"Bannerlord open source modding tool {Environment.NewLine} " +
            $"Version: {settingsAsset.BDTVersion} {Environment.NewLine} " +
            $"Bannerlord compatibility: {settingsAsset.BannerlordVersionCompatibility} {Environment.NewLine}{Environment.NewLine}" +
            $"Tool Creators: {Environment.NewLine}" +
            $"Developer - Mark7 {Environment.NewLine}{Environment.NewLine}" +
            $"Credits: {Environment.NewLine}" +
            $"Resources & Game - TaleWorlds Entertainment{Environment.NewLine}" +
            $"SDF Generator - Weesals{Environment.NewLine}"
            , MessageType.None);
        DrawUILine(colUILine, 1, 4);

    }

    void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }


}

