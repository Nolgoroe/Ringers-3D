using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintSystem : MonoBehaviour
{
    public int idleTimeHint = 3;
    public float currentIdleTime = 0;

    public bool canShowHint;


    // gamemanager.instance.currentleve.stonetile....
    // gamemanager.instance.currentleve.slices
    //

    void Update()
    {
        if(GameManager.Instance.levelStarted)
        {
            if (Input.touchCount > 0)
            {
                canShowHint = false;
                currentIdleTime = 0;
            }

            if (!canShowHint)
            {
                currentIdleTime += Time.deltaTime;

                if (currentIdleTime >= idleTimeHint)
                {
                    canShowHint = true;
                }
            }
        }
    }

    private void ResetTimer()
    {
        canShowHint = false;
        currentIdleTime = 0;
    }
}
