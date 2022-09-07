using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class TutorialSaveData : MonoBehaviour
{
    public static TutorialSaveData Instance;
    
    public List<int> completedTutorialLevelId;
    public List<int> completedSpecificTutorialLevelId;

    public bool hasFinishedIntro = false;
    public bool hasFinishedDen = false;
    public bool hasFinishedAnimalAlbum = false;
    public bool hasFinishedPotion = false;
    //public bool completedLootTutorial;

    string path;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;

    }

    //[ContextMenu("Save Tutorial Save Data")]
    //public void SaveTutorialSaveData()
    //{
    //    string savedData = JsonUtility.ToJson(this);

    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/TutorialSaveData.txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/TutorialSaveData.txt";
    //    }

    //    File.WriteAllText(path, savedData);
    //}

    //[ContextMenu("Load Tutorial Save Data")]
    //public void LoadTutorialSaveData()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string path = Application.persistentDataPath + "/TutorialSaveData.txt";
    //    }
    //    else
    //    {
    //        string path = Application.dataPath + "/Save Files Folder/TutorialSaveData.txt";
    //    }

    //    JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

    //    Instance = this;
    //}

}
