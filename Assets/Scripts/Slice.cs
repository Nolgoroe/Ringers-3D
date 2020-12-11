using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    public PieceColor sliceColor;
    public PieceSymbol sliceSymbol;
    public SliceCatagory sliceCatagory;
    public LootPacks lootPack;
    public GameObject lootIcon;

    public GameObject child;

    public bool isLock;
    public bool isLoot;
    public bool isLimiter;
    public bool fulfilledCondition;
    public bool isKey;

    public int sliceIndex;

    public void SetData(Transform parent, SliceCatagory sc, bool islocking, bool isLooting, bool isLimiting, LootPacks lp)
    {
        //SpriteRenderer sr = child.GetComponent<SpriteRenderer>();

        int sliceCatagorycount = System.Enum.GetValues(typeof(SliceCatagory)).Length;
        int sliceSymbolcount = System.Enum.GetValues(typeof(PieceSymbol)).Length;
        int sliceColorcount = System.Enum.GetValues(typeof(PieceColor)).Length;

        isLoot = isLooting;
        isLock = islocking;
        isLimiter = isLimiting;
        sliceCatagory = sc;
        lootPack = lp;

        if (!isLock && !isLimiter)
        {
            InstantiateLootSlice(sliceSymbolcount, sliceColorcount);
        }

        if (isLock && !isLimiter)
        {
            InstantiateLootLockSlice(sliceSymbolcount, sliceColorcount);
        }

        if (isLimiter)
        {
            InstantiateLootLockLimiterSlice(sliceSymbolcount, sliceColorcount);
        }

        lootIcon.SetActive(isLoot);

        lootIcon.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.sliceManager.lootToIcon[lp];

        if (isLimiter)
        {
            lootIcon.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void InstantiateLootSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        GameObject go = Instantiate(GameManager.Instance.sliceManager.lootSlicePrefab, transform);

        SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();

        child = go;
        switch (sliceCatagory)
        {
            case SliceCatagory.Shape:
                sliceSymbol = PieceSymbol.None;
                sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[PieceSymbol.None];
                //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                break;
            case SliceCatagory.Color:
                sliceColor = PieceColor.None;
                sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                break;
            case SliceCatagory.SpecificShape:
                sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
                sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                break;
            case SliceCatagory.SpecificColor:
                sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
                sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];
                break;
            default:
                break;
        }
    }

    public void InstantiateLootLockSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        GameObject go = Instantiate(GameManager.Instance.sliceManager.lootLockSlicePrefab, transform);

        SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
        child = go;

        switch (sliceCatagory)
        {
            case SliceCatagory.Shape:
                sliceSymbol = PieceSymbol.None;
                sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[PieceSymbol.None];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
                break;
            case SliceCatagory.Color:
                sliceColor = PieceColor.None;
                sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];

                break;
            case SliceCatagory.SpecificShape:
                sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
                sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
                break;
            case SliceCatagory.SpecificColor:
                sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
                sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];
                break;
            default:
                break;
        }
    }

    public void InstantiateLootLockLimiterSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        GameObject go = Instantiate(GameManager.Instance.sliceManager.lootLockLimiterSlicePrefab, transform);

        SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
        child = go;

        switch (sliceCatagory)
        {
            case SliceCatagory.Shape:
                sliceSymbol = PieceSymbol.None;
                sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[PieceSymbol.None];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
                break;
            case SliceCatagory.Color:
                sliceColor = PieceColor.None;
                sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];

                break;
            case SliceCatagory.SpecificShape:
                sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
                sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
                break;
            case SliceCatagory.SpecificColor:
                sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
                sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];
                break;
            default:
                break;
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
