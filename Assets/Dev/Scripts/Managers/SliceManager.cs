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
    public GameObject slicePrefab;
    public GameObject slicePrefabLimiter;

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

    public Sprite[] limiterSliceColors;
    public Sprite[] limiterSliceSymbolsSprites;

    public Sprite[] sliceLootIcons;


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

    public Dictionary<PieceSymbol, Sprite> limiterSliceSymbolToSprite;
    public Dictionary<PieceColor, Sprite> limiterSlicecolorToSprite;
    //public Dictionary<PieceColor, Material> pieceColorToColor;

    public Dictionary<LootPacks, Sprite> lootToIcon;

    List<int> possibleSlots;

    public List<Slice> fullSlices;
    //GameObject go;

    private void Awake()
    {
        GameManager.Instance.sliceManager = this;
    }

    public void Init()
    {
        sliceSymbolToSprite = new Dictionary<PieceSymbol, Texture>();
        slicecolorToSprite = new Dictionary<PieceColor, Texture>();

        limiterSliceSymbolToSprite = new Dictionary<PieceSymbol, Sprite>();
        limiterSlicecolorToSprite = new Dictionary<PieceColor, Sprite>();

        //pieceColorToColor = new Dictionary<PieceColor, Material>();

        //lootSliceSymbolDict = new Dictionary<PieceSymbol, Sprite>();
        //lootSliceColorDict = new Dictionary<PieceColor, Sprite>();

        //lootLockSliceSymbolSpritesDict = new Dictionary<PieceSymbol, Sprite>();
        //lootLockSliceColorDict = new Dictionary<PieceColor, Sprite>();

        //lootLockLimiterSliceSymbolSpritesDict = new Dictionary<PieceSymbol, Sprite>();
        //lootLockSLimiterliceColorDict = new Dictionary<PieceColor, Sprite>();

        lootToIcon = new Dictionary<LootPacks, Sprite>();

        for (int i = 1; i < System.Enum.GetValues(typeof(LootPacks)).Length; i++)
        {
            string lootPackName = System.Enum.GetName(typeof(LootPacks), i);

            if (lootPackName.Contains("I"))
            {
                lootToIcon.Add((LootPacks)i, sliceLootIcons[1]); ////// The last sprite in the list is the same sprite for all Loot packes with 'I' in them
            }
            else
            {
                lootToIcon.Add((LootPacks)i, sliceLootIcons[0]);
            }
        }

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

        //for (int i = 0; i < System.Enum.GetValues(typeof(PieceColor)).Length; i++)
        //{
        //    pieceColorToColor.Add((PieceColor)i, GameManager.Instance.clipManager.gameColors[i]);
        //}

        //for (int i = 0; i < lootSliceSymbolSprites.Length; i++)
        //{
        //    lootSliceSymbolDict.Add((PieceSymbol)i, lootSliceSymbolSprites[i]);
        //}

        //for (int i = 0; i < lootSliceColorSprites.Length; i++)
        //{
        //    lootSliceColorDict.Add((PieceColor)i, lootSliceColorSprites[i]);
        //}

        //for (int i = 0; i < lootLockSliceSymbolSprites.Length; i++)
        //{
        //    lootLockSliceSymbolSpritesDict.Add((PieceSymbol)i, lootSliceSymbolSprites[i]);
        //}

        //for (int i = 0; i < lootLockSliceColorSprites.Length; i++)
        //{
        //    lootLockSliceColorDict.Add((PieceColor)i, lootSliceColorSprites[i]);
        //}

        //for (int i = 0; i < lootLockLimiterSliceSymbolSprites.Length; i++)
        //{
        //    lootLockLimiterSliceSymbolSpritesDict.Add((PieceSymbol)i, lootSliceSymbolSprites[i]);
        //}

        //for (int i = 0; i < lootLockLimiterSliceColorSprites.Length; i++)
        //{
        //    lootLockSLimiterliceColorDict.Add((PieceColor)i, lootSliceColorSprites[i]);
        //}
    }

    internal void InitTutorial()
    {
        throw new System.NotImplementedException();
    }

    public void SpawnSlices(int numOfSlices)
    {
        if(numOfSlices > 0)
        {
            fullSlices = new List<Slice>();

            possibleSlots = new List<int>();

            for (int i = 0; i < sliceSlots.Length; i++)
            {
                possibleSlots.Add(i);
            }

            int randomPos = Random.Range(0, sliceSlots.Length);

            //go = Instantiate(slicePrefabs, sliceSlots[randomPos]);

            //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
            fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());
            //o.transform.parent.GetComponent<Slice>().child = go;

            if (numOfSlices < 4)
            {
                RemovePositions(randomPos);

                for (int i = 1; i < numOfSlices; i++)
                {

                    randomPos = Random.Range(0, possibleSlots.Count);
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[possibleSlots[randomPos]]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[possibleSlots[randomPos]].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;

                    RemovePositions(possibleSlots[randomPos]);
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
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[randomPos]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;

                }
            }
            else
            {
                possibleSlots.Remove(randomPos);

                for (int i = 1; i < sliceSlots.Length / 2; i++)
                {
                    randomPos += 2;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[randomPos]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;
                    possibleSlots.Remove(randomPos);
                }

                for (int i = 0; i < numOfSlices - sliceSlots.Length / 2; i++)
                {
                    randomPos = Random.Range(0, possibleSlots.Count);
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[possibleSlots[randomPos]]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[possibleSlots[randomPos]].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;
                    possibleSlots.Remove(possibleSlots[randomPos]);
                }
            }

            //if (GameManager.Instance.currentLevel.isRandomDistributionToSlices)
            //{
            //    List<LootPacks> tempList = new List<LootPacks>();
            //    tempList.AddRange(GameManager.Instance.currentLevel.RewardBags);

            //    for (int i = 0; i < fullSlices.Count; i++)
            //    {
            //        fullSlices[i].SetSliceData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i]);
            //    }

            //    for (int i = 0; i < GameManager.Instance.currentLevel.RewardBags.Length; i++)
            //    {
            //        int randomSlice = Random.Range(0, tempList.Count);
            //        fullSlices[i].SetSliceLootData(tempList[randomSlice]);
            //        tempList.RemoveAt(randomSlice);
            //    }
            //}
            //else
            //{

            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetSliceData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i]);
            }

            if (GameManager.Instance.currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                for (int i = 0; i < fullSlices.Count; i++)
                {
                    fullSlices[i].SetSliceLootData(GameManager.Instance.currentLevel.RewardBags[Random.Range(0, GameManager.Instance.currentLevel.RewardBags.Length)]);
                }
            }

            //}

            /// Distribute Key to a random slice
            if (ZoneManager.Instance.isKeyLevel)
            {
                int randomSlice = Random.Range(0, fullSlices.Count);
                Debug.Log(randomSlice);
                fullSlices[randomSlice].isKey = true;
            }
        }
    }
    public void SpawnSlicesTutorial(int numOfSlices)
    {
        if(numOfSlices > 0)
        {
            fullSlices = new List<Slice>();

            possibleSlots = new List<int>();

            for (int i = 0; i < sliceSlots.Length; i++)
            {
                possibleSlots.Add(i);
            }

            int randomPos = Random.Range(0, sliceSlots.Length);

            //go = Instantiate(slicePrefabs, sliceSlots[randomPos]);

            //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
            fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());
            //o.transform.parent.GetComponent<Slice>().child = go;

            if (numOfSlices < 4)
            {
                RemovePositions(randomPos);

                for (int i = 1; i < numOfSlices; i++)
                {

                    randomPos = Random.Range(0, possibleSlots.Count);
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[possibleSlots[randomPos]]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[possibleSlots[randomPos]].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;

                    RemovePositions(possibleSlots[randomPos]);
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
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[randomPos]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;

                }
            }
            else
            {
                possibleSlots.Remove(randomPos);

                for (int i = 1; i < sliceSlots.Length / 2; i++)
                {
                    randomPos += 2;

                    if (randomPos >= sliceSlots.Length)
                    {
                        randomPos -= sliceSlots.Length;
                    }

                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[randomPos]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[randomPos].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;
                    possibleSlots.Remove(randomPos);
                }

                for (int i = 0; i < numOfSlices - sliceSlots.Length / 2; i++)
                {
                    randomPos = Random.Range(0, possibleSlots.Count);
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    //go = Instantiate(slicePrefabs, sliceSlots[possibleSlots[randomPos]]);
                    //fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    fullSlices.Add(sliceSlots[possibleSlots[randomPos]].transform.GetComponent<Slice>());
                    //go.transform.parent.GetComponent<Slice>().child = go;
                    possibleSlots.Remove(possibleSlots[randomPos]);
                }
            }

            //if (GameManager.Instance.currentLevel.isRandomDistributionToSlices)
            //{
            //    List<LootPacks> tempList = new List<LootPacks>();
            //    tempList.AddRange(GameManager.Instance.currentLevel.RewardBags);

            //    for (int i = 0; i < fullSlices.Count; i++)
            //    {
            //        fullSlices[i].SetSliceData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i]);
            //    }

            //    for (int i = 0; i < GameManager.Instance.currentLevel.RewardBags.Length; i++)
            //    {
            //        int randomSlice = Random.Range(0, tempList.Count);
            //        fullSlices[i].SetSliceLootData(tempList[randomSlice]);
            //        tempList.RemoveAt(randomSlice);
            //    }
            //}
            //else
            //{

            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetSliceData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i]);
            }

            if (GameManager.Instance.currentLevel.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                for (int i = 0; i < fullSlices.Count; i++)
                {
                    fullSlices[i].SetSliceLootData(GameManager.Instance.currentLevel.RewardBags[Random.Range(0, GameManager.Instance.currentLevel.RewardBags.Length)]);
                }
            }

            //}

            /// Distribute Key to a random slice
            if (ZoneManager.Instance.isKeyLevel)
            {
                int randomSlice = Random.Range(0, fullSlices.Count);
                Debug.Log(randomSlice);
                fullSlices[randomSlice].isKey = true;
            }
        }
    }

    public void RemovePositions(int rPos)
    {
        possibleSlots.Remove(rPos);

        if (rPos == sliceSlots.Length - 1)
        {
            possibleSlots.Remove(0);
        }
        else
        {
            possibleSlots.Remove(rPos + 1);
        }

        if (rPos == 0)
        {
            possibleSlots.Remove(sliceSlots.Length - 1);
        }
        else
        {
            possibleSlots.Remove(rPos - 1);
        }
    }

    public void GetPrefabSliceToInstantiate()
    {

    }
}
