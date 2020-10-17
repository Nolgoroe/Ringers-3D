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
    public CellManager cellManager;

    public LevelScriptableObject currentLevel;

    public int currentFilledCellCount;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameBoard = Instantiate(circleBoardPrefab, destroyOutOfLevel);
        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);


        clipManager.Init();
        cursorControl.Init();



        StartLevel();//// Needs to be called by a button later

    }

    public void StartLevel()
    {
        sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);
    }

    public void CheckCanEndLevel()
    {
        if (currentFilledCellCount == currentLevel.cellsCountInLevel)
        {
            Debug.Log("Can Finish Level");
        }
    }
}
