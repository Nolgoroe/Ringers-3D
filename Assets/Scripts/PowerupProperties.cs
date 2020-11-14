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
    ///public TMP_Text nameOfPowerup;
    public PieceColor transformColor;
    public PieceSymbol transformSymbol;
    public EquipmentData connectedEquipment;

    public void SetProperties(PowerUp type)
    {
        string path = GameManager.Instance.powerupManager.spriteByType[type];

        icon = Resources.Load(path) as Texture2D;
        powerupText = GameManager.Instance.powerupManager.nameTextByType[type];
        powerupType = type;

        GetComponent<RawImage>().texture = icon;


        if(type == PowerUp.FourColorTransform)
        {
            //nameOfPowerup.text = powerupText + "\n" + connectedEquipment.specificColor;
        }
        else if(type == PowerUp.FourShapeTransform)
        {
            //nameOfPowerup.text = powerupText + "\n" + connectedEquipment.specificSymbol;
        }
        else
        {
            //nameOfPowerup.text = powerupText;
        }
    }
}
