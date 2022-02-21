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
    Red,
    None,
    Joker
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
    public bool partOfBoard;

    public void SetPieces()
    {
        bool isSamePiece = true;
        int repeatIndicator = 0;
        rightChild.isRightSubPiece = true; // this needs to be better


        while (isSamePiece)
        {
            if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
            {
                if (repeatIndicator > 0 || GameManager.Instance.copyOfArrayOfPiecesTutorial.Count <= 0)
                {
                    rightChild.SetPiece();
                    leftChild.SetPiece();

                    repeatIndicator = 0;
                }
                else
                {
                    rightChild.SetPieceTutorial(true);
                    leftChild.SetPieceTutorial(false);
                }
            }
            else
            {
                rightChild.SetPiece();
                leftChild.SetPiece();
            }

            isSamePiece = CheckNoRepeatPieceClip();


            //// To make sure unity doesnt get stuck because of human error - we check to see if we need to default same piece to false.
            //// we default same piece to false when the color OR symbol arrays contain only 1 element.. since all pieces will be the same
            ////  we default same piece to false when the color OR symbol arrays contain more than 1 element, BUT ALL ELEMENTS ARE THE SAME!
            if (GameManager.Instance.currentLevel.levelAvailableColors.Length > 0)
            {
                if(GameManager.Instance.currentLevel.levelAvailableColors.Length == 1)
                {
                    isSamePiece = false;
                }
                else
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
                                isSamePiece = false;
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

            //// To make sure unity doesnt get stuck because of human error - we check to see if we need to default same piece to false.
            //// we default same piece to false when the color OR symbol arrays contain only 1 element.. since all pieces will be the same
            ////  we default same piece to false when the color OR symbol arrays contain more than 1 element, BUT ALL ELEMENTS ARE THE SAME!
            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0) 
            {
                if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length == 1)
                {
                    isSamePiece = false;
                }
                else
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

                            if(same > 1)
                            {
                                Debug.LogError("Found duplicates in the level Available symbols array!");
                            }

                            if(same == GameManager.Instance.currentLevel.levelAvailablesymbols.Length)
                            {
                                isSamePiece = false;
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

            if (isSamePiece)
            {
                repeatIndicator++;
            }
            else if(!isSamePiece && !GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial )
            {
                if (GameManager.Instance.copyOfArrayOfPiecesTutorial.Count > 0)
                {
                    GameManager.Instance.copyOfArrayOfPiecesTutorial.RemoveAt(0);
                }
            }
        }
    }

    public void SetStonePiece(stonePieceDataStruct SPDS)
    {
        bool isRepeatPieceSides = true;
        int repeatIndicator = 0;

        rightChild.SetStonePiece(SPDS);
        leftChild.SetStonePiece(SPDS);

        while (isRepeatPieceSides)
        {
            if (repeatIndicator > 0)
            {
                rightChild.SetStonePiece(SPDS);
                leftChild.SetStonePiece(SPDS);

                repeatIndicator = 0;
            }

            isRepeatPieceSides = CheckNoRepeatPieceSides();


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

                        //if (!isSamePiece)
                        //{
                        //    break;
                        //}
                    }
                }
            }

            if (isRepeatPieceSides)
            {
                repeatIndicator++;
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

    public bool CheckNoRepeatPieceClip()
    {
        Piece currectCheckPiece = GetComponent<Piece>();

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            if (p)
            {
                if (p != currectCheckPiece)
                {
                    bool isSame = ComparerPiece(currectCheckPiece, p);

                    if (isSame)
                    {
                        return true; //// found a repeat so can't continue, is same piece = true
                    }
                }

            }

        }

        return false; //// There was no repeat, is same piece = false
    }

    private bool ComparerPiece(Piece currectCheckPiece, Piece p)
    {
        if ((currectCheckPiece.rightChild.colorOfPiece == p.rightChild.colorOfPiece) && (currectCheckPiece.rightChild.symbolOfPiece == p.rightChild.symbolOfPiece))
        {
            if ((currectCheckPiece.leftChild.colorOfPiece == p.leftChild.colorOfPiece) && (currectCheckPiece.leftChild.symbolOfPiece == p.leftChild.symbolOfPiece))
            {
                Debug.Log("Pieces were the same!" + currectCheckPiece + " " + p);
                return true; ///// Pieces are the same
            }
        }

        return false; //// Pieces are not the same
    }
}
