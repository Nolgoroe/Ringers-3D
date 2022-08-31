using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class AnimalDenDataChecker
{
    public AnimalsInGame animalEnum;
    public GameObject animalLiveDenPrefab;
    public Transform parentSummonUnder;
}
[Serializable]
public class TreePrefabPerZone
{
    public Zone theZone;
    public GameObject treePrefab;
}

[Serializable]
public class SoundsPerAnimal
{
    public AnimalsInGame animalEnum;
    public Sounds soundClipToPlay;
}

public class AnimalManagerDataHelper : MonoBehaviour
{
    public static AnimalManagerDataHelper instance;

    public List<AnimalDenDataChecker> animalDataCheckTo;

    public TreePrefabPerZone[] treesPerZone;

    public SoundsPerAnimal[] soundsPerAnimalEnum;

    private void Awake()
    {
        instance = this;
    }
}
