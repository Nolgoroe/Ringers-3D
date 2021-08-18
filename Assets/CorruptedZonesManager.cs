using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptedZonesManager : MonoBehaviour
{
    public static CorruptedZonesManager instance;

    public GameObject lineGO;
    public GameObject generalDevicePrefab;

    public CorruptedZoneData currentActiveZone;

    public tempMoveScript currentDeviceToPlace;
    private void Start()
    {
        instance = this;
    }
}
