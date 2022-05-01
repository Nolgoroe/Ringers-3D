using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public enum ObjectHollowType
{
    All,
    H1,
    H2,
    H3,
    H4,
    H5,
    H6
}

public enum HollowItems
{
    GrassCot,
    WaterThrought,
    CloverPatch,
    MediumMossCot,
    Fireplace,
    FoodAndWaterBowls,
}

[Serializable]
public class zoneSlotAndType
{
    public HollowZoneSlot zoneSlot;
    public List<ObjectHollowType> acceptedHollowTypes;
}

public class HollowCraftAndOwnedManager : MonoBehaviour
{
    public static HollowCraftAndOwnedManager Instance;

    public GameObject HollowObjectPrefab;
    public GameObject HollowScreenCraftPrefab;
    public Transform HollowObjectContent; /// Parent

    public GameObject HollowObjectOwnedPrefab;
    public Transform HollowObjectOwnedContent; /// Parent

    //public List<HollowObjectDisplayer> objectInHollow; /// Equipment that the player does not have / has not created yet
    //public List<OwnedHollowObjectData> objectsInOwned; /// Equipment that the player does not have / has not created yet

    //public ObjectHollowType hollowTypeToFill;

    public List<zoneSlotAndType> hollowZones;
    public List<GameObject> hollowObjectsCreated;
    //public Dictionary<ObjectHollowType, OwnedHollowObjectData> hollowTypeToGameobject;

    //public bool isPlaceThroughHollow; /// Either place through hollow or thorugh normal open bag

    public HollowObjectDisplayer currentlyToCraftNoramlMehtod;
    public HollowObjectScreenDisplayer currentlyToCraftSecondMethod;

    public Sprite[] denItemSprites;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //hollowTypeToGameobject = new Dictionary<ObjectHollowType, OwnedHollowObjectData>();

        //for (int i = 1; i < System.Enum.GetValues(typeof(ObjectHollowType)).Length; i++)
        //{
        //    hollowTypeToGameobject.Add((ObjectHollowType)i, hollowZones[i - 1].gameObject.GetComponent<OwnedHollowObjectData>());
        //}
        //filledHollowItems = new List<string>();

        foreach (zoneSlotAndType HZS in hollowZones)
        {
            for (int i = 0; i < HZS.zoneSlot.objectsInZone.Length; i++)
            {
                HZS.zoneSlot.objectsInZone[i].gameObject.SetActive(false);
            }
        }

