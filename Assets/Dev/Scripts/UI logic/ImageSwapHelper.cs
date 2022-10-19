using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwapHelper : MonoBehaviour
{
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
    public void ActivateAnimation()
    {
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().SetTrigger("Activate Star");
    }
}
