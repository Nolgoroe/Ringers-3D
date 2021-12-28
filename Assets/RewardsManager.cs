using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance;

    public List<string> dailyRewards;

    public int indexDayToGive;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;
    }

    void Start()
    {
        dailyRewards = new List<string>();
    }

    [ContextMenu("give daily rewards")]
    public void giveRewardsTemp()
    {
        GiveDailyRewards(indexDayToGive);
    }

    public void GiveDailyRewards(int index)
    {
        if (dailyRewards[index].Contains("-"))
        {
            string[] temp = dailyRewards[index].Split('-');

            for (int j = 0; j < temp.Length; j++)
            {
                temp[j] = temp[j].Trim();
                temp[j] = temp[j].Replace(" ", string.Empty);
            }

            CraftingMats matToRecieve = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), temp[0]);

            int amountToGet = Convert.ToInt16(temp[1]);

            PlayerManager.Instance.AddMaterials(matToRecieve, amountToGet);
        }


        indexDayToGive++;
    }
}
