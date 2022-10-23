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
    public bool isChest;

    public int NextZoneID;///only if key level
    public int indexInCluster;

    public GameObject NextLevelVFX;

    public LevelScriptableObject connectedLevelScriptableObject;
    public ClusterScriptableObject connectedClusterScriptableObject;

    public UnityEvent interactEvent;

    private void Start()
    {
        NextLevelVFX.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!UIManager.Instance.isUsingUI)
        {
            ShootEvent();
        }
    }

    public void ShootEvent()
    {
        interactEvent.Invoke();
    }

    public void LaunchLevelDisplayData()
    {
        TestLevelsSystemManager.instance.selectedLevelButton = this;

        ZoneManager.Instance.SetCurrentZone(connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.currentIndexInCluster = indexInCluster;
                GameManager.Instance.ChooseLevel(connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum);
                GameManager.Instance.currentCluster = connectedClusterScriptableObject;
                StartCoroutine(UIManager.Instance.LaunchLevelDisplay());
            }

        }
    }

    public void ChooseTypeLevelLaunch()
    {
        UIManager.Instance.testLevelsDataScreen.SetActive(false);

        if (isTutorialLevel && isKeyLevel)
        {
            LaunchKeyAndTutorialLevel();
            //Debug.LogError("Launched tutorial + key level");
        }
        else if (isTutorialLevel)
        {
            LaunchTutorialLevel();
            //Debug.LogError("Launched tutorial level");
        }
        else if (isKeyLevel)
        {
            LaunchKeyLevel();
            //Debug.LogError("Launched key level");
        }
        else if (isGrindLevel)
        {
            LaunchGrindLevel();
            //Debug.LogError("Launched grind level");
        }
        else if (isBossLevel)
        {
            LaunchBossLevel();
            //Debug.LogError("Launched boss level");
        }
        else
        {
            LaunchNoramlLevel();
            //Debug.LogError("Launched normal level");
        }
    }

    public void LaunchNoramlLevel()
    {
        ZoneManager.Instance.SetCurrentZone(connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

                GameManager.Instance.StartLevel(true, false);
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

                GameManager.Instance.ChooseLevelGrind(connectedLevelScriptableObject.levelIndexInZone);
                GameManager.Instance.setCurrentLevelBG(connectedLevelScriptableObject.worldNum);

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);

                GameManager.Instance.StartLevel(true, false);
            }

        }
    }

    public void LaunchTutorialLevel()
    {
        ZoneManager.Instance.SetCurrentZone(connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);


                GameManager.Instance.StartTutorialLevel(true);

            }

        }
    }

    public void LaunchKeyLevel()
    {
        ZoneManager.Instance.CheckZoneAwardedKey(connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum);
        ZoneManager.Instance.SetUnlockZone(NextZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;

                TurnOffVFX();

                SoundManager.Instance.FadeOutMapBGMusic(SoundManager.Instance.timeFadeOutBGMusic, true);


                GameManager.Instance.StartLevel(true, false);

            }

        }
    }

    public void LaunchKeyAndTutorialLevel()
    {
        ZoneManager.Instance.CheckZoneAwardedKey(connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum);
        ZoneManager.Instance.SetUnlockZone(NextZoneID);

        if (ZoneManagerHelpData.Instance.currentZoneCheck.isUnlocked)
        {
            if (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone == ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone || (connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone < ZoneManagerHelpData.Instance.currentZoneCheck.maxLevelReachedInZone && ServerRelatedData.instance.canRepeatLevels))
            {
                GameManager.Instance.levelStarted = true;

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


        isTutorialLevel = false;
        isKeyLevel = false;
        isGrindLevel = false;
        isAnimalLevel = false;
        isBossLevel = false;
        isChest = false;
        indexInCluster = -1;

        for (int i = 0; i < connectedClusterScriptableObject.clusterLevels.Length; i++)
        {
            if (connectedClusterScriptableObject.clusterLevels[i] == connectedLevelScriptableObject)
            {
                indexInCluster = i;
            }
        }

        string newName = "Level " + connectedClusterScriptableObject.clusterLevels[indexInCluster].levelIndexInZone + " ";

        if ((connectedClusterScriptableObject.clusterLevels[indexInCluster].isTutorial || connectedClusterScriptableObject.clusterLevels[indexInCluster].isSpecificTutorial) && connectedClusterScriptableObject.clusterLevels[indexInCluster].isKeyLevel)
        {
            isTutorialLevel = true;
            isKeyLevel = true;
        }

        if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isTutorial || connectedClusterScriptableObject.clusterLevels[indexInCluster].isSpecificTutorial)
        {
            isTutorialLevel = true;

        }

        if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isKeyLevel)
        {
            isKeyLevel = true;

        }

        if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isGrindLevel)
        {
            isGrindLevel = true;
        }

        if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isAnimalLevel)
        {
            isAnimalLevel = true;
        }

        
        if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isBoss)
        {
            isBossLevel = true;
        }

        if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isBoss)
        {
            isBossLevel = true;
        }

        if(connectedLevelScriptableObject == connectedClusterScriptableObject.clusterLevels[connectedClusterScriptableObject.clusterLevels.Length - 1])
        {
            isChest = true;
        }

        if (isTutorialLevel || connectedClusterScriptableObject.clusterLevels[indexInCluster].isSpecificTutorial)
        {
            if (connectedClusterScriptableObject.clusterLevels[indexInCluster].isSpecificTutorial)
            {
                newName += " Specific Tutorial" + " " + connectedClusterScriptableObject.clusterLevels[indexInCluster].specificTutorialEnum;
            }
            else
            {
                newName += " Tutorial" + " ";
            }
        }


        if (isGrindLevel)
        {
            newName += " Grind" + " ";

        }

        if (isKeyLevel)
        {
            newName += " Key" + " ";
        }

        if (isAnimalLevel)
        {
            newName += " Animal" + " ";
        }

        if (isBossLevel)
        {
            newName += " Boss" + " ";
        }

        if (isChest)
        {
            newName += " is chest";
        }

        transform.name = newName;

        NextZoneID = connectedClusterScriptableObject.clusterLevels[indexInCluster].worldNum + 1;
    }
}
