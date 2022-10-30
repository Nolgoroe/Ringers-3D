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
[Serializable]
public class AllZonesAndLevels
{
    public Zone zone;
    public LevelScriptableObject[] levelsInZone;
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
    public DialogueScriptableObject currentDialogue;
    public ClusterScriptableObject currentCluster;
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

    public AllZonesAndLevels[] allZonesAndLevels;

    public Camera secondCam;

    public bool hasRestartedLevel;


    public int currentIndexInCluster = -1;

    [Header("Dialogue")]
    public int currentIndexInDialogue = -1;
    public int currentDialogueMultiplier = -1;
    public float currentDialogueHeightValue = -1;
    public bool hasFinishedShowingDialogue;
    public DialogueObjectRefrences latestEntry;

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


    public void StartLevel(bool DoFade, bool isRestart)
    {
        if (DoFade)
        {
            StartCoroutine(UIManager.Instance.FadeIntoLevel(false, false));

            //yield return new WaitForSeconds(UIManager.Instance.fadeIntoLevelSpeed + 0.1f);
        }
        else
        {
            StartCoroutine(ResetDataStartLevel(false, isRestart));
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
            StartCoroutine(ResetDataStartBossLevel());
        }
    }

    public IEnumerator ResetDataStartLevel(bool isTutorial, bool isRestart)
    {
        AnimationManager.instance.ResetEnterLevelAnimation();
        AnimalsManager.Instance.ResetAnimalManagerData();

        AnimationManager.instance.hasGivenChest = false;

        timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

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


        UIManager.Instance.PrepareObjectForEndBoardAnim();
        LootManager.Instance.finishedGivingLoot = false;
        LootManager.Instance.chestLootPacks = currentCluster.chestLootPacksCluster;

        UIManager.Instance.toMapButton.SetActive(true);
        UIManager.Instance.googlePlayButton.SetActive(false);
        UIManager.Instance.switchAccountButton.SetActive(false);

        if (isRestart)
        {
            ZoneManagerHelpData.Instance.ChangeZoneToBlurryZoneDisplay();
        }
        else
        {
            ZoneManagerHelpData.Instance.ChangeZoneToNormalZoneDisplay();
        }

        LevelEnded = false;
        levelStarted = true;

        UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
        UIManager.Instance.TurnOnGameplayUI();
        UIManager.Instance.dealButton.interactable = true;
        UIManager.Instance.ActivateGmaeplayCanvas();

        Camera.main.transform.position = inGameCamPos;
        TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
        Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);


        if (selectedLevelBG)
        {
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

            if (AnimalsManager.Instance.statueToSwap)
            {
                AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Set Rive " + currentIndexInCluster);
            }
        }

