using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class Cell : MonoBehaviour
{
    public bool isFull;
    public bool isDuringConnectionAnim;

    public bool isOuter;
    public bool isLockedCell;
    public bool isStoneCell;

    public int cellIndex;
    public Piece pieceHeld;
    //public bool isLimited;
    //public GameObject lockSprite;
    //public GameObject lockSpriteCellRight, lockSpriteCellLeft; // previous lock system
    //public GameObject outlinedSpriteRight, outlinedSpriteLeft; // previous lock system
    public Transform rightParticleZone, leftParticleZone;
    public Transform interconnectedRightParticleZone, interconnectedLeftParticleZone;

    public ParticleSystem highlightParticle;

    public Slice leftSlice;
    public Slice rightSlice;
    public void AddPiece(Transform followerTarget, bool isNew)
    {
        isFull = true;
        followerTarget.SetParent(transform);
        followerTarget.position = new Vector3(followerTarget.parent.position.x, followerTarget.parent.position.y, followerTarget.parent.position.z);
        followerTarget.rotation = followerTarget.parent.rotation;
        pieceHeld = followerTarget.GetComponent<Piece>();
        pieceHeld.partOfBoard = true;
        pieceHeld.transform.localScale = GameManager.Instance.clipManager.pieceScaleOnBoard;

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


            if (pieceHeld.rightChild.subPieceIndex >= ConnectionManager.Instance.subPiecesOnBoard.Length - 1)
            {
                pieceHeld.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[0];
            }
            else
            {
                pieceHeld.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[Mathf.CeilToInt((float)pieceHeld.rightChild.subPieceIndex / 2)];
            }

        }

        if (isNew)
        {
            GameManager.Instance.currentFilledCellCount++;

            if (GameManager.Instance.currentLevel.ver1Boss)
            {
                BossBattleManager.instance.CheckEndLevelBossVersionOne();
            }
            else
            {
                if (GameManager.Instance.currentFilledCellCount == GameManager.Instance.currentLevel.cellsCountInLevel)
                {
                    ConnectionManager.Instance.CallConnection(cellIndex, isOuter, true);

                    return;
                }
            }
        }
        else
        {
            //Debug.LogError("Moved piece inside ring");
            GameAnalytics.NewDesignEvent("Pice moved in ring:" + GameManager.Instance.currentLevel.worldName + ":" + "Level " + GameManager.Instance.currentLevel.levelNum);
        }

        GameObject go = Instantiate(UIManager.Instance.placePieceVFX, followerTarget);

        //StartCoroutine(ConnectionManager.Instance.JumpPiecesEffect(pieceHeld));
        ConnectionManager.Instance.JumpPiecesEffect(pieceHeld);

        ConnectionManager.Instance.CallConnection(cellIndex, isOuter, false);
    }
    public void AddStonePieceToBoard(stonePieceDataStruct SPDS)
    {
        if (!isFull)
        {
            GameObject go = Instantiate(GameManager.Instance.clipManager.corruptedPiece, transform);

            ConnectionManager.Instance.amountStonePiecesInstantiated++;

            Piece p = null;
            if (go)
            {
                p = go.GetComponent<Piece>();
                p.SetStonePiece(SPDS);
                p.isStone = true;
                isStoneCell = true;
                pieceHeld = p;
            }
            else
            {
                Debug.Log("Faild to create piece");
            }


            isFull = true;
            p.transform.SetParent(transform);
            p.transform.localPosition = new Vector3(0, 0, -0.2f);
            p.transform.rotation = p.transform.parent.rotation;

            p.partOfBoard = true;
            p.transform.localScale = GameManager.Instance.clipManager.pieceScaleOnBoard;

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

                if (p.rightChild.subPieceIndex >= ConnectionManager.Instance.subPiecesOnBoard.Length - 1)
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

            //if (GameManager.Instance.currentLevel.isDoubleRing)
            //{
            //    int badInterConnections = 0;

            //    badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

            //    GameManager.Instance.unsuccessfullConnectionCount += badInterConnections;

            //}

            GameManager.Instance.currentFilledCellCount++;

            ConnectionManager.Instance.CallConnection(cellIndex, isOuter, false);
        }
        else
        {
            Debug.LogError("Trying to create piece on an already full cell");
        }
    }

    public void RemovePiece(/*bool destoryLock,*/ bool fromSlicePower)
    {
        isFull = false;

        if (rightParticleZone.childCount > 0)
        {
            //Destroy(rightParticleZone.GetChild(0).gameObject);
            rightParticleZone.GetChild(0).gameObject.SetActive(false);
        }

        if (leftParticleZone.childCount > 0)
        {
            //Destroy(leftParticleZone.GetChild(0).gameObject);
            leftParticleZone.GetChild(0).gameObject.SetActive(false);
        }

        //if (GameManager.Instance.currentLevel.isDoubleRing)
        //{
        //    if (interconnectedRightParticleZone.childCount > 0)
        //    {
        //        Destroy(interconnectedRightParticleZone.GetChild(0).gameObject);
        //    }

        //    if (interconnectedLeftParticleZone.childCount > 0)
        //    {
        //        Destroy(interconnectedLeftParticleZone.GetChild(0).gameObject);
        //    }


        //    int badInterConnections = 0;

        //    badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

        //    GameManager.Instance.unsuccessfullConnectionCount -= badInterConnections;
        //}

        //if (GameManager.Instance.currentLevel.isDoubleRing)
        //{
        //    int badInterConnections = 0;

        //    badInterConnections = InterconnectionManager.Instance.CheckInterConnection(cellIndex, isOuter);

        //    GameManager.Instance.unsuccessfullConnectionCount -= badInterConnections;

        //}

        Cell leftCell = ConnectionManager.Instance.cells[ConnectionManager.Instance.CheckIntRangeCells(cellIndex - 1)];
        Cell rightCell = ConnectionManager.Instance.cells[ConnectionManager.Instance.CheckIntRangeCells(cellIndex + 1)];

        if (pieceHeld.leftChild.relevantSlice)
        {
            if (pieceHeld.leftChild.relevantSlice.hasSlice)
            {
                GameManager.Instance.sliceManager.SetSliceNotSolvedSprite(pieceHeld.leftChild.relevantSlice);

                if (pieceHeld.leftChild.relevantSlice.isLimiter)
                {
                    if (pieceHeld.leftChild.relevantSlice.fulfilledCondition)
                    {
                        GameManager.Instance.unsuccessfullSlicesCount++;
                        SoundManager.Instance.PlaySound(Sounds.RuneLimiterUnMatch);
                    }
                    else
                    {
                        GameManager.Instance.unsuccessfullSlicesCount--;
                    }
                }
                //else
                //{
                //    if (!pieceHeld.leftChild.relevantSlice.fulfilledCondition)
                //    {
                //        GameManager.Instance.unsuccessfullSlicesCount--;
                //    }
                //}

                //if (!leftCell.isFull)
                //{

                //    GameManager.Instance.unsuccessfullSlicesCount--;
                //}
                //else
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount++;
                //}
            }
        }

        if (pieceHeld.rightChild.relevantSlice)
        {
            if (pieceHeld.rightChild.relevantSlice.hasSlice)
            {
                GameManager.Instance.sliceManager.SetSliceNotSolvedSprite(pieceHeld.rightChild.relevantSlice);

                if (pieceHeld.rightChild.relevantSlice.isLimiter)
                {
                    if (pieceHeld.rightChild.relevantSlice.fulfilledCondition)
                    {
                        GameManager.Instance.unsuccessfullSlicesCount++;
                        SoundManager.Instance.PlaySound(Sounds.RuneLimiterUnMatch);
                    }
                    else
                    {
                        GameManager.Instance.unsuccessfullSlicesCount--;
                    }
                }
                //else
                //{
                //    if (!pieceHeld.rightChild.relevantSlice.fulfilledCondition)
                //    {
                //        GameManager.Instance.unsuccessfullSlicesCount--;
                //    }
                //}

                //if (!rightCell.isFull)
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount--;
                //}
                //else
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount++;
                //}
            }
        }


        if (ConnectionManager.Instance.cells[cellIndex].pieceHeld.isLocked)
        {
            ConnectionManager.Instance.UnlockPieces(ConnectionManager.Instance.cells[cellIndex], leftCell, rightCell/*, destoryLock*/);
        }

        if (leftCell.isFull)
        {
            if (pieceHeld.leftChild.isBadConnection)
            {
                GameManager.Instance.unsuccessfullConnectionCount--;

                pieceHeld.leftChild.isBadConnection = false;
                int i = ConnectionManager.Instance.CheckIntRangeSubPieces(pieceHeld.leftChild.subPieceIndex - 1);

                //if (pieceHeld.leftChild.relevantSlice.hasSlice)
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount--;
                //}

            }
            else
            {
                //if (pieceHeld.leftChild.relevantSlice.hasSlice)
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount--;
                //}

                //pieceHeld.leftChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                SoundManager.Instance.PlaySoundChangeVolume(Sounds.TileUnmatch, 0.5f);

                if (!fromSlicePower)
                {
                    pieceHeld.leftChild.SetDisconnectedMaterial();
                }

                if (pieceHeld.leftChild.relevantSlice.isLimiter && pieceHeld.leftChild.relevantSlice.fulfilledCondition)
                {
                    //SoundManager.Instance.PlaySound(Sounds.RuneLimiterUnMatch);

                    if (pieceHeld.leftChild.relevantSlice.anim)
                    {
                        pieceHeld.leftChild.relevantSlice.anim.SetBool("Reverse", true);
                        pieceHeld.leftChild.relevantSlice.anim.SetBool("Activate", false);
                    }
                }

                if (!leftCell.pieceHeld.rightChild.isBadConnection)
                {
                    //leftCell.pieceHeld.rightChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    if (!fromSlicePower)
                    {
                        leftCell.pieceHeld.rightChild.SetDisconnectedMaterial();
                    }
                }
            }
        }

        if (rightCell.isFull)
        {
            if (pieceHeld.rightChild.isBadConnection)
            {
                GameManager.Instance.unsuccessfullConnectionCount--;

                //if (pieceHeld.rightChild.relevantSlice.hasSlice)
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount--;
                //}

                pieceHeld.rightChild.isBadConnection = false;
                int i = ConnectionManager.Instance.CheckIntRangeSubPieces(pieceHeld.rightChild.subPieceIndex + 1);

            }
            else
            {
                //if (pieceHeld.rightChild.relevantSlice.hasSlice)
                //{
                //    GameManager.Instance.unsuccessfullSlicesCount--;
                //}

                //pieceHeld.rightChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                SoundManager.Instance.PlaySoundChangeVolume(Sounds.TileUnmatch, 0.5f);

                if (!fromSlicePower)
                {
                    pieceHeld.rightChild.SetDisconnectedMaterial();
                }

                if (pieceHeld.rightChild.relevantSlice.isLimiter && pieceHeld.rightChild.relevantSlice.fulfilledCondition)
                {
                    //SoundManager.Instance.PlaySound(Sounds.RuneLimiterUnMatch);

                    if (pieceHeld.rightChild.relevantSlice.anim)
                    {
                        pieceHeld.rightChild.relevantSlice.anim.SetBool("Reverse", true);
                        pieceHeld.rightChild.relevantSlice.anim.SetBool("Activate", false);
                    }
                }

                if (rightCell.isFull)
                {
                    if (!rightCell.pieceHeld.leftChild.isBadConnection)
                    {
                        //rightCell.pieceHeld.leftChild.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                        if (!fromSlicePower)
                        {
                            rightCell.pieceHeld.leftChild.SetDisconnectedMaterial();
                        }
                    }
                }
            }
        }


        if (pieceHeld.rightChild.relevantSlice.hasSlice && pieceHeld.rightChild.relevantSlice.isLimiter)
        {
            pieceHeld.rightChild.relevantSlice.fulfilledCondition = false;
        }

        if (pieceHeld.leftChild.relevantSlice.hasSlice && pieceHeld.leftChild.relevantSlice.isLimiter)
        {
            pieceHeld.leftChild.relevantSlice.fulfilledCondition = false;
        }


        pieceHeld.rightChild.relevantSlice = null;
        pieceHeld.leftChild.relevantSlice = null;

        ConnectionManager.Instance.RemoveSubPieceIndex(pieceHeld.leftChild.subPieceIndex, isOuter);
        ConnectionManager.Instance.RemoveSubPieceIndex(pieceHeld.rightChild.subPieceIndex, isOuter);


        pieceHeld = null;
    }


    [ContextMenu("add piece random to cell")]
    public void AddPieceRandom()
    {
        GameObject go = Instantiate(GameManager.Instance.clipManager.piece, transform);
        Piece p = go.GetComponent<Piece>();
        p.SetPieces();

        isFull = true;


        //followerTarget.SetParent(transform);
        p.transform.localPosition = Vector3.zero;
        p.transform.rotation = transform.rotation;

        pieceHeld = p;
        pieceHeld.partOfBoard = true;
        pieceHeld.transform.localScale = GameManager.Instance.clipManager.pieceScaleOnBoard;


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


            if (pieceHeld.rightChild.subPieceIndex >= ConnectionManager.Instance.subPiecesOnBoard.Length - 1)
            {
                pieceHeld.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[0];
            }
            else
            {
                pieceHeld.rightChild.relevantSlice = ConnectionManager.Instance.slicesOnBoard[Mathf.CeilToInt((float)pieceHeld.rightChild.subPieceIndex / 2)];
            }

        }


        GameManager.Instance.currentFilledCellCount++;

        if (GameManager.Instance.currentFilledCellCount == GameManager.Instance.currentLevel.cellsCountInLevel)
        {

            Debug.LogError("LOSE BOSS BATTLE");
            return;
        }
    }
 

    public void TurnOnHighlightParticle()
    {
        highlightParticle.gameObject.SetActive(true);
    }

    public void TurnOffHighlighParticle()
    {
        highlightParticle.gameObject.SetActive(false);
    }


    public void AddToSubPiecesOnBoardTemp()
    {
        ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2] = CursorController.Instance.followerTarget.GetComponent<Piece>().leftChild;
        ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2 + 1] = CursorController.Instance.followerTarget.GetComponent<Piece>().rightChild;


        CursorController.Instance.followerTarget.GetComponent<Piece>().leftChild.subPieceIndex = cellIndex * 2;
        CursorController.Instance.followerTarget.GetComponent<Piece>().rightChild.subPieceIndex = cellIndex * 2 + 1;

    }
    public void RemoveToSubPiecesOnBoardTemp()
    {
        ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2] = null;
        ConnectionManager.Instance.subPiecesOnBoard[cellIndex * 2 + 1] = null;

        CursorController.Instance.followerTarget.GetComponent<Piece>().leftChild.subPieceIndex = -1;
        CursorController.Instance.followerTarget.GetComponent<Piece>().rightChild.subPieceIndex = -1;
    }
}
