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
    public bool hasAwardedKey, isUnlocked, hasUnlockedGrind;

    public int maxLevelReachedInZone;
    public int lastLevelNum;

    string path;

    public string levelDonePath, levelFirstTimeIconPath/*, unlockedZonePath*//*, lockedZonePath*/;

    public Color levelDoneColor, levelFirstTimeColor;

    public GameObject zoneGrindLevel;

    //public GameObject zoneBackground;

    [HideInInspector]
    public Image zoneHeader;

    //private void Start()
    //{
    //    //if (Application.platform == RuntimePlatform.Android)
    //    //{
    //    //    path = Application.persistentDataPath + "/Zone" + id + ".txt";
    //    //}
    //    //else
    //    //{
    //    //    path = Application.dataPath + "/Save Files Folder/Zone" + id + ".txt";
    //    //}

    //    //if (File.Exists(path))
    //    //{
    //    //    LoadZone();
    //    //}

    //    //foreach (Transform child in transform)
    //    //{
    //    //    if(child.CompareTag("Zone Header"))
    //    //    {
    //    //        zoneHeader = child.GetComponent<Image>();
    //    //    }
    //    //}
    //    //if (isUnlocked)
    //    //{
    //    //    zoneHeader.sprite = Resources.Load<Sprite>(unlockedZonePath);
    //    //}
    //    //else
    //    //{
    //    //    zoneHeader.sprite = Resources.Load<Sprite>(lockedZonePath);
    //    //}
    //}

    //[ContextMenu("Save Game Data")]
    //public void SaveZone()
    //{
    //    string savedData = JsonUtility.ToJson(this);

    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/Zone" + id + ".txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/Zone" + id + ".txt";
    //    }
    //    File.WriteAllText(path, savedData);
    //}

    //[ContextMenu("Game Load Data")]
    //public void LoadZone()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/Zone" + id + ".txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/Zone" + id + ".txt";
    //    }
    //    JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);
    //}
}
