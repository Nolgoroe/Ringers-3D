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
    int serverVersion;

    void Start()
    {
        instance = this;
    }

    public void CheckMostRecentVersionWithServer()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onGetTitleData, OnError);
    }

    void onGetTitleData(GetTitleDataResult result)
    {
        if (result.Data == null)
        {
            Debug.Log("No Message in title data!");
        }
        else
        {
            serverVersion = Convert.ToInt32(result.Data["Game Most Recent Version"]);

            CompareVersions();
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
    }

    void CompareVersions()
    {
        if (serverVersion > mostRecentGameVersion)
        {
            Debug.LogError("Not same version as server!!!!");

            if (mostRecentGameVersion == 0)
            {
                ZeroToOne();
            }
        }
        else
        {
            Debug.LogError("At most recent version!");
            PlayfabManager.doneWithStep = true;
        }
    }

    void ZeroToOne()
    {
        mostRecentGameVersion = 1;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.VersionUpdaterData});

        CompareVersions();
    }
}
