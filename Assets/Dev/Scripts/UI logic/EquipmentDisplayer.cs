using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Linq;



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

    private void Awake()
    {
        BDL = GetComponentInParent<BreweryDisplayLogic>();
    }
    private void Start()
    {
        craftButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));


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

    public void ForgeItem() ///// Here because the forge button and resources data are local
    {
        foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
        {
            Debug.Log(CMN.mat.ToString());
            Debug.Log(CMN.amount);

            PlayerManager.Instance.DecreaseNumOfMats(CMN);
        }

        SortMaster.Instance.ClearAllForgeScreens();
        //SortMaster.Instance.RefreshAllScreens();

        //PlayerManager.Instance.wardrobeEquipment.Add(data);
        //WardrobeManager.Instance.SpawnWardrobeEquipment(data);

        EquipmentData newData = new EquipmentData(data.name, data.power, data.specificSymbol, data.specificColor, data.numOfUses, data.scopeOfUses,
                                                  data.timeForCooldown, data.nextTimeAvailable, data.Description, data.isTutorialPower, data.mats, data.spritePath);

        PlayerManager.Instance.EquipMe(newData);
    }

    public void CheckIfCanForgeEquipment(List<CraftingMatsNeeded> CMN)
    {
        if (!BDL)
        {
            BDL = GetComponentInParent<BreweryDisplayLogic>();
        }

        bool canCraft = true;

        foreach (CraftingMatsNeeded CM in CMN)
        {
            if (!PlayerManager.Instance.CheckIfHasMaterialts(CM))
            {
                canCraft = false;
                break;
            }
        }

        if (canCraft)
        {
            BDL.brewButton.interactable = true;
            BDL.brewButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Brew";
            //craftButton.interactable = true;
            //forgeButton.gameObject.SetActive(true);
        }
        else
        {
            BDL.brewButton.interactable = false;
            BDL.brewButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "No mats!";
            //craftButton.interactable = false;

            //forgeButton.gameObject.SetActive(false);
        }
    }
}
