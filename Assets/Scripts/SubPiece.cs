using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPiece : MonoBehaviour
{
    public PieceSymbol symbolOfPiece;
    public PieceColor colorOfPiece;

    int random;
    public Renderer rend;
    public Renderer symbolHolder;

    public bool isBadConnection;
    public int subPieceIndex;

    public void SetPiece()
    {
        /// Shorten but keep logic
        random = Random.Range(0, GameManager.Instance.clipManager.gameColors.Length);
        rend.material.SetColor("_BaseColor", GameManager.Instance.clipManager.gameColors[random]);
        colorOfPiece = (PieceColor)random;


        random = Random.Range(0, GameManager.Instance.clipManager.gameSymbols.Length);
        symbolHolder.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random].texture);
        symbolOfPiece = (PieceSymbol)random;
    }
}
