using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptedZoneViewHelpData : MonoBehaviour
{
    public int ZoneIDView;
    public CorruptedZoneData connectedCZD;

    public Transform placedObjetZone;

    public Slider harmonySliderInCorruptedZone;
    public Slider harmonySliderOnMap;

    public bool isFullyClensed;

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
