using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SortMaster : MonoBehaviour
{
    public void ForgeFilter(TMP_Dropdown filter) 
    {
        if((slotType)filter.value == slotType.All)
        {
            foreach (EquipmentDisplayer ED in MaterialsAndForgeManager.Instance.equipmentInForge)
            {
                ED.gameObject.SetActive(true);
            }
            return;
        }

        foreach (EquipmentDisplayer ED in MaterialsAndForgeManager.Instance.equipmentInForge)
        {
            if((int)ED.data.slot != filter.value)
            {
                ED.gameObject.SetActive(false);
            }
            else
            {
                ED.gameObject.SetActive(true);
            }
        }
    } 
}
