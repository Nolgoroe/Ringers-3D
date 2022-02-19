    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteAlways]
public class autoScaler : MonoBehaviour
{
    public int defaultWidth = 1080, defaultHeight = 1920;
    public Vector3 scale;

    public Text textone, textTwo;

    private void Start()
    {
        scale = new Vector3((float)defaultWidth / (float)Display.main.systemWidth, (float)defaultHeight / (float)Display.main.systemHeight, 1f);

        Debug.Log(scale);
        Debug.Log(transform.localScale);
        Debug.Log(Vector3.Scale(transform.localScale, scale));

        Vector3 s = Vector3.Scale(transform.localScale, scale);

        transform.localScale = new Vector3(s.y, s.y, 1);
        //if (defaultWidth == Screen.width || defaultHeight == Screen.height)
        //{
        //    transform.localScale = new Vector3(s.y, s.y, 1);
        //}
        //else
        //{
        //    transform.localScale = new Vector3(s.x, s.y, 1);
        //}

        textone.text = transform.localScale.x.ToString();
        textTwo.text = transform.localScale.y.ToString();
    }
}
