using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClipManager : MonoBehaviour
{
    public Transform[] slots;
    public GameObject piece;
    public GameObject corruptedPiece;
    public Transform emptyClip;
    public Transform latestPiece;

    public Vector3 originalPiecePos;
    public Quaternion originalPieceRot;

    //public Material[] gameColors;
    //public Texture2D[] gameSymbols;

    public ColorsAndMats[] colorsToMats;
    public Texture[] corruptedColorsToMatsLeft;
    public Texture[] corruptedColorsToMatsRight;
    public SymbolToMat[] symbolToMat;

    //public ColorsAndMats[] corruptedColorsToMats;
    //public SymbolToMat[] corruptedSymbolToMat;

    public int clipCount;

    public Material generalPieceMat;

    public Color darkTintedColor;

    public Vector3[] piecesDealPositionsOut;
    public float delayClipMove, timeToAnimateMove, WaitTimeBeforeIn, delayDarkenClip, delayBrightenClip, timeToDarkenClip, timeToBrightenClip;

    public Vector3 pieceScaleOnBoard;

    public float delaySpecialPowerFirefly;
    [Serializable]
    public class ColorsAndMats
    {
        public PieceColor matColor;
        //public Material[] colorMats;
        public Texture[] colorTex;
    }

    [Serializable]
    public class SymbolToMat
    {
        public PieceSymbol mat;
        //public Material symbolMat;
        public Texture symbolTex;
    }

    private void Awake()
    {
        GameManager.Instance.clipManager = this;
    }

    public void Init()
    {
        clipCount = slots.Length;
        int testnum = 0;

        foreach (Transform s in slots)
        {
            testnum++;
            PopulateSlot(s,testnum);
        }

        originalPiecePos = piece.transform.position;
        originalPieceRot = piece.transform.rotation;
    }


    public void PopulateSlot(Transform s, int testnum)
    {
        GameObject go = Instantiate(piece, s);
        go.name = "Piece" + " " + testnum;
        Piece p = go.GetComponent<Piece>();
        p.SetPieces();
    }

    public void RerollSlotPieceData(Piece p)
    {
        p.SetPieces();
    }

    public void RefreshSlots()
    {
        if(slots[clipCount - 1].childCount > 0)
        {
            for (int i = 0; i < slots[clipCount - 1].childCount; i++)
            {
                if (slots[clipCount - 1].GetChild(i).CompareTag("MainPiece"))
                {
                    Destroy(slots[clipCount - 1].GetChild(i).gameObject);
                }
            }
        }

        for (int i = 0; i < clipCount; i++)
        {
            Piece p = slots[i].GetComponentInChildren<Piece>();
            RerollSlotPieceData(p);
        }
    }
    public void ExtraDealSlots()
    {
        foreach (Transform t in slots)
        {
            if (t.childCount > 0)
            {
                Destroy(t.GetChild(0).gameObject);
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            PopulateSlot(slots[i], i);
        }
    }
    public void ExtraDealSlotsBadgerSpecial(InGameSpecialPowerUp IGSP)
    {
        if(clipCount < 4)
        {
            PopulateSlot(slots[clipCount], clipCount);
            StartCoroutine(ActivateClip(clipCount));
            clipCount++;
            IGSP.ResetValues();
        }
    }
    public IEnumerator DragonflyCrossSpecial(InGameSpecialPowerUp IGSP)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].isAnimalSymbolCollectionPhase)
            {
                StartCoroutine(TutorialSequence.Instacne.IncrementCurrentPhaseInSequence());
            }
        }

        int topOrBottom = UnityEngine.Random.Range(1, 3);
        IGSP.ResetValues();

        if (topOrBottom == 1)
        {
            bool up = true;

            for (int i = 0; i < clipCount; i++)
            {
                Piece p = slots[i].transform.GetComponentInChildren<Piece>();

                if (up) 
                {
                    //left wing
                    p.leftChild.symbolOfPiece = PieceSymbol.Joker;
                    p.leftChild.colorOfPiece = PieceColor.Joker;
                    p.leftChild.SetPieceAsJoker();
                }
                else
                {
                    //right wing

                    p.rightChild.symbolOfPiece = PieceSymbol.Joker;
                    p.rightChild.colorOfPiece = PieceColor.Joker;
                    p.rightChild.SetPieceAsJoker();

                }

                up = !up;


                yield return new WaitForSeconds(delaySpecialPowerFirefly);
            }
        }
        else
        {
            bool up = false;

            for (int i = 0; i < clipCount; i++)
            {
                Piece p = slots[i].transform.GetComponentInChildren<Piece>();

                if (up)
                {
                    //left wing
                    p.leftChild.symbolOfPiece = PieceSymbol.Joker;
                    p.leftChild.colorOfPiece = PieceColor.Joker;
                    p.leftChild.SetPieceAsJoker();

                }
                else
                {
                    //right wing

                    p.rightChild.symbolOfPiece = PieceSymbol.Joker;
                    p.rightChild.colorOfPiece = PieceColor.Joker;
                    p.rightChild.SetPieceAsJoker();
                }

                up = !up;


                yield return new WaitForSeconds(delaySpecialPowerFirefly);
            }
        }

        yield return null;
    }

    public void RepopulateLatestClip() //// In case player clicked the hell no option after placing last piece
    {
        if (latestPiece.GetComponent<Piece>())
        {
            Piece p = latestPiece.GetComponent<Piece>();

            if (p.rightChild.relevantSlice)
            {
                Slice relevantSlice = p.rightChild.relevantSlice;

                if (relevantSlice.isLock)
                {
                    relevantSlice.lockSpriteHeighlightAnim.gameObject.SetActive(false);
                }
            }

            if (p.leftChild.relevantSlice)
            {
                Slice relevantSlice = p.leftChild.relevantSlice;

                if (relevantSlice.isLock)
                {
                    relevantSlice.lockSpriteHeighlightAnim.gameObject.SetActive(false);
                }
            }

            if (p.transform.parent.GetComponent<Cell>())
            {
                Cell c = p.transform.parent.GetComponent<Cell>();

                c.RemovePiece(false);
            }
            else
            {
                Debug.LogError("No supposed to be like this - this is supposed to be a cell!");
            }

            p.transform.SetParent(emptyClip);
            p.transform.localPosition = originalPiecePos;
            p.transform.localRotation = originalPieceRot;

            p.partOfBoard = false;
            p.isLocked = false;
            p.isTutorialLocked = false;

            GameManager.Instance.currentFilledCellCount--;
        }
        else
        {
            Debug.LogError("No supposed to be like this - this is supposed to be a piece!");
        }

    }

    public IEnumerator DealAnimation()
    {
        UIManager.Instance.dealButton.interactable = false;
        StartCoroutine(DeactivateClip(clipCount - 1));

        //switch (clipCount)
        //{
        //    case 4:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimFourPieces);
        //        break;
        //    case 3:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimThreePieces);
        //        break;
        //    case 2:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimTwoPieces);
        //        break;
        //    case 1:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimOnePiece);
        //        break;
        //    default:
        //        break;
        //}

        //yield return new WaitForSeconds(1f); /// TEMPORARY

        for (int i = 0; i < clipCount; i++)
        {
            GameObject toMove = slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, piecesDealPositionsOut[i], timeToAnimateMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate

            SoundManager.Instance.PlaySound(Sounds.PieceMoveDeal);

            yield return new WaitForSeconds(delayClipMove);
        }


        yield return new WaitForSeconds(WaitTimeBeforeIn);
        DealAnimClipLogic();

        clipCount--;


        //switch (clipCount)
        //{
        //    case 4:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimFourPieces);
        //        break;
        //    case 3:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimThreePieces);
        //        break;
        //    case 2:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimTwoPieces);
        //        break;
        //    case 1:
        //        SoundManager.Instance.PlaySound(Sounds.DealAnimOnePiece);
        //        break;
        //    default:
        //        break;
        //}

        //yield return new WaitForSeconds(0.8f); /// TEMPORARY


        for (int i = clipCount -1; i > -1; i--)
        {
            GameObject toMove = slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, originalPiecePos, timeToAnimateMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate


            Invoke("playReturnPiecePlaceSound", timeToAnimateMove - 0.25f);

            yield return new WaitForSeconds(delayClipMove);

        }
        UIManager.Instance.dealButton.interactable = true;
    }

    void playReturnPiecePlaceSound()
    {
        SoundManager.Instance.PlaySound(Sounds.PieceMoveDeal);
    }

    public void DealAnimClipLogic()
    {
        RefreshSlots();
    }
    public IEnumerator DeactivateClip(int index)
    {
        yield return new WaitForSeconds(delayDarkenClip);

        Color fromColor = slots[index].GetComponent<SpriteRenderer>().color;
        Color toColor = darkTintedColor;

        LeanTween.value(slots[index].gameObject, fromColor, toColor, timeToDarkenClip).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
        {
            SpriteRenderer sr = slots[index].gameObject.GetComponent<SpriteRenderer>();
            Color newColor = sr.color;
            newColor = Color.Lerp(fromColor, toColor, val);
            sr.color = newColor;
        });
    }

    public IEnumerator ActivateClip(int index)
    {
        yield return new WaitForSeconds(delayBrightenClip);

        Color fromColor = slots[index].GetComponent<SpriteRenderer>().color;
        Color toColor = Color.white;

        LeanTween.value(slots[index].gameObject, fromColor, toColor, timeToBrightenClip).setEase(LeanTweenType.linear).setOnUpdate((float val) =>
        {
            SpriteRenderer sr = slots[index].gameObject.GetComponent<SpriteRenderer>();
            Color newColor = sr.color;
            newColor = Color.Lerp(fromColor, toColor, val);
            sr.color = newColor;
        });
    }

    public void ReactivateClip(int index)
    {
        slots[index].GetComponent<SpriteRenderer>().color = Color.white;
    }
}
