using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwapOnClick : MonoBehaviour
{
    public Image connectedImageComponent;

    public Sprite clickedSprite, notClickedSprite;

    public bool isClicked;

    private void Start()
    {
        connectedImageComponent = GetComponent<Image>();
    }

    public void SetClicked()
    {
        connectedImageComponent.sprite = clickedSprite;
        isClicked = true;
    }
    public void SetNotClicked()
    {
        connectedImageComponent.sprite = notClickedSprite;
        isClicked = false;

    }

    public void SetClickOrNotClicked()
    {
        if (isClicked)
        {
            isClicked = false;
            connectedImageComponent.sprite = notClickedSprite;
        }
        else
        {
            isClicked = true;
            connectedImageComponent.sprite = clickedSprite;
        }
    }
}
