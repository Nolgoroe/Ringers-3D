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
public class SummonedAnimalData
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
            path = Application.dataPath + "/AnimalsSaveData.txt";
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
            string path = Application.dataPath + "/AnimalsSaveData.txt";
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
            string path = Application.dataPath + "/AnimalsSaveData.txt";
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
        Instantiate(APD.animatedPrefab, statueToSwap.transform.parent);


        Destroy(statueToSwap.gameObject);
        statueToSwap = null;

        yield return null;
    }

    public GameObject PopulateWeightSystemAnimals()
    {
        List<int[]> listOfArrayInts = new List<int[]>();
        
        int currentInnerIndex = 0;

        int totalSum = 0; ////// THIS VARIABLE IS HERE INCASE TEAM DECIDED TOTAL IS NOT ALWAYS 100!

        foreach (SummonedAnimalData SAD in ZoneManagerHelpData.Instance.currentZoneCheck.possibleAnimalsInLevel)
        {
            int[] animalChances = new int[SAD.weight];

            totalSum += (int)SAD.weight;

            for (int i = 0; i < SAD.weight; i++)
            {
                currentInnerIndex++;

                animalChances[i] = currentInnerIndex;
            }

            listOfArrayInts.Add(animalChances);
        }

        if(totalSum < 100) //// THIS IS TEMPORARY, UNTIL TEAM DECIDES IF MAX IS ALWAYS 100 OR IS IT DYNAMIC
        {
            totalSum = 100;
        }

        return GetFromAnimalChances(listOfArrayInts, totalSum);
    }

    public GameObject GetFromAnimalChances(List<int[]> array, int totalSum)
    {
        int rand = UnityEngine.Random.Range(1, totalSum + 1);

        int indexForMainAnimalArray = 0; ///// THIS VARIABLE IS USED TO FIND THE INDEX OF THE ANIMAL TO SUMMON IN ZoneManagerHelpData.Instance.currentZoneCheck.possibleAnimalsInLevel.

        foreach (int[] item in array)
        {
            
            if (item.Contains(rand))
            {
                Debug.Log("FOUND THE NUMBER!");
                if (ZoneManagerHelpData.Instance.currentZoneCheck.possibleAnimalsInLevel[indexForMainAnimalArray].animalPrefab)
                {
                    return ZoneManagerHelpData.Instance.currentZoneCheck.possibleAnimalsInLevel[indexForMainAnimalArray].animalPrefab;
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
