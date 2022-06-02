using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameAnalyticsSDK;
using System.IO;
using System.Linq;

[Serializable]
public class NumAnimalTypedOnBoard
{
    public PieceSymbol animalSymbol;
    public int amount;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool LevelEnded;

    //public GameObject circleBoardPrefab;
    //public GameObject doubleCircleBoardPrefab;

    //public float secondsFromGameStart;

    public GameObject[] levelBGModels;
    public GameObject selectedLevelBG;
    public GameObject clipPrefab;

    public GameObject gameBoard;
    public GameObject gameClip;
    public GameObject endLevelAnimStatueShakeVFXPrefab;

    public Transform destroyOutOfLevel;

    public CursorController cursorControl;
    public ClipManager clipManager;
    public SliceManager sliceManager;
    public PowerUpManager powerupManager;
    //public LightingSettingsManager lightSettingsManager;

    public LevelScriptableObject currentLevel;
    public string timeStartLevel;

    public CSVParser csvParser;

    public int currentFilledCellCount;
    public int unsuccessfullConnectionCount;
    public int unsuccessfullSlicesCount;

    public bool levelStarted;
    public bool clickedPlayButton; /// why do I need this?
    public bool isSecondaryControls;
    public bool isDisableTutorials;

    public Vector3 inGameCamPos;
    public Vector3 inGameCamRot;

    public List<pieceDataStruct> copyOfArrayOfPiecesTutorial;
    public List<int> copyOfSpecificSliceSpotsTutorial;
    public List<PieceColor> copyOfSpecificSliceColorsTutorial;
    public List<PieceSymbol> copyOfSpecificSliceSymbolsTutorial;

    public NumAnimalTypedOnBoard[] numAnimalsOnBoard;

    public LevelScriptableObject[] allLevels;

    public Camera secondCam;

    private void Awake()
    {
        Instance = this;
    }

    //private void Update()
    //{
    //    if (PlayfabManager.isLoggedIn)
    //    {
    //        secondsFromGameStart += Time.deltaTime;
    //    }
    //}

    private void Start()
    {
        GameAnalytics.Initialize();
        isSecondaryControls = false;
        isDisableTutorials = false;
        clickedPlayButton = false;

        foreach (GameObject g in levelBGModels)
        {
            g.SetActive(false);
        }

        LevelEnded = false;
    }

    //public void CallStartLevel(bool isTutorial)
    //{
    //    if (isTutorial)
    //    {
    //        //StartCoroutine(StartTutorialLevel(true));
    //        StartTutorialLevel(true);
    //    }
    //    else
    //    {
    //        //StartCoroutine(StartLevel(true));
    //        StartLevel(true);
    //    }
    //}

    public void StartLevel(bool DoFade)
    {
        if (DoFade)
        {
            StartCoroutine(UIManager.Instance.FadeIntoLevel(false, false));

            //yield return new WaitForSeconds(UIManager.Instance.fadeIntoLevelSpeed + 0.1f);
        }
        else
        {
            ResetDataStartLevel(false);
        }

    }

    public void startBossLevel(bool DoFade)
    {
        if (DoFade)
        {
            StartCoroutine(UIManager.Instance.FadeIntoLevel(false, true));
        }
        else
        {
            ResetDataStartBossLevel();
        }
    }

