using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdControllerFixMesh : MonoBehaviour
{
    public GameObject openWings, closedWings;

    public void openWingsOn()
    {
        openWings.SetActive(true);
        closedWings.SetActive(false);
        Debug.Log("Open Wings On");

    }

    public void closedWingsOn()
    {
        openWings.SetActive(false);
        closedWings.SetActive(true);

        Debug.Log("Closed Wings On");
    }
}
