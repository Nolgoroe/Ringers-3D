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
        OwnedHollowObjectData OHOD = HollowCraftAndOwnedManager.Instance.hollowTypeToGameobject[HollowCraftAndOwnedManager.Instance.hollowTypeToFill];
        OwnedHollowObjectData OHODToPlace = ToPlace.GetComponent<OwnedHollowObjectData>();

        if (OHODToPlace.requiredHollowType.Contains(OHOD.requiredHollowType[0]))
        {
            if (OHOD.objectData.objectHollowType.Count == 0)
            {
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

}
