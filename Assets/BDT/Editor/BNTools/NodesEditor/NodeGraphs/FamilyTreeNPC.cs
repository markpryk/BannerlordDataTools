using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;



public class FamilyTreeNPC : Node
{
    [SerializeField]
    [HideInInspector]
    public FamilyNodeOut portAsset;

    [HideInInspector]
    public NPCCharacter npcAsset;
    [HideInInspector]
    public Hero heroAsset;

    [HideInInspector]
    public Hero father;
    [HideInInspector]
    public Hero mother;
    [HideInInspector]
    public Hero spouse;

    [HideInInspector]
    public string ID;

    //[HideInInspector]
    //public bool external_faction;

    [HideInInspector]
    public Faction faction;


    [Input(ShowBackingValue.Never, ConnectionType.Override)] public FamilyNodeOut F;
    [Input(ShowBackingValue.Never, ConnectionType.Override)] public FamilyNodeOut M;
    [Input(ShowBackingValue.Never, ConnectionType.Override)] public FamilyNodeOut S;

    // [Input(ShowBackingValue.Never, ConnectionType.Override)] public string H;

    // [Output] public string houseOut;
    [Output] public FamilyNodeOut Out;


    public override object GetValue(NodePort port)
    {
        portAsset = (FamilyNodeOut)ScriptableObject.CreateInstance(typeof(FamilyNodeOut));

        portAsset.heroAsset = (Hero)ScriptableObject.CreateInstance(typeof(Hero));
        portAsset.heroAsset = heroAsset;

        portAsset.npcAsset = (NPCCharacter)ScriptableObject.CreateInstance(typeof(NPCCharacter));
        portAsset.npcAsset = npcAsset;

        Out = portAsset;
        return Out;

    }





}
