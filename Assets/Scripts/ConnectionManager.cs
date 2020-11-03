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
                if (!CheckSubPieceConnection(subPiecesOnBoard[currentRight], subPiecesOnBoard[rightContested], out bool conditionmet))
                {
                    Debug.Log("Bad Connection Right Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;
                    subPiecesOnBoard[currentRight].isBadConnection = true;
                    subPiecesOnBoard[rightContested].isBadConnection = true;
                    subPiecesOnBoard[currentRight].relevantSlice.fulfilledCondition = false;
                }
                else
                {
                    if (conditionmet)
                    {
                        subPiecesOnBoard[currentRight].relevantSlice.fulfilledCondition = true;

                        if (subPiecesOnBoard[currentRight].relevantSlice.isLoot)
                        {
                            GiveLoot(subPiecesOnBoard[currentRight].relevantSlice, subPiecesOnBoard[currentRight].relevantSlice.isLimiter);
                        }

                        if (subPiecesOnBoard[currentRight].relevantSlice.isLock)
                        {
                            LockCell(subPiecesOnBoard[currentRight].relevantSlice, subPiecesOnBoard[currentRight].relevantSlice.isLimiter);
                        }
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
                if (!CheckSubPieceConnection(subPiecesOnBoard[currentLeft], subPiecesOnBoard[leftContested], out bool conditionmet))
                {
                    Debug.Log("Bad Connection Left Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;
                    subPiecesOnBoard[currentLeft].isBadConnection = true;
                    subPiecesOnBoard[leftContested].isBadConnection = true;
                    subPiecesOnBoard[currentLeft].relevantSlice.fulfilledCondition = false;
                }
                else
                {
                    if (conditionmet)
                    {
                        subPiecesOnBoard[currentLeft].relevantSlice.fulfilledCondition = true;
                        if (subPiecesOnBoard[currentLeft].relevantSlice.isLoot)
                        {
                            GiveLoot(subPiecesOnBoard[currentLeft].relevantSlice, subPiecesOnBoard[currentLeft].relevantSlice.isLimiter);
                        }

                        if (subPiecesOnBoard[currentLeft].relevantSlice.isLock)
                        {
                            LockCell(subPiecesOnBoard[currentLeft].relevantSlice, subPiecesOnBoard[currentLeft].relevantSlice.isLimiter);
                        }
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
    public bool CheckSubPieceConnection(SubPiece currentSide, SubPiece contestedSide, out bool conditionMet)
    {
        bool isGoodConnect = false;
        bool conditionCheck = false;

        if (currentSide.relevantSlice.sliceCatagory != SliceCatagory.None)
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (!currentSide.relevantSlice.isLimiter)
            {

                conditionCheck = CheckFulfilledSliceCondition(currentSide.relevantSlice, result, currentSide, contestedSide);

                if (conditionCheck)
                {
                    conditionMet = conditionCheck;
                    return true;
                }

                if (result.gColorMatch)
                {
                    isGoodConnect = true;
                }

                if (result.gSymbolMatch)
                {
                    isGoodConnect = true;
                }
            }
            else
            {
                conditionCheck = CheckFulfilledSliceCondition(currentSide.relevantSlice, result, currentSide, contestedSide);

                if (conditionCheck)
                {
                    conditionMet = conditionCheck;
                    return true;
                }
                else
                {
                    conditionMet = false;
                    return false;
                }

            }
        }
        else
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (result.gColorMatch)
            {
                isGoodConnect = true;
            }

            if (result.gSymbolMatch)
            {
                isGoodConnect = true;
            }
        }

        conditionMet = conditionCheck;
        return isGoodConnect;
    }
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

    public void GiveLoot(Slice relevent, bool isLimiter)
    {
        Debug.Log("Loot");
        Debug.Log(relevent.lootPack);

        LootManager.Instance.currentLevelLootToGive.Add(relevent.lootPack);
        //LootManager.Instance.RollOnTable(relevent.lootPack);

        if (!isLimiter)
        {
            relevent.child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
            relevent.diamondPrefab.GetComponent<Rigidbody2D>().simulated = true;
            relevent.diamondPrefab.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);
            relevent.isLoot = false;
            Destroy(relevent.diamondPrefab, 2f);
        }
        else
        {
            relevent.isLoot = false;
        }
    }
    public void LockCell(Slice relevent, bool isLimiter)
    {
        Debug.Log("Lock");

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

        if (!isLimiter)
        {
            relevent.child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
            relevent.lockPrefab.GetComponent<Rigidbody2D>().simulated = true;
            relevent.lockPrefab.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);
            relevent.isLock = false;
            Destroy(relevent.lockPrefab, 2f);

        }
        else
        {
            relevent.isLock = false;
        }
    }

    public bool CheckFulfilledSliceCondition(Slice relevent, CompareResault result, SubPiece a, SubPiece b)
    {
        bool isConditionMet = false;

        switch (relevent.sliceCatagory)
        {
            case SliceCatagory.Shape:
                if (result.gSymbolMatch)
                {
                    isConditionMet = true;
                }
                break;
            case SliceCatagory.Color:
                if (result.gColorMatch)
                {
                    isConditionMet = true;
                }
                break;
            case SliceCatagory.SpecificShape:

                if (result.gSymbolMatch)
                {
                    isConditionMet = EqualSymbolOrJoker(a.symbolOfPiece, relevent.sliceSymbol) && EqualSymbolOrJoker(b.symbolOfPiece, relevent.sliceSymbol);
                }

                break;
            case SliceCatagory.SpecificColor:
                if (result.gColorMatch)
                {
                    isConditionMet = EqualColorOrJoker(a.colorOfPiece, relevent.sliceColor) && EqualColorOrJoker(b.colorOfPiece, relevent.sliceColor);
                }
                break;
            default:
                isConditionMet = false;
                break;
        }

        return isConditionMet;
    }
}
