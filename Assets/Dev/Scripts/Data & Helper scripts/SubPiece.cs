using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPiece : MonoBehaviour
{
    public PieceSymbol symbolOfPiece;
    public PieceColor colorOfPiece;

    int random;
    public Renderer rend;
    //public Renderer symbolHolder;

    public bool isBadConnection;
    public int subPieceIndex;

    public Slice relevantSlice;

    public void SetPiece()
    {
        ///// Shorten but keep logic
        //random = Random.Range(0, GameManager.Instance.clipManager.gameColors.Length - 1);
        //rend.material.SetColor("_BaseColor", GameManager.Instance.clipManager.gameColors[random]);
        //colorOfPiece = (PieceColor)random;


        //random = Random.Range(0, GameManager.Instance.clipManager.gameSymbols.Length - 1);
        //rend.material.SetTexture("_BumpMap", GameManager.Instance.clipManager.gameSymbols[random]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random]);
        //symbolOfPiece = (PieceSymbol)random;

        /// Shorten but keep logic
        random = Random.Range(0, GameManager.Instance.clipManager.gameColors.Length - 2);
        rend.material = GameManager.Instance.clipManager.gameColors[random];
        colorOfPiece = (PieceColor)random;


        random = Random.Range(0, GameManager.Instance.clipManager.gameSymbols.Length - 1);
        //rend.material.SetTexture("_BumpMap", GameManager.Instance.clipManager.gameSymbols[random]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random]);
        rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random]);
        symbolOfPiece = (PieceSymbol)random;
    }

    public void RefreshPiece()
    {
        /// Shorten but keep logic
        rend.material = GameManager.Instance.clipManager.gameColors[(int)colorOfPiece];


        //rend.material.SetTexture("_BumpMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
        rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
    }
}
