using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossesSaveDataManager : MonoBehaviour
{
    public static BossesSaveDataManager instance;


    public int BossOneSaveHP;
    public int BossTwoSaveHP;

    void Start()
    {
        instance = this;
    }
}
