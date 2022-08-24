using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByCurve : MonoBehaviour
{
    float currentTime;
    float lastKeyTime;

    public AnimationCurve myCurve;

    public bool animate;
    public bool isRect;

    public Vector3 target;

    Vector3 currentLocation;
    Vector3 startLocation;

    RectTransform rect;
    void Start()
    {
        Keyframe lastframe = myCurve[myCurve.length - 1];
        lastKeyTime = lastframe.time;



        if(isRect)
        {
            rect = GetComponent<RectTransform>();

            startLocation = rect.anchoredPosition;
        }
        else
        {
            startLocation = transform.position;
        }
    }

    void Update()
    {
        if (animate)
        {
            currentTime += Time.deltaTime; //elapsed time

            float curveValue = myCurve.Evaluate(currentTime);

            Vector3 location = startLocation; // we reset location to start location every frame since we always want the offset from the start location!
            // if we don't do this we would "Escape" from the bounds of the animation.

            // this calculates the amount we need to move in each axis to get to "target"
            Vector3 road = new Vector3((target.x - startLocation.x), (target.y - startLocation.y), (target.z - startLocation.z));
            
            location += curveValue * road; // add the current offset location to the "start locaiton" given the value*target.
            // this means that if the value is 1 - we'll be at "target" but anything else will move us from the target
            // accordign to curve


            if(isRect)
            {
                rect.anchoredPosition = location;
            }
            else
            {
                transform.position = location; // set the new locaiton by curve * target
            }

            if (currentTime >= lastKeyTime)
            {
                animate = false;
                currentTime = 0;
                return;
            }

        }
    }

    [ContextMenu("Reset")]
    private void Reset()
    {
        transform.position = Vector3.zero;
    }

    private void OnEnable()
    {
        animate = true;
    }

    private void OnDisable()
    {
        if(isRect)
        {
            rect.anchoredPosition = startLocation;
        }
        else
        {
            transform.position = startLocation;
        }
    }
}
