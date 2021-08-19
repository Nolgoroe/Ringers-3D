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
public class DeviceConnections
{
    public string deviceToConnectWith;
    public List<float> distances;
    public List<float> HPM; /// Harmony Per Minute
}

[Serializable]
public class CorruptedDevicesData
{
    public string deviceName;
    public CorruptedDeviceType deciveType;
    public string mats;
    public string spritePath;
    public string prefabPath;
    public List<DeviceConnections> deviceConnectionsList;
    public float currentHarmonyToGive = 0;


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
