using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameAnalyticsSDK;
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

    public Vector3 inGameCamPos;

    public List<pieceDataStruct> copyOfArrayOfPiecesTutorial;
    public List<int> copyOfSpecificSliceSpotsTutorial;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameAnalytics.Initialize();
    }
    public void StartLevel()
    {
        UIManager.Instance.TurnOnGameplayUI();

        //Camera.main.orthographicSize = 12;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 60f;
        Camera.main.transform.position = inGameCamPos;

        gameStarted = true;

        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);

        //UIManager.Instance.GetCommitButton(gameBoard); 
        clipManager.Init();
        sliceManager.Init();
        cursorControl.Init();


        ConnectionManager.Instance.GrabCellList(gameBoard.transform);
        ConnectionManager.Instance.SetLevelConnectionData();

        sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);

        PlayerManager.Instance.HandleItemCooldowns();

        PlayerManager.Instance.PopulatePowerUps();

        if (backGroundPrefab)
        {
            GameObject go = Instantiate(backGroundPrefab, destroyOutOfLevel);

            AnimalPrefabData data = InstantiateAnimals(go);

            if (data != null)
            {
                AnimalsManager.Instance.currentLevelAnimal = data.animalType;
            }
            else
            {
                Debug.Log("BIG ANIMALS ERROR - NO DATA - CHECK SCRIPTABLE OBJECTS FOR DATA");
            }
        }


        powerupManager.instnatiatedZonesCounter = 0;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelNum);
    }

    public void StartTutorialLevel()
    {

        if (copyOfArrayOfPiecesTutorial == null)
        {
            copyOfArrayOfPiecesTutorial = new List<pieceDataStruct>();
        }
        else
        {
            copyOfArrayOfPiecesTutorial.Clear();
        }

        copyOfArrayOfPiecesTutorial.AddRange(currentLevel.arrayOfPieces);

        copyOfSpecificSliceSpotsTutorial = new List<int>();
        copyOfSpecificSliceSpotsTutorial.AddRange(currentLevel.specificSliceSpots);

        UIManager.Instance.TurnOnGameplayUI();

        //Camera.main.orthographicSize = 12;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 60f;
        Camera.main.transform.position = inGameCamPos;

        gameStarted = true;

        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);

        //UIManager.Instance.GetCommitButton(gameBoard); 
        clipManager.Init();
        sliceManager.Init();
        cursorControl.Init();


        ConnectionManager.Instance.GrabCellList(gameBoard.transform);
        ConnectionManager.Instance.SetLevelConnectionData();

        sliceManager.SpawnSlicesTutorial(currentLevel.slicesToSpawn.Length);

        PlayerManager.Instance.HandleItemCooldowns();

        PlayerManager.Instance.PopulatePowerUps();

        if (backGroundPrefab)
        {
            GameObject go = Instantiate(backGroundPrefab, destroyOutOfLevel);

            AnimalPrefabData data = InstantiateAnimals(go);

            if (data != null)
            {
                AnimalsManager.Instance.currentLevelAnimal = data.animalType;
            }
            else
            {
                Debug.Log("BIG ANIMALS ERROR - NO DATA - CHECK SCRIPTABLE OBJECTS FOR DATA");
            }
        }

        powerupManager.instnatiatedZonesCounter = 0;

        TutorialSequence.Instacne.StartSequence(currentLevel.tutorialIndexForList);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelNum);
    }

    AnimalPrefabData InstantiateAnimals(GameObject parent)
    {
        if (currentLevel.possibleAnimalsInLevel.Length != 0)
        {
            if (currentLevel.possibleAnimalsInLevel.Length == 1)
            {
                AnimalsManager.Instance.statueToSwap = Instantiate(currentLevel.possibleAnimalsInLevel[0].animalPrefab, parent.transform);
                return AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();
            }
            else
            {
                AnimalsManager.Instance.statueToSwap = Instantiate(currentLevel.possibleAnimalsInLevel[Random.Range(0, currentLevel.possibleAnimalsInLevel.Length)].animalPrefab, parent.transform);
                return AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();
            }
        }
        else
        {
            return null;
        }
    }
    public void ChooseLevel(int levelNum)
    {
        currentLevel = (LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/Level " + levelNum);
    }

    public void DestroyAllLevelChildern()
    {
        Debug.Log("Destroying Level");

        gameStarted = false;

        foreach (Transform child in destroyOutOfLevel)
        {
            Destroy(child.gameObject);
        }

        foreach (Button butt in powerupManager.powerupButtons)
        {
            Destroy(butt.gameObject);
        }

        GameObject[] lootEffects = GameObject.FindGameObjectsWithTag("End Level Loot Effect");

        foreach (GameObject GO in lootEffects)
        {
            Destroy(GO.gameObject);
        }


        powerupManager.powerupButtons.Clear();

        currentFilledCellCount = 0;
        unsuccessfullConnectionCount = 0;
    }

    public void CheckEndLevel()
    {
        //UIManager.Instance.DisableCommitButton();
        if (currentFilledCellCount == currentLevel.cellsCountInLevel && unsuccessfullConnectionCount == 0)
        {
            if (ZoneManagerHelpData.Instance.currentZoneCheck)
            {
                if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex && !ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey)
                {
                    LootManager.Instance.giveKey = true;
                }
            }

            StartCoroutine(AnimationManager.instance.StartEndLevelAnim());
            Debug.Log("YOU WIN");

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, currentLevel.worldName, currentLevel.levelNum);
        }
        else
        {
            UIManager.Instance.DisplayEndLevelMessage();

            //UIManager.Instance.LoseLevel();
            Debug.Log("You Lose");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, currentLevel.worldName, currentLevel.levelNum);
        }

        PlayerManager.Instance.SavePlayerData();
    }

    public void LoseLevelAction()
    {
        //LootManager.Instance.currentLevelLootToGive.Clear();
        LootManager.Instance.craftingMatsLootForLevel.Clear();
    }
    public void WinAfterAnimation()
    {
        if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
        {
            ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone++;
            LootManager.Instance.GiveLoot();
        }

        UIManager.Instance.WinLevel();

        if (currentLevel.levelNum != ZoneManagerHelpData.Instance.currentZoneCheck.lastLevelNum)
        {

            UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(true);
        }
        else
        {
            UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
        }

    }

    public void RestartCurrentLevel()
    {
        DestroyAllLevelChildern();
        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();
        TutorialSequence.Instacne.currentPhaseInSequence = 0;
        TutorialSequence.Instacne.duringSequence = false;

        if (currentLevel.isTutorial)
        {
            StartTutorialLevel();
        }
        else
        {
            StartLevel();
        }
    }


    public void NextLevelFromWinScreen()
    {
        if (GameManager.Instance.currentLevel.isTutorial)
        {
            TutorialSequence.Instacne.currentPhaseInSequence = 0;
        }

        LootManager.Instance.rubiesToRecieveInLevel = 0;

        GameObject[] lootEffects = GameObject.FindGameObjectsWithTag("End Level Loot Effect");

        foreach (GameObject GO in lootEffects)
        {
            Destroy(GO.gameObject);
        }

        DestroyAllLevelChildern();
        LootManager.Instance.DestoryWinScreenDisplyedLoot();

        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();

        if (currentLevel.levelIndexInZone + 1 == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex)
        {
            ZoneManager.Instance.CheckZoneAwardedKey(ZoneManagerHelpData.Instance.currentZoneCheck.id);
            ZoneManager.Instance.SetUnlockZone(ZoneManagerHelpData.Instance.currentZoneCheck.id + 1);
        }


        UIManager.Instance.youWinScreen.SetActive(false);
        UIManager.Instance.TurnOnGameplayUI();
        UIManager.isUsingUI = false;

        ZoneManagerHelpData.Instance.listOfAllZones[ZoneManagerHelpData.Instance.currentZoneCheck.id].SaveZone();

        ChooseLevel(currentLevel.levelNum + 1);
    }
}
