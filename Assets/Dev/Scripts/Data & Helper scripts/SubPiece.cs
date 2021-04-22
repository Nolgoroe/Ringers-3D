using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPiece : MonoBehaviour
{
    public PieceSymbol symbolOfPiece;
    public PieceColor colorOfPiece;

    int randomColor;
    int randomSymbol;
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

        randomColor = Random.Range(0, GameManager.Instance.clipManager.colorsToMats.Length);
        //random = Random.Range(0, GameManager.Instance.clipManager.gameColors.Length - 2);
        //rend.material = GameManager.Instance.clipManager.gameColors[random];
        colorOfPiece = (PieceColor)randomColor;


        randomSymbol = Random.Range(0, GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats.Length);
        //rend.material.SetTexture("_BumpMap", GameManager.Instance.clipManager.gameSymbols[random]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[random]);
        rend.material = GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats[randomSymbol];
        symbolOfPiece = (PieceSymbol)randomSymbol;
    }

    public void RefreshPiece()
    {
        /// Shorten but keep logic
        //rend.material = GameManager.Instance.clipManager.gameColors[(int)colorOfPiece];
        randomColor = (int)colorOfPiece;


        //rend.material.SetTexture("_BumpMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
        rend.material = GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats[(int)symbolOfPiece];
    }

    public void SetConnectedMaterial()
    {
        Material[] matArray = rend.materials;
        //matArray[1] = ConnectionManager.Instance.rockLIT;
        matArray[0].EnableKeyword("_EMISSION");
        StartCoroutine(LerpColors(matArray, matArray[0].GetColor("_EmissionColor")));
        //matArray[0].SetColor("_EmissionMap", Color.white);
        //matArray[0].SetColor("_EmissionColor", Color.red);
        rend.materials = matArray;
    }

    public void SetDisconnectedMaterial()
    {
        Material[] matArray = rend.materials;
        //matArray[1] = ConnectionManager.Instance.rockUnLIT;
        matArray[0].DisableKeyword("_EMISSION");
        //matArray[0].SetColor("_EmissionMap", Color.white);
        //matArray[0].SetColor("_EmissionColor", Color.white);
        rend.materials = matArray;

    }

    IEnumerator LerpColors(Material[] matArray, Color targetColor)
    {
        matArray[0].SetColor("_EmissionColor", Color.black);

        while (matArray[0].GetColor("_EmissionColor") != targetColor)
        {
            matArray[0].SetColor("_EmissionColor", Color.Lerp(Color.black, targetColor, Mathf.Lerp(0, 1)));
            yield return null;
        }
    }
}
