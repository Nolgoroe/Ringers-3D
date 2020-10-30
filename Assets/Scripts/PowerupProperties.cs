using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupProperties : MonoBehaviour
{
    public Texture2D icon;
    public string powerupText;
    public PowerUp powerupType;
    public int numOfUses;
    public TMP_Text nameOfPowerup;
    public PieceColor transformColor;
    public PieceSymbol transformSymbol;
    public void SetProperties(PowerUp type)
    {
        transformColor = PieceColor.None;
        transformSymbol = PieceSymbol.None;
        switch (type)
        {
            //case PowerUp.Joker:
            //    break;
            //case PowerUp.Switch:
            //    break;
            //case PowerUp.PieceBomb:
            //    break;
            //case PowerUp.SliceBomb:
            //    break;
            //case PowerUp.ExtraDeal:
            //    break;
            case PowerUp.FourColorTransform:
                ////READ FROM ITEM
                break;
            case PowerUp.FourShapeTransform:
                ////READ FROM ITEM
                break;
            //case PowerUp.None:
            //    break;
            default:
                break;
        }
        string path = GameManager.Instance.powerupManager.spriteByType[type];

        icon = Resources.Load(path) as Texture2D;
        powerupText = GameManager.Instance.powerupManager.nameTextByType[type];
        powerupType = type;
        numOfUses = 2;

        GetComponent<RawImage>().texture = icon;
        nameOfPowerup.text = powerupText;
    }
}
