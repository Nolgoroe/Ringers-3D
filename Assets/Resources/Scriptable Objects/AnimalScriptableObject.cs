using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum AnimalRarity { Noraml, Rare, Exotic, None}

[CreateAssetMenu(fileName = "Animal", menuName = "ScriptableObjects/Create Animal Data")]
public class AnimalScriptableObject : ScriptableObject
{
    public string animalName;
    public AnimalRarity rarity;
    public ObjectHollowType hollowBelongTo;
    public AnimalTypesInGame animalType;
    public bool isUsedInCrafting;
}
