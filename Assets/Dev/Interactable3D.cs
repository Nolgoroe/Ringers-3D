using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable3D : MonoBehaviour
{
    public bool isTutorialLevel;
    public bool isKeyLevel;
    public bool isGrindLevel;

    public int currentZoneID;
    public int NextZoneID;///only if key level
    public int overallLevelNum;
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
        if (!GameManager.Instance.levelStarted && GameManager.Instance.clickedPlayButton)
        {
            ZoneManager.Instance.SetCurrentZone(currentZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || GameManager.Instance.canRepeatLevels)
                {
                    GameManager.Instance.levelStarted = true;

                    GameManager.Instance.ChooseLevel(overallLevelNum);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(false);

                    TurnOffVFX();

                    GameManager.Instance.StartLevel(true);
                }

            }
        }
    }

    public void LaunchGrindLevel()
    {
        if (!GameManager.Instance.levelStarted && GameManager.Instance.clickedPlayButton)
        {
            ZoneManager.Instance.SetCurrentZone(currentZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind)
                {
                    GameManager.Instance.levelStarted = true;

                    GameManager.Instance.ChooseLevelGrind(overallLevelNum);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(false);

                    TurnOffVFX();

                    GameManager.Instance.StartLevel(true);
                }

            }
        }
    }

    public void LaunchTutorialLevel()
    {
        if (!GameManager.Instance.levelStarted && GameManager.Instance.clickedPlayButton)
        {
            ZoneManager.Instance.SetCurrentZone(currentZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || GameManager.Instance.canRepeatLevels)
                {
                    GameManager.Instance.levelStarted = true;

                    GameManager.Instance.ChooseLevel(overallLevelNum);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(true);

                    TurnOffVFX();

                    GameManager.Instance.StartTutorialLevel(true);

                }

            }
        }
    }

    public void LaunchKeyLevel()
    {
        if (!GameManager.Instance.levelStarted && GameManager.Instance.clickedPlayButton)
        {
            ZoneManager.Instance.CheckZoneAwardedKey(currentZoneID);
            ZoneManager.Instance.SetUnlockZone(NextZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || GameManager.Instance.canRepeatLevels)
                {
                    GameManager.Instance.levelStarted = true;

                    GameManager.Instance.ChooseLevel(overallLevelNum);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(false);

                    TurnOffVFX();

                    GameManager.Instance.StartLevel(true);

                }

            }
        }
    }

    public void LaunchKeyAndTutorialLevel()
    {
        if (!GameManager.Instance.levelStarted && GameManager.Instance.clickedPlayButton)
        {
            ZoneManager.Instance.CheckZoneAwardedKey(currentZoneID);
            ZoneManager.Instance.SetUnlockZone(NextZoneID);

            if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
            {
                if (indexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || GameManager.Instance.canRepeatLevels)
                {
                    GameManager.Instance.levelStarted = true;

                    GameManager.Instance.ChooseLevel(overallLevelNum);
                    GameManager.Instance.setCurrentLevelBG(currentZoneID);
                    //GameManager.Instance.CallStartLevel(true);

                    TurnOffVFX();

                    GameManager.Instance.StartTutorialLevel(true);

                }

            }
        }
    }

    public void LaunchBossLevel(LevelScriptableObject bossLevel)
    {
        if (!GameManager.Instance.levelStarted && GameManager.Instance.clickedPlayButton)
        {
            if (PlayerManager.Instance.bossTicketCount > 0)
            {
                GameManager.Instance.levelStarted = true;

                PlayerManager.Instance.bossTicketCount--;
                ZoneManager.Instance.SetCurrentZone(0);

                BossBattleManager.instance.bossLevelSO = bossLevel;
                GameManager.Instance.currentLevel = bossLevel;
                GameManager.Instance.setCurrentLevelBG(0);
                GameManager.Instance.startBossLevel(true);
            }
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
