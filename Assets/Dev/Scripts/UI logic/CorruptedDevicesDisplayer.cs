using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Linq;



public class CorruptedDevicesDisplayer : MonoBehaviour
{
    public Button craftButton;

    //public TMP_Text itemName;
    //public TMP_Text usageCount;
    //public RawImage powerUp;
    public RawImage itemImage;

    public Transform ingrediantContentParent;
    public GameObject ingrediantDisplayerPrefab;

    public List<string> materialCountPairs;

    public CorruptedDevicesData data;

    public List<CraftingMatsNeeded> craftingMatsForEquipment;

    private void Start()
    {
        craftButton.onClick.AddListener(() => SoundManager.Instance.PlaySound(Sounds.ButtonPressUI));
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

            //CMD.SetImageAndMaterialCount(MaterialsAndForgeManager.Instance.materialSpriteByName[parsed_enum], matAndCount[1]);
            CMD.SetImageAndMaterialCount(LootManager.Instance.allMaterialSprites[(int)parsed_enum], parsed_enum, matAndCount[1]);

            SetDataMatsNeeded(nameOfMat, Convert.ToInt16(matAndCount[1]), CMD, parsed_enum);

            //CMD.SetImageAndMaterialCount("Crafting Mat Icons/" + matAndCount[0], matAndCount[1]);
            //CMD.SetImageAndMaterialCount(MaterialsAndForgeManager.Instance.materialSpriteByName[parsed_enum], matAndCount[1]);

            //SetDataMatsNeeded(matAndCount[0], Convert.ToInt16(matAndCount[1]), CMD);
        }


        CheckIfCanForgeEquipment(craftingMatsForEquipment);
    }

    public void SetDataMatsNeeded(string nameOfMat, int amountOfMat, CraftingMatDisplayer CMD, CraftingMats parsed_enum)
    {
        CraftingMatsNeeded CMN = new CraftingMatsNeeded();

        CMN.mat = parsed_enum;
        CMN.amount = amountOfMat;

        craftingMatsForEquipment.Add(CMN);

        CMD.CheckIfHasEnough(parsed_enum, amountOfMat);
    }

    public void ForgeCorruptedDevice() ///// Here because the forge button and resources data are local
    {
        foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
        {
            Debug.Log(CMN.mat.ToString());
            Debug.Log(CMN.amount);

            PlayerManager.Instance.DecreaseNumOfMats(CMN);
        }

        //SortMaster.Instance.RefreshAllForgeScreens();
        //SortMaster.Instance.RefreshAllScreens();

        //PlayerManager.Instance.wardrobeEquipment.Add(data);
        //WardrobeManager.Instance.SpawnWardrobeEquipment(data);

        PlayerManager.Instance.ownedCorruptDevices.Add(data);

        //PlayerManager.Instance.SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();
    }

    public void CheckIfCanForgeEquipment(List<CraftingMatsNeeded> CMN)
    {
        bool canCraft = true;

        foreach (CraftingMatsNeeded CM in CMN)
        {
            if (PlayerManager.Instance.CheckIfHasMaterialts(CM, out int neededAmount) > 0)
            {
                canCraft = false;
                break;
            }
        }

        if (canCraft)
        {
            craftButton.interactable = true;
            //forgeButton.gameObject.SetActive(true);
        }
        else
        {
            craftButton.interactable = false;

            //forgeButton.gameObject.SetActive(false);
        }
    }
}
