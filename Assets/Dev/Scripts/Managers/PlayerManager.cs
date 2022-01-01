using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

[Serializable]
public class CraftingMatEntry
{
    public CraftingMats mat;
    public CraftingMatType craftingMatType;
    public int amount;
    public int priceInGems;
    public int amountPerPurchaseGems;
}
public class PlayerManager : MonoBehaviour
{
    //public int maxLevel = 1;

    public int /*goldCount,*/ rubyCount;
    public int collectedDewDrops;

    public List<PowerUp> activePowerups;

    public List<CraftingMatEntry> craftingMatsInInventory;

    //public List<EquipmentData> wardrobeEquipment;
    public List<EquipmentData> ownedPowerups;
    public List<EquipmentData> equipmentInCooldown;

    public List<HollowCraftObjectData> ownedHollowObjects;
    public List<CorruptedDevicesData> ownedCorruptDevices;

    public static PlayerManager Instance;

    string path;

    public int highestLevelReached;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

        MaterialsAndForgeManager.Instance.PopulateMaterialBagAll();

        SortMaster.Instance.ClearAllForgeScreens();
        //SortMaster.Instance.RefreshAllScreens();

        HandleItemCooldowns();

        GameManager.Instance.powerupManager.ClearTutorialPowerups();

        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    path = Application.persistentDataPath + "/PlayerSaveData.txt";
        //}
        //else
        //{
        //    path = Application.dataPath + "/Save Files Folder/PlayerSaveData.txt";
        //}

        //if (File.Exists(path))
        //{

        //    //UIManager.Instance.RefreshDewDropsDisplay(collectedDewDrops);
        //}

        //LoadPlayerData();


        //ownedCorruptDevices.Clear();
        //if (wardrobeEquipment.Count > 0)
        //{
        //    foreach (EquipmentData ED in wardrobeEquipment)
        //    {
        //        WardrobeManager.Instance.SpawnWardrobeEquipment(ED);
        //    }
        //}

