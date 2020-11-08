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

    public Image SpriteBounds;

    public Camera MainCam;

    Transform Target;

    private float rightBound;
    private float leftBound;
    private float topBound;
    private float bottomBound;

    float vertExtent;
    float horzExtent;

    float OriginalOrthofraphicsize;

    Vector3 OriginalCamPos;

    bool isZoom;
    private void OnDisable()
    {
        MainCam.orthographicSize = OriginalOrthofraphicsize;
        MainCam.transform.position = OriginalCamPos;
    }

    private void Start()
    {
        MainCam = Camera.main;

        MainCam.orthographic = true;

        OriginalCamPos = MainCam.transform.position;
        MainCam.orthographicSize = MaxZoom;

        OriginalOrthofraphicsize = MainCam.orthographicSize;

        MainCam.orthographicSize = OriginalOrthofraphicsize;
        MainCam.transform.position = OriginalCamPos;

        vertExtent = MainCam.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;

        Target = MainCam.transform;
    }

    void Update()
    {
        leftBound = (horzExtent - SpriteBounds.sprite.bounds.size.x / 2.0f);
        rightBound = (SpriteBounds.sprite.bounds.size.x / 2.0f - horzExtent);
        bottomBound = (vertExtent - SpriteBounds.sprite.bounds.size.y / 2.0f);
        topBound = (SpriteBounds.sprite.bounds.size.y / 2.0f - vertExtent);

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (Input.touchCount < 2)
            {
                if (!isZoom)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        TouchStart = MainCam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 18));
                    }

                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector3 Direction = TouchStart - MainCam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 18));
                        MainCam.transform.position += Direction;


                        Vector3 pos = new Vector3(Target.position.x, Target.position.y, 18);
                        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
                        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
                        MainCam.transform.position = pos;
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

                Zoom(Difference * 0.01f);
            }
        }
        else
        {
            isZoom = false;
        }
    }

    public void Zoom(float Increment)
    {
        isZoom = true;
        MainCam.orthographicSize = Mathf.Clamp(MainCam.orthographicSize - Increment, MinZoom, MaxZoom);



        vertExtent = MainCam.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;


        Vector3 pos = new Vector3(Target.position.x, Target.position.y, 18);
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        MainCam.transform.position = pos;
    }
}
