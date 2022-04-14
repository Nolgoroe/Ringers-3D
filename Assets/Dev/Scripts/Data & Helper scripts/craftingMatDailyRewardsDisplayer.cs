using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class craftingMatDailyRewardsDisplayer : MonoBehaviour
{
    public Image materialImage;
    public TMP_Text materialCount;

    public TMP_Text dayText;

    public GameObject todayEffect;

    public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetImageAndMaterialCount(Sprite icon, string amount)
    {
        materialImage.sprite = icon;
        materialCount.text = amount;
    }

    public void TurnOnTodayVFX()
    {
        todayEffect.SetActive(true);
    }
    public void TurnOFFTodayVFX()
    {
        todayEffect.SetActive(false);
    }
    public void SetScaleToday()
    {
        materialImage.transform.localScale = RewardScreenDisplayDataHelper.Instance.givenRewardDisplayScale;

        if (anim)
        {
            Invoke("enableAnim", 2f);
        }
    }

    void enableAnim()
    {
        anim.enabled = true;
    }
    public void SetScaleNormal()
    {
        materialImage.transform.localScale = Vector3.one;
    }
    public void PlayGiveAnim()
    {
        anim.SetBool("Give Daily", true);
    }
    public void TurnOffAnimator()
    {
        anim.enabled = false;
    }
    public void TurnOnAnimator()
    {
        anim.enabled = true;
    }
    public void OnEndAnimation()
    {
        UIManager.Instance.CallDeactivateDailyRewardScreen();
    }

    public void VFXRecieveDaily()
    {
        Instantiate(RewardScreenDisplayDataHelper.Instance.VFXRecieveDailyPrefab, todayEffect.transform);
    }

    public void SetDayText(string day)
    {
        dayText.text = day;
    }
}
