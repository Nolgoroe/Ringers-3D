using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Lighting Settings", menuName = "ScriptableObjects/Create Lighting Settings")]
public class LightingSettingsSO : ScriptableObject
{
    public Material skyBox;

    public Light mainLight;

    [ColorUsage(true, true)]
    public Color skyColor, equatorColor, groundColor;

    public Color realtimeShadowColor;

    public bool fogON;

    public Color fogColor;

    public FogMode fodMode;

    public float fogDensity;

    public float fogStart;
    public float fogEnd;
}
