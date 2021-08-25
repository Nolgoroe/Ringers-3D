using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedZoneViewHelpData : MonoBehaviour
{
    public int ZoneID;
    public CorruptedZoneData connectedCZD;

    public Transform placedObjetZone;

    //private void Start()
    //{
    //    connectedCZD = GetComponentInChildren<CorruptedZoneData>();
    //}

    public void UpdateCorruptionManagerData()
    {
        CorruptedZonesManager.Instance.currentActiveZoneData = connectedCZD;
        CorruptedZonesManager.Instance.currentActiveZoneView = this;
    }
}
