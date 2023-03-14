using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Globalization;

public class DewDropsManager : MonoBehaviour
{
    public static DewDropsManager Instance;

    public float timeTillGiveDrewDropStatic;
    public float timeLeftToGiveDrop = 0;
    public int maxDrops;

    public string savedDateTime;
    public DateTime currentTime;
    public string CurrentTimeTemp;

    public string path;

    private void Update()
    {
        CurrentTimeTemp = currentTime.ToString();
    }
    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

        //timeLeftToGiveDrop = timeTillGiveDrewDropStatic * 60;

        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    path = Application.persistentDataPath + "/DewDropInfo.txt";
        //}
        //else
        //{
        //    path = Application.dataPath + "/Save Files Folder/DewDropInfo.txt";
        //}

        //LoadDewDropsData();

        UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();

    }

    public void CalculateReturnDeltaTime()
    {
        //DateTime currentTime = DateTime.Now.ToLocalTime();
        //Debug.Log(currentTime);

        //Debug.Log("Debug 7 " + savedDateTime);
        //Debug.Log("Debug 8 " + currentTime);
        StopAllCoroutines();
        if (savedDateTime != "" && PlayerManager.Instance.collectedDewDrops < maxDrops)
        {
            //Debug.Log("has previos save time: " + savedDateTime);
            TimeSpan deltaDateTime = currentTime - Convert.ToDateTime(savedDateTime);
            //DateTime saved = DateTime.ParseExact(savedDateTime, formats, CultureInfo.CurrentCulture);
            //TimeSpan deltaDateTime = currentTime - saved;

            //Debug.LogError("THIS IS THE DELTA TIME: " + deltaDateTime);

            GiveElapsedTimeDewDrops(deltaDateTime);
        }
        else
        {
            timeLeftToGiveDrop = timeTillGiveDrewDropStatic * 60;
            StartCoroutine(DisplayTime());
        }

        //savedDateTime = "";
    }

    private void GiveElapsedTimeDewDrops(TimeSpan elapsedTime)
    {
        //timeLeftToGiveDrop = timeLeftToGiveDrop - Math.Abs((float)elapsedTime.TotalSeconds);
        timeLeftToGiveDrop -= ((float)elapsedTime.TotalSeconds % (timeTillGiveDrewDropStatic * 60));
        int absDrops = 0;

        if (Mathf.Abs((float)elapsedTime.TotalMinutes) >= 1)
        {
            float numDropToGive = (float)elapsedTime.TotalMinutes / timeTillGiveDrewDropStatic;

            absDrops = (int)Mathf.Abs(numDropToGive);
        }
        else
        {
            //Debug.LogError("1111 GIVE!!! TIME LEFT IS: " + timeLeftToGiveDrop);

            if (timeLeftToGiveDrop < 0)
            {
                timeLeftToGiveDrop = (timeTillGiveDrewDropStatic * 60) + timeLeftToGiveDrop;

                savedDateTime = currentTime.ToString();

                //Debug.LogError("GIVE!!! TIME LEFT IS: " + timeLeftToGiveDrop);
                GiveDrop(1);
            }
            else
            {
                //Debug.Log("NO GIVE!!! TIME LEFT IS: " + timeLeftToGiveDrop);
            }
        }

        if (absDrops > 0)
        {
            //Debug.Log("GIVE!!! AMOUNT: " + absDrops);

            GiveDrop(absDrops);

            //UIManager.Instance.RefreshDewDropsDisplay(PlayerManager.Instance.collectedDewDrops);

            //PlayerManager.Instance.SavePlayerData();
        }

        StartCoroutine(DisplayTime());
    }

    //public void OnApplicationQuit()
    //{
    //    savedDateTime = System.DateTime.Now.ToString();

    //    Debug.Log(savedDateTime);
    //    Debug.Log("Application is Quitting");
    //    RewardsManager.Instance.savedDateTime
    //    PlayfabManager.instance.SaveAllGameData();
    //    //SaveDewDropsInfo();
    //}

    //[ContextMenu("Save")]
    //public void SaveDewDropsInfo()
    //{
    //    string savedData = JsonUtility.ToJson(this);

    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        path = Application.persistentDataPath + "/DewDropInfo.txt";
    //    }
    //    else
    //    {
    //        path = Application.dataPath + "/Save Files Folder/DewDropInfo.txt";
    //    }
    //    File.WriteAllText(path, savedData);
    //}
    
    //public void LoadDewDropsData()
    //{
    //    JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

    //    UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
    //}

    public IEnumerator DisplayTime()
    {
        DisplayTimeNoDelay();
        while (PlayerManager.Instance.collectedDewDrops < maxDrops)
        {
            UIManager.Instance.dewDropsTextTime.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(1);
            //Debug.Log("Inside Coroutine!");

            timeLeftToGiveDrop--;

            if (timeLeftToGiveDrop <= 0)
            {
                timeLeftToGiveDrop = timeTillGiveDrewDropStatic * 60;

                if (PlayerManager.Instance.collectedDewDrops < maxDrops)
                {
                    //Debug.LogError("222 GIVE!!! TIME LEFT IS: " + timeLeftToGiveDrop);

                    GiveDrop(1);
                }
            }

            float minutes = Mathf.FloorToInt(timeLeftToGiveDrop / 60);
            float seconds = Mathf.FloorToInt(timeLeftToGiveDrop % 60);

            UIManager.Instance.dewDropsTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        }

        UIManager.Instance.dewDropsTextTime.gameObject.SetActive(false);
    }

    void DisplayTimeNoDelay() ///// This function is only for the start of the game so that players wont see the defult time while the real time is updating
    {
        float minutes = Mathf.FloorToInt(timeLeftToGiveDrop / 60);
        float seconds = Mathf.FloorToInt(timeLeftToGiveDrop % 60);

        UIManager.Instance.dewDropsTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GiveDrop(int amount)
    {
        //Debug.LogError("Gave drop");
        PlayerManager.Instance.collectedDewDrops += amount;

        if (PlayerManager.Instance.collectedDewDrops > maxDrops)
        {
            PlayerManager.Instance.collectedDewDrops = maxDrops;
        }

        UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
        //UIManager.Instance.RefreshDewDropsDisplay(PlayerManager.Instance.collectedDewDrops);

        //PlayerManager.Instance.SavePlayerData();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.DewDrops});
    }

    public void GiveDropCheat(int amount)
    {
        PlayerManager.Instance.collectedDewDrops += amount;
        UIManager.Instance.dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.DewDrops });

    }

    public void UpdateCurrentTime(DateTime time)
    {
        currentTime = time;

        //Debug.Log("Debug 5 " + currentTime);
    }

    public void UpdateQuitTime(DateTime time)
    {
        savedDateTime = time.ToString();

        //Debug.Log("Debug 6 " + savedDateTime);
    }
}
