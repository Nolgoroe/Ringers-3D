using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level")]
public class LevelScriptableObject : ScriptableObject
{
    public bool isDoubleRing;
    public GameObject boardPrefab;

    public int levelNum;
    public int cellsCountInLevel;

    public SliceCatagory[] slicesToSpawn;
    public bool[] lockSlices;
    public bool[] lootSlices;
    public bool[] limiterSlices;
    public LootPacks[] RewardBags;
    public bool isRandomDistributionToSlices;
}
