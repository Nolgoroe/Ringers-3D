using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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

[System.Serializable]
public class ListLootChest
{
    public List<LootPacks> lootPacksForChest;
}
public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    public GameObject keyPrefab;

    public GameObject lootDisplayPrefab;
    public GameObject lootDisplayPrefabChest;
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

    //TEMP
    public List<ListLootChest> chestLootPacks;
    public int rubiesToGiveChest;
    public List<LootToRecieve> materialsToGiveChest;
    public List<CraftingMats> tempDataListChest;

    [Header("Chest Loot Animations")]
    public Transform[] chestLootPosition;
    public Transform parentChestLoot;
    public float animationSpeedChestLoot;
    public float timeBetweenLoots;
    public float openChestTime;
    private int currentChestLootPos;

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
                    //UIManager.Instance.TurnOnLeaderboardButtons();
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

            //giveKey = false;
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

                pointsOfInterestSaveData.instance.AddToPointsOfInterest(LTR.type);

                yield return new WaitForSeconds(AnimationManager.instance.DelayBetweenLootAppear);
            }

            if(TutorialSaveData.Instance.hasFinishedPotion)
            {
                InterestPointsManager.instance.TurnOnPointsOfInterestDisplay(TypesPointOfInterest.inventory);
            }

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.InterestPontSaveData});
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

    public void CheckGiveKey()
    {
        if(giveKey)
        {
            //Instantiate(keyPrefab, GameManager.Instance.destroyOutOfLevel);

            giveKey = false;
        }
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

    public void DisplayLootFromChest(int count, Sprite sprite)
    {
        currentChestLootPos++;

        GameObject go = Instantiate(lootDisplayPrefabChest, parentChestLoot);

        craftingMatDisplayerChestLoot CMD = go.GetComponent<craftingMatDisplayerChestLoot>();

        CMD.spriteRenderer.sprite = sprite;
        CMD.materialCount.text = count.ToString();
        CMD.materialCount.color = Color.white;

        LeanTween.move(go, chestLootPosition[currentChestLootPos], animationSpeedChestLoot).setOnComplete(() => go.transform.parent = chestLootPosition[currentChestLootPos].transform);
        SoundManager.Instance.PlaySound(Sounds.GiveChestItem);
    }

    public void DisplayLootFromChest(int count, CraftingMats CM)
    {
        currentChestLootPos++;

        if(currentChestLootPos >= chestLootPosition.Length)
        {
            currentChestLootPos -= currentChestLootPos;
        }

        GameObject go = Instantiate(lootDisplayPrefabChest, parentChestLoot);

        craftingMatDisplayerChestLoot CMD = go.GetComponent<craftingMatDisplayerChestLoot>();

        CMD.spriteRenderer.sprite = allMaterialSprites[(int)CM];
        CMD.materialCount.text = count.ToString();
        CMD.materialCount.color = Color.white;

        LeanTween.move(go, chestLootPosition[currentChestLootPos], animationSpeedChestLoot).setOnComplete(() => go.transform.parent = chestLootPosition[currentChestLootPos].transform);
        SoundManager.Instance.PlaySound(Sounds.GiveChestItem);
    }

    public void DestoryWinScreenDisplyedLoot()
    {
        foreach (Transform t in winScreenLootDisplayContent)
        {
            Destroy(t.gameObject);
        }
    }

    [ContextMenu("Unpack Chest")]
    public void UnpackChestLoot()
    {
        currentChestLootPos = -1;

        foreach (ListLootChest listLoot in chestLootPacks)
        {
            foreach (LootPacks lootPack in listLoot.lootPacksForChest)
            {
                switch (lootPack.ToString()[0])
                {
                    case 'R':
                        UnpackToRubiesChest(lootPack);
                        break;

                    case 'L':
                        UnpackToMaterialsChest(lootPack);
                        break;

                    default:
                        Debug.LogError("Error in chest loot here");
                        break;
                }
            }

        }


        StartCoroutine(GiveLootFromChest());
    }

    private void UnpackToRubiesChest(LootPacks pack)
    {
        int[] valuesToRecieve;

        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = lootpackEnumToRewardBag[pack];

        valuesToRecieve = rewardBagByLootPack.minMaxValues;

        int randomNum = UnityEngine.Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1);

        rubiesToGiveChest += randomNum;
    }

    private void UnpackToMaterialsChest(LootPacks pack)
    {
        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = lootpackEnumToRewardBag[pack];

        List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();

        for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
        {
            craftingMatsFromTables.AddRange(itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

            int chance = UnityEngine.Random.Range(1, 101);

            if (chance > rewardBagByLootPack.chancesPerItemTable[i])
            {
                craftingMatsFromTables.Clear();
            }
            else
            {
                int randomMat = UnityEngine.Random.Range(0, craftingMatsFromTables.Count);

                LootToRecieve LTR = new LootToRecieve(craftingMatsFromTables[randomMat], UnityEngine.Random.Range(1, 6));

                if (!tempDataListChest.Contains(LTR.type))
                {
                    materialsToGiveChest.Add(LTR);
                    tempDataListChest.Add(LTR.type);
                }
                else
                {
                    LootToRecieve LTR_exsists = materialsToGiveChest.Where(p => p.type == LTR.type).Single();
                    LTR_exsists.amount += LTR.amount;
                }
            }
        }
    }

    private IEnumerator GiveLootFromChest()
    {
        bool gaveLoot = false;

        yield return new WaitForSeconds(openChestTime);

        if (rubiesToGiveChest > 0)
        {
            gaveLoot = true;

            //display the loot
            //StartCoroutine(DisplayLootGoldRubyToPlayer(rubiesToRecieveInLevel, rubySprite));

            //DisplayLootFromChest(rubiesToGiveChest, rubySprite);
            //SoundManager.Instance.PlaySound(Sounds.ItemPop);

            PlayerManager.Instance.AddRubies(rubiesToGiveChest);

            Debug.LogError("Rubies Recieved chest: " + rubiesToGiveChest);

            yield return new WaitForSeconds(timeBetweenLoots);
        }

        if (materialsToGiveChest.Count > 0)
        {
            gaveLoot = true;

            foreach (LootToRecieve LTR in materialsToGiveChest)
            {
                //display the loot
                //StartCoroutine(DisplayLootMaterialsToPlayer(LTR.amount, LTR.type));

                //DisplayLootFromChest(LTR.amount, LTR.type);
                //SoundManager.Instance.PlaySound(Sounds.ItemPop);

                PlayerManager.Instance.AddMaterials(LTR.type, LTR.amount); //////// Figure out how to get amount from outside dynamically

                Debug.LogError("materials recieved chest: " + LTR.type);

                pointsOfInterestSaveData.instance.AddToPointsOfInterest(LTR.type);

                yield return new WaitForSeconds(timeBetweenLoots);
            }

            if (TutorialSaveData.Instance.hasFinishedPotion)
            {
                InterestPointsManager.instance.TurnOnPointsOfInterestDisplay(TypesPointOfInterest.inventory);
            }

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.InterestPontSaveData });
        }

        if(gaveLoot)
        {
            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }

        //TestLevelsSystemManagerSaveData.instance.ResetData();

        CheckGiveKey();
    }

    public IEnumerator DisplayLootFromChest()
    {
        yield return new WaitForSeconds(1.1f);
        if (rubiesToGiveChest > 0)
        {
            DisplayLootFromChest(rubiesToGiveChest, rubySprite);
            SoundManager.Instance.PlaySound(Sounds.ItemPop);

            yield return new WaitForSeconds(timeBetweenLoots);
        }

        if (materialsToGiveChest.Count > 0)
        {
            foreach (LootToRecieve LTR in materialsToGiveChest)
            {
                DisplayLootFromChest(LTR.amount, LTR.type);
                SoundManager.Instance.PlaySound(Sounds.ItemPop);

                yield return new WaitForSeconds(timeBetweenLoots);
            }

        }

        parentChestLoot.GetComponent<Animator>().SetTrigger("FinishedLootDisplay");
        parentChestLoot = null;

        yield return new WaitForSeconds(0.1f);

        SoundManager.Instance.PlaySound(Sounds.ChestClose);

        materialsToGiveChest.Clear();
        rubiesToGiveChest = 0;
        currentChestLootPos = 0;
        tempDataListChest.Clear();

        yield return new WaitForSeconds(2f);

        TutorialSequence.Instacne.CheckDoPotionTutorial();
        TutorialSequence.Instacne.CheckDoAnimalAlbumTutorial();

        if (GameManager.Instance.currentLevel.powerupsForLevel.Count() > 0)
        {
            MaterialsAndForgeManager.Instance.UnlockPotion(GameManager.Instance.currentLevel.powerupsForLevel[0]);
        }
    }
    public void DestroyAllChestLootData()
    {
        foreach (Transform lootPos in chestLootPosition)
        {
            for (int i = 0; i < lootPos.childCount; i++)
            {
                Destroy(lootPos.GetChild(i).gameObject);
            }
        }

        chestLootPacks = null;
    }
}
