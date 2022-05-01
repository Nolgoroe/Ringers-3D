using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSpecialPowerUp : MonoBehaviour
{
    public SpecialPowerUp type;
    public PieceSymbol SymbolNeeded;


    public Slider slider;

    public Button button;

    public Image icon;

    public int amountNeededToActivate = 0;
    public int currentTotalAmount = 0;

    public Sprite deactivatedImage, activatedImage;

    public bool isAdding = false;
    public int amountToAdd = 0;

    private void Awake()
    {
        //button = GetComponent<Button>();

        slider.value = 0;
        button.interactable = false;
    }

    public void UpdateSlider(int amount)
    {
        if (!isAdding)
        {
            if(amountToAdd > 0)
            {
                amountToAdd--;
            }

            isAdding = true;
            currentTotalAmount += amount;

            float target = (float)currentTotalAmount / (float)amountNeededToActivate;

            LeanTween.value(slider.gameObject, slider.value, target, 1f).setEase(LeanTweenType.linear).setOnComplete(() => CheckCanUseSpecialPower()).setOnUpdate((float val) =>
            {
                float temp = val;
                slider.value = temp;
            });
        }
        else
        {
            amountToAdd ++;
        }
    }

    private void CheckCanUseSpecialPower()
    {
        isAdding = false;

        if (slider.value >= 1)
        {
            button.interactable = true;

            GameManager.Instance.powerupManager.TurnOnSpecialPowerDisplay(this, SymbolNeeded, false);
        }
        else
        {
            if(amountToAdd > 0)
            {
                UpdateSlider(1);
            }
        }
    }
    public void ResetValues()
    {
        button.interactable = false;
        slider.value = 0;
        currentTotalAmount = 0;
        GameManager.Instance.powerupManager.TurnOnSpecialPowerDisplay(this, SymbolNeeded, true);
    }
}
