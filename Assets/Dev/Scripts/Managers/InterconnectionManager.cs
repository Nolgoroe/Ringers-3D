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

    Cell interconnectedCellOne;
    Cell interconnectedCellTwo;

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
            interconnectedCellOne = innerCells[CheckIntRange(cellIndex)];
            interconnectedCellTwo = innerCells[CheckIntRange(cellIndex + 1) ];

            Debug.Log(interconnectedCellOne);
            Debug.Log(interconnectedCellTwo);

            A = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * cellIndex)];
            C = subPiecesOnBoard[CheckIntRangeSubPiece(2 * cellIndex + 1)];

            B = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * (cellIndex )+ 1)];
            D = subPiecesOnBoard[CheckIntRangeSubPiece(2 * (cellIndex + 1))];
        }
        else
        {
            interconnectedCellOne = outerCells[CheckIntRange(cellIndex)];
            interconnectedCellTwo = outerCells[CheckIntRange(cellIndex - 1)];
            Debug.Log(interconnectedCellOne);
            Debug.Log(interconnectedCellTwo);

            A = subPiecesOnBoard[CheckIntRangeSubPiece(2 * cellIndex)];
            C = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * (cellIndex -1) + 1)];

            B = subPiecesOnBoard[CheckIntRangeSubPiece(2 * (cellIndex) + 1)];
            D = subPiecesDoubleRing[CheckIntRangeSubPiece(2 * cellIndex)] ;
        }

        if(A && C)
        {
            Debug.Log("A + C" + A.symbolOfPiece + " " + C.symbolOfPiece);
            CompareResault resault1 = ConnectionManager.Instance.TotalCheck(A, C);

            if (!resault1.gColorMatch && !resault1.gSymbolMatch)
            {
                newBadConnections++;
                if (isOuter)
                {
                    //Instantiate(ConnectionManager.Instance.badConnectionParticle, interconnectedCellOne.interconnectedRightParticleZone);
                }
                else
                {
                    //Instantiate(ConnectionManager.Instance.badConnectionParticle, interconnectedCellTwo.interconnectedRightParticleZone);
                }
            }
            else
            {
                if (isOuter)
                {
                    //Instantiate(ConnectionManager.Instance.goodConnectionParticle, interconnectedCellOne.interconnectedRightParticleZone);
                }
                else
                {
                    //Instantiate(ConnectionManager.Instance.goodConnectionParticle, interconnectedCellTwo.interconnectedRightParticleZone);
                }
            }
        }

        if (B && D)
        {
            Debug.Log("B + D" + B.symbolOfPiece + " " + D.symbolOfPiece);

            CompareResault resault2 = ConnectionManager.Instance.TotalCheck(B, D);

            if (!resault2.gColorMatch && !resault2.gSymbolMatch)
            {
                newBadConnections++;
                if (isOuter)
                {
                    //Instantiate(ConnectionManager.Instance.badConnectionParticle, interconnectedCellTwo.interconnectedLeftParticleZone);
                }
                else
                {
                    //Instantiate(ConnectionManager.Instance.badConnectionParticle, interconnectedCellOne.interconnectedLeftParticleZone);

                }
            }
            else
            {
                if (isOuter)
                {
                    //Instantiate(ConnectionManager.Instance.goodConnectionParticle, interconnectedCellTwo.interconnectedLeftParticleZone);
                }
                else
                {
                    //Instantiate(ConnectionManager.Instance.goodConnectionParticle, interconnectedCellOne.interconnectedLeftParticleZone);
                }
            }
        }

        return newBadConnections;
    }


    public int CheckIntRange(int num)
    {
        if (num < 0)
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
        if (num < 0)
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
