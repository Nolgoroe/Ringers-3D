using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
public class CraftingMatDisplayer : MonoBehaviour
{
    public RawImage materialImage;
    //public Image textBG;
    public TMP_Text materialCount;


    public void SetImageAndMaterialCount(string iconPath, string count)
    {
        materialImage.texture = Resources.Load(iconPath) as Texture2D;
        materialCount.text = count;
    }

    public void CheckIfHasEnough(CraftingMats matEnum, int amountRequired)
    {
        CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == matEnum).Single();

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
