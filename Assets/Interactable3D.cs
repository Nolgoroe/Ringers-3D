using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable3D : MonoBehaviour
{
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

        if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
        {
            GameManager.Instance.ChooseLevel(overallLevelNum);
            UIManager.Instance.ActivateGmaeplayCanvas();
            GameManager.Instance.StartLevel();
        }
    }

    public void LaunchGrindLevel()
    {
        ZoneManager.Instance.SetCurrentZone(currentZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind)
        {
            GameManager.Instance.ChooseLevelGrind(overallLevelNum);
            UIManager.Instance.ActivateGmaeplayCanvas();
            GameManager.Instance.StartLevel();
        }
    }

    public void LaunchTutorialLevel()
    {
        ZoneManager.Instance.SetCurrentZone(currentZoneID);

        if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
        {
            GameManager.Instance.ChooseLevel(overallLevelNum);
            UIManager.Instance.ActivateGmaeplayCanvas();
            GameManager.Instance.StartTutorialLevel();
        }
    }

    public void LaunchKeyLevel()
    {
        ZoneManager.Instance.CheckZoneAwardedKey(currentZoneID);
        ZoneManager.Instance.SetUnlockZone(NextZoneID);

        if (indexInZone <= ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone)
        {
            GameManager.Instance.ChooseLevel(overallLevelNum);
            UIManager.Instance.ActivateGmaeplayCanvas();
            GameManager.Instance.StartLevel();
        }
    }
}
