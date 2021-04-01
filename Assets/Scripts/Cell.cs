﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isFull;
    public bool isOuter;
    public int cellIndex;
    public Piece pieceHeld;
    public bool isLimited;
    public GameObject lockSprite;
    public Transform rightParticleZone, leftParticleZone;
    public Transform interconnectedRightParticleZone, interconnectedLeftParticleZone;

    public void AddPiece(Transform followerTarget, bool isNew)
    {
        isFull = true;
        followerTarget.SetParent(transform);
        followerTarget.position = new Vector3(followerTarget.parent.position.x, followerTarget.parent.position.y, followerTarget.parent.position.z + 0.1f);
        followerTarget.rotation = followerTarget.parent.rotation;
        pieceHeld = followerTarget.GetComponent<Piece>();
        pieceHeld.partOfBoard = true;
        pieceHeld.transform.localScale = Vector3.one;

        if (!isOuter)
        {
            ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2] = pieceHeld.leftChild;
            ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2 + 1] = pieceHeld.rightChild;
            ConnectionManager.Instance.FillSubPieceIndex();
        }
        else
        {
            ConnectionManager.Instance.subPiecesDoubleRing[cellIndex * 2] = pieceHeld.leftChild;
            ConnectionManager.Instance.subPiecesDoubleRing[cellIndex * 2 + 1] = pieceHeld.rightChild;
            ConnectionManager.Instance.FillSubPieceIndex();
        }

        if (!isOuter)
        {
            pieceHeld.leftChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[Mathf.CeilToInt((float)pieceHeld.leftChild.subPieceIndex / 2)];


            if (pieceHeld.rightChild.subPieceIndex >= ConnectionManager.Instance.lengthOfSubPiecesOuter - 1)
            {
                pieceHeld.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[0];
            }
            else
            {
                pieceHeld.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[Mathf.CeilToInt((float)pieceHeld.rightChild.subPieceIndex / 2)];
            }

        }

        if (GameManager.Instance.currentLevel.isDoubleRing)
        {
            int badInterConnections = 0;

            badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

            GameManager.Instance.unsuccessfullConnectionCount += badInterConnections;

        }

        if (isNew)
        {
            GameManager.Instance.currentFilledCellCount++;

            if(GameManager.Instance.currentFilledCellCount == GameManager.Instance.currentLevel.cellsCountInLevel)
            {
                UIManager.Instance.ActivateCommitButton();
            }
        }

        ConnectionManager.Instance.CallConnection(cellIndex, isOuter);
    }

    public void RemovePiece()
    {
        isFull = false;

        if (rightParticleZone.childCount > 0)
        {
            Destroy(rightParticleZone.GetChild(0).gameObject);
        }

        if (leftParticleZone.childCount > 0)
        {
            Destroy(leftParticleZone.GetChild(0).gameObject);
        }

        if (GameManager.Instance.currentLevel.isDoubleRing)
        {
            if (interconnectedRightParticleZone.childCount > 0)
            {
                Destroy(interconnectedRightParticleZone.GetChild(0).gameObject);
            }

            if (interconnectedLeftParticleZone.childCount > 0)
            {
                Destroy(interconnectedLeftParticleZone.GetChild(0).gameObject);
            }


            int badInterConnections = 0;

            badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

            GameManager.Instance.unsuccessfullConnectionCount -= badInterConnections;
        }

        //if (GameManager.Instance.currentLevel.isDoubleRing)
        //{
        //    int badInterConnections = 0;

        //    badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

        //    GameManager.Instance.unsuccessfullConnectionCount -= badInterConnections;

        //}
        Cell leftCell = ConnectionManager.Instance.cells[ConnectionManager.Instance.CheckIntRangeCells(cellIndex - 1)];
        Cell rightCell = ConnectionManager.Instance.cells[ConnectionManager.Instance.CheckIntRangeCells(cellIndex + 1)];

        if (pieceHeld.leftChild.isBadConnection)
        {
            GameManager.Instance.unsuccessfullConnectionCount--;
            pieceHeld.leftChild.isBadConnection = false;
            int i = ConnectionManager.Instance.CheckIntRange(pieceHeld.leftChild.subPieceIndex - 1);

        }
        else
        {
            pieceHeld.leftChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

            if (leftCell.isFull)
            {
                if (!leftCell.pieceHeld.rightChild.isBadConnection)
                {
                    leftCell.pieceHeld.rightChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                }
            }
        }

        if (pieceHeld.rightChild.isBadConnection)
        {
            GameManager.Instance.unsuccessfullConnectionCount--;
            pieceHeld.rightChild.isBadConnection = false;
            int i = ConnectionManager.Instance.CheckIntRange(pieceHeld.rightChild.subPieceIndex + 1);

        }
        else
        {
            pieceHeld.rightChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

            if (rightCell.isFull)
            {
                if (!rightCell.pieceHeld.leftChild.isBadConnection)
                {
                    rightCell.pieceHeld.leftChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                }
            }
        }

        if (pieceHeld.rightChild.relevantSlice)
        {
            pieceHeld.rightChild.relevantSlice.fulfilledCondition = false;
            pieceHeld.leftChild.relevantSlice.fulfilledCondition = false;
            pieceHeld.rightChild.relevantSlice = null;
            pieceHeld.leftChild.relevantSlice = null;
        }
        ConnectionManager.Instance.RemoveSubPieceIndex(pieceHeld.leftChild.subPieceIndex, isOuter);
        ConnectionManager.Instance.RemoveSubPieceIndex(pieceHeld.rightChild.subPieceIndex, isOuter);
    }
}