        if (isTutorial)
        {
            if (!isDisableTutorials)
            {
                powerupManager.ClearTutorialPowerups();// Make sure there are no leftoever tutorial powerups

                for (int i = 0; i < numAnimalsOnBoard.Length; i++)
                {
                    numAnimalsOnBoard[i].amount = 0;
                }

                //UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
                //UIManager.Instance.TurnOnGameplayUI();
                //UIManager.Instance.dealButton.interactable = true;
                //UIManager.Instance.ActivateGmaeplayCanvas();

                if (AnimationManager.instance.endLevelAnimationON)
                {
                    Debug.LogError("CHECK THIS PROBLEM");
                    AnimationManager.instance.endLevelAnimationON = false;
                }

                //Camera.main.transform.position = inGameCamPos;
                //TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
                //Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);


                LightingSettingsManager.instance.ChooseLightSettings(ZoneManagerHelpData.Instance.currentZoneCheck.id);

                gameClip = Instantiate(currentLevel.clipPrefab, destroyOutOfLevel);

                gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);
                sliceManager = gameBoard.GetComponent<SliceManager>();

                clipManager.Init();
                sliceManager.Init();
                cursorControl.Init();


                ConnectionManager.Instance.GrabCellList(gameBoard.transform);
                ConnectionManager.Instance.SetLevelConnectionData(currentLevel.is12PieceRing);

                sliceManager.SpawnSlicesTutorial(currentLevel.slicesToSpawn.Length);

                PlayerManager.Instance.HandleItemCooldowns();

                powerupManager.InstantiateSpecialPowers();

                //if (selectedLevelBG)
                //{
                //    selectedLevelBG.SetActive(true);

                //    AnimalPrefabData data = InstantiateAnimals(selectedLevelBG);

                //    if (data != null)
                //    {
                //        AnimalsManager.Instance.currentLevelAnimal = data.animalType;

                //    }
                //    else
                //    {
                //        Debug.Log("BIG ANIMALS ERROR - NO DATA - CHECK SCRIPTABLE OBJECTS FOR DATA");
                //    }

                //    if(AnimalsManager.Instance.statueToSwap)
                //    {
                //        AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Set Rive " + currentIndexInCluster);
                //    }

                //}

                InstantiateStonePieces();

                TutorialSequence.Instacne.activatedHeighlights.Clear();
                TutorialSequence.Instacne.activatedBoardParticles.Clear();


                PlayerManager.Instance.PopulatePowerUps();
                powerupManager.instnatiatedZonesCounter = 0;


                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());
            }
            else
            {
                StartCoroutine(ResetDataStartLevel(false, false));
            }
        }
        else
        {
            TutorialSequence.Instacne.DisableTutorialSequence(); //// Make sure tutorial is disabled
            powerupManager.ClearTutorialPowerups(); /// Make sure there are no leftover powerups

            for (int i = 0; i < numAnimalsOnBoard.Length; i++)
            {
                numAnimalsOnBoard[i].amount = 0;
            }

            //UIManager.Instance.ChangeZoneName(currentLevel.worldName, currentLevel.levelIndexInZone);
            //UIManager.Instance.TurnOnGameplayUI();
            //UIManager.Instance.dealButton.interactable = true;
            //UIManager.Instance.ActivateGmaeplayCanvas();

            //Camera.main.transform.position = inGameCamPos;
            //TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
            //Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);


            LightingSettingsManager.instance.ChooseLightSettings(ZoneManagerHelpData.Instance.currentZoneCheck.id);
            gameClip = Instantiate(currentLevel.clipPrefab, destroyOutOfLevel);

            gameBoard = Instantiate(currentLevel.boardPrefab, destroyOutOfLevel);
            sliceManager = gameBoard.GetComponent<SliceManager>();

            clipManager.Init();
            sliceManager.Init();
            cursorControl.Init();


            ConnectionManager.Instance.GrabCellList(gameBoard.transform);
            ConnectionManager.Instance.SetLevelConnectionData(currentLevel.is12PieceRing);


            sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);

            PlayerManager.Instance.HandleItemCooldowns();

            PlayerManager.Instance.PopulatePowerUps();

            powerupManager.InstantiateSpecialPowers();


            //if (selectedLevelBG)
            //{
            //    selectedLevelBG.SetActive(true);

            //    AnimalPrefabData data = InstantiateAnimals(selectedLevelBG);

            //    if (data != null)
            //    {
            //        AnimalsManager.Instance.currentLevelAnimal = data.animalType;

            //    }
            //    else
            //    {
            //        Debug.Log("NO DATA - CHECK SCRIPTABLE OBJECTS FOR DATA - OR STATUE IS GRIND STATE/TREE STATUE");
            //    }

            //    if (AnimalsManager.Instance.statueToSwap)
            //    {
            //        AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Set Rive " + currentIndexInCluster);
            //    }
            //}

            InstantiateStonePieces();

            powerupManager.instnatiatedZonesCounter = 0;


            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());
        }

        SoundManager.Instance.CancelLeantweensSound();
        SoundManager.Instance.CancelCoRoutinesSound();

        //if (currentLevel.isTestLevel)
        //{
        //    StartCoroutine(TestLevelsSystemManager.instance.InitTestLevel());
        //}
        //else
        //{
        //    UIManager.Instance.gameplayCanvasTop.SetActive(true);
        //    UIManager.Instance.gameplayCanvasTopTestLevels.SetActive(false);
        //}

        if (!SoundManager.Instance.normalAmbienceLevel.isPlaying)
        {
            Debug.Log("Change music");

            StartCoroutine(SoundManager.Instance.FadeInAmbientMusicLevel(ZoneManagerHelpData.Instance.musicPerZone[ZoneManagerHelpData.Instance.currentZoneCheck.id].levelAmbience));
        }
        else
        {
            if (!hasRestartedLevel)
            {
                Debug.Log("Just volume up");
                StartCoroutine(SoundManager.Instance.FadeInOnlyLevelVolume(ZoneManagerHelpData.Instance.musicPerZone[ZoneManagerHelpData.Instance.currentZoneCheck.id].levelAmbience));
            }
        }



        if (currentLevel.levelStartDialogueSO)
        {
            AnimationManager.instance.SetInLevelValuesimmediateForDialogue();

            currentDialogue = null;
            currentIndexInDialogue = 0;
            currentDialogueMultiplier = -1;
            currentDialogueHeightValue = -1;
            UIManager.Instance.dialogueScroller.content.localPosition = Vector3.zero;
            latestEntry = null;

            currentDialogue = currentLevel.levelStartDialogueSO;

            currentLevel.levelStartDialogueSO.InitDialogue();

            yield return new WaitUntil(() => hasFinishedShowingDialogue == true);
        }

        AnimationManager.instance.ResetEnterLevelAnimation();

        if (!isRestart)
        {
            StartCoroutine(TestLevelsSystemManager.instance.InitTestLevel());
        }



        if (!isRestart && currentLevel.showIntroLevelAnimation)
        {
            UIManager.Instance.isUsingUI = true; //used to disable pickup pieces
            UIManager.Instance.restartButton.interactable = false;
            UIManager.Instance.optionsButtonIngame.interactable = false;
            UIManager.Instance.cheatOptionsButtonIngame.interactable = false;
            UIManager.Instance.dealButton.interactable = false;
            powerupManager.PowerupButtonsActivation(false);

            StartCoroutine(AnimationManager.instance.PopulateRefrencesEnterLevelAnim(true));

            yield return new WaitForSeconds(2.5f);

            // these become false in the animation progress to prevent player actions
            // so we enable them after this time.
            UIManager.Instance.isUsingUI = false; //used to disable pickup pieces
            UIManager.Instance.restartButton.interactable = true;
            UIManager.Instance.optionsButtonIngame.interactable = true;
            UIManager.Instance.cheatOptionsButtonIngame.interactable = true;
            UIManager.Instance.dealButton.interactable = true;
            powerupManager.PowerupButtonsActivation(true);

            if (currentLevel.isTutorial && !TutorialSaveData.Instance.completedTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                TutorialSequence.Instacne.StartTutorialLevelSequence();
            }

            if (currentLevel.isSpecificTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                if (currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
                {
                    StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
                    TutorialSequence.Instacne.currentSpecificTutorial = currentLevel.specificTutorialEnum;
                }
            }
        }
        else
        {
            // these become false in the animation progress to prevent player actions
            // so we enable them after this time.
            UIManager.Instance.restartButton.interactable = true;
            UIManager.Instance.optionsButtonIngame.interactable = true;
            UIManager.Instance.cheatOptionsButtonIngame.interactable = true;

            StartCoroutine(AnimationManager.instance.PopulateRefrencesEnterLevelAnim(false));
            yield return new WaitForEndOfFrame();

            if (currentLevel.isTutorial && !TutorialSaveData.Instance.completedTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                TutorialSequence.Instacne.StartTutorialLevelSequence();
            }

            if (currentLevel.isSpecificTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                if (currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
                {
                    StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
                    TutorialSequence.Instacne.currentSpecificTutorial = currentLevel.specificTutorialEnum;
                }
            }
        }
    }

    public IEnumerator ResetDataStartBossLevel()
    {
        timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

        AnimationManager.instance.ResetEnterLevelAnimation();
        AnimalsManager.Instance.ResetAnimalManagerData();

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

        UIManager.Instance.toMapButton.SetActive(true);
        UIManager.Instance.googlePlayButton.SetActive(false);
        UIManager.Instance.switchAccountButton.SetActive(false);

        if (AnimationManager.instance.endLevelAnimationON)
        {
            Debug.LogError("CHECK THIS PROBLEM");
            AnimationManager.instance.endLevelAnimationON = false;
        }

        Camera.main.transform.position = inGameCamPos;
        TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, inGameCamPos.y, -0.05f);
        Camera.main.transform.rotation = Quaternion.Euler(inGameCamRot);

        LightingSettingsManager.instance.ChooseLightSettings(ZoneManagerHelpData.Instance.currentZoneCheck.id);

        ZoneManagerHelpData.Instance.ChangeZoneToNormalZoneDisplay();

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

        SoundManager.Instance.CancelLeantweensSound();
        SoundManager.Instance.CancelCoRoutinesSound();

        StartCoroutine(SoundManager.Instance.FadeInAmbientMusicLevel(Sounds.LevelAmbience));



        if (currentLevel.levelStartDialogueSO)
        {
            AnimationManager.instance.SetInLevelValuesimmediateForDialogue();

            currentDialogue = null;
            currentIndexInDialogue = 0;
            currentDialogueMultiplier = -1;
            currentDialogueHeightValue = -1;
            UIManager.Instance.dialogueScroller.content.localPosition = Vector3.zero;
            latestEntry = null;

            currentDialogue = currentLevel.levelStartDialogueSO;

            currentLevel.levelStartDialogueSO.InitDialogue();

            yield return new WaitUntil(() => hasFinishedShowingDialogue == true);
        }

        AnimationManager.instance.ResetEnterLevelAnimation();

        //if (!isRestart)
        //{
        //    StartCoroutine(TestLevelsSystemManager.instance.InitTestLevel());
        //}


        if (/*!isRestart &&*/ currentLevel.showIntroLevelAnimation)
        {
            UIManager.Instance.isUsingUI = true; //used to disable pickup pieces
            UIManager.Instance.restartButton.interactable = false;
            UIManager.Instance.optionsButtonIngame.interactable = false;
            UIManager.Instance.cheatOptionsButtonIngame.interactable = false;
            UIManager.Instance.dealButton.interactable = false;
            powerupManager.PowerupButtonsActivation(false);

            StartCoroutine(AnimationManager.instance.PopulateRefrencesEnterLevelAnim(true));

            yield return new WaitForSeconds(2.5f);

            // these become false in the animation progress to prevent player actions
            // so we enable them after this time.
            UIManager.Instance.isUsingUI = false; //used to disable pickup pieces
            UIManager.Instance.restartButton.interactable = true;
            UIManager.Instance.optionsButtonIngame.interactable = true;
            UIManager.Instance.cheatOptionsButtonIngame.interactable = true;
            UIManager.Instance.dealButton.interactable = true;
            powerupManager.PowerupButtonsActivation(true);

            if (currentLevel.isTutorial && !TutorialSaveData.Instance.completedTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                TutorialSequence.Instacne.StartTutorialLevelSequence();
            }

            if (currentLevel.isSpecificTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                if (currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
                {
                    StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
                    TutorialSequence.Instacne.currentSpecificTutorial = currentLevel.specificTutorialEnum;
                }
            }
        }
        else
        {
            // these become false in the animation progress to prevent player actions
            // so we enable them after this time.
            UIManager.Instance.restartButton.interactable = true;
            UIManager.Instance.optionsButtonIngame.interactable = true;
            UIManager.Instance.cheatOptionsButtonIngame.interactable = true;

            StartCoroutine(AnimationManager.instance.PopulateRefrencesEnterLevelAnim(false));
            yield return new WaitForEndOfFrame();

            if (currentLevel.isTutorial && !TutorialSaveData.Instance.completedTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                TutorialSequence.Instacne.StartTutorialLevelSequence();
            }

            if (currentLevel.isSpecificTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
            {
                if (currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
                {
                    StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
                    TutorialSequence.Instacne.currentSpecificTutorial = currentLevel.specificTutorialEnum;
                }
            }
        }


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
            selectedLevelBG = levelBGModels[backgroundID];
        }

        if(selectedLevelBG.transform.Find("RingMask"))
        {
            selectedLevelBG.transform.Find("RingMask").gameObject.SetActive(true); //// put this someplace better in the future
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
            StartCoroutine(ResetDataStartLevel(true, false));
            //ResetDataStartLevel(true, false);
        }

    }

    AnimalPrefabData InstantiateAnimals(GameObject parent)
    {
        ZoneManagerHelpData ZMHD = ZoneManagerHelpData.Instance;

        if (currentCluster.specificClsuterStatue)
        {
            GameObject go = currentCluster.specificClsuterStatue;

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
                GameObject go = AnimalManagerDataHelper.instance.treesPerZone[ZoneManagerHelpData.Instance.currentZoneCheck.id].treePrefab;

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

    public void ChooseLevel(int levelNum)
    {
        currentLevel = Instantiate((LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/" + ZoneManagerHelpData.Instance.currentZoneName + "/Level " + levelNum));
    }
    public void ChooseLevelGrind(int levelNum)
    {
        currentLevel = Instantiate((LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/" + ZoneManagerHelpData.Instance.currentZoneName + "/Grind Level " + levelNum));
    }


    public void DestroyAllLevelChildern()
    {
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
                // temp keep
                //TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add(currentLevel.numIndexForLeaderBoard);

                TutorialSequence.Instacne.CheatTutorialClearSpecific();
            }

            foreach (GameObject go in ZoneManagerHelpData.Instance.zoneGrindLevelPerZone)
            {
                if (go.GetComponent<Interactable3D>())
                {
                    Interactable3D interactable = go.GetComponent<Interactable3D>();

                    if(ZoneManagerHelpData.Instance.currentZoneCheck.id == interactable.connectedLevelScriptableObject.worldNum)
                    {
                        if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.grindLevelIndex)
                        {
                            ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind = true;
                        }
                    }
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
            if (currentFilledCellCount == currentLevel.cellsCountInLevel && unsuccessfullConnectionCount == 0 && unsuccessfullSlicesCount == 0)
            {
                if (ZoneManagerHelpData.Instance.currentZoneCheck)
                {
                    if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex && !ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey)
                    {
                        LootManager.Instance.giveKey = true;
                    }

                    foreach (GameObject go in ZoneManagerHelpData.Instance.zoneGrindLevelPerZone)
                    {
                        if (go.GetComponent<Interactable3D>())
                        {
                            Interactable3D interactable = go.GetComponent<Interactable3D>();

                            if (ZoneManagerHelpData.Instance.currentZoneCheck.id == interactable.connectedLevelScriptableObject.worldNum)
                            {
                                if (currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.grindLevelIndex)
                                {
                                    ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind = true;
                                }
                            }
                        }
                    }
                }

                if (currentLevel.isTutorial)
                {
                    if (!TutorialSaveData.Instance.completedTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
                    {
                        TutorialSaveData.Instance.completedTutorialLevelId.Add(currentLevel.numIndexForLeaderBoard);
                        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });
                    }
                }

                if (currentLevel.isSpecificTutorial && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
                {
                    if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(currentLevel.numIndexForLeaderBoard))
                    {
                        TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add(currentLevel.numIndexForLeaderBoard);
                    }

                    PlayerManager.Instance.CheckTransformTempPowersToActualPowers();
                    PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });
                }

                Debug.Log("YOU WIN");

                LevelEnded = true;

                powerupManager.CheckTurnTempPowerToPermaPower();
                AnimationManager.instance.StartEndLevelAnimSequence(false); ///// loot is given here

                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, currentLevel.worldName, currentLevel.levelIndexInZone.ToString());


                TimeSpan deltaLevelTime = DateTime.Now - DateTime.Parse(timeStartLevel);

                string time = deltaLevelTime.ToString();
                time = string.Format("{0} - {1} - {2}", deltaLevelTime.Hours, deltaLevelTime.Minutes, deltaLevelTime.Seconds);

                GameAnalytics.NewDesignEvent("Finished level:" + currentLevel.worldName + ":" + "Level " + currentLevel.levelNum + ":" + "Time taken " + time);

                Debug.Log(time);

                timeStartLevel = "";

                return true;
            }
            else
            {
                UIManager.Instance.DisplayEndLevelMessage();

                Debug.Log("You Lose");

                LevelEnded = false;

                return false;
            }
        }
    }

    public void LoseLevelAction()
    {
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

            if(currentLevel.numIndexForLeaderBoard > PlayerManager.Instance.highestLevelReached)
            {
                PlayerManager.Instance.UpdateMaxLevelReached(currentLevel);
            }

            UIManager.Instance.WinLevel();

            if (currentLevel.levelIndexInZone != ZoneManagerHelpData.Instance.currentZoneCheck.lastLevelNum)
            {
                //UIManager.Instance.nextLevelFromWinScreen.interactable = true;
                //UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(true);

            }
            else
            {
                UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
            }
        }

        if (!ServerRelatedData.instance.hasRatedOnGoogle)
        {
            AppReviewManager.instance.CheckShowReviewMessages();
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneX, SystemsToSave.ZoneManager, SystemsToSave.Player, SystemsToSave.animalManager });
    }

    public void RestartCurrentLevel()
    {
        hasRestartedLevel = true;

        string worldName = currentLevel.worldName;
        string levelNum = currentLevel.levelNum.ToString();

        GameAnalytics.NewDesignEvent("RestartLevel:Restarted:" + worldName + ":" + "Level " + levelNum);

        UIManager.Instance.isUsingUI = false;

        DestroyAllLevelChildern();
        LootManager.Instance.DestroyAllChestLootData();
        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();
        ConnectionManager.Instance.tempSymbolPiecesStoneFound.Clear();
        ConnectionManager.Instance.amountStonePiecesInstantiated = 0;
        ConnectionManager.Instance.ResetAllLastPieceAlgoritmData();

        CursorController.Instance.tutorialBadConnection = false;

        UIManager.Instance.youWinScreen.SetActive(false);
        UIManager.Instance.bGPanelDisableTouch.SetActive(false);
        UIManager.Instance.ResetTopAndBottomPos();
        UIManager.Instance.PrepareObjectForEndBoardAnim();

        LootManager.Instance.DestoryWinScreenDisplyedLoot();
        powerupManager.DestroySpecialPowersObjects();

        powerupManager.ResetData();

        if (selectedLevelBG.transform.Find("RingMask"))
        {
            selectedLevelBG.transform.Find("RingMask").gameObject.SetActive(true); //// put this someplace better in the future
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

            powerupManager.ClearTutorialPowerups();
        }

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
            StartLevel(false, true);
        }
    }

    public void CallNextLevelFromWinScreen()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);
        StartCoroutine(NextLevelFromWinScreen());
    }
    public IEnumerator NextLevelFromWinScreen()
    {
        StartCoroutine(UIManager.Instance.FadeInLevelNoLaunch());
        yield return new WaitForSeconds(UIManager.Instance.fadeIntoLevelDelay);

        hasRestartedLevel = false;

        AnimationManager.instance.ResetAllSkipData();

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
        LootManager.Instance.DestroyAllChestLootData();
        LootManager.Instance.DestoryWinScreenDisplyedLoot();

        LootManager.Instance.ResetLevelLootData();
        ConnectionManager.Instance.cells.Clear();
        ConnectionManager.Instance.tempSymbolPiecesStoneFound.Clear();
        ConnectionManager.Instance.amountStonePiecesInstantiated = 0;
        ConnectionManager.Instance.ResetAllLastPieceAlgoritmData();

        if (currentLevel.levelIndexInZone + 1 == ZoneManagerHelpData.Instance.currentZoneCheck.keyLevelIndex)
        {
            ZoneManager.Instance.CheckZoneAwardedKey(ZoneManagerHelpData.Instance.currentZoneCheck.id);
            ZoneManager.Instance.SetUnlockZone(ZoneManagerHelpData.Instance.currentZoneCheck.id + 1);
        }

        UIManager.Instance.bGPanelDisableTouch.SetActive(false);
        UIManager.Instance.youWinScreen.SetActive(false);
        UIManager.Instance.TurnOnGameplayUI();
        UIManager.Instance.ResetTopAndBottomPos();
        UIManager.Instance.isUsingUI = false;

        powerupManager.ResetData();

        foreach (GameObject go in UIManager.Instance.allTutorialScreens)
        {
            go.SetActive(false);
        }

        foreach (getChildrenHelpData GCHD in  powerupManager.instnatiateZones)
        {
            GCHD.referenceNumUsesText.gameObject.SetActive(false);
        }

        bool nextIsTutorial = CheckNextLevelIsTutorial(currentLevel.levelNum + 1);

        ChooseLevel(currentLevel.levelIndexInZone + 1);
        currentIndexInCluster++;

        if (selectedLevelBG.transform.Find("RingMask"))
        {
            selectedLevelBG.transform.Find("RingMask").gameObject.SetActive(true); //// put this someplace better in the future
        }

        if (nextIsTutorial)
        {
            TutorialSequence.Instacne.currentPhaseInSequenceLevels = 0;

            StartTutorialLevel(false);
        }
        else
        { 
            StartLevel(false, false);
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
