using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class ZoneManager : MonoBehaviour
{
    public Zone[] listOfAllZones;
    public List<Zone> listOfUnlockedZones;

    public Zone currentZoneCheck, nextZoneCheck;

    public bool isKeyLevel;

    public static ZoneManager Instance;

    string path;

    private void Start()
    {
        path = Application.dataPath + "/ZoneManager.txt";

        if (File.Exists(path))
        {
            LoadZoneManager();
        }

        Instance = this;
    }

    public void CheckZoneAwardedKey(int current) ////// if zone did not award key yet - they button that has this function connected to it IS the key level
    {
        if (listOfAllZones[current].hasAwardedKey)
        {
            ResetZoneManagerData(); ///// In case of a problem - reset all data to default.
            return;
        }

        currentZoneCheck = listOfAllZones[current];

        isKeyLevel = true;
    }
    public void SetUnlockZone(int zoneToUnlock)
    {
        nextZoneCheck = listOfAllZones[zoneToUnlock];
    }
    public void SetCurrentZone(int current)
    {
        currentZoneCheck = listOfAllZones[current];
    }

    public void ResetZoneManagerData()
    {
        currentZoneCheck = null;
        nextZoneCheck = null;
        isKeyLevel = false;
    }

    [ContextMenu("Save Game Data")]
    public void SaveZoneManager()
    {
        string savedData = JsonUtility.ToJson(this);

        string path = Application.dataPath + "/ZoneManager.txt";

        File.WriteAllText(path, savedData);
    }

    [ContextMenu("Game Load Data")]
    public void LoadZoneManager()
    {
        string path = Application.dataPath + "/ZoneManager.txt";

        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

        Instance = this;
    }
}
