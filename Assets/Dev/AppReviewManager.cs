using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using GameAnalyticsSDK;

public class AppReviewManager : MonoBehaviour
{
    public static AppReviewManager instance;

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;

    public int[] levelsToShowReviewMessage;

    private void Start()
    {
        instance = this;
    }

    public void DontAskAgainClicked()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        ServerRelatedData.instance.hasRatedOnGoogle = !ServerRelatedData.instance.hasRatedOnGoogle;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });
    }
    public void RateButtonClicked()
    {
        UIManager.Instance.reviewUsPanel.SetActive(false);

        if (ServerRelatedData.instance.appReviewStarsAmountSelected == 5)
        {
            UIManager.Instance.thankyou5Stars.SetActive(true);
        }
        else
        {
            UIManager.Instance.thankyou4orLowerStars.SetActive(true);
            ServerRelatedData.instance.hasRatedOnGoogle = true;
            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });
        }

        GameAnalytics.NewDesignEvent("RatedGame:StarAmount" +ServerRelatedData.instance.appReviewStarsAmountSelected);

    }
    public void SureButtonClicked()
    {
        CallRequestReviews();
    }


    public void CheckShowReviewMessages()
    {
        for (int i = 0; i < levelsToShowReviewMessage.Length; i++)
        {
            if (PlayerManager.Instance.highestLevelReached == levelsToShowReviewMessage[i])
            {
                ServerRelatedData.instance.canShowReviewMessage = true;
                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });

                return;
            }
        }
    }

    public void ShowReviewMessage()
    {
        ServerRelatedData.instance.canShowReviewMessage = false ;

        if (!ServerRelatedData.instance.hasShowsInitialReviewMessage)
        {
            UIManager.Instance.reviewUsPanel.SetActive(true);
            ServerRelatedData.instance.hasShowsInitialReviewMessage = true;
        }
        else
        {
            UIManager.Instance.reviewUsPanelRepeatable.SetActive(true);
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });
    }

    public void CallRequestReviews()
    {
        StartCoroutine(RequestReviews());

        ServerRelatedData.instance.hasRatedOnGoogle = true;
        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });
    }
    IEnumerator RequestReviews()
    {
        //request Reviewinfo object from google play
        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.LogError("Big problem with review!");
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
        Debug.LogError(_playReviewInfo);

        //launch in app review
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);

        Debug.LogError(launchFlowOperation);

        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.LogError("Big problem with review! again!");
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.

        Debug.LogError("no problem - done!");
    }

}
