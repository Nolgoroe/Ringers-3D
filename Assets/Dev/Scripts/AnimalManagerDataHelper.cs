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

public class AnimalManagerDataHelper : MonoBehaviour
{
    public static AnimalManagerDataHelper instance;

    public List<AnimalDenDataChecker> animalDataCheckTo;

    public TreePrefabPerZone[] treesPerZone;

    private void Awake()
    {
        instance = this;
    }
}
