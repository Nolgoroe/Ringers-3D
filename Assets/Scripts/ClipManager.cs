using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipManager : MonoBehaviour
{
    public Transform[] slots;
    public GameObject piece;

    public Color[] gameColors;
    public Sprite[] gameSymbols;

    public int clipCount;
    private void Awake()
    {
        GameManager.Instance.clipManager = this;
    }

    public void Init()
    {
        clipCount = slots.Length;
        foreach (Transform s in slots)
        {
            PopulateSlot(s);
        }
    }

    public void PopulateSlot(Transform s)
    {
        GameObject go = Instantiate(piece, s);
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
            PopulateSlot(slots[i]);
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
            PopulateSlot(slots[i]);
        }
    }
}
