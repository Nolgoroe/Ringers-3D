using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WardrobeEquipmentDisplayer : MonoBehaviour, IPointerClickHandler
{
    public EquipmentData theEquipmentData;

    public RawImage equipmentImage;
    public RawImage powerupImage;

    public TMP_Text equipementName;
    public TMP_Text equipmentUsage;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Equip");
        //WardrobeManager.Instance.EquipMe(theEquipmentData, this);
    }
}
