using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using GameAnalyticsSDK;

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

    public float pickupSpeed;
    public float moveSpeed;

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

    public GameObject previousHeighlightChosen;

    [HideInInspector]
    public bool tutorialBadConnection = false;

    public static bool OverUI;

    PanZoom pz = null;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mouseRay.origin, mouseRay.origin + rayLength * mouseRay.direction);

        Gizmos.DrawSphere(cursorPos.position, radiusCollide);
    }
    private void Start()
    {
        Instance = this;

        pz = Camera.main.GetComponent<PanZoom>();
    }
    public void Init()
    {
        gameBoard = GameManager.Instance.gameBoard;
        gameClip = GameManager.Instance.gameClip;
    }

    void Update()
    {
        //test game analytics custom events

        //if (Input.touchCount > 0)
        //{
        //    touch = Input.touches[0];

        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        GameAnalytics.NewDesignEvent("TouchDetected:TouchDown:Complete");
        //        GameAnalytics.NewDesignEvent("TouchDetected:TouchDown:Complete", 101);
        //    }
        //}


        if (UIManager.isDuringIntro && UIManager.canAdvanceIntro)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("ADVANCING");
                    StartCoroutine(UIManager.Instance.AdvanceIntroScreen());
                }
            }
        }

        if (TutorialSequence.Instacne.screensDeactivateOnTouch.Count > 0)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    TutorialSequence.Instacne.screensDeactivateOnTouch[0].SetActive(false);
                    TutorialSequence.Instacne.screensDeactivateOnTouch.Remove(TutorialSequence.Instacne.screensDeactivateOnTouch[0]);
                }
            }
        } // deactiavte screens on touch from that list

        if (TutorialSequence.Instacne.duringSequence)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    if (GameManager.Instance.currentLevel)
                    {
                        if (GameManager.Instance.currentLevel.isSpecificTutorial)
                        {
                            if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isEmptyTouchPhase)
                            {
                                TutorialSequence.Instacne.CheckEmptyTouchIncrementPhase(true);
                            }
                        }

                        if (GameManager.Instance.currentLevel.isTutorial)
                        {
                            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].isEmptyTouchPhase)
                            {
                                TutorialSequence.Instacne.CheckEmptyTouchIncrementPhase(false);
                            }
                        }
                    }
                }
            }
        } /// detect empty touch phase in tutorial sequence



        if (!GameManager.Instance.levelStarted && !UIManager.isUsingUI && GameManager.Instance.clickedPlayButton && !pz.isDragging && !TutorialSequence.Instacne.duringSequence)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Ended)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    Debug.Log(IsPointerOverUIObject());

                    if (IsPointerOverUIObject())
                    {
                        if (EventSystem.current.currentSelectedGameObject)
                        {
                            Debug.Log("Selected UI element: " + EventSystem.current.currentSelectedGameObject.name);

                            Debug.Log("Over UI");
                            OverUI = true;
                            return;
                        }
                    }
                    else
                    {
                        OverUI = false;
                    }

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("Level Button") || hit.collider.CompareTag("Grind Level Button"))
                        {
                            hit.transform.GetComponent<Interactable3D>().ShootEvent();
                        }
                    }
                }
            }
            else
            {
                OverUI = false;
            }
        } // detect touch on level prefab

        if (!GameManager.Instance.levelStarted && ZoneManager.CanUnlockZone && UIManager.Instance.UnlockedZoneMessageView.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    SoundManager.Instance.audioSourceSFX.Stop();
                    UIManager.Instance.UnlockedZoneMessageView.SetActive(false);
                    Debug.Log("THIS IS HOW MANY TIMES");
                    if (ZoneManager.Instance.zonesToUnlock.Count <= 0)
                    {
                        ZoneManager.CanUnlockZone = false;
                        UIManager.isUsingUI = false;


                        if(ZoneManager.Instance.unlockedZoneID.Count == 2)
                        {
                            UIManager.Instance.DisplayDailyRewardsScreen();
                        }
                    }
                    else
                    {
                        ZoneManager.CanUnlockZone = false;
                        ZoneManager.Instance.UnlockLevelViewSequence();
                    }
                }
            }
        } // detect deactivate unlocked level vfx


        if (!GameManager.Instance.levelStarted && UIManager.isUsingUI && UIManager.Instance.brewedPotionScreen.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    SoundManager.Instance.audioSourceSFX.Stop();
                    UIManager.Instance.brewedPotionScreen.SetActive(false);
                    Debug.Log("THIS IS HOW MANY TIMES");

                    //UIManager.isUsingUI = false;
                }
            }
        } // detect deactivate brewed potion vfx

        if (!GameManager.Instance.levelStarted && UIManager.isUsingUI && UIManager.Instance.craftedHollowItemScreen.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    SoundManager.Instance.audioSourceSFX.Stop();
                    UIManager.Instance.craftedHollowItemScreen.SetActive(false);
                    Debug.Log("THIS IS HOW MANY TIMES");

                    //UIManager.isUsingUI = false;
                }
            }
        } // detect deactivate brewed potion vfx




        if (GameManager.Instance.levelStarted && !UIManager.isUsingUI && !GameManager.Instance.isSecondaryControls && !PowerUpManager.IsUsingPowerUp)
        {
            NormalControls();
            return;
        } // normal controls

        if (GameManager.Instance.levelStarted && !UIManager.isUsingUI && GameManager.Instance.isSecondaryControls && !PowerUpManager.IsUsingPowerUp)
        {
            SecondaryControls();
        } // secondary controls

        if (GameManager.Instance.levelStarted && PowerUpManager.IsUsingPowerUp)
        {
            PowerUpControls();
            return;
        } // powerup controls
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

                            SnapFollower(cellhitSecondaryControls.transform, followerTarget);
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
                                SnapFollower(hit.transform, followerTarget);
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

                                    SnapFollower(closest.transform, followerTarget);
                                }
                            }
                            else
                            {
                                ResetSecondaryControlsPieceColor(followerTarget);

                                SnapFollower(null, followerTarget);
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

        //tempTileHighlight = Instantiate(secondaryControlsTileHighlightChosenPrefab, p.transform);
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

        //Destroy(tempTileHighlight.gameObject);
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
                    if (!p.isDuringConnectionAnim && !AnimationManager.instance.endLevelAnimationON)
                    {
                        GrabPiece(p);
                    }
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
                if (gameBoard)
                {
                    cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y, gameBoard.transform.position.z);
                }

                if (followerTarget && !followerTarget.GetComponent<Piece>().isDuringConnectionAnim)
                {
                    MoveFollower();
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit hit;

                if (Physics.Raycast(mouseRay, out hit, rayLength, boardCellLayer))
                {
                    if (TutorialSequence.Instacne.duringSequence)
                    {
                        if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.lootTutorial )
                        {
                            if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetCells.Length > 0)
                            {
                                if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetCells.Contains(hit.transform.GetComponent<Cell>().cellIndex))
                                {
                                    ConnectionManager.Instance.ConnectionManagerAnim(hit.transform.GetComponent<Cell>().cellIndex, hit.transform.GetComponent<Cell>().isOuter);
                                }
                                else
                                {
                                    SnapFollower(null, followerTarget);
                                }
                            }
                            else
                            {
                                SnapFollower(null, followerTarget);
                            }
                        }
                        else
                        {
                            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Length > 0)
                            {
                                if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Contains(hit.transform.GetComponent<Cell>().cellIndex))
                                {
                                    ConnectionManager.Instance.ConnectionManagerAnim(hit.transform.GetComponent<Cell>().cellIndex, hit.transform.GetComponent<Cell>().isOuter);
                                }
                                else
                                {
                                    SnapFollower(null, followerTarget);
                                }
                            }
                            else
                            {
                                SnapFollower(null, followerTarget);
                            }
                        }
                    }
                    else
                    {
                        if (followerTarget && !followerTarget.GetComponent<Piece>().isDuringConnectionAnim)
                        {
                            if (!hit.transform.GetComponent<Cell>().isFull && !hit.transform.GetComponent<Cell>().isDuringConnectionAnim)
                            {
                                Debug.Log("In first");
                                Debug.Log(hit.transform.name + "UAHSIUASUBFS");
                                ConnectionManager.Instance.ConnectionManagerAnim(hit.transform.GetComponent<Cell>().cellIndex, hit.transform.GetComponent<Cell>().isOuter);
                            }
                            else
                            {
                                SnapFollower(null, followerTarget);
                            }

                            //SnapFollower(hit.transform);
                        }
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
                        if (TutorialSequence.Instacne.duringSequence)
                        {
                            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Length > 0)
                            {
                                if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Contains(closest.transform.GetComponent<Cell>().cellIndex))
                                {
                                    ConnectionManager.Instance.ConnectionManagerAnim(closest.transform.GetComponent<Cell>().cellIndex, closest.transform.GetComponent<Cell>().isOuter);
                                }
                                else
                                {
                                    SnapFollower(null, followerTarget);
                                }
                            }
                            else
                            {
                                SnapFollower(null, followerTarget);
                            }

                        }
                        else
                        {
                            if (followerTarget && !followerTarget.GetComponent<Piece>().isDuringConnectionAnim)
                            {
                                if (!closest.transform.GetComponent<Cell>().isFull && !closest.transform.GetComponent<Cell>().isDuringConnectionAnim)
                                {
                                    ConnectionManager.Instance.ConnectionManagerAnim(closest.GetComponent<Cell>().cellIndex, closest.GetComponent<Cell>().isOuter);
                                    Debug.Log("In Second");
                                }
                                else
                                {
                                    SnapFollower(null, followerTarget);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (followerTarget)
                        {
                            if (!followerTarget.GetComponent<Piece>().isDuringConnectionAnim)
                            {
                                SnapFollower(null, followerTarget);
                            }
                        }
                    }
                }

                if (previousHeighlightChosen)
                {
                    previousHeighlightChosen.GetComponent<Cell>().TurnOffHighlighParticle();

                    previousHeighlightChosen = null;
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
                        if (TutorialSequence.Instacne.duringSequence)
                        {
                            if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetCells.Length > 0)
                            {
                                if (hit.transform.parent.GetComponent<Cell>())
                                {
                                    if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetCells.Contains(hit.transform.parent.GetComponent<Cell>().cellIndex))
                                    {
                                        PowerUpManager.IsUsingPowerUp = false;
                                        PowerUpManager.HasUsedPowerUp = true;
                                        PowerUpManager.ObjectToUsePowerUpOn = hit.transform.gameObject;
                                        LeanTween.scale(hit.transform.gameObject, new Vector3(hit.transform.localScale.x - 0.2f, hit.transform.localScale.y - 0.2f, 1), 0.1f).setOnComplete(() => ScaleGameObjectBack(hit.transform.gameObject));

                                    }
                                }
                            }


                            if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetSlices.Length > 0)
                            {
                                if (hit.transform.parent.GetComponent<Slice>())
                                {
                                    if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetSlices.Contains(hit.transform.parent.GetComponent<Slice>().sliceIndex))
                                    {
                                        PowerUpManager.IsUsingPowerUp = false;
                                        PowerUpManager.HasUsedPowerUp = true;
                                        PowerUpManager.ObjectToUsePowerUpOn = hit.transform.gameObject;
                                        LeanTween.scale(hit.transform.gameObject, new Vector3(hit.transform.localScale.x - 0.2f, hit.transform.localScale.y - 0.2f, 1), 0.1f).setOnComplete(() => ScaleGameObjectBack(hit.transform.gameObject));
                                    }
                                }
                            }

                        }
                        else
                        {
                            PowerUpManager.IsUsingPowerUp = false;
                            PowerUpManager.HasUsedPowerUp = true;
                            PowerUpManager.ObjectToUsePowerUpOn = hit.transform.gameObject;
                            LeanTween.scale(hit.transform.gameObject, new Vector3(hit.transform.localScale.x - 0.2f, hit.transform.localScale.y - 0.2f, 1), 0.1f).setOnComplete(() => ScaleGameObjectBack(hit.transform.gameObject));
                        }
                    }
                    else
                    {
                        if (TutorialSequence.Instacne.duringSequence)
                        {
                            if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isSingleCellPhase
                                 || TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isSingleSlice)
                            {
                                return;
                            }
                        }

                        if (GameManager.Instance.powerupManager.currentlyInUse)
                        {
                            GameManager.Instance.powerupManager.CancelPowerup(GameManager.Instance.powerupManager.currentlyInUse);
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
        for (int i = 0; i < p.transform.childCount; i++)
        {
            if (p.transform.GetChild(i).CompareTag("DestroyVFX"))
            {
                Destroy(p.transform.GetChild(i).gameObject);
            }
        }

        if (!p.isLocked && !p.isTutorialLocked && !p.isStone && !p.isDuringConnectionAnim)
        {
            if (p.partOfBoard)
            {
                p.partOfBoard = false;
            }

            followerTarget = p.transform;
            Cell c = p.transform.parent.GetComponent<Cell>();

            if (c)
            {
                c.RemovePiece(false);
            }

            cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y - 0.05f, -0.05f);

            if (!GameManager.Instance.isSecondaryControls)
            {
                LeanTween.move(followerTarget.gameObject, cursorPos, pickupSpeed); // animate
            }

            if (GameManager.Instance.currentLevel.is12PieceRing)
            {
                LeanTween.scale(followerTarget.gameObject, Vector3.one, pickupSpeed - 0.3f); // animate
            }

            float angle = Mathf.Atan2(gameBoard.transform.position.y - followerTarget.position.y, gameBoard.transform.position.x - followerTarget.position.x) * Mathf.Rad2Deg;

            if (!GameManager.Instance.isSecondaryControls)
            {
                followerTarget.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            }
        }

        if (followerTarget && !GameManager.Instance.isSecondaryControls)
        {
            MoveFollower();
        }
    }
    public void MoveFollower()
    {
        LeanTween.cancel(followerTarget.gameObject);

        //followerTarget.position = new Vector3(cursorPos.position.x, cursorPos.position.y - 0.05f, -0.1f);
        float angle = Mathf.Atan2(gameBoard.transform.position.y - cursorPos.position.y, gameBoard.transform.position.x - cursorPos.position.x) * Mathf.Rad2Deg;
        followerTarget.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

        cursorPos.position = new Vector3(cursorPos.position.x, cursorPos.position.y - 0.05f, -0.05f);
        LeanTween.move(followerTarget.gameObject, cursorPos, moveSpeed); // animate



        RaycastHit hit;

        if (Physics.Raycast(mouseRay, out hit, rayLength, boardCellLayer))
        {
            if (followerTarget)
            {
                Cell c = hit.transform.GetComponent<Cell>();

                if (!c.isFull && !c.isDuringConnectionAnim)
                {
                    if (previousHeighlightChosen != hit.transform.gameObject)
                    {
                        if (previousHeighlightChosen != null)
                        {
                            previousHeighlightChosen.GetComponent<Cell>().TurnOffHighlighParticle();
                            previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
                        }

                        if (TutorialSequence.Instacne.duringSequence)
                        {
                            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Length > 0)
                            {
                                if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Contains(c.cellIndex))
                                {
                                    c.TurnOnHighlightParticle();
                                }
                            }
                        }
                        else
                        {
                            c.TurnOnHighlightParticle();
                        }

                        previousHeighlightChosen = hit.transform.gameObject;

                        c.AddToSubPiecesOnBoardTemp();



                        ConnectionManager.Instance.UpdateReleventSliceConnectAnim(followerTarget.GetComponent<Piece>());
                    }
                }
                else
                {
                    if (previousHeighlightChosen != null)
                    {
                        previousHeighlightChosen.GetComponent<Cell>().TurnOffHighlighParticle();
                        previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
                        ConnectionManager.Instance.NullifyReleventSliceAnim(followerTarget.GetComponent<Piece>());

                        previousHeighlightChosen = null;
                    }
                }
            }

        }
        else
        {
            Debug.Log("AUISBFIASBFAF");
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

                Cell c = closest.GetComponent<Cell>();

                if (!c.isFull && !c.isDuringConnectionAnim)
                {
                    if (previousHeighlightChosen != closest.gameObject)
                    {
                        Debug.Log(closest.gameObject.name);

                        if (previousHeighlightChosen != null)
                        {
                            previousHeighlightChosen.GetComponent<Cell>().TurnOffHighlighParticle();
                            previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
                        }

                        if (TutorialSequence.Instacne.duringSequence)
                        {
                            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Length > 0)
                            {
                                if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Contains(c.cellIndex))
                                {
                                    c.TurnOnHighlightParticle();
                                }
                            }
                        }
                        else
                        {
                            c.TurnOnHighlightParticle();
                        }

                        previousHeighlightChosen = closest.gameObject;

                        c.AddToSubPiecesOnBoardTemp();



                        ConnectionManager.Instance.UpdateReleventSliceConnectAnim(followerTarget.GetComponent<Piece>());
                    }
                }
                else
                {
                    if (previousHeighlightChosen != null)
                    {
                        previousHeighlightChosen.GetComponent<Cell>().TurnOffHighlighParticle();
                        previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
                        ConnectionManager.Instance.NullifyReleventSliceAnim(followerTarget.GetComponent<Piece>());

                        previousHeighlightChosen = null;
                    }
                }
            }
            else
            {
                if (previousHeighlightChosen != null)
                {
                    previousHeighlightChosen.GetComponent<Cell>().TurnOffHighlighParticle();
                    previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
                    ConnectionManager.Instance.NullifyReleventSliceAnim(followerTarget.GetComponent<Piece>());

                    previousHeighlightChosen = null;
                }
            }
        }
    }
    public void SnapFollower(Transform cellHit, Transform toMove)
    {
        if(cellHit == null)
        {
            if (previousHeighlightChosen)
            {
                previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
            }

        }

        if (toMove)
        {
            //toMove.GetComponent<Piece>().isDuringConnectionAnim = false;

            if (cellHit)
            {
                cellHit.GetComponent<Cell>().isDuringConnectionAnim = false;
            }

            LeanTween.cancel(toMove.gameObject);
        }

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
                Cell cell = cellHit.GetComponent<Cell>();
                Cell previousCell = toMove.parent.GetComponent<Cell>(); //// Only relevant if piece is moved from cell to cell

                bool newPiece = toMove.transform.parent.CompareTag("Clip");

                if (GameManager.Instance.currentFilledCellCount + 1 != GameManager.Instance.currentLevel.cellsCountInLevel)
                {
                    if (newPiece && !cell.isFull)
                    {
                        GameManager.Instance.clipManager.PopulateSlot(toMove.transform.parent, 10);

                        if (GameManager.Instance.currentLevel.symbolsNeededForSpecialPowers.Length > 0)
                        {
                            AddNumAnimalsToBoard(toMove);
                        }
                    }
                }
                else
                {
                    GameManager.Instance.clipManager.emptyClip = toMove.transform.parent;
                    GameManager.Instance.clipManager.latestPiece = toMove;
                    //AddNumAnimalsToBoard(followerTarget);
                }


                if (!cell.isFull)
                {
                    cell.AddPiece(toMove, newPiece);

                    if (!newPiece && cell != previousCell)
                    {
                        previousCell.isFull = false;
                    }
                }
                else
                {
                    ReturnHome();
                }

                if(followerTarget == toMove)
                {
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
                if (followerTarget == toMove)
                {
                    followerTarget = null;
                }

                cellhitSecondaryControls = null;
            }
        }
    }
    private void SnapFollowerTutorial(Transform cellHit)
    {
        if (followerTarget)
        {
            LeanTween.cancel(followerTarget.gameObject);
        }

        if (cellHit != null)
        {
            if (followerTarget)
            {
                if (GameManager.Instance.currentLevel.isSpecificTutorial)
                {
                    if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetCells.Length > 0)
                    {
                        if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].targetCells.Contains(cellHit.GetComponent<Cell>().cellIndex))
                        {
                            Cell cell = cellHit.GetComponent<Cell>();

                            Transform clipParent = followerTarget.parent; //// ONLY IF WERE COMING FROM A CLIP THIS IS RELEVANT
                            Cell previousCell = followerTarget.parent.GetComponent<Cell>(); //// Only relevant if piece is moved from cell to cell

                            bool isFromClip = followerTarget.transform.parent.CompareTag("Clip");

                            Vector3 originalScale = followerTarget.transform.localScale;

                            if (!cell.isFull && cell != previousCell)
                            {
                                cell.AddPiece(followerTarget, isFromClip);

                                if (GameManager.Instance.currentFilledCellCount + 1 != GameManager.Instance.currentLevel.cellsCountInLevel && !tutorialBadConnection)
                                {

                                    if (isFromClip/* && !cell.isFull*/)
                                    {
                                        GameManager.Instance.clipManager.PopulateSlot(clipParent, 10);
                                    }

                                    TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                                }
                                else
                                {
                                    cell.RemovePiece(false);

                                    if (isFromClip)
                                    {
                                        GameManager.Instance.currentFilledCellCount--;
                                    }

                                    followerTarget.transform.SetParent(clipParent.transform);
                                    followerTarget.transform.localScale = originalScale;

                                    GameManager.Instance.clipManager.emptyClip = followerTarget.transform.parent;
                                    GameManager.Instance.clipManager.latestPiece = followerTarget;
                                    ReturnHome();
                                }

                                if (!isFromClip && cell != previousCell)
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

                if (GameManager.Instance.currentLevel.isTutorial)
                {
                    if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].targetCells.Contains(cellHit.GetComponent<Cell>().cellIndex))
                    {
                        Cell cell = cellHit.GetComponent<Cell>();

                        Transform clipParent = followerTarget.parent; //// ONLY IF WERE COMING FROM A CLIP THIS IS RELEVANT
                        Cell previousCell = followerTarget.parent.GetComponent<Cell>(); //// Only relevant if piece is moved from cell to cell

                        bool isFromClip = followerTarget.transform.parent.CompareTag("Clip");

                        Vector3 originalScale = followerTarget.transform.localScale;

                        if (!cell.isFull && cell != previousCell)
                        {
                            cell.AddPiece(followerTarget, isFromClip);

                            if (GameManager.Instance.currentFilledCellCount + 1 != GameManager.Instance.currentLevel.cellsCountInLevel && !tutorialBadConnection)
                            {

                                if (isFromClip/* && !cell.isFull*/)
                                {
                                    GameManager.Instance.clipManager.PopulateSlot(clipParent, 10);

                                    if (GameManager.Instance.currentLevel.symbolsNeededForSpecialPowers.Length > 0)
                                    {
                                        AddNumAnimalsToBoard(followerTarget);
                                    }
                                }

                                StartCoroutine(TutorialSequence.Instacne.IncrementCurrentPhaseInSequence());
                            }
                            else
                            {
                                cell.RemovePiece(false);

                                if (isFromClip)
                                {
                                    GameManager.Instance.currentFilledCellCount--;
                                }

                                followerTarget.transform.SetParent(clipParent.transform);
                                followerTarget.transform.localScale = originalScale;

                                GameManager.Instance.clipManager.emptyClip = followerTarget.transform.parent;
                                GameManager.Instance.clipManager.latestPiece = followerTarget;
                                ReturnHome();
                            }

                            if (!isFromClip && cell != previousCell)
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
        followerTarget.localScale = new Vector3(1.45f, 1.45f, 1);

        if (followerTarget.transform.parent.GetComponent<Cell>())
        {
            followerTarget.transform.parent.GetComponent<Cell>().AddPiece(followerTarget, false);
        }
        //else
        //{
        //    previousHeighlightChosen.GetComponent<Cell>().RemoveToSubPiecesOnBoardTemp();
        //}

        followerTarget = null;
    }
    //private bool SpecialTutorialConnectionLogic(int cellindex, Piece pieceHeld)
    //{
    //    Slice relavent = GameManager.Instance.sliceManager.sliceSlots[cellindex].GetComponent<Slice>();

    //    if (relavent.sliceCatagory == SliceCatagory.None)
    //    {
    //        return true;
    //        //if (pieceHeld.rightChild.colorOfPiece != PieceColor.Blue)
    //        //{
    //        //    ReturnHome();
    //        //    return false;
    //        //}
    //    }
    //    else
    //    {
    //        if (pieceHeld.leftChild.colorOfPiece != PieceColor.Blue)
    //        {
    //            ReturnHome();
    //            return false;
    //        }
    //    }


    //    return true;
    //}
    void AddNumAnimalsToBoard(Transform piece)
    {
        Piece p = piece.GetComponent<Piece>();

        PowerUpManager PUM = GameManager.Instance.powerupManager;

        PieceSymbol symbolNeeded = PUM.specialPowerupsInGame[0].SymbolNeeded;

        SubPiece right = p.rightChild;
        SubPiece left = p.leftChild;

        if (symbolNeeded == right.symbolOfPiece && symbolNeeded == left.symbolOfPiece)
        {
            GameObject side1 = Instantiate(PUM.specialPowerVFXPrefab, right.transform);
            GameObject side2 = Instantiate(PUM.specialPowerVFXPrefab, left.transform);

            PUM.MoveSpecialPowerVFXToTarget(side1, symbolNeeded);
            PUM.MoveSpecialPowerVFXToTarget(side2, symbolNeeded);
            return;
        }

        if (symbolNeeded == right.symbolOfPiece || symbolNeeded == left.symbolOfPiece)
        {
            if (symbolNeeded == right.symbolOfPiece)
            {
                GameObject side1 = Instantiate(PUM.specialPowerVFXPrefab, right.transform);
                PUM.MoveSpecialPowerVFXToTarget(side1, symbolNeeded);
            }
            else
            {
                GameObject side2 = Instantiate(PUM.specialPowerVFXPrefab, left.transform);
                PUM.MoveSpecialPowerVFXToTarget(side2, symbolNeeded);
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

    void ScaleGameObjectBack(GameObject toScale)
    {
        LeanTween.scale(toScale, new Vector3(toScale.transform.localScale.x + 0.2f, toScale.transform.localScale.y + 0.2f, 1), 0.1f);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
