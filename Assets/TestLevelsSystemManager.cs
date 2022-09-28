using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLevelsSystemManager : MonoBehaviour
{
    public static TestLevelsSystemManager instance;

    public int numOfSections;
    public float barWidth;
    public GameObject starPrefab;

    public Transform starsParent;

    public Slider StarSlider;

    public float barAnimateSpeed;


    public Slider starSliderTestLevelMapDisplay;
    public Transform starsParentMapDisplay;

    void Start()
    {
        instance = this;


    }

    public IEnumerator InitTestLevel()
    {
        StarSlider.maxValue = numOfSections;

        ShowRelaventUI();
        yield return new WaitForEndOfFrame();
        InstantiateBarStars();

        ActivateStarOrChestLevel(TestLevelsSystemManagerSaveData.instance.CompletedCount);
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
            float amout = barWidth / (numOfSections);

            //we start from 1 since the "chest" is already considerd a "section"
            for (int i = 1; i < numOfSections; i++)
            {
                GameObject go = Instantiate(starPrefab, starsParent);
                RectTransform starRect = go.GetComponent<RectTransform>();
                starRect.anchoredPosition = new Vector2(amout * i, 0);

                ImageSwapHelper swapHelper = go.GetComponent<ImageSwapHelper>();

                swapHelper.SetDeActivatedChild();
            }
        }
    }
    public void InstantiateBarStarsMapDisplay()
    {
        if (numOfSections > 1)
        {
            float amout = barWidth / (numOfSections );

            //we start from 1 since the "chest" is already considerd a "section"
            for (int i = 1; i < numOfSections; i++)
            {
                GameObject go = Instantiate(starPrefab, starsParentMapDisplay);
                RectTransform starRect = go.GetComponent<RectTransform>();
                starRect.anchoredPosition = new Vector2(amout * i, 0);

                ImageSwapHelper swapHelper = go.GetComponent<ImageSwapHelper>();

                swapHelper.SetDeActivatedChild();
            }
        }

        ActivateStarOrChestMap();
    }

    public void UpdateBarValue()
    {
        LeanTween.value(StarSlider.gameObject, StarSlider.value, StarSlider.value + 1, barAnimateSpeed).setOnComplete(() => ActivateStarOrChestLevel((int)StarSlider.value)).setOnUpdate((float val) =>
        {
            StarSlider.value = val;
        });

    }
    public void UpdateBarValueOnMap()
    {
        starSliderTestLevelMapDisplay.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
    }

    private void ActivateStarOrChestLevel(int index)
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

        for (int i = 0; i < TestLevelsSystemManagerSaveData.instance.CompletedCount; i++)
        {
            if (starsParent.childCount > 0)
            {
                ImageSwapHelper swapHelper = starsParent.GetChild(i).GetComponent<ImageSwapHelper>();

                swapHelper.SetActivatedChild();
            }
        }

    }
    private void ActivateStarOrChestMap()
    {
        for (int i = 0; i < TestLevelsSystemManagerSaveData.instance.CompletedCount; i++)
        {
            if (starsParentMapDisplay.childCount > 0)
            {
                ImageSwapHelper swapHelper = starsParentMapDisplay.GetChild(i).GetComponent<ImageSwapHelper>();

                swapHelper.SetActivatedChild();
            }
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
    public void ResetDisplayMap()
    {
        starSliderTestLevelMapDisplay.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
        for (int i = 0; i < starsParentMapDisplay.childCount; i++)
        {
            Destroy(starsParentMapDisplay.GetChild(i).gameObject);
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
