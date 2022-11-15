using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using GameAnalyticsSDK;
using System.Text.RegularExpressions;

[System.Serializable]
public class ButtonsPerZone
{
    public Zone theZone;

    public GameObject[] zone3DButtons; //3D Map
    //public Button[] zoneButtons;
}

[System.Serializable]
public class ImageTextCombo
{
    public GameObject imageObject;

    public GameObject[] textObjects;
    //public Button[] zoneButtons;
}
public enum BottomUIToShow
{
    None,
    OnlyDeal,
    All
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool isUsingUI;

    public GameObject mainMenu, worldGameObject, hudCanvasUI, itemForgeCanvas, gameplayCanvas, gameplayCanvasTop, ringersHutDisplay, ringersHutUICanvas, hollowCraftAndOwned;
    //public GameObject gameplayCanvasTopTestLevels;
    public GameObject InGameUiScreens;
    //public GameObject blackBagBG;
    public GameObject zoomInCorruptedBlack;
    public GameObject tutorialCanvasLevels;
    public GameObject tutorialCanvasSpecific;
    public GameObject tutorialCanvasParent;
    //public GameObject tutorialCanvasHolesParent;
    public GameObject forge, itemBag, Brewery;
    public GameObject craft, owned;
    public GameObject animalAlbum;
    public GameObject OptionsScreen;
    public GameObject cheatOptionsButton;
    public GameObject cheatOptionsScreen;
    public GameObject inLevelSettings;
    public GameObject wardrobe;
    public GameObject usingPowerupText;
    public GameObject youWinScreen, loseScreen/*, youLoseText*/;
    public GameObject endLevelSureMessage;
    public GameObject clipsAboutToEndMessage;
    public GameObject sureWantToRestartWithLoot;
    public GameObject sureWantToRestartNoLoot;
    public GameObject sureWantToLeaveLevel;
    public GameObject corruptedZoneScreen;
    public GameObject corruptedZoneSureMessage;
    public GameObject hudCanvasUIBottomZoneMainMap;
    public GameObject hudCanvasUIBottomZoneCorruption;
    public GameObject hudCanvasUIBottomZoneDenScreen;
    public GameObject UnlockedZoneMessageView;
    public GameObject dealButtonHeighlight;
    public GameObject openInventoryButtonHeighlight;
    public GameObject openInventoryButtonHeighlightDenScreen;
    public GameObject openDenButtonHeighlight;
    public GameObject openAnimalAlbumButtonHeighlight;
    public GameObject animalAlbumStagTabButtonHeighlight;
    public GameObject potionTabHeighlight;
    public GameObject hollowCraftTabHeighlight;
    public GameObject toHubButtonHeighlight;
    public GameObject brewButtonHeighlight;
    public GameObject closeInventoryHeighlight;
    public GameObject[] brewMaterialZonesHeighlights;
    public GameObject normalBookBG, potionsBookBG, craftingBookBG;
    public GameObject leaderboardScreen;
    public GameObject sureWantToResetDataScreen;
    public GameObject sureWantToLogOutScreen;
    public GameObject bGPanelDisableTouch;
    public GameObject DailyRewardScreen;
    public GameObject MissingMaterialsPotionCraftScreen;
    public GameObject MissingMaterialsHollowCraftScreen;
    public GameObject MissingMaterialsHollowObjectScreen;
    //public GameObject cantBuyPotionCraftScreen;
    //public GameObject gameplayCanvasScreensUIHEIGHLIGHTS;
    //public GameObject HudCanvasUIHEIGHLIGHTS;
    //public GameObject ItemAndForgeBagHEIGHLIGHTS;
    public GameObject brewedPotionScreen;
    public GameObject craftedHollowItemScreen;
    public GameObject fadeIntoLevel;
    public GameObject introScreenParent;
    public GameObject TEMPBgIntro;
    public GameObject placePieceVFX;
    public GameObject dealButtonVFX;
    public GameObject startAppLoadingScreen;
    public GameObject flowerUIMask;
    public GameObject releaseAnimalToDenScreen;
    public GameObject animalIsHappyScreen;
    public GameObject SystemUpdaterScreen;
    public GameObject bossLevelsParent;
    public GameObject disconnectedFromInternetScreen;
    public GameObject quitGameScreen;
    public GameObject gameplayCanvasBotom;
    public GameObject gameplayCanvasBotomDeal;
    public GameObject gameplayCanvasBotomPotions;

    public RectTransform zoneMoveObjectOnMap;

    public Image dewDropsImage;

    public Image tapControlsImage, dragControlsImage, tutorialDisableImage;
    public Image matsInventoryButton, forgeInventoryButton, potionInventoryButton;
    public Image canRepeatLevelsImage;

    public float fadeIntoLevelSpeed;
    public float fadeIntoLevelDelay;

    public float speedFadeInIntro;
    public float offsetTimeForFirstPage;
    public float speedFadeOutIntro;

    public Sprite toggleOffSprite, toggleOnSprite;

    public Transform sureLevelRestartLootDislpay;
    public Transform buyPotionLootDisplay;
    public Transform buyHollowItemDisplay;
    public Transform buyHollowItemSecondaryDisplay;
    public Transform ownedCorruptDevicesZone;

    public Text /*hubGoldText,*/ hubRubyText, dewDropsText;
    public Text width, height;
    public Text deltaWidth, deltaHeight;
    public Text boardScale;
    public TMP_Text dewDropsTextTime;

    public TMP_Text dailyLootTextTime;

    public TMP_Text /*gameplayGoldText,*/ gameplayRubyText/*, gameplayDewDropsText*/;
    public TMP_Text animalNameText;
    public TMP_Text sucessText;
    public TMP_Text nameOfPlayer;

    public TMP_Text currentLevelWorldName;
    public TMP_Text currentLevelNumber;
    public TMP_Text corruptedZoneSureMessageText;
    public TMP_Text zoneToUnlcokNameText;
    public TMP_Text versionText;
    public TMP_Text buyPotionRubieCoseText;
    public TMP_Text buyHollowItemRubieCostText;
    public TMP_Text buyHollowItemSecondaryRubieCostText;
    public TMP_Text animalReleaseToDenText;
    public TMP_Text animalIsHappyText;
    public TMP_Text SystemUpdaterScreenText;
    public TMP_Text TEMPTextBarBug;
    public TMP_Text TEMPTexthasGivenChest;
    //public TMP_Text cantBuyPotionText;

    //public Button commitButton;
    public Button nextLevelFromWinScreen;
    public Button restartGrindLevel;
    public Button restartButton;
    public Button optionsButtonIngame;
    public Button cheatOptionsButtonIngame;
    public Button skipAnimationButton;
    public Button getDailyLootButton;
    public Button dealButton;
    public Button backToHubButton;
    public Button buyPotionYesButton;
    public Button buyHollowItemYesButton;
    public Button buyHollowItemSecondaryYesButton;
    public Button openInventoryButttonMap;
    public Button openLeaderboardButttonMap;
    public Button openDenButttonMap;
    public Button openAnimalAlbumButttonMap;
    public Button animalAlbumStagTabButton;
    public Button returnToMapButttonDen;
    public Button openLeaderboardButttonDen;
    public Button openInventoryButttonDen;
    public Button openDailyLootButton;
    public Button openSettingsButton;
    public Button openCheatSettingsButton;
    public Button closeInventoryButton;
    public Button brewPotionButton;


    public Button requiredButtonForTutorialPhase;

    public CanvasGroup IntroSkipButton;
    public CanvasGroup IntroTapToContinue;

    public ScrollRect forgeScrollRectView;
    public ScrollRect mapScrollRect;

    //public Button[] levelButtons;
    public Slider bossHealthSlider;

    public ButtonsPerZone[] buttonsPerZone;
    public InventorySortButtonData[] inventorySortButtons;
    public InventorySortButtonData[] animalAlbumSortButtons;
    public GameObject[] allTutorialScreens;
    public ImageTextCombo[] introImages;
    //public Sprite[] dewDropsSprites;

    public Vector3 hubCameraPos;
    public Vector3 hubCameraRot;
    public Vector3 denCameraPosForTutorial;
    //public Vector3 denCameraPos;
    //public Vector3 denCameraRot;

    public SpriteMask maskImageDenScreen;

    //public Color gameTextColor;
    public static bool isDuringIntro;
    public static bool canAdvanceIntro;


    bool levelSettingsOpen = false;

    PanZoom PZ;

    public int introImageIndex = 0;
    public int introImageTextIndex = 0;

    public GameObject activeScreen;

    [Header("Hud Map Screen")]
    public float focusOffset;

    [Header("Review screen")]
    public ImageSwapOnClick[] starImages;
    public GameObject reviewUsPanel;
    public GameObject reviewUsPanelRepeatable;
    public GameObject thankyou4orLowerStars;
    public GameObject thankyou5Stars;

    [Header("Boss Header")]
    public GameObject bossBattleUIScreen;
    public GameObject bossScreensParent;
    public GameObject bossWellDoneScreen;
    public GameObject bossWinScreen;
    public TMP_Text bossScreenHPText;
    public TMP_Text bossDamageDoneText;
    public TMP_Text bossV2TimerText;

    [Header("Animal Album")]
    public GameObject getRewardScreen;
    public TMP_Text rubyAmountText;

    [Header("Inventory Hut and Leaderboard")]
    public Button[] inventoryButtons;
    public Button[] hutButtons;
    public Button[] LeaderboardButtons;
    public Button[] animalAlbumButtons;

    [Header("options screen")]
    public GameObject toMapButton;
    public GameObject googlePlayButton;
    public GameObject switchAccountButton;

    [Header("Test Levels Data Display")]
    public GameObject testLevelsDataScreen;
    public TMP_Text levelNumText;
    public TMP_Text levelDifficultyText;
    public TMP_Text zoneNameText;

    [Header("Dialogue ")]
    public GameObject dialogueMainGameobject;
    public Transform DialogueParent;
    public Button continueDialogueButton;
    public Button endDialogueButton;
    public Button skipButton;
    public ScrollRect dialogueScroller;
    public Coroutine textCoroutine;
    public float textSpeed;
    public float maxDownLimit;
    public float timeToScroll;
    public float heightScrollToAdd;
    public float startingHeight;
    public float dialogueEntryOffsetAddLeft;
    public float dialogueEntryOffsetAddRight;
    public float dialogueEntryOffsetAddAfterImage;
    public float imageEntryOffsetAdd;

    [Header("Sound")]
    public Image musicIconButtonLevel;
    public Image SFXIconButtonLevel;
    public Image musicIconButton, SFXIconButton;
    public Sprite musicIconOnLevel, musicIconOffLevel;
    public Sprite SFXIconOnLevel, SFXIconOffLevel;
    public Sprite musicIconOn, musicIconOff;
    public Sprite SFXIconOn, SFXIconOff;

