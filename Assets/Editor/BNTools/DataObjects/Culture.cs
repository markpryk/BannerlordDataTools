using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Culture : ScriptableObject
{
    public string moduleID;
    [SerializeField]
    public string XmlFileName;
    [SerializeField]

    public string id;
    public string cultureName;

    public string is_main_culture;
    public string color;
    public string color2;
    public string elite_basic_troop;
    public string basic_troop;
    public string melee_militia_troop;
    public string ranged_militia_troop;
    public string melee_elite_militia_troop;
    public string ranged_elite_militia_troop;
    public string can_have_settlement;
    public string town_edge_number;
    public string villager_party_template;
    public string default_party_template;
    public string caravan_party_template;
    public string elite_caravan_party_template;
    public string militia_party_template;
    public string rebels_party_template;
    public string prosperity_bonus;
    public string encounter_background_mesh;
    public string default_face_key;
    public string text;
    public string tournament_master;
    public string villager;
    public string caravan_master;
    public string armed_trader;
    public string caravan_guard;
    public string veteran_caravan_guard;
    public string duel_preset;
    public string prison_guard;
    public string guard;
    public string blacksmith;
    public string weaponsmith;
    public string townswoman;
    public string townswoman_infant;
    public string townswoman_child;
    public string townswoman_teenager;
    public string townsman;
    public string townsman_infant;
    public string townsman_child;
    public string village_woman;
    public string villager_male_child;
    public string villager_male_teenager;
    public string villager_female_child;
    public string villager_female_teenager;
    public string townsman_teenager;
    public string ransom_broker;
    public string gangleader_bodyguard;
    public string merchant_notary;
    public string artisan_notary;
    public string preacher_notary;
    public string rural_notable_notary;
    public string shop_worker;
    public string tavernkeeper;
    public string taverngamehost;
    public string musician;
    public string tavern_wench;
    public string armorer;
    public string horseMerchant;
    public string barber;
    public string merchant;
    public string beggar;
    public string female_beggar;
    public string female_dancer;
    public string gear_practice_dummy;
    public string weapon_practice_stage_1;
    public string weapon_practice_stage_2;
    public string weapon_practice_stage_3;
    public string gear_dummy;
    public string board_game_type;

    public string default_battle_equipment_roster;
    public string default_civilian_equipment_roster;

    /// UPDATE 
    public string vassal_reward_party_template;
    public string faction_banner_key;
    public string militia_bonus;
    public string is_bandit;

    public string bandit_chief;
    public string bandit_raider;
    public string bandit_bandit;
    public string bandit_boss;
    public string bandit_boss_party_template;

    public string[] reward_item_id;
    public string[] cultural_feat_id;
    public string[] banner_icon_id;
    ///

    public string[] male_names;
    public string[] female_names;
    public string[] clan_names;

    public string[] TTT_one_participants;
    public string[] TTT_two_participants;
    public string[] TTT_four_participants;

    //public string[] child_character_templates;
    public string[] notable_and_wanderer_templates;
    public string[] lord_templates;
    public string[] rebellion_hero_templates;

    public string[] banner_bearer_replacement_weapons;

}
