using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RewardScreenDisplayDataHelper : MonoBehaviour
{
    public static RewardScreenDisplayDataHelper Instance;

    public GameObject materialPrefabDisplay;

    public List<GameObject> lootPrefabsInstantiated;

    public Transform displayZone;

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
        foreach (string s in RewardsManager.Instance.dailyRewardPacks[RewardsManager.Instance.indexPack].dailyRewards)
        {
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
                    CMDRD.SetImageAndMaterialCount(LootManager.Instance.rubySprite, CraftingMats.None, amount.ToString());

                }
                else
                {
                    CraftingMats matToRecieve = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), temp[0]);

                    int amount = Convert.ToInt16(temp[1]);

                    GameObject go = Instantiate(materialPrefabDisplay, displayZone);
                    lootPrefabsInstantiated.Add(go);

                    craftingMatDailyRewardsDisplayer CMDRD = go.GetComponent<craftingMatDailyRewardsDisplayer>();
                    CMDRD.SetImageAndMaterialCount(LootManager.Instance.allMaterialSprites[(int)matToRecieve], matToRecieve, amount.ToString());
                }
            }
        }
    }

    public void HeighlightSpecificIndex()
    {
        Image currentImage = lootPrefabsInstantiated[RewardsManager.Instance.indexDayToGive].GetComponent<Image>();
        currentImage.color = new Color(255, 0, 0, 255);
    }
}
