using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class CraftingMatDisplayer : MonoBehaviour, IPointerClickHandler
{
    public Image materialImage;
    public TMP_Text materialCount;

    public GameObject parentObject;

    public CraftingMats craftingMatEnum;

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

        craftingMatEnum = matEnum;
    }

    public void CheckIfHasEnough(CraftingMats matEnum, int amountRequired)
    {
        if(matEnum == CraftingMats.DewDrops)
        {
            int hasAmount = PlayerManager.Instance.collectedDewDrops;

            if (hasAmount >= amountRequired)
            {
                //materialCount.color = UIManager.Instance.gameTextColor;

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
                //materialCount.color = UIManager.Instance.gameTextColor;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pointsOfInterestSaveData.instance.inventoryPointOfInterest.Contains(craftingMatEnum))
        {
            GetComponent<PointOfInterest>().HideInterestPointImage();

            pointsOfInterestSaveData.instance.inventoryPointOfInterest.Remove(craftingMatEnum);

            if (pointsOfInterestSaveData.instance.inventoryPointOfInterest.Count <= 0)
            {
                InterestPointsManager.instance.TurnOffPointsOfInterestDisplay(TypesPointOfInterest.inventory);
            }

            InterestPointsManager.instance.PointsOfInterestInventorySort();
        }
    }

    public void CheckShowPointInterestList(CraftingMats matEnum)
    {
        if (pointsOfInterestSaveData.instance.inventoryPointOfInterest.Contains(matEnum))
        {
            GetComponent<PointOfInterest>().ShowInterestPointImage();
        }
    }
}
