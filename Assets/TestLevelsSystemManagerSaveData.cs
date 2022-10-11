using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelsSystemManagerSaveData : MonoBehaviour
{
    public static TestLevelsSystemManagerSaveData instance;
    public int CompletedCount;

    public bool canGetChest;

    private void Start()
    {
        instance = this;
    }

    public void AddToChestBar()
    {
        CompletedCount++;

        TestLevelsSystemManager.instance.UpdateBarValue();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TestLevelsSystemManagerSaveData });
    }

    public void ResetData()
    {
        CompletedCount = 0;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TestLevelsSystemManagerSaveData });
    }
}
