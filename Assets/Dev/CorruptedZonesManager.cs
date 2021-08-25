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

    private void Start()
    {
        Instance = this;
    }


    public void SetClensingZone()
    {
        if (!currentActiveZoneData.isClensing)
        {
            currentActiveZoneData.isClensing = true;
            currentActiveZoneData.transform.SetParent(clensingZone);
        }
    }
}
