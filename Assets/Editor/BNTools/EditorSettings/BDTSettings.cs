using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "BDT_settings.asset", menuName = "BDT/Settings Asset", order = 1)]
public class BDTSettings : ScriptableObject
{
    public bool load_a;
    public bool load_b;
    
    public string BNModulesPath;
    public string currentModule;

    //Language
    [SerializeField]
    public ModLanguage[] LanguagesDefinitions;
    //Kingdoms
    public string[] PoliciesDefinitions;
    
    //Settlements
    [SerializeField]
    public LocationComplexTemplate[] LocationComplexTemplateDefinitions;
    public string[] VillagesTypeDefinitions;
    public string[] SettlementsTypeDefinitions;

    //NPC

    public string[] NPCVoiceDefinitions;
    public string[] NPCOccupationDefinitions;
    public string[] NPCDefaultGroupsDefinitions;
    public string[] FormationPosPrefDefinitions;
    public string[] SkillsDefinitions;
    public string[] TraitsDefinitions;
    public string[] EquipmentSlotsDefinitions;

    // ITEMS
    [SerializeField]
    public List<ItemType> ItemTypesDefinitions;
    public string[] WeaponClassDefinitions;
    public string[] SubTypesDefinitions;
    public string[] ItemCategoryDefinitions;
    public string[] ModifierGroupDefinitions;
    public string[] MaterialTypesDefinitions;
    public string[] HairBeardCoverTypesDefinitions;
    public string[] PhysicsMatsDefinitions;
    public string[] ThurstDamageTypesDefinitions;
    public string[] SwingDamageTypesDefinitions;
    public string[] ItemUsageDefinitions;
    public string[] BodyMeshTypesDefinitions;
    public string[] MonstersDefinitions;
    public string[] HorseFamilyTypes;
    public string[] HorseSkeletonTypes;
    public string[] ManeCoverTypes;
    public string[] AmmoClassesDefinitions;
    public string[] CraftingTemplatesDefinitions;
    public string[] PieceTypesDefinitions;
    public CraftingPiece[] NativePiecesDefinitions;

    // Update 1.7.2
    public string[] CulturalFeatsDefinitions;
    public string[] ItemModifierGroupDefinitions;
    public string[] HairTagDefinitions;
    public string[] BeardTagDefinitions;

}