using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class GooglePlayConnectManager : MonoBehaviour
{
    public static GooglePlayConnectManager instance;

    public TMP_Text statusText;
    public TMP_Text desc;

    public TMP_Text displayName;
    public TMP_Text userNameDesc;

    public TMP_Text connectedToGooglePlayText;

    string googlePlayerID;

    private void Start()
    {
        instance = this;

        ConfigureGPGS();
        //SignIntoGPGS(SignInInteractivity.CanPromptOnce, clientConfiguration);
    }


    void ConfigureGPGS()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public void SignInToGPGSHasAccount(SignInInteractivity interactivity)
    {
        PlayGamesPlatform.Instance.Authenticate(interactivity, (code) =>
        {
            if (code == SignInStatus.Success)
            {
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                statusText.text = serverAuthCode;

                var request = new LoginWithGoogleAccountRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = serverAuthCode,
                    CreateAccount = false
                };

                PlayFabClientAPI.LoginWithGoogleAccount(request, OnLoginSuccessGP, OnErrorLoginGP);

                //googlePlayerID = Social.localUser.id;

                //if (googlePlayerID.Length > 20)
                //{
                //    googlePlayerID = googlePlayerID.Substring(0, 20);
                //}
                //else
                //{
                //    googlePlayerID = googlePlayerID.Substring(0, googlePlayerID.Length);
                //}

                //PlayfabManager.instance.playerPlayfabUsername = googlePlayerID;

                //userNameDesc.text = googlePlayerID;
            }
            else
            {
                statusText.text = "fail";
                desc.text = "WHY FAIL: " + code;
                StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));
            }
        });

    }
    void OnLoginSuccessGP(LoginResult result)
    {
        statusText.text = "Success";
        desc.text = "connectd with google play";
        userNameDesc.text = PlayfabManager.instance.playerPlayfabUsername;

        var request = new GetAccountInfoRequest
        {
            Username = PlayfabManager.instance.playerPlayfabUsername,
        };

        PlayFabClientAPI.GetAccountInfo(request, OnGetExsistingPlayerDataSuccess, OnGetExsistingPlayerDataError);


        PlayfabManager.instance.ClearSystemMessage();
    }
    void OnErrorLoginGP(PlayFabError error)
    {
        PlayfabManager.instance.ClearSystemMessage();

        statusText.text = "FAILED";
        desc.text = error.GenerateErrorReport();

        Debug.LogError(error.GenerateErrorReport());

        StartCoroutine(UIManager.Instance.MoveAfterLoadingScreen(false));

        SignOutButton();
        //if (error.HttpStatus.Contains("Bad"))
        //{
        //    UIManager.Instance.TurnOnDisconnectedScreen();
        //}
    }

    public void SignIntoGPGS(SignInInteractivity interactivity)
    {
        PlayGamesPlatform.Instance.Authenticate(interactivity, (code) =>
        {
            statusText.text = "on it";

            if(code == SignInStatus.Success)
            {
                statusText.text = "Success";
                desc.text = "Hello " + Social.localUser.userName + " You have an ID of " + Social.localUser.id;

                //googlePlayerID = Social.localUser.id;

                //if (googlePlayerID.Length > 20)
                //{
                //    googlePlayerID = googlePlayerID.Substring(0, 20);
                //}
                //else
                //{
                //    googlePlayerID = googlePlayerID.Substring(0, googlePlayerID.Length);
                //}

                //PlayfabManager.instance.playerPlayfabUsername = googlePlayerID;
                //userNameDesc.text = googlePlayerID;

                //var request = new LoginWithPlayFabRequest
                //{
                //    Username = googlePlayerID,
                //    Password = "123456",
                //    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                //    {
                //        GetPlayerProfile = true
                //    },
                //};

                //PlayFabClientAPI.LoginWithPlayFab(request, LoginWithPlayFabeSuccess, LoginWithPlayFabFail);'

                var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                statusText.text = authCode;

                var request = new LinkGoogleAccountRequest
                {
                    ForceLink = true,
                    ServerAuthCode = authCode,
                };

                PlayFabClientAPI.LinkGoogleAccount(request, GoogleLinkSuccess, GoogleLinkFail);
            }
            else
            {
                statusText.text = "fail";
                desc.text = "WHY FAIL: " + code;
            }
        });
    }

    void GoogleLinkSuccess(LinkGoogleAccountResult result)
    {
        statusText.text = "Signed In";
        desc.text = PlayfabManager.instance.playerPlayfabUsername;

        PlayfabManager.instance.ClearSystemMessage();

        var request = new GetAccountInfoRequest
        {
            Username = PlayfabManager.instance.playerPlayfabUsername,
        };

        PlayFabClientAPI.GetAccountInfo(request, OnGetExsistingPlayerDataSuccess, OnGetExsistingPlayerDataError);
    }
    void GoogleLinkFail(PlayFabError error)
    {
        statusText.text = "Link failed!";

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());
        desc.text = "WHY FAIL: " + error.GenerateErrorReport();

        //var request = new RegisterPlayFabUserRequest
        //{
        //    Username = googlePlayerID,
        //    Password = "123456",
        //    RequireBothUsernameAndEmail = false,
        //    DisplayName = googlePlayerID
        //};

        //PlayFabClientAPI.RegisterPlayFabUser(request, OnGooglePlayCreatedCharSuccess, OnGooglePlayCreatedCharFail);
    }

    void OnGetExsistingPlayerDataSuccess(GetAccountInfoResult result)
    {
        statusText.text = "Successful!!!!";
        desc.text = "connectd with google play";

        ServerRelatedData.instance.hasConnectedWithGooglePlay = true;
        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData, SystemsToSave.LoginData });

        PlayfabManager.instance.playerName = result.AccountInfo.TitleInfo.DisplayName;
        UIManager.Instance.nameOfPlayer.text = "Username: " + PlayfabManager.instance.playerName;
        displayName.text = PlayfabManager.instance.playerName;

        StartCoroutine(DelayMoveToGooglePlayAccountLogin());
    }
    void OnGetExsistingPlayerDataError(PlayFabError error)
    {
        statusText.text = "There was a problem here!";
        desc.text = "WHY FAIL: " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());
    }

    IEnumerator DelayMoveToGooglePlayAccountLogin()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayfabManager.instance.LoginInit());
    }

    IEnumerator DelayMoveToGoogleAccountCreate()
    {
        if (TutorialSaveData.Instance.hasFinishedIntro)
        {
            yield return new WaitForSeconds(1);
            PlayfabManager.instance.SaveAllGameData();
        }

        PlayfabManager.instance.SetGameVersionSameAsServer();

        yield return new WaitForSeconds(2);
        StartCoroutine(PlayfabManager.instance.LoginInit());
    }

    public void SignInButton()
    {
        SignIntoGPGS(SignInInteractivity.CanPromptAlways);
    }
    public void SignOutButton()
    {
        //ServerRelatedData.instance.hasConnectedWithGooglePlay = false;
        //PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData, SystemsToSave.LoginData });

        PlayGamesPlatform.Instance.SignOut();
        statusText.text = "Sign out!";
        desc.text = "";
    }
}
