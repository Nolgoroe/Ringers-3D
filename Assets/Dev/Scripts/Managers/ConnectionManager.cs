using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public List<Cell> cells;
    public List<Cell> outerCells;

    public SubPiece[] subPiecesOnBoard;
    public SubPiece[] subPiecesDoubleRing;

    public Slice[] slicesOnBoard;

    public int lengthOfSubPiecesRegular;
    public int lengthOfSubPiecesOuter;

    public ParticleSystem goodConnectionParticle, badConnectionParticle;

    public GameObject lootEffectPrefab;

    public Material rockLIT,rockUnLIT;

    private void Start()
    {
        Instance = this;
    }

    public void SetLevelConnectionData()
    {
        subPiecesOnBoard = new SubPiece[lengthOfSubPiecesRegular];
        subPiecesDoubleRing = new SubPiece[lengthOfSubPiecesOuter];
    }

    public void GrabCellList(Transform gb)
    {
        foreach (Cell c in gb.GetComponentsInChildren<Cell>())
        {
            if (!c.isOuter)
            {
                cells.Add(c);
            }
            else
            {
                outerCells.Add(c);
            }
        }
        slicesOnBoard = gb.GetComponentsInChildren<Slice>();
    }

    public void CallConnection(int cellIndex, bool isOuterCell)
    {
        if (!isOuterCell)
        {
            CheckConnections(subPiecesOnBoard, cells, cellIndex, isOuterCell);
        }
        else
        {
            CheckConnections(subPiecesDoubleRing, outerCells,cellIndex, isOuterCell);
        }
    }

    public void CheckConnections(SubPiece[] supPieceArray, List<Cell> cellList, int cellIndex, bool isOuterCell)
    {
        int leftContested = CheckIntRange((cellIndex * 2) - 1);
        int rightContested = CheckIntRange((cellIndex * 2) + 2);

        int currentLeft = cellIndex * 2;
        int currentRight = cellIndex * 2 + 1;

        //int rightContested = CheckIntRange((cellIndex * 2) + 2);
        //int leftContested = CheckIntRange((cellIndex * 2) -1);

        //int currentRight = cellIndex * 2 + 1;
        //int currentLeft = cellIndex * 2;

        if (supPieceArray[leftContested])
        {
            if (supPieceArray[currentLeft])
            {
                if (!CheckSubPieceConnection(supPieceArray[currentLeft], supPieceArray[leftContested], out bool conditionmet))
                {
                    Debug.Log("Bad Connection Right Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;
                    supPieceArray[currentLeft].isBadConnection = true;
                    supPieceArray[leftContested].isBadConnection = true;
                    //supPieceArray[currentLeft].gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    //supPieceArray[leftContested].gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                    if (supPieceArray[currentLeft].relevantSlice)
                    {
                        supPieceArray[currentLeft].relevantSlice.fulfilledCondition = false;
                    }

                    //Instantiate(badConnectionParticle, cellList[cellIndex].rightParticleZone);
                }
                else
                {
                    //Instantiate(goodConnectionParticle, cellList[cellIndex].rightParticleZone);
                    supPieceArray[currentLeft].SetConnectedMaterial();
                    supPieceArray[leftContested].SetConnectedMaterial();
                    //supPieceArray[currentLeft].gameObject.GetComponent<Renderer>().material.EnableKeyword ("_EMISSION");
                    //supPieceArray[leftContested].gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

                    if (conditionmet)
                    {
                        supPieceArray[currentLeft].relevantSlice.fulfilledCondition = true;

                        if (supPieceArray[currentLeft].relevantSlice.isLoot)
                        {
                            GiveLoot(supPieceArray[currentLeft].relevantSlice, supPieceArray[currentLeft].relevantSlice.isLimiter);
                        }

                        if (supPieceArray[currentLeft].relevantSlice.isLock)
                        {
                            LockCell(supPieceArray[currentLeft].relevantSlice, supPieceArray[currentLeft].relevantSlice.isLimiter);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Bad Connection Right Conetsted FUCKKKKAKAAAAAA");
                GameManager.Instance.unsuccessfullConnectionCount++;
                supPieceArray[leftContested].isBadConnection = true;
            }
        }
        else
        {
            if (!isOuterCell)
            {
                supPieceArray[currentLeft].relevantSlice.fulfilledCondition = false;
            }
        }

        if (supPieceArray[rightContested])
        {
            if (supPieceArray[currentRight])
            {
                if (!CheckSubPieceConnection(supPieceArray[currentRight], supPieceArray[rightContested], out bool conditionmet))
                {
                    Debug.Log("Bad Connection Left Conetsted");
                    GameManager.Instance.unsuccessfullConnectionCount++;
                    supPieceArray[currentRight].isBadConnection = true;
                    supPieceArray[rightContested].isBadConnection = true;
                    //supPieceArray[currentRight].gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    //supPieceArray[rightContested].gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

                    if (supPieceArray[currentRight].relevantSlice)
                    {
                        supPieceArray[currentRight].relevantSlice.fulfilledCondition = false;
                    }

                    //Instantiate(badConnectionParticle, cellList[cellIndex].leftParticleZone);
                }
                else
                {
                    //Instantiate(goodConnectionParticle, cellList[cellIndex].leftParticleZone);

                    supPieceArray[currentRight].SetConnectedMaterial();
                    supPieceArray[rightContested].SetConnectedMaterial();

                    //supPieceArray[currentRight].gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    //supPieceArray[rightContested].gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

                    if (conditionmet)
                    {
                        supPieceArray[currentRight].relevantSlice.fulfilledCondition = true;
                        if (supPieceArray[currentRight].relevantSlice.isLoot)
                        {
                            GiveLoot(supPieceArray[currentRight].relevantSlice, supPieceArray[currentRight].relevantSlice.isLimiter);
                        }

                        if (supPieceArray[currentRight].relevantSlice.isLock)
                        {
                            LockCell(supPieceArray[currentRight].relevantSlice, supPieceArray[currentRight].relevantSlice.isLimiter);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Bad Connection Right Conetsted");
                GameManager.Instance.unsuccessfullConnectionCount++;
                supPieceArray[rightContested].isBadConnection = true;
            }
        }
        else
        {
            if (!isOuterCell)
            {
                supPieceArray[currentRight].relevantSlice.fulfilledCondition = false;
            }
        }

        /// Get slice
        /// Check list of conditions for correct connections, 2 connection.
        /// 
    }
    public bool CheckSubPieceConnection(SubPiece currentSide, SubPiece contestedSide, out bool conditionMet)
    {
        bool isGoodConnect = false;
        bool conditionCheck = false;

        if (currentSide.relevantSlice)
        {
            if (currentSide.relevantSlice.sliceCatagory != SliceCatagory.None)
            {
                CompareResault result = TotalCheck(currentSide, contestedSide);

                if (!currentSide.relevantSlice.isLimiter)
                {

                    conditionCheck = CheckFulfilledSliceCondition(currentSide.relevantSlice, result, currentSide, contestedSide);

                    if (conditionCheck)
                    {
                        conditionMet = conditionCheck;
                        return true;
                    }

                    if (result.gColorMatch)
                    {
                        isGoodConnect = true;
                    }

                    if (result.gSymbolMatch)
                    {
                        isGoodConnect = true;
                    }
                }
                else
                {
                    conditionCheck = CheckFulfilledSliceCondition(currentSide.relevantSlice, result, currentSide, contestedSide);

                    if (conditionCheck)
                    {
                        conditionMet = conditionCheck;
                        return true;
                    }
                    else
                    {
                        conditionMet = false;
                        return false;
                    }

                }
            }
            else
            {
                CompareResault result = TotalCheck(currentSide, contestedSide);

                if (result.gColorMatch)
                {
                    isGoodConnect = true;
                }

                if (result.gSymbolMatch)
                {
                    isGoodConnect = true;
                }
            }
        }
        else
        {
            CompareResault result = TotalCheck(currentSide, contestedSide);

            if (result.gColorMatch)
            {
                isGoodConnect = true;
            }

            if (result.gSymbolMatch)
            {
                isGoodConnect = true;
            }

        }

        conditionMet = conditionCheck;
        return isGoodConnect;
    }
    public CompareResault TotalCheck(SubPiece current, SubPiece contested/*, PieceColor sCol, PieceSymbol sSym*/)
    {
        CompareResault result = new CompareResault();

        if(current && contested)
        {
            result.gColorMatch = EqualColorOrJoker(current.colorOfPiece, contested.colorOfPiece);
            result.gSymbolMatch = EqualSymbolOrJoker(current.symbolOfPiece, contested.symbolOfPiece);
        }

        return result;
    }
    public int CheckIntRange(int num)
    {
        if (num <= 0)
        {
            return lengthOfSubPiecesRegular - 1;
        }

        if (num >= lengthOfSubPiecesRegular)
        {
            return 0;
        }

        return num;
    }

    public int CheckIntRangeCells(int num)
    {
        if (num < 0)
        {
            return cells.Count - 1;
        }

        if (num >= cells.Count)
        {
            return 0;
        }


        return num;
    }
    public void FillSubPieceIndex()
    {
        for (int i = 0; i < subPiecesOnBoard.Length; i++)
        {
            if (subPiecesOnBoard[i])
            {
                subPiecesOnBoard[i].subPieceIndex = i;
            }
        }
        for (int i = 0; i < subPiecesDoubleRing.Length; i++)
        {
            if (subPiecesDoubleRing[i])
            {
                subPiecesDoubleRing[i].subPieceIndex = i;
            }
        }
    }
    public void RemoveSubPieceIndex(int i, bool isOuterCell)
    {
        if (isOuterCell)
        {
            subPiecesDoubleRing[i] = null;
        }
        else
        {
            subPiecesOnBoard[i] = null;
        }
    }
    public bool EqualColorOrJoker(PieceColor colA, PieceColor colB)/// Colorcheck is to see if we need to check color or symbol
    {
        if(colA == colB || (colA == PieceColor.Joker || colB == PieceColor.Joker))
        {
            return true;
        }

        return false;
    }
    public bool EqualSymbolOrJoker(PieceSymbol symA, PieceSymbol symB)/// Colorcheck is to see if we need to check color or symbol
    {
        if (symA == symB || (symA == PieceSymbol.Joker || symB == PieceSymbol.Joker))
        {
            return true;
        }
        return false;

    }

    public void GiveLoot(Slice relevent, bool isLimiter)
    {
        Debug.Log("Loot");
        Debug.Log(relevent.lootPack);

        if(relevent.lootPack != LootPacks.None)
        {
            LootManager.Instance.currentLevelLootToGive.Add(relevent.lootPack);
        }

        //LootManager.Instance.RollOnTable(relevent.lootPack);
        if (!isLimiter)
        {
            SpriteRenderer relevantSliceSR = relevent.child.GetComponent<SpriteRenderer>();
            //relevantSliceSR.color = new Color(relevantSliceSR.color.r, relevantSliceSR.color.g, relevantSliceSR.color.b, 0.4f);

            //SpriteRenderer ciconSR = relevent.child.transform.GetChild(0).GetComponent<SpriteRenderer>();

            //ciconSR.color = new Color(ciconSR.color.r, ciconSR.color.g, ciconSR.color.b, 0.4f);
            //relevent.lootIcon.GetComponent<Rigidbody2D>().simulated = true;
            //relevent.lootIcon.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);


            switch (relevent.lootPack.ToString()[0])
            {
                //case 'M':
                //    StartCoroutine(InstantiateLootEffect(relevent,relevent.lootIcon.transform, LootManager.Instance.goldSprite, LootTargetsData.instance.goldTargetLoot));
                //    break;

                case 'R':
                    StartCoroutine(InstantiateLootEffect(relevent,relevent.lootIcon.transform, LootManager.Instance.rubySprite, LootTargetsData.instance.rubyTargetLoot));
                    break;

                case 'I':
                    if (relevent.lootPack != LootPacks.None)
                    {
                        StartCoroutine(InstantiateLootEffectMaterials(relevent, relevent.lootIcon.transform, LootTargetsData.instance.materialsTargetLoot));
                    }
                    break;

                default:
                    break;
            }

            relevent.isLoot = false;
        }
        else
        {
            switch (relevent.lootPack.ToString()[0])
            {
                //case 'M':
                //    StartCoroutine(InstantiateLootEffect(relevent, relevent.lootIcon.transform, relevent.lootIcon.GetComponent<SpriteRenderer>().sprite, LootTargetsData.instance.goldTargetLoot));
                //    break;

                case 'R':
                    StartCoroutine(InstantiateLootEffect(relevent, relevent.lootIcon.transform, relevent.lootIcon.GetComponent<SpriteRenderer>().sprite, LootTargetsData.instance.rubyTargetLoot));
                    break;

                case 'I':
                    if (relevent.lootPack != LootPacks.None)
                    {
                        StartCoroutine(InstantiateLootEffectMaterials(relevent, relevent.lootIcon.transform, LootTargetsData.instance.materialsTargetLoot));
                    }
                    break;

                default:
                    break;
            }

            relevent.isLoot = false;
        }
    }
    public void LockCell(Slice relevent, bool isLimiter)
    {
        Debug.Log("Lock");

        cells[relevent.sliceIndex].lockSprite.SetActive(true);
        cells[relevent.sliceIndex].pieceHeld.isLocked = true;

        if (relevent.sliceIndex == 0)
        {
            cells[cells.Count - 1].lockSprite.SetActive(true);
            cells[cells.Count - 1].pieceHeld.isLocked = true;
        }
        else
        {
            cells[relevent.sliceIndex - 1].lockSprite.SetActive(true);
            cells[relevent.sliceIndex - 1].pieceHeld.isLocked = true;
        }

        if (!isLimiter)
        {
            //SpriteRenderer relevantSliceSR = relevent.child.GetComponent<SpriteRenderer>();
            //relevantSliceSR.color = new Color(relevantSliceSR.color.r, relevantSliceSR.color.g, relevantSliceSR.color.b, 0.4f);

           //SpriteRenderer ciconSR = relevent.child.transform.GetChild(0).GetComponent<SpriteRenderer>();

            //ciconSR.color = new Color(ciconSR.color.r, ciconSR.color.g, ciconSR.color.b, 0.4f);

            //relevent.lockPrefab.GetComponent<Rigidbody2D>().simulated = true;
            //relevent.lockPrefab.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5, ForceMode2D.Impulse);
            relevent.isLock = false;
            //Destroy(relevent.lockPrefab, 2f);

        }
        else
        {
            relevent.isLock = false;
        }
    }

    public bool CheckFulfilledSliceCondition(Slice relevent, CompareResault result, SubPiece a, SubPiece b)
    {
        bool isConditionMet = false;

        switch (relevent.sliceCatagory)
        {
            case SliceCatagory.Shape:
                if (result.gSymbolMatch)
                {
                    isConditionMet = true;
                }
                break;
            case SliceCatagory.Color:
                if (result.gColorMatch)
                {
                    isConditionMet = true;
                }
                break;
            case SliceCatagory.SpecificShape:

                if (result.gSymbolMatch)
                {
                    isConditionMet = EqualSymbolOrJoker(a.symbolOfPiece, relevent.sliceSymbol) && EqualSymbolOrJoker(b.symbolOfPiece, relevent.sliceSymbol);
                }

                break;
            case SliceCatagory.SpecificColor:
                if (result.gColorMatch)
                {
                    isConditionMet = EqualColorOrJoker(a.colorOfPiece, relevent.sliceColor) && EqualColorOrJoker(b.colorOfPiece, relevent.sliceColor);
                }
                break;
            default:
                isConditionMet = false;
                break;
        }

        return isConditionMet;
    }

    public void ResetConnectionData()
    {
        cells.Clear();
        outerCells.Clear();
        subPiecesOnBoard = new SubPiece[0];
        subPiecesDoubleRing = new SubPiece[0];
        slicesOnBoard = new Slice[0];
    }


    public IEnumerator InstantiateLootEffect(Slice relevent, Transform instantiateposition, Sprite look, Transform target)
    {
        int amount = 0;
        switch (relevent.lootPack)
        {
            //case LootPacks.M1:
            //    amount = 2;
            //    break;
            //case LootPacks.M2:
            //    amount = 4;
            //    break;
            //case LootPacks.M3:
            //    amount = 6;
            //    break;
            case LootPacks.R1:
                amount = 2;
                break;
            case LootPacks.R2:
                amount = 4;
                break;
            default:
                break;
        }

        for (int i = 0; i < amount; i++)
        {

            GameObject go = Instantiate(lootEffectPrefab, instantiateposition.position, Quaternion.identity);

            MoveToLootTarget MTLT = go.GetComponent<MoveToLootTarget>();
            MTLT.look = look;
            MTLT.target = target;

            MTLT.LeanMove();
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(relevent.lootIcon.gameObject);
    }

    public IEnumerator InstantiateLootEffectMaterials(Slice relevent, Transform instantiateposition, Transform target)
    {
        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = LootManager.Instance.lootpackEnumToRewardBag[relevent.lootPack];

        if (!rewardBagByLootPack.IsMoneyOrRubies)
        {
            List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();


            for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
            {
                craftingMatsFromTables.AddRange(LootManager.Instance.itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

                int chance = Random.Range(1, 101);

                if (chance > rewardBagByLootPack.chancesPerItemTable[i])
                {
                    Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
                    craftingMatsFromTables.Clear();
                }
                else
                {
                    int randomMat = Random.Range(0, craftingMatsFromTables.Count);

                    Debug.Log(craftingMatsFromTables[randomMat]);

                    LootManager.Instance.craftingMatsLootForLevel.Add(craftingMatsFromTables[randomMat]);

                    GameObject go = Instantiate(lootEffectPrefab, instantiateposition.position, Quaternion.identity);

                    MoveToLootTarget MTLT = go.GetComponent<MoveToLootTarget>();
                    MTLT.look = Resources.Load <Sprite>(MaterialsAndForgeManager.Instance.materialSpriteByName[craftingMatsFromTables[randomMat]]);
                    MTLT.target = target;

                    MTLT.LeanMove();
                    yield return new WaitForSeconds(0.2f);

                    craftingMatsFromTables.Clear();
                }
            }
        }
        Destroy(relevent.lootIcon.gameObject);
    }
}
