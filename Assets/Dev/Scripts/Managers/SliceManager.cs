using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SliceCatagory
{
    Shape,
    Color,
    SpecificShape,
    SpecificColor,
    None
}

public class SliceManager : MonoBehaviour
{
    //public GameObject lootSlicePrefab;
    //public GameObject lootLockSlicePrefab;
    //public GameObject lootLockLimiterSlicePrefab;
    public GameObject endLevelAnimVFX;
    public GameObject slicePrefab;
    public GameObject slicePrefabLimiter;
    public GameObject particleZonesParent;

    public Texture emptyScrollTexture;

    public Transform[] sliceSlots;

    //public Sprite[] lootSliceSymbolSprites;
    //public Sprite[] lootSliceColorSprites;

    //public Sprite[] lootLockSliceSymbolSprites;
    //public Sprite[] lootLockSliceColorSprites;

    //public Sprite[] lootLockLimiterSliceSymbolSprites;
    //public Sprite[] lootLockLimiterSliceColorSprites;

    //public Sprite[] sliceColors;

    //public Sprite[] sliceLootIcons;

    //public Sprite[] sliceSymbolsSprites;
    public Texture[] sliceColors;
    public Texture[] sliceSymbolsSprites;

    public Sprite[] sliceCompletedSpritesColor;
    public Sprite[] sliceCompletedSpritesShapes;

    //public Texture[] limiterSliceColors; // this was used for 3D limiters
    public Sprite[] limiterSliceColors;
    //public Texture[] limiterSliceSymbolsSprites;
    public Sprite[] limiterSliceSymbolsSprites;

   //public Sprite[] sliceLootIcons;


    //public Dictionary<PieceSymbol, Sprite> lootSliceSymbolDict;
    //public Dictionary<PieceColor, Sprite> lootSliceColorDict;

    //public Dictionary<PieceSymbol, Sprite> lootLockSliceSymbolSpritesDict;
    //public Dictionary<PieceColor, Sprite> lootLockSliceColorDict;

    //public Dictionary<PieceSymbol, Sprite> lootLockLimiterSliceSymbolSpritesDict;
    //public Dictionary<PieceColor, Sprite> lootLockSLimiterliceColorDict;

    //public Dictionary<PieceSymbol, Sprite> pieceSymbolToSprite;
    //public Dictionary<PieceColor, Sprite> piececolorToSprite;
    public Dictionary<PieceSymbol, Texture> sliceSymbolToSprite;
    public Dictionary<PieceColor, Texture> slicecolorToSprite;

    //public Dictionary<PieceSymbol, Texture> limiterSliceSymbolToSprite; // this was used for 3D limiters
    public Dictionary<PieceSymbol, Sprite> limiterSliceSymbolToSprite;
    //public Dictionary<PieceColor, Texture> limiterSlicecolorToSprite; // this was used for 3D limiters
    public Dictionary<PieceColor, Sprite> limiterSlicecolorToSprite;
    //public Dictionary<PieceColor, Material> pieceColorToColor;

    //public Dictionary<LootPacks, Sprite> lootToIcon;

    List<int> possibleSlotsTemp;

    public List<Slice> fullSlices;
    public List<GameObject> activeLocksLockAnims;
    //GameObject go;

    private int fourRandomSlicePos;

    public void Init()
    {
        GameManager.Instance.sliceManager = this;

        endLevelAnimVFX.SetActive(false);

        sliceSymbolToSprite = new Dictionary<PieceSymbol, Texture>();
        slicecolorToSprite = new Dictionary<PieceColor, Texture>();

        limiterSliceSymbolToSprite = new Dictionary<PieceSymbol, Sprite>();
        limiterSlicecolorToSprite = new Dictionary<PieceColor, Sprite>();

        activeLocksLockAnims = new List<GameObject>();

        for (int i = 0; i < sliceSymbolsSprites.Length; i++)
        {
            sliceSymbolToSprite.Add((PieceSymbol)i, sliceSymbolsSprites[i]);
        }

        for (int i = 0; i < sliceColors.Length; i++)
        {
            slicecolorToSprite.Add((PieceColor)i, sliceColors[i]);
        }

        for (int i = 0; i < limiterSliceSymbolsSprites.Length; i++)
        {
            limiterSliceSymbolToSprite.Add((PieceSymbol)i, limiterSliceSymbolsSprites[i]);
        }

        for (int i = 0; i < limiterSliceColors.Length; i++)
        {
            limiterSlicecolorToSprite.Add((PieceColor)i, limiterSliceColors[i]);
        }
    }

