using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [Header("Health Settings")]
    public float waitBetweenPieceMove;
    public float speedPieceMove;

    public float waitBetweenPiecePullIn;
    public float speedPiecePullIn;

    //public float paceFade;

    public float waitTimeParticlesStart;
    public float waitTimeMidParticleAppear;
    public float waitTimeDissolveTiles;
    public float waitTimePullIn;
    public float waitTimeFadeIn;
    public float waitTimeFadeOut;
    public float waitTimeWinScreen;

    public float fadingSpeed;

    public float animalWaitTime;

    public Image fadeImageEndLevel;

    public bool noWaitPieces;
    public bool noWaitParticles;
    public bool noWaitPullIn;
    public bool noWaitAnimal;
    //public ParticleSystem midPieceParticle;
    public List<SubPiece> tempSubPieceArray;

    public timeWaitPull minMaxWaitPullInPieces;


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
    public void StartEndLevelAnimSequence()
    {
        endAnim = StartCoroutine(StartEndLevelAnim());
    }
    public void StartZoomIntoCorruptArea(int ID)
    {
        ZoomIntoCorruptArea(ID);
    }

    public IEnumerator StartEndLevelAnim()
    {
        UIManager.Instance.skipAnimationButton.gameObject.SetActive(true);

        UIManager.Instance.restartButton.interactable = false;

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

        yield return new WaitForSeconds(waitTimeDissolveTiles);

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

        LeanTween.value(fadeImageEndLevel.gameObject, 0f, 1, fadingSpeed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImageEndLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        yield return new WaitForSeconds(waitTimeWinScreen);

        GameManager.Instance.WinAfterAnimation();

        Destroy(GameManager.Instance.gameBoard.gameObject);
        Destroy(GameManager.Instance.gameClip.gameObject);

        yield return new WaitForSeconds(waitTimeFadeOut);

        LeanTween.value(fadeImageEndLevel.gameObject, 1, 0, fadingSpeed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImageEndLevel;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);
        UIManager.Instance.TurnOffGameplayUI();
        UIManager.Instance.InGameUiScreens.SetActive(true);

        if (!noWaitAnimal)
        {
            yield return new WaitForSeconds(animalWaitTime);
        }

        AnimalsManager.Instance.CheckUnlockAnimal(AnimalsManager.Instance.currentLevelAnimal);


        //yield return new WaitForSeconds(waitTimeFadeOut);
        yield return new WaitUntil((() => fadeImageEndLevel.color.a <= 0.1f));
        fadeImageEndLevel.gameObject.SetActive(false);

        UIManager.Instance.restartButton.interactable = true;

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

    public void DissolveTiles()
    {
        foreach (SubPiece SP in ConnectionManager.Instance.subPiecesOnBoard)
        {
            List<Material> matArray = new List<Material>();

            Renderer rend = SP.GetComponent<Renderer>();

            matArray.AddRange(rend.materials);

            Material mat = GameManager.Instance.clipManager.symbolToMat.Where(p => p.mat == SP.symbolOfPiece).Single().symbolMat;

            if (matArray.Count > 1)
            {
                matArray.Clear();
                matArray.Add(mat);
                rend.materials = matArray.ToArray();
            }
            else
            {
                rend.material = mat;
            }
        }
    }

    public void SkipEndLevelAnimation()
    {
        StopCoroutine(endAnim);

        UIManager.Instance.skipAnimationButton.gameObject.SetActive(false);

        tempSubPieceArray.Clear();

        GameObject[] turnOff = GameObject.FindGameObjectsWithTag("Off on end level");

        foreach (GameObject GO in turnOff)
        {
            GO.SetActive(false);
        }

        fadeImageEndLevel.gameObject.SetActive(false);


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
        UIManager.Instance.hudCanvasDisplay.SetActive(false);
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
}
