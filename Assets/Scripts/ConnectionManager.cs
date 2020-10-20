using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConnectionManager : MonoBehaviour
{
    public Cell[] cells;

    public SubPiece[] subPiecesOnBoard;

    public Slice[] slicesOnBoard;

    public int lengthOfSubPieces;

    private void Start()
    {
        GameManager.Instance.connectionManager = this;
        subPiecesOnBoard = new SubPiece[lengthOfSubPieces];
    }
    public void GrabCellList(Transform gb)
    {
        cells = gb.GetComponentsInChildren<Cell>();
        slicesOnBoard = gb.GetComponentsInChildren<Slice>();
    }
    public void CheckConnections(int cellIndex)
    {
        int rightContested = CheckIntRange((cellIndex * 2) - 1);
        int leftContested = CheckIntRange((cellIndex * 2) + 2);

        int currentRight = cellIndex * 2;
        int currentLeft = cellIndex * 2 + 1;

        if (subPiecesOnBoard[rightContested])
        {
            if (subPiecesOnBoard[currentRight])
            {
                if (!CheckSubPieceConnection(subPiecesOnBoard[currentRight], subPiecesOnBoard[rightContested]))
                {
                    Debug.Log("Bad Connection Right Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;
                    subPiecesOnBoard[currentRight].isBadConnection = true;
                    subPiecesOnBoard[rightContested].isBadConnection = true;
                    subPiecesOnBoard[currentRight].relevantSlice.fulfilledCondition = false;
                }
                else
                {
                    subPiecesOnBoard[currentRight].relevantSlice.fulfilledCondition = true;

                    if (subPiecesOnBoard[currentRight].relevantSlice.isLoot)
                    {
                        GiveLoot(subPiecesOnBoard[currentRight].relevantSlice);
                    }

                    if (subPiecesOnBoard[currentRight].relevantSlice.isLock)
                    {
                        LockCell(subPiecesOnBoard[currentRight].relevantSlice);
                    }
                }
            }
            else
            {
                Debug.Log("Bad Connection Right Conetsted");
                GameManager.Instance.unsuccessfullConnectionCount++;
                subPiecesOnBoard[rightContested].isBadConnection = true;
            }
        }
        else
        {
            subPiecesOnBoard[currentRight].relevantSlice.fulfilledCondition = false;
        }

        if (subPiecesOnBoard[leftContested])
        {
            if (subPiecesOnBoard[currentLeft])
            {
                if (!CheckSubPieceConnection(subPiecesOnBoard[currentLeft], subPiecesOnBoard[leftContested]))
                {
                    Debug.Log("Bad Connection Left Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;
                    subPiecesOnBoard[currentLeft].isBadConnection = true;
                    subPiecesOnBoard[leftContested].isBadConnection = true;
                    subPiecesOnBoard[currentLeft].relevantSlice.fulfilledCondition = false;
                }
                else
                {
                    subPiecesOnBoard[currentLeft].relevantSlice.fulfilledCondition = true;
                    if (subPiecesOnBoard[currentLeft].relevantSlice.isLoot)
                    {
                        GiveLoot(subPiecesOnBoard[currentLeft].relevantSlice);
                    }

                    if (subPiecesOnBoard[currentLeft].relevantSlice.isLock)
                    {
                        LockCell(subPiecesOnBoard[currentLeft].relevantSlice);
                    }
                }
            }
            else
            {
                Debug.Log("Bad Connection Right Conetsted");
                GameManager.Instance.unsuccessfullConnectionCount++;
                subPiecesOnBoard[leftContested].isBadConnection = true;
            }
        }
        else
        {
            subPiecesOnBoard[currentLeft].relevantSlice.fulfilledCondition = false;
        }

        /// Get slice
        /// Check list of conditions for correct connections, 2 connection.
        /// 
    }
    public bool CheckSubPieceConnection(SubPiece currentSide, SubPiece contestedSide)
    {
        //int relaventSlice;

        //if (currentSide.subPieceIndex >= lengthOfSubPieces - 1)
        //{
        //    relaventSlice = 0;
        //}
        //else
        //{
        //    relaventSlice = Mathf.CeilToInt((float)currentSide.subPieceIndex / 2);
        //}

        //relavent = slicesOnBoard[relaventSlice];

        bool doColorCheck = true, doShapeCheck = true, doSpecificColor = false, doSpecificShape = false;

        PieceColor col = PieceColor.None;
        PieceSymbol sym = PieceSymbol.None;

        if (currentSide.relevantSlice.sliceCatagory != SliceCatagory.None)
        {
            switch (currentSide.relevantSlice.sliceCatagory)
            {
                case SliceCatagory.Shape:
                    doColorCheck = false;
                    break;
                case SliceCatagory.Color:
                    doShapeCheck = false;
                    break;
                case SliceCatagory.SpecificShape:
                    doShapeCheck = false;
                    doColorCheck = false;
                    doSpecificColor = false;
                    doSpecificShape = true;
                    sym = currentSide.relevantSlice.sliceSymbol;
                    break;
                case SliceCatagory.SpecificColor:
                    doShapeCheck = false;
                    doColorCheck = false;
                    doSpecificShape = false;
                    doSpecificColor = true;
                    col = currentSide.relevantSlice.sliceColor;
                    break;
                default:
                    break;
            }
        }

        CompareResault result = TotalCheck(currentSide, contestedSide);

        if (doColorCheck && result.gColorMatch)
        {
            return true;
        }

        if (doShapeCheck && result.gSymbolMatch)
        {
            return true;
        }

        if (doSpecificColor && result.gColorMatch)
        {
            PieceColor sCol = currentSide.relevantSlice.sliceColor;

            return EqualColorOrJoker(currentSide.colorOfPiece, sCol) && EqualColorOrJoker(contestedSide.colorOfPiece, sCol);
            

            //if (currentSide.colorOfPiece == sCol && contestedSide.colorOfPiece == sCol)
            //{
            //    return true;
            //}
        }

        if (doSpecificShape && result.gSymbolMatch)
        {
            PieceSymbol sSym = currentSide.relevantSlice.sliceSymbol;

            return EqualSymbolOrJoker(currentSide.symbolOfPiece, sSym) && EqualSymbolOrJoker(contestedSide.symbolOfPiece, sSym);

        }

        return false;
        //if (JokerCheck(currentSide, doColorCheck, doShapeCheck) || JokerCheck(contestedSide, doColorCheck, doShapeCheck))
        //{
        //    Debug.Log("Joker Connect");
        //    return true;
        //}

        //if (doColorCheck && ColorCheck(currentSide, contestedSide, col))
        //{
        //    Debug.Log("Color Connect");
        //    return true;
        //}

        //if (doShapeCheck && ShapeCheck(currentSide, contestedSide, sym))
        //{
        //    Debug.Log("Shape Connect");
        //    return true;
        //}

    }

    //public bool ColorCheck(SubPiece rp, SubPiece lp, PieceColor col)
    //{
    //    if (rp.colorOfPiece == lp.colorOfPiece)
    //    {
    //        return true;
    //    }

    //    return false;
    //}
    //public bool ShapeCheck(SubPiece rp, SubPiece lp, PieceSymbol sym)
    //{
    //    if (rp.symbolOfPiece == lp.symbolOfPiece)
    //    {
    //        return true;
    //    }

    //    return false;
    //}
    //public bool JokerCheck(SubPiece sp, bool doColor, bool doSymbol)
    //{
    //    if (doColor && sp.colorOfPiece == PieceColor.Joker)
    //    {
    //        return true;
    //    }

    //    if (doSymbol && sp.symbolOfPiece == PieceSymbol.Joker)
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    public CompareResault TotalCheck(SubPiece current, SubPiece contested/*, PieceColor sCol, PieceSymbol sSym*/)
    {
        CompareResault result = new CompareResault();

        result.gColorMatch = EqualColorOrJoker(current.colorOfPiece, contested.colorOfPiece);
        result.gSymbolMatch = EqualSymbolOrJoker(current.symbolOfPiece, contested.symbolOfPiece);

        //if (sCol != PieceColor.None)
        //{
        //    result.sColorMatch = current.colorOfPiece == contested.colorOfPiece && (current.colorOfPiece == sCol && contested.colorOfPiece == sCol);
        //}

        //if(sSym != PieceSymbol.None)
        //{
        //    result.sSymbolMatch = current.symbolOfPiece == contested.symbolOfPiece && (current.symbolOfPiece == sSym &&  contested.symbolOfPiece == sSym);
        //}

        //Debug.Log(result.gColorMatch);
        //Debug.Log(result.gSymbolMatch);
        return result;
    }
    public int CheckIntRange(int num)
    {
        if (num <= 0)
        {
            return lengthOfSubPieces - 1;
        }

        if (num >= lengthOfSubPieces)
        {
            return 0;
        }

        return num;
    }
    public void FillSubPieceIndex()
    {
        for (int i = 0; i < subPiecesOnBoard.Length; i++)
        {
            if (subPiecesOnBoard[i])
            {
                subPiecesOnBoard[i].subPieceIndex = i;
            }
        }
    }
    public void RemoveSubPieceIndex(int i)
    {
        subPiecesOnBoard[i] = null;
    }
    public bool EqualColorOrJoker(PieceColor colA, PieceColor colB)/// Colorcheck is to see if we need to check color or symbol
    {
        if(colA == colB || (colA == PieceColor.Joker || colB == PieceColor.Joker))
        {
            return true;
        }

        return false;
    }
    public bool EqualSymbolOrJoker(PieceSymbol symA, PieceSymbol symB)/// Colorcheck is to see if we need to check color or symbol
    {
        if (symA == symB || (symA == PieceSymbol.Joker || symB == PieceSymbol.Joker))
        {
            return true;
        }
        return false;

    }

    public void GiveLoot(Slice relevent)
    {
        relevent.child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        relevent.diamondPrefab.GetComponent<Rigidbody2D>().simulated = true;
        relevent.diamondPrefab.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);
        relevent.isLoot = false;
        Destroy(relevent.diamondPrefab, 2f);
    }
    public void LockCell(Slice relevent)
    {
        cells[relevent.sliceIndex].lockSprite.SetActive(true);
        cells[relevent.sliceIndex].pieceHeld.isLocked = true;

        if (relevent.sliceIndex == 0)
        {
            cells[cells.Length - 1].lockSprite.SetActive(true);
            cells[cells.Length - 1].pieceHeld.isLocked = true;
        }
        else
        {
            cells[relevent.sliceIndex - 1].lockSprite.SetActive(true);
            cells[relevent.sliceIndex - 1].pieceHeld.isLocked = true;
        }

        relevent.child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        relevent.lockPrefab.GetComponent<Rigidbody2D>().simulated = true;
        relevent.lockPrefab.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);
        relevent.isLock = false;
        Destroy(relevent.lockPrefab, 2f);
    }
}
