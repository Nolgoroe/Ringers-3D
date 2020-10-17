using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    public PieceColor sliceColor;
    public PieceSymbol sliceSymbol;
    public SliceCatagory sliceCatagory;

    public GameObject lockPrefab, diamondPrefab;

    public GameObject child;

    public bool isLock;
    public bool isLoot;

    public void SetData(SliceCatagory sc, bool islocking, bool isLooting)
    {
        SpriteRenderer sr = child.GetComponent<SpriteRenderer>();

        int sliceCatagorycount = System.Enum.GetValues(typeof(SliceCatagory)).Length;
        int sliceSymbolcount = System.Enum.GetValues(typeof(PieceSymbol)).Length;
        int sliceColorcount = System.Enum.GetValues(typeof(PieceColor)).Length;

        isLoot = isLooting;
        isLock = islocking;
        sliceCatagory = sc;

        switch (sliceCatagory)
        {
            case SliceCatagory.Shape:
                sliceSymbol = (PieceSymbol)sliceSymbolcount - 2;
                sr.sprite = GameManager.Instance.sliceManager.sliceSymbolDict[sliceSymbol];
                break;
            case SliceCatagory.Color:
                sliceColor = (PieceColor)sliceColorcount - 2;
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
    }
}
