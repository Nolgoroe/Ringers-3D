using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToCam : MonoBehaviour
{
    public Camera toStickTo;
    private void Update()
    {
        transform.position = new Vector3(toStickTo.transform.position.x, toStickTo.transform.position.y, transform.position.z);
    }
}
