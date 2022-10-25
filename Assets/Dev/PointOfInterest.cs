using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public GameObject interestPointImage;

    void Start()
    {
        //interestPointImage.SetActive(false);
    }

    public void ShowInterestPointImage()
    {
        interestPointImage.SetActive(true);
    }
    public void HideInterestPointImage()
    {
        interestPointImage.SetActive(false);
    }
}
