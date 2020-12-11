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
    public GameObject lootSlicePrefab;
    public GameObject lootLockSlicePrefab;
    public GameObject lootLockLimiterSlicePrefab;

    public Transform[] sliceSlots;

    //public Sprite[] lootSliceSymbolSprites;
    //public Sprite[] lootSliceColorSprites;

    //public Sprite[] lootLockSliceSymbolSprites;
    //public Sprite[] lootLockSliceColorSprites;

    //public Sprite[] lootLockLimiterSliceSymbolSprites;
    //public Sprite[] lootLockLimiterSliceColorSprites;

    public Sprite[] sliceColors;

    public Sprite[] slicelootIcons;

    public Sprite[] sliceSymbolsSprites;

    //public Dictionary<PieceSymbol, Sprite> lootSliceSymbolDict;
    //public Dictionary<PieceColor, Sprite> lootSliceColorDict;

    //public Dictionary<PieceSymbol, Sprite> lootLockSliceSymbolSpritesDict;
    //public Dictionary<PieceColor, Sprite> lootLockSliceColorDict;

    //public Dictionary<PieceSymbol, Sprite> lootLockLimiterSliceSymbolSpritesDict;
    //public Dictionary<PieceColor, Sprite> lootLockSLimiterliceColorDict;

    public Dictionary<PieceSymbol, Sprite> pieceSymbolToSprite;
    public Dictionary<PieceColor, Sprite> piececolorToSprite;
    public Dictionary<PieceColor, Color> pieceColorToColor;

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
        pieceSymbolToSprite = new Dictionary<PieceSymbol, Sprite>();
        piececolorToSprite = new Dictionary<PieceColor, Sprite>();
        pieceColorToColor = new Dictionary<PieceColor, Color>();

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
                lootToIcon.Add((LootPacks)i, slicelootIcons[slicelootIcons.Length - 1]); ////// The last sprite in the list is the same sprite for all Loot packes with 'I' in them
            }
            else
            {
                lootToIcon.Add((LootPacks)i, slicelootIcons[i - 1]);
            }
        }

        for (int i = 0; i < sliceSymbolsSprites.Length; i++)
        {
            pieceSymbolToSprite.Add((PieceSymbol)i, sliceSymbolsSprites[i]);
        }

        for (int i = 0; i < sliceColors.Length; i++)
        {
            piececolorToSprite.Add((PieceColor)i, sliceColors[i]);
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(PieceColor)).Length; i++)
        {
            pieceColorToColor.Add((PieceColor)i, GameManager.Instance.clipManager.gameColors[i]);
        }

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

            if (GameManager.Instance.currentLevel.isRandomDistributionToSlices)
            {
                List<LootPacks> tempList = new List<LootPacks>();
                tempList.AddRange(GameManager.Instance.currentLevel.RewardBags);

                for (int i = 0; i < fullSlices.Count; i++)
                {
                    int randomSlice = Random.Range(0, tempList.Count);
                    fullSlices[i].SetData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i], tempList[randomSlice]);
                    tempList.RemoveAt(randomSlice);
                }
            }
            else
            {
                for (int i = 0; i < fullSlices.Count; i++)
                {
                    fullSlices[i].SetData(fullSlices[i].transform, GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i], GameManager.Instance.currentLevel.RewardBags[i]);
                }
            }

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
