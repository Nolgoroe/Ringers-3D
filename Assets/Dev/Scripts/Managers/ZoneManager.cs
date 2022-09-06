using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class ZoneManager : MonoBehaviour
{
    /// <summary>
    /// SAVE ZONE IS BEING CALLED TO MANY TIMES - OPTEMIZE THIS!
    /// </summary>
    public bool isKeyLevel;
    public int maxZoneIdReached;

    string path;

    public static ZoneManager Instance;

    public List<int> zonesToUnlock;

    public List<int> unlockedZoneID;

    public static bool CanUnlockZone;

    public bool hasStartedVinebloom = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

        isKeyLevel = false;

    }


    public void CheckZoneAwardedKey(int current) ////// if zone did not award key yet - the button that has this function connected to it IS the key level
    {

        ZoneManagerHelpData.Instance.currentZoneCheck = ZoneManagerHelpData.Instance.listOfAllZones[current];

        ZoneManagerHelpData.Instance.currentZoneName = ZoneManagerHelpData.Instance.currentZoneCheck.zoneName;

        isKeyLevel = true;
    }
    public void SetUnlockZone(int zoneToUnlock)
    {
        if(zoneToUnlock < ZoneManagerHelpData.Instance.listOfAllZones.Length)
        {
            ZoneManagerHelpData.Instance.nextZoneCheck = ZoneManagerHelpData.Instance.listOfAllZones[zoneToUnlock];
        }

    }
    public void SetCurrentZone(int current)
    {
        ZoneManagerHelpData.Instance.currentZoneCheck = ZoneManagerHelpData.Instance.listOfAllZones[current];

        ZoneManagerHelpData.Instance.currentZoneName = ZoneManagerHelpData.Instance.currentZoneCheck.zoneName;
    }

    public void ResetZoneManagerData()
    {
        ZoneManagerHelpData.Instance.currentZoneCheck = null;
        ZoneManagerHelpData.Instance.nextZoneCheck = null;
        isKeyLevel = false;
    }

    public void DiactiavteLevelDisplay()
    {
        for (int i = 0; i < ZoneManagerHelpData.Instance.zoneLevelDisplays.Length; i++)
        {
            ZoneManagerHelpData.Instance.zoneLevelDisplays[i].SetActive(false);
        }
    }
    public void ActivateLevelDisplay()
    {
        for (int i = 0; i < ZoneManagerHelpData.Instance.zoneLevelDisplays.Length; i++)
        {
            ZoneManagerHelpData.Instance.zoneLevelDisplays[i].SetActive(true);
        }
    }

    public void UnlockLevelViewSequence()
    {
        if (zonesToUnlock.Count > 0)
        {

            StartCoroutine(UIManager.Instance.SetIsUsingUI(true));



            int num = zonesToUnlock[0];

            zonesToUnlock.Remove(num);
            UIManager.Instance.DisplayUnlockedZoneMessage(num);
        }
        else
        {
            CanUnlockZone = false;
        }
    }

    public void UpdateMaxZoneIdReached(int id)
    {
        maxZoneIdReached = id;
    }
}
