using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    Purple,
    Blue,
    Yellow,
    Green,
    Joker,
    None
}

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

    public bool isLocked;
    public bool partOfBoard;
    public void SetPieces()
    {
        rightChild.SetPiece();
        leftChild.SetPiece();
    }
}
