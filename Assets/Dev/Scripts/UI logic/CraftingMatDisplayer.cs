﻿using System.Collections;
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
    }

    public void CheckIfHasEnough(CraftingMats matEnum, int amountRequired)
    {
        if(matEnum == CraftingMats.DewDrops)
        {
            int hasAmount = PlayerManager.Instance.collectedDewDrops;
        }
        else
        {
            CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == matEnum).Single();
        }

        //CMN.indexMatInPlayerInventory = i; //// Nice trick - ask Alon what he thinks

        //if (CME.amount >= amountRequired)
        //{
        //    textBG.color = Color.white;
        //}
        //else
        //{
        //    textBG.color = Color.red;
        //}

        //textBG.color = new Color(textBG.color.r, textBG.color.g, textBG.color.b,1);
    }
}
