using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using System.Linq;

public class HollowZoneSlot : MonoBehaviour
{
    public int zoneIndex;

    public List<ObjectHollowType> acceptedHollowItemTypes;

    public GameObject[] objectsInZone;
    public Transform[] hollowObjectZones;

    public void CheckWork(ObjectHollowType typeWorked)
    {
        Debug.Log("Is this detectiong the hollow zone? " + typeWorked);
    }

    public void PlaceHollowObject(int indexInZone, HollowItems itemEnum)
    {
        objectsInZone[indexInZone].SetActive(true);
        FilledItemAndZoneIndex FITZI = new FilledItemAndZoneIndex();
        FITZI.hollowItem = itemEnum;
        FITZI.zoneIndex = zoneIndex;
        FITZI.indexInZone = indexInZone;

        HollowManagerSaveData.Instance.filledHollowItemsToIndex.Add(FITZI);

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
            }
        }
    }

    public void InstantiateObject(HollowCraftObjectData HCOD)
    {
            GameObject go = Instantiate(HollowCraftAndOwnedManager.Instance.HollowScreenCraftPrefab, hollowObjectZones[HCOD.indexInHollow]);
            HollowObjectScreenDisplayer HOZD = go.GetComponent<HollowObjectScreenDisplayer>();

            HOZD.objectIcon.texture = Resources.Load(HCOD.spritePath) as Texture2D;

            HOZD.objectData = HCOD;

            HOZD.SpawnMaterialsNeeded(HCOD.mats);

            HOZD.connectedZoneSlot = this;

    }

}
