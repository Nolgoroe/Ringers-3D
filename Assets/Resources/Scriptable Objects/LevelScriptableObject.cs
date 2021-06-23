using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class stonePieceDataStruct
{
    public int cellIndex;
    public PieceColor colorOfPieceRight;
    public PieceSymbol symbolOfPieceRight;
    public PieceColor colorOfPieceLeft;
    public PieceSymbol symbolOfPieceLeft;
}

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level")]
public class LevelScriptableObject : ScriptableObject
{
    public string worldName;
    public bool isDoubleRing;
    public GameObject boardPrefab;

    public int levelNum;
    public int levelIndexInZone;
    public int tutorialIndexForList;
    public int cellsCountInLevel;

    public bool RandomSlices;

    public stonePieceDataStruct[] stoneSlices;
    public SliceCatagory[] slicesToSpawn;
    public int[] sliceCellIndexes;
    public bool[] lockSlices;
    public bool[] lootSlices;
    public bool[] limiterSlices;
    public LootPacks[] RewardBags;


    public PieceColor[] levelAvailableColors;
    public PieceSymbol[] levelAvailablesymbols;
    public SpecialPowerData[] symbolsNeededForSpecialPowers;


    public bool isTutorial;
    public List<pieceDataStruct> arrayOfPieces;
    public int[] specificSliceSpots;
    public PieceSymbol[] specificSlicesShapes;
    public PieceColor[] specificSlicesColors;
}
