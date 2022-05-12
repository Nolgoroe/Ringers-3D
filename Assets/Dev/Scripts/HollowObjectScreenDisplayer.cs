using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class HollowObjectScreenDisplayer : MonoBehaviour, IPointerDownHandler
{
    public Transform craftingMatZone;
    public GameObject materialPrefabHollowScreen;
    public Image objectIcon;
    public HollowCraftObjectData objectData;

    public List<string> materialCountPairs;
    public List<CraftingMatsNeeded> craftingMatsForEquipment;

    public bool canCraft;

    public List<CraftingMatsNeededToRubies> craftingMatsToRubiesHollow;

    public int rubiesNeededToBuyHollow;

    public Button craftButton;

    public HollowZoneSlot connectedZoneSlot;

    //private void Start()
    //{
    //    craftButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));
    //    craftButton.onClick.AddListener(() => UIManager.Instance.OpenForgeImmidietly(objectData.objectname));
    //    //craftButton.onClick.AddListener(() => CraftHollowObject(false));
    //}

    public void SpawnMaterialsNeeded(string matList)
    {
        materialCountPairs = new List<string>();

        string[] temp = matList.Split('|'); /// Split by the | Char

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = temp[i].Trim();  //remove the blank spaces
            materialCountPairs.Add(temp[i]);
        }

        foreach (string item in materialCountPairs)
        {
            GameObject go = Instantiate(materialPrefabHollowScreen, craftingMatZone);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            string[] matAndCount = item.Split('-'); /// Split by the - Char

            for (int i = 0; i < matAndCount.Length; i++)
            {
                matAndCount[i] = matAndCount[i].Trim();  //remove the blank spaces
            }

            string nameOfMat = matAndCount[0].Replace(" ", string.Empty);

            CraftingMats parsed_enum = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), nameOfMat);

            CMD.SetImageAndMaterialCount(LootManager.Instance.allMaterialSprites[(int)parsed_enum], parsed_enum, matAndCount[1]);

            SetDataMatsNeeded(nameOfMat, Convert.ToInt16(matAndCount[1]), CMD, parsed_enum);
        }


        CheckIfCanCraftHollowObject(craftingMatsForEquipment);
    }


    public void SetDataMatsNeeded(string nameOfMat, int amountOfMat, CraftingMatDisplayer CMD, CraftingMats parsed_enum)
    {
        CraftingMatsNeeded CMN = new CraftingMatsNeeded();

        CMN.mat = parsed_enum;
        CMN.amount = amountOfMat;

        craftingMatsForEquipment.Add(CMN);

        CMD.CheckIfHasEnough(parsed_enum, amountOfMat);
    }

    public void CheckIfCanCraftHollowObject(List<CraftingMatsNeeded> CMN)
    {
        canCraft = true;

        foreach (CraftingMatsNeeded CM in CMN)
        {
            if (PlayerManager.Instance.CheckIfHasMaterialts(CM, out int neededAmount) > 0)
            {
                canCraft = false;
                AddNeededCraftingMatsToRubiesHollowItem(CM.mat, neededAmount);
                //break;
            }
        }

        if (!canCraft)
        {
            CalculateNeededRubiesToBuyHollow();
        }
    }


    public void AddNeededCraftingMatsToRubiesHollowItem(CraftingMats _mat, int _amount)
    {
        CraftingMatsNeededToRubies CMNTR = new CraftingMatsNeededToRubies(_mat, _amount);

        craftingMatsToRubiesHollow.Add(CMNTR);
    }

    public void CalculateNeededRubiesToBuyHollow()
    {
        rubiesNeededToBuyHollow = 0;

        foreach (CraftingMatsNeededToRubies CMNTR in craftingMatsToRubiesHollow)
        {
            if (CMNTR.mat == CraftingMats.DewDrops)
            {
                for (int i = 0; i < CMNTR.amountMissing; i++)
                {
                    rubiesNeededToBuyHollow += PlayerManager.Instance.priceInGemsDewDrops;
                }
            }
            else
            {
                CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == CMNTR.mat).Single();

                for (int i = 0; i < CMNTR.amountMissing; i++)
                {
                    rubiesNeededToBuyHollow += CME.amountPerPurchaseGems;
                }
            }
        }
    }


    public void CraftHollowObject(bool isBought) ///// Here because the forge button and resources data are local
    {
        if (PlayerManager.Instance.ownedHollowObjects.Count > 0)
        {
            HollowCraftObjectData HCOD = PlayerManager.Instance.ownedHollowObjects.Where(p => p.hollowItemEnum == objectData.hollowItemEnum).SingleOrDefault();

            if (HCOD != null)
            {
                Debug.LogError("Already OWN this hollow item!");
                return;
            }
        }

        if (HollowManagerSaveData.Instance.filledHollowItemsToIndex.Count > 0)
        {
            FilledItemAndZoneIndex FIAZI = HollowManagerSaveData.Instance.filledHollowItemsToIndex.Where(p => p.hollowItem == objectData.hollowItemEnum).SingleOrDefault();

            if (FIAZI != null)
            {
                Debug.LogError("Already PLACED this hollow item!");
                return;
            }
        }

        if (isBought)
        {
            //PlayerManager.Instance.ownedHollowObjects.Add(objectData);

            //HollowCraftAndOwnedManager.Instance.RefreshHollowObjects();
            //HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

            //PlaceCraftedItemImmidietly();

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.HollowManager});

            return;
        }


        if (canCraft)
        {
            if (!isBought)
            {
                foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
                {
                    Debug.Log(CMN.mat.ToString());
                    Debug.Log(CMN.amount);

                    PlayerManager.Instance.DecreaseNumOfMats(CMN);
                }
            }

            //PlayerManager.Instance.ownedHollowObjects.Add(objectData);

            //PlaceCraftedItemImmidietly();

            //HollowCraftAndOwnedManager.Instance.RefreshHollowObjects();
            //HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();


            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.HollowManager });
        }
        else
        {
            if (!UIManager.Instance.buyHollowItemSecondaryDisplay.gameObject.activeInHierarchy) /////// Find a way to do this better
            {
                HollowCraftAndOwnedManager.Instance.currentlyToCraftSecondMethod = this;

                bool canBuy = false;

                if (PlayerManager.Instance.rubyCount >= rubiesNeededToBuyHollow)
                {
                    canBuy = true;
                }

                UIManager.Instance.DisplayHollowScreenSecondaryRubyCostText(rubiesNeededToBuyHollow, canBuy);
                UIManager.Instance.DisplayBuySecondaryHollowItemNeeded(craftingMatsToRubiesHollow);
                UIManager.Instance.DisplayBuyHollowSecondaryScreen();
            }

        }
        //SortMaster.Instance.RefreshAllScreens();

        //PlayerManager.Instance.SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();

    }

    public void PlaceCraftedItemImmidietly()
    {
        connectedZoneSlot.PlaceHollowObject(objectData.indexInHollow, objectData.hollowItemEnum);


        HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            return;
        }
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);
        UIManager.Instance.OpenForgeImmidietly(objectData.objectname);
    }
}
