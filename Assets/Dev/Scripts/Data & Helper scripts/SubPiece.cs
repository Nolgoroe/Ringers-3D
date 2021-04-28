using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPiece : MonoBehaviour
{
    public PieceSymbol symbolOfPiece;
    public PieceColor colorOfPiece;
    public Color connectedColor;

    public Renderer rend;
    //public Renderer symbolHolder;

    public bool isBadConnection;
    public int subPieceIndex;

    public Slice relevantSlice;
    int randomColor;
    int randomSymbol;

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

        if (rend.materials[0].IsKeywordEnabled("_EMISSION"))
        {
            rend.materials[0].DisableKeyword("_EMISSION");
        }


    }
    public void SetPieceTutorial(bool isRight)
    {
        if (isRight)
        {
            colorOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].colorOfPieceRight;

            symbolOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].symbolOfPieceRight;

            rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
        }
        else
        {
            colorOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].colorOfPieceLeft;

            symbolOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].symbolOfPieceLeft;

            rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
        }

        if (rend.materials[0].IsKeywordEnabled("_EMISSION"))
        {
            rend.materials[0].DisableKeyword("_EMISSION");
        }

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
        Debug.Log("what?");
        Material[] matArray = rend.materials;
        //matArray[1] = ConnectionManager.Instance.rockLIT;
        if (!matArray[0].IsKeywordEnabled("_EMISSION"))
        {
            matArray[0].EnableKeyword("_EMISSION");
        }

        matArray[0].SetColor("_EmissionColor", Color.black);


        StartCoroutine(LerpColors(matArray, connectedColor));
        //matArray[0].SetColor("_EmissionMap", Color.white);
        //matArray[0].SetColor("_EmissionColor", Color.red);
        rend.materials = matArray;
    }

    public void SetDisconnectedMaterial()
    {
        Debug.Log("what????");

        Material[] matArray = rend.materials;
        //matArray[1] = ConnectionManager.Instance.rockUnLIT;
        StartCoroutine(LerpColors(matArray, Color.black));

        //matArray[0].DisableKeyword("_EMISSION");
        //matArray[0].SetColor("_EmissionMap", Color.white);
        //matArray[0].SetColor("_EmissionColor", Color.white);
        rend.materials = matArray;

    }

    IEnumerator LerpColors(Material[] matArray, Color targetColor)
    {

        float timeToLerp = ConnectionManager.Instance.timeToLerpConnectionEmission;
        float timePassed = 0;

        while (matArray[0].GetColor("_EmissionColor") != targetColor)
        {
            Debug.Log("???");
            timePassed += Time.deltaTime;

            if (timePassed > timeToLerp)
            {
                break;
            }

            matArray[0].SetColor("_EmissionColor", Color.Lerp(matArray[0].GetColor("_EmissionColor"), targetColor, timePassed/timeToLerp));

            Debug.Log(matArray[0].GetColor("_EmissionColor"));

            yield return null;
        }
    }
}