    public void SpawnSlices(int numOfSlices)
    {
        if(numOfSlices > 0)
        {
            fullSlices = new List<Slice>();

            possibleSlotsTemp = new List<int>();

            for (int i = 0; i < sliceSlots.Length; i++)
            {
                possibleSlotsTemp.Add(i);
            }

            if (GameManager.Instance.currentLevel.is12PieceRing)
            {
                SpawnTwelveRingSlices(numOfSlices);
            }
            else
            {
                SpawnEightRingSlices(numOfSlices);
            }


            if (GameManager.Instance.currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || GameManager.Instance.currentLevel.isGrindLevel || ServerRelatedData.instance.canRepeatLevels)
            {
                if(GameManager.Instance.currentLevel.RewardBags.Length > 0)
                {
                    for (int i = 0; i < fullSlices.Count; i++)
                    {
                        fullSlices[i].SetSliceLootData(GameManager.Instance.currentLevel.RewardBags[Random.Range(0, GameManager.Instance.currentLevel.RewardBags.Length)]);
                    }
                }
            }

            //}

            /// Distribute Key to a random slice
            //if (ZoneManager.Instance.isKeyLevel)
            //{
            //    int randomSlice = Random.Range(0, fullSlices.Count);
            //    Debug.Log(randomSlice);
            //    fullSlices[randomSlice].isKey = true;
            //    ////// KEY HERE
            //}
        }
    }