    public void ResetDataStartLevel(bool isTutorial)
    {
        //if(isTutorial || currentLevel.isSpecificTutorial)
        //{
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
        //}


        UIManager.Instance.PrepareObjectForEndBoardAnim();
        LootManager.Instance.finishedGivingLoot = false;

        if (isTutorial)
        {
            LevelEnded = false;
            levelStarted = true;

            if (!isDisableTutorials)
            {
                powerupManager.ClearTutorialPowerups();// Make sure there are no leftoever tutorial powerups

                for (int i = 0; i < numAnimalsOnBoard.Length; i++)
                {
                    numAnimalsOnBoard[i].amount = 0;
                }

                UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
                UIManager.Instance.TurnOnGameplayUI();
                UIManager.Instance.dealButton.interactable = true;
                UIManager.Instance.ActivateGmaeplayCanvas();
                
                //Camera.main.orthographicSize = 12;
                //Camera.main.orthographic = false;
                //secondCam.orthographic = false;
                //Camera.main.fieldOfView = 60f;
                //secondCam.fieldOfView = 60f;

                Camera.main.transform.position = inGameCamPos;
                TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
                Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);

                //levelStarted = true;

                LightingSettingsManager.instance.ChooseLightSettings(ZoneManagerHelpData.Instance.currentZoneCheck.id);

                gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

                gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);
                sliceManager = gameBoard.GetComponent<SliceManager>();

                //UIManager.Instance.GetCommitButton(gameBoard); 
                clipManager.Init();
                sliceManager.Init();
                cursorControl.Init();


                ConnectionManager.Instance.GrabCellList(gameBoard.transform);
                ConnectionManager.Instance.SetLevelConnectionData(currentLevel.is12PieceRing);

                sliceManager.SpawnSlicesTutorial(currentLevel.slicesToSpawn.Length);

                PlayerManager.Instance.HandleItemCooldowns();

                powerupManager.InstantiateSpecialPowers();

                if (selectedLevelBG)
                {
                    //GameObject go = Instantiate(backGroundPrefab, destroyOutOfLevel);
                    //GameObject go = levelBGModels;
                    //go.SetActive(true);
                    selectedLevelBG.SetActive(true);

                    AnimalPrefabData data = InstantiateAnimals(selectedLevelBG);

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

                TutorialSequence.Instacne.activatedHeighlights.Clear();
                TutorialSequence.Instacne.activatedBoardParticles.Clear();


                PlayerManager.Instance.PopulatePowerUps();
                powerupManager.instnatiatedZonesCounter = 0;

                if (!TutorialSaveData.Instance.completedTutorialLevelId.Contains(currentLevel.levelNum))
                {
                    TutorialSequence.Instacne.StartTutorialLevelSequence();
                }


                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());
            }
            else
            {
                ResetDataStartLevel(false);
            }
        }
        else
        {
            LevelEnded = false;
            levelStarted = true;
            TutorialSequence.Instacne.DisableTutorialSequence(); //// Make sure tutorial is disabled
            powerupManager.ClearTutorialPowerups(); /// Make sure there are no leftover powerups

            for (int i = 0; i < numAnimalsOnBoard.Length; i++)
            {
                numAnimalsOnBoard[i].amount = 0;
            }

            UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
            UIManager.Instance.TurnOnGameplayUI();
            UIManager.Instance.dealButton.interactable = true;
            UIManager.Instance.ActivateGmaeplayCanvas();

            Camera.main.transform.position = inGameCamPos;
            TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
            Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);

            //levelStarted = true;

            LightingSettingsManager.instance.ChooseLightSettings(ZoneManagerHelpData.Instance.currentZoneCheck.id);
            gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

            gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);
            sliceManager = gameBoard.GetComponent<SliceManager>();

            //UIManager.Instance.GetCommitButton(gameBoard); 
            clipManager.Init();
            sliceManager.Init();
            cursorControl.Init();


            ConnectionManager.Instance.GrabCellList(gameBoard.transform);
            ConnectionManager.Instance.SetLevelConnectionData(currentLevel.is12PieceRing);


            sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);

            PlayerManager.Instance.HandleItemCooldowns();

            PlayerManager.Instance.PopulatePowerUps();

            powerupManager.InstantiateSpecialPowers();


            if (selectedLevelBG)
            {
                //GameObject go = Instantiate(backGroundPrefab, destroyOutOfLevel);
                //GameObject go = levelBGModels;
                //go.SetActive(true);
                selectedLevelBG.SetActive(true);

                AnimalPrefabData data = InstantiateAnimals(selectedLevelBG);

                if (data != null)
                {
                    AnimalsManager.Instance.currentLevelAnimal = data.animalType;
                }
                else
                {
                    Debug.Log("NO DATA - CHECK SCRIPTABLE OBJECTS FOR DATA - OR STATUE IS GRIND STATE/TREE STATUE");
                }
            }

            InstantiateStonePieces();

            powerupManager.instnatiatedZonesCounter = 0;

            if (currentLevel.isSpecificTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains((int)currentLevel.specificTutorialEnum))
            {
                if (currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen)
                {
                    //TutorialSequence.Instacne.DisplaySpecificTutorialSequence();
                    StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
                    TutorialSequence.Instacne.currentSpecificTutorial = currentLevel.specificTutorialEnum;
                }
            }

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());
        }

        //SoundManager.Instance.PlayAmbience(Sounds.LevelAmbience);
        StartCoroutine(SoundManager.Instance.FadeInAmbientMusic(ZoneManagerHelpData.Instance.musicPerZone[ZoneManagerHelpData.Instance.currentZoneCheck.id].levelAmbience));
    }
        
    public void ResetDataStartBossLevel()
    {
        if (currentLevel.ver1Boss)
        {
            BossBattleManager.instance.ResetDataBossVer1();
        }

        //LevelEnded = false;
        levelStarted = true;

        TutorialSequence.Instacne.DisableTutorialSequence(); //// Make sure tutorial is disabled
        powerupManager.ClearTutorialPowerups(); /// Make sure there are no leftover powerups

        for (int i = 0; i < numAnimalsOnBoard.Length; i++)
        {
            numAnimalsOnBoard[i].amount = 0;
        }

        UIManager.Instance.ChangeZoneName(BossBattleManager.instance.bossLevelSO.worldName, BossBattleManager.instance.bossLevelSO.levelIndexInZone);
        UIManager.Instance.TurnOnGameplayUI();
        UIManager.Instance.dealButton.interactable = true;
        UIManager.Instance.ActivateGmaeplayCanvas();
        UIManager.Instance.bossScreensParent.SetActive(false);

        Camera.main.transform.position = inGameCamPos;
        TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
        Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);

        LightingSettingsManager.instance.ChooseLightSettings(ZoneManagerHelpData.Instance.currentZoneCheck.id);
        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        gameClip.transform.position = new Vector3(gameClip.transform.position.x, 2.65f, gameClip.transform.position.z);

        gameBoard = Instantiate(BossBattleManager.instance.bossLevelSO.boardPrefab, destroyOutOfLevel);
        gameBoard.transform.position = new Vector3(gameBoard.transform.position.x, 1.45f, gameBoard.transform.position.z);
        sliceManager = gameBoard.GetComponent<SliceManager>();

        //UIManager.Instance.GetCommitButton(gameBoard); 
        clipManager.Init();
        sliceManager.Init();
        cursorControl.Init();


        ConnectionManager.Instance.GrabCellList(gameBoard.transform);
        ConnectionManager.Instance.SetLevelConnectionData(BossBattleManager.instance.bossLevelSO.is12PieceRing);


        sliceManager.SpawnSlices(BossBattleManager.instance.bossLevelSO.slicesToSpawn.Length);

        PlayerManager.Instance.HandleItemCooldowns();

        PlayerManager.Instance.PopulatePowerUps();

        powerupManager.InstantiateSpecialPowers();


        if (selectedLevelBG)
        {
            selectedLevelBG.SetActive(true);
        }

        InstantiateStonePieces();
        InstantiatePrePiecesOnBoard();

        powerupManager.instnatiatedZonesCounter = 0;

        StartCoroutine(SoundManager.Instance.FadeInAmbientMusic(Sounds.LevelAmbienceFast));

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());

        if (currentLevel.ver1Boss)
        {
            StartCoroutine(BossBattleManager.instance.delayStartBossActions());

            UIManager.Instance.bossV2TimerText.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(BossBattleManager.instance.delayStartBossActionsVer2());
            UIManager.Instance.bossV2TimerText.gameObject.SetActive(true);
        }
    }


    public void setCurrentLevelBG(int backgroundID)
    {
        if (ZoneManagerHelpData.Instance.currentZoneCheck.id == 0)
        {
            selectedLevelBG = levelBGModels[0];
        }
        else
        {
            selectedLevelBG = levelBGModels[backgroundID - 1]; // -1 since FOR NOW we don't have a bg for tutorial zone.. so we skip index 0
        }

        if(selectedLevelBG.transform.Find("color mask"))
        {
            selectedLevelBG.transform.Find("color mask").gameObject.SetActive(true); //// put this someplace better in the future
        }
    }

    public void StartTutorialLevel(bool DoFade)
    {
        if (DoFade)
        {
            StartCoroutine(UIManager.Instance.FadeIntoLevel(true, false));
            //yield return new WaitForSeconds(UIManager.Instance.fadeIntoLevelSpeed + 0.1f);
        }
        else
        {
            ResetDataStartLevel(true);
        }

    }

    AnimalPrefabData InstantiateAnimals(GameObject parent)
    {
        ZoneManagerHelpData ZMHD = ZoneManagerHelpData.Instance;

        if (currentLevel.specificAnimalForLevel)
        {
            GameObject go = currentLevel.specificAnimalForLevel;

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
        else
        {
            if (currentLevel.isAnimalLevel)
            {
                if (ZMHD.possibleAnimalsPerZone.Length != 0)
                {
                    if (ZMHD.possibleAnimalsPerZone[ZMHD.currentZoneCheck.id].animalsData.Length == 1)
                    {
                        AnimalsManager.Instance.statueToSwap = Instantiate(ZMHD.possibleAnimalsPerZone[ZMHD.currentZoneCheck.id].animalsData[0].animalPrefab, destroyOutOfLevel);
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
            else
            {
                GameObject go = AnimationManager.instance.endLevelTreePrefab;

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
    }

    public void SetZoneName(string zoneName)
    {

    }
    public void ChooseLevel(int levelNum/*, string zoneName*/)
    {
        //if (currentLevel)
        //{
        //    DestroyImmediate(currentLevel);
        //}

        currentLevel = Instantiate((LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/" + ZoneManagerHelpData.Instance.currentZoneName + "/Level " + levelNum));
    }
    public void ChooseLevelGrind(int levelNum)
    {
        currentLevel = Instantiate((LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/Grind Levels" + "/Level " + levelNum));
    }


    public void DestroyAllLevelChildern()
    {
        //Debug.Log("Destroying Level");

        levelStarted = false;
        selectedLevelBG.SetActive(false);

        foreach (Transform child in destroyOutOfLevel)
        {
            Destroy(child.gameObject);
        }

        foreach (PowerupProperties butt in powerupManager.powerupButtons)
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
        unsuccessfullSlicesCount = 0;
    }

    public bool CheckEndLevel(bool cheat)
    {
        if (cheat)
        {
            LevelEnded = true;

            if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex && !ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey)
            {
                LootManager.Instance.giveKey = true;
            }

            if (currentLevel.isTutorial)
            {
                TutorialSequence.Instacne.CheatTutorialClearNormal();
            }

            if (currentLevel.isSpecificTutorial)
            {
                TutorialSequence.Instacne.CheatTutorialClearSpecific();
            }

            if (ZoneManagerHelpData.Instance.currentZoneCheck.zoneGrindLevel)
            {
                if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.grindLevelIndex)
                {
                    ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind = true;
                }
            }

            foreach (GameObject go in TutorialSequence.Instacne.activatedHeighlights)
            {
                if (go)
                {
                    go.SetActive(false);
                }
            }
            TutorialSequence.Instacne.activatedHeighlights.Clear();

            AnimationManager.instance.StartEndLevelAnimSequence(true); ///// loot is given here

            return true;
        }
        else
        {
            //UIManager.Instance.dealButton.interactable = false;

            //UIManager.Instance.DisableCommitButton();
            if (currentFilledCellCount == currentLevel.cellsCountInLevel && unsuccessfullConnectionCount == 0 && unsuccessfullSlicesCount == 0)
            {
                if (ZoneManagerHelpData.Instance.currentZoneCheck)
                {
                    if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex && !ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey)
                    {
                        LootManager.Instance.giveKey = true;
                    }

                    if (ZoneManagerHelpData.Instance.currentZoneCheck.zoneGrindLevel)
                    {
                        if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.grindLevelIndex)
                        {
                            ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind = true;
                            //ZoneManagerHelpData.Instance.currentZoneCheck.zoneGrindLevel.GetComponent<Image>().sprite = Resources.Load<Sprite>(ZoneManagerHelpData.Instance.currentZoneCheck.levelDonePath);
                            //ZoneManagerHelpData.Instance.currentZoneCheck.zoneGrindLevel.GetComponent<Button>().interactable = true;
                            //ZoneManagerHelpData.Instance.currentZoneCheck.zoneGrindLevel.GetComponent<Renderer>().material.SetColor("_BaseColor", ZoneManagerHelpData.Instance.currentZoneCheck.levelFirstTimeColor);

                        }
                    }
                }

                Debug.Log("YOU WIN");

                LevelEnded = true;

                SoundManager.Instance.PlaySound(Sounds.SolvedRing);
                powerupManager.CheckTurnTempPowerToPermaPower();
                AnimationManager.instance.StartEndLevelAnimSequence(false); ///// loot is given here

                //PlayerManager.Instance.SavePlayerData();
                //PlayfabManager.instance.SaveAllGameData();


                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());


                TimeSpan deltaLevelTime = DateTime.Now - DateTime.Parse(timeStartLevel);

                string time = deltaLevelTime.ToString();
                time = string.Format("{0} - {1} - {2}", deltaLevelTime.Hours, deltaLevelTime.Minutes, deltaLevelTime.Seconds);

                GameAnalytics.NewDesignEvent("Finished level:" + currentLevel.worldName + ":" + "Level " + currentLevel.levelNum + ":" + "Time taken " + time);

                Debug.Log(time);

                timeStartLevel = "";

                //TutorialSequence.Instacne.CheckContinuedTutorials();
                return true;
            }
            else
            {
                UIManager.Instance.DisplayEndLevelMessage();

                //UIManager.Instance.LoseLevel();
                Debug.Log("You Lose");

                LevelEnded = false;

                //PlayerManager.Instance.SavePlayerData();
                //PlayfabManager.instance.SaveAllGameData();

                return false;
            }
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
        if (currentLevel.isGrindLevel)
        {
            StartCoroutine(LootManager.Instance.GiveLoot());

            UIManager.Instance.WinLevel();

            UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
            UIManager.Instance.restartGrindLevel.gameObject.SetActive(true);
        }
        else
        {

            if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone++;
                StartCoroutine(LootManager.Instance.GiveLoot());

            }

            if(currentLevel.levelNum > PlayerManager.Instance.highestLevelReached)
            {
                PlayerManager.Instance.UpdateMaxLevelReached(currentLevel);
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

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneX, SystemsToSave.ZoneManager, SystemsToSave.Player, SystemsToSave.animalManager });
    }

    public void RestartCurrentLevel()
    {
        string worldName = GameManager.Instance.currentLevel.worldName;
        string levelNum = GameManager.Instance.currentLevel.levelNum.ToString();

        GameAnalytics.NewDesignEvent("RestartLevel:Restarted:" + worldName + ":" + "Level " + levelNum);

        UIManager.isUsingUI = false;

        DestroyAllLevelChildern();
        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();
        CursorController.Instance.tutorialBadConnection = false;

        UIManager.Instance.youWinScreen.SetActive(false);
        UIManager.Instance.bGPanelDisableTouch.SetActive(false);
        UIManager.Instance.ResetTopAndBottomPos();
        UIManager.Instance.PrepareObjectForEndBoardAnim();

        LootManager.Instance.DestoryWinScreenDisplyedLoot();
        powerupManager.DestroySpecialPowersObjects();

        powerupManager.ResetData();

        if (selectedLevelBG.transform.Find("color mask"))
        {
            selectedLevelBG.transform.Find("color mask").gameObject.SetActive(true); //// put this someplace better in the future
        }

        if (!isDisableTutorials && (currentLevel.isTutorial || currentLevel.isSpecificTutorial))
        {
            TutorialSequence.Instacne.currentPhaseInSequenceLevels = 0;
            TutorialSequence.Instacne.duringSequence = false;

            TutorialSequence.Instacne.currentPhaseInSequenceSpecific = 0;

            foreach (GameObject go in TutorialSequence.Instacne.activatedHeighlights)
            {
                if (go)
                {
                    go.SetActive(false);
                }
            }

            TutorialSequence.Instacne.activatedHeighlights.Clear();
            TutorialSequence.Instacne.activatedBoardParticles.Clear();
            //TutorialSequence.Instacne.maskImage.gameObject.SetActive(true);

            powerupManager.ClearTutorialPowerups();
        }

        //if (currentLevel.isGrindLevel)
        //{
        //    ChooseLevelGrind(currentLevel.levelNum);
        //}
        //else
        //{
        //    ChooseLevel(currentLevel.levelNum);
        //}

        foreach (InGameSpecialPowerUp IGSP in powerupManager.specialPowerupsInGame)
        {
            IGSP.ResetValues();
        }

        if (!isDisableTutorials && currentLevel.isTutorial)
        {
            StartTutorialLevel(false);
        }
        else if(currentLevel.isBoss)
        {
            startBossLevel(false);
        }
        else
        {
            StartLevel(false);
        }
    }

    public void NextLevelFromWinScreen()
    {
        //StartCoroutine(UIManager.Instance.FadeIntoLevel(nextIsTutorial));
        //yield return new WaitForSeconds(UIManager.Instance.fadeIntoLevelSpeed + 0.1f);

        LootManager.Instance.rubiesToRecieveInLevel = 0;

        GameObject[] lootEffects = GameObject.FindGameObjectsWithTag("End Level Loot Effect");

        foreach (GameObject GO in lootEffects)
        {
            Destroy(GO.gameObject);
        }

        if (AnimalsManager.Instance.currentLevelLiveAnimal)
        {
            Destroy(AnimalsManager.Instance.currentLevelLiveAnimal.gameObject);
        }

        foreach (GameObject go in TutorialSequence.Instacne.activatedHeighlights)
        {
            if (go)
            {
                go.SetActive(false);
            }
        }

        powerupManager.DestroySpecialPowersObjects();
        TutorialSequence.Instacne.activatedHeighlights.Clear();

        DestroyAllLevelChildern();
        LootManager.Instance.DestoryWinScreenDisplyedLoot();

        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();

        if (currentLevel.levelIndexInZone + 1 == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex)
        {
            ZoneManager.Instance.CheckZoneAwardedKey(ZoneManagerHelpData.Instance.currentZoneCheck.id);
            ZoneManager.Instance.SetUnlockZone(ZoneManagerHelpData.Instance.currentZoneCheck.id + 1);
        }

        UIManager.Instance.bGPanelDisableTouch.SetActive(false);
        UIManager.Instance.youWinScreen.SetActive(false);
        UIManager.Instance.TurnOnGameplayUI();
        UIManager.Instance.ResetTopAndBottomPos();
        UIManager.isUsingUI = false;

        powerupManager.ResetData();

        foreach (GameObject go in UIManager.Instance.allTutorialScreens)
        {
            go.SetActive(false);
        }

        foreach (getChildrenHelpData GCHD in  powerupManager.instnatiateZones)
        {
            GCHD.referenceNumUsesText.gameObject.SetActive(false);
        }

        // ZoneManagerHelpData.Instance.listOfAllZones[ZoneManagerHelpData.Instance.currentZoneCheck.id].SaveZone();


        bool nextIsTutorial = CheckNextLevelIsTutorial(currentLevel.levelNum + 1);

        ChooseLevel(currentLevel.levelNum + 1/*, currentLevel.worldName*/);

        if (selectedLevelBG.transform.Find("color mask"))
        {
            selectedLevelBG.transform.Find("color mask").gameObject.SetActive(true); //// put this someplace better in the future
        }

        if (nextIsTutorial)
        {
            TutorialSequence.Instacne.currentPhaseInSequenceLevels = 0;

            //StartCoroutine(StartTutorialLevel(false));
            StartTutorialLevel(false);
        }
        else
        { 
            //StartCoroutine(StartLevel(false));
            StartLevel(false);
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
            if (currentLevel.is12PieceRing)
            {
                if (SPDS.cellIndex >= 0 && SPDS.cellIndex <= 11)
                {
                    ConnectionManager.Instance.cells[SPDS.cellIndex].AddStonePieceToBoard(SPDS);
                }
                else
                {
                    Debug.LogError("Cell index is either too high or too low - Min 0, Max 7");
                }
            }
            else
            {
                if (SPDS.cellIndex >= 0 && SPDS.cellIndex <= 7)
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

    void InstantiatePrePiecesOnBoard()
    {
        List<Cell> fullCells = new List<Cell>();

        List<int> possibleSlotsTemp = new List<int>();

        for (int i = 0; i < ConnectionManager.Instance.cells.Count; i++)
        {
            possibleSlotsTemp.Add(i);
        }

        if (currentLevel.isRandomPieces)
        {
            int randomPos = UnityEngine.Random.Range(0, ConnectionManager.Instance.cells.Count);

            fullCells.Add(ConnectionManager.Instance.cells[randomPos].transform.GetComponent<Cell>());

            possibleSlotsTemp = removeNearCellsFromPossible(possibleSlotsTemp, randomPos);

            if(currentLevel.specificPiecesIndex.Count() == 4)
            {
                for (int i = 1; i < currentLevel.specificPiecesIndex.Count(); i++)
                {

                    randomPos += 2;


                    if (randomPos >= ConnectionManager.Instance.cells.Count())
                    {
                        randomPos -= ConnectionManager.Instance.cells.Count();
                    }

                    fullCells.Add(ConnectionManager.Instance.cells[randomPos].transform.GetComponent<Cell>());
                }
            }
            else
            {
                if (currentLevel.is12PieceRing)
                {
                    if (currentLevel.specificPiecesIndex.Count() <= currentLevel.cellsCountInLevel / 2)
                    {
                        for (int i = 1; i < currentLevel.specificPiecesIndex.Count(); i++)
                        {

                            randomPos = UnityEngine.Random.Range(0, possibleSlotsTemp.Count());

                            fullCells.Add(ConnectionManager.Instance.cells[possibleSlotsTemp[randomPos]].transform.GetComponent<Cell>());

                            possibleSlotsTemp = removeNearCellsFromPossible(possibleSlotsTemp, possibleSlotsTemp[randomPos]);

                        }
                    }
                    else
                    {
                        Debug.LogError("Not possible to summon pieces with 1 space between them");
                        return;
                    }
                }
                else
                {
                    if (currentLevel.specificPiecesIndex.Count() <= currentLevel.cellsCountInLevel / 2)
                    {
                        for (int i = 1; i < currentLevel.specificPiecesIndex.Count(); i++)
                        {
                            randomPos = UnityEngine.Random.Range(0, possibleSlotsTemp.Count());

                            fullCells.Add(ConnectionManager.Instance.cells[possibleSlotsTemp[randomPos]].transform.GetComponent<Cell>());

                            possibleSlotsTemp = removeNearCellsFromPossible(possibleSlotsTemp, possibleSlotsTemp[randomPos]);

                        }
                    }
                    else
                    {
                        Debug.LogError("Not possible to summon pieces with 1 space between them");
                        return;
                    }

                }
            }

            for (int i = 0; i < fullCells.Count; i++)
            {
                fullCells[i].AddPieceRandom();
            }
        }
        else
        {
            for (int i = 0; i < currentLevel.specificPiecesIndex.Count(); i++)
            {
                fullCells.Add(ConnectionManager.Instance.cells[currentLevel.specificPiecesIndex[i]].transform.GetComponent<Cell>());
            }


            for (int i = 0; i < fullCells.Count; i++)
            {
                fullCells[i].AddPieceRandom();
            }
        }
    }

    private List<int> removeNearCellsFromPossible(List<int> possibleSlotsTemp, int index)
    {
        possibleSlotsTemp.Remove(index);


        if (index - 1 < 0)
        {
            int newIndex = ConnectionManager.Instance.cells.Count() - 1;
            possibleSlotsTemp.Remove(newIndex);
        }
        else
        {
            possibleSlotsTemp.Remove(index - 1);
        }

        if (index + 1 >= ConnectionManager.Instance.cells.Count())
        {
            int newIndex = 0;
            possibleSlotsTemp.Remove(newIndex);
        }
        else
        {
            possibleSlotsTemp.Remove(index + 1);

        }

        return possibleSlotsTemp;
    }

    public void ResetAllSaveData()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);

        foreach (string filePath in filePaths)
        {
            File.Delete(filePath);
        }

        SceneManager.LoadScene(0);
    }


    [ContextMenu("Cheat win")]
    public void WinLevelCheat()
    {
        CheckEndLevel(true);
    }

    public void DestroyChildrenOfTransform(Transform inObejct)
    {
        foreach (Transform child in inObejct)
        {
            Destroy(child.gameObject);
        }
    }

}
