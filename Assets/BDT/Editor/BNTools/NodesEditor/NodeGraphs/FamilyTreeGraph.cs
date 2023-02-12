using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]

public class FamilyTreeGraph : NodeGraph
{
    public Faction mainFaction;
    public List<NPCCharacter> charData  ;
    public List<Hero> heroData ;
    public List<Faction> facData ;
    //void OnEnable()
    //{
    //    Debug.Log("Start");
    //}


}