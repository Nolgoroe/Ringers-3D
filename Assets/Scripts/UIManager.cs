using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu, hudCanvas, itemForgeCanvas, gameplayCanvas, ringersHut;
    public GameObject forge, itemBag;
    public GameObject OptionsScreen;
    public GameObject wardrobe;

    public static UIManager instance;

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
            GameManager.Instance.DestroyAllLevelChildern();
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
}
