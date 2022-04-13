using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

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

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level")]
public class LevelScriptableObject : ScriptableObject
{
    public string worldName;
    public bool isGrindLevel;
    public bool is12PieceRing;
    public bool isDoubleRing;
    public bool isBoss;
    public bool isReleaseAnimalToDen;
    public GameObject boardPrefab;

    public int levelNum;
    public int worldNum;
    public int levelIndexInZone;
    public int tutorialIndexForList;
    public int cellsCountInLevel;

    public bool RandomSlicePositions;
    public bool allowRepeatSlices;
    public PowerUp[] powerupsForLevel;
    public GameObject specificAnimalForLevel;

    public stonePieceDataStruct[] stoneTiles;
    public SliceCatagory[] slicesToSpawn;
    public bool[] lockSlices;
    public bool[] lootSlices;
    public bool[] limiterSlices;
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


    [Header("Boss Settings")]
    public int BossHealth;
    public bool isRandomPieces;
    public int[] specificPiecesIndex;
    

    [ContextMenu("Change Name")]
    public void NameChange()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.RenameAsset(assetPath, "Level " + levelNum);
        AssetDatabase.Refresh();
    }
}
