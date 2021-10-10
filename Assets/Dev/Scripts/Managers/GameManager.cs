using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameAnalyticsSDK;

[Serializable]
public class NumAnimalTypedOnBoard
{
    public PieceSymbol animalSymbol;
    public int amount;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //public GameObject circleBoardPrefab;
    //public GameObject doubleCircleBoardPrefab;
    public GameObject levelBGModel;
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
    public bool isSecondaryControls;
    public bool isDisableTutorials;

    public Vector3 inGameCamPos;
    public Vector3 inGameCamRot;

    public List<pieceDataStruct> copyOfArrayOfPiecesTutorial;
    public List<int> copyOfSpecificSliceSpotsTutorial;
    public List<PieceColor> copyOfSpecificSliceColorsTutorial;
    public List<PieceSymbol> copyOfSpecificSliceSymbolsTutorial;

    public NumAnimalTypedOnBoard[] numAnimalsOnBoard;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameAnalytics.Initialize();
        isSecondaryControls = false;
        isDisableTutorials = false;
        levelBGModel.SetActive(false); /// temp
    }

    public void StartLevel()
    {
        TutorialSequence.Instacne.DisableTutorialSequence(); //// Make sure tutorial is disabled

        for (int i = 0; i < numAnimalsOnBoard.Length; i++)
        {
            numAnimalsOnBoard[i].amount = 0;
        }

        UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
        UIManager.Instance.TurnOnGameplayUI();

        //Camera.main.orthographicSize = 12;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 60f;
        Camera.main.transform.position = inGameCamPos;
        //Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);

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

        powerupManager.InstantiateSpecialPowers();


        if (levelBGModel)
        {
            //GameObject go = Instantiate(backGroundPrefab, destroyOutOfLevel);
            GameObject go = levelBGModel;
            go.SetActive(true);
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

        InstantiateStonePieces();

        powerupManager.instnatiatedZonesCounter = 0;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelNum);
    }

    public void StartTutorialLevel()
    {
        if (!isDisableTutorials)
        {
            for (int i = 0; i < numAnimalsOnBoard.Length; i++)
            {
                numAnimalsOnBoard[i].amount = 0;
            }

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
            copyOfSpecificSliceColorsTutorial = new List<PieceColor>();
            copyOfSpecificSliceSymbolsTutorial = new List<PieceSymbol>();

            copyOfSpecificSliceSpotsTutorial.AddRange(currentLevel.specificSliceSpots);
            copyOfSpecificSliceColorsTutorial.AddRange(currentLevel.specificSlicesColors);
            copyOfSpecificSliceSymbolsTutorial.AddRange(currentLevel.specificSlicesShapes);

            UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
            UIManager.Instance.TurnOnGameplayUI();

            //Camera.main.orthographicSize = 12;
            Camera.main.orthographic = false;
            Camera.main.fieldOfView = 60f;
            Camera.main.transform.position = inGameCamPos;
            //Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);

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

            powerupManager.InstantiateSpecialPowers();

            if (levelBGModel)
            {
                //GameObject go = Instantiate(backGroundPrefab, destroyOutOfLevel);
                GameObject go = levelBGModel;
                go.SetActive(true);

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

            InstantiateStonePieces();
            powerupManager.instnatiatedZonesCounter = 0;

            TutorialSequence.Instacne.activatedHeighlights.Clear();
            TutorialSequence.Instacne.activatedBoardParticles.Clear();

            TutorialSequence.Instacne.StartSequence(currentLevel.tutorialIndexForList);


            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelNum);
        }
        else
        {
            StartLevel();
        }
    }

    AnimalPrefabData InstantiateAnimals(GameObject parent)
    {
        ZoneManagerHelpData ZMHD = ZoneManagerHelpData.Instance;

        if (ZMHD.possibleAnimalsInZones.Length != 0)
        {
            if (ZMHD.possibleAnimalsInZones.Length == 1)
            {
                AnimalsManager.Instance.statueToSwap = Instantiate(ZMHD.possibleAnimalsInZones[ZMHD.currentZoneCheck.id].animalsData[0].animalPrefab, destroyOutOfLevel);
                return AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();
            }
            else
            {
                GameObject go = AnimalsManager.Instance.PopulateWeightSystemAnimals();

                if (go)
                {
                    AnimalsManager.Instance.statueToSwap = Instantiate(go, destroyOutOfLevel);
                    return AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();
                }
                else
                {
                    return null;
                }
            }
        }
        else
        {
            return null;
        }
    }

    public void SetZoneName(string zoneName)
    {

    }
    public void ChooseLevel(int levelNum/*, string zoneName*/)
    {
        if (currentLevel)
        {
            DestroyImmediate(currentLevel);
        }

        //currentLevel = (LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/Level " + levelNum);
        Debug.Log("Scriptable Objects/Levels/" + ZoneManagerHelpData.Instance.currentZoneName + "/Level " + levelNum);
        currentLevel = Instantiate((LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/" + ZoneManagerHelpData.Instance.currentZoneName + "/Level " + levelNum));
    }

    public void DestroyAllLevelChildern()
    {
        //Debug.Log("Destroying Level");

        gameStarted = false;
        levelBGModel.SetActive(false);

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

    public bool CheckEndLevel()
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

            Debug.Log("YOU WIN");

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, currentLevel.worldName, currentLevel.levelNum);

            PlayerManager.Instance.SavePlayerData();

            //TutorialSequence.Instacne.CheckContinuedTutorials();
            return true;
        }
        else
        {
            UIManager.Instance.DisplayEndLevelMessage();

            //UIManager.Instance.LoseLevel();
            Debug.Log("You Lose");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, currentLevel.worldName, currentLevel.levelNum);

            PlayerManager.Instance.SavePlayerData();

            return false;
        }
    }

    public void LoseLevelAction()
    {
        //LootManager.Instance.currentLevelLootToGive.Clear();
        LootManager.Instance.craftingMatsLootForLevel.Clear();
        LootManager.Instance.tempDataList.Clear();

    }
    public void WinAfterAnimation()
    {
        Debug.Log("IN HERE");
        if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
        {
            ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone++;
            LootManager.Instance.GiveLoot();
        }

        UIManager.Instance.WinLevel();

        if (currentLevel.levelIndexInZone != ZoneManagerHelpData.Instance.currentZoneCheck.lastLevelNum)
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
        CursorController.Instance.tutorialBadConnection = false;

        if (!isDisableTutorials && currentLevel.isTutorial)
        {
            TutorialSequence.Instacne.currentPhaseInSequence = 0;
            TutorialSequence.Instacne.duringSequence = false;
            TutorialSequence.Instacne.maskImage.gameObject.SetActive(true);
        }

        ChooseLevel(currentLevel.levelNum/*, currentLevel.worldName*/);

        foreach (InGameSpecialPowerUp IGSP in powerupManager.specialPowerupsInGame)
        {
            IGSP.ResetValues();
        }

        if (!isDisableTutorials && currentLevel.isTutorial )
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


        bool nextIsTutorial = CheckNextLevelIsTutorial(currentLevel.levelNum + 1);

        ChooseLevel(currentLevel.levelNum + 1/*, currentLevel.worldName*/);

        if (nextIsTutorial)
        {
            TutorialSequence.Instacne.currentPhaseInSequence = 0;

            StartTutorialLevel();
        }
        else
        {
            StartLevel();
        }
    }

    private bool CheckNextLevelIsTutorial(int levelNum)
    {
        LevelScriptableObject next = (LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/" + ZoneManagerHelpData.Instance.currentZoneName + "/Level " + levelNum);


        if (!isDisableTutorials && next.isTutorial)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void InstantiateStonePieces()
    {
        foreach (stonePieceDataStruct SPDS in currentLevel.stoneTiles)
        {
            if(SPDS.cellIndex >= 0 && SPDS.cellIndex <= 7)
            {
                ConnectionManager.Instance.cells[SPDS.cellIndex].AddStonePieceToBoard(SPDS);
            }
            else
            {
                Debug.LogError("Cell index is either too high or too low - Min 0, Max 7");
            }
        }
    }
}
