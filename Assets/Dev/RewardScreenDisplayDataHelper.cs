using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RewardScreenDisplayDataHelper : MonoBehaviour
{
    public static RewardScreenDisplayDataHelper Instance;

    public GameObject materialPrefabDisplay;
    public GameObject VFXRecieveDailyPrefab;

    public List<GameObject> lootPrefabsInstantiated;

    public Transform displayZone;

    public Color givenRewardDisplayColor;
    public Vector3 givenRewardDisplayScale;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lootPrefabsInstantiated = new List<GameObject>();
    }

    public void DisplayDailyRewards()
    {
        foreach (Transform child in displayZone)
        {
            Destroy(child.gameObject);
        }

        lootPrefabsInstantiated.Clear();

        int tempIndex = 0;

        foreach (string s in RewardsManager.Instance.dailyRewardPacks[RewardsManager.Instance.indexPack].dailyRewards)
        {
            tempIndex++;

            if (s.Contains("-"))
            {
                string[] temp = s.Split('-');

                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Trim();
                    temp[j] = temp[j].Replace(" ", string.Empty);
                }

                if (temp[0].Contains("Ruby"))
                {
                    int amount = Convert.ToInt16(temp[1]);

                    GameObject go = Instantiate(materialPrefabDisplay, displayZone);
                    lootPrefabsInstantiated.Add(go);

                    craftingMatDailyRewardsDisplayer CMDRD = go.GetComponent<craftingMatDailyRewardsDisplayer>();
                    CMDRD.SetImageAndMaterialCount(LootManager.Instance.rubySprite, amount.ToString());
                    CMDRD.SetDayText("day " + tempIndex);
                }
                else if (temp[0].Contains("Potion"))
                {
                    string[] tempData = s.Split('-');

                    for (int j = 0; j < tempData.Length; j++)
                    {
                        tempData[j] = tempData[j].Trim();
                        tempData[j] = tempData[j].Replace(" ", string.Empty);
                    }

                    PowerUp power = (PowerUp)Convert.ToInt16(tempData[1]);
                    int amount = Convert.ToInt16(tempData[2]);

                    GameObject go = Instantiate(materialPrefabDisplay, displayZone);
                    lootPrefabsInstantiated.Add(go);


                    string path = GameManager.Instance.powerupManager.spriteByType[power];

                    craftingMatDailyRewardsDisplayer CMDRD = go.GetComponent<craftingMatDailyRewardsDisplayer>();

                    Sprite icon = Resources.Load<Sprite>(path);

                    CMDRD.SetImageAndMaterialCount(icon, amount.ToString());
                    CMDRD.SetDayText("day " + tempIndex);
                }
                else
                {
                    CraftingMats matToRecieve = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), temp[0]);

                    int amount = Convert.ToInt16(temp[1]);

                    GameObject go = Instantiate(materialPrefabDisplay, displayZone);
                    lootPrefabsInstantiated.Add(go);

                    craftingMatDailyRewardsDisplayer CMDRD = go.GetComponent<craftingMatDailyRewardsDisplayer>();
                    CMDRD.SetImageAndMaterialCount(LootManager.Instance.allMaterialSprites[(int)matToRecieve], amount.ToString());

                    CMDRD.SetDayText("day " + tempIndex);
                }
            }
        }
    }

    public void HeighlightSpecificIndex()
    {
        int index = RewardsManager.Instance.indexDayToGive;

        
        for (int i = 0; i < lootPrefabsInstantiated.Count; i++)
        {
            lootPrefabsInstantiated[i].GetComponent<craftingMatDailyRewardsDisplayer>().TurnOFFTodayVFX();
            lootPrefabsInstantiated[i].GetComponent<craftingMatDailyRewardsDisplayer>().SetScaleNormal();
            lootPrefabsInstantiated[i].GetComponent<craftingMatDailyRewardsDisplayer>().TurnOffAnimator();
        }


        lootPrefabsInstantiated[index].GetComponent<craftingMatDailyRewardsDisplayer>().TurnOnTodayVFX();
        lootPrefabsInstantiated[index].GetComponent<craftingMatDailyRewardsDisplayer>().SetScaleToday();
        lootPrefabsInstantiated[index].GetComponent<craftingMatDailyRewardsDisplayer>().TurnOnAnimator();
    }

    public void CheckGivenRewardsDisplay()
    {
        int index = RewardsManager.Instance.indexDayToGive;

        for (int i = 0; i < lootPrefabsInstantiated.Count; i++)
        {
            if(i < index)
            {
                lootPrefabsInstantiated[i].GetComponent<craftingMatDailyRewardsDisplayer>().materialImage.color = givenRewardDisplayColor;
            }
        }
    }
}
