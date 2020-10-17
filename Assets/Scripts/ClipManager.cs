using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipManager : MonoBehaviour
{
    public Transform[] slots;
    public GameObject piece;

    public Color[] gameColors;
    public Sprite[] gameSymbols;

    private void Awake()
    {
        GameManager.Instance.clipManager = this;
    }

    public void Init()
    {
        foreach (Transform s in slots)
        {
            PopulateSlot(s);
        }
    }

    public void PopulateSlot(Transform s)
    {
        GameObject go = Instantiate(piece, s);
        Piece p = go.GetComponent<Piece>();

        p.SetPiece();
    }
}
