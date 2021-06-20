using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassAlongAnimationEventData : MonoBehaviour
{
    public void SetEmptySlice()
    {
        Slice parentslice = GetComponentInParent<Slice>();

        Transform go = transform.GetChild(1).transform;

        if (go)
        {
            go.gameObject.SetActive(true);
        }

        if (parentslice)
        {
            parentslice.SetEmpty();
        }
        else
        {
            Debug.LogError("WTF IS GOING OOOOONNNNNNN .... no slice script in parent");
        }
    }
}
