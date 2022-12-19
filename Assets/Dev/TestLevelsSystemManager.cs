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
    public GameObject starPrefabMap;

    public Transform starsParent;

    public Slider StarSlider;

    public float barAnimateSpeed;


    public Slider starSliderTestLevelMapDisplay;
    public Transform starsParentMapDisplay;

    public Interactable3D selectedLevelButton;

    public GameObject chestPrefab;
    public Animator chestAnimator;
    void Start()
    {
        instance = this;
    }

    public IEnumerator InitTestLevel()
    {
        LeanTween.cancel(StarSlider.gameObject);

        StarSlider.maxValue = numOfSections;

        //ShowRelaventUI();
        ResetDisplay();
        yield return new WaitForEndOfFrame();
        InstantiateBarStars();

        yield return new WaitForEndOfFrame();
        InitLevelStars();

        //yield return new WaitForEndOfFrame();

    }

    //private void ShowRelaventUI()
    //{
    //    ResetDisplay();

    //    UIManager.Instance.gameplayCanvasTop.SetActive(false);
    //    UIManager.Instance.gameplayCanvasTopTestLevels.SetActive(true);
    //}

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
                GameObject go = Instantiate(starPrefabMap, starsParentMapDisplay);
                RectTransform starRect = go.GetComponent<RectTransform>();
                starRect.anchoredPosition = new Vector2(amout * i, 0);

                ImageSwapHelper swapHelper = go.GetComponent<ImageSwapHelper>();

                swapHelper.SetDeActivatedChild();
            }
        }

        ActivateStarOrChestMap();
    }

    public IEnumerator UpdateBarValue()
    {
        DecideSoundToPlayChestBar();

        LeanTween.value(StarSlider.gameObject, StarSlider.value, StarSlider.value + 1, barAnimateSpeed).setOnUpdate((float val) =>
        {
            StarSlider.value = val;
        });

        yield return new WaitForSeconds(barAnimateSpeed + 0.1f);

        ActivateStarOrChestLevel(TestLevelsSystemManagerSaveData.instance.CompletedCount);
    }
    public void UpdateBarValueOnMap()
    {
        starSliderTestLevelMapDisplay.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
    }

    private void InitLevelStars()
    {
        for (int i = 0; i < TestLevelsSystemManagerSaveData.instance.CompletedCount; i++)
        {
            if (starsParent.childCount > 0)
            {
                ImageSwapHelper swapHelper = starsParent.GetChild(i).GetComponent<ImageSwapHelper>();

                swapHelper.SetActivatedChild();
            }
        }
    }
    private void ActivateStarOrChestLevel(int index)
    {
        if (index == numOfSections)
        {
            Debug.LogError("Giving Chest");

            chestAnimator = Instantiate(chestPrefab).GetComponent<Animator>();
            LootManager.Instance.parentChestLoot = chestAnimator.transform;

            //animate chest here
            TestLevelsSystemManagerSaveData.instance.canGetChest = true;

            //UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < TestLevelsSystemManagerSaveData.instance.CompletedCount; i++)
        {
            if (starsParent.childCount > 0)
            {
                ImageSwapHelper swapHelper = starsParent.GetChild(i).GetComponent<ImageSwapHelper>();

                if (i == TestLevelsSystemManagerSaveData.instance.CompletedCount - 1)
                {
                    swapHelper.ActivateAnimation();
                }
                else
                {
                    swapHelper.SetActivatedChild();
                }
            }
        }
    }

    public void DecideSoundToPlayChestBar()
    {
        int tempNum = TestLevelsSystemManagerSaveData.instance.CompletedCount - 1;

        if (TestLevelsSystemManagerSaveData.instance.CompletedCount == numOfSections)
        {
            SoundManager.Instance.PlaySound(Sounds.ChestAppear);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.soundForChestBar[tempNum]);
        }
    }

    public bool isGiveChest(int index)
    {
        return index >= numOfSections;
    }
    private void ActivateStarOrChestMap()
    {
        int amountToCheck = TestLevelsSystemManagerSaveData.instance.CompletedCount == numOfSections ? numOfSections - 1 : TestLevelsSystemManagerSaveData.instance.CompletedCount;
        
        for (int i = 0; i < amountToCheck; i++)
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
        StarSlider.gameObject.SetActive(true);
        StarSlider.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
        for (int i = 0; i < starsParent.childCount; i++)
        {
            Destroy(starsParent.GetChild(i).gameObject);
        }
    }
    public void ResetDisplayMap()
    {
        starSliderTestLevelMapDisplay.gameObject.SetActive(true);

        starSliderTestLevelMapDisplay.value = TestLevelsSystemManagerSaveData.instance.CompletedCount;
        for (int i = 0; i < starsParentMapDisplay.childCount; i++)
        {
            Destroy(starsParentMapDisplay.GetChild(i).gameObject);
        }
    }


    //called by button
    public void LaunchTestLevel()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        selectedLevelButton.ChooseTypeLevelLaunch();

        //UIManager.Instance.testLevelsDataScreen.SetActive(false);

        //GameManager.Instance.levelStarted = true;

        //SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

        //GameManager.Instance.StartLevel(true, false);
    }

    public void SetDeactivatedLevelData()
    {
        StarSlider.gameObject.SetActive(false);
    }
    public void SetDeactivatedmapData()
    {
        starSliderTestLevelMapDisplay.gameObject.SetActive(false);
    }
}
