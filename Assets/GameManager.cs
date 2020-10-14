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

    void Start()
    {
        Instance = this;

        gameBoard = Instantiate(circleBoardPrefab, destroyOutOfLevel);
        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        cursorControl.Init();

    }

    void Update()
    {
        
    }
}
