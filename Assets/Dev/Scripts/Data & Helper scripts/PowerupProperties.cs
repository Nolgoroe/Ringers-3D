using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PowerupProperties : MonoBehaviour, IPointerClickHandler
{
    public Sprite icon;
    public string powerupText;
    public PowerUp powerupType;
    public int numOfUses;
    ///public TMP_Text nameOfPowerup;
    public PieceColor transformColor;
    public PieceSymbol transformSymbol;
    public EquipmentData connectedEquipment;
    public TMP_Text numOfUsesText;

    //public bool isSelected = false;
    public bool canBeSelected = false;

    public UnityEvent interactEvent;

    public void SetProperties(PowerUp type)
    {
        string path = GameManager.Instance.powerupManager.spriteByType[type];

        icon = Resources.Load<Sprite>(path);
        powerupText = GameManager.Instance.powerupManager.nameTextByType[type];
        powerupType = type;

        GetComponent<SpriteRenderer>().sprite = icon;


        //if(type == PowerUp.FourColorTransform)
        //{
        //    //nameOfPowerup.text = powerupText + "\n" + connectedEquipment.specificColor;
        //}
        //else if(type == PowerUp.FourShapeTransform)
        //{
        //    //nameOfPowerup.text = powerupText + "\n" + connectedEquipment.specificSymbol;
        //}
        //else
        //{
        //    //nameOfPowerup.text = powerupText;
        //}
    }

    public void FindNumOfUsesTextObject()
    {
        //TMP_Text text = transform.parent.GetComponentInChildren<TMP_Text>();
        //numOfUsesText = text;

        getChildrenHelpData GCHD = transform.parent.GetComponent<getChildrenHelpData>();
        numOfUsesText = GCHD.referenceNumUsesText.GetComponentInChildren<TMP_Text>();

        numOfUsesText.transform.parent.SetAsLastSibling();
        numOfUsesText.transform.parent.gameObject.SetActive(true);
    }

    public void UpdateNumOfUsesText()
    {
        numOfUsesText.text = numOfUses.ToString();

        if(numOfUses == 0)
        {
            numOfUsesText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBeSelected)
        {
            interactEvent.Invoke();
        }
        Debug.Log("Shooting event powerup");
    }
}
