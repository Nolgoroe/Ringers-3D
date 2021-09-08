using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

[System.Serializable]
public class ButtonsPerZone
{
    public Zone theZone;

    public Button[] zoneButtons;
}
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject mainMenu, hudCanvasDisplay, hudCanvasUI, itemForgeCanvas, gameplayCanvas, gameplayCanvasBotom, gameplayCanvasTop, ringersHutDisplay, ringersHutUICanvas, hollowCraftAndOwned;
    public GameObject InGameUiScreens;
    public GameObject blackLevelBG;
    public GameObject zoomInCorruptedBlack;
    public GameObject tutorialCanvas;
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
    public GameObject UnlockedZoneMessageView;
    public Image dewDropsImage;

    public Image tapControlsImage, dragControlsImage, tutorialDisableImage;

    public Sprite toggleOffSprite, toggleOnSprite;

    public Transform sureLevelRestartLootDislpay;
    public Transform ownedCorruptDevicesZone;

    public Text /*hubGoldText,*/ hubRubyText, dewDropsText;
    public TMP_Text dewDropsTextTime;

    public TMP_Text /*gameplayGoldText,*/ gameplayRubyText/*, gameplayDewDropsText*/;
    public TMP_Text animalNameText;

    public TMP_Text currentLevelWorldName;
    public TMP_Text currentLevelNumber;
    public TMP_Text corruptedZoneSureMessageText;
    public TMP_Text zoneToUnlcokNameText;

    //public Button commitButton;
    public Button nextLevelFromWinScreen;
    public Button restartButton;
    public Button skipAnimationButton;

    public Button dealButton;
    //public Button[] levelButtons;

    public ButtonsPerZone[] buttonsPerZone;
    public InventorySortButtonData[] inventorySortButtons;
    public GameObject[] allTutorialScreens;
    //public Sprite[] dewDropsSprites;

    public Vector3 hubCameraPos;



    public static bool isUsingUI;
    private void Start()
    {
        Instance = this;
        mainMenu.SetActive(true); /// ony screen we should see at the start

        hudCanvasDisplay.SetActive(true);
        hudCanvasUI.SetActive(false);
        hudCanvasUIBottomZoneMainMap.SetActive(true);

        itemForgeCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
        forge.SetActive(false);
        Brewery.SetActive(false);

        itemBag.SetActive(true); //// so this will be the first screen displayed, or else everyone will be turned off

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
        tutorialCanvas.SetActive(false);
        blackLevelBG.SetActive(false);
        zoomInCorruptedBlack.SetActive(false);
        skipAnimationButton.gameObject.SetActive(false);
        InGameUiScreens.SetActive(false);
        corruptedZoneScreen.SetActive(false);
        corruptedZoneSureMessage.SetActive(false);
        hudCanvasUIBottomZoneCorruption.SetActive(false);
        ownedCorruptDevicesZone.gameObject.SetActive(false);
        UnlockedZoneMessageView.gameObject.SetActive(false);


        dragControlsImage.sprite = toggleOnSprite;
        tapControlsImage.sprite = toggleOffSprite;
        tutorialDisableImage.sprite = toggleOnSprite;

        animalNameText.text = "";
        foreach (GameObject go in allTutorialScreens)
        {
            go.SetActive(false);
        }
    }
    public void PlayButton()
    {
        ToHud(mainMenu);
        ZoneManager.Instance.UnlockLevelViewSequence();
        UnlockLevels();
    }
    public void ActivateGmaeplayCanvas()
    {
        gameplayCanvas.SetActive(true);
        hudCanvasDisplay.SetActive(false);
        hudCanvasUI.SetActive(false);
    }
    public void ToMainMenu()
    {
        hudCanvasDisplay.SetActive(false);
        hudCanvasUI.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void DisplayEndLevelMessage()
    {
        endLevelSureMessage.SetActive(true);
        isUsingUI = true;
    }
    public void EndLevelMessageNo()
    {
        endLevelSureMessage.SetActive(false);

        GameManager.Instance.clipManager.RepopulateLatestClip();
        isUsingUI = false;

    }
    public void EndLevelMessageYes()
    {
        isUsingUI = false;

        endLevelSureMessage.SetActive(false);


        DisplayLoseScreen();
    }
    public void DisplayClipsAboutToEndMessage()
    {
        clipsAboutToEndMessage.SetActive(true);
        isUsingUI = true;
    }
    public void ClipsAboutToEndMessageNo()
    {
        isUsingUI = false;

        clipsAboutToEndMessage.SetActive(false);

    }
    public void ClipsAboutToEndMessageYes()
    {
        isUsingUI = false;

        clipsAboutToEndMessage.SetActive(false);
        //LootManager.Instance.currentLevelLootToGive.Clear();
        LootManager.Instance.craftingMatsLootForLevel.Clear();
        DisplayLoseScreen();

    }
    public void SureWantToRestartMessage()
    {
        if (!isUsingUI)
        {
            isUsingUI = true;

            if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
            {
                foreach (GameObject go in TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens)
                {
                    go.SetActive(false);
                }
            }

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

            CMD.materialImage.texture = LootManager.Instance.rubySprite.texture;
            CMD.materialCount.gameObject.SetActive(false);
        }

        foreach (CraftingMats CM in LootManager.Instance.craftingMatsLootForLevel)
        {
            GameObject go = Instantiate(LootManager.Instance.lootDisplayPrefab, sureLevelRestartLootDislpay);

            CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

            CMD.materialImage.texture = Resources.Load(MaterialsAndForgeManager.Instance.materialSpriteByName[CM]) as Texture2D;
            CMD.materialCount.gameObject.SetActive(false);
        }
    }
    public void SureWantToRestartMessageNo(bool withLoot)
    {
        isUsingUI = false;

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
            TutorialSequence.Instacne.TurnOnTutorialScreensAfterOptions();
        }
    }
    public void SureWantToRestartMessageYes(bool withLoot)
    {
        isUsingUI = false;


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
        RefreshGoldAndRubyDisplay();
        PlayerManager.Instance.activePowerups.Clear();
        PlayerManager.Instance.SavePlayerData();
        isUsingUI = false;

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 9f;
        Camera.main.transform.position = hubCameraPos;
        Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);

        if (currentCanvas == gameplayCanvas)
        {
            TutorialSequence.Instacne.currentPhaseInSequence = 0;
            TutorialSequence.Instacne.duringSequence = false;
            GameManager.Instance.copyOfArrayOfPiecesTutorial.Clear();
            ZoneManager.Instance.ActivateLevelDisplay();
            LootManager.Instance.DestoryWinScreenDisplyedLoot();
            GameManager.Instance.powerupManager.DestroySpecialPowersObjects();

            Camera.main.transform.position = hubCameraPos;

            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);
            youWinScreen.SetActive(false);
            //youLoseText.SetActive(false);
            loseScreen.SetActive(false);
            tutorialCanvas.SetActive(false);
            blackLevelBG.SetActive(false);

            foreach (GameObject go in allTutorialScreens)
            {
                go.SetActive(false);
            }

            GameManager.Instance.DestroyAllLevelChildern();

            LootManager.Instance.ResetLevelLootData();
            LootManager.Instance.giveKey = false;
            ZoneManager.Instance.ResetZoneManagerData();
            ConnectionManager.Instance.ResetConnectionData();

            foreach (int ID in ZoneManager.Instance.unlockedZoneID)
            {
                ZoneManagerHelpData.Instance.listOfAllZones[ID].SaveZone();
            }


            ZoneManager.Instance.UnlockLevelViewSequence();
            UnlockLevels();

            //// SAVE ZONE MANAGER HERE???
        }

        if (currentCanvas == ringersHutDisplay)
        {
            ringersHutDisplay.SetActive(false);
            ringersHutUICanvas.SetActive(false);

            ZoneManager.Instance.ActivateLevelDisplay();
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

            hudCanvasUIBottomZoneMainMap.SetActive(true);
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

        hudCanvasDisplay.SetActive(true);
        hudCanvasUI.SetActive(true);
    }
    public void OpenItemsAndForgeZone()
    {
        itemForgeCanvas.SetActive(true);

        isUsingUI = true;

        ChangeInventorySortButtonSprites((int)CraftingMatType.Build);
        //SortMaster.Instance.SortMatInventory(CraftingMatType.Build); //// For now we always open the inventory sorted on gems
    }
    public void OpenHollowCraftAndOwnedZone()
    {
        hollowCraftAndOwned.SetActive(true);
        owned.SetActive(false);
        craft.SetActive(true);

        HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow = false;
        isUsingUI = true;
    }
    public void OpenHollowOwnedObjectsToPlace()
    {
        hollowCraftAndOwned.SetActive(true);
        owned.SetActive(true);
        craft.SetActive(false);
        HollowCraftAndOwnedManager.Instance.isPlaceThroughHollow = true;

        isUsingUI = true;
    }
    public void closeWindow(GameObject ToClose)
    {
        isUsingUI = false;

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
            HollowCraftAndOwnedManager.Instance.hollowTypeToFill = ObjectHollowType.All;
        }
    }
    public void ToForge()
    {
        itemBag.SetActive(false);
        Brewery.SetActive(false);
        forge.SetActive(true);
    }
    public void ToItemsBag()
    {
        itemBag.SetActive(true);
        forge.SetActive(false);
        Brewery.SetActive(false);
    }
    public void ToBrewery()
    {
        itemBag.SetActive(false);
        forge.SetActive(false);
        Brewery.SetActive(true);
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

        SortMaster.Instance.FilterHollowOwnedScreenByEnum(ObjectHollowType.All);
    }
    public void ToRingersHut()
    {
        ringersHutDisplay.SetActive(true);
        ringersHutUICanvas.SetActive(true);
        hudCanvasDisplay.SetActive(false);
        hudCanvasUI.SetActive(false);
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
        blackLevelBG.SetActive(false);
        youWinScreen.SetActive(true);

        AnimalPrefabData prefabData = AnimalsManager.Instance.statueToSwap.GetComponent<AnimalPrefabData>();

        animalNameText.text = "You have released the " + prefabData.animalSO.animalName;
    }
    public void DisplayLoseScreen()
    {
        //gameplayCanvasBotom.SetActive(false);

        ClearLootDisplays();
        isUsingUI = true;

        loseScreen.SetActive(true);
    }
    private void ClearLootDisplays()
    {
        foreach (Transform child in sureLevelRestartLootDislpay)
        {
            Destroy(child.gameObject);
        }
    }
    public void RestartLevelFromLoseScreenUI()
    {
        isUsingUI = false;
        //gameplayCanvasBotom.SetActive(true);

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

            for (int i = 0; i < BPZ.theZone.maxLevelReachedInZone; i++)
            {
                if (i == BPZ.zoneButtons.Length)
                {
                    break;
                }

                BPZ.zoneButtons[i].interactable = true;

                if (i + 1 != BPZ.theZone.maxLevelReachedInZone)
                {
                    BPZ.zoneButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelDonePath);
                }
                else
                {
                    BPZ.zoneButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelFirstTimeIconPath);
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
    public void RefreshGoldAndRubyDisplay()
    {
        //hubGoldText.text = PlayerManager.Instance.goldCount.ToString();
        hubRubyText.text = PlayerManager.Instance.rubyCount.ToString();
        //gameplayGoldText.text = PlayerManager.Instance.goldCount.ToString();
        gameplayRubyText.text = PlayerManager.Instance.rubyCount.ToString();
    }
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
    public void ChangeInventorySortButtonSprites(int buttonID)
    {
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
        blackLevelBG.SetActive(false);
        gameplayCanvasBotom.SetActive(false);
        gameplayCanvasTop.SetActive(false);
        InGameUiScreens.SetActive(false);
    }
    public void TurnOnGameplayUI()
    {
        blackLevelBG.SetActive(true);
        gameplayCanvasBotom.SetActive(true);
        gameplayCanvasTop.SetActive(true);
        InGameUiScreens.SetActive(true);
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
}
