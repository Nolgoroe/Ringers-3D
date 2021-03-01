using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public enum AnimalsInGame
{
    Fox,
    Bear,
    Ram,
    Turtle,
    Elephant,
    Deer,
    Firefly,
    Dog,
    Cat,
    Sheep,
    Bull,
    Worm,
    Zebra,
    Lion,
    Giraffe,
    Panda
}


[Serializable]
public class Page
{
    public int pageNum;
    public AnimalsInGame[] animalsInPage;
}

public class AnimalAlbumManager : MonoBehaviour
{
    public static AnimalAlbumManager Instance;
    public int pageNumInspector = 0;
    public int maxPageNum = 4;

    public List<AnimalsInGame> unlockedAnimals;

    public AlbumImageAnimalData[] pageImages;

    public Page[] pages;

    public Sprite[] animalSprites;

    public Dictionary<AnimalsInGame, Sprite> animalEnumToSprite;

    void Start()
    {
        Instance = this;
        pageNumInspector = 0;

        animalEnumToSprite = new Dictionary<AnimalsInGame, Sprite>();

        for (int i = 0; i < System.Enum.GetValues(typeof(AnimalsInGame)).Length; i++)
        {
            animalEnumToSprite.Add((AnimalsInGame)i, animalSprites[i]);
        }
    }

    public void ChangePageLogic(int pageNumber)
    {
        for (int i = 0; i < pageImages.Length; i++)
        {
            pageImages[i].imageAnimalEnum = pages[pageNumber].animalsInPage[i];
            CheckHasAnimal(pageImages[i]);
        }
    }

    public void CheckHasAnimal(AlbumImageAnimalData data)
    {
        if(data.isUnlocked = unlockedAnimals.Contains(data.imageAnimalEnum))
        {
            data.animalImage.sprite = animalEnumToSprite[data.imageAnimalEnum];
        }
        else
        {
            data.isUnlocked = false;
        }
    }

    public void ChangePageOnButton(bool plus)
    {
        if (plus)
        {
            pageNumInspector++;

            if (pageNumInspector > maxPageNum - 1)
            {
                pageNumInspector = 0;
            }
        }
        else
        {
            pageNumInspector--;

            if (pageNumInspector < 0)
            {
                pageNumInspector = maxPageNum - 1;
            }
        }

        ChangePageLogic(pageNumInspector);
    }
}
