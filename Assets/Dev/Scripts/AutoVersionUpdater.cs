using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class AutoVersionUpdater : MonoBehaviour
{
    public static AutoVersionUpdater instance;
    public int mostRecentGameVersion;
    public int numOfKeyValuePairsInServer;
    int serverVersion;
    int startVersion;

    bool hasUpdated;

    ///sub steps
    public static bool? doneWithSubStep = null;
    //public static int saveCounter;
    public static int resetSystemCounter;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        startVersion = mostRecentGameVersion;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedForPairs, OnError);
    }
    void OnDataRecievedForPairs(GetUserDataResult result)
    {
        numOfKeyValuePairsInServer = result.Data.Keys.Count;
    }

    public IEnumerator CheckMostRecentVersionWithServer()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onGetTitleData, OnError);
        yield return new WaitUntil(() => PlayfabManager.successfullyDoneWithStep != null);
    }

    void onGetTitleData(GetTitleDataResult result)
    {
        if (result.Data == null)
        {
            Debug.LogError("No data recieved from title data");
            PlayfabManager.successfullyDoneWithStep = false;
        }
        else
        {
            serverVersion = Convert.ToInt32(result.Data["Game Most Recent Version"]);

            CompareVersions();
            //StartCoroutine(CompareVersions());
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());

        //PlayfabManager.isSuccessfullConnection = false;
        PlayfabManager.successfullyDoneWithStep = false;
    }

    void CompareVersions()
    {
        if (serverVersion > mostRecentGameVersion)
        {
            hasUpdated = true;

            Debug.LogError("Not same version as server!!!!");

            if (mostRecentGameVersion == 0)
            {
               StartCoroutine(ZeroToOne());
            }
        }
        else
        {
            Debug.LogError("At most recent version!");

            if (hasUpdated)
            {
                UIManager.Instance.SystemUpdaterScreen.SetActive(true);
                UIManager.Instance.SystemUpdaterScreenText.text = "Successfully updated from version " + startVersion + " to version " + serverVersion;
                hasUpdated = false;
            }

            PlayfabManager.successfullyDoneWithStep = true;
        }
    }

    IEnumerator ZeroToOne()
    {
        // Here change data in local build and then save the game.

        mostRecentGameVersion = 1;

        ZoneManager.Instance.unlockedZoneID.RemoveAt(0);

        if(ZoneManager.Instance.unlockedZoneID.Count == 0)
        {
            ZoneManager.Instance.unlockedZoneID.Add(0);
        }

        for (int i = 0; i < ZoneManager.Instance.unlockedZoneID.Count; i++)
        {
            ZoneManager.Instance.unlockedZoneID[i] = i;
        }

        resetSystemCounter = 0;
        yield return StartCoroutine(PlayfabManager.instance.ResetAllDataAutoUpdater());

        //saveCounter = 0;
        //PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ALL});

        //StartCoroutine(WaitForEndSave(2 + ZoneManager.Instance.unlockedZoneID.Count)); // 2(one each for zone manager and VUD) + amount of unlocked zones

        yield return new WaitUntil(() => doneWithSubStep != null);

        //StartCoroutine(CompareVersions());
        resetSystemCounter = 0;
        //saveCounter = 0;

        Debug.Log("Successfull Data Update");
        CompareVersions();
    }


    //public IEnumerator WaitForEndSave(int amountSystemsToSave)
    //{
    //    float counter = 0;

    //    while (saveCounter != amountSystemsToSave)
    //    {
    //        yield return new WaitForSeconds(0.1f);

    //        counter += 0.1f;

    //        if (counter >= 5)
    //        {
    //            break;
    //        }
    //    }

    //    if (saveCounter != amountSystemsToSave)
    //    {
    //        Debug.LogError($"what happened? Counter: {saveCounter} Length:{amountSystemsToSave}");
    //        doneWithSubStep = true;
    //        yield break;
    //    }
    //    else
    //    {
    //        doneWithSubStep = true;
    //    }
    //}
}
