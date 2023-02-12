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

public class ItemsEditor : EditorWindow
{
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    public string[] sortBy_options = new string[3];
    public int sortBy_index;
    public ModuleReceiver loadedMod;
    Vector2 scrollPos;
    string[] alpb = new string[27] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };

    public Dictionary<string, List<Item>> item_List;
    public Dictionary<string, List<Item>> item_SearchBackup;
    public Dictionary<string, List<Item>> item_Search;
    public Dictionary<string, bool> item_ListBool;
    public Dictionary<string, bool> item_ListBoolBackup;
    public List<Item> modifyList;


    bool is_Input_now;
    string searchInput;

    bool show_armor = true;
    bool show_weapon = true;

    bool show_horse = true;
    bool show_crafted = false;
    bool show_goods = true;
    bool show_other = true;

    //

    Color armorCol;
    GUIStyle armorStyle;
    Color weaponCol;
    GUIStyle weaponStyle;
    Color tradeCol;
    GUIStyle tradeStyle;
    Color horseCol;
    GUIStyle horseStyle;

    // 

    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    // string dataPath = "Assets/BDT/Resources/Data/";

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
        EditorWindow.GetWindow(typeof(ItemsEditor));
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


        item_List = new Dictionary<string, List<Item>>();
        item_SearchBackup = new Dictionary<string, List<Item>>();
        item_Search = new Dictionary<string, List<Item>>();
        item_ListBool = new Dictionary<string, bool>();
        item_ListBoolBackup = new Dictionary<string, bool>();


        currEventSelection = new string[2];


        if (loadedMod != null && loadedMod.modFilesData != null)
        {
            CheckAndResort();

        }

    }

    void OnGUI()
    {

        armorStyle = new GUIStyle(EditorStyles.boldLabel);
        weaponStyle = new GUIStyle(EditorStyles.boldLabel);
        tradeStyle = new GUIStyle(EditorStyles.boldLabel);
        horseStyle = new GUIStyle(EditorStyles.boldLabel);

        ColorUtility.TryParseHtmlString("#629aa9", out armorCol);
        ColorUtility.TryParseHtmlString("#aea400", out weaponCol);
        ColorUtility.TryParseHtmlString("#ffa500", out horseCol);
        ColorUtility.TryParseHtmlString("#ffc2e5", out tradeCol);

        armorStyle.normal.textColor = armorCol;
        weaponStyle.normal.textColor = weaponCol;
        horseStyle.normal.textColor = horseCol;
        tradeStyle.normal.textColor = tradeCol;



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
            var npc = loadedMod.modFilesData.npcChrData.NPCCharacters;
            GUILayout.Space(16);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Sort By:", EditorStyles.boldLabel);
            sortBy_index = EditorGUILayout.Popup(sortBy_index, sortBy_options, GUILayout.Width(128));

            GUILayout.Space(4);

            EditorGUILayout.LabelField("Filter:", EditorStyles.boldLabel);

            searchInput = EditorGUILayout.TextField(searchInput, EditorStyles.toolbarSearchField, GUILayout.Width(256));
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {
                show_armor = true;
                show_weapon = true;
                show_horse = true;
                show_other = true;
                show_crafted = true;
                show_goods = true;

                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                show_armor = false;
                show_weapon = false;
                show_horse = false;
                show_other = false;
                show_crafted = false;
                show_goods = false;
                CheckAndResort();
                this.Repaint();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawUILine(colUILine, 1, 4);


            EditorGUILayout.BeginHorizontal();

            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_crafted, "Crafted");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_armor, "Armor");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_weapon, "Weapon");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_horse, "Horse");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_goods, "Goods");
            DrawUILineVertical(colUILine, 1, 1, 16);
            CreateEditorToggle(ref show_other, "Other");

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            DrawUILine(colUILine, 1, 4);

            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("check");

                CheckAndResort();

            }

            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.LabelField("Items", headerLabelStyle);
            // GUILayout.Space(2);
            DrawUILine(colUILine, 1, 12);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create Item", GUILayout.Width(128)))
            {

                if (ITEM_EDITOR_Manager == null)
                {
                    ItemsEditorManager assetMng = (ItemsEditorManager)ScriptableObject.CreateInstance(typeof(ItemsEditorManager));
                    assetMng.windowStateID = 1;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "item_template";
                    assetMng.assetName_new = "item_template";
                    assetMng.assetID_new = "item_template_ID";
                }
                else
                {
                    ItemsEditorManager assetMng = ITEM_EDITOR_Manager;
                    assetMng.windowStateID = 1;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "item_template";
                    assetMng.assetName_new = "item_template";
                    assetMng.assetID_new = "item_template_ID";

                }


            }
            DrawUILineVertical(colUILine, 1, 1, 16);
            if (GUILayout.Button("Modify Item", GUILayout.Width(128)))
            {

                var dic = new List<Item>();

                for (int i = 0; i < item_ListBool.Count; i++)
                {
                    if (item_ListBool.ToArray()[i].Value == true)
                    {
                        foreach (var facGrp in item_List)
                        {
                            foreach (var tem_InList in facGrp.Value)
                            {
                                if (item_ListBool.ToArray()[i].Key == tem_InList.id)
                                {
                                    dic.Add(tem_InList);
                                }
                            }
                        }
                    }

                }

                if (ITEM_EDITOR_Manager == null)
                {
                    ItemsEditorManager assetMng = (ItemsEditorManager)ScriptableObject.CreateInstance(typeof(ItemsEditorManager));
                    assetMng.windowStateID = 2;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;
                    // assetMng.obj = npc;

                    assetMng.assetName_org = "item_template";
                    assetMng.assetName_new = "item_template";
                    assetMng.assetID_new = "item_template_ID";

                    assetMng.modifyDic = new List<Item>(dic);
                }
                else
                {
                    ItemsEditorManager assetMng = ITEM_EDITOR_Manager;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 2;
                    assetMng.bdt_settings = settingsAsset;

                    assetMng.assetName_org = "item_template";
                    assetMng.assetName_new = "item_template";
                    assetMng.assetID_new = "item_template_ID";

                    assetMng.modifyDic = new List<Item>(dic);

                }



            }
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", EditorStyles.boldLabel);

            // EditorGUILayout.BeginHorizontal();

            GUILayout.Button("Select: ", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            if (GUILayout.Button("All", GUILayout.Width(48)))
            {

                string[] _keys = new string[item_ListBool.Keys.Count];

                int i = 0;
                foreach (var ck in item_ListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < item_ListBool.Keys.Count; i3++)
                {
                    item_ListBool[_keys[i3]] = true;
                }
                CheckAndResort();

            }
            DrawUILineVertical(colUILine, 1, 1, 16);

            if (GUILayout.Button("None", GUILayout.Width(48)))
            {
                string[] _keys = new string[item_ListBool.Keys.Count];

                int i = 0;
                foreach (var ck in item_ListBool.Keys)
                {
                    _keys[i] = ck;
                    i++;
                }

                for (int i3 = 0; i3 < item_ListBool.Keys.Count; i3++)
                {
                    item_ListBool[_keys[i3]] = false;
                }
                CheckAndResort();

            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            // DrawUILine(colUILine, 1, 4);
            DrawUILine(colUILine, 6, 12);


            if (is_Input_now)
            {

                item_SearchBackup = new Dictionary<string, List<Item>>();

                foreach (var key in item_List)
                {
                    var tempFacList = new List<Item>();

                    foreach (var Item_InList in key.Value)
                    {

                        if (sortBy_index == 0)
                        {
                            string soloName = Item_InList.itemName;

                            RemoveTSString(ref soloName);

                            if (soloName != null && soloName != "" && searchInput != null)
                            {
                                if (soloName.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(Item_InList);
                                }
                            }
                        }
                        else if (sortBy_index == 1)
                        {
                            if (searchInput != null)
                            {
                                if (Item_InList.id.ToLower().Contains(searchInput.ToLower()))
                                {
                                    tempFacList.Add(Item_InList);
                                }
                            }
                        }
                    }
                    item_SearchBackup.Add(key.Key, tempFacList);
                }
                item_Search = new Dictionary<string, List<Item>>(item_SearchBackup);
                is_Input_now = false;

            }

            if (searchInput != null && searchInput != "")
            {
                DrawList(item_Search);
            }
            else
            {
                DrawList(item_List);
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
        var itemData = loadedMod.modFilesData.itemsData.items;

        if (item_ListBool != null && item_ListBool.Count != 0)
        {
            item_ListBoolBackup = item_ListBool;
        }

        if (show_armor || show_weapon || show_horse || show_crafted || show_goods || show_other)
        {
            // EditorGUILayout.BeginHorizontal();
            SortByMethod(itemData);
        }
        else
        {
            // SortByMethodNoneBool(itemData);
            item_List = new Dictionary<string, List<Item>>();
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

        if (item_List.Count != 0)
        {
            item_ListBool = new Dictionary<string, bool>();
            foreach (var list in item_List)
            {
                foreach (var npc in list.Value)
                {
                    if (!item_ListBool.ContainsKey(npc.id))
                    {
                        item_ListBool.Add(npc.id, false);
                    }
                }
            }
        }

        // check selection bools 
        if (item_ListBoolBackup != null && item_ListBool != null && item_ListBoolBackup.Count != 0 && item_ListBool.Count != 0)
        {
            for (int i = 0; i < item_ListBool.Count; i++)
            {

                if (item_ListBoolBackup.ContainsKey(item_ListBool.ToArray()[i].Key))
                {
                    var b = false;
                    item_ListBoolBackup.TryGetValue(item_ListBool.ToArray()[i].Key, out b);
                    item_ListBool[item_ListBool.ToArray()[i].Key] = b;
                }

            }
        }
    }

    private void DrawList(Dictionary<string, List<Item>> facListInput)
    {

        var originLabelWidth = EditorGUIUtility.labelWidth;
        var tagLabelStyle = new GUIStyle(EditorStyles.helpBox);

        Color newCol;
        ColorUtility.TryParseHtmlString("#da5a47", out newCol);

        tagLabelStyle.normal.textColor = newCol;
        tagLabelStyle.fontSize = 24;
        tagLabelStyle.fontStyle = FontStyle.Bold;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));

        if (facListInput != null && facListInput.Count != 0)
        {

            foreach (var fac_list in facListInput)
            {
                if (fac_list.Value.Count != 0)
                {
                    var chrUP = fac_list.Key.ToCharArray();
                    var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                    DrawUILine(colUILine, 1, 12);


                    if (sortBy_index == 0 || sortBy_index == 1)
                    {
                        EditorGUILayout.LabelField(chrUPstr, tagLabelStyle);
                    }


                    DrawUILine(colUILine, 1, 12);
                    DrawUILine(colUILine, 1, 12);

                    int i_FAC = 0;
                    foreach (var facChar in fac_list.Value)
                    {

                        var item = fac_list.Value[i_FAC];

                        var soloName = "";
                        if (sortBy_index == 0)
                        {
                            soloName = item.itemName;
                            RemoveTSString(ref soloName);

                            if (soloName == "")
                            {
                                soloName = item.id;
                            }
                        }
                        if (sortBy_index == 1)
                        {
                            soloName = item.id;
                        }

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(512));

                        // if (fac.is_bandit == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, banditStyle);
                        // }
                        // else if (fac.is_mafia == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, mafiaStyle);
                        // }


                        // else if (fac.is_minor_faction == "true")
                        // {
                        //     Color titleColor;
                        //     ColorUtility.TryParseHtmlString(fac._BannerColorA, out titleColor);
                        //     minorFactionStyle.normal.textColor = titleColor;

                        //     EditorGUILayout.LabelField(soloName, minorFactionStyle);
                        // }
                        // else if (fac.is_outlaw == "true")
                        // {
                        //     EditorGUILayout.LabelField(soloName, outlawStyle);
                        // }
                        // else
                        // {

                        // if (item._BannerColorB != "")
                        // {
                        //     Color titleColor;
                        //     ColorUtility.TryParseHtmlString(item._BannerColorB, out titleColor);
                        //     minorFactionStyle.normal.textColor = titleColor * 2;
                        //     EditorGUILayout.LabelField(soloName, minorFactionStyle);

                        // }
                        // else
                        // {
                        EditorGUILayout.LabelField(soloName, EditorStyles.label);

                        // }

                        // }


                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(""));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        EditorGUILayout.Space(-128);

                        EditorGUI.BeginChangeCheck();

                        if (item_ListBool.ContainsKey(item.id))
                        {
                            item_ListBool[item.id] = EditorGUILayout.Toggle(item_ListBool[item.id], GUILayout.ExpandWidth(false));

                        }

                        if (EditorGUI.EndChangeCheck())
                        {

                            if (currEvent.shift)
                            {
                                if (currEventSelection[0] == "")
                                {
                                    currEventSelection[0] = item.id;
                                }
                                else
                                {
                                    currEventSelection[1] = item.id;


                                    if (currEventSelection[1] != "")
                                    {
                                        int start = 0;
                                        int end = 0;

                                        bool boolValue = true;

                                        if (item_ListBool[currEventSelection[0]] == false)
                                        {
                                            boolValue = false;
                                        }


                                        for (int i3 = 0; i3 < item_ListBool.Count; i3++)
                                        {
                                            if (item_ListBool.ToArray()[i3].Key == currEventSelection[0])
                                            {
                                                start = i3;
                                            }
                                            else if (item_ListBool.ToArray()[i3].Key == currEventSelection[1])
                                            {
                                                end = i3;
                                            }
                                        }

                                        for (int i3 = 0; i3 < item_ListBool.Count; i3++)
                                        {
                                            if (IsValueBetween(i3, start, end))
                                            {
                                                item_ListBool[item_ListBool.ToArray()[i3].Key] = boolValue;
                                            }
                                        }
                                    }
                                }




                            }

                        }


                        EditorGUIUtility.labelWidth = originLabelWidth;

                        // EditorGUILayout.Space(-64);

                        EditorGUILayout.ObjectField(item, typeof(Item), true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(4);
                        DrawUILine(colUILine, 1, 4);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.ExpandWidth(false));
                        GUILayout.Space(32);


                        EditorGUILayout.EndHorizontal();

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

    public void SortByMethod(List<Item> facListInput)
    {
        this.item_List = new Dictionary<string, List<Item>>();


        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Item>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var item in facListInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = item.itemName;

                        if (soloName == "")
                        {
                            soloName = item.id;
                        }
                    }
                    if (sortBy_index == 1)
                    {
                        soloName = item.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = "";
                    if (soloName != "")
                    {
                        soloFirstChar = soloName.ToCharArray()[0].ToString();
                    }

                    if (character != "other")
                    {
                        if (soloFirstChar == character || soloFirstChar == chrUPstr)
                        {

                            if (item.IsArmor)
                            {
                                CheckShowBool(newList, item, ref item.IsArmor, ref show_armor);
                            }
                            else if (item.IsWeapon)
                            {
                                CheckShowBool(newList, item, ref item.IsWeapon, ref show_weapon);
                            }

                            else if (item.IsHorse)
                            {
                                CheckShowBool(newList, item, ref item.IsHorse, ref show_horse);
                            }
                            else if (item.IsCraftedItem)
                            {
                                CheckShowBool(newList, item, ref item.IsCraftedItem, ref show_crafted);
                            }
                            else if (item.Type == "Goods")
                            {
                                var b = true;
                                CheckShowBool(newList, item, ref b, ref show_goods);
                            }
                            else
                            {
                                if (show_other)
                                {
                                    newList.Add(item);
                                }
                            }
                        }

                    }

                }
                item_List.Add(character, newList);
            }

        }


    }


    public void SortByMethodNoneBool(List<Item> itemInput)
    {
        item_List = new Dictionary<string, List<Item>>();

        // Debug.Log(sortBy_index);
        if (sortBy_index == 0 || sortBy_index == 1)
        {
            foreach (var character in alpb)
            {
                var newList = new List<Item>();
                var chrUP = character.ToCharArray();
                var chrUPstr = Char.ToUpper(chrUP[0]).ToString();

                foreach (var item in itemInput)
                {
                    var soloName = "";
                    if (sortBy_index == 0)
                    {
                        soloName = item.itemName;

                        if (soloName == "")
                        {
                            soloName = item.id;
                        }
                    }
                    if (sortBy_index == 1)
                    {
                        soloName = item.id;
                    }

                    RemoveTSString(ref soloName);

                    var soloFirstChar = soloName.ToCharArray()[0].ToString();

                    if (soloFirstChar == character || soloFirstChar == chrUPstr)
                    {
                        if (!item.IsArmor && !item.IsWeapon && !item.IsHorse && !item.IsCraftedItem && item.Type != "Goods")
                        {
                            newList.Add(item);
                        }
                    }
                }
                item_List.Add(character, newList);
            }


            // foreach (var npcChar in itemInput)
            // {
            //     bool contains = false;
            //     foreach (var item in item_List)
            //     {
            //         if (item.Value.Contains(npcChar))
            //         {
            //             contains = true;
            //             break;

            //         }
            //     }

            //     if (!contains)
            //     {
            //         item_List["other"].Add(npcChar);
            //     }

            // }
        }

    }

    private void CheckShowBool(List<Item> newList, Item item, ref bool facParam, ref bool faBoolParam)
    {
        if (facParam)
        {
            if (faBoolParam != false)
            {
                newList.Add(item);
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

    public static ItemsEditorManager ITEM_EDITOR_Manager
    {
        get { return EditorWindow.GetWindow<ItemsEditorManager>(); }
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