    public void SpawnEightRingSlices(int numOfSlices)
    {
        if (GameManager.Instance.currentLevel.RandomSlicePositions)
        {
            int randomPos = Random.Range(0, sliceSlots.Length);

            fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

            if (numOfSlices == 2)
            {
                for (int i = 1; i < numOfSlices; i++)
                {
                    randomPos += 4;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                }

            }
            else if (numOfSlices == 3)
            {
                for (int i = 1; i < numOfSlices; i++)
                {
                    randomPos += 3;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                }
            }
            else if (numOfSlices == 4)
            {
                for (int i = 1; i < numOfSlices; i++)
                {
                    randomPos += 2;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }
                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                }
            }
            else if (numOfSlices > 4)
            {
                possibleSlotsTemp.Remove(randomPos);

                for (int i = 0; i < 3; i++)
                {
                    randomPos += 2;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                    possibleSlotsTemp.Remove(randomPos);
                }

                int slicesLeft = numOfSlices - fullSlices.Count;

                for (int i = 0; i < slicesLeft; i++)
                {
                    randomPos = Random.Range(0, possibleSlotsTemp.Count);

                    fullSlices.Add(sliceSlots[possibleSlotsTemp[randomPos]].transform.GetComponent<Slice>());

                    possibleSlotsTemp.Remove(possibleSlotsTemp[randomPos]);
                }
            }

            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetSliceData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i].sliceToSpawn, GameManager.Instance.currentLevel.slicesToSpawn[i].isLock, GameManager.Instance.currentLevel.slicesToSpawn[i].isLoot, GameManager.Instance.currentLevel.slicesToSpawn[i].isLimiter);
            }
        }
        else
        {

            for (int i = 0; i < GameManager.Instance.currentLevel.slicesToSpawn.Length; i++)
            {
                fullSlices.Add(sliceSlots[GameManager.Instance.currentLevel.specificSliceSpots[i]].transform.GetComponent<Slice>());
            }


            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetSliceData(sliceSlots[GameManager.Instance.currentLevel.specificSliceSpots[i]].transform, GameManager.Instance.currentLevel.slicesToSpawn[i].sliceToSpawn, GameManager.Instance.currentLevel.slicesToSpawn[i].isLock, GameManager.Instance.currentLevel.slicesToSpawn[i].isLoot, GameManager.Instance.currentLevel.slicesToSpawn[i].isLimiter);
            }
        }
    }

    public void SpawnTwelveRingSlices(int numOfSlices)
    {
        if (GameManager.Instance.currentLevel.RandomSlicePositions)
        {
            int randomPos = Random.Range(0, sliceSlots.Length);

            fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

            if (numOfSlices == 2)
            {
                for (int i = 1; i < numOfSlices; i++)
                {
                    randomPos += 6;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                }

            }
            else if (numOfSlices == 3)
            {
                for (int i = 1; i < numOfSlices; i++)
                {
                    randomPos += 4;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                }
            }
            else if (numOfSlices == 4)
            {
                for (int i = 1; i < numOfSlices; i++)
                {
                    randomPos += 3;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }
                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                }
            }
            else if (numOfSlices == 5)
            {
                possibleSlotsTemp.Remove(randomPos);

                for (int i = 0; i < 2; i++) // 3 spaces
                {
                    randomPos += 3;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                    possibleSlotsTemp.Remove(randomPos);
                }

                for (int i = 0; i < 2; i++) // 2 spaces
                {
                    randomPos += 2;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                    possibleSlotsTemp.Remove(randomPos);
                }
            }
            else if (numOfSlices > 5)
            {
                possibleSlotsTemp.Remove(randomPos);

                for (int i = 0; i < 5; i++)
                {
                    randomPos += 2;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());

                    possibleSlotsTemp.Remove(randomPos);
                }

                int slicesLeft = numOfSlices - fullSlices.Count;

                for (int i = 0; i < slicesLeft; i++)
                {
                    randomPos = Random.Range(0, possibleSlotsTemp.Count);

                    fullSlices.Add(sliceSlots[possibleSlotsTemp[randomPos]].transform.GetComponent<Slice>());

                    possibleSlotsTemp.Remove(possibleSlotsTemp[randomPos]);
                }
            }

            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetSliceData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i].sliceToSpawn, GameManager.Instance.currentLevel.slicesToSpawn[i].isLock, GameManager.Instance.currentLevel.slicesToSpawn[i].isLoot, GameManager.Instance.currentLevel.slicesToSpawn[i].isLimiter);
            }
        }
        else
        {

            for (int i = 0; i < GameManager.Instance.currentLevel.slicesToSpawn.Length; i++)
            {
                fullSlices.Add(sliceSlots[GameManager.Instance.currentLevel.specificSliceSpots[i]].transform.GetComponent<Slice>());
            }


            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetSliceData(sliceSlots[GameManager.Instance.currentLevel.specificSliceSpots[i]].transform, GameManager.Instance.currentLevel.slicesToSpawn[i].sliceToSpawn, GameManager.Instance.currentLevel.slicesToSpawn[i].isLock, GameManager.Instance.currentLevel.slicesToSpawn[i].isLoot, GameManager.Instance.currentLevel.slicesToSpawn[i].isLimiter);
            }
        }
    }
    public void SpawnSlicesTutorial(int numOfSlices)
    {
        if(numOfSlices > 0)
        {
            fullSlices = new List<Slice>();

            possibleSlotsTemp = new List<int>();

            for (int i = 0; i < sliceSlots.Length; i++)
            {
                possibleSlotsTemp.Add(i);
            }

            if (numOfSlices == 4)
            {
                fourRandomSlicePos = Random.Range(0, 2);
            }

            for (int i = 0; i < numOfSlices; i++)
            {
                if (GameManager.Instance.currentLevel.RandomSlicePositions)
                {
                    //int randomPos = Random.Range(0, possibleSlotsTemp.Count);
                    int randomPos = 0;


                    //fullSlices.Add(sliceSlots[possibleSlotsTemp[randomPos]].transform.GetComponent<Slice>());
                    //RemovePositions(randomPos);

                    if (numOfSlices < 4)
                    {

                        randomPos = Random.Range(0, possibleSlotsTemp.Count);

                        fullSlices.Add(sliceSlots[possibleSlotsTemp[randomPos]].transform.GetComponent<Slice>());

                        RemovePositions(possibleSlotsTemp[randomPos]);
                    }
                    else if (numOfSlices == 4)
                    {
                        fullSlices.Add(sliceSlots[fourRandomSlicePos].transform.GetComponent<Slice>());

                        //for (int j = 1; j < numOfSlices; j++)
                        //{
                        fourRandomSlicePos += 2;

                        if (fourRandomSlicePos >= sliceSlots.Length)
                        {
                            fourRandomSlicePos -= sliceSlots.Length;
                        }

                        //}
                    }
                    else
                    {
                        if(fullSlices.Count < 4)
                        {
                            fullSlices.Add(sliceSlots[fourRandomSlicePos].transform.GetComponent<Slice>());
                            possibleSlotsTemp.Remove(fourRandomSlicePos);

                            // for (int j = 1; j < sliceSlots.Length / 2; j++)
                            //{
                            fourRandomSlicePos += 2;

                            if (fourRandomSlicePos >= sliceSlots.Length)
                            {
                                fourRandomSlicePos -= sliceSlots.Length;
                            }
                            //}
                        }
                        else
                        {
                            for (int j = 0; j < numOfSlices - sliceSlots.Length / 2; j++)
                            {
                                randomPos = Random.Range(0, possibleSlotsTemp.Count);

                                fullSlices.Add(sliceSlots[possibleSlotsTemp[randomPos]].transform.GetComponent<Slice>());
                                possibleSlotsTemp.Remove(possibleSlotsTemp[randomPos]);
                            }
                        }
                    }
                }
                else
                {
                    if (GameManager.Instance.copyOfSpecificSliceSpotsTutorial[0] >= ConnectionManager.Instance.cells.Count)
                    {
                        Debug.LogError("The value you put in the slice index list is larger than 7!");
                        return;
                    }
                    else
                    {
                        int specifPos = GameManager.Instance.copyOfSpecificSliceSpotsTutorial[0];

                        fullSlices.Add(sliceSlots[specifPos].transform.GetComponent<Slice>());

                        GameManager.Instance.copyOfSpecificSliceSpotsTutorial.RemoveAt(0);
                    }
                }
            }

            for (int k = 0; k < fullSlices.Count; k++)
            {
                fullSlices[k].SetSliceData(fullSlices[k].transform, GameManager.Instance.currentLevel.slicesToSpawn[k].sliceToSpawn, GameManager.Instance.currentLevel.slicesToSpawn[k].isLock, GameManager.Instance.currentLevel.slicesToSpawn[k].isLoot, GameManager.Instance.currentLevel.slicesToSpawn[k].isLimiter);
            }

            if (GameManager.Instance.currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || GameManager.Instance.currentLevel.isGrindLevel || ServerRelatedData.instance.canRepeatLevels)
            {
                for (int i = 0; i < fullSlices.Count; i++)
                {
                    fullSlices[i].SetSliceLootData(GameManager.Instance.currentLevel.RewardBags[Random.Range(0, GameManager.Instance.currentLevel.RewardBags.Length)]);
                }
            }

            //if (ZoneManager.Instance.isKeyLevel)
            //{
            //    int randomSlice = Random.Range(0, fullSlices.Count);
            //    Debug.Log(randomSlice);
            //    fullSlices[randomSlice].isKey = true;

            //    ////// KEY HERE
            //}
        }
    }

    public void RemovePositions(int rPos)
    {
        possibleSlotsTemp.Remove(rPos);

        if (rPos == sliceSlots.Length - 1)
        {
            possibleSlotsTemp.Remove(0);
        }
        else
        {
            possibleSlotsTemp.Remove(rPos + 1);
        }

        if (rPos == 0)
        {
            possibleSlotsTemp.Remove(sliceSlots.Length - 1);
        }
        else
        {
            possibleSlotsTemp.Remove(rPos - 1);
        }
    }

    public void GetPrefabSliceToInstantiate()
    {

    }

    public void SetSliceSolvedSprite(Slice slice)
    {
        if (slice.isLimiter)
        {
            SpriteRenderer sr = slice.child.GetComponent<SpriteRenderer>();
            
            if(slice.sliceCatagory == SliceCatagory.Color || slice.sliceCatagory == SliceCatagory.SpecificColor)
            {
                sr.sprite = sliceCompletedSpritesColor[(int)slice.sliceColor];
            }

            if(slice.sliceCatagory == SliceCatagory.Shape || slice.sliceCatagory == SliceCatagory.SpecificShape)
            {
                sr.sprite = sliceCompletedSpritesShapes[(int)slice.sliceSymbol];
            }
        }
    }
    public void SetSliceNotSolvedSprite(Slice slice)
    {
        if (slice.isLimiter)
        {
            SpriteRenderer sr = slice.child.GetComponent<SpriteRenderer>();
            
            if(slice.sliceCatagory == SliceCatagory.Color || slice.sliceCatagory == SliceCatagory.SpecificColor)
            {
                sr.sprite = limiterSliceColors[(int)slice.sliceColor];
            }

            if(slice.sliceCatagory == SliceCatagory.Shape || slice.sliceCatagory == SliceCatagory.SpecificShape)
            {
                sr.sprite = limiterSliceSymbolsSprites[(int)slice.sliceSymbol];
            }
        }
    }
}
