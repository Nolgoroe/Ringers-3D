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

    string googlePlayerID;

    PlayGamesClientConfiguration clientConfiguration;
    private void Start()
    {
        instance = this;

        ConfigureGPGS();
        //SignIntoGPGS(SignInInteractivity.CanPromptOnce, clientConfiguration);
    }


    void ConfigureGPGS()
    {
        clientConfiguration = new PlayGamesClientConfiguration.Builder().Build();
    }


    void SignIntoGPGS(SignInInteractivity interactivity, PlayGamesClientConfiguration configuration)
    {
        configuration = clientConfiguration;
        PlayGamesPlatform.InitializeInstance(configuration);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(interactivity, (code) =>
        {
            statusText.text = "on it";

            if(code == SignInStatus.Success)
            {
                statusText.text = "Success";
                desc.text = "Hello " + Social.localUser.userName + " You have an ID of " + Social.localUser.id;

                googlePlayerID = Social.localUser.id;

                if (googlePlayerID.Length > 20)
                {
                    googlePlayerID = googlePlayerID.Substring(0, 20);
                }
                else
                {
                    googlePlayerID = googlePlayerID.Substring(0, googlePlayerID.Length);
                }
                
                var request = new LoginWithPlayFabRequest
                {
                    Username = googlePlayerID,
                    Password = "123456",
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true
                    },
                };

                PlayFabClientAPI.LoginWithPlayFab(request, LoginWithPlayFabeSuccess, LoginWithPlayFabFail);
            }
            else
            {
                statusText.text = "fail";
                desc.text = "WHY FAIL: " + code;
            }
        });
    }

    void LoginWithPlayFabeSuccess(LoginResult result)
    {
        statusText.text = "Signed In as " + googlePlayerID;

        if (result.InfoResultPayload.PlayerProfile != null)
        {
            PlayfabManager.instance.playerName = result.InfoResultPayload.PlayerProfile.DisplayName;
            UIManager.Instance.nameOfPlayer.text = "Username: " + PlayfabManager.instance.playerName;
        }

        StartCoroutine(DelayMoveToGooglePlayAccountLogin());
        //SAVE ALL GAME DATA TO SERVER AND MAKE SURE SAVING FROM NOW ON IS ON THIS NEW ACCOUNT
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
    void LoginWithPlayFabFail(PlayFabError error)
    {
        Debug.Log("Google user doesn't exsist");
        statusText.text = "Google user doesn't exsist";

        var request = new RegisterPlayFabUserRequest
        {
            Username = googlePlayerID,
            Password = "123456",
            RequireBothUsernameAndEmail = false,
            DisplayName = googlePlayerID
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnGooglePlayCreatedCharSuccess, OnGooglePlayCreatedCharFail);
    }

    void OnGooglePlayCreatedCharSuccess(RegisterPlayFabUserResult result)
    {
        statusText.text = "Created Player!";

        PlayfabManager.instance.playerName = googlePlayerID;
        UIManager.Instance.nameOfPlayer.text = "Username: " + PlayfabManager.instance.playerName;

        StartCoroutine(DelayMoveToGoogleAccountCreate());

    }

    void OnGooglePlayCreatedCharFail(PlayFabError error)
    {
        statusText.text = "Fail! " + error.GenerateErrorReport();

        Debug.LogError("FAILED");

        Debug.LogError(error.GenerateErrorReport());

    }

    public void SignInButton()
    {
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, clientConfiguration);
    }
    public void SignOutButton()
    {
        PlayGamesPlatform.Instance.SignOut();
        statusText.text = "Sign out!";
        desc.text = "";
    }

    //void Start()
    //{
    //    PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    //        .AddOauthScope("profile")
    //        .RequestServerAuthCode(false)
    //        .Build();
    //    PlayGamesPlatform.InitializeInstance(config);

    //    // recommended for debugging:
    //    PlayGamesPlatform.DebugLogEnabled = true;

    //    // Activate the Google Play Games platform
    //    PlayGamesPlatform.Activate();
    //    //SignInToGooglePlay();

    //    //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    //}
    //public void OnSignInButtonClicked()
    //{
    //    Social.localUser.Authenticate((bool success) => {

    //        if (success)
    //        {
    //            statusText.text = "Google Signed In";
    //            var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
    //            Debug.Log("Server Auth Code: " + serverAuthCode);

    //            var request = new LoginWithGoogleAccountRequest
    //            {
    //                TitleId = PlayFabSettings.TitleId,
    //                ServerAuthCode = serverAuthCode,
    //                CreateAccount = true
    //            };


    //            PlayFabClientAPI.LoginWithGoogleAccount(request, loginWithGoogleSuccess, loginWithGoogleFail);

    //        }
    //        else
    //        {
    //            statusText.text = "Google Failed to Authorize your login";
    //        }
    //    });

    //}



    //public void SignInToGooglePlay()
    //{
    //    //PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    //}
}
