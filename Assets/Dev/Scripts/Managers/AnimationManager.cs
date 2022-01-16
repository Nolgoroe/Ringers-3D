﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    public float waitBetweenPieceMove;
    public float speedPieceMove;

    public float waitBetweenPiecePullIn;
    public float speedPiecePullIn;
    public float dissolveSpeed = 5;

    //public float paceFade;

    public float waitTimeParticlesStart;
    public float waitTimeMidParticleAppear;
    public float waitTimeDissolveTiles;
    public float waitTimePullIn;
    public float waitTimeFadeIn;
    public float waitTimeFadeOut;
    public float waitTimeWinScreen;

    public float fadingSpeedGameplay;

    public float animalWaitTime;

    public Image fadeImageEndLevel;

    public bool noWaitPieces;
    public bool noWaitParticles;
    public bool noWaitDissolve;
    public bool noWaitPullIn;
    public bool noWaitAnimal;
    //public ParticleSystem midPieceParticle;
    public List<SubPiece> tempSubPieceArray;

    public timeWaitPull minMaxWaitPullInPieces;

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

    [HideInInspector]
    public Coroutine endAnim = null;

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
            SkipEndLevelAnimation();
        }
        else
        {
            endAnim = StartCoroutine(StartEndLevelAnim());
        }
    }
    public void StartZoomIntoCorruptArea(int ID)
    {
        ZoomIntoCorruptArea(ID);
    }

    public IEnumerator StartEndLevelAnim()
    {
        UIManager.Instance.skipAnimationButton.gameObject.SetActive(true);

        UIManager.Instance.restartButton.interactable = false;
        UIManager.Instance.dealButton.interactable = false;

        tempSubPieceArray.AddRange(ConnectionManager.Instance.subPiecesOnBoard);

        GameObject[] turnOff = GameObject.FindGameObjectsWithTag("Off on end level");

        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(false);
        }

        foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
        {
            MoveSubPiece(SP);

            if (!noWaitPieces)
            {
                yield return new WaitForSeconds(waitBetweenPieceMove);
            }
        }

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
        DissolveTiles();

        yield return new WaitForSeconds(waitTimePullIn);


        for (int i = 0; i < ConnectionManager.Instance.subPiecesOnBoard.Length; i++)
        {

            int rand = Random.Range(0, tempSubPieceArray.Count);

            PullIn(tempSubPieceArray[rand]);

            tempSubPieceArray.RemoveAt(rand);

            if (!noWaitPullIn)
            {
                yield return new WaitForSeconds(Random.Range(minMaxWaitPullInPieces.a, minMaxWaitPullInPieces.b));
            }
        }

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

        CheckShowLootTutorial();

        GameManager.Instance.WinAfterAnimation();

        Destroy(GameManager.Instance.gameBoard.gameObject);
        Destroy(GameManager.Instance.gameClip.gameObject);

        TutorialSequence.Instacne.CheckDoPotionTutorial();

        yield return new WaitForSeconds(waitTimeFadeOut);

        LeanTween.value(fadeImageEndLevel.gameObject, 1, 0, fadingSpeedGameplay).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImageEndLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);
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
        yield return null;
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

    //public void ActivateParticleEffectsMiddle(GameObject midPiece)
    //{
    //    Instantiate(midPieceParticle, midPiece.transform);
    //}

    public void PullIn(SubPiece toMove)
    {
        LeanTween.move(toMove.gameObject, GameManager.Instance.gameBoard.transform.position, speedPieceMove).setEase(LeanTweenType.easeInOutQuad); // animate
    }

    [ContextMenu("Disslove Tiles")]
    public void DissolveTiles()
    {
        foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
        {
            Material mat = SP.GetComponent<Renderer>().material;

            LeanTween.value(mat.GetFloat("Dissolve_Amount"), 0.5f, dissolveSpeed).setOnUpdate((float val) =>
            {
                mat.SetFloat("Dissolve_Amount", val);
            });
        }
    }

    public void SkipEndLevelAnimation()
    {
        SoundManager.Instance.audioSource.Stop();

        if (endAnim != null)
        {
            StopCoroutine(endAnim);
        }

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);

        tempSubPieceArray.Clear();

        GameObject[] turnOff = GameObject.FindGameObjectsWithTag("Off on end level");

        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(false);
        }

        GameManager.Instance.selectedLevelBG.transform.Find("color mask").gameObject.SetActive(false);

        fadeImageEndLevel.gameObject.SetActive(false);

        CheckShowLootTutorial();

        GameManager.Instance.WinAfterAnimation();

        if (GameManager.Instance.gameBoard.gameObject)
        {
            Destroy(GameManager.Instance.gameBoard.gameObject);
            Destroy(GameManager.Instance.gameClip.gameObject);
        }
        UIManager.Instance.restartButton.interactable = true;

        UIManager.Instance.TurnOffGameplayUI();

        UIManager.Instance.InGameUiScreens.SetActive(true);
        AnimalsManager.Instance.CheckUnlockAnimal(AnimalsManager.Instance.currentLevelAnimal);


        TutorialSequence.Instacne.CheckDoPotionTutorial();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.ZoneX, SystemsToSave.ZoneManager, SystemsToSave.Player, SystemsToSave.animalManager });
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

        Camera toMove = Camera.main;

        UIManager.Instance.zoneToUnlcokNameText.text = ZoneManagerHelpData.Instance.listOfAllZones[ID].zoneName;

        Transform target = ZoneManagerHelpData.Instance.listOfAllZones[ID].transform;

        LeanTween.move(toMove.gameObject, new Vector3(target.position.x, target.transform.position.y, -50), cameraMoveTime).setEase(LeanTweenType.easeInOutQuad); // animate

        yield return new WaitUntil((() => toMove.transform.position.x - target.position.x <= 0.1f));
        FadeInUnlcokScreen();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] {SystemsToSave.ZoneManager, SystemsToSave.ZoneX});
    }

    public void AnimateBrewScreen(string PotionName, string PotionSpritePath)
    {
        SoundManager.Instance.PlaySound(Sounds.UnlockZone);
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
