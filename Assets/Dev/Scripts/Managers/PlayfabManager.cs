using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.IO;
using System.Linq;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

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
    ServerRelatedData,
    BossesSaveData,
    AllZones,
    ALL
}

public class PlayfabManager : MonoBehaviour
{
    public static bool isLoggedIn = false;
    public static bool? successfullyDoneWithStep = null;
    //public static bool isSuccessfullConnection = true;

    public static PlayfabManager instance;

    public string playerName;
    public string playerPlayfabUsername;

    public TMP_Text testingContentTabServer;

    public int timeToWaitForDailyRewardSeconds = 0;


    public TMP_InputField userNameInput;
    public TMP_Text displayMessages;

    public TMP_InputField newDisplayNameInputField;

    public Transform leaderboardDisplayZone;
    public GameObject leaderboardPersonPrefab;
    string filePath;

    public DateTime currentTimeReference;

    public string currentTimeRef;

    string Saveseperator = "###";

    private void Update()
    {
        currentTimeRef = currentTimeReference.ToString();
    }
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
            filePath = Application.persistentDataPath + "/GeneralSaveData.txt";
        }
        else
        {
            filePath = Application.dataPath + "/Save Files Folder/GeneralSaveData.txt";
        }

        if (File.Exists(filePath))
        {
            ServerRelatedData.instance.hasConnectedWithGooglePlay = System.Convert.ToBoolean(GetGeneralSaveData(0));
            GooglePlayConnectManager.instance.connectedToGooglePlayText.text = ServerRelatedData.instance.hasConnectedWithGooglePlay.ToString();

            nameInFile = GetGeneralSaveData(1);

            //string test = GetGeneralSaveData(1);

            if (ServerRelatedData.instance.hasConnectedWithGooglePlay)
            {
                UIManager.Instance.googlePlayButton.SetActive(false);
                UIManager.Instance.switchAccountButton.SetActive(true);

                if (nameInFile != null && nameInFile != "")
                {
                    LoginAutomaticallyGooglePlay(nameInFile);
                }
                else
                {
                    StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
                }
            }
            else
            {
                UIManager.Instance.googlePlayButton.SetActive(true);
                UIManager.Instance.switchAccountButton.SetActive(false);

                if (nameInFile != null && nameInFile != "")
                {
                    LoginAutomatically(nameInFile);
                }
                else
                {
                    StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
                }
            }
        }
        else
        {
            UIManager.Instance.googlePlayButton.SetActive(true);
            UIManager.Instance.switchAccountButton.SetActive(false);

            StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
        }
    }


    public void LoginAutomaticallyGooglePlay(string nameInFile)
    {
        playerPlayfabUsername = nameInFile;

        GooglePlayConnectManager.instance.userNameDesc.text = playerPlayfabUsername;

        GooglePlayConnectManager.instance.SignInToGPGSHasAccount(SignInInteractivity.CanPromptAlways);
        //var request = new LoginWithGoogleAccountRequest
        //{
        //    TitleId = PlayFabSettings.TitleId,
        //    ServerAuthCode = serverAuthCode,

        //    Username = nameInFile,
        //    Password = "123456",
        //    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
        //    {
        //        GetPlayerProfile = true
        //    },
        //};
        //PlayFabClientAPI.LoginWithGoogleAccount(request, OnLoginSuccessNormal, OnErrorLogin);
    }
    public void LoginAutomatically(string nameInFile)
    {
        playerPlayfabUsername = nameInFile;

        GooglePlayConnectManager.instance.userNameDesc.text = playerPlayfabUsername;

        var request = new LoginWithPlayFabRequest
        {
            Username = nameInFile,
            Password = "123456",
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            },
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccessNormal, OnErrorLogin);
    }
    void OnLoginSuccessNormal(LoginResult result)
    {
        ClearSystemMessage();

        if (result.InfoResultPayload.PlayerProfile != null)
        {
            playerName = result.InfoResultPayload.PlayerProfile.DisplayName;
            UIManager.Instance.nameOfPlayer.text = "Username: " + playerName;

            GooglePlayConnectManager.instance.displayName.text = playerName;
        }

        SaveGameData(new SystemsToSave[] { SystemsToSave.LoginData });

        //doneWithStep = true; //setup fir the loginInit function

        AfterLoginSuccess();
    }
    void OnErrorLogin(PlayFabError error)
    {
        ClearSystemMessage();

        displayMessages.text = error.ErrorMessage;

        Debug.LogError(error.GenerateErrorReport());

        StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));

        //if (error.HttpStatus.Contains("Bad"))
        //{
        //    UIManager.Instance.TurnOnDisconnectedScreen();
        //}
    }


    public void AfterLoginSuccess()
    {
        GooglePlayConnectManager.instance.statusText.text = "Loading";
        GooglePlayConnectManager.instance.desc.text = "Loading";

        StartCoroutine(LoginInit());

        //CheckLoginWithGP();

    }
    public IEnumerator LoginInit()
    {
        UIManager.Instance.startAppLoadingScreen.SetActive(true);

        successfullyDoneWithStep = null;

        displayMessages.text = "Connecting to server";


        yield return StartCoroutine(LoadGameVersion());

        successfullyDoneWithStep = null;
        CheckActionConnectionError();

        yield return StartCoroutine(AutoVersionUpdater.instance.CheckMostRecentVersionWithServer());

        successfullyDoneWithStep = null;
        CheckActionConnectionError();

        yield return StartCoroutine(LoadupAllGameData());

        successfullyDoneWithStep = null;
        CheckActionConnectionError();

        //yield return new WaitForSeconds(3);

        displayMessages.text = "Logged In!";

        yield return StartCoroutine(GetServerCurrentTime());

        successfullyDoneWithStep = null;
        CheckActionConnectionError();

        yield return StartCoroutine(InitAllSystems()); // this does not connect to database!!
        Debug.LogError("Init all systems!!!");


        // from here on can do actions however we want since we loaded and initted all systems

        //Debug.Log("Debug 2 " + currentTimeReference);
        if (currentTimeReference != DateTime.MinValue)
        {
            Debug.LogError("Updated Time");
            RewardsManager.Instance.UpdateCurrentTime(currentTimeReference);
            DewDropsManager.Instance.UpdateCurrentTime(currentTimeReference);
        }
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(GetDailyRewardsData());

        successfullyDoneWithStep = null;
        CheckActionConnectionError();



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

            StartCoroutine(SoundManager.Instance.PlaySoundFadeIn(Sounds.IntroMusic));
        }

        UIManager.Instance.SetCanRepeatLevelsDisplay();

        //UIManager.Instance.FocusOnArea(ZoneManager.Instance.unlockedZoneID[ZoneManager.Instance.unlockedZoneID.Count - 1]);
        UIManager.Instance.FocusOnMaxLevelReached();


        if (TutorialSaveData.Instance.hasFinishedDen || TutorialSaveData.Instance.hasFinishedPotion)
        {
            UIManager.Instance.TurnOnRingersHutAndInventoryButtons();
        }

        if (TutorialSaveData.Instance.hasFinishedAnimalAlbum)
        {
            UIManager.Instance.TurnOnAnimalAlbumButtons();
        }

        //if (ZoneManager.Instance.hasStartedVinebloom)
        //{
        //    UIManager.Instance.TurnOnLeaderboardButtons();
        //}

        yield return new WaitForSeconds(2);

        Debug.LogError("Finished all Login Init sequence!!!!");

        if (TutorialSaveData.Instance.hasFinishedIntro)
        {
            UIManager.Instance.PlayButton();
        }

        UIManager.Instance.startAppLoadingScreen.SetActive(false);

        SaveGameData(new SystemsToSave[] { SystemsToSave.ALL});

        CancelInvoke("UpdateAndSaveTimeSensitiveData");

        yield return new WaitForSeconds(1);

        InvokeRepeating("UpdateAndSaveTimeSensitiveData", 1, 5);
    }

    private void CheckLoginWithGP()
    {
        if (ServerRelatedData.instance.hasConnectedWithGooglePlay)
        {
            GooglePlayConnectManager.instance.SignInToGPGSHasAccount(SignInInteractivity.CanPromptAlways);
        }
    }

    void OnError(PlayFabError error)
    {
        ClearSystemMessage();

        displayMessages.text = error.ErrorMessage;

        //isSuccessfullConnection = false;
        successfullyDoneWithStep = false;

        Debug.LogError(error.GenerateErrorReport());

        if (error.HttpStatus.Contains("Bad"))
        {
            UIManager.Instance.TurnOnDisconnectedScreen();
        }
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

    string findWorldAndLevelByScriptableObject(int levelNumForLeaderBoard)
    {
        //if (levelNumForLeaderBoard != 0)
        //{
        //    levelNumForLeaderBoard--; // cause we start checking a list/arry from 0
        //}

        int lastLevelLeaderboardNum = 0;

        foreach (AllZonesAndLevels AZAL in GameManager.Instance.allZonesAndLevels)
        {
            lastLevelLeaderboardNum += AZAL.levelsInZone.Length;
        }
            

        if (levelNumForLeaderBoard > lastLevelLeaderboardNum)
        {
            levelNumForLeaderBoard = lastLevelLeaderboardNum;
        }

        int worldNum = -1;
        int indexInZone = -2;
        bool found = false;

        foreach (AllZonesAndLevels AZAL in GameManager.Instance.allZonesAndLevels)
        {
            for (int i = 0; i < AZAL.levelsInZone.Length; i++)
            {
                if(AZAL.levelsInZone[i].numIndexForLeaderBoard == levelNumForLeaderBoard)
                {
                    worldNum = AZAL.zone.id;
                    indexInZone = i;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                break;
            }
        }

        indexInZone += 1;
        return (worldNum + " - " + indexInZone).ToString();
        //return "no";
    }


    [ContextMenu("Get Daily Rewards Data from Server")]
    IEnumerator GetDailyRewardsData()
    {
        //doneWithStep = false;
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), onDailyRewardsDataGet, OnDailyRewardsDataGetFail);
        yield return new WaitUntil(() => successfullyDoneWithStep != null);

        Debug.LogError("Got daily rewards data!!!");
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

        //RewardsManager.Instance.timeLeftToGiveDailyLoot = timeToWaitForDailyRewardSeconds;

        successfullyDoneWithStep = true;
    }

    void OnDailyRewardsDataGetFail(PlayFabError error)
    {
        GooglePlayConnectManager.instance.desc.text = "Fail! " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());
    }
    [ContextMenu("Load ALL game data from server - STEP 1")]
    public IEnumerator LoadupAllGameData()
    {
        // Player Data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
        yield return new WaitUntil(() => successfullyDoneWithStep != null);

        Debug.LogError("Loaded all game data!");
    }

    public IEnumerator LoadGameVersion()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedVersion, OnDataRecievedVersionFailed);
        yield return new WaitUntil(() => successfullyDoneWithStep != null);

        Debug.LogError("Loaded game version");
    }

    void OnDataRecievedVersion(GetUserDataResult result)
    {
        // Auto Version Updater
        if (result.Data != null && result.Data.ContainsKey("Version Updater Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Version Updater Data"].Value, AutoVersionUpdater.instance);

            AutoVersionUpdater.instance.Init();
        }


        successfullyDoneWithStep = true;
    }
    void OnDataRecievedVersionFailed(PlayFabError error)
    {
        GooglePlayConnectManager.instance.desc.text = "Fail! " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());

        successfullyDoneWithStep = true;
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
            if (result.Data != null && result.Data.ContainsKey("Zone Data " + zone.zoneName))
            {
                JsonUtility.FromJsonOverwrite(result.Data["Zone Data " + zone.zoneName].Value, zone);
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
        if (result.Data != null && result.Data.ContainsKey("Server Related Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Server Related Data"].Value, ServerRelatedData.instance);
        }
        // Bosses Save Data
        if (result.Data != null && result.Data.ContainsKey("Bosses Save Data"))
        {
            JsonUtility.FromJsonOverwrite(result.Data["Bosses Save Data"].Value, BossesSaveDataManager.instance);
        }

        successfullyDoneWithStep = true;
    }

    [ContextMenu("Step 2")]
    public IEnumerator InitAllSystems()
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

        yield return null;
        //SortMaster.Instance.ClearAllForgeScreens();

        //StartCoroutine(HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame));

        //MaterialsAndForgeManager.Instance.FillCorruptionDevices(GameManager.Instance.csvParser.allCorruptedDevicesInGame);

    }

    public void SaveGameData(SystemsToSave[] systemsToSave)
    {
        foreach (var item in systemsToSave)
        {
            //Debug.Log("Inside Save game. systems: " + item);
        }

        string savedData = " ";

        //UpdateAndSaveTimeSensitiveData();

        foreach (SystemsToSave STS in systemsToSave)
        {
            switch (STS)
            {
                case SystemsToSave.Player:
                    savedData = JsonUtility.ToJson(PlayerManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.Player, "");
                    break;
                case SystemsToSave.DewDrops:
                    // Dew Drops Data
                    savedData = JsonUtility.ToJson(DewDropsManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.DewDrops, "");
                    break;
                case SystemsToSave.animalManager:
                    // Animal Manager Data
                    savedData = JsonUtility.ToJson(AnimalsManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.animalManager, "");
                    break;
                case SystemsToSave.corruptedZonesManager:
                    // Corrupted Zones Data
                    savedData = JsonUtility.ToJson(CorruptedZonesSaveData.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.corruptedZonesManager, "");
                    break;
                case SystemsToSave.TutorialSaveData:
                    // Tutorial Save Data
                    savedData = JsonUtility.ToJson(TutorialSaveData.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.TutorialSaveData, "");
                    break;
                case SystemsToSave.ZoneManager:
                    // Zone Manager Data
                    savedData = JsonUtility.ToJson(ZoneManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.ZoneManager, "");
                    break;
                case SystemsToSave.ZoneX:
                    // Zone X Data
                    foreach (int zoneindex in ZoneManager.Instance.unlockedZoneID)
                    {
                        Zone zone = ZoneManagerHelpData.Instance.listOfAllZones[zoneindex];

                        savedData = JsonUtility.ToJson(zone);
                        SendDataToBeSavedJson(savedData, SystemsToSave.ZoneX, zone.zoneName);
                    }
                    break;
                case SystemsToSave.AllZones:
                    // All Zones Data
                    foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
                    {
                        savedData = JsonUtility.ToJson(zone);
                        SendDataToBeSavedJson(savedData, SystemsToSave.ZoneX, zone.zoneName);
                    }
                    break;
                case SystemsToSave.RewardsManager:
                    // Rewards Manager Data
                    savedData = JsonUtility.ToJson(RewardsManager.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.RewardsManager, "");
                    break;
                case SystemsToSave.LoginData:
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        filePath = Application.persistentDataPath + "/GeneralSaveData.txt";
                    }
                    else
                    {
                        filePath = Application.dataPath + "/Save Files Folder/GeneralSaveData.txt";
                    }
                    File.WriteAllText(filePath, SetGeneralSaveData());
                    break;
                case SystemsToSave.HollowManager:
                    //Hollow manager
                    savedData = JsonUtility.ToJson(HollowManagerSaveData.Instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.HollowManager, "");
                    break;
                case SystemsToSave.VersionUpdaterData:
                    //Auto Version Updater
                    savedData = JsonUtility.ToJson(AutoVersionUpdater.instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.VersionUpdaterData, "");
                    break;
                case SystemsToSave.ServerRelatedData:
                    //Cheating Save Data
                    GooglePlayConnectManager.instance.statusText.text = "Saved this!";

                    savedData = JsonUtility.ToJson(ServerRelatedData.instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.ServerRelatedData, "");
                    break;
                case SystemsToSave.BossesSaveData:
                    //Bosses Save Data
                    savedData = JsonUtility.ToJson(BossesSaveDataManager.instance);
                    SendDataToBeSavedJson(savedData, SystemsToSave.BossesSaveData, "");
                    break;
                case SystemsToSave.ALL:
                    SaveAllGameData();
                    break;
                default:
                    break;
            }
        }
    }


    public void UpdateAndSaveTimeSensitiveData()
    {
        //Debug.Log("Save time sensitive data");

        //DateTime timeToSave = currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());
        //RewardsManager.Instance.UpdateQuitTime(timeToSave);
        //DewDropsManager.Instance.UpdateQuitTime(timeToSave);
        StartCoroutine(GetServerCurrentTimeUpdated());

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
        GooglePlayConnectManager.instance.statusText.text = "at save all game!";

        string savedData = " ";

        //UpdateAndSaveTimeSensitiveData();

        // General Save Data
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Application.persistentDataPath + "/GeneralSaveData.txt";
        }
        else
        {
            filePath = Application.dataPath + "/Save Files Folder/GeneralSaveData.txt";
        }
        File.WriteAllText(filePath, SetGeneralSaveData());

        // Player Data
        savedData = JsonUtility.ToJson(PlayerManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.Player, "");

        // Dew Drops Data
        savedData = JsonUtility.ToJson(DewDropsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.DewDrops, "");

        // Animal Manager Data
        savedData = JsonUtility.ToJson(AnimalsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.animalManager, "");

        // Corrupted Zones Data
        savedData = JsonUtility.ToJson(CorruptedZonesSaveData.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.corruptedZonesManager, "");

        // Tutorial Save Data
        savedData = JsonUtility.ToJson(TutorialSaveData.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.TutorialSaveData, "");

        // Zone Manager Data
        savedData = JsonUtility.ToJson(ZoneManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.ZoneManager, "");

        // Zone X Data
        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            savedData = JsonUtility.ToJson(zone);
            SendDataToBeSavedJson(savedData, SystemsToSave.ZoneX, zone.zoneName);
        }

        //doneWithStep = false; //// set it back to false so last action will reset it

        // Rewards Manager Data
        savedData = JsonUtility.ToJson(RewardsManager.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.RewardsManager, "");

        //Hollow manager
        savedData = JsonUtility.ToJson(HollowManagerSaveData.Instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.HollowManager, "");

        //Version Updater Data
        savedData = JsonUtility.ToJson(AutoVersionUpdater.instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.VersionUpdaterData, "");

        //Cheating Save Data
        GooglePlayConnectManager.instance.statusText.text = "Saved this!";
        savedData = JsonUtility.ToJson(ServerRelatedData.instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.ServerRelatedData, "");

        //Bosses Save Data
        savedData = JsonUtility.ToJson(BossesSaveDataManager.instance);
        SendDataToBeSavedJson(savedData, SystemsToSave.BossesSaveData, "");

        GooglePlayConnectManager.instance.statusText.text = "Done.";

    }

    public void SendDataToBeSavedJson(string saveData, SystemsToSave system, string zoneName)
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

                        { "Zone Data " + zoneName, saveData }
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
            case SystemsToSave.ServerRelatedData:
                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Server Related Data", saveData }
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
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnDataSendFail);
        }
        else
        {
            Debug.LogError("Something went horribly wrong!");
        }
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        //AutoVersionUpdater.saveCounter++;
    }
    void OnDataSendFail(PlayFabError error)
    {
        GooglePlayConnectManager.instance.desc.text = "Fail! " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());
    }


    //public IEnumerator ResetAllDataAutoUpdater()
    //{
    //    yield return StartCoroutine(ResetActionAutoUpdater());
    //}

    //IEnumerator ResetActionAutoUpdater()
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedReset, OnError);
    //    yield return WaitForEndReset(AutoVersionUpdater.instance.numOfKeyValuePairsInServer);

    //    AutoVersionUpdater.doneWithSubStep = true;
    //}

    //void OnDataRecievedReset(GetUserDataResult result)
    //{
    //    foreach (string key in result.Data.Keys)
    //    {
    //        UpdateUserDataRequest request = null;

    //        request = new UpdateUserDataRequest
    //        {
    //            Data = new Dictionary<string, string>()
    //            {
    //                { key, null }
    //            }
    //        };

    //        if (request != null)
    //        {
    //            PlayFabClientAPI.UpdateUserData(request, OnDataSendResetUpdater, OnError);
    //        }
    //    }
    //}

    //void OnDataSendResetUpdater(UpdateUserDataResult result)
    //{
    //    AutoVersionUpdater.resetSystemCounter++;
    //}

    public IEnumerator WaitForEndReset(int amountSystemsToReset)
    {
        float counter = 0;

        while (AutoVersionUpdater.resetSystemCounter < amountSystemsToReset)
        {
            yield return new WaitForSeconds(0.1f);

            counter += 0.1f;

            if (counter >= 10)
            {
                break;
            }
        }

        if (AutoVersionUpdater.resetSystemCounter != amountSystemsToReset)
        {
            Debug.LogError($"what happened? Counter: {AutoVersionUpdater.resetSystemCounter} Length:{amountSystemsToReset}");
            yield break;
        }
    }

    [ContextMenu("Reset All Data")]
    public void ResetAllData()
    {
        StartCoroutine(ResetAction());
        //StartCoroutine(ResetAction());
    }
    IEnumerator ResetAction()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecievedReset, OnError);

        yield return new WaitForSeconds(2);

        //doneWithStep = false;
        //UpdateUserDataRequest request = null;

        //request = new UpdateUserDataRequest
        //{
        //    Data = new Dictionary<string, string>()
        //    {
        //        {"Player Data", "" },
        //        {"Dew Drops Data", "" },
        //        {"Animal Manager Data", "" },
        //        {"corrupted Zones Manager Data", "" },
        //        {"Tutorial Save Data", "" },
        //        {"Zone Manager Data", "" },
        //        {"Rewards Manager Data", ""},
        //        {"Bosses Save Data", ""},
        //        {"Hollow Manager Data", ""}
                
        //    }
        //};

        //if (request != null)
        //{
        //    PlayFabClientAPI.UpdateUserData(request, OnDataSendReset, OnError);
        //}

        ///// what to do with DoneWithStep here???
        
        //foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        //{
        //    request = new UpdateUserDataRequest
        //    {
        //        Data = new Dictionary<string, string>()
        //            {
        //                { "Zone Data" + zone.id, "" }
        //            }
        //    };

        //    if (request != null)
        //    {
        //        PlayFabClientAPI.UpdateUserData(request, OnDataSendReset, OnError);
        //    }
        //}

        SendLeaderboard(0);
        TimeReferenceDataScript.Reset();
        //yield return new WaitUntil(() => doneWithStep == true);

        SceneManager.LoadScene(0);

        SetGameVersionSameAsServer();
        //doneWithStep = false;
    }

    void OnDataRecievedReset(GetUserDataResult result)
    {
        foreach (string key in result.Data.Keys)
        {
            if (!key.Contains("Server") && !key.Contains("Version Updater Data"))
            {
                UpdateUserDataRequest request = null;

                request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>()
                {
                    { key, null }
                }
                };

                if (request != null)
                {
                    PlayFabClientAPI.UpdateUserData(request, OnDataSendResetSuccess, OnError);
                }
            }
        }
    }
    void OnDataSendResetSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Check here");
    }

    public void LogOut()
    {
        playerName = null;
        //playerPlayfabUsername = null;

        SaveGameData(new SystemsToSave[] { SystemsToSave.ALL });

        if (ServerRelatedData.instance.hasConnectedWithGooglePlay)
        {
            GooglePlayConnectManager.instance.SignOutButton();
        }
        StartCoroutine(logOutAction());
    }

    IEnumerator logOutAction()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
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

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFail);
    }

    void OnRegisterFail(PlayFabError error)
    {
        ClearSystemMessage();

        int textLength = userNameInput.text.Length;
        string[] parts = userNameInput.text.Split(' ');



        //isSuccessfullConnection = false;
        successfullyDoneWithStep = false;

        Debug.LogError(error.GenerateErrorReport());

        if(parts.Length > 1)
        {
            displayMessages.text = "Name cannot contain spaces!";
            return;
        }

        if (textLength < 3)
        {
            displayMessages.text = "Name must be between 3 and 20 characters!";
            return;
        }

    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        ClearSystemMessage();

        displayMessages.text = "Registered Successfully!";
        playerName = userNameInput.text;
        UIManager.Instance.nameOfPlayer.text = "Username: " + playerName;

        SetGameVersionSameAsServer();

        StartCoroutine(LoginInit());

        Debug.Log("Registered Successfully!");
    }

    public void SetGameVersionSameAsServer()
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

            //doneWithStep = true;
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
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccessNormal, OnLoginError);
    }

    void OnLoginError(PlayFabError error)
    {
        ClearSystemMessage();

        Debug.LogError(error.GenerateErrorReport());

        int textLength = userNameInput.text.Length;
        string[] parts = userNameInput.text.Split(' ');



        //isSuccessfullConnection = false;
        successfullyDoneWithStep = false;

        Debug.LogError(error.GenerateErrorReport());

        if (parts.Length > 1)
        {
            displayMessages.text = "Name cannot contain spaces!";
            return;
        }

        if (textLength < 3)
        {
            displayMessages.text = "Name must be between 3 and 20 characters!";
            return;
        }

    }


    public IEnumerator GetServerCurrentTime()
    {
        //doneWithStep = false;
        PlayFabClientAPI.GetTime(new GetTimeRequest(), OnGetTimeSuccess, OnGetTimeFail);
        yield return new WaitUntil(() => successfullyDoneWithStep != null);

        //Debug.LogError("Got server time");
    }

    void OnGetTimeSuccess(GetTimeResult result)
    {
        currentTimeReference = result.Time;
        //Debug.Log("Debug 1 " + currentTimeReference);

        successfullyDoneWithStep = true;
    }
    void OnGetTimeFail(PlayFabError error)
    {
        GooglePlayConnectManager.instance.desc.text = "Fail! " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());

        successfullyDoneWithStep = true;
    }

    public IEnumerator GetServerCurrentTimeUpdated()
    {
        //doneWithStep = false;
        PlayFabClientAPI.GetTime(new GetTimeRequest(), OnGetTimeUpdatedSuccess, OnGetTimeUpdatedFail);
        yield return new WaitUntil(() => successfullyDoneWithStep != null);

        //Debug.LogError("Got server time");
    }

    void OnGetTimeUpdatedSuccess(GetTimeResult result)
    {
        currentTimeReference = result.Time;
        //Debug.LogError(result.Time + " what?");

        RewardsManager.Instance.UpdateQuitTime(currentTimeReference);
        DewDropsManager.Instance.UpdateQuitTime(currentTimeReference);
        //Debug.Log("Debug 1 " + currentTimeReference);

        SaveGameData(new SystemsToSave[] { SystemsToSave.DewDrops });

        successfullyDoneWithStep = true;
    }
    void OnGetTimeUpdatedFail(PlayFabError error)
    {
        GooglePlayConnectManager.instance.desc.text = "Fail! " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());

        successfullyDoneWithStep = true;
    }

    public void CheckActionConnectionError()
    {
        if (successfullyDoneWithStep.HasValue && !successfullyDoneWithStep.Value)
        {
            StopCoroutine("LoginInit");

            //// stop the coroutine here
        }

    }
    public void PlayButtonAutoRegister()
    {
        ClearSystemMessage();

        /// check here if we have the connected with google play saved on local device - that is our only way to know if this device has 
        /// already connected to google play BEFORE we login to playfab....
        
        if (ServerRelatedData.instance.hasConnectedWithGooglePlay)/* if we know from the saved data that we have already connected to google play from this device*/
        {
            // login with google play.
            GooglePlayConnectManager.instance.SignInToGPGSHasAccount(SignInInteractivity.CanPromptAlways);
        }
        else
        {
            GetDeviceID(out string androidID, out string customID);

            string name = string.Empty;

            if (!string.IsNullOrEmpty(androidID))
            {
                name = androidID;
            }
            else if (!string.IsNullOrEmpty(customID))
            {
                name = customID;
            }
            else
            {
                Debug.LogError("PROBLEM HERE");
                return;
            }

            playerPlayfabUsername = name;
            GooglePlayConnectManager.instance.userNameDesc.text = playerPlayfabUsername;

            var request = new LoginWithPlayFabRequest
            {
                Username = playerPlayfabUsername,
                Password = "123456",
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                },
            };

            PlayFabClientAPI.LoginWithPlayFab(request, OnPressPlayPlayerExists, OnPressPlayPlayerExistsError);
        }
    }

    void GetDeviceID(out string androidID, out string customID)
    {
        androidID = string.Empty;
        customID = string.Empty;


#if UNITY_EDITOR
        Debug.Log("Unity Editor");

        customID = SystemInfo.deviceUniqueIdentifier;

        if (customID.Length > 20)
        {
            customID = customID.Substring(0, 20);
        }
        else
        {
            customID = customID.Substring(0, customID.Length);
        }

        GooglePlayConnectManager.instance.statusText.text = "Unity Editor";
#else

        GooglePlayConnectManager.instance.statusText.text = "NOT Unity Editor";

        androidID = string.Empty;
        customID = string.Empty;

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            androidID = secure.CallStatic<string>("getString", contentResolver, "android_id");

            if (androidID.Length > 20)
            {
                androidID = androidID.Substring(0, 20);
            }
            else
            {
                androidID = androidID.Substring(0, androidID.Length);
            }
        }
        else
        {
            customID = SystemInfo.deviceUniqueIdentifier;

            if (customID.Length > 20)
            {
                customID = customID.Substring(0, 20);
            }
            else
            {
                customID = customID.Substring(0, customID.Length);
            }

        }

