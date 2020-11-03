﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

[Serializable]
public enum CraftingMats
{
    None,
    Wood,
    Stone,
    PurpleFlower,
    FireShard,
    Feather,
    Wax,
    LeatherBand,
    RedRibbon,
    SilverNugget,
    Amber,
    BlueFelt,
    MoonStone,
    VenerableOakBranch,
    MoonwellWater,
    ScarletPimpernel,
    Saphire,
}
public enum CraftingMatType
{
    None,
    Basic,
    Magical,
    Textile,
}
[Serializable]
public class CraftingMatEntry
{
    public CraftingMats mat;
    public CraftingMatType craftingMatType;
    public int amount;
}
public class PlayerManager : MonoBehaviour
{
    public int maxLevel = 1;

    public int goldCount, rubyCount;

    public List<PowerUp> activePowerups;

    public List<CraftingMatEntry> craftingMatsInInventory;

    public List<EquipmentData> wardrobeEquipment;
    public List<EquipmentData> equippedItems;

    public static PlayerManager Instance;

    string path;

    private void Start()
    {
        Instance = this;

        MaterialsAndForgeManager.Instance.PopulateMaterialBag();

        UIManager.Instance.RefreshGoldAndRubyDisplay();

        path = Application.dataPath + "/PlayerSaveData.txt";

        if (File.Exists(path))
        {
            LoadPlayerData();

            MaterialsAndForgeManager.Instance.RefreshForge();
            MaterialsAndForgeManager.Instance.RefreshMaterialBag();
        }

        if (wardrobeEquipment.Count > 0)
        {
            foreach (EquipmentData ED in wardrobeEquipment)
            {
                WardrobeManager.Instance.SpawnWardrobeEquipment(ED);
            }
        }

        if(equippedItems.Count > 0)
        {
            foreach (EquipmentData ED in equippedItems)
            {
                WardrobeManager.Instance.EquipMe(ED);
            }
        }
    }

    public bool CheckIfHasMaterialts(CraftingMatsNeeded CMN)
    {
        CraftingMatEntry CME = craftingMatsInInventory.Where(p => p.mat == CMN.mat).Single();

        return CME.amount >= CMN.amount;
    }
    public void DecreaseNumOfMats(CraftingMatsNeeded CMN)
    {
        CraftingMatEntry CME = craftingMatsInInventory.Where(p => p.mat == CMN.mat).Single();

        CME.amount -= CMN.amount;
    }

    public void AddMaterials(CraftingMats matToAdd, int amountToAdd)
    {
        CraftingMatEntry CME = craftingMatsInInventory.Where(p => p.mat == matToAdd).Single();

        CME.amount += amountToAdd;

        MaterialsAndForgeManager.Instance.RefreshMaterialBag();
        MaterialsAndForgeManager.Instance.RefreshForge();
    }

    public void AddGold(int amount)
    {
        goldCount += amount;
        Debug.Log(amount);
        UIManager.Instance.RefreshGoldAndRubyDisplay();
    }
    public void AddRubies(int amount)
    {
        rubyCount += amount;
        Debug.Log(amount);
        UIManager.Instance.RefreshGoldAndRubyDisplay();
    }
    public void PopulatePowerUps()
    {
        foreach (EquipmentData ED in equippedItems)
        {
            activePowerups.Add(ED.power);

            GameManager.Instance.powerupManager.InstantiatePowerUps(ED);
        }
    }

    [ContextMenu("Save")]
    public void SavePlayerData()
    {
       string savedData = JsonUtility.ToJson(this);

        string path = Application.dataPath + "/PlayerSaveData.txt";

        File.WriteAllText(path, savedData);
    }

    [ContextMenu("Load")]
    public void LoadPlayerData()
    {
        string path = Application.dataPath + "/PlayerSaveData.txt";

        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

        Instance = this;
    }
}
