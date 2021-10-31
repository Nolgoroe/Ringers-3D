using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public enum CraftingMats
{
    DawnDew,
    Wood,
    TreeSap,
    GrassBlades,
    Wax,
    MossFleck,
    Scarletpimpernel,
    PeppermintLeaves,
    RockClump,
    AmberShard,
    CloverSeeds,
    Feverfew,
    MoonDust,
    Amethyst,
    ElderStalks,
    SilkThread,
    MoonStone,
    DewDrops,
    None
}
public enum CraftingMatType
{
    None,
    Build,
    Gem,
    Herb,
    Witchcraft
}

public enum LootPacks ////// Names of loot packs containing loot packs
{
    None,
    //M1,
    //M2,
    //M3,
    R1,
    R2,
    R3,
    R4,
    R5,
    I1,
    I2,
    I3,
    I4,
    I5,
    I6,
    I7,
    I8,
    I9,
    I10,
    //I11,
    //I12,
    //I13,
    //I14,
    //I15
}

[System.Serializable]
public enum ItemTables
{
    None,
    L1,
    L2,
    L3,
    L4,
    //L5,
    //L6,
    //L7,
    //L8,
    //L9,
    //L10,
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
public class ItemTablesToCraftingMats
{
    public List<CraftingMats> tableToMat;
}

[System.Serializable]
public class LootToRecieve
{
    public CraftingMats type;
    public int amount;


    public LootToRecieve(CraftingMats type_In, int amount_In)
    {
        type = type_In;
        amount = amount_In;
    }
}

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    public GameObject keyPrefab;

    public GameObject lootDisplayPrefab;
    public Transform winScreenLootDisplayContent;

    public Sprite /*goldSprite,*/ rubySprite;
    public Sprite dewDropsSprite;

    public Sprite[] allMaterialSprites;

    public bool giveKey;

    [Header("List for Loot by Level")]
    /////// LIST FOR LOOT BY LEVEL
    public List<ItemTablesToCraftingMats> listOfTablesToMats;

    [Header("List for Reward Bags")]
    /////// LIST FOR REWARDBAGS
    public List<RewardBag> listOfRewardBags;

    //[Header("During Gameplay")]
    //public List<LootPacks> currentLevelLootToGive;

    public Dictionary<LootPacks, RewardBag> lootpackEnumToRewardBag;

    public Dictionary<ItemTables, List<CraftingMats>> itemTableToListOfMats;

    public List<LootToRecieve> craftingMatsLootForLevel;

    public List<CraftingMats> tempDataList;

    public int rubiesToRecieveInLevel;
    private void Start()
    {
        Instance = this;

        craftingMatsLootForLevel = new List<LootToRecieve>();
        tempDataList = new List<CraftingMats>();
        //currentLevelLootToGive = new List<LootPacks>();
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
        Debug.Log("GIVING LOOT");

        if(rubiesToRecieveInLevel > 0)
        {
            DisplayLootGoldRubyToPlayer(rubiesToRecieveInLevel, rubySprite);
            PlayerManager.Instance.AddRubies(rubiesToRecieveInLevel);

            Debug.Log("Rubied to recieve " + rubiesToRecieveInLevel);
        }
        
        if(craftingMatsLootForLevel.Count > 0)
        {
            foreach (LootToRecieve LTR in craftingMatsLootForLevel)
            {
                DisplayLootMaterialsToPlayer(LTR.amount, LTR.type);

                PlayerManager.Instance.AddMaterials(LTR.type, LTR.amount); //////// Figure out how to get amount from outside dynamically

                Debug.Log("materials recieved " + LTR.type);
            }
        }

        craftingMatsLootForLevel.Clear();
        LootManager.Instance.tempDataList.Clear();

        //foreach (LootPacks LP in currentLevelLootToGive)
        //{
        //    RollOnTable(LP);
        //}

        //currentLevelLootToGive.Clear();

        if (giveKey)
        {
            Instantiate(keyPrefab, GameManager.Instance.destroyOutOfLevel);
            ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey = true;

            if (ZoneManagerHelpData.Instance.nextZoneCheck)
            {
                ZoneManagerHelpData.Instance.nextZoneCheck.isUnlocked = true;
                ZoneManagerHelpData.Instance.nextZoneCheck.maxLevelReachedInZone = 1;

                //ZoneManagerHelpData.Instance.nextZoneCheck.zoneHeader.sprite = Resources.Load<Sprite>(ZoneManagerHelpData.Instance.nextZoneCheck.unlockedZonePath);

                //UIManager.Instance.UnlockZoneFirstTime(ZoneManagerHelpData.Instance.nextZoneCheck.id);

                //ZoneManager.Instance.unlockedZoneID.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id); //// this needs to move to zone manager (WIP)
                ZoneManager.Instance.zonesToUnlock.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id);


                ZoneManager.Instance.SaveZoneManager();
                ZoneManagerHelpData.Instance.currentZoneCheck.SaveZone();
                ZoneManagerHelpData.Instance.nextZoneCheck.SaveZone();
            }

            giveKey = false;
        }

