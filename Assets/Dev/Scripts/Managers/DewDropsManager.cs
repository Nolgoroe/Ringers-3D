using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class DewDropsManager : MonoBehaviour
{
    public static DewDropsManager Instance;

    public float timeTillGiveDrewDropStatic;
    public float timeLeftToGiveDrop = 0;
    public int maxDrops;

    public string savedDateTime;

    public string path;

    void Start()
    {
        Instance = this;

        timeLeftToGiveDrop = timeTillGiveDrewDropStatic * 60;

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/DewDropInfo.txt";
        }
        else
        {
            path = Application.dataPath + "/Save Files Folder/DewDropInfo.txt";
        }

        if (File.Exists(path))
        {
            LoadDewDropsData();
        }
        else
        {
            UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
        }

        DateTime currentTime = DateTime.Now.ToLocalTime();
        //Debug.Log(currentTime);

        if (savedDateTime != "")
        {
            //Debug.Log("has previos save time: " + savedDateTime);
            TimeSpan deltaDateTime = DateTime.Parse(savedDateTime) - currentTime;

            //Debug.Log("THIS IS THE DELTA TIME: " + deltaDateTime);

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
            //Debug.Log("GIVE!!! TIME LEFT IS: " + timeLeftToGiveDrop);
            GiveDrop(1);
        }
        else
        {
            //Debug.Log("NO GIVE!!! TIME LEFT IS: " + timeLeftToGiveDrop);
        }
        float numDropToGive = (float)elapsedTime.TotalMinutes / timeTillGiveDrewDropStatic;

        int absDrops = (int)Mathf.Abs(numDropToGive);

        if (absDrops > 0)
        {
            //Debug.Log("GIVE!!! AMOUNT: " + absDrops);

            GiveDrop(absDrops);

            //UIManager.Instance.RefreshDewDropsDisplay(PlayerManager.Instance.collectedDewDrops);

            PlayerManager.Instance.SavePlayerData();
        }

        StartCoroutine(DisplayTime());
    }

    public void OnApplicationQuit()
    {
        savedDateTime = System.DateTime.Now.ToString();

        Debug.Log(savedDateTime);
        Debug.Log("Application is Quitting");
        SaveDewDropsInfo();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            savedDateTime = System.DateTime.Now.ToString();

            Debug.Log(savedDateTime);
            Debug.Log("PAUSED " + pause);
            SaveDewDropsInfo();
        }
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
            path = Application.dataPath + "/Save Files Folder/DewDropInfo.txt";
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
            path = Application.dataPath + "/Save Files Folder/DewDropInfo.txt";
        }

        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

        UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
    }

    public IEnumerator DisplayTime()
    {
        DisplayTimeNoDelay();
        while (PlayerManager.Instance.collectedDewDrops < maxDrops)
        {
            UIManager.Instance.dewDropsTextTime.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(1);
            Debug.Log("Inside Coroutine!");
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

    void DisplayTimeNoDelay() ///// This function is only for the star of the game no that players wont see the defult time while the real time is updating
    {
        float minutes = Mathf.FloorToInt(timeLeftToGiveDrop / 60);
        float seconds = Mathf.FloorToInt(timeLeftToGiveDrop % 60);

        UIManager.Instance.dewDropsTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GiveDrop(int amount)
    {
        PlayerManager.Instance.collectedDewDrops += amount;

        if (PlayerManager.Instance.collectedDewDrops > maxDrops)
        {
            PlayerManager.Instance.collectedDewDrops = maxDrops;
        }

        UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
        //UIManager.Instance.RefreshDewDropsDisplay(PlayerManager.Instance.collectedDewDrops);

        PlayerManager.Instance.SavePlayerData();
    }
}
