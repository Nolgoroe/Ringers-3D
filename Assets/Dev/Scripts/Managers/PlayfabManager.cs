using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.IO;
using System.Linq;

public class PlayfabManager : MonoBehaviour
{
    public static bool isLoggedIn = false;

    public enum SystemsToSave { Player, DewDrops, animalManager, corruptedZonesManager, TutorialSaveData, ZoneManager, ZoneX, RewardsManager}

    public static PlayfabManager instance;

    public string playerName;

    public TMP_Text testingContentTabServer;

    public int timeToWaitForDailyRewardSeconds = 0;


    public TMP_InputField userNameInput;
    public TMP_Text displayMessages;

    bool doneWithStep = false;

    public Transform leaderboardDisplayZone;
    public GameObject leaderboardPersonPrefab;
    string filePath;

    public DateTime currentTimeReference;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        displayMessages.text = "";

        string nameInFile = "";

        //// WHAT TO DO WITH IPHONE???
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Application.persistentDataPath + "/username.txt";
        }
        else
        {
            filePath = Application.dataPath + "/Save Files Folder/username.txt";
        }

        if (File.Exists(filePath))
        {
            StreamReader reader = new StreamReader(filePath);
            nameInFile = reader.ReadLine(); //There is only 1 line here always so we can use read line

            reader.Close();

            if (nameInFile != null)
            {
                LoginAutomatically(nameInFile);
            }
        }
    }


    public void LoginAutomatically(string nameInFile)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = nameInFile,
            Password = "123456",
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            },
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    IEnumerator LoginInit()
    {
        displayMessages.text = "Logged In!";

        LoadupAllGameData();

        yield return new WaitUntil(() => doneWithStep == true);

        GetServerCurrentTime();

        yield return new WaitUntil(() => doneWithStep == true);
        doneWithStep = false;
        InitAllSystems();


        // from here on can do actions however we want since we loaded and initted all systems

        //Debug.Log("Debug 2 " + currentTimeReference);
        if(currentTimeReference != DateTime.MinValue)
        {
            RewardsManager.Instance.UpdateCurrentTime(currentTimeReference);
            DewDropsManager.Instance.UpdateCurrentTime(currentTimeReference);

        }

        GetDailyRewardsData();


        yield return new WaitUntil(() => doneWithStep == true);

        RewardsManager.Instance.CalculateReturnDeltaTime();
        DewDropsManager.Instance.CalculateReturnDeltaTime();

        SaveAllGameData();


        UIManager.Instance.PlayButton();

        isLoggedIn = true;
        TimeReferenceDataScript.Start();
    }

    void OnError(PlayFabError error)
    {
        displayMessages.text = error.ErrorMessage;
        Debug.Log("Errrrrror!!! " + error.ErrorMessage);
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int highestLevelReached)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Farthest level",
                    Value = highestLevelReached,
                },
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leaderboard sent!");
    }

    [ContextMenu("Get leaderboard")]
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Farthest level",
            StartPosition = 0,
            MaxResultsCount = 10,
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform child in leaderboardDisplayZone)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject go = Instantiate(leaderboardPersonPrefab, leaderboardDisplayZone);

            PersonLeaderboardData PLD = go.GetComponent<PersonLeaderboardData>();

            PLD.positionText.text = item.Position.ToString();
            PLD.nameText.text = item.DisplayName;
            PLD.maxLevelText.text = findWorldAndLevelByScriptableObject(item.StatValue);

            //Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }

        UIManager.Instance.leaderboardScreen.SetActive(true);
    }

    string findWorldAndLevelByScriptableObject(int levelNum)
    {
        if(levelNum != 0)
        {
            levelNum--; // cause we start checking a list/arry from 0
        }

        int lastNum = GameManager.Instance.allLevels.Last().levelNum;

        if(levelNum > lastNum)
        {
            levelNum = lastNum;
        }

        return (GameManager.Instance.allLevels[levelNum].worldNum + " - " + GameManager.Instance.allLevels[levelNum].levelIndexInZone).ToString();
    }



    [ContextMenu("Get Daily Rewards Data from Server")]
    void GetDailyRewardsData()
    {
        doneWithStep = false;
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onDailyRewardsDataGet, OnError);
    }

    void onDailyRewardsDataGet(GetTitleDataResult result)
    {
        if(result.Data == null)
        {
            Debug.Log("No Message in title data!");
        }

        List<DailyRewardsPacks> dailyRewardPacks = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<List< DailyRewardsPacks>>(result.Data["dailyRewardPacks"]);


        //List<DailyRewardsPacks> dailyRewardPacks = PlayFab.PfEditor.Json.JsonWrapper.DeserializeObject<List<DailyRewardsPacks>>(result.Data["dailyRewardPacks"]);

        RewardsManager.Instance.UpdateRewardListServer(dailyRewardPacks);



        timeToWaitForDailyRewardSeconds = Convert.ToInt32(result.Data["TimeToWaitForDailySeconds"]);


        doneWithStep = true;
    }

    [ContextMenu("Load ALL game data from server - STEP 1")]
    public void LoadupAllGameData()
    {
        // Player Data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
        //DewDropsManager.Instance.LoadDewDropsData();
    }

    void OnDataRecieved(GetUserDataResult result)
    {
        //Debug.Log("Recieved Character Data");

        // Player Data
        if (result.Data != null && result.Data.ContainsKey("Player Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Player Data"].Value, PlayerManager.Instance);
        }

        // Dew Drops Data
        if (result.Data != null && result.Data.ContainsKey("Dew Drops Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Dew Drops Data"].Value, DewDropsManager.Instance);
        }

        // Animal Manager Data
        if (result.Data != null && result.Data.ContainsKey("Animal Manager Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Animal Manager Data"].Value, AnimalsManager.Instance);
        }

        // Corrupted Zones Data
        if (result.Data != null && result.Data.ContainsKey("corrupted Zones Manager Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["corrupted Zones Manager Data"].Value, CorruptedZonesSaveData.Instance);
        }

        // Tutorial Save Data
        if (result.Data != null && result.Data.ContainsKey("Tutorial Save Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Tutorial Save Data"].Value, TutorialSaveData.Instance);
        }

        // Zone Manager Data
        if (result.Data != null && result.Data.ContainsKey("Zone Manager Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Zone Manager Data"].Value, ZoneManager.Instance);
        }

        // Zone X Data
        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            if (result.Data != null && result.Data.ContainsKey("Zone Data" + zone.id))
            {
                JsonUtility.FromJsonOverwrite(result.Data["Zone Data" + zone.id].Value, zone);
            }
        }

        // Rewards Manager Data
        if (result.Data != null && result.Data.ContainsKey("Rewards Manager Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Rewards Manager Data"].Value, RewardsManager.Instance);
        }

        doneWithStep = true;
    }

    [ContextMenu("Step 2")]
    public void  InitAllSystems()
    {
        doneWithStep = false;
        PlayerManager.Instance.Init();
        DewDropsManager.Instance.Init();
        AnimalsManager.Instance.Init();
        CorruptedZonesSaveData.Instance.Init();
        TutorialSaveData.Instance.Init();
        ZoneManager.Instance.Init();
        RewardsManager.Instance.Init();

        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            zone.Init();
        }

    }

    [ContextMenu("Save ALL game data to server - STEP 3")]
    public void SaveAllGameData()
    {
        string savedData = " ";


        // Player Login Username
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Application.persistentDataPath + "/username.txt";
        }
        else
        {
            filePath = Application.dataPath + "/Save Files Folder/username.txt";
        }
        File.WriteAllText(filePath, playerName);

        // Player Data
        savedData = JsonUtility.ToJson(PlayerManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.Player, -1);

        // Dew Drops Data
        savedData = JsonUtility.ToJson(DewDropsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.DewDrops, -1);

        // Animal Manager Data
        savedData = JsonUtility.ToJson(AnimalsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.animalManager, -1);

        // Corrupted Zones Data
        savedData = JsonUtility.ToJson(CorruptedZonesSaveData.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.corruptedZonesManager, -1);

        // Tutorial Save Data
        savedData = JsonUtility.ToJson(TutorialSaveData.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.TutorialSaveData, -1);

        // Zone Manager Data
        savedData = JsonUtility.ToJson(ZoneManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.ZoneManager, -1);

        // Zone X Data
        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            savedData = JsonUtility.ToJson(zone);
            SendDataToBeSavedJson(savedData, SystemsToSave.ZoneX, zone.id);
        }


        doneWithStep = false;


        // Rewards Manager Data
        savedData = JsonUtility.ToJson(RewardsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.RewardsManager, -1);
    }

    public void SendDataToBeSavedJson(string saveData, SystemsToSave system, int zoneNumber)
    {
        UpdateUserDataRequest request = null;

        switch (system)
        {
            case SystemsToSave.Player:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Player Data", saveData }
                    },
                };
                break;
            case SystemsToSave.DewDrops:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Dew Drops Data", saveData }
                    }
                };
                break;
            case SystemsToSave.animalManager:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Animal Manager Data", saveData }
                    }
                };
                break;
            case SystemsToSave.corruptedZonesManager:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "corrupted Zones Manager Data", saveData }
                    }
                };
                break;
            case SystemsToSave.TutorialSaveData:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Tutorial Save Data", saveData }
                    }
                };
                break;
            case SystemsToSave.ZoneManager:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Zone Manager Data", saveData }
                    }
                };
                break;
            case SystemsToSave.ZoneX:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Zone Data" + zoneNumber, saveData }
                    }
                };
                break;
            case SystemsToSave.RewardsManager:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Rewards Manager Data", saveData }
                    }
                };
                break;
            default:
                break;
        }

        if (request != null)
        {
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }
        else
        {
            Debug.LogError("Something went horribly wrong!");
        }
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        doneWithStep = true;
        //Debug.Log("Updated Player Data on Server!");
    }

    [ContextMenu("Reset All Data")]
    public void ResetAllData()
    {
        StartCoroutine(ResetAction());
    }

    IEnumerator ResetAction()
    {
        doneWithStep = false;
        UpdateUserDataRequest request = null;

        request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                { "Player Data", "" },
                { "Dew Drops Data", "" },
                { "Animal Manager Data", "" },
                { "corrupted Zones Manager Data", "" },
                { "Tutorial Save Data", "" },
                { "Zone Manager Data", "" },
                {"Rewards Manager Data", ""}
                
            }
        };

        if (request != null)
        {
            PlayFabClientAPI.UpdateUserData(request, OnDataSendReset, OnError);
        }

        /// what to do with DoneWithStep here???
        
        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                    {
                        { "Zone Data" + zone.id, "" }
                    }
            };

            if (request != null)
            {
                PlayFabClientAPI.UpdateUserData(request, OnDataSendReset, OnError);
            }
        }

        SendLeaderboard(0);
        TimeReferenceDataScript.Reset();
        yield return new WaitUntil(() => doneWithStep == true);

        SceneManager.LoadScene(0);
        doneWithStep = false;

    }


    public void LogOut()
    {
        playerName = null;

        SaveAllGameData();

        StartCoroutine(logOutAction());
    }

    IEnumerator logOutAction()
    {
        yield return new WaitUntil(() => doneWithStep == true);
        SceneManager.LoadScene(0);
    }
    void OnDataSendReset(UpdateUserDataResult result)
    {
        doneWithStep = true;
    }

    public void RegisterButton()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = userNameInput.text,
            Password = "123456",
            RequireBothUsernameAndEmail = false,
            DisplayName = userNameInput.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }
    
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        displayMessages.text = "Registered Successfully!";
        playerName = userNameInput.text;
        StartCoroutine(LoginInit());

        Debug.Log("Registered Successfully!");
    }

    public void LoginButton()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = userNameInput.text,
            Password = "123456",
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            },
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        if(result.InfoResultPayload.PlayerProfile != null)
        {
            playerName = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        StartCoroutine(LoginInit());
    }


    public void GetServerCurrentTime()
    {
        doneWithStep = false;
        PlayFabClientAPI.GetTime(new GetTimeRequest(), OnGetTimeSuccess, OnError);
    }

    void OnGetTimeSuccess(GetTimeResult result)
    {
        currentTimeReference = result.Time;
        Debug.Log(result.Time + " what?");
        //Debug.Log("Debug 1 " + currentTimeReference);

        doneWithStep = true;
    }

    // check if need this AND Focus
    private void OnApplicationPause(bool pause)
    {
        Debug.Log("Pause is: " + pause);
        Debug.Log("is Logged In is: " + isLoggedIn);

        if (pause && isLoggedIn)
        {
            Debug.Log("On Application Pause");
            //GetServerCurrentTime();
            //yield return new WaitUntil(() => doneWithStep == true);

            //DateTime timeSpanSinceStart = Convert.ToDateTime();


            DateTime timeToSave = currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());



            Debug.Log(timeToSave + "Pause time!");

            RewardsManager.Instance.UpdateQuitTime(timeToSave);
            DewDropsManager.Instance.UpdateQuitTime(timeToSave);

            SaveAllGameData();

            Debug.Log("Saved all data! - pause");
        }
        else if(!pause && isLoggedIn)
        {
            DewDropsManager.Instance.UpdateCurrentTime(currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed()));
            DewDropsManager.Instance.CalculateReturnDeltaTime();

            RewardsManager.Instance.UpdateCurrentTime(currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed()));
            RewardsManager.Instance.CalculateReturnDeltaTime();
        }
    }


    // check if need this AND Pause 
    private void OnApplicationFocus(bool focus)
    {
        if (Application.isEditor)
        {
            Debug.Log("focus is: " + focus);
            Debug.Log("is Logged In is: " + isLoggedIn);

            if (!focus && isLoggedIn)
            {
                Debug.Log("On Application Focus");
                //GetServerCurrentTime();
                //yield return new WaitUntil(() => doneWithStep == true);

                DateTime timeToSave = currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());


                Debug.Log(timeToSave + "Focus time!");


                RewardsManager.Instance.UpdateQuitTime(timeToSave);
                DewDropsManager.Instance.UpdateQuitTime(timeToSave);

                SaveAllGameData();

                Debug.Log("Saved all data! - focus");
            }
            else if (focus && isLoggedIn)
            {
                DewDropsManager.Instance.UpdateCurrentTime(currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed()));
                DewDropsManager.Instance.CalculateReturnDeltaTime();

                RewardsManager.Instance.UpdateCurrentTime(currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed()));
                RewardsManager.Instance.CalculateReturnDeltaTime();
            }
        }
    }
}
