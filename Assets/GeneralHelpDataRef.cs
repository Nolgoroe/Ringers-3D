using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralHelpDataRef : MonoBehaviour
{
    public Text posX1, posY1, posZ1, S1, S12,S13, posX2, posY2, posZ2, S21,S22,S23;
    public Text screenWidth, screenHeight;
    public Text screenWidthDisplay, screenHeightDisplay;

    public static GeneralHelpDataRef Instance;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        screenWidth.text = "width " + Screen.width.ToString();
        screenHeight.text = "height " + Screen.height.ToString();

        screenWidthDisplay.text = "width Display" + Display.main.systemWidth.ToString();
        screenHeightDisplay.text = "height Display " + Display.main.systemHeight.ToString();
    }
}
