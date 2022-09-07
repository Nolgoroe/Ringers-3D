using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;



[Serializable]
public class Page
{
    public int pageID;
    public GameObject pageGO;
    public AnimalsInGame[] animalsInPage;
    public int ownedAnimalsCount;
}

[Serializable]
public class PageToPageImages
{
    public Page page;
    public AlbumImageAnimalData[] pageImages;

}
public class AnimalAlbumManager : MonoBehaviour
{
    public static AnimalAlbumManager Instance;

    public PageToPageImages[] pageToPageImages;

    //public Page[] pages;
    public Page currentActivePage;

    public Sprite[] animalSprites;
    public Sprite[] animalSpritesLocked;

    public Dictionary<AnimalsInGame, Sprite> animalEnumToSprite;
    public Dictionary<AnimalsInGame, Sprite> animalEnumToSpriteLocked;

    public Button giveRewardButton;

    int currentPageNum = 0;

    public float revealAnimalSpeed;

    public int minRubyRewardClearPage;
    public int maxRubyRewardClearPage;

    bool hasChangedSaveData;
    void Start()
    {
        Instance = this;

        animalEnumToSprite = new Dictionary<AnimalsInGame, Sprite>();
        animalEnumToSpriteLocked = new Dictionary<AnimalsInGame, Sprite>();

        for (int i = 0; i < System.Enum.GetValues(typeof(AnimalsInGame)).Length; i++)
        {
            if((AnimalsInGame)i != AnimalsInGame.None)
            {
                animalEnumToSprite.Add((AnimalsInGame)i, animalSprites[i]);
            }
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(AnimalsInGame)).Length; i++)
        {
            if((AnimalsInGame)i != AnimalsInGame.None)
            {
                animalEnumToSpriteLocked.Add((AnimalsInGame)i, animalSpritesLocked[i]);
            }
        }

        foreach (PageToPageImages item in pageToPageImages)
        {
            item.page.pageGO.SetActive(false);
        }


        giveRewardButton.gameObject.SetActive(false);
    }

    public void ChangePageLogic(int pageNumber)
    {
        hasChangedSaveData = false;
        currentPageNum = pageNumber;

        if (currentActivePage.pageGO != null)
        {
            currentActivePage.pageGO.SetActive(false);
        }

        currentActivePage = pageToPageImages[pageNumber].page;
        currentActivePage.pageGO.SetActive(true);


        pageToPageImages[pageNumber].page.ownedAnimalsCount = 0;

        for (int i = 0; i < pageToPageImages[pageNumber].pageImages.Length; i++)
        {
            pageToPageImages[pageNumber].pageImages[i].imageAnimalEnum = pageToPageImages[pageNumber].page.animalsInPage[i];
            StartCoroutine(CheckHasAnimal(pageToPageImages[pageNumber].pageImages[i]));
        }

        if(hasChangedSaveData)
        {
            hasChangedSaveData = false;
            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.animalManager });
        }
    }

    public IEnumerator CheckHasAnimal(AlbumImageAnimalData data)
    {

        OwnedAnimalDataSet OADS = AnimalsManager.Instance.unlockedAnimals.Where(p => p.animalEnum == data.imageAnimalEnum).FirstOrDefault();


        if (OADS != null)
        {
            data.isUnlocked = true;

            if (!AnimalsManager.Instance.revealedAnimalsInAlbum.Contains(data.imageAnimalEnum))
            {
                hasChangedSaveData = true;

                AnimalsManager.Instance.revealedAnimalsInAlbum.Add(data.imageAnimalEnum);
                data.TransferToRevealed();
            }
            else
            {
                data.TransferToRevealedImmediate();
                data.AfterReveal();
            }
            //data.animalImage.sprite = animalEnumToSprite[data.imageAnimalEnum];

            pageToPageImages[currentPageNum].page.ownedAnimalsCount++;
        }
        else
        {
            data.isUnlocked = false;
            //data.animalImage.sprite = animalEnumToSpriteLocked[data.imageAnimalEnum];
        }

        if(hasChangedSaveData)
        {
            yield return new WaitForSeconds(revealAnimalSpeed + 0.1f);
        }

        if (pageToPageImages[currentPageNum].page.ownedAnimalsCount == 4)
        {
            CompletedPageToReward CPTR = AnimalsManager.Instance.completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == currentPageNum).FirstOrDefault();


            if(CPTR == null)
            {
                CompletedPageToReward newCPTR = new CompletedPageToReward();
                newCPTR.completedAnimalPagesID = currentPageNum;

                AnimalsManager.Instance.completedAnimalPagesToReward.Add(newCPTR);
                giveRewardButton.gameObject.SetActive(true);

                Debug.LogError("Do the completed combo thing here");
                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.animalManager });

            }
            else
            {
                if (!CPTR.gaveReward)
                {
                    giveRewardButton.gameObject.SetActive(true);
                }
                else
                {
                    giveRewardButton.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            CompletedPageToReward CPTR = AnimalsManager.Instance.completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == currentPageNum).FirstOrDefault();
            giveRewardButton.gameObject.SetActive(false);
        }


    }

    public void GiveReward()
    {
        CompletedPageToReward CPTR = AnimalsManager.Instance.completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == currentPageNum).FirstOrDefault();

        if (CPTR != null && !CPTR.gaveReward)
        {
            CPTR.gaveReward = true;
            giveRewardButton.gameObject.SetActive(false);

            int randomNumRecieve = GetAmountOfRubyRewardClearAlbumPage();
            PlayerManager.Instance.AddRubies(randomNumRecieve);
            UIManager.Instance.ShowAnimalAlbumGiveLoot(randomNumRecieve);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.animalManager, SystemsToSave.Player});



            Debug.LogError("gave reward");
        }
    }

    private int GetAmountOfRubyRewardClearAlbumPage()
    {
        int randomNum = UnityEngine.Random.Range(minRubyRewardClearPage, maxRubyRewardClearPage);

        return randomNum;
    }
}
