using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IconSpritesPerZone
{
    public Zone zone;
    public Sprite levelDoneSprite, levelFirstTimeIconSprite, nextLevelSprite, grindLevelSprite;
}
[Serializable]
public class AmbientMusicPerZone
{
    public Zone zone;
    public Sounds levelAmbience;
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

    public AmbientMusicPerZone[] musicPerZone;

    public Vector3[] unlockPosPerZone;

    public List<GameObject> zoneGrindLevelPerZone;

    private void Start()
    {
        Instance = this;    


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
        foreach (Zone z in listOfAllZones)
        {
            z.hasAwardedKey = true;
            z.isUnlocked = true;

            foreach (GameObject go in zoneGrindLevelPerZone)
            {
                if (go.GetComponent<Interactable3D>())
                {
                    Interactable3D interactable = go.GetComponent<Interactable3D>();

                    if (interactable.currentZoneID == z.id)
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

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.ZoneManager, SystemsToSave.ZoneX});
    }
}
