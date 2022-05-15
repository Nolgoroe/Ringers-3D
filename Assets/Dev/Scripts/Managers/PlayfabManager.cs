﻿using System.Collections;
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

public enum SystemsToSave 
{ 
    Player, 
    DewDrops, 
    animalManager, 
    corruptedZonesManager, 
    TutorialSaveData, 
    ZoneManager, 
    ZoneX, 
    RewardsManager, 
    LoginData, 
    HollowManager,
    VersionUpdaterData,
    CheatingSaveData,
    BossesSaveData,
    ALL
}

public class PlayfabManager : MonoBehaviour
{
    public static bool isLoggedIn = false;
    public static bool doneWithStep = false;

    public static PlayfabManager instance;

    public string playerName;

    public TMP_Text testingContentTabServer;

    public int timeToWaitForDailyRewardSeconds = 0;


    public TMP_InputField userNameInput;
    public TMP_Text displayMessages;


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
        //// WE DO NOT WANT THIS TO STAY THIS WAY - IN THE FUTURE WE WILL WANT TO INTERFACE WITH GOOGLE PLAY!
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
            else
            {
                StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
            }
        }
        else
        {
            StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
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
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnErrorLogin);
    }

    IEnumerator LoginInit()
    {
        doneWithStep = false;

        LoadupAllGameData();

        yield return new WaitUntil(() => doneWithStep == true);

        LoadGameVersion();

        yield return new WaitUntil(() => doneWithStep == true);

        doneWithStep = false;

        AutoVersionUpdater.instance.CheckMostRecentVersionWithServer();

        yield return new WaitUntil(() => doneWithStep == true);

        displayMessages.text = "Logged In!";

        doneWithStep = false;

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



        isLoggedIn = true;
        TimeReferenceDataScript.Start();

        UIManager.Instance.CheckDisplayCheatMenusAndObjects();

        StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(true));

        if (TutorialSaveData.Instance.hasFinishedIntro)
        {
            UIManager.Instance.DisplayDailyRewardsScreen();
        }
        else
        {
            StartCoroutine(UIManager.Instance.DisplayIntro());
        }

        UIManager.Instance.SetCanRepeatLevelsDisplay();

        yield return new WaitForSeconds(2);

        if (TutorialSaveData.Instance.hasFinishedIntro)
        {
            UIManager.Instance.PlayButton();
        }


        SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.RewardsManager, SystemsToSave.DewDrops, SystemsToSave.LoginData, SystemsToSave.CheatingSaveData });


        InvokeRepeating("UpdateAndSaveTimeSensitiveData", 1, 10);
    }

    void OnError(PlayFabError error)
    {
        displayMessages.text = error.ErrorMessage;
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
    }
    void OnErrorLogin(PlayFabError error)
    {
        displayMessages.text = error.ErrorMessage;
        Debug.LogError("Errrrrror!!! " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());

        StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
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

            PLD.positionText.text = (item.Position + 1).ToString();
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

        List<DailyRewardsPacks> dailyRewardPacks = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<List<DailyRewardsPacks>>(result.Data["dailyRewardPacks_New"]);


        //List<DailyRewardsPacks> dailyRewardPacks = PlayFab.PfEditor.Json.JsonWrapper.DeserializeObject<List<DailyRewardsPacks>>(result.Data["dailyRewardPacks"]);

        RewardsManager.Instance.UpdateRewardListServer(dailyRewardPacks);



        timeToWaitForDailyRewardSeconds = Convert.ToInt32(result.Data["TimeToWaitForDailySeconds_New"]);

        RewardsManager.Instance.timeLeftToGiveDailyLoot = timeToWaitForDailyRewardSeconds;

        doneWithStep = true;
    }

    [ContextMenu("Load ALL game data from server - STEP 1")]
    public void LoadupAllGameData()
    {
        // Player Data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
        //DewDropsManager.Instance.LoadDewDropsData();
    }

    public void LoadGameVersion()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedVersion, OnError);
    }

    void OnDataRecievedVersion(GetUserDataResult result)
    {
        // Auto Version Updater
        if (result.Data != null && result.Data.ContainsKey("Version Updater Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Version Updater Data"].Value, AutoVersionUpdater.instance);

            AutoVersionUpdater.instance.Init();
        }

        doneWithStep = true;
    }

    void OnDataRecieved(GetUserDataResult result)
    {
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

        // Hollow Manager Data
        if (result.Data != null && result.Data.ContainsKey("Hollow Manager Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Hollow Manager Data"].Value, HollowManagerSaveData.Instance);
        }

        // Cheating Save Data
        if (result.Data != null && result.Data.ContainsKey("Cheating Save Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Cheating Save Data"].Value, CheatingSaveData.instance);
        }
        // Bosses Save Data
        if (result.Data != null && result.Data.ContainsKey("Bosses Save Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Bosses Save Data"].Value, BossesSaveDataManager.instance);
        }

        doneWithStep = true;
    }

    [ContextMenu("Step 2")]
    public void  InitAllSystems()
    {
        PlayerManager.Instance.Init();
        DewDropsManager.Instance.Init();
        AnimalsManager.Instance.Init();
        CorruptedZonesSaveData.Instance.Init();
        TutorialSaveData.Instance.Init();
        ZoneManager.Instance.Init();
        RewardsManager.Instance.Init();
        HollowManagerSaveData.Instance.Init();

        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            zone.Init();
        }

        //SortMaster.Instance.ClearAllForgeScreens();

        //StartCoroutine(HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame));

        //MaterialsAndForgeManager.Instance.FillCorruptionDevices(GameManager.Instance.csvParser.allCorruptedDevicesInGame);

    }

    public void SaveGameData(SystemsToSave[] systemsToSave)
    {
        Debug.Log("Inside Save all game data script");
        string savedData = " ";

        UpdateAndSaveTimeSensitiveData();

        foreach (SystemsToSave STS in systemsToSave)
        {
            switch (STS)
            {
                case SystemsToSave.Player:
                    savedData = JsonUtility.ToJson(PlayerManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.Player, -1);
                    break;
                case SystemsToSave.DewDrops:
                    // Dew Drops Data
                    savedData = JsonUtility.ToJson(DewDropsManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.DewDrops, -1);
                    break;
                case SystemsToSave.animalManager:
                    // Animal Manager Data
                    savedData = JsonUtility.ToJson(AnimalsManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.animalManager, -1);
                    break;
                case SystemsToSave.corruptedZonesManager:
                    // Corrupted Zones Data
                    savedData = JsonUtility.ToJson(CorruptedZonesSaveData.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.corruptedZonesManager, -1);
                    break;
                case SystemsToSave.TutorialSaveData:
                    // Tutorial Save Data
                    savedData = JsonUtility.ToJson(TutorialSaveData.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.TutorialSaveData, -1);
                    break;
                case SystemsToSave.ZoneManager:
                    // Zone Manager Data
                    savedData = JsonUtility.ToJson(ZoneManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.ZoneManager, -1);
                    break;
                case SystemsToSave.ZoneX:
                    // Zone X Data
                    foreach (int zoneindex in ZoneManager.Instance.unlockedZoneID)
                    {
                        Zone zone = ZoneManagerHelpData.Instance.listOfAllZones[zoneindex];

                        savedData = JsonUtility.ToJson(zone);
                        SendDataToBeSavedJson(savedData, SystemsToSave.ZoneX, zone.id);
                    }
                    break;
                case SystemsToSave.RewardsManager:
                    // Rewards Manager Data
                    savedData = JsonUtility.ToJson(RewardsManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.RewardsManager, -1);
                    break;
                case SystemsToSave.LoginData:
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        filePath = Application.persistentDataPath + "/username.txt";
                    }
                    else
                    {
                        filePath = Application.dataPath + "/Save Files Folder/username.txt";
                    }
                    File.WriteAllText(filePath, playerName);
                    break;
                case SystemsToSave.HollowManager:
                    //Hollow manager
                    savedData = JsonUtility.ToJson(HollowManagerSaveData.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.HollowManager, -1);
                    break;
                case SystemsToSave.VersionUpdaterData:
                    //Auto Version Updater
                    savedData = JsonUtility.ToJson(AutoVersionUpdater.instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.VersionUpdaterData, -1);
                    break;
                case SystemsToSave.CheatingSaveData:
                    //Cheating Save Data
                    savedData = JsonUtility.ToJson(CheatingSaveData.instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.CheatingSaveData, -1);
                    break;
                case SystemsToSave.BossesSaveData:
                    //Bosses Save Data
                    savedData = JsonUtility.ToJson(BossesSaveDataManager.instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.BossesSaveData, -1);
                    break;
                case SystemsToSave.ALL:
                    SaveAllGameData();
                    break;
                default:
                    break;
            }
        }

        doneWithStep = true;
    }


    public void UpdateAndSaveTimeSensitiveData()
    {
        Debug.Log("Save time sensitive data");

        DateTime timeToSave = currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());
        RewardsManager.Instance.UpdateQuitTime(timeToSave);
        DewDropsManager.Instance.UpdateQuitTime(timeToSave);

        UpdateUserDataRequest request = null;
        string saveDataDrop = " ";
        string savedDataRewards = " ";

        saveDataDrop = JsonUtility.ToJson(DewDropsManager.Instance);

        savedDataRewards = JsonUtility.ToJson(RewardsManager.Instance);

        request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
                    {
                        { "Dew Drops Data", saveDataDrop },
                        { "Rewards Manager Data", savedDataRewards },
                    },
        };

        if (request != null)
        {
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }
        else
        {
            Debug.LogError("Something went horribly wrong!");
        }
    }


    [ContextMenu("Save ALL game data to server - STEP 3")]
    public void SaveAllGameData()
    {
        string savedData = " ";

        UpdateAndSaveTimeSensitiveData();

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

        //doneWithStep = false; //// set it back to false so last action will reset it

        // Rewards Manager Data
        savedData = JsonUtility.ToJson(RewardsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.RewardsManager, -1);

        //Hollow manager
        savedData = JsonUtility.ToJson(HollowManagerSaveData.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.HollowManager, -1);

        //Version Updater Data
        savedData = JsonUtility.ToJson(AutoVersionUpdater.instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.VersionUpdaterData, -1);

        //Cheating Save Data
        savedData = JsonUtility.ToJson(CheatingSaveData.instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.CheatingSaveData, -1);

        //Bosses Save Data
        savedData = JsonUtility.ToJson(BossesSaveDataManager.instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.BossesSaveData, -1);

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
                        { "Player Data", saveData },
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
            case SystemsToSave.HollowManager:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Hollow Manager Data", saveData }
                    }
                };
                break;
            case SystemsToSave.VersionUpdaterData:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Version Updater Data", saveData }
                    }
                };
                break;
            case SystemsToSave.CheatingSaveData:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Cheating Save Data", saveData }
                    }
                };
                break;
            case SystemsToSave.BossesSaveData:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Bosses Save Data", saveData }
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
                {"Rewards Manager Data", ""},
                {"Hollow Manager Data", ""}
                
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

        SetGameVersionSameAsServer();
        doneWithStep = false;
    }


    public void LogOut()
    {
        playerName = null;

        SaveGameData(new SystemsToSave[] { SystemsToSave.ALL });

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
        UIManager.Instance.nameOfPlayer.text = "Username: " + playerName;

        SetGameVersionSameAsServer();

        StartCoroutine(LoginInit());

        Debug.Log("Registered Successfully!");
    }

    void SetGameVersionSameAsServer()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onGetVersionFromServer, OnError);
    }

    void onGetVersionFromServer(GetTitleDataResult result)
    {
        if (result.Data == null)
        {
            Debug.Log("No Message in title data!");
        }
        else
        {
            AutoVersionUpdater.instance.mostRecentGameVersion = Convert.ToInt32(result.Data["Game Most Recent Version"]);
            SaveGameData(new SystemsToSave[] { SystemsToSave.VersionUpdaterData });

            doneWithStep = true;
        }
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
            UIManager.Instance.nameOfPlayer.text = "Username: " + playerName;
        }

        doneWithStep = true; //setup fir the loginInit function

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
        //Debug.Log(result.Time + " what?");
        //Debug.Log("Debug 1 " + currentTimeReference);

        doneWithStep = true;
    }

    // check if need this AND Focus
    private void OnApplicationPause(bool pause)
    {
        //Debug.Log("Pause is: " + pause);
       // Debug.Log("is Logged In is: " + isLoggedIn);

        if (pause && isLoggedIn)
        {
            //Debug.Log("On Application Pause");
            //GetServerCurrentTime();
            //yield return new WaitUntil(() => doneWithStep == true);

            //DateTime timeSpanSinceStart = Convert.ToDateTime();


            DateTime timeToSave = currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());



           // Debug.Log(timeToSave + "Pause time!");

            RewardsManager.Instance.UpdateQuitTime(timeToSave);
            DewDropsManager.Instance.UpdateQuitTime(timeToSave);

            SaveGameData(new SystemsToSave[] { SystemsToSave.ALL });

            //Debug.Log("Saved all data! - PAUSE");
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
            //Debug.Log("focus is: " + focus);
            //Debug.Log("is Logged In is: " + isLoggedIn);

            if (!focus && isLoggedIn)
            {
                //Debug.Log("On Application Focus");
                //GetServerCurrentTime();
                //yield return new WaitUntil(() => doneWithStep == true);

                DateTime timeToSave = currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());


                Debug.Log(timeToSave + "Focus time!");


                RewardsManager.Instance.UpdateQuitTime(timeToSave);
                DewDropsManager.Instance.UpdateQuitTime(timeToSave);

                SaveGameData(new SystemsToSave[] { SystemsToSave.ALL });

                //Debug.Log("Saved all data! - focus");
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