    public NpcNametagCombo[] npcNametagsCombos;
    private void Start()
    {
        Instance = this;

        textCoroutine = null;

        isDuringIntro = false;
        canAdvanceIntro = true;

        startAppLoadingScreen.SetActive(true);/// ony screen we should see at the start


        versionText.gameObject.SetActive(true);

        reviewUsPanel.SetActive(false);
        reviewUsPanelRepeatable.SetActive(false);
        thankyou4orLowerStars.SetActive(false);
        thankyou5Stars.SetActive(false);

        mainMenu.SetActive(false); 

        worldGameObject.SetActive(true);
        hudCanvasUI.SetActive(false);
        hudCanvasUIBottomZoneMainMap.SetActive(true);

        itemForgeCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
        forge.SetActive(false);
        Brewery.SetActive(false);

        itemBag.SetActive(true); //// so this will be the first screen displayed, or else everyone will be turned off

        leaderboardScreen.SetActive(false);
        OptionsScreen.SetActive(false);
        cheatOptionsButton.SetActive(false);
        cheatOptionsButtonIngame.gameObject.SetActive(false);
        cheatOptionsScreen.SetActive(false);
        ringersHutDisplay.SetActive(false);
        ringersHutUICanvas.SetActive(false);
        wardrobe.SetActive(false);
        usingPowerupText.SetActive(false);
        youWinScreen.SetActive(false);
        //youLoseText.SetActive(false);
        craft.SetActive(true); //// so this will be the first screen displayed, or else everyone will be turned off
        owned.SetActive(false);
        hollowCraftAndOwned.SetActive(false);
        animalAlbum.SetActive(false);
        endLevelSureMessage.SetActive(false);
        clipsAboutToEndMessage.SetActive(false);
        sureWantToRestartWithLoot.SetActive(false);
        sureWantToRestartNoLoot.SetActive(false);
        sureWantToLeaveLevel.SetActive(false);
        loseScreen.SetActive(false);
        //blackLevelBG.SetActive(false);
        zoomInCorruptedBlack.SetActive(false);
        skipAnimationButton.gameObject.SetActive(false);
        InGameUiScreens.SetActive(false);
        corruptedZoneScreen.SetActive(false);
        corruptedZoneSureMessage.SetActive(false);
        hudCanvasUIBottomZoneCorruption.SetActive(false);
        hudCanvasUIBottomZoneDenScreen.SetActive(false);
        ownedCorruptDevicesZone.gameObject.SetActive(false);
        UnlockedZoneMessageView.gameObject.SetActive(false);
        tutorialCanvasParent.gameObject.SetActive(false);
        //tutorialCanvasHolesParent.gameObject.SetActive(false);
        tutorialCanvasSpecific.SetActive(false);
        tutorialCanvasLevels.SetActive(false);
        restartGrindLevel.gameObject.SetActive(false);
        sureWantToResetDataScreen.SetActive(false);
        bGPanelDisableTouch.SetActive(false);
        DailyRewardScreen.SetActive(false);
        MissingMaterialsPotionCraftScreen.SetActive(false);
        MissingMaterialsHollowCraftScreen.SetActive(false);
        MissingMaterialsHollowObjectScreen.SetActive(false);
        //cantBuyPotionCraftScreen.SetActive(false);
        //gameplayCanvasScreensUIHEIGHLIGHTS.SetActive(false);
        //HudCanvasUIHEIGHLIGHTS.SetActive(false);
        //ItemAndForgeBagHEIGHLIGHTS.SetActive(false);
        brewedPotionScreen.SetActive(false);
        craftedHollowItemScreen.SetActive(false);
        fadeIntoLevel.SetActive(false);
        introScreenParent.SetActive(false);
        bossBattleUIScreen.SetActive(false);
        releaseAnimalToDenScreen.SetActive(false);
        animalIsHappyScreen.SetActive(false);
        bossWellDoneScreen.SetActive(false);
        bossWinScreen.SetActive(false);
        bossScreensParent.SetActive(false);
        SystemUpdaterScreen.SetActive(false);
        bossLevelsParent.SetActive(false);
        disconnectedFromInternetScreen.SetActive(false);
        quitGameScreen.SetActive(false);
        getRewardScreen.SetActive(false);
        testLevelsDataScreen.SetActive(false);
        dialogueMainGameobject.SetActive(false);
        inLevelSettings.SetActive(false);

        activeScreen = null;


        foreach (var item in inventoryButtons)
        {
            item.interactable = false;
        }

        foreach (var item in hutButtons)
        {
            item.interactable = false;
        }

        foreach (var item in LeaderboardButtons)
        {
            item.interactable = false;
        }

        foreach (var item in animalAlbumButtons)
        {
            item.interactable = false;
        }

        dragControlsImage.sprite = toggleOnSprite;
        tapControlsImage.sprite = toggleOffSprite;
        tutorialDisableImage.sprite = toggleOnSprite;

        animalNameText.text = "";

        foreach (GameObject go in allTutorialScreens)
        {
            go.SetActive(false);
        }

        //foreach (ImageTextCombo imageTextCombo in introImages)
        //{
        //    imageTextCombo.imageObject.gameObject.SetActive(false);
        //    imageTextCombo.textObject.gameObject.SetActive(false);
        //}

        versionText.text = Application.version;

        PZ = Camera.main.GetComponent<PanZoom>();
    }
    private void Update()
    {
        //Debug.Log("Using UI? " + isUsingUI);
        //width.text = "Width: " + Display.main.systemWidth;
        //height.text = "Height: " + Display.main.systemHeight;

        float a = (float)1080 / (float)Display.main.systemWidth;
        float b = (float)1920 / (float)Display.main.systemHeight;
        deltaHeight.text = "Delta Width: " + a;
        deltaWidth.text = "Delta Height: " + b ;

        if (GameManager.Instance.gameBoard)
        {
            boardScale.text = "Board Scale" + GameManager.Instance.gameBoard.transform.localScale.x.ToString();
        }
        else
        {
            boardScale.text = "No board";
        }

        TEMPTextBarBug.text = "Testing bug system: " + TestLevelsSystemManagerSaveData.instance.CompletedCount.ToString();
        TEMPTexthasGivenChest.text = "Has given chest: " + AnimationManager.instance.hasGivenChest.ToString();


    }
    public void PlayButton()
    {
        ToHud(mainMenu);
        UnlockLevels();

        GameManager.Instance.clickedPlayButton = true;
    }
    public void ActivateGmaeplayCanvas()
    {
        gameplayCanvas.SetActive(true);
        worldGameObject.SetActive(false);
        hudCanvasUI.SetActive(false);

        levelSettingsOpen = false;
    }
    public void ToMainMenu()
    {
        worldGameObject.SetActive(false);
        hudCanvasUI.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void DisplayEndLevelMessage()
    {
        endLevelSureMessage.SetActive(true);
        bGPanelDisableTouch.SetActive(true);
    }
    public void EndLevelMessageNo()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        endLevelSureMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);

