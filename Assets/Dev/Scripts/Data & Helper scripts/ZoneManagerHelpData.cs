using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManagerHelpData : MonoBehaviour
{
    public GameObject[] zoneLevelDisplays;

    public Zone[] listOfAllZones;

    public Zone currentZoneCheck, nextZoneCheck;

    public static ZoneManagerHelpData Instance;

    public animalsPerZone[] possibleAnimalsInZones;

    private void Start()
    {
        Instance = this;


        ZoneManager.Instance.ActivateLevelDisplay();
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
