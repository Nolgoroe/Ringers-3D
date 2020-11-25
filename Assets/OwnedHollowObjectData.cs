using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OwnedHollowObjectData : MonoBehaviour
{
    public HollowCraftObjectData objectData;

    public List<ObjectHollowType> requiredHollowType;

    public void PlaceFurniture(GameObject ToPlace)
    {
        if (HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow)
        {
            OwnedHollowObjectData OHOD = HollowCraftAndOwnedManager.Instance.hollowTypeToGameobject[HollowCraftAndOwnedManager.Instance.hollowTypeToFill];
            OwnedHollowObjectData OHODToPlace = ToPlace.GetComponent<OwnedHollowObjectData>();

            if (OHODToPlace.requiredHollowType.Contains(OHOD.requiredHollowType[0]))
            {
                if (OHOD.gameObject.GetComponent<HollowZone>().isEmpty)
                {
                    OHOD.gameObject.GetComponent<HollowZone>().isEmpty = false;
                    OHOD.objectData = objectData;
                    OHOD.GetComponent<RawImage>().texture = Resources.Load(OHODToPlace.objectData.spritePath) as Texture2D;
                    OHOD.transform.GetChild(0).gameObject.SetActive(true);

                    HollowCraftAndOwnedManager.Instance.objectsInOwned.Remove(this);
                    PlayerManager.Instance.ownedHollowObjects.Remove(objectData);

                    HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

                    SortMaster.Instance.FilterHollowOwnedScreenByEnum(HollowCraftAndOwnedManager.Instance.hollowTypeToFill);
                }
            }
        }
        else
        {
            foreach (HollowZone HZ in HollowCraftAndOwnedManager.Instance.hollowZones)
            {
                OwnedHollowObjectData OHOD = HZ.gameObject.GetComponent<OwnedHollowObjectData>();
                OwnedHollowObjectData OHODToPlace = ToPlace.GetComponent<OwnedHollowObjectData>();

                if (OHODToPlace.requiredHollowType.Contains(OHOD.requiredHollowType[0]))
                {
                    if (OHOD.gameObject.GetComponent<HollowZone>().isEmpty)
                    {
                        OHOD.gameObject.GetComponent<HollowZone>().isEmpty = false;
                        OHOD.objectData = objectData;
                        OHOD.GetComponent<RawImage>().texture = Resources.Load(OHODToPlace.objectData.spritePath) as Texture2D;
                        OHOD.transform.GetChild(0).gameObject.SetActive(true);

                        HollowCraftAndOwnedManager.Instance.objectsInOwned.Remove(this);
                        PlayerManager.Instance.ownedHollowObjects.Remove(objectData);

                        HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

                        SortMaster.Instance.FilterHollowOwnedScreenByEnum(HollowCraftAndOwnedManager.Instance.hollowTypeToFill);

                        return;
                    }
                }
            }
        }
    }

}