        hollowObjectsCreated = new List<GameObject>();
    }

    public void FillCraftScreen(List<HollowCraftObjectData> HollowCraftObjects)
    {
        hollowObjectsCreated.Clear();


        foreach (HollowCraftObjectData HCOD in HollowCraftObjects)
        {
            FilledItemAndZoneIndex FIAZI = HollowManagerSaveData.Instance.filledHollowItemsToIndex.Where(p => p.hollowItem == HCOD.hollowItemEnum).SingleOrDefault();
            HollowCraftObjectData HCODTemp = PlayerManager.Instance.ownedHollowObjects.Where(p => p.hollowItemEnum == HCOD.hollowItemEnum).SingleOrDefault();

            if(FIAZI == null && HCODTemp == null)
            {
                GameObject go = Instantiate(HollowObjectPrefab, HollowObjectContent);
                HollowObjectDisplayer HOD = go.GetComponent<HollowObjectDisplayer>();
                hollowObjectsCreated.Add(go);

                HOD.itemName.text = HCOD.objectname;
                HOD.objectData = HCOD;

                //HOD.itemImage.texture = Resources.Load(HCOD.spritePath) as Texture2D;
                HOD.itemImage.sprite = denItemSprites[HCOD.spriteIndex];
                HOD.name = HCOD.objectname;
                //objectInHollow.Add(HOD);
                HOD.SpawnMaterialsNeeded(HCOD.mats);
            }
        }
    }


    public void FillHollowScreenCraft(List<HollowCraftObjectData> HollowCraftObjects)
    {
        RefreshHollowScreenObjects();

        foreach (HollowCraftObjectData HCOD in HollowCraftObjects)
        {
            foreach (ObjectHollowType type in HCOD.objectHollowType)
            {
                zoneSlotAndType[] releventZones = hollowZones.Where(p => p.acceptedHollowTypes.Contains(type)).ToArray();

                foreach (zoneSlotAndType ZSAT in releventZones)
                {
                    if (HollowManagerSaveData.Instance.filledHollowItemsToIndex.Count > 0 || PlayerManager.Instance.ownedHollowObjects.Count > 0)
                    {
                        FilledItemAndZoneIndex FIAZI = HollowManagerSaveData.Instance.filledHollowItemsToIndex.Where(p => p.hollowItem == HCOD.hollowItemEnum).SingleOrDefault();

                        HollowCraftObjectData owned = PlayerManager.Instance.ownedHollowObjects.Where(p => p.hollowItemEnum == HCOD.hollowItemEnum).SingleOrDefault();

                        if (FIAZI == null && owned == null)
                        {
                            ZSAT.zoneSlot.InstantiateObject(HCOD);
                        }
                        else
                        {
                            if (owned != null && FIAZI == null)
                            {
                                Debug.LogError("skipped cause OWNED");
                            }

                            if(FIAZI != null && owned == null)
                            {
                                Debug.LogError("skipped cause PLACED");
                            }

                            if (FIAZI != null && owned != null)
                            {
                                Debug.LogError("skipped cause PLACED & OWNED");
                            }
                        }
                    }
                    else
                    {
                        ZSAT.zoneSlot.InstantiateObject(HCOD);
                    }
                }
            }

            //GameObject go = Instantiate(HollowObjectPrefab, HollowObjectContent);
            //HollowObjectDisplayer HOD = go.GetComponent<HollowObjectDisplayer>();

            ////HOD.itemName.text = HCOD.objectname;
            //HOD.objectData = HCOD;

            //HOD.itemImage.texture = Resources.Load(HCOD.spritePath) as Texture2D;

            //HOD.name = HCOD.objectname;
            ////objectInHollow.Add(HOD);
            //HOD.SpawnMaterialsNeeded(HCOD.mats);
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
            //objectsInOwned.Add(OHOD);

            //go.GetComponentInChildren<RawImage>().texture = Resources.Load(HCOD.spritePath) as Texture2D;
            OHOD.itemSprite.sprite = denItemSprites[HCOD.spriteIndex];
        }
    }

    [ContextMenu("Refresh Owned Screen")]
    public void RefreshOwnedScreen()
    {
        //objectsInOwned.Clear();

        foreach (Transform ownedObject in HollowObjectOwnedContent)
        {
            Destroy(ownedObject.gameObject);
        }

        FillOwnedScreen();
    }

    [ContextMenu("Refresh Hollow Objects")]
    public void RefreshHollowObjects()
    {
        //objectInHollow.Clear();

        foreach (Transform HO in HollowObjectContent)
        {
            Destroy(HO.gameObject);
        }


        FillCraftScreen(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);
    }

    [ContextMenu("Refresh Hollow Objects")]
    public void RefreshHollowScreenObjects()
    {
        //objectInHollow.Clear();

        foreach (zoneSlotAndType ZSAT in hollowZones)
        {
            for (int i = 0; i < ZSAT.zoneSlot.hollowObjectZones.Count(); i++)
            {
                GameManager.Instance.DestroyChildrenOfTransform(ZSAT.zoneSlot.hollowObjectZones[i]);
            }
        }
    }

    //public void OpenOwnedFurnitureToPlace(int typeOfHollow)
    //{
    //    hollowTypeToFill = (ObjectHollowType)typeOfHollow;

    //    SortMaster.Instance.FilterHollowOwnedScreenByEnum((ObjectHollowType)typeOfHollow);
    //}


    //public void RemoveFurniture(GameObject ToRemove)
    //{
    //    OwnedHollowObjectData OHODToRemove = ToRemove.GetComponent<OwnedHollowObjectData>();

    //    PlayerManager.Instance.ownedHollowObjects.Add(OHODToRemove.objectData);
    //    RefreshOwnedScreen();

    //    OHODToRemove.GetComponent<RawImage>().texture = null;
    //    OHODToRemove.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
    //    OHODToRemove.transform.GetChild(0).gameObject.SetActive(false);
    //    OHODToRemove.objectData = new HollowCraftObjectData();
    //    OHODToRemove.gameObject.GetComponent<HollowZone>().isEmpty = true;

    //}


    public void BuyHollowAction()
    {
        if (PlayerManager.Instance.rubyCount >= currentlyToCraftNoramlMehtod.rubiesNeededToBuyHollow)
        {
            PlayerManager.Instance.rubyCount -= currentlyToCraftNoramlMehtod.rubiesNeededToBuyHollow;

            currentlyToCraftNoramlMehtod.CraftHollowObject(true);

            UIManager.Instance.updateRubyAndDewDropsCount();


            //PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }


        currentlyToCraftNoramlMehtod = null;
    }

    public void BuyHollowActionSecondary()
    {
        if (PlayerManager.Instance.rubyCount >= currentlyToCraftSecondMethod.rubiesNeededToBuyHollow)
        {
            PlayerManager.Instance.rubyCount -= currentlyToCraftSecondMethod.rubiesNeededToBuyHollow;

            //currentlyToCraftSecondMethod.CraftHollowObject(true);
            currentlyToCraftSecondMethod.CraftHollowObject(true);

            UIManager.Instance.updateRubyAndDewDropsCount();


            //PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }


        currentlyToCraftSecondMethod = null;
    }

    public void DisplayLoadedHollowItems()
    {
        foreach (FilledItemAndZoneIndex FIAZI in HollowManagerSaveData.Instance.filledHollowItemsToIndex)
        {
            hollowZones[FIAZI.zoneIndex].zoneSlot.objectsInZone[FIAZI.indexInZone].gameObject.SetActive(true);
            hollowZones[FIAZI.zoneIndex].zoneSlot.currentFilledAmount++;

            if(hollowZones[FIAZI.zoneIndex].zoneSlot.currentFilledAmount == hollowZones[FIAZI.zoneIndex].zoneSlot.maxFiilledZoneAmount)
            {
                hollowZones[FIAZI.zoneIndex].zoneSlot.isFilled = true;
            }
        }
    }

    public void ResetDenZoneIndications()
    {
        foreach (zoneSlotAndType ZSAT in hollowZones)
        {
            ZSAT.zoneSlot.zoneIndication.SetActive(false);

            TurnOffSpecificHeighlights(ZSAT.zoneSlot.zoneIndication);
        }
    }


    public IEnumerator TurnOnSpecificHeighlights(GameObject turnOn)
    {
        turnOn.SetActive(true);
        yield return new WaitForEndOfFrame();

        TutorialSequence.Instacne.toTextureDenScreen();
    }
    public void TurnOffSpecificHeighlights(GameObject turnOn)
    {
        turnOn.SetActive(false);
        UIManager.Instance.maskImageDenScreen.gameObject.SetActive(false);
        //yield return new WaitForEndOfFrame();


        //TutorialSequence.Instacne.toTexture();
    }

}
