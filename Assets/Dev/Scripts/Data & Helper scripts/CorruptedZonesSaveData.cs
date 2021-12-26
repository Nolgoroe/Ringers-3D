using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

[System.Serializable]

public class savedDevicesData
{
    public CorruptedDevicesData deviceData;

    public Vector3 position;

    public int zoneID;
}

[System.Serializable]
public class SavedTimePerZone
{
    //public CorruptedZoneViewHelpData connectedCZVHD;

    public int ID;

    public string timelastClosed = " ";

    public string timeElapsed = " ";

    public CorruptionLevel corruptionLevel;

    public bool isClensing = false;
    public bool isCompletlyClensed = false;

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

    public List<savedDevicesData> devicesToSave;

    string savePath;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

        timesPerZones = new List<SavedTimePerZone>();
        devicesToSave = new List<savedDevicesData>();

        UpdateStateInGame();

        InvokeRepeating("SaveIteration", 0, 5f);

        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    savePath = Application.persistentDataPath + "/CorruptZoneSaveData.txt";
        //}
        //else
        //{
        //    savePath = Application.dataPath + "/Save Files Folder/CorruptZoneSaveData.txt";
        //}


        //if (File.Exists(savePath))
        //{
        //    LoadZonesData();
        //}
    }


    public void SaveIteration() ///// THIS IS INVOKED!!
    {
        //Debug.Log("Save Corruption");

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

        //SaveZonesData();
    }

    //public void RemoveElementFromSaveData(CorruptedZoneViewHelpData CZVHD)
    //{
    //    if (timesPerZones.Contains(CZVHD.connectedCZD.saveDataZone))
    //    {
    //        timesPerZones.Remove(CZVHD.connectedCZD.saveDataZone);
    //    }
    //}

    //public void LoadZonesData()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        savePath = Application.persistentDataPath + "/CorruptZoneSaveData.txt";
    //    }
    //    else
    //    {
    //        savePath = Application.dataPath + "/Save Files Folder/CorruptZoneSaveData.txt";
    //    }

    //    JsonUtility.FromJsonOverwrite(File.ReadAllText(savePath), this);


    //    UpdateStateInGame();
    //}

    private void UpdateStateInGame()
    {
        foreach (SavedTimePerZone STPZ in timesPerZones)
        {
            CorruptedZoneViewHelpData CZVHD = CorruptedZonesManager.Instance.allCorruptedZonesView.Where(p => p.ZoneIDView == STPZ.ID).Single();

            if (STPZ.isClensing)
            {
                CZVHD.connectedCZD.connectedView = CZVHD;

                CZVHD.connectedCZD.saveDataZone = STPZ;

                CZVHD.connectedCZD.gameObject.SetActive(true);

                UpdateCorruptionLevelElapsedTime(STPZ, CZVHD);

                if (CZVHD.connectedCZD)
                {
                    CorruptedZonesManager.Instance.SetClensingZone(CZVHD.connectedCZD);
                }
            }

            if (STPZ.isCompletlyClensed)
            {
                if (CZVHD.connectedCZD)
                {
                    CZVHD.connectedCZD.connectedView = CZVHD;

                    CZVHD.connectedCZD.FullyClensedLogic();
                }
            }
        }

        List<savedDevicesData> tempList = new List<savedDevicesData>();
        tempList = devicesToSave;

        foreach (savedDevicesData SDD in tempList)
        {
            CorruptedZoneViewHelpData CZVHD = CorruptedZonesManager.Instance.allCorruptedZonesView.Where(p => p.ZoneIDView == SDD.zoneID).Single();

            if (CZVHD.isFullyClensed)
            {
                devicesToSave.Remove(SDD);
            }
            else
            {
                GameObject device = Instantiate(CorruptedZonesManager.Instance.generalDevicePrefab, CZVHD.placedObjetZone);
                device.name = SDD.deviceData.deviceName;

                RectTransform RT = device.GetComponent<RectTransform>();

                RT.anchoredPosition3D = SDD.position;


                GameObject go = Resources.Load<GameObject>(SDD.deviceData.prefabPath);

                Instantiate(go, device.transform);
            }
        }
        devicesToSave = tempList;
        //ClearTimesPerZonesListAfterLoad();
    }

    public void UpdateCorruptionLevelElapsedTime(SavedTimePerZone STPZ, CorruptedZoneViewHelpData CZVHD)
    {
        DateTime currentTime = DateTime.Now.ToLocalTime();

        if (STPZ.timelastClosed != " ")
        {
            TimeSpan deltaDateTime = DateTime.Parse(STPZ.timelastClosed) - currentTime;

            STPZ.timeElapsed = deltaDateTime.ToString();

            int minutes = Mathf.Abs(deltaDateTime.Minutes);
            int seconds = Mathf.Abs(deltaDateTime.Seconds);

            float totalAmountsByMinutes = minutes * STPZ.HPM;
            float totalAmountsBySeconds = (STPZ.HPM / 60) * seconds;

            float totalSum = totalAmountsByMinutes + totalAmountsBySeconds;


            float totalSumOutOfCurrentCorruption = totalSum / STPZ.originalCorruptionAmountPerStage;

            if(totalSumOutOfCurrentCorruption >= 1)
            {
                STPZ.corruptionLevel -= (int)totalSumOutOfCurrentCorruption;

                if(STPZ.corruptionLevel <= CorruptionLevel.level0)
                {
                    STPZ.corruptionLevel = CorruptionLevel.level0;
                    CZVHD.connectedCZD.FullyClensedLogic();
                    return;
                }

                float moduloNum = totalSum % STPZ.originalCorruptionAmountPerStage;

                STPZ.corruptionAmountPerStage = STPZ.originalCorruptionAmountPerStage - moduloNum;
            }
            else
            {
                STPZ.corruptionAmountPerStage -= totalSum;

                if(STPZ.corruptionAmountPerStage <= 0)
                {
                    STPZ.corruptionLevel--;

                    if (STPZ.corruptionLevel <= CorruptionLevel.level0)
                    {
                        STPZ.corruptionLevel = CorruptionLevel.level0;

                        CZVHD.connectedCZD.FullyClensedLogic();
                        return;
                    }

                    STPZ.corruptionAmountPerStage = STPZ.originalCorruptionAmountPerStage - Mathf.Abs(STPZ.corruptionAmountPerStage);
                }
            }
        }
    }

    //public void SaveZonesData()
    //{
    //    string savedData = JsonUtility.ToJson(this);

    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        savePath = Application.persistentDataPath + "/CorruptZoneSaveData.txt";
    //    }
    //    else
    //    {
    //        savePath = Application.dataPath + "/Save Files Folder/CorruptZoneSaveData.txt";
    //    }
    //    File.WriteAllText(savePath, savedData);
    //}


    //public void ClearTimesPerZonesListAfterLoad()
    //{
    //    List<SavedTimePerZone> tempTimesPerZones = new List<SavedTimePerZone>();

    //    tempTimesPerZones.AddRange(timesPerZones);

    //    foreach (SavedTimePerZone STPZ in tempTimesPerZones)
    //    {
    //        if (STPZ.isCompletlyClensed)
    //        {
    //            timesPerZones.Remove(STPZ);
    //        }
    //    }
    //}
    private void OnApplicationQuit()
    {
        foreach (SavedTimePerZone STPZ in timesPerZones)
        {
            STPZ.timelastClosed = DateTime.Now.ToLocalTime().ToString();
        }

        //SaveZonesData();
    }

    public void SetNewDeviceToSave(CorruptedDevicesData CDD, Vector3 position)
    {
        savedDevicesData newDevice = new savedDevicesData();

        newDevice.deviceData = CDD;
        newDevice.zoneID = CorruptedZonesManager.Instance.currentActiveZoneView.ZoneIDView;
        newDevice.position = position;

        devicesToSave.Add(newDevice);

    }
}
