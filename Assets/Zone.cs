using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


[System.Serializable]
public class Zone : MonoBehaviour
{
    public int id;
    public bool hasAwardedKey, isUnlocked;

    public int maxLevelReachedInZone;
    public int lastLevelNum;

    string path;

    private void Start()
    {
        path = Application.dataPath + "/Zone" + id + ".txt";

        if (File.Exists(path))
        {
            LoadZone();
        }
    }
    [ContextMenu("Save Game Data")]
    public void SaveZone()
    {
        string savedData = JsonUtility.ToJson(this);

        string path = Application.dataPath + "/Zone" + id + ".txt";

        File.WriteAllText(path, savedData);
    }

    [ContextMenu("Game Load Data")]
    public void LoadZone()
    {
        string path = Application.dataPath + "/Zone" + id + ".txt";

        JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);
    }
}
