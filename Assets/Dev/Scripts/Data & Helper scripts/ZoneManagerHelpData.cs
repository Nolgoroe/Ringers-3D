using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[Serializable]
public class IconSpritesPerZone
{
    public Zone zone;
    public Sprite levelDoneSprite, levelFirstTimeIconSprite, nextLevelSprite, grindLevelSprite;
}
[Serializable]
public class IconSpritesPerCondition
{
    public bool isTutorial;
    public bool isAnimal;
    public bool isChest;
    public bool isTimedLevel;
    public Sprite levelDoneSprite, levelFirstTimeIconSprite, nextLevelSprite;
}
[Serializable]
public class AmbientMusicPerZone
{
    public Zone zone;
    public Sounds levelAmbience;
}

[Serializable]
public class ZoneAndObjectToBlurUnblur
{
    public Zone zone;
    public GameObject[] planesToChangeFront;
    public GameObject[] planesToChangeMiddle;
    public SpriteRenderer[] BGToChange;

    public Material blurMat;
    public Material normalMat;
    public Sprite blurBGSprite;
    public Sprite normalBGSprite;
}

public class ZoneManagerHelpData : MonoBehaviour
{
    public static ZoneManagerHelpData Instance;

    public string currentZoneName;

    public GameObject[] zoneLevelDisplays;

    public Zone[] listOfAllZones;


    public Zone currentZoneCheck, nextZoneCheck;

    public animalsPerZone[] possibleAnimalsPerZone;

    public IconSpritesPerZone[] iconsPerZone;
    public IconSpritesPerCondition[] iconsPerConditon;

    public AmbientMusicPerZone[] musicPerZone;
    public ZoneAndObjectToBlurUnblur[] blurUnblurPerZone;

    public Vector3[] unlockPosPerZone;

    public List<GameObject> zoneGrindLevelPerZone;

    public GameObject testZone;

    //public Sprite bossLevelSpriteFirstTime, bossLevelSpriteDone;
    private void Start()
    {
        Instance = this;

        testZone.SetActive(false);

        ZoneManager.Instance.ActivateLevelDisplay();
        currentZoneName = null;
    }

    public bool CompareZonesSame(Zone first, Zone second)
    {
        if(first.Equals(second))
        {
            return true;
        }

        return false;
    }

    public void CheatUnlockAllLevels()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        foreach (Zone z in listOfAllZones)
        {
            if(z.isTestZone)
            {
                continue;
            }

            z.hasAwardedKey = true;
            z.isUnlocked = true;

            foreach (GameObject go in zoneGrindLevelPerZone)
            {
                if (go.GetComponent<Interactable3D>())
                {
                    Interactable3D interactable = go.GetComponent<Interactable3D>();

                    if (interactable.connectedLevelScriptableObject.worldNum == z.id)
                    {
                        z.hasUnlockedGrind = true;
                    }
                }
            }
            z.maxLevelReachedInZone = z.lastLevelNum;

            if (!ZoneManager.Instance.unlockedZoneID.Contains(z.id))
            {
                ZoneManager.Instance.unlockedZoneID.Add(z.id);
            }

            ZoneManager.Instance.UpdateMaxZoneIdReached(ZoneManager.Instance.unlockedZoneID[ZoneManager.Instance.unlockedZoneID.Count -1]);

            Interactable3D[] array = z.GetComponentsInChildren<Interactable3D>();

            foreach (Interactable3D child in array)
            {
                child.NextLevelVFX.SetActive(false);
            }

        }


        UIManager.Instance.UnlockLevels();

        PlayerManager.Instance.rubyCount = 5000;
        PlayerManager.Instance.collectedDewDrops = 5000;

        UIManager.Instance.updateRubyAndDewDropsCount();

        MaterialsAndForgeManager.Instance.UnlockAllPotions();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.ZoneManager, SystemsToSave.ZoneX});
    }


    public void ChangeZoneToBlurryZoneDisplay()
    {
        ZoneAndObjectToBlurUnblur ZOB = blurUnblurPerZone.Where(p => p.zone == currentZoneCheck).SingleOrDefault();

        if (ZOB != null)
        {
            foreach (GameObject go in ZOB.planesToChangeFront)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();

                renderer.material = ZOB.blurMat;
            }

            foreach (GameObject go in ZOB.planesToChangeMiddle)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();

                renderer.material = ZOB.blurMat;
            }

            foreach (SpriteRenderer sr in ZOB.BGToChange)
            {
                sr.sprite = ZOB.blurBGSprite;
            }
        }
    }
    public void ChangeZoneToNormalZoneDisplay()
    {
        ZoneAndObjectToBlurUnblur ZOB = blurUnblurPerZone.Where(p => p.zone == currentZoneCheck).SingleOrDefault();

        if (ZOB != null)
        {
            foreach (GameObject go in ZOB.planesToChangeFront)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();

                renderer.material = ZOB.normalMat;
            }

            foreach (GameObject go in ZOB.planesToChangeMiddle)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();

                renderer.material = ZOB.normalMat;
            }

            foreach (SpriteRenderer sr in ZOB.BGToChange)
            {
                sr.sprite = ZOB.normalBGSprite;
            }
        }
    }
}
