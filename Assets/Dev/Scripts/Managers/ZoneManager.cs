using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class ZoneManager : MonoBehaviour
{

    public bool isKeyLevel;

    string path;

    public static ZoneManager Instance;

    public List<int> unlockedZoneID;
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/ZoneManager.txt";
        }
        else
        {
            path = Application.dataPath + "/ZoneManager.txt";
        }

        if (File.Exists(path))
        {
            LoadZoneManager();
        }
        Instance = this;

    }

    public void CheckZoneAwardedKey(int current) ////// if zone did not award key yet - the button that has this function connected to it IS the key level
    {
        if (ZoneManagerHelpData.Instance.listOfAllZones[current].hasAwardedKey)
        {
            ResetZoneManagerData(); ///// In case of a problem - reset all data to default.
            return;
        }

        ZoneManagerHelpData.Instance.currentZoneCheck = ZoneManagerHelpData.Instance.listOfAllZones[current];

        isKeyLevel = true;
    }
    public void SetUnlockZone(int zoneToUnlock)
    {
        ZoneManagerHelpData.Instance.nextZoneCheck = ZoneManagerHelpData.Instance.listOfAllZones[zoneToUnlock];
    }
    public void SetCurrentZone(int current)
    {
        ZoneManagerHelpData.Instance.currentZoneCheck = ZoneManagerHelpData.Instance.listOfAllZones[current];

        UIManager.Instance.ChangeZoneName(ZoneManagerHelpData.Instance.currentZoneCheck.zoneName);
    }

    public void ResetZoneManagerData()
    {
        ZoneManagerHelpData.Instance.currentZoneCheck = null;
        ZoneManagerHelpData.Instance.nextZoneCheck = null;
        isKeyLevel = false;
    }

    [ContextMenu("Save Game Data")]
    public void SaveZoneManager()
    {
        string savedData = JsonUtility.ToJson(this);

        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath + "/ZoneManager.txt";
        }
        else
        {
            string path = Application.dataPath + "/ZoneManager.txt";
        }
        File.WriteAllText(path, savedData);
    }

    [ContextMenu("Game Load Data")]
    public void LoadZoneManager()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath + "/ZoneManager.txt";
        }
        else
        {
            string path = Application.dataPath + "/ZoneManager.txt";
        }
        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

        Instance = this;
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
}
