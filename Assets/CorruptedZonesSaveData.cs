using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class SavedTimePerZone
{
    //public CorruptedZoneViewHelpData connectedCZVHD;

    public string timelastClosed = " ";

    public CorruptionLevel corruptionLevel;

    public bool isClensing = false;

    //public float timeLeftForClense = 0;

    public float corruptionAmountPerStage = 1500;
    public float originalCorruptionAmountPerStage;
    public float HPM = 0;
    public List<tempMoveScript> currentDevicesInZone;

}
public class CorruptedZonesSaveData : MonoBehaviour
{
    public static CorruptedZonesSaveData Instance;

    public List<SavedTimePerZone> timesPerZones;

    string savePath;

    void Start()
    {
        Instance = this;
        timesPerZones = new List<SavedTimePerZone>();

        InvokeRepeating("SaveIteration", 0, 5f);

        if (Application.platform == RuntimePlatform.Android)
        {
            savePath = Application.persistentDataPath + "/timesPerZones.txt";
        }
        else
        {
            savePath = Application.dataPath + "/timesPerZones.txt";
        }


        if (File.Exists(savePath))
        {
            LoadZonesData();
        }


        DateTime currentTime = DateTime.Now.ToLocalTime();
    }


    public void SaveIteration()
    {
        Debug.Log("Save Corruption");

        if(CorruptedZonesManager.Instance.allCurrentlyCorruptedZonesBeingClensed.Count > 0)
        {
            foreach (CorruptedZoneViewHelpData CZVHD in CorruptedZonesManager.Instance.allCurrentlyCorruptedZonesBeingClensed)
            {
                if (!timesPerZones.Contains(CZVHD.connectedCZD.saveDataZone))
                {
                    timesPerZones.Add(CZVHD.connectedCZD.saveDataZone);
                }
            }
        }
    }

    public void RemoveElementFromSaveData(CorruptedZoneViewHelpData CZVHD)
    {
        if (timesPerZones.Contains(CZVHD.connectedCZD.saveDataZone))
        {
            timesPerZones.Remove(CZVHD.connectedCZD.saveDataZone);
        }
    }

    public void LoadZonesData()
    {

    }
}
