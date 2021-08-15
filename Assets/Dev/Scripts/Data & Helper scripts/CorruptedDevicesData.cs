using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CorruptedDeviceType
{
    All,
    Type1,
    Type2
}


[Serializable]
public class HPMPerDistance
{
    public float distance = 0;
    public float HPM = 0; /// Harmony Per Minute
}

[Serializable]
public class CorruptedDevicesData
{
    public string deviceName;
    public CorruptedDeviceType deciveType;
    public string mats;
    public string spritePath;
    public List<HPMPerDistance> hpmPerDistance;


    public CorruptedDevicesData(string objectName, CorruptedDeviceType objecttype, string objectmatList, string path)
    {
        deviceName = objectName;
        deciveType = objecttype;
        mats = objectmatList;
        spritePath = path;
    }
    public CorruptedDevicesData() //// Override for an empty constructor
    {

    }

}
