using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
[System.Serializable]

[CustomEditor(typeof(Item))]
public class ItemAssetEditor : Editor
{
    string dataPath = "Assets/Resources/Data/";
    string modsSettingsPath = "Assets/Resources/SubModulesData/";
    string folder = "ItemsTranslationData";


    ///Crafting Templates
    ///
    public bool showCraftingPieces;

    public string[] crafting_templates_options;
    public int crafting_templates_index = 0;
    public string[] piece_type_options;
    public int piece_type_index = 0;
    ///
    public string[] item_class_options;
    public int item_class_index = 0;
    public string[] item_type_options;
    public int item_type_index = 0;
    public string[] weapon_class_options;
    public int weapon_class_index = 0;
    public string[] item_category_options;
    public int item_category_index = 0;
    public string[] subtypes_options;
    public int subtypes_index = 0;

    public string[] wpn_effect_options;
    public int wpn_effect_index = 0;

    //-------
    public string[] physics_mats_options;
    public int physics_mats_index = 0;
    public string[] item_usage_options;
    public int item_usage_index = 0;
    public string[] thrust_type_options;
    public int thrust_type_index = 0;
    public string[] swing_type_options;
    public int swing_type_index = 0;

    ///
    public string[] modifier_group_options;
    public int modifier_group_index = 0;
    public string[] material_type_options;
    public int material_type_index = 0;

    //-----

    public string[] ammo_class_options;
    public int ammo_class_index = 0;
    public string[] body_mesh_type_options;
    public int body_mesh_type_index = 0;

    //
    public string[] monster_options;
    public int monster_index = 0;
    public string[] HRS_skeletons_options;
    public int HRS_skeletons_index = 0;

    //-----

    public string[] hair_cover_type_options;
    public int hair_cover_type_index = 0;
    public string[] beard_cover_type_options;
    public int beard_cover_type_index = 0;
    public string[] mane_cover_type_options;
    public int mane_cover_type_index = 0;
    public string[] family_type_options;
    public int family_type_index = 0;

    //-----

    //Attribute bools
    bool recalcBody_Bool;
    bool IsMultiplayer_Bool;
    bool UseTableau_Bool;
    bool HolsterPriority_Bool;

    //WEAPON FLAGS FLAGS
    bool WPN_FLG_CanBlockRanged_Bool;
    bool WPN_FLG_HasHitPoints_Bool;

    //
    bool WPN_FLG_MeleeWeapon_Bool;
    bool WPN_FLG_PenaltyWithShield_Bool;
    bool WPN_FLG_NotUsableWithOneHand_Bool;
    bool WPN_FLG_TwoHandIdleOnMount_Bool;
    bool WPN_FLG_WideGrip_Bool;

    //

    bool WPN_FLG_RangedWeapon_Bool;
    bool WPN_FLG_HasString_Bool;
    bool WPN_FLG_StringHeldByHand_Bool;
    bool WPN_FLG_AutoReload_Bool;
    bool WPN_FLG_UnloadWhenSheathed_Bool;
    bool WPN_FLG_CantReloadOnHorseback_Bool;

    //

    bool WPN_FLG_Consumable_Bool;
    bool WPN_FLG_UseHandAsThrowBase_Bool;
    bool WPN_FLG_CanPenetrateShield_Bool;
    bool WPN_FLG_MultiplePenetration_Bool;
    bool WPN_FLG_Burning_Bool;
    bool WPN_FLG_LeavesTrail_Bool;
    bool WPN_FLG_AmmoCanBreakOnBounceBack_Bool;
    bool WPN_FLG_AmmoSticksWhenShot_Bool;
    bool WPN_FLG_AttachAmmoToVisual_Bool;
    bool WPN_FLG_CanKnockDown_Bool;
    bool WPN_FLG_MissileWithPhysics_Bool;
    bool WPN_FLG_AmmoBreaksOnBounceBack_Bool;
    bool WPN_FLG_AffectsArea_Bool;

    // Armor Bools
    bool ARMOR_isCoverBody_Bool;
    bool ARMOR_isCoverHead_Bool;
    bool ARMOR_isCoverHands_Bool;
    bool ARMOR_isCoverLegs_Bool;
    bool ARMOR_hasGenderVariations_Bool;

    // FLAGS
    bool FLG_Civilian_Bool;
    bool FLG_UseTeamColor_Bool;
    bool FLG_DoesNotHideChest_Bool;

    //
    bool FLG_WoodenParry_Bool;
    bool FLG_ForceAttachOffHandPrimaryItemBone_Bool;
    bool FLG_DropOnWeaponChange_Bool;
    bool FLG_HeldInOffHand_Bool;
    bool FLG_HasToBeHeldUp_Bool;
    bool FLG_DoNotScaleBodyAccordingToWeaponLength_Bool;
    bool FLG_ForceAttachOffHandSecondaryItemBone_Bool;
    bool FLG_QuickFadeOut_Bool;
    bool FLG_CannotBePickedUp_Bool;
    bool FLG_DropOnAnyAction_Bool;
    ///
    bool HRS_IsMountable_Bool;
    bool HRS_IsPackAnimal_Bool;
    ///
    bool TRADE_IsFood_Bool;
    bool TRADE_MoraleBonus_Bool;
    bool TRADE_IsMerchandise_Bool = true;

    // Update 1.7.2
    // int
    public int WPN_reload_phase_count_int;

    // bool
    public bool WPN_FLG_AffectsAreaBig_Bool;

    // bool
    public bool CT_has_modifier_bool;

    // Weapon Modifiers Group
    //public string[] wpn_modifier_group_options;
    //public int wpn_modifier_group_index = 0;

    // Update 1.8.0
    public bool FLG_CanBePickedUpFromCorpse_bool;
    public bool FLG_WoodenAttack_bool;

    public bool prerequisite_item_bool;
    public bool using_arm_band_bool;

    public float WPN_shield_width;
    public float WPN_shield_down_length;

    public bool WPN_FLG_FirearmAmmo_bool;
    public bool WPN_FLG_NotUsableWithTwoHand_bool;
    public bool WPN_FLG_BonusAgainstShield_bool;
    public bool WPN_FLG_CanDismount_bool;

    // TODO
    //public float HRS_mane_mesh_multiplier;
    //public string[] body_deform_type_options;
    //public int body_deform_type_index = 0;
    //public int WPN_banner_level_value;
    //public int WPN_effect_amount;
   

    // HORSE MATERIALS 
    List<HorseManeMaterial> horseMatsList;
    bool[] showMaterials = new bool[4];
    bool isCoveringMat;
    Color[] matColorsA = new Color[4];
    Color[] matColorsB = new Color[4];


    Vector3 posVec3;
    Vector3 rotVec3;
    Vector3 rotSpeedVec3;
    Vector3 centerMassVec3;
    Vector3 stickingPosVec3;
    Vector3 stickingRotVec3;
    Vector3 ammoOffset;
    Vector3 HolsterPosition;

    Item item;
    List<CraftingPiece> CP_list;

    string soloName;
    string soloText;
    string nameTranslationString;
    TranslationString translationStringName;
    public Culture cultureIs;

