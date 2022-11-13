using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Linq;
using GameAnalyticsSDK;


[Serializable]
public class CraftingMatsNeeded
{
    public CraftingMats mat;
    public int amount;
}
public class EquipmentDisplayer : MonoBehaviour
{
    public Button selectPotionButton;
    public Button craftButton;

    public TMP_Text itemName;
    public TMP_Text usageCount;
    //public RawImage powerUp;
    public RawImage itemImage;

    public Transform ingrediantContentParent;
    public GameObject ingrediantDisplayerPrefab;

    public List<string> materialCountPairs;

    public EquipmentData data;

    public List<CraftingMatsNeeded> craftingMatsForEquipment;

    public BreweryDisplayLogic BDL;

    TMP_Text buttonText;

    public Vector2 originalPotionPosForSelection;

    private void Awake()
    {
        BDL = GetComponentInParent<BreweryDisplayLogic>();

        buttonText = BDL.brewButton.transform.GetChild(0).GetComponent<TMP_Text>();

    }
    private void Start()
    {

        //craftButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));


        selectPotionButton.onClick.AddListener(() => BDL.SetSelectedPotion(this));
        selectPotionButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));
    }

    public void SpawnMaterialsNeeded(string matList, Transform[] matPositions)
    {
        foreach (Transform matPos in matPositions)
        {
            for (int i = 0; i < matPos.childCount; i++)
            {
                Destroy(matPos.GetChild(i).gameObject);
            }

        }

        craftingMatsForEquipment.Clear();

        materialCountPairs = new List<string>();

        string[] temp = matList.Split('|'); /// Split by the | Char

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = temp[i].Trim();  //remove the blank spaces
            materialCountPairs.Add(temp[i]);
        }

        for (int i = 0; i < materialCountPairs.Count; i++)
        {
            GameObject go = Instantiate(ingrediantDisplayerPrefab, matPositions[i]);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            string[] matAndCount = materialCountPairs[i].Split('-'); /// Split by the - Char

            for (int j = 0; j < matAndCount.Length; j++)
            {
                matAndCount[j] = matAndCount[j].Trim();  //remove the blank spaces
            }

            string nameOfMat = matAndCount[0].Replace(" ", string.Empty);

            if (nameOfMat.Contains("Drops"))
            {
                CraftingMats parsed_enum = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), nameOfMat);

                CMD.SetImageAndMaterialCount(LootManager.Instance.dewDropsSprite, parsed_enum, matAndCount[1]);

                SetDataMatsNeeded(nameOfMat, Convert.ToInt16(matAndCount[1]), CMD, parsed_enum);

            }
            else
            {
                CraftingMats parsed_enum = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), nameOfMat);

                //CMD.SetImageAndMaterialCount(MaterialsAndForgeManager.Instance.materialSpriteByName[parsed_enum], matAndCount[1]);
                CMD.SetImageAndMaterialCount(LootManager.Instance.allMaterialSprites[(int)parsed_enum], parsed_enum, matAndCount[1]);

                SetDataMatsNeeded(nameOfMat, Convert.ToInt16(matAndCount[1]), CMD, parsed_enum);

                //CMD.SetImageAndMaterialCount("Crafting Mat Icons/" + matAndCount[0], matAndCount[1]);
                //CMD.SetImageAndMaterialCount(MaterialsAndForgeManager.Instance.materialSpriteByName[parsed_enum], matAndCount[1]);

                //SetDataMatsNeeded(matAndCount[0], Convert.ToInt16(matAndCount[1]), CMD);
            }
        }
    }

    public void SetDataMatsNeeded(string nameOfMat, int amountOfMat, CraftingMatDisplayer CMD, CraftingMats parsed_enum)
    {
        CraftingMatsNeeded CMN = new CraftingMatsNeeded();

        CMN.mat = parsed_enum;
        CMN.amount = amountOfMat;

        craftingMatsForEquipment.Add(CMN);

        CMD.CheckIfHasEnough(parsed_enum, amountOfMat);
    }

    public void ForgeItem(bool canForgePotion, bool isBought) ///// Here because the forge button and resources data are local
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (!TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isBrewPhase)
            {
                return;
            }
        }


        if (canForgePotion)
        {
            Debug.Log("Crafted potion!!");

            EquipmentData TempED = null;

            //if (!isBought)
            //{
                GameAnalytics.NewDesignEvent(PlayfabManager.instance.playerName + "Crafted Potion:" + data.power.ToString());

                foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
                {
                    Debug.Log(CMN.mat.ToString());
                    Debug.Log(CMN.amount);

                    PlayerManager.Instance.DecreaseNumOfMats(CMN);
                }

                //SortMaster.Instance.RefreshAllForgeScreens();
            //}

            //SortMaster.Instance.RefreshAllScreens();

            //PlayerManager.Instance.wardrobeEquipment.Add(data);
            //WardrobeManager.Instance.SpawnWardrobeEquipment(data);

            EquipmentData newData = new EquipmentData(data.name, data.power, data.specificSymbol, data.specificColor, data.numOfUses, data.scopeOfUses,
                                                      data.timeForCooldown, data.nextTimeAvailable, data.Description, data.isTutorialPower, data.mats, data.spritePath);

            if (PlayerManager.Instance.ownedPowerups.Count > 0)
            {
                TempED = PlayerManager.Instance.ownedPowerups.Where(equip => equip.power == newData.power).FirstOrDefault();
            }

            if(TempED != null)
            {
                TempED.numOfUses += newData.numOfUses;
            }
            else
            {
                PlayerManager.Instance.EquipMe(newData);
            }


            if (TutorialSequence.Instacne.duringSequence)
            {
                if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isBrewPhase)
                {
                    StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
                }
            }


            UIManager.Instance.brewedPotionScreen.SetActive(true);

            //UIManager.Instance.Brewery.GetComponent<BreweryDisplayLogic>().GetAllAnchorPositions();

            AnimationManager.instance.AnimateBrewScreen(BDL.potionName.text, data.spritePath);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }
        else
        {
            Debug.Log("CAN'T craft potion!!");

            bool canBuy = false;

            if (PlayerManager.Instance.rubyCount >= BDL.rubiesNeededToBuyPotion)
            {
                canBuy = true;
            }

            UIManager.Instance.DisplayBuyPotionScreenRubyCostText(BDL.rubiesNeededToBuyPotion, canBuy);
            UIManager.Instance.DisplayBuyPotionLootNeeded(BDL.materialsNeedToBuyPotion);
            UIManager.Instance.DisplayBuyPotionScreen();
        }

    }

    public bool CheckIfCanForgeEquipment(List<CraftingMatsNeeded> CMN)
    {
        if (!BDL)
        {
            BDL = GetComponentInParent<BreweryDisplayLogic>();
        }

        bool canCraft = true;

        foreach (CraftingMatsNeeded CM in CMN)
        {
            if (PlayerManager.Instance.CheckIfHasMaterialts(CM, out int neededAmount) > 0) /// checks if there are materials needed/missing for potion - if there are more than 0 materials needed, we are missing materials
            {
                canCraft = false;
                BDL.AddNeededCraftingMatsToRubiesItem(CM.mat, neededAmount);
                //break;
            }
        }


        BDL.CalculateNeededRubiesToBuyPotion();

        if (canCraft)
        {
            //BDL.brewButton.interactable = true;

            buttonText.text = "Brew";
            //buttonText.color = UIManager.Instance.gameTextColor;
            //craftButton.interactable = true;
            //forgeButton.gameObject.SetActive(true);
        }
        else
        {
            //BDL.brewButton.interactable = false;

            buttonText.text = "Brew"; /// might not need this line of code...
            //buttonText.color = UIManager.Instance.gameTextColor;
            //craftButton.interactable = false;

            //forgeButton.gameObject.SetActive(false);
        }

        return canCraft;
    }

    public IEnumerator GetAnchoredPosition()
    {
        yield return new WaitForEndOfFrame();
        originalPotionPosForSelection = transform.GetComponent<RectTransform>().anchoredPosition;
    }
}
