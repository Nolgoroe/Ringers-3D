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

    public Animator anim;
    public List<Cell> connectedCells;
    public void SetSliceData(Transform parent, SliceCatagory sc, bool islocking, bool isLooting, bool isLimiting)
    {
        int sliceCatagorycount = System.Enum.GetValues(typeof(SliceCatagory)).Length;
        int sliceSymbolcount = System.Enum.GetValues(typeof(PieceSymbol)).Length;
        int sliceColorcount = System.Enum.GetValues(typeof(PieceColor)).Length;

        isLoot = isLooting;
        isLock = islocking;
        isLimiter = isLimiting;
        sliceCatagory = sc;

        InstantiateSlice(sliceSymbolcount, sliceColorcount);

        connectedCells.Add(CheckIntRangeSliceCells(sliceIndex)); ///// CHECK TO SEE IF CAN DO BETTER
        connectedCells.Add(CheckIntRangeSliceCells(sliceIndex-1));  ///// CHECK TO SEE IF CAN DO BETTER

        if (isLock)
        {
            foreach (Cell c in connectedCells)
            {
                if (c.cellIndex == sliceIndex)
                {
                    c.lockSpriteCellLeft.gameObject.SetActive(true);
                }
                else
                {
                    c.lockSpriteCellRight.gameObject.SetActive(true);
                }
            }
        }
        //if (!isLock && !isLimiter)
        //{
        //    InstantiateLootSlice(sliceSymbolcount, sliceColorcount);
        //}

        //if (isLock && !isLimiter)
        //{
        //    InstantiateLootLockSlice(sliceSymbolcount, sliceColorcount);
        //}

        //if (isLimiter)
        //{
        //    InstantiateLootLockLimiterSlice(sliceSymbolcount, sliceColorcount);
        //}


        //if (isLimiter)
        //{
        //    //lootIcon.GetComponent<SpriteRenderer>().color = Color.red;
        //}
    }
    public void SetSliceLootData(LootPacks lp)
    {
        lootPack = lp;

        //lootIcon.SetActive(isLoot);

        //lootIcon.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.sliceManager.lootToIcon[lp];
    }


    public void InstantiateSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        //GameObject go = Instantiate(GameManager.Instance.sliceManager.lootSlicePrefab, transform);
        if (!isLimiter)
        {
            InstantiateNonLimiterSlice(pieceSymbolEnumCount, pieceColorEnumCount);
        }
        else
        {
            InstantiateLimiterSlice(pieceSymbolEnumCount, pieceColorEnumCount);
        }
    }

    private void InstantiateLimiterSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        GameObject go = Instantiate(GameManager.Instance.sliceManager.slicePrefabLimiter, transform);
        Renderer[] rend = go.transform.GetComponentsInChildren<Renderer>();
        anim = go.GetComponent<Animator>();

        //SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (GameManager.Instance.currentLevel.isTutorial)
        {
            switch (sliceCatagory)
            {
                case SliceCatagory.Shape:
                    sliceSymbol = PieceSymbol.None;
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];

                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]);
                    }

                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.None;
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];

                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]);
                    }

                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    break;
                case SliceCatagory.SpecificShape:
                    sliceSymbol = GameManager.Instance.copyOfSpecificSliceSymbolsTutorial[0];
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];

                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]);
                    }

                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];

                    GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.RemoveAt(0);
                    break;
                case SliceCatagory.SpecificColor:
                    sliceColor = GameManager.Instance.copyOfSpecificSliceColorsTutorial[0];
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    // sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];

                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]);
                    }

                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    GameManager.Instance.copyOfSpecificSliceColorsTutorial.RemoveAt(0);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (sliceCatagory)
            {
                case SliceCatagory.Shape:
                    sliceSymbol = PieceSymbol.None;
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];

                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]);
                    }
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.None;
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];

                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]);
                    }
                    break;
                case SliceCatagory.SpecificShape:
                    sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];
                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]);
                    }
                    break;
                case SliceCatagory.SpecificColor:
                    sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];
                    foreach (Renderer r in rend)
                    {
                        r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void InstantiateNonLimiterSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        GameObject go = Instantiate(GameManager.Instance.sliceManager.slicePrefab, transform);
        Renderer rend = go.transform.GetChild(0).GetComponent<Renderer>();
        Material[] matArray = rend.materials;
        anim = go.GetComponent<Animator>();
        rend.materials = matArray;

        //SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
        child = go;

        if (GameManager.Instance.currentLevel.isTutorial)
        {
            switch (sliceCatagory)
            {
                case SliceCatagory.Shape:
                    sliceSymbol = PieceSymbol.None;
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.None;
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    break;
                case SliceCatagory.SpecificShape:
                    sliceSymbol = GameManager.Instance.copyOfSpecificSliceSymbolsTutorial[0];
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];

                    GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.RemoveAt(0);
                    break;
                case SliceCatagory.SpecificColor:
                    sliceColor = GameManager.Instance.copyOfSpecificSliceColorsTutorial[0];
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    GameManager.Instance.copyOfSpecificSliceColorsTutorial.RemoveAt(0);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (sliceCatagory)
            {
                case SliceCatagory.Shape:
                    sliceSymbol = PieceSymbol.None;
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.None;
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    break;
                case SliceCatagory.SpecificShape:
                    if (GameManager.Instance.currentLevel.RandomSlices)
                    {
                        sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
                        //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                        matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                        //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    }
                    else
                    {
                        if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
                        {
                            int rand = Random.Range(0, GameManager.Instance.currentLevel.levelAvailablesymbols.Length);
                            sliceSymbol = GameManager.Instance.currentLevel.levelAvailablesymbols[rand];

                            matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                        }
                        else
                        {
                            sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);

                            matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                        }
                    }
                    break;
                case SliceCatagory.SpecificColor:
                    if (GameManager.Instance.currentLevel.RandomSlices)
                    {
                        sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
                        //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                        matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                        //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                        //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];
                    }
                    else
                    {
                        if(GameManager.Instance.currentLevel.levelAvailableColors.Length > 0)
                        {
                            int rand = Random.Range(0, GameManager.Instance.currentLevel.levelAvailableColors.Length);
                            sliceColor = GameManager.Instance.currentLevel.levelAvailableColors[rand];

                            matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                        }
                        else
                        {
                            sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);

                            matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

    }
    //public void InstantiateLootSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    //{
    //    //GameObject go = Instantiate(GameManager.Instance.sliceManager.lootSlicePrefab, transform);
    //    GameObject go = Instantiate(GameManager.Instance.sliceManager.slicePrefab, transform);
    //    Renderer rend = go.GetComponent<Renderer>();
    //    Material[] matArray = rend.materials;
    //    rend.materials = matArray;

    //    //SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();

    //    child = go;
    //    switch (sliceCatagory)
    //    {
    //        case SliceCatagory.Shape:
    //            sliceSymbol = PieceSymbol.None;
    //            //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
    //            matArray[1].SetTexture("_BaseMap", GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol]);
    //            //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
    //            break;
    //        case SliceCatagory.Color:
    //            sliceColor = PieceColor.None;
    //            //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
    //            matArray[1].SetTexture("_BaseMap", GameManager.Instance.sliceManager.piececolorToSprite[sliceColor]);
    //            //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

    //            break;
    //        case SliceCatagory.SpecificShape:
    //            sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
    //            //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
    //            matArray[1].SetTexture("_BaseMap", GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol]);
    //            //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
    //            break;
    //        case SliceCatagory.SpecificColor:
    //            sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
    //            //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
    //            matArray[1].SetTexture("_BaseMap", GameManager.Instance.sliceManager.piececolorToSprite[sliceColor]);
    //            //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //public void InstantiateLootLockSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    //{
    //    //GameObject go = Instantiate(GameManager.Instance.sliceManager.lootLockSlicePrefab, transform);
    //    GameObject go = Instantiate(GameManager.Instance.sliceManager.slicePrefab, transform);
    //    Renderer rend = go.GetComponent<Renderer>();
    //    Material[] matArray = rend.materials;
    //    rend.materials = matArray;

    //    //SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
    //    child = go;

    //    switch (sliceCatagory)
    //    {
    //        case SliceCatagory.Shape:
    //            sliceSymbol = PieceSymbol.None;
    //            sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[PieceSymbol.None];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
    //            break;
    //        case SliceCatagory.Color:
    //            sliceColor = PieceColor.None;
    //            sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
    //            //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];

    //            break;
    //        case SliceCatagory.SpecificShape:
    //            sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
    //            sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
    //            break;
    //        case SliceCatagory.SpecificColor:
    //            sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
    //            sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
    //            //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //public void InstantiateLootLockLimiterSlice(int pieceSymbolEnumCount, int pieceColorEnumCount)
    //{
    //    //GameObject go = Instantiate(GameManager.Instance.sliceManager.lootLockLimiterSlicePrefab, transform);
    //    GameObject go = Instantiate(GameManager.Instance.sliceManager.slicePrefab, transform);
    //    Renderer rend = go.GetComponent<Renderer>();
    //    Material[] matArray = rend.materials;
    //    rend.materials = matArray;

    //    //SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
    //    child = go;

    //    switch (sliceCatagory)
    //    {
    //        case SliceCatagory.Shape:
    //            sliceSymbol = PieceSymbol.None;
    //            sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[PieceSymbol.None];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
    //            break;
    //        case SliceCatagory.Color:
    //            sliceColor = PieceColor.None;
    //            sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
    //            //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];

    //            break;
    //        case SliceCatagory.SpecificShape:
    //            sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
    //            sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceSymbolSpritesDict[sliceSymbol];
    //            break;
    //        case SliceCatagory.SpecificColor:
    //            sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
    //            sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
    //            //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
    //            //sr.sprite = GameManager.Instance.sliceManager.lootLockSliceColorDict[sliceColor];
    //            break;
    //        default:
    //            break;
    //    }
    //}

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

    public Cell CheckIntRangeSliceCells(int num)
    {
        if (num < 0)
        {
            return ConnectionManager.Instance.cells[ConnectionManager.Instance.cells.Count - 1];
        }
        else if (num > ConnectionManager.Instance.cells.Count)
        {
            return ConnectionManager.Instance.cells[0];
        }
        else
        {
            return ConnectionManager.Instance.cells[num];
        }
    }
}
