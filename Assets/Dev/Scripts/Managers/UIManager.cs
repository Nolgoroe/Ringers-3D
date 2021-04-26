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

    public GameObject mainMenu, hudCanvasDisplay,hudCanvasUI, itemForgeCanvas, gameplayCanvas, gameplayCanvasBotom, gameplayCanvasTop, ringersHutDisplay, ringersHutUICanvas, hollowCraftAndOwned;
    public GameObject forge, itemBag;
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
    public Image dewDropsImage;


    public Transform sureLevelRestartLootDislpay;

    public Text /*hubGoldText,*/ hubRubyText/*, dewDropsText*/, dewDropsTextTime;

    public TMP_Text /*gameplayGoldText,*/ gameplayRubyText, gameplayDewDropsText;
    //public TMP_Text currentLevelName;

    //public Button commitButton;
    public Button nextLevelFromWinScreen;
    public Button dealButton;
    //public Button[] levelButtons;

    public ButtonsPerZone[] buttonsPerZone;
    public InventorySortButtonData[] inventorySortButtons;
    public GameObject[] allTutorialScreens;
    public Sprite[] dewDropsSprites;

    public Vector3 hubCameraPos;

    public static bool isUsingUI;
    private void Start()
    {
        Instance = this;
        mainMenu.SetActive(true); /// ony screen we should see at the start

        hudCanvasDisplay.SetActive(true);
        hudCanvasUI.SetActive(false);

        itemForgeCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
        forge.SetActive(false);

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

        foreach (GameObject go in allTutorialScreens)
        {
            go.SetActive(false);
        }

        RefreshGoldAndRubyDisplay();

    }
    public void PlayButton()
    {
        ToHud(mainMenu);
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
        foreach (GameObject go in TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens)
        {
            go.SetActive(false);
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
    private void DisplayRestartLoot()
    {
        if(LootManager.Instance.rubiesToRecieveInLevel > 0)
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

        TutorialSequence.Instacne.TurnOnTutorialScreensAfterOptions();
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

    //public void ChangeZoneName(string name)
    //{
    //    currentLevelName.text = name;
    //}
    public void ToHud(GameObject currentCanvas)
    {
        isUsingUI = false;

        PlayerManager.Instance.activePowerups.Clear();
        PlayerManager.Instance.SavePlayerData();
        isUsingUI = false;

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 9f;
        Camera.main.transform.position = hubCameraPos;

        if (currentCanvas == gameplayCanvas)
        {
            TutorialSequence.Instacne.currentPhaseInSequence = 0;
            TutorialSequence.Instacne.duringSequence = false;
            GameManager.Instance.copyOfArrayOfPiecesTutorial.Clear();
            ZoneManager.Instance.ActivateLevelDisplay();
            LootManager.Instance.DestoryWinScreenDisplyedLoot();

            Camera.main.transform.position = hubCameraPos;

            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);
            youWinScreen.SetActive(false);
            //youLoseText.SetActive(false);
            loseScreen.SetActive(false);

            GameManager.Instance.DestroyAllLevelChildern();

            LootManager.Instance.ResetLevelLootData();
            LootManager.Instance.giveKey = false;
            ZoneManager.Instance.ResetZoneManagerData();
            ConnectionManager.Instance.ResetConnectionData();
            UnlockLevels();

            foreach (int ID in ZoneManager.Instance.unlockedZoneID)
            {
                ZoneManagerHelpData.Instance.listOfAllZones[ID].SaveZone();
            }

            ZoneManager.Instance.SaveZoneManager();
        }

        if(currentCanvas == ringersHutDisplay)
        {
            ringersHutDisplay.SetActive(false);
            ringersHutUICanvas.SetActive(false);

            ZoneManager.Instance.ActivateLevelDisplay();
        }

        if (currentCanvas == mainMenu)
        {
            mainMenu.SetActive(false);
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

        SortMaster.Instance.SortMatInventory(CraftingMatType.Gem); //// For now we always open the inventory sorted on gems
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
        forge.SetActive(true);
    }
    public void ToItemsBag()
    {
        itemBag.SetActive(true);
        forge.SetActive(false);
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
        OptionsScreen.SetActive(true);
        isUsingUI = true;
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

        youWinScreen.SetActive(true);
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
                if(i == BPZ.zoneButtons.Length)
                {
                    break;
                }

                BPZ.zoneButtons[i].interactable = true;

                if(i + 1 != BPZ.theZone.maxLevelReachedInZone)
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
    public void UnlockZoneFirstTime(int ID)
    {
        ButtonsPerZone BPZ = buttonsPerZone.Where(p => p.theZone == ZoneManagerHelpData.Instance.listOfAllZones[ID]).Single();

        for (int i = 0; i < BPZ.zoneButtons.Length; i++)
        {
            if( i == 0)
            {
                BPZ.zoneButtons[i].interactable = true;
            }

            BPZ.zoneButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(BPZ.theZone.levelFirstTimeIconPath);
        }
    }
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
            AnimalAlbumManager.Instance.pageNumInspector =0;
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
                ISBD.transformImage.sprite = ISBD.notSelectedSprite;
            }
            else
            {
                ISBD.transformImage.sprite = ISBD.selectedSprite;
            }
        }

        SortMaster.Instance.SortMatInventory((CraftingMatType)buttonID);
    }
    public void TurnOffGameplayUI()
    {
        gameplayCanvasBotom.SetActive(false);
        gameplayCanvasTop.SetActive(false);
    }
    public void TurnOnGameplayUI()
    {
        gameplayCanvasBotom.SetActive(true);
        gameplayCanvasTop.SetActive(true);
    }

    public void RefreshDewDropsDisplay(int spriteIndex)
    {
        dewDropsImage.sprite = dewDropsSprites[spriteIndex];
    }
}
