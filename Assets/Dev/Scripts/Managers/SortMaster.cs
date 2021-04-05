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
    public TMP_Dropdown hollowObjectsFilter;
    public TMP_Dropdown ownedHollowObjectsFilter;
    public static SortMaster Instance;

    private void Start()
    {
        Instance = this;
    }
    public void FilterForge() 
    {
        //if((slotType)forgeFilter.value == slotType.All)
        //{
        //    foreach (EquipmentDisplayer ED in MaterialsAndForgeManager.Instance.equipmentInForge)
        //    {
        //        ED.gameObject.SetActive(true);
        //    }
        //    return;
        //}

        //foreach (EquipmentDisplayer ED in MaterialsAndForgeManager.Instance.equipmentInForge)
        //{
        //    if((int)ED.data.slot != forgeFilter.value)
        //    {
        //        ED.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        ED.gameObject.SetActive(true);
        //    }
        //}
    } 
    public void FilterHollowCraftScreen() 
    {
        if((ObjectHollowType)hollowObjectsFilter.value == ObjectHollowType.All)
        {
            foreach (HollowObjectDisplayer HOD in HollowCraftAndOwnedManager.Instance.objectInHollow)
            {
                HOD.gameObject.SetActive(true);
            }
            return;
        }

        foreach (HollowObjectDisplayer HOD in HollowCraftAndOwnedManager.Instance.objectInHollow)
        {
            if (HOD.objectData.objectHollowType.Contains((ObjectHollowType)hollowObjectsFilter.value))
            {
                HOD.gameObject.SetActive(true);
            }
            else
            {
                HOD.gameObject.SetActive(false);
            }
        }
    } 
    public void FilterHollowOwnedScreen() 
    {
        if((ObjectHollowType)ownedHollowObjectsFilter.value == ObjectHollowType.All)
        {
            foreach (OwnedHollowObjectData OHOD in HollowCraftAndOwnedManager.Instance.objectsInOwned)
            {
                OHOD.gameObject.SetActive(true);
            }
            return;
        }

        foreach (OwnedHollowObjectData OHOD in HollowCraftAndOwnedManager.Instance.objectsInOwned)
        {
            if (OHOD.objectData.objectHollowType.Contains((ObjectHollowType)ownedHollowObjectsFilter.value))
            {
                OHOD.gameObject.SetActive(true);
            }
            else
            {
                OHOD.gameObject.SetActive(false);
            }
        }
    } 
    public void FilterHollowOwnedScreenByEnum(ObjectHollowType typeOfHollow) 
    {
        if(typeOfHollow == ObjectHollowType.All)
        {
            foreach (OwnedHollowObjectData OHOD in HollowCraftAndOwnedManager.Instance.objectsInOwned)
            {
                OHOD.gameObject.SetActive(true);
            }
            return;
        }

        foreach (OwnedHollowObjectData OHOD in HollowCraftAndOwnedManager.Instance.objectsInOwned)
        {
            if (OHOD.objectData.objectHollowType.Contains(typeOfHollow))
            {
                OHOD.gameObject.SetActive(true);
            }
            else
            {
                OHOD.gameObject.SetActive(false);
            }
        }
    } 
    public void FilterWardrobe() 
    {
        //if((slotType)wardrobeFilter.value == slotType.All)
        //{
        //    foreach (WardrobeEquipmentDisplayer WED in WardrobeManager.Instance.equipmentInWardrobe)
        //    {
        //        WED.gameObject.SetActive(true);
        //    }
        //    return;
        //}

        //foreach (WardrobeEquipmentDisplayer WED in WardrobeManager.Instance.equipmentInWardrobe)
        //{
        //    if((int)WED.theEquipmentData.slot != wardrobeFilter.value)
        //    {
        //        WED.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        WED.gameObject.SetActive(true);
        //    }
        //}
    } 
}
