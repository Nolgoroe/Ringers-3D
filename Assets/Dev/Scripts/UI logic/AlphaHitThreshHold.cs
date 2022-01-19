using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaHitThreshHold : MonoBehaviour
{
    public float alphaThrshhold = 0.1f;
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThrshhold;
    }

}
