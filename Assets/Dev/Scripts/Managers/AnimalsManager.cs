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
    SandFox,
    BlueFox,
    BrownStag,
    RedStag,
    LegendaryStag,
    BrownOwl,
    YellowOwl,
    SimpleHawk,
    Sheep,
    Bull,
    Worm,
    Zebra,
    Lion,
    Giraffe,
    Panda,
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
    public AnimalsInGame animalEnum;
    public ObjectHollowType hollowBelongsTo;
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

    public void CheckUnlockAnimal(AnimalsInGame toUnclock)
    {
        OwnedAnimalDataSet OADS = unlockedAnimals.Where(p => p.animalEnum == currentLevelAnimal).SingleOrDefault();

        if (OADS == null)
        {
            AnimalPrefabData APD = statueToSwap.GetComponent<AnimalPrefabData>();

            OwnedAnimalDataSet newData = new OwnedAnimalDataSet();
            newData.animalEnum = toUnclock;
            newData.hollowBelongsTo = APD.animalSO.hollowBelongTo;

            unlockedAnimals.Add(newData);
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
}
