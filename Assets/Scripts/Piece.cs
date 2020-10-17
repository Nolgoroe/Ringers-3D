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
    public GameObject leftChild, rightChild;
    public Renderer lChildRenderer, rChildRenderer;
    public Renderer lChildSymbolHolder, rChildSymbolHolder;

    public PieceColor rColor;
    public PieceSymbol rSymbol;

    public PieceColor lColor;
    public PieceSymbol lSymbol;
    int random;

    public void SetPiece()
    {
        /// Shorten but keep logic
        random = Random.Range(0, GameManager.Instance.clipManager.gameColors.Length);
        lChildRenderer.material.SetColor("_BaseColor", GameManager.Instance.clipManager.gameColors[random]);
        lColor = (PieceColor)random;

        random = Random.Range(0, GameManager.Instance.clipManager.gameColors.Length);
        rChildRenderer.material.SetColor("_BaseColor", GameManager.Instance.clipManager.gameColors[random]);
        rColor = (PieceColor)random;

        random = Random.Range(0, GameManager.Instance.clipManager.gameSymbols.Length);
        lChildSymbolHolder.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random].texture);
        lSymbol = (PieceSymbol)random;

        random = Random.Range(0, GameManager.Instance.clipManager.gameSymbols.Length);
        rChildSymbolHolder.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random].texture);
        rSymbol = (PieceSymbol)random;

    }
}
