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
using System.Reflection;

public class BannerEditor : EditorWindow
{

    public string bannerKey;

    // string bannerKeyRef = "10.148.148.2000.2000.764.764.1.0.0.106.0.116.525.700.764.714.0.0.0.344.0.116.250.250.1164.739.0.0.0.344.0.116.250.250.364.739.0.1.0.523.42.116.50.50.764.519.0.1.0.529.42.116.50.50.769.499.0.0.0.510.0.116.1896.100.746.1164.0.0.0.510.0.116.1896.100.769.364.0.0.0.510.42.116.1896.100.756.1144.0.0.0.510.42.116.1896.100.787.384.0.0.0.522.116.116.50.50.709.939.0.0.0.510.116.116.50.50.829.939.0.0.90.528.116.116.50.50.774.939.0.0.0.143.116.116.200.200.764.239.0.1.0.510.0.116.1896.100.764.1124.0.0.0.510.0.116.1896.100.764.404.0.0.0.143.116.116.200.200.764.1289.0.0.0";
    // string bannerKeyLayout = "10.148.148.2000.2000.764.764.1.0.0.510.0.116.1800.30.764.123.0.0.0.510.0.116.1800.20.764.1092.0.0.0.510.0.116.1800.20.764.368.0.0.0.510.0.116.1800.30.764.1337.0.0.0.510.0.116.1800.30.1500.764.0.0.90.510.0.116.1800.30.36.764.0.0.90.510.0.116.1800.20.1064.764.0.0.90.510.0.116.1800.20.464.764.0.0.90";
    // string bannerKeyLayout_old = "10.148.148.2000.2000.764.764.1.0.0.510.0.116.1800.30.764.224.0.0.0.510.0.116.1800.20.764.1132.0.0.0.510.0.116.1800.20.764.396.0.0.0.510.0.116.1800.30.764.1312.0.0.0.510.0.116.1800.30.1463.764.0.0.90.510.0.116.1800.30.73.764.0.0.90.510.0.116.1800.20.1064.764.0.0.90.510.0.116.1800.20.464.764.0.0.90";
    string nullBanner = "11.148.0.2000.2000.764.764.1.0.0";

    public Kingdom inputKingdom;
    public Faction inputFaction;
    public Culture inputCulture;
    BannerEditorSettings bannerSettings;
    Color32 colUILine = new Color32(82, 86, 94, 255);

    string pivotTexture = "Assets/Settings/BannersEditor/Resources/pivot.tga";
    public Vector2 scrollPosition = Vector2.zero;
    public Vector2 bannerKeyScrollPoss = Vector2.zero;
    public Vector2 pickerScrollPos = Vector2.zero;
    public Vector2 scrollPositionZero = Vector2.zero;

    bool readed;
    public string[] image_cat_options;
    public int image_cat_index = 0;
    public bool show_image_selector;
    public bool show_color_selector;
    Dictionary<int, Texture2D> imgSelector;

    // bool LayerUpdate = true;

    List<BannerLayer> layerList;
    // Current Layer
    public BannerLayer selectedLayer;

    bool buttonPressed;
    // string draggingID;
    public Vector2 selectedLayerPivot;
    GUIStyle stlTitle = new GUIStyle(EditorStyles.boldLabel);
    public bool sel_color_A;
    public bool sel_color_B;
    public bool sel_stroke;
    public bool sel_mirror;
    public float sel_pos_X;
    public float sel_pos_Y;
    public float sel_scale_height;
    public float sel_scale_width;
    public float sel_rotation;

    // OPTIONS

    public bool show_guideLines;
    public int guideLinesHeight = 2;
    public Color gdCL = Color.red;


    [MenuItem("Examples/Banner Editor")]
    static void Init()
    {
        var window = GetWindow<BannerEditor>("Banner Editor");
        window.position = new Rect(0, 0, 1024, 1024);
        window.Show();
    }

