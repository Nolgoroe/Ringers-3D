using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class forceSetResolution : MonoBehaviour
{
    RectTransform rect;
    public Text posX, posY, posZ, ScaleX, ScaleY, ScaleZ, text1, text2, text3, text4;

    void Start()
    {
        //posX = GeneralHelpDataRef.Instance.posX1;
        //posY = GeneralHelpDataRef.Instance.posY1;

        rect = transform.GetComponent<RectTransform>();

        //Canvas parentCanvas = transform.GetComponentInParent<Canvas>();

       // RectTransform parentRect = parentCanvas.GetComponent<RectTransform>();



        //posX.text = "rect width: " + rect.rect.width.ToString();
        //posY.text = "rect height: " + rect.rect.height.ToString();

    }


    private void Update()
    {
        rect.sizeDelta = new Vector2(Screen.width, Screen.height);

    }
}
