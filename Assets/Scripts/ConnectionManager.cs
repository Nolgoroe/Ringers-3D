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
            if (!CheckSubPieceConnection(subPiecesOnBoard[currentRight], subPiecesOnBoard[rightContested]))
            {
                Debug.Log("Bad Connection Right Conetsted");
                GameManager.Instance.unsuccessfullConnectionCount++;
                subPiecesOnBoard[currentRight].isBadConnection = true;
                subPiecesOnBoard[rightContested].isBadConnection = true;
            }
        }

        if (subPiecesOnBoard[leftContested])
        {
            if (!CheckSubPieceConnection(subPiecesOnBoard[currentLeft], subPiecesOnBoard[leftContested]))
            {
                Debug.Log("Bad Connection Left Conetsted");
                GameManager.Instance.unsuccessfullConnectionCount++;
                subPiecesOnBoard[currentLeft].isBadConnection = true;
                subPiecesOnBoard[leftContested].isBadConnection = true;
            }
        }

        /// Get slice
        /// Check list of conditions for correct connections, 2 connection.
        /// 
    }
    public bool CheckSubPieceConnection(SubPiece currentSide, SubPiece contestedSide)
    {
        int relaventSlice;

        if (currentSide.subPieceIndex >= lengthOfSubPieces - 1)
        {
            relaventSlice = 0;
        }
        else
        {
            relaventSlice = Mathf.CeilToInt((float)currentSide.subPieceIndex / 2);
        }

        bool doColorCheck = true, doShapeCheck = true;

        if (slicesOnBoard[relaventSlice].sliceCatagory != SliceCatagory.None)
        {
            switch (slicesOnBoard[relaventSlice].sliceCatagory)
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
                    return SliceComparerSymbol(currentSide, contestedSide, slicesOnBoard[relaventSlice].sliceSymbol);
                case SliceCatagory.SpecificColor:
                    doShapeCheck = false;
                    doColorCheck = false;
                    return SliceComparerColor(currentSide, contestedSide, slicesOnBoard[relaventSlice].sliceColor);
                default:
                    break;
            }
        }

        if (JokerCheck(currentSide, doColorCheck, doShapeCheck) || JokerCheck(contestedSide, doColorCheck, doShapeCheck))
        {
            Debug.Log("Joker Connect");
            return true;
        }

        if (doColorCheck && ColorCheck(currentSide, contestedSide))
        {
            Debug.Log("Color Connect");
            return true;
        }

        if (doShapeCheck && SymbolCheck(currentSide, contestedSide))
        {
            Debug.Log("Shape Connect");
            return true;
        }

        return false;
    }
    public bool ColorCheck(SubPiece rp, SubPiece lp)
    {
        if (rp.colorOfPiece == lp.colorOfPiece)
        {
            return true;
        }

        return false;
    }
    public bool SymbolCheck(SubPiece rp, SubPiece lp)
    {
        if (rp.symbolOfPiece == lp.symbolOfPiece)
        {
            return true;
        }

        return false;
    }
    public bool JokerCheck(SubPiece sp, bool doColor, bool doSymbol)
    {
        if (doColor && sp.colorOfPiece == PieceColor.Joker)
        {
            return true;
        }

        if (doSymbol && sp.symbolOfPiece == PieceSymbol.Joker)
        {
            return true;
        }

        return false;
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
    public bool SliceComparerColor(SubPiece current, SubPiece contested, PieceColor colorOfSlice)
    {
        bool jokerSlice = JokerCheck(current , true, false);

        if ((current.colorOfPiece == contested.colorOfPiece && current.colorOfPiece == colorOfSlice) || jokerSlice)
        {
            Debug.Log("Good by slice color");
            return true;
        }

        return false;
    }
    public bool SliceComparerSymbol(SubPiece current, SubPiece contested, PieceSymbol symbolOfSlice)
    {
        bool jokerSlice = JokerCheck(current, false, true);

        if ((current.symbolOfPiece == contested.symbolOfPiece && current.symbolOfPiece == symbolOfSlice)|| jokerSlice)
        {
            Debug.Log("Good by slice symbol");

            return true;
        }
        return false;
    }
}
