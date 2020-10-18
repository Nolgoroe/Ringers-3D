using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject circleBoardPrefab;
    public GameObject clipPrefab;

    public GameObject gameBoard;
    public GameObject gameClip;

    public Transform destroyOutOfLevel;

    public CursorController cursorControl;
    public ClipManager clipManager;
    public SliceManager sliceManager;
    public ConnectionManager connectionManager;

    public LevelScriptableObject currentLevel;

    public int currentFilledCellCount;
    public int unsuccessfullConnectionCount;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        gameBoard = Instantiate(circleBoardPrefab, destroyOutOfLevel);
        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        clipManager.Init();
        cursorControl.Init();

        sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);
        connectionManager.GrabCellList(gameBoard.transform);
    }

    public void CheckCanEndLevel()
    {
        if (currentFilledCellCount == currentLevel.cellsCountInLevel)
        {
            Debug.Log("Can Finish Level");
        }
    }

    public void ChooseLevel(int levelNum)
    {
        currentLevel = (LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/Level " + levelNum);


        StartLevel();
    }
}
