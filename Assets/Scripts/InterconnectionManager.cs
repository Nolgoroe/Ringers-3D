using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterconnectionManager : MonoBehaviour
{
    public int lengthOfSubPieces;

    public static InterconnectionManager Instance;
    List<Cell> innerCells;
    List<Cell> outerCells;
    SubPiece[] subPiecesOnBoard;
    SubPiece[] subPiecesDoubleRing;
    private void Start()
    {
        Instance = this;
        innerCells = ConnectionManager.Instance.cells;
        outerCells = ConnectionManager.Instance.outerCells;
        subPiecesOnBoard = ConnectionManager.Instance.subPiecesOnBoard;
        subPiecesDoubleRing = ConnectionManager.Instance.subPiecesDoubleRing;
    }

    public int CheckInterConnection(int cellIndex, bool isOuter) 
    {       
        SubPiece A;
        SubPiece B;
        SubPiece C;
        SubPiece D;

        int newBadConnections = 0;

        if (isOuter)
        {
            Cell interconnectedCellOne = innerCells[CheckIntRange(cellIndex + 1)];
            Cell interconnectedCellTwo = innerCells[CheckIntRange(cellIndex)];

            A = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * cellIndex)];
            C = subPiecesOnBoard[CheckIntRangeSubPiece(2 * cellIndex + 1)];

            B = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * (cellIndex + 1))];
            D = subPiecesOnBoard[CheckIntRangeSubPiece(2 * (cellIndex + 1))];
        }
        else
        {
            Cell interconnectedCellOne = outerCells[CheckIntRange(cellIndex - 1)];
            Cell interconnectedCellTwo = outerCells[CheckIntRange(cellIndex)];

            A = subPiecesOnBoard[CheckIntRangeSubPiece(2 * cellIndex)];
            C = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * (cellIndex -1) + 1)];

            B = subPiecesOnBoard[CheckIntRangeSubPiece(2 * (cellIndex) + 1)];
            D = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * cellIndex)] ;
        }

        if(A && C)
        {
            CompareResault resault1 = ConnectionManager.Instance.TotalCheck(A, C);

            if (!resault1.gColorMatch && !resault1.gSymbolMatch)
            {
                newBadConnections++;
            }
        }

        if (B && D)
        {
            CompareResault resault2 = ConnectionManager.Instance.TotalCheck(B, D);

            if (!resault2.gColorMatch && !resault2.gSymbolMatch)
            {
                newBadConnections++;
            }
        }

        return newBadConnections;
    }


    public int CheckIntRange(int num)
    {
        if (num <= 0)
        {
            return innerCells.Count - 1;
        }

        if (num >= innerCells.Count)
        {
            return 0;
        }

        return num;
    }
    public int CheckIntRangeSubPiece(int num)
    {
        if (num <= 0)
        {
            return subPiecesOnBoard.Length - 1;
        }

        if (num >= subPiecesOnBoard.Length)
        {
            return 0;
        }

        return num;
    }
}