        GameManager.Instance.clipManager.RepopulateLatestClip();

    }
    public void EndLevelMessageYes()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        endLevelSureMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);


        DisplayLoseScreen();
    }
    public void DisplayClipsAboutToEndMessage()
    {
        clipsAboutToEndMessage.SetActive(true);
        bGPanelDisableTouch.SetActive(true);
    }
    public void ClipsAboutToEndMessageNo()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        clipsAboutToEndMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);

    }
    public void ClipsAboutToEndMessageYes()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        clipsAboutToEndMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);
        //LootManager.Instance.currentLevelLootToGive.Clear();
        LootManager.Instance.craftingMatsLootForLevel.Clear();
        LootManager.Instance.tempDataList.Clear();
        DisplayLoseScreen();

    }
    public void SureWantToRestartMessage()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (TutorialSequence.Instacne.duringSequence)
        {
            return;
        }

        bGPanelDisableTouch.SetActive(true);

        //isUsingUI = true;

        if (LootManager.Instance.rubiesToRecieveInLevel > 0 || LootManager.Instance.craftingMatsLootForLevel.Count > 0)
        {
            sureWantToRestartWithLoot.SetActive(true);
            DisplayRestartLoot();
        }
        else
        {
            sureWantToRestartNoLoot.SetActive(true);
        }
    }

    public void SureWantToLeaveLevelMessage()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (TutorialSequence.Instacne.duringSequence)
        {
            return;
        }

        bGPanelDisableTouch.SetActive(true);

        sureWantToLeaveLevel.SetActive(true);
    }
    public void SureWantToLeaveLevelMessageNo()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        bGPanelDisableTouch.SetActive(false);
        sureWantToLeaveLevel.SetActive(false);

        DisableInLevelSettings();
    }
    public void SureWantToLeaveLevelMessageYes()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        bGPanelDisableTouch.SetActive(false);
        sureWantToLeaveLevel.SetActive(false);
        DisableInLevelSettings();

        ToHud(gameplayCanvas);
    }
    private void DisplayRestartLoot()
    {
        if (LootManager.Instance.rubiesToRecieveInLevel > 0)
        {
            GameObject go = Instantiate(LootManager.Instance.lootDisplayPrefab, sureLevelRestartLootDislpay);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            CMD.materialImage.sprite = LootManager.Instance.rubySprite;
            CMD.materialCount.gameObject.SetActive(false);
        }

        foreach (LootToRecieve LTR in LootManager.Instance.craftingMatsLootForLevel)
        {
            GameObject go = Instantiate(LootManager.Instance.lootDisplayPrefab, sureLevelRestartLootDislpay);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            //CMD.materialImage.texture = Resources.Load(MaterialsAndForgeManager.Instance.materialSpriteByName[CM]) as Texture2D;
            CMD.materialImage.sprite = LootManager.Instance.allMaterialSprites[(int)LTR.type];

            CMD.materialCount.gameObject.SetActive(false);
        }
    }
    public void SureWantToRestartMessageNo(bool withLoot)
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        //isUsingUI = false;
        bGPanelDisableTouch.SetActive(false);

        if (withLoot)
        {
            sureWantToRestartWithLoot.SetActive(false);
            ClearLootDisplays();
        }
        else
        {
            sureWantToRestartNoLoot.SetActive(false);
        }

        //OptionsScreen.SetActive(false);
        DisableInLevelSettings();

        //if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
        //{
        //    TutorialSequence.Instacne.TurnOnTutorialScreensAfterRestart();
        //}
    }
    public void SureWantToRestartMessageYes(bool withLoot)
    {
        //isUsingUI = false;
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        bGPanelDisableTouch.SetActive(false);

        if (withLoot)
        {
            sureWantToRestartWithLoot.SetActive(false);
            ClearLootDisplays();
        }
        else
        {
            sureWantToRestartNoLoot.SetActive(false);
        }

        //OptionsScreen.SetActive(false);
        DisableInLevelSettings();

        //GameManager.Instance.RestartCurrentLevel();
        StartCoroutine(GameManager.Instance.RestartCurrentLevel());
    }
    public void ChangeZoneName(string name, int levelID)
    {
        currentLevelWorldName.text = name;

        currentLevelNumber.text = levelID.ToString();
    }
    public void ToHud(GameObject currentCanvas)
    {
        if (!SoundManager.Instance.hudBGMuisc.isPlaying)
        {
            StartCoroutine(SoundManager.Instance.FadeInMapBGMusic());
        }

        updateRubyAndDewDropsCount();
        PlayerManager.Instance.activePowerups.Clear();

        Camera.main.transform.rotation = Quaternion.Euler(hubCameraRot);
        GameManager.Instance.secondCam.fieldOfView = 60;

        PZ.SetFieldOfView();
        hudCanvasUIBottomZoneMainMap.SetActive(true);

        SortMaster.Instance.RefreshAllForgeScreens();

        toMapButton.SetActive(false);

        if (ServerRelatedData.instance.hasConnectedWithGooglePlay)
        {
            googlePlayButton.SetActive(false);
            switchAccountButton.SetActive(true);
        }
        else
        {
            googlePlayButton.SetActive(true);
            switchAccountButton.SetActive(false);
        }

        if (ZoneManagerHelpData.Instance.currentZoneCheck)
        {
            Vector3 currentZoneTransform = ZoneManagerHelpData.Instance.currentZoneCheck.transform.position;
            
            Vector3 tempForClamp = new Vector3(0f, currentZoneTransform.y, -3f);
            PanZoom pz = Camera.main.GetComponent<PanZoom>();
            tempForClamp.y = Mathf.Clamp(tempForClamp.y, pz.bottomBound, pz.topBound);

            Camera.main.transform.position = new Vector3(0,-1.6f, -3f);

            TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, Camera.main.transform.position.y, -0.05f);
        }
        else
        {
            Camera.main.transform.position = hubCameraPos;
            TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, Camera.main.transform.position.y, -0.05f);
        }

        FocusOnMaxLevelReached();






        if (currentCanvas == gameplayCanvas)
        {

            SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);


            if (GameManager.Instance.currentLevel.isTimerLevel)
            {
                TimerLevelManager.instance.DeactivateAll();
            }

            TestLevelsSystemManager.instance.SetDeactivatedLevelData();


            GameManager.Instance.levelStarted = false;
            GameManager.Instance.timeStartLevel = "";
            GameManager.Instance.hasRestartedLevel = false;

            if (SoundManager.Instance.normalAmbienceLevel.isPlaying)
            {
                SoundManager.Instance.CancelLeantweensSound();
                SoundManager.Instance.CancelCoRoutinesSound();

                SoundManager.Instance.CallFadeOutAmbientMusicLevel(SoundManager.Instance.timeFadeOutAmbienceLevel, true);
            }


            if (AnimalsManager.Instance.currentLevelLiveAnimal)
            {
                Destroy(AnimalsManager.Instance.currentLevelLiveAnimal.gameObject);
            }

            AnimationManager.instance.StopAllCoroutines();
            AnimationManager.instance.ResetAllSkipData();

            if (GameManager.Instance.currentLevel.isTutorial || GameManager.Instance.currentLevel.isSpecificTutorial)
            {
                TutorialSequence.Instacne.currentPhaseInSequenceLevels = 0;
                TutorialSequence.Instacne.currentPhaseInSequenceSpecific = 0;
                TutorialSequence.Instacne.DeactivateAllTutorialScreens();


                GameManager.Instance.powerupManager.ClearTutorialPowerups();
            }

            GameManager.Instance.powerupManager.ForceStopAllPowerupCoroutines();

            GameManager.Instance.copyOfArrayOfPiecesTutorial.Clear();
            ZoneManager.Instance.ActivateLevelDisplay();
            LootManager.Instance.DestoryWinScreenDisplyedLoot();
            GameManager.Instance.powerupManager.ResetData();
            GameManager.Instance.powerupManager.DestroySpecialPowersObjects();
            LightingSettingsManager.instance.ResetLightData();

            AnimationManager.instance.turnOff = null;
            AnimationManager.instance.destroyOnSkipEndLevel = null;

            restartGrindLevel.gameObject.SetActive(false);

            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);

            if (ServerRelatedData.instance.isAdmin)
            {
                cheatOptionsButton.SetActive(true);
                cheatOptionsButtonIngame.gameObject.SetActive(false);
            }

            cheatOptionsScreen.SetActive(false);
            youWinScreen.SetActive(false);
            bGPanelDisableTouch.SetActive(false);

            loseScreen.SetActive(false);
            tutorialCanvasLevels.SetActive(false);
            ResetTopAndBottomPos();

            foreach (GameObject go in allTutorialScreens)
            {
                go.SetActive(false);
            }

            foreach (getChildrenHelpData GCHD in GameManager.Instance.powerupManager.instnatiateZones)
            {
                GCHD.referenceNumUsesText.gameObject.SetActive(false);
            }

            foreach (GameObject go in TutorialSequence.Instacne.activatedHeighlights)
            {
                if (go)
                {
                    go.SetActive(false);
                }
            }

            GameManager.Instance.DestroyAllLevelChildern();
            LootManager.Instance.DestroyAllChestLootData();

            LootManager.Instance.ResetLevelLootData();
            LootManager.Instance.giveKey = false;
            ZoneManager.Instance.ResetZoneManagerData();
            ConnectionManager.Instance.ResetConnectionData();
            ConnectionManager.Instance.ResetAllLastPieceAlgoritmData();


            if (TutorialSequence.Instacne.duringSequence)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial)
                {
                    isUsingUI = true;

                    if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.AnimalAlbum)
                    {
                        TurnOnAnimalAlbumButtons();
                    }

                    if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft || GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
                    {
                        TurnOnRingersHutAndInventoryButtons();
                    }

                    StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
                }
                else
                {
                    TutorialSequence.Instacne.activatedHeighlights.Clear();
                    TutorialSequence.Instacne.activatedBoardParticles.Clear();



                    TutorialSequence.Instacne.duringSequence = false;
                }
            }
            else
            {
                StartCoroutine(SetIsUsingUI(false));
            }

            if (ZoneManager.Instance.zonesToUnlock.Count > 0)
            {
                ZoneManager.Instance.UnlockLevelViewSequence();
            }
            else
            {
                if (ServerRelatedData.instance.canShowReviewMessage)
                {
                    AppReviewManager.instance.ShowReviewMessage();
                }
            }

            UnlockLevels();



            if (GameManager.Instance.currentLevel.isBoss)
            {
                DeactivateBossBattleUIScreen();
                BossBattleManager.instance.bossLevelSO = null;
                BossBattleManager.instance.bossBattleStarted = false;

                BossBattleManager.instance.StopAllCoroutines();

                if (GameManager.Instance.currentLevel.ver1Boss)
                {
                    BossBattleManager.instance.ResetDataBossVer1();
                }
                else
                {
                    foreach (GameObject go in BossBattleManager.instance.completedRings)
                    {
                        Destroy(go.gameObject);
                    }

                    BossBattleManager.instance.completedRings.Clear();
                }
            }


            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }

        if (currentCanvas == ringersHutDisplay)
        {
            ringersHutDisplay.SetActive(false);
            ringersHutUICanvas.SetActive(false);
            hudCanvasUIBottomZoneDenScreen.SetActive(false);

            ZoneManager.Instance.ActivateLevelDisplay();
            PZ.isInDenScreen = false;

            StartCoroutine(SetIsUsingUI(false));

            TutorialSequence.Instacne.maskImage.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, TutorialSequence.Instacne.maskImage.transform.position.z);
        }

        if (currentCanvas == mainMenu)
        {
            mainMenu.SetActive(false);

            if (ZoneManager.Instance.zonesToUnlock.Count > 0 && !DailyRewardScreen.activeInHierarchy)
            {
                ZoneManager.Instance.UnlockLevelViewSequence();
            }
        }

        if (currentCanvas == corruptedZoneScreen)
        {
            CorruptedZonesManager.Instance.currentActiveZoneView.gameObject.SetActive(false);

            CorruptedZonesManager.Instance.currentActiveZoneData = null;
            CorruptedZonesManager.Instance.currentActiveZoneView = null;

            hudCanvasUIBottomZoneCorruption.SetActive(false);
            corruptedZoneScreen.SetActive(false);

            for (int i = 0; i < ownedCorruptDevicesZone.transform.childCount; i++)
            {
                Destroy(ownedCorruptDevicesZone.transform.GetChild(i).gameObject);
            }
        }

        if (GameObject.FindWithTag("Key"))
        {
            Destroy(GameObject.FindWithTag("Key").gameObject);
        }

        worldGameObject.SetActive(true);
        hudCanvasUI.SetActive(true);
    }
    public void OpenItemsAndForgeZone()
    {
        if (!itemForgeCanvas.activeInHierarchy)
        {
            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != openInventoryButttonMap && requiredButtonForTutorialPhase != openInventoryButttonDen)
                {
                    return;
                }
            }

            if (activeScreen)
            {
                activeScreen.SetActive(false);

                //itemForgeCanvas.SetActive(false);
                forge.SetActive(false);
                Brewery.SetActive(false);

                itemBag.SetActive(true);
            }

            activeScreen = itemForgeCanvas;

            SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

            itemForgeCanvas.SetActive(true);

            //isUsingUI = true;

            openInventoryTab();

            matsInventoryButton.enabled = false;
            forgeInventoryButton.enabled = true;
            potionInventoryButton.enabled = true;

            normalBookBG.SetActive(true);
            potionsBookBG.SetActive(false);
            craftingBookBG.SetActive(false);

            if (TutorialSequence.Instacne.duringSequence)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial && (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft || GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen))
                {
                    //HudCanvasUIHEIGHLIGHTS.SetActive(false);
                    //ItemAndForgeBagHEIGHLIGHTS.SetActive(true);

                    StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
                }
            }
        }
        else
        {
            if (!TutorialSequence.Instacne.duringSequence)
            {
                itemForgeCanvas.SetActive(false);

                forge.SetActive(false);
                Brewery.SetActive(false);

                itemBag.SetActive(true);

                //isUsingUI = false;
                activeScreen = null;
            }
        }
        //SortMaster.Instance.SortMatInventory(CraftingMatType.Build); //// For now we always open the inventory sorted on gems
    }
    public void ChangeInventorySortButtonSprites(int buttonID)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                return;
            }

            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != inventorySortButtons[buttonID - 1])
                {
                    return;
                }
            }
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        foreach (InventorySortButtonData ISBD in inventorySortButtons)
        {
            if (ISBD.id != buttonID)
            {
                Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 0);
                ISBD.transformImage.color = c;
                ISBD.transformImage.sprite = null;
            }
            else
            {
                Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 255);
                ISBD.transformImage.color = c;
                ISBD.transformImage.sprite = ISBD.selectedSprite;
            }
        }

        SortMaster.Instance.SortMatInventory((CraftingMatType)buttonID);
    }

    public void OpenAnimalAlbum()
    {
        if(!animalAlbum.activeInHierarchy)
        {
            SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != openAnimalAlbumButttonMap)
                {
                    return;
                }
            }

            if (activeScreen)
            {
                activeScreen.SetActive(false);
            }

            activeScreen = animalAlbum;


            foreach (InventorySortButtonData ISBD in animalAlbumSortButtons)
            {
                if (ISBD.id != 0)
                {
                    Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 0);
                    ISBD.transformImage.color = c;
                    ISBD.transformImage.sprite = null;
                }
                else
                {
                    Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 255);
                    ISBD.transformImage.color = c;
                    ISBD.transformImage.sprite = ISBD.selectedSprite;
                }
            }

            SortMaster.Instance.SortAnimalsInAlbum(0);

            animalAlbum.SetActive(true);

            if (TutorialSequence.Instacne.duringSequence)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.AnimalAlbum)
                {
                    StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
                }
            }
        }
        else
        {
            if (!TutorialSequence.Instacne.duringSequence)
            {
                animalAlbum.SetActive(false);

                activeScreen = null;
            }
        }

    }

    public void ChangeAnimalAlbumSortButtonSprites(int buttonID)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (requiredButtonForTutorialPhase.gameObject != animalAlbumSortButtons[buttonID].gameObject)
            {
                return;
            }
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        foreach (InventorySortButtonData ISBD in animalAlbumSortButtons)
        {
            if (ISBD.id != buttonID)
            {
                Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 0);
                ISBD.transformImage.color = c;
                ISBD.transformImage.sprite = null;
            }
            else
            {
                Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 255);
                ISBD.transformImage.color = c;
                ISBD.transformImage.sprite = ISBD.selectedSprite;
            }
        }

        SortMaster.Instance.SortAnimalsInAlbum(buttonID);


        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.AnimalAlbum)
            {
                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
            }
        }
    }

    public void OpenHollowCraftAndOwnedZone()
    {
        hollowCraftAndOwned.SetActive(true);
        owned.SetActive(false);
        craft.SetActive(true);

        //HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow = false;
        //isUsingUI = true;
    }
    public void OpenHollowOwnedObjectsToPlace()
    {
        hollowCraftAndOwned.SetActive(true);
        owned.SetActive(true);
        craft.SetActive(false);
        //HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow = true;

        //isUsingUI = true;
    }

    public void CloseWindowNoAdditionalAction(GameObject ToClose)
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        ToClose.SetActive(false);
    }

    public void closeWindow(GameObject ToClose)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
                {
                    return;
                }

            }
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (ToClose == itemForgeCanvas)
        {
            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != closeInventoryButton)
                {
                    return;
                }
            }

            itemForgeCanvas.SetActive(false);
            forge.SetActive(false);
            Brewery.SetActive(false);

            itemBag.SetActive(true);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.InterestPontSaveData });

        }

        if (ToClose == OptionsScreen)
        {
            OptionsScreen.SetActive(false);
        }

        if (ToClose == cheatOptionsScreen)
        {
            cheatOptionsScreen.SetActive(false);
        }

        if (ToClose == wardrobe)
        {
            wardrobe.SetActive(false);
        }

        if (ToClose == hollowCraftAndOwned)
        {
            hollowCraftAndOwned.SetActive(false);
            craft.SetActive(true);
            owned.SetActive(false);
        }

        if(ToClose == leaderboardScreen)
        {
            leaderboardScreen.SetActive(false);
        }

        if(ToClose == DailyRewardScreen)
        {
            DailyRewardScreen.SetActive(false);
            PlayfabManager.instance.UpdateAndSaveTimeSensitiveData(); //// could be that we need to send to save here

            ZoneManager.Instance.UnlockLevelViewSequence();
        }

        if(ToClose == SystemUpdaterScreen)
        {
            SystemUpdaterScreen.SetActive(false);
        }

        if(ToClose == disconnectedFromInternetScreen)
        {
            disconnectedFromInternetScreen.SetActive(false);
        }

        if(ToClose == quitGameScreen)
        {
            quitGameScreen.SetActive(false);
        }

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
            }
        }

        //if (!itemForgeCanvas.activeInHierarchy && !leaderboardScreen.activeInHierarchy && !DailyRewardScreen.activeInHierarchy && ZoneManager.Instance.zonesToUnlock.Count <= 0)
        //{
        //    Debug.Log("Here");

        //    StartCoroutine(SetIsUsingUI(false));
        //}

        StartCoroutine(SetIsUsingUI(false));

        activeScreen = null;
    }
    public void ToForge()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                return;
            }
        }
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);
        SoundManager.Instance.PlaySound(Sounds.PageFlip);

        itemBag.SetActive(false);
        Brewery.SetActive(false);
        forge.SetActive(true);
        matsInventoryButton.enabled = true;
        forgeInventoryButton.enabled = false;
        potionInventoryButton.enabled = true;
        normalBookBG.SetActive(false);
        potionsBookBG.SetActive(false);
        craftingBookBG.SetActive(true);

        if (TutorialSequence.Instacne.duringSequence)
        {
            HollowObjectDisplayer HOD = HollowCraftAndOwnedManager.Instance.hollowObjectsCreated[0].GetComponent<HollowObjectDisplayer>();

            //if (!HOD.canCraft)
            //{
                if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
                {
                    TutorialSequence.Instacne.AddToPlayerMatsForHollowCraft(HOD.craftingMatsForEquipment);
                    HOD.canCraft = true;
                }
            //}

            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());


                matsInventoryButton.GetComponent<Button>().interactable = true;
                potionInventoryButton.GetComponent<Button>().interactable = true;
            }
        }
    }
    public void OpenForgeImmidietly(string objectName)
    {
        for (int i = 0; i < HollowCraftAndOwnedManager.Instance.hollowObjectsCreated.Count; i++)
        {
            if(HollowCraftAndOwnedManager.Instance.hollowObjectsCreated[i].name == objectName)
            {
                HollowCraftAndOwnedManager.Instance.hollowObjectsCreated[i].transform.SetAsFirstSibling();
            }
        }

        RectTransform rt = MaterialsAndForgeManager.Instance.ForgeContent.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(0,0,0);

        OpenItemsAndForgeZone();
        ToForge();


    }
    public void ToItemsBag()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                return;
            }

            if (requiredButtonForTutorialPhase != matsInventoryButton.GetComponent<Button>())
            {
                return;
            }

        }

        openInventoryTab();

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);
        SoundManager.Instance.PlaySound(Sounds.PageFlip);

        itemBag.SetActive(true);
        forge.SetActive(false);
        Brewery.SetActive(false);

        matsInventoryButton.enabled = false;
        forgeInventoryButton.enabled = true;
        potionInventoryButton.enabled = true;

        normalBookBG.SetActive(true);
        potionsBookBG.SetActive(false);
        craftingBookBG.SetActive(false);

    }
    public void ToBrewery()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (requiredButtonForTutorialPhase != potionInventoryButton.GetComponent<Button>())
            {
                return;
            }
        }

        if (MaterialsAndForgeManager.Instance.equipmentInBrewScreen.Count <= 0)
        {
            return;
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);
        SoundManager.Instance.PlaySound(Sounds.PageFlip);

        itemBag.SetActive(false);
        forge.SetActive(false);
        Brewery.SetActive(true);

        matsInventoryButton.enabled = true;
        forgeInventoryButton.enabled = true;
        potionInventoryButton.enabled = false;

        craftingBookBG.SetActive(false);
        normalBookBG.SetActive(false);
        potionsBookBG.SetActive(true);


        Brewery.GetComponent<BreweryDisplayLogic>().GetAllAnchorPositions();

        if(MaterialsAndForgeManager.Instance.equipmentInBrewScreen.Count > 0)
        {
            Brewery.GetComponent<BreweryDisplayLogic>().SetSelectedPotion(MaterialsAndForgeManager.Instance.equipmentInBrewScreen[0]);
        }

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());
            }
        }

    }
    public void ToCraft()
    {
        owned.SetActive(false);
        craft.SetActive(true);
    }
    public void ToOwned()
    {
        owned.SetActive(true);
        craft.SetActive(false);

        //SortMaster.Instance.FilterHollowOwnedScreenByEnum(ObjectHollowType.All);
    }
    public void ToDenScreen(bool returningToHud)
    {
        if (AnimationManager.instance.isPlacingDenItem)
        {
            return;
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (returningToHud)
        {
            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != returnToMapButttonDen)
                {
                    return;
                }
            }

            isUsingUI = true;

            ToHud(ringersHutDisplay);
            return;
        }
        else
        {
            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != openDenButttonMap)
                {
                    return;
                }
            }
        }
        //StartCoroutine(HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame));
        GameManager.Instance.secondCam.fieldOfView = 120;
        HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);

        ringersHutDisplay.SetActive(true);
        ringersHutUICanvas.SetActive(true);
        hudCanvasUIBottomZoneDenScreen.SetActive(true);

        worldGameObject.SetActive(false);
        hudCanvasUIBottomZoneMainMap.SetActive(false);
        //hudCanvasUI.SetActive(false);

        //Camera.main.transform.position = denCameraPos;
        //Camera.main.transform.rotation = Quaternion.Euler(denCameraRot);

        LightingSettingsManager.instance.SetdenLight();

        PZ.isInDenScreen = true;

        HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                Camera.main.transform.position = denCameraPosForTutorial;
                TutorialSequence.Instacne.maskImage.transform.position = new Vector3(denCameraPosForTutorial.x, denCameraPosForTutorial.y, -2.02f);

                //HudCanvasUIHEIGHLIGHTS.SetActive(false);
                //ItemAndForgeBagHEIGHLIGHTS.SetActive(true);

                StartCoroutine(TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial());

            }
        }

    }
    public void OpenOptions()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            return;
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        OptionsScreen.SetActive(true);
        //isUsingUI = true;
    }
    public void OpenLevelSettings()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            return;
        }

        levelSettingsOpen = !levelSettingsOpen;

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (levelSettingsOpen)
        {
            isUsingUI = true;
            inLevelSettings.SetActive(true);
        }
        else
        {
            isUsingUI = false;
            inLevelSettings.SetActive(false);
        }
    }
    public void OpenCheatOptions()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            return;
        }

        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        cheatOptionsScreen.SetActive(true);
        //isUsingUI = true;
    }
    public void CloseGame()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        Debug.Log("Saved all data! - close game");

        Application.Quit();
    }
    public void OpenWardrobe()
    {
        wardrobe.SetActive(true);

        //isUsingUI = true;
    }
    public void ActivateUsingPowerupMessage(bool on)
    {
        if (on)
        {
            Debug.Log("usingPowerupText = true");
            //usingPowerupText.SetActive(true);
        }
        else
        {
            Debug.Log("usingPowerupText = false");

            //usingPowerupText.SetActive(false);
        }
    }
    public void WinLevel()
    {
        ClearLootDisplays();
        isUsingUI = true;
        //bGPanelDisableTouch.SetActive(true);
        //blackLevelBG.SetActive(false);
        //youWinScreen.SetActive(true);

        //AnimalPrefabData prefabData = AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();

        //string animalName = Regex.Replace(AnimalsManager.Instance.currentLevelAnimal.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

        //animalNameText.text = "You have released the " + animalName;
    }
    public void DisplayLoseScreen()
    {
        //gameplayCanvasBotom.SetActive(false);

        ClearLootDisplays();
        //isUsingUI = true;
        bGPanelDisableTouch.SetActive(true);

        loseScreen.SetActive(true);

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail,GameManager.Instance.currentLevel.worldName, GameManager.Instance.currentLevel.levelIndexInZone.ToString());
    }
    private void ClearLootDisplays()
    {
        foreach (Transform child in sureLevelRestartLootDislpay)
        {
            Destroy(child.gameObject);
        }
    }
    private void ClearBuyPotionLootDisplay()
    {
        foreach (Transform child in buyPotionLootDisplay)
        {
            Destroy(child.gameObject);
        }
    }
    private void ClearBuyHollowDisplay()
    {
        foreach (Transform child in buyHollowItemDisplay)
        {
            Destroy(child.gameObject);
        }

        HollowCraftAndOwnedManager.Instance.currentlyToCraftNoramlMehtod = null;
        HollowCraftAndOwnedManager.Instance.currentlyToCraftSecondMethod = null;
    }
    private void ClearBuyHollowSecondaryDisplay()
    {
        foreach (Transform child in buyHollowItemSecondaryDisplay)
        {
            Destroy(child.gameObject);
        }

        HollowCraftAndOwnedManager.Instance.currentlyToCraftNoramlMehtod = null;
        HollowCraftAndOwnedManager.Instance.currentlyToCraftSecondMethod = null;
    }
    public void RestartLevelFromLoseScreenUI()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        //isUsingUI = false;
        //gameplayCanvasBotom.SetActive(true);
        bGPanelDisableTouch.SetActive(false);

        loseScreen.SetActive(false);

        TurnOnGameplayUI();
    }
    public void UnlockLevels()
    {
        foreach (int ID in ZoneManager.Instance.unlockedZoneID)
        {
            ButtonsPerZone BPZ = buttonsPerZone.Where(p => p.theZone == ZoneManagerHelpData.Instance.listOfAllZones[ID]).Single();

            foreach (GameObject item in BPZ.zone3DButtons)
            {
                item.GetComponent<Interactable3D>().TurnOffVFX();
            }

            IconSpritesPerZone ISPZ = ZoneManagerHelpData.Instance.iconsPerZone.Where(p => p.zone == BPZ.theZone).Single();


            if (BPZ.theZone.hasUnlockedGrind)
            {
                foreach (GameObject go in ZoneManagerHelpData.Instance.zoneGrindLevelPerZone)
                {
                    if (go.GetComponent<Interactable3D>())
                    {
                        Interactable3D interactable = go.GetComponent<Interactable3D>();

                        if(interactable.connectedLevelScriptableObject.worldNum == BPZ.theZone.id)
                        {
                            go.GetComponent<Image>().sprite = ISPZ.grindLevelSprite;
                        }
                    }
                }
            }

            for (int i = 0; i < BPZ.zone3DButtons.Length; i++)
            {
                
                if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isChest)
                {
                    IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isChest).SingleOrDefault();

                    BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.levelFirstTimeIconSprite;


                }
                else if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isTutorialLevel)
                {
                    IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isTutorial).SingleOrDefault();

                    BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.levelFirstTimeIconSprite;
                }
                else if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isAnimalLevel)
                {
                    IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isAnimal).SingleOrDefault();

                    BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.levelFirstTimeIconSprite;
                }
                else
                {
                    BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPZ.levelFirstTimeIconSprite;
                }
            }

            for (int i = 0; i < BPZ.theZone.maxLevelReachedInZone; i++)
            {
                if (i == BPZ.zone3DButtons.Length)
                {
                    break;
                }

                if (i + 1 != BPZ.theZone.maxLevelReachedInZone)
                {                    
                    if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isChest)
                    {
                        IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isChest).SingleOrDefault();

                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.levelDoneSprite;

                    }
                    else if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isTutorialLevel)
                    {
                        IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isTutorial).SingleOrDefault();

                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.levelDoneSprite;
                    }
                    else if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isAnimalLevel)
                    {
                        IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isAnimal).SingleOrDefault();

                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.levelDoneSprite;
                    }
                    else
                    {
                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPZ.levelDoneSprite;
                    }
                }
                else
                {
                    BPZ.zone3DButtons[i].GetComponent<Interactable3D>().TurnOnVFX();
                    
                    if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isChest)
                    {
                        IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isChest).SingleOrDefault();

                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.nextLevelSprite;

                    }
                    else if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isTutorialLevel)
                    {
                        IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isTutorial).SingleOrDefault();

                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.nextLevelSprite;
                    }
                    else if (BPZ.zone3DButtons[i].GetComponent<Interactable3D>().isAnimalLevel)
                    {
                        IconSpritesPerCondition ISPC = ZoneManagerHelpData.Instance.iconsPerConditon.Where(p => p.isAnimal).SingleOrDefault();

                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPC.nextLevelSprite;
                    }
                    else
                    {
                        BPZ.zone3DButtons[i].GetComponent<Image>().sprite = ISPZ.nextLevelSprite;
                    }
                }

            }
        }
    }
    public void openInventoryTab()
    {
        foreach (InventorySortButtonData ISBD in inventorySortButtons)
        {
            if (ISBD.id != 1)
            {
                Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 0);
                ISBD.transformImage.color = c;
                ISBD.transformImage.sprite = null;
            }
            else
            {
                Color c = new Color(ISBD.transformImage.color.r, ISBD.transformImage.color.g, ISBD.transformImage.color.b, 255);
                ISBD.transformImage.color = c;
                ISBD.transformImage.sprite = ISBD.selectedSprite;
            }
        }

        SortMaster.Instance.SortMatInventory(CraftingMatType.Build);
    }
    public void TurnOffGameplayUI()
    {
        DecideBottmUIShow(GameManager.Instance.currentLevel.bottomUIToShow);
        gameplayCanvasTop.SetActive(false);
        InGameUiScreens.SetActive(false);

        if (ServerRelatedData.instance.isAdmin)
        {
            cheatOptionsButton.SetActive(false);
            cheatOptionsButtonIngame.gameObject.SetActive(false);
        }
    }
    public void TurnOnGameplayUI()
    {
        DecideBottmUIShow(GameManager.Instance.currentLevel.bottomUIToShow);
        gameplayCanvasTop.SetActive(true);
        InGameUiScreens.SetActive(true);
        dealButton.interactable = true;

        if (ServerRelatedData.instance.isAdmin)
        {
            cheatOptionsButton.SetActive(false);
            cheatOptionsButtonIngame.gameObject.SetActive(true);
        }
    }
    public void ChangeControlsTap()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        GameManager.Instance.isSecondaryControls = true;
        tapControlsImage.sprite = toggleOnSprite;
        dragControlsImage.sprite = toggleOffSprite;
    }
    public void ChangeControlsDrag()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        GameManager.Instance.isSecondaryControls = false;
        dragControlsImage.sprite = toggleOnSprite;
        tapControlsImage.sprite = toggleOffSprite;
    }
    public void DisableTutorials()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        GameManager.Instance.isDisableTutorials = !GameManager.Instance.isDisableTutorials;

        if (GameManager.Instance.isDisableTutorials)
        {
            tutorialDisableImage.sprite = toggleOffSprite;
        }
        else
        {
            tutorialDisableImage.sprite = toggleOnSprite;
        }
    }
    public void StartEnterCorruptedSequence(int ID)
    {
        zoomInCorruptedBlack.SetActive(true);


        AnimationManager.instance.StartZoomIntoCorruptArea(ID);
    }
    public void ActivateAreYouSureCorruptZone(string name)
    {
        corruptedZoneSureMessageText.text = "Is this the spot you want to place your " + name;
        corruptedZoneSureMessage.SetActive(true);
    }
    public void AreYouSureCorruptZoneYes()
    {
        corruptedZoneSureMessage.SetActive(false);

        CorruptedZonesManager.Instance.currentDeviceToPlace.SetDeviceOnZone();

        CorruptedZonesManager.Instance.currentDeviceToPlace = null;
    }
    public void AreYouSureCorruptZoneNo()
    {
        corruptedZoneSureMessage.SetActive(false);

        CorruptedZonesManager.Instance.currentDeviceToPlace.transform.SetParent(ownedCorruptDevicesZone);
        CorruptedZonesManager.Instance.currentDeviceToPlace.DiscradLines();

        CorruptedZonesManager.Instance.currentDeviceToPlace = null;
    }
    public void SetCorruptedDeviceImage(tempMoveScript TMS)
    {
        Image deviceImage = TMS.gameObject.GetComponent<Image>();

        deviceImage.sprite = Resources.Load<Sprite>(TMS.connectedCDD.spritePath);
    }
    public void DisplayUnlockedZoneMessage(int ID)
    {
        StartCoroutine(AnimationManager.instance.AnimateUnlockScreen(ID));
    }
    public void OpenLeaderboardScreen()
    {
        if (!leaderboardScreen.activeInHierarchy)
        {
            if (TutorialSequence.Instacne.duringSequence)
            {
                if (requiredButtonForTutorialPhase != openLeaderboardButttonMap && requiredButtonForTutorialPhase != openLeaderboardButttonDen)
                {
                    return;
                }
            }

            SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

            if (activeScreen)
            {
                activeScreen.SetActive(false);
            }

            activeScreen = leaderboardScreen;

            PlayfabManager.instance.GetLeaderboard();
        }
        else
        {
            leaderboardScreen.SetActive(false);
            activeScreen = null;
        }
    }
    public void SureWantToResetDataMessage()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (activeScreen)
        {
            activeScreen.SetActive(false);
        }

        activeScreen = sureWantToResetDataScreen;

        // no need to check if using UI since we can only get in here if the options menu is open so UI is used
        sureWantToResetDataScreen.SetActive(true);
    }
    public void SureWantToResetDataYes()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        sureWantToResetDataScreen.SetActive(false);

        PlayfabManager.instance.ResetAllData();
    }
    public void SureWantToResetDataNo()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        activeScreen = null;

        sureWantToResetDataScreen.SetActive(false);
    }
    public void SureWantToLogOutMessage()
    {
        // no need to check if using UI since we can only get in here if the options menu is open so UI is used

        if (activeScreen)
        {
            activeScreen.SetActive(false);
        }

        activeScreen = sureWantToLogOutScreen;
        sureWantToLogOutScreen.SetActive(true);

        StartCoroutine(PlayfabManager.instance.GetServerCurrentTimeUpdated());
    }
    public void SureWantToLogOutYes()
    {
        sureWantToLogOutScreen.SetActive(false);

        PlayfabManager.instance.LogOut();
    }
    public void SureWantToLogOutNo()
    {
        activeScreen = null;

        sureWantToLogOutScreen.SetActive(false);
    }
    public void DisplayDailyRewardsScreen()
    {
        if (ZoneManager.Instance.unlockedZoneID.Count > 1 && RewardsManager.Instance.canGiveDaily)
        {
            if (!isUsingUI)
            {
                DailyRewardScreen.SetActive(true);

                activeScreen = DailyRewardScreen;
            }
        }
    }
    public void DisplayDailyRewardsScreenNoCondition()
    {
        if (!isUsingUI)
        {
            if (TutorialSequence.Instacne.duringSequence)
            {
                if(requiredButtonForTutorialPhase != openDailyLootButton)
                {
                    return;
                }
            }
            SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

            DailyRewardScreen.SetActive(true);

            activeScreen = DailyRewardScreen;
        }
    }
    public void CallDeactivateDailyRewardScreen()
    {
        DeactivateDailyRewardScreen();
    }
    void DeactivateDailyRewardScreen()
    {
        closeWindow(DailyRewardScreen);
        ZoneManager.Instance.UnlockLevelViewSequence();
    }
    public void DisplayBuyPotionScreen()
    {
        MissingMaterialsPotionCraftScreen.SetActive(true);
    }
    public void DisplayBuyPotionScreenRubyCostText(int amount, bool canbuy)
    {
        if (canbuy)
        {
            buyPotionYesButton.interactable = true;
            buyPotionRubieCoseText.color = Color.white;
        }
        else
        {
            buyPotionYesButton.interactable = false;
            buyPotionRubieCoseText.color = Color.red;
        }

        buyPotionRubieCoseText.text = amount.ToString();
    }
    public void BuyPotionScreenYes()
    {
        MissingMaterialsPotionCraftScreen.SetActive(false);
        ClearBuyPotionLootDisplay();
    }
    public void BuyPotionScreenNo()
    {
        MissingMaterialsPotionCraftScreen.SetActive(false);
        ClearBuyPotionLootDisplay();
    }
    public void DisplayBuyPotionLootNeeded(List<CraftingMatsNeededToRubies> INmaterialsNeedToBuyPotion)
    {
        foreach (CraftingMatsNeededToRubies CMNTR in INmaterialsNeedToBuyPotion)
        {
            GameObject go = Instantiate(LootManager.Instance.lootDisplayPrefab, buyPotionLootDisplay);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            if (CMNTR.mat == CraftingMats.DewDrops)
            {
                CMD.materialImage.sprite = LootManager.Instance.dewDropsSprite;
                CMD.materialCount.text = CMNTR.amountMissing.ToString();
            }
            else
            {
                CMD.materialImage.sprite = LootManager.Instance.allMaterialSprites[(int)CMNTR.mat];

                CMD.materialCount.text = CMNTR.amountMissing.ToString();
            }
        }
    }
    public void DisplayBuyHollowScreen()
    {
        MissingMaterialsHollowCraftScreen.SetActive(true);
    }
    public void DisplayBuyHollowSecondaryScreen()
    {
        MissingMaterialsHollowObjectScreen.SetActive(true);
    }
    public void DisplayHollowScreenRubyCostText(int amount, bool canbuy)
    {
        if (canbuy)
        {
            buyHollowItemYesButton.interactable = true;
            buyHollowItemRubieCostText.color = Color.white;
        }
        else
        {
            buyHollowItemYesButton.interactable = false;
            buyHollowItemRubieCostText.color = Color.red;
        }

        buyHollowItemRubieCostText.text = amount.ToString();
    }
    public void DisplayHollowScreenSecondaryRubyCostText(int amount, bool canbuy)
    {
        if (canbuy)
        {
            buyHollowItemSecondaryYesButton.interactable = true;
            buyHollowItemSecondaryRubieCostText.color = Color.white;
        }
        else
        {
            buyHollowItemSecondaryYesButton.interactable = false;
            buyHollowItemSecondaryRubieCostText.color = Color.red;
        }

        buyHollowItemSecondaryRubieCostText.text = amount.ToString();
    }
    public void BuyHollowItemScreenYes()
    {
        MissingMaterialsHollowCraftScreen.SetActive(false);
        ClearBuyHollowDisplay();
    }
    public void BuyHollowItemScreenNo()
    {
        MissingMaterialsHollowCraftScreen.SetActive(false);
        ClearBuyHollowDisplay();
    }
    public void BuyHollowItemSecondaryScreenYes()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        MissingMaterialsHollowObjectScreen.SetActive(false);
        ClearBuyHollowSecondaryDisplay();
    }
    public void BuyHollowItemSecondaryScreenNo()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        MissingMaterialsHollowObjectScreen.SetActive(false);
        ClearBuyHollowSecondaryDisplay();
    }
    public void DisplayBuyHollowItemNeeded(List<CraftingMatsNeededToRubies> INmaterialsNeedToBuyHollow)
    {
        foreach (CraftingMatsNeededToRubies CMNTR in INmaterialsNeedToBuyHollow)
        {
            GameObject go = Instantiate(LootManager.Instance.lootDisplayPrefab, buyHollowItemDisplay);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            if (CMNTR.mat == CraftingMats.DewDrops)
            {
                CMD.materialImage.sprite = LootManager.Instance.dewDropsSprite;
                CMD.materialCount.text = CMNTR.amountMissing.ToString();
            }
            else
            {
                CMD.materialImage.sprite = LootManager.Instance.allMaterialSprites[(int)CMNTR.mat];

                CMD.materialCount.text = CMNTR.amountMissing.ToString();
            }
        }
    }
    public void DisplayBuySecondaryHollowItemNeeded(List<CraftingMatsNeededToRubies> INmaterialsNeedToBuyHollow)
    {
        foreach (CraftingMatsNeededToRubies CMNTR in INmaterialsNeedToBuyHollow)
        {
            GameObject go = Instantiate(LootManager.Instance.lootDisplayPrefab, buyHollowItemSecondaryDisplay);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            if (CMNTR.mat == CraftingMats.DewDrops)
            {
                CMD.materialImage.sprite = LootManager.Instance.dewDropsSprite;
                CMD.materialCount.text = CMNTR.amountMissing.ToString();
            }
            else
            {
                CMD.materialImage.sprite = LootManager.Instance.allMaterialSprites[(int)CMNTR.mat];

                CMD.materialCount.text = CMNTR.amountMissing.ToString();
            }
        }
    }
    public void updateRubyAndDewDropsCount()
    {
        gameplayRubyText.text = PlayerManager.Instance.rubyCount.ToString();
        hubRubyText.text = PlayerManager.Instance.rubyCount.ToString();
        dewDropsText.text = PlayerManager.Instance.collectedDewDrops.ToString();
    }
    public IEnumerator FadeIntoLevel(bool isTutorialLevel, bool isBossLevel)
    {
        fadeIntoLevel.SetActive(true);

        if (!isBossLevel)
        {
            LeanTween.value(fadeIntoLevel, 0, 1, fadeIntoLevelSpeed).setEase(LeanTweenType.linear).setOnComplete(() => StartCoroutine(GameManager.Instance.ResetDataStartLevel(isTutorialLevel, false))).setOnUpdate((float val) =>
            {
                Image sr = fadeIntoLevel.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }
        else
        {
            LeanTween.value(fadeIntoLevel, 0, 1, fadeIntoLevelSpeed).setEase(LeanTweenType.linear).setOnComplete(() => StartCoroutine(GameManager.Instance.ResetDataStartBossLevel())).setOnUpdate((float val) =>
            {
                Image sr = fadeIntoLevel.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }
        yield return new WaitForSeconds(fadeIntoLevelDelay);


        LeanTween.value(fadeIntoLevel, 1, 0, fadeIntoLevelSpeed).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
        {
            Image sr = fadeIntoLevel.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        yield return new WaitForSeconds(fadeIntoLevelSpeed + 0.1f);
        fadeIntoLevel.SetActive(false);
    }

    public IEnumerator FadeInLevelNoLaunch()
    {
        fadeIntoLevel.SetActive(true);

        LeanTween.value(fadeIntoLevel, 0, 1, fadeIntoLevelSpeed).setOnUpdate((float val) =>
        {
            Image sr = fadeIntoLevel.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        yield return new WaitForSeconds(fadeIntoLevelDelay);


        LeanTween.value(fadeIntoLevel, 1, 0, fadeIntoLevelSpeed).setOnUpdate((float val) =>
        {
            Image sr = fadeIntoLevel.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        yield return new WaitForSeconds(fadeIntoLevelSpeed + 0.1f);
        fadeIntoLevel.SetActive(false);
    }
    public IEnumerator DisplayIntro()
    {   isDuringIntro = true;
        canAdvanceIntro = false;

        introScreenParent.SetActive(true);

        LeanTween.value(TEMPBgIntro, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = TEMPBgIntro.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        LeanTween.value(introImages[0].imageObject, 0, 1, speedFadeInIntro + offsetTimeForFirstPage).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = introImages[0].imageObject.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });


        yield return new WaitForSeconds(speedFadeInIntro + offsetTimeForFirstPage);

        TextMeshProEffect effect = introImages[0].textObjects[0].GetComponent<TextMeshProEffect>();
        effect.gameObject.SetActive(true);
        effect.Play();

        LeanTween.value(IntroSkipButton.gameObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            IntroSkipButton.alpha = val;
        });

        LeanTween.value(IntroTapToContinue.gameObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            IntroTapToContinue.alpha = val;
        });

        foreach (ImageTextCombo ITC in introImages)
        {
            if(ITC != introImages[0])
            {
                LeanTween.value(ITC.imageObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    Image sr = ITC.imageObject.GetComponent<Image>();
                    Color newColor = sr.color;
                    newColor.a = val;
                    sr.color = newColor;
                });

                LeanTween.value(ITC.textObjects[0], 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    TMP_Text sr = ITC.textObjects[0].GetComponent<TMP_Text>();
                    Color newColor = sr.color;
                    newColor.a = val;
                    sr.color = newColor;
                });
            }
        }

        yield return new WaitForSeconds(speedFadeInIntro);

        canAdvanceIntro = true;
        introImageTextIndex++;
    }
    public IEnumerator AdvanceIntroScreen()
    {
        bool pageFlipped = false;
        canAdvanceIntro = false;

        if (introImageTextIndex >= introImages[introImageIndex].textObjects.Count())
        {
            SoundManager.Instance.PlaySound(Sounds.PageFlip);
            canAdvanceIntro = false;

            LeanTween.value(introImages[introImageIndex].imageObject, 1, 0, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                Image sr = introImages[introImageIndex].imageObject.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });

            LeanTween.value(IntroSkipButton.gameObject, 1, 0, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                IntroSkipButton.alpha = val;
            });

            LeanTween.value(IntroTapToContinue.gameObject, 1, 0, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                IntroTapToContinue.alpha = val;
            });


            LeanTween.value(introImages[introImageIndex].textObjects[introImages[introImageIndex].textObjects.Count() - 1], 1, 0, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                TMP_Text sr = introImages[introImageIndex].textObjects[introImages[introImageIndex].textObjects.Count() - 1].GetComponent<TMP_Text>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });

            pageFlipped = true;
            introImageTextIndex = 0;
        }
        else
        {
            if(introImageTextIndex > 0)
            {
                LeanTween.value(introImages[introImageIndex].textObjects[introImageTextIndex - 1], 1, 0, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    TMP_Text sr = introImages[introImageIndex].textObjects[introImageTextIndex - 1].GetComponent<TMP_Text>();
                    Color newColor = sr.color;
                    newColor.a = val;
                    sr.color = newColor;
                });
            }
            else
            {
                LeanTween.value(introImages[introImageIndex].textObjects[0], 1, 0, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    TMP_Text sr = introImages[introImageIndex].textObjects[0].GetComponent<TMP_Text>();
                    Color newColor = sr.color;
                    newColor.a = val;
                    sr.color = newColor;
                });
            }

            TextMeshProEffect effect = introImages[introImageIndex].textObjects[introImageTextIndex].GetComponent<TextMeshProEffect>();
            effect.gameObject.SetActive(true);
            effect.Play();

        }


        if (pageFlipped)
        {
            yield return new WaitForSeconds(speedFadeOutIntro + 0.1f);
            introImageIndex++;

            if (introImageIndex >= introImages.Count())
            {
                isDuringIntro = false;
                canAdvanceIntro = false;

                TutorialSaveData.Instance.hasFinishedIntro = true;
                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

                DisplayDailyRewardsScreen();

                PlayButton();
                StartCoroutine(SoundManager.Instance.PlaySoundAmbienceFadeOut(SoundManager.Instance.fadeOutIntroSound));

                LeanTween.value(TEMPBgIntro, 1, 0, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    Image sr = TEMPBgIntro.GetComponent<Image>();
                    Color newColor = sr.color;
                    newColor.a = val;
                    sr.color = newColor;
                });

                yield return new WaitForSeconds(speedFadeOutIntro + 0.1f);

                introScreenParent.SetActive(false);
                yield break;
            }
            else
            {                 
                LeanTween.value(IntroSkipButton.gameObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    IntroSkipButton.alpha = val;
                });

                LeanTween.value(IntroTapToContinue.gameObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    IntroTapToContinue.alpha = val;
                });

                TextMeshProEffect effect = introImages[introImageIndex].textObjects[introImageTextIndex].GetComponent<TextMeshProEffect>();
                effect.gameObject.SetActive(true);
                effect.Play();
            }

            yield return new WaitForSeconds(speedFadeInIntro);

            introImages[introImageIndex - 1].imageObject.SetActive(false);
        }

        yield return new WaitForSeconds(speedFadeInIntro + 0.1f);

        canAdvanceIntro = true;
        introImageTextIndex++;
    }
    public void SkipIntroScreens()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        isDuringIntro = false;
        canAdvanceIntro = false;

        TutorialSaveData.Instance.hasFinishedIntro = true;
        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

        DisplayDailyRewardsScreen();

        PlayButton();
        StartCoroutine(SoundManager.Instance.PlaySoundAmbienceFadeOut(SoundManager.Instance.fadeOutIntroSound));

        StartCoroutine(SkipIntroFade());
    }
    public IEnumerator SkipIntroFade()
    {
        if (introImageIndex != introImages.Count() - 1)
        {
            LeanTween.value(introScreenParent, 1, 0, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                CanvasGroup group = introScreenParent.GetComponent<CanvasGroup>();
                group.alpha = val;
            });
        }

        yield return new WaitForSeconds(speedFadeOutIntro + 0.1f);

        introScreenParent.SetActive(false);

    }
    public void DisplayBossBattleUIScreen()
    {
        bossBattleUIScreen.SetActive(true);

        bossHealthSlider.value = (float)BossBattleManager.instance.currentBossHealth / (float)BossBattleManager.instance.bossLevelSO.BossHealth;

        float currentHP = (float)BossBattleManager.instance.currentBossHealth;
        float MaxHP = (float)BossBattleManager.instance.bossLevelSO.BossHealth;

        bossScreenHPText.text = currentHP.ToString() + "/" + MaxHP.ToString();
    }
    public void DeactivateBossBattleUIScreen()
    {
        bossBattleUIScreen.SetActive(false);
    }
    public void UpdateBossHealth()
    {
        bossHealthSlider.value = (float)BossBattleManager.instance.currentBossHealth / (float)BossBattleManager.instance.bossLevelSO.BossHealth;

        float currentHP = (float)BossBattleManager.instance.currentBossHealth;
        float MaxHP = (float)BossBattleManager.instance.bossLevelSO.BossHealth;

        bossScreenHPText.text = currentHP.ToString() + "/" + MaxHP.ToString();
    }
    public void ResetTopAndBottomPos()
    {
        RectTransform top = gameplayCanvasTop.GetComponent<RectTransform>();
        RectTransform bottom = gameplayCanvasBotom.GetComponent<RectTransform>();

        top.anchoredPosition = new Vector2(0, 0);
        bottom.anchoredPosition = new Vector2(0, 0);
    }
    public void turnOnDealVFX()
    {
        GameObject go = Instantiate(dealButtonVFX, dealButton.transform);
    }
    public IEnumerator SetIsUsingUI(bool isTrue)
    {
        yield return new WaitForSeconds(0.3f);

        isUsingUI = isTrue;
    }
    public void CallSetIsUsingUI(bool isTrue)
    {
        StartCoroutine(SetIsUsingUI(isTrue));
    }
    public IEnumerator MoveAfterLoadingScreen(bool goodLogin)
    {
        yield return new WaitForSeconds(2);

        if (goodLogin)
        {
            startAppLoadingScreen.SetActive(false);
        }
        else
        {
            startAppLoadingScreen.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
    public void PrepareObjectForEndBoardAnim()
    {
        sucessText.color = new Color(sucessText.color.r, sucessText.color.g, sucessText.color.b, 0);
        animalNameText.color = new Color(animalNameText.color.r, animalNameText.color.g, animalNameText.color.b, 0);

        Image backToHubImage = backToHubButton.GetComponent<Image>();
        backToHubImage.color = new Color(backToHubImage.color.r, backToHubImage.color.g, backToHubImage.color.b, 0);

        Image nextLevelButtonImage = nextLevelFromWinScreen.GetComponent<Image>();
        nextLevelButtonImage.color = new Color(nextLevelButtonImage.color.r, nextLevelButtonImage.color.g, nextLevelButtonImage.color.b, 0);

        CanvasGroup restartGrind = restartGrindLevel.GetComponent<CanvasGroup>();
        restartGrind.alpha = 0;

        flowerUIMask.transform.localScale = Vector3.zero;
    }


    public void CheckTurnOnReleaseAnimalScreen()
    {
        if (GameManager.Instance.currentLevel.isReleaseAnimalToDen)
        {
            AnimalPrefabData APD = AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();
            zoneSlotAndType ZSAT = HollowCraftAndOwnedManager.Instance.hollowZones.Where(p => p.acceptedHollowTypes.Contains(APD.animalSO.hollowBelongTo)).SingleOrDefault();

            if (AnimalsManager.Instance.CheckConditionsSummonAnimalDen(APD, ZSAT))
            {
                TurnOnReleaseAnimalDenScreen();
            }
        }
    }

    public void TurnOnReleaseAnimalDenScreen()
    {
        string animalName = Regex.Replace(AnimalsManager.Instance.currentLevelAnimal.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

        animalReleaseToDenText.text = animalName + " seems to be following you. Would you like to take it to your ringer's den?";

        AnimalPrefabData APD = AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();

        if (AnimalsManager.Instance.CheckIfAlreadyPlacedAnimalInDenForUI(APD))
        {
            releaseAnimalToDenScreen.SetActive(true);
        }
    }
    public void ReleaseAnimalDenScreenYes()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        releaseAnimalToDenScreen.SetActive(false);

        ToHud(gameplayCanvas);

        AnimalsManager.Instance.CallSpawnAnimalInDen();

        ToDenScreen(false);

        TurnOnAnimalIsHappyScreen();
    }
    public void ReleaseAnimalDenScreenNo()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        releaseAnimalToDenScreen.SetActive(false);
    }

    public void TurnOnAnimalIsHappyScreen()
    {
        string animalName = Regex.Replace(AnimalsManager.Instance.currentLevelAnimal.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

        animalIsHappyText.text = animalName + " seems very happy with it's new home!";

        animalIsHappyScreen.SetActive(true);
    }
    public void TurnOFFAnimalIsHappyScreen()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        animalIsHappyScreen.SetActive(false);
    }

    public void SetCanRepeatLevelsDisplay()
    {
        if (ServerRelatedData.instance.canRepeatLevels)
        {
            canRepeatLevelsImage.sprite = toggleOnSprite;
        }
        else
        {
            canRepeatLevelsImage.sprite = toggleOffSprite;
        }
    }

    public void CheckDisplayCheatMenusAndObjects()
    {
        if (ServerRelatedData.instance.isAdmin)
        {
            cheatOptionsButton.SetActive(true);
            cheatOptionsButtonIngame.gameObject.SetActive(false);
            bossLevelsParent.SetActive(true);
            //ZoneManagerHelpData.Instance.testZone.SetActive(true);

            foreach (Button button in inventoryButtons)
            {
                button.interactable = true;
            }

            foreach (Button button in animalAlbumButtons)
            {
                button.interactable = true;
            }

            TutorialSaveData.Instance.hasFinishedPotion = true;
            TutorialSaveData.Instance.hasFinishedAnimalAlbum = true;
        }
    }

    public void DisplayBossWellDoneScreen()
    {
        bossScreensParent.SetActive(true);
        bossWellDoneScreen.SetActive(true);
        GameManager.Instance.gameClip.SetActive(false);
        gameplayCanvas.SetActive(false);
        bossV2TimerText.gameObject.SetActive(false);
        PlayerManager.Instance.AddRubies(GameManager.Instance.currentLevel.rubyRewardNoDefeat);

        bossDamageDoneText.text = "You managed to deal " + BossBattleManager.instance.damageDealtToBossCurrentFight + " damage to the Rive!";

    }
    public void DisplayBossWinScreen()
    {
        bossScreensParent.SetActive(true);
        bossWinScreen.SetActive(true);
        gameplayCanvas.SetActive(false);
        GameManager.Instance.gameClip.SetActive(false);
        bossV2TimerText.gameObject.SetActive(false);
        PlayerManager.Instance.AddRubies(GameManager.Instance.currentLevel.rubyRewardDefeat);
    }

    public void PressedGreatButton()
    {
        bossWellDoneScreen.SetActive(false);
        bossWinScreen.SetActive(false);

        ToHud(gameplayCanvas);
    }

    public void BGOff()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (GameManager.Instance.selectedLevelBG.activeInHierarchy)
        {
            GameManager.Instance.selectedLevelBG.SetActive(false);
        }
        else
        {
            GameManager.Instance.selectedLevelBG.SetActive(true);
        }
    }
    public void HideLevelData()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (GameManager.Instance.destroyOutOfLevel.gameObject.activeInHierarchy)
        {
            GameManager.Instance.destroyOutOfLevel.gameObject.SetActive(false);
        }
        else
        {
            GameManager.Instance.destroyOutOfLevel.gameObject.SetActive(true);
        }
    }

    public void TurnOnDisconnectedScreen()
    {
        disconnectedFromInternetScreen.SetActive(true);
    }

    public void ShowAreYouSureQuitScreen()
    {
        StartCoroutine(PlayfabManager.instance.GetServerCurrentTimeUpdated());

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ALL });

        quitGameScreen.SetActive(true);
    }

    public void DecideBottmUIShow(BottomUIToShow toShow)
    {
        switch (toShow)
        {
            case BottomUIToShow.None:
                NoBottomUI();
                break;
            case BottomUIToShow.OnlyDeal:
                BottomUIOnlyDeal();
                break;
            case BottomUIToShow.All:
                AllBottomUI();
                break;
            default:
                break;
        }
    }
    private void NoBottomUI()
    {
        //gameplayCanvasBotom.SetActive(false);
        gameplayCanvasBotomDeal.SetActive(false);
        gameplayCanvasBotomPotions.SetActive(false);

    }
    private void BottomUIOnlyDeal()
    {
        gameplayCanvasBotom.SetActive(true);
        gameplayCanvasBotomDeal.SetActive(true);
        gameplayCanvasBotomPotions.SetActive(false);
    }
    private void AllBottomUI()
    {
        gameplayCanvasBotom.SetActive(true);
        gameplayCanvasBotomDeal.SetActive(true);
        gameplayCanvasBotomPotions.SetActive(true);
    }

    public void FocusOnArea(int id)
    {
        zoneMoveObjectOnMap.anchoredPosition = new Vector3(zoneMoveObjectOnMap.transform.position.x, ZoneManagerHelpData.Instance.unlockPosPerZone[id].y, 0);
    }

    public void TurnOnRingersHutAndInventoryButtons()
    {  
        foreach (var item in inventoryButtons)
        {
            item.interactable = true;
        }

        foreach (var item in hutButtons)
        {
            item.interactable = true;
        }
    }

    public void TurnOnLeaderboardButtons()
    {
        foreach (var item in LeaderboardButtons)
        {
            item.interactable = true;
        }
    }
    public void TurnOnAnimalAlbumButtons()
    {
        foreach (var item in animalAlbumButtons)
        {
            item.interactable = true;
        }
    }

    public void SetReviewStarDisplayAmount(int amount)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetNotClicked();
        }

        for (int i = 0; i <= amount; i++)
        {
            starImages[i].SetClicked();
        }

        ServerRelatedData.instance.appReviewStarsAmountSelected = amount + 1;
    }

    public void FocusOnMaxLevelReached()
    {
        RectTransform levelRect = null;
        RectTransform zoneRect = null;

        levelRect = GetLevelDisplayRef();
        zoneRect = GetZoneDisplayRef();

        float levelPosRectY = levelRect.anchoredPosition.y;
        float zonePosRectY = Mathf.Abs(zoneRect.anchoredPosition.y);

        float posY = zonePosRectY - levelPosRectY;
        posY -= focusOffset;

        Vector3 newPos = new Vector3(zoneMoveObjectOnMap.transform.position.x, posY, zoneMoveObjectOnMap.transform.position.z);
        zoneMoveObjectOnMap.anchoredPosition = newPos;
    }

    public RectTransform GetLevelDisplayRef()
    {
        RectTransform levelButtonRect = null;
        int highestLevelReached = PlayerManager.Instance.highestLevelReached;

        if (highestLevelReached == 0)
        {
            highestLevelReached = 1;
        }

        foreach (ButtonsPerZone BPZ in buttonsPerZone)
        {
            foreach (GameObject go in BPZ.zone3DButtons)
            {
                Interactable3D buttonInteract = go.GetComponent<Interactable3D>();

                if(buttonInteract.connectedLevelScriptableObject.numIndexForLeaderBoard == highestLevelReached)
                {
                    levelButtonRect = go.GetComponent<RectTransform>();
                    return levelButtonRect;
                }
            }
        }

        return null;
    }

    public RectTransform GetZoneDisplayRef()
    {
        RectTransform zoneRectPos = null;

        int highestLevelReached = PlayerManager.Instance.highestLevelReached;

        if (highestLevelReached == 0)
        {
            highestLevelReached = 1;
        }

        foreach (ButtonsPerZone BPZ in buttonsPerZone)
        {
            foreach (GameObject go in BPZ.zone3DButtons)
            {
                Interactable3D buttonInteract = go.GetComponent<Interactable3D>();

                if(buttonInteract.connectedLevelScriptableObject.numIndexForLeaderBoard == highestLevelReached)
                {
                    zoneRectPos = BPZ.theZone.GetComponent<RectTransform>();
                    return zoneRectPos;
                }
            }
        }

        return null;
    }


    public void ShowAnimalAlbumGiveLoot(int amount)
    {
        activeScreen = getRewardScreen;
        getRewardScreen.SetActive(true);
        rubyAmountText.text = amount.ToString();
    }

    public IEnumerator LaunchLevelDisplay()
    {
        TestLevelsSystemManager.instance.numOfSections = GameManager.Instance.currentCluster.clusterLevels.Length;

        SetTestLevelDataDisplayData();

        TestLevelsSystemManager.instance.ResetDisplayMap();

        yield return new WaitForEndOfFrame();

        if (GameManager.Instance.currentLevel.isTimerLevel)
        {
            TestLevelsSystemManager.instance.SetDeactivatedmapData();
        }
        else
        {
            TestLevelsSystemManager.instance.InstantiateBarStarsMapDisplay();
        }


        testLevelsDataScreen.SetActive(true);
    }

    public void SetTestLevelDataDisplayData()
    {
        if(GameManager.Instance.currentLevel.isTimerLevel)
        {
            levelNumText.text = "Boss Level ";
        }
        else
        {
            levelNumText.text = "Level " + GameManager.Instance.currentLevel.levelIndexInZone.ToString();
        }

        levelDifficultyText.text = GameManager.Instance.currentLevel.levelDifficulty.ToString();
        zoneNameText.text = GameManager.Instance.currentLevel.worldName;
        TestLevelsSystemManager.instance.starSliderTestLevelMapDisplay.maxValue = TestLevelsSystemManager.instance.numOfSections;

        TestLevelsSystemManager.instance.UpdateBarValueOnMap();
    }

    //public void CallTypewriterText(DialogueScriptableObject dialogueRef, int index, TMP_Text textRef)
    //{
    //    textCoroutine = StartCoroutine(TypewriterText(dialogueRef, index, textRef));
    //}

    //IEnumerator TypewriterText(DialogueScriptableObject dialogueRef, int index, TMP_Text textRef)
    //{
    //    string fullText = dialogueRef.allEntries[index].conversationBlock;

    //    string currentText = "";
    //    for (int i = 0; i < fullText.Length + 1; i++)
    //    {
    //        currentText = fullText.Substring(0, i);
    //        textRef.text = currentText;
    //        yield return new WaitForSeconds(textSpeed);
    //    }

    //    textCoroutine = null;
    //    dialogueRef.LaunchEndEventsEntry(index);
    //}

    public void CallContinueDialogueSequence()
    {
        if (GameManager.Instance.currentIndexInDialogue == GameManager.Instance.currentDialogue.allEntries.Length)
        {
            Debug.LogError("HEZZE");
        }
        else
        {
            GameManager.Instance.currentDialogue.onClickOnScreen?.Invoke();
        }

    }

    public void CallEndDialogueSequence()
    {
        //Debug.Break();

        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }


        StartCoroutine(GameManager.Instance.currentDialogue.EndAllDialogue());

    }


    public void CallRestartCurrentLevel()
    {
        StartCoroutine(GameManager.Instance.RestartCurrentLevel());

    }

    private void DisableInLevelSettings()
    {
        isUsingUI = false;

        levelSettingsOpen = false;
        inLevelSettings.SetActive(false);

    }

    public void SetMusicOffIcons()
    {
        musicIconButtonLevel.sprite = musicIconOffLevel;

        musicIconButton.sprite = musicIconOff;
    }
    public void SetMusicOnIcons()
    {
        musicIconButtonLevel.sprite = musicIconOnLevel;

        musicIconButton.sprite = musicIconOn;
    }
    public void SetSFXOffIcons()
    {
        SFXIconButtonLevel.sprite = SFXIconOffLevel;

        SFXIconButton.sprite = SFXIconOff;
    }
    public void SetSFXOnIcons()
    {
        SFXIconButtonLevel.sprite = SFXIconOnLevel;

        SFXIconButton.sprite = SFXIconOn;
    }
}
