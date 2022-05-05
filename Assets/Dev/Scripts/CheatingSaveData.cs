using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatingSaveData : MonoBehaviour
{
    public static CheatingSaveData instance;

    public bool isAdmin;

    void Start()
    {
        instance = this;
    }
}
