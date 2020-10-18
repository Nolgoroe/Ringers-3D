using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    Red,
    Blue,
    Yellow,
    Magenta,
    None,
    Joker
}

public enum PieceSymbol
{
    FireFly,
    Badger,
    Goat,
    Turtle,
    None,
    Joker

}

public class Piece : MonoBehaviour
{
    public SubPiece leftChild, rightChild;

    public void SetPieces()
    {
        leftChild.SetPiece();
        rightChild.SetPiece();
    }
    //public PieceColor rColor;
    //public PieceSymbol rSymbol;

    //public PieceColor lColor;
    //public PieceSymbol lSymbol;

}
