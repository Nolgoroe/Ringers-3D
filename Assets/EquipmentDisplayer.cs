using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class EquipmentDisplayer : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text usageCount;
    public RawImage powerUp;
    public RawImage itemImage;

    public Transform ingrediantContentParent;
    public GameObject ingrediantDisplayerPrefab;

    private List<string> materialCountPairs;

    public EquipmentData data;

    public void SpawnMaterialsNeeded(string matList)
    {
        materialCountPairs = new List<string>();

        string[] temp = matList.Split('|');

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = temp[i].Trim();  //removed the blank spaces
            materialCountPairs.Add(temp[i]);
        }

        foreach (string item in materialCountPairs)
        {
            GameObject go = Instantiate(ingrediantDisplayerPrefab, ingrediantContentParent);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            string[] matAndCount = item.Split('-');

            for (int i = 0; i < matAndCount.Length; i++)
            {
                matAndCount[i] = matAndCount[i].Trim();  //removed the blank spaces
            }

            CMD.SetImageAndMaterialCount("Assets/Resources/Crafting Mat Icons/" + matAndCount[0], matAndCount[1]);
        }
    }
}
