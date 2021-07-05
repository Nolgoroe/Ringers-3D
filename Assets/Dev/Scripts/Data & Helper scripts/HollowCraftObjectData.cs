using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ObjectHollowType
{
    All,
    H1,
    H2,
    H3,
    H4,
    H5,
    H6
}

[Serializable]
public class HollowCraftObjectData
{
    public string objectname;
    public List<ObjectHollowType> objectHollowType;
    public string mats;
    public string spritePath;

    public HollowCraftObjectData(string objectName, int objecttype, string objectmatList, string path)
    {
        objectname = objectName;
        objectHollowType.Add((ObjectHollowType)objecttype);
        mats = objectmatList;
        spritePath = path;
    }
    public HollowCraftObjectData() //// Override for an empty constructor
    {

    }
}
