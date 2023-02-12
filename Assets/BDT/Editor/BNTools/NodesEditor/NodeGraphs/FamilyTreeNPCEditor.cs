using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomNodeEditor(typeof(FamilyTreeNPC))]
public class FamilyTreeNPCEditor : NodeEditor
{

    string dataPath = "Assets/BDT/Resources/Data/";
    string modsSettingsPath = "Assets/BDT/Resources/SubModulesData/";

    public bool isFemale = false;
    public string heroID = "";
    public string heroName = "";
    public int heroAge = 0;

    public bool isAnotherFaction;

    [HideInInspector]
    public Faction faction;
    //public string factionID;

    [HideInInspector]
    public Hero father;
    [HideInInspector]
    public Hero mother;
    [HideInInspector]
    public Hero spouse;

    public string name_Father = "";
    public string name_Mother = "";
    public string name_Spouse = "";

    Color newCol;
    Color newCol_names;
    Color newCol_names2;
    Color newCol_names3;

    FamilyTreeGraph tree;
    FamilyTreeNPC node;
    public void TreeLoader()
    {
         node = target as FamilyTreeNPC;

        ColorUtility.TryParseHtmlString("#6a737b", out newCol);

        //Debug.Log("Load Tree");
        var nodesGraphPath = "Assets/BDT/Settings/NodeGraphs/FamilyTreeGraphEditor.asset";
        tree = (FamilyTreeGraph)AssetDatabase.LoadAssetAtPath(nodesGraphPath, typeof(FamilyTreeGraph));

        if (tree != null)
        {
            FamilyTreeNPC node = target as FamilyTreeNPC;
            if (node.heroAsset != null)
            {
                if (node.heroAsset.faction.Replace("Faction.", "") == tree.mainFaction.id)
                {
                    node.heroAsset.isMixedClans = false;
                    faction = tree.mainFaction;

                    if (node.heroAsset.node_X != 0 && node.heroAsset.node_Y != 0)
                    {
                        node.position.x = node.heroAsset.node_X;
                        node.position.y = node.heroAsset.node_Y;
                    }
                    else
                    {
                        node.heroAsset.node_X = (float)node.position.x;
                        node.heroAsset.node_Y = (float)node.position.y;
                    }
                }
                else
                {
                    //node.heroAsset.isMixedClans = true;
                    foreach (var fac_data in tree.facData)
                    {
                        if (fac_data.id == node.heroAsset.faction.Replace("Faction.", ""))
                        {
                            faction = fac_data;
                        }
                    }

                }

                isAnotherFaction = node.heroAsset.isMixedClans;

                //if (node.heroAsset.node_X != 0 && node.heroAsset.node_Y != 0)
                //{
                //    node.position.x = node.heroAsset.node_X;
                //    node.position.y = node.heroAsset.node_Y;
                //}
                //else
                //{
                //    node.heroAsset.node_X = node.position.x;
                //    node.heroAsset.node_Y = node.position.y;
                //}

            }

            Debug.LogWarning("Node Editor: Node Tree Loaded!!!");


        }
        else
        {
            Debug.LogWarning("Node Editor: not Tree Loaded!!!");
        }



    }

