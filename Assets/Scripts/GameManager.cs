using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject circleBoardPrefab;
    //public GameObject doubleCircleBoardPrefab;
    public GameObject backGroundPrefab;
    public GameObject clipPrefab;

    public GameObject gameBoard;
    public GameObject gameClip;

    public Transform destroyOutOfLevel;

    public CursorController cursorControl;
    public ClipManager clipManager;
    public SliceManager sliceManager;
    public PowerUpManager powerupManager;

    public LevelScriptableObject currentLevel;

    public CSVParser csvParser;

    public int currentFilledCellCount;
    public int unsuccessfullConnectionCount;

    public bool gameStarted;

    public LevelScriptableObject[] levelArray;

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel()
    {
        //Camera.main.orthographicSize = 12;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 60f;

        gameStarted = true;

        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);

        UIManager.Instance.GetCommitButton(gameBoard); 
        clipManager.Init();
        sliceManager.Init();
        cursorControl.Init();

        sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);
        ConnectionManager.Instance.GrabCellList(gameBoard.transform);
        ConnectionManager.Instance.SetLevelConnectionData();

        PlayerManager.Instance.HandleItemCooldowns();

        PlayerManager.Instance.PopulatePowerUps();


        Instantiate(backGroundPrefab, destroyOutOfLevel);

        powerupManager.instnatiatedZonesCounter = 0;
    }

    public void ChooseLevel(int levelNum)
    {
        currentLevel = (LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/Level " + levelNum);


        StartLevel();
    }

    public void DestroyAllLevelChildern()
    {
        gameStarted = false;

        foreach (Transform child in destroyOutOfLevel)
        {
            Destroy(child.gameObject);
        }

        foreach (Button butt in powerupManager.powerupButtons)
        {
            Destroy(butt.gameObject);
        }
        powerupManager.powerupButtons.Clear();

        currentFilledCellCount = 0;
        unsuccessfullConnectionCount = 0;
    }

    public void CheckEndLevel()
    {

        if(currentFilledCellCount == currentLevel.cellsCountInLevel && unsuccessfullConnectionCount == 0)
        {
            Debug.Log("YOU WIN");
            LootManager.Instance.GiveLoot();
            UIManager.Instance.WinLevel();

            if (currentLevel.levelNum >= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && currentLevel.levelNum != ZoneManagerHelpData.Instance.currentZoneCheck.lastLevelNum)
            {
                ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone++;
            }

        }
        else
        {
            LootManager.Instance.currentLevelLootToGive.Clear();
            UIManager.Instance.LoseLevel();
            Debug.Log("You Lose");
        }

        PlayerManager.Instance.SavePlayerData();
    }


    public void RestartCurrentLevel()
    {
        DestroyAllLevelChildern();
        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();

        StartLevel();
    }

}
