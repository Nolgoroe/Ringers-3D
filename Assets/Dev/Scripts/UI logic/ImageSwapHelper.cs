using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwapHelper : MonoBehaviour
{
    public Image objectImage;
    public GameObject deActivatedChild, activatedChild;

    public void SetActivatedChild()
    {
        activatedChild.SetActive(true);
        deActivatedChild.SetActive(false);
    }

    public void SetDeActivatedChild()
    {
        activatedChild.SetActive(false);
        deActivatedChild.SetActive(true);
    }
}
