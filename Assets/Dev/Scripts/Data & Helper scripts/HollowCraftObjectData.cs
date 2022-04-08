using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class HollowCraftObjectData
{
    public string objectname;
    public List<ObjectHollowType> objectHollowType;
    public HollowItems hollowItemEnum;
    public int indexInHollow;

    public string mats;
    //public string spritePath;
    public int spriteIndex;
    public HollowCraftObjectData(string objectName, int INindexInHollow, HollowItems INhollowItemEnum, int objecttype, string objectmatList, int pathIndex)
    {
        objectname = objectName;
        objectHollowType.Add((ObjectHollowType)objecttype);
        indexInHollow = INindexInHollow;
        hollowItemEnum = INhollowItemEnum;
        mats = objectmatList;
        //spritePath = path;
        spriteIndex = pathIndex;
    }
    public HollowCraftObjectData() //// Override for an empty constructor
    {

    }
}