    Vector2 textScrollPos;
    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);
    bool isDependency = false;
    string configPath = "Assets/Settings/BDT_settings.asset";
    BDTSettings settingsAsset;

    string isDependMsg = "|DPD-MSG|";
    public void OnEnable()
    {
        item = (Item)target;
        EditorUtility.SetDirty(item);

        item_class_options = new string[2];
        item_class_options[0] = "Item";
        item_class_options[1] = "CraftedItem";

        if (item.IsCraftedItem == true)
        {
            item_class_index = 1;
        }
        else
        {
            item_class_index = 0;
        }

        if (settingsAsset == null)
        {
            if (System.IO.File.Exists(configPath))
            {
                settingsAsset = (BDTSettings)AssetDatabase.LoadAssetAtPath(configPath, typeof(BDTSettings));


                // ITEM TYPE
                item_type_options = new string[settingsAsset.ItemTypesDefinitions.Count];

                int i = 0;
                foreach (var typeData in settingsAsset.ItemTypesDefinitions)
                {
                    item_type_options[i] = typeData.itemTypeName;
                    i++;
                }

                i = 0;
                foreach (var type in item_type_options)
                {
                    if (item.Type == "headArmor")
                    {
                        item.Type = "HeadArmor";
                        item.IsArmorNobleHelm = true;
                    }

                    if (type == item.Type)
                    {
                        item_type_index = i;
                    }
                    i++;
                }

                CreateOptions(ref item.WPN_weapon_class, ref weapon_class_options, ref weapon_class_index, settingsAsset.WeaponClassDefinitions);
                CreateOptions(ref item.WPN_physics_material, ref physics_mats_options, ref physics_mats_index, settingsAsset.PhysicsMatsDefinitions);
                CreateOptions(ref item.WPN_item_usage, ref item_usage_options, ref item_usage_index, settingsAsset.ItemUsageDefinitions);
                CreateOptions(ref item.WPN_thrust_damage_type, ref thrust_type_options, ref thrust_type_index, settingsAsset.ThurstDamageTypesDefinitions);
                CreateOptions(ref item.WPN_swing_damage_type, ref swing_type_options, ref swing_type_index, settingsAsset.SwingDamageTypesDefinitions);
                CreateOptions(ref item.WPN_ammo_class, ref ammo_class_options, ref ammo_class_index, settingsAsset.AmmoClassesDefinitions);
                CreateOptions(ref item.WPN_modifier_group, ref modifier_group_options, ref modifier_group_index, settingsAsset.ItemModifierGroupDefinitions);
                CreateOptions(ref item.ARMOR_material_type, ref material_type_options, ref material_type_index, settingsAsset.MaterialTypesDefinitions);
                CreateOptions(ref item.ARMOR_body_mesh_type, ref body_mesh_type_options, ref body_mesh_type_index, settingsAsset.BodyMeshTypesDefinitions);

                CreateOptions(ref item.ARMOR_hair_cover_type, ref hair_cover_type_options, ref hair_cover_type_index, settingsAsset.HairBeardCoverTypesDefinitions);
                CreateOptions(ref item.ARMOR_beard_cover_type, ref beard_cover_type_options, ref beard_cover_type_index, settingsAsset.HairBeardCoverTypesDefinitions);

                CreateOptions(ref item.ARMOR_mane_cover_type, ref mane_cover_type_options, ref mane_cover_type_index, settingsAsset.ManeCoverTypes);
                CreateOptions(ref item.ARMOR_family_type, ref family_type_options, ref family_type_index, settingsAsset.HorseFamilyTypes);

                CreateOptions(ref item.HRS_skeleton_scale, ref HRS_skeletons_options, ref HRS_skeletons_index, settingsAsset.HorseSkeletonTypes);

                CreateOptions(ref item.item_category, ref item_category_options, ref item_category_index, settingsAsset.ItemCategoryDefinitions);
                CreateOptions(ref item.subtype, ref subtypes_options, ref subtypes_index, settingsAsset.SubTypesDefinitions);

                CreateOptions(ref item.CT_crafting_template, ref crafting_templates_options, ref crafting_templates_index, settingsAsset.CraftingTemplatesDefinitions);

                // MODIFIERS GROUPS WPN - update 1.7.2
                //CreateOptions(ref item.WPN_item_modifier_group, ref wpn_modifier_group_options, ref wpn_modifier_group_index, settingsAsset.ItemModifierGroupDefinitions);

                CreateOptions(ref item.WPN_effect, ref wpn_effect_options, ref wpn_effect_index, settingsAsset.ItemEffectsDefinitions);


                if (item.IsCraftedItem)
                    RefreshCraftingPieces();

                //if (item.CT_pieces_Type != null)
                //{
                //i = 0;
                //foreach (var piece_type in item.CT_pieces_Type)
                //{
                //    CreateOptions(ref item.CT_pieces_Type[i], ref piece_type_options, ref piece_type_index, settingsAsset.PieceTypesDefinitions);
                //    i++;
                //}
                //}

                // Monsters TYPE
                monster_options = new string[settingsAsset.MonstersDefinitions.Length];

                int Mi = 0;
                foreach (var mnstrData in settingsAsset.MonstersDefinitions)
                {
                    monster_options[Mi] = mnstrData;
                    Mi++;
                }

                Mi = 0;
                foreach (var mnstrType in monster_options)
                {
                    if (item.HRS_monster != null)
                    {
                        if (item.HRS_monster.Contains(mnstrType))
                        {
                            monster_index = Mi;
                        }
                    }
                    Mi++;
                }
            }
            else
            {
                Debug.Log("BDT settings dont exist");
            }
        }

        posVec3 = StringToVector3(item.WPN_position);
        rotVec3 = StringToVector3(item.WPN_rotation);
        rotSpeedVec3 = StringToVector3(item.WPN_rotation_speed);
        centerMassVec3 = StringToVector3(item.WPN_center_of_mass);

        stickingPosVec3 = StringToVector3(item.WPN_sticking_position);
        stickingRotVec3 = StringToVector3(item.WPN_sticking_rotation);

        ammoOffset = StringToVector3(item.AmmoOffset);
        HolsterPosition = StringToVector3(item.holster_position_shift);

        CheckFlagBool(item.WPN_FLG_CanBlockRanged, ref WPN_FLG_CanBlockRanged_Bool);
        CheckFlagBool(item.WPN_FLG_HasHitPoints, ref WPN_FLG_HasHitPoints_Bool);

        CheckFlagBool(item.WPN_FLG_MeleeWeapon, ref WPN_FLG_MeleeWeapon_Bool);
        CheckFlagBool(item.WPN_FLG_PenaltyWithShield, ref WPN_FLG_PenaltyWithShield_Bool);
        CheckFlagBool(item.WPN_FLG_NotUsableWithOneHand, ref WPN_FLG_NotUsableWithOneHand_Bool);
        CheckFlagBool(item.WPN_FLG_TwoHandIdleOnMount, ref WPN_FLG_TwoHandIdleOnMount_Bool);
        CheckFlagBool(item.WPN_FLG_WideGrip, ref WPN_FLG_WideGrip_Bool);

        CheckFlagBool(item.WPN_FLG_RangedWeapon, ref WPN_FLG_RangedWeapon_Bool);
        CheckFlagBool(item.WPN_FLG_HasString, ref WPN_FLG_HasString_Bool);
        CheckFlagBool(item.WPN_FLG_StringHeldByHand, ref WPN_FLG_StringHeldByHand_Bool);
        CheckFlagBool(item.WPN_FLG_AutoReload, ref WPN_FLG_AutoReload_Bool);
        CheckFlagBool(item.WPN_FLG_UnloadWhenSheathed, ref WPN_FLG_UnloadWhenSheathed_Bool);
        CheckFlagBool(item.WPN_FLG_CantReloadOnHorseback, ref WPN_FLG_CantReloadOnHorseback_Bool);

        CheckFlagBool(item.WPN_FLG_Consumable, ref WPN_FLG_Consumable_Bool);
        CheckFlagBool(item.WPN_FLG_UseHandAsThrowBase, ref WPN_FLG_UseHandAsThrowBase_Bool);
        CheckFlagBool(item.WPN_FLG_CanPenetrateShield, ref WPN_FLG_CanPenetrateShield_Bool);
        CheckFlagBool(item.WPN_FLG_MultiplePenetration, ref WPN_FLG_MultiplePenetration_Bool);
        CheckFlagBool(item.WPN_FLG_LeavesTrail, ref WPN_FLG_LeavesTrail_Bool);
        CheckFlagBool(item.WPN_FLG_AmmoCanBreakOnBounceBack, ref WPN_FLG_AmmoCanBreakOnBounceBack_Bool);
        CheckFlagBool(item.WPN_FLG_Burning, ref WPN_FLG_Burning_Bool);
        CheckFlagBool(item.WPN_FLG_AmmoSticksWhenShot, ref WPN_FLG_AmmoSticksWhenShot_Bool);

        CheckFlagBool(item.WPN_FLG_AttachAmmoToVisual, ref WPN_FLG_AttachAmmoToVisual_Bool);
        CheckFlagBool(item.WPN_FLG_CanKnockDown, ref WPN_FLG_CanKnockDown_Bool);
        CheckFlagBool(item.WPN_FLG_MissileWithPhysics, ref WPN_FLG_MissileWithPhysics_Bool);
        CheckFlagBool(item.WPN_FLG_AmmoBreaksOnBounceBack, ref WPN_FLG_AmmoBreaksOnBounceBack_Bool);
        CheckFlagBool(item.WPN_FLG_AffectsArea, ref WPN_FLG_AffectsArea_Bool);

        // Update 1.7.2
        CheckFlagBool(item.WPN_FLG_AffectsAreaBig, ref WPN_FLG_AffectsAreaBig_Bool);

        // ARMOR BOOLS

        CheckFlagBool(item.ARMOR_covers_body, ref ARMOR_isCoverBody_Bool);
        CheckFlagBool(item.ARMOR_covers_head, ref ARMOR_isCoverHead_Bool);
        CheckFlagBool(item.ARMOR_covers_hands, ref ARMOR_isCoverHands_Bool);
        CheckFlagBool(item.ARMOR_covers_legs, ref ARMOR_isCoverLegs_Bool);
        CheckFlagBool(item.ARMOR_has_gender_variations, ref ARMOR_hasGenderVariations_Bool);

        CheckFlagBool(item.FLG_Civilian, ref FLG_Civilian_Bool);
        CheckFlagBool(item.FLG_UseTeamColor, ref FLG_UseTeamColor_Bool);
        CheckFlagBool(item.FLG_DoesNotHideChest, ref FLG_DoesNotHideChest_Bool);

        CheckFlagBool(item.FLG_WoodenParry, ref FLG_WoodenParry_Bool);
        CheckFlagBool(item.FLG_ForceAttachOffHandPrimaryItemBone, ref FLG_ForceAttachOffHandPrimaryItemBone_Bool);
        CheckFlagBool(item.FLG_DropOnWeaponChange, ref FLG_DropOnWeaponChange_Bool);
        CheckFlagBool(item.FLG_HeldInOffHand, ref FLG_HeldInOffHand_Bool);
        CheckFlagBool(item.FLG_HasToBeHeldUp, ref FLG_HasToBeHeldUp_Bool);
        CheckFlagBool(item.FLG_DoNotScaleBodyAccordingToWeaponLength, ref FLG_DoNotScaleBodyAccordingToWeaponLength_Bool);
        CheckFlagBool(item.FLG_ForceAttachOffHandSecondaryItemBone, ref FLG_ForceAttachOffHandSecondaryItemBone_Bool);
        CheckFlagBool(item.FLG_QuickFadeOut, ref FLG_QuickFadeOut_Bool);
        CheckFlagBool(item.FLG_CannotBePickedUp, ref FLG_CannotBePickedUp_Bool);
        CheckFlagBool(item.FLG_DropOnAnyAction, ref FLG_DropOnAnyAction_Bool);

        CheckFlagBool(item.HRS_is_mountable, ref HRS_IsMountable_Bool);
        CheckFlagBool(item.HRS_is_pack_animal, ref HRS_IsPackAnimal_Bool);

        CheckFlagBool(item.TRADE_morale_bonus, ref TRADE_MoraleBonus_Bool);
        CheckFlagBool(item.IsFood, ref TRADE_IsFood_Bool);

        CheckFlagBool(item.recalculate_body, ref recalcBody_Bool);
        CheckFlagBool(item.multiplayer_item, ref IsMultiplayer_Bool);
        CheckFlagBool(item.using_tableau, ref UseTableau_Bool);

        CheckFlagBool(item.has_lower_holster_priority, ref HolsterPriority_Bool);

        // Update 1.8.0
        CheckFlagBool(item.FLG_CanBePickedUpFromCorpse, ref FLG_CanBePickedUpFromCorpse_bool);
        CheckFlagBool(item.FLG_WoodenAttack, ref FLG_WoodenAttack_bool);

        CheckFlagBool(item.prerequisite_item, ref prerequisite_item_bool);
        CheckFlagBool(item.using_arm_band, ref using_arm_band_bool);
        CheckFlagBool(item.WPN_FLG_FirearmAmmo, ref WPN_FLG_FirearmAmmo_bool);
        CheckFlagBool(item.WPN_FLG_NotUsableWithTwoHand, ref WPN_FLG_NotUsableWithTwoHand_bool);
        CheckFlagBool(item.WPN_FLG_BonusAgainstShield, ref WPN_FLG_BonusAgainstShield_bool);
        CheckFlagBool(item.WPN_FLG_CanDismount, ref WPN_FLG_CanDismount_bool);

        if (item.ADD_cover_Mesh_name != null && item.ADD_cover_Mesh_name != "" && item.ADD_mat_name.Length != 0)
        {

            horseMatsList = new List<HorseManeMaterial>();
            int i = 0;
            foreach (var mat in item.ADD_mat_name)
            {
                HorseManeMaterial hrsMat = ScriptableObject.CreateInstance<HorseManeMaterial>();

                hrsMat.ID = i;

                hrsMat.matNameID = mat;
                hrsMat.meshMultiplierA = item.aDD_mat_meshMult_a[i];
                hrsMat.meshMultiplierA_prc = item.aDD_mat_meshMult_a_prc[i];
                hrsMat.meshMultiplierB = item.aDD_mat_meshMult_b[i];
                hrsMat.meshMultiplierB_prc = item.aDD_mat_meshMult_b_prc[i];

                horseMatsList.Add(hrsMat);

                i++;
            }

        }

        // if (item.ADD_mat_name.Length == 0)
        // {
        //     horseMatsList = new List<HorseManeMaterial>();
        // }

        if (item.ADD_Mesh_affected_by_cover == "true")
        {
            isCoveringMat = true;
        }
        else
        {
            isCoveringMat = false;
        }

        // merchandice

        if (item.is_merchandise == "true")
        {
            TRADE_IsMerchandise_Bool = true;
        }
        else
        {
            TRADE_IsMerchandise_Bool = false;
        }

        // Update 1.7.2
        if (item.CT_has_modifier == "true")
        {
            CT_has_modifier_bool = true;
        }
        else
        {
            CT_has_modifier_bool = false;
        }

        // Crafting pieces
        CraftingPiecesList();
    }

    private void CraftingPiecesList()
    {
        settingsAsset.CheckAndRefreshCraftingPieces();

        CP_list = new List<CraftingPiece>();

        if (item.CT_pieces_id != null)
        {
            foreach (var piece in item.CT_pieces_id)
            {
                foreach (var nt_piece in settingsAsset.NativePiecesDefinitions)
                {
                    if (nt_piece.ID == piece)
                    {
                        CP_list.Add(nt_piece);
                    }
                }
            }
        }
    }

    private void CreateOptions(ref string typeString, ref string[] options, ref int index, string[] definitions)
    {
        //WPN CLASS
        options = new string[definitions.Length];

        int i = 0;
        foreach (var data in definitions)
        {
            options[i] = data;
            // Debug.Log("");
            i++;
        }

        i = 0;
        foreach (var type in options)
        {
            if (type == typeString)
            {
                // Debug.Log("");
                index = i;
            }
            i++;
        }
    }

    private void CheckFlagBool(string itemString, ref bool stringBool)
    {
        if (itemString == "true")
        {
            stringBool = true;
        }
        else if (itemString == "false")
        {
            stringBool = false;
        }
        else if (itemString == "")
        {
            stringBool = false;
        }
    }

    public override void OnInspectorGUI()
    {


        if (settingsAsset.currentModule != item.moduleID)
        {
            isDependency = true;

            if (System.IO.File.Exists(modsSettingsPath + settingsAsset.currentModule + ".asset"))
            {
                var currModSettings = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + settingsAsset.currentModule + ".asset", typeof(ModuleReceiver));
                // Debug.Log(currModSettings.id);
                foreach (var depend in currModSettings.modDependenciesInternal)
                {
                    if (depend == item.moduleID)
                    {
                        //
                        isDependMsg = "Current Asset is used from " + " ' " + settingsAsset.currentModule
                        + " ' " + " Module as dependency. Switch to " + " ' " + item.moduleID + " ' " + " Module to edit it, or create a override asset for current module.";
                        break;
                    }
                    else
                    {
                        isDependMsg = "Switch to " + " ' " + item.moduleID + " ' " + " Module to edit it, or create asset copy for current module.";
                    }
                }
            }


            EditorGUILayout.HelpBox(isDependMsg, MessageType.Warning);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch to " + " ' " + item.moduleID + "'"))
            {
                BNDataEditorWindow window = (BNDataEditorWindow)EditorWindow.GetWindow(typeof(BNDataEditorWindow));

                if (System.IO.File.Exists(modsSettingsPath + item.moduleID + ".asset"))
                {
                    var mod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modsSettingsPath + item.moduleID + ".asset", typeof(ModuleReceiver));
                    window.source = mod;
                    window.currentMod = mod;
                    settingsAsset.currentModule = mod.id;

                    window.Repaint();
                    this.Repaint();

                    // Debug.Log("Switched to mod: " + window.currentMod);
                }

            }


            if (isDependency)
            {
                if (GUILayout.Button("Create Override Asset"))
                {
                    if (ADM_Instance == null)
                    {
                        AssetsDataManager assetMng = new AssetsDataManager();
                        assetMng.windowStateID = 1;
                        assetMng.objID = 7;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = item;

                        assetMng.assetName_org = item.id;
                        assetMng.assetName_new = item.id;
                    }
                    else
                    {
                        AssetsDataManager assetMng = ADM_Instance;
                        assetMng.windowStateID = 1;
                        assetMng.objID = 7;
                        assetMng.bdt_settings = settingsAsset;
                        assetMng.obj = item;

                        assetMng.assetName_org = item.id;
                        assetMng.assetName_new = item.id;
                    }

                    // assetMng.CopyAssetAsOverride();
                }
            }
            GUILayout.EndHorizontal();
            DrawUILine(colUILine, 3, 12);

        }
        else
        {
            isDependency = false;
        }

        // LOAD ITEM TYPES
        // Debug.Log(settingsAsset.ItemTypesDefinitions.Count);
        foreach (ItemType type in settingsAsset.ItemTypesDefinitions)
        {

            if (type.itemTypeName == item_type_options[item_type_index])
            {
                if (type.itemTypeID == "horse")
                {

                    item.IsHorse = true;
                    item.IsArmor = false;
                    item.IsWeapon = false;
                    item.IsTrade = false;
                }
                if (type.itemTypeID == "weapon")
                {
                    item.IsHorse = false;
                    item.IsArmor = false;
                    item.IsWeapon = true;
                    item.IsTrade = false;
                }
                if (type.itemTypeID == "armor")
                {
                    item.IsHorse = false;
                    item.IsArmor = true;
                    item.IsWeapon = false;
                    item.IsTrade = false;

                }
                if (type.itemTypeID == "trade")
                {
                    item.IsHorse = false;
                    item.IsArmor = false;
                    item.IsWeapon = false;
                    item.IsTrade = true;
                }
            }
        }


        // Setup Item Class Type
        EditorGUILayout.LabelField("Item Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
        item_class_index = EditorGUILayout.Popup(item_class_index, item_class_options, GUILayout.Width(128));
        if (item_class_index == 1)
        {
            item.IsCraftedItem = true;
        }
        else
        {
            item.IsCraftedItem = false;
        }
        DrawUILine(colUILine, 3, 12);

        EditorGUI.BeginDisabledGroup(isDependency);

        if (item.IsCraftedItem != true)
        {
            //! ITEM
            DrawItemEditorUI();
        }
        else
        {
            //! CRAFTED ITEM
            var originDimensions = EditorGUIUtility.labelWidth;
            // Vector2 textDimensions;

            EditorGUI.BeginDisabledGroup(true);
            item.id = EditorGUILayout.TextField("Item ID", item.id);


            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(2);

            if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
            {

                if (ADM_Instance == null)
                {
                    AssetsDataManager assetMng = new AssetsDataManager();
                    assetMng.windowStateID = 2;
                    assetMng.objID = 7;
                    assetMng.bdt_settings = settingsAsset;
                    assetMng.obj = item;

                    assetMng.assetName_org = item.id;
                    assetMng.assetName_new = item.id;
                }
                else
                {
                    AssetsDataManager assetMng = ADM_Instance;
                    assetMng.windowStateID = 2;
                    assetMng.objID = 7;
                    assetMng.bdt_settings = settingsAsset;
                    assetMng.obj = item;

                    assetMng.assetName_org = item.id;
                    assetMng.assetName_new = item.id;
                }
            }

            DrawUILine(colUILine, 3, 12);

            // 7 item
            SetLabelFieldTS(ref item.itemName, ref soloName, ref nameTranslationString, folder, translationStringName, "Item Name:", item.moduleID, item, 7, item.id);
            EditorGUILayout.Space(2);
            DrawUILine(colUILine, 3, 12);

            DrawItemCultureUI();
            EditorGUILayout.Space(2);
            // DrawUILine(colUILine, 1, 4);
            EditorGUILayout.Space(1);
            CreateAttributeToggle(ref TRADE_IsMerchandise_Bool, ref item.is_merchandise, "Is Merchandise");
            EditorGUILayout.Space(1);
            CreateAttributeToggle(ref CT_has_modifier_bool, ref item.CT_has_modifier, "Has Modifier");
            EditorGUILayout.Space(1);

            DrawUILine(colUILine, 3, 12);
            var originLabelWidth = EditorGUIUtility.labelWidth;

            if (CT_has_modifier_bool)
            {
                var textDim = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                EditorGUIUtility.labelWidth = textDim.x;

                modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                item.WPN_modifier_group = modifier_group_options[modifier_group_index];

                DrawUILine(colUILine, 3, 12);
            }

            EditorGUILayout.LabelField("Crafting Template:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

            crafting_templates_index = EditorGUILayout.Popup(crafting_templates_index, crafting_templates_options, GUILayout.Width(128));
            item.CT_crafting_template = crafting_templates_options[crafting_templates_index];
            DrawUILine(colUILine, 3, 12);

            var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Crafting Pieces Editor:  "));
            EditorGUIUtility.labelWidth = textDimensions.x;
            GUIStyle hiderStyle = new GUIStyle(EditorStyles.foldout);


            var showEditorLabel = "Hide - Crafting Pieces Editor";
            if (!showCraftingPieces)
            {
                hiderStyle.fontSize = 16;
                showEditorLabel = "Crafting Pieces Editor";
            }
            else
            {
                hiderStyle.fontSize = 10;
            }

            EditorGUI.BeginChangeCheck();
            showCraftingPieces = EditorGUILayout.Foldout(showCraftingPieces, showEditorLabel, hiderStyle);

            if (EditorGUI.EndChangeCheck())
            {
                if (showCraftingPieces)
                {
                    RefreshCraftingPieces();
                }

            }

            if (showCraftingPieces)
            {
                if (piece_type_options != null && piece_type_options.Length > 0)
                {
                    DrawUILine(colUILine, 2, 6);
                    //piece_type_options[piece_type_index] = item.CT_pieces_Type[i];

                    EditorGUILayout.BeginHorizontal();

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Crafting Pieces: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    piece_type_index = EditorGUILayout.Popup("Crafting Pieces:", piece_type_index, piece_type_options, GUILayout.Width(190));
                    //piece_type_options[piece_type_index[i]] = item.CT_pieces_Type[i];
                    //    piece_type_index[i] = EditorGUILayout.Popup(piece_type_index[i], piece_type_options, GUILayout.Width(128));
                    //    item.CT_pieces_Type[i] = piece_type_options[piece_type_index[i]];

                    GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                    buttonStyle.fontStyle = FontStyle.Bold;
                    buttonStyle.hover.textColor = Color.green;

                    // DrawUILine(colUILine, 3, 12);
                    if (GUILayout.Button((new GUIContent("Add Crafting Piece", "Add selected crafting piece for this Item")), buttonStyle, GUILayout.Width(160)))
                    {
                        //var Plist = new List<Piece>();

                        //if (item.CT_pieces_id != null && item.CT_pieces_id.Length > 0)
                        //{
                        //    int i = 0;
                        //    foreach (string pieceASST in item.CT_pieces_id)
                        //    {

                        //        Piece pieceObj = new Piece();
                        //        pieceObj.pieceName = pieceASST;
                        //        pieceObj.Type = item.CT_pieces_Type[i];
                        //        pieceObj.scale_factor = item.CT_pieces_scale_factor[i];
                        //        Plist.Add(pieceObj);
                        //        // Debug.Log(pieceObj.pieceName);
                        //        i++;
                        //    }

                        //    int indexVal = Plist.Count + 1;

                        //    item.CT_pieces_id = new string[indexVal];
                        //    item.CT_pieces_Type = new string[indexVal];
                        //    item.CT_pieces_scale_factor = new string[indexVal];

                        //    item.CT_pieces_Type[Plist.Count] = "none";
                        //}
                        //else
                        //{
                        //    item.CT_pieces_id = new string[1];
                        //    item.CT_pieces_Type = new string[1];
                        //    item.CT_pieces_scale_factor = new string[1];

                        //    item.CT_pieces_Type[0] = "none";
                        //}


                        //// Debug.Log(indexVal);
                        //int i2 = 0;
                        //foreach (var element in Plist)
                        //{
                        //    item.CT_pieces_id[i2] = element.pieceName;
                        //    item.CT_pieces_Type[i2] = element.Type;
                        //    item.CT_pieces_scale_factor[i2] = element.scale_factor;
                        //    // Debug.Log(item.CT_pieces_id[i2]);
                        //    i2++;
                        //}

                        var temp = new string[item.CT_pieces_id.Length + 1];
                        item.CT_pieces_id.CopyTo(temp, 0);
                        item.CT_pieces_id = temp;

                        item.CT_pieces_id[item.CT_pieces_id.Length - 1] = "";

                        temp = new string[item.CT_pieces_Type.Length + 1];
                        item.CT_pieces_Type.CopyTo(temp, 0);
                        item.CT_pieces_Type = temp;

                        item.CT_pieces_Type[item.CT_pieces_Type.Length - 1] = piece_type_options[piece_type_index];

                        temp = new string[item.CT_pieces_scale_factor.Length + 1];
                        item.CT_pieces_scale_factor.CopyTo(temp, 0);
                        item.CT_pieces_scale_factor = temp;

                        item.CT_pieces_scale_factor[item.CT_pieces_scale_factor.Length - 1] = "0";

                        RefreshCraftingPieces();

                        return;



                        // CraftingPiecesList();

                        // foreach (string pieceASST in item.CT_pieces_id)
                        // {
                        //     Debug.Log(pieceASST);
                        // }

                        // indexVal -= 1;

                        // item.CT_pieces_id[indexVal] = "";
                        // item.CT_pieces_Type[indexVal] = "";
                        // item.CT_pieces_scale_factor[indexVal] = "";

                    }

                    EditorGUIUtility.labelWidth = originLabelWidth;
                    EditorGUILayout.EndHorizontal();
                }

                DrawUILine(colUILine, 3, 12);


                if (item.CT_pieces_id != null)
                {
                    int i = 0;

                    GUIStyle removeButton = new GUIStyle(EditorStyles.miniButtonLeft);
                    removeButton.fontStyle = FontStyle.Bold;
                    removeButton.hover.textColor = Color.red;

                    GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
                    titleStyle.fontSize = 13;

                    foreach (var pieceAsset in item.CT_pieces_id)
                    {
                        CraftingPiece cp = null;
                        foreach (CraftingPiece piece in CP_list)
                        {
                            if (piece.ID == pieceAsset)
                            {
                                cp = piece;
                            }

                        }

                        if (cp != null)
                        {
                            var soloString = cp.craftName;
                            var TS_Name = cp.craftName;
                            Regex regex = new Regex("{=(.*)}");
                            if (regex != null)
                            {
                                var v = regex.Match(TS_Name);
                                string s = v.Groups[1].ToString();
                                TS_Name = "{=" + s + "}";
                            }

                            if (TS_Name != "" && TS_Name != "{=}")
                            {
                                soloString = soloString.Replace(TS_Name, "");
                            }

                            EditorGUILayout.LabelField(soloString + " - " + item.CT_pieces_Type[i], titleStyle, GUILayout.ExpandWidth(true));
                            EditorGUILayout.Space(4);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("None - " + item.CT_pieces_Type[i], titleStyle, GUILayout.ExpandWidth(true));
                            EditorGUILayout.Space(4);
                        }

                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();

                        object cpObj = EditorGUILayout.ObjectField(cp, typeof(CraftingPiece), true, GUILayout.Width(256));

                        cp = (CraftingPiece)cpObj;

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (cp != null && cp.piece_type == item.CT_pieces_Type[i])
                            {
                                if (cp.ID != "")
                                {
                                    item.CT_pieces_id[i] = cp.ID;
                                    CP_list.Add(cp);
                                }
                                else
                                {
                                    item.CT_pieces_id[i] = "";
                                }
                            }
                            else
                            {
                                item.CT_pieces_id[i] = "";
                                CP_list.Remove(cp);

                                Debug.LogWarning($"WARNING: You traing to add piece with {cp.piece_type} type to {item.CT_pieces_Type[i]} type holder!");
                            }
                        }


                        //if (item.CT_pieces_Type != null && item.CT_pieces_id != null )
                        //{
                        //    piece_type_options[piece_type_index[i]] = item.CT_pieces_Type[i];
                        //    piece_type_index[i] = EditorGUILayout.Popup(piece_type_index[i], piece_type_options, GUILayout.Width(128));
                        //    item.CT_pieces_Type[i] = piece_type_options[piece_type_index[i]];
                        CreateIntAttribute(ref item.CT_pieces_scale_factor[i], "Scale Factor:");
                        //}

                        if (GUILayout.Button((new GUIContent("X", "Remove Piece")), removeButton, GUILayout.Width(32)))
                        {
                            //List<Piece> pieceList = new List<Piece>();

                            //item.CT_pieces_id[i] = "_remove_piece";
                            //item.CT_pieces_Type[i] = "";
                            //item.CT_pieces_scale_factor[i] = "";

                            //int i2 = 0;
                            //foreach (string pieceASST in item.CT_pieces_id)
                            //{
                            //    if (pieceASST != "_remove_piece")
                            //    {
                            //        Piece pieceObj = new Piece();
                            //        pieceObj.pieceName = pieceASST;
                            //        pieceObj.Type = item.CT_pieces_Type[i2];
                            //        pieceObj.scale_factor = item.CT_pieces_scale_factor[i2];
                            //        pieceList.Add(pieceObj);
                            //        // Debug.Log(pieceASST);
                            //    }
                            //    i2++;
                            //}

                            //item.CT_pieces_id = new string[pieceList.Count];
                            //item.CT_pieces_Type = new string[pieceList.Count];
                            //item.CT_pieces_scale_factor = new string[pieceList.Count];

                            //int i3 = 0;
                            //foreach (Piece piece in pieceList)
                            //{
                            //    item.CT_pieces_id[i3] = piece.pieceName;
                            //    item.CT_pieces_Type[i3] = piece.Type;
                            //    item.CT_pieces_scale_factor[i3] = piece.scale_factor;
                            //    i3++;
                            //    // Debug.Log(piece.pieceName);
                            //}

                            //int index_opt = 0;
                            //foreach (var piece_type in item.CT_pieces_Type)
                            //{
                            //    CreateOptions(ref item.CT_pieces_Type[index_opt], ref piece_type_options, ref piece_type_index, settingsAsset.PieceTypesDefinitions);
                            //    index_opt++;
                            //}


                            var count = item.CT_pieces_Type.Length - 1;
                            var temp_id = new string[count];
                            var temp_type = new string[count];
                            var temp_scale_factor = new string[count];


                            int i2 = 0;
                            int i3 = 0;
                            foreach (string trg in item.CT_pieces_Type)
                            {
                                if (i3 != i)
                                {
                                    temp_id[i2] = item.CT_pieces_id[i3];
                                    temp_type[i2] = item.CT_pieces_Type[i3];
                                    temp_scale_factor[i2] = item.CT_pieces_scale_factor[i3];

                                    i2++;
                                }
                                i3++;
                            }

                            item.CT_pieces_id = temp_id;
                            item.CT_pieces_Type = temp_type;
                            item.CT_pieces_scale_factor = temp_scale_factor;


                            RefreshCraftingPieces();
                            return;
                        }
                        EditorGUILayout.EndHorizontal();

                        i++;

                        DrawUILine(colUILine, 3, 12);
                    }
                }
            }


        }

    }

    private void DrawItemEditorUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        item.id = EditorGUILayout.TextField("Item ID", item.id);


        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(2);

        if (GUILayout.Button("Edit ID", GUILayout.Width(64)))
        {

            if (ADM_Instance == null)
            {
                AssetsDataManager assetMng = new AssetsDataManager();
                assetMng.windowStateID = 2;
                assetMng.objID = 7;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = item;

                assetMng.assetName_org = item.id;
                assetMng.assetName_new = item.id;
            }
            else
            {
                AssetsDataManager assetMng = ADM_Instance;
                assetMng.windowStateID = 2;
                assetMng.objID = 7;
                assetMng.bdt_settings = settingsAsset;
                assetMng.obj = item;

                assetMng.assetName_org = item.id;
                assetMng.assetName_new = item.id;
            }
        }

        // color labels
        var labelStyle = new GUIStyle(EditorStyles.label);


        // faction name & translationString tag
        DrawUILine(colUILine, 3, 12);

        // 7 item
        SetLabelFieldTS(ref item.itemName, ref soloName, ref nameTranslationString, folder, translationStringName, "Item:", item.moduleID, item, 7, item.id);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Item Type:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

        item_type_index = EditorGUILayout.Popup(item_type_index, item_type_options, GUILayout.Width(128));

        if (EditorGUI.EndChangeCheck())
        {
            item.Type = item_type_options[item_type_index];

            if (item_type_index == 0)
            {
                item.IsArmor = false;
                item.IsWeapon = false;
                item.IsHorse = false;
            }

            if (item.IsCraftedItem)
                RefreshCraftingPieces();

        }

        DrawUILine(colUILine, 3, 12);

        DrawItemCultureUI();

        foreach (ItemType type in settingsAsset.ItemTypesDefinitions)
        {
            if (type.itemTypeName == item_type_options[item_type_index])
            {

                var originDimensions = EditorGUIUtility.labelWidth;
                Vector2 textDimensions;

                // item.Type = EditorGUILayout.TextField("Item Type:", item.Type);
                item.mesh = EditorGUILayout.TextField("Mesh Name:", item.mesh);
                DrawUILine(colUILine, 3, 12);

                CreateAttributeToggle(ref TRADE_IsMerchandise_Bool, ref item.is_merchandise, "Is Merchandise");
                EditorGUILayout.Space(1);

                if (type.itemTypeID == "armor" || type.itemTypeID == "weapon")
                {
                    CreateAttributeToggle(ref IsMultiplayer_Bool, ref item.multiplayer_item, "Is Multiplayer Item");
                    EditorGUILayout.Space(1);

                    //if (type.itemTypeID == "weapon")
                    //{

                    //}
                }

                CreateAttributeToggle(ref UseTableau_Bool, ref item.using_tableau, "Using Tableau");
                EditorGUILayout.Space(1);
                CreateAttributeToggle(ref prerequisite_item_bool, ref item.prerequisite_item, "Prerequisite item");
                EditorGUILayout.Space(1);
                CreateAttributeToggle(ref using_arm_band_bool, ref item.using_arm_band, "Using Arm Band");
                EditorGUILayout.Space(1);

                // EditorGUILayout.Space(4);
                // item.is_merchandise = EditorGUILayout.TextField("Is Merchandise", item.is_merchandise);
                // item.value = EditorGUILayout.TextField("Value", item.value);

                DrawUILine(colUILine, 3, 12);

                if (type.itemTypeID == "horse" || type.itemTypeID == "trade")
                {
                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Item Category: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item_category_index = EditorGUILayout.Popup("Item Category:", item_category_index, item_category_options, GUILayout.Width(240));
                    item.item_category = item_category_options[item_category_index];

                    DrawUILine(colUILine, 3, 12);
                }

                EditorGUILayout.BeginHorizontal();
                textDimensions = GUI.skin.label.CalcSize(new GUIContent("Value: "));
                EditorGUIUtility.labelWidth = textDimensions.x;

                int val;
                int.TryParse(item.value, out val);
                val = EditorGUILayout.IntField("Value:", val, GUILayout.MaxWidth(162));
                item.value = val.ToString();
                // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                DrawUILineVertical(colUILine, 1, 1, 16);

                textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Weight: "));
                EditorGUIUtility.labelWidth = textDimensions.x;

                int wght;
                int.TryParse(item.weight, out wght);
                wght = EditorGUILayout.IntField(" Weight:", wght, GUILayout.MaxWidth(162));
                item.weight = wght.ToString();
                // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                DrawUILineVertical(colUILine, 1, 1, 16);

                textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Appearance: "));
                EditorGUIUtility.labelWidth = textDimensions.x;

                int app;
                int.TryParse(item.appearance, out app);
                app = EditorGUILayout.IntField(" Appearance:", app, GUILayout.MaxWidth(162));
                item.appearance = app.ToString();
                // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                DrawUILineVertical(colUILine, 1, 1, 16);

                textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Difficulty: "));
                EditorGUIUtility.labelWidth = textDimensions.x;

                int diff;
                int.TryParse(item.difficulty, out diff);
                diff = EditorGUILayout.IntField(" Difficulty:", diff, GUILayout.MaxWidth(162));
                item.difficulty = diff.ToString();
                // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                DrawUILineVertical(colUILine, 1, 1, 16);
                EditorGUILayout.EndHorizontal();

                // DrawUILine(colUILine, 3, 12);


                // CreateAttributeToggle(ref IsMultiplayer_Bool, ref item.multiplayer_item, "Is Multiplayer Item");
                // EditorGUILayout.Space(1);
                // CreateAttributeToggle(ref UseTableau_Bool, ref item.using_tableau, "Using Tableau");
                // // EditorGUILayout.Space(4);

                DrawUILine(colUILine, 3, 12);

                if (type.itemTypeID != "trade")
                {

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Subtype: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    subtypes_index = EditorGUILayout.Popup("Subtype:", subtypes_index, subtypes_options, GUILayout.Width(240));
                    item.subtype = subtypes_options[subtypes_index];

                    if (type.itemTypeID != "armor")
                    {
                        DrawUILine(colUILine, 3, 12);
                    }

                }


                if (type.itemTypeID == "armor")
                {
                    EditorGUILayout.Space(4);
                    CreateIntAttribute(ref item.lod_atlas_index, "LOD Atlas Index:");
                    DrawUILine(colUILine, 3, 12);
                }


                if (type.itemTypeID == "weapon")
                {

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Body Name:"));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.body_name = EditorGUILayout.TextField("Body Name:", item.body_name);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Shield Body Name: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.shield_body_name = EditorGUILayout.TextField("Shield Body Name:", item.shield_body_name);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Flying Mesh: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.flying_mesh = EditorGUILayout.TextField("Flying Mesh:", item.flying_mesh);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Prefab: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.prefab = EditorGUILayout.TextField("Prefab:", item.prefab);
                    EditorGUILayout.Space(1);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Ammo Offset: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    ammoOffset = EditorGUILayout.Vector3Field(" Ammo Offset:", ammoOffset, GUILayout.MaxWidth(352));

                    string amm_offst = ammoOffset.x + "," + ammoOffset.y + "," + ammoOffset.z;
                    item.AmmoOffset = amm_offst;

                    DrawUILine(colUILine, 3, 12);

                    CreateAttributeToggle(ref HolsterPriority_Bool, ref item.has_lower_holster_priority, "Has Lower Holster Priority");
                    EditorGUILayout.Space(2);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Item Holsters: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.item_holsters = EditorGUILayout.TextField("Item Holsters:", item.item_holsters);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Holster Body Name: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.holster_body_name = EditorGUILayout.TextField("Holster Body Name:", item.holster_body_name);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Holster Name: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    item.holster_mesh = EditorGUILayout.TextField("Holster Name:", item.holster_mesh);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Holster Mesh With Weapon: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;
                    item.holster_mesh_with_weapon = EditorGUILayout.TextField("Holster Mesh With Weapon:", item.holster_mesh_with_weapon);
                    // item.has_lower_holster_priority = EditorGUILayout.TextField("has_lower_holster_priority:", item.has_lower_holster_priority);
                    EditorGUILayout.Space(1);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Holster Position Shift: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;

                    HolsterPosition = EditorGUILayout.Vector3Field(" Holster Position Shift:", HolsterPosition, GUILayout.MaxWidth(352));
                    string hol_pos = HolsterPosition.x + "," + HolsterPosition.y + "," + HolsterPosition.z;
                    item.holster_position_shift = hol_pos;
                    DrawUILine(colUILine, 3, 12);
                }



                // item.mesh = EditorGUILayout.TextField("Mesh Name:", item.mesh);


                // Draw ARMOR EDITOR

                if (type.itemTypeID == "trade")
                {

                    CreateAttributeToggle(ref TRADE_IsFood_Bool, ref item.IsFood, "Is Food");
                    EditorGUILayout.Space(1);

                    CreateAttributeToggle(ref FLG_Civilian_Bool, ref item.FLG_Civilian, "Is Civilian");
                    EditorGUILayout.Space(1);

                    DrawUILine(colUILine, 3, 12);

                    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Morale Bonus: "));
                    EditorGUIUtility.labelWidth = textDimensions.x;
                    int moral_b;
                    int.TryParse(item.TRADE_morale_bonus, out moral_b);
                    moral_b = EditorGUILayout.IntField("Morale Bonus:", moral_b, GUILayout.MaxWidth(162));
                    item.TRADE_morale_bonus = moral_b.ToString();
                }

                if (type.itemTypeID == "horse")
                {
                    // FLG_Civilian_Bool FLAGS
                    CreateAttributeToggle(ref FLG_Civilian_Bool, ref item.FLG_Civilian, "Is Civilian");
                    EditorGUILayout.Space(1);
                    DrawUILine(colUILine, 3, 12);

                    if (item.Type == "Horse" /* || item.Type == "Animal"*/)
                    {
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Monster: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        monster_index = EditorGUILayout.Popup("Monster: ", monster_index, monster_options, GUILayout.Width(256));
                        item.HRS_monster = "Monster." + monster_options[monster_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // FLG_Civilian_Bool FLAGS
                        CreateAttributeToggle(ref HRS_IsMountable_Bool, ref item.HRS_is_mountable, "Is Mountable");
                        EditorGUILayout.Space(1);
                        // FLG_UseTeamColor_Bool FLAGS
                        CreateAttributeToggle(ref HRS_IsPackAnimal_Bool, ref item.HRS_is_pack_animal, "Is Pack Animal");
                        EditorGUILayout.Space(1);
                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Skeleton Scale: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        HRS_skeletons_index = EditorGUILayout.Popup("Skeleton Scale:", HRS_skeletons_index, HRS_skeletons_options, GUILayout.Width(240));
                        item.HRS_skeleton_scale = HRS_skeletons_options[HRS_skeletons_index];

                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Default Mane Mesh: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        item.HRS_mane_mesh = EditorGUILayout.TextField("Default Mane Mesh:", item.HRS_mane_mesh);
                        DrawUILine(colUILine, 3, 12);


                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Maneuver: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int manuv;
                        int.TryParse(item.HRS_maneuver, out manuv);
                        manuv = EditorGUILayout.IntField("Maneuver:", manuv, GUILayout.MaxWidth(162));
                        item.HRS_maneuver = manuv.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Lenght: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int bl;
                        int.TryParse(item.HRS_body_length, out bl);
                        bl = EditorGUILayout.IntField("Body Lenght:", bl, GUILayout.MaxWidth(162));
                        item.HRS_body_length = bl.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Charge Damage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int chd;
                        int.TryParse(item.HRS_charge_damage, out chd);
                        chd = EditorGUILayout.IntField("Charge Damage:", chd, GUILayout.MaxWidth(162));
                        item.HRS_charge_damage = chd.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space(6);

                        EditorGUILayout.BeginHorizontal();

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Speed: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int spd;
                        int.TryParse(item.HRS_speed, out spd);
                        spd = EditorGUILayout.IntField("Speed:", spd, GUILayout.MaxWidth(162));
                        item.HRS_speed = spd.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Lenght: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int extra_hp;
                        int.TryParse(item.HRS_extra_health, out extra_hp);
                        extra_hp = EditorGUILayout.IntField("Extra Health:", extra_hp, GUILayout.MaxWidth(162));
                        item.HRS_extra_health = extra_hp.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        if (item.HRS_mane_mesh == "")
                        {
                            EditorGUILayout.LabelField("Aditional Mesh:", EditorStyles.boldLabel);
                            item.ADD_Mesh = EditorGUILayout.TextField(item.ADD_Mesh, GUILayout.MaxWidth(256));


                            EditorGUILayout.LabelField("Mane Mesh:", EditorStyles.boldLabel);
                            EditorGUILayout.BeginHorizontal();
                            item.ADD_cover_Mesh_name = EditorGUILayout.TextField(item.ADD_cover_Mesh_name, GUILayout.MaxWidth(256));
                            DrawUILineVertical(colUILine, 1, 1, 16);
                            DrawUILineVertical(colUILine, 0, 0, 16);

                            CreateAttributeToggle(ref isCoveringMat, ref item.ADD_Mesh_affected_by_cover, "Affected by Cover");

                            EditorGUILayout.EndHorizontal();


                            if (isCoveringMat && item.ADD_cover_Mesh_name != "")
                            {
                                DrawUILine(colUILine, 3, 12);
                                GUIStyle removeButton = new GUIStyle(EditorStyles.miniButtonLeft);
                                removeButton.fontStyle = FontStyle.Bold;
                                removeButton.hover.textColor = Color.red;

                                GUIStyle mediumLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                                // removeButton.fontStyle = FontStyle.Bold;
                                mediumLabelStyle.normal.textColor = Color.gray;
                                mediumLabelStyle.fontSize = 13;

                                if (horseMatsList != null)
                                {
                                    int i = 0;
                                    EditorGUILayout.LabelField(" Cover Materials:", mediumLabelStyle);
                                    foreach (var mat in horseMatsList)
                                    {
                                        showMaterials[mat.ID] = EditorGUILayout.Foldout(showMaterials[mat.ID], mat.matNameID, EditorStyles.foldoutHeader);

                                        // Debug.Log(mat.ID);

                                        if (showMaterials[mat.ID])
                                        {
                                            DrawUILine(colUILine, 3, 12);


                                            EditorGUILayout.LabelField("Material:", EditorStyles.boldLabel);
                                            EditorGUILayout.BeginHorizontal();
                                            mat.matNameID = EditorGUILayout.TextField(mat.matNameID, GUILayout.MaxWidth(256));


                                            if (GUILayout.Button((new GUIContent("X", "Remove Material")), removeButton, GUILayout.Width(32)))
                                            {

                                                horseMatsList.Remove(mat);

                                                if (horseMatsList.Count != 0)
                                                {
                                                    item.ADD_mat_name = new string[horseMatsList.Count];
                                                    item.aDD_mat_meshMult_a = new string[horseMatsList.Count];
                                                    item.aDD_mat_meshMult_a_prc = new string[horseMatsList.Count];
                                                    item.aDD_mat_meshMult_b = new string[horseMatsList.Count];
                                                    item.aDD_mat_meshMult_b_prc = new string[horseMatsList.Count];

                                                    int idNum = 0;
                                                    foreach (var matFromList in horseMatsList)
                                                    {
                                                        matFromList.ID = idNum;

                                                        item.ADD_mat_name[idNum] = matFromList.matNameID;
                                                        item.aDD_mat_meshMult_a[idNum] = matFromList.meshMultiplierA;
                                                        item.aDD_mat_meshMult_a_prc[idNum] = matFromList.meshMultiplierA_prc;
                                                        item.aDD_mat_meshMult_b[idNum] = matFromList.meshMultiplierB;
                                                        item.aDD_mat_meshMult_b_prc[idNum] = matFromList.meshMultiplierB_prc;

                                                        idNum++;
                                                    }
                                                }
                                                else
                                                {
                                                    item.ADD_mat_name = new string[0];
                                                    item.aDD_mat_meshMult_a = new string[0];
                                                    item.aDD_mat_meshMult_a_prc = new string[0];
                                                    item.aDD_mat_meshMult_b = new string[0];
                                                    item.aDD_mat_meshMult_b_prc = new string[0];
                                                }

                                            }

                                            EditorGUILayout.EndHorizontal();

                                            EditorGUILayout.Space(6);

                                            CreateColorLabel(ref mat.meshMultiplierA, ref matColorsA[i], "Mesh Multiplier 1:");

                                            float prc;
                                            float.TryParse(mat.meshMultiplierA_prc, out prc);

                                            if (prc == 0)
                                            {
                                                EditorGUILayout.LabelField("Percentage 1: (Unused)");
                                            }
                                            else
                                            {
                                                EditorGUILayout.LabelField("Percentage 1:");
                                            }

                                            // EditorGUILayout.LabelField("Percentage 1:");
                                            prc = EditorGUILayout.Slider(prc, 0, 1);
                                            mat.meshMultiplierA_prc = prc.ToString();

                                            DrawUILine(colUILine, 1, 6);

                                            CreateColorLabel(ref mat.meshMultiplierB, ref matColorsB[i], "Mesh Multiplier 2:");

                                            float prc2;
                                            float.TryParse(mat.meshMultiplierB_prc, out prc2);

                                            if (prc2 == 0)
                                            {
                                                EditorGUILayout.LabelField("Percentage 2: (Unused)");
                                            }
                                            else
                                            {
                                                EditorGUILayout.LabelField("Percentage 2:");
                                            }

                                            prc2 = EditorGUILayout.Slider(prc2, 0, 1);
                                            mat.meshMultiplierB_prc = prc2.ToString();


                                            // DrawUILine(colUILine, 3, 12);

                                            if (item.ADD_mat_name.Length != 0)
                                            {
                                                item.ADD_mat_name[i] = mat.matNameID;
                                                item.aDD_mat_meshMult_a[i] = mat.meshMultiplierA;
                                                item.aDD_mat_meshMult_a_prc[i] = mat.meshMultiplierA_prc;
                                                item.aDD_mat_meshMult_b[i] = mat.meshMultiplierB;
                                                item.aDD_mat_meshMult_b_prc[i] = mat.meshMultiplierB_prc;
                                            }

                                            i++;


                                        }
                                    }
                                }

                                if ((horseMatsList == null || horseMatsList.Count < 4) && isCoveringMat)
                                {
                                    DrawUILine(colUILine, 3, 12);
                                    if (GUILayout.Button("Add New Mane Material"))
                                    {
                                        if (horseMatsList == null)
                                        {
                                            horseMatsList = new List<HorseManeMaterial>();
                                        }

                                        HorseManeMaterial mat = new HorseManeMaterial();
                                        mat.ID = horseMatsList.Count;
                                        mat.matNameID = "New_Material";
                                        horseMatsList.Add(mat);

                                        item.ADD_mat_name = new string[horseMatsList.Count];
                                        item.aDD_mat_meshMult_a = new string[horseMatsList.Count];
                                        item.aDD_mat_meshMult_a_prc = new string[horseMatsList.Count];
                                        item.aDD_mat_meshMult_b = new string[horseMatsList.Count];
                                        item.aDD_mat_meshMult_b_prc = new string[horseMatsList.Count];

                                        item.ADD_mat_name[mat.ID] = mat.matNameID;
                                    }
                                }

                            }


                        }

                    }
                    else if (item.Type == "Animal")
                    {
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Monster: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        monster_index = EditorGUILayout.Popup("Monster: ", monster_index, monster_options, GUILayout.Width(256));
                        item.HRS_monster = "Monster." + monster_options[monster_index];
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Maneuver: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int manuv;
                        int.TryParse(item.HRS_maneuver, out manuv);
                        manuv = EditorGUILayout.IntField("Maneuver:", manuv, GUILayout.MaxWidth(162));
                        item.HRS_maneuver = manuv.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Speed: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int spd;
                        int.TryParse(item.HRS_speed, out spd);
                        spd = EditorGUILayout.IntField("Speed:", spd, GUILayout.MaxWidth(162));
                        item.HRS_speed = spd.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Charge Damage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int chd;
                        int.TryParse(item.HRS_charge_damage, out chd);
                        chd = EditorGUILayout.IntField("Charge Damage:", chd, GUILayout.MaxWidth(162));
                        item.HRS_charge_damage = chd.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Lenght: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int bl;
                        int.TryParse(item.HRS_body_length, out bl);
                        bl = EditorGUILayout.IntField("Body Lenght:", bl, GUILayout.MaxWidth(162));
                        item.HRS_body_length = bl.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // item.HRS_maneuver = EditorGUILayout.TextField(" Maneuver:", item.HRS_maneuver, GUILayout.MaxWidth(162));
                        // item.HRS_speed = EditorGUILayout.TextField(" Speed:", item.HRS_speed, GUILayout.MaxWidth(162));
                        // item.HRS_charge_damage = EditorGUILayout.TextField(" Charge Damage:", item.HRS_charge_damage, GUILayout.MaxWidth(162));
                        // item.HRS_body_length = EditorGUILayout.TextField(" Body Length:", item.HRS_body_length, GUILayout.MaxWidth(162));
                    }
                }
                if (type.itemTypeID == "armor")
                {
                    // FLG_Civilian_Bool FLAGS
                    CreateAttributeToggle(ref FLG_Civilian_Bool, ref item.FLG_Civilian, "Is Civilian");
                    EditorGUILayout.Space(1);
                    // FLG_UseTeamColor_Bool FLAGS
                    CreateAttributeToggle(ref FLG_UseTeamColor_Bool, ref item.FLG_UseTeamColor, "Use Team Color");
                    EditorGUILayout.Space(1);
                    // FLG_DoesNotHideChest_Bool FLAGS
                    CreateAttributeToggle(ref FLG_DoesNotHideChest_Bool, ref item.FLG_DoesNotHideChest, "Does Not Hide Chest");
                    EditorGUILayout.Space(1);

                    DrawUILine(colUILine, 3, 12);

                    if (item.Type == "BodyArmor")
                    {

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Material Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        material_type_index = EditorGUILayout.Popup("Material Type: ", material_type_index, material_type_options, GUILayout.Width(256));
                        item.ARMOR_material_type = material_type_options[material_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // LargeShield FLAGS
                        CreateAttributeToggle(ref ARMOR_isCoverBody_Bool, ref item.ARMOR_covers_body, "Covers Body");
                        EditorGUILayout.Space(1);
                        // LargeShield FLAGS
                        CreateAttributeToggle(ref ARMOR_hasGenderVariations_Bool, ref item.ARMOR_has_gender_variations, "Has Gender Variations");
                        EditorGUILayout.Space(1);

                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Body Mesh Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        body_mesh_type_index = EditorGUILayout.Popup("Body Mesh Type:", body_mesh_type_index, body_mesh_type_options, GUILayout.Width(240));
                        item.ARMOR_body_mesh_type = body_mesh_type_options[body_mesh_type_index];

                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Body Armor:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ba;
                        int.TryParse(item.ARMOR_body_armor, out ba);
                        ba = EditorGUILayout.IntField(" Body Armor:", ba, GUILayout.MaxWidth(162));
                        item.ARMOR_body_armor = ba.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        int ha;
                        int.TryParse(item.ARMOR_head_armor, out ha);
                        ha = EditorGUILayout.IntField(" Head Armor:", ha, GUILayout.MaxWidth(162));
                        item.ARMOR_head_armor = ha.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        int hnd_a;
                        int.TryParse(item.ARMOR_arm_armor, out hnd_a);
                        hnd_a = EditorGUILayout.IntField(" Hand Armor:", hnd_a, GUILayout.MaxWidth(162));
                        item.ARMOR_arm_armor = hnd_a.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Leg Armor:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        int la;
                        int.TryParse(item.ARMOR_leg_armor, out la);
                        la = EditorGUILayout.IntField(" Leg Armor:", la, GUILayout.MaxWidth(162));
                        item.ARMOR_leg_armor = la.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // item.ARMOR_body_mesh_type = EditorGUILayout.TextField("Body Mesh Type:", item.ARMOR_body_mesh_type, GUILayout.MaxWidth(256));


                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    else if (item.Type == "HeadArmor" || item.Type == "headArmor")
                    {
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Material Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        material_type_index = EditorGUILayout.Popup("Material Type: ", material_type_index, material_type_options, GUILayout.Width(256));
                        item.ARMOR_material_type = material_type_options[material_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // ARMOR_hasGenderVariations_Bool FLAGS
                        CreateAttributeToggle(ref ARMOR_hasGenderVariations_Bool, ref item.ARMOR_has_gender_variations, "Has Gender Variations");
                        EditorGUILayout.Space(1);
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Hair Cover Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        hair_cover_type_index = EditorGUILayout.Popup("Hair Cover Type:", hair_cover_type_index, hair_cover_type_options, GUILayout.Width(240));
                        item.ARMOR_hair_cover_type = hair_cover_type_options[hair_cover_type_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Beard Cover Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        beard_cover_type_index = EditorGUILayout.Popup("Beard Cover Type: ", beard_cover_type_index, beard_cover_type_options, GUILayout.Width(256));
                        item.ARMOR_beard_cover_type = beard_cover_type_options[beard_cover_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Head Armor: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        int ha;
                        int.TryParse(item.ARMOR_head_armor, out ha);
                        ha = EditorGUILayout.IntField("Head Armor:", ha, GUILayout.MaxWidth(162));
                        item.ARMOR_head_armor = ha.ToString();

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    else if (item.Type == "LegArmor")
                    {
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Material Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        material_type_index = EditorGUILayout.Popup("Material Type: ", material_type_index, material_type_options, GUILayout.Width(256));
                        item.ARMOR_material_type = material_type_options[material_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // ARMOR_hasGenderVariations_Bool FLAGS
                        CreateAttributeToggle(ref ARMOR_isCoverLegs_Bool, ref item.ARMOR_covers_legs, "Covers Legs");
                        EditorGUILayout.Space(1);
                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Leg Armor: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        int la;
                        int.TryParse(item.ARMOR_leg_armor, out la);
                        la = EditorGUILayout.IntField("Leg Armor:", la, GUILayout.MaxWidth(162));
                        item.ARMOR_leg_armor = la.ToString();

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    else if (item.Type == "HandArmor")
                    {
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Material Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        material_type_index = EditorGUILayout.Popup("Material Type: ", material_type_index, material_type_options, GUILayout.Width(256));
                        item.ARMOR_material_type = material_type_options[material_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // ARMOR_hasGenderVariations_Bool FLAGS
                        CreateAttributeToggle(ref ARMOR_isCoverHands_Bool, ref item.ARMOR_covers_hands, "Covers Hands");
                        EditorGUILayout.Space(1);
                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Hand Armor: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        int aa;
                        int.TryParse(item.ARMOR_arm_armor, out aa);
                        aa = EditorGUILayout.IntField("Hand Armor:", aa, GUILayout.MaxWidth(162));
                        item.ARMOR_arm_armor = aa.ToString();

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    else if (item.Type == "Cape")
                    {
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Material Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        material_type_index = EditorGUILayout.Popup("Material Type: ", material_type_index, material_type_options, GUILayout.Width(256));
                        item.ARMOR_material_type = material_type_options[material_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Body Armor:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ba;
                        int.TryParse(item.ARMOR_body_armor, out ba);
                        ba = EditorGUILayout.IntField(" Body Armor:", ba, GUILayout.MaxWidth(162));
                        item.ARMOR_body_armor = ba.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        int hnd_a;
                        int.TryParse(item.ARMOR_arm_armor, out hnd_a);
                        hnd_a = EditorGUILayout.IntField(" Hand Armor:", hnd_a, GUILayout.MaxWidth(162));
                        item.ARMOR_arm_armor = hnd_a.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);
                    }
                    else if (item.Type == "HorseHarness")
                    {
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Material Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        material_type_index = EditorGUILayout.Popup("Material Type: ", material_type_index, material_type_options, GUILayout.Width(256));
                        item.ARMOR_material_type = material_type_options[material_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // ARMOR_hasGenderVariations_Bool FLAGS
                        CreateAttributeToggle(ref ARMOR_isCoverHead_Bool, ref item.ARMOR_covers_head, "Covers Head");
                        EditorGUILayout.Space(1);
                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Body Armor:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ba;
                        int.TryParse(item.ARMOR_body_armor, out ba);
                        ba = EditorGUILayout.IntField("Body Armor:", ba, GUILayout.MaxWidth(162));
                        item.ARMOR_body_armor = ba.ToString();
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Hair Cover Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        hair_cover_type_index = EditorGUILayout.Popup("Hair Cover Type:", hair_cover_type_index, hair_cover_type_options, GUILayout.Width(178));
                        item.ARMOR_hair_cover_type = hair_cover_type_options[hair_cover_type_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Mane Cover Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        mane_cover_type_index = EditorGUILayout.Popup("Mane Cover Type: ", mane_cover_type_index, mane_cover_type_options, GUILayout.Width(178));
                        item.ARMOR_mane_cover_type = mane_cover_type_options[mane_cover_type_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Family Type: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        family_type_index = EditorGUILayout.Popup("Family Type: ", family_type_index, family_type_options, GUILayout.Width(148));
                        item.ARMOR_family_type = family_type_options[family_type_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        item.ARMOR_reins_mesh = EditorGUILayout.TextField("Reins Mesh:", item.ARMOR_reins_mesh, GUILayout.MaxWidth(320));
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Maneuver Bonus: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int mb;
                        int.TryParse(item.ARMOR_maneuver_bonus, out mb);
                        mb = EditorGUILayout.IntField("Maneuver Bonus:", mb, GUILayout.MaxWidth(162));
                        item.ARMOR_maneuver_bonus = mb.ToString();
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Speed Bonus: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int sb;
                        int.TryParse(item.ARMOR_speed_bonus, out sb);
                        sb = EditorGUILayout.IntField("Speed Bonus:", sb, GUILayout.MaxWidth(162));
                        item.ARMOR_speed_bonus = sb.ToString();
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Charge Bonus: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ch_b;
                        int.TryParse(item.ARMOR_charge_bonus, out ch_b);
                        ch_b = EditorGUILayout.IntField("Charge Bonus:", ch_b, GUILayout.MaxWidth(162));
                        item.ARMOR_charge_bonus = ch_b.ToString();
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();

                        // item.ARMOR_maneuver_bonus = EditorGUILayout.TextField("Maneuver Bonus:", item.ARMOR_maneuver_bonus, GUILayout.MaxWidth(256));
                        // item.ARMOR_speed_bonus = EditorGUILayout.TextField("Speed Bonus:", item.ARMOR_speed_bonus, GUILayout.MaxWidth(256));
                        // item.ARMOR_charge_bonus = EditorGUILayout.TextField("Charge Bonus:", item.ARMOR_charge_bonus, GUILayout.MaxWidth(256));

                        EditorGUIUtility.labelWidth = originDimensions;
                    }

                }

                ///
                // ? WEAPONS EDITOR

                else if (type.itemTypeID == "weapon")
                {
                    // FLG_WoodenParry_Bool FLAGS
                    CreateAttributeToggle(ref FLG_WoodenParry_Bool, ref item.FLG_WoodenParry, "Wooden Parry");
                    EditorGUILayout.Space(1);
                    // FLG_ForceAttachOffHandPrimaryItemBone_Bool FLAGS
                    CreateAttributeToggle(ref FLG_ForceAttachOffHandPrimaryItemBone_Bool, ref item.FLG_ForceAttachOffHandPrimaryItemBone, "Force Attach Off Hand Primary Item Bone");
                    EditorGUILayout.Space(1);
                    // FLG_DropOnWeaponChange_Bool FLAGS
                    CreateAttributeToggle(ref FLG_DropOnWeaponChange_Bool, ref item.FLG_DropOnWeaponChange, "Drop On Weapon Change");
                    EditorGUILayout.Space(1);
                    // FLG_HeldInOffHand_Bool FLAGS
                    CreateAttributeToggle(ref FLG_HeldInOffHand_Bool, ref item.FLG_HeldInOffHand, "Held In Off Hand");
                    EditorGUILayout.Space(1);
                    // FLG_HasToBeHeldUp_Bool FLAGS
                    CreateAttributeToggle(ref FLG_HasToBeHeldUp_Bool, ref item.FLG_HasToBeHeldUp, "Has To Be Held Up");
                    EditorGUILayout.Space(1);
                    // FLG_DoNotScaleBodyAccordingToWeaponLength_Bool FLAGS
                    CreateAttributeToggle(ref FLG_DoNotScaleBodyAccordingToWeaponLength_Bool, ref item.FLG_DoNotScaleBodyAccordingToWeaponLength, "Do Not Scale Body According To Weapon Length");
                    EditorGUILayout.Space(1);
                    // FLG_ForceAttachOffHandSecondaryItemBone_Bool FLAGS
                    CreateAttributeToggle(ref FLG_ForceAttachOffHandSecondaryItemBone_Bool, ref item.FLG_ForceAttachOffHandSecondaryItemBone, "Force Attach Off Hand Secondary Item Bone");
                    EditorGUILayout.Space(1);
                    // FLG_QuickFadeOut_Bool FLAGS
                    CreateAttributeToggle(ref FLG_QuickFadeOut_Bool, ref item.FLG_QuickFadeOut, "Quick Fade Out");
                    EditorGUILayout.Space(1);
                    // FLG_CannotBePickedUp_Bool FLAGS
                    CreateAttributeToggle(ref FLG_CannotBePickedUp_Bool, ref item.FLG_CannotBePickedUp, "Cannot Be Picked Up");
                    EditorGUILayout.Space(1);
                    // FLG_DropOnAnyAction_Bool FLAGS
                    CreateAttributeToggle(ref FLG_DropOnAnyAction_Bool, ref item.FLG_DropOnAnyAction, "Drop On Any Action");
                    EditorGUILayout.Space(1);
                    // FLG_WoodenParry_Bool FLAGS
                    CreateAttributeToggle(ref FLG_CanBePickedUpFromCorpse_bool, ref item.FLG_CanBePickedUpFromCorpse, "Can Be Picked Up From Corpse");
                    EditorGUILayout.Space(1);
                    // FLG_WoodenParry_Bool FLAGS
                    CreateAttributeToggle(ref FLG_WoodenAttack_bool, ref item.FLG_WoodenAttack, "Wooden Attack");
                    EditorGUILayout.Space(1);
                    CreateAttributeToggle(ref WPN_FLG_FirearmAmmo_bool, ref item.WPN_FLG_FirearmAmmo, "Firearm Ammo");
                    EditorGUILayout.Space(1);
                    CreateAttributeToggle(ref WPN_FLG_NotUsableWithTwoHand_bool, ref item.WPN_FLG_NotUsableWithTwoHand, "Not Usable With Two Hand");
                    EditorGUILayout.Space(1);
                    CreateAttributeToggle(ref WPN_FLG_BonusAgainstShield_bool, ref item.WPN_FLG_BonusAgainstShield, "Bonus Against Shield");
                    EditorGUILayout.Space(1);
                    CreateAttributeToggle(ref WPN_FLG_CanDismount_bool, ref item.WPN_FLG_CanDismount, "Can Dismount");
                    EditorGUILayout.Space(1);
                    DrawUILine(colUILine, 3, 12);

                    // SHIELD EDITOR
                    if (item.WPN_weapon_class == "LargeShield")
                    {

                        //
                        // DrawUILine(colUILine, 3, 12);
                        EditorGUILayout.LabelField("Weapon Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("---------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        // item.WPN_weapon_class = EditorGUILayout.TextField(" Weapon Class:", item.WPN_weapon_class, GUILayout.MaxWidth(256));
                        weapon_class_index = EditorGUILayout.Popup(weapon_class_index, weapon_class_options, GUILayout.Width(162));

                        item.WPN_weapon_class = weapon_class_options[weapon_class_index];

                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];
                        DrawUILine(colUILine, 3, 12);

                        // LargeShield FLAGS
                        CreateAttributeToggle(ref WPN_FLG_CanBlockRanged_Bool, ref item.WPN_FLG_CanBlockRanged, "Can Block Ranged");
                        EditorGUILayout.Space(1);

                        // hit points flags
                        CreateAttributeToggle(ref WPN_FLG_HasHitPoints_Bool, ref item.WPN_FLG_HasHitPoints, "Hit Points");
                        EditorGUILayout.Space(1);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Shield Down Length: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        var dwn = 0.0f;
                        float.TryParse(item.WPN_shield_down_length, out dwn);
                        item.WPN_shield_down_length = EditorGUILayout.FloatField("Shield Down Length", dwn, GUILayout.Width(180)).ToString();
                        EditorGUILayout.Space(1);

                        var wdt = 0.0f;
                        float.TryParse(item.WPN_shield_width, out wdt);
                        item.WPN_shield_width = EditorGUILayout.FloatField("Shield Width", wdt, GUILayout.Width(180)).ToString();
                        EditorGUILayout.Space(1);

                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Physics Material: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        // item.WPN_physics_material = EditorGUILayout.TextField(" Physics Material:", item.WPN_physics_material, GUILayout.MaxWidth(256));
                        physics_mats_index = EditorGUILayout.Popup(" Physics Material:", physics_mats_index, physics_mats_options, GUILayout.Width(240));
                        item.WPN_physics_material = physics_mats_options[physics_mats_index];

                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Item Usage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        // item.WPN_item_usage = EditorGUILayout.TextField(" Item Usage:", item.WPN_item_usage, GUILayout.MaxWidth(256));
                        item_usage_index = EditorGUILayout.Popup(" Item Usage: ", item_usage_index, item_usage_options, GUILayout.Width(256));
                        item.WPN_item_usage = item_usage_options[item_usage_index];


                        EditorGUILayout.EndHorizontal();

                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Center of Mass V3: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        posVec3 = EditorGUILayout.Vector3Field(" Position:", posVec3, GUILayout.MaxWidth(352));
                        DrawUILine(colUILine, 1, 6);
                        rotVec3 = EditorGUILayout.Vector3Field(" Rotation:", rotVec3, GUILayout.MaxWidth(352));
                        DrawUILine(colUILine, 1, 6);
                        centerMassVec3 = EditorGUILayout.Vector3Field(" Center of Mass:", centerMassVec3, GUILayout.MaxWidth(352));
                        // DrawUILine(colUILine, 1, 4);

                        string pos = posVec3.x + "," + posVec3.y + "," + posVec3.z;
                        item.WPN_position = pos;
                        string rot = rotVec3.x + "," + rotVec3.y + "," + rotVec3.z;
                        item.WPN_rotation = rot;
                        string centerMass = centerMassVec3.x + "," + centerMassVec3.y + "," + centerMassVec3.z;
                        item.WPN_center_of_mass = centerMass;

                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Body Armor:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ba;
                        int.TryParse(item.ARMOR_body_armor, out ba);
                        ba = EditorGUILayout.IntField("Body Armor:", ba, GUILayout.MaxWidth(162));
                        item.ARMOR_body_armor = ba.ToString();
                        // item.ARMOR_body_armor = EditorGUILayout.TextField(" Body Armor:", item.ARMOR_body_armor, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Speed Rating:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int sr;
                        int.TryParse(item.WPN_speed_rating, out sr);
                        sr = EditorGUILayout.IntField("Speed Rating:", sr, GUILayout.MaxWidth(162));
                        item.WPN_speed_rating = sr.ToString();
                        // item.WPN_speed_rating = EditorGUILayout.TextField(" Speed Rating:", item.WPN_speed_rating, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Weapon Lenght:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int wl;
                        int.TryParse(item.WPN_weapon_length, out wl);
                        wl = EditorGUILayout.IntField("Weapon Lenght:", wl, GUILayout.MaxWidth(162));
                        item.WPN_weapon_length = wl.ToString();
                        // item.WPN_weapon_length = EditorGUILayout.TextField(" Weapon Lenght:", item.WPN_weapon_length, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        if (WPN_FLG_HasHitPoints_Bool)
                        {
                            textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Hit Points:"));
                            EditorGUIUtility.labelWidth = textDimensions.x;

                            int hp;
                            int.TryParse(item.WPN_hit_points, out hp);
                            hp = EditorGUILayout.IntField("Hit Points:", hp, GUILayout.MaxWidth(162));
                            item.WPN_hit_points = hp.ToString();
                            // item.WPN_hit_points = EditorGUILayout.TextField(" Hit Points:", item.WPN_hit_points, GUILayout.MaxWidth(256));
                            DrawUILineVertical(colUILine, 1, 1, 16);
                        }
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // Thrust damage
                        EditorGUILayout.LabelField("Thrust Damage Type:", EditorStyles.boldLabel);
                        thrust_type_index = EditorGUILayout.Popup(thrust_type_index, thrust_type_options, GUILayout.Width(128));
                        item.WPN_thrust_damage_type = thrust_type_options[thrust_type_index];
                        EditorGUILayout.Space(6);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("--------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ths;
                        int.TryParse(item.WPN_thrust_speed, out ths);
                        ths = EditorGUILayout.IntField("Thrust Speed:", ths, GUILayout.MaxWidth(162));
                        item.WPN_thrust_speed = ths.ToString();
                        // item.WPN_thrust_speed = EditorGUILayout.TextField("Thrust Speed:", item.WPN_thrust_speed, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("----------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int td;
                        int.TryParse(item.WPN_thrust_damage, out td);
                        td = EditorGUILayout.IntField("Thrust Damage:", td, GUILayout.MaxWidth(162));
                        item.WPN_thrust_damage = td.ToString();
                        // item.WPN_thrust_damage = EditorGUILayout.TextField("Thrust Damage:", item.WPN_thrust_damage, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();

                        DrawUILine(colUILine, 3, 12);

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    // TwoHandedPolearm EDITOR
                    else if (item.WPN_weapon_class == "TwoHandedPolearm" || item.WPN_weapon_class == "OneHandedAxe" || item.WPN_weapon_class == "OneHandedSword" || item.WPN_weapon_class == "Banner")
                    {
                        EditorGUILayout.LabelField("Weapon Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("---------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        weapon_class_index = EditorGUILayout.Popup(weapon_class_index, weapon_class_options, GUILayout.Width(162));

                        item.WPN_weapon_class = weapon_class_options[weapon_class_index];

                        DrawUILine(colUILine, 3, 12);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Modifier Group: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        modifier_group_index = EditorGUILayout.Popup("Modifier Group:", modifier_group_index, modifier_group_options, GUILayout.Width(240));
                        item.WPN_modifier_group = modifier_group_options[modifier_group_index];

                        DrawUILine(colUILine, 3, 12);

                        // MeleeWeapon FLAGS
                        CreateAttributeToggle(ref WPN_FLG_MeleeWeapon_Bool, ref item.WPN_FLG_MeleeWeapon, "Melee Weapon");
                        EditorGUILayout.Space(1);

                        // PenaltyWithShield flags
                        CreateAttributeToggle(ref WPN_FLG_PenaltyWithShield_Bool, ref item.WPN_FLG_PenaltyWithShield, "Penalty With Shield");
                        EditorGUILayout.Space(1);

                        // NotUsableWithOneHand flags
                        CreateAttributeToggle(ref WPN_FLG_NotUsableWithOneHand_Bool, ref item.WPN_FLG_NotUsableWithOneHand, "Not Usable With One Hand");
                        EditorGUILayout.Space(1);

                        // TwoHandIdleOnMount flags
                        CreateAttributeToggle(ref WPN_FLG_TwoHandIdleOnMount_Bool, ref item.WPN_FLG_TwoHandIdleOnMount, "Two Hand Idle On Mount");
                        EditorGUILayout.Space(1);

                        // WideGrip flags
                        CreateAttributeToggle(ref WPN_FLG_WideGrip_Bool, ref item.WPN_FLG_WideGrip, "Wide Grip");
                        EditorGUILayout.Space(1);

                        DrawUILine(colUILine, 3, 12);


                        /// banner level & effect
                        if (item.WPN_weapon_class == "Banner"&&  wpn_effect_options != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Banner Level: "));
                            EditorGUIUtility.labelWidth = textDimensions.x;

                            int bl;
                            int.TryParse(item.WPN_banner_level, out bl);
                            bl = EditorGUILayout.IntField("Banner Level:", bl, GUILayout.MaxWidth(162));
                            item.WPN_banner_level = bl.ToString();
                            //DrawUILineVertical(colUILine, 1, 1, 16);
                            EditorGUILayout.EndHorizontal();

                            DrawUILine(colUILine, 3, 12);
                           
                            // EFFECT
                            EditorGUILayout.LabelField("Effect:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Effect---"));
                            EditorGUIUtility.labelWidth = textDimensions.x;

                            wpn_effect_index = EditorGUILayout.Popup(wpn_effect_index, wpn_effect_options, GUILayout.Width(300));

                            item.WPN_effect = wpn_effect_options[wpn_effect_index];


                            DrawUILine(colUILine, 3, 12);
                        }


                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Physics Material: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        physics_mats_index = EditorGUILayout.Popup("Physics Material:", physics_mats_index, physics_mats_options, GUILayout.Width(240));
                        item.WPN_physics_material = physics_mats_options[physics_mats_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Item Usage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        item_usage_index = EditorGUILayout.Popup("Item Usage: ", item_usage_index, item_usage_options, GUILayout.Width(256));
                        item.WPN_item_usage = item_usage_options[item_usage_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);



                        // POSITION 
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Position: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        posVec3 = EditorGUILayout.Vector3Field(" Position:", posVec3, GUILayout.MaxWidth(352));

                        string pos = posVec3.x + "," + posVec3.y + "," + posVec3.z;
                        item.WPN_position = pos;
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Weapon Balance: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int wb;
                        int.TryParse(item.WPN_weapon_balance, out wb);
                        wb = EditorGUILayout.IntField("Weapon Balance:", wb, GUILayout.MaxWidth(256));
                        item.WPN_weapon_balance = wb.ToString();
                        // item.WPN_weapon_balance = EditorGUILayout.TextField("Weapon Balance:", item.WPN_weapon_balance, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Speed Rating:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int sr;
                        int.TryParse(item.WPN_speed_rating, out sr);
                        sr = EditorGUILayout.IntField("Speed Rating:", sr, GUILayout.MaxWidth(256));
                        item.WPN_speed_rating = sr.ToString();
                        // item.WPN_speed_rating = EditorGUILayout.TextField("Speed Rating:", item.WPN_speed_rating, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Weapon Lenght:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int wl;
                        int.TryParse(item.WPN_weapon_length, out wl);
                        wl = EditorGUILayout.IntField("Weapon Lenght:", wl, GUILayout.MaxWidth(256));
                        item.WPN_weapon_length = wl.ToString();
                        // item.WPN_weapon_length = EditorGUILayout.TextField("Weapon Lenght:", item.WPN_weapon_length, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        EditorGUILayout.EndHorizontal();

                        DrawUILine(colUILine, 3, 12);

                        /// MISSILE SPEED
                        //if (item.WPN_weapon_class == "OneHandedAxe" || item.WPN_weapon_class == "OneHandedSword")
                        //{
                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Missile Speed: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ms;
                        int.TryParse(item.WPN_missile_speed, out ms);
                        ms = EditorGUILayout.IntField("Missile Speed:", ms, GUILayout.MaxWidth(162));
                        item.WPN_missile_speed = ms.ToString();
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);
                        //}

                        // Thrust damage
                        EditorGUILayout.LabelField("Thrust Damage Type:", EditorStyles.boldLabel);
                        thrust_type_index = EditorGUILayout.Popup(thrust_type_index, thrust_type_options, GUILayout.Width(128));
                        item.WPN_thrust_damage_type = thrust_type_options[thrust_type_index];
                        EditorGUILayout.Space(6);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("--------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ths;
                        int.TryParse(item.WPN_thrust_speed, out ths);
                        ths = EditorGUILayout.IntField("Thrust Speed:", ths, GUILayout.MaxWidth(162));
                        item.WPN_thrust_speed = ths.ToString();
                        // item.WPN_thrust_speed = EditorGUILayout.TextField("Thrust Speed:", item.WPN_thrust_speed, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("----------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int td;
                        int.TryParse(item.WPN_thrust_damage, out td);
                        td = EditorGUILayout.IntField("Thrust Damage:", td, GUILayout.MaxWidth(162));
                        item.WPN_thrust_damage = td.ToString();
                        // item.WPN_thrust_damage = EditorGUILayout.TextField("Thrust Damage:", item.WPN_thrust_damage, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();

                        DrawUILine(colUILine, 3, 12);

                        // Swing Damage
                        EditorGUILayout.LabelField("Swing Damage Type:", EditorStyles.boldLabel);
                        swing_type_index = EditorGUILayout.Popup(swing_type_index, swing_type_options, GUILayout.Width(128));
                        item.WPN_swing_damage_type = swing_type_options[swing_type_index];
                        EditorGUILayout.Space(6);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Swing Damage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int sd;
                        int.TryParse(item.WPN_swing_damage, out sd);
                        sd = EditorGUILayout.IntField("Swing Damage:", sd, GUILayout.MaxWidth(162));
                        item.WPN_swing_damage = sd.ToString();
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        // item.WPN_swing_damage = EditorGUILayout.TextField("Swing Damage:", item.WPN_swing_damage, GUILayout.MaxWidth(192));
                        DrawUILine(colUILine, 3, 12);

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    // BOW/CROSSBOW EDITOR
                    else if (item.WPN_weapon_class == "Bow" || item.WPN_weapon_class == "Crossbow")
                    {

                        EditorGUILayout.LabelField("Weapon Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("---------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        weapon_class_index = EditorGUILayout.Popup(weapon_class_index, weapon_class_options, GUILayout.Width(162));

                        item.WPN_weapon_class = weapon_class_options[weapon_class_index];

                        DrawUILine(colUILine, 3, 12);

                        // WPN_FLG_RangedWeapon_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_RangedWeapon_Bool, ref item.WPN_FLG_RangedWeapon, "Ranged Weapon");
                        EditorGUILayout.Space(1);

                        // WPN_FLG_HasString_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_HasString_Bool, ref item.WPN_FLG_HasString, "Has String");
                        EditorGUILayout.Space(1);

                        // WPN_FLG_StringHeldByHand_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_StringHeldByHand_Bool, ref item.WPN_FLG_StringHeldByHand, "String Held By Hand");
                        EditorGUILayout.Space(1);

                        // WPN_FLG_AutoReload_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AutoReload_Bool, ref item.WPN_FLG_AutoReload, "Auto Reload");
                        EditorGUILayout.Space(1);

                        // WPN_FLG_UnloadWhenSheathed_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_UnloadWhenSheathed_Bool, ref item.WPN_FLG_UnloadWhenSheathed, "Unload When Sheathed");
                        EditorGUILayout.Space(1);

                        // NotUsableWithOneHand flags                        
                        CreateAttributeToggle(ref WPN_FLG_NotUsableWithOneHand_Bool, ref item.WPN_FLG_NotUsableWithOneHand, "Not Usable With One Hand");
                        EditorGUILayout.Space(1);

                        // TwoHandIdleOnMount flags
                        CreateAttributeToggle(ref WPN_FLG_TwoHandIdleOnMount_Bool, ref item.WPN_FLG_TwoHandIdleOnMount, "Two Hand Idle On Mount");
                        EditorGUILayout.Space(1);

                        // AmmoSticksWhenShot flags
                        CreateAttributeToggle(ref WPN_FLG_AmmoSticksWhenShot_Bool, ref item.WPN_FLG_AmmoSticksWhenShot, "Ammo Sticks When Shot");
                        EditorGUILayout.Space(1);

                        // CantReloadOnHorseback flags
                        CreateAttributeToggle(ref WPN_FLG_CantReloadOnHorseback_Bool, ref item.WPN_FLG_CantReloadOnHorseback, "Cant Reload On Horseback");
                        EditorGUILayout.Space(1);



                        DrawUILine(colUILine, 3, 12);
                        //-------

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Physics Material: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        physics_mats_index = EditorGUILayout.Popup("Physics Material:", physics_mats_index, physics_mats_options, GUILayout.Width(240));
                        item.WPN_physics_material = physics_mats_options[physics_mats_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Item Usage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        item_usage_index = EditorGUILayout.Popup("Item Usage: ", item_usage_index, item_usage_options, GUILayout.Width(256));
                        item.WPN_item_usage = item_usage_options[item_usage_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // POSITION 
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Center of Mass: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        centerMassVec3 = EditorGUILayout.Vector3Field("Center of Mass:", centerMassVec3, GUILayout.MaxWidth(352));

                        string centerMass = centerMassVec3.x + "," + centerMassVec3.y + "," + centerMassVec3.z;
                        item.WPN_center_of_mass = centerMass;
                        DrawUILine(colUILine, 3, 12);

                        // BAR 1
                        EditorGUILayout.BeginHorizontal();

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Accuracy: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int acc;
                        int.TryParse(item.WPN_accuracy, out acc);
                        acc = EditorGUILayout.IntField("Accuracy:", acc, GUILayout.MaxWidth(256));
                        item.WPN_accuracy = acc.ToString();
                        // item.WPN_weapon_balance = EditorGUILayout.TextField("Weapon Balance:", item.WPN_weapon_balance, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Speed Rating:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int sr;
                        int.TryParse(item.WPN_speed_rating, out sr);
                        sr = EditorGUILayout.IntField("Speed Rating:", sr, GUILayout.MaxWidth(256));
                        item.WPN_speed_rating = sr.ToString();
                        // item.WPN_speed_rating = EditorGUILayout.TextField("Speed Rating:", item.WPN_speed_rating, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Weapon Lenght:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int wl;
                        int.TryParse(item.WPN_weapon_length, out wl);
                        wl = EditorGUILayout.IntField("Weapon Lenght:", wl, GUILayout.MaxWidth(256));
                        item.WPN_weapon_length = wl.ToString();
                        // item.WPN_weapon_length = EditorGUILayout.TextField("Weapon Lenght:", item.WPN_weapon_length, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        if (item.WPN_weapon_class == "Crossbow")
                        {
                            EditorGUILayout.BeginHorizontal();
                            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Reload Phase Count:"));
                            EditorGUIUtility.labelWidth = textDimensions.x;

                            int rphs;
                            int.TryParse(item.WPN_reload_phase_count, out rphs);
                            rphs = EditorGUILayout.IntField("Reload Phase Count:", rphs, GUILayout.MaxWidth(162));
                            item.WPN_reload_phase_count = rphs.ToString();
                            //DrawUILineVertical(colUILine, 1, 1, 16);
                            EditorGUILayout.EndHorizontal();

                            DrawUILine(colUILine, 3, 12);
                        }


                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Missile Speed: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ms;
                        int.TryParse(item.WPN_missile_speed, out ms);
                        ms = EditorGUILayout.IntField("Missile Speed:", ms, GUILayout.MaxWidth(162));
                        item.WPN_missile_speed = ms.ToString();
                        // DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // AMMO Class
                        EditorGUILayout.LabelField("Ammo Class:", EditorStyles.boldLabel);
                        ammo_class_index = EditorGUILayout.Popup(ammo_class_index, ammo_class_options, GUILayout.Width(128));
                        item.WPN_ammo_class = ammo_class_options[ammo_class_index];
                        EditorGUILayout.Space(6);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("--------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int amm_lim;
                        int.TryParse(item.WPN_ammo_limit, out amm_lim);
                        amm_lim = EditorGUILayout.IntField("Ammo Limit:", amm_lim, GUILayout.MaxWidth(162));
                        item.WPN_ammo_limit = amm_lim.ToString();
                        // item.WPN_thrust_speed = EditorGUILayout.TextField("Thrust Speed:", item.WPN_thrust_speed, GUILayout.MaxWidth(256));
                        // DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);


                        // Thrust damage
                        EditorGUILayout.LabelField("Thrust Damage Type:", EditorStyles.boldLabel);
                        thrust_type_index = EditorGUILayout.Popup(thrust_type_index, thrust_type_options, GUILayout.Width(128));
                        item.WPN_thrust_damage_type = thrust_type_options[thrust_type_index];
                        EditorGUILayout.Space(6);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("--------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ths;
                        int.TryParse(item.WPN_thrust_speed, out ths);
                        ths = EditorGUILayout.IntField("Thrust Speed:", ths, GUILayout.MaxWidth(162));
                        item.WPN_thrust_speed = ths.ToString();
                        // item.WPN_thrust_speed = EditorGUILayout.TextField("Thrust Speed:", item.WPN_thrust_speed, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("----------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int td;
                        int.TryParse(item.WPN_thrust_damage, out td);
                        td = EditorGUILayout.IntField("Thrust Damage:", td, GUILayout.MaxWidth(162));
                        item.WPN_thrust_damage = td.ToString();
                        // item.WPN_thrust_damage = EditorGUILayout.TextField("Thrust Damage:", item.WPN_thrust_damage, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    // Stone/Arrow/Boulder/Bolt EDITOR
                    ///
                    else if (item.WPN_weapon_class == "Stone" || item.WPN_weapon_class == "Arrow" || item.WPN_weapon_class == "Boulder" || item.WPN_weapon_class == "Bolt")
                    {


                        EditorGUILayout.LabelField("Weapon Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("---------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        weapon_class_index = EditorGUILayout.Popup(weapon_class_index, weapon_class_options, GUILayout.Width(162));

                        item.WPN_weapon_class = weapon_class_options[weapon_class_index];

                        DrawUILine(colUILine, 3, 12);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        // WPN_FLG_RangedWeapon_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_RangedWeapon_Bool, ref item.WPN_FLG_RangedWeapon, "Ranged Weapon");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_AutoReload_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AutoReload_Bool, ref item.WPN_FLG_AutoReload, "Auto Reload");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_Consumable_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_Consumable_Bool, ref item.WPN_FLG_Consumable, "Consumable");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_UseHandAsThrowBase_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_UseHandAsThrowBase_Bool, ref item.WPN_FLG_UseHandAsThrowBase, "Use Hand As Throw Base");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_CanPenetrateShield_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_CanPenetrateShield_Bool, ref item.WPN_FLG_CanPenetrateShield, "Can Penetrate Shield");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_MultiplePenetration_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_MultiplePenetration_Bool, ref item.WPN_FLG_MultiplePenetration, "Multiple Penetration");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_Burning_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_Burning_Bool, ref item.WPN_FLG_Burning, "Burning");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_LeavesTrail_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_LeavesTrail_Bool, ref item.WPN_FLG_LeavesTrail, "Leaves Trail");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_AmmoSticksWhenShot_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AmmoSticksWhenShot_Bool, ref item.WPN_FLG_AmmoSticksWhenShot, "Ammo Sticks When Shot");
                        EditorGUILayout.Space(1);
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical();
                        // WPN_FLG_AttachAmmoToVisual_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AttachAmmoToVisual_Bool, ref item.WPN_FLG_AttachAmmoToVisual, "Attach Ammo To Visual");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_CanKnockDown_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_CanKnockDown_Bool, ref item.WPN_FLG_CanKnockDown, "Can Knock Down");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_MissileWithPhysics_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_MissileWithPhysics_Bool, ref item.WPN_FLG_MissileWithPhysics, "Missile With Physics");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_AmmoCanBreakOnBounceBack_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AmmoCanBreakOnBounceBack_Bool, ref item.WPN_FLG_AmmoCanBreakOnBounceBack, "Ammo Can Break On Bounce Back");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_AmmoBreaksOnBounceBack_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AmmoBreaksOnBounceBack_Bool, ref item.WPN_FLG_AmmoBreaksOnBounceBack, "Ammo Breaks On Bounce Back");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_AffectsArea_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AffectsArea_Bool, ref item.WPN_FLG_AffectsArea, "Affects Area");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_AffectsAreaBig_Bool flags
                        CreateAttributeToggle(ref WPN_FLG_AffectsAreaBig_Bool, ref item.WPN_FLG_AffectsAreaBig, "Affects Area Big");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_NotUsableWithOneHand flags
                        CreateAttributeToggle(ref WPN_FLG_NotUsableWithOneHand_Bool, ref item.WPN_FLG_NotUsableWithOneHand, "Not Usable With One Hand");
                        EditorGUILayout.Space(1);
                        // WPN_FLG_TwoHandIdleOnMount flags
                        CreateAttributeToggle(ref WPN_FLG_TwoHandIdleOnMount_Bool, ref item.WPN_FLG_TwoHandIdleOnMount, "Two Hand Idle On Mount");
                        EditorGUILayout.Space(1);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Physics Material: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        physics_mats_index = EditorGUILayout.Popup("Physics Material:", physics_mats_index, physics_mats_options, GUILayout.Width(240));
                        item.WPN_physics_material = physics_mats_options[physics_mats_index];
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Item Usage: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;
                        item_usage_index = EditorGUILayout.Popup("Item Usage: ", item_usage_index, item_usage_options, GUILayout.Width(256));
                        item.WPN_item_usage = item_usage_options[item_usage_index];
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        // POSITION S
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Center of Mass V3: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        if (item.WPN_weapon_class == "Boulder")
                        {
                            posVec3 = EditorGUILayout.Vector3Field(" Position:", posVec3, GUILayout.MaxWidth(352));
                            string pos = posVec3.x + "," + posVec3.y + "," + posVec3.z;
                            item.WPN_position = pos;
                        }
                        else
                        {
                            rotVec3 = EditorGUILayout.Vector3Field(" Rotation:", rotVec3, GUILayout.MaxWidth(352));
                            string rot = rotVec3.x + "," + rotVec3.y + "," + rotVec3.z;
                            item.WPN_rotation = rot;
                        }


                        if (item.WPN_weapon_class != "Stone" && item.WPN_weapon_class != "Boulder")
                        {
                            centerMassVec3 = EditorGUILayout.Vector3Field(" Center of Mass:", centerMassVec3, GUILayout.MaxWidth(352));
                            string centerMass = centerMassVec3.x + "," + centerMassVec3.y + "," + centerMassVec3.z;
                            item.WPN_center_of_mass = centerMass;

                            DrawUILine(colUILine, 3, 12);

                            textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Center of Mass V3: "));
                            EditorGUIUtility.labelWidth = textDimensions.x;

                            stickingPosVec3 = EditorGUILayout.Vector3Field(" Sticking Position:", stickingPosVec3, GUILayout.MaxWidth(352));
                            DrawUILine(colUILine, 1, 6);
                            stickingRotVec3 = EditorGUILayout.Vector3Field(" Sticking Rotation:", stickingRotVec3, GUILayout.MaxWidth(352));
                            DrawUILine(colUILine, 1, 6);

                            string stk_pos = stickingPosVec3.x + "," + stickingPosVec3.y + "," + stickingPosVec3.z;
                            item.WPN_rotation = stk_pos;
                            string stk_rot = stickingRotVec3.x + "," + stickingRotVec3.y + "," + stickingRotVec3.z;
                            item.WPN_rotation = stk_rot;
                        }
                        else
                        {

                            if (item.WPN_weapon_class == "Boulder")
                            {
                                centerMassVec3 = EditorGUILayout.Vector3Field(" Center of Mass:", centerMassVec3, GUILayout.MaxWidth(352));
                                string centerMass = centerMassVec3.x + "," + centerMassVec3.y + "," + centerMassVec3.z;
                                item.WPN_center_of_mass = centerMass;
                            }

                            rotSpeedVec3 = EditorGUILayout.Vector3Field(" Rotation Speed:", rotSpeedVec3, GUILayout.MaxWidth(352));
                            string rotSpeed = rotSpeedVec3.x + "," + rotSpeedVec3.y + "," + rotSpeedVec3.z;
                            item.WPN_rotation_speed = rotSpeed;
                        }



                        DrawUILine(colUILine, 3, 12);


                        // PSYS & SOUND CODES
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Passby Sound Code: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;


                        item.WPN_passby_sound_code = EditorGUILayout.TextField("Passby Sound Code: ", item.WPN_passby_sound_code);

                        if (item.WPN_weapon_class != "Stone" && item.WPN_weapon_class != "Bolt")
                        {
                            item.WPN_flying_sound_code = EditorGUILayout.TextField("Flying Sound Code: ", item.WPN_flying_sound_code);
                            DrawUILine(colUILine, 1, 6);
                            item.WPN_trail_particle_name = EditorGUILayout.TextField("Trail Particle Name: ", item.WPN_trail_particle_name);
                        }

                        DrawUILine(colUILine, 3, 12);



                        // BAR 1
                        EditorGUILayout.BeginHorizontal();

                        if (item.WPN_weapon_class == "Stone")
                        {
                            textDimensions = GUI.skin.label.CalcSize(new GUIContent("Accuracy: "));
                            EditorGUIUtility.labelWidth = textDimensions.x;

                            int acc;
                            int.TryParse(item.WPN_accuracy, out acc);
                            acc = EditorGUILayout.IntField("Accuracy:", acc, GUILayout.MaxWidth(256));
                            item.WPN_accuracy = acc.ToString();
                            // item.WPN_weapon_balance = EditorGUILayout.TextField("Weapon Balance:", item.WPN_weapon_balance, GUILayout.MaxWidth(256));
                            DrawUILineVertical(colUILine, 1, 1, 16);
                        }

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Speed Rating:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int sr;
                        int.TryParse(item.WPN_speed_rating, out sr);
                        sr = EditorGUILayout.IntField("Speed Rating:", sr, GUILayout.MaxWidth(256));
                        item.WPN_speed_rating = sr.ToString();
                        // item.WPN_speed_rating = EditorGUILayout.TextField("Speed Rating:", item.WPN_speed_rating, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Weapon Lenght:"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int wl;
                        int.TryParse(item.WPN_weapon_length, out wl);
                        wl = EditorGUILayout.IntField("Weapon Lenght:", wl, GUILayout.MaxWidth(256));
                        item.WPN_weapon_length = wl.ToString();
                        // item.WPN_weapon_length = EditorGUILayout.TextField("Weapon Lenght:", item.WPN_weapon_length, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        EditorGUILayout.EndHorizontal();

                        DrawUILine(colUILine, 3, 12);



                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Missile Speed: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ms;
                        int.TryParse(item.WPN_missile_speed, out ms);
                        ms = EditorGUILayout.IntField("Missile Speed:", ms, GUILayout.MaxWidth(162));
                        item.WPN_missile_speed = ms.ToString();
                        // DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);



                        // AMMO Class

                        if (item.WPN_weapon_class == "Stone")
                        {
                            EditorGUILayout.LabelField("Ammo Class:", EditorStyles.boldLabel);
                            ammo_class_index = EditorGUILayout.Popup(ammo_class_index, ammo_class_options, GUILayout.Width(128));
                            item.WPN_ammo_class = ammo_class_options[ammo_class_index];
                            EditorGUILayout.Space(6);
                        }


                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("Stack Ammount: "));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int stack_amm;
                        int.TryParse(item.WPN_stack_amount, out stack_amm);
                        stack_amm = EditorGUILayout.IntField("Stack Ammount:", stack_amm, GUILayout.MaxWidth(162));
                        item.WPN_stack_amount = stack_amm.ToString();
                        // item.WPN_thrust_speed = EditorGUILayout.TextField("Thrust Speed:", item.WPN_thrust_speed, GUILayout.MaxWidth(256));
                        // DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        if (item.WPN_weapon_class != "Boulder")
                        {
                            // Thrust damage
                            EditorGUILayout.LabelField("Thrust Damage Type:", EditorStyles.boldLabel);
                            thrust_type_index = EditorGUILayout.Popup(thrust_type_index, thrust_type_options, GUILayout.Width(128));
                            item.WPN_thrust_damage_type = thrust_type_options[thrust_type_index];
                            EditorGUILayout.Space(6);
                        }

                        EditorGUILayout.BeginHorizontal();
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("--------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int ths;
                        int.TryParse(item.WPN_thrust_speed, out ths);
                        ths = EditorGUILayout.IntField("Thrust Speed:", ths, GUILayout.MaxWidth(162));
                        item.WPN_thrust_speed = ths.ToString();
                        // item.WPN_thrust_speed = EditorGUILayout.TextField("Thrust Speed:", item.WPN_thrust_speed, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);

                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("----------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        int td;
                        int.TryParse(item.WPN_thrust_damage, out td);
                        td = EditorGUILayout.IntField("Thrust Damage:", td, GUILayout.MaxWidth(162));
                        item.WPN_thrust_damage = td.ToString();
                        // item.WPN_thrust_damage = EditorGUILayout.TextField("Thrust Damage:", item.WPN_thrust_damage, GUILayout.MaxWidth(256));
                        DrawUILineVertical(colUILine, 1, 1, 16);
                        EditorGUILayout.EndHorizontal();
                        DrawUILine(colUILine, 3, 12);

                        EditorGUIUtility.labelWidth = originDimensions;
                    }
                    // Banner EDITOR
                    ///
                    //else if (item.WPN_weapon_class == "Banner")
                    //{
                    //    EditorGUILayout.LabelField("Weapon Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                    //    textDimensions = GUI.skin.label.CalcSize(new GUIContent("---------------"));
                    //    EditorGUIUtility.labelWidth = textDimensions.x;

                    //    weapon_class_index = EditorGUILayout.Popup(weapon_class_index, weapon_class_options, GUILayout.Width(162));

                    //    item.WPN_weapon_class = weapon_class_options[weapon_class_index];

                    //    DrawUILine(colUILine, 3, 12);

                    //    EditorGUILayout.BeginHorizontal();
                    //    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Physics Material: "));
                    //    EditorGUIUtility.labelWidth = textDimensions.x;
                    //    physics_mats_index = EditorGUILayout.Popup("Physics Material:", physics_mats_index, physics_mats_options, GUILayout.Width(240));
                    //    item.WPN_physics_material = physics_mats_options[physics_mats_index];
                    //    DrawUILineVertical(colUILine, 1, 1, 16);

                    //    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Item Usage: "));
                    //    EditorGUIUtility.labelWidth = textDimensions.x;
                    //    item_usage_index = EditorGUILayout.Popup("Item Usage: ", item_usage_index, item_usage_options, GUILayout.Width(256));
                    //    item.WPN_item_usage = item_usage_options[item_usage_index];
                    //    EditorGUILayout.EndHorizontal();
                    //    DrawUILine(colUILine, 3, 12);

                    //    // POSITION S
                    //    textDimensions = GUI.skin.label.CalcSize(new GUIContent(" Position: "));
                    //    EditorGUIUtility.labelWidth = textDimensions.x;

                    //    posVec3 = EditorGUILayout.Vector3Field(" Position:", posVec3, GUILayout.MaxWidth(352));
                    //    string pos = posVec3.x + "," + posVec3.y + "," + posVec3.z;
                    //    item.WPN_position = pos;

                    //    DrawUILine(colUILine, 3, 12);

                    //    // BAR 1
                    //    EditorGUILayout.BeginHorizontal();

                    //    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Speed Rating:"));
                    //    EditorGUIUtility.labelWidth = textDimensions.x;

                    //    int sr;
                    //    int.TryParse(item.WPN_speed_rating, out sr);
                    //    sr = EditorGUILayout.IntField("Speed Rating:", sr, GUILayout.MaxWidth(256));
                    //    item.WPN_speed_rating = sr.ToString();
                    //    // item.WPN_speed_rating = EditorGUILayout.TextField("Speed Rating:", item.WPN_speed_rating, GUILayout.MaxWidth(256));
                    //    DrawUILineVertical(colUILine, 1, 1, 16);

                    //    textDimensions = GUI.skin.label.CalcSize(new GUIContent("Weapon Lenght:"));
                    //    EditorGUIUtility.labelWidth = textDimensions.x;

                    //    int wl;
                    //    int.TryParse(item.WPN_weapon_length, out wl);
                    //    wl = EditorGUILayout.IntField("Weapon Lenght:", wl, GUILayout.MaxWidth(256));
                    //    item.WPN_weapon_length = wl.ToString();
                    //    // item.WPN_weapon_length = EditorGUILayout.TextField("Weapon Lenght:", item.WPN_weapon_length, GUILayout.MaxWidth(256));
                    //    DrawUILineVertical(colUILine, 1, 1, 16);

                    //    EditorGUILayout.EndHorizontal();

                    //    DrawUILine(colUILine, 3, 12);


                    //}
                    else
                    {
                        EditorGUILayout.LabelField("Weapon Class:", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                        textDimensions = GUI.skin.label.CalcSize(new GUIContent("---------------"));
                        EditorGUIUtility.labelWidth = textDimensions.x;

                        weapon_class_index = EditorGUILayout.Popup(weapon_class_index, weapon_class_options, GUILayout.Width(162));

                        item.WPN_weapon_class = weapon_class_options[weapon_class_index];

                        DrawUILine(colUILine, 3, 12);
                        EditorGUILayout.HelpBox("Select Weapon Class.", MessageType.Warning);
                    }
                }
            }
        }
    }

    private void DrawItemCultureUI()
    {
        // CULTURE
        if (item.culture != null && item.culture != "")
        {
            if (item.culture.Contains("Culture."))
            {
                string dataName = item.culture.Replace("Culture.", "");
                string asset = dataPath + item.moduleID + "/Cultures/" + dataName + ".asset";

                if (System.IO.File.Exists(asset))
                {
                    cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(asset, typeof(Culture));
                }
                else
                {
                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + item.moduleID + ".asset";
                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {
                            string dpdAsset = dataPath + dpdMod + "/Cultures/" + dataName + ".asset";

                            if (System.IO.File.Exists(dpdAsset))
                            {
                                cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Culture));
                                break;
                            }
                            else
                            {
                                cultureIs = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (cultureIs == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == item.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.culturesData.cultures)
                                    {
                                        if (data.id == dataName)
                                        {
                                            string dpdAsset = dataPath + iSDependencyOfMod.id + "/Cultures/" + dataName + ".asset";
                                            cultureIs = (Culture)AssetDatabase.LoadAssetAtPath(dpdAsset, typeof(Culture));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (cultureIs == null)
                        {
                            Debug.Log("Culture " + dataName + " - Not EXIST in" + " ' " + item.moduleID + " ' " + "resources, and they dependencies.");
                        }
                    }
                }
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Item Culture:", EditorStyles.label);
        object cultureField = EditorGUILayout.ObjectField(cultureIs, typeof(Culture), true);
        cultureIs = (Culture)cultureField;

        if (cultureIs != null)
        {
            item.culture = "Culture." + cultureIs.id;
        }
        else
        {
            item.culture = "";
        }
        GUILayout.EndHorizontal();

        //CULTURE END
    }

    public void RefreshCraftingPieces()
    {
        var temp = new List<string>();

        if (item.CT_pieces_Type != null && item.CT_pieces_Type.Length > 0)
        {
            foreach (var pType in item.CT_pieces_Type)
            {
                if (pType != "")
                    temp.Add(pType);
            }
        }


        if (temp.Count > 0)
        {
            var val = settingsAsset.PieceTypesDefinitions.Length - temp.Count;
            //if (val < 0)
            //    val = 0;

            piece_type_options = new string[val];
            //Debug.Log(areaType_options.Length);
            piece_type_index = 0;
            int index = 0;
            foreach (var piece_type_id in settingsAsset.PieceTypesDefinitions)
            {
                if (!temp.Contains(piece_type_id))
                {
                    //Debug.Log(piece_type_id);
                    piece_type_options[index] = piece_type_id;
                    index++;

                }
            }
        }
        else
        {
            piece_type_options = new string[settingsAsset.PieceTypesDefinitions.Length];
            piece_type_index = 0;
            int index = 0;
            foreach (var p_type in settingsAsset.PieceTypesDefinitions)
            {
                piece_type_options[index] = p_type;
                index++;
            }
        }

        //int index_opt = 0;
        //foreach (var piece_type in item.CT_pieces_Type)
        //{
        //    CreateOptions(ref item.CT_pieces_Type[index_opt], ref piece_type_options, ref piece_type_index, settingsAsset.PieceTypesDefinitions);
        //    index_opt++;
        //}
    }

    void CreateIntAttribute(ref string attribute, string label)
    {

        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label + " "));
        EditorGUIUtility.labelWidth = textDimensions.x;

        int val;
        int.TryParse(attribute, out val);
        val = EditorGUILayout.IntField(label, val, GUILayout.MaxWidth(162));
        attribute = val.ToString();

    }

    public void CreateColorLabel(ref string HTMLColor, ref Color color, string colorLabel)
    {
        if (HTMLColor != null && HTMLColor != "")
        {
            GUILayout.BeginHorizontal();
            ColorUtility.TryParseHtmlString("#" + HTMLColor, out color);
            EditorGUILayout.LabelField(colorLabel);
            color = EditorGUILayout.ColorField(color);
            GUILayout.EndHorizontal();
            HTMLColor = ColorUtility.ToHtmlStringRGBA(color);
            // Debug.Log("Color Parsed: " + labelColor);

            if (color == new Color(0, 0, 0, 0))
            {
                HTMLColor = "";
            }
        }
        else
        {
            if (color == new Color(0, 0, 0, 0))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(colorLabel + " (Unused)");
                color = EditorGUILayout.ColorField(color);
                GUILayout.EndHorizontal();
                HTMLColor = "";
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(colorLabel);
                color = EditorGUILayout.ColorField(color);
                GUILayout.EndHorizontal();
                HTMLColor = ColorUtility.ToHtmlStringRGBA(color);
            }
        }
    }

    private void CreateAttributeToggle(ref bool attrBool, ref string attr, string toggleLabel)
    {
        attrBool = GUILayout.Toggle(attrBool, toggleLabel);

        if (attrBool)
        {
            attr = "true";
        }
        else
        {
            attr = "false";
        }
    }

    private void SetLabelFieldTS(ref string inputString, ref string soloString, ref string TS_Name, string TSfolder, TranslationString TS, string typeName, string moduleID, UnityEngine.Object obj, int objID, string tsLabel)
    {

        bool isDependencyTS = false;
        var labelStyle = new GUIStyle(EditorStyles.label);
        if (soloString == null || soloString == "")
        {
            EditorGUILayout.HelpBox(typeName + " field is empty", MessageType.Error);
        }


        if (name != null && inputString != null && inputString != "")
        {
            var WorkString = inputString;

            var plural_string = WorkString;
            var soloID = "";

            soloString = inputString;
            TS_Name = inputString;

            if (WorkString.Contains("{@Plural}"))
            {

                // TS TAG - example: plural
                Regex regex_tag = new Regex(@"{@Plural}(.*){\\@}");


                var v_plu = regex_tag.Match(plural_string);
                string p_string = v_plu.Groups[1].ToString();
                // Debug.Log(p_string);

                plural_string = p_string;

                TS_Name = TS_Name.Replace("{@Plural}" + plural_string + "{\\@}", "");

                Regex regex = new Regex("{=(.*)}");

                var v = regex.Match(TS_Name);
                string s = v.Groups[1].ToString();
                soloID = s;

                TS_Name = "{=" + s + "}";

                // Debug.Log(WorkString);

            }
            else
            {

                Regex regex = new Regex("{=(.*)}");
                if (regex != null)
                {
                    var v = regex.Match(TS_Name);
                    string s = v.Groups[1].ToString();
                    TS_Name = "{=" + s + "}";
                }

            }

            if (TS_Name != "" && TS_Name != "{=}")
            {
                soloString = soloString.Replace(TS_Name, "");
                soloString = soloString.Replace("{@Plural}" + plural_string + "{\\@}", "");


                string TSasset = (dataPath + moduleID + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset");

                if (System.IO.File.Exists(TSasset))
                {
                    TS = (TranslationString)AssetDatabase.LoadAssetAtPath(TSasset, typeof(TranslationString));
                }
                else
                {

                    // SEARCH IN DEPENDENCIES
                    string modSett = modsSettingsPath + moduleID + ".asset";

                    ModuleReceiver currMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(modSett, typeof(ModuleReceiver));

                    foreach (string dpdMod in currMod.modDependenciesInternal)
                    {
                        string dpdPath = modsSettingsPath + dpdMod + ".asset";

                        if (System.IO.File.Exists(dpdPath))
                        {

                            string dpdTSAsset = dataPath + dpdMod + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";

                            if (System.IO.File.Exists(dpdTSAsset))
                            {
                                TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdTSAsset, typeof(TranslationString));
                                isDependencyTS = true;
                                break;
                            }
                            else
                            {
                                TS = null;
                            }

                        }
                    }

                    //Check is dependency OF
                    if (TS == null)
                    {
                        string[] mods = Directory.GetFiles(modsSettingsPath, "*.asset");

                        foreach (string mod in mods)
                        {
                            ModuleReceiver iSDependencyOfMod = (ModuleReceiver)AssetDatabase.LoadAssetAtPath(mod, typeof(ModuleReceiver));

                            foreach (var depend in iSDependencyOfMod.modDependenciesInternal)
                            {
                                if (depend == item.moduleID)
                                {
                                    foreach (var data in iSDependencyOfMod.modFilesData.translationData.translationStrings)
                                    {
                                        if (data.id == TS_Name)
                                        {
                                            string dpdTSAsset = dataPath + iSDependencyOfMod.id + "/TranslationData/" + TSfolder + "/" + TS_Name + ".asset";
                                            TS = (TranslationString)AssetDatabase.LoadAssetAtPath(dpdTSAsset, typeof(TranslationString));
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (TS == null)
                        {
                            Debug.Log("Translation String " + TS_Name + " - Not EXIST in" + " ' " + item.moduleID + " ' " + "resources, and they dependencies.");
                        }

                    }
                }
            }
        }

        soloString = EditorGUILayout.TextField(typeName, soloString);

        // Draw UI - & Edit Translation String 
        EditorGUILayout.BeginHorizontal();
        // translationStringID = EditorGUILayout.TextField("Translation String", translationStringID);

        if (TS == null)
        {
            labelStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
            EditorGUILayout.LabelField("Translation String: (Unused)", labelStyle);
        }
        else
        {
            EditorGUILayout.LabelField("Translation String:", EditorStyles.label);
        }


        EditorGUILayout.ObjectField(TS, typeof(TranslationString), true);

        if (GUILayout.Button("Edit Translation"))
        {

            TranslationEditor transEditor = (TranslationEditor)EditorWindow.GetWindow(typeof(TranslationEditor), true, "Translation Editor");
            transEditor.defaultData = TS;
            transEditor.module = moduleID;
            transEditor.obj = obj;
            transEditor.objectID = objID;
            transEditor.translationLabel = typeName.ToUpper() + " ( - " + tsLabel + " - )";

            transEditor.isDependency = isDependencyTS;

            transEditor.SearchStrings();
            transEditor.Show();
        }
        EditorGUILayout.EndHorizontal();
        // DrawUILine(colUILine, 3, 12);


        // Solo Name Check
        if (soloString != null && soloString != "")
        {
            if (TS_Name != "" && TS_Name != "{=}")
            {
                inputString = TS_Name + soloString;
            }
            else
            {
                inputString = soloString;
            }


        }

        if (soloString != null && soloString.Length <= 2)
        {
            // soloText = "";
            // translationString = "";
            if (TS != null)
            {
                inputString = TS.name + soloString;
            }
            else
            {
                inputString = soloString;
            }

        }

        if (TS != null && isDependencyTS == false)
        {
            TS.stringText = soloString;
        }

    }

    public static AssetsDataManager ADM_Instance
    {
        get { return EditorWindow.GetWindow<AssetsDataManager>(); }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        if (sVector != null && sVector != "")
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
        else
        {
            Vector3 result = new Vector3(0, 0, 0);
            return result;
        }
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
        r.x += (padding / 2) + 2;
        r.y -= 2;
        r.height += 6 + lenght;
        EditorGUI.DrawRect(r, color);
    }


}
