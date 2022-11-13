using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Linq;


public class HollowObjectDisplayer : MonoBehaviour
{
    public Button craftButton;

    public TMP_Text itemName;
    public Image itemImage;

    public Transform ingrediantContentParent;
    public GameObject ingrediantDisplayerPrefab;

    public List<string> materialCountPairs;

    public HollowCraftObjectData objectData;

    public List<CraftingMatsNeeded> craftingMatsForEquipment;
    public List<CraftingMatsNeededToRubies> craftingMatsToRubiesHollow;

    public int rubiesNeededToBuyHollow;

    public GameObject tutorialHole;

    public bool canCraft;
    private void Start()
    {
        craftButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));
        craftButton.onClick.AddListener(() => CraftHollowObject(false));
    }

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
            GameObject go = Instantiate(ingrediantDisplayerPrefab, ingrediantContentParent);

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
        if (TutorialSequence.Instacne.duringSequence)
        {
           if(UIManager.Instance.requiredButtonForTutorialPhase != craftButton)
            {
                return;
            }
        }

        if (PlayerManager.Instance.ownedHollowObjects.Count > 0)
        {
            HollowCraftObjectData HCOD = PlayerManager.Instance.ownedHollowObjects.Where(p => p.hollowItemEnum == objectData.hollowItemEnum).SingleOrDefault();

            if(HCOD != null)
            {
                Debug.LogError("Already OWN this hollow item!");
                return;
            }
        }

        if(HollowManagerSaveData.Instance.filledHollowItemsToIndex.Count > 0)
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
            PlayerManager.Instance.ownedHollowObjects.Add(objectData);

            foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
            {
                Debug.Log(CMN.mat.ToString());
                Debug.Log(CMN.amount);

                PlayerManager.Instance.DecreaseNumOfMats(CMN);
            }

            HollowCraftAndOwnedManager.Instance.RefreshHollowObjects();
            HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

            if (UIManager.Instance.ringersHutDisplay.gameObject.activeInHierarchy) /////// Find a way to do this better
            {
                HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);
            }

            UIManager.Instance.craftedHollowItemScreen.SetActive(true);

            //AnimationManager.instance.AnimateCraftedHollowcreen(objectData.objectname, objectData.spritePath);
            AnimationManager.instance.AnimateCraftedHollowcreen(objectData.objectname, objectData.spriteIndex);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });

            return;
        }


        if (canCraft)
        {
            //if (!isBought)
            //{
                foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
                {
                    Debug.Log(CMN.mat.ToString());
                    Debug.Log(CMN.amount);

                    PlayerManager.Instance.DecreaseNumOfMats(CMN);
                }
            //}

            PlayerManager.Instance.ownedHollowObjects.Add(objectData);

            HollowCraftAndOwnedManager.Instance.RefreshHollowObjects();
            HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

            if (UIManager.Instance.ringersHutDisplay.gameObject.activeInHierarchy) /////// Find a way to do this better
            {
                HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);
            }

            UIManager.Instance.craftedHollowItemScreen.SetActive(true);

            //AnimationManager.instance.AnimateCraftedHollowcreen(objectData.objectname, objectData.spritePath);
            AnimationManager.instance.AnimateCraftedHollowcreen(objectData.objectname, objectData.spriteIndex);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }
        else
        {
            if (!UIManager.Instance.buyHollowItemDisplay.gameObject.activeInHierarchy) /////// Find a way to do this better
            {
                HollowCraftAndOwnedManager.Instance.currentlyToCraftNoramlMehtod = this;

                bool canBuy = false;

                if (PlayerManager.Instance.rubyCount >= rubiesNeededToBuyHollow)
                {
                    canBuy = true;
                }

                UIManager.Instance.DisplayHollowScreenRubyCostText(rubiesNeededToBuyHollow, canBuy);
                UIManager.Instance.DisplayBuyHollowItemNeeded(craftingMatsToRubiesHollow);
                UIManager.Instance.DisplayBuyHollowScreen();
            }

        }

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
            }
        }

        //SortMaster.Instance.RefreshAllScreens();

        //PlayerManager.Instance.SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();

    }

}
