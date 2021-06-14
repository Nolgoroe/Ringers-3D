using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSpecialPowerUp : MonoBehaviour
{
    public PowerUp type;
    public PieceSymbol SymbolNeeded;


    public Slider slider;

    [HideInInspector]
    public Button button;

    public int amountNeededToActivate = 0;
    private void Awake()
    {
        button = GetComponent<Button>();

        slider.value = 0;
        button.interactable = false;
    }

    public void UpdateSlider(int amount)
    {
        slider.value += (float)amount / amountNeededToActivate;

        if(slider.value >= 1)
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
