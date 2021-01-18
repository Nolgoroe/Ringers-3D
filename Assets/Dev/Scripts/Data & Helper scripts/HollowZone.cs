using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowZone : MonoBehaviour
{
    public bool isEmpty;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<OwnedHollowObjectData>().objectData = new HollowCraftObjectData();
    }

    // Update is called once per frame
}
