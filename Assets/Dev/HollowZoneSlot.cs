using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HollowZoneSlot : MonoBehaviour
{
    public int zoneIndex;

    public List<ObjectHollowType> acceptedHollowItemTypes;

    public GameObject[] objectsInZone;

    public void CheckWork(ObjectHollowType typeWorked)
    {
        Debug.Log("Is this detectiong the hollow zone? " + typeWorked);
    }

    public void PlaceHollowOnject(int indexInZone, HollowItems itemEnum)
    {
        objectsInZone[indexInZone].SetActive(true);
        FilledItemAndZoneIndex FITZI = new FilledItemAndZoneIndex();
        FITZI.hollowItem = itemEnum;
        FITZI.zoneIndex = zoneIndex;
        FITZI.indexInZone = indexInZone;

        HollowManagerSaveData.Instance.filledHollowItemsToIndex.Add(FITZI);
    }

}
