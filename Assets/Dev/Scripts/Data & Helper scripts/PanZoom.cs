using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanZoom : MonoBehaviour
{
    Vector3 TouchStart;

    public float MinZoom = 1;
    public float MaxZoom = 8;
    public float currentZoom = 0;
    public float addedOffsetTop = 2, addedOffsetBottom = 2;

    Touch touch;

    public Sprite SpriteBounds;

    public Camera mainCam;

    Transform Target;

    private float rightBound;
    private float leftBound;
    private float topBound;
    private float bottomBound;

    float vertExtent;
    float horzExtent;

    float originalOrthofraphicsize;

    Vector3 OriginalCamPos;

    bool isZoom;
    //private void OnDisable()
    //{
    //    if (mainCam)
    //    {
    //        mainCam.transform.position = new Vector3(0, 0, -20);
    //    }
    //}

    private void Start()
    {
        mainCam = Camera.main;

        mainCam.orthographic = true;

        OriginalCamPos = mainCam.transform.position;
        mainCam.orthographicSize = MaxZoom;

        originalOrthofraphicsize = mainCam.orthographicSize;

        mainCam.orthographicSize = originalOrthofraphicsize;
        mainCam.transform.position = OriginalCamPos;

        vertExtent = MinZoom;
        horzExtent = vertExtent * Screen.width / Screen.height;

        Target = mainCam.transform;
    }

    void Update()
    {
        if (!UIManager.isUsingUI)
        {
            leftBound = (horzExtent - (SpriteBounds.bounds.size.x / 2.0f));
            rightBound = ((SpriteBounds.bounds.size.x / 2.0f - horzExtent));

            bottomBound = ((vertExtent - SpriteBounds.rect.size.y / 2.0f)) / SpriteBounds.pixelsPerUnit + 2;
            topBound = ((SpriteBounds.rect.size.y / 2.0f - vertExtent)) / SpriteBounds.pixelsPerUnit + 2;

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (Input.touchCount < 2)
                {
                    if (!isZoom)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            TouchStart = mainCam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -18));
                        }

                        if (touch.phase == TouchPhase.Moved)
                        {
                            Vector3 Direction = TouchStart - mainCam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -18));
                            mainCam.transform.position += Direction;


                            Vector3 pos = new Vector3(Target.position.x, Target.position.y, -18);
                            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
                            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
                            mainCam.transform.position = pos;
                        }
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    isZoom = false;
                }

                if (Input.touchCount == 2)
                {
                    Touch FirstFinger = Input.GetTouch(0);
                    Touch SecondFinger = Input.GetTouch(1);

                    Vector2 FirstFingerPrevPos = FirstFinger.position - FirstFinger.deltaPosition;
                    Vector2 SecondFingerPrevPos = SecondFinger.position - SecondFinger.deltaPosition;

                    float PrevMagnitude = (FirstFingerPrevPos - SecondFingerPrevPos).magnitude;
                    float CurrentMagnitude = (FirstFinger.position - SecondFinger.position).magnitude;

                    float Difference = CurrentMagnitude - PrevMagnitude;

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
