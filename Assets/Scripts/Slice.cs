using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    public PieceColor sliceColor;
    public PieceSymbol sliceSymbol;
    public SliceCatagory sliceCatagory;
    public LootPacks lootPack;
    public GameObject lockPrefab, diamondPrefab;

    public GameObject child;

    public bool isLock;
    public bool isLoot;
    public bool isLimiter;
    public bool fulfilledCondition;
    public bool isKey;

    public int sliceIndex;
    public void SetData(SliceCatagory sc, bool islocking, bool isLooting, bool isLimiting, LootPacks lp)
    {
        SpriteRenderer sr = child.GetComponent<SpriteRenderer>();

        int sliceCatagorycount = System.Enum.GetValues(typeof(SliceCatagory)).Length;
        int sliceSymbolcount = System.Enum.GetValues(typeof(PieceSymbol)).Length;
        int sliceColorcount = System.Enum.GetValues(typeof(PieceColor)).Length;

        isLoot = isLooting;
        isLock = islocking;
        isLimiter = isLimiting;
        sliceCatagory = sc;
        lootPack = lp;

        switch (sliceCatagory)
        {
            case SliceCatagory.Shape:
                sliceSymbol = PieceSymbol.None;
                sr.sprite = GameManager.Instance.sliceManager.sliceSymbolDict[sliceSymbol];
                break;
            case SliceCatagory.Color:
                sliceColor = PieceColor.None;
                sr.sprite = GameManager.Instance.sliceManager.sliceColorDict[sliceColor];

                break;
            case SliceCatagory.SpecificShape:
                sliceSymbol = (PieceSymbol)Random.Range(0, sliceSymbolcount - 2);
                sr.sprite = GameManager.Instance.sliceManager.sliceSymbolDict[sliceSymbol];
                break;
            case SliceCatagory.SpecificColor:
                sliceColor = (PieceColor)Random.Range(0, sliceColorcount - 2);
                sr.sprite = GameManager.Instance.sliceManager.sliceColorDict[sliceColor];
                break;
            default:
                break;
        }

        diamondPrefab.SetActive(isLoot);
        lockPrefab.SetActive(isLock);

        if (isLimiter)
        {
            diamondPrefab.GetComponent<SpriteRenderer>().color = Color.red;
            lockPrefab.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void ResetDate()
    {
        sliceColor = PieceColor.None;
        sliceSymbol = PieceSymbol.None;
        sliceCatagory = SliceCatagory.None;
        isLock = false;
        isLoot = false;
        isLimiter = false;
        fulfilledCondition = false;
    }
}
