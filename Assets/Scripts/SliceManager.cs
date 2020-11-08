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
    public GameObject slicePrefab;

    public Transform[] sliceSlots;

    public Sprite[] sliceSymbolSprites;
    public Sprite[] sliceColorSprites;

    public Dictionary<PieceSymbol, Sprite> sliceSymbolDict;
    public Dictionary<PieceColor, Sprite> sliceColorDict;

    List<int> possibleSlots;

    public List<Slice> fullSlices;
    GameObject go;

    private void Awake()
    {
        GameManager.Instance.sliceManager = this;
        sliceSymbolDict = new Dictionary<PieceSymbol, Sprite>();
        sliceColorDict = new Dictionary<PieceColor, Sprite>();

        for (int i = 0; i < sliceSymbolSprites.Length; i++)
        {
            sliceSymbolDict.Add((PieceSymbol)i, sliceSymbolSprites[i]);
        }

        for (int i = 0; i < sliceColorSprites.Length; i++)
        {
            sliceColorDict.Add((PieceColor)i, sliceColorSprites[i]);
        }
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

            go = Instantiate(slicePrefab, sliceSlots[randomPos]);

            fullSlices.Add(go.transform.parent.GetComponent<Slice>());
            go.transform.parent.GetComponent<Slice>().child = go;

            if (numOfSlices < 4)
            {
                RemovePositions(randomPos);

                for (int i = 1; i < numOfSlices; i++)
                {

                    randomPos = Random.Range(0, possibleSlots.Count);
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    go = Instantiate(slicePrefab, sliceSlots[possibleSlots[randomPos]]);
                    fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    go.transform.parent.GetComponent<Slice>().child = go;

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

                    go = Instantiate(slicePrefab, sliceSlots[randomPos]);
                    fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    go.transform.parent.GetComponent<Slice>().child = go;

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

                    go = Instantiate(slicePrefab, sliceSlots[randomPos]);
                    fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    go.transform.parent.GetComponent<Slice>().child = go;
                    possibleSlots.Remove(randomPos);
                }

                for (int i = 0; i < numOfSlices - sliceSlots.Length / 2; i++)
                {
                    randomPos = Random.Range(0, possibleSlots.Count);
                    //randomPrefab = Random.Range(0, slicePrefabs.Length);

                    go = Instantiate(slicePrefab, sliceSlots[possibleSlots[randomPos]]);
                    fullSlices.Add(go.transform.parent.GetComponent<Slice>());
                    go.transform.parent.GetComponent<Slice>().child = go;
                    possibleSlots.Remove(possibleSlots[randomPos]);
                }
            }

            for (int i = 0; i < fullSlices.Count; i++)
            {
                fullSlices[i].SetData(GameManager.Instance.currentLevel.slicesToSpawn[i], GameManager.Instance.currentLevel.lockSlices[i], GameManager.Instance.currentLevel.lootSlices[i], GameManager.Instance.currentLevel.limiterSlices[i], GameManager.Instance.currentLevel.RewardBags[i]);
            }

            /// Give Key
            //if (GameManager.Instance.isKeyLevel)
            //{
            //    int randomSlice = Random.Range(0, fullSlices.Count);
            //    Debug.Log(randomSlice);
            //    fullSlices[randomSlice].isKey = true;
            //}
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
}
