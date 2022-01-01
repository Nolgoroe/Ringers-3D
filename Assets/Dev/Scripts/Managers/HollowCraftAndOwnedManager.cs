using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HollowCraftAndOwnedManager : MonoBehaviour
{
    public static HollowCraftAndOwnedManager Instance;

    public GameObject HollowObjectPrefab;
    public Transform HollowObjectContent; /// Parent

    public GameObject HollowObjectOwnedPrefab;
    public Transform HollowObjectOwnedContent; /// Parent

    public List<HollowObjectDisplayer> objectInHollow; /// Equipment that the player does not have / has not created yet
    public List<OwnedHollowObjectData> objectsInOwned; /// Equipment that the player does not have / has not created yet

    public ObjectHollowType hollowTypeToFill;

    public HollowZone[] hollowZones;
    public Dictionary<ObjectHollowType, OwnedHollowObjectData> hollowTypeToGameobject;

    public bool isPlaceThroughHollow; /// Either place through hollow or thorugh normal open bag

    private void Start()
    {
        Instance = this;

        hollowTypeToGameobject = new Dictionary<ObjectHollowType, OwnedHollowObjectData>();

        for (int i = 1; i < System.Enum.GetValues(typeof(ObjectHollowType)).Length; i++)
        {
            hollowTypeToGameobject.Add((ObjectHollowType)i, hollowZones[i - 1].gameObject.GetComponent<OwnedHollowObjectData>());
        }

    }

    public void FillCraftScreen(List<HollowCraftObjectData> HollowCraftObjects)
    {
        foreach (HollowCraftObjectData HCOD in HollowCraftObjects)
        {
            GameObject go = Instantiate(HollowObjectPrefab, HollowObjectContent);
            HollowObjectDisplayer HOD = go.GetComponent<HollowObjectDisplayer>();

            //HOD.itemName.text = HCOD.objectname;
            HOD.objectData = HCOD;

            HOD.itemImage.texture = Resources.Load(HCOD.spritePath) as Texture2D;

            HOD.name = HCOD.objectname;
            objectInHollow.Add(HOD);
            HOD.SpawnMaterialsNeeded(HCOD.mats);
        }
    }

    public void FillOwnedScreen()
    {
        foreach (HollowCraftObjectData HCOD in PlayerManager.Instance.ownedHollowObjects)
        {
            GameObject go = Instantiate(HollowObjectOwnedPrefab, HollowObjectOwnedContent);
            OwnedHollowObjectData OHOD = go.GetComponent<OwnedHollowObjectData>();

            OHOD.objectData = HCOD;
            OHOD.name = HCOD.objectname;
            OHOD.requiredHollowType = HCOD.objectHollowType;
            objectsInOwned.Add(OHOD);

            go.GetComponentInChildren<RawImage>().texture = Resources.Load(HCOD.spritePath) as Texture2D;
        }
    }

    [ContextMenu("Refresh Owned Screen")]
    public void RefreshOwnedScreen()
    {
        objectsInOwned.Clear();

        foreach (Transform ownedObject in HollowObjectOwnedContent)
        {
            Destroy(ownedObject.gameObject);
        }

        //FillOwnedScreen();
    }

    [ContextMenu("Refresh Hollow Objects")]
    public void RefreshHollowObjects()
    {
        objectInHollow.Clear();

        foreach (Transform HO in HollowObjectContent)
        {
            Destroy(HO.gameObject);
        }

        FillCraftScreen(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);
    }

    public void OpenOwnedFurnitureToPlace(int typeOfHollow)
    {
        hollowTypeToFill = (ObjectHollowType)typeOfHollow;

        SortMaster.Instance.FilterHollowOwnedScreenByEnum((ObjectHollowType)typeOfHollow);
    }


    public void RemoveFurniture(GameObject ToRemove)
    {
        OwnedHollowObjectData OHODToRemove = ToRemove.GetComponent<OwnedHollowObjectData>();

        PlayerManager.Instance.ownedHollowObjects.Add(OHODToRemove.objectData);
        RefreshOwnedScreen();

        OHODToRemove.GetComponent<RawImage>().texture = null;
        OHODToRemove.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
        OHODToRemove.transform.GetChild(0).gameObject.SetActive(false);
        OHODToRemove.objectData = new HollowCraftObjectData();
        OHODToRemove.gameObject.GetComponent<HollowZone>().isEmpty = true;

    }
}
