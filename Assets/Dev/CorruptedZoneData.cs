using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedZoneData : MonoBehaviour
{
    public CorruptedZoneScriptableObject CZSO;

    public CorruptionLevel corruptionLevel;
    public bool isClensing = false;
    public float timeLeftForClense = 0;
    public float corruptionAmountPerStage = 1500;
    float originalCorruptionAmountPerStage;
    public float HPM = 0;

    public List<tempMoveScript> currentDevicesInZone;

    public CorruptedZoneViewHelpData connectedView;

    //public float timeLeftToNextMin = 10;

    public void Start()
    {
        originalCorruptionAmountPerStage = corruptionAmountPerStage;

        corruptionLevel = CZSO.corruptionLevel;

        connectedView = GetComponentInParent< CorruptedZoneViewHelpData>();
    }

    private void Update()
    {
        if (isClensing)
        {
            ClenseWave();
        }
    }

    public void ClenseWave()
    {
        corruptionAmountPerStage -= (HPM / 60) * Time.deltaTime;

        if (corruptionAmountPerStage <= 0)
        {
            corruptionAmountPerStage = originalCorruptionAmountPerStage;
            Debug.Log((int)corruptionLevel);

            corruptionLevel--;
        }

        PlayerManager.Instance.SavePlayerData();
    }
}
