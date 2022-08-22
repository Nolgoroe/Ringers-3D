using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameAnalyticsSDK;
using UnityEngine.UI;

public class BossBattleManager : MonoBehaviour
{
    public static BossBattleManager instance;

    [Header("Boss Gameplay Data")]
    public int currentBossHealth;

    public int twoComboBaseDMG;

    public int damageDealtToBossCurrentFight;

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


    [Header("Boss V2 General Settings")]
    public List<GameObject> completedRings;
    public Transform completedRingsParent;

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
        if (bossBattleStarted && !UIManager.Instance.isUsingUI)
        {
            if (bossLevelSO.ver1Boss)
            {
                CheckEndLevelBossVersionOne();

                currentTimeTillNextTick += Time.fixedDeltaTime;

                if (currentTimeTillNextTick >= currentMaxTimeTillNextTick)
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
    }


    public void BossAddRandomPieceToBoard()
    {
        Cell[] emptyCells = ConnectionManager.Instance.cells.Where(p => p.isFull == false).ToArray();

        int randomCell = Random.Range(0, emptyCells.Length);

        emptyCells[randomCell].AddPieceRandom();
    }

    public void CheckEndLevelBossVersionOne()
    {
        if(currentBossHealth <= 0)
        {
            bossBattleStarted = false;
            UIManager.Instance.DisplayBossWinScreen();
            BossesSaveDataManager.instance.BossOneSaveHP = 0;

            Debug.Log("YOU WIN");

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.BossesSaveData });

            return;
        }

        if (GameManager.Instance.currentFilledCellCount == bossLevelSO.cellsCountInLevel)
        {
            bossBattleStarted = false;
            UIManager.Instance.DisplayBossWellDoneScreen();
            BossesSaveDataManager.instance.BossOneSaveHP = currentBossHealth;

            Debug.Log("YOU LOSE");

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.BossesSaveData });

            return;
        }
    }

    public void ResetDataBossVer1()
    {
        currentMaxTimeTillNextTick = originalTimeTillNextTick;
        bossBattleStarted = false;
        currentTimeTillNextTick = 0;
        currentBossHealth = 0;
        damageDealtToBossCurrentFight = 0;

        piecesToRemove.Clear();
        colorToDmg.Clear();
        symbolToDmg.Clear();
    }
    public void NextRing()
    {
        GameManager.Instance.gameBoard.name = "Done ring";
        GameManager.Instance.gameBoard.SetActive(false);

        GameManager.Instance.gameBoard = Instantiate(bossLevelSO.boardPrefab, GameManager.Instance.destroyOutOfLevel);
        GameManager.Instance.gameBoard.transform.position = new Vector3(GameManager.Instance.gameBoard.transform.position.x, 1.45f, GameManager.Instance.gameBoard.transform.position.z);

        GameManager.Instance.sliceManager = GameManager.Instance.gameBoard.GetComponent<SliceManager>();

        ConnectionManager.Instance.ResetConnectionData();

        ConnectionManager.Instance.GrabCellList(GameManager.Instance.gameBoard.transform);
        ConnectionManager.Instance.SetLevelConnectionData(bossLevelSO.is12PieceRing);

        GameManager.Instance.sliceManager.SpawnSlices(bossLevelSO.slicesToSpawn.Length);

        GameManager.Instance.currentFilledCellCount = 0;
        GameManager.Instance.unsuccessfullConnectionCount = 0;



        GameManager.Instance.clipManager.PopulateSlot(GameManager.Instance.clipManager.emptyClip, 10);
    }
    public void EndDataBossVer2()
    {
        if (GameManager.Instance.gameBoard)
        {
            Destroy(GameManager.Instance.gameBoard.gameObject);
        }

        ConnectionManager.Instance.ResetConnectionData();

        ConnectionManager.Instance.SetLevelConnectionData(bossLevelSO.is12PieceRing);

        GameManager.Instance.currentFilledCellCount = 0;
        GameManager.Instance.unsuccessfullConnectionCount = 0;
    }

    public IEnumerator delayStartBossActions()
    {
        if(BossesSaveDataManager.instance.BossOneSaveHP > 0)
        {
            currentBossHealth = BossesSaveDataManager.instance.BossOneSaveHP;
        }
        else
        {
            currentBossHealth = bossLevelSO.BossHealth;
        }

        yield return new WaitForSeconds(delayTimeBossActions);
        bossBattleStarted = true;
        UIManager.Instance.DisplayBossBattleUIScreen();

    }
    public IEnumerator delayStartBossActionsVer2()
    {
        if(BossesSaveDataManager.instance.BossTwoSaveHP > 0)
        {
            currentBossHealth = BossesSaveDataManager.instance.BossTwoSaveHP;
        }
        else
        {
            currentBossHealth = bossLevelSO.BossHealth;
        }

        yield return new WaitForSeconds(delayTimeBossActions);
        bossBattleStarted = true;
        UIManager.Instance.DisplayBossBattleUIScreen();
        DisplayTimeNoDelay();
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

            damageDealtToBossCurrentFight += twoComboBaseDMG;
        }
        else if(piecesToRemove.Count() == 3)
        {
            currentBossHealth -= twoComboBaseDMG * 3;
            damageDealtToBossCurrentFight += twoComboBaseDMG * 3;
        }

        UIManager.Instance.UpdateBossHealth();
    }
    public void DmgBossCalcBossVer2()
    {
        /// we do not have weaknesses / vulnerabilities... but they will be calculated here!
        currentBossHealth -= (int)GameManager.Instance.currentLevel.damageToBossCompeleRing;
        damageDealtToBossCurrentFight += (int)GameManager.Instance.currentLevel.damageToBossCompeleRing;

        UIManager.Instance.UpdateBossHealth();

        if (currentBossHealth <= 0)
        {
            GameManager.Instance.gameBoard = null;

            bossBattleStarted = false;
            UIManager.Instance.DisplayBossWinScreen();
            BossesSaveDataManager.instance.BossTwoSaveHP = 0;
            OnEndBossVer2Fight();

            Debug.Log("YOU WIN");

            return;
        }
    }


    public IEnumerator DealAnimationBossVer2()
    {
        UIManager.Instance.dealButton.interactable = false;

        Image dealButtonImage = UIManager.Instance.dealButton.GetComponent<Image>();
        dealButtonImage.fillAmount = 0;

        LeanTween.value(dealButtonImage.gameObject, dealButtonImage.fillAmount, 1, GameManager.Instance.currentLevel.dealButtonTimer).setOnComplete(() => UIManager.Instance.dealButton.interactable = true).setOnUpdate((float val) =>
        {
            dealButtonImage.fillAmount = val;
        });

        for (int i = 0; i < GameManager.Instance.clipManager.clipCount; i++)
        {
            GameObject toMove = GameManager.Instance.clipManager.slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, GameManager.Instance.clipManager.piecesDealPositionsOut, GameManager.Instance.clipManager.timeToAnimateMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate

            SoundManager.Instance.PlaySound(Sounds.PieceMoveDeal);

            yield return new WaitForSeconds(GameManager.Instance.clipManager.delayClipMove);
        }


        yield return new WaitForSeconds(GameManager.Instance.clipManager.WaitTimeBeforeIn);
        GameManager.Instance.clipManager.DealAnimClipLogic();

        for (int i = GameManager.Instance.clipManager.clipCount - 1; i > -1; i--)
        {
            GameObject toMove = GameManager.Instance.clipManager.slots[i].GetChild(1).gameObject;

            LeanTween.move(toMove, GameManager.Instance.clipManager.originalPiecePos, GameManager.Instance.clipManager.timeToAnimateMove).setEase(LeanTweenType.easeInOutQuad).setMoveLocal(); // animate

            Invoke("playReturnPiecePlaceSound", GameManager.Instance.clipManager.timeToAnimateMove - 0.25f);

            yield return new WaitForSeconds(GameManager.Instance.clipManager.delayClipMove);

        }
    }

    void playReturnPiecePlaceSound()
    {
        SoundManager.Instance.PlaySound(Sounds.PieceMoveDeal);
    }


    void DisplayTimeNoDelay() ///// This function is only for the start of the game so that players wont see the defult time while the real time is updating
    {
        float m = GameManager.Instance.currentLevel.timeForLevelInSeconds % 3600;
        float minutes = Mathf.FloorToInt(m / 60);
        float seconds = Mathf.FloorToInt(GameManager.Instance.currentLevel.timeForLevelInSeconds % 60);

        UIManager.Instance.bossV2TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        StartCoroutine(DisplayTimeLiveBossV2());
    }

    IEnumerator DisplayTimeLiveBossV2()
    {
        float currentTimer = GameManager.Instance.currentLevel.timeForLevelInSeconds;

        while (currentTimer > 0)
        {
            UIManager.Instance.dailyLootTextTime.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(1);

            currentTimer--;

            float m = currentTimer % 3600;
            float minutes = Mathf.FloorToInt(m / 60);
            float seconds = Mathf.FloorToInt(currentTimer % 60);

            UIManager.Instance.bossV2TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (currentBossHealth <= 0)
            {
                break;
            }
        }

        if (currentBossHealth > 0)
        {
            bossBattleStarted = false;
            UIManager.Instance.DisplayBossWellDoneScreen();
            BossesSaveDataManager.instance.BossTwoSaveHP = currentBossHealth;
            OnEndBossVer2Fight();

            Debug.Log("YOU LOSE");
        }
    }

    void OnEndBossVer2Fight()
    {
        EndDataBossVer2();

        foreach (GameObject go in completedRings)
        {
            go.SetActive(true);
        }

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.BossesSaveData});
    }
    public IEnumerator CheckCompletedRingVer2Boss()
    {
        yield return new WaitForSeconds(0.1f);

        if (GameManager.Instance.currentFilledCellCount == GameManager.Instance.currentLevel.cellsCountInLevel && GameManager.Instance.unsuccessfullConnectionCount == 0 && GameManager.Instance.unsuccessfullSlicesCount == 0)
        {
            completedRings.Add(GameManager.Instance.gameBoard);
            GameManager.Instance.gameBoard.transform.SetParent(completedRingsParent);
            Destroy(GameManager.Instance.sliceManager.particleZonesParent.gameObject);
            Destroy(GameManager.Instance.sliceManager.endLevelAnimVFX.gameObject);
            GameManager.Instance.gameBoard.AddComponent<RectTransform>();
            GameManager.Instance.gameBoard.transform.localScale = new Vector3(25, 25, 25);


            DmgBossCalcBossVer2();


            if (currentBossHealth > 0)
            {
                NextRing();
            }
        }
        else
        {
            UIManager.Instance.DisplayEndLevelMessage();
        }
    }
}
