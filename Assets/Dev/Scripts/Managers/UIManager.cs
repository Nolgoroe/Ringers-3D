using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using GameAnalyticsSDK;

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

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject mainMenu, worldGameObject, hudCanvasUI, itemForgeCanvas, gameplayCanvas, gameplayCanvasBotom, gameplayCanvasTop, ringersHutDisplay, ringersHutUICanvas, hollowCraftAndOwned;
    public GameObject InGameUiScreens;
    //public GameObject blackLevelBG;
    public GameObject zoomInCorruptedBlack;
    public GameObject tutorialCanvasLevels;
    public GameObject tutorialCanvasSpecific;
    public GameObject tutorialCanvasParent;
    //public GameObject tutorialCanvasHolesParent;
    public GameObject forge, itemBag, Brewery;
    public GameObject craft, owned;
    public GameObject animalAlbum;
    public GameObject OptionsScreen;
    public GameObject wardrobe;
    public GameObject usingPowerupText;
    public GameObject youWinScreen, loseScreen/*, youLoseText*/;
    public GameObject endLevelSureMessage;
    public GameObject clipsAboutToEndMessage;
    public GameObject sureWantToRestartWithLoot;
    public GameObject sureWantToRestartNoLoot;
    public GameObject corruptedZoneScreen;
    public GameObject corruptedZoneSureMessage;
    public GameObject hudCanvasUIBottomZoneMainMap;
    public GameObject hudCanvasUIBottomZoneCorruption;
    public GameObject hudCanvasUIBottomZoneDenScreen;
    public GameObject UnlockedZoneMessageView;
    public GameObject dealButtonHeighlight;
    public GameObject openInventoryButtonHeighlight;
    public GameObject potionTabHeighlight;
    public GameObject toHubButtonHeighlight;
    public GameObject brewButtonHeighlight;
    public GameObject[] brewMaterialZonesHeighlights;
    public GameObject normalBookBG, potionsBookBG;
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
    public GameObject fadeIntoLevel;
    public GameObject introScreenParent;
    public GameObject TEMPBgIntro;
    public GameObject bossBattleUIScreen;
    public GameObject placePieceVFX;
    public GameObject dealButtonVFX;

    public Image dewDropsImage;

    public Image tapControlsImage, dragControlsImage, tutorialDisableImage;
    public Image matsInventoryButton, forgeInventoryButton, potionInventoryButton;

    public float fadeIntoLevelSpeed;
    public float fadeIntoLevelDelay;

    public float speedFadeInIntro;
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

    public TMP_Text currentLevelWorldName;
    public TMP_Text currentLevelNumber;
    public TMP_Text corruptedZoneSureMessageText;
    public TMP_Text zoneToUnlcokNameText;
    public TMP_Text versionText;
    public TMP_Text buyPotionRubieCoseText;
    public TMP_Text buyHollowItemRubieCostText;
    public TMP_Text buyHollowItemSecondaryRubieCostText;
    //public TMP_Text cantBuyPotionText;

    //public Button commitButton;
    public Button nextLevelFromWinScreen;
    public Button restartGrindLevel;
    public Button restartButton;
    public Button skipAnimationButton;
    public Button getDailyLootButton;
    public Button dealButton;
    public Button backToHubButton;
    public Button buyPotionYesButton;
    public Button buyHollowItemYesButton;
    public Button buyHollowItemSecondaryYesButton;

    //public Button[] levelButtons;
    public Slider bossHealthSlider;

    public ButtonsPerZone[] buttonsPerZone;
    public InventorySortButtonData[] inventorySortButtons;
    public GameObject[] allTutorialScreens;
    public ImageTextCombo[] introImages;
    //public Sprite[] dewDropsSprites;

    public Vector3 hubCameraPos;
    public Vector3 hubCameraRot;
    public Vector3 denCameraPos;
    public Vector3 denCameraRot;


    //public Color gameTextColor;
    public static bool isUsingUI;
    public static bool isDuringIntro;
    public static bool canAdvanceIntro;




    PanZoom PZ;

    int introImageIndex = 0;

    private void Start()
    {
        Instance = this;

        isDuringIntro = false;
        canAdvanceIntro = true;

        mainMenu.SetActive(true); /// ony screen we should see at the start

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
        fadeIntoLevel.SetActive(false);
        introScreenParent.SetActive(false);
        bossBattleUIScreen.SetActive(false);

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
        Debug.Log("IS USING UI? " + isUsingUI);
        width.text = "Width: " + Display.main.systemWidth;
        height.text = "Height: " + Display.main.systemHeight;

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
        
    }
    public void PlayButton()
    {
        ToHud(mainMenu);
        //ZoneManager.Instance.UnlockLevelViewSequence();
        UnlockLevels();

        GameManager.Instance.clickedPlayButton = true;
    }
    public void ActivateGmaeplayCanvas()
    {
        gameplayCanvas.SetActive(true);
        worldGameObject.SetActive(false);
        hudCanvasUI.SetActive(false);
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
        isUsingUI = true;
    }
    public void EndLevelMessageNo()
    {
        endLevelSureMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);

        GameManager.Instance.clipManager.RepopulateLatestClip();

        isUsingUI = false;

    }
    public void EndLevelMessageYes()
    {
        isUsingUI = false;

        endLevelSureMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);


        DisplayLoseScreen();
    }
    public void DisplayClipsAboutToEndMessage()
    {
        clipsAboutToEndMessage.SetActive(true);
        bGPanelDisableTouch.SetActive(true);

        isUsingUI = true;
    }
    public void ClipsAboutToEndMessageNo()
    {
        isUsingUI = false;

        clipsAboutToEndMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);

    }
    public void ClipsAboutToEndMessageYes()
    {
        isUsingUI = false;

        clipsAboutToEndMessage.SetActive(false);
        bGPanelDisableTouch.SetActive(false);
        //LootManager.Instance.currentLevelLootToGive.Clear();
        LootManager.Instance.craftingMatsLootForLevel.Clear();
        LootManager.Instance.tempDataList.Clear();
        DisplayLoseScreen();

    }
    public void SureWantToRestartMessage()
    {
        if (!isUsingUI)
        {
            bGPanelDisableTouch.SetActive(true);

            isUsingUI = true;

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
        isUsingUI = false;
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

        if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
        {
            TutorialSequence.Instacne.TurnOnTutorialScreensAfterRestart();
        }
    }
    public void SureWantToRestartMessageYes(bool withLoot)
    {
        isUsingUI = false;

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

        GameManager.Instance.RestartCurrentLevel();
    }
    public void ChangeZoneName(string name, int levelID)
    {
        currentLevelWorldName.text = name;

        currentLevelNumber.text = levelID.ToString();
    }
    public void ToHud(GameObject currentCanvas)
    {
        isUsingUI = false;

        updateRubyAndDewDropsCount();
        PlayerManager.Instance.activePowerups.Clear();
        //PlayerManager.Instance.SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();

        //Camera.main.orthographic = true;
        //GameManager.Instance.secondCam.orthographic = true;
        //Camera.main.orthographicSize = 9f;
        //GameManager.Instance.secondCam.orthographicSize = 9f;

        Camera.main.transform.rotation = Quaternion.Euler(hubCameraRot);

        PZ.SetFieldOfView();
        hudCanvasUIBottomZoneMainMap.SetActive(true);

        SortMaster.Instance.RefreshAllForgeScreens();

        if (ZoneManagerHelpData.Instance.currentZoneCheck)
        {
            Vector3 currentZoneTransform = ZoneManagerHelpData.Instance.currentZoneCheck.transform.position;
            
            Vector3 tempForClamp = new Vector3(0f, currentZoneTransform.y, -3f);
            PanZoom pz = Camera.main.GetComponent<PanZoom>();
            tempForClamp.y = Mathf.Clamp(tempForClamp.y, pz.bottomBound, pz.topBound);

            Camera.main.transform.position = tempForClamp;

            TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, tempForClamp.y, TutorialSequence.Instacne.maskImage.transform.position.z);
        }
        else
        {
            Camera.main.transform.position = hubCameraPos;
            TutorialSequence.Instacne.maskImage.transform.position = new Vector3(TutorialSequence.Instacne.maskImage.transform.position.x, hubCameraPos.y, TutorialSequence.Instacne.maskImage.transform.position.z);
        }


        if (currentCanvas == gameplayCanvas)
        {
            if (GameManager.Instance.currentLevel.isTutorial || GameManager.Instance.currentLevel.isSpecificTutorial)
            {
                TutorialSequence.Instacne.currentPhaseInSequenceLevels = 0;
                TutorialSequence.Instacne.currentPhaseInSequenceSpecific = 0;
                TutorialSequence.Instacne.DeactivateAllTutorialScreens();


                GameManager.Instance.powerupManager.ClearTutorialPowerups();
            }

            GameManager.Instance.copyOfArrayOfPiecesTutorial.Clear();
            ZoneManager.Instance.ActivateLevelDisplay();
            LootManager.Instance.DestoryWinScreenDisplyedLoot();
            GameManager.Instance.powerupManager.ResetData();
            GameManager.Instance.powerupManager.DestroySpecialPowersObjects();
            LightingSettingsManager.instance.ResetLightData();

            restartGrindLevel.gameObject.SetActive(false);

            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);
            youWinScreen.SetActive(false);
            bGPanelDisableTouch.SetActive(false);

            //youLoseText.SetActive(false);
            loseScreen.SetActive(false);
            tutorialCanvasLevels.SetActive(false);
            //blackLevelBG.SetActive(false);
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
                go.SetActive(false);
            }

            GameManager.Instance.DestroyAllLevelChildern();

            LootManager.Instance.ResetLevelLootData();
            LootManager.Instance.giveKey = false;
            ZoneManager.Instance.ResetZoneManagerData();
            ConnectionManager.Instance.ResetConnectionData();

            //foreach (int ID in ZoneManager.Instance.unlockedZoneID)
            //{
            //    ZoneManagerHelpData.Instance.listOfAllZones[ID].SaveZone();
            //}


            ZoneManager.Instance.UnlockLevelViewSequence();
            UnlockLevels();


            if (TutorialSequence.Instacne.duringSequence)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
                {
                    //gameplayCanvasScreensUIHEIGHLIGHTS.SetActive(false);
                    //HudCanvasUIHEIGHLIGHTS.SetActive(true);
                    isUsingUI = true;

                    TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                }
                else
                {
                    TutorialSequence.Instacne.activatedHeighlights.Clear();
                    TutorialSequence.Instacne.activatedBoardParticles.Clear();

                    BossBattleManager.instance.ResetData();

                    if (GameManager.Instance.currentLevel.isBoss)
                    {
                        DeactivateBossBattleUIScreen();
                        BossBattleManager.instance.bossLevelSO = null;
                    }

                    GameManager.Instance.currentLevel = null;

                    TutorialSequence.Instacne.duringSequence = false;
                }
            }
            else
            {
                BossBattleManager.instance.ResetData();

                if (GameManager.Instance.currentLevel.isBoss)
                {
                    DeactivateBossBattleUIScreen();
                    BossBattleManager.instance.bossLevelSO = null;
                }

                GameManager.Instance.currentLevel = null;
            }
            //// SAVE ZONE MANAGER HERE???
        }

        if (currentCanvas == ringersHutDisplay)
        {
            ringersHutDisplay.SetActive(false);
            ringersHutUICanvas.SetActive(false);
            hudCanvasUIBottomZoneDenScreen.SetActive(false);

            ZoneManager.Instance.ActivateLevelDisplay();
            PZ.isInDenScreen = false;
        }

        if (currentCanvas == mainMenu)
        {
            mainMenu.SetActive(false);
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
            SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

            itemForgeCanvas.SetActive(true);

            isUsingUI = true;

            openInventoryTab();

            matsInventoryButton.enabled = false;
            forgeInventoryButton.enabled = true;
            potionInventoryButton.enabled = true;

            normalBookBG.SetActive(true);
            potionsBookBG.SetActive(false);

            if (TutorialSequence.Instacne.duringSequence)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
                {
                    //HudCanvasUIHEIGHLIGHTS.SetActive(false);
                    //ItemAndForgeBagHEIGHLIGHTS.SetActive(true);

                    TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                }
            }
        }

        //SortMaster.Instance.SortMatInventory(CraftingMatType.Build); //// For now we always open the inventory sorted on gems
    }
    public void OpenHollowCraftAndOwnedZone()
    {
        hollowCraftAndOwned.SetActive(true);
        owned.SetActive(false);
        craft.SetActive(true);

        //HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow = false;
        isUsingUI = true;
    }
    public void OpenHollowOwnedObjectsToPlace()
    {
        hollowCraftAndOwned.SetActive(true);
        owned.SetActive(true);
        craft.SetActive(false);
        //HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow = true;

        isUsingUI = true;
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

        isUsingUI = false;
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        if (ToClose == itemForgeCanvas)
        {
            itemForgeCanvas.SetActive(false);
            forge.SetActive(false);
            Brewery.SetActive(false);

            itemBag.SetActive(true);

        }

        if (ToClose == OptionsScreen)
        {
            OptionsScreen.SetActive(false);
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
            //HollowCraftAndOwnedManager.Instance.hollowTypeToFill = ObjectHollowType.All;
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
        normalBookBG.SetActive(true);
        potionsBookBG.SetActive(false);
    }
    public void ToItemsBag()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
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
        potionsBookBG.SetActive(false);

    }
    public void ToBrewery()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);
        SoundManager.Instance.PlaySound(Sounds.PageFlip);

        itemBag.SetActive(false);
        forge.SetActive(false);
        Brewery.SetActive(true);

        matsInventoryButton.enabled = true;
        forgeInventoryButton.enabled = true;
        potionInventoryButton.enabled = false;

        normalBookBG.SetActive(false);
        potionsBookBG.SetActive(true);


        Brewery.GetComponent<BreweryDisplayLogic>().GetAllAnchorPositions();
        Brewery.GetComponent<BreweryDisplayLogic>().SetSelectedPotion(MaterialsAndForgeManager.Instance.equipmentInBrewScreen[0]);

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
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
        if (isUsingUI)
        {
            return;
        }

        if (returningToHud)
        {
            ToHud(ringersHutDisplay);
            return;
        }

        //StartCoroutine(HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame));
       HollowCraftAndOwnedManager.Instance.FillHollowScreenCraft(GameManager.Instance.csvParser.allHollowCraftObjectsInGame);

        ringersHutDisplay.SetActive(true);
        ringersHutUICanvas.SetActive(true);
        hudCanvasUIBottomZoneDenScreen.SetActive(true);

        worldGameObject.SetActive(false);
        hudCanvasUIBottomZoneMainMap.SetActive(false);
        //hudCanvasUI.SetActive(false);

        Camera.main.transform.position = denCameraPos;
        Camera.main.transform.rotation = Quaternion.Euler(denCameraRot);

        LightingSettingsManager.instance.SetdenLight();

        PZ.isInDenScreen = true;

        HollowCraftAndOwnedManager.Instance.RefreshOwnedScreen();

    }
    public void OpenOptions()
    {
        if (!isUsingUI)
        {
            OptionsScreen.SetActive(true);
            isUsingUI = true;
        }
    }
    public void CloseGame()
    {
        DateTime timeToSave = PlayfabManager.instance.currentTimeReference.Add(TimeReferenceDataScript.GetTimeElapsed());



        Debug.Log(timeToSave + "Pause time!");

        RewardsManager.Instance.UpdateQuitTime(timeToSave);
        DewDropsManager.Instance.UpdateQuitTime(timeToSave);

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ALL });

        Debug.Log("Saved all data! - close game");

        Application.Quit();
    }
    public void OpenWardrobe()
    {
        wardrobe.SetActive(true);

        isUsingUI = true;
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
        youWinScreen.SetActive(true);

        //AnimalPrefabData prefabData = AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();

        animalNameText.text = "You have released the " + AnimalsManager.Instance.currentLevelAnimal;
    }
    public void DisplayLoseScreen()
    {
        //gameplayCanvasBotom.SetActive(false);

        ClearLootDisplays();
        isUsingUI = true;
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
        isUsingUI = false;
        //gameplayCanvasBotom.SetActive(true);
        bGPanelDisableTouch.SetActive(false);

        loseScreen.SetActive(false);

        TurnOnGameplayUI();
    }
    //public void GetCommitButton(GameObject board)
    //{
    //    commitButton = board.GetComponentInChildren<Button>();
    //    commitButton.onClick.AddListener(GameManager.Instance.CheckEndLevel);
    //    Debug.Log(commitButton.onClick);
    //    commitButton.interactable = false;
    //}

    //public void ActivateCommitButton()
    //{
    //    commitButton.interactable = true;
    //    commitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 1);
    //}

    //public void DisableCommitButton()
    //{
    //    commitButton.gameObject.SetActive(false);
    //}
    public void UnlockLevels()
    {
        foreach (int ID in ZoneManager.Instance.unlockedZoneID)
        {
            ButtonsPerZone BPZ = buttonsPerZone.Where(p => p.theZone == ZoneManagerHelpData.Instance.listOfAllZones[ID]).Single();

            if (BPZ.theZone.hasUnlockedGrind)
            {
                //BPZ.theZone.zoneGrindLevel.GetComponent<Renderer>().material.SetColor("_BaseColor", BPZ.theZone.levelFirstTimeColor); // 3D map

                BPZ.theZone.zoneGrindLevel.GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelFirstTimeIconPath);
                //BPZ.theZone.zoneGrindLevel.GetComponent<Button>().interactable = true;
            }

            for (int i = 0; i < BPZ.theZone.maxLevelReachedInZone; i++)
            {
                if (i == BPZ.zone3DButtons.Length)
                {
                    break;
                }

                if (i + 1 != BPZ.theZone.maxLevelReachedInZone)
                {
                    //BPZ.zone3DButtons[i].GetComponent<Renderer>().material.SetColor("_BaseColor", BPZ.theZone.levelDoneColor); //3D map

                    BPZ.zone3DButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelDonePath);
                    //BPZ.zone3DButtons[i].GetComponent<Renderer>().material.color = BPZ.theZone.levelDoneColor;
                    //BPZ.zoneButtons[i].interactable = false; //// Disable levels that have already been completed
                    //BPZ.zone3DButtons[i].interactable = true; /// temp for testing - isn't button anymore
                }
                else
                {
                    //BPZ.zone3DButtons[i].GetComponent<Renderer>().material.SetColor("_BaseColor", BPZ.theZone.levelFirstTimeColor); 3D map

                    BPZ.zone3DButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelFirstTimeIconPath);
                    //BPZ.zone3DButtons[i].interactable = true; // isn't button anymore
                }

            }
        }
    }
    //public void UnlockZoneFirstTime(int ID)
    //{
    //    ButtonsPerZone BPZ = buttonsPerZone.Where(p => p.theZone == ZoneManagerHelpData.Instance.listOfAllZones[ID]).Single();

    //    for (int i = 0; i < BPZ.zoneButtons.Length; i++)
    //    {
    //        if( i == 0)
    //        {
    //            BPZ.zoneButtons[i].interactable = true;
    //        }

    //        BPZ.zoneButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelFirstTimeIconPath);
    //    }
    //}
    //public void RefreshGoldAndRubyDisplay()
    //{
    //    //hubGoldText.text = PlayerManager.Instance.goldCount.ToString();
    //    hubRubyText.text = PlayerManager.Instance.rubyCount.ToString();
    //    //gameplayGoldText.text = PlayerManager.Instance.goldCount.ToString();
    //    gameplayRubyText.text = PlayerManager.Instance.rubyCount.ToString();
    //}
    public void ToggleAnimalAlbum(bool Open)
    {
        if (Open)
        {
            animalAlbum.SetActive(true);
            AnimalAlbumManager.Instance.pageNumInspector = 0;
            AnimalAlbumManager.Instance.ChangePageLogic(0);
        }
        else
        {
            animalAlbum.SetActive(false);
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

    public void ChangeInventorySortButtonSprites(int buttonID)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                return;
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
    public void TurnOffGameplayUI()
    {
        //blackLevelBG.SetActive(false);
        gameplayCanvasBotom.SetActive(false);
        gameplayCanvasTop.SetActive(false);
        InGameUiScreens.SetActive(false);
    }
    public void TurnOnGameplayUI()
    {
        //blackLevelBG.SetActive(true);
        gameplayCanvasBotom.SetActive(true);
        gameplayCanvasTop.SetActive(true);
        InGameUiScreens.SetActive(true);
        dealButton.interactable = true;
    }
    //public void RefreshDewDropsDisplay(int spriteIndex)
    //{
    //    dewDropsImage.sprite = dewDropsSprites[spriteIndex];
    //}
    public void ChangeControlsTap()
    {
        GameManager.Instance.isSecondaryControls = true;
        tapControlsImage.sprite = toggleOnSprite;
        dragControlsImage.sprite = toggleOffSprite;
    }
    public void ChangeControlsDrag()
    {
        GameManager.Instance.isSecondaryControls = false;
        dragControlsImage.sprite = toggleOnSprite;
        tapControlsImage.sprite = toggleOffSprite;
    }

    public void DisableTutorials()
    {
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
        if (!isUsingUI)
        {
            isUsingUI = true;

            PlayfabManager.instance.GetLeaderboard();
        }
    }


    public void SureWantToResetDataMessage()
    {
        // no need to check if using UI since we can only get in here if the options menu is open so UI is used
        sureWantToResetDataScreen.SetActive(true);
    }

    public void SureWantToResetDataYes()
    {
        isUsingUI = false;

        sureWantToResetDataScreen.SetActive(false);

        PlayfabManager.instance.ResetAllData();
    }

    public void SureWantToResetDataNo()
    {
        //isUsingUI = false;

        sureWantToResetDataScreen.SetActive(false);
    }

    public void SureWantToLogOutMessage()
    {
        // no need to check if using UI since we can only get in here if the options menu is open so UI is used
        sureWantToLogOutScreen.SetActive(true);
    }

    public void SureWantToLogOutYes()
    {
        isUsingUI = false;

        sureWantToLogOutScreen.SetActive(false);

        PlayfabManager.instance.LogOut();
    }

    public void SureWantToLogOutNo()
    {
        //isUsingUI = false;

        sureWantToLogOutScreen.SetActive(false);
    }

    public void DisplayDailyRewardsScreen()
    {
        isUsingUI = true;
        DailyRewardScreen.SetActive(true);
    }

    public void DeactivateDailyRewardScreen()
    {
        isUsingUI = false;

        DailyRewardScreen.SetActive(false);
    }


    //public void DisplayCantBuyPotionScreen()
    //{
    //    cantBuyPotionCraftScreen.SetActive(true);
    //}

    //public void DisplayCantBuyPotionText(int amount)
    //{
    //    cantBuyPotionText.text = "Rubies needed: " + PlayerManager.Instance.rubyCount + "/" + amount;
    //}

    //public void CantBuyPotionScreenOK()
    //{
    //    cantBuyPotionCraftScreen.SetActive(false);
    //}

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
        isUsingUI = true;
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
        MissingMaterialsHollowObjectScreen.SetActive(false);
        ClearBuyHollowSecondaryDisplay();
        isUsingUI = false;
    }

    public void BuyHollowItemSecondaryScreenNo()
    {
        MissingMaterialsHollowObjectScreen.SetActive(false);
        ClearBuyHollowSecondaryDisplay();
        isUsingUI = false;
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
            LeanTween.value(fadeIntoLevel, 0, 1, fadeIntoLevelSpeed).setEase(LeanTweenType.linear).setOnComplete(() => GameManager.Instance.ResetDataStartLevel(isTutorialLevel)).setOnUpdate((float val) =>
            {
                Image sr = fadeIntoLevel.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }
        else
        {
            LeanTween.value(fadeIntoLevel, 0, 1, fadeIntoLevelSpeed).setEase(LeanTweenType.linear).setOnComplete(() => GameManager.Instance.ResetDataStartBossLevel()).setOnUpdate((float val) =>
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


    public IEnumerator DisplayIntro()
    {
        isDuringIntro = true;
        canAdvanceIntro = false;

        introScreenParent.SetActive(true);

        LeanTween.value(TEMPBgIntro, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = TEMPBgIntro.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        LeanTween.value(introImages[0].imageObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = introImages[0].imageObject.GetComponent<Image>();
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });


        foreach (GameObject go in introImages[0].textObjects)
        {
            LeanTween.value(go, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                TMP_Text sr = go.GetComponent<TMP_Text>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }

        yield return new WaitForSeconds(speedFadeInIntro + 0.1f);

        canAdvanceIntro = true;
    }


    public IEnumerator AdvanceIntroScreen()
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

        foreach (GameObject go in introImages[introImageIndex].textObjects)
        {
            LeanTween.value(go, 1, 0, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                TMP_Text sr = go.GetComponent<TMP_Text>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });

        }


        yield return new WaitForSeconds(speedFadeOutIntro + 0.1f);
        introImageIndex++;

        if (introImageIndex >= introImages.Count())
        {
            isDuringIntro = false;
            canAdvanceIntro = false;

            TutorialSaveData.Instance.hasFinishedIntro = true;
            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

            PlayButton();
            DisplayDailyRewardsScreen();

            LeanTween.value(TEMPBgIntro, 1, 0, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
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
            LeanTween.value(introImages[introImageIndex].imageObject, 0, 1, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                Image sr = introImages[introImageIndex].imageObject.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });

            foreach (GameObject go in introImages[introImageIndex].textObjects)
            {
                LeanTween.value(go, 0, 1, speedFadeOutIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
                {
                    TMP_Text sr = go.GetComponent<TMP_Text>();
                    Color newColor = sr.color;
                    newColor.a = val;
                    sr.color = newColor;
                });

            }
        }

        yield return new WaitForSeconds(speedFadeInIntro + 0.1f);
        canAdvanceIntro = true;
    }


    public void SkipIntroScreens()
    {
        isDuringIntro = false;
        canAdvanceIntro = false;

        TutorialSaveData.Instance.hasFinishedIntro = true;
        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

        PlayButton();
        DisplayDailyRewardsScreen();

        StartCoroutine(SkipIntroFade());
    }

    public IEnumerator SkipIntroFade()
    {
        if (introImageIndex != introImages.Count() - 1)
        {
            LeanTween.value(TEMPBgIntro, 1, 0, speedFadeInIntro).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                Image sr = TEMPBgIntro.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }

        yield return new WaitForSeconds(speedFadeOutIntro + 0.1f);

        introScreenParent.SetActive(false);

    }


    public void DisplayBossBattleUIScreen()
    {
        bossBattleUIScreen.SetActive(true);

        bossHealthSlider.value = (float)BossBattleManager.instance.currentBossHealth / (float)BossBattleManager.instance.bossLevelSO.BossHealth;
    }

    public void DeactivateBossBattleUIScreen()
    {
        bossBattleUIScreen.SetActive(false);
    }

    public void UpdateBossHealth()
    {
        bossHealthSlider.value = (float)BossBattleManager.instance.currentBossHealth / (float)BossBattleManager.instance.bossLevelSO.BossHealth;
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
}
