using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SortMaster : MonoBehaviour
{
    public TMP_Dropdown forgeFilter;
    public TMP_Dropdown wardrobeFilter;
    public static SortMaster Instance;

    private void Start()
    {
        Instance = this;
    }
    public void FilterForge() 
    {
        if((slotType)forgeFilter.value == slotType.All)
        {
            foreach (EquipmentDisplayer ED in MaterialsAndForgeManager.Instance.equipmentInForge)
            {
                ED.gameObject.SetActive(true);
            }
            return;
        }

        foreach (EquipmentDisplayer ED in MaterialsAndForgeManager.Instance.equipmentInForge)
        {
            if((int)ED.data.slot != forgeFilter.value)
            {
                ED.gameObject.SetActive(false);
            }
            else
            {
                ED.gameObject.SetActive(true);
            }
        }
    } 
    public void FilterWardrobe() 
    {
        if((slotType)wardrobeFilter.value == slotType.All)
        {
            foreach (WardrobeEquipmentDisplayer WED in WardrobeManager.Instance.equipmentInWardrobe)
            {
                WED.gameObject.SetActive(true);
            }
            return;
        }

        foreach (WardrobeEquipmentDisplayer WED in WardrobeManager.Instance.equipmentInWardrobe)
        {
            if((int)WED.theEquipmentData.slot != wardrobeFilter.value)
            {
                WED.gameObject.SetActive(false);
            }
            else
            {
                WED.gameObject.SetActive(true);
            }
        }
    } 
}
