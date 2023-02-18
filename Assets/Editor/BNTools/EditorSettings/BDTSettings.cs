using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "BDT_settings.asset", menuName = "BDT/Settings Asset", order = 1)]
public class BDTSettings : ScriptableObject
{
    const string BDT_Version = "0.0.63.2b";
    const string BN_Version_compatibility = "v1.1.0";

    public string BDTVersion
    {
        get { return BDT_Version; }
    }
    public string BannerlordVersionCompatibility
    {
        get { return BN_Version_compatibility; }
    }

    public bool load_a;
    public bool load_b;

    public string BNModulesPath;
    public string currentModule;

    public bool ableToDeleteModOrigins;

    public BDT_GUI GUI;
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
    public Sprite[] VillageTypesSprites;

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
    public string[] MaterialTypesDefinitions;
    public string[] HairBeardCoverTypesDefinitions;
    public string[] PhysicsMatsDefinitions;
    public string[] ThurstDamageTypesDefinitions;
    public string[] SwingDamageTypesDefinitions;
    public string[] ItemUsageDefinitions;
    public string[] ItemEffectsDefinitions;
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
    public string[] TattooTagDefinitions;

    //Update 1.8.0
    public string[] PreferredUpgradeFormationDefinitions;
    public string[] RaceDefinitions;

    public void AssignWorldMapProductionIcons()
    {
        VillageTypesSprites = new Sprite[VillagesTypeDefinitions.Length];

        for (int i = 0; i < VillagesTypeDefinitions.Length; i++)
        {

            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/prod_none.png", typeof(Sprite));
            string def = VillagesTypeDefinitions[i];

            if (def == "none")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/prod_none.png", typeof(Sprite));
            else if (def == "silk_plant")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/silk.png", typeof(Sprite));
            else if (def == "cattle_farm")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/cow.png", typeof(Sprite));
            else if (def == "silver_mine")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/silver.png", typeof(Sprite));
            else if (def == "iron_mine")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/iron.png", typeof(Sprite));
            else if (def == "lumberjack")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/wood.png", typeof(Sprite));
            else if (def == "wheat_farm")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/wheat.png", typeof(Sprite));
            else if (def == "fisherman")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/fish.png", typeof(Sprite));
            else if (def == "europe_horse_ranch")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/euro_horse.png", typeof(Sprite));
            else if (def == "sheep_farm")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/sheep.png", typeof(Sprite));
            else if (def == "flax_plant")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/flax.png", typeof(Sprite));
            else if (def == "vineyard")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/vine.png", typeof(Sprite));
            else if (def == "date_farm")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/dates.png", typeof(Sprite));
            else if (def == "olive_trees")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/olives.png", typeof(Sprite));
            else if (def == "swine_farm")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/swine.png", typeof(Sprite));
            else if (def == "salt_mine")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/salt.png", typeof(Sprite));
            else if (def == "clay_mine")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/clay.png", typeof(Sprite));
            else if (def == "sturgian_horse_ranch")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/sturg_horse.png", typeof(Sprite));
            else if (def == "trapper")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/skin.png", typeof(Sprite));
            else if (def == "desert_horse_ranch")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/desert_horse.png", typeof(Sprite));
            else if (def == "vlandian_horse_ranch")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/vland_horse.png", typeof(Sprite));
            else if (def == "battanian_horse_ranch")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/battan_horse.png", typeof(Sprite));
            else if (def == "steppe_horse_ranch")
                icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/steppe_horse.png", typeof(Sprite));

            VillageTypesSprites[i] = icon;
        }

        //if (!VillagesTypeDefinitions.Contains("camel_ranch"))
        //{
        //    var path = "Assets/Settings/EditorResources/VillageTypesSprites/WMP_Editor/camel.png";
        //    AddDefinitionAndIcon(path, "camel_ranch");
        //}

    }

    private void AddDefinitionAndIcon(string spritePath, string definition)
    {
        var spr = (Sprite)AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite));
        if (spr == null)
            return;

        var tempSprite = new Sprite[VillageTypesSprites.Length + 1];
        VillageTypesSprites.CopyTo(tempSprite, 0);
        VillageTypesSprites = tempSprite;
        VillageTypesSprites[VillageTypesSprites.Length - 1] = spr;

        var tempDefinition = new string[VillagesTypeDefinitions.Length + 1];
        VillagesTypeDefinitions.CopyTo(tempDefinition, 0);
        VillagesTypeDefinitions = tempDefinition;
        VillagesTypeDefinitions[VillagesTypeDefinitions.Length - 1] = definition;
    }

    public void CheckAndRefreshCraftingPieces()
    {
        if (NativePiecesDefinitions == null || NativePiecesDefinitions.Length == 0 || NativePiecesDefinitions[0] == null)
        {
            var cp = new List<CraftingPiece>();
            var cp_path = "Assets/Settings/Definitions/CraftingPieces/";

            string[] assets_pieces = Directory.GetFiles(cp_path);

            foreach (var piecePath in assets_pieces)
            {
                var piece = (CraftingPiece)AssetDatabase.LoadAssetAtPath(piecePath, typeof(CraftingPiece));

                if (piece != null)
                    cp.Add(piece);
            }

            NativePiecesDefinitions = cp.ToArray();
        }
    }

    public void CheckGUI()
    {
        if (GUI == null)
        {
            var path = "Assets/Settings/EditorResources/BDT_GUI/BDT_GUI.asset";
            GUI = (BDT_GUI)AssetDatabase.LoadAssetAtPath(path, typeof(BDT_GUI));
        }
    }
}