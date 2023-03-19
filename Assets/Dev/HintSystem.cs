using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintSystem : MonoBehaviour
{
    [SerializeField] private int idleTimeHint = 3;
    [SerializeField] private float currentIdleTime = 0;

    [SerializeField] private bool canShowHint;

    [SerializeField] private List<Cell> stoneTilesInCellsInRing;
    [SerializeField] private List<Slice> specificSymbolLimiters;
    [SerializeField] private List<Slice> specificColorLimiters;
    [SerializeField] private List<Slice> generalSymbolLimiters;
    [SerializeField] private List<Slice> generalColorLimiters;

    [SerializeField] private List<Cell> currentCellsToCheck;

    private  List<Cell> cells;

    private void Start()
    {
        cells = new List<Cell>();
    }

    void Update()
    {
        if(GameManager.Instance.levelStarted)
        {
            if (Input.touchCount > 0)
            {
                canShowHint = false;
                currentIdleTime = 0;
            }

            if (!canShowHint)
            {
                currentIdleTime += Time.deltaTime;

                if (currentIdleTime >= idleTimeHint)
                {
                    canShowHint = true;
                    CheckHintCorruptedTiles();
                }
            }
        }
    }

    private void ResetTimer()
    {
        canShowHint = false;
        currentIdleTime = 0;
    }

    private void CheckHintCorruptedTiles()
    {
        currentCellsToCheck.Clear();

        //find all empty cells next to corrupted tiles
        if (stoneTilesInCellsInRing.Count > 0)
        {
            foreach (Cell cell in stoneTilesInCellsInRing)
            {
                int indexOfCell = cell.cellIndex;

                int toCheck = CheckIntRangeCells(indexOfCell - 1, cells);

                if (!cells[toCheck].pieceHeld)
                {
                    if(!currentCellsToCheck.Contains(cells[toCheck]))
                    {
                        currentCellsToCheck.Add(cells[toCheck]);
                    }
                }

                toCheck = CheckIntRangeCells(indexOfCell + 1, cells);

                if (!cells[toCheck].pieceHeld)
                {
                    if (!currentCellsToCheck.Contains(cells[toCheck]))
                    {
                        currentCellsToCheck.Add(cells[toCheck]);
                    }
                }
            }

            bool foundHint = CheckConnectionWithCellsAndTiles();

            if (foundHint)
            {
                return;
            }
        }


        //if we got here we did not find a match for corrupted tiles, continue to next blocker.
        CheckHintSpecificLimiter(specificSymbolLimiters, SliceCatagory.SpecificShape);
    }

    private void CheckHintSpecificLimiter(List<Slice> sliceList, SliceCatagory currentCategory)
    {
        currentCellsToCheck.Clear();

        if (sliceList.Count > 0)
        {
            foreach (Slice slice in sliceList)
            {
                int indexOfSlice = slice.sliceIndex;

                int indexOfCellLeft = indexOfSlice;
                int indexOfCellRight = indexOfSlice - 1;

                int toCheck = CheckIntRangeCells(indexOfCellLeft, cells);

                if (!cells[toCheck].pieceHeld)
                {
                    if (!currentCellsToCheck.Contains(cells[toCheck]))
                    {
                        currentCellsToCheck.Add(cells[toCheck]);
                    }
                }

                toCheck = CheckIntRangeCells(indexOfCellRight, cells);

                if (!cells[toCheck].pieceHeld)
                {
                    if (!currentCellsToCheck.Contains(cells[toCheck]))
                    {
                        currentCellsToCheck.Add(cells[toCheck]);
                    }
                }

            }

            bool foundHint = CheckConnectionWithCellsAndTiles();

            if (foundHint)
            {
                return;
            }

            switch (currentCategory)
            {
                case SliceCatagory.Shape:
                    CheckHintSpecificLimiter(generalColorLimiters, SliceCatagory.Color);
                    break;
                case SliceCatagory.Color:
                    CheckHintSpecificLimiter(null, SliceCatagory.None);
                    break;
                case SliceCatagory.SpecificShape:
                    CheckHintSpecificLimiter(specificColorLimiters, SliceCatagory.SpecificColor);
                    break;
                case SliceCatagory.SpecificColor:
                    CheckHintSpecificLimiter(generalSymbolLimiters, SliceCatagory.Shape);
                    break;
                default:
                    break;
            }
        }


        //if we got here we did not find a match for any slices, continue to next blocker (normal).
        CheckHintNormalCells();
    }

    private void CheckHintNormalCells()
    {
        currentCellsToCheck.Clear();

        foreach (Cell cell in cells)
        {
            if (!cell.pieceHeld)
            {
                if (!currentCellsToCheck.Contains(cell))
                {
                    currentCellsToCheck.Add(cell);
                }
            }
        }

        bool foundHint = CheckConnectionWithCellsAndTiles();

        if (foundHint)
        {
            return;
        }

        DealHint();
    }

    private void DealHint()
    {
        //show deal hint
    }

    private bool CheckConnectionWithCellsAndTiles()
    {
        //do hint algo on the empty tiles
        if (currentCellsToCheck.Count > 0)
        {
            foreach (Cell cell in currentCellsToCheck)
            {
                int toCheckLeft = CheckIntRangeCells(cell.cellIndex - 1, cells);
                int toCheckRight = CheckIntRangeCells(cell.cellIndex + 1, cells);

                if (cells[toCheckLeft].pieceHeld && cells[toCheckRight].pieceHeld)
                {
                    //cell is surrounded by pieces
                    Debug.Log("Surrounded cell index: " + cell.cellIndex);

                    ClipHolder match = CheckAvailableTileInClip(cells[toCheckRight], cells[toCheckLeft]);

                    if (match)
                    {
                        //found good hint piece
                        Debug.Log("Found good hint piece: " + match.name + " For cell index: " + cell.cellIndex);

                        //target is Cell and solution clip holder is match
                        return true;
                    }
                }
                else if (cells[toCheckLeft].pieceHeld)
                {
                    ClipHolder match = CheckAvailableTileInClip(null, cells[toCheckLeft]);

                    if (match)
                    {
                        //found good hint piece one side
                        Debug.Log("Found good hint piece one side: " + match.name + " For cell index: " + cell.cellIndex);

                        //target is Cell and solution clip holder is match
                        return true;
                    }
                }
                else if (cells[toCheckRight].pieceHeld)
                {
                    ClipHolder match = CheckAvailableTileInClip(cells[toCheckRight], null);

                    if (match)
                    {
                        //found good hint piece one side
                        Debug.Log("Found good hint piece one side: " + match.name + " For cell index: " + cell.cellIndex);

                        //target is Cell and solution clip holder is match
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private int CheckIntRangeCells(int num, List<Cell> cells)
    {
        if (num < 0)
        {
            return cells.Count - 1;
        }

        if (num >= cells.Count)
        {
            return 0;
        }


        return num;
    }

    private ClipHolder CheckAvailableTileInClip(Cell rightCell, Cell leftCell)
    {
        bool conditionMet = false;
        bool isGoodConnect = false;

        bool goodRight = false;
        bool goodLeft = false;

        foreach (ClipHolder holder in GameManager.Instance.clipManager.slots)
        {
            if(holder.heldPiece)
            {
                if(rightCell)
                {
                    goodRight = ConnectionManager.Instance.CheckSubPieceConnection(holder.heldPiece.rightChild, rightCell.pieceHeld.leftChild, out conditionMet, out isGoodConnect);
                }
                else
                {
                    goodRight = true;
                    //default good right to true since in the case this does not exsist we're checking to see only 1 side connections
                }

                if(leftCell)
                {
                    goodLeft = ConnectionManager.Instance.CheckSubPieceConnection(holder.heldPiece.leftChild, leftCell.pieceHeld.rightChild, out conditionMet, out isGoodConnect);
                }
                else
                {
                    goodLeft = true;
                    //default good left to true since in the case this does not exsist we're checking to see only 1 side connections

                }
            }

            if (goodLeft && goodRight && conditionMet)
            {
                return holder;
            }
        }

        return null;
    }

    public void PupulateLists()
    {
        SliceManager gameBoard = GameManager.Instance.sliceManager;

        cells = ConnectionManager.Instance.cells;
        stoneTilesInCellsInRing = gameBoard.stoneTilesInCellOnBoard;

        foreach (Slice slice in gameBoard.fullSlices)
        {
            switch (slice.sliceCatagory)
            {
                case SliceCatagory.Shape:
                    generalSymbolLimiters.Add(slice);
                    break;
                case SliceCatagory.Color:
                    generalColorLimiters.Add(slice);
                    break;
                case SliceCatagory.SpecificShape:
                    specificSymbolLimiters.Add(slice);
                    break;
                case SliceCatagory.SpecificColor:
                    specificColorLimiters.Add(slice);
                    break;
                case SliceCatagory.None:
                    break;
                default:
                    break;
            }
        }
    }

    public void ResetData()
    {
        stoneTilesInCellsInRing.Clear();
        specificSymbolLimiters.Clear();
        specificColorLimiters.Clear();
        generalSymbolLimiters.Clear();
        generalColorLimiters.Clear();
        cells.Clear();

        canShowHint = false;
        currentIdleTime = 0;
    }

}
