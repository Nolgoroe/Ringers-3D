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

public class AnimalManagerDataHelper : MonoBehaviour
{
    public static AnimalManagerDataHelper instance;

    public List<AnimalDenDataChecker> animalDataCheckTo;

    private void Awake()
    {
        instance = this;
    }
}
