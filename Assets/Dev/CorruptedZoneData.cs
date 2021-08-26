using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorruptedZoneData : MonoBehaviour
{
    public CorruptedZoneScriptableObject CZSO;
    public CorruptedZoneViewHelpData connectedView;

    //public CorruptionLevel corruptionLevel;
    //public bool isClensing = false;
    //public float timeLeftForClense = 0;
    //public float corruptionAmountPerStage = 1500;
    //float originalCorruptionAmountPerStage;
    //public float HPM = 0;

    //public List<tempMoveScript> currentDevicesInZone;



    public SavedTimePerZone saveDataZone;
    //public float timeLeftToNextMin = 10;

    public void Start()
    {
        saveDataZone.originalCorruptionAmountPerStage = saveDataZone.corruptionAmountPerStage;

        saveDataZone.corruptionLevel = CZSO.corruptionLevel;

        CorruptedZoneViewHelpData CZVHD = CorruptedZonesManager.Instance.allCorruptedZonesView.Where(p => p.ZoneIDView == CZSO.zoneID).Single();

        connectedView = CZVHD;
    }

    private void Update()
    {
        if (saveDataZone.isClensing)
        {
            Clense();
        }
    }

    public void Clense()
    {
        if (saveDataZone.corruptionLevel > CorruptionLevel.level0)
        {
            saveDataZone.corruptionAmountPerStage -= (saveDataZone.HPM / 60) * Time.deltaTime;

            connectedView.harmonySliderInCorruptedZone.value = saveDataZone.corruptionAmountPerStage / saveDataZone.originalCorruptionAmountPerStage;
            connectedView.harmonySliderOnMap.value = saveDataZone.corruptionAmountPerStage / saveDataZone.originalCorruptionAmountPerStage;

            if (saveDataZone.corruptionAmountPerStage <= 0)
            {
                saveDataZone.corruptionAmountPerStage = saveDataZone.originalCorruptionAmountPerStage;
                Debug.Log((int)saveDataZone.corruptionLevel);

                saveDataZone.corruptionLevel--;

                if (saveDataZone.corruptionLevel == CorruptionLevel.level0)
                {
                    Destroy(connectedView.harmonySliderInCorruptedZone.gameObject);
                    Destroy(connectedView.harmonySliderOnMap.gameObject);
                    Destroy(connectedView.harmonySliderOnMap.gameObject);
                    Destroy(connectedView.placedObjetZone.gameObject);

                    CorruptedZonesSaveData.Instance.RemoveElementFromSaveData(connectedView);
                    CorruptedZonesManager.Instance.RemoveElementFromBeingClensed(connectedView);

                    connectedView.harmonySliderOnMap = null;
                    connectedView.harmonySliderInCorruptedZone = null;
                    connectedView.connectedCZD = null;

                    connectedView.placedObjetZone = null;
                    connectedView.isFullyClensed = true;

                    if (UIManager.Instance.ownedCorruptDevicesZone.gameObject.activeInHierarchy)
                    {
                        UIManager.Instance.ownedCorruptDevicesZone.gameObject.SetActive(false);
                    }

                    Destroy(gameObject, 3f);
                }
            }
        }
        //PlayerManager.Instance.SavePlayerData();
    }
}
