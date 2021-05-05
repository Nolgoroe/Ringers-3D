using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PieceColor
{
    Purple,
    Blue,
    Yellow,
    Green,
    Joker,
    None
}
[System.Serializable]
public enum PieceSymbol
{
    FireFly,
    Badger,
    Goat,
    Turtle,
    Joker,
    None
}

public class Piece : MonoBehaviour
{
    public SubPiece rightChild, leftChild;
    //public GameObject midPiece;

    public bool isLocked;
    public bool isTutorialLocked;
    public bool partOfBoard;
    public void SetPieces()
    {
        bool isSamePiece = true;
        int repeatIndicator = 0;

        while (isSamePiece)
        {

            if (GameManager.Instance.currentLevel.isTutorial)
            {
                if (repeatIndicator > 0)
                {
                    rightChild.SetPiece();
                    leftChild.SetPiece();

                    repeatIndicator = 0;
                }
                else
                {
                    rightChild.SetPieceTutorial(true);
                    leftChild.SetPieceTutorial(false);
                }


            }
            else
            {
                rightChild.SetPiece();
                leftChild.SetPiece();
            }

            isSamePiece = CheckNoRepeatPiece();

            if (isSamePiece)
            {
                repeatIndicator++;
            }
            else if(!isSamePiece && GameManager.Instance.currentLevel.isTutorial)
            {
                GameManager.Instance.copyOfArrayOfPiecesTutorial.RemoveAt(0);
            }
        }
    }

    public bool CheckNoRepeatPiece()
    {
        Piece currectCheckPiece = GetComponent<Piece>();

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            if (p)
            {
                if (p != currectCheckPiece)
                {
                    bool isSame = ComparerPiece(currectCheckPiece, p);

                    if (isSame)
                    {
                        return true; //// found a repeat so can't continue, is same piece = true
                    }
                }

            }

        }

        return false; //// There was no repeat, is same piece = false
    }

    private bool ComparerPiece(Piece currectCheckPiece, Piece p)
    {
        if ((currectCheckPiece.rightChild.colorOfPiece == p.rightChild.colorOfPiece) && (currectCheckPiece.rightChild.symbolOfPiece == p.rightChild.symbolOfPiece))
        {
            if ((currectCheckPiece.leftChild.colorOfPiece == p.leftChild.colorOfPiece) && (currectCheckPiece.leftChild.symbolOfPiece == p.leftChild.symbolOfPiece))
            {
                Debug.Log("Pieces were the same!" + currectCheckPiece + " " + p);
                return true; ///// Pieces are the same
            }
        }

        return false; //// Pieces are not the same
    }
}
