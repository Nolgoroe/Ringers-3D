using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

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

    public Image fadeImageEndLevel;

    public bool noWaitPieces;
    public bool noWaitParticles;
    public bool noWaitDissolve;
    public bool noWaitPullIn;
    public bool noWaitAnimal;

    public bool hasSkippedToAnimalAnim;

    //public ParticleSystem midPieceParticle;
    public List<SubPiece> tempSubPieceArray;

    public timeWaitPull minMaxWaitPullInPieces;

    private GameObject boardScreenshot;

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

    [Header("Enter Corrupt Zone")]
    [Space(30)]
    public float zoomSpeed = 0;
    public float transitionTime;

    public float cameraOrthoSizeTarget;
    public Image fadeIntoLevel;

    private Coroutine endAnimToAnimal = null;

    [HideInInspector]
    public Coroutine endAnimToWinScreen = null;

    bool dissolveStart = false;

    public GameObject[] turnOff;

    public GameObject toMoveOpenZone;

    void Start()
    {
        instance = this;
        tempSubPieceArray = new List<SubPiece>();
        fadeImageEndLevel.gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct timeWaitPull
    {
        public float a, b;
    }
    public void StartEndLevelAnimSequence(bool cheat)
    {
        if (cheat)
        {
            SkipEndLevelAnimation(true);
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
        turnOff = GameObject.FindGameObjectsWithTag("Off on end level");

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(true);

        UIManager.Instance.restartButton.interactable = false;
        UIManager.Instance.dealButton.interactable = false;

        tempSubPieceArray.AddRange(ConnectionManager.Instance.subPiecesOnBoard);

  


        MoveTopButtonAnim();


        //yield return new WaitForSeconds(waitTimeMoveTopBottomTime);


        //foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
        //{
        //    MoveSubPiece(SP);

        //    if (!noWaitPieces)
        //    {
        //        yield return new WaitForSeconds(waitBetweenPieceMove);
        //    }
        //}

        ConnectionManager.Instance.TurnOffAllConnectedVFX();

        yield return new WaitForSeconds(speedOutTopBottom + 0.1f);
        boardScreenshot = Instantiate(GameManager.Instance.gameBoard, new Vector3(100, 0, 0), Quaternion.identity);
        boardScreenshot.transform.SetParent(GameManager.Instance.destroyOutOfLevel);
        yield return new WaitForSeconds(0.1f);

        GameManager.Instance.sliceManager.endLevelAnimVFX.SetActive(true);


        
        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(false);
        }

        GameManager.Instance.gameBoard.GetComponent<SpriteRenderer>().enabled = false;



        //yield return new WaitForSeconds(waitTimeParticlesStart);

        //foreach (Cell C in ConnectionManager.Instance.cells)
        //{
        //    ActivateParticleEffectsMiddle(C.pieceHeld.midPiece);
        //    if (!noWaitParticles)
        //    {
        //        yield return new WaitForSeconds(waitBetweenPieceMove);
        //    }
        //}

        if (!noWaitDissolve)
        {
            yield return new WaitForSeconds(waitTimeDissolveTiles);
        }

        //// Dissolve Tiles Here
        StartCoroutine(DissolveTiles());

        yield return new WaitUntil(() => dissolveStart == true);

        //yield return new WaitForSeconds(waitTimePullIn);


        //for (int i = 0; i < ConnectionManager.Instance.subPiecesOnBoard.Length; i++)
        //{

        //    int rand = UnityEngine.Random.Range(0, tempSubPieceArray.Count);

        //    PullIn(tempSubPieceArray[rand]);

        //    tempSubPieceArray.RemoveAt(rand);

        //    if (!noWaitPullIn)
        //    {
        //        yield return new WaitForSeconds(UnityEngine.Random.Range(minMaxWaitPullInPieces.a, minMaxWaitPullInPieces.b));
        //    }
        //}

        yield return new WaitForSeconds(waitTimeFadeIn);
        fadeImageEndLevel.gameObject.SetActive(true);

        LeanTween.value(fadeImageEndLevel.gameObject, 0f, 1, fadingSpeedGameplay).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImageEndLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });


        yield return new WaitForSeconds(waitTimeWinScreen);


        //Destroy(GameManager.Instance.gameBoard.gameObject);
        GameManager.Instance.gameBoard.gameObject.transform.position = new Vector3(100, 0, 0);
        Destroy(GameManager.Instance.gameClip.gameObject);

        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(true);
        }

        turnOff = null;

        //GameManager.Instance.WinAfterAnimation();
        //MoveBoardScreenshotToPosition(boardScreenshot);
        //UnDissolveTiles();


        yield return new WaitForSeconds(waitTimeFadeOut);

        LeanTween.value(fadeImageEndLevel.gameObject, 1, 0, fadingSpeedGameplay).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImageEndLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        UIManager.Instance.TurnOffGameplayUI();
        UIManager.Instance.InGameUiScreens.SetActive(true);
        GameManager.Instance.selectedLevelBG.transform.Find("color mask").gameObject.SetActive(false);

        if (!noWaitAnimal)
        {
            yield return new WaitForSeconds(animalWaitTime);
        }

        AnimalsManager.Instance.CheckUnlockAnimal(AnimalsManager.Instance.currentLevelAnimal);


        //yield return new WaitForSeconds(waitTimeFadeOut);
        yield return new WaitUntil((() => fadeImageEndLevel.color.a <= 0.1f));
        fadeImageEndLevel.gameObject.SetActive(false);

        UIManager.Instance.restartButton.interactable = true;
        UIManager.Instance.dealButton.interactable = true;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneX, SystemsToSave.ZoneManager, SystemsToSave.Player, SystemsToSave.animalManager });

        yield return new WaitForSeconds(4f);

        UnDissolveTiles(false);

        yield return new WaitForSeconds(0.1f); //unDissolve time
        MoveBoardScreenshotToPosition(boardScreenshot);
        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);
        GameManager.Instance.WinAfterAnimation();
        CheckShowLootTutorial();
        TutorialSequence.Instacne.CheckDoPotionTutorial();
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
                SP.UnDissolveSubPiece();
            }
        }
    }

    public void SkipEndLevelAnimation(bool isCheat)
    {
        if (hasSkippedToAnimalAnim)
        {
            StopCoroutine(endAnimToWinScreen);

            endAnimToWinScreen = StartCoroutine(AfterAnimalAnimation(true));
            return;
        }
        else
        {
            hasSkippedToAnimalAnim = true;
            UIManager.Instance.skipAnimationButton.gameObject.SetActive(true);
            endAnimToWinScreen = StartCoroutine(AfterAnimalAnimation(false));
        }

        SoundManager.Instance.audioSource.Stop();

        if (endAnimToAnimal != null)
        {
            StopCoroutine(endAnimToAnimal);
            LeanTween.cancelAll();
        }

        GameManager.Instance.gameBoard.gameObject.transform.position = new Vector3(100, 0, 0);

        //if (!isCheat)
        //{
        //    UnDissolveTiles(true);
        //    ConnectionManager.Instance.TurnOffAllConnectedVFX();

        //    if(boardScreenshot == null)
        //    {
        //        boardScreenshot = Instantiate(GameManager.Instance.gameBoard, new Vector3(100, 0, 0), Quaternion.identity);
        //        boardScreenshot.transform.SetParent(GameManager.Instance.destroyOutOfLevel);
        //    }


        //    MoveBoardScreenshotToPosition(boardScreenshot);
        //}

        tempSubPieceArray.Clear();

        if(turnOff == null)
        {
            turnOff = GameObject.FindGameObjectsWithTag("Off on end level");
        }

        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(true);
        }

        turnOff = null;

        GameManager.Instance.selectedLevelBG.transform.Find("color mask").gameObject.SetActive(false);

        fadeImageEndLevel.gameObject.SetActive(false);


        if (GameManager.Instance.gameBoard.gameObject)
        {
            //Destroy(GameManager.Instance.gameBoard.gameObject);
            Destroy(GameManager.Instance.gameClip.gameObject);
        }

        UIManager.Instance.restartButton.interactable = true;

        UIManager.Instance.TurnOffGameplayUI();

        UIManager.Instance.InGameUiScreens.SetActive(true);
        AnimalsManager.Instance.CheckUnlockAnimal(AnimalsManager.Instance.currentLevelAnimal);


        //TutorialSequence.Instacne.CheckDoPotionTutorial();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneX, SystemsToSave.ZoneManager, SystemsToSave.Player, SystemsToSave.animalManager });
    }

    public IEnumerator AfterAnimalAnimation(bool isSkip)
    {
        if (!isSkip)
        {
            yield return new WaitForSeconds(4);
        }

        UnDissolveTiles(true);
        ConnectionManager.Instance.TurnOffAllConnectedVFX();

        if (boardScreenshot == null)
        {
            boardScreenshot = Instantiate(GameManager.Instance.gameBoard, new Vector3(100, 0, 0), Quaternion.identity);
            boardScreenshot.transform.SetParent(GameManager.Instance.destroyOutOfLevel);
        }

        CheckShowLootTutorial();

        MoveBoardScreenshotToPosition(boardScreenshot);

        hasSkippedToAnimalAnim = false;

        Destroy(AnimalsManager.Instance.currentLevelLiveAnimal.gameObject);

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);

        GameManager.Instance.WinAfterAnimation();

        TutorialSequence.Instacne.CheckDoPotionTutorial();
    }
    private void CheckShowLootTutorial()
    {
        if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.lootTutorial && GameManager.Instance.currentLevel.isSpecificTutorial && !TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains((int)GameManager.Instance.currentLevel.specificTutorialEnum)) /// specificTutorialIndex == 0  is loot tutorial
        {
            //TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add(GameManager.Instance.currentLevel.specificTutorialIndex);
            LootManager.Instance.rubiesToRecieveInLevel = 8;
            StartCoroutine(TutorialSequence.Instacne.DisplaySpecificTutorialSequence());
            //TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add((int)GameManager.Instance.currentLevel.specificTutorialEnum);
            TutorialSequence.Instacne.currentSpecificTutorial = SpecificTutorialsEnum.lootTutorial;
            //TutorialSaveData.Instance.SaveTutorialSaveData();
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

        LeanTween.move(toMoveOpenZone.GetComponent<RectTransform>(), new Vector3(toMoveOpenZone.transform.position.x, ZoneManagerHelpData.Instance.listOfAllZones[ID].zonePosForUnlock.y, 0), cameraMoveTime).setEase(LeanTweenType.easeInOutQuad); // animate

        yield return new WaitForSeconds(cameraMoveTime - 0.8f);
        FadeInUnlcokScreen();

        yield return new WaitForSeconds(1);
        ZoneManager.CanUnlockZone = true;

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] {SystemsToSave.ZoneManager, SystemsToSave.ZoneX});
    }

    public void AnimateBrewScreen(string PotionName, string PotionSpritePath)
    {
        SoundManager.Instance.PlaySound(Sounds.ElementCrafted);
        UIManager.isUsingUI = true;

        brewNameText.text = PotionName;
        potionIcon.sprite = Resources.Load<Sprite>(PotionSpritePath);

        FadeInBrewedScreen();
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
}
