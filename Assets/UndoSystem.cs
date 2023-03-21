using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class OriginalCellData
{
    public Cell originalCellParent;
    public bool isLocked;
    public bool isStone;
}
[Serializable]
public class UndoEntry
{
    [Header("Original Clip Parent Data")]
    public ClipHolder originalClipParent;

    [Header("Original Cell Parent Data")]
    public OriginalCellData originalCell;

    [Header("Current Cell Parent Data")]
    public Cell currntCellParent;

    public Piece movedPiece;
}
public class UndoSystem : MonoBehaviour
{
    public static UndoSystem instance;

    public List<UndoEntry> undoEntries;


    private void Start()
    {
        instance = this;
    }

    public void CallUndoAction()
    {
        if (undoEntries.Count <= 0) return;

        StartCoroutine(OneStepBack());
    }

    private IEnumerator OneStepBack()
    {
        int lastIndex = undoEntries.Count - 1;

        if(undoEntries[lastIndex].currntCellParent)
        {
            undoEntries[lastIndex].currntCellParent.RemovePiece(false);
            yield return new WaitForEndOfFrame();

            undoEntries[lastIndex].currntCellParent.isFull = false;
            undoEntries[lastIndex].currntCellParent.isStoneCell = false;
            undoEntries[lastIndex].currntCellParent.pieceHeld = null;

            if (undoEntries[lastIndex].originalCell.originalCellParent)
            {
                undoEntries[lastIndex].originalCell.originalCellParent.isLockedCell = undoEntries[lastIndex].originalCell.isLocked;
                undoEntries[lastIndex].originalCell.originalCellParent.isStoneCell = undoEntries[lastIndex].originalCell.isStone;
                undoEntries[lastIndex].originalCell.originalCellParent.pieceHeld = undoEntries[lastIndex].movedPiece;

                undoEntries[lastIndex].movedPiece.transform.SetParent(undoEntries[lastIndex].originalCell.originalCellParent.transform);

                undoEntries[lastIndex].movedPiece.transform.localPosition = Vector3.zero;
                undoEntries[lastIndex].movedPiece.transform.localRotation = Quaternion.identity;

                undoEntries[lastIndex].originalCell.originalCellParent.AddPiece(undoEntries[lastIndex].movedPiece.transform, false);

            }
            else
            {
                if (undoEntries[lastIndex].originalClipParent)
                {
                    GameManager.Instance.currentFilledCellCount--;
                    Destroy(undoEntries[lastIndex].originalClipParent.heldPiece.gameObject);
                    yield return new WaitForEndOfFrame();

                    undoEntries[lastIndex].originalClipParent.heldPiece = undoEntries[lastIndex].movedPiece;
                    undoEntries[lastIndex].movedPiece.transform.SetParent(undoEntries[lastIndex].originalClipParent.transform);
                    undoEntries[lastIndex].movedPiece.transform.localPosition = new Vector3(-0.13f, -0.27f, 0.1f);
                    undoEntries[lastIndex].movedPiece.transform.localRotation = Quaternion.Euler(0, 180, 67.5f);

                    undoEntries[lastIndex].movedPiece.partOfBoard = false;

                }
            }
        }

        if (undoEntries[lastIndex].movedPiece.isLocked)
        {
            undoEntries[lastIndex].movedPiece.isLocked = false;

            foreach (Cell cell in undoEntries[lastIndex].currntCellParent.leftSlice.connectedCells)
            {
                cell.isLockedCell = false;
                if (cell.pieceHeld)
                {
                    cell.pieceHeld.isLocked = false;
                }
            }

            foreach (Cell cell in undoEntries[lastIndex].currntCellParent.rightSlice.connectedCells)
            {
                cell.isLockedCell = false;

                if(cell.pieceHeld)
                {
                    cell.pieceHeld.isLocked = false;
                }
            }

            ConnectionManager.Instance.UnlockCell(undoEntries[lastIndex].currntCellParent.leftSlice);
            ConnectionManager.Instance.UnlockCell(undoEntries[lastIndex].currntCellParent.rightSlice);
        }

        undoEntries.RemoveAt(lastIndex);
    }

    public void RemoveEntriesOnDeal(ClipHolder holder)
    {
        for (int i = undoEntries.Count - 1; i >= 0; i--)
        {
            if(undoEntries[i].originalClipParent && undoEntries[i].originalClipParent == holder)
            {
                undoEntries.RemoveAt(i);
            }
        }
    }

    public void AddNewUndoEntry(Transform originalParent, Transform currentParent, Piece piece)
    {
        if (originalParent == null || currentParent == null || piece == null) return;

        UndoEntry newEntry = new UndoEntry();
        newEntry.originalCell = new OriginalCellData();
        newEntry.movedPiece = piece;

        originalParent.TryGetComponent<ClipHolder>(out newEntry.originalClipParent);

        originalParent.TryGetComponent<Cell>(out newEntry.originalCell.originalCellParent);

        if(newEntry.originalCell.originalCellParent)
        {
            Cell cell = originalParent.GetComponent<Cell>();
            if (cell == null) return;

            newEntry.originalCell.isLocked = cell.isLockedCell;
            newEntry.originalCell.isStone = cell.isStoneCell;
        }
        currentParent.TryGetComponent<Cell>(out newEntry.currntCellParent);

        if(originalParent != currentParent)
        {
            undoEntries.Add(newEntry);
        }
    }
}
