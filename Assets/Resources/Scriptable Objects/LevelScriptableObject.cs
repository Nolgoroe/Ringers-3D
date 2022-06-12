using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor;

[System.Serializable]
public class stonePieceDataStruct
{
    public int cellIndex;
    public bool randomValues;
    public bool isNeutral;
    public PieceColor colorOfPieceRight;
    public PieceSymbol symbolOfPieceRight;
    public PieceColor colorOfPieceLeft;
    public PieceSymbol symbolOfPieceLeft;
}

[System.Serializable]
public class sliceToSpawnDataStruct
{
    public SliceCatagory sliceToSpawn;
    public bool isLock;
    public bool isLoot;
    public bool isLimiter;

}

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level")]
public class LevelScriptableObject : ScriptableObject
{
    public string worldName;
    public bool isGrindLevel;
    public bool is12PieceRing;
    public bool isDoubleRing;
    public bool isBoss;
    public bool isAnimalLevel = true;
    public bool isReleaseAnimalToDen;
    public GameObject boardPrefab;
    public GameObject clipPrefab;

    public int levelNum;
    public int worldNum;
    public int levelIndexInZone;
    public int numIndexForLeaderBoard;
    public int tutorialIndexForList;
    public int cellsCountInLevel;

    public bool RandomSlicePositions;
    public bool allowRepeatSlices;
    public PowerUp[] powerupsForLevel;
    public GameObject specificAnimalForLevel;

    public stonePieceDataStruct[] stoneTiles;
    public sliceToSpawnDataStruct[] slicesToSpawn;
    //public SliceCatagory[] slicesToSpawn;
    //public bool[] lockSlices;
    //public bool[] lootSlices;
    //public bool[] limiterSlices;
    public LootPacks[] RewardBags;


    public PieceColor[] levelAvailableColors;
    public PieceSymbol[] levelAvailablesymbols;
    public SpecialPowerData[] symbolsNeededForSpecialPowers;

    [Header("Tutorial Settings")]
    public bool isSpecificTutorial;
    public bool isTutorial;
    public SpecificTutorialsEnum specificTutorialEnum;

    [Header("Specific Slice Settings")]
    public List<pieceDataStruct> arrayOfPieces;
    public int[] specificSliceSpots;
    public PieceSymbol[] specificSlicesShapes;
    public PieceColor[] specificSlicesColors;


    [Header("Boss General Settings")]
    public int BossHealth;
    public bool isRandomPieces;
    public int[] specificPiecesIndex;
    public int rubyRewardNoDefeat;
    public int rubyRewardDefeat;
    public bool ver1Boss;

    [Header("Boss V2 General Settings")]
    public float timeForLevelInSeconds;
    public float dealButtonTimer;
    public float damageToBossCompeleRing;


    [ContextMenu("THIS")]
    public void actionhere()
    {
        string n = name;
        n = n.Replace("Level ", "");

        int num = Convert.ToInt32(n);

        levelNum = num;
        levelIndexInZone = num;

        Debug.Log(num);
    }
    [ContextMenu("THIS Leaderboard")]
    public void actionhere2()
    {
        numIndexForLeaderBoard = 105 + levelNum;
    }
}
