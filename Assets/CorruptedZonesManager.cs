using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptedZonesManager : MonoBehaviour
{
    public static CorruptedZonesManager instance;

    public GameObject lineGO;

    public List<GameObject> currentDevices;

    private void Start()
    {
        instance = this;
    }
}