#endif
    }

    void OnPressPlayPlayerExists(LoginResult result)
    {
        Debug.LogError("Found User");
        displayMessages.text = "Found User";

        ClearSystemMessage();

        var request = new GetAccountInfoRequest
        {
            Username = playerPlayfabUsername,
        };

        PlayFabClientAPI.GetAccountInfo(request, OnGetExsistingPlayerDataSuccess, OnGetExsistingPlayerDataError);


    }

    void OnGetExsistingPlayerDataSuccess(GetAccountInfoResult result)
    {
        playerName = result.AccountInfo.TitleInfo.DisplayName;
        UIManager.Instance.nameOfPlayer.text = "Username: " + playerName;
        GooglePlayConnectManager.instance.displayName.text = playerName;

        StartCoroutine(LoginInit());
    }
    void OnGetExsistingPlayerDataError(PlayFabError error)
    {
        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());
    }

    void OnPressPlayPlayerExistsError(PlayFabError error)
    {
        Debug.LogError("No such user");
        displayMessages.text = "Creating User";
        Debug.LogError(playerPlayfabUsername);

        var request = new RegisterPlayFabUserRequest
        {
            Username = playerPlayfabUsername,
            Password = "123456",
            RequireBothUsernameAndEmail = false,
            DisplayName = playerPlayfabUsername
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnPressPlayPlayerCreated, OnPressPlayPlayerCreatedFailed);
    }


    void OnPressPlayPlayerCreated(RegisterPlayFabUserResult result)
    {
        ClearSystemMessage();

        displayMessages.text = "Registered Successfully!";

        playerName = playerPlayfabUsername;

        UIManager.Instance.nameOfPlayer.text = "Username: " + playerName;

        GooglePlayConnectManager.instance.displayName.text = playerName;

        SetGameVersionSameAsServer();

        StartCoroutine(LoginInit());
    }

    void OnPressPlayPlayerCreatedFailed(PlayFabError error)
    {
        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());

    }

    public void ChangeDisplayName()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = newDisplayNameInputField.text,
        };

        playerName = newDisplayNameInputField.text;

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateNewDisplayNameSuccess, OnUpdateNewDisplayNameError);
    }

    void OnUpdateNewDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.LogError("SUCCESS!");

        UIManager.Instance.nameOfPlayer.text = "Username: " + newDisplayNameInputField.text;
    }
    void OnUpdateNewDisplayNameError(PlayFabError error)
    {
        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());
    }

    public void ClearSystemMessage()
    {
        displayMessages.text = " ";
    }

    private string SetGeneralSaveData()
    {
        string[] contents = new string[]
        {
            "" + ServerRelatedData.instance.hasConnectedWithGooglePlay,
            playerPlayfabUsername,       
        };

        string saveString = string.Join(Saveseperator, contents);

        return saveString;
    }
    private string GetGeneralSaveData(int index)
    {
        string savedString = File.ReadAllText(filePath);

        string[] contents = savedString.Split(new[] { Saveseperator }, System.StringSplitOptions.None);

        switch (index)
        {
            case 0:
                return contents[0];
            case 1:
                return contents[1];
            default:
                break;
        }

        return "WRONG ERROR!!";
    }
}