    public override void OnHeaderGUI()
    {
        node = target as FamilyTreeNPC;

        string nameResult = "";

        if (node.npcAsset != null)
        {
            if (node.npcAsset != null /*&& node.npcAsset.npcName !=  */)
            {
                nameResult = node.npcAsset.npcName;
                RemoveTSString(ref nameResult);
            }
        }
        else
        {
            nameResult = "Unknown NPC";
        }

        GUILayout.Label(nameResult, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

        // GUI.color = Color.white;
        // FamilyTreeGraph graph = node.graph as FamilyTreeGraph;
        // // if (graph.current == node) GUI.color = Color.blue;
        // string title = target.name;
        // GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        // GUI.color = Color.white;

        if (tree == null)
            TreeLoader();
    }

    public override void OnBodyGUI()
    {
       if(node.heroAsset !=null)
            EditorUtility.SetDirty(node.heroAsset);
        if(node.npcAsset !=null)
            EditorUtility.SetDirty(node.npcAsset);

        //if (node.heroAsset != null && node.heroAsset.faction.Replace("Faction.", "") == fac.id)

        FamilyNodeOut fat = new FamilyNodeOut();
        FamilyNodeOut mot = new FamilyNodeOut();
        FamilyNodeOut spo = new FamilyNodeOut();

        if (node.heroAsset != null)
        {
            if (!node.heroAsset.isMixedClans || (node.heroAsset.isMixedClans && node.heroAsset.mixedFather_redactable))
            {
                //heroID = node.ID;

                fat = node.GetInputValue<FamilyNodeOut>("F");
                if (fat != null)
                {
                    father = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
                    father = fat.heroAsset;
                    if (father != null)
                    {
                        name_Father = fat.npcAsset.npcName;
                        node.heroAsset.father = $"Hero.{fat.npcAsset.id}";
                    }

                }
                else
                {
                    // Debug.Log("Father Null");
                    if (node.heroAsset != null)
                        node.heroAsset.father = "";
                }
            }

            if (!node.heroAsset.isMixedClans || (node.heroAsset.isMixedClans && node.heroAsset.mixedMother_redactable))
            {
                mot = node.GetInputValue<FamilyNodeOut>("M");

                if (mot != null)
                {
                    mother = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
                    mother = mot.heroAsset;
                    if (mother != null)
                    {
                        name_Mother = mot.npcAsset.npcName;
                        node.heroAsset.mother = $"Hero.{mot.npcAsset.id}";
                    }

                }
                else
                {
                    // Debug.Log("Mother Null");
                    if (node.heroAsset != null)
                        node.heroAsset.mother = "";
                    // node.heroAsset
                }
            }

            if (!node.heroAsset.isMixedClans || (node.heroAsset.isMixedClans && node.heroAsset.mixedSpouse_redactable))
            {
                spo = node.GetInputValue<FamilyNodeOut>("S");

                if (spo != null)
                {
                    spouse = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
                    spouse = spo.heroAsset;
                    if (spouse != null)
                    {
                        name_Spouse = spo.npcAsset.npcName;
                        node.heroAsset.spouse = $"Hero.{spo.npcAsset.id}";
                    }

                }
                else
                {
                    // Debug.Log("Spouse Null");
                    if (node.heroAsset != null)
                        node.heroAsset.spouse = "";
                }
            }
        }
        //EditorGUILayout.LabelField($"ID - {node.heroAsset.id}", EditorStyles.miniBoldLabel);

        EditorGUI.BeginChangeCheck();
        node.heroAsset = (Hero)EditorGUILayout.ObjectField(node.heroAsset, typeof(Hero));

        var npc_prev = node.npcAsset;

        if (EditorGUI.EndChangeCheck())
        {
            NPCCharacter npc = null;
            foreach (var character in tree.charData)
            {
                if (character.id == node.heroAsset.id)
                {
                    npc = character;
                    node.npcAsset = character;
                    CheckClanMixTree(ref node.heroAsset, faction, tree.charData, tree.heroData, tree.facData);


                }
            }

            node.npcAsset = npc;
            CheckClanMixTree(ref node.heroAsset, faction, tree.charData, tree.heroData, tree.facData);

            foreach (var fac_data in tree.facData)
            {
                if (fac_data.id == node.heroAsset.faction.Replace("Faction.", ""))
                {
                    faction = fac_data;
                }
            }

            isAnotherFaction = node.heroAsset.isMixedClans;

        }

        if (node.heroAsset != null)
        {
            EditorGUI.BeginChangeCheck();

            node.npcAsset = (NPCCharacter)EditorGUILayout.ObjectField(node.npcAsset, typeof(NPCCharacter));

            if (EditorGUI.EndChangeCheck())
            {
                if (node.npcAsset.is_hero == "true")
                {
                    Hero hero = null;
                    foreach (var character in tree.heroData)
                    {
                        if (character.id == node.npcAsset.id)
                        {
                            hero = character;
                            node.heroAsset = character;
                            return;
                        }
                    }

                    if (hero != null && hero.isMixedClans)
                    {
                        foreach (var fac_data in tree.facData)
                        {
                            if (fac_data.id == node.heroAsset.faction.Replace("Faction.", ""))
                            {
                                faction = fac_data;
                            }
                        }
                    }
                    else
                    {
                        if (faction != null)
                            faction = tree.mainFaction;
                    }

                    node.heroAsset = hero;
                }
                else
                {
                    if (node.heroAsset != null && node.heroAsset.id == npc_prev.id)
                        node.npcAsset = npc_prev;
                    else
                        node.npcAsset = null;
                }
            }

        }



        //Color newCol;
        //ColorUtility.TryParseHtmlString("#6a737b", out newCol);
        GUILayout.Space(2);
        DrawUILine(newCol, 1, 2);

        if (node.heroAsset != null)
        {
            EditorGUI.BeginChangeCheck();
            var originLabelWidth = EditorGUIUtility.labelWidth;
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is From Another Faction"));
            EditorGUIUtility.labelWidth = textDimensions.x;

            isAnotherFaction = EditorGUILayout.Toggle("Is From Another Faction", isAnotherFaction);

            EditorGUIUtility.labelWidth = originLabelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                if (isAnotherFaction)
                {
                    if (faction == tree.mainFaction)
                        faction = null;
                }
                else
                {
                    faction = tree.mainFaction;
                    node.heroAsset.faction = "Faction." + tree.mainFaction.id;
                }
            }

        }
        GUILayout.Space(2);

        if (isAnotherFaction)
        {
            //DrawUILine(newCol, 1, 2);

            if (faction != null)
            {
                Color fac_col;
                ColorUtility.TryParseHtmlString("#fdb94e", out fac_col);
                GUIStyle stlFac = new GUIStyle(EditorStyles.boldLabel);
                stlFac.normal.textColor = fac_col;
                stlFac.fontSize = 14;

                var fac_naming = faction.factionName;
                RemoveTSString(ref fac_naming);
                EditorGUILayout.LabelField(fac_naming, stlFac);
                GUILayout.Space(2);

            }

            EditorGUI.BeginChangeCheck();
            faction = (Faction)EditorGUILayout.ObjectField(faction, typeof(Faction));

            if (EditorGUI.EndChangeCheck())
            {
                node.heroAsset.faction = "Faction." + faction.id;
            }


        }
        else
        {
            faction = tree.mainFaction;
        }
        GUILayout.Space(4);

        DrawUILine(newCol, 2, 2);

        if (node.npcAsset != null)
        {

            if (node.npcAsset.is_female == "true")
            {
                isFemale = true;
            }
            else
            {
                isFemale = false;
            }

            var originLabelWidth = EditorGUIUtility.labelWidth;
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("IsFemale "));
            EditorGUIUtility.labelWidth = textDimensions.x;
            isFemale = EditorGUILayout.Toggle("IsFemale", isFemale);

            EditorGUIUtility.labelWidth = originLabelWidth;

            if (isFemale == true)
            {
                node.npcAsset.is_female = "true";
            }
            else
            {
                node.npcAsset.is_female = "";
            }

            GUILayout.Space(4);


            heroName = node.npcAsset.npcName;
            var ts = heroName;
            TSString(ref ts);

            if (ts != "")
                heroName = heroName.Replace(ts, "");

            EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
            heroName = EditorGUILayout.TextField(heroName);

            node.npcAsset.npcName = ts + heroName;

            heroAge = int.Parse(node.npcAsset.age);
            EditorGUILayout.LabelField("Age:", EditorStyles.boldLabel);
            heroAge = EditorGUILayout.IntField(heroAge, GUILayout.Width(48));
            node.npcAsset.age = heroAge.ToString();

            GUILayout.Space(6);
            DrawUILine(newCol, 2, 2);
        }
        //base.OnBodyGUI();
        DrawNodeGUI(fat, mot, spo);

