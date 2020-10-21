using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu, hudCanvas, itemForgeCanvas, gameplayCanvas, ringersHut;
    public GameObject forge, itemBag;
    public GameObject OptionsScreen;
    public GameObject wardrobe;
    public GameObject usingPowerupText;
    public GameObject youWinText, youLoseText;

    public static UIManager instance;

    public Button commitButton;
    public Button[] levelButtons;
    private void Start()
    {
        instance = this;
        mainMenu.SetActive(true);
        hudCanvas.SetActive(false);
        itemForgeCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
        forge.SetActive(false);
        itemBag.SetActive(true);
        OptionsScreen.SetActive(false);
        ringersHut.SetActive(false);
        wardrobe.SetActive(false);
        usingPowerupText.SetActive(false);
        youWinText.SetActive(false);
        youLoseText.SetActive(false);


        UnlockLevels();
    }

    public void PlayButton()
    {
        mainMenu.SetActive(false);
        hudCanvas.SetActive(true);
    }
    public void ActivateGmaeplayCanvas()
    {
        gameplayCanvas.SetActive(true);
        hudCanvas.SetActive(false);
    }
    public void ToMainMenu()
    {
        hudCanvas.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void ToHud(GameObject currentCanvas)
    {
        if(currentCanvas == gameplayCanvas)
        {
            gameplayCanvas.SetActive(false);
            OptionsScreen.SetActive(false);
            youWinText.SetActive(false);
            youLoseText.SetActive(false);

            GameManager.Instance.DestroyAllLevelChildern();
            UnlockLevels();
        }

        if(currentCanvas == ringersHut)
        {
            ringersHut.SetActive(false);
        }

        hudCanvas.SetActive(true);
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
        ringersHut.SetActive(true);
        hudCanvas.SetActive(false);
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
        for (int i = 0; i < GameManager.Instance.maxLevel; i++)
        {
            levelButtons[i].interactable = true;
        }
    }
}
