using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManagerHelpData : MonoBehaviour
{
    public static ZoneManagerHelpData Instance;

    public string currentZoneName;

    public GameObject[] zoneLevelDisplays;

    public Zone[] listOfAllZones;

    public Zone currentZoneCheck, nextZoneCheck;

    public animalsPerZone[] possibleAnimalsPerZone;

    private void Start()
    {
        Instance = this;


        ZoneManager.Instance.ActivateLevelDisplay();
        currentZoneName = null;
    }

    public bool CompareZonesSame(Zone first, Zone second)
    {
        if(first.Equals(second))
        {
            return true;
        }

        return false;
    }
}
