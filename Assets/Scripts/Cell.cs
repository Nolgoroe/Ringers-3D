using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isFull;
    public int cellIndex;
    public Piece pieceHeld;
    public bool isLimited;

    public void AddPiece(Transform followerTarget, bool isNew)
    {
        isFull = true;
        followerTarget.SetParent(transform);
        followerTarget.position = new Vector3(followerTarget.parent.position.x, followerTarget.parent.position.y, followerTarget.parent.position.z + 0.1f);
        followerTarget.rotation = followerTarget.parent.rotation;
        pieceHeld = followerTarget.GetComponent<Piece>();

        GameManager.Instance.connectionManager.subPiecesOnBoard[cellIndex * 2] = pieceHeld.rightChild;
        GameManager.Instance.connectionManager.subPiecesOnBoard[cellIndex * 2 + 1] = pieceHeld.leftChild;
        GameManager.Instance.connectionManager.FillSubPieceIndex();

        GameManager.Instance.connectionManager.CheckConnections(cellIndex);

        if (isNew)
        {
            GameManager.Instance.currentFilledCellCount++;
        }

        //GameManager.Instance.cellManager.NeighborTest(cellIndex);
    }

    public void RemovePiece()
    {
        if (pieceHeld.rightChild.isBadConnection)
        {
            GameManager.Instance.unsuccessfullConnectionCount--;
            pieceHeld.rightChild.isBadConnection = false;
            int i = GameManager.Instance.connectionManager.CheckIntRange(pieceHeld.rightChild.subPieceIndex - 1);
            GameManager.Instance.connectionManager.subPiecesOnBoard[i].isBadConnection = false;
        }

        if (pieceHeld.leftChild.isBadConnection)
        {
            GameManager.Instance.unsuccessfullConnectionCount--;
            pieceHeld.leftChild.isBadConnection = false;
            int i = GameManager.Instance.connectionManager.CheckIntRange(pieceHeld.leftChild.subPieceIndex + 1);
            GameManager.Instance.connectionManager.subPiecesOnBoard[i].isBadConnection = false;
        }

        GameManager.Instance.connectionManager.RemoveSubPieceIndex(pieceHeld.rightChild.subPieceIndex);
        GameManager.Instance.connectionManager.RemoveSubPieceIndex(pieceHeld.leftChild.subPieceIndex);
        GameManager.Instance.connectionManager.CheckConnections(cellIndex + 1);
        GameManager.Instance.connectionManager.CheckConnections(cellIndex - 1);
    }
}
