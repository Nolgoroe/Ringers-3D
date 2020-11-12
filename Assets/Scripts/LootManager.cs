using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LootPacks ////// Names of loot packs containing loot packs
{
    None,
    M1,
    M2,
    M3,
    R1,
    R2,
    I1,
    I2,
    I3,
    I4,
    I5,
    I6,
    I7
}

[System.Serializable]
public enum ItemTables
{
    None,
    L1,
    L2,
    L3,
    L4,
    L5,
    L6,
    L7
}

[System.Serializable]

public class RewardBag /// Class containing the values for lootpakcs (The loot latbles for each pack, OR the integer values for money and rubies).
{
    public bool IsMoneyOrRubies; ///// Differenciates between (Money & Rubies) Or Items.

    public List<ItemTables> Pack;

    public int[] minMaxValues;

    public int[] chancesPerItemTable;
}

[System.Serializable]
public class LootPackChancesList
{
    public int[] minMaxValues; ///// Change this naming to Percentage Num
}

[System.Serializable]
public class ItemTablesToCraftingMats
{
    public List<CraftingMats> tableToMat;
}

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    public GameObject keyPrefab;

    public bool giveKey;

    [Header("List for Loot by Level")]
    /////// LIST FOR LOOT BY LEVEL
    public List<ItemTablesToCraftingMats> listOfTablesToMats;

    [Header("List for Reward Bags")]
    /////// LIST FOR REWARDBAGS
    public List<RewardBag> listOfRewardBags;

    [Header("During Gameplay")]
    public List<LootPacks> currentLevelLootToGive;

    public Dictionary<LootPacks, RewardBag> lootpackEnumToRewardBag;

    public Dictionary<ItemTables, List<CraftingMats>> itemTableToListOfMats;

    private void Start()
    {
        Instance = this;

        currentLevelLootToGive = new List<LootPacks>();
        itemTableToListOfMats = new Dictionary<ItemTables, List<CraftingMats>>();
        lootpackEnumToRewardBag = new Dictionary<LootPacks, RewardBag>();

        for (int i = 1; i < System.Enum.GetValues(typeof(LootPacks)).Length; i++)
        {
            lootpackEnumToRewardBag.Add((LootPacks)i, listOfRewardBags[i - 1]);
        }

        for (int i = 1; i < System.Enum.GetValues(typeof(ItemTables)).Length; i++)
        {
            itemTableToListOfMats.Add((ItemTables)i, listOfTablesToMats[i - 1].tableToMat);
        }
    }
    
    public void GiveLoot()
    {
        foreach (LootPacks LP in currentLevelLootToGive)
        {
            RollOnTable(LP);
        }

        currentLevelLootToGive.Clear();

        if (giveKey)
        {
            Instantiate(keyPrefab, GameManager.Instance.destroyOutOfLevel);
            ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey = true;
            ZoneManagerHelpData.Instance.nextZoneCheck.isUnlocked = true;
            ZoneManagerHelpData.Instance.nextZoneCheck.maxLevelReachedInZone = 1;
            ZoneManagerHelpData.Instance.nextZoneCheck.zoneHeader.sprite = ZoneManagerHelpData.Instance.nextZoneCheck.unlockedZone;

            ZoneManager.Instance.unlockedZoneID.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id);
        }

        giveKey = false;
    }
    public void RollOnTable(LootPacks lootPack)
    {
        if (lootPack == LootPacks.None)
        {
            return;
        }

        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = lootpackEnumToRewardBag[lootPack];

        if (!rewardBagByLootPack.IsMoneyOrRubies)
        {
            List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();


            for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
            {
                craftingMatsFromTables.AddRange(itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

                int chance = Random.Range(1, 101);

                if (chance > rewardBagByLootPack.chancesPerItemTable[i])
                {
                    Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
                    craftingMatsFromTables.Clear();
                }
                else
                {
                    int randomMat = Random.Range(0, craftingMatsFromTables.Count);

                    Debug.Log(craftingMatsFromTables[randomMat]);
                    PlayerManager.Instance.AddMaterials(craftingMatsFromTables[randomMat], 5); //////// Figure out how to get amount from outside dynamically

                    craftingMatsFromTables.Clear();
                }
            }
        }
        else
        {
            int[] valuesToRecieve;
            valuesToRecieve = rewardBagByLootPack.minMaxValues;

            if (lootPack.ToString().Contains("M"))
            {
                PlayerManager.Instance.AddGold(Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));
            }
            else
            {
                PlayerManager.Instance.AddRubies(Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));
            }

        }
    }


    public void ResetLevelLootData()
    {
        currentLevelLootToGive.Clear();
    }
}