        if (node.heroAsset != null && !node.heroAsset.isMixedClans)
        {
            //    node.position.x = node.heroAsset.node_X;
            //    node.position.y = node.heroAsset.node_Y;
            //}
            //else
            //{
            node.heroAsset.node_X = (float)node.position.x;
            node.heroAsset.node_Y = (float)node.position.y;
        }

    }

    public void DrawNodeGUI(FamilyNodeOut fat, FamilyNodeOut mot, FamilyNodeOut spo)
    {
        node = target as FamilyTreeNPC;

        if (node.heroAsset != null)
        {
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };
            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;


            ColorUtility.TryParseHtmlString("#cfcfcf", out newCol_names);
            ColorUtility.TryParseHtmlString("#ff9c2a", out newCol_names2);
            ColorUtility.TryParseHtmlString("#efefef", out newCol_names3);

            GUIStyle styleNames = new GUIStyle(EditorStyles.label);
            styleNames.normal.textColor = newCol_names;
            GUIStyle styleNames2 = new GUIStyle(EditorStyles.boldLabel);
            styleNames2.normal.textColor = newCol_names2;
            GUIStyle styleNames3 = new GUIStyle(EditorStyles.boldLabel);
            styleNames3.normal.textColor = newCol_names3;

            int port = 0;

            while (iterator.NextVisible(enterChildren))
            {

                if (node.heroAsset.isMixedClans)
                {
                    if (port == 1)
                    {
                        if (!node.heroAsset.mixedFather_redactable)
                        {
                            EditorGUILayout.LabelField("Father:", styleNames);
                            if (node.heroAsset.mixedFather != "")
                            {
                                var f_mix_name = node.heroAsset.mixedFather;
                                RemoveTSString(ref f_mix_name);
                                EditorGUILayout.LabelField($"{f_mix_name} - from {node.heroAsset.mixedFather_fac}", styleNames3);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Father:", styleNames);
                            if (fat != null)
                            {
                                RemoveTSString(ref name_Father);
                                EditorGUILayout.LabelField(name_Father, styleNames2);
                            }
                        }

                        DrawUILine(newCol, 1, 2);
                    }
                    if (port == 2)
                    {
                        if (!node.heroAsset.mixedMother_redactable)
                        {
                            EditorGUILayout.LabelField("Mother:", styleNames);
                            if (node.heroAsset.mixedMother != "")
                            {
                                var m_mix_name = node.heroAsset.mixedMother;
                                RemoveTSString(ref m_mix_name);
                                EditorGUILayout.LabelField($"{m_mix_name} - from {node.heroAsset.mixedMother_fac}", styleNames3);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Mother:", styleNames);
                            if (mot != null)
                            {
                                RemoveTSString(ref name_Mother);
                                EditorGUILayout.LabelField(name_Mother, styleNames2);
                            }
                        }
                        DrawUILine(newCol, 1, 2);
                    }
                    if (port == 3)
                    {
                        if (!node.heroAsset.mixedSpouse_redactable)
                        {
                            EditorGUILayout.LabelField("Spouse:", styleNames);
                            if (node.heroAsset.mixedSpouse != "")
                            {
                                var s_mix_name = node.heroAsset.mixedSpouse;
                                RemoveTSString(ref s_mix_name);
                                EditorGUILayout.LabelField($"{s_mix_name} - from {node.heroAsset.mixedSpouse_fac}", styleNames3);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Spouse:", styleNames);
                            if (spo != null)
                            {
                                RemoveTSString(ref name_Spouse);
                                EditorGUILayout.LabelField(name_Spouse, styleNames2);
                            }
                        }
                    }
                }
                else
                {
                    if (port == 1)
                    {
                        EditorGUILayout.LabelField("Father:", styleNames);
                        if (fat != null)
                        {
                            RemoveTSString(ref name_Father);
                            EditorGUILayout.LabelField(name_Father, styleNames2);
                        }
                        DrawUILine(newCol, 1, 2);

                    }
                    if (port == 2)
                    {
                        EditorGUILayout.LabelField("Mother:", styleNames);
                        if (mot != null)
                        {
                            RemoveTSString(ref name_Mother);
                            EditorGUILayout.LabelField(name_Mother, styleNames2);
                        }
                        DrawUILine(newCol, 1, 2);

                    }

                    if (port == 3)
                    {
                        EditorGUILayout.LabelField("Spouse:", styleNames);
                        if (spo != null)
                        {
                            RemoveTSString(ref name_Spouse);
                            EditorGUILayout.LabelField(name_Spouse, styleNames2);
                        }
                    }

                }



                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                if (port == 0)
                {
                    if (node.heroAsset.isMixedClans)
                    {
                        if (node.heroAsset.mixedFather_redactable)
                        {
                            NodeEditorGUILayout.PropertyField(iterator, true);
                            DrawUILine(newCol, 1, 2);

                        }
                    }
                    else
                    {
                        NodeEditorGUILayout.PropertyField(iterator, true);
                    }
                }
                else if (port == 1)
                {
                    if (node.heroAsset.isMixedClans)
                    {
                        if (node.heroAsset.mixedMother_redactable)
                        {
                            NodeEditorGUILayout.PropertyField(iterator, true);
                        }
                    }
                    else
                    {
                        NodeEditorGUILayout.PropertyField(iterator, true);
                    }

                }
                else if (port == 2)
                {
                    if (node.heroAsset.isMixedClans)
                    {
                        if (node.heroAsset.mixedSpouse_redactable)
                        {
                            NodeEditorGUILayout.PropertyField(iterator, true);
                        }
                    }
                    else
                    {
                        NodeEditorGUILayout.PropertyField(iterator, true);
                    }
                }
                else
                {
                    NodeEditorGUILayout.PropertyField(iterator, true);
                }
                port++;

            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (XNode.NodePort dynamicPort in target.DynamicPorts)
            {


                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
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
    private static void TSString(ref string inputString)
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
            inputString = TS_Name;
        }
    }


    public override int GetWidth()
    {
        return 320;
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

    /// NodesChecker
    /// 
    private void CheckClanMixTree(ref Hero hero, Faction fac, List<NPCCharacter> npc_list, List<Hero> hero_list, List<Faction> faction_list)
    {
        //Debug.Log(hero.id);
        hero.isMixedClans = false;
        hero.mixedFather = "";
        hero.mixedFather_fac = "";
        hero.mixedFather_redactable = false;
        hero.mixedMother = "";
        hero.mixedMother_fac = "";
        hero.mixedMother_redactable = false;
        hero.mixedSpouse = "";
        hero.mixedSpouse_fac = "";
        hero.mixedSpouse_redactable = false;


        if (hero.faction.Replace("Faction.", "") != fac.id)
        {
            hero.isMixedClans = true;
        }

        if (hero.isMixedClans)
        {
            if (hero.father != "")
            {
                var nm = hero.father.Replace("Hero.", "");

                foreach (var father_npc in npc_list)
                {
                    if (father_npc.id == nm && father_npc != hero)
                    {

                        var npc_name_str = father_npc.npcName;
                        RemoveTSString(ref npc_name_str);
                        hero.mixedFather = npc_name_str;

                        foreach (var father_hero in hero_list)
                        {
                            if (father_hero.id == nm)
                            {
                                if (father_hero.faction.Replace("Faction.", "") != fac.id)
                                {
                                    hero.mixedFather_fac = father_hero.faction.Replace("Faction.", "");
                                }
                                else
                                {
                                    hero.mixedFather_redactable = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                hero.mixedFather_redactable = true;
            }


            if (hero.mother != "")
            {
                var nm = hero.mother.Replace("Hero.", "");

                foreach (var mother_npc in npc_list)
                {
                    if (mother_npc.id == nm && mother_npc != hero)
                    {

                        var npc_name_str = mother_npc.npcName;
                        RemoveTSString(ref npc_name_str);
                        hero.mixedMother = npc_name_str;

                        foreach (var mother_hero in hero_list)
                        {
                            if (mother_hero.id == nm)
                            {

                                if (mother_hero.faction.Replace("Faction.", "") != fac.id)
                                {
                                    hero.mixedMother_fac = mother_hero.faction.Replace("Faction.", "");
                                }
                                else
                                {
                                    hero.mixedMother_redactable = true;
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                hero.mixedMother_redactable = true;
            }


            if (hero.spouse != "")
            {
                var nm = hero.spouse.Replace("Hero.", "");

                foreach (var spouse_npc in npc_list)
                {
                    if (spouse_npc.id == nm && spouse_npc != hero)
                    {

                        var npc_name_str = spouse_npc.npcName;
                        RemoveTSString(ref npc_name_str);
                        hero.mixedSpouse = npc_name_str;

                        foreach (var spouse_hero in hero_list)
                        {
                            if (spouse_hero.id == nm)
                            {

                                if (spouse_hero.faction.Replace("Faction.", "") != fac.id)
                                {
                                    hero.mixedSpouse_fac = spouse_hero.faction.Replace("Faction.", "");

                                }
                                else
                                {
                                    hero.mixedSpouse_redactable = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                hero.mixedSpouse_redactable = true;
            }
        }



        if (hero.faction.Replace("Faction.", "") == fac.id)
        {
            foreach (var parent_hero in hero_list)
            {
                if (parent_hero.isMixedClans && parent_hero != hero)
                {
                    if (hero.father.Replace("Hero.", "") == parent_hero.id)
                    {
                        hero.mixedFather = parent_hero.id;
                    }
                    if (hero.mother.Replace("Hero.", "") == parent_hero.id)
                    {
                        hero.mixedMother = parent_hero.id;
                    }
                    if (hero.spouse.Replace("Hero.", "") == parent_hero.id)
                    {
                        hero.mixedSpouse = parent_hero.id;
                    }
                }
            }
        }



        if (hero.isMixedClans)
            foreach (var faction in faction_list)
            {
                if (faction.id == hero.mixedFather_fac)
                {
                    var fac_name_str = faction.factionName;
                    RemoveTSString(ref fac_name_str);
                    hero.mixedFather_fac = fac_name_str;

                }
                if (faction.id == hero.mixedMother_fac)
                {
                    var fac_name_str = faction.factionName;
                    RemoveTSString(ref fac_name_str);
                    hero.mixedMother_fac = fac_name_str;

                }
                if (faction.id == hero.mixedSpouse_fac)
                {
                    var fac_name_str = faction.factionName;
                    RemoveTSString(ref fac_name_str);
                    hero.mixedSpouse_fac = fac_name_str;
                }
            }

       // AssetDatabase.Refresh();
        return;
    }
}
