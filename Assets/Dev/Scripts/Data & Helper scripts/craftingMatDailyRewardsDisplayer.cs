using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class craftingMatDailyRewardsDisplayer : MonoBehaviour
{
    public Image materialImage;
    public TMP_Text materialCount;

    public void SetImageAndMaterialCount(Sprite icon, CraftingMats matEnum, string amountNeeded)
    {
        materialImage.sprite = icon;
        materialCount.text = amountNeeded;
    }
}
