using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IconSpritesPerZone
{
    public Zone zone;
    public Sprite levelDoneSprite, levelFirstTimeIconSprite;
}
public class ZoneManagerHelpData : MonoBehaviour
{
    public static ZoneManagerHelpData Instance;

    public string currentZoneName;

    public GameObject[] zoneLevelDisplays;

    public Zone[] listOfAllZones;

    public Zone currentZoneCheck, nextZoneCheck;

    public animalsPerZone[] possibleAnimalsPerZone;

    public IconSpritesPerZone[] iconsPerZone;

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
