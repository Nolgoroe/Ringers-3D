using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubPiece : MonoBehaviour
{
    public PieceSymbol symbolOfPiece;
    public PieceColor colorOfPiece;
    public Color connectedColor;

    //public Renderer rend;
    public Renderer rend;
    //public Renderer symbolHolder;

    public bool isBadConnection;
    //public bool isLocked;
    public int subPieceIndex;

    public Slice relevantSlice;
    int randomColor;
    int randomSymbol;

    Piece parentPiece;

    public bool isRightSubPiece;

    private void Awake()
    {
        parentPiece = transform.GetComponentInParent<Piece>();
    }

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
                indexcSymbol = Random.Range(0, GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex.Length);
            }

            //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            //MeshRenderer r = GetComponent<MeshRenderer>();

            //r.GetPropertyBlock(mpb);
            //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[indexcColor].colorTex[indexcSymbol]);
            //r.SetPropertyBlock(mpb);
            ///////PERFORMACE FOR DRAW CALLS

            //rend.material = GameManager.Instance.clipManager.colorsToMats[indexcColor].colorTex[indexcSymbol];
            rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[indexcColor].colorTex[indexcSymbol]);
            rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[indexcSymbol].symbolTex);
        }
        else
        {
            randomColor = Random.Range(0, GameManager.Instance.clipManager.colorsToMats.Length - 1);
            colorOfPiece = (PieceColor)randomColor;

            randomSymbol = Random.Range(0, GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex.Length);
            symbolOfPiece = (PieceSymbol)randomSymbol;


            //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            //MeshRenderer r = GetComponent<MeshRenderer>();

            //r.GetPropertyBlock(mpb);
            //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex[randomSymbol]);
            //r.SetPropertyBlock(mpb);

            //rend.material = GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex[randomSymbol];
            rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex[randomSymbol]);
            rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[randomSymbol].symbolTex);

        }
    }

    public void SetStonePiece(stonePieceDataStruct SPDS)
    {
        if (SPDS.randomValues && !SPDS.isNeutral)
        {
            SetPiece();
            //rend.material.SetColor("_BaseColor", CursorController.Instance.stonePieceColorTint);
        }
        else if(SPDS.randomValues && SPDS.isNeutral)
        {
            colorOfPiece = PieceColor.None;
            int indexcSymbol = 0;

            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
            {
                randomSymbol = Random.Range(0, GameManager.Instance.currentLevel.levelAvailablesymbols.Length);
                symbolOfPiece = GameManager.Instance.currentLevel.levelAvailablesymbols[randomSymbol];
                indexcSymbol = (int)symbolOfPiece;
            }
            else
            {
                randomSymbol = Random.Range(0, GameManager.Instance.clipManager.colorsToMats[0].colorTex.Length); // we just put 0 since the length is always the same between all colors - safe to assume we'll have the same length of shapes in all colors
                symbolOfPiece = GameManager.Instance.currentLevel.levelAvailablesymbols[randomSymbol];
                indexcSymbol = (int)symbolOfPiece;
            }

            //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            //MeshRenderer r = GetComponent<MeshRenderer>();

            //r.GetPropertyBlock(mpb);
            //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[(int)PieceColor.None].colorTex[indexcSymbol]);
            //r.SetPropertyBlock(mpb);

            //rend.material = GameManager.Instance.clipManager.colorsToMats[(int)PieceColor.None].colorMats[indexcSymbol];

            if (isRightSubPiece)
            {
                rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.corruptedColorsToMatsRight[indexcSymbol]);
                rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[indexcSymbol].symbolTex);
            }
            else
            {
                rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.corruptedColorsToMatsLeft[indexcSymbol]);
                rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[indexcSymbol].symbolTex);
            }
        }
        else
        {
            if (isRightSubPiece)
            {
                colorOfPiece = SPDS.colorOfPieceRight;

                symbolOfPiece = SPDS.symbolOfPieceRight;

                //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                //MeshRenderer r = GetComponent<MeshRenderer>();

                //r.GetPropertyBlock(mpb);
                //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
                //r.SetPropertyBlock(mpb);

                //rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
                if(colorOfPiece == PieceColor.None)
                {
                    rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.corruptedColorsToMatsRight[(int)symbolOfPiece]);
                }
                else
                {
                    rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
                }

                rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[(int)symbolOfPiece].symbolTex);
            }
            else
            {
                colorOfPiece = SPDS.colorOfPieceLeft;

                symbolOfPiece = SPDS.symbolOfPieceLeft;

                //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                //MeshRenderer r = GetComponent<MeshRenderer>();

                //r.GetPropertyBlock(mpb);
                //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
                //r.SetPropertyBlock(mpb);

                //rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
                
                if(colorOfPiece == PieceColor.None)
                {
                    rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.corruptedColorsToMatsLeft[(int)symbolOfPiece]);
                }
                else
                {
                    rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
                }

                rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[(int)symbolOfPiece].symbolTex);
            }
        }

        //rend.material.SetColor("_BaseColor", CursorController.Instance.stonePieceColorTint);
    }
    public void SetPieceTutorial(bool isRight)
    {
        if (isRight)
        {
            colorOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].colorOfPieceRight;

            symbolOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].symbolOfPieceRight;


            //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            //MeshRenderer r = GetComponent<MeshRenderer>();

            //r.GetPropertyBlock(mpb);
            //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
            //r.SetPropertyBlock(mpb);

            //rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
            rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
            rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[(int)symbolOfPiece].symbolTex);
        }
        else
        {
            colorOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].colorOfPieceLeft;

            symbolOfPiece = GameManager.Instance.copyOfArrayOfPiecesTutorial[0].symbolOfPieceLeft;


            //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            //MeshRenderer r = GetComponent<MeshRenderer>();

            //r.GetPropertyBlock(mpb);
            //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
            //r.SetPropertyBlock(mpb);

            //rend.material = GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorMats[(int)symbolOfPiece];
            rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[(int)colorOfPiece].colorTex[(int)symbolOfPiece]);
            rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[(int)symbolOfPiece].symbolTex);
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

        //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        //MeshRenderer r = GetComponent<MeshRenderer>();

        //r.GetPropertyBlock(mpb);
        //mpb.SetTexture("_BaseMap", GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex[(int)symbolOfPiece]);
        //r.SetPropertyBlock(mpb);

        //rend.material = GameManager.Tile_Albedo_Map.clipManager.colorsToMats[randomColor].colorMats[(int)symbolOfPiece];
        rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[randomColor].colorTex[(int)symbolOfPiece]);
        rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[(int)symbolOfPiece].symbolTex);
        //if (rend.materials[0].IsKeywordEnabled("_EMISSION"))
        //{
        //    rend.materials[0].DisableKeyword("_EMISSION");
        //}
    }

    public void SetConnectedMaterial()
    {
        Material mat = GetComponent<Renderer>().material;

        mat.SetInt("Is_Piece_Match", 1);

        Destroy(Instantiate(ConnectionManager.Instance.connectedTilesVFX, transform).gameObject, 1.5f);
        //Debug.Log("what?");
        //List<Material> matArray = new List<Material>();
        //matArray.AddRange(rend.materials);

        //Material mat = GameManager.Instance.clipManager.symbolToMat.Where(p => p.mat == symbolOfPiece).Single().symbolMat;

        //matArray.Add(mat);

        ////matArray[1] = ConnectionManager.Instance.rockLIT;
        //if (!matArray[0].IsKeywordEnabled("_EMISSION"))
        //{
        //    matArray[0].EnableKeyword("_EMISSION");
        //}

        //matArray[0].SetColor("_EmissionColor", Color.black);


        //StartCoroutine(LerpColors(matArray, connectedColor));
        ////matArray[0].SetColor("_EmissionMap", Color.white);
        ////matArray[0].SetColor("_EmissionColor", Color.red);
        //rend.materials = matArray.ToArray();
    }

    public void SetDisconnectedMaterial()
    {
        Material mat = GetComponent<Renderer>().material;

        mat.SetInt("Is_Piece_Match", 0);

        //Debug.Log("what????");

        //List<Material> matArray = new List<Material>();
        //matArray.AddRange(rend.materials);


        //matArray.RemoveAt(1);

        //matArray[1] = ConnectionManager.Instance.rockUnLIT;
        //StartCoroutine(LerpColors(matArray, Color.black));

        //matArray[0].DisableKeyword("_EMISSION");
        //matArray[0].SetColor("_EmissionMap", Color.white);
        //matArray[0].SetColor("_EmissionColor", Color.white);
        //rend.materials = matArray.ToArray();

    }

    //IEnumerator LerpColors(List<Material> matArray, Color targetColor)
    //{

    //    //float timeToLerp = ConnectionManager.Instance.timeToLerpConnectionEmission;
    //    //float timePassed = 0;

    //    //while (matArray[0].GetColor("_EmissionColor") != targetColor)
    //    //{
    //    //    //Debug.Log("???");
    //    //    timePassed += Time.deltaTime;

    //    //    if (timePassed > timeToLerp)
    //    //    {
    //    //        break;
    //    //    }

    //    //    matArray[0].SetColor("_EmissionColor", Color.Lerp(matArray[0].GetColor("_EmissionColor"), targetColor, timePassed/timeToLerp));

    //    //    //Debug.Log(matArray[0].GetColor("_EmissionColor"));

    //        yield return null;
    //    //}

    //    ////FINISH THIS
    //}

    public void SetSpecificSymbol(PieceSymbol symbol)
    {
        int indexcSymbol = (int)symbol;
        symbolOfPiece = symbol;

        if (isRightSubPiece)
        {
            rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.corruptedColorsToMatsRight[indexcSymbol]);
            rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[indexcSymbol].symbolTex);
        }
        else
        {
            rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.corruptedColorsToMatsLeft[indexcSymbol]);
            rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[indexcSymbol].symbolTex);
        }

        //rend.material.SetTexture("Tile_Albedo_Map", GameManager.Instance.clipManager.colorsToMats[(int)PieceColor.None].colorTex[indexcSymbol]);
        //rend.material.SetTexture("MatchedSymbolTex", GameManager.Instance.clipManager.symbolToMat[(int)symbolOfPiece].symbolTex);

    }

    public void SetPieceAsJoker()
    {
        rend.material = GameManager.Instance.powerupManager.jokerMat;
    }

}
