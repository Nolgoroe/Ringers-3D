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

    [Serializable]
    public class ColorsAndMats
    {
        public PieceColor matColor;
        public Material[] colorMats;
    }

    [Serializable]
    public class SymbolToMat
    {
        public PieceSymbol mat;
        //public Material symbolMat;
        public Material symbolMat;
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

    public void RefreshSlots()
    {
        foreach (Transform t in slots)
        {
            if (t.childCount > 0)
            {
                Destroy(t.GetChild(0).gameObject);
            }
        }

        for (int i = 0; i < clipCount; i++)
        {
            PopulateSlot(slots[i], i);
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
        latestPiece.transform.parent.GetComponent<Cell>().RemovePiece();

        latestPiece.transform.SetParent(emptyClip);
        latestPiece.localPosition = originalPiecePos;
        latestPiece.localRotation = originalPieceRot;

        GameManager.Instance.currentFilledCellCount--;
    }
}
