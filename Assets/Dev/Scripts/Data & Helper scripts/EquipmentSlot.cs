using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    public EquipmentData equipmentInSlot;

    //public slotType typeOfSlot;

    public bool isFull;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Unequip");
        WardrobeManager.Instance.UnEquipMe(equipmentInSlot);

        equipmentInSlot = null;
    }
}