    public void Setup()
    {
        if (inputKingdom != null)
            EditorUtility.SetDirty(inputKingdom);
        if (inputFaction != null)
            EditorUtility.SetDirty(inputFaction);
        if (inputCulture != null)
            EditorUtility.SetDirty(inputCulture);

        image_cat_options = new string[6];
        image_cat_options[0] = "Backgrounds";
        image_cat_options[1] = "Animals";
        image_cat_options[2] = "Flora";
        image_cat_options[3] = "Handmade";
        image_cat_options[4] = "Sign";
        image_cat_options[5] = "Shape";

        if (bannerKey == "")
            bannerKey = nullBanner;

    }
    void OnGUI()
    {
        if (!readed)
        {
            Setup();
            ReadBannerKey();
        }
        else
        {
            // EditorGUI.BeginChangeCheck();
            // check = EditorGUILayout.Toggle(check, GUILayout.ExpandWidth(false));

            // if (EditorGUI.EndChangeCheck())
            // {
            //     LayerApplyColors();
            // }

            Color bgRectcolor_b;
            ColorUtility.TryParseHtmlString("#282828", out bgRectcolor_b);

            var wievRect = new Rect(320, 32, 1280, 768);
            var originLabelWidth = EditorGUIUtility.labelWidth;
            Vector2 textDimensions;

            DrawLayersOnCanvas(wievRect);

            EditorGUIUtility.labelWidth = originLabelWidth;

            DrawGuides(wievRect);

            Color bgCol;
            ColorUtility.TryParseHtmlString("#383838", out bgCol);

            DrawWindowRectsBG(wievRect, bgCol);

            DrawControlGizmos(bgCol);

            GUILayout.Space(16);

            DrawLayerControlSettings(bgRectcolor_b, ref wievRect);

            textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Show Guide Lines "));
            EditorGUIUtility.labelWidth = textDimensions.x;

            DrawViewOptions(bgRectcolor_b, wievRect);

            var layerManager = new Rect(wievRect.x - 320, 32, 320, 768);
            EditorGUI.DrawRect(layerManager, bgRectcolor_b);

            // var originColor = GUI.color;
            // var style = new GUIStyle(GUI.skin.button);

            // GUILayout.Button("", style, GUILayout.Width(64), GUILayout.Height(64));

            GUILayout.Space(-980);

            DrawSelectors(bgRectcolor_b, wievRect);

            DrawLayersList(bgRectcolor_b, wievRect);

            DrawLayerOptions();

            // TODO - cleaned console (not important gui errors)
            //Debug.ClearDeveloperConsole();
            ClearLog();
        }
    }

   
    private void DrawLayerOptions()
    {
        var layerOptionsRect = new Rect(14, 32, 160, 768);
        EditorGUI.DrawRect(layerOptionsRect, new Color32(70, 70, 70, 255));

        var initYPos = 140;
        if (GUI.Button(new Rect(32, initYPos, 124, 32), "Add new Layer  ✚"))
        {
            BannerLayer newBL = (BannerLayer)ScriptableObject.CreateInstance(typeof(BannerLayer));

            newBL.color_A = "0";
            newBL.color_B = "5";

            newBL.pos_X = "740";
            newBL.pos_Y = "768";

            newBL.stroke = true;

            newBL.rotation = "0";

            newBL.scale_width = "256";
            newBL.scale_height = "256";
            newBL.sprite_ID = "347";

            newBL.color_A_hex = bannerSettings.colors[0];
            newBL.color_B_hex = bannerSettings.colors[5];

            for (int i = 0; i < bannerSettings.sprite_IDs.Length; i++)
            {
                if (bannerSettings.sprite_IDs[i] == newBL.sprite_ID)
                {
                    Texture2D texture = Instantiate((Texture2D)AssetDatabase.LoadAssetAtPath(bannerSettings.sprites[i], typeof(Texture2D))) as Texture2D;
                    newBL.textureOriginal = texture;
                    newBL.textureWork = texture;

                }
            }

            layerList.Add(newBL);

            selectedLayer = layerList[layerList.Count - 1];

            RepaintSelectedLayer();

            WriteLayersToKey();


        }

        initYPos = initYPos + 48;

        if (GUI.Button(new Rect(32, initYPos, 124, 32), "Move to Top       ∆"))
        {

            var tempList = new List<BannerLayer>();
            BannerLayer bgLayer = layerList[0];

            for (int i = 1; i < layerList.Count; i++)
            {
                if (layerList[i] != selectedLayer)
                {
                    tempList.Add(layerList[i]);
                }
            }

            layerList = new List<BannerLayer>();

            layerList.Add(bgLayer);
            layerList.Add(selectedLayer);

            foreach (var layer in tempList)
            {
                layerList.Add(layer);
            }

            selectedLayer = layerList[1];

            RepaintSelectedLayer();
        }

        if (GUI.Button(new Rect(32, initYPos + 32, 124, 32), "Move to Bottom ∇"))
        {
            var tempList = new List<BannerLayer>();
            BannerLayer bgLayer = layerList[0];

            for (int i = 1; i < layerList.Count; i++)
            {
                if (layerList[i] != selectedLayer)
                {
                    tempList.Add(layerList[i]);
                }
            }

            layerList = new List<BannerLayer>();

            layerList.Add(bgLayer);

            foreach (var layer in tempList)
            {
                layerList.Add(layer);
            }

            layerList.Add(selectedLayer);

            selectedLayer = layerList[layerList.Count - 1];

            RepaintSelectedLayer();
            WriteLayersToKey();

        }

        if (GUI.Button(new Rect(32, initYPos + 80, 124, 32), "Move up      ↑"))
        {
            if (selectedLayer != layerList[1])
            {
                var tempList = new List<BannerLayer>();
                var tempListB = new List<BannerLayer>();
                int orderID = 0;
                BannerLayer bgLayer = layerList[0];

                for (int i = 1; i < layerList.Count; i++)
                {
                    if (layerList[i] != selectedLayer)
                    {
                        tempList.Add(layerList[i]);
                    }
                    else
                    {
                        orderID = i;
                        break;
                    }
                }

                for (int i = orderID + 1; i < layerList.Count; i++)
                {
                    tempListB.Add(layerList[i]);
                }

                layerList = new List<BannerLayer>();

                layerList.Add(bgLayer);

                for (int i = 0; i < tempList.Count - 1; i++)
                {
                    layerList.Add(tempList[i]);
                }

                layerList.Add(selectedLayer);
                layerList.Add(tempList[tempList.Count - 1]);

                for (int i = 0; i < tempListB.Count; i++)
                {
                    layerList.Add(tempListB[i]);
                }

                selectedLayer = layerList[orderID - 1];

                RepaintSelectedLayer();
                WriteLayersToKey();

            }
        }


        if (GUI.Button(new Rect(32, initYPos + 112, 124, 32), "Move down ↓"))
        {
            if (selectedLayer != layerList[layerList.Count - 1])
            {
                var tempList = new List<BannerLayer>();
                var tempListB = new List<BannerLayer>();
                int orderID = 0;
                BannerLayer bgLayer = layerList[0];

                for (int i = 1; i < layerList.Count; i++)
                {
                    if (layerList[i] != selectedLayer)
                    {
                        tempList.Add(layerList[i]);
                    }
                    else
                    {
                        orderID = i;
                        break;
                    }
                }

                for (int i = orderID + 1; i < layerList.Count; i++)
                {
                    tempListB.Add(layerList[i]);
                }

                layerList = new List<BannerLayer>();

                layerList.Add(bgLayer);

                for (int i = 0; i < tempList.Count; i++)
                {
                    layerList.Add(tempList[i]);
                }

                layerList.Add(tempListB[0]);
                layerList.Add(selectedLayer);

                for (int i = 1; i < tempListB.Count; i++)
                {
                    layerList.Add(tempListB[i]);
                }

                selectedLayer = layerList[orderID + 1];

                RepaintSelectedLayer();
                WriteLayersToKey();

            }
        }

        if (GUI.Button(new Rect(32, initYPos + 160, 124, 32), "Duplicate  ❐"))
        {
            if (selectedLayer != layerList[0])
            {
                int orderID = 0;
                var tempList = new List<BannerLayer>();

                for (int i = 0; i < layerList.Count; i++)
                {
                    tempList.Add(layerList[i]);
                    if (layerList[i] == selectedLayer)
                    {
                        orderID = i;
                        BannerLayer dublicate = Instantiate(selectedLayer);
                        tempList.Add(dublicate);
                    }
                }

                layerList = new List<BannerLayer>();

                for (int i = 0; i < tempList.Count; i++)
                {
                    layerList.Add(tempList[i]);
                }

                selectedLayer = layerList[orderID + 1];

                RepaintSelectedLayer();
                WriteLayersToKey();

            }
        }

        // GUI.Button(new Rect(32, initYPos + 208, 60, 32), "↩ Undo");
        // GUI.Button(new Rect(96, initYPos + 208, 60, 32), "Redo ↪");

        if (GUI.Button(new Rect(32, initYPos + 256, 124, 32), "Remove Layer   〤"))
        {
            if (selectedLayer != layerList[0])
            {
                var tempList = new List<BannerLayer>();
                var orderID = 0;
                for (int i = 0; i < layerList.Count; i++)
                {
                    if (layerList[i] != selectedLayer)
                    {
                        tempList.Add(layerList[i]);
                    }
                    else
                    {
                        orderID = i;
                    }
                }

                layerList = new List<BannerLayer>();

                for (int i = 0; i < tempList.Count; i++)
                {
                    layerList.Add(tempList[i]);
                }

                selectedLayer = layerList[orderID - 1];

                RepaintSelectedLayer();
                WriteLayersToKey();

            }
        }
    }

