using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameAnalyticsSDK;

public class BossBattleManager : MonoBehaviour
{
    public static BossBattleManager instance;

    [Header("Boss Gameplay Data")]
    public int currentBossHealth;

    public int twoComboBaseDMG;

    [Header("Boss Action Data")]
    public bool bossBattleStarted = false;
    public float originalTimeTillNextTick;
    public float currentMaxTimeTillNextTick;
    public float minimalTimeForTick;
    public float currentTimeTillNextTick;
    public float reduceEveryTick;

    public float delayTimeBossActions;

    [Header("Boss Origin Data")]
    public LevelScriptableObject bossLevelSO;


    [Header("Boss Recieve DMG Data")]
    public List<Piece> piecesToRemove;
    public List<PieceColor> colorToDmg;
    public List<PieceSymbol> symbolToDmg;

    private void Start()
    {
        instance = this;
        currentTimeTillNextTick = 0;
        currentMaxTimeTillNextTick = originalTimeTillNextTick;

        piecesToRemove = new List<Piece>();
        colorToDmg = new List<PieceColor>();
        symbolToDmg = new List<PieceSymbol>();
    }


    private void FixedUpdate()
    {
        if (bossBattleStarted && !UIManager.isUsingUI)
        {
            CheckEndLevelBoss();

            currentTimeTillNextTick += Time.fixedDeltaTime;

            if(currentTimeTillNextTick >= currentMaxTimeTillNextTick)
            {
                BossAddRandomPieceToBoard();


                if (currentMaxTimeTillNextTick - reduceEveryTick >= minimalTimeForTick)
                {
                    currentMaxTimeTillNextTick -= reduceEveryTick;
                }

                currentTimeTillNextTick = 0;
            }
        }
    }


    public void BossAddRandomPieceToBoard()
    {
        Cell[] emptyCells = ConnectionManager.Instance.cells.Where(p => p.isFull == false).ToArray();

        int randomCell = Random.Range(0, emptyCells.Length);

        emptyCells[randomCell].AddPieceRandom();
    }

    public void CheckEndLevelBoss()
    {
        if(currentBossHealth <= 0)
        {
            bossBattleStarted = false;
            UIManager.Instance.youWinScreen.SetActive(true);

            Debug.Log("YOU WIN");

            return;
        }

        if (GameManager.Instance.currentFilledCellCount == bossLevelSO.cellsCountInLevel)
        {
            bossBattleStarted = false;
            UIManager.Instance.DisplayLoseScreen();

            Debug.Log("YOU LOSE");

            return;
        }
    }

    public void ResetData()
    {
        currentMaxTimeTillNextTick = originalTimeTillNextTick;
        bossBattleStarted = false;
        currentTimeTillNextTick = 0;
        currentBossHealth = 0;

        piecesToRemove.Clear();
        colorToDmg.Clear();
        symbolToDmg.Clear();
    }

    public IEnumerator delayStartBossActions()
    {
        currentBossHealth = bossLevelSO.BossHealth;


        yield return new WaitForSeconds(delayTimeBossActions);
        bossBattleStarted = true;
        UIManager.Instance.DisplayBossBattleUIScreen();

    }


    public IEnumerator DamageBoss()
    {
        PopulateTypesOfDmg();

        DmgBossCalc();



        yield return new WaitForEndOfFrame();

        foreach (Piece p in piecesToRemove)
        {
            p.GetComponentInParent<Cell>().RemovePiece(false);
            //yield return new WaitForEndOfFrame();
            Destroy(p.gameObject);
        }

        GameManager.Instance.currentFilledCellCount -= piecesToRemove.Count();

        yield return new WaitForEndOfFrame();
        piecesToRemove.Clear();
        colorToDmg.Clear();
        symbolToDmg.Clear();
    }

    public void PopulateTypesOfDmg()
    {
        foreach (Piece p in piecesToRemove)
        {
            colorToDmg.Add(p.rightChild.colorOfPiece);
            colorToDmg.Add(p.leftChild.colorOfPiece);

            symbolToDmg.Add(p.rightChild.symbolOfPiece);
            symbolToDmg.Add(p.leftChild.symbolOfPiece);
        }
    }

    public void DmgBossCalc()
    {
        /// we do not have weaknesses / vulnerabilities... but they will be calculated here!
        

        if(piecesToRemove.Count() == 2)
        {
            currentBossHealth -= twoComboBaseDMG;
        }
        else if(piecesToRemove.Count() == 3)
        {
            currentBossHealth -= twoComboBaseDMG * 3;
        }

        UIManager.Instance.UpdateBossHealth();
    }
}
