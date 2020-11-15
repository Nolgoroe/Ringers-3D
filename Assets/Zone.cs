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
    public string zoneName;
    public bool hasAwardedKey, isUnlocked;

    public int maxLevelReachedInZone;
    public int lastLevelNum;

    string path;

    public Sprite levelDone, levelFirstTimeIcon, unlockedZone, lockedZone;
    public Image zoneHeader;
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/Zone" + id + ".txt";
        }
        else
        {
            path = Application.dataPath + "/Zone" + id + ".txt";
        }

        if (File.Exists(path))
        {
            LoadZone();
        }

        if (isUnlocked)
        {
            zoneHeader.sprite = unlockedZone;
        }
        else
        {
            zoneHeader.sprite = lockedZone;
        }
    }
    [ContextMenu("Save Game Data")]
    public void SaveZone()
    {
        string savedData = JsonUtility.ToJson(this);

        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath + "/Zone" + id + ".txt";
        }
        else
        {
            string path = Application.dataPath + "/Zone" + id + ".txt";
        }
        File.WriteAllText(path, savedData);
    }

    [ContextMenu("Game Load Data")]
    public void LoadZone()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath + "/Zone" + id + ".txt";
        }
        else
        {
            string path = Application.dataPath + "/Zone" + id + ".txt";
        }
        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);
    }
}
