using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    Red,
    Blue,
    Yellow,
    Magenta,
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
    public SubPiece leftChild, rightChild;

    public bool isLocked;
    public bool partOfBoard;
    public void SetPieces()
    {
        leftChild.SetPiece();
        rightChild.SetPiece();
    }
}