        //if (ownedPowerups.Count > 0)
        //{
        //    foreach (EquipmentData ED in ownedPowerups)
        //    {
        //        WardrobeManager.Instance.EquipMe(ED);
        //    }
        //}

    }
    public bool CheckIfHasMaterialts(CraftingMatsNeeded CMN)
    {
        if(CMN.mat == CraftingMats.DewDrops)
        {
            int amount = collectedDewDrops;
            return amount >= CMN.amount;
        }
        else
        {
            CraftingMatEntry CME = craftingMatsInInventory.Where(p => p.mat == CMN.mat).Single();
            return CME.amount >= CMN.amount;
        }

    }
    public void DecreaseNumOfMats(CraftingMatsNeeded CMN)
    {
        if (CMN.mat == CraftingMats.DewDrops)
        {
            int currentAmountOfDrops = collectedDewDrops;

            collectedDewDrops -= CMN.amount;

            if(collectedDewDrops < 0)
            {
                Debug.LogError("Something wrong with dew drops counter!");
            }

            UIManager.Instance.dewDropsText.text = collectedDewDrops.ToString();

            if(currentAmountOfDrops >= DewDropsManager.Instance.maxDrops)
            {           
                if(currentAmountOfDrops - CMN.amount < DewDropsManager.Instance.maxDrops)
                {
                    StartCoroutine(DewDropsManager.Instance.DisplayTime());
                }
            }
        }
        else
        {
            CraftingMatEntry CME = craftingMatsInInventory.Where(p => p.mat == CMN.mat).Single();

            CME.amount -= CMN.amount;
        }
    }
    public void AddMaterials(CraftingMats matToAdd, int amountToAdd)
    {
        CraftingMatEntry CME = craftingMatsInInventory.Where(p => p.mat == matToAdd).Single();

        CME.amount += amountToAdd;

        MaterialsAndForgeManager.Instance.RefreshMaterialBag();
        MaterialsAndForgeManager.Instance.RefreshForge();
    }
    //public void AddGold(int amount)
    //{
    //    goldCount += amount;
    //    Debug.Log(amount);
    //    UIManager.Instance.RefreshGoldAndRubyDisplay();
    //}
    public void AddRubies(int amount)
    {
        rubyCount += amount;
        Debug.Log(amount);
        UIManager.Instance.RefreshGoldAndRubyDisplay();
    }
    public void PopulatePowerUps()
    {
        int instantiatedCount = 0;
        activePowerups.Clear();

        if(/*(GameManager.Instance.currentLevel.isTutorial || GameManager.Instance.currentLevel.isSpecificTutorial) && */GameManager.Instance.currentLevel.powerupsForLevel.Length > 0)
        {
            foreach (PowerUp PU in GameManager.Instance.currentLevel.powerupsForLevel)
            {
                EquipmentData ED = GameManager.Instance.csvParser.allEquipmentInGame.Where(p => p.power == PU).Single();

                EquipmentData newData = new EquipmentData(ED.name, ED.power, ED.specificSymbol, ED.specificColor, ED.numOfUses, ED.scopeOfUses,
                                          ED.timeForCooldown, ED.nextTimeAvailable, ED.Description, ED.isTutorialPower, ED.mats, ED.spritePath);

                EquipMeTutorial(newData);
            }
        }

        foreach (EquipmentData ED in ownedPowerups)
        {
            if(instantiatedCount < GameManager.Instance.powerupManager.instnatiateZones.Length)
            {
                instantiatedCount++;
                activePowerups.Add(ED.power);

                GameManager.Instance.powerupManager.InstantiatePowerUps(ED);
            }
        }
    }

    //[ContextMenu("Save")]
    //public void SavePlayerData()
    //{
    //    //Debug.Log("Saved Player");
    //    //string savedData = JsonUtility.ToJson(this);

    //    //PlayfabManager.instance.SavePlayerDataJsonServer(savedData);
    //}

    //[ContextMenu("Load")]
    //public void LoadPlayerData()
    //{
    //    //PlayfabManager.instance.LoadAllData();

    //    //Instance = this;

    //}

    public void HandleItemCooldowns()
    {
        List<EquipmentData> toRemove = new List<EquipmentData>();

        foreach (EquipmentData ED in equipmentInCooldown)
        {
            DateTime currentTime = DateTime.Now.ToLocalTime();

            TimeSpan deltaDateTime = DateTime.Parse(ED.nextTimeAvailable) - currentTime;


            if (deltaDateTime <= TimeSpan.Zero)
            {
                toRemove.Add(ED);

                bool foundEquipped = SearchEquippedItemsForMatch(ED);


                if (!foundEquipped)
                {
                    SearchWardrobeForMatch(ED);
                }

                Debug.Log("Cooldown Done! " + ED.name);

            }
        }

        foreach (EquipmentData ED in toRemove)
        {
            equipmentInCooldown.Remove(ED);
        }
    }

    [ContextMenu("Reset Player Mats")]
    public void ResetPlayerMats()
    {
        foreach (CraftingMatEntry CME in craftingMatsInInventory)
        {
            CME.amount = 0;
        }

        //PlayfabManager.instance.SaveAllGameData();

        SortMaster.Instance.ClearAllForgeScreens();
        //SortMaster.Instance.RefreshAllScreens();
    }

    [ContextMenu("Add 100 Player Mats")]
    public void AddPlayerMats()
    {
        foreach (CraftingMatEntry CME in craftingMatsInInventory)
        {
            CME.amount += 100;
        }

        //SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();

        //MaterialsAndForgeManager.Instance.RefreshMaterialBag();
        //MaterialsAndForgeManager.Instance.RefreshForge();
        //HollowCraftAndOwnedManager.Instance.RefreshHollowObjects();

        SortMaster.Instance.ClearAllForgeScreens();
        //SortMaster.Instance.RefreshAllScreens();

        PlayfabManager.instance.SaveAllGameData();
    }

    public bool SearchEquippedItemsForMatch(EquipmentData ED)
    {
        foreach (EquipmentData edEquipped in ownedPowerups)
        {
            if (ED.name == edEquipped.name)
            {
                edEquipped.nextTimeAvailable = "";
                return true;
            }
        }

        return false;
    }
    public void SearchWardrobeForMatch(EquipmentData ED)
    {
        //foreach (EquipmentData edWardrobe in wardrobeEquipment)
        //{
        //    if (ED.name == edWardrobe.name)
        //    {
        //        edWardrobe.nextTimeAvailable = "";
        //        return;
        //    }
        //}
    }

    public void SpawnOwnedCorruptionDevices()
    {
        UIManager.Instance.ownedCorruptDevicesZone.gameObject.SetActive(true);
        int num = 1;

        foreach (CorruptedDevicesData CDD in ownedCorruptDevices)
        {
            GameObject go = Instantiate(CorruptedZonesManager.Instance.generalDevicePrefab, UIManager.Instance.ownedCorruptDevicesZone);

            go.name = "D" + num;

            tempMoveScript TMS = go.GetComponent<tempMoveScript>();

            TMS.connectedCDD = CDD;

            UIManager.Instance.SetCorruptedDeviceImage(TMS);

            num++;
        }
    }

    public void EquipMe(EquipmentData data/*, WardrobeEquipmentDisplayer displayer*/)
    {
        //if (!slotToSpot[data.slot].isFull)
        //{
        //    slotToSpot[data.slot].isFull = true;

        //    slotToSpot[data.slot].equipmentInSlot = data;

        //    equipmentInWardrobe.Remove(displayer);
        //    PlayerManager.Instance.wardrobeEquipment.Remove(data);

        //    Destroy(displayer.gameObject);

        //    GameObject go = Instantiate(equippedPrefab, slotToSpot[data.slot].transform);

        //    go.GetComponentInChildren<RawImage>().texture = Resources.Load(data.spritePath) as Texture2D;

        ownedPowerups.Add(data);

        //SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();
        //}
    }

    public void EquipMeTutorial(EquipmentData data)
    {
        data.isTutorialPower = true;
        ownedPowerups.Add(data);
    }

    public void UpdateMaxLevelReached(LevelScriptableObject level)
    {
        highestLevelReached = level.levelNum;

        PlayfabManager.instance.SendLeaderboard(highestLevelReached);

        //SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();
    }
}
