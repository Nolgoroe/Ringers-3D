using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptedZonesManager : MonoBehaviour
{
    public static CorruptedZonesManager Instance;

    public GameObject lineGO;
    public GameObject generalDevicePrefab;

    public Transform clensingZone;

    public CorruptedZoneViewHelpData currentActiveZoneView;
    public CorruptedZoneData currentActiveZoneData;

    public tempMoveScript currentDeviceToPlace;

    public CorruptedZoneViewHelpData[] allCorruptedZonesView;
    public List<CorruptedZoneViewHelpData> allCurrentlyCorruptedZonesBeingClensed;

    private void Start()
    {
        Instance = this;
    }


    public void SetClensingZone()
    {
        if (!currentActiveZoneData.saveDataZone.isClensing)
        {
            currentActiveZoneData.saveDataZone.isClensing = true;

            allCurrentlyCorruptedZonesBeingClensed.Add(currentActiveZoneView);

            currentActiveZoneData.transform.SetParent(clensingZone);

            currentActiveZoneData.connectedView.harmonySliderInCorruptedZone.gameObject.SetActive(true);
            currentActiveZoneData.connectedView.harmonySliderOnMap.gameObject.SetActive(true);
        }
    }

    public void RemoveElementFromBeingClensed(CorruptedZoneViewHelpData CZVHD)
    {
        if (allCurrentlyCorruptedZonesBeingClensed.Contains(CZVHD))
        {
            allCurrentlyCorruptedZonesBeingClensed.Remove(CZVHD);
        }
    }
}
