using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable3D : MonoBehaviour
{
    public bool isTutorialLevel;
    public int currentZoneID;
    public int NextZoneID;///only if key level
    public int overallLevelNum;
    public int indexInZone;

    public UnityEvent interactEvent;

    public void ShootEvent()
    {
        interactEvent.Invoke();
    }


    public void LaunchNoramlLevel()
    {
        ZoneManager.Instance.SetCurrentZone(currentZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                GameManager.Instance.ChooseLevel(overallLevelNum);
                GameManager.Instance.setCurrentLevelBG(currentZoneID);
                //GameManager.Instance.CallStartLevel(false);
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
                GameManager.Instance.ChooseLevelGrind(overallLevelNum);
                GameManager.Instance.setCurrentLevelBG(currentZoneID);
                //GameManager.Instance.CallStartLevel(false);
                GameManager.Instance.StartLevel(true);
            }

        }

    }

    public void LaunchTutorialLevel()
    {
        ZoneManager.Instance.SetCurrentZone(currentZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                GameManager.Instance.ChooseLevel(overallLevelNum);
                GameManager.Instance.setCurrentLevelBG(currentZoneID);
                //GameManager.Instance.CallStartLevel(true);
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
            if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                GameManager.Instance.ChooseLevel(overallLevelNum);
                GameManager.Instance.setCurrentLevelBG(currentZoneID);
                //GameManager.Instance.CallStartLevel(false);
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
            if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
            {
                GameManager.Instance.ChooseLevel(overallLevelNum);
                GameManager.Instance.setCurrentLevelBG(currentZoneID);
                //GameManager.Instance.CallStartLevel(true);
                GameManager.Instance.StartTutorialLevel(true);

            }

        }
    }


    public void LaunchBossLevel(LevelScriptableObject bossLevel)
    {
        if(PlayerManager.Instance.bossTicketCount > 0)
        {
            PlayerManager.Instance.bossTicketCount--;
            ZoneManager.Instance.SetCurrentZone(0);

            BossBattleManager.instance.bossLevelSO = bossLevel;
            GameManager.Instance.currentLevel = bossLevel;
            GameManager.Instance.setCurrentLevelBG(0);
            GameManager.Instance.startBossLevel(true);
        }
    }
}
