using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticScaler : MonoBehaviour
{
    public int originalWidth = 1080;
    public int originalHeight = 1920;

    public float buffer = 0;

    public bool doUpdate;
    Vector3 originalScale;

    CameraAnchor CA;

    public bool dobuffer = false;
    private void Start()
    {
        CA = GetComponent<CameraAnchor>();
        originalScale = transform.localScale;

        //if(transform.tag == "Clip" || transform.tag == "Board")
        //{
        //    CA.isMovePos = false;
        //}
        //else
        //{
        //    CA.isMovePos = true;
        //}

        Scaler();
    }

    private void Update()
    {
        if (doUpdate)
        {
            Scaler();
        }
    }
    public void Scaler()
    {
        float width = Display.main.systemWidth;
        float height = Display.main.systemHeight;

        float deltaWidth = 0;
        float deltaHeight = 0;

        if ( width >= originalWidth)
        {
            dobuffer = false;

            deltaWidth = originalWidth / width;
        }
        else
        {
            dobuffer = true;

            deltaWidth = width / originalWidth;
        }

        if (height >= originalHeight)
        {
            deltaHeight = originalHeight / height;
        }
        else
        {
            deltaHeight = height / originalHeight;
        }

        float actualDelta = 1;

        Vector3 newScale;

        if(deltaWidth == 1 && deltaHeight == 1)
        {
            CA.isMovePos = false;

            return;
        }

        CA.isMovePos = true;

        if (deltaWidth != 1)
        {
            actualDelta = deltaWidth;
        }
        else
        {
            actualDelta = deltaHeight;
        }

        //Debug.LogError("ACTUAL DELTA IS: " + actualDelta);

        //dobuffer = false; //// DISABLE THIS

        if (dobuffer)
        {
            actualDelta += buffer;
        }

        newScale = new Vector3((originalScale.x * actualDelta), (originalScale.y * actualDelta), originalScale.z);

        //Debug.LogError("NEW SCALE FOR " + transform.name + "IS: " + newScale);
        transform.localScale = newScale;
    }
}
