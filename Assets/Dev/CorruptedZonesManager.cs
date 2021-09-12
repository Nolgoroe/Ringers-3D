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


    [Header("Clamp Objects")]
    public float minClampPosX;
    public float maxClampPosX, minClampPosY, maxClampPosY;

    private void Start()
    {
        Instance = this;
    }


    public void SetClensingZone(CorruptedZoneData CZD)
    {
        if (!allCurrentlyCorruptedZonesBeingClensed.Contains(CZD.connectedView))
        {
            allCurrentlyCorruptedZonesBeingClensed.Add(CZD.connectedView);

            CZD.connectedView.harmonySliderInCorruptedZone.gameObject.SetActive(true);
            CZD.connectedView.harmonySliderOnMap.gameObject.SetActive(true);
        }

        CZD.transform.SetParent(clensingZone);


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
