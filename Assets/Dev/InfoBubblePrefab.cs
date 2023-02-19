using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoBubblePrefab : MonoBehaviour
{
    public TMP_Text descriptionText;
    public Image potionImage;
    
    public void SetDescriptionText(Slice slice)
    {
        string text = "";

        switch (slice.sliceCatagory)
        {
            case SliceCatagory.Shape:
                text = "Match between any two shapes";
                break;
            case SliceCatagory.Color:
                text = "Match between any two colors";
                break;
            case SliceCatagory.SpecificShape:
                text = $"Match {slice.sliceSymbol} symbol only";
                break;
            case SliceCatagory.SpecificColor:
                text = $"Match {slice.sliceColor} color only";
                break;
            case SliceCatagory.None:
                break;
            default:
                break;
        }

        descriptionText.text = text;
    }

    public void SetDescriptionText(PowerupProperties powerProperties)
    {
        typeOfPotions potionType = typeOfPotions.None;

        switch (powerProperties.powerupType)
        {
            case PowerUp.Switch:
                potionType = typeOfPotions.AllPiecePotion;
                break;
            case PowerUp.Joker:
                potionType = typeOfPotions.AllPiecePotion;
                break;
            case PowerUp.PieceBomb:
                potionType = typeOfPotions.BoardPiecePotion;
                break;
            case PowerUp.SliceBomb:
                potionType = typeOfPotions.LimiterPotion;
                break;
            case PowerUp.None:
                break;
            default:
                break;
        }
        potionImage.sprite = powerProperties.icon;

        descriptionText.text = powerProperties.powerupText;

        StartCoroutine(TutorialSequence.Instacne.SelectRelaventHighlightsBubbleInfo(potionType));

        TutorialSequence.Instacne.maskImage.gameObject.SetActive(true);
    }
}
