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

    public int clipCount;

    [Serializable]
    public class ColorsAndMats
    {
        public PieceColor mat;
        public Material[] colorMats;
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

    public void RepopulateLatestClip() //// In case player clicked the hell no option after placing last piece
    {
        latestPiece.transform.parent.GetComponent<Cell>().RemovePiece();

        latestPiece.transform.SetParent(emptyClip);
        latestPiece.localPosition = originalPiecePos;
        latestPiece.localRotation = originalPieceRot;

        GameManager.Instance.currentFilledCellCount--;
    }
}
