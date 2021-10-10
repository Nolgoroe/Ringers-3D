using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isFull;
    public bool isOuter;
    public int cellIndex;
    public Piece pieceHeld;
    //public bool isLimited;
    //public GameObject lockSprite;
    public GameObject lockSpriteCellRight, lockSpriteCellLeft;
    public GameObject outlinedSpriteRight, outlinedSpriteLeft;
    public Transform rightParticleZone, leftParticleZone;
    public Transform interconnectedRightParticleZone, interconnectedLeftParticleZone;

    public ParticleSystem highlightParticle;

    public void AddPiece(Transform followerTarget, bool isNew)
    {
        isFull = true;
        followerTarget.SetParent(transform);
        followerTarget.position = new Vector3(followerTarget.parent.position.x, followerTarget.parent.position.y, followerTarget.parent.position.z);
        followerTarget.rotation = followerTarget.parent.rotation;
        pieceHeld = followerTarget.GetComponent<Piece>();
        pieceHeld.partOfBoard = true;
        pieceHeld.transform.localScale = new Vector3(1.15f,1.15f,1f);

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
                ConnectionManager.Instance.CallConnection(cellIndex, isOuter, true);

                bool gameWon = GameManager.Instance.CheckEndLevel();

                if (gameWon)
                {
                    SoundManager.Instance.PlaySound(Sounds.SolvedRing);
                    AnimationManager.instance.StartEndLevelAnimSequence();
                }

                return;
                //UIManager.Instance.ActivateCommitButton();
            }
        }

        ConnectionManager.Instance.CallConnection(cellIndex, isOuter, false);
    }
    public void AddStonePieceToBoard(stonePieceDataStruct SPDS)
    {
        if (!isFull)
        {
            GameObject go = Instantiate(GameManager.Instance.clipManager.piece, transform);

            Piece p = null;
            if (go)
            {
                p = go.GetComponent<Piece>();
                p.SetStonePiece(SPDS);
                p.isStone = true;
                pieceHeld = p;
            }
            else
            {
                Debug.Log("Faild to create piece");
            }


            isFull = true;
            p.transform.SetParent(transform);
            p.transform.position = new Vector3(p.transform.parent.position.x, p.transform.parent.position.y, p.transform.parent.position.z);
            p.transform.rotation = p.transform.parent.rotation;

            p.partOfBoard = true;
            p.transform.localScale = new Vector3(1.15f, 1.15f, 1f);

            if (!isOuter)
            {
                ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2] = p.leftChild;
                ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2 + 1] = p.rightChild;
                ConnectionManager.Instance.FillSubPieceIndex();
            }
            else
            {
                ConnectionManager.Instance.subPiecesDoubleRing[cellIndex * 2] = p.leftChild;
                ConnectionManager.Instance.subPiecesDoubleRing[cellIndex * 2 + 1] = p.rightChild;
                ConnectionManager.Instance.FillSubPieceIndex();
            }

            if (!isOuter)
            {
                p.leftChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[Mathf.CeilToInt((float)p.leftChild.subPieceIndex / 2)];

                if (p.leftChild.relevantSlice.sliceSymbol != PieceSymbol.None)
                {
                    p.leftChild.SetSpecificSymbol(p.leftChild.relevantSlice.sliceSymbol);
                }

                if (p.rightChild.subPieceIndex >= ConnectionManager.Instance.lengthOfSubPiecesOuter - 1)
                {
                    p.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[0];
                }
                else
                {
                    p.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[Mathf.CeilToInt((float)p.rightChild.subPieceIndex / 2)];

                    if (p.rightChild.relevantSlice.sliceSymbol != PieceSymbol.None)
                    {
                        p.rightChild.SetSpecificSymbol(p.rightChild.relevantSlice.sliceSymbol);
                    }
                }

            }

            if (GameManager.Instance.currentLevel.isDoubleRing)
            {
                int badInterConnections = 0;

                badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

                GameManager.Instance.unsuccessfullConnectionCount += badInterConnections;

            }

            GameManager.Instance.currentFilledCellCount++;

            ConnectionManager.Instance.CallConnection(cellIndex, isOuter, false);
        }
        else
        {
            Debug.LogError("Trying to create piece on an already full cell");
        }
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

        if (ConnectionManager.Instance.cells[cellIndex].pieceHeld.isLocked)
        {
            ConnectionManager.Instance.UnlockPieces(ConnectionManager.Instance.cells[cellIndex], leftCell, rightCell);
        }

        if (leftCell.isFull)
        {
            if (pieceHeld.leftChild.isBadConnection)
            {
                GameManager.Instance.unsuccessfullConnectionCount--;
                pieceHeld.leftChild.isBadConnection = false;
                int i = ConnectionManager.Instance.CheckIntRange(pieceHeld.leftChild.subPieceIndex - 1);

            }
            else
            {
                //pieceHeld.leftChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                SoundManager.Instance.PlaySound(Sounds.TileUnmatch);
                pieceHeld.leftChild.SetDisconnectedMaterial();

                if (pieceHeld.leftChild.relevantSlice.isLimiter && pieceHeld.leftChild.relevantSlice.fulfilledCondition)
                {
                    SoundManager.Instance.PlaySound(Sounds.RuneLimiterUnMatch);

                    pieceHeld.leftChild.relevantSlice.anim.SetBool("Reverse", true);
                    pieceHeld.leftChild.relevantSlice.anim.SetBool("Activate", false);
                }

                if (!leftCell.pieceHeld.rightChild.isBadConnection)
                {
                    //leftCell.pieceHeld.rightChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    leftCell.pieceHeld.rightChild.SetDisconnectedMaterial();
                }
            }
        }

        if (rightCell.isFull)
        {
            if (pieceHeld.rightChild.isBadConnection)
            {
                GameManager.Instance.unsuccessfullConnectionCount--;
                pieceHeld.rightChild.isBadConnection = false;
                int i = ConnectionManager.Instance.CheckIntRange(pieceHeld.rightChild.subPieceIndex + 1);

            }
            else
            {
                //pieceHeld.rightChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                SoundManager.Instance.PlaySound(Sounds.TileUnmatch);
                pieceHeld.rightChild.SetDisconnectedMaterial();

                if (pieceHeld.rightChild.relevantSlice.isLimiter && pieceHeld.rightChild.relevantSlice.fulfilledCondition)
                {
                    SoundManager.Instance.PlaySound(Sounds.RuneLimiterUnMatch);

                    pieceHeld.rightChild.relevantSlice.anim.SetBool("Reverse", true);
                    pieceHeld.rightChild.relevantSlice.anim.SetBool("Activate", false);
                }

                if (rightCell.isFull)
                {
                    if (!rightCell.pieceHeld.leftChild.isBadConnection)
                    {
                        //rightCell.pieceHeld.leftChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                        rightCell.pieceHeld.leftChild.SetDisconnectedMaterial();
                    }
                }
            }
        }


        if (pieceHeld.rightChild.relevantSlice || pieceHeld.leftChild.relevantSlice)
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
