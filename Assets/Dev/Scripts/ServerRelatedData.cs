using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRelatedData : MonoBehaviour
{
    public static ServerRelatedData instance;

    public bool isAdmin;
    public bool canRepeatLevels;
    public bool hasConnectedWithGooglePlay;

    [Header("Rate Game")]
    public bool canShowReviewMessage;
    public bool hasShowsInitialReviewMessage;
    public bool hasRatedOnGoogle;
    public int appReviewStarsAmountSelected;

    void Start()
    {
        instance = this;
    }

    public void ToggleCanRepeatLevels()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        canRepeatLevels = !canRepeatLevels;

        UIManager.Instance.SetCanRepeatLevelsDisplay();


        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });
    }
}
