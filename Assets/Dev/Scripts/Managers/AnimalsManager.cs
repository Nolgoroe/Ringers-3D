using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;


[Serializable]
public enum AnimalsInGame
{
    RedFox,
    YellowFox,
    WhiteFox,
    OrangeFox,
    BrownStag,
    PinkStag,
    OrangeStag,
    YellowStag,
    BrownOwl,
    YellowOwl,
    GreyOwl,
    WhiteOwl,
    OrangeBoar,
    DarkBoar,
    BrownBoar,
    WhiteBoar,
    BrownCharmander,
    YellowCharmander,
    BlueCharmander,
    PinkCharmander,
    None
}
[Serializable]
public enum AnimalTypesInGame
{
    Fox,
    Stag,
    Owl,
    Boar,
    Charmander,
    None
}

[Serializable]
public class animalsPerZone
{
    public Zone theZone;
    public SummonedAnimalsData[] animalsData;
}

[Serializable]
public class SummonedAnimalsData
{
    public AnimalsInGame animalEnum;
    public GameObject animalPrefab;
    public int weight;
}
[Serializable]
public class OwnedAnimalDataSet
{
    public AnimalTypesInGame animalTypeEnum;
    public AnimalsInGame animalEnum;
    public ObjectHollowType hollowBelongsTo;
}

[Serializable]
public class CompletedPageToReward
{
    public int completedAnimalPagesID;
    public bool gaveReward;
}

[Serializable]
public class AnimalsManager : MonoBehaviour
{
    public static AnimalsManager Instance;

    public List<OwnedAnimalDataSet> unlockedAnimals;
    public List<AnimalsInGame> placedAnimalsInDen;

    public AnimalsInGame currentLevelAnimal;
    public GameObject currentLevelLiveAnimal;

    public GameObject statueToSwap;

    public List<CompletedPageToReward> completedAnimalPagesToReward;
    public List<AnimalsInGame> revealedAnimalsInAlbum;

    string path;

    public bool spawnedDenAnimals;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

        currentLevelAnimal = AnimalsInGame.None;
        currentLevelLiveAnimal = null;
        statueToSwap = null;
        spawnedDenAnimals = false;

