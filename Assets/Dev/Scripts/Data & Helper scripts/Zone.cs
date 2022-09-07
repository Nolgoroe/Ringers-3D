using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


[System.Serializable]
public class Zone : MonoBehaviour
{
    public int id;
    public int keyLevelIndex;
    public int grindLevelIndex;
    public string zoneName;
    public bool hasAwardedKey, isUnlocked, hasUnlockedGrind, isTestZone;

    public int maxLevelReachedInZone;
    public int lastLevelNum;

    string path;

    public Color levelDoneColor, levelFirstTimeColor;

    [HideInInspector]
    public Image zoneHeader;

    public void Init()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Grind Level Button"))
            {
                ZoneManagerHelpData.Instance.zoneGrindLevelPerZone.Add(child.gameObject);
            }
        }
    }
}
