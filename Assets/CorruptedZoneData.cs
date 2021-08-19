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

    //public float timeLeftToNextMin = 10;

    public void Start()
    {
        originalCorruptionAmountPerStage = corruptionAmountPerStage;

        corruptionLevel = CZSO.corruptionLevel;

        StartCoroutine(DisplayTime());
    }

    IEnumerator DisplayTime()
    {
        //DisplayTimeNoDelay();

        while (corruptionLevel > 0)
        {
            //UIManager.Instance.dewDropsTextTime.gameObject.SetActive(true);

            //yield return new WaitForSecondsRealtime(1);
            //timeLeftToNextMin--;

            //if (timeLeftToNextMin <= 0)
            //{
            //timeLeftToNextMin = 1 * 10;

            //}

            //float minutes = Mathf.FloorToInt(timeLeftToNextMin / 60);
            //float seconds = Mathf.FloorToInt(timeLeftToNextMin % 60);

            ClenseWave();
            yield return null;
        }

        //UIManager.Instance.dewDropsTextTime.gameObject.SetActive(false);
    }

    //void DisplayTimeNoDelay() ///// This function is only for the star of the game no that players wont see the defult time while the real time is updating
    //{
    //    float minutes = Mathf.FloorToInt(timeLeftToNextMin / 60);
    //    float seconds = Mathf.FloorToInt(timeLeftToNextMin % 60);

    //    UIManager.Instance.dewDropsTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    //}

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
