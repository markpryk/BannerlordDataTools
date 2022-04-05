using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using XNode;
using XNodeEditor;

public class PartyTemplatesEditor : EditorWindow
{

    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<PartyTemplate>> PT_list;
    public Dictionary<string, List<PartyTemplate>> PT_SearchBackup;
    public Dictionary<string, List<PartyTemplate>> PT_Search;
    public Dictionary<string, bool> PT_ListBool;
    public Dictionary<string, bool> PT_ListBoolBackup;
    public List<PartyTemplate> modifyList;


    bool is_Input_now;
    string searchInput;

    bool show_bandit = true;
    bool show_mafia = true;
    bool show_outlaw = true;
    bool show_minor = true;
    bool show_other = true;

    //

    Color banditCol;
    GUIStyle banditStyle;
    Color mafiaCol;
    GUIStyle mafiaStyle;
    Color minorFactionCol;
    GUIStyle minorFactionStyle;
    Color outlawCol;
    GUIStyle outlawStyle;

    // 

    string configPath = "Assets/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    // string dataPath = "Assets/Resources/Data/";

    BDTSettings settingsAsset;

    Event currEvent;
    // ModuleReceiver currMod;

    string[] currEventSelection = new string[2];

    void OnFocus()
    {
        if (loadedMod == null || loadedMod.modFilesData == null)
        {
            // Debug.Log(null);
            EditorWindow.GetWindow(typeof(BNDataEditorWindow));
        }
        else
        {
            CheckAndResort();
        }
    }


    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PartyTemplatesEditor));
    }

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

        if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
        {
            loadedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));

        }



        sortBy_options[0] = "Name";
        sortBy_options[1] = "ID";
        sortBy_options[2] = "Kingdom";


        PT_list = new Dictionary<string, List<PartyTemplate>>();
        PT_SearchBackup = new Dictionary<string, List<PartyTemplate>>();
        PT_Search = new Dictionary<string, List<PartyTemplate>>();
        PT_ListBool = new Dictionary<string, bool>();
        PT_ListBoolBackup = new Dictionary<string, bool>();


        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            CheckAndResort();

        }

    }

    void OnGUI()
    {

        var headerLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newColLabel;
        ColorUtility.TryParseHtmlString("#fdd666", out newColLabel);

        headerLabelStyle.normal.textColor = newColLabel;
        headerLabelStyle.fontSize = 24;
        headerLabelStyle.fontStyle = FontStyle.Bold;

        currEvent = Event.current;

        if (!currEvent.shift)
        {
            currEventSelection[0] = "";
            currEventSelection[1] = "";

        }

        if (settingsAsset.currentModule != loadedMod.id)
        {
            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                loadedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
            }

            CheckAndResort();

        }

        if (loadedMod)
        {

            EditorGUI.BeginChangeCheck();

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Filter:", EditorStyles.boldLabel);

            searchInput = EditorGUILayout.TextField(searchInput, EditorStyles.toolbarSearchField, GUILayout.Width(256));

            // EditorGUILayout.BeginHorizontal();

            // if (GUILayout.Button("All", GUILayout.Width(48)))
            // {
            //     show_bandit = true;
            //     show_mafia = true;
            //     show_minor = true;
            //     show_outlaw = true;
            //     show_other = true;

            //     CheckAndResort();

            // }
            // DrawUILineVertical(colUILine, 1, 1, 16);

            // if (GUILayout.Button("None", GUILayout.Width(48)))
            // {
            //     show_bandit = false;
            //     show_mafia = false;
            //     show_minor = false;
            //     show_outlaw = false;
            //     show_other = false;
            //     CheckAndResort();
            //     this.Repaint();
            // }

            // EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);

            DrawUILine(colUILine, 1, 4);

            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("check");

                CheckAndResort();

            }

            // DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Party Templates", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create PT", GUILayout.Width(128)))
            {

                if (PT_EDITOR_Manager == null)
                {
                    PartyTemplatesEditorManager assetMng = (PartyTemplatesEditorManager)ScriptableObject.CreateInstance(typeof(PartyTemplatesEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "pt_template";
                    assetMng.assetName_new = "pt_template";
                    assetMng.assetID_new = "pt_template_ID";
                }
                else
                {
                    PartyTemplatesEditorManager assetMng = PT_EDITOR_Manager;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "pt_template";
                    assetMng.assetName_new = "pt_template";
                    assetMng.assetID_new = "pt_template";

                }

                // assetMng.CopyAssetAsOverride();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify PT", GUILayout.Width(128)))
            {

                var dic = new List<PartyTemplate>();

                for (int i = 0; i < PT_ListBool.Count; i++)
                {
                    if (PT_ListBool.ToArray()[i].Value == true)
                    {
                        foreach (var facGrp in PT_list)
                        {
                            foreach (var facInList in facGrp.Value)
                            {
                                if (PT_ListBool.ToArray()[i].Key == facInList.id)
                                {
                                    dic.Add(facInList);
                                }
                            }
                        }
                    }

                }

                if (PT_EDITOR_Manager == null)
                {
                    PartyTemplatesEditorManager assetMng = (PartyTemplatesEditorManager)ScriptableObject.CreateInstance(typeof(PartyTemplatesEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "pt_template";
                    assetMng.assetName_new = "pt_template";
                    assetMng.assetID_new = "pt_template_ID";

                    assetMng.modifyDic = new List<PartyTemplate>(dic);
                }
                else
                {
                    PartyTemplatesEditorManager assetMng = PT_EDITOR_Manager;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "pt_template";
                    assetMng.assetName_new = "pt_template";
                    assetMng.assetID_new = "pt_template";

                    assetMng.modifyDic = new List<PartyTemplate>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[PT_ListBool.Keys.Count];

                int i = 0;
                foreach (var ck in PT_ListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < PT_ListBool.Keys.Count; i3++)
                {
                    PT_ListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[PT_ListBool.Keys.Count];

                int i = 0;
                foreach (var ck in PT_ListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < PT_ListBool.Keys.Count; i3++)
                {
                    PT_ListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);


            if (is_Input_now)
            {

                PT_SearchBackup = new Dictionary<string, List<PartyTemplate>>();

                foreach (var key in PT_list)
                {
                    var tempFacList = new List<PartyTemplate>();

                    foreach (var pt_inList in key.Value)
                    {


                        if (searchInput != null)
                        {
                            if (pt_inList.id.ToLower().Contains(searchInput.ToLower()))
                            {
                                tempFacList.Add(pt_inList);
                            }
                        }


                    }
                    PT_SearchBackup.Add(key.Key, tempFacList);
                }
                PT_Search = new Dictionary<string, List<PartyTemplate>>(PT_SearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(PT_Search);
            }
            else
            {
                DrawList(PT_list);
            }




        }
        else
        {
            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                loadedMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
            }
        }


    }

    // ! CHECK
    public void CheckAndResort()
    {
        var PTData = loadedMod.modFilesData.PTdata.partyTemplates;

        if (PT_ListBool != null && PT_ListBool.Count != 0)
        {
            PT_ListBoolBackup = PT_ListBool;
        }

        if (show_bandit || show_mafia || show_minor || show_outlaw || show_other)
        {
            // EditorGUILayout.BeginHorizontal();
            SortByMethod(PTData);
        }
        else
        {
            // SortByMethodNoneBool(npcData);
            PT_list = new Dictionary<string, List<PartyTemplate>>();
        }

        if (searchInput != "" && is_Input_now == false)
        {
            is_Input_now = true;
        }

        // if (show_HeroLink && show_Hero)
        // {
        //     CheckHeroesData();

        //     SortByMethod(npcData);
        // }

        if (PT_list.Count != 0)
        {
            PT_ListBool = new Dictionary<string, bool>();
            foreach (var list in PT_list)
            {
                foreach (var npc in list.Value)
                {
                    if (!PT_ListBool.ContainsKey(npc.id))
                    {
                        PT_ListBool.Add(npc.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (PT_ListBoolBackup != null && PT_ListBool != null && PT_ListBoolBackup.Count != 0 && PT_ListBool.Count != 0)
        {
            for (int i = 0; i < PT_ListBool.Count; i++)
            {

                if (PT_ListBoolBackup.ContainsKey(PT_ListBool.ToArray()[i].Key))
                {
                    var b = false;
                    PT_ListBoolBackup.TryGetValue(PT_ListBool.ToArray()[i].Key, out b);
                    PT_ListBool[PT_ListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<PartyTemplate>> ptListImput)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var tagLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newCol;
        ColorUtility.TryParseHtmlString("#da5a47", out newCol);

        tagLabelStyle.normal.textColor = newCol;
        tagLabelStyle.fontSize = 24;
        tagLabelStyle.fontStyle = FontStyle.Bold;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(720));

        if (ptListImput != null && ptListImput.Count != 0)
        {

            foreach (var PT_list in ptListImput)
            {
                if (PT_list.Value.Count != 0)
                {
                    var chrUP = PT_list.Key.ToCharArray();
                    var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    DrawUILine(colUILine, 1, 12);

                    EditorGUILayout.LabelField(chrUPstr, tagLabelStyle);

                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_FAC = 0;
                    foreach (var ptChar in PT_list.Value)
                    {

                        var PT = PT_list.Value[i_FAC];
                        var soloName = PT.id;

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        EditorGUILayout.LabelField(soloName, EditorStyles.label);


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-128);

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.Space(64);

                        if (PT_ListBool.ContainsKey(PT.id))
                        {
                            PT_ListBool[PT.id] = EditorGUILayout.Toggle(PT_ListBool[PT.id], GUILayout.ExpandWidth(false));
                        }

                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = PT.id;
                                }
                                else
                                {
                                    currEventSelection[1] = PT.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (PT_ListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < PT_ListBool.Count; i3++)
                                        {
                                            if (PT_ListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (PT_ListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < PT_ListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                PT_ListBool[PT_ListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }




                            }

                        }


                        EditorGUIUtility.labelWidth = originLabelWidth;

                        EditorGUILayout.ObjectField(PT, typeof(PartyTemplate), true);

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(-38);
                        DrawUILine(colUILine, 1, 4);

                        EditorGUILayout.BeginVertical();
                        // GUILayout.Space(16);
                        DrawUILine(colUILine, 4, 12);
                        EditorGUILayout.EndVertical();


                        i_FAC++;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        // Repaint();


    }

    public void SortByMethod(List<PartyTemplate> facListInput)
    {
        this.PT_list = new Dictionary<string, List<PartyTemplate>>();

        foreach (var character in alpb)
        {
            var newList = new List<PartyTemplate>();
            var chrUP = character.ToCharArray();
            var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

            foreach (var PT in facListInput)
            {
                var soloName = PT.id;

                RemoveTSString(ref soloName);

                var soloFirstChar = soloName.ToCharArray()[0].ToString();
                if (character != "other")
                {
                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {
                        newList.Add(PT);
                    }
                }
            }
            PT_list.Add(character, newList);
        }

    }


    public void SortByMethodNoneBool(List<PartyTemplate> facInput)
    {
        PT_list = new Dictionary<string, List<PartyTemplate>>();

        // Debug.Log(sortBy_index);
        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<PartyTemplate>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var PT in facInput)
                {
                    var soloName = PT.id;

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();

                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {
                        newList.Add(PT);
                    }
                }
                PT_list.Add(character, newList);
            }


            foreach (var npcChar in facInput)
            {
                bool contains = false;
                foreach (var facL in PT_list)
                {
                    if (facL.Value.Contains(npcChar))
                    {
                        contains = true;
                        break;

                    }
                }

                if (!contains)
                {
                    PT_list["other"].Add(npcChar);
                }

            }
        }

    }

    private void CheckShowBool(List<Faction> newList, Faction fac, ref string facParam, ref bool faBoolParam)
    {
        if (facParam == "true")
        {
            if (faBoolParam != false)
            {
                newList.Add(fac);
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

    public static PartyTemplatesEditorManager PT_EDITOR_Manager
    {
        get { return EditorWindow.GetWindow<PartyTemplatesEditorManager>(); }
    }

    public bool IsValueBetween(double testValue, double bound1, double bound2)
    {
        return (testValue >= Math.Min(bound1, bound2) && testValue <= Math.Max(bound1, bound2));
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
