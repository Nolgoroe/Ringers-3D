using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftingMatDisplayer : MonoBehaviour
{
    public RawImage materialImage;
    public TMP_Text materialCount;


    public void SetImageAndMaterialCount(string iconPath, string count)
    {
        materialImage.texture = Resources.Load(iconPath) as Texture2D;
        materialCount.text = count;
    }
}
