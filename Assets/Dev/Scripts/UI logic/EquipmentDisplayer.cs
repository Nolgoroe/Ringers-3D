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
    public Button forgeButton;

    public TMP_Text itemName;
    public TMP_Text usageCount;
    //public RawImage powerUp;
    public RawImage itemImage;

    public Transform ingrediantContentParent;
    public GameObject ingrediantDisplayerPrefab;

    public List<string> materialCountPairs;

    public EquipmentData data;

    public List<CraftingMatsNeeded> craftingMatsForEquipment;

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

            CMD.SetImageAndMaterialCount(MaterialsAndForgeManager.Instance.materialSpriteByName[parsed_enum], matAndCount[1]);

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

    public void ForgeItem() ///// Here because the forge button and resources data are local
    {
        foreach (CraftingMatsNeeded CMN in craftingMatsForEquipment)
        {
            Debug.Log(CMN.mat.ToString());
            Debug.Log(CMN.amount);

            PlayerManager.Instance.DecreaseNumOfMats(CMN);
        }

        MaterialsAndForgeManager.Instance.RefreshMaterialBag();
        MaterialsAndForgeManager.Instance.RefreshForge();

        //PlayerManager.Instance.wardrobeEquipment.Add(data);
        WardrobeManager.Instance.SpawnWardrobeEquipment(data);

        WardrobeManager.Instance.EquipMe(data);
    }

    public void CheckIfCanForgeEquipment(List<CraftingMatsNeeded> CMN)
    {
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
            forgeButton.gameObject.SetActive(true);
        }
        else
        {
            forgeButton.gameObject.SetActive(false);
        }
    }
}
