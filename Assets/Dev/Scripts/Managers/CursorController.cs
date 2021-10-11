using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CursorController : MonoBehaviour
{
    public static CursorController Instance;
    public LayerMask pieceLayer;
    public LayerMask boardCellLayer;

    public Transform cursorPos;
    public Transform followerTarget;

    public GameObject gameBoard; ////// turn private later
    public GameObject gameClip; ////// turn private later

    public float gameplayDepth;
    public float distanceFromBoard;
    public float rayLength;
    public float radiusCollide;
    public float piecePickupHeight;

    Touch touch;
    Ray mouseRay;

    bool hasclickedPowerUp;

    //public Color secondaryControlsPieceColor;
    public GameObject secondaryControlsCellChosenPrefab;
    public GameObject secondaryControlsTileHighlightChosenPrefab;
    public Material secondaryControlsPieceMat;
    public Color stonePieceColorTint;

    private Cell cellhitSecondaryControls;
    private GameObject tempTileHighlight;

    [HideInInspector]
    public bool tutorialBadConnection = false;
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mouseRay.origin, mouseRay.origin + rayLength * mouseRay.direction);

        Gizmos.DrawSphere(cursorPos.position, radiusCollide);
    }
    private void Start()
    {
        Instance = this;
    }
    public void Init()
    {
        gameBoard = GameManager.Instance.gameBoard;
        gameClip = GameManager.Instance.gameClip;
    }
    void Update()
    {
        if(UIManager.isUsingUI && UIManager.Instance.UnlockedZoneMessageView.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    SoundManager.Instance.audioSource.Stop();
                    UIManager.Instance.UnlockedZoneMessageView.SetActive(false);
                    Debug.Log("THIS IS HOW MANY TIMES");
                    if (ZoneManager.Instance.zonesToUnlock.Count <= 0)
                    {

                        UIManager.isUsingUI = false;
                    }
                    else
                    {
                        ZoneManager.Instance.UnlockLevelViewSequence();
                    }
                }
            }
        }

        if (PowerUpManager.IsUsingPowerUp)
        {
            PowerUpControls();
        }

        if (GameManager.Instance.gameStarted && !UIManager.isUsingUI && !GameManager.Instance.isSecondaryControls)
        {
            NormalControls();
        }

        if (GameManager.Instance.gameStarted && !UIManager.isUsingUI && GameManager.Instance.isSecondaryControls)
        {
            SecondaryControls();
        }
    }
    public void SecondaryControls()
    {

        if (Input.touchCount > 0 /*&& Input.touchCount < 2*/)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                hasclickedPowerUp = false;

                mouseRay = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));

                if (!followerTarget && !cellhitSecondaryControls)
                {
                    transform.position = mouseRay.origin;
                    cursorPos.position = mouseRay.origin + mouseRay.direction * distanceFromBoard;

                    Debug.Log(cursorPos.position + " Follower");

                    if (gameBoard)
                    {
                        cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);
                    }

                    RaycastHit hit;

                    if (Physics.Raycast(mouseRay, out hit, rayLength, pieceLayer))
                    {
                        Piece p = hit.transform.parent.GetComponent<Piece>();
                        GrabPiece(p);
                        SetSecondaryControlsPieceColor(followerTarget);
                    }

                    if (!followerTarget)
                    {
                        if (Physics.Raycast(mouseRay, out hit, rayLength, boardCellLayer))
                        {
                            cellhitSecondaryControls = hit.transform.GetComponent<Cell>();
                            Instantiate(secondaryControlsCellChosenPrefab, cellhitSecondaryControls.transform);
                        }
                    }
                }
                else
                {
                    cursorPos.position = mouseRay.origin + mouseRay.direction * distanceFromBoard;
                    cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);

                    Debug.Log(cursorPos.position);

                    RaycastHit hit;

                    if (!followerTarget)
                    {
                        if (Physics.Raycast(mouseRay, out hit, rayLength, pieceLayer))
                        {
                            Piece p = hit.transform.parent.GetComponent<Piece>();
                            GrabPiece(p);

                            DestroySecondaryControlsPrefabCell(true);

                            SnapFollower(cellhitSecondaryControls.transform);
                        }
                        else
                        {
                            DestroySecondaryControlsPrefabCell(false);
                        }
                    }
                    else
                    {
                        if (Physics.Raycast(mouseRay, out hit, rayLength, boardCellLayer))
                        {
                            if (followerTarget)
                            {
                                ResetSecondaryControlsPieceColor(followerTarget);
                                SnapFollower(hit.transform);
                            }
                        }
                        else
                        {
                            float minDist = 1000;

                            Collider closest = null;

                            Collider[] hitColliders = Physics.OverlapSphere(cursorPos.position, radiusCollide, boardCellLayer);

                            if (hitColliders.Length != 0)
                            {
                                foreach (Collider col in hitColliders)
                                {
                                    if (Vector3.Distance(col.transform.position, cursorPos.transform.position) < minDist)
                                    {
                                        minDist = Vector3.Distance(col.transform.position, cursorPos.transform.position);
                                        closest = col;
                                    }
                                }
                            }

                            if (closest != null)
                            {
                                if (followerTarget)
                                {
                                    ResetSecondaryControlsPieceColor(followerTarget);

                                    SnapFollower(closest.transform);
                                }
                            }
                            else
                            {
                                ResetSecondaryControlsPieceColor(followerTarget);

                                SnapFollower(null);
                            }
                        }
                    }
                }
            }
        }
    }
    void SetSecondaryControlsPieceColor(Transform piece)
    {
        Piece p = piece.GetComponent<Piece>();

        tempTileHighlight = Instantiate(secondaryControlsTileHighlightChosenPrefab, p.transform);
        Renderer rightWing = p.rightChild.GetComponent<Renderer>();
        Renderer LeftWing = p.leftChild.GetComponent<Renderer>();

        List<Material> matArrayRight = new List<Material>();
        List<Material> matArrayLeft = new List<Material>();

        matArrayRight.AddRange(rightWing.materials);
        matArrayLeft.AddRange(LeftWing.materials);

        matArrayRight.Add(secondaryControlsPieceMat);
        matArrayLeft.Add(secondaryControlsPieceMat);

        rightWing.materials = matArrayRight.ToArray();
        LeftWing.materials = matArrayLeft.ToArray();

        //rightWing.material.SetColor("_BaseColor", color);
        //LeftWing.material.SetColor("_BaseColor", color);
    }
    void ResetSecondaryControlsPieceColor(Transform piece)
    {
        Piece p = piece.GetComponent<Piece>();

        Renderer rightWing = p.rightChild.GetComponent<Renderer>();
        Renderer LeftWing = p.leftChild.GetComponent<Renderer>();

        List<Material> matArrayRight = new List<Material>();
        List<Material> matArrayLeft = new List<Material>();

        matArrayRight.AddRange(rightWing.materials);
        matArrayLeft.AddRange(LeftWing.materials);

        matArrayRight.RemoveAt(1);
        matArrayLeft.RemoveAt(1);

        rightWing.materials = matArrayRight.ToArray();
        LeftWing.materials = matArrayLeft.ToArray();

        Destroy(tempTileHighlight.gameObject);
        //rightWing.material.SetColor("_BaseColor", color);
        //LeftWing.material.SetColor("_BaseColor", color);
    }
    public void NormalControls()
    {
        if (Input.touchCount > 0 /*&& Input.touchCount < 2*/)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                hasclickedPowerUp = false;

                mouseRay = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));

                transform.position = mouseRay.origin;
                cursorPos.position = mouseRay.origin + mouseRay.direction * distanceFromBoard;

                if (gameBoard)
                {
                    cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);
                }

                RaycastHit hit;

                if (Physics.Raycast(mouseRay, out hit, rayLength, pieceLayer))
                {
                    Piece p = hit.transform.parent.GetComponent<Piece>();
                    //Debug.Log(hit.transform.name);
                    GrabPiece(p);
                }

                //// Shoot ray
                ///Hit Something
                ///If hit piece - cache referance for it
                ///Make piece follow mouse
            }

            if (touch.phase == TouchPhase.Moved)
            {
                mouseRay = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));
                //transform.position = mouseRay.origin;
                cursorPos.position = mouseRay.origin + mouseRay.direction * distanceFromBoard;
                cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);

                if (followerTarget)
                {
                    MoveFollower();
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit hit;

                if (Physics.Raycast(mouseRay, out hit, rayLength, boardCellLayer))
                {
                    if (followerTarget)
                    {

                        SnapFollower(hit.transform);
                    }

                }
                else
                {
                    float minDist = 1000;

                    Collider closest = null;

                    Collider[] hitColliders = Physics.OverlapSphere(cursorPos.position, radiusCollide, boardCellLayer);

                    if (hitColliders.Length != 0)
                    {
                        foreach (Collider col in hitColliders)
                        {
                            if (Vector3.Distance(col.transform.position, cursorPos.transform.position) < minDist)
                            {
                                minDist = Vector3.Distance(col.transform.position, cursorPos.transform.position);
                                closest = col;
                            }

                        }
                    }

                    if (closest != null)
                    {
                        if (followerTarget)
                        {
                            SnapFollower(closest.transform);
                        }
                    }
                    else
                    {
                        SnapFollower(null);
                    }
                }


                ///If we have a piece - drop it
                ///If its in the board - snap
                ///If not in board - Snap back to original pos and parent
                ///If on board but slot full - snap to origin
                ///
            }

        }
    }
    public void PowerUpControls()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (!hasclickedPowerUp)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("here");
                    hasclickedPowerUp = true;
                    mouseRay = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));

                    transform.position = mouseRay.origin;
                    cursorPos.position = mouseRay.origin + mouseRay.direction * distanceFromBoard;
                    cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);

                    RaycastHit hit;

                    if (Physics.Raycast(mouseRay, out hit, rayLength, GameManager.Instance.powerupManager.layerToHit))
                    {
                        PowerUpManager.IsUsingPowerUp = false;
                        PowerUpManager.HasUsedPowerUp = true;
                        PowerUpManager.ObjectToUsePowerUpOn = hit.transform.gameObject;
                    }
                    else
                    {
                        if (GameManager.Instance.powerupManager.currentlyInUse)
                        {
                            GameManager.Instance.powerupManager.FinishedUsingPowerup(false, GameManager.Instance.powerupManager.currentlyInUse);
                        }
                    }
                }

            }

            if (touch.phase == TouchPhase.Ended)
            {
                hasclickedPowerUp = false;
            }

        }
    }
    public void GrabPiece(Piece p)
    {
        if (!p.isLocked && !p.isTutorialLocked && !p.isStone)
        {
            followerTarget = p.transform;
            Cell c = p.transform.parent.GetComponent<Cell>();

            if (c)
            {
                c.RemovePiece();
            }
        }
    }
    public void MoveFollower()
    {
        followerTarget.position = new Vector3(cursorPos.position.x, cursorPos.position.y - 0.05f, -0.07f);
        float angle = Mathf.Atan2(gameBoard.transform.position.y - followerTarget.position.y, gameBoard.transform.position.x - followerTarget.position.x) * Mathf.Rad2Deg;
        followerTarget.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }
    public void SnapFollower(Transform cellHit)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            //if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequence].isBoardPhase || TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequence].isClipPhase)
            //{
            SnapFollowerTutorial(cellHit);
            //}
        }
        else
        {
            if (cellHit != null)
            {
                if (followerTarget)
                {
                    Cell cell = cellHit.GetComponent<Cell>();
                    Cell previousCell = followerTarget.parent.GetComponent<Cell>(); //// Only relevant if piece is moved from cell to cell

                    bool newPiece = followerTarget.transform.parent.CompareTag("Clip");

                    if (GameManager.Instance.currentFilledCellCount + 1 != GameManager.Instance.currentLevel.cellsCountInLevel)
                    {
                        if (newPiece && !cell.isFull)
                        {
                            GameManager.Instance.clipManager.PopulateSlot(followerTarget.transform.parent, 10);
                            AddNumAnimalsToBoard(followerTarget);
                        }
                    }
                    else
                    {
                        GameManager.Instance.clipManager.emptyClip = followerTarget.transform.parent;
                        GameManager.Instance.clipManager.latestPiece = followerTarget;
                        AddNumAnimalsToBoard(followerTarget);
                    }


                    if (!cell.isFull)
                    {
                        cell.AddPiece(followerTarget, newPiece);

                        if (!newPiece && cell != previousCell)
                        {
                            previousCell.isFull = false;
                        }
                    }
                    else
                    {
                        ReturnHome();
                    }

                    followerTarget = null;
                }
            }
            else
            {
                if (followerTarget)
                {
                    ReturnHome();
                }
            }

            if (GameManager.Instance.isSecondaryControls)
            {
                followerTarget = null;
                cellhitSecondaryControls = null;
            }
        }
    }
    private void SnapFollowerTutorial(Transform cellHit)
    {
        if (cellHit != null)
        {
            if (followerTarget)
            {
                bool isAccording = false;

                if (GameManager.Instance.currentLevel.tutorialIndexForList == 4)
                {
                    Cell cell = cellHit.GetComponent<Cell>();
                    Piece p = followerTarget.GetComponent<Piece>();
                    isAccording = SpecialTutorialConnectionLogic(cell.cellIndex, p);

                    if (!isAccording)
                    {
                        return;
                    }
                }

                if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[TutorialSequence.Instacne.currentPhaseInSequence].targetCells.Contains(cellHit.GetComponent<Cell>().cellIndex))
                {
                    Cell cell = cellHit.GetComponent<Cell>();

                    Transform currentParent = followerTarget.parent; //// ONLY IF WERE COMING FROM A CLIP THIS IS RELEVANT
                    Cell previousCell = followerTarget.parent.GetComponent<Cell>(); //// Only relevant if piece is moved from cell to cell

                    bool newPiece = followerTarget.transform.parent.CompareTag("Clip");
                    if (!cell.isFull && cell != previousCell)
                    {
                        cell.AddPiece(followerTarget, newPiece);

                        if (GameManager.Instance.currentFilledCellCount + 1 != GameManager.Instance.currentLevel.cellsCountInLevel && !tutorialBadConnection)
                        {
                            if (newPiece/* && !cell.isFull*/)
                            {
                                GameManager.Instance.clipManager.PopulateSlot(currentParent, 10);
                            }

                            TutorialSequence.Instacne.IncrementCurrentPhaseInSequence();
                        }
                        else
                        {
                            ReturnHome();

                            GameManager.Instance.clipManager.emptyClip = followerTarget.transform.parent;
                            GameManager.Instance.clipManager.latestPiece = followerTarget;
                        }

                        if (!newPiece && cell != previousCell)
                        {
                            previousCell.isFull = false;
                        }
                    }
                    else
                    {
                        ReturnHome();
                    }


                    followerTarget = null;

                }
                else
                {
                    ReturnHome();
                }
            }
        }
        else
        {
            if (followerTarget)
            {
                ReturnHome();
            }
        }
    }
    public void ReturnHome()
    {
        Vector3 home = GameManager.Instance.clipManager.piece.transform.position;
        followerTarget.localPosition = home;
        followerTarget.localRotation = Quaternion.Euler(0, 180, 67); ///// reset piece rotation to it's original local rotation

        if (followerTarget.transform.parent.GetComponent<Cell>())
        {
            followerTarget.transform.parent.GetComponent<Cell>().AddPiece(followerTarget, false);
        }

        followerTarget = null;
    }
    private bool SpecialTutorialConnectionLogic(int cellindex, Piece pieceHeld)
    {
        Slice relavent = GameManager.Instance.sliceManager.sliceSlots[cellindex].GetComponent<Slice>();

        if (relavent.sliceCatagory == SliceCatagory.None)
        {
            if (pieceHeld.rightChild.colorOfPiece != PieceColor.Blue)
            {
                ReturnHome();
                return false;
            }
        }
        else
        {
            if (pieceHeld.leftChild.colorOfPiece != PieceColor.Blue)
            {
                ReturnHome();
                return false;
            }
        }


        return true;
    }
    void AddNumAnimalsToBoard(Transform piece)
    {
        Piece p = piece.GetComponent<Piece>();

        SubPiece right = p.rightChild;
        SubPiece left = p.leftChild;

        foreach (NumAnimalTypedOnBoard NATB in GameManager.Instance.numAnimalsOnBoard)
        {
            if (NATB.animalSymbol == right.symbolOfPiece && NATB.animalSymbol == left.symbolOfPiece)
            {
                NATB.amount += 2;
                GameManager.Instance.powerupManager.UpdateSpecialPowerupsCount(2, NATB.animalSymbol);
                break;
            }

            if (NATB.animalSymbol == right.symbolOfPiece || NATB.animalSymbol == left.symbolOfPiece)
            {
                NATB.amount++;
                GameManager.Instance.powerupManager.UpdateSpecialPowerupsCount(1, NATB.animalSymbol);
            }
        }

    }
    void DestroySecondaryControlsPrefabCell(bool snappedFollower)
    {
        int numToDestroy = cellhitSecondaryControls.transform.childCount;

        for (int i = 0; i < numToDestroy; i++)
        {
            if (cellhitSecondaryControls.transform.GetChild(i).CompareTag("Secondary Destroy"))
            {
                Destroy(cellhitSecondaryControls.transform.GetChild(i).gameObject);
            }
        }

        if (!snappedFollower)
        {
            followerTarget = null;
            cellhitSecondaryControls = null;
        }
    }
}
