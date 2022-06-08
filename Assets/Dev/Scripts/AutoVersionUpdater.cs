using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Linq;

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

        Debug.LogError("Compared versions and updated the game!!");
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

            //Debug.LogError("Not same version as server!!!!");

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
            //Debug.LogError("At most recent version!");

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
        yield return new WaitForSeconds(2);
        mostRecentGameVersion = 1;

        Debug.Log("Successfull Data Update Zero to one");
        CompareVersions();
    }
    IEnumerator OneToTwo()
    {
        // Here change data in local build and then save the game.

        mostRecentGameVersion = 2;

        doneWithSubStep = null;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedOneToTwo, OnErrorRecieveOneToTwo); // change zone manager data + rewrite zones from index based to unique identifier base

        yield return new WaitUntil(() => doneWithSubStep != null);

        doneWithSubStep = null;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedUpdateZones, OnErrorUpdateZones); // load zones data from server - now with new unique identifier base

        yield return new WaitUntil(() => doneWithSubStep != null);

        doneWithSubStep = null;
        StartCoroutine(ResetZoneIndexesOneToTwo()); // reset zone indexes to new indexes - we took out zone index 0 - noew we need to make sure the indexes go from 0 - 4 instead of 1 - 5 since we have arrays using the zones id's

        yield return new WaitUntil(() => doneWithSubStep != null);
        //Debug.LogError("Done with zero to one!");

        doneWithSubStep = null;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedUpdateTutorialData, OnErrorUpdateZones); // rewrite tutorial data

        yield return new WaitUntil(() => doneWithSubStep != null);

        Debug.Log("Successfull Data Update One to two!");
        CompareVersions();
    }

    void OnErrorUpdateTutorialData(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
        doneWithSubStep = true;
    }

    void OnDataRecievedUpdateTutorialData(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Tutorial Save Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Tutorial Save Data"].Value, TutorialSaveData.Instance);
        }

        TutorialSaveData.Instance.completedTutorialLevelId.Clear();
        TutorialSaveData.Instance.completedSpecificTutorialLevelId.Clear();

        StartCoroutine(ResetTutorialDataOneToTwo());
        //ResetTutorialDataOneToTwo();
    }

    IEnumerator ResetTutorialDataOneToTwo()
    {
        foreach (int zoneindex in ZoneManager.Instance.unlockedZoneID)
        {
            Zone zone = ZoneManagerHelpData.Instance.listOfAllZones[zoneindex];

            AllZonesAndLevels AZAL = GameManager.Instance.allZonesAndLevels.Where(p => p.zone == zone).SingleOrDefault();

            for (int i = 0; i < zone.maxLevelReachedInZone; i++)
            {
                if(AZAL.levelsInZone[i].isTutorial)
                {
                    TutorialSaveData.Instance.completedTutorialLevelId.Add(AZAL.levelsInZone[i].levelNum);
                }
                
                if(AZAL.levelsInZone[i].isSpecificTutorial)
                {
                    TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add(AZAL.levelsInZone[i].levelNum);
                }
                

            }
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

        yield return new WaitForSeconds(2);

        //yield return null;
        doneWithSubStep = true;
    }



    void OnErrorUpdateZones(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
        doneWithSubStep = true;
    }
    void OnDataRecievedUpdateZones(GetUserDataResult result)
    {
        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            if (result.Data != null && result.Data.ContainsKey("Zone Data " + zone.zoneName))
            {
                JsonUtility.FromJsonOverwrite(result.Data["Zone Data " + zone.zoneName].Value, zone);
            }
        }

        doneWithSubStep = true;
        //Debug.LogError("Loaded levels");
    }

    void OnErrorRecieveOneToTwo(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
        doneWithSubStep = true;
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
        }

        if (ZoneManagerHelpData.Instance.listOfAllZones[ZoneManager.Instance.unlockedZoneID[0]].maxLevelReachedInZone <= 0)
        {
            ZoneManagerHelpData.Instance.listOfAllZones[ZoneManager.Instance.unlockedZoneID[0]].maxLevelReachedInZone = 1;
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneManager });




        numNeededToCompleteZeroToOne = 0;
        UserDataRecord USR;

        for (int i = 0; i < 6; i++) //6 was the number of all zones in this version of the game. we need to potentially check for all of them
        {
            if (result.Data.TryGetValue("Zone Data" + i, out USR))
            {
                numNeededToCompleteZeroToOne++;

            }
        }

        if(numNeededToCompleteZeroToOne == 0)
        {
            doneWithSubStep = true;
            return;
        }

        for (int i = 0; i < numNeededToCompleteZeroToOne; i++)
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
                        PlayFabClientAPI.UpdateUserData(request, OnDataSendOneToTwo, OnErrorOneToTwoDataSend);
                    }
                }
            }
        }

    }

    void OnDataSendOneToTwo(UpdateUserDataResult result)
    {
        numCompleteToCompleteZeroToOne++;

        if (numCompleteToCompleteZeroToOne == numNeededToCompleteZeroToOne)
        {
            doneWithSubStep = true;

            //Debug.LogError("Now done with transferring data!!!");
        }
    }
    IEnumerator ResetZoneIndexesOneToTwo()
    {
        for (int i = 0; i < ZoneManagerHelpData.Instance.listOfAllZones.Length; i++)
        {
            ZoneManagerHelpData.Instance.listOfAllZones[i].id = i;
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.AllZones });

        //Debug.LogError("Now waiting");

        yield return new WaitForSeconds(2);
        doneWithSubStep = true;
    }
    void OnErrorOneToTwoDataSend(PlayFabError error)
    {
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());

        doneWithSubStep = true;
    }
}
