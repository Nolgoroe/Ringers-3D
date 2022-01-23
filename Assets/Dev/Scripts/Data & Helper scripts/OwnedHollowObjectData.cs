using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OwnedHollowObjectData : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public HollowCraftObjectData objectData;

    public List<ObjectHollowType> requiredHollowType;

    private RectTransform rect;

    public Transform originalParent;

    public Vector2 pos;

    private void Awake()
    {
        rect = transform.GetComponent<RectTransform>();
        originalParent = transform.parent;
    }

    private void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CursorController.OverUI = true;
        pos = rect.anchoredPosition;

        Debug.Log("Is this detecting the pointer down? " + objectData.objectname);

        //transform.SetParent(originalParent.parent);

    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        CursorController.OverUI = false;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Hollow Zone"))
            {
                HollowZoneSlot HZS = hit.transform.GetComponent<HollowZoneSlot>();

                foreach (ObjectHollowType OHT in requiredHollowType)
                {
                    if (HZS.acceptedHollowItemTypes.Contains(OHT) || HZS.acceptedHollowItemTypes.Contains(ObjectHollowType.All))
                    {
                        HZS.PlaceHollowOnject(objectData.indexInHollow, objectData.hollowItemEnum);
                        PlayerManager.Instance.ownedHollowObjects.Remove(objectData);

                        HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

                        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.HollowManager, SystemsToSave.Player });
                        break;
                    }
                }
            }
        }

        rect.anchoredPosition = pos;
        //transform.SetParent(originalParent);
    }










    public void PlaceFurniture(GameObject ToPlace)
    {
        //if (HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow)
        //{
        //    OwnedHollowObjectData OHOD = HollowCraftAndOwnedManager.Instance.hollowTypeToGameobject[HollowCraftAndOwnedManager.Instance.hollowTypeToFill];
        //    OwnedHollowObjectData OHODToPlace = ToPlace.GetComponent<OwnedHollowObjectData>();

        //    if (OHODToPlace.requiredHollowType.Contains(OHOD.requiredHollowType[0]))
        //    {
        //        if (OHOD.gameObject.GetComponent<HollowZone>().isEmpty)
        //        {
        //            OHOD.gameObject.GetComponent<HollowZone>().isEmpty = false;
        //            OHOD.objectData = objectData;
        //            OHOD.GetComponent<RawImage>().texture = Resources.Load(OHODToPlace.objectData.spritePath) as Texture2D;
        //            OHOD.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
        //            OHOD.transform.GetChild(0).gameObject.SetActive(true);

        //            HollowCraftAndOwnedManager.Instance.objectsInOwned.Remove(this);
        //            PlayerManager.Instance.ownedHollowObjects.Remove(objectData);

        //            HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

        //            SortMaster.Instance.FilterHollowOwnedScreenByEnum(HollowCraftAndOwnedManager.Instance.hollowTypeToFill);
        //        }
        //    }
        //}
        //else
        //{
        //    foreach (HollowZone HZ in HollowCraftAndOwnedManager.Instance.hollowZones)
        //    {
        //        OwnedHollowObjectData OHOD = HZ.gameObject.GetComponent<OwnedHollowObjectData>();
        //        OwnedHollowObjectData OHODToPlace = ToPlace.GetComponent<OwnedHollowObjectData>();

        //        if (OHODToPlace.requiredHollowType.Contains(OHOD.requiredHollowType[0]))
        //        {
        //            if (OHOD.gameObject.GetComponent<HollowZone>().isEmpty)
        //            {
        //                OHOD.gameObject.GetComponent<HollowZone>().isEmpty = false;
        //                OHOD.objectData = objectData;
        //                OHOD.GetComponent<RawImage>().texture = Resources.Load(OHODToPlace.objectData.spritePath) as Texture2D;
        //                OHOD.transform.GetChild(0).gameObject.SetActive(true);

        //                HollowCraftAndOwnedManager.Instance.objectsInOwned.Remove(this);
        //                PlayerManager.Instance.ownedHollowObjects.Remove(objectData);

        //                HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

        //                SortMaster.Instance.FilterHollowOwnedScreenByEnum(HollowCraftAndOwnedManager.Instance.hollowTypeToFill);

        //                return;
        //            }
        //        }
        //    }
        //}
    }

}
