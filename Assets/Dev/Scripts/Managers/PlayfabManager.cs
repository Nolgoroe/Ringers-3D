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
    public enum SystemsToSave { Player, DewDrops, animalManager, corruptedZonesManager, TutorialSaveData, ZoneManager, ZoneX}

    public static PlayfabManager instance;

    public string playerName;

    public TMP_Text testingContentTabServer;

    public int multiplier = 0;


    public TMP_InputField userNameInput;
    public TMP_Text displayMessages;

    bool doneWithStep = false;

    public Transform leaderboardDisplayZone;
    public GameObject leaderboardPersonPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        displayMessages.text = "";
    }
    //private void Start()
    //{
    //    Login();
    //}




    IEnumerator LoginInit()
    {
        //var request = new LoginWithCustomIDRequest
        //{
        //    CustomId = userNameInput.text,
        //    CreateAccount = true
        //};

        //PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);

        displayMessages.text = "Logged In!";

        LoadupAllGameData();

        yield return new WaitUntil(() => doneWithStep == true);
        doneWithStep = false;
        InitAllSystems();

        SaveAllGameData();
    }

    //void OnSuccess(LoginResult result)
    //{
    //    Debug.Log("Success!!!");
    //}

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

        return (GameManager.Instance.allLevels[levelNum].worldNum + " - " + GameManager.Instance.allLevels[levelNum].levelIndexInZone).ToString();
    }



    [ContextMenu("Get Title Data Server")]
    void GerTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnTitleDataRecieved, OnError);
    }

    void OnTitleDataRecieved(GetTitleDataResult result)
    {
        if(result.Data == null)
        {
            Debug.Log("No Message in title data!");
        }

        testingContentTabServer.text = result.Data["Value To Check"];
        multiplier = Convert.ToInt16(result.Data["Multiplier"]);

        Debug.Log("Recieved all data!");
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
        Debug.Log("Recieved Character Data");

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
            JsonUtility.FromJsonOverwrite(result.Data["Zone Manager Data"].Value, TutorialSaveData.Instance);
        }

        // Zone X Data
        foreach (Zone zone in ZoneManagerHelpData.Instance.listOfAllZones)
        {
            if (result.Data != null && result.Data.ContainsKey("Zone Data" + zone.id))
            {
                JsonUtility.FromJsonOverwrite(result.Data["Zone Data" + zone.id].Value, zone);
            }
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

        UIManager.Instance.PlayButton();

    }

    [ContextMenu("Save ALL game data to server - STEP 3")]
    public void SaveAllGameData()
    {

        doneWithStep = false;
        string savedData = " ";

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
                    }
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
        Debug.Log("Updated Player Data on Server!");
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
                         { "Zone Manager Data", "" }
                    }
        };
        if (request != null)
        {
            PlayFabClientAPI.UpdateUserData(request, OnDataSendReset, OnError);
        }

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

        yield return new WaitUntil(() => doneWithStep == true);

        SceneManager.LoadScene(0);
        doneWithStep = false;

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
}
