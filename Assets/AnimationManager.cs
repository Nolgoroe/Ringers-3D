using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    public float waitBetweenPieceMove;
    public float speedPieceMove;

    public float waitBetweenPiecePullIn;
    public float speedPiecePullIn;

    public float paceFade;

    public float waitTimeParticlesStart;
    public float waitTimeMidParticleAppear;
    public float waitTimePullIn;
    public float waitTimeFadeIn;
    public float waitTimeFadeOut;
    public float waitTimeWinScreen;

    public float fadingSpeed;

    public Image fadeImage;

    public bool noWaitPieces;
    public bool noWaitParticles;
    public bool noWaitPullIn;
    public ParticleSystem midPieceParticle;
    public List<SubPiece> temp;

    public timeWaitPull minMaxWait;
    void Start()
    {
        instance = this;
        temp = new List<SubPiece>();
        fadeImage.gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct timeWaitPull
    {
        public float a, b;
    }
    public IEnumerator StartEndLevelAnim()
    {
        temp.AddRange(ConnectionManager.Instance.subPiecesOnBoard);

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

        yield return new WaitForSeconds(waitTimeParticlesStart);

        foreach (Cell C in ConnectionManager.Instance.cells)
        {
            ActivateParticleEffectsMiddle(C.pieceHeld.midPiece);
            if (!noWaitParticles)
            {
                yield return new WaitForSeconds(waitBetweenPieceMove);
            }
        }

        yield return new WaitForSeconds(waitTimePullIn);


        for (int i = 0; i < ConnectionManager.Instance.subPiecesOnBoard.Length; i++)
        {

            int rand = Random.Range(0, temp.Count);

            PullIn(temp[rand]);

            temp.RemoveAt(rand);

            if (!noWaitPullIn)
            {
                yield return new WaitForSeconds(Random.Range(minMaxWait.a, minMaxWait.b));
            }
        }

        yield return new WaitForSeconds(waitTimeFadeIn);
        fadeImage.gameObject.SetActive(true);

        LeanTween.value(fadeImage.gameObject, 0f, 1, fadingSpeed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImage;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });

        yield return new WaitForSeconds(waitTimeWinScreen);

        GameManager.Instance.WinAfterAnimation();

        Destroy(GameManager.Instance.gameBoard.gameObject);
        Destroy(GameManager.Instance.gameClip.gameObject);
        yield return new WaitForSeconds(waitTimeFadeOut);

        LeanTween.value(fadeImage.gameObject, 1, 0, fadingSpeed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
        {
            Image sr = fadeImage;
            Color newColor = sr.color;
            newColor.a = val;
            sr.color = newColor;
        });


        yield return null;
    }

    public void MoveSubPiece(SubPiece toMove)
    {
        if(toMove.subPieceIndex % 2 != 0)
        {
            LeanTween.move(toMove.gameObject, new Vector3(0.5f, 0, 0), speedPieceMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate
        }
        else
        {
            LeanTween.move(toMove.gameObject, new Vector3(toMove.transform.localPosition.x, 0, 0), speedPieceMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate
        }
    }

    public void ActivateParticleEffectsMiddle(GameObject midPiece)
    {
        Instantiate(midPieceParticle, midPiece.transform);
    }

    public void PullIn(SubPiece toMove)
    {
        LeanTween.move(toMove.gameObject, GameManager.Instance.gameBoard.transform.position, speedPieceMove).setEase(LeanTweenType.easeInOutQuad); // animate
    }
}
