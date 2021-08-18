using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public GameObject CDDOrigin;          
    public GameObject CDDTarget;

    public Material lineMat;

    private LineRenderer line1;

    public List<float> distnaces;

    void Start()
    {
        line1 = gameObject.AddComponent<LineRenderer>();

        line1.startWidth = 0.1f;
        line1.endWidth = 0.1f;
        line1.positionCount = 2;
        line1.material = lineMat;
        line1.sortingOrder = 2;
    }
    void Update()
    {
        line1.SetPosition(0, CDDOrigin.transform.position);
        line1.SetPosition(1, CDDTarget.transform.position);
    }
}
