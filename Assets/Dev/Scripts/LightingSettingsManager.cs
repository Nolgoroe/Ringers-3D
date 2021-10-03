using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSettingsManager : MonoBehaviour
{
    public LightingSettingsSO lightingSO;


    private void Start()
    {
        RenderSettings.skybox = lightingSO.skyBox;
        RenderSettings.sun = lightingSO.mainLight;

        RenderSettings.ambientSkyColor = lightingSO.skyColor;
        RenderSettings.ambientEquatorColor = lightingSO.equatorColor;
        RenderSettings.ambientGroundColor = lightingSO.groundColor;


        RenderSettings.subtractiveShadowColor = lightingSO.realtimeShadowColor;
    }
}
