using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSettingsManager : MonoBehaviour
{
    public static LightingSettingsManager instance;

    public LightingSettingsSO[] allLightingSO;
    public LightingSettingsSO DenLight;

    private void Awake()
    {
        instance = this;
    }

    public void ChooseLightSettings(int zoneID)
    {
        if(zoneID == 0)
        {
            RenderSettings.skybox = allLightingSO[0].skyBox;
            RenderSettings.sun = allLightingSO[0].mainLight;

            RenderSettings.ambientSkyColor = allLightingSO[0].skyColor;
            RenderSettings.ambientEquatorColor = allLightingSO[0].equatorColor;
            RenderSettings.ambientGroundColor = allLightingSO[0].groundColor;


            RenderSettings.subtractiveShadowColor = allLightingSO[0].realtimeShadowColor;

            RenderSettings.fog = allLightingSO[0].fogON;
            RenderSettings.fogColor = allLightingSO[0].fogColor;
            RenderSettings.fogMode = allLightingSO[0].fodMode;
            RenderSettings.fogDensity = allLightingSO[0].fogDensity;
            RenderSettings.fogStartDistance = allLightingSO[0].fogStart;
            RenderSettings.fogEndDistance = allLightingSO[0].fogEnd;
        }
        else
        {
            //zoneID -= 1; // we take down one since there is no specific lighting for tutorial zone
            //             // meaning we skip the first index
            RenderSettings.skybox = allLightingSO[zoneID].skyBox;
            RenderSettings.sun = allLightingSO[zoneID].mainLight;

            RenderSettings.ambientSkyColor = allLightingSO[zoneID].skyColor;
            RenderSettings.ambientEquatorColor = allLightingSO[zoneID].equatorColor;
            RenderSettings.ambientGroundColor = allLightingSO[zoneID].groundColor;


            RenderSettings.subtractiveShadowColor = allLightingSO[zoneID].realtimeShadowColor;

            RenderSettings.fog = allLightingSO[zoneID].fogON;
            RenderSettings.fogColor = allLightingSO[zoneID].fogColor;
            RenderSettings.fogMode = allLightingSO[zoneID].fodMode;
            RenderSettings.fogDensity = allLightingSO[zoneID].fogDensity;
            RenderSettings.fogStartDistance = allLightingSO[zoneID].fogStart;
            RenderSettings.fogEndDistance = allLightingSO[zoneID].fogEnd;
        }
    }

    public void SetdenLight()
    {
        RenderSettings.skybox = DenLight.skyBox;
        RenderSettings.sun = DenLight.mainLight;

        RenderSettings.ambientSkyColor = DenLight.skyColor;
        RenderSettings.ambientEquatorColor = DenLight.equatorColor;
        RenderSettings.ambientGroundColor = DenLight.groundColor;


        RenderSettings.subtractiveShadowColor = DenLight.realtimeShadowColor;

        RenderSettings.fog = DenLight.fogON;
        RenderSettings.fogColor = DenLight.fogColor;
        RenderSettings.fogMode = DenLight.fodMode;
        RenderSettings.fogDensity = DenLight.fogDensity;
        RenderSettings.fogStartDistance = DenLight.fogStart;
        RenderSettings.fogEndDistance = DenLight.fogEnd;

    }

    public void ResetLightData()
    {
        RenderSettings.skybox = null;
        RenderSettings.sun = null;

        RenderSettings.ambientSkyColor = Color.white;
        RenderSettings.ambientEquatorColor = Color.white;
        RenderSettings.ambientGroundColor = Color.white;


        RenderSettings.subtractiveShadowColor = Color.white;

        RenderSettings.fog = false;
        RenderSettings.fogColor = Color.white;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0;
        RenderSettings.fogStartDistance = 0;
        RenderSettings.fogEndDistance = 300;


    }
}
