using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRelatedData : MonoBehaviour
{
    public static ServerRelatedData instance;

    public bool isAdmin;
    public bool canRepeatLevels;
    public bool hasConnectedWithGooglePlay;

    void Start()
    {
        instance = this;
    }

    public void ToggleCanRepeatLevels()
    {
        canRepeatLevels = !canRepeatLevels;

        UIManager.Instance.SetCanRepeatLevelsDisplay();


        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ServerRelatedData });
    }
}
