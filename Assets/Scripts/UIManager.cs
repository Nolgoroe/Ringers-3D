using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class ButtonsPerZone
{
    public Zone theZone;

    public Button[] zoneButtons;
}
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject mainMenu, hudCanvasDisplay,hudCanvasUI, itemForgeCanvas, gameplayCanvas, ringersHutDisplay, ringersHutUICanvas;
    public GameObject forge, itemBag;
    public GameObject OptionsScreen;
    public GameObject wardrobe;
    public GameObject usingPowerupText;
    public GameObject youWinText, youLoseText;

    public Text goldText, rubyText;

    public TMP_Text currentLevelName;

    public Button commitButton;
    //public Button[] levelButtons;

    public ButtonsPerZone[] buttonsPerZone;


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

        itemBag.SetActive(true); //// so this will be the first bag displayed, or else everyone will be turned off

        OptionsScreen.SetActive(false);
        ringersHutDisplay.SetActive(false);
        ringersHutUICanvas.SetActive(false);
        wardrobe.SetActive(false);
        usingPowerupText.SetActive(false);
        youWinText.SetActive(false);
        youLoseText.SetActive(false);


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

    public void ChangeZoneName(string name)
    {
        currentLevelName.text = name;
    }

    public void ToHud(GameObject currentCanvas)
    {
        PlayerManager.Instance.activePowerups.Clear();
        PlayerManager.Instance.SavePlayerData();
        isUsingUI = false;
        if (currentCanvas == gameplayCanvas)
        {
            Camera.main.orthographicSize = 9.5f;
            ZoneManager.Instance.DiactiavteLevelDisplay();

            Camera.main.transform.position = hubCameraPos;

            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);
            youWinText.SetActive(false);
            youLoseText.SetActive(false);

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
            Camera.main.orthographicSize = 9.5f;

            ringersHutDisplay.SetActive(false);
            ringersHutUICanvas.SetActive(false);

            ZoneManager.Instance.DiactiavteLevelDisplay();
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
    }
    public void closeWindow(GameObject ToClose)
    {
        if(ToClose == itemForgeCanvas)
        {
            itemForgeCanvas.SetActive(false);
            forge.SetActive(false);
            itemBag.SetActive(true);
            isUsingUI = false;
        }

        if (ToClose == OptionsScreen)
        {
            OptionsScreen.SetActive(false);
            isUsingUI = false;
        }

        if (ToClose == wardrobe)
        {
            wardrobe.SetActive(false);
            isUsingUI = false;
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
            usingPowerupText.SetActive(true);
        }
        else
        {
            usingPowerupText.SetActive(false);
        }
    }
    public void WinLevel()
    {
        youWinText.SetActive(true);
    }
    public void LoseLevel()
    {
        youLoseText.SetActive(true);
    }
    public void GetCommitButton(GameObject board)
    {
        commitButton = board.GetComponentInChildren<Button>();
        commitButton.onClick.AddListener(GameManager.Instance.CheckEndLevel);
        Debug.Log(commitButton.onClick);
        commitButton.interactable = false;
    }
    public void ActivateCommitButton()
    {
        commitButton.interactable = true;
        commitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 1);
    }
    public void UnlockLevels()
    {
        foreach (int ID in ZoneManager.Instance.unlockedZoneID)
        {
            ButtonsPerZone BPZ = buttonsPerZone.Where(p => p.theZone == ZoneManagerHelpData.Instance.listOfAllZones[ID]).Single();

            for (int i = 0; i < BPZ.theZone.maxLevelReachedInZone; i++)
            {
                BPZ.zoneButtons[i].interactable = true;

                if(i + 1 != BPZ.theZone.maxLevelReachedInZone)
                {
                    BPZ.zoneButtons[i].GetComponent<Image>().sprite = BPZ.theZone.levelDone;
                }
                else
                {
                    BPZ.zoneButtons[i].GetComponent<Image>().sprite = BPZ.theZone.levelFirstTimeIcon;
                }
            }

        }
    }
    public void RefreshGoldAndRubyDisplay()
    {
        goldText.text = PlayerManager.Instance.goldCount.ToString();
        rubyText.text = PlayerManager.Instance.rubyCount.ToString();
    }
}
