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
            CheckHasAnimal(pageToPageImages[pageNumber].pageImages[i]);
        }

        CompletedPageToReward CPTR = AnimalsManager.Instance.completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == pageNumber).FirstOrDefault();

        if(CPTR != null && !CPTR.gaveReward)
        {
            giveRewardButton.gameObject.SetActive(true);
        }
        else
        {
            giveRewardButton.gameObject.SetActive(false);
        }

    }

    public void CheckHasAnimal(AlbumImageAnimalData data)
    {

        OwnedAnimalDataSet OADS = AnimalsManager.Instance.unlockedAnimals.Where(p => p.animalEnum == data.imageAnimalEnum).FirstOrDefault();


        if (OADS != null)
        {
            data.isUnlocked = true;
            data.animalImage.sprite = animalEnumToSprite[data.imageAnimalEnum];
            pageToPageImages[currentPageNum].page.ownedAnimalsCount++;
        }
        else
        {
            data.isUnlocked = false;
            data.animalImage.sprite = animalEnumToSpriteLocked[data.imageAnimalEnum];
        }

        if(pageToPageImages[currentPageNum].page.ownedAnimalsCount == 4)
        {
            CompletedPageToReward CPTR = AnimalsManager.Instance.completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == currentPageNum).FirstOrDefault();


            if(CPTR == null)
            {
                CompletedPageToReward newCPTR = new CompletedPageToReward();
                newCPTR.completedAnimalPagesID = currentPageNum;

                AnimalsManager.Instance.completedAnimalPagesToReward.Add(newCPTR);
                Debug.LogError("Do the completed combo thing here");

                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.animalManager });
            }
        }
    }

    public void GiveReward()
    {
        CompletedPageToReward CPTR = AnimalsManager.Instance.completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == currentPageNum).FirstOrDefault();

        if (CPTR != null && !CPTR.gaveReward)
        {
            CPTR.gaveReward = true;
            giveRewardButton.gameObject.SetActive(false);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.animalManager });
            Debug.LogError("gave reward");
        }
    }
}
