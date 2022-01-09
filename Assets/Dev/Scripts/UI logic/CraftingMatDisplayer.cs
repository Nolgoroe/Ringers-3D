using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
public class CraftingMatDisplayer : MonoBehaviour
{
    public Image materialImage;
    //public Image textBG;
    public TMP_Text materialCount;

    public GameObject parentObject;

    public void SetImageAndMaterialCount(Sprite icon, CraftingMats matEnum,string amountNeeded)
    {
        string amountHas = null;

        if (matEnum == CraftingMats.DewDrops)
        {
            amountHas = PlayerManager.Instance.collectedDewDrops.ToString();
        }
        else
        {
            CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == matEnum).Single();
            amountHas = CME.amount.ToString();
        }

        materialImage.sprite = icon;
        materialCount.text = amountHas + "/" + amountNeeded;

        parentObject = transform.parent.gameObject;
    }

    public void CheckIfHasEnough(CraftingMats matEnum, int amountRequired)
    {
        if(matEnum == CraftingMats.DewDrops)
        {
            int hasAmount = PlayerManager.Instance.collectedDewDrops;

            if (hasAmount >= amountRequired)
            {
                materialCount.color = UIManager.Instance.gameTextColor;

                if (parentObject)
                {
                    if (parentObject.GetComponent<SpecificBrewMatZonePrompts>())
                    {
                        SpecificBrewMatZonePrompts prompts = parentObject.GetComponent<SpecificBrewMatZonePrompts>();
                        prompts.exclimationMark.SetActive(false);
                    }
                }
            }
            else
            {
                materialCount.color = Color.red;

                if (parentObject)
                {
                    if (parentObject.GetComponent<SpecificBrewMatZonePrompts>())
                    {
                        SpecificBrewMatZonePrompts prompts = parentObject.GetComponent<SpecificBrewMatZonePrompts>();
                        prompts.exclimationMark.SetActive(true);
                    }
                }
            }
        }
        else
        {
            CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == matEnum).Single();

            if (CME.amount >= amountRequired)
            {
                materialCount.color = UIManager.Instance.gameTextColor;

                if (parentObject)
                {
                    if (parentObject.GetComponent<SpecificBrewMatZonePrompts>())
                    {
                        SpecificBrewMatZonePrompts prompts = parentObject.GetComponent<SpecificBrewMatZonePrompts>();
                        prompts.exclimationMark.SetActive(false);
                    }
                }
            }
            else
            {
                materialCount.color = Color.red;

                if (parentObject)
                {
                    if (parentObject.GetComponent<SpecificBrewMatZonePrompts>())
                    {
                        SpecificBrewMatZonePrompts prompts = parentObject.GetComponent<SpecificBrewMatZonePrompts>();
                        prompts.exclimationMark.SetActive(true);
                    }
                }
            }
        }

        //CMN.indexMatInPlayerInventory = i; //// Nice trick - ask Alon what he thinks


        //textBG.color = new Color(textBG.color.r, textBG.color.g, textBG.color.b,1);
    }
}
