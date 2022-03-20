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
    private void Awake()
    {
        //button = GetComponent<Button>();

        slider.value = 0;
        button.interactable = false;
    }

    public void UpdateSlider(int amount)
    {
        currentTotalAmount += amount;

        float target = (float)currentTotalAmount / (float)amountNeededToActivate;

        LeanTween.value(slider.gameObject, slider.value, target, 1f).setEase(LeanTweenType.linear).setOnComplete(() => CheckCanUseSpecialPower()).setOnUpdate((float val) =>
        {
            float temp = val;
            slider.value = temp;
        });
    }

    private void CheckCanUseSpecialPower()
    {
        if (slider.value >= 1)
        {
            button.interactable = true;
        }
    }
    public void ResetValues()
    {
        button.interactable = false;
        slider.value = 0;
    }
}
