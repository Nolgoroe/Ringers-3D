using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLevelsSystemManager : MonoBehaviour
{
    public static TestLevelsSystemManager instance;

    public int numOfSections;
    public GameObject starPrefab;

    public Transform starsParent;

    public Slider StarSlider;

    public float barAnimateSpeed;


    public Slider starSliderTestLevelMapDisplay;
    public Transform starsParentMapDisplay;

    void Start()
    {
        instance = this;


        InstantiateBarStarsMapDisplay();
    }

    public void InitTestLevel()
    {
        ShowRelaventUI();
        InstantiateBarStars();

        StarSlider.maxValue = numOfSections;
    }

    private void ShowRelaventUI()
    {
        ResetDisplay();

        UIManager.Instance.gameplayCanvasTop.SetActive(false);
        UIManager.Instance.gameplayCanvasTopTestLevels.SetActive(true);
    }

    private void InstantiateBarStars()
    {
        if(numOfSections > 1)
        {
            //we start from 1 since the "chest" is already considerd a "section"
            for (int i = 1; i < numOfSections; i++)
            {
                Instantiate(starPrefab, starsParent);
            }
        }
    }
    private void InstantiateBarStarsMapDisplay()
    {
        starSliderTestLevelMapDisplay.maxValue = numOfSections;

        if (numOfSections > 1)
        {
            //we start from 1 since the "chest" is already considerd a "section"
            for (int i = 1; i < numOfSections; i++)
            {
                Instantiate(starPrefab, starsParentMapDisplay);
            }
        }
    }

    public void UpdateBarValue()
    {
        LeanTween.value(StarSlider.gameObject, StarSlider.value, StarSlider.value + 1, barAnimateSpeed).setOnComplete(() => ActivateStarOrChest((int)StarSlider.value)).setOnUpdate((float val) =>
        {
            StarSlider.value = val;
        });

    }
    public void UpdateBarValueOnMap()
    {
        starSliderTestLevelMapDisplay.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
    }

    private void ActivateStarOrChest(int index)
    {
        if (index >= numOfSections)
        {
            Debug.LogError("Giving Chest");

            //animate chest here
            LootManager.Instance.UnpackChestLoot();

            TestLevelsSystemManagerSaveData.instance.ResetData();
            UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
            return;
        }

        if (starsParent.childCount > 0)
        {
            // animate stars here
            Debug.LogError("Activated Star: " + starsParent.GetChild(index - 1).name);
        }
    }

    public void ResetDisplay()
    {
        StarSlider.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
        for (int i = 0; i < starsParent.childCount; i++)
        {
            Destroy(starsParent.GetChild(i).gameObject);
        }
    }


    //called by button
    public void LaunchTestLevel()
    {
        UIManager.Instance.testLevelsDataScreen.SetActive(false);

        GameManager.Instance.levelStarted = true;

        SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

        GameManager.Instance.StartLevel(true, false);
    }
}
