using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    //public float waitBetweenPieceMove;
    public float speedPieceMove;
    public float speedOutTopBottom;

    public float waitBetweenPiecePullIn;
    public float speedPiecePullIn;
    public float dissolveSpeedMaskCrack = 5;
    public float fillGlowSpeed = 5;
    public float finalGlowSpeed = 5;

    //public float paceFade;

    //public float waitTimeMoveTopBottomTime;
    public float waitTimeParticlesStart;
    public float waitTimeMidParticleAppear;
    public float waitTimeDissolveTiles;
    //public float waitTimePullIn;
    public float waitTimeBeforeFillGlow;
    public float waitTimeBeforeFinalGlow;
    public float waitTimeFadeIn;
    public float waitTimeFadeOut;
    public float waitTimeWinScreen;
    public float completeDissolveAnimTime;

    public float fadingSpeedGameplay;

    public float animalWaitTime;

    public float waitTimeVFXShakeStatue;

    //public Image fadeImageEndLevel;

    public bool noWaitPieces;
    public bool noWaitParticles;
    public bool noWaitDissolve;
    public bool noWaitPullIn;
    public bool noWaitAnimal;

    public bool hasSkippedToAnimalAnim;
    public bool hasSkippedToAfterAnimalAnim;
    public bool hasSkippedToBoardAnim;

    //public ParticleSystem midPieceParticle;
    public List<SubPiece> tempSubPieceArray;

    public timeWaitPull minMaxWaitPullInPieces;

    //private GameObject boardScreenshot;


    [Header("After Animal Animation")]
    //public float waitTimeBoardFadeIn;
    public float fadeInTimeBoard;
    public float waitTimeInPieces;
    public float offsetWaitTimeInPieces;
    public float delayBetweenPiecesAppear;
    public float fadeInTimeTextSuccess;
    public float fadeInTimeTextAnimalName;
    public float fadeInTimeButtons;
    public float timeToScaleBoard;
    public float waitTimeButtonsAppear;
    public float DelayBetweenLootAppear;
    public float timeToScaleLoot;
    public float timeToScaleUIFlowers;
    public float timeToScaleBoardAppearGrow;
    public float timeToScaleBoardAppearBackOriginal;

    public Vector3 boardScaleTo;
    public Vector3 boardScaleToAppear;
    public Vector3 lootScaleTo;
    public Vector3 flowerUIScaleTo;



    public GameObject successVFX;

    [Header("Unlcok Zone Settings")]
    public float cameraMoveTime;
    public float fadingTime;

    public Image bgFadeImage;
    public ParticleSystem particleFade;
    public TMP_Text zoneToUnlcokHeaderText;
    public TMP_Text zoneToUnlcokNameText;
    public TMP_Text tapToContinueText;

    [Header("Brew Potion Settings")]
    public float brewCameraMoveTime;
    public float brewfadingTime;

    public Image brewbgFadeImage;
    public SpriteRenderer potionIcon;
    public ParticleSystem brewparticleFade;
    public TMP_Text brewHeaderText;
    public TMP_Text brewNameText;
    public TMP_Text brewtapToContinueText;

    [Header("Crafted Hollow Item Settings")]
    public float craftedCameraMoveTime;
    public float craftedfadingTime;

    public Image craftedbgFadeImage;
    public SpriteRenderer hollowIcon;
    public ParticleSystem craftedparticleFade;
    public TMP_Text craftedHeaderText;
    public TMP_Text craftedNameText;
    public TMP_Text craftedtapToContinueText;

    [Header("Enter Corrupt Zone")]
    [Space(30)]
    public float zoomSpeed = 0;
    public float transitionTime;

    public float cameraOrthoSizeTarget;
    public Image fadeIntoLevel;

    private Coroutine endAnimToAnimal = null;

    //[HideInInspector]
    //public Coroutine endAnimToWinScreen = null;

    bool dissolveStart = false;

    [Header("Place hollow item den")]
    [Space(30)]

    public bool isPlacingDenItem;

    [Header("Enter Level animation")]
    public float startAnimDelayTime;
    public SpriteRenderer ring;
    public SpriteRenderer colorMask;
    float originalColorMaskAlpha = 0;
    public float timeToFadeRingAndColormask;
    public Transform[] clips;
    public float timeToFadeClip;
    public List<SpriteRenderer> locks;
    public float timeToFadeLocks;
    public List<Piece> corruptedTiles;
    public Vector3 scaleToSize;
    public float MoveLocalYTimeUp;
    public float MoveLocalYTimeDown;
    public float MoveLocalToY;
    public float timeToScaleCorruptedTilesUp;
    public float timeToScaleCorruptedTilesDown;
    //public float delayCorruptedTileStart;
    public List<Slice> slices;
    public float timeToScaleSlices;

    [Header("Other")]
    [Space(30)]

    public GameObject[] turnOff;
    public GameObject[] destroyOnSkipEndLevel;

    public GameObject toMoveOpenZone;


    public bool endLevelAnimationON;
    void Start()
    {
        instance = this;
        tempSubPieceArray = new List<SubPiece>();
        //fadeImageEndLevel.gameObject.SetActive(false);
        endLevelAnimationON = false;

        destroyOnSkipEndLevel = null;
        turnOff = null;
    }

    [System.Serializable]
    public struct timeWaitPull
    {
        public float a, b;
    }
    public void StartEndLevelAnimSequence(bool cheat)
    {
        SoundManager.Instance.CallFadeOutAmbientMusicLevel(SoundManager.Instance.timeFadeOutAmbienceLevel, false);

        if (cheat)
        {
            StartCoroutine(SkipEndLevelAnimation());
        }
        else
        {
            endAnimToAnimal = StartCoroutine(StartEndLevelAnim());
        }
    }
    public void StartZoomIntoCorruptArea(int ID)
    {
        ZoomIntoCorruptArea(ID);
    }

    public IEnumerator StartEndLevelAnim()
    {
        endLevelAnimationON = true;
        turnOff = GameObject.FindGameObjectsWithTag("Off on end level");


        UIManager.Instance.restartButton.interactable = false;
        UIManager.Instance.dealButton.interactable = false;

        tempSubPieceArray.AddRange(ConnectionManager.Instance.subPiecesOnBoard);

        foreach (InGameSpecialPowerUp IGSP in GameManager.Instance.powerupManager.specialPowerupsInGame)
        {
            IGSP.gameObject.SetActive(false);
        }



        MoveTopButtonAnim();

        if(GameManager.Instance.currentLevel.isAnimalLevel)
        {
            SoundManager.Instance.PlaySound(Sounds.RiveRootRelease);
            AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Release Animal");
        }

        yield return new WaitForSeconds(speedOutTopBottom + 0.1f);

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(true);



        ConnectionManager.Instance.TurnOffAllConnectedVFX();


        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            Instantiate(GameManager.Instance.endLevelAnimStatueShakeVFXPrefab, GameManager.Instance.destroyOutOfLevel);
            yield return new WaitForSeconds(waitTimeVFXShakeStatue);
        }

        GameManager.Instance.sliceManager.endLevelAnimVFX.SetActive(true);
        //// Dissolve Tiles Here
        StartCoroutine(DissolveTiles());



        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(false);
        }

        GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>().enabled = false;



        if (!noWaitDissolve)
        {
            yield return new WaitForSeconds(waitTimeDissolveTiles);
        }


        yield return new WaitUntil(() => dissolveStart == true);

        ZoneManagerHelpData.Instance.ChangeZoneToNormalZoneDisplay();

        SpriteRenderer boardSR = GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>();
        boardSR.color = new Color(boardSR.color.r, boardSR.color.g, boardSR.color.b, 0);

        foreach (Slice slice in GameManager.Instance.gameBoard.GetComponent<SliceManager>().fullSlices)
        {
            if (slice.child)
            {
                SpriteRenderer sliceSR = slice.child.GetComponent<SpriteRenderer>();
                sliceSR.color = new Color(sliceSR.color.r, sliceSR.color.g, sliceSR.color.b, 0);
            }
        }

        foreach (GameObject go in GameManager.Instance.gameBoard.GetComponent<SliceManager>().activeLocksLockAnims)
        {
            SpriteRenderer goSR = go.GetComponent<SpriteRenderer>();
            goSR.color = new Color(goSR.color.r, goSR.color.g, goSR.color.b, 0);
        }

        UnDissolveTiles(false);

        foreach (Cell cell in ConnectionManager.Instance.cells)
        {
            cell.pieceHeld.gameObject.SetActive(false);
        }


        Destroy(GameManager.Instance.gameClip.gameObject);

        UIManager.Instance.TurnOffGameplayUI();
        UIManager.Instance.InGameUiScreens.SetActive(true);
        GameManager.Instance.selectedLevelBG.transform.Find("RingMask").gameObject.SetActive(false);

        if (!noWaitAnimal)
        {
            yield return new WaitForSeconds(animalWaitTime);
        }

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            if (GameManager.Instance.currentLevel.isAnimalLevel)
            {
                SoundsPerAnimal SPA = AnimalManagerDataHelper.instance.soundsPerAnimalEnum.Where(p => p.animalEnum == AnimalsManager.Instance.currentLevelAnimal).SingleOrDefault();

                if(SPA != null)
                {
                    SoundManager.Instance.PlaySound(SPA.soundClipToPlay);
                }

                AnimalsManager.Instance.CheckUnlockAnimal(AnimalsManager.Instance.currentLevelAnimal);

            }
            else
            {
                AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Clear Rive");
                SoundManager.Instance.PlaySound(Sounds.RiveRelease);
            }

            hasSkippedToAnimalAnim = true;
        }

        UIManager.Instance.restartButton.interactable = true;
        UIManager.Instance.dealButton.interactable = true;

        if (GameManager.Instance.currentLevel.isGrindLevel)
        {
            StartCoroutine(AfterAnimalAnimation());
        }
    }

    private void MoveTopButtonAnim()
    {
        GameObject clip = GameManager.Instance.gameClip;
        GameObject topZone = UIManager.Instance.gameplayCanvasTop;
        GameObject bottomeZone = UIManager.Instance.gameplayCanvasBotom;

        LeanTween.move(clip, new Vector3(clip.transform.position.x, clip.transform.position.y + 2.2f, clip.transform.position.z), speedOutTopBottom).setEase(LeanTweenType.easeInOutQuad); // animate

        LeanTween.move(topZone, new Vector3(topZone.transform.position.x, topZone.transform.position.y + 1f, topZone.transform.position.z), speedOutTopBottom).setEase(LeanTweenType.easeInOutQuad); // animate

        LeanTween.move(bottomeZone, new Vector3(bottomeZone.transform.position.x, bottomeZone.transform.position.y - 1f, bottomeZone.transform.position.z), speedOutTopBottom).setEase(LeanTweenType.easeInOutQuad); // animate
    }

    private void SkipMoveTopBottom()
    {
        GameObject clip = GameManager.Instance.gameClip;
        GameObject topZone = UIManager.Instance.gameplayCanvasTop;
        GameObject bottomeZone = UIManager.Instance.gameplayCanvasBotom;

        if (clip)
        {
            clip.transform.position = new Vector3(clip.transform.position.x, clip.transform.position.y + 2.2f, clip.transform.position.z);
        }

        if (topZone)
        {
            topZone.transform.position = new Vector3(topZone.transform.position.x, topZone.transform.position.y + 1f, topZone.transform.position.z);
        }

        if (bottomeZone)
        {
            bottomeZone.transform.position = new Vector3(bottomeZone.transform.position.x, bottomeZone.transform.position.y - 1f, bottomeZone.transform.position.z);
        }
    }

    public void MoveSubPiece(SubPiece toMove)
    {
        if(toMove.subPieceIndex % 2 != 0)
        {
            LeanTween.move(toMove.gameObject, new Vector3(toMove.transform.localPosition.x + 0.4f, toMove.transform.localPosition.y + 0.8f, toMove.transform.localPosition.z), speedPieceMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate
        }
        else
        {
            LeanTween.move(toMove.gameObject, new Vector3(toMove.transform.localPosition.x, toMove.transform.localPosition.y + 0.85f, toMove.transform.localPosition.z), speedPieceMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate
        }
    }

    public void MoveBoardScreenshotToPosition(GameObject board)
    {
        board.transform.position = new Vector3(-0.002f, 1.636f, 0.055f);

        board.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);

    }

    //public void ActivateParticleEffectsMiddle(GameObject midPiece)
    //{
    //    Instantiate(midPieceParticle, midPiece.transform);
    //}

    public void PullIn(SubPiece toMove)
    {
        LeanTween.move(toMove.gameObject, GameManager.Instance.gameBoard.transform.position, speedPieceMove).setEase(LeanTweenType.easeInOutQuad); // animate
    }

    [ContextMenu("Disslove Tiles")]
    public IEnumerator DissolveTiles()
    {
        dissolveStart = false;

        foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
        {
            StartCoroutine(SP.DissolveSubPiece(dissolveSpeedMaskCrack, waitTimeBeforeFillGlow,fillGlowSpeed, waitTimeBeforeFinalGlow, finalGlowSpeed));
        }

        yield return new WaitForSeconds(completeDissolveAnimTime);
        dissolveStart = true;
    }

    public void UnDissolveTiles(bool isImmidiate)
    {
        if (isImmidiate)
        {
            foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
            {
                //Material mat = SP.GetComponent<Renderer>().material;
                //mat.SetFloat("Dissolve_Amount", 0.24f);
                if (SP)
                {
                    SP.UnDissolveSubPiece();
                }

            }
        }
        else
        {
            foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
            {
                if (SP)
                {
                    SP.UnDissolveSubPiece();
                }
            }
        }
    }

    public void CallSkipEndLevelAnim()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        StartCoroutine(SkipEndLevelAnimation());
    }

    public IEnumerator SkipEndLevelAnimation()
    {
        endLevelAnimationON = true;
        //StopCoroutine(endAnimToAnimal);
        StopAllCoroutines();
        //LeanTween.cancelAll();

        if (hasSkippedToAnimalAnim)
        {
            //hasSkippedToAnimalAnim = false;

            StartCoroutine(AfterAnimalAnimation());

            StopCoroutine("SkipEndLevelAnimation");

            //Debug.LogError("HERE");
            //endAnimToWinScreen = StartCoroutine(AfterAnimalAnimation(true));
            //if (AnimalsManager.Instance.currentLevelLiveAnimal)
            //{
            //    Destroy(AnimalsManager.Instance.currentLevelLiveAnimal.gameObject);
            //}

            yield break;
        }
        else
        {
            hasSkippedToAnimalAnim = true;
            UIManager.Instance.skipAnimationButton.gameObject.SetActive(true);
            //endAnimToWinScreen = StartCoroutine(AfterAnimalAnimation(false));
        }

        SoundManager.Instance.audioSourceSFX.Stop();

        //if (endAnimToAnimal != null)
        //{
        //    StopCoroutine(endAnimToAnimal);
        //    LeanTween.cancelAll();
        //}

        SkipMoveTopBottom();

        if (destroyOnSkipEndLevel == null)
        {
            destroyOnSkipEndLevel = GameObject.FindGameObjectsWithTag("DestroyOnSkipEndLevel");
        }

        foreach (GameObject go in destroyOnSkipEndLevel)
        {
            Destroy(go);
        }

        destroyOnSkipEndLevel = null;

        foreach (InGameSpecialPowerUp IGSP in GameManager.Instance.powerupManager.specialPowerupsInGame)
        {
            IGSP.gameObject.SetActive(false);
        }

        ZoneManagerHelpData.Instance.ChangeZoneToNormalZoneDisplay();

        SpriteRenderer boardSR = GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>();
        boardSR.color = new Color(boardSR.color.r, boardSR.color.g, boardSR.color.b, 0);

        foreach (Slice slice in GameManager.Instance.gameBoard.GetComponent<SliceManager>().fullSlices)
        {
            if (slice.child)
            {
                SpriteRenderer sliceSR = slice.child.GetComponent<SpriteRenderer>();
                sliceSR.color = new Color(sliceSR.color.r, sliceSR.color.g, sliceSR.color.b, 0);
            }
        }

        foreach (GameObject go in GameManager.Instance.gameBoard.GetComponent<SliceManager>().activeLocksLockAnims)
        {
            SpriteRenderer goSR = go.GetComponent<SpriteRenderer>();
            goSR.color = new Color(goSR.color.r, goSR.color.g, goSR.color.b, 0);
        }

        UnDissolveTiles(false);

        foreach (Cell cell in ConnectionManager.Instance.cells)
        {
            if (cell.pieceHeld)
            {
                cell.pieceHeld.gameObject.SetActive(false);
            }
        }

        ConnectionManager.Instance.TurnOffAllConnectedVFX();


        GameManager.Instance.sliceManager.endLevelAnimVFX.SetActive(false);
        //foreach (GameObject GO in turnOff)
        //{
        //    GO.SetActive(true);
        //}

        //turnOff = null;

        GameManager.Instance.selectedLevelBG.transform.Find("RingMask").gameObject.SetActive(false);

        //fadeImageEndLevel.gameObject.SetActive(false);


        if (GameManager.Instance.gameBoard.gameObject)
        {
            //Destroy(GameManager.Instance.gameBoard.gameObject);
            Destroy(GameManager.Instance.gameClip.gameObject);
        }

        //UIManager.Instance.restartButton.interactable = false;

        UIManager.Instance.TurnOffGameplayUI();

        UIManager.Instance.InGameUiScreens.SetActive(true);

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            if (GameManager.Instance.currentLevel.isAnimalLevel)
            {
                SoundsPerAnimal SPA = AnimalManagerDataHelper.instance.soundsPerAnimalEnum.Where(p => p.animalEnum == AnimalsManager.Instance.currentLevelAnimal).SingleOrDefault();

                if (SPA != null)
                {
                    SoundManager.Instance.PlaySound(SPA.soundClipToPlay);
                }

                AnimalsManager.Instance.CheckUnlockAnimal(AnimalsManager.Instance.currentLevelAnimal);
                AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Release Animal");

            }
            else
            {
                AnimalsManager.Instance.statueToSwap.GetComponent<Animator>().SetTrigger("Clear Rive");
                SoundManager.Instance.PlaySound(Sounds.RiveRelease);
            }
        }
        else
        {
           StartCoroutine(AfterAnimalAnimation());
        }

        //TutorialSequence.Instacne.CheckDoPotionTutorial();

        //hasSkippedToAnimalAnim = false;

        Debug.LogError("Reached end of skip");
        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneX, SystemsToSave.ZoneManager, SystemsToSave.Player, SystemsToSave.animalManager });
    }

    public void CallAfterAnimalAnimation()
    {
        StartCoroutine(AfterAnimalAnimation());

    }
    public IEnumerator AfterAnimalAnimation()
    {
        endLevelAnimationON = true;

        if (hasSkippedToAfterAnimalAnim)
        {

            StopCoroutine("AfterAnimalAnimation");

            SkipBoardAnim();

            yield break;
        }
        else
        {
            hasSkippedToAfterAnimalAnim = true;
        }

        SoundManager.Instance.PlaySound(Sounds.LevelWin);


        if (turnOff != null)
        {
            foreach (GameObject GO in turnOff)
            {
                if (GO)
                {
                    GO.SetActive(true);
                }
            }
        }

        Instantiate(successVFX, GameManager.Instance.destroyOutOfLevel);

        if (AnimalsManager.Instance.currentLevelLiveAnimal)
        {
            Destroy(AnimalsManager.Instance.currentLevelLiveAnimal.gameObject);
            AnimalsManager.Instance.currentLevelLiveAnimal.SetActive(false);
        }
        ///summon VFX HERE
        ///
        GameManager.Instance.gameBoard.transform.position = new Vector3(0, 1.55f, 0);
        GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>().enabled = true;

        turnOff = null;

        SpriteRenderer boardSR = GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>();
        boardSR.color = new Color(boardSR.color.r, boardSR.color.g, boardSR.color.b, 1);

        foreach (Slice slice in GameManager.Instance.gameBoard.GetComponent<SliceManager>().fullSlices)
        {
            if (slice.child)
            {
                SpriteRenderer sliceSR = slice.child.GetComponent<SpriteRenderer>();
                sliceSR.color = new Color(sliceSR.color.r, sliceSR.color.g, sliceSR.color.b, 1);
            }
        }

        foreach (GameObject go in GameManager.Instance.gameBoard.GetComponent<SliceManager>().activeLocksLockAnims)
        {
            SpriteRenderer goSR = go.GetComponent<SpriteRenderer>();
            goSR.color = new Color(goSR.color.r, goSR.color.g, goSR.color.b, 1);
        }

        GameManager.Instance.gameBoard.transform.localScale = boardScaleTo;

        Vector3 originalScale = GameManager.Instance.gameBoard.transform.localScale;

        LeanTween.scale(GameManager.Instance.gameBoard, boardScaleToAppear, timeToScaleBoardAppearGrow);

        yield return new WaitForSeconds(timeToScaleBoardAppearGrow);

        LeanTween.scale(GameManager.Instance.gameBoard, originalScale, timeToScaleBoardAppearBackOriginal);

        yield return new WaitForSeconds(waitTimeInPieces + 0.5f);


        foreach (Cell cell in ConnectionManager.Instance.cells)
        {
            if (cell.pieceHeld)
            {
                cell.pieceHeld.gameObject.SetActive(true);
                yield return new WaitForSeconds(delayBetweenPiecesAppear);
            }
        }

        UIManager.Instance.youWinScreen.SetActive(true);

        LeanTween.value(UIManager.Instance.sucessText.gameObject, 0f, 1, fadeInTimeTextSuccess).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
        {
            TMP_Text succesText = UIManager.Instance.sucessText;
            Color newColor = succesText.color;
            newColor.a = val;
            succesText.color = newColor;
        });

        LeanTween.scale(UIManager.Instance.flowerUIMask, flowerUIScaleTo, timeToScaleUIFlowers);

        yield return new WaitForSeconds(fadeInTimeTextSuccess + 0.1f);

        string animalName = Regex.Replace(AnimalsManager.Instance.currentLevelAnimal.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            if (GameManager.Instance.currentLevel.isAnimalLevel)
            {
                UIManager.Instance.animalNameText.text = animalName + " Released!";
            }
            else
            {
                UIManager.Instance.animalNameText.text = "Corruption cleansed!";
            }
        }
        else
        {
            UIManager.Instance.animalNameText.text = "Arena completed!";
        }

        LeanTween.value(UIManager.Instance.animalNameText.gameObject, 0f, 1, fadeInTimeTextAnimalName).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
        {
            TMP_Text succesText = UIManager.Instance.animalNameText;
            Color newColor = succesText.color;
            newColor.a = val;
            succesText.color = newColor;
        });


        LeanTween.scale(GameManager.Instance.gameBoard.gameObject, boardScaleTo, timeToScaleBoard);

        yield return new WaitForSeconds(timeToScaleBoard + 0.1f);
        GameManager.Instance.WinAfterAnimation();

        yield return new WaitForSeconds(waitTimeButtonsAppear);
        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            LeanTween.value(UIManager.Instance.backToHubButton.gameObject, 0f, 1, fadeInTimeButtons).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
            {
                Image image = UIManager.Instance.backToHubButton.GetComponent<Image>();
                Color newColor = image.color;
                newColor.a = val;
                image.color = newColor;
            });
        }
        else
        {
            LeanTween.value(UIManager.Instance.backToHubButton.gameObject, 0f, 1, fadeInTimeButtons).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
            {
                Image image = UIManager.Instance.backToHubButton.GetComponent<Image>();
                Color newColor = image.color;
                newColor.a = val;
                image.color = newColor;
            });

            LeanTween.value(UIManager.Instance.restartGrindLevel.gameObject, 0f, 1, fadeInTimeButtons).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
            {
                CanvasGroup image = UIManager.Instance.restartGrindLevel.GetComponent<CanvasGroup>();
                float num = image.alpha;
                num = val;
                image.alpha = num;
            });
        }

        if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
        {
            if (GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
            {
                LeanTween.value(UIManager.Instance.nextLevelFromWinScreen.gameObject, 0f, 1, fadeInTimeButtons).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
                {
                    Image image = UIManager.Instance.nextLevelFromWinScreen.GetComponent<Image>(); ;
                    Color newColor = image.color;
                    newColor.a = val;
                    image.color = newColor;
                });
            }
        }
        else
        {
            LeanTween.value(UIManager.Instance.nextLevelFromWinScreen.gameObject, 0f, 1, fadeInTimeButtons).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
            {
                Image image = UIManager.Instance.nextLevelFromWinScreen.GetComponent<Image>(); ;
                Color newColor = image.color;
                newColor.a = val;
                image.color = newColor;
            });
        }

        yield return new WaitForSeconds(fadeInTimeButtons + 0.1f);
        ConnectionManager.Instance.TurnOffAllConnectedVFX();


        UIManager.Instance.restartButton.interactable = true;
        UIManager.Instance.dealButton.interactable = true;

        TutorialSequence.Instacne.CheckDoPotionTutorial();
        TutorialSequence.Instacne.CheckDoAnimalAlbumTutorial();
        //TutorialSequence.Instacne.CheckDoDenTutorial(); //temp keep here

        StartCoroutine(CheckShowLootTutorial());

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            UIManager.Instance.CheckTurnOnReleaseAnimalScreen();
        }



        ///check bar progress here for test levels
        /// this might change places depending on where we want it in animation
        if(GameManager.Instance.currentLevel.isTestLevel)
        {
            Debug.LogError("end of end level sequence! adding to counter");

            // progress bar and check to see if reched max.. if we did - what to do with loot?
        }


        Debug.LogError("Reached end of end level sequence");
        endLevelAnimationON = false;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
    }

    public void SkipBoardAnim()
    {
        endLevelAnimationON = true;
        hasSkippedToBoardAnim = true;

        GameManager.Instance.gameBoard.transform.position = new Vector3(0, 1.55f, 0);
        GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>().enabled = true;

        SpriteRenderer boardSR = GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>();
        boardSR.color = new Color(boardSR.color.r, boardSR.color.g, boardSR.color.b, 1);

        foreach (Slice slice in GameManager.Instance.gameBoard.GetComponent<SliceManager>().fullSlices)
        {
            if (slice.child)
            {
                SpriteRenderer slicedSR = slice.child.GetComponent<SpriteRenderer>();
                slicedSR.color = new Color(slicedSR.color.r, slicedSR.color.g, slicedSR.color.b, 1);
            }
        }

        foreach (GameObject go in GameManager.Instance.gameBoard.GetComponent<SliceManager>().activeLocksLockAnims)
        {
            SpriteRenderer goSR = go.GetComponent<SpriteRenderer>();
            goSR.color = new Color(goSR.color.r, goSR.color.g, goSR.color.b, 1);
        }


        foreach (Cell cell in ConnectionManager.Instance.cells)
        {
            if (cell.pieceHeld)
            {
                cell.pieceHeld.gameObject.SetActive(true);
            }
        }


        UIManager.Instance.youWinScreen.SetActive(true);


        UIManager.Instance.sucessText.color = new Color(UIManager.Instance.sucessText.color.r, UIManager.Instance.sucessText.color.g, UIManager.Instance.sucessText.color.b, 1);

        string animalName = Regex.Replace(AnimalsManager.Instance.currentLevelAnimal.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        LeanTween.scale(UIManager.Instance.flowerUIMask, flowerUIScaleTo, timeToScaleUIFlowers);

        if (GameManager.Instance.currentLevel.isGrindLevel)
        {
            UIManager.Instance.animalNameText.text = "Arena completed!";
        }
        else
        {
            if (GameManager.Instance.currentLevel.isAnimalLevel)
            {
                UIManager.Instance.animalNameText.text = animalName + " Released!";
            }
            else
            {
                UIManager.Instance.animalNameText.text = "Corruption cleansed!";
            }
        }

        UIManager.Instance.animalNameText.color = new Color(UIManager.Instance.animalNameText.color.r, UIManager.Instance.animalNameText.color.g, UIManager.Instance.animalNameText.color.b, 1);

        GameManager.Instance.gameBoard.transform.localScale = boardScaleTo;

        GameManager.Instance.WinAfterAnimation();

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            Image backToHubImage = UIManager.Instance.backToHubButton.GetComponent<Image>();

            backToHubImage.color = new Color(backToHubImage.color.r, backToHubImage.color.g, backToHubImage.color.b, 1);
        }
        else
        {
            Image backToHubImage = UIManager.Instance.backToHubButton.GetComponent<Image>();

            backToHubImage.color = new Color(backToHubImage.color.r, backToHubImage.color.g, backToHubImage.color.b, 1);

            CanvasGroup restartGrind = UIManager.Instance.restartGrindLevel.GetComponent<CanvasGroup>();

            restartGrind.alpha = 1;
        }

        if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
        {
            if (GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.PotionCraft && GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.DenScreen && GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.AnimalAlbum)
            {
                Image nextLevelButtonImage = UIManager.Instance.nextLevelFromWinScreen.GetComponent<Image>();
                nextLevelButtonImage.color = new Color(nextLevelButtonImage.color.r, nextLevelButtonImage.color.g, nextLevelButtonImage.color.b, 1);
            }
        }
        else
        {
            Image nextLevelButtonImage = UIManager.Instance.nextLevelFromWinScreen.GetComponent<Image>();
            nextLevelButtonImage.color = new Color(nextLevelButtonImage.color.r, nextLevelButtonImage.color.g, nextLevelButtonImage.color.b, 1);
        }

        ConnectionManager.Instance.TurnOffAllConnectedVFX();


        if (AnimalsManager.Instance.currentLevelLiveAnimal)
        {
            Destroy(AnimalsManager.Instance.currentLevelLiveAnimal.gameObject);
        }


        UIManager.Instance.restartButton.interactable = true;
        UIManager.Instance.dealButton.interactable = true;


        TutorialSequence.Instacne.CheckDoPotionTutorial();
        TutorialSequence.Instacne.CheckDoAnimalAlbumTutorial();
        //TutorialSequence.Instacne.CheckDoDenTutorial(); // temp keep here
        StartCoroutine(CheckShowLootTutorial());

        if (!GameManager.Instance.currentLevel.isGrindLevel)
        {
            UIManager.Instance.CheckTurnOnReleaseAnimalScreen();
        }

        ///check bar progress here for test levels
        /// this might change places depending on where we want it in animation
        if(GameManager.Instance.currentLevel.isTestLevel)
        {
            Debug.LogError("Skipped now! adding to counter");
            // progress bar and check to see if reched max.. if we did - what to do with loot?
        }

        Debug.LogError("Reached end of sequence - skipped board");

        endLevelAnimationON = false;


        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
    }
    private IEnumerator CheckShowLootTutorial()
    {
        yield return new WaitForSeconds(0.1f);
        if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.lootTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard)) /// specificTutorialIndex == 0  is loot tutorial
        {
            LootManager.Instance.rubiesToRecieveInLevel = 8;
            TutorialSequence.Instacne.maskImage.gameObject.SetActive(true);
            StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
            TutorialSequence.Instacne.currentSpecificTutorial = SpecificTutorialsEnum.lootTutorial;
        }
    }
    public void ZoomIntoCorruptArea(int ID)
    {
        LeanTween.value(Camera.main.orthographicSize, cameraOrthoSizeTarget, transitionTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate(UpdateCamOrthoSize);

        LeanTween.value(fadeIntoLevel.gameObject, 0f, 1, transitionTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => FadeInCorruptZone(ID)).setOnUpdate((float val) =>
        {
            Image sr = fadeIntoLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });
    }

    void UpdateCamOrthoSize(float value)
    {
        Camera.main.orthographicSize = value;
    }

    void FadeInCorruptZone(int ID)
    {
        UIManager.Instance.worldGameObject.SetActive(false);
        UIManager.Instance.hudCanvasUIBottomZoneMainMap.SetActive(false);
        //UIManager.Instance.hudCanvasUI.SetActive(false);

        UIManager.Instance.corruptedZoneScreen.SetActive(true);
        UIManager.Instance.hudCanvasUIBottomZoneCorruption.SetActive(true);

        CorruptedZoneViewHelpData CZVHD = CorruptedZonesManager.Instance.allCorruptedZonesView.Where(p => p.ZoneIDView == ID).Single();

        CZVHD.gameObject.SetActive(true);

        CZVHD.UpdateCorruptionManagerData();

        if (CZVHD.connectedCZD)
        {
            if (CZVHD.connectedCZD.saveDataZone.isClensing)
            {
                CZVHD.harmonySliderInCorruptedZone.gameObject.SetActive(true);
                CZVHD.harmonySliderOnMap.gameObject.SetActive(true);
            }
        }

        if (!CorruptedZonesManager.Instance.currentActiveZoneView.isFullyClensed)
        {
            PlayerManager.Instance.SpawnOwnedCorruptionDevices();
        }

        LeanTween.value(fadeIntoLevel.gameObject, 1, 0, transitionTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeIntoLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });
    }

    public IEnumerator AnimateUnlockScreen(int ID)
    {
        SoundManager.Instance.PlaySound(Sounds.UnlockZone);

        //Camera toMove = Camera.main;

        UIManager.Instance.zoneToUnlcokNameText.text = ZoneManagerHelpData.Instance.listOfAllZones[ID].zoneName;

        //Transform target = ZoneManagerHelpData.Instance.listOfAllZones[ID].transform;

        RectTransform target = ZoneManagerHelpData.Instance.listOfAllZones[ID].GetComponent<RectTransform>();
        Debug.Log("Target Y" + target.localPosition.y);

        LeanTween.move(toMoveOpenZone.GetComponent<RectTransform>(), new Vector3(toMoveOpenZone.transform.position.x, ZoneManagerHelpData.Instance.unlockPosPerZone[ID].y, 0), cameraMoveTime).setEase(LeanTweenType.easeInOutQuad); // animate

        yield return new WaitForSeconds(cameraMoveTime - 0.8f);
        FadeInUnlcokScreen();

        yield return new WaitForSeconds(1);
        ZoneManager.CanUnlockZone = true;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] {SystemsToSave.ZoneManager, SystemsToSave.ZoneX});
    }

    public void AnimateBrewScreen(string PotionName, string PotionSpritePath)
    {
        SoundManager.Instance.PlaySound(Sounds.ElementCrafted);

        brewNameText.text = PotionName;
        potionIcon.sprite = Resources.Load<Sprite>(PotionSpritePath);

        FadeInBrewedScreen();
    }
    public void AnimateCraftedHollowcreen(string HollowItemname, int HollowItemSpriteIndex)
    {
        SoundManager.Instance.PlaySound(Sounds.ElementCrafted);

        craftedNameText.text = HollowItemname;
        //hollowIcon.sprite = Resources.Load<Sprite>(HollowItemSpritePath);
        hollowIcon.sprite = HollowCraftAndOwnedManager.Instance.denItemSprites[HollowItemSpriteIndex];

        FadeInHollowCraftedScreen();
    }

    internal void CheckContinuedTutorials()
    {
        throw new System.NotImplementedException();
    }

    void FadeInUnlcokScreen()
    {
        bgFadeImage.color = new Color(bgFadeImage.color.r, bgFadeImage.color.g, bgFadeImage.color.b, 0);
        zoneToUnlcokHeaderText.color = new Color(zoneToUnlcokHeaderText.color.r, zoneToUnlcokHeaderText.color.g, zoneToUnlcokHeaderText.color.b, 0);
        zoneToUnlcokNameText.color = new Color(zoneToUnlcokNameText.color.r, zoneToUnlcokNameText.color.g, zoneToUnlcokNameText.color.b, 0);
        tapToContinueText.color = new Color(tapToContinueText.color.r, tapToContinueText.color.g, tapToContinueText.color.b, 0);


        ParticleSystemRenderer r = particleFade.GetComponent<ParticleSystemRenderer>();
        //r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0);

        Color c = Color.white;
        r.sharedMaterial.SetColor("_BaseColor", new Color(c.r, c.g, c.b, 0));

        UIManager.Instance.UnlockedZoneMessageView.SetActive(true);


        LeanTween.value(bgFadeImage.gameObject, 0f, 0.65f, fadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image image = bgFadeImage;
            Color newColor = image.color;
            newColor.a = val;
            image.color = newColor;
        });

        LeanTween.value(particleFade.gameObject, 0f, 1f, fadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            //ParticleSystemRenderer r = particleFade.GetComponent<ParticleSystemRenderer>();

            Color newColor = Color.white;
            newColor.a = val;
            //r.material.color = newColor;
            r.sharedMaterial.SetColor("_BaseColor", newColor);
        });

        LeanTween.value(zoneToUnlcokHeaderText.gameObject, 0f, 1f, fadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = zoneToUnlcokHeaderText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(zoneToUnlcokNameText.gameObject, 0f, 1, fadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = zoneToUnlcokNameText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(tapToContinueText.gameObject, 0f, 1, fadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = tapToContinueText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

    }


    void FadeInBrewedScreen()
    {
        brewbgFadeImage.color = new Color(brewbgFadeImage.color.r, brewbgFadeImage.color.g, brewbgFadeImage.color.b, 0);
        brewHeaderText.color = new Color(brewHeaderText.color.r, brewHeaderText.color.g, brewHeaderText.color.b, 0);
        brewNameText.color = new Color(brewNameText.color.r, brewNameText.color.g, brewNameText.color.b, 0);
        brewtapToContinueText.color = new Color(brewtapToContinueText.color.r, brewtapToContinueText.color.g, brewtapToContinueText.color.b, 0);
        potionIcon.color = new Color(potionIcon.color.r, potionIcon.color.g, potionIcon.color.b, 0);

        ParticleSystemRenderer r = brewparticleFade.GetComponent<ParticleSystemRenderer>();
        //r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0);

        Color c = Color.white;
        r.sharedMaterial.SetColor("_BaseColor", new Color(c.r, c.g, c.b, 0));

        UIManager.Instance.brewedPotionScreen.SetActive(true);


        LeanTween.value(brewbgFadeImage.gameObject, 0f, 0.65f, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image image = brewbgFadeImage;
            Color newColor = image.color;
            newColor.a = val;
            image.color = newColor;
        });

        LeanTween.value(brewparticleFade.gameObject, 0f, 1f, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            //ParticleSystemRenderer r = particleFade.GetComponent<ParticleSystemRenderer>();

            Color newColor = Color.white;
            newColor.a = val;
            //r.material.color = newColor;
            r.sharedMaterial.SetColor("_BaseColor", newColor);
        });

        LeanTween.value(brewHeaderText.gameObject, 0f, 1f, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = brewHeaderText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(brewNameText.gameObject, 0f, 1, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = brewNameText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(brewtapToContinueText.gameObject, 0f, 1, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = brewtapToContinueText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(potionIcon.gameObject, 0f, 1, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            SpriteRenderer sprite = potionIcon;
            Color newColor = sprite.color;
            newColor.a = val;
            sprite.color = newColor;
        });

    }
    void FadeInHollowCraftedScreen()
    {
        craftedbgFadeImage.color = new Color(craftedbgFadeImage.color.r, craftedbgFadeImage.color.g, craftedbgFadeImage.color.b, 0);
        craftedHeaderText.color = new Color(craftedHeaderText.color.r, craftedHeaderText.color.g, craftedHeaderText.color.b, 0);
        craftedNameText.color = new Color(craftedNameText.color.r, craftedNameText.color.g, craftedNameText.color.b, 0);
        craftedtapToContinueText.color = new Color(craftedtapToContinueText.color.r, craftedtapToContinueText.color.g, craftedtapToContinueText.color.b, 0);
        hollowIcon.color = new Color(hollowIcon.color.r, hollowIcon.color.g, hollowIcon.color.b, 0);

        ParticleSystemRenderer r = brewparticleFade.GetComponent<ParticleSystemRenderer>();
        //r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0);

        Color c = Color.white;
        r.sharedMaterial.SetColor("_BaseColor", new Color(c.r, c.g, c.b, 0));

        UIManager.Instance.craftedHollowItemScreen.SetActive(true);


        LeanTween.value(craftedbgFadeImage.gameObject, 0f, 0.65f, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image image = craftedbgFadeImage;
            Color newColor = image.color;
            newColor.a = val;
            image.color = newColor;
        });

        LeanTween.value(brewparticleFade.gameObject, 0f, 1f, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            //ParticleSystemRenderer r = particleFade.GetComponent<ParticleSystemRenderer>();

            Color newColor = Color.white;
            newColor.a = val;
            //r.material.color = newColor;
            r.sharedMaterial.SetColor("_BaseColor", newColor);
        });

        LeanTween.value(craftedHeaderText.gameObject, 0f, 1f, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = craftedHeaderText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(craftedNameText.gameObject, 0f, 1, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = craftedNameText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(craftedtapToContinueText.gameObject, 0f, 1, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            TMP_Text text = craftedtapToContinueText;
            Color newColor = text.color;
            newColor.a = val;
            text.color = newColor;
        });

        LeanTween.value(hollowIcon.gameObject, 0f, 1, brewfadingTime).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            SpriteRenderer sprite = hollowIcon;
            Color newColor = sprite.color;
            newColor.a = val;
            sprite.color = newColor;
        });

    }

    public void ResetAllSkipData()
    {
        hasSkippedToAnimalAnim = false;
        hasSkippedToAfterAnimalAnim = false;
        hasSkippedToBoardAnim = false;
    }


    public void ResetEnterLevelAnimation()
    {
        ring = null;
        colorMask = null;
        originalColorMaskAlpha = 0;
        clips = null;
        locks.Clear();
        corruptedTiles.Clear();
        slices.Clear();
    }

    public void PopulateRefrencesEnterLevelAnim()
    {
        ring = GameManager.Instance.gameBoard.transform.GetComponent<SpriteRenderer>();

        colorMask = GameManager.Instance.selectedLevelBG.transform.Find("RingMask").GetComponent<SpriteRenderer>();
        originalColorMaskAlpha = colorMask.color.a;

        clips = GameManager.Instance.clipManager.slots;

        SliceManager sliceManager = GameManager.Instance.gameBoard.GetComponent<SliceManager>();
        for (int i = 0; i < sliceManager.activeLocksLockAnims.Count; i++)
        {
            SpriteRenderer renderer = sliceManager.activeLocksLockAnims[i].GetComponent<SpriteRenderer>();
            locks.Add(renderer);
        }

        Piece[] allPieces = FindObjectsOfType<Piece>();

        for (int i = 0; i < allPieces.Length; i++)
        {
            if(allPieces[i].isStone)
            {
                corruptedTiles.Add(allPieces[i]);
            }
        }
        slices.AddRange(GameManager.Instance.gameBoard.GetComponent<SliceManager>().fullSlices);

        //corrupted tiles are added when they are instantiated.
        SetDefaultValuesEnterLevelAnimation();
    }

    public void SetDefaultValuesEnterLevelAnimation()
    {
        //ring.color = new Color(ring.color.r, ring.color.g, ring.color.b, 0);
        ring.material.SetFloat("_DissolveSprite", 0.6f);

        foreach (var clip in clips)
        {
            SpriteRenderer renderer = clip.GetComponent<SpriteRenderer>();

            renderer.material.SetFloat("_DissolveSprite", 0.6f);
        }

        colorMask.color = new Color(colorMask.color.r, colorMask.color.g, colorMask.color.b, 0);

        foreach (var lockObject in locks)
        {
            SpriteRenderer lockRenderer = lockObject.GetComponent<SpriteRenderer>();

            lockRenderer.color = new Color(lockRenderer.color.r, lockRenderer.color.g, lockRenderer.color.b, 0);
        }

        foreach (var tile in corruptedTiles)
        {
            tile.transform.localScale = Vector3.zero;
        }

        foreach (var slice in slices)
        {
            slice.transform.localScale = Vector3.zero;
        }


        for (int i = 0; i < GameManager.Instance.clipManager.slots.Count(); i++)
        {
            GameObject toMove = GameManager.Instance.clipManager.slots[i].GetChild(1).gameObject;

            toMove.transform.localPosition = GameManager.Instance.clipManager.piecesDealPositionsOut;
        }

        StartCoroutine(ActivateLevelAnim());
    }

    IEnumerator ActivateLevelAnim()
    {
        UIManager.Instance.restartButton.interactable = false;
        UIManager.Instance.optionsButtonIngame.interactable = false;
        UIManager.Instance.cheatOptionsButtonIngame.interactable = false;
        UIManager.Instance.dealButton.interactable = false;
        GameManager.Instance.powerupManager.PowerupButtonsActivation(false);

        ZoneManagerHelpData.Instance.ChangeZoneToNormalZoneDisplay();

        yield return new WaitForSeconds(startAnimDelayTime);

        ring.gameObject.SetActive(true);

        if (clips.Length > 0)
        {
            clips[0].transform.parent.gameObject.SetActive(true);

            foreach (var clip in clips)
            {
                clip.gameObject.SetActive(true);
            }
        }


        //fade ring
        float startDissolve = ring.material.GetFloat("_DissolveSprite");
        float endDissolve = 1.5f;
        SoundManager.Instance.PlaySound(Sounds.RingAppear);
        LeanTween.value(ring.gameObject, startDissolve, endDissolve, timeToFadeRingAndColormask).setOnUpdate((float val) =>
        {
            ring.material.SetFloat("_DissolveSprite", val);
        });

        //fade clips
        foreach (var clip in clips)
        {
            SpriteRenderer renderer = clip.GetComponent<SpriteRenderer>();

            float startDissolveClip = renderer.material.GetFloat("_DissolveSprite");
            float endDissolveClip = 1.5f;
            LeanTween.value(clip.gameObject, startDissolveClip, endDissolveClip, timeToFadeClip).setOnUpdate((float val) =>
            {
                renderer.material.SetFloat("_DissolveSprite", val);
            });
        }





        LeanTween.value(colorMask.gameObject, colorMask.color.a, originalColorMaskAlpha, timeToFadeRingAndColormask).setOnUpdate((float val) =>
        {
            Color newColor = colorMask.color;
            newColor.a = val;
            colorMask.color = newColor;
        });

        foreach (var lockObject in locks)
        {
            LeanTween.value(lockObject.gameObject, lockObject.color.a, 1, timeToFadeLocks).setOnUpdate((float val) =>
            {
                Color newColor = lockObject.color;
                newColor.a = val;
                lockObject.color = newColor;
            });
        }

        yield return new WaitForSeconds(timeToFadeRingAndColormask + 0.01f);

        ZoneManagerHelpData.Instance.ChangeZoneToBlurryZoneDisplay();

        foreach (var tile in corruptedTiles)
        {
            LeanTween.scale(tile.gameObject, scaleToSize, timeToScaleCorruptedTilesUp);
            LeanTween.moveLocalY(tile.gameObject, MoveLocalToY, MoveLocalYTimeUp);
            yield return new WaitForSeconds(timeToScaleCorruptedTilesUp + 0.01f);
            LeanTween.scale(tile.gameObject, Vector3.one, timeToScaleCorruptedTilesDown);
            LeanTween.moveLocalY(tile.gameObject, 0f, MoveLocalYTimeDown);

            SoundManager.Instance.PlaySound(Sounds.CorruptedTilePop);
            yield return new WaitForSeconds(timeToScaleCorruptedTilesDown + 0.01f);
        }

        foreach (var slice in slices)
        {
            LeanTween.scale(slice.gameObject, new Vector3(1.5f, 1.5f, 1.5f), timeToScaleSlices);
            yield return new WaitForSeconds(timeToScaleSlices + 0.01f);
            LeanTween.scale(slice.gameObject, Vector3.one, timeToScaleSlices);

            SoundManager.Instance.PlaySound(Sounds.LimiterPop);
            yield return new WaitForSeconds(timeToScaleSlices / 10);
        }

        //yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < GameManager.Instance.clipManager.slots.Count(); i++)
        {
            GameObject toMove = GameManager.Instance.clipManager.slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, GameManager.Instance.clipManager.originalPiecePos, 0.5f).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate

            yield return new WaitForSeconds(0.1f);
        }
        SoundManager.Instance.PlaySound(Sounds.TileEnterLevel);

    }
}
