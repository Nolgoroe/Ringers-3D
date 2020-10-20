using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
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

    bool hasclicked;
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mouseRay.origin, mouseRay.origin + rayLength * mouseRay.direction);

        Gizmos.DrawSphere(cursorPos.position, radiusCollide);
    }

    public void Init()
    {
        gameBoard = GameManager.Instance.gameBoard;
        gameClip = GameManager.Instance.gameClip;
    }

    void Update()
    {
        if (GameManager.Instance.gameStarted)
        {
            if (PowerUpManager.IsUsingPowerUp)
            {
                if (Input.touchCount > 0)
                {
                    touch = Input.GetTouch(0);

                    if (!hasclicked)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            Debug.Log("here");
                            hasclicked = true;
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
                                GameManager.Instance.powerupManager.FinishedUsingPowerup(false, GameManager.Instance.powerupManager.currentlyInUse);
                            }
                        }

                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        hasclicked = false;
                    }

                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        mouseRay = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));

                        transform.position = mouseRay.origin;
                        cursorPos.position = mouseRay.origin + mouseRay.direction * distanceFromBoard;
                        cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);

                        RaycastHit hit;

                        if (Physics.Raycast(mouseRay, out hit, rayLength, pieceLayer))
                        {
                            //Debug.Log(hit.transform.name);
                            GrabPiece(hit.transform.parent);
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
                            SnapFollower(hit.transform);
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
                                SnapFollower(closest.transform);
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
        }
    }

    public void GrabPiece(Transform FT)
    {
        if (!FT.GetComponent<Piece>().isLocked)
        {
            followerTarget = FT;
            Cell c = FT.transform.parent.GetComponent<Cell>();

            if (c)
            {
                c.RemovePiece();
            }
        }
    }

    public void MoveFollower()
    {
        followerTarget.position = cursorPos.position;
        followerTarget.position += new Vector3(0, 0, piecePickupHeight);
        float angle = Mathf.Atan2(gameBoard.transform.position.y - followerTarget.position.y, gameBoard.transform.position.x - followerTarget.position.x) * Mathf.Rad2Deg;
        followerTarget.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    public void SnapFollower(Transform cellHit)
    {
        if (cellHit != null)
        {
            if (followerTarget)
            {
                Cell cell = cellHit.GetComponent<Cell>();
                Cell previousCell = followerTarget.parent.GetComponent<Cell>(); //// Only relevant if piece is moved from cell to cell

                bool newPiece = followerTarget.transform.parent.CompareTag("Clip");

                if (newPiece && !cell.isFull)
                {
                    GameManager.Instance.clipManager.PopulateSlot(followerTarget.transform.parent);
                }


                if (!cell.isFull)
                {
                    cell.AddPiece(followerTarget, newPiece);
                    GameManager.Instance.CheckCanEndLevel();

                    if (!newPiece)
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
    }

    public void ReturnHome()
    {
        followerTarget.transform.position = new Vector3(followerTarget.transform.parent.position.x, followerTarget.transform.parent.position.y, followerTarget.transform.parent.position.z + 0.1f);
        followerTarget.rotation = followerTarget.transform.parent.rotation;

        if (followerTarget.transform.parent.GetComponent<Cell>())
        {
            followerTarget.transform.parent.GetComponent<Cell>().AddPiece(followerTarget, false);
        }

        followerTarget = null;
    }
}
