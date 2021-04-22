using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;


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
    Panda,
    None
}

[Serializable]
public class SummonedAnimalData
{
    public AnimalsInGame animal;
    public GameObject animalPrefab;
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
}
