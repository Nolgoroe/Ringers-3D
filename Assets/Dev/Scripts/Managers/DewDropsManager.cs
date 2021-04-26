﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class DewDropsManager : MonoBehaviour
{
    public float timeTillGiveDrewDropStatic;
    public float timeLeftToGiveDrop = 0;
    public int maxDrops;

    public string savedDateTime;

    public string path;

    void Start()
    {
        timeLeftToGiveDrop = timeTillGiveDrewDropStatic * 60;

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/DewDropInfo.txt";
        }
        else
        {
            path = Application.dataPath + "/DewDropInfo.txt";
        }

        if (File.Exists(path))
        {
            LoadDewDropsData();
        }

        DateTime currentTime = DateTime.Now.ToLocalTime();
        Debug.Log(currentTime);

        if (savedDateTime != "")
        {
            //Debug.Log("has previos save time");
            TimeSpan deltaDateTime = DateTime.Parse(savedDateTime) - currentTime;

            Debug.Log(deltaDateTime);

            GiveElapsedTimeDewDrops(deltaDateTime);
        }
        else
        {
            StartCoroutine(DisplayTime());
        }
        savedDateTime = "";
    }

    private void GiveElapsedTimeDewDrops(TimeSpan elapsedTime)
    {
        timeLeftToGiveDrop -= -((float)elapsedTime.TotalSeconds % (timeTillGiveDrewDropStatic * 60));

        if(timeLeftToGiveDrop < 0)
        {
            timeLeftToGiveDrop = (timeTillGiveDrewDropStatic * 60) + timeLeftToGiveDrop; /// its plus because if timeLeftToGiveDrop is below zero then if you use - it'll add to the time
            GiveDrop(1);
        }

        float numDropToGive = (float)elapsedTime.TotalMinutes / timeTillGiveDrewDropStatic;

        int absDrops = (int)Mathf.Abs(numDropToGive);

        if (absDrops > 0)
        {
            GiveDrop(absDrops);

            UIManager.Instance.RefreshDewDropsDisplay(PlayerManager.Instance.collectedDewDrops);

            PlayerManager.Instance.SavePlayerData();
        }

        StartCoroutine(DisplayTime());
    }

    public void OnApplicationQuit()
    {
        savedDateTime = System.DateTime.Now.ToString();

        Debug.Log(savedDateTime);
        SaveDewDropsInfo();
    }

    [ContextMenu("Save")]
    public void SaveDewDropsInfo()
    {
        string savedData = JsonUtility.ToJson(this);

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/DewDropInfo.txt";
        }
        else
        {
            path = Application.dataPath + "/DewDropInfo.txt";
        }
        File.WriteAllText(path, savedData);
    }
    
    public void LoadDewDropsData()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/DewDropInfo.txt";
        }
        else
        {
            path = Application.dataPath + "/DewDropInfo.txt";
        }
        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);
    }

    IEnumerator DisplayTime()
    {
        while (PlayerManager.Instance.collectedDewDrops < maxDrops)
        {
            UIManager.Instance.dewDropsTextTime.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(1);
            timeLeftToGiveDrop--;

            if (timeLeftToGiveDrop <= 0)
            {
                timeLeftToGiveDrop = timeTillGiveDrewDropStatic * 60;

                if (PlayerManager.Instance.collectedDewDrops < maxDrops)
                {
                    GiveDrop(1);
                }
            }

            float minutes = Mathf.FloorToInt(timeLeftToGiveDrop / 60);
            float seconds = Mathf.FloorToInt(timeLeftToGiveDrop % 60);

            UIManager.Instance.dewDropsTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        }

        UIManager.Instance.dewDropsTextTime.gameObject.SetActive(false);
    }

    public void GiveDrop(int amount)
    {
        PlayerManager.Instance.collectedDewDrops += amount;

        if (PlayerManager.Instance.collectedDewDrops > maxDrops)
        {
            PlayerManager.Instance.collectedDewDrops = maxDrops;
        }

        UIManager.Instance.RefreshDewDropsDisplay(PlayerManager.Instance.collectedDewDrops);

        PlayerManager.Instance.SavePlayerData();
    }
}
