using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FilledItemAndZoneIndex
{
    public HollowItems hollowItem;
    public int zoneIndex;
    public int indexInZone;
}

public class HollowManagerSaveData : MonoBehaviour
{
    public static HollowManagerSaveData Instance;


    public List<FilledItemAndZoneIndex> filledHollowItemsToIndex;

    private void Awake()
    {
        Instance = this;
    }


    public void Init()
    {
        Instance = this;

        HollowCraftAndOwnedManager.Instance.DisplayLoadedHollowItems();
    }
}
