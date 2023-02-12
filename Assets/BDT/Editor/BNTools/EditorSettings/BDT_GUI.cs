using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "BDT_GUI.asset", menuName = "BDT/GUI Asset", order = 1)]
public class BDT_GUI : ScriptableObject
{
    [SerializeField]
    private Texture[] _iconsContainer;
    public enum GUI_Icon
    {
        none,
        Culture,
        Kingdom,
        Faction,
        Settlement,
        NPC,
        Item,
        Equip,
        Party
    }

    public Texture GetGuiIcon(GUI_Icon icoID)
    {
        switch (icoID)
        {
            case GUI_Icon.none:
                return _iconsContainer[0];
            case GUI_Icon.Culture:
                return _iconsContainer[1];
            case GUI_Icon.Kingdom:
                return _iconsContainer[2];
            case GUI_Icon.Faction:
                return _iconsContainer[3];
            case GUI_Icon.Settlement:
                return _iconsContainer[4];
            case GUI_Icon.NPC:
                return _iconsContainer[5];
            case GUI_Icon.Item:
                return _iconsContainer[6];
            case GUI_Icon.Equip:
                return _iconsContainer[7];
            case GUI_Icon.Party:
                return _iconsContainer[8];
        }

        return EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
    }
}