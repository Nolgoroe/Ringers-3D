using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Interactable3D : MonoBehaviour
{
    public bool isTutorialLevel;
    public bool isKeyLevel;
    public bool isGrindLevel;

    public int currentZoneID;
    public int NextZoneID;///only if key level
    public int indexInZone;

    public UnityEvent interactEvent;

    public GameObject NextLevelVFX;

    private void Start()
    {
        NextLevelVFX.SetActive(false);
    }
    public void ShootEvent()
    {
        interactEvent.Invoke();
    }

    public void LaunchNoramlLevel()
    {
        ZoneManager.Instance.SetCurrentZone(currentZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (indexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && CheatingSaveData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;
                //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                GameManager.Instance.ChooseLevel(indexInZone);
                GameManager.Instance.setCurrentLevelBG(currentZoneID);
                //GameManager.Instance.CallStartLevel(false);

                TurnOffVFX();

                GameManager.Instance.StartLevel(true);
            }

        }
    }

    public void LaunchGrindLevel()
    {
            ZoneManager.Instance.SetCurrentZone(currentZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind)
                {
                    GameManager.Instance.levelStarted = true;
                    //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                    GameManager.Instance.ChooseLevelGrind(indexInZone);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(false);

                    TurnOffVFX();

                    GameManager.Instance.StartLevel(true);
                }

            }
    }

    public void LaunchTutorialLevel()
    {
            ZoneManager.Instance.SetCurrentZone(currentZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (indexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && CheatingSaveData.instance.canRepeatLevels))
                {
                    GameManager.Instance.levelStarted = true;
                    //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                    GameManager.Instance.ChooseLevel(indexInZone);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(true);

                    TurnOffVFX();

                    GameManager.Instance.StartTutorialLevel(true);

                }

            }
    }

    public void LaunchKeyLevel()
    {
            ZoneManager.Instance.CheckZoneAwardedKey(currentZoneID);
            ZoneManager.Instance.SetUnlockZone(NextZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (indexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && CheatingSaveData.instance.canRepeatLevels))
                {
                    GameManager.Instance.levelStarted = true;
                    //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                    GameManager.Instance.ChooseLevel(indexInZone);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(false);

                    TurnOffVFX();

                    GameManager.Instance.StartLevel(true);

                }

            }
    }

    public void LaunchKeyAndTutorialLevel()
    {
            ZoneManager.Instance.CheckZoneAwardedKey(currentZoneID);
            ZoneManager.Instance.SetUnlockZone(NextZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (indexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && CheatingSaveData.instance.canRepeatLevels))
                {
                    GameManager.Instance.levelStarted = true;
                    //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                    GameManager.Instance.ChooseLevel(indexInZone);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(true);

                    TurnOffVFX();

                    GameManager.Instance.StartTutorialLevel(true);

                }

            }
    }

    public void LaunchBossLevel(LevelScriptableObject bossLevel)
    {
            if (PlayerManager.Instance.bossTicketCount > 0)
            {
                GameManager.Instance.levelStarted = true;
                GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                PlayerManager.Instance.bossTicketCount--;
                ZoneManager.Instance.SetCurrentZone(0);

                BossBattleManager.instance.bossLevelSO = bossLevel;
                GameManager.Instance.currentLevel = bossLevel;
                GameManager.Instance.setCurrentLevelBG(0);
                GameManager.Instance.startBossLevel(true);
            }
    }

    public void TurnOnVFX()
    {
        NextLevelVFX.SetActive(true);
    }

    public void TurnOffVFX()
    {
        NextLevelVFX.SetActive(false);
    }

    [ContextMenu("Rename")]
    public void RenameObject()
    {
        string newName = "Level " + indexInZone + " ";

        if (isTutorialLevel)
        {
            newName += "Tutorial" + " ";
        }

        if (isGrindLevel)
        {
            newName += "Grind" + " ";

        }

        if (isKeyLevel)
        {
            newName += "Key" + " ";
        }

        transform.name = newName;
    }
}
