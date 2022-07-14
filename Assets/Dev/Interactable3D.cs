using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

public class Interactable3D : MonoBehaviour, IPointerClickHandler
{
    public bool isTutorialLevel;
    public bool isKeyLevel;
    public bool isGrindLevel;
    public bool isAnimalLevel;
    public bool isBossLevel;

    public int NextZoneID;///only if key level
    //public int indexInZone;

    public GameObject NextLevelVFX;

    public LevelScriptableObject connectedLevelScriptableObject;

    public UnityEvent interactEvent;

    private void Start()
    {
        NextLevelVFX.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShootEvent();
    }

    public void ShootEvent()
    {
        interactEvent.Invoke();
    }

    public void ChooseTypeLevelLaunch()
    {
        if(isTutorialLevel && isKeyLevel)
        {
            LaunchKeyAndTutorialLevel();
            Debug.LogError("Launched tutorial + key level");
        }
        else if (isTutorialLevel)
        {
            LaunchTutorialLevel();
            Debug.LogError("Launched tutorial level");
        }
        else if (isKeyLevel)
        {
            LaunchKeyLevel();
            Debug.LogError("Launched key level");
        }
        else if (isGrindLevel)
        {
            LaunchGrindLevel();
            Debug.LogError("Launched grind level");
        }
        else if (isBossLevel)
        {
            LaunchBossLevel();
            Debug.LogError("Launched boss level");
        }
        else
        {
            LaunchNoramlLevel();
            Debug.LogError("Launched normal level");
        }
    }

    public void LaunchNoramlLevel()
    {
        ZoneManager.Instance.SetCurrentZone(connectedLevelScriptableObject.worldNum);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedLevelScriptableObject.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedLevelScriptableObject.levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;
                //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                GameManager.Instance.ChooseLevel(connectedLevelScriptableObject.levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedLevelScriptableObject.worldNum);
                //GameManager.Instance.CallStartLevel(false);

                TurnOffVFX();

                //StartCoroutine(SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true));
                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

                GameManager.Instance.StartLevel(true);
            }

        }
    }

    public void LaunchGrindLevel()
    {
        ZoneManager.Instance.SetCurrentZone(connectedLevelScriptableObject.worldNum);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (ZoneManagerHelpData.Instance.currentZoneCheck.hasUnlockedGrind)
            {
                GameManager.Instance.levelStarted = true;
                //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                GameManager.Instance.ChooseLevelGrind(connectedLevelScriptableObject.levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedLevelScriptableObject.worldNum);
                //GameManager.Instance.CallStartLevel(false);

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

                GameManager.Instance.StartLevel(true);
            }

        }
    }

    public void LaunchTutorialLevel()
    {
        ZoneManager.Instance.SetCurrentZone(connectedLevelScriptableObject.worldNum);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedLevelScriptableObject.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedLevelScriptableObject.levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;
                //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                GameManager.Instance.ChooseLevel(connectedLevelScriptableObject.levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedLevelScriptableObject.worldNum);
                //GameManager.Instance.CallStartLevel(true);

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);


                GameManager.Instance.StartTutorialLevel(true);

            }

        }
    }

    public void LaunchKeyLevel()
    {
        ZoneManager.Instance.CheckZoneAwardedKey(connectedLevelScriptableObject.worldNum);
        ZoneManager.Instance.SetUnlockZone(NextZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedLevelScriptableObject.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedLevelScriptableObject.levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;
                //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                GameManager.Instance.ChooseLevel(connectedLevelScriptableObject.levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedLevelScriptableObject.worldNum);
                //GameManager.Instance.CallStartLevel(false);

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);


                GameManager.Instance.StartLevel(true);

            }

        }
    }

    public void LaunchKeyAndTutorialLevel()
    {
        ZoneManager.Instance.CheckZoneAwardedKey(connectedLevelScriptableObject.worldNum);
        ZoneManager.Instance.SetUnlockZone(NextZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedLevelScriptableObject.levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedLevelScriptableObject.levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;
                //GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

                GameManager.Instance.ChooseLevel(connectedLevelScriptableObject.levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedLevelScriptableObject.worldNum);
                //GameManager.Instance.CallStartLevel(true);

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);


                GameManager.Instance.StartTutorialLevel(true);

            }

        }
    }

    public void LaunchBossLevel()
    {
        if (PlayerManager.Instance.bossTicketCount > 0)
        {
            GameManager.Instance.levelStarted = true;
            GameManager.Instance.timeStartLevel = DateTime.Now.ToString("HH:mm:ss");

            PlayerManager.Instance.bossTicketCount--;
            ZoneManager.Instance.SetCurrentZone(0);

            BossBattleManager.instance.bossLevelSO = connectedLevelScriptableObject;
            GameManager.Instance.currentLevel = connectedLevelScriptableObject;
            GameManager.Instance.setCurrentLevelBG(0);


            SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

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
        string newName = "Level " + connectedLevelScriptableObject.levelIndexInZone + " ";


        isTutorialLevel = false;
        isKeyLevel = false;
        isGrindLevel = false;
        isAnimalLevel = false;
        isBossLevel = false;

        if ((connectedLevelScriptableObject.isTutorial || connectedLevelScriptableObject.isSpecificTutorial) && connectedLevelScriptableObject.isKeyLevel)
        {
            isTutorialLevel = true;
            isKeyLevel = true;
        }

        if (connectedLevelScriptableObject.isTutorial || connectedLevelScriptableObject.isSpecificTutorial)
        {
            isTutorialLevel = true;

        }

        if (connectedLevelScriptableObject.isKeyLevel)
        {
            isKeyLevel = true;

        }

        if (connectedLevelScriptableObject.isGrindLevel)
        {
            isGrindLevel = true;
        }

        if (connectedLevelScriptableObject.isAnimalLevel)
        {
            isAnimalLevel = true;
        }

        
        if (connectedLevelScriptableObject.isBoss)
        {
            isBossLevel = true;
        }

        if (isTutorialLevel || connectedLevelScriptableObject.isSpecificTutorial)
        {
            if (connectedLevelScriptableObject.isSpecificTutorial)
            {
                newName += "Specific Tutorial" + " " + connectedLevelScriptableObject.specificTutorialEnum;
            }
            else
            {
                newName += "Tutorial" + " ";
            }
        }

        if (isGrindLevel)
        {
            newName += "Grind" + " ";

        }

        if (isKeyLevel)
        {
            newName += "Key" + " ";
        }

        if (isAnimalLevel)
        {
            newName += "Animal" + " ";
        }

        if (isBossLevel)
        {
            newName += "Boss" + " ";
        }

        transform.name = newName;

        NextZoneID = connectedLevelScriptableObject.worldNum + 1;
    }

}
