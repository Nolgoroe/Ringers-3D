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
    int startVersion;

    bool hasUpdated;

    void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        startVersion = mostRecentGameVersion;
    }

    public void CheckMostRecentVersionWithServer()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onGetTitleData, OnError);
    }

    void onGetTitleData(GetTitleDataResult result)
    {
        if (result.Data == null)
        {
            Debug.LogError("No data recieved from title data");
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
            hasUpdated = true;

            Debug.LogError("Not same version as server!!!!");

            if (mostRecentGameVersion == 0)
            {
                ZeroToOne();
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