        PlayerManager.Instance.SavePlayerData();

    }
    public void RollOnTable(LootPacks lootPack)
    {
        //if (lootPack == LootPacks.None)
        //{
        //    return;
        //}

        //RewardBag rewardBagByLootPack = new RewardBag();

        //rewardBagByLootPack = lootpackEnumToRewardBag[lootPack];

        //if (!rewardBagByLootPack.IsMoneyOrRubies)
        //{
            //foreach (CraftingMats CM in craftingMatsLootForLevel)
            //{
            //    DisplayLootMaterialsToPlayer(5, CM);

            //    PlayerManager.Instance.AddMaterials(CM, 5); //////// Figure out how to get amount from outside dynamically

            //}

            //craftingMatsLootForLevel.Clear();
            //List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();


            //for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
            //{
            //    craftingMatsFromTables.AddRange(itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

            //    int chance = Random.Range(1, 101);

            //    if (chance > rewardBagByLootPack.chancesPerItemTable[i])
            //    {
            //        Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
            //        craftingMatsFromTables.Clear();
            //    }
            //    else
            //    {
            //        int randomMat = Random.Range(0, craftingMatsFromTables.Count);

            //        Debug.Log(craftingMatsFromTables[randomMat]);

            //        DisplayLootMaterialsToPlayer(5, craftingMatsFromTables[randomMat]);

            //        PlayerManager.Instance.AddMaterials(craftingMatsFromTables[randomMat], 5); //////// Figure out how to get amount from outside dynamically

            //        craftingMatsFromTables.Clear();
            //    }
            //}
        //}
        //else
        //{
            //int[] valuesToRecieve;
            //valuesToRecieve = rewardBagByLootPack.minMaxValues;


            //int randomNum = (Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));

            //if (lootPack.ToString().Contains("M"))
            //{
            //    DisplayLootGoldRubyToPlayer(randomNum, goldSprite.texture);
            //    PlayerManager.Instance.AddGold(randomNum);
            //}
            //else
            //{
                //DisplayLootGoldRubyToPlayer(randomNum, rubySprite.texture);
                //PlayerManager.Instance.AddRubies(randomNum);
            //}

        //}
    }

    public void ResetLevelLootData()
    {
        //currentLevelLootToGive.Clear();
        rubiesToRecieveInLevel = 0;
        craftingMatsLootForLevel.Clear();
        LootManager.Instance.tempDataList.Clear();
    }

    public void DisplayLootMaterialsToPlayer(int amount, CraftingMats CM)
    {
        GameObject go = Instantiate(lootDisplayPrefab, winScreenLootDisplayContent);

        CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

        CMD.materialImage.sprite = allMaterialSprites[(int)CM];
        CMD.materialCount.text = amount.ToString();
    }
    public void DisplayLootGoldRubyToPlayer(int count, Sprite sprite)
    {
        GameObject go = Instantiate(lootDisplayPrefab, winScreenLootDisplayContent);

        CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

        CMD.materialImage.sprite = sprite;
        CMD.materialCount.text = count.ToString();

        ////Tutorial Level 2
    }

    public void DestoryWinScreenDisplyedLoot()
    {
        foreach (Transform t in winScreenLootDisplayContent)
        {
            Destroy(t.gameObject);
        }
    }
}
