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
    public List<float> HPMPerDistance;

    public float maxDistance, minDistance, mediumDistance;

    public float currentHPMMultiplier = 1;


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


    public void UpdateHPMValue(float distance, LineController LC)
    {
        if (distance > LC.maxDistance || distance < LC.minDistance)
        {
            currentHPMMultiplier = 1;
        }
        else
        {
            currentHPMMultiplier = SwitchHPMMultiplier(distance, LC);
        }

        Debug.Log(transform.name + " " + currentHPMMultiplier);
    }

    private float SwitchHPMMultiplier(float distance, LineController LC)
    {
        if (distance >= LC.maxDistance || distance <= LC.minDistance)
        {
            return 1;
        }

        if (LC.maxDistance == LC.distnaces[2])
        {
            if (distance <= LC.maxDistance && distance >= LC.mediumDistance)
            {
                return LC.HPMPerDistance[1];
            }
            else if (distance >= LC.minDistance && distance <= LC.mediumDistance)
            {
                return LC.HPMPerDistance[0];
            }
        }
        else
        {
            if (distance >= LC.minDistance && distance <= LC.mediumDistance)
            {
                return LC.HPMPerDistance[1];
            }
            else if (distance >= LC.mediumDistance && distance <= LC.maxDistance)
            {
                return LC.HPMPerDistance[0];
            }
        }

        Debug.LogError("WHAHHAHAHAHAA");
        return -1;
    }
}
