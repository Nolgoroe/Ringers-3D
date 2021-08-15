﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CorruptionLevel
{
    level1,
    level2,
    level3,
    level4,
    level5,
    level6,
    level7,

}
[CreateAssetMenu(fileName = "Corrupted Zone", menuName = "ScriptableObjects/Create Corrupted Zone")]
public class CorruptedZoneScriptableObject : ScriptableObject
{
    public CorruptionLevel corruptionLevel;

    public Sprite backGroundSprite;
}
