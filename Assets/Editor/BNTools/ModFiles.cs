using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModFiles : ScriptableObject
{
    [SerializeField]
    public ModuleReceiver mod;
    [SerializeField]

    public ExportSettings exportSettings;
    [SerializeField]
    public string BNResourcesPath;
    [SerializeField]
    public string modSettingsPath;
    [SerializeField]
    public string modResourcesPath;
    [SerializeField]
    public KingdomsData kingdomsData;
    [SerializeField]
    public FactionsData factionsData;
    [SerializeField]
    public SettlementsData settlementsData;
    [SerializeField]
    public NPCCharactersData npcChrData;
    [SerializeField]
    public CulturesData culturesData;
    [SerializeField]
    public PartyTemplatesData PTdata;
    [SerializeField]
    public HeroesData heroesData;
    [SerializeField]
    public EquipmentSetData equipmentSetData;
    [SerializeField]
    public EquipmentData equipmentsData;
    [SerializeField]
    public TranslationData translationData;
    [SerializeField]
    public LanguagesData languagesData;
    [SerializeField]
    public ItemsData itemsData;

}
