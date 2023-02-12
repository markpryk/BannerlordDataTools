using UnityEditor;
using UnityEngine;
[System.Serializable]


public class Item : ScriptableObject
{

    public string moduleID;
    public string XmlFileName;

    public string id;
    public string itemName;
    public string mesh;
    public string culture;

    public string is_merchandise;
    public string value;
    public string item_category;
    public string weight;
    public string difficulty;
    public string appearance;
    public string Type;

    // Update
    public string subtype;
    public string lod_atlas_index;
    public string recalculate_body;
    public string multiplayer_item;
    public string using_tableau;

    public string IsFood;
    public string body_name;
    public string shield_body_name;
    public string flying_mesh;
    public string AmmoOffset;
    public string prefab;
    public string item_holsters;
    public string holster_body_name;
    public string holster_mesh;
    public string holster_mesh_with_weapon;
    public string has_lower_holster_priority;
    public string holster_position_shift;

    // ITEM COMPONENT

    // ARM ARMOR
    public string ARMOR_arm_armor;
    public string ARMOR_covers_hands;
    public string WPN_modifier_group;
    public string ARMOR_material_type;
    public string ARMOR_family_type;

    // BODY ARMOR

    public string ARMOR_body_armor;
    public string ARMOR_leg_armor;
    public string ARMOR_has_gender_variations;
    public string ARMOR_covers_body;
    public string ARMOR_body_mesh_type;
    public string ARMOR_head_armor;
    public string ARMOR_hair_cover_type;
    public string ARMOR_beard_cover_type;
    public string ARMOR_covers_legs;

    // ARMOR UPDATE

    public string ARMOR_mane_cover_type;
    public string ARMOR_maneuver_bonus;
    public string ARMOR_speed_bonus;
    public string ARMOR_charge_bonus;
    public string ARMOR_reins_mesh;
    public string ARMOR_covers_head;

    //WEAPONS
    public string WPN_weapon_class;
    public string WPN_thrust_speed;
    public string WPN_thrust_damage;
    public string WPN_thrust_damage_type;
    public string WPN_speed_rating;
    public string WPN_physics_material;
    public string WPN_item_usage;
    public string WPN_position;
    public string WPN_rotation;
    public string WPN_weapon_length;
    public string WPN_center_of_mass;
    public string WPN_hit_points;
    public string WPN_weapon_balance;
    public string WPN_missile_speed;
    public string WPN_swing_damage;
    public string WPN_swing_damage_type;
    public string WPN_ammo_class;
    public string WPN_ammo_limit;
    public string WPN_accuracy;
    public string WPN_flying_sound_code;
    public string WPN_rotation_speed;
    public string WPN_trail_particle_name;
    public string WPN_stack_amount;
    public string WPN_passby_sound_code;
    public string WPN_sticking_rotation;
    public string WPN_sticking_position;

    // WEAPON FLAGS

    public string WPN_FLG_RangedWeapon;
    public string WPN_FLG_HasString;
    public string WPN_FLG_CantReloadOnHorseback;
    public string WPN_FLG_NotUsableWithOneHand;
    public string WPN_FLG_TwoHandIdleOnMount;
    public string WPN_FLG_Consumable;
    public string WPN_FLG_AutoReload;
    public string WPN_FLG_UseHandAsThrowBase;
    public string WPN_FLG_AmmoSticksWhenShot;
    public string WPN_FLG_AmmoBreaksOnBounceBack;
    public string WPN_FLG_AttachAmmoToVisual;
    public string WPN_FLG_CanBlockRanged;
    public string WPN_FLG_HasHitPoints;
    public string WPN_FLG_MeleeWeapon;
    public string WPN_FLG_PenaltyWithShield;
    public string WPN_FLG_WideGrip;

    // WEAPONS FLAG UPDATE

    public string WPN_FLG_StringHeldByHand;
    public string WPN_FLG_UnloadWhenSheathed;
    public string WPN_FLG_CanPenetrateShield;
    public string WPN_FLG_MultiplePenetration;
    public string WPN_FLG_Burning;
    public string WPN_FLG_LeavesTrail;
    public string WPN_FLG_CanKnockDown;
    public string WPN_FLG_MissileWithPhysics;
    public string WPN_FLG_AmmoCanBreakOnBounceBack;
    public string WPN_FLG_AffectsArea;

    // ITEM HORSES & OTHER

    public string HRS_monster;
    public string HRS_maneuver;
    public string HRS_speed;
    public string HRS_charge_damage;
    public string HRS_body_length;
    public string HRS_is_mountable;
    public string HRS_extra_health;
    public string HRS_skeleton_scale;
    public string HRS_is_pack_animal;
    public string HRS_mane_mesh;

    // TRADE GOODS
    public string TRADE_morale_bonus;

    // FLAGS

    public string FLG_DropOnWeaponChange;
    public string FLG_ForceAttachOffHandPrimaryItemBone;
    public string FLG_HeldInOffHand;
    public string FLG_HasToBeHeldUp;
    public string FLG_WoodenParry;
    public string FLG_DoNotScaleBodyAccordingToWeaponLength;
    public string FLG_QuickFadeOut;
    public string FLG_CannotBePickedUp;
    public string FLG_DropOnAnyAction;
    public string FLG_Civilian;
    public string FLG_UseTeamColor;
    public string FLG_ForceAttachOffHandSecondaryItemBone;
    public string FLG_DoesNotHideChest;

    ///  ADDITIONAL MESHES

    public string ADD_cover_Mesh_name;
    public string ADD_Mesh_affected_by_cover;
    public string ADD_Mesh;

    public string[] ADD_mat_name;
    public string[] aDD_mat_meshMult_a;
    public string[] aDD_mat_meshMult_a_prc;
    public string[] aDD_mat_meshMult_b;
    public string[] aDD_mat_meshMult_b_prc;


    /// IS CRAFTING TEMPLATE

    public string CT_crafting_template;
    public string[] CT_pieces_id;
    public string[] CT_pieces_Type;
    public string[] CT_pieces_scale_factor;

    /// 1.7.2
    ///  UPDATE

    public string WPN_reload_phase_count;
    //public string WPN_item_modifier_group;
    public string WPN_FLG_AffectsAreaBig;
    public string CT_has_modifier;

    // Update 1.8.0
    public string FLG_CanBePickedUpFromCorpse;
    public string FLG_WoodenAttack;

    public string prerequisite_item;
    public string using_arm_band;

    public string WPN_shield_width;
    public string WPN_shield_down_length;

    public string WPN_FLG_FirearmAmmo;
    public string WPN_FLG_NotUsableWithTwoHand;
    public string WPN_FLG_BonusAgainstShield;
    public string WPN_FLG_CanDismount;

    // public string HRS_mane_mesh_multiplier; (TODO) or obsolete
    // public string ARMOR_body_deform_type; (TODO) or obsolete
    // public string WPN_effect_amount; (TODO) or obsolete

    public string WPN_banner_level;
    public string WPN_effect;

    /// Internal Variables

    public bool IsArmor;
    public bool IsArmorNobleHelm;
    public bool IsWeapon;
    public bool IsHorse;
    public bool IsTrade;
    public bool IsCraftedItem;

}
