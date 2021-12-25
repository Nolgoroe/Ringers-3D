using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClipManager : MonoBehaviour
{
    public Transform[] slots;
    public GameObject piece;
    public Transform emptyClip;
    public Transform latestPiece;

    public Vector3 originalPiecePos;
    public Quaternion originalPieceRot;

    //public Material[] gameColors;
    //public Texture2D[] gameSymbols;

    public ColorsAndMats[] colorsToMats;
    public SymbolToMat[] symbolToMat;

    public int clipCount;

    public Material generalPieceMat;

    public Color darkTintedColor;

    public Vector3[] piecesDealPositionsOut;
    public float delayClipMove, timeToAnimateMove, WaitTimeBeforeIn, delayDarkenClip, timeToDarkenClip;


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
            clipCount++;
            IGSP.ResetValues();
        }
    }
    public IEnumerator DragonflyCrossSpecial(InGameSpecialPowerUp IGSP)
    {
        int topOrBottom = UnityEngine.Random.Range(1, 3);
        IGSP.ResetValues();

        if (topOrBottom == 1)
        {
            bool up = true;

            for (int i = 0; i < clipCount; i++)
            {
                Piece p = slots[i].transform.GetChild(0).GetComponent<Piece>();

                if (up) 
                {
                    //left wing
                    p.leftChild.symbolOfPiece = IGSP.SymbolNeeded;
                }
                else
                {
                    //right wing

                    p.rightChild.symbolOfPiece = IGSP.SymbolNeeded;
                }

                up = !up;

                p.leftChild.RefreshPiece();
                p.rightChild.RefreshPiece();

                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            bool up = false;

            for (int i = 0; i < clipCount; i++)
            {
                Piece p = slots[i].transform.GetChild(0).GetComponent<Piece>();

                if (up)
                {
                    //left wing
                    p.leftChild.symbolOfPiece = IGSP.SymbolNeeded;
                }
                else
                {
                    //right wing

                    p.rightChild.symbolOfPiece = IGSP.SymbolNeeded;
                }

                up = !up;
                p.leftChild.RefreshPiece();
                p.rightChild.RefreshPiece();

                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return null;
    }

    public void RepopulateLatestClip() //// In case player clicked the hell no option after placing last piece
    {
        latestPiece.transform.parent.GetComponent<Cell>().RemovePiece(false, false);

        latestPiece.transform.SetParent(emptyClip);
        latestPiece.localPosition = originalPiecePos;
        latestPiece.localRotation = originalPieceRot;

        GameManager.Instance.currentFilledCellCount--;
    }

    public IEnumerator DealAnimation()
    {
        UIManager.Instance.dealButton.interactable = false;
        StartCoroutine(DeactivateClip(clipCount - 1));

        for (int i = 0; i < clipCount; i++)
        {
            GameObject toMove = slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, piecesDealPositionsOut[i], timeToAnimateMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate


            yield return new WaitForSeconds(delayClipMove);
        }

        yield return new WaitForSeconds(WaitTimeBeforeIn);
        DealAnimClipLogic();
        clipCount--;

        for (int i = clipCount -1; i > -1; i--)
        {
            GameObject toMove = slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, originalPiecePos, timeToAnimateMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate
            yield return new WaitForSeconds(delayClipMove);
        }
        UIManager.Instance.dealButton.interactable = true;
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

    public void ReactivateClip(int index)
    {
        slots[index].GetComponent<SpriteRenderer>().color = Color.white;
    }
}
