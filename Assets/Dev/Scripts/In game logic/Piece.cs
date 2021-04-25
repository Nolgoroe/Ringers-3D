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
    public bool partOfBoard;
    public void SetPieces()
    {
        if (GameManager.Instance.currentLevel.isTutorial)
        {
            rightChild.SetPieceTutorial(true);
            leftChild.SetPieceTutorial(false);
        }
        else
        {
            rightChild.SetPiece();
            leftChild.SetPiece();
        }

        GameManager.Instance.copyOfArrayOfPiecesTutorial.RemoveAt(0);
    }
}
