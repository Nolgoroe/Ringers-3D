using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


[Serializable]
public class DailyRewardsPacks
{
    public string[] dailyRewards;
}

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance;

    public int indexDayToGive = 0;
    public int indexPack = -1;

    public bool canGiveDaily;

    public List<DailyRewardsPacks> dailyRewardPacks;

    public float timeLeftToGiveDailyLoot = 0;
    public DateTime currentTime;
    public string savedDateTime;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        Instance = this;
    }

    public void CalculateReturnDeltaTime()
    {
        //DateTime currentTime = (PlayfabManager.instance.GetCurrentTime();

        //Debug.Log("Debug 7 " + savedDateTime);
        //Debug.Log("Debug 8 " + currentTime);
        StopAllCoroutines();

        if (savedDateTime != "" && !canGiveDaily)
        {
            TimeSpan deltaDateTime = currentTime - Convert.ToDateTime(savedDateTime);

            timeLeftToGiveDailyLoot -= ((float)deltaDateTime.TotalSeconds % PlayfabManager.instance.timeToWaitForDailyRewardSeconds);


            if(timeLeftToGiveDailyLoot < 0)
            {
                canGiveDaily = true;

                //timeLeftToGiveDailyLoot = PlayfabManager.instance.timeToWaitForDailyRewardSeconds + timeLeftToGiveDailyLoot; //it's plus a minus - time left to give loot will be minus here
                timeLeftToGiveDailyLoot = PlayfabManager.instance.timeToWaitForDailyRewardSeconds;

                RewardScreenDisplayDataHelper.Instance.HeighlightSpecificIndex();
            }
        }
        else
        {
            timeLeftToGiveDailyLoot = PlayfabManager.instance.timeToWaitForDailyRewardSeconds;
        }


        if (!canGiveDaily)
        {
            UIManager.Instance.getDailyLootButton.interactable = false;
        }
        else
        {
            UIManager.Instance.getDailyLootButton.interactable = true;
            UIManager.Instance.dailyLootTextTime.gameObject.SetActive(false);
        }

        StartCoroutine(DisplayTimeTillDaily());

        //savedDateTime = DateTime.MinValue;
    }


    public IEnumerator DisplayTimeTillDaily()
    {
        DisplayTimeNoDelay();
        while (!canGiveDaily)
        {
            UIManager.Instance.dailyLootTextTime.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(1);
            //Debug.Log("Inside daily loot Coroutine!");

            timeLeftToGiveDailyLoot--;

            if (timeLeftToGiveDailyLoot < 0)
            {
                canGiveDaily = true;

                timeLeftToGiveDailyLoot = PlayfabManager.instance.timeToWaitForDailyRewardSeconds;


                UIManager.Instance.getDailyLootButton.interactable = true;
                UIManager.Instance.dailyLootTextTime.gameObject.SetActive(false);
                RewardScreenDisplayDataHelper.Instance.HeighlightSpecificIndex();
            }

            float hours = Mathf.FloorToInt(timeLeftToGiveDailyLoot / 3600);
            float m = timeLeftToGiveDailyLoot % 3600;
            float minutes = Mathf.FloorToInt(m / 60);
            float seconds = Mathf.FloorToInt(timeLeftToGiveDailyLoot % 60);

            //UIManager.Instance.dailyLootTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            UIManager.Instance.dailyLootTextTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours ,minutes, seconds);

        }

        UIManager.Instance.dailyLootTextTime.gameObject.SetActive(false);
    }

    void DisplayTimeNoDelay() ///// This function is only for the start of the game so that players wont see the defult time while the real time is updating
    {
        float hours = Mathf.FloorToInt(timeLeftToGiveDailyLoot / 3600);
        float m = timeLeftToGiveDailyLoot % 3600;
        float minutes = Mathf.FloorToInt(m / 60);
        float seconds = Mathf.FloorToInt(timeLeftToGiveDailyLoot % 60);

        //UIManager.Instance.dailyLootTextTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        UIManager.Instance.dailyLootTextTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public void UpdateRewardListServer(List<DailyRewardsPacks> _dailyRewardPacks)
    {
        dailyRewardPacks = _dailyRewardPacks;

        if (indexPack < 0)
        {
            DecideOnDailyGiftPackIndex();
        }

        if (indexDayToGive >= 8)
        {
            indexDayToGive = 0;

            DecideOnDailyGiftPackIndex();
        }

        RewardScreenDisplayDataHelper.Instance.DisplayDailyRewards();

        RewardScreenDisplayDataHelper.Instance.HeighlightSpecificIndex();

        RewardScreenDisplayDataHelper.Instance.CheckGivenRewardsDisplay();
    }

    public void RealTimeRewardPacks()
    {
        if (indexDayToGive >= 8)
        {
            indexDayToGive = 0;

            DecideOnDailyGiftPackIndex();

            RewardScreenDisplayDataHelper.Instance.DisplayDailyRewards();
        }

        //RewardScreenDisplayDataHelper.Instance.CheckGivenRewardsDisplay();
    }

    [ContextMenu("give daily rewards")]
    public void giveRewardsButton()
    {
        SoundManager.Instance.PlaySound(Sounds.ButtonPressUI);

        //RewardScreenDisplayDataHelper.Instance.lootPrefabsInstantiated[indexDayToGive].GetComponent<craftingMatDailyRewardsDisplayer>().TurnOFFTodayVFX();
        RewardScreenDisplayDataHelper.Instance.lootPrefabsInstantiated[indexDayToGive].GetComponent<craftingMatDailyRewardsDisplayer>().PlayGiveAnim();
        GiveDailyRewards(indexPack);

        RealTimeRewardPacks();


        canGiveDaily = false;
        UIManager.Instance.getDailyLootButton.interactable = false;



        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player, SystemsToSave.RewardsManager});

        StartCoroutine(DisplayTimeTillDaily());
    }

    public void GiveDailyRewards(int packIndex)
    {
        if (dailyRewardPacks[packIndex].dailyRewards[indexDayToGive].Contains("-"))
        {
            string[] temp = dailyRewardPacks[packIndex].dailyRewards[indexDayToGive].Split('-');

            for (int j = 0; j < temp.Length; j++)
            {
                temp[j] = temp[j].Trim();
                temp[j] = temp[j].Replace(" ", string.Empty);
            }

            if (temp[0].Contains("Ruby"))
            {
                int amount = Convert.ToInt16(temp[1]);
                PlayerManager.Instance.AddRubies(amount);
            }
            else if (temp[0].Contains("Potion"))
            {
                string[] tempData = dailyRewardPacks[packIndex].dailyRewards[indexDayToGive].Split('-');

                for (int j = 0; j < tempData.Length; j++)
                {
                    tempData[j] = tempData[j].Trim();
                    tempData[j] = tempData[j].Replace(" ", string.Empty);
                }

                PowerUp power = (PowerUp)Convert.ToInt16(tempData[1]);


                PlayerManager.Instance.AddSpecificPowerupToInventory(power);
            }
            else 
            {
                CraftingMats matToRecieve = (CraftingMats)System.Enum.Parse(typeof(CraftingMats), temp[0]);

                int amountToGet = Convert.ToInt16(temp[1]);

                PlayerManager.Instance.AddMaterials(matToRecieve, amountToGet);
            }
        }


        indexDayToGive++;
    }

    public void DecideOnDailyGiftPackIndex()
    {
        int tempNum = UnityEngine.Random.Range(0, dailyRewardPacks.Count);
        
        while(tempNum == indexPack)
        {
            Debug.LogError("Rolled same week gift packs, rerolling");
            tempNum = UnityEngine.Random.Range(0, dailyRewardPacks.Count);
        }

        indexPack = tempNum;
    }

    public void UpdateCurrentTime(DateTime time)
    {
        currentTime = time;

        //Debug.Log("Debug 3 " + currentTime);
    }

    public void UpdateQuitTime(DateTime time)
    {
        savedDateTime = time.ToString();

        //Debug.Log("Debug 4 " + savedDateTime);
    }
}
