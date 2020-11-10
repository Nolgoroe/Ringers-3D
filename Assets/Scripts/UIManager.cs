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

    public Button commitButton;
    //public Button[] levelButtons;

    public ButtonsPerZone[] buttonsPerZone;
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
    public void ToHud(GameObject currentCanvas)
    {
        PlayerManager.Instance.activePowerups.Clear();
        PlayerManager.Instance.SavePlayerData();

        if (currentCanvas == gameplayCanvas)
        {
            //Camera.main.orthographic = true;

            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);
            youWinText.SetActive(false);
            youLoseText.SetActive(false);

            GameManager.Instance.DestroyAllLevelChildern();

            LootManager.Instance.ResetLevelLootData();
            LootManager.Instance.giveGey = false;
            ZoneManager.Instance.ResetZoneManagerData();

            UnlockLevels();

            foreach (Zone z in ZoneManager.Instance.listOfUnlockedZones)
            {
                z.SaveZone();
            }

            ZoneManager.Instance.SaveZoneManager();
        }

        if(currentCanvas == ringersHutDisplay)
        {
            ringersHutDisplay.SetActive(false);
            ringersHutUICanvas.SetActive(false);
            //Camera.main.orthographic = false;
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
    }
    public void closeWindow(GameObject ToClose)
    {
        if(ToClose == itemForgeCanvas)
        {
            itemForgeCanvas.SetActive(false);
            forge.SetActive(false);
            itemBag.SetActive(true);
        }

        if(ToClose == OptionsScreen)
        {
            OptionsScreen.SetActive(false);
        }

        if(ToClose == wardrobe)
        {
            wardrobe.SetActive(false);
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
    }
    public void CloseGame()
    {
        Application.Quit();
    }
    public void OpenWardrobe()
    {
        wardrobe.SetActive(true);
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
        foreach (Zone z in ZoneManager.Instance.listOfUnlockedZones)
        {
            ButtonsPerZone BPZ = buttonsPerZone.Where(p => p.theZone == z).Single();

            for (int i = 0; i < BPZ.theZone.maxLevelReachedInZone; i++)
            {
                BPZ.zoneButtons[i].interactable = true;
                //levelButtons[i].interactable = true;
            }
        }
    }
    public void RefreshGoldAndRubyDisplay()
    {
        goldText.text = PlayerManager.Instance.goldCount.ToString();
        rubyText.text = PlayerManager.Instance.rubyCount.ToString();
    }
}
