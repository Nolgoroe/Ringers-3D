using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level")]
public class LevelScriptableObject : ScriptableObject
{
    public string worldName;
    public bool isDoubleRing;
    public GameObject boardPrefab;

    public int levelNum;
    public int levelIndexInZone;
    public int cellsCountInLevel;

    public SliceCatagory[] slicesToSpawn;
    public bool[] lockSlices;
    public bool[] lootSlices;
    public bool[] limiterSlices;
    public LootPacks[] RewardBags;

    public SummonedAnimalData[] possibleAnimalsInLevel;

    public bool isTutorial;
    public List<pieceDataStruct> arrayOfPieces;
    public int[] specificSliceSpots;

    //public bool isRandomDistributionToSlices;
}
