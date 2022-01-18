using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSettingsManager : MonoBehaviour
{
    public static LightingSettingsManager instance;

    public LightingSettingsSO[] allLightingSO;

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
        }
        else
        {
            zoneID -= 1; // we take down one since there is no specific lighting for tutorial zone
                         // meaning we skip the first index
            RenderSettings.skybox = allLightingSO[zoneID].skyBox;
            RenderSettings.sun = allLightingSO[zoneID].mainLight;

            RenderSettings.ambientSkyColor = allLightingSO[zoneID].skyColor;
            RenderSettings.ambientEquatorColor = allLightingSO[zoneID].equatorColor;
            RenderSettings.ambientGroundColor = allLightingSO[zoneID].groundColor;


            RenderSettings.subtractiveShadowColor = allLightingSO[zoneID].realtimeShadowColor;
            RenderSettings.fogColor = allLightingSO[zoneID].fogColor;
        }
    }

    public void ResetLightData()
    {
        RenderSettings.skybox = null;
        RenderSettings.sun = null;

        RenderSettings.ambientSkyColor = Color.white;
        RenderSettings.ambientEquatorColor = Color.white;
        RenderSettings.ambientGroundColor = Color.white;


        RenderSettings.subtractiveShadowColor = Color.white;
    }
}
