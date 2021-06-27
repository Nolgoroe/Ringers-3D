using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        if(GameManager.Instance.currentLevel.levelAvailableColors.Length > 0 || GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
        {
            int indexcColor = 0;
            int indexcSymbol = 0;

            if (GameManager.Instance.currentLevel.levelAvailableColors.Length > 0)
            {
                randomColor = Random.Range(0, GameManager.Instance.currentLevel.levelAvailableColors.Length);
                colorOfPiece = GameManager.Instance.currentLevel.levelAvailableColors[randomColor];
                indexcColor = System.Array.IndexOf(GameManager.Instance.clipManager.colorsToMats, GameManager.Instance.clipManager.colorsToMats.Where(p => p.matColor == colorOfPiece).Single());
            }
            else
            {
                indexcColor = Random.Range(0, GameManager.Instance.clipManager.colorsToMats.Length);
            }

            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
            {
                randomSymbol = Random.Range(0, GameManager.Instance.currentLevel.levelAvailablesymbols.Length);
                symbolOfPiece = GameManager.Instance.currentLevel.levelAvailablesymbols[randomSymbol];
                indexcSymbol = (int)symbolOfPiece;
            }
            else
            {
                indexcSymbol = Random.Range(0, GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats.Length);
            }

            rend.material = GameManager.Instance.clipManager.colorsToMats[indexcColor].colorMats[indexcSymbol];
        }
        else
        {
            randomColor = Random.Range(0, GameManager.Instance.clipManager.colorsToMats.Length);
            colorOfPiece = (PieceColor)randomColor;

            randomSymbol = Random.Range(0, GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats.Length);
            symbolOfPiece = (PieceSymbol)randomSymbol;

            rend.material = GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats[randomSymbol];
        }
    }

    public void SetStonePiece(stonePieceDataStruct SPDS, bool isRight)
    {
        if (SPDS.randomValues)
        {
            SetPiece();
            rend.material.SetColor("_BaseColor", CursorController.Instance.stonePieceColorTInt);
        }
        else
        {
            if (isRight)
            {
                colorOfPiece = SPDS.colorOfPieceRight;

                symbolOfPiece = SPDS.symbolOfPieceRight;

                rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
            }
            else
            {
                colorOfPiece = SPDS.colorOfPieceLeft;

                symbolOfPiece = SPDS.symbolOfPieceLeft;

                rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
            }

            rend.material.SetColor("_BaseColor", new Color(210, 210, 210, rend.material.color.a));
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

        //if (rend.materials[0].IsKeywordEnabled("_EMISSION"))
        //{
        //    rend.materials[0].DisableKeyword("_EMISSION");
        //}

    }

    public void RefreshPiece()
    {
        /// Shorten but keep logic
        //rend.material = GameManager.Instance.clipManager.gameColors[(int)colorOfPiece];
        randomColor = (int)colorOfPiece;


        //rend.material.SetTexture("_BumpMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
        //rend.material.SetTexture("_BaseMap", GameManager.Instance.clipManager.gameSymbols[(int)symbolOfPiece]);
        rend.material = GameManager.Instance.clipManager.colorsToMats[randomColor].colorMats[(int)symbolOfPiece];

        if (rend.materials[0].IsKeywordEnabled("_EMISSION"))
        {
            rend.materials[0].DisableKeyword("_EMISSION");
        }
    }

    public void SetConnectedMaterial()
    {
        //Debug.Log("what?");
        List<Material> matArray = new List<Material>();
        matArray.AddRange(rend.materials);

        Material mat = GameManager.Instance.clipManager.symbolToMat.Where(p => p.mat == symbolOfPiece).Single().symbolMat;

        matArray.Add(mat);

        ////matArray[1] = ConnectionManager.Instance.rockLIT;
        //if (!matArray[0].IsKeywordEnabled("_EMISSION"))
        //{
        //    matArray[0].EnableKeyword("_EMISSION");
        //}

        //matArray[0].SetColor("_EmissionColor", Color.black);


        //StartCoroutine(LerpColors(matArray, connectedColor));
        ////matArray[0].SetColor("_EmissionMap", Color.white);
        ////matArray[0].SetColor("_EmissionColor", Color.red);
        rend.materials = matArray.ToArray();
    }

    public void SetDisconnectedMaterial()
    {
        //Debug.Log("what????");

        List<Material> matArray = new List<Material>();
        matArray.AddRange(rend.materials);


        matArray.RemoveAt(1);

        //matArray[1] = ConnectionManager.Instance.rockUnLIT;
        //StartCoroutine(LerpColors(matArray, Color.black));

        //matArray[0].DisableKeyword("_EMISSION");
        //matArray[0].SetColor("_EmissionMap", Color.white);
        //matArray[0].SetColor("_EmissionColor", Color.white);
        rend.materials = matArray.ToArray();

    }

    IEnumerator LerpColors(List<Material> matArray, Color targetColor)
    {

        float timeToLerp = ConnectionManager.Instance.timeToLerpConnectionEmission;
        float timePassed = 0;

        while (matArray[0].GetColor("_EmissionColor") != targetColor)
        {
            //Debug.Log("???");
            timePassed += Time.deltaTime;

            if (timePassed > timeToLerp)
            {
                break;
            }

            matArray[0].SetColor("_EmissionColor", Color.Lerp(matArray[0].GetColor("_EmissionColor"), targetColor, timePassed/timeToLerp));

            //Debug.Log(matArray[0].GetColor("_EmissionColor"));

            yield return null;
        }
    }

}
