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

    public int maxFiilledZoneAmount;
    public int currentFilledAmount;
    public bool isFilled;

    public GameObject zoneIndication;

    private void Start()
    {
        zoneIndication.SetActive(false);
    }

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
                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
            }
        }

        currentFilledAmount++;

        if (currentFilledAmount == maxFiilledZoneAmount)
        {
            isFilled = true;
        }

        objectsInZone[indexInZone].GetComponent<Animator>().SetBool("Release Now", true);
        AnimationManager.instance.isPlacingDenItem = true; // this is here to diable the option of moving out of screen until animation is done!
    }

    public void InstantiateObject(HollowCraftObjectData HCOD)
    {
        GameObject go = Instantiate(HollowCraftAndOwnedManager.Instance.HollowScreenCraftPrefab, hollowObjectZones[HCOD.indexInHollow]);
        HollowObjectScreenDisplayer HOZD = go.GetComponent<HollowObjectScreenDisplayer>();

        //HOZD.objectIcon.texture = Resources.Load(HCOD.spritePath) as Texture2D;
        HOZD.objectIcon.sprite = HollowCraftAndOwnedManager.Instance.denItemSprites[HCOD.spriteIndex];

        HOZD.objectData = HCOD;

        HOZD.SpawnMaterialsNeeded(HCOD.mats);

        HOZD.connectedZoneSlot = this;

    }
}
