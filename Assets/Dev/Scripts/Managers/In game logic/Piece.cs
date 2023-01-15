using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PieceColor
{
    Purple,
    Blue,
    Yellow,
    Green,
    Pink,
    General,
    Stone,
    Joker,
}
[System.Serializable]
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
    public SubPiece rightChild, leftChild;
    //public GameObject midPiece;

    public bool isLocked;
    public bool isStone;
    public bool isTutorialLocked;
    public bool isDuringConnectionAnim;
    public bool partOfBoard;

    public void SetPieces()
    {
        //if(!GameManager.Instance.currentLevel.allowRepeatTiles && !GameManager.Instance.currentLevel.allowRepeatTileSides)
        //{

        //    return;
        //}

        int isSamePiece = 0;
        bool isRepeatPieceSides = true;

        int repeatIndicator = 1;
        rightChild.isRightSubPiece = true; // this needs to be better

        int repeatBlock = 0;

        while (repeatIndicator > 0)
        {
            repeatBlock++;

            if (repeatBlock > 100)
            {
                Debug.LogError("Bug in tile generation - check boolean values");
                return;
            }

            if (!GameManager.Instance.isDisableTutorials && (GameManager.Instance.currentLevel.isTutorial || GameManager.Instance.currentLevel.isSpecificTutorial))
            {
                if(GameManager.Instance.copyOfArrayOfPiecesTutorial.Count > 0)
                {
                    rightChild.SetPieceTutorial(true);
                    leftChild.SetPieceTutorial(false);
                }
                else
                {
                    if (repeatIndicator > 0)
                    {
                        rightChild.SetPiece();
                        leftChild.SetPiece();
                    }
                }
            }
            else
            {
                rightChild.SetPiece();
                leftChild.SetPiece();
            }

            isSamePiece = CheckNoRepeatPieceClip();
            //Debug.LogError(isSamePiece);
            isRepeatPieceSides = CheckNoRepeatPieceSidesNormalPiece();


            if (GameManager.Instance.currentLevel.levelAvailableColors.Length == 1 && GameManager.Instance.currentLevel.levelAvailablesymbols.Length == 1)
            {
                isSamePiece = 0;
                isRepeatPieceSides = false;
            }
            else
            {
                //// To make sure unity doesnt get stuck because of human error - we check to see if we need to default same piece to false.
                //// we default same piece to false when the color OR symbol arrays contain only 1 element.. since all pieces will be the same
                ////  we default same piece to false when the color OR symbol arrays contain more than 1 element, BUT ALL ELEMENTS ARE THE SAME!
                if (GameManager.Instance.currentLevel.levelAvailableColors.Length > 1)
                {
                    for (int i = 0; i < System.Enum.GetValues(typeof(PieceColor)).Length; i++)
                    {
                        int same = 0;
                        foreach (PieceColor PC in GameManager.Instance.currentLevel.levelAvailableColors)
                        {
                            if (PC == (PieceColor)i)
                            {
                                same++;
                            }

                            if (same > 1)
                            {
                                Debug.LogError("Found duplicates in the level Available colors array!");
                            }

                            if (same == GameManager.Instance.currentLevel.levelAvailableColors.Length)
                            {
                                isSamePiece = 0;
                                isRepeatPieceSides = false;
                                break;
                            }
                        }

                        //if (!isSamePiece)
                        //{
                        //    break;
                        //}
                    }
                }

                //// To make sure unity doesnt get stuck because of human error - we check to see if we need to default same piece to false.
                //// we default same piece to false when the color AND symbol arrays contain only 1 element.. since all pieces will be the same
                ////  we default same piece to false when the color OR symbol arrays contain more than 1 element, BUT ALL ELEMENTS ARE THE SAME!
                if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 1)
                {
                    for (int i = 0; i < System.Enum.GetValues(typeof(PieceSymbol)).Length; i++)
                    {
                        int same = 0;
                        foreach (PieceSymbol PS in GameManager.Instance.currentLevel.levelAvailablesymbols)
                        {
                            if (PS == (PieceSymbol)i)
                            {
                                same++;
                            }

                            if (same > 1)
                            {
                                Debug.LogError("Found duplicates in the level Available symbols array!");
                            }

                            if (same == GameManager.Instance.currentLevel.levelAvailablesymbols.Length)
                            {
                                isSamePiece = 0;
                                isRepeatPieceSides = false;
                                break;
                            }
                        }

                        //if (!isSamePiece)
                        //{
                        //    break;
                        //}
                    }
                }
            }

            repeatIndicator = 0;

            if (isSamePiece >= 2)
            {
                repeatIndicator++;
            }
            
            if (isSamePiece > 0 && !GameManager.Instance.currentLevel.allowRepeatTiles)
            {
                repeatIndicator++;
            }
            else if(isRepeatPieceSides && !GameManager.Instance.currentLevel.allowRepeatTileSides)
            {
                repeatIndicator++;
            }
            else /*if(!isRepeatPieceSides && !GameManager.Instance.isDisableTutorials && (GameManager.Instance.currentLevel.isTutorial || GameManager.Instance.currentLevel.isSpecificTutorial))*/
            {
                if (GameManager.Instance.copyOfArrayOfPiecesTutorial.Count > 0)
                {
                    GameManager.Instance.copyOfArrayOfPiecesTutorial.RemoveAt(0);
                }
            }

            isRepeatPieceSides = false;
            isSamePiece = 0;

        }
    }
    public void SetPiecesSpecificData(EdgePathFoundData dataNeeded)
    {
        rightChild.SetPieceSpecific(dataNeeded.rightAnimalSymbolNeeded, dataNeeded.rightColorNeeded);
        leftChild.SetPieceSpecific(dataNeeded.leftAnimalSymbolNeeded, dataNeeded.leftColorNeeded);
    }

    public void SetStonePiece(stonePieceDataStruct SPDS)
    {
        bool isRepeatPieceSides = true;
        bool isRepeatPieceOnBoard = true;
        bool hasEnoughSymbols = true;
        int repeatIndicator = 0;

        rightChild.SetStonePiece(SPDS);
        leftChild.SetStonePiece(SPDS);

        int errorBreakerCount = 0;

        while (isRepeatPieceSides || isRepeatPieceOnBoard || !hasEnoughSymbols)
        {
            errorBreakerCount++;

            if(errorBreakerCount >= 1000)
            {
                Debug.LogError("Problem here!");
                return;
            }

            if (repeatIndicator > 0)
            {
                rightChild.SetStonePiece(SPDS);
                leftChild.SetStonePiece(SPDS);

                repeatIndicator = 0;
            }

            isRepeatPieceSides = CheckNoRepeatPieceSides();
            isRepeatPieceOnBoard  = ConnectionManager.Instance.CheckRepeatingStonePieces(this);


            //Debug.LogError("Same Pieces? " + isRepeatPieceSides);
             Debug.LogError("Same Pieces on board? " + isRepeatPieceOnBoard);

            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
            {
                if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length == 1)
                {
                    isRepeatPieceSides = false;
                }
                else
                {
                    // This loop checkes to see if someone accidentaly filled the "levelAvailablesymbols" list
                    // with multiples of the same symbol - like 3 times goat.
                    for (int i = 0; i < System.Enum.GetValues(typeof(PieceSymbol)).Length; i++)
                    {
                        int same = 0;
                        foreach (PieceSymbol PS in GameManager.Instance.currentLevel.levelAvailablesymbols)
                        {
                            if (PS == (PieceSymbol)i)
                            {
                                same++;
                            }

                            if (same > 1)
                            {
                                Debug.LogError("Found duplicates in the level Available symbols array!");
                            }


                            if (same == GameManager.Instance.currentLevel.levelAvailablesymbols.Length)
                            {
                                isRepeatPieceSides = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (isRepeatPieceSides || isRepeatPieceOnBoard)
            {
                repeatIndicator++;
            }


            if (!isRepeatPieceSides && !isRepeatPieceOnBoard)
            {
                if (!ConnectionManager.Instance.tempSymbolPiecesStoneFound.Contains(leftChild.symbolOfPiece))
                {
                    ConnectionManager.Instance.tempSymbolPiecesStoneFound.Add(leftChild.symbolOfPiece);
                }

                if (!ConnectionManager.Instance.tempSymbolPiecesStoneFound.Contains(rightChild.symbolOfPiece))
                {
                    ConnectionManager.Instance.tempSymbolPiecesStoneFound.Add(rightChild.symbolOfPiece);
                }
            }

            if (ConnectionManager.Instance.amountStonePiecesInstantiated > 1)
            {
                hasEnoughSymbols = ConnectionManager.Instance.CheckHasEnoughSymbolsStonePieces(this);
            }
        }


        //rightChild.SetStonePiece(SPDS, true);
        //leftChild.SetStonePiece(SPDS, false);
    }

    public bool CheckNoRepeatPieceSides() // for now it's only for stone pieces
    {
        Piece currectCheckPiece = GetComponent<Piece>();

        if(currectCheckPiece.rightChild.symbolOfPiece == currectCheckPiece.leftChild.symbolOfPiece)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool CheckNoRepeatPieceSidesNormalPiece() // for now it's only for stone pieces
    {
        Piece currectCheckPiece = GetComponent<Piece>();

        if(currectCheckPiece.rightChild.symbolOfPiece == currectCheckPiece.leftChild.symbolOfPiece && currectCheckPiece.rightChild.colorOfPiece == currectCheckPiece.leftChild.colorOfPiece)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public int CheckNoRepeatPieceClip()
    {
        Piece currectCheckPiece = GetComponent<Piece>();
        int amount = 0;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponent<ClipHolder>().heldPiece;

            if (p)
            {
                if (p != currectCheckPiece)
                {
                    bool isSame = ConnectionManager.Instance.ComparerPiece(currectCheckPiece, p);

                    if (isSame)
                    {
                        amount++;
                    }
                }

            }

        }

        return amount;
    }

    public void TransformTo12RingTile()
    {
        MeshFilter meshFilterLeft = leftChild.GetComponent<MeshFilter>();
        MeshCollider meshColliderLeft = leftChild.GetComponent<MeshCollider>();

        MeshFilter meshFilterRight = rightChild.GetComponent<MeshFilter>();
        MeshCollider meshColliderRight = rightChild.GetComponent<MeshCollider>();

        Mesh tile12RingLeft = GameManager.Instance.clipManager.tile12RingLeftSubPieceMesh;
        Mesh tile12RingRight = GameManager.Instance.clipManager.tile12RingRightSubPieceMesh;

        meshFilterLeft.mesh = tile12RingLeft;
        meshColliderLeft.sharedMesh = tile12RingLeft;

        meshFilterRight.mesh = tile12RingRight;
        meshColliderRight.sharedMesh = tile12RingRight;

        leftChild.SetPieceAs12RingPiece();
        rightChild.SetPieceAs12RingPiece();

        if(leftChild.symbolOfPiece == PieceSymbol.Joker)
        {
            Material mat = leftChild.GetComponent<Renderer>().material;

            mat.SetTexture("Glass_Overlay", GameManager.Instance.powerupManager.ring12TileRingJokerTex);
        }

        if(rightChild.symbolOfPiece == PieceSymbol.Joker)
        {
            Material mat = rightChild.GetComponent<Renderer>().material;

            mat.SetTexture("Glass_Overlay", GameManager.Instance.powerupManager.ring12TileRingJokerTex);
        }
    }

    public void TransformTo8RingTile()
    {
        MeshFilter meshFilterLeft = leftChild.GetComponent<MeshFilter>();
        MeshCollider meshColliderLeft = leftChild.GetComponent<MeshCollider>();

        MeshFilter meshFilterRight = rightChild.GetComponent<MeshFilter>();
        MeshCollider meshColliderRight = rightChild.GetComponent<MeshCollider>();

        Mesh tile8RingLeft = GameManager.Instance.clipManager.tile8RingLeftSubPieceMesh;
        Mesh tile8RingRight = GameManager.Instance.clipManager.tile8RingRightSubPieceMesh;

        meshFilterLeft.mesh = tile8RingLeft;
        meshColliderLeft.sharedMesh = tile8RingLeft;

        meshFilterRight.mesh = tile8RingRight;
        meshColliderRight.sharedMesh = tile8RingRight;


        leftChild.SetPieceAs8RingPiece();
        rightChild.SetPieceAs8RingPiece();
    }
}
