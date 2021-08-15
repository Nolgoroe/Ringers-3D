using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public GameObject gameObject1;          
    public GameObject gameObject2;

    public Material lineMat;

    private LineRenderer line1;                           

    void Start()
    {
        line1 = gameObject.AddComponent<LineRenderer>();

        line1.startWidth = 0.1f;
        line1.endWidth = 0.1f;
        line1.positionCount = 2;
        line1.material = lineMat;
        line1.sortingOrder = 10;
    }
    void Update()
    {
        line1.SetPosition(0, gameObject1.transform.position);
        line1.SetPosition(1, gameObject2.transform.position);
    }
}
