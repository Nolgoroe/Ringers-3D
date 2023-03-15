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
    public GameObject tileprefab;
    public bool isLockedOnSpawn;
}

[System.Serializable]
public class sliceToSpawnDataStruct
{
    public SliceCatagory sliceToSpawn;
    public bool isLock;
    public bool isLoot;
    public bool isLimiter;
}

public enum LevelDifficulty
{
    Normal,
    Hard
}

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level")]
public class LevelScriptableObject : ScriptableObject
{
    [Header("General Settings")]
    public string worldName;
    public bool isGrindLevel;
    public bool is12PieceRing;
    public bool isDoubleRing;
    public bool isBoss;
    public bool isTimerLevel;
    public bool isNotChestLevel;
    public bool isAnimalLevel = true;
    public bool isReleaseAnimalToDen;
    public bool isKeyLevel;
    //public bool isTestLevel;
    public bool showIntroLevelAnimation;
    public bool allowBackToMap;
    public GameObject boardPrefab;
    public GameObject clipPrefab;

    public int levelNum;
    public int worldNum;
    public int levelIndexInZone;
    public int numIndexForLeaderBoard;
    public int tutorialIndexForList;
    public int cellsCountInLevel;
    public LevelDifficulty levelDifficulty;

    public bool RandomSlicePositions;
    public bool allowRepeatSlices;
    public bool allowRepeatTiles;
    public bool allowRepeatTileSides;
    public PowerUp[] powerupsForLevel;
    //public GameObject specificAnimalForLevel;

    public stonePieceDataStruct[] stoneTiles;
    public sliceToSpawnDataStruct[] slicesToSpawn;
    public LootPacks[] RewardBags;


    public PieceColor[] levelAvailableColors;
    public PieceSymbol[] levelAvailablesymbols;
    public SpecialPowerData[] symbolsNeededForSpecialPowers;


    [Header("Tutorial Settings")]
    public bool isSpecificTutorial;
    public bool isTutorial;
    public SpecificTutorialsEnum specificTutorialEnum;
    public SpecificTutorialsEnum specificTutorialEnumEndLevel;
    public bool colorMatch;
    public bool symbolMatch;
    public BottomUIToShow bottomUIToShow;

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

    [Header("Last clip algoritm")]
    public bool useLastClipAlgoritm;
    public int algoritmStepsWanted;

    [Header("Dialogue Data")]
    public DialogueScriptableObject levelStartDialogueSO;
    public DialogueScriptableObject levelEndDialogueSO;

    [Header("Timer Level Data")]
    public float timeForLevel;

    [Header("Editor Actions")]
    public GameObject stoneTilePrefab;

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
        numIndexForLeaderBoard = 115 + levelNum;
    }

    [ContextMenu("Set stone tile prefab")]
    public void SetStoneTilePrefabSO()
    {
        foreach (stonePieceDataStruct stoneTile in stoneTiles)
        {
            stoneTile.tileprefab = stoneTilePrefab;
        }
    }
}
