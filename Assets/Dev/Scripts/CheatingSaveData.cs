using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatingSaveData : MonoBehaviour
{
    public static CheatingSaveData instance;

    public bool isAdmin;
    public bool canRepeatLevels;

    void Start()
    {
        instance = this;
    }

    public void ToggleCanRepeatLevels()
    {
        canRepeatLevels = !canRepeatLevels;

        UIManager.Instance.SetCanRepeatLevelsDisplay();


        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.CheatingSaveData });
    }
}