        SpawnAllAnimalsInDenStartApplication();
    }

    public void ResetAnimalManagerData()
    {
        currentLevelAnimal = AnimalsInGame.None;
        currentLevelLiveAnimal = null;
        statueToSwap = null;
        spawnedDenAnimals = false;
    }

    public void CheckUnlockAnimal(AnimalsInGame toUnclock)
    {
        OwnedAnimalDataSet OADS = unlockedAnimals.Where(p => p.animalEnum == currentLevelAnimal).SingleOrDefault();

        if (OADS == null)
        {
            AnimalPrefabData APD = statueToSwap.GetComponent<AnimalPrefabData>();

            OwnedAnimalDataSet newData = new OwnedAnimalDataSet();
            newData.animalEnum = toUnclock;
            newData.hollowBelongsTo = APD.animalSO.hollowBelongTo;
            newData.animalTypeEnum = APD.animalSO.animalType;

            unlockedAnimals.Add(newData);

            if(TutorialSaveData.Instance.hasFinishedAnimalAlbum)
            {
                InterestPointsManager.instance.TurnOnPointsOfInterestDisplay(TypesPointOfInterest.AnimalAlbum);
            }

            //SaveAnimalData();
        }
        else
        {
            Debug.Log("Already has animal");
        }

        StartCoroutine(RescueAnimalSequance());
    }

    //[ContextMenu("Save")]
    //public void SaveAnimalData()
    //{
    //    string savedData = JsonUtility.ToJson(this);

    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/AnimalsSaveData.txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/AnimalsSaveData.txt";
    //    }
    //    File.WriteAllText(path, savedData);
    //}

    //[ContextMenu("Load")]
    //public void LoadAnimalData()
    //{
    //    //if (Application.platform == RuntimePlatform.Android)
    //    //{
    //    //    string path = Application.persistentDataPath + "/AnimalsSaveData.txt";
    //    //}
    //    //else
    //    //{
    //    //    string path = Application.dataPath + "/Save Files Folder/AnimalsSaveData.txt";
    //    //}
    //    JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

    //    Instance = this;
    //}


    [ContextMenu("Get Animal")]
    public void GiveAnimal()
    {
        TempEasyLifeScript t = GetComponent<TempEasyLifeScript>();

        t.turnoffall();

        CheckUnlockAnimal(currentLevelAnimal);
    }

    public IEnumerator RescueAnimalSequance()
    {
        AnimalPrefabData APD = statueToSwap.GetComponent<AnimalPrefabData>();

        statueToSwap.GetComponent<AnimalPrefabData>().renderer.materials[0].DisableKeyword("_EMISSION");

        statueToSwap.SetActive(false);


        GameObject go = Instantiate(APD.animatedPrefab, statueToSwap.transform.parent);

        currentLevelLiveAnimal = go;

        //Destroy(statueToSwap.gameObject);
        //Destroy(go, 3.7f);
        //statueToSwap = null;

        yield return null;
    }

    public GameObject PopulateWeightSystemAnimals()
    {
        List<int[]> listOfArrayInts = new List<int[]>();
        
        int currentInnerIndex = 0;

        int totalSum = 0; ////// THIS VARIABLE IS HERE INCASE TEAM DECIDED TOTAL IS NOT ALWAYS 100!

        int zoneID = 0;


        foreach (animalsPerZone APZ in ZoneManagerHelpData.Instance.possibleAnimalsPerZone)
        {
            if(ZoneManagerHelpData.Instance.CompareZonesSame(APZ.theZone, ZoneManagerHelpData.Instance.currentZoneCheck))
            {
                zoneID = APZ.theZone.id;

                foreach (SummonedAnimalsData SAD in APZ.animalsData)
                {
                    int[] animalChances = new int[SAD.weight];

                    totalSum += SAD.weight;

                    for (int i = 0; i < SAD.weight; i++)
                    {
                        currentInnerIndex++;

                        animalChances[i] = currentInnerIndex;
                    }

                    listOfArrayInts.Add(animalChances);
                }

                break;
            }
        }

        //if(totalSum < 100) //// THIS IS TEMPORARY, UNTIL TEAM DECIDES IF MAX IS ALWAYS 100 OR IS IT DYNAMIC
        //{
        //    totalSum = 100; ///// THIS IS A BUG PROBABLY
        //}

        return GetFromAnimalChances(listOfArrayInts, totalSum, zoneID);
    }

    public GameObject GetFromAnimalChances(List<int[]> array, int totalSum, int zoneID)
    {
        int rand = UnityEngine.Random.Range(1, totalSum + 1);

        int indexForMainAnimalArray = 0; ///// THIS VARIABLE IS USED TO FIND THE INDEX OF THE ANIMAL TO SUMMON IN ZoneManagerHelpData.Instance.currentZoneCheck.possibleAnimalsInLevel.

        foreach (int[] item in array)
        {            
            if (item.Contains(rand))
            {
                //Debug.Log("FOUND THE NUMBER!");
                if (ZoneManagerHelpData.Instance.possibleAnimalsPerZone[zoneID].animalsData[indexForMainAnimalArray].animalPrefab)
                {
                    return ZoneManagerHelpData.Instance.possibleAnimalsPerZone[zoneID].animalsData[indexForMainAnimalArray].animalPrefab;
                }
                else
                {
                    return null;
                }
            }

            indexForMainAnimalArray++;
        }

        return null;
    }

    public void CallSpawnAnimalInDen()
    {
        AnimalPrefabData APD = statueToSwap.GetComponent<AnimalPrefabData>();

        SpawnAnimalInDen(APD);
    }
    public void SpawnAnimalInDen(AnimalPrefabData animalToCheck)
    {
        zoneSlotAndType ZSAT = HollowCraftAndOwnedManager.Instance.hollowZones.Where(p => p.acceptedHollowTypes.Contains(animalToCheck.animalSO.hollowBelongTo)).SingleOrDefault();

        bool canSummon = CheckConditionsSummonAnimalDen(animalToCheck, ZSAT);

        if (canSummon)
        {
            AnimalDenDataChecker animalCheckTo = AnimalManagerDataHelper.instance.animalDataCheckTo.Where(p => p.animalEnum == animalToCheck.animalType).SingleOrDefault();

            if (animalCheckTo != null)
            {
                Instantiate(animalCheckTo.animalLiveDenPrefab, animalCheckTo.parentSummonUnder);
                placedAnimalsInDen.Add(animalCheckTo.animalEnum);

                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.animalManager });

                Debug.LogError("Summoned Animal");
            }
        }
    }

    public bool CheckConditionsSummonAnimalDen(AnimalPrefabData animalToCheck, zoneSlotAndType ZSAT)
    {
        AnimalDenDataChecker animalCheckTo = AnimalManagerDataHelper.instance.animalDataCheckTo.Where(p => p.animalEnum == animalToCheck.animalType).SingleOrDefault();

        if (animalCheckTo == null)
        {
            return false;
        }

        if (placedAnimalsInDen.Contains(animalToCheck.animalType))
        {
            return false;
        }

        if(ZSAT != null)
        {
            if (!ZSAT.zoneSlot.isFilled)
            {
                return false;
            }
        }

        return true;
    }
    public void SpawnAllAnimalsInDenStartApplication()
    {
        if (!spawnedDenAnimals)
        {
            spawnedDenAnimals = true;

            foreach (AnimalsInGame animal in placedAnimalsInDen)
            {
                AnimalDenDataChecker animalCheckTo = AnimalManagerDataHelper.instance.animalDataCheckTo.Where(p => p.animalEnum == animal).Single();

                if(animalCheckTo != null)
                {
                    Instantiate(animalCheckTo.animalLiveDenPrefab, animalCheckTo.parentSummonUnder);
                }
            }
        }
    }

    public bool CheckIfAlreadyPlacedAnimalInDenForUI(AnimalPrefabData animalToCheck)
    {
        if (placedAnimalsInDen.Contains(animalToCheck.animalType))
        {
            return false;
        }

        return true;
    }


    public void CheatClearAnimalAlbumPage()
    {
        unlockedAnimals.Clear();
        revealedAnimalsInAlbum.Clear();
        completedAnimalPagesToReward.Clear();
        //CompletedPageToReward CPTR = completedAnimalPagesToReward.Where(p => p.completedAnimalPagesID == AnimalAlbumManager.Instance.currentActivePage.pageID).FirstOrDefault();

        //if(CPTR != null)
        //{
        //    completedAnimalPagesToReward.Remove(CPTR);
        //}

        //for (int i = revealedAnimalsInAlbum.Count() - 1; i >= 0; i--)
        //{
        //    for (int j = 0; j < AnimalAlbumManager.Instance.currentActivePage.animalsInPage.Count(); j++)
        //    {
        //        if (AnimalAlbumManager.Instance.currentActivePage.animalsInPage.Contains(revealedAnimalsInAlbum[i]))
        //        {
        //            revealedAnimalsInAlbum.RemoveAt(i);
        //        }
        //    }
        //}
    }
}
