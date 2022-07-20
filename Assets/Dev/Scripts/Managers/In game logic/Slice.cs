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

    public bool hasSlice;
    public bool isLock;
    public bool isLoot;
    public bool isLimiter;
    public bool fulfilledCondition;
    public bool isKey;

    public int sliceIndex;

    public Animator anim;
    public List<Cell> connectedCells;
    public Animator lockSpriteAnim;
    public Animator lockSpriteHeighlightAnim;


    public void SetSliceData(Transform parent, SliceCatagory sc, bool islocking, bool isLooting, bool isLimiting)
    {
        hasSlice = true;

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
            //foreach (Cell c in connectedCells) // old lock system
            //{
            //    if (c.cellIndex == sliceIndex)
            //    {
            //        c.lockSpriteCellLeft.gameObject.SetActive(true);
            //    }
            //    else
            //    {
            //        c.lockSpriteCellRight.gameObject.SetActive(true);
            //    }
            //}

            lockSpriteAnim.gameObject.SetActive(true);
            GameManager.Instance.gameBoard.GetComponent<SliceManager>().activeLocksLockAnims.Add(lockSpriteAnim.gameObject);
        }
        else
        {
            lockSpriteAnim.gameObject.SetActive(false);
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
        //Renderer[] rend = go.transform.GetComponentsInChildren<Renderer>();
        SpriteRenderer[] rend = go.transform.GetComponentsInChildren<SpriteRenderer>();
        //anim = go.GetComponent<Animator>();
        child = go;

        SetRendereDataLimiter(rend, pieceSymbolEnumCount, pieceColorEnumCount);

        if (!GameManager.Instance.currentLevel.allowRepeatSlices)
        {
            if (sliceCatagory == SliceCatagory.SpecificColor || sliceCatagory == SliceCatagory.SpecificShape)
            {
                bool repetingSlices = CheckRepeatingSlices(this);

                int tries = 0;

                while (repetingSlices)
                {
                    tries++;

                    if(tries == 1000)
                    {
                        Debug.LogError("There are repeat slices but code won't allow");
                        break;
                    }

                    SetRendereDataLimiter(rend, pieceSymbolEnumCount, pieceColorEnumCount);
                    repetingSlices = CheckRepeatingSlices(this);
                }
            }
        }
        //SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
    }

    public void SetRendereDataLimiter(SpriteRenderer[] rend, int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        if (GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.Count > 0 || GameManager.Instance.copyOfSpecificSliceColorsTutorial.Count > 0)
        {
            switch (sliceCatagory)
            {
                case SliceCatagory.Shape:
                    sliceSymbol = PieceSymbol.None;
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];

                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]); // this was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];
                    }

                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.General;
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];

                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]);
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];
                    }

                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    break;
                case SliceCatagory.SpecificShape:
                    if (GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.Count > 0)
                    {
                        sliceSymbol = GameManager.Instance.copyOfSpecificSliceSymbolsTutorial[0];
                    }
                    else
                    {
                        int random = Random.Range(0, GameManager.Instance.currentLevel.levelAvailablesymbols.Length);
                        sliceSymbol = GameManager.Instance.currentLevel.levelAvailablesymbols[random];

                    }
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];

                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]); // this was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];
                    }

                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];

                    if (GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.Count > 0)
                    {
                        GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.RemoveAt(0);
                    }
                    break;
                case SliceCatagory.SpecificColor:

                    if (GameManager.Instance.copyOfSpecificSliceColorsTutorial.Count > 0)
                    {
                        sliceColor = GameManager.Instance.copyOfSpecificSliceColorsTutorial[0];
                    }
                    else
                    {
                        int random = Random.Range(0, GameManager.Instance.currentLevel.levelAvailableColors.Length);
                        sliceColor = GameManager.Instance.currentLevel.levelAvailableColors[random];
                    }
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    // sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];

                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]); // this was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];
                    }

                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[sliceColor];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    if (GameManager.Instance.copyOfSpecificSliceColorsTutorial.Count > 0)
                    {
                        GameManager.Instance.copyOfSpecificSliceColorsTutorial.RemoveAt(0);
                    }
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

                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]); // this was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];
                    }
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.General;
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];

                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]);// This was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];
                    }
                    break;
                case SliceCatagory.SpecificShape:

                    if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
                    {
                        int random = Random.Range(0, GameManager.Instance.currentLevel.levelAvailablesymbols.Length);
                        sliceSymbol = GameManager.Instance.currentLevel.levelAvailablesymbols[random];
                    }
                    else
                    {
                        sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);
                    }
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];
                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol]); // This was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSliceSymbolToSprite[sliceSymbol];
                    }
                    break;
                case SliceCatagory.SpecificColor:

                    if (GameManager.Instance.currentLevel.levelAvailableColors.Length > 0)
                    {
                        int random = Random.Range(0, GameManager.Instance.currentLevel.levelAvailableColors.Length);
                        sliceColor = GameManager.Instance.currentLevel.levelAvailableColors[random];
                    }
                    else
                    {
                        sliceColor = (PieceColor)Random.Range(0, pieceSymbolEnumCount - 2);
                    }

                    //sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);
                    //sr.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];
                    //foreach (Renderer r in rend)
                    //{
                    //    r.material.SetTexture("_BaseMap", GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor]); // This was used for 3D limiters
                    //}
                    foreach (SpriteRenderer r in rend)
                    {
                        r.sprite = GameManager.Instance.sliceManager.limiterSlicecolorToSprite[sliceColor];
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
        Material mat = rend.material;
        anim = go.GetComponent<Animator>();
        //rend.material = mat;
        GameObject ropeChild = go.transform.GetChild(1).gameObject;
        ropeChild.SetActive(false);

        //SpriteRenderer sr = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
        child = go;

        SetRendererDataNonLimiter(mat, pieceSymbolEnumCount, pieceColorEnumCount);

        if (!GameManager.Instance.currentLevel.allowRepeatSlices)
        {
            if (sliceCatagory == SliceCatagory.SpecificColor || sliceCatagory == SliceCatagory.SpecificShape)
            {
                bool repetingSlices = CheckRepeatingSlices(this);

                int tries = 0;

                while (repetingSlices)
                {

                    tries++;

                    if (tries == 1000)
                    {
                        Debug.LogError("There are repeat slices but code won't allow");
                        break;
                    }

                    SetRendererDataNonLimiter(mat, pieceSymbolEnumCount, pieceColorEnumCount);
                    repetingSlices = CheckRepeatingSlices(this);
                }
            }
        }
    }

    public void SetRendererDataNonLimiter(Material matArray, int pieceSymbolEnumCount, int pieceColorEnumCount)
    {
        if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
        {
            switch (sliceCatagory)
            {
                case SliceCatagory.Shape:
                    sliceSymbol = PieceSymbol.None;
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.General;
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    break;
                case SliceCatagory.SpecificShape:
                    sliceSymbol = GameManager.Instance.copyOfSpecificSliceSymbolsTutorial[0];
                    //sr.sprite = GameManager.Instance.sliceManager.pieceSymbolToSprite[sliceSymbol];
                    matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];

                    GameManager.Instance.copyOfSpecificSliceSymbolsTutorial.RemoveAt(0);
                    break;
                case SliceCatagory.SpecificColor:
                    sliceColor = GameManager.Instance.copyOfSpecificSliceColorsTutorial[0];
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
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
                    matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceSymbolDict[sliceSymbol];
                    break;
                case SliceCatagory.Color:
                    sliceColor = PieceColor.General;
                    //sr.sprite = GameManager.Instance.sliceManager.piececolorToSprite[sliceColor];
                    matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                    //sr.color = GameManager.Instance.sliceManager.pieceColorToColor[PieceColor.None];
                    //sr.sprite = GameManager.Instance.sliceManager.lootSliceColorDict[sliceColor];

                    break;
                case SliceCatagory.SpecificShape:
                    if (GameManager.Instance.currentLevel.levelAvailablesymbols.Length > 0)
                    {
                        int rand = Random.Range(0, GameManager.Instance.currentLevel.levelAvailablesymbols.Length);
                        sliceSymbol = GameManager.Instance.currentLevel.levelAvailablesymbols[rand];

                        matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    }
                    else
                    {
                        sliceSymbol = (PieceSymbol)Random.Range(0, pieceSymbolEnumCount - 2);

                        matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.sliceSymbolToSprite[sliceSymbol]);
                    }
                    break;
                case SliceCatagory.SpecificColor:
                    if (GameManager.Instance.currentLevel.levelAvailableColors.Length > 0)
                    {
                        int rand = Random.Range(0, GameManager.Instance.currentLevel.levelAvailableColors.Length);
                        sliceColor = GameManager.Instance.currentLevel.levelAvailableColors[rand];

                        matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
                    }
                    else
                    {
                        sliceColor = (PieceColor)Random.Range(0, pieceColorEnumCount - 2);

                        matArray.SetTexture("_BaseMap", GameManager.Instance.sliceManager.slicecolorToSprite[sliceColor]);
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
        sliceColor = PieceColor.General;
        sliceSymbol = PieceSymbol.None;
        sliceCatagory = SliceCatagory.None;
        isLock = false;
        isLoot = false;
        isLimiter = false;
        fulfilledCondition = false;
        hasSlice = false;
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

    public void SetEmpty()
    {
        Renderer rend = child.transform.GetChild(0).GetComponent<Renderer>();
        Material[] matArray = rend.materials;
        matArray[0].SetTexture("_BaseMap", GameManager.Instance.sliceManager.emptyScrollTexture);
    }

    private bool CheckRepeatingSlices(Slice current)
    {
        bool sameSlice = false;

        for (int i = 0; i < GameManager.Instance.sliceManager.sliceSlots.Length; i++)
        {
            Slice compareTo = GameManager.Instance.sliceManager.sliceSlots[i].GetComponent<Slice>();

            if(current != compareTo)
            {
                sameSlice = CompareSlices(current , compareTo);

                if (sameSlice)
                {
                    Debug.Log("SAME SLICES! " + current.name + " " + compareTo.name);
                    return sameSlice;
                }
            }
        }

        return false;
    }

    private bool CompareSlices(Slice currentSlice, Slice compareTo)
    {
        bool sameSlice = false;

        if(currentSlice.sliceSymbol != PieceSymbol.None)
        {
            if(currentSlice.sliceSymbol == compareTo.sliceSymbol)
            {
                sameSlice = true;
            }
        }

        if(currentSlice.sliceColor != PieceColor.General)
        {
            if (currentSlice.sliceColor == compareTo.sliceColor)
            {
                sameSlice = true;
            }
        }

        return sameSlice;
    }
}
