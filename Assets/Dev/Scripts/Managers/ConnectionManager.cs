using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CellsObjetData
{
    public List<Cell> possibleCells = new List<Cell>();
    public CellsObjetData()
    {
        possibleCells = new List<Cell>();
    }
}

[System.Serializable]
public class PossibleCellPathList
{
    public List<CellsObjetData> possibleCellsPath = new List<CellsObjetData>();
}

[System.Serializable]
public class EdgePathFoundData
{
    public List<Cell> foundCells = new List<Cell>();
    public Cell lastCellToMove;

    public PieceSymbol leftAnimalSymbolNeeded;
    public PieceColor leftColorNeeded;

    public PieceSymbol rightAnimalSymbolNeeded;
    public PieceColor rightColorNeeded;

    public EdgePathFoundData()
    {
        foundCells = new List<Cell>();
    }
}

[System.Serializable]
public class FoundCellPath
{
    public List<EdgePathFoundData> foundCellPath = new List<EdgePathFoundData>();
}

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public List<Cell> cells;
    public List<Cell> outerCells;

    public SubPiece[] subPiecesOnBoard;
    public SubPiece[] subPiecesDoubleRing;

    public Slice[] slicesOnBoard;

    public int lengthOfSubPiecesRegular;
    public int lengthOfSubPieces12Pieces;
    public int lengthOfSubPiecesOuter;
    public float timeToLerpConnectionEmission;

    //public ParticleSystem connectedTilesVFX /*, badConnectionParticle*/;
    public GameObject connectedTilesVFX;

    public GameObject lootEffectPrefab;
    //public GameObject rightPieceLockObject, leftPieceLockObject; // old lock system
    //public GameObject rightPieceLockLimiterObject, leftPieceLockLimiterObject; // old lock system

    //public Material rockLIT,rockUnLIT;

    bool playedConnectedSound = false;
    bool nextToOtherpiece = false;

    public float upAmountPieceEffectMaxX, upAmountPieceEffectMinX;
    public float upAmountPieceEffectMaxY, upAmountPieceEffectMinY;
    public float upAmountPieceEffectMaxZ, upAmountPieceEffectMinZ;
    public float speedUpPieceEffect;


    public float speedPieceConnectAnim;

    public List<PieceSymbol> tempSymbolPiecesStoneFound;
    public int amountStonePiecesInstantiated;

    [Header("Last clip algoritm zone")]

    public int movesMade;
    public bool hasFinishedAlgorithm;
    public float TEMPDelayTime;

    public EdgePathFoundData decidedAlgoritmPath = null;

    public PossibleCellPathList cellsThatCanConnect;
    public FoundCellPath pathsFound;

    private List<Cell> bufferList;
    private List<SubPiece> subPiecesOnBoardTempAlgoritm;
    private List<Cell> previousEmptyCells;
    private Cell originalMissingCell;
    private int mostCurrentListIndex;

    private void Start()
    {
        Instance = this;


        subPiecesOnBoardTempAlgoritm = new List<SubPiece>();
        previousEmptyCells = new List<Cell>();
        bufferList = new List<Cell>();
    }

    public void SetLevelConnectionData(bool is12Pieces)
    {
        if (is12Pieces)
        {
            subPiecesOnBoard = new SubPiece[lengthOfSubPieces12Pieces];
        }
        else
        {
            subPiecesOnBoard = new SubPiece[lengthOfSubPiecesRegular];
        }

        subPiecesDoubleRing = new SubPiece[lengthOfSubPiecesOuter];
    }

    public void GrabCellList(Transform gb)
    {
        foreach (Cell c in gb.GetComponentsInChildren<Cell>())
        {
            if (!c.isOuter)
            {
                cells.Add(c);
            }
            else
            {
                outerCells.Add(c);
            }
        }
        slicesOnBoard = gb.GetComponentsInChildren<Slice>();
    }

    public void CallConnection(int cellIndex, bool isOuterCell, bool lastPiece)
    {
        playedConnectedSound = false;
        nextToOtherpiece = false;

        if (!isOuterCell)
        {
            CheckConnections(subPiecesOnBoard, cells, cellIndex, isOuterCell, lastPiece);
        }
        else
        {
            CheckConnections(subPiecesDoubleRing, outerCells, cellIndex, isOuterCell, lastPiece);
        }

    }

    public void CheckConnections(SubPiece[] supPieceArray, List<Cell> cellList, int cellIndex, bool isOuterCell, bool lastPiece)
    {

        StartCheckLeft(supPieceArray, cellList, cellIndex, isOuterCell, lastPiece); //// check start from left side which then checks the right side aswell
                                                                                    ///// This function works like this to accomodate the last piece logic. 
                                                                                    //When the last piece is placed on the board we HAVE TO check connections before activating slice animations and logic.
                                                                                    //If one of the sides of the LAST PIECE are wrong then we don't activate any slices even if condition is met

        if (GameManager.Instance.currentLevel.ver1Boss)
        {
            StartCoroutine(BossBattleManager.instance.DamageBoss());
        }
    }


    public void StartCheckLeft(SubPiece[] supPieceArray, List<Cell> cellList, int cellIndex, bool isOuterCell, bool lastPiece)
    {
        int leftContested = CheckIntRangeSubPieces((cellIndex * 2) - 1);
        int currentLeft = cellIndex * 2;

        if (supPieceArray[leftContested] && subPiecesOnBoard[leftContested].parentPiece.partOfBoard)
        {
            nextToOtherpiece = true;

            if (supPieceArray[currentLeft])
            {
                if (!CheckSubPieceConnection(supPieceArray[currentLeft], supPieceArray[leftContested], out bool conditionmet, out bool isGoodConnect))
                {
                    SoundManager.Instance.PlaySoundChangeVolume(Sounds.AddTileBoard, 0.5f);


                    if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
                    {
                        CursorController.Instance.tutorialBadConnection = true;
                    }

                    //Debug.Log("Bad Connection Right Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;

                    if (supPieceArray[currentLeft].relevantSlice.hasSlice)
                    {
                        if (supPieceArray[currentLeft].relevantSlice.isLimiter)
                        {
                            GameManager.Instance.unsuccessfullSlicesCount++;
                        }
                    }


                    supPieceArray[currentLeft].isBadConnection = true;
                    supPieceArray[leftContested].isBadConnection = true;
                    //supPieceArray[currentLeft].gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    //supPieceArray[leftContested].gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                    if (supPieceArray[currentLeft].relevantSlice)
                    {
                        supPieceArray[currentLeft].relevantSlice.fulfilledCondition = false;
                    }

                    CheckRight(supPieceArray, cellList, cellIndex, isOuterCell, lastPiece);

                    //Instantiate(badConnectionParticle, cellList[cellIndex].rightParticleZone);
                }
                else
                {
                    if (supPieceArray[currentLeft].relevantSlice.hasSlice)
                    {
                        if (supPieceArray[currentLeft].relevantSlice.isLimiter)
                        {
                            if (!conditionmet)
                            {
                                GameManager.Instance.unsuccessfullSlicesCount++;
                            }
                            else
                            {
                                GameManager.Instance.unsuccessfullSlicesCount--;
                            }
                        }
                    }


                    supPieceArray[currentLeft].isBadConnection = false;
                    supPieceArray[leftContested].isBadConnection = false;

                    //supPieceArray[currentLeft].transform.parent.GetComponent<CameraShake>().ShakeOnce();
                    //supPieceArray[leftContested].transform.parent.GetComponent<CameraShake>().ShakeOnce();

                    CursorController.Instance.tutorialBadConnection = false;

                    //Instantiate(goodConnectionParticle, cellList[cellIndex].rightParticleZone);
                    supPieceArray[currentLeft].SetConnectedMaterial();
                    supPieceArray[leftContested].SetConnectedMaterial();
                    //Instantiate(connectedTilesVFX, cellList[cellIndex].rightParticleZone);
                    cellList[cellIndex].leftParticleZone.GetChild(0).gameObject.SetActive(true);


                    playedConnectedSound = true;

                    //Debug.Log("Emission is happening");
                    //supPieceArray[currentLeft].gameObject.GetComponent<Renderer>().material.EnableKeyword ("_EMISSION");
                    //supPieceArray[leftContested].gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

                    if (lastPiece && !GameManager.Instance.currentLevel.isBoss)
                    {
                        CheckRight(supPieceArray, cellList, cellIndex, isOuterCell, lastPiece);

                        //bool gameWon = GameManager.Instance.CheckEndLevel();

                        //if (GameManager.gameWon)
                        //{
                        if (conditionmet)
                        {
                            GameManager.Instance.sliceManager.SetSliceSolvedSprite(supPieceArray[currentLeft].relevantSlice);

                            //if (supPieceArray[currentLeft].relevantSlice.anim)
                            //{
                            //supPieceArray[currentLeft].relevantSlice.anim.SetBool("Activate", true);

                            if (supPieceArray[currentLeft].relevantSlice.hasSlice)
                            {
                                if (supPieceArray[currentLeft].relevantSlice.isLimiter)
                                {
                                    SoundManager.Instance.PlaySound(Sounds.RuneLimiterMatch);

                                    //supPieceArray[currentLeft].relevantSlice.anim.SetBool("Reverse", false);
                                }
                                else
                                {
                                    SoundManager.Instance.PlaySound(Sounds.SliceLimiterMatch);
                                }
                            }
                            //}

                            supPieceArray[currentLeft].relevantSlice.fulfilledCondition = true;

                            if (supPieceArray[currentLeft].relevantSlice.isLoot)
                            {
                                GiveLootFromConnections(supPieceArray[currentLeft].relevantSlice, supPieceArray[currentLeft].relevantSlice.isLimiter);
                            }

                            if (supPieceArray[currentLeft].relevantSlice.isLock)
                            {
                                StartCoroutine(LockCell(supPieceArray[currentLeft].relevantSlice, supPieceArray[currentLeft].relevantSlice.isLimiter));
                            }
                        }
                        //}
                    }
                    else
                    {
                        SoundManager.Instance.PlaySound(Sounds.TileMatch);

                        if (conditionmet)
                        {
                            GameManager.Instance.sliceManager.SetSliceSolvedSprite(supPieceArray[currentLeft].relevantSlice);

                            //if (supPieceArray[currentLeft].relevantSlice.anim)
                            //{

                            //supPieceArray[currentLeft].relevantSlice.anim.SetBool("Activate", true);

                            if (supPieceArray[currentLeft].relevantSlice.hasSlice)
                            {
                                if (supPieceArray[currentLeft].relevantSlice.isLimiter)
                                {
                                    SoundManager.Instance.PlaySound(Sounds.RuneLimiterMatch);

                                    //supPieceArray[currentLeft].relevantSlice.anim.SetBool("Reverse", false);
                                }
                                else
                                {
                                    if (!supPieceArray[currentLeft].relevantSlice.fulfilledCondition)
                                    {
                                        SoundManager.Instance.PlaySound(Sounds.SliceLimiterMatch);
                                    }
                                }
                            }
                            //}

                            supPieceArray[currentLeft].relevantSlice.fulfilledCondition = true;

                            if (supPieceArray[currentLeft].relevantSlice.isLoot)
                            {
                                GiveLootFromConnections(supPieceArray[currentLeft].relevantSlice, supPieceArray[currentLeft].relevantSlice.isLimiter);
                            }

                            if (supPieceArray[currentLeft].relevantSlice.isLock)
                            {
                                StartCoroutine(LockCell(supPieceArray[currentLeft].relevantSlice, supPieceArray[currentLeft].relevantSlice.isLimiter));
                            }
                        }

                        if (GameManager.Instance.currentLevel.ver1Boss)
                        {
                            BossBattleManager.instance.piecesToRemove.Add(supPieceArray[currentLeft].parentPiece); /// middle piece
                            BossBattleManager.instance.piecesToRemove.Add(supPieceArray[leftContested].parentPiece); /// left piece
                        }

                        CheckRight(supPieceArray, cellList, cellIndex, isOuterCell, lastPiece);
                    }


                    //supPieceArray[currentLeft].parentPiece.GetComponentInParent<Cell>().RemovePiece(true, false);
                    //supPieceArray[leftContested].parentPiece.GetComponentInParent<Cell>().RemovePiece(true, false);

                    //Destroy(supPieceArray[currentLeft].parentPiece.gameObject);
                    //Destroy(supPieceArray[leftContested].parentPiece.gameObject);
                }
            }
            else
            {
                Debug.Log("Bad Connection Right Conetsted FUCKKKKAKAAAAAA");
                GameManager.Instance.unsuccessfullConnectionCount++;
                supPieceArray[leftContested].isBadConnection = true;
            }
        }
        else
        {
            CheckRight(supPieceArray, cellList, cellIndex, isOuterCell, lastPiece); //// if there is no piece connected to left - check right side

            if (!isOuterCell)
            {
                if (supPieceArray[currentLeft].relevantSlice.hasSlice)
                {
                    if (supPieceArray[currentLeft].relevantSlice.isLimiter)
                    {
                        GameManager.Instance.unsuccessfullSlicesCount++;
                    }
                }
            }
        }

    }

    public void CheckRight(SubPiece[] supPieceArray, List<Cell> cellList, int cellIndex, bool isOuterCell, bool lastPiece)
    {
        int rightContested = CheckIntRangeSubPieces((cellIndex * 2) + 2);

        int currentRight = cellIndex * 2 + 1;

        if (supPieceArray[rightContested] && subPiecesOnBoard[rightContested].parentPiece.partOfBoard)
        {
            nextToOtherpiece = true;

            if (supPieceArray[currentRight])
            {
                if (!CheckSubPieceConnection(supPieceArray[currentRight], supPieceArray[rightContested], out bool conditionmet, out bool isGoodConnect))
                {
                    SoundManager.Instance.PlaySoundChangeVolume(Sounds.AddTileBoard, 0.5f);

                    if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
                    {
                        CursorController.Instance.tutorialBadConnection = true;
                    }
                    GameManager.Instance.unsuccessfullConnectionCount++;

                    if (supPieceArray[currentRight].relevantSlice.hasSlice)
                    {
                        if (supPieceArray[currentRight].relevantSlice.isLimiter)
                        {
                            GameManager.Instance.unsuccessfullSlicesCount++;
                        }
                    }

                    supPieceArray[currentRight].isBadConnection = true;
                    supPieceArray[rightContested].isBadConnection = true;

                    if (supPieceArray[currentRight].relevantSlice)
                    {
                        supPieceArray[currentRight].relevantSlice.fulfilledCondition = false;
                    }

                }
                else
                {
                    if (supPieceArray[currentRight].relevantSlice.hasSlice)
                    {
                        if (supPieceArray[currentRight].relevantSlice.isLimiter)
                        {
                            if (!conditionmet)
                            {
                                GameManager.Instance.unsuccessfullSlicesCount++;
                            }
                            else
                            {
                                GameManager.Instance.unsuccessfullSlicesCount--;
                            }
                        }
                    }

                    CursorController.Instance.tutorialBadConnection = false;

                    supPieceArray[currentRight].isBadConnection = false;
                    supPieceArray[rightContested].isBadConnection = false;


                    supPieceArray[currentRight].SetConnectedMaterial();
                    supPieceArray[rightContested].SetConnectedMaterial();
                    cellList[cellIndex].rightParticleZone.GetChild(0).gameObject.SetActive(true);


                    if (!playedConnectedSound)
                    {
                        SoundManager.Instance.PlaySound(Sounds.TileMatch);
                    }

                    if (conditionmet)
                    {
                        GameManager.Instance.sliceManager.SetSliceSolvedSprite(supPieceArray[currentRight].relevantSlice);

                        //if (supPieceArray[currentRight].relevantSlice.anim)
                        //{
                        //supPieceArray[currentRight].relevantSlice.anim.SetBool("Activate", true);

                        if (supPieceArray[currentRight].relevantSlice.hasSlice)
                        {
                            if (supPieceArray[currentRight].relevantSlice.isLimiter)
                            {
                                SoundManager.Instance.PlaySound(Sounds.RuneLimiterMatch);

                                //supPieceArray[currentRight].relevantSlice.anim.SetBool("Reverse", false);
                            }
                            else
                            {
                                if (!supPieceArray[currentRight].relevantSlice.fulfilledCondition)
                                {
                                    SoundManager.Instance.PlaySound(Sounds.SliceLimiterMatch);
                                }
                            }
                        }
                        //}

                        supPieceArray[currentRight].relevantSlice.fulfilledCondition = true;
                        if (supPieceArray[currentRight].relevantSlice.isLoot)
                        {
                            GiveLootFromConnections(supPieceArray[currentRight].relevantSlice, supPieceArray[currentRight].relevantSlice.isLimiter);
                        }

                        if (supPieceArray[currentRight].relevantSlice.isLock)
                        {
                            StartCoroutine(LockCell(supPieceArray[currentRight].relevantSlice, supPieceArray[currentRight].relevantSlice.isLimiter));
                        }
                    }

                    if (GameManager.Instance.currentLevel.ver1Boss)
                    {
                        if (!BossBattleManager.instance.piecesToRemove.Contains(supPieceArray[currentRight].parentPiece))
                        {
                            BossBattleManager.instance.piecesToRemove.Add(supPieceArray[currentRight].parentPiece); // middle pice - if we already have this piece - we do not need it again!!
                        }

                        BossBattleManager.instance.piecesToRemove.Add(supPieceArray[rightContested].parentPiece); // right piece

                    }
                }
            }
            else
            {
                Debug.Log("Bad Connection Right Conetsted");
                GameManager.Instance.unsuccessfullConnectionCount++;
                supPieceArray[rightContested].isBadConnection = true;
            }
        }
        else
        {
            if (!isOuterCell)
            {
                if (supPieceArray[currentRight].relevantSlice.hasSlice)
                {
                    if (supPieceArray[currentRight].relevantSlice.isLimiter)
                    {
                        GameManager.Instance.unsuccessfullSlicesCount++;
                    }
                }
            }
        }

        if (!nextToOtherpiece && !supPieceArray[currentRight].GetComponentInParent<Piece>().isStone)
        {
            SoundManager.Instance.PlaySoundChangeVolume(Sounds.AddTileBoard, 0.5f);
        }

        if (lastPiece)
        {
            if (!GameManager.Instance.currentLevel.isBoss)
            {
                GameManager.Instance.CheckEndLevel(false);
            }
            else
            {
                if (!GameManager.Instance.currentLevel.ver1Boss)
                {
                    StartCoroutine(BossBattleManager.instance.CheckCompletedRingVer2Boss());
                }
            }
        }
    }


    public void ConnectionManagerAnim(int cellIndex, bool isOuterCell)
    {
        //Debug.LogError("here");
        StartCheckLeftConnectAnim(cellIndex, isOuterCell);

    }

    public void StartCheckLeftConnectAnim(int cellIndex, bool isOuterCell)
    {
        int leftContested = CheckIntRangeSubPieces((cellIndex * 2) - 1);
        int currentLeft = cellIndex * 2;

        bool isGoodConnectLeft = false;

        if (subPiecesOnBoard[leftContested] && subPiecesOnBoard[leftContested].parentPiece.partOfBoard)
        {
            nextToOtherpiece = true;

            if (subPiecesOnBoard[currentLeft])
            {
                if (CheckSubPieceConnection(subPiecesOnBoard[currentLeft], subPiecesOnBoard[leftContested], out bool conditionmet, out bool isGoodConnect))
                {
                    //Debug.Log("Good Connect Anim Left");
                    if (conditionmet)
                    {
                        isGoodConnectLeft = true;
                    }

                    StartCheckRightConnectAnim(cellIndex, isOuterCell, isGoodConnectLeft);
                }
                else
                {
                    StartCheckRightConnectAnim(cellIndex, isOuterCell, false);
                }
            }
        }
        else
        {
            StartCheckRightConnectAnim(cellIndex, isOuterCell, false); //// if there is no piece connected to left - check right side
        }
    }

    public void StartCheckRightConnectAnim(int cellIndex, bool isOuterCell, bool isGoodConnectLeft)
    {
        int rightContested = CheckIntRangeSubPieces((cellIndex * 2) + 2);

        int currentRight = cellIndex * 2 + 1;

        bool isGoodConnectRight = false;


        if (subPiecesOnBoard[rightContested] && subPiecesOnBoard[rightContested].parentPiece.partOfBoard)
        {
            nextToOtherpiece = true;

            if (subPiecesOnBoard[currentRight])
            {
                if (CheckSubPieceConnection(subPiecesOnBoard[currentRight], subPiecesOnBoard[rightContested], out bool conditionmet, out bool isGoodConnect))
                {
                    if (conditionmet)
                    {
                        isGoodConnectRight = true;
                    }

                    Debug.Log("Good Connect Anim Right");

                }
            }
        }



        StartCoroutine(DoConnectAnim(cellIndex, isGoodConnectLeft, isGoodConnectRight));
    }

    public IEnumerator DoConnectAnim(int cellIndex, bool isGoodConnectLeft, bool isGoodConnectRight)
    {
        int leftContested = CheckIntRangeSubPieces((cellIndex * 2) - 1);
        int currentLeft = cellIndex * 2;

        int rightContested = CheckIntRangeSubPieces((cellIndex * 2) + 2);
        int currentRight = cellIndex * 2 + 1;

        if (isGoodConnectLeft)
        {
            subPiecesOnBoard[leftContested].GetComponent<CameraShake>().ShakeOnce();
            subPiecesOnBoard[currentLeft].GetComponent<CameraShake>().ShakeOnce();
            subPiecesOnBoard[currentLeft].parentPiece.isDuringConnectionAnim = true;
            subPiecesOnBoard[leftContested].parentPiece.isDuringConnectionAnim = true;
            cells[cellIndex].isDuringConnectionAnim = true;
            Transform parentCell = cells[cellIndex].transform;
            LeanTween.move(subPiecesOnBoard[currentLeft].transform.parent.gameObject, new Vector3(parentCell.position.x, parentCell.position.y, parentCell.position.z), speedPieceConnectAnim).setEaseInExpo(); // animate
        }

        if (isGoodConnectRight)
        {
            subPiecesOnBoard[rightContested].GetComponent<CameraShake>().ShakeOnce();
            subPiecesOnBoard[currentRight].GetComponent<CameraShake>().ShakeOnce();
            subPiecesOnBoard[currentRight].parentPiece.isDuringConnectionAnim = true;
            subPiecesOnBoard[rightContested].parentPiece.isDuringConnectionAnim = true;
            cells[cellIndex].isDuringConnectionAnim = true;

            Transform parentCell = cells[cellIndex].transform;
            LeanTween.move(subPiecesOnBoard[currentRight].transform.parent.gameObject, new Vector3(parentCell.position.x, parentCell.position.y, parentCell.position.z), speedPieceConnectAnim).setEaseInExpo(); // animate
        }

        if (!isGoodConnectLeft && !isGoodConnectRight)
        {
            if (subPiecesOnBoard[currentLeft])
            {
                CursorController.Instance.SnapFollower(cells[cellIndex].transform);
            }

            yield break;
        }


        //this is after the connect anim
        yield return new WaitForSeconds(speedPieceConnectAnim - 0.05f);

        if (subPiecesOnBoard[currentLeft].parentPiece)
        {
            if (subPiecesOnBoard[currentLeft].parentPiece.isDuringConnectionAnim)
            {
                CursorController.Instance.SnapFollower(cells[cellIndex].transform);

                //if (isGoodConnectLeft)
                //{
                //    subPiecesOnBoard[currentLeft].parentPiece.isDuringConnectionAnim = false;
                //    subPiecesOnBoard[leftContested].parentPiece.isDuringConnectionAnim = false;
                //}

                //if (isGoodConnectRight)
                //{
                //    subPiecesOnBoard[currentRight].parentPiece.isDuringConnectionAnim = false;
                //    subPiecesOnBoard[rightContested].parentPiece.isDuringConnectionAnim = false;
                //}
            }
        }

        yield return new WaitForSeconds(speedPieceConnectAnim + 0.05f);
        
        if (isGoodConnectLeft)
        {
            subPiecesOnBoard[currentLeft].parentPiece.isDuringConnectionAnim = false;
            subPiecesOnBoard[leftContested].parentPiece.isDuringConnectionAnim = false;
        }

        if (isGoodConnectRight)
        {
            subPiecesOnBoard[currentRight].parentPiece.isDuringConnectionAnim = false;
            subPiecesOnBoard[rightContested].parentPiece.isDuringConnectionAnim = false;
        }

    }


    public void UpdateReleventSliceConnectAnim(Piece p)
    {
        p.leftChild.relevantSlice = slicesOnBoard[Mathf.CeilToInt((float)p.leftChild.subPieceIndex / 2)];


        if (p.rightChild.subPieceIndex >= subPiecesOnBoard.Length - 1)
        {
            p.rightChild.relevantSlice = slicesOnBoard[0];
        }
        else
        {
            p.rightChild.relevantSlice = slicesOnBoard[Mathf.CeilToInt((float)p.rightChild.subPieceIndex / 2)];
        }
    }

    public void NullifyReleventSliceAnim(Piece p)
    {
        p.leftChild.relevantSlice = null;
        p.rightChild.relevantSlice = null;
    }

    public bool CheckSubPieceConnection(SubPiece currentSide, SubPiece contestedSide, out bool conditionMet, out bool isGoodConnect)
    {
        conditionMet = true;
        isGoodConnect = false;

        if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.ShapeMatch)
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (result.gSymbolMatch)
            {
                isGoodConnect = true;
            }

            if (isGoodConnect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.ColorMatch)
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (result.gColorMatch)
            {
                isGoodConnect = true;
            }

            if (isGoodConnect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (currentSide.relevantSlice)
        {
            if (currentSide.relevantSlice.sliceCatagory != SliceCatagory.None)
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

                conditionMet = CheckFulfilledSliceCondition(currentSide.relevantSlice, result, currentSide, contestedSide);

                if (!currentSide.relevantSlice.isLimiter)
                {
                    if (isGoodConnect)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (conditionMet)
                    {
                        return true;
                    }
                    else
                    {
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

                if (isGoodConnect)
                {
                    return true;
                }
                else
                {
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

            if (isGoodConnect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public CompareResault TotalCheck(SubPiece current, SubPiece contested/*, PieceColor sCol, PieceSymbol sSym*/)
    {
        CompareResault result = new CompareResault();

        if (current && contested)
        {
            result.gColorMatch = EqualColorOrJoker(current.colorOfPiece, contested.colorOfPiece);
            result.gSymbolMatch = EqualSymbolOrJoker(current.symbolOfPiece, contested.symbolOfPiece);
        }

        return result;
    }
    public int CheckIntRangeSubPieces(int num)
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
    public int CheckIntRangeSubPiecesAlgoritm(int num)
    {
        if (num < 0)
        {
            return subPiecesOnBoardTempAlgoritm.Count - 1;
        }

        if (num >= subPiecesOnBoardTempAlgoritm.Count)
        {
            return 0;
        }

        return num;
    }

    public int CheckIntRangeCells(int num)
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
    public void FillSubPieceIndex()
    {
        for (int i = 0; i < subPiecesOnBoard.Length; i++)
        {
            if (subPiecesOnBoard[i])
            {
                subPiecesOnBoard[i].subPieceIndex = i;
            }
        }

        for (int i = 0; i < subPiecesDoubleRing.Length; i++)
        {
            if (subPiecesDoubleRing[i])
            {
                subPiecesDoubleRing[i].subPieceIndex = i;
            }
        }
    }
    public void RemoveSubPieceIndex(int i, bool isOuterCell)
    {
        if (isOuterCell)
        {
            subPiecesDoubleRing[i] = null;
        }
        else
        {
            subPiecesOnBoard[i] = null;
        }
    }
    public bool EqualColorOrJoker(PieceColor colA, PieceColor colB)/// Colorcheck is to see if we need to check color or symbol
    {
        if ((colA == colB || (colA == PieceColor.Joker || colB == PieceColor.Joker)) && (colA != PieceColor.None && colB != PieceColor.None))
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

    public void GiveLootFromConnections(Slice relevent, bool isLimiter)
    {
        //Debug.Log("Loot");
        //Debug.Log(relevent.lootPack);

        //if(relevent.lootPack != LootPacks.None)
        //{
        //    LootManager.Instance.currentLevelLootToGive.Add(relevent.lootPack);
        //}

        //LootManager.Instance.RollOnTable(relevent.lootPack);
        if (!isLimiter)
        {
            SpriteRenderer relevantSliceSR = relevent.child.GetComponent<SpriteRenderer>();
            //relevantSliceSR.color = new Color(relevantSliceSR.color.r, relevantSliceSR.color.g, relevantSliceSR.color.b, 0.4f);

            //SpriteRenderer ciconSR = relevent.child.transform.GetChild(0).GetComponent<SpriteRenderer>();

            //ciconSR.color = new Color(ciconSR.color.r, ciconSR.color.g, ciconSR.color.b, 0.4f);
            //relevent.lootIcon.GetComponent<Rigidbody2D>().simulated = true;
            //relevent.lootIcon.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);


            switch (relevent.lootPack.ToString()[0])
            {
                //case 'M':
                //    StartCoroutine(InstantiateLootEffect(relevent,relevent.lootIcon.transform, LootManager.Instance.goldSprite, LootTargetsData.instance.goldTargetLoot));
                //    break;

                case 'R':
                    //StartCoroutine(InstantiateLootEffect(relevent,relevent.lootIcon.transform, LootManager.Instance.rubySprite, LootTargetsData.instance.rubyTargetLoot));
                    AddRubiesToLoot(relevent);
                    break;

                case 'L':
                    if (relevent.lootPack != LootPacks.None)
                    {
                        AddMaterialsToLootList(relevent);
                        //StartCoroutine(InstantiateLootEffectMaterials(relevent, relevent.lootIcon.transform, LootTargetsData.instance.materialsTargetLoot));
                    }
                    break;

                default:
                    break;
            }

            relevent.isLoot = false;
        }
        else
        {
            switch (relevent.lootPack.ToString()[0])// TAKE CARE OF THIS IMMIDIETLY
            {
                //case 'M':
                //    StartCoroutine(InstantiateLootEffect(relevent, relevent.lootIcon.transform, relevent.lootIcon.GetComponent<SpriteRenderer>().sprite, LootTargetsData.instance.goldTargetLoot));
                //    break;

                case 'R':
                    //StartCoroutine(InstantiateLootEffect(relevent, relevent.lootIcon.transform, relevent.lootIcon.GetComponent<SpriteRenderer>().sprite, LootTargetsData.instance.rubyTargetLoot));
                    AddRubiesToLoot(relevent);
                    break;

                case 'L':
                    if (relevent.lootPack != LootPacks.None) //// this should not be here - check this before we even enter this area
                    {
                        AddMaterialsToLootList(relevent);
                        //StartCoroutine(InstantiateLootEffectMaterials(relevent, relevent.lootIcon.transform, LootTargetsData.instance.materialsTargetLoot));
                    }
                    break;

                default:
                    break;
            }

            relevent.isLoot = false;
        }
    }
    public IEnumerator LockCell(Slice relevent, bool isLimiter)
    {
        //Debug.Log("Lock");

        //cells[relevent.sliceIndex].lockSprite.SetActive(true);
        //cells[relevent.sliceIndex].pieceHeld.isLocked = true;

        //if (relevent.sliceIndex == 0)
        //{
        //    //cells[cells.Count - 1].lockSprite.SetActive(true);
        //    cells[cells.Count - 1].pieceHeld.isLocked = true;
        //}
        //else
        //{
        //    //cells[relevent.sliceIndex - 1].lockSprite.SetActive(true);
        //    cells[relevent.sliceIndex - 1].pieceHeld.isLocked = true;
        //}

        //if (!isLimiter)
        //{

        /// TURN ON HEIGHLIGHT ON SLICE HERE

        SoundManager.Instance.PlaySound(Sounds.TileLock);

        relevent.lockSpriteHeighlightAnim.gameObject.SetActive(true);
        GameManager.Instance.gameBoard.GetComponent<SliceManager>().activeLocksLockAnims.Add(relevent.lockSpriteHeighlightAnim.gameObject);

        foreach (Cell c in relevent.connectedCells)
        {
            Piece p = c.pieceHeld;
            p.isLocked = true;
            c.isLockedCell = true;
        }

        relevent.lockSpriteHeighlightAnim.SetBool("Lock", true);

        yield return new WaitForSeconds(0.15f);
        relevent.lockSpriteAnim.SetBool("Lock", true);
        //foreach (Cell c in relevent.connectedCells)  // old lock system
        //{
        //    Piece p = c.pieceHeld;
        //    //SubPiece rightPiece = c.pieceHeld.rightChild.GetComponent<SubPiece>();
        //    //SubPiece leftPiece = c.pieceHeld.leftChild.GetComponent<SubPiece>();
        //    if (!isLimiter)
        //    {
        //        if (c.cellIndex == relevent.sliceIndex)
        //        {
        //            if (!p.isLocked)/// IF THERE IS NO LOCK ALREADY ON PIECE
        //            {
        //                GameObject go = Instantiate(leftPieceLockObject, c.pieceHeld.leftChild.transform);
        //                p.isLocked = true;
        //            }

        //        }
        //        else
        //        {
        //            if (!p.isLocked)/// IF THERE IS NO LOCK ALREADY ON PIECE
        //            {
        //                GameObject go = Instantiate(rightPieceLockObject, c.pieceHeld.rightChild.transform);
        //                p.isLocked = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (c.cellIndex == relevent.sliceIndex)
        //        {
        //            if (!p.isLocked)/// IF THERE IS NO LOCK ALREADY ON PIECE
        //            {
        //                GameObject go = Instantiate(leftPieceLockLimiterObject, c.pieceHeld.leftChild.transform);
        //                p.isLocked = true;
        //            }
        //        }
        //        else
        //        {
        //            if (!p.isLocked)/// IF THERE IS NO LOCK ALREADY ON PIECE
        //            {
        //                GameObject go = Instantiate(rightPieceLockLimiterObject, c.pieceHeld.rightChild.transform);
        //                p.isLocked = true;
        //            }
        //        }
        //    }
        //}

        //relevent.isLock = false;
        //}
        //else
        //{
        //    foreach (Cell c in relevent.connectedCells)
        //    {
        //        if (c.cellIndex == relevent.sliceIndex)
        //        {
        //            Instantiate(leftPieceLockBrown, c.pieceHeld.leftChild.transform);
        //        }
        //        else
        //        {
        //            Instantiate(rightPieceLockBrown, c.pieceHeld.rightChild.transform);
        //        }
        //    }
        //    relevent.isLock = false;
        //}
    }

    public void UnlockPieces(Cell currentCell, Cell left, Cell right/*, bool destoryLock*/)
    {
        //if (destoryLock)
        //{
        //    foreach (Transform t in currentCell.pieceHeld.transform)
        //    {
        //        if (t.childCount > 0)
        //        {
        //            Destroy(t.GetChild(0).gameObject);
        //        }
        //    }

        //    currentCell.pieceHeld.isLocked = false;
        //}

        //if (currentCell.pieceHeld.leftChild.transform.childCount > 0)
        //{
        //    Destroy(currentCell.pieceHeld.leftChild.transform.GetChild(0).gameObject);
        //}

        //if (currentCell.pieceHeld.rightChild.transform.childCount > 0)
        //{
        //    Destroy(currentCell.pieceHeld.rightChild.transform.GetChild(0).gameObject);
        //}

        if (left.pieceHeld)
        {
            if (left.pieceHeld.rightChild.transform.childCount > 0)
            {
                //if (destoryLock)
                //{
                //    Destroy(left.pieceHeld.rightChild.transform.GetChild(0).gameObject);
                //}

                StartCoroutine(CheckAreCellsLocked(left));
            }
        }

        if (right.pieceHeld)
        {
            if (right.pieceHeld.leftChild.transform.childCount > 0)
            {
                //if (destoryLock)
                //{
                //    Destroy(right.pieceHeld.leftChild.transform.GetChild(0).gameObject);
                //}

                StartCoroutine(CheckAreCellsLocked(right));
            }
        }
    }

    private IEnumerator CheckAreCellsLocked(Cell toCheck) // THIS IS ENUMERATOR BCAUSE I'M DESTROYING THE LOCK IN THE SAME FRAME I'M CHECKING IF IT'S DESTROYED.. SO I NEED A DEALY
    {
        yield return new WaitForEndOfFrame();

        foreach (Transform t in toCheck.pieceHeld.transform)
        {
            if (t.childCount > 0)
            {
                yield break;
            }
        }

        toCheck.pieceHeld.isLocked = false;
        toCheck.isLockedCell = false;
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

    public void ResetConnectionData()
    {
        cells.Clear();
        tempSymbolPiecesStoneFound.Clear();
        amountStonePiecesInstantiated = 0;
        outerCells.Clear();
        subPiecesOnBoard = new SubPiece[0];
        subPiecesDoubleRing = new SubPiece[0];
        slicesOnBoard = new Slice[0];
    }


    //public IEnumerator InstantiateLootEffect(Slice relevent, Transform instantiateposition, Sprite look, Transform target)
    //{
    //    int amount = 0;
    //    switch (relevent.lootPack)
    //    {
    //        //case LootPacks.M1:
    //        //    amount = 2;
    //        //    break;
    //        //case LootPacks.M2:
    //        //    amount = 4;
    //        //    break;
    //        //case LootPacks.M3:
    //        //    amount = 6;
    //        //    break;
    //        case LootPacks.R1:
    //            amount = 2;
    //            break;
    //        case LootPacks.R2:
    //            amount = 4;
    //            break;
    //        default:
    //            break;
    //    }

    //    for (int i = 0; i < amount; i++)
    //    {

    //        GameObject go = Instantiate(lootEffectPrefab, instantiateposition.position, Quaternion.identity);

    //        MoveToLootTarget MTLT = go.GetComponent<MoveToLootTarget>();
    //        MTLT.look = look;
    //        MTLT.target = target;

    //        MTLT.LeanMove();
    //        yield return new WaitForSeconds(0.05f);
    //    }

    //    Destroy(relevent.lootIcon.gameObject);
    //}

    public void AddRubiesToLoot(Slice relevent)
    {
        int[] valuesToRecieve;

        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = LootManager.Instance.lootpackEnumToRewardBag[relevent.lootPack];

        valuesToRecieve = rewardBagByLootPack.minMaxValues;

        int randomNum = (Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));

        LootManager.Instance.rubiesToRecieveInLevel += randomNum;

        Debug.LogError("GAVE RUBIES");

        //Destroy(relevent.lootIcon.gameObject);
    }
    public void AddMaterialsToLootList(Slice relevent)
    {
        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = LootManager.Instance.lootpackEnumToRewardBag[relevent.lootPack];

        List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();

        for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
        {
            craftingMatsFromTables.AddRange(LootManager.Instance.itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

            int chance = Random.Range(1, 101);

            if (chance > rewardBagByLootPack.chancesPerItemTable[i])
            {
                //Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
                craftingMatsFromTables.Clear();
            }
            else
            {
                int randomMat = Random.Range(0, craftingMatsFromTables.Count);

                //Debug.Log(craftingMatsFromTables[randomMat]);

                LootToRecieve LTR = new LootToRecieve(craftingMatsFromTables[randomMat], Random.Range(1, 6));

                if (!LootManager.Instance.tempDataList.Contains(LTR.type))
                {
                    LootManager.Instance.craftingMatsLootForLevel.Add(LTR);
                    LootManager.Instance.tempDataList.Add(LTR.type);
                }
                else
                {
                    LootToRecieve LTR_exsists = LootManager.Instance.craftingMatsLootForLevel.Where(p => p.type == LTR.type).Single();
                    LTR_exsists.amount += LTR.amount;

                    Debug.Log("Added " + LTR.amount + " " + "To " + LTR_exsists.type);
                }

                Debug.LogError("GAVE LOOT");

                craftingMatsFromTables.Clear();
            }
        }
        //Destroy(relevent.lootIcon.gameObject);
    }
    //public IEnumerator InstantiateLootEffectMaterials(Slice relevent, Transform instantiateposition, Transform target)
    //{
    //    RewardBag rewardBagByLootPack = new RewardBag();

    //    rewardBagByLootPack = LootManager.Instance.lootpackEnumToRewardBag[relevent.lootPack];

    //    if (!rewardBagByLootPack.IsMoneyOrRubies)
    //    {
    //        List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();


    //        for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
    //        {
    //            craftingMatsFromTables.AddRange(LootManager.Instance.itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

    //            int chance = Random.Range(1, 101);

    //            if (chance > rewardBagByLootPack.chancesPerItemTable[i])
    //            {
    //                Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
    //                craftingMatsFromTables.Clear();
    //            }
    //            else
    //            {
    //                int randomMat = Random.Range(0, craftingMatsFromTables.Count);

    //                Debug.Log(craftingMatsFromTables[randomMat]);

    //                LootManager.Instance.craftingMatsLootForLevel.Add(craftingMatsFromTables[randomMat]);

    //                GameObject go = Instantiate(lootEffectPrefab, instantiateposition.position, Quaternion.identity);

    //                MoveToLootTarget MTLT = go.GetComponent<MoveToLootTarget>();
    //                MTLT.look = Resources.Load <Sprite>(MaterialsAndForgeManager.Instance.materialSpriteByName[craftingMatsFromTables[randomMat]]);
    //                MTLT.target = target;

    //                MTLT.LeanMove();
    //                yield return new WaitForSeconds(0.2f);

    //                craftingMatsFromTables.Clear();
    //            }
    //        }
    //    }
    //    Destroy(relevent.lootIcon.gameObject);
    //}

    public void JumpPiecesEffect(Piece p)
    {
        Vector3 originalPosReturnTo = Vector3.zero;

        foreach (Cell c in cells)
        {
            if (c.pieceHeld != null && c.pieceHeld != p && !c.pieceHeld.isLocked && !c.pieceHeld.isStone)
            {
                float toMoveZRight = Random.Range(upAmountPieceEffectMinZ, upAmountPieceEffectMaxZ);
                float toMoveZLeft = Random.Range(upAmountPieceEffectMinZ, upAmountPieceEffectMaxZ);

                float toMoveXRight = Random.Range(upAmountPieceEffectMinX, upAmountPieceEffectMaxX);
                float toMoveXLeft = Random.Range(upAmountPieceEffectMinX, upAmountPieceEffectMaxX);

                float toMoveYRight = Random.Range(upAmountPieceEffectMinY, upAmountPieceEffectMaxY);
                float toMoveYLeft = Random.Range(upAmountPieceEffectMinY, upAmountPieceEffectMaxY);


                Vector3 targetPosRight = new Vector3(c.pieceHeld.rightChild.transform.localPosition.x + toMoveXRight, c.pieceHeld.rightChild.transform.localPosition.y + toMoveYRight, c.pieceHeld.rightChild.transform.localPosition.z - toMoveZRight);
                Vector3 targetPosLeft = new Vector3(c.pieceHeld.leftChild.transform.localPosition.x + toMoveXLeft, c.pieceHeld.leftChild.transform.localPosition.y + toMoveYLeft, c.pieceHeld.leftChild.transform.localPosition.z - toMoveZLeft);

                //Debug.Log(c);
                //Debug.Log(c.pieceHeld);

                LeanTween.moveLocal(c.pieceHeld.rightChild.gameObject, targetPosRight, speedUpPieceEffect).setEaseOutBack(); // animate
                LeanTween.moveLocal(c.pieceHeld.leftChild.gameObject, targetPosLeft, speedUpPieceEffect).setEaseOutBack(); // animate

                //yield return new WaitForSeconds(speedUpPieceEffect);
                originalPosReturnTo = c.pieceHeld.leftChild.transform.localPosition;
                returnPieceToOriginPosUpEffect(c.pieceHeld.leftChild, originalPosReturnTo);

                originalPosReturnTo = c.pieceHeld.rightChild.transform.localPosition;
                returnPieceToOriginPosUpEffect(c.pieceHeld.rightChild, originalPosReturnTo);
            }
        }
    }

    public void returnPieceToOriginPosUpEffect(SubPiece p, Vector3 targetPos)
    {
        //targetPos = Vector3.zero;

        //if (p.isRightSubPiece)
        //{
        //    targetPos = new Vector3(0.7826648f, 0, 0);
        //}
        //else
        //{
        //    targetPos = new Vector3(-0.7826648f, 0, 0);
        //}

        //LeanTween.moveLocal(p.gameObject, targetPos, speedUpPieceEffect).setDelay(speedUpPieceEffect); // animate
        LeanTween.moveLocal(p.gameObject, targetPos, speedUpPieceEffect);// animate
    }


    public void TurnOffAllConnectedVFX()
    {
        foreach (Cell c in cells)
        {
            c.rightParticleZone.GetChild(0).gameObject.SetActive(false);
            c.leftParticleZone.GetChild(0).gameObject.SetActive(false);
        }
    }

    public bool CheckRepeatingStonePieces(Piece p)
    {
        foreach (Cell c in cells)
        {
            if (c.isFull)
            {
                return ComparerPiece(c.pieceHeld, p);
            }
        }

        return false;
    }

    public bool CheckHasEnoughSymbolsStonePieces(Piece p)
    {
        // we already take care of the fact of 1 stone tile since no tile can repeat itself.
        if (GameManager.Instance.currentLevel.stoneTiles.Count() == 1)
        {
            return true;
        }


        if (GameManager.Instance.currentLevel.stoneTiles.Count() == 2)
        {
            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Count() < 3)
            {
                if (tempSymbolPiecesStoneFound.Count >= GameManager.Instance.currentLevel.levelAvailablesymbols.Count())
                {
                    return true;
                }
            }
            else
            {
                if (tempSymbolPiecesStoneFound.Count >= 3)
                {
                    return true;
                }
            }
        }
        else if (GameManager.Instance.currentLevel.stoneTiles.Count() == 3)
        {
            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Count() < 4)
            {
                if (tempSymbolPiecesStoneFound.Count >= GameManager.Instance.currentLevel.levelAvailablesymbols.Count())
                {
                    return true;
                }
            }
            else
            {
                if (tempSymbolPiecesStoneFound.Count >= 4)
                {
                    return true;
                }
            }
        }
        else
        {
            if (GameManager.Instance.currentLevel.levelAvailablesymbols.Count() < 4)
            {
                if (tempSymbolPiecesStoneFound.Count >= GameManager.Instance.currentLevel.levelAvailablesymbols.Count())
                {
                    return true;
                }
            }
            else
            {
                if (tempSymbolPiecesStoneFound.Count >= 4)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void FindAmountOFSymbols(Piece currectCheckPiece, Piece p)
    {

        if ((currectCheckPiece.leftChild.symbolOfPiece != p.leftChild.symbolOfPiece) && (currectCheckPiece.leftChild.symbolOfPiece != p.rightChild.symbolOfPiece))
        {
            if (!tempSymbolPiecesStoneFound.Contains(currectCheckPiece.leftChild.symbolOfPiece))
            {
                tempSymbolPiecesStoneFound.Add(currectCheckPiece.leftChild.symbolOfPiece);
            }
        }
        
        if ((currectCheckPiece.rightChild.symbolOfPiece != p.rightChild.symbolOfPiece) && (currectCheckPiece.rightChild.symbolOfPiece != p.leftChild.symbolOfPiece))
        {
            if (!tempSymbolPiecesStoneFound.Contains(currectCheckPiece.rightChild.symbolOfPiece))
            {
                tempSymbolPiecesStoneFound.Add(currectCheckPiece.rightChild.symbolOfPiece);
            }
        }
    }

    public bool ComparerPiece(Piece currectCheckPiece, Piece p)
    {
        if ((currectCheckPiece.rightChild.colorOfPiece == p.rightChild.colorOfPiece) && (currectCheckPiece.rightChild.symbolOfPiece == p.rightChild.symbolOfPiece))
        {
            if ((currectCheckPiece.leftChild.colorOfPiece == p.leftChild.colorOfPiece) && (currectCheckPiece.leftChild.symbolOfPiece == p.leftChild.symbolOfPiece))
            {
                Debug.Log("Pieces were the same!" + currectCheckPiece + " " + p);
                return true; ///// Pieces are the same
            }
        }

        return false; //// Pieces are not the same
    }





    public void StartLastClipAlgoritm()
    {
        ResetAllLastPieceAlgoritmData();

        InitAlgoritmPiecesData();

        foreach (Cell cell in cells)
        {
            if (!cell.isFull)
            {
                Debug.LogError("Origin cell is: " + cell.name);
                originalMissingCell = cell;

                //StartCoroutine(RecursiveCheckes(cell));
                RecursiveCheckes(cell);

                break;
            }
        }

    }
    
    void InitAlgoritmPiecesData()
    {
        subPiecesOnBoardTempAlgoritm.AddRange(subPiecesOnBoard);
    }

    public void RecursiveCheckes(Cell CurrentEmptyCell)
    {
        //Debug.LogError("Started Recursive");

        //yield return new WaitForSeconds(5);

        //yield return StartCoroutine(CheckAllPossibleMovesToEmptyCell(CurrentEmptyCell));
        StartCoroutine(CheckAllPossibleMovesToEmptyCell(CurrentEmptyCell));
        //CheckAllPossibleMovesToEmptyCell(CurrentEmptyCell);
    }

    public IEnumerator CheckAllPossibleMovesToEmptyCell(Cell EmptyCell)
    {
        bool hasFoundOptions = false;

        cellsThatCanConnect.possibleCellsPath.Add(new CellsObjetData());
        mostCurrentListIndex = cellsThatCanConnect.possibleCellsPath.Count - 1;

        //yield return new WaitForSeconds(5);
        Cell previousCell = null;

        if (previousEmptyCells.Count > 0)
        {
            previousCell = previousEmptyCells.Last();
        }

        foreach (Cell cellCompareTo in cells)
        {
            if(cellCompareTo != previousCell && cellCompareTo != EmptyCell && !cellCompareTo.isLockedCell && !cellCompareTo.isStoneCell)
            {
                if (CheckConnectionsAlgoritm(EmptyCell, cellCompareTo))
                {
                    cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells.Add(cellCompareTo);
                }
            }
        }

        //yield return new WaitForSeconds(5);

        //yield return new WaitForSeconds(5);

        if (cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells.Count == 0)
        {
            /// we have no moves to make from this index.
            hasFoundOptions = false;

            yield return new WaitForSeconds(TEMPDelayTime);


            Debug.LogError("Called from second!!");

            //yield return StartCoroutine(MoveBackInList(EmptyCell));
            MoveBackInList(EmptyCell);

            //Debug.Break();
        }
        else
        {
            hasFoundOptions = true;
        }

        if(mostCurrentListIndex >= 0)
        {
            movesMade++;

            int tempCheck = 0;

            if (movesMade == GameManager.Instance.currentLevel.algoritmStepsWanted)
            {
                //yield return new WaitForSeconds(5);

                foreach (Cell edgeCell in cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells)
                {
                    tempCheck++;

                    if (pathsFound.foundCellPath.Count == 500)
                    {
                        Debug.LogError("Done with alogoritm, no more needed");

                        decidedAlgoritmPath = ChooseRandomFoundPathAlgo();
                        hasFinishedAlgorithm = true;
                        yield break;
                        //return;
                    }


                    pathsFound.foundCellPath.Add(new EdgePathFoundData());
                    int lastIndex = pathsFound.foundCellPath.Count - 1;

                    pathsFound.foundCellPath[lastIndex].foundCells.AddRange(bufferList);
                    pathsFound.foundCellPath[lastIndex].foundCells.Add(edgeCell);
                    pathsFound.foundCellPath[lastIndex].lastCellToMove = edgeCell;

                    //Debug.LogError("Doing this now!!");

                    yield return new WaitForSeconds(TEMPDelayTime);
                    ChangeSubPiecesAlgorithmData(edgeCell, EmptyCell);

                    Debug.LogError("Setting end data path!");
                    yield return new WaitForSeconds(TEMPDelayTime);

                    SetDataEndPath(edgeCell, lastIndex);

                    yield return new WaitForSeconds(TEMPDelayTime);
                    ChangeSubPiecesAlgorithmData(EmptyCell, edgeCell);

                }

                //Debug.LogError(tempCheck);

                //Debug.LogError("Has enough moves!");

                Debug.LogError("Moving back in list after path!");
                yield return new WaitForSeconds(TEMPDelayTime);

                //yield return StartCoroutine(MoveBackInList(EmptyCell));
                MoveBackInList(EmptyCell);

                if (cellsThatCanConnect.possibleCellsPath.Count == 0)
                {
                    Debug.LogError("Done with algoritm not reached max options");

                    if(pathsFound.foundCellPath.Count > 0)
                    {
                        decidedAlgoritmPath = ChooseRandomFoundPathAlgo();
                    }
                    else
                    {
                        Debug.LogError("Can't find path - no path in list");
                    }
                    ///summon the correct piece here!!!!
                    hasFinishedAlgorithm = true;

                    //return;
                    yield break;
                }

                if (mostCurrentListIndex >= 0)
                {
                    Cell newEmptyCell = cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells[0];
                    bufferList.Add(newEmptyCell);


                    yield return new WaitForSeconds(TEMPDelayTime);

                    ChangeSubPiecesAlgorithmData(newEmptyCell, previousEmptyCells.Last());

                    yield return new WaitForSeconds(TEMPDelayTime);
                    //Debug.LogError("Added to buffer!!!");

                    //yield return new WaitForSeconds(5);

                    //StartCoroutine(RecursiveCheckes(newEmptyCell));

                    //previousEmptyCell = EmptyCell; // remeber the cell that we just moved to since we don't want to move from that cell.

                    RecursiveCheckes(newEmptyCell);
                }
                else
                {
                    Debug.Log("Done with alogoritm, no more options");
                    yield break;
                    //return;
                }
                //yield return new WaitForSeconds(5);

            }
            else
            {
                if (hasFoundOptions)
                {
                    if (previousEmptyCells.Count == 0)
                    {
                        previousEmptyCells.Add(originalMissingCell);
                    }
                    else
                    {
                        previousEmptyCells.Add(EmptyCell); // remeber the cell that we just moved to since we don't want to move from that cell.
                    }
                }

                Cell newEmptyCell = cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells[0];
                bufferList.Add(newEmptyCell);

                Debug.LogError("Updated buffer");
                yield return new WaitForSeconds(TEMPDelayTime);

                //Debug.LogError("Added to buffer!");

                ChangeSubPiecesAlgorithmData(newEmptyCell, previousEmptyCells.Last());

                Debug.LogError("Moved Piece");

                yield return new WaitForSeconds(TEMPDelayTime);

                //yield return new WaitForSeconds(5);

                //StartCoroutine(RecursiveCheckes(newEmptyCell));



                RecursiveCheckes(newEmptyCell);
            }

        }
        else
        {
            Debug.LogError("Done!!!");

            if (pathsFound.foundCellPath.Count > 0)
            {
                decidedAlgoritmPath = ChooseRandomFoundPathAlgo();
            }
            else
            {
                Debug.LogError("Can't find path - no path in list");
            }

            hasFinishedAlgorithm = true;

        }
    }

    void ChangeSubPiecesAlgorithmData(Cell cellFrom, Cell cellTo)
    {
        int fromIndex = cellFrom.cellIndex;
        int toIndex = cellTo.cellIndex;

        int CellFromSubPieceLeft = CheckIntRangeSubPieces(fromIndex * 2);
        int CellFromSubPieceRight = CheckIntRangeSubPieces((fromIndex * 2) + 1);

        int CellToSubPieceLeft = CheckIntRangeSubPieces(toIndex * 2);
        int CellToSubPieceRight = CheckIntRangeSubPieces((toIndex * 2) + 1);

        subPiecesOnBoardTempAlgoritm[CellToSubPieceLeft] = subPiecesOnBoardTempAlgoritm[CellFromSubPieceLeft];
        subPiecesOnBoardTempAlgoritm[CellToSubPieceRight] = subPiecesOnBoardTempAlgoritm[CellFromSubPieceRight];

        subPiecesOnBoardTempAlgoritm[CellFromSubPieceLeft] = null;
        subPiecesOnBoardTempAlgoritm[CellFromSubPieceRight] = null;

        Transform p = null;

        if (GameManager.Instance.currentLevel.is12PieceRing)
        {
            p = cellFrom.transform.Find("MDL_Tile_12Slots(Clone)");
        }
        else
        {
            p = cellFrom.transform.Find("New piece Prefab 23_2_2021(Clone)");
        }

        if(p == null)
        {
            Debug.LogError("NO CHILD HERE BUG! " + "Cell From = " + cellFrom.name + " Cell To = " + cellTo.name);

            
        }

        p.transform.SetParent(cellTo.transform);
        p.transform.localPosition = Vector3.zero;
        p.transform.localRotation = Quaternion.identity;
    }

    void MoveBackInList(Cell currentEmptyCell)
    {
        do
        {
            cellsThatCanConnect.possibleCellsPath.Remove(cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex]);

            //yield return new WaitForSeconds(delayTime);

            mostCurrentListIndex--;
            movesMade--;

            //yield return new WaitForSeconds(delayTime);

            if (mostCurrentListIndex >= 0)
            {
                cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells.RemoveAt(0);

                //yield return new WaitForSeconds(delayTime);

                if (previousEmptyCells.Count > 0)
                {
                    ChangeSubPiecesAlgorithmData(previousEmptyCells.Last(), bufferList.Last());
                }

                //yield return new WaitForSeconds(delayTime);

                Debug.LogError("Right after move pieces from in list");
                //Debug.Break();

                if (cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells.Count == 0)
                {
                    previousEmptyCells.RemoveAt(previousEmptyCells.Count - 1);
                }

                //yield return new WaitForSeconds(delayTime);

                bufferList.RemoveAt(bufferList.Count - 1);
            }
            else
            {
                break;
            }

        } while (cellsThatCanConnect.possibleCellsPath[mostCurrentListIndex].possibleCells.Count == 0);
    }

    bool CheckConnectionsAlgoritm(Cell EmptyCell, Cell checkAgainst)
    {
        int mycellIndex = EmptyCell.cellIndex;
        int againstCellIndex = checkAgainst.cellIndex;

        int leftContested = CheckIntRangeSubPieces((mycellIndex * 2) - 1);
        int rightContested = CheckIntRangeSubPieces((mycellIndex * 2) + 2);

        int currentCellLeft = CheckIntRangeSubPieces(againstCellIndex * 2);
        int currentCellRight = CheckIntRangeSubPieces((againstCellIndex * 2) + 1);

        bool conditionmet;
        bool isGoodConnect;

        bool adjacentLeft;
        bool adjacentRight;

        CheckCellAdjecency(EmptyCell, checkAgainst, out adjacentLeft, out adjacentRight);

        if (adjacentLeft)
        {
            if (CheckSubPieceConnectionAlgoritm(false, subPiecesOnBoardTempAlgoritm[rightContested], subPiecesOnBoardTempAlgoritm[currentCellRight], EmptyCell, out conditionmet, out isGoodConnect))
            {
                if (!conditionmet)
                {

                }
                else
                {
                    return true;
                }
            }
        }

        if (adjacentRight)
        {
            if (CheckSubPieceConnectionAlgoritm(true, subPiecesOnBoardTempAlgoritm[leftContested], subPiecesOnBoardTempAlgoritm[currentCellLeft], EmptyCell, out conditionmet, out isGoodConnect))
            {
                if (!conditionmet)
                {

                }
                else
                {
                    return true;
                }
            }
        }


        if (CheckSubPieceConnectionAlgoritm(true, subPiecesOnBoardTempAlgoritm[leftContested], subPiecesOnBoardTempAlgoritm[currentCellLeft], EmptyCell, out conditionmet, out isGoodConnect))
        {
            if (CheckSubPieceConnectionAlgoritm(false, subPiecesOnBoardTempAlgoritm[rightContested], subPiecesOnBoardTempAlgoritm[currentCellRight], EmptyCell, out conditionmet, out isGoodConnect))
            {
                if (!conditionmet)
                {

                }
                else
                {
                    return true;
                }
            }
        }

        return false;

    }

    void CheckCellAdjecency(Cell currentEmptyCell, Cell cellToCheck, out bool adjacentLeft, out bool adjacentRight)
    {
        int emptyCellIndex = currentEmptyCell.cellIndex;
        int cellToCheckIndex = cellToCheck.cellIndex;

        adjacentLeft = false;
        adjacentRight = false;

        if (cellToCheckIndex == CheckIntRangeCells(emptyCellIndex - 1))
        {
            adjacentLeft = true;
            return;
        }

        if (cellToCheckIndex == CheckIntRangeCells(emptyCellIndex + 1))
        {
            adjacentRight = true;
            return;
        }
    }
 
    void SetDataEndPath(Cell edgeCell, int index)
    {
        int edgeCellIndex = edgeCell.cellIndex;
        //int emptyCellIndex = emptyCell.cellIndex;

        int leftContested = CheckIntRangeSubPiecesAlgoritm((edgeCellIndex * 2) - 1);
        int rightContested = CheckIntRangeSubPiecesAlgoritm((edgeCellIndex * 2) + 2);

        //int localLeft = CheckIntRangeSubPiecesAlgoritm(edgeCellIndex * 2);
        //int localRight = CheckIntRangeSubPiecesAlgoritm((edgeCellIndex * 2) + 1);

        //bool adjacentLeft;
        //bool adjacentRight;

        //CheckCellAdjecency(emptyCell, edgeCell, out adjacentLeft, out adjacentRight);

        //if (adjacentLeft)
        //{
        //    Debug.LogError("Adjacent Left: " + emptyCell.name + " " + edgeCell.name);

        //    pathsFound.foundCellPath[index].leftAnimalSymbolNeeded = subPiecesOnBoardTempAlgoritm[leftContested].symbolOfPiece;
        //    pathsFound.foundCellPath[index].leftColorNeeded = subPiecesOnBoardTempAlgoritm[leftContested].colorOfPiece;

        //    pathsFound.foundCellPath[index].rightAnimalSymbolNeeded = subPiecesOnBoardTempAlgoritm[rightContested].symbolOfPiece;
        //    pathsFound.foundCellPath[index].rightColorNeeded = subPiecesOnBoardTempAlgoritm[rightContested].colorOfPiece;

        //    return;
        //}
        //else if (adjacentRight)
        //{
        //    Debug.LogError("Adjacent Right: " + emptyCell.name + " " + edgeCell.name);


        //    pathsFound.foundCellPath[index].leftAnimalSymbolNeeded = subPiecesOnBoardTempAlgoritm[leftContested].symbolOfPiece;
        //    pathsFound.foundCellPath[index].leftColorNeeded = subPiecesOnBoardTempAlgoritm[leftContested].colorOfPiece;

        //    pathsFound.foundCellPath[index].rightAnimalSymbolNeeded = subPiecesOnBoardTempAlgoritm[rightContested].symbolOfPiece;
        //    pathsFound.foundCellPath[index].rightColorNeeded = subPiecesOnBoardTempAlgoritm[rightContested].colorOfPiece;

        //    return;

        //}
        //else
        //{
            //Debug.LogError("Not adjacent: " + emptyCell.name + " " + edgeCell.name);

            pathsFound.foundCellPath[index].leftAnimalSymbolNeeded = subPiecesOnBoardTempAlgoritm[leftContested].symbolOfPiece;
            pathsFound.foundCellPath[index].leftColorNeeded = subPiecesOnBoardTempAlgoritm[leftContested].colorOfPiece;

            pathsFound.foundCellPath[index].rightAnimalSymbolNeeded = subPiecesOnBoardTempAlgoritm[rightContested].symbolOfPiece;
            pathsFound.foundCellPath[index].rightColorNeeded = subPiecesOnBoardTempAlgoritm[rightContested].colorOfPiece;

            //return;
        //}
    }

    public bool CheckSubPieceConnectionAlgoritm(bool isLeft, SubPiece currentSide, SubPiece contestedSide, Cell cellTo, out bool conditionMet, out bool isGoodConnect)
    {
        conditionMet = true;
        isGoodConnect = false;

        if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.ShapeMatch)
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (result.gSymbolMatch)
            {
                isGoodConnect = true;
            }

            if (isGoodConnect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.ColorMatch)
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (result.gColorMatch)
            {
                isGoodConnect = true;
            }

            if (isGoodConnect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        if (isLeft)
        {
            if (cellTo.leftSlice.sliceCatagory != SliceCatagory.None)
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

                conditionMet = CheckFulfilledSliceCondition(cellTo.leftSlice, result, currentSide, contestedSide);

                if (!cellTo.leftSlice.isLimiter)
                {
                    if (isGoodConnect)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (conditionMet)
                    {
                        return true;
                    }
                    else
                    {
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

                if (isGoodConnect)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {

            if (cellTo.rightSlice.sliceCatagory != SliceCatagory.None)
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

                conditionMet = CheckFulfilledSliceCondition(cellTo.rightSlice, result, currentSide, contestedSide);

                if (!cellTo.rightSlice.isLimiter)
                {
                    if (isGoodConnect)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (conditionMet)
                    {
                        return true;
                    }
                    else
                    {
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

                if (isGoodConnect)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public EdgePathFoundData ChooseRandomFoundPathAlgo()
    {
        EdgePathFoundData path = null;

        int randomNum = Random.Range(0, pathsFound.foundCellPath.Count);

        path = pathsFound.foundCellPath[randomNum];

        if(path == null)
        {
            Debug.LogError("Big problem here");
            return null;
        }
        else
        {
            return path;
        }
    }

    public void ResetAllLastPieceAlgoritmData()
    {
        cellsThatCanConnect.possibleCellsPath.Clear();
        pathsFound.foundCellPath.Clear();
        bufferList.Clear();
        subPiecesOnBoardTempAlgoritm.Clear();
        previousEmptyCells.Clear();

        mostCurrentListIndex = 0;
        movesMade = 0;

        originalMissingCell = null;
        decidedAlgoritmPath = null;

        hasFinishedAlgorithm = false;
    }
}
