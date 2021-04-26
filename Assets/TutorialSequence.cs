using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class pieceDataStruct
{
    public PieceColor colorOfPieceRight;
    public PieceSymbol symbolOfPieceRight;
    public PieceColor colorOfPieceLeft;
    public PieceSymbol symbolOfPieceLeft;
}

[System.Serializable]
public class Sequence
{
    public int levelID;
    public int EndPhaseID;
    public OutLineData[] cellOutlines;
    public Phase[] phase;
    public GameObject[] screens;
}

[System.Serializable]
public class Phase
{
    public int phaseID;
    public bool isClipPhase, isBoardPhase;
    public bool dealPhase;

    public int unlockedClips;
    public int unlockedBoardCells;

    public int targetCell;

}
[System.Serializable]
public class OutLineData
{
    public int cellIndex;
    public bool right;
}

public class TutorialSequence : MonoBehaviour
{
    public static TutorialSequence Instacne;
    public int currentPhaseInSequence;

    public Sequence[] levelSequences;

    public bool duringSequence;
    private void Start()
    {
        Instacne = this;
    }

    public void StartSequence(int levelNum)
    {
        DisplayTutorialScreens();
        OutlineInstantiate();
        UIManager.Instance.dealButton.interactable = false;

        currentPhaseInSequence = 0;
        duringSequence = true;
        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            //for (int k = 0; k < levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
            //{
                if (i == levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
            //}
        }
    }

    private void DisplayTutorialScreens()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens)
        {
            go.SetActive(false);
        }

        levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens[0].SetActive(true);
    }

    public void IncrementCurrentPhaseInSequence()
    {
        currentPhaseInSequence++;

        if(levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens[currentPhaseInSequence])
        {
            levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens[currentPhaseInSequence - 1].SetActive(false);
            levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens[currentPhaseInSequence].SetActive(true);
        }

        if (currentPhaseInSequence >= levelSequences[GameManager.Instance.currentLevel.levelNum - 1].EndPhaseID)
        {
            duringSequence = false;
            Debug.Log("Phases are done!");
            Invoke("UnlockAll", 2);
            return;
        }


        ChangePhase();
    }

    public void ChangePhase()
    {
        if (levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].isClipPhase)
        {
            ClipPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].isBoardPhase)
        {
            BoardPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].dealPhase)
        {
            DealPhaseLogic();
        }
    }

    public void ClipPhaseLogic()
    {
            UIManager.Instance.dealButton.interactable = false;

            foreach (Cell c in ConnectionManager.Instance.cells)
            {
                if (c.isFull)
                {
                    c.pieceHeld.isTutorialLocked = true;
                }
            }

            for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
            {
                Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

                //for (int k = 0; k < levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
                //{
                if (i == levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedClips/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }
    }
    public void BoardPhaseLogic()
    {
        UIManager.Instance.dealButton.interactable = false;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = true;
        }


        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                //int length = levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedBoardCells.Length;
                //for (int i = 0; i < length; i++)
                //{
                if (c.cellIndex == levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedBoardCells/*[i]*/)
                {
                    c.pieceHeld.isTutorialLocked = false;
                }
                //}
            }
        }
    }
    public void DealPhaseLogic()
    {
        UIManager.Instance.dealButton.interactable = true;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = true;
        }

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = true;
            }
        }
    }
    public void OutlineInstantiate()
    {
        foreach (OutLineData old in levelSequences[GameManager.Instance.currentLevel.levelNum - 1].cellOutlines)
        {
            if (old.right)
            {
                Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteRight, ConnectionManager.Instance.cells[old.cellIndex].transform);
            }
            else
            {
                Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteLeft, ConnectionManager.Instance.cells[old.cellIndex].transform);
            }
        }
    }

    public void UnlockAll()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.levelNum - 1].screens)
        {
            go.SetActive(false);
        }

        UIManager.Instance.dealButton.interactable = true;

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = false;
            }
        }

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = false;
        }
    }
}

