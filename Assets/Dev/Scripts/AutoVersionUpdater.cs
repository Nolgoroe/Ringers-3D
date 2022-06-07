using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
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


    int numNeededToCompleteZeroToOne, numCompleteToCompleteZeroToOne;
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
            else if (mostRecentGameVersion == 1)
            {
               StartCoroutine(OneToTwo());
            }
            else
            {
                PlayfabManager.successfullyDoneWithStep = true;
                Debug.Log("Server version is too high for this version of the game.");
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
        yield return new WaitForSeconds(3);
        mostRecentGameVersion = 1;

        Debug.Log("Successfull Data Update");
        CompareVersions();
    }
    IEnumerator OneToTwo()
    {
        // Here change data in local build and then save the game.

        mostRecentGameVersion = 2;

        doneWithSubStep = null;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedOneToTwo, OnError);

        yield return new WaitUntil(() => doneWithSubStep != null);
        Debug.LogError("Done with zero to one!");

        Debug.Log("Successfull Data Update");
        CompareVersions();
    }

    void OnDataRecievedOneToTwo(GetUserDataResult result)
    {

        if (result.Data != null && result.Data.ContainsKey("Zone Manager Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Zone Manager Data"].Value, ZoneManager.Instance);
        }

        ZoneManager.Instance.unlockedZoneID.RemoveAt(0);

        if (ZoneManager.Instance.unlockedZoneID.Count == 0)
        {
            ZoneManager.Instance.unlockedZoneID.Add(0);
            ZoneManagerHelpData.Instance.listOfAllZones[ZoneManager.Instance.unlockedZoneID[0]].isUnlocked = true;
            ZoneManagerHelpData.Instance.listOfAllZones[ZoneManager.Instance.unlockedZoneID[0]].maxLevelReachedInZone = 1;
        }

        for (int i = 0; i < ZoneManager.Instance.unlockedZoneID.Count; i++)
        {
            ZoneManager.Instance.unlockedZoneID[i] = i;
            if (ZoneManagerHelpData.Instance.listOfAllZones[ZoneManager.Instance.unlockedZoneID[0]].maxLevelReachedInZone <= 0)
            {
                ZoneManagerHelpData.Instance.listOfAllZones[ZoneManager.Instance.unlockedZoneID[0]].maxLevelReachedInZone = 1;
            }
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneManager });


        numNeededToCompleteZeroToOne = 0;
        UserDataRecord USR;

        for (int i = 0; i < 6; i++)
        {
            if (result.Data.TryGetValue("Zone Data" + i, out USR))
            {
                numNeededToCompleteZeroToOne++;
            }
        }

        for (int i = 0; i < 6; i++)
        {            
            if (result.Data.TryGetValue("Zone Data" + i, out USR))
            {
                JsonObject testJsonObj = (JsonObject)PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject(USR.Value);

                object testJsonValue;

                if (testJsonObj.TryGetValue("zoneName", out testJsonValue))
                {
                    string savedData = testJsonObj.ToString();

                    UpdateUserDataRequest request = null;

                    if(i == 0)
                    {
                        request = new UpdateUserDataRequest
                        {
                            Data = new Dictionary<string, string>()
                            {
                                { "Zone Data" + i, null }
                            }
                        };

                    }
                    else
                    {
                        request = new UpdateUserDataRequest
                        {
                            Data = new Dictionary<string, string>()
                            {
                                { "Zone Data " + testJsonValue, savedData },
                                { "Zone Data" + i, null }
                            }
                        };

                    }

                    if (request != null)
                    {
                        PlayFabClientAPI.UpdateUserData(request, OnDataSendOneToTwo, OnErrorOneToTwo);
                    }
                }
            }
        }
    }

    void OnDataSendOneToTwo(UpdateUserDataResult result)
    {
        numCompleteToCompleteZeroToOne++;

        if(numCompleteToCompleteZeroToOne == numNeededToCompleteZeroToOne)
        {
            doneWithSubStep = true;
        }
    }

    void OnErrorOneToTwo(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());

        doneWithSubStep = true;
    }
}