    private void DrawLayersList(Color bgRectcolor_b, Rect wievRect)
    {
        GUILayout.BeginVertical();
        var layersBrowserRect = new Rect(wievRect.x - 92, 64, 320, 720);
        var layersBrowserRectBG = new Rect(wievRect.x - 128, 32, 264, 768);

        EditorGUI.DrawRect(layersBrowserRectBG, new Color32(70, 70, 70, 255));
        EditorGUI.DrawRect(layersBrowserRect, bgRectcolor_b);
        // GUI.BeginGroup(layersBrowserRect);

        scrollPosition = GUI.BeginScrollView(layersBrowserRect, scrollPosition, new Rect(0, 0, 248, (layerList.Count * 78)));

        if (show_color_selector)
        {
            GUILayout.Space(-1188);
        }
        else if (show_image_selector)
        {
            if (image_cat_index == 0)
            {
                if (selectedLayer == layerList[0])
                {
                    GUILayout.Space(396);
                }
                else
                {
                    GUILayout.Space(858);
                }
            }
            if (image_cat_index == 2)
                GUILayout.Space(594);
            if (image_cat_index == 3)
                GUILayout.Space(264);
            if (image_cat_index == 4)
                GUILayout.Space(66);
            if (image_cat_index == 5)
                GUILayout.Space(396);
        }
        else
        {
            GUILayout.Space(858);
        }

        GUILayout.BeginHorizontal();

        if (show_color_selector)
        {
            GUILayout.Space(-201);
        }
        else if (show_image_selector)
        {
            if (selectedLayer != layerList[0] && image_cat_index == 0)
            {
                GUILayout.Space(16);
            }

            if (image_cat_index == 2)
                GUILayout.Space(-268);
            if (image_cat_index == 3)
                GUILayout.Space(-201);
            if (image_cat_index == 4)
                GUILayout.Space(-268);
            if (image_cat_index == 5)
                GUILayout.Space(-67);
        }
        else
        {
            GUILayout.Space(16);
        }

        GUILayout.BeginVertical();


        GUILayout.Space(-92);
        EditorGUILayout.LabelField("Banner Layers", stlTitle);
        GUILayout.Space(4);
        DrawUILine(colUILine, 2, 8);

        for (int i = 0; i < layerList.Count; i++)
        {
            GUILayout.BeginHorizontal();

            var originColor = GUI.color;
            var style = new GUIStyle(GUI.skin.button);

            if (layerList[i] == selectedLayer)
            {
                Color curSelCol;
                ColorUtility.TryParseHtmlString("#f48924", out curSelCol);
                GUI.color = curSelCol * 3;
            }

            if (GUILayout.Button(layerList[i].textureWork, style, GUILayout.Width(64), GUILayout.Height(64)))
            {
                selectedLayer = layerList[i];
            }
            GUI.color = originColor;

            if (i == 0)
            {
                EditorGUILayout.LabelField("Background Layer", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField($"Layer {i}", EditorStyles.boldLabel);
            }
            GUILayout.EndHorizontal();
            DrawUILine(colUILine, 1, 4);

        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();


        GUI.EndScrollView();

        GUILayout.EndVertical();

        var bnKeyRect = new Rect(wievRect.x * 1.91f, wievRect.y * 22.56f, 710, 64);
        var bnKeyRectHeader = new Rect(wievRect.x * 1.91f, wievRect.y * 21.8f, 100, 24);
        var bnKeyRectHeaderCopy = new Rect(bnKeyRectHeader.x + 100, bnKeyRectHeader.y, 32, 24);
        EditorGUI.DrawRect(bnKeyRectHeader, Color.gray);

        GUI.Label(bnKeyRectHeader, " Banner Key", stlTitle);

        if (GUI.Button(bnKeyRectHeaderCopy, "C"))
        {
            GUIUtility.systemCopyBuffer = bannerKey;
        }

        var keyScrollRect = new Rect(bnKeyRect.x, bnKeyRect.y, bnKeyRect.width - 16, bannerKey.Length / 4);

        bannerKeyScrollPoss = GUI.BeginScrollView(bnKeyRect, bannerKeyScrollPoss, keyScrollRect);

        GUI.TextArea(keyScrollRect, bannerKey);
        GUI.EndScrollView();

    }

    private void DrawSelectors(Color bgRectcolor_b, Rect wievRect)
    {
        var orgColor = GUI.color;

        if (!show_image_selector && !show_color_selector)
        {
            // PICKER
            var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
            EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
        }

        if (show_image_selector)
        {
            switch (image_cat_index)
            {
                case 0:

                    if (selectedLayer == layerList[0])
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, -1, 100, "Backgrounds", true);
                    }
                    else
                    {
                        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
                        EditorGUI.HelpBox(new Rect(wievRect.x * 4.91f, 108, 320, 40), "Select Background category, after preview BG category.", MessageType.Error);
                    }
                    break;
                case 1:
                    if (selectedLayer != layerList[0])
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 99, 200, "Animals", true);
                    }
                    else
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 99, 200, "Animals", false);
                        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
                        EditorGUI.HelpBox(new Rect(wievRect.x * 4.91f, 108, 320, 40), "Select Background category.", MessageType.Error);
                    }
                    break;
                case 2:
                    if (selectedLayer != layerList[0])
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 199, 300, "Flora", true);
                    }
                    else
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 199, 300, "Flora", false);
                        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
                        EditorGUI.HelpBox(new Rect(wievRect.x * 4.91f, 108, 320, 40), "Select Background category.", MessageType.Error);
                    }
                    break;
                case 3:
                    if (selectedLayer != layerList[0])
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 299, 400, "Handmade", true);
                    }
                    else
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 299, 400, "Handmade", false);
                        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
                        EditorGUI.HelpBox(new Rect(wievRect.x * 4.91f, 108, 320, 40), "Select Background category.", MessageType.Error);
                    }
                    break;
                case 4:
                    if (selectedLayer != layerList[0])
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 399, 500, "Sign", true);
                    }
                    else
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 399, 500, "Sign", false);
                        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
                        EditorGUI.HelpBox(new Rect(wievRect.x * 4.91f, 108, 320, 40), "Select Background category.", MessageType.Error);
                    }
                    break;
                case 5:
                    if (selectedLayer != layerList[0])
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 499, 600, "Shape", true);
                    }
                    else
                    {
                        DrawImageSelector(bgRectcolor_b, wievRect, 499, 600, "Shape", false);
                        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
                        EditorGUI.HelpBox(new Rect(wievRect.x * 4.91f, 108, 320, 40), "Select Background category.", MessageType.Error);
                    }
                    break;
            }
        }

        if (show_color_selector)
        {
            DrawColorSelector(bgRectcolor_b, wievRect);
        }

        GUI.color = orgColor;
        var selectoreHeaderRect = new Rect(wievRect.x * 4.82f, 32, 372, 60);
        EditorGUI.DrawRect(selectoreHeaderRect, bgRectcolor_b);
        GUI.BeginGroup(selectoreHeaderRect);

        if (show_color_selector)
        {
            Color curSelCol;
            ColorUtility.TryParseHtmlString("#f48924", out curSelCol);
            GUI.color = curSelCol * 3;
        }

        if (GUI.Button(new Rect(32, 16, 92, 32), "Color"))
        {

            if (!show_color_selector)
            {
                show_image_selector = false;
                show_color_selector = true;
            }
            else
            {
                show_color_selector = false;
            }

        }

        GUI.color = orgColor;

        if (show_image_selector)
        {
            Color curSelCol;
            ColorUtility.TryParseHtmlString("#f48924", out curSelCol);
            GUI.color = curSelCol * 3;
        }

        if (GUI.Button(new Rect(92 + 48, 16, 92, 32), "Images"))
        {
            if (!show_image_selector)
            {
                show_color_selector = false;
                show_image_selector = true;
            }
            else
            {
                show_image_selector = false;
                var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
                EditorGUI.DrawRect(pickerRect, bgRectcolor_b);
            }
        }
        GUI.color = orgColor;

        if (show_image_selector)
        {
            EditorGUI.LabelField(new Rect(248, 4, 92, 32), "Category:");
            image_cat_index = EditorGUI.Popup(new Rect(248, 30, 100, 32), image_cat_index, image_cat_options);
        }

        GUI.EndGroup();

        GUI.color = orgColor;
        // return wievRect;
    }

    private void DrawViewOptions(Color bgRectcolor_b, Rect wievRect)
    {
        var optionsHeight = 180;
        if (!show_guideLines)
        {
            optionsHeight = 80;
        }

        // OPTIONS
        var optionsRect = new Rect(wievRect.x * 4.25f, 416, 160, optionsHeight);
        EditorGUI.DrawRect(optionsRect, bgRectcolor_b);
        GUI.BeginGroup(optionsRect);

        GUILayout.BeginHorizontal();
        GUILayout.Space(16);
        GUILayout.BeginVertical();
        GUILayout.Space(-208);
        stlTitle = new GUIStyle(EditorStyles.boldLabel);
        stlTitle.fontSize = 16;
        EditorGUILayout.LabelField("View Options", stlTitle);

        DrawUILine(colUILine, 1, 4);

        show_guideLines = EditorGUILayout.Toggle("Show Guide Lines", show_guideLines);

        if (show_guideLines)
        {
            DrawUILine(colUILine, 1, 2);

            EditorGUILayout.LabelField("Guide lines Height", EditorStyles.boldLabel);
            GUILayout.Space(4);

            guideLinesHeight = EditorGUILayout.IntSlider(guideLinesHeight, 1, 4, GUILayout.MaxWidth(128));

            EditorGUILayout.LabelField("Guide lines Color", EditorStyles.boldLabel);
            GUILayout.Space(4);
            gdCL = EditorGUILayout.ColorField(gdCL, GUILayout.MaxWidth(128));


        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        // GUILayout.Space(-160);

        GUI.EndGroup();
        // return wievRect;
    }

    private void DrawLayerControlSettings(Color bgRectcolor_b, ref Rect wievRect)
    {
        Vector2 textDimensions;
        var selecteLayerOPTRect = new Rect(wievRect.x * 4.25f, 128, 160, 256);
        EditorGUI.DrawRect(selecteLayerOPTRect, bgRectcolor_b);
        GUI.BeginGroup(selecteLayerOPTRect);

        GUILayout.BeginHorizontal();
        GUILayout.Space(16);
        GUILayout.BeginVertical();

        stlTitle = new GUIStyle(EditorStyles.boldLabel);
        stlTitle.fontSize = 16;
        EditorGUILayout.LabelField("Selected Layer", stlTitle);

        GUILayout.Space(4);


        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Stroke"));
        EditorGUIUtility.labelWidth = textDimensions.x;

        DrawUILine(colUILine, 1, 4);

        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();

        selectedLayer.stroke = EditorGUILayout.Toggle("Stroke", selectedLayer.stroke, GUILayout.Width(64));

        DrawUILineVertical(colUILine, 1, 1, 15);

        selectedLayer.mirror = EditorGUILayout.Toggle(" Mirror", selectedLayer.mirror, GUILayout.Width(64));
        GUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            RepaintSelectedLayer();
            WriteLayersToKey();

        }

        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Rotation"));
        EditorGUIUtility.labelWidth = textDimensions.x;

        DrawUILine(colUILine, 1, 4);

        var orgColor = GUI.color;
        Color colorTemp;

        ColorUtility.TryParseHtmlString(selectedLayer.color_A_hex, out colorTemp);
        GUI.color = colorTemp * 3;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(24)))
        {
            sel_color_A = true;
            sel_color_B = false;
            show_image_selector = false;
            show_color_selector = true;
        }
        GUILayout.Space(4);
        if (sel_color_A)
        {
            GUILayout.Toggle(sel_color_A, "Selected");
        }
        else
        {
            GUILayout.Toggle(sel_color_A, "");
        }
        GUILayout.EndHorizontal();

        GUI.color = orgColor;

        GUILayout.Space(2);

        ColorUtility.TryParseHtmlString(selectedLayer.color_B_hex, out colorTemp);
        GUI.color = colorTemp * 3;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(24)))
        {
            sel_color_A = false;
            sel_color_B = true;
            show_image_selector = false;
            show_color_selector = true;

        }
        GUILayout.Space(4);
        if (sel_color_B)
        {
            GUILayout.Toggle(sel_color_B, "Selected");
        }
        else
        {
            GUILayout.Toggle(sel_color_B, "");
        }
        GUILayout.EndHorizontal();

        GUI.color = orgColor;

        // sel_color_A = EditorGUILayout.ColorField("Color A", sel_color_A, GUILayout.Width(128));
        // sel_color_B = EditorGUILayout.ColorField("Color B", sel_color_B, GUILayout.Width(128));

        DrawUILine(colUILine, 1, 4);

        // Draw Controllers
        EditorGUI.BeginChangeCheck();

        sel_pos_X = float.Parse(selectedLayer.pos_X);
        sel_pos_X = EditorGUILayout.FloatField("Pos X", sel_pos_X, GUILayout.Width(128));

        sel_pos_Y = float.Parse(selectedLayer.pos_Y);
        sel_pos_Y = EditorGUILayout.FloatField("Pos Y", sel_pos_Y, GUILayout.Width(128));

        sel_rotation = float.Parse(selectedLayer.rotation);
        sel_rotation = EditorGUILayout.FloatField("Rotation", sel_rotation, GUILayout.Width(128));

        sel_scale_width = float.Parse(selectedLayer.scale_width);
        sel_scale_width = EditorGUILayout.FloatField("Width", sel_scale_width, GUILayout.Width(128));

        sel_scale_height = float.Parse(selectedLayer.scale_height);
        sel_scale_height = EditorGUILayout.FloatField("Height", sel_scale_height, GUILayout.Width(128));


        if (EditorGUI.EndChangeCheck())
        {
            selectedLayer.pos_X = ((int)sel_pos_X).ToString();

            selectedLayer.pos_Y = ((int)sel_pos_Y).ToString();

            selectedLayer.rotation = ((int)sel_rotation).ToString();

            selectedLayer.scale_width = ((int)sel_scale_width).ToString();

            selectedLayer.scale_height = ((int)sel_scale_height).ToString();


            WriteLayersToKey();
        }


        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUI.EndGroup();
    }

    private void WriteLayersToKey()
    {
        string newKey = "";
        for (int i = 0; i < layerList.Count; i++)
        {
            var lyr = layerList[i];
            var dt = ".";
            var mrr = "";
            var strk = "";

            if (lyr.stroke == true)
            {
                strk = "1";
            }
            else
            {
                strk = "0";
            }

            if (lyr.mirror == true)
            {
                mrr = "1";
            }
            else
            {
                mrr = "0";
            }

            if (i != layerList.Count - 1)
            {
                newKey = newKey + lyr.sprite_ID + dt + lyr.color_B + dt + lyr.color_A + dt + lyr.scale_width + dt + lyr.scale_height + dt + lyr.pos_X + dt + lyr.pos_Y + dt + strk + dt + mrr + dt + lyr.rotation + dt;

            }
            else
            {
                newKey = newKey + lyr.sprite_ID + dt + lyr.color_B + dt + lyr.color_A + dt + lyr.scale_width + dt + lyr.scale_height + dt + lyr.pos_X + dt + lyr.pos_Y + dt + strk + dt + mrr + dt + lyr.rotation;
            }

        }
        bannerKey = newKey;

        if (inputKingdom != null)
        {
            inputKingdom.banner_key = bannerKey;
        }
        else if (inputFaction != null)
        {
            inputFaction.banner_key = bannerKey;
        }
        else
        {
            inputCulture.faction_banner_key = bannerKey;
        }
    }

    private void DrawControlGizmos(Color bgCol)
    {
        Color guiColor;
        //PIVOT
        // selectedLayerPivot = pivotPoint;
        var tex = (Texture2D)AssetDatabase.LoadAssetAtPath(pivotTexture, typeof(Texture2D));
        // EditorGUI.DrawTextureTransparent(new Rect(selectedLayerPivot.x, selectedLayerPivot.y, 16, 16), tex);

        guiColor = GUI.color;
        Color pvtCOl;
        ColorUtility.TryParseHtmlString("#00a1f1", out pvtCOl);
        GUI.color = pvtCOl * 3;

        var pivotRect = new Rect(selectedLayerPivot.x - 7, selectedLayerPivot.y - 7, 14, 14);
        EditorGUI.DrawRect(pivotRect, bgCol);

        if (pivotRect.Contains(Event.current.mousePosition))
        {

            if (Event.current.type == EventType.MouseDown)
            {
                buttonPressed = true;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                buttonPressed = false;
            }

        }

        if (buttonPressed && Event.current.type == EventType.MouseDrag)
        {

            selectedLayer.pos_X = (float.Parse(selectedLayer.pos_X) + (Event.current.delta.x * 2)).ToString();
            selectedLayer.pos_Y = (float.Parse(selectedLayer.pos_Y) + (Event.current.delta.y * 2)).ToString();

            WriteLayersToKey();
        }

        GUI.Button(new Rect(selectedLayerPivot.x - 5, selectedLayerPivot.y - 5, 10, 10), tex);

        GUI.color = guiColor;
        // return guiColor;
    }

    private static void DrawWindowRectsBG(Rect wievRect, Color bgCol)
    {
        EditorGUI.DrawRect(new Rect(wievRect.x, -wievRect.y * 23, wievRect.width, wievRect.height), bgCol);
        EditorGUI.DrawRect(new Rect(wievRect.x, wievRect.y * 25, wievRect.width, wievRect.height), bgCol);

        EditorGUI.DrawRect(new Rect(wievRect.x * 5, wievRect.y, wievRect.width, wievRect.height), bgCol);
        EditorGUI.DrawRect(new Rect(-wievRect.x * 3, wievRect.y, wievRect.width, wievRect.height), bgCol);


        // ! DRAW MAKET
        var editorZonePath = "Assets/Settings/BannersEditor/Resources/editorMaket.png";
        var editorZoneTex = (Texture2D)AssetDatabase.LoadAssetAtPath(editorZonePath, typeof(Texture2D));


        Color guiColor = GUI.color;
        GUI.color = Color.clear;
        EditorGUI.DrawTextureTransparent(wievRect, editorZoneTex);

        GUI.color = guiColor;
        // return wievRect;
    }

    private void DrawGuides(Rect wievRect)
    {
        if (show_guideLines)
        {
            // draw vertical guidelines
            EditorGUI.DrawRect(new Rect(wievRect.x + 488 - (guideLinesHeight / 2), 0, guideLinesHeight, 2048), gdCL);
            EditorGUI.DrawRect(new Rect(wievRect.x + 788 - (guideLinesHeight / 2), 0, guideLinesHeight, 2048), gdCL);

            // draw horizontal guidelines
            EditorGUI.DrawRect(new Rect(0, wievRect.y + 168 - (guideLinesHeight / 2), 2048, guideLinesHeight), gdCL);
            EditorGUI.DrawRect(new Rect(0, wievRect.y + 530 - (guideLinesHeight / 2), 2048, guideLinesHeight), gdCL);
        }


    }

    private void DrawLayersOnCanvas(Rect wievRect)
    {
        scrollPositionZero = GUI.BeginScrollView(wievRect, scrollPosition, wievRect);

        for (int i = 0; i < layerList.Count; i++)
        {
            var pivotPoint2 = new Vector2(wievRect.center.x, wievRect.center.y);
            var multiplier = 2;
            var width = int.Parse(layerList[i].scale_width);
            if (layerList[i].mirror)
            {
                width = -width;
            }
            var x_com = int.Parse(layerList[i].pos_X) - width / 2;
            var y_com = int.Parse(layerList[i].pos_Y) - (int.Parse(layerList[i].scale_height) / 2);

            Color tempGUICol = GUI.color;
            GUI.color = Color.clear;

            if (layerList[i].textureWork != null)
            {
                var oldMatrixRotate = GUI.matrix;
                Rect layerRect = new Rect(x_com / multiplier + (1280 / 2) - 64, (y_com + 32) / multiplier, width / multiplier, int.Parse(layerList[i].scale_height) / multiplier);
                var pivotPoint = new Vector2(layerRect.center.x, layerRect.center.y);

                GUIUtility.RotateAroundPivot(-int.Parse(layerList[i].rotation), pivotPoint);
                EditorGUI.DrawTextureTransparent(layerRect, layerList[i].textureWork);

                if (layerList[i] == selectedLayer)
                {
                    selectedLayerPivot = pivotPoint;
                }
                GUI.matrix = oldMatrixRotate;

            }
            // this.Repaint();

            GUI.color = tempGUICol;


        }

        GUI.EndScrollView();
    }

    private void DrawImageSelector(Color bgRectcolor_b, Rect wievRect, int min, int max, string category, bool draw)
    {
        // PICKER
        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);

        imgSelector = new Dictionary<int, Texture2D>();

        for (int i = 0; i < bannerSettings.sprite_IDs.Length; i++)
        {
            var imgID = bannerSettings.sprite_IDs[i];
            var val = int.Parse(imgID);
            if (val > min && val < max)
            {
                var img = bannerSettings.sprites[i];
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(img, typeof(Texture2D));
                imgSelector.Add(val, tex);
            }
        }

        pickerScrollPos = GUI.BeginScrollView(pickerRect, pickerScrollPos, new Rect(0, -832 + 64, 358, ((imgSelector.Count * 64) / 5) + 64));


        int loops = (imgSelector.Count / 5) + 1;
        int img_index = 0;
        for (int i = 0; i < loops; i++)
        {
            // Debug.Log(img_index);

            GUILayout.BeginHorizontal();
            GUILayout.Space(16);


            if (imgSelector.ToArray().Length <= img_index)
            {
                break;
            }

            Texture2D tex = imgSelector.ToArray()[img_index].Value;
            if (GUILayout.Button(tex, GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (draw)
                {
                    imgSelector.ToArray()[img_index].Key.ToString();
                    selectedLayer.textureOriginal = tex;
                    selectedLayer.textureWork = tex;
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
            }

            if (imgSelector.ToArray().Length <= img_index + 1)
            {
                break;
            }

            tex = imgSelector.ToArray()[img_index + 1].Value;
            if (GUILayout.Button(tex, GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (draw)
                {
                    selectedLayer.sprite_ID = (img_index + 1).ToString();
                    selectedLayer.textureOriginal = tex;
                    selectedLayer.textureWork = tex;
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
            }

            if (imgSelector.ToArray().Length <= img_index + 2)
            {
                break;
            }


            tex = imgSelector.ToArray()[img_index + 2].Value;
            if (GUILayout.Button(tex, GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (draw)
                {
                    selectedLayer.sprite_ID = imgSelector.ToArray()[img_index + 2].Key.ToString();

                    selectedLayer.textureOriginal = tex;
                    selectedLayer.textureWork = tex;
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
            }

            if (imgSelector.ToArray().Length <= img_index + 3)
            {
                break;
            }

            tex = imgSelector.ToArray()[img_index + 3].Value;
            if (GUILayout.Button(tex, GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (draw)
                {
                    selectedLayer.sprite_ID = imgSelector.ToArray()[img_index + 3].Key.ToString();

                    selectedLayer.textureOriginal = tex;
                    selectedLayer.textureWork = tex;
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
            }

            if (imgSelector.ToArray().Length <= img_index + 4)
            {
                break;
            }

            tex = imgSelector.ToArray()[img_index + 4].Value;
            if (GUILayout.Button(tex, GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (draw)
                {
                    imgSelector.ToArray()[img_index + 4].Key.ToString();

                    selectedLayer.textureOriginal = tex;
                    selectedLayer.textureWork = tex;
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }

            }


            GUILayout.EndHorizontal();
            img_index = img_index + 5;
        }

        GUI.EndScrollView();
    }
    private void DrawColorSelector(Color bgRectcolor_b, Rect wievRect)
    {
        // PICKER
        var pickerRect = new Rect(wievRect.x * 4.82f, 92, 372, 708);
        EditorGUI.DrawRect(pickerRect, bgRectcolor_b);

        // GUI.BeginGroup(pickerRect);
        pickerScrollPos = GUI.BeginScrollView(pickerRect, pickerScrollPos, new Rect(0, -832 + 64, 358, ((bannerSettings.color_IDs.Length * 64) / 5) + 128));

        int loops = (bannerSettings.color_IDs.Length / 5) + 1;
        int col_index = 0;
        for (int col_i = 0; col_i < loops; col_i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(16);
            var orgColor = GUI.color;


            Color curSelCol;
            ColorUtility.TryParseHtmlString(bannerSettings.colors[col_index], out curSelCol);

            GUI.color = curSelCol * 3;
            if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (sel_color_A)
                {
                    selectedLayer.color_A = bannerSettings.color_IDs[col_index];
                    selectedLayer.color_A_hex = bannerSettings.colors[col_index];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
                else if (sel_color_B)
                {
                    selectedLayer.color_B = bannerSettings.color_IDs[col_index];
                    selectedLayer.color_B_hex = bannerSettings.colors[col_index];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }

            }



            Color curSelCol_1;
            ColorUtility.TryParseHtmlString(bannerSettings.colors[col_index + 1], out curSelCol_1);

            GUI.color = curSelCol_1 * 3;
            if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (sel_color_A)
                {
                    selectedLayer.color_A = bannerSettings.color_IDs[col_index + 1];
                    selectedLayer.color_A_hex = bannerSettings.colors[col_index + 1];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
                else if (sel_color_B)
                {
                    selectedLayer.color_B = bannerSettings.color_IDs[col_index + 1];
                    selectedLayer.color_B_hex = bannerSettings.colors[col_index + 1];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }

            }



            Color curSelCol_2;
            ColorUtility.TryParseHtmlString(bannerSettings.colors[col_index + 2], out curSelCol_2);

            GUI.color = curSelCol_2 * 3;
            if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
            {

                if (sel_color_A)
                {
                    selectedLayer.color_A = bannerSettings.color_IDs[col_index + 2];
                    selectedLayer.color_A_hex = bannerSettings.colors[col_index + 2];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
                else if (sel_color_B)
                {
                    selectedLayer.color_B = bannerSettings.color_IDs[col_index + 2];
                    selectedLayer.color_B_hex = bannerSettings.colors[col_index + 2];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
            }

            if (col_index + 2 == 157)
            {
                break;
            }

            Color curSelCol_3;
            ColorUtility.TryParseHtmlString(bannerSettings.colors[col_index + 3], out curSelCol_3);

            GUI.color = curSelCol_3 * 3;
            if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (sel_color_A)
                {
                    selectedLayer.color_A = bannerSettings.color_IDs[col_index + 3];
                    selectedLayer.color_A_hex = bannerSettings.colors[col_index + 3];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
                else if (sel_color_B)
                {
                    selectedLayer.color_B = bannerSettings.color_IDs[col_index + 3];
                    selectedLayer.color_B_hex = bannerSettings.colors[col_index + 3];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
            }



            Color curSelCol_4;
            ColorUtility.TryParseHtmlString(bannerSettings.colors[col_index + 4], out curSelCol_4);

            GUI.color = curSelCol_4 * 3;
            if (GUILayout.Button("", GUILayout.Width(64), GUILayout.Height(64)))
            {
                if (sel_color_A)
                {
                    selectedLayer.color_A = bannerSettings.color_IDs[col_index + 4];
                    selectedLayer.color_A_hex = bannerSettings.colors[col_index + 4];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }
                else if (sel_color_B)
                {
                    selectedLayer.color_B = bannerSettings.color_IDs[col_index + 4];
                    selectedLayer.color_B_hex = bannerSettings.colors[col_index + 4];
                    RepaintSelectedLayer();
                    WriteLayersToKey();

                }

            }
            GUI.color = orgColor;


            GUILayout.EndHorizontal();
            col_index = col_index + 5;
            // col_i = col_index + 4;
        }
        GUI.EndScrollView();
        // GUI.EndGroup();
    }

    private void LayerApplyColors()
    {
        // Debug.Log("Check");

        for (int i = 0; i < layerList.Count; i++)
        {
            if (layerList[i].textureWork != null && layerList[i].textureOriginal != null)
            {
                Texture2D texture = layerList[i].textureWork;
                Texture2D texCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);

                // RGBA32 texture format data layout exactly matches Color32 struct
                var dataOriginal = layerList[i].textureOriginal.GetRawTextureData<Color32>();
                var data = texCopy.GetRawTextureData<Color32>();

                // fill texture data with a simple pattern
                Color col_A = new Color32(255, 165, 0, 255);
                Color col_B = new Color32(0, 128, 128, 255);

                var col_ID = 0;
                foreach (var colorID in bannerSettings.color_IDs)
                {

                    if (colorID == layerList[i].color_B)
                    {
                        ColorUtility.TryParseHtmlString(bannerSettings.colors[col_ID], out col_B);
                        layerList[i].color_B_hex = bannerSettings.colors[col_ID];
                    }

                    if (layerList[i] != layerList[0])
                    {

                        if (layerList[i].stroke == true)
                        {
                            if (colorID == layerList[i].color_A)
                            {
                                ColorUtility.TryParseHtmlString(bannerSettings.colors[col_ID], out col_A);
                                layerList[i].color_A_hex = bannerSettings.colors[col_ID];
                            }
                        }
                        else
                        {
                            if (colorID == layerList[i].color_A)
                            {
                                col_A = Color.clear;
                                layerList[i].color_A_hex = bannerSettings.colors[col_ID];
                            }

                        }
                    }
                    else
                    {
                        if (colorID == layerList[i].color_A)
                        {
                            ColorUtility.TryParseHtmlString(bannerSettings.colors[col_ID], out col_A);
                            layerList[i].color_A_hex = bannerSettings.colors[col_ID];
                        }
                    }

                    col_ID++;
                }

                int index = 0;
                for (int y = 0; y < texCopy.height; y++)
                {
                    for (int x = 0; x < texCopy.width; x++)
                    {
                        index++;

                        var satColor = dataOriginal[index].g;
                        // dataOriginal[index] = dataOriginal[index].g;

                        if (dataOriginal[index] == Color.red)
                        {
                            data[index] = col_A;
                        }
                        else if (dataOriginal[index] == Color.green)
                        {
                            data[index] = col_B;
                        }
                        else
                        {
                            data[index] = Color.clear;
                        }

                    }
                }
                // upload to the GPU
                texCopy.Apply();

                layerList[i].textureWork = Instantiate(texCopy) as Texture2D;

            }
        }
    }
    private void RepaintSelectedLayer()
    {

        Texture2D texture = selectedLayer.textureOriginal;
        Texture2D texCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);

        // RGBA32 texture format data layout exactly matches Color32 struct
        var dataOriginal = texture.GetRawTextureData<Color32>();
        var data = texCopy.GetRawTextureData<Color32>();

        // fill texture data with a simple pattern
        Color col_A = new Color32(255, 165, 0, 255);
        Color col_B = new Color32(0, 128, 128, 255);

        var col_ID = 0;
        foreach (var colorID in bannerSettings.color_IDs)
        {

            if (colorID == selectedLayer.color_B)
            {
                ColorUtility.TryParseHtmlString(bannerSettings.colors[col_ID], out col_B);
            }

            if (selectedLayer != layerList[0])
            {

                if (selectedLayer.stroke == true)
                {
                    if (colorID == selectedLayer.color_A)
                    {
                        ColorUtility.TryParseHtmlString(bannerSettings.colors[col_ID], out col_A);
                    }
                }
                else
                {
                    col_A = Color.clear;
                }
            }
            else
            {
                if (colorID == selectedLayer.color_A)
                {
                    ColorUtility.TryParseHtmlString(bannerSettings.colors[col_ID], out col_A);
                }
            }

            col_ID++;
        }

        int index = 0;
        for (int y = 0; y < texCopy.height; y++)
        {
            for (int x = 0; x < texCopy.width; x++)
            {
                index++;

                var satColor = dataOriginal[index].g;
                // dataOriginal[index] = dataOriginal[index].g;

                if (dataOriginal[index] == Color.red)
                {
                    data[index] = col_A;
                }
                else if (dataOriginal[index] == Color.green)
                {
                    data[index] = col_B;
                }
                else
                {
                    data[index] = Color.clear;
                }

            }
        }
        // upload to the GPU
        texCopy.Apply();

        selectedLayer.textureWork = Instantiate(texCopy) as Texture2D;

        // Debug.Log("Repaint");


    }

    public static Texture2D CombineTextures(Texture2D aBaseTexture, Texture2D aToCopyTexture)
    {
        int aWidth = aBaseTexture.width;
        int aHeight = aBaseTexture.height;
        Texture2D aReturnTexture = new Texture2D(aWidth, aHeight, TextureFormat.RGBA32, false);

        Color[] aBaseTexturePixels = aBaseTexture.GetPixels();
        Color[] aCopyTexturePixels = aToCopyTexture.GetPixels();
        Color[] aColorList = new Color[aBaseTexturePixels.Length];
        int aPixelLength = aBaseTexturePixels.Length;

        for (int p = 0; p < aPixelLength; p++)
        {
            aColorList[p] = Color.Lerp(aBaseTexturePixels[p], aCopyTexturePixels[p], aCopyTexturePixels[p].a);
        }

        aReturnTexture.SetPixels(aColorList);
        aReturnTexture.Apply(false);

        return aReturnTexture;
    }
    public void ReadBannerKey()
    {
        var path = "Assets/Settings/BannersEditor/BannerEditor_settings.asset";

        bannerSettings = (BannerEditorSettings)AssetDatabase.LoadAssetAtPath(path, typeof(BannerEditorSettings));

        int layerCount = 0;
        int layerIterCount = 0;
        string[] characters = new string[bannerKey.Length];

        layerList = new List<BannerLayer>();
        var bgLayer = (BannerLayer)ScriptableObject.CreateInstance(typeof(BannerLayer));
        // bgLayer.name = "BG_Layer";
        layerList.Add(bgLayer);

        for (int i = 0; i < bannerKey.Length; i++)
        {
            characters[i] = System.Convert.ToString(bannerKey[i]);

            var layer = layerList[layerList.Count - 1];
            // Debug.Log(layer.name);

            if (characters[i] == ".")
            {
                layerIterCount++;
            }
            else
            {
                switch (layerIterCount)
                {
                    case 0:
                        layer.sprite_ID = layer.sprite_ID + characters[i];
                        break;
                    case 1:
                        layer.color_B = layer.color_B + characters[i];
                        break;
                    case 2:
                        layer.color_A = layer.color_A + characters[i];
                        break;
                    case 3:
                        layer.scale_width = layer.scale_width + characters[i];
                        break;
                    case 4:
                        layer.scale_height = layer.scale_height + characters[i];
                        break;
                    case 5:
                        layer.pos_X = layer.pos_X + characters[i];
                        break;

                    case 6:
                        layer.pos_Y = layer.pos_Y + characters[i];

                        break;
                    case 7:
                        if (characters[i] == "1")
                        {
                            layer.stroke = true;
                        }
                        break;
                    case 8:
                        if (characters[i] == "1")
                        {
                            layer.mirror = true;
                        }
                        break;
                    case 9:
                        layer.rotation = layer.rotation + characters[i];
                        break;
                    default:
                        Debug.LogError("Banner Key Import Error");
                        break;
                }
            }

            if (layerIterCount == 10)
            {
                layerCount++;

                var newLayer = (BannerLayer)ScriptableObject.CreateInstance(typeof(BannerLayer));
                layerList.Add(newLayer);

                layerIterCount = 0;
            }
        }

        foreach (var lay in layerList)
        {
            for (int i = 0; i < bannerSettings.sprite_IDs.Length; i++)
            {
                if (bannerSettings.sprite_IDs[i] == lay.sprite_ID)
                {
                    Texture2D texture = Instantiate((Texture2D)AssetDatabase.LoadAssetAtPath(bannerSettings.sprites[i], typeof(Texture2D))) as Texture2D;
                    lay.textureOriginal = texture;
                    lay.textureWork = texture;

                }
            }
        }
        selectedLayer = layerList[0];

        LayerApplyColors();

        readed = true;
        // Debug.Log(layerList.Count);
    }

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
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
        r.x -= 16;
        r.width += 96;
        EditorGUI.DrawRect(r, color);
    }

}