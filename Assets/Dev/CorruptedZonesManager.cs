using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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


    public void SetClensingZone(CorruptedZoneData CZD)
    {
        allCurrentlyCorruptedZonesBeingClensed.Add(CZD.connectedView);

        CZD.transform.SetParent(clensingZone);

        CZD.connectedView.harmonySliderInCorruptedZone.gameObject.SetActive(true);
        CZD.connectedView.harmonySliderOnMap.gameObject.SetActive(true);

        if (!CZD.saveDataZone.isClensing)
        {
            CZD.saveDataZone.isClensing = true;

            CorruptedZonesSaveData.Instance.SaveIteration();
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
