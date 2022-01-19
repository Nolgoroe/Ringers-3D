using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanZoom : MonoBehaviour
{
    Vector3 TouchStart;

    public float MinZoom = 1;
    public float MaxZoom = 8;

    Touch touch;

    public Camera mainCam;

    public float panSpeed;
    public float rightBound;
    public float leftBound;
    public float topBound;
    public float bottomBound;

    //float vertExtent;
    //float horzExtent;

    //float originalOrthofraphicsize;

    //Vector3 OriginalCamPos;

    bool isZoom;
    public bool canZoom;

    public Text fovText;

    private void Start()
    {
        mainCam = Camera.main;
        //mainCam.orthographic = true;

        //OriginalCamPos = mainCam.transform.position;
        //mainCam.orthographicSize = MaxZoom;

        //originalOrthofraphicsize = mainCam.orthographicSize;

        //mainCam.orthographicSize = originalOrthofraphicsize;
        //mainCam.transform.position = OriginalCamPos;

        //vertExtent = MinZoom;
        //horzExtent = vertExtent * Screen.width / Screen.height;

        //Target = mainCam.transform;
    }

    void Update()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        if (aspectRatio >= (9 / 16f))
            {
            mainCam.fieldOfView = 60;
        }
        else if (aspectRatio >= 3 / 4f)
        {
            mainCam.fieldOfView = 55;
        }
        else
        {
            mainCam.fieldOfView = 58;
        }

        Debug.Log("Aspect " + aspectRatio);
        //mainCam.fieldOfView = aspectRatio * 60;
        Debug.Log("Aspect calc" + aspectRatio * 60);

        fovText.text = mainCam.fieldOfView.ToString();

        if (!UIManager.isUsingUI)
        {
            //leftBound = (horzExtent - (SpriteBounds.bounds.size.x / 2.0f));
            //rightBound = ((SpriteBounds.bounds.size.x / 2.0f - horzExtent));

            //bottomBound = ((vertExtent - SpriteBounds.rect.size.y / 2.0f)) / SpriteBounds.pixelsPerUnit + 2;
            //topBound = ((SpriteBounds.rect.size.y / 2.0f - vertExtent)) / SpriteBounds.pixelsPerUnit + 2;

            if (Input.touchCount > 0 && !GameManager.Instance.levelStarted)
            {
                touch = Input.GetTouch(0);

                if (Input.touchCount < 2)
                {
                    if (!isZoom)
                    {
                        //if (touch.phase == TouchPhase.Began)
                        //{
                        //    TouchStart = mainCam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -50));
                        //}

                        if (touch.phase == TouchPhase.Moved)
                        {

                            Vector2 touchDeltaPos = touch.deltaPosition;

                            //Debug.Log(touchDeltaPos);
                            //float zPos = -touchDeltaPos.y; // for 3D map

                            mainCam.transform.Translate(-touchDeltaPos.x * panSpeed, -touchDeltaPos.y * panSpeed, 0);

                            mainCam.transform.position = new Vector3(
                                Mathf.Clamp(mainCam.transform.position.x, leftBound, rightBound),
                                Mathf.Clamp(mainCam.transform.position.y, bottomBound, topBound),
                                -3f);

                            //float zPos = -touchDeltaPos.y;

                            //mainCam.transform.Translate(-touchDeltaPos.x * panSpeed, 0, zPos * panSpeed);

                            //mainCam.transform.position = new Vector3(
                            //    Mathf.Clamp(mainCam.transform.position.x, leftBound, rightBound),
                            //    50,
                            //    Mathf.Clamp(mainCam.transform.position.z, bottomBound, topBound));
                        }
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    isZoom = false;
                }

                if (Input.touchCount == 2 && canZoom)
                {
                    Touch FirstFinger = Input.GetTouch(0);
                    Touch SecondFinger = Input.GetTouch(1);

                    Vector2 FirstFingerPrevPos = FirstFinger.position - FirstFinger.deltaPosition;
                    Vector2 SecondFingerPrevPos = SecondFinger.position - SecondFinger.deltaPosition;

                    float PrevMagnitude = (FirstFingerPrevPos - SecondFingerPrevPos).magnitude;
                    float CurrentMagnitude = (FirstFinger.position - SecondFinger.position).magnitude;

                    float Difference = PrevMagnitude - CurrentMagnitude;

                    Camera.main.fieldOfView += Difference * 0.1f;
                    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, MinZoom, MaxZoom);

                    Camera.main.GetComponent<HideTiles>().updateMaxDistance(Difference);
                    //Zoom(Difference * 0.01f);
                }
            }
            else
            {
                isZoom = false;
            }
        }
    }

    //public void Zoom(float Increment)
    //{
    //    isZoom = true;
    //    mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize - Increment, MinZoom, MaxZoom);

    //    currentZoom = mainCam.orthographicSize;

    //    if(currentZoom >= MinZoom + 1)
    //    {
    //        ZoneManager.Instance.DiactiavteLevelDisplay();
    //    }
    //    else
    //    {
    //        ZoneManager.Instance.ActivateLevelDisplay();
    //    }

    //    vertExtent = mainCam.orthographicSize;
    //    horzExtent = vertExtent * Screen.width / Screen.height;


    //    Vector3 pos = new Vector3(Target.position.x, Target.position.y, -18);
    //    pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
    //    pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
    //    mainCam.transform.position = pos;
    //}
}
