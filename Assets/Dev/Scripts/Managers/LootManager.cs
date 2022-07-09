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
    ScarletPimpernels,
    PeppermintLeaves,
    RockClump,
    VervainBlossom,
    AmberShard,
    DandelionRoot,
    CloverSeeds,
    NettleTears,
    RedClay,
    Feverfew,
    JadeShard,
    SilverNugget,
    Colsfoot,
    SapphireShard,
    MoonDust,
    Amethyst,
    BlueFelt,
    JuniperNeedles,
    CloudBerry,
    ElderStalks,
    BlueQuartzCluster,
    BirchBark,
    Limestone,
    SilkThread,
    PoppyPetals,
    CherryBark,
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
    LT1,
    LT2,
    LT3,
    LT4,
    LT5,
    LT6,
    LT7,
    LT8,
    LT9,
    LT10,
    LT11,
    LT12,
    LT13,
    LT14,
    LT15,
    LT16,
    LT17,
    LT18,
    LT19,
    LT20,
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
    L5,
    L6,
    L7,
    L8,
    L9,
    L10,
    L11,
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

    public bool finishedGivingLoot;

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
    
    public IEnumerator GiveLoot()
    {
        Debug.Log("GIVING LOOT");

        if (giveKey)
        {
            Instantiate(keyPrefab, GameManager.Instance.destroyOutOfLevel);
            ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey = true;

            if (ZoneManagerHelpData.Instance.nextZoneCheck)
            {
                ZoneManagerHelpData.Instance.nextZoneCheck.isUnlocked = true;
                ZoneManagerHelpData.Instance.nextZoneCheck.maxLevelReachedInZone = 1;
                ZoneManager.Instance.unlockedZoneID.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id);
                ZoneManager.Instance.UpdateMaxZoneIdReached(ZoneManagerHelpData.Instance.nextZoneCheck.id);

                if (ZoneManagerHelpData.Instance.nextZoneCheck.id == 1)
                {
                    ZoneManager.Instance.hasStartedVinebloom = true;
                    UIManager.Instance.TurnOnLeaderboardButtons();
                }

                //ZoneManagerHelpData.Instance.nextZoneCheck.zoneHeader.sprite = Resources.Load<Sprite>(ZoneManagerHelpData.Instance.nextZoneCheck.unlockedZonePath);

                //UIManager.Instance.UnlockZoneFirstTime(ZoneManagerHelpData.Instance.nextZoneCheck.id);

                //ZoneManager.Instance.unlockedZoneID.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id); //// this needs to move to zone manager (WIP)
                ZoneManager.Instance.zonesToUnlock.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id);

                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { });

                //ZoneManager.Instance.SaveZoneManager();
                //ZoneManagerHelpData.Instance.currentZoneCheck.SaveZone();
                //ZoneManagerHelpData.Instance.nextZoneCheck.SaveZone();
            }

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneManager, SystemsToSave.AllZones });

            giveKey = false;
        }

        if (rubiesToRecieveInLevel > 0)
        {
            StartCoroutine(DisplayLootGoldRubyToPlayer(rubiesToRecieveInLevel, rubySprite));
            PlayerManager.Instance.AddRubies(rubiesToRecieveInLevel);

            Debug.Log("Rubies to recieve " + rubiesToRecieveInLevel);

            SoundManager.Instance.PlaySound(Sounds.ItemPop);
            yield return new WaitForSeconds(AnimationManager.instance.DelayBetweenLootAppear);
        }

        if (craftingMatsLootForLevel.Count > 0)
        {
            foreach (LootToRecieve LTR in craftingMatsLootForLevel)
            {
                StartCoroutine(DisplayLootMaterialsToPlayer(LTR.amount, LTR.type));

                PlayerManager.Instance.AddMaterials(LTR.type, LTR.amount); //////// Figure out how to get amount from outside dynamically

                Debug.Log("materials recieved " + LTR.type);
                SoundManager.Instance.PlaySound(Sounds.ItemPop);

                yield return new WaitForSeconds(AnimationManager.instance.DelayBetweenLootAppear);
            }
        }

        finishedGivingLoot = true;
        craftingMatsLootForLevel.Clear();
        tempDataList.Clear();

        //foreach (LootPacks LP in currentLevelLootToGive)
        //{
        //    RollOnTable(LP);
        //}

        //currentLevelLootToGive.Clear();


        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.ZoneManager, SystemsToSave.AllZones });

        //PlayerManager.Instance.SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();

    }

    public void ResetLevelLootData()
    {
        //currentLevelLootToGive.Clear();
        rubiesToRecieveInLevel = 0;
        craftingMatsLootForLevel.Clear();
        tempDataList.Clear();
    }

    public IEnumerator DisplayLootMaterialsToPlayer(int amount, CraftingMats CM)
    {
        GameObject go = Instantiate(lootDisplayPrefab, winScreenLootDisplayContent);

        go.transform.localScale = Vector3.zero;

        CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

        CMD.materialImage.sprite = allMaterialSprites[(int)CM];
        CMD.materialCount.text = amount.ToString();
        CMD.materialCount.color = Color.white;

        Vector3 newSize = new Vector3(AnimationManager.instance.lootScaleTo.x + 0.3f, AnimationManager.instance.lootScaleTo.y + 0.3f, AnimationManager.instance.lootScaleTo.z + 0.3f);

        LeanTween.scale(go, newSize, AnimationManager.instance.timeToScaleLoot).setEaseLinear();

        yield return new WaitForSeconds(AnimationManager.instance.timeToScaleLoot);

        LeanTween.scale(go, AnimationManager.instance.lootScaleTo, AnimationManager.instance.timeToScaleLoot).setEaseLinear();
    }
    public IEnumerator DisplayLootGoldRubyToPlayer(int count, Sprite sprite)
    {
        GameObject go = Instantiate(lootDisplayPrefab, winScreenLootDisplayContent);

        go.transform.localScale = Vector3.zero;

        CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

        CMD.materialImage.sprite = sprite;
        CMD.materialCount.text = count.ToString();
        CMD.materialCount.color = Color.white;

        Vector3 newSize = new Vector3(AnimationManager.instance.lootScaleTo.x + 0.3f, AnimationManager.instance.lootScaleTo.y + 0.3f, AnimationManager.instance.lootScaleTo.z + 0.3f);

        LeanTween.scale(go, newSize, AnimationManager.instance.timeToScaleLoot).setEaseLinear();

        yield return new WaitForSeconds(AnimationManager.instance.timeToScaleLoot);

        LeanTween.scale(go, AnimationManager.instance.lootScaleTo, AnimationManager.instance.timeToScaleLoot).setEaseLinear();
    }

    public void DestoryWinScreenDisplyedLoot()
    {
        foreach (Transform t in winScreenLootDisplayContent)
        {
            Destroy(t.gameObject);
        }
    }
}
