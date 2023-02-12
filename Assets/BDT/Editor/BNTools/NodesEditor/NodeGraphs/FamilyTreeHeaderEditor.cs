using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using System.Text.RegularExpressions;
using Unity.Collections;
using System.IO;


[CustomNodeEditor(typeof(FamilyTreeHeader))]
public class FamilyTreeHeaderEditor : NodeEditor
{

    FamilyTreeGraph tree;
    bool isTreeLoaded;

    string configPath = "Assets/BDT/Settings/BDT_settings.asset";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";
    string dataPath = "Assets/BDT/Resources/Data/";

    Color newCol_names;
    Color color_line;
    GUIStyle styleNames;

    string headerTitle;
    string fac_name;
    Hero owner;
    string owner_nm;
    Kingdom owner_kingdom;
    FamilyTreeHeader node;

    public override void OnHeaderGUI()
    {
        FamilyTreeNPC node = target as FamilyTreeNPC;

        GUILayout.Label("Faction", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }
    public override void OnBodyGUI()
    {
        if (!isTreeLoaded)
            TreeLoader();

        // GUILayout.Space(32);


        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 32;
        headerStyle.normal.textColor = newCol_names;

        headerTitle = node.header_faction.factionName;

        RemoveTSString(ref headerTitle);
        EditorGUILayout.LabelField(headerTitle, headerStyle, GUILayout.MaxHeight(64));

        //DrawUILine(color_line, 2, 6);
        EditorGUILayout.ObjectField(node.header_faction, typeof(Faction), true);

        DrawUILine(color_line, 1, 8);
        //GUILayout.Space(4);


        EditorGUILayout.LabelField("Faction Name:", EditorStyles.boldLabel);

        fac_name = node.header_faction.factionName;
        RemoveTSString(ref fac_name);
        EditorGUI.BeginChangeCheck();
        fac_name = EditorGUILayout.TextField(fac_name);
        if (EditorGUI.EndChangeCheck())
        {
            var naming = GetTSString(ref node.header_faction.factionName) + fac_name;
            node.header_faction.factionName = naming;
            //AssetDatabase.Refresh();

        }
        DrawUILine(color_line, 1, 2);

        // owner
        if (node.header_faction.owner != null && node.header_faction.owner != "")
        {
            if (node.header_faction.owner.Contains("Hero."))
            {
                string dataName = node.header_faction.owner.Replace("Hero.", "");

                foreach (var hero in tree.heroData)
                {
                    if (dataName == hero.id)
                    {
                        owner = hero;
                        break;
                    }
                }

            }
        }

        EditorGUILayout.LabelField("Faction Owner:", EditorStyles.boldLabel);
        if (owner != null && owner_nm != "")
        {
            EditorGUILayout.LabelField(owner_nm, styleNames);
            GUILayout.Space(4);
        }

        EditorGUI.BeginChangeCheck();
        owner = (Hero)EditorGUILayout.ObjectField(owner, typeof(Hero), true);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(node.header_faction);
            if (owner != null)
            {
                node.header_faction.owner = "Hero." + owner.id;
            }
            else
            {
                node.header_faction.owner = "";
            }
            //AssetDatabase.Refresh();
        }

        DrawUILine(color_line, 1, 2);

        FindKingdomOwner(node);

        EditorGUILayout.LabelField("Kingdom:", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        owner_kingdom = (Kingdom)EditorGUILayout.ObjectField(owner_kingdom, typeof(Kingdom), true);
        if (EditorGUI.EndChangeCheck())
        {
            if (owner_kingdom != null)
            {
                node.header_faction.super_faction = "Kingdom." + owner_kingdom.id;

                foreach (var npc in tree.charData)
                {
                    if (npc.id == owner.id)
                    {
                        owner_nm = npc.name;
                        RemoveTSString(ref owner_nm);
                        break;
                    }
                }
            }
            else
            {
                node.header_faction.super_faction = "";
                owner_nm = "";
            }
        }

        DrawUILine(color_line, 1, 2);

        node.header_faction.Node_X = (float)node.position.x;
        node.header_faction.Node_Y = (float)node.position.y;

    }

    private void FindKingdomOwner(FamilyTreeHeader node)
    {


        // Super Faction

        if (node.header_faction.super_faction != null && node.header_faction.super_faction != "")
        {
            if (node.header_faction.super_faction.Contains("Kingdom."))
            {

                string dataName = node.header_faction.super_faction.Replace("Kingdom.", "");
                string asset = dataPath + node.header_faction.moduleID + "/Kingdoms/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    owner_kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(asset, typeof(Kingdom));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + node.header_faction.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Kingdoms/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                owner_kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Kingdom));
                                break;
                            }
                            else
                            {
                                owner_kingdom = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (owner_kingdom == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == node.header_faction.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.kingdomsData.kingdoms)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Kingdoms/" + dataName + ".asset";
                                            owner_kingdom = (Kingdom)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Kingdom));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (owner_kingdom == null)
                        {
                            Debug.Log("Kingdom " + dataName + " - Not EXIST in" + " ' " + node.header_faction.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }

            }
        }

    }

    public void TreeLoader()
    {
        node = target as FamilyTreeHeader;

        EditorUtility.SetDirty(node.header_faction);

        styleNames = new GUIStyle(EditorStyles.boldLabel);
        styleNames.normal.textColor = newCol_names;

        ColorUtility.TryParseHtmlString("#fcd000", out newCol_names);
        ColorUtility.TryParseHtmlString("#4d4f53", out color_line);

        //Debug.Log("Load Tree");
        var nodesGraphPath = "Assets/BDT/Settings/NodeGraphs/FamilyTreeGraphEditor.asset";
        tree = (FamilyTreeGraph)AssetDatabase.LoadAssetAtPath(nodesGraphPath, typeof(FamilyTreeGraph));

        if (node.header_faction.owner != null && node.header_faction.owner != "")
        {
            string dataName = node.header_faction.owner.Replace("Hero.", "");

            foreach (var npc in tree.charData)
            {
                if (npc.id == dataName)
                {
                    owner_nm = npc.npcName;
                    RemoveTSString(ref owner_nm);
                    break;
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

    public string GetTSString(ref string inputString)
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
            return TS_Name;
        }
        else
        {
            return "";
        }
    }

    public override int GetWidth()
    {
        FamilyTreeHeader node = target as FamilyTreeHeader;
        var headerWidth = GUI.skin.label.CalcSize(new GUIContent(node.header_faction.factionName));

        if ((int)headerWidth.x < 90)
        {
            return 256;

        }
        else
        {
            int size = (int)headerWidth.x * 3;
            return size;
        }

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
