using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public int goldCount, rubyCount;

    public List<PowerUp> activePowerups;
    public List<PieceSymbol> specificSymbol;
    public List<PieceColor> specificColor; ///// ask alon if he has any idea for a better way

    public List<CraftingMatEntry> craftingMatsInInventory;

    public List<EquipmentData> wardrobeEquipment;
    public List<EquipmentData> equippedItems;

    public static PlayerManager Instance;
    private void Start()
    {
        Instance = this;

        MaterialsAndForgeManager.Instance.PopulateMaterialBag();

        UIManager.Instance.RefreshGoldAndRubyDisplay();
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
        UIManager.Instance.RefreshGoldAndRubyDisplay();
    }
    public void AddRubies(int amount)
    {
        rubyCount += amount;
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
}
