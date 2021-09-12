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
    BrownFox,
    BlueFox,
    CelestialFox,
    WoodMarmot,
    SimpleMarmot,
    SecondMarmot,
    LegendaryMarmot,
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
public class AnimalsManager : MonoBehaviour
{
    public static AnimalsManager Instance;

    public List<AnimalsInGame> unlockedAnimals;

    public AnimalsInGame currentLevelAnimal;

    public GameObject statueToSwap;

    string path;

    private void Start()
    {
        Instance = this;

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/AnimalsSaveData.txt";
        }
        else
        {
            path = Application.dataPath + "/Save Files Folder/AnimalsSaveData.txt";
        }

        if (File.Exists(path))
        {
            LoadAnimalData();
        }


        currentLevelAnimal = AnimalsInGame.None;
    }

    public void CheckUnlockAnimal(AnimalsInGame toUnclock)
    {
        StartCoroutine(RescueAnimalSequance());

        if (!unlockedAnimals.Contains(currentLevelAnimal))
        {
            unlockedAnimals.Add(toUnclock);
            SaveAnimalData();
        }
        else
        {
            Debug.Log("Already has animal");
        }
    }

    [ContextMenu("Save")]
    public void SaveAnimalData()
    {
        string savedData = JsonUtility.ToJson(this);

        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath + "/AnimalsSaveData.txt";
        }
        else
        {
            string path = Application.dataPath + "/Save Files Folder/AnimalsSaveData.txt";
        }
        File.WriteAllText(path, savedData);
    }

    [ContextMenu("Load")]
    public void LoadAnimalData()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath + "/AnimalsSaveData.txt";
        }
        else
        {
            string path = Application.dataPath + "/Save Files Folder/AnimalsSaveData.txt";
        }
        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

        Instance = this;
    }


    [ContextMenu("Get Animal")]
    public void GiveAnimal()
    {
        CheckUnlockAnimal(currentLevelAnimal);
    }

    public IEnumerator RescueAnimalSequance()
    {
        AnimalPrefabData APD = statueToSwap.GetComponent<AnimalPrefabData>();
        GameObject go = Instantiate(APD.animatedPrefab, statueToSwap.transform.parent);

        Destroy(statueToSwap.gameObject);
        //Destroy(go, 3.7f);
        statueToSwap = null;

        yield return null;
    }

    public GameObject PopulateWeightSystemAnimals()
    {
        List<int[]> listOfArrayInts = new List<int[]>();
        
        int currentInnerIndex = 0;

        int totalSum = 0; ////// THIS VARIABLE IS HERE INCASE TEAM DECIDED TOTAL IS NOT ALWAYS 100!

        int zoneID = 0;


        foreach (animalsPerZone APZ in ZoneManagerHelpData.Instance.possibleAnimalsInZones)
        {
            if(ZoneManagerHelpData.Instance.CompareZonesSame(APZ.theZone, ZoneManagerHelpData.Instance.currentZoneCheck)) //// Ask Alon
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

        if(totalSum < 100) //// THIS IS TEMPORARY, UNTIL TEAM DECIDES IF MAX IS ALWAYS 100 OR IS IT DYNAMIC
        {
            totalSum = 100; ///// THIS IS A BUG PROBABLY
        }

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
                if (ZoneManagerHelpData.Instance.possibleAnimalsInZones[zoneID].animalsData[indexForMainAnimalArray].animalPrefab)
                {
                    return ZoneManagerHelpData.Instance.possibleAnimalsInZones[zoneID].animalsData[indexForMainAnimalArray].animalPrefab;
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
}
