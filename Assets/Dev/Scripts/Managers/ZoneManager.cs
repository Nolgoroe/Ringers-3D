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

    public bool hasFinishedVinebloom = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

        isKeyLevel = false;

    }

    //private void Start()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        path = Application.persistentDataPath + "/ZoneManager.txt";
    //    }
    //    else
    //    {
    //        path = Application.dataPath + "/Save Files Folder/ZoneManager.txt";
    //    }

    //    if (File.Exists(path))
    //    {
    //        LoadZoneManager();
    //    }

    //}

    public void CheckZoneAwardedKey(int current) ////// if zone did not award key yet - the button that has this function connected to it IS the key level
    {
        //if (ZoneManagerHelpData.Instance.listOfAllZones[current].hasAwardedKey)
        //{
        //    ResetZoneManagerData(); ///// In case of a problem - reset all data to default.
        //    return;
        //}

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

        //UIManager.Instance.ChangeZoneName(ZoneManagerHelpData.Instance.currentZoneCheck.zoneName);

        //GameManager.Instance.StartLevel();
    }

    public void ResetZoneManagerData()
    {
        ZoneManagerHelpData.Instance.currentZoneCheck = null;
        ZoneManagerHelpData.Instance.nextZoneCheck = null;
        isKeyLevel = false;
    }

    //[ContextMenu("Save Game Data")]
    //public void SaveZoneManager()
    //{
    //    string savedData = JsonUtility.ToJson(this);

    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/ZoneManager.txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/ZoneManager.txt";
    //    }
    //    File.WriteAllText(path, savedData);
    //}

    //[ContextMenu("Game Load Data")]
    //public void LoadZoneManager()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/ZoneManager.txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/ZoneManager.txt";
    //    }
    //    JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

    //    Instance = this;
    //}

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
            UIManager.isUsingUI = true;

            int num = zonesToUnlock[0];
            //List<int> temp = new List<int>();
            //temp.AddRange(zonesToUnlock);

            //foreach (int num in temp)
            //{
            //unlockedZoneID.Add(num);
            zonesToUnlock.Remove(num);
            UIManager.Instance.DisplayUnlockedZoneMessage(num);
            //}

            //SaveZoneManager();
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
