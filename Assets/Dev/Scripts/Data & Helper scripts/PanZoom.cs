using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanZoom : MonoBehaviour
{
    bool isZoom;
    public bool isDragging;
    public bool canZoom;

    Vector3 TouchStart;
    Touch touch;
    public Camera mainCam;


    [Header("Data for world map")]

    public float MinZoom = 1;
    public float MaxZoom = 8;



    public float panSpeed;
    public float rightBound;
    public float leftBound;
    public float topBound;
    public float bottomBound;


    [Header("Data for Den screen")]

    public bool isInDenScreen = false;
    public float MinZoomDen = 1;
    public float MaxZoomDen = 8;

    public float panSpeedDen;
    public float panSpeedDenMinSpeed = 1;
    public float panSpeedDenMaxSpeed = 8;

    public float rightBoundDen;
    public float leftBoundDen;
    public float topBoundDen;
    public float bottomBoundDen;

    //public Text fovText;


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

        SetFieldOfView();
    }

    public void SetFieldOfView()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        if (aspectRatio >= (9 / 16f))
        {
            mainCam.fieldOfView = 60;

            if(Screen.height == 1920)
            {
                mainCam.fieldOfView = 57;
            }
        }
        else if (aspectRatio >= 3 / 4f)
        {
            mainCam.fieldOfView = 55;
        }
        else
        {
            mainCam.fieldOfView = 58;
        }

        //Debug.Log("Aspect " + aspectRatio);
        //mainCam.fieldOfView = aspectRatio * 60;
        //Debug.Log("Aspect calc" + aspectRatio * 60);
    }

    void Update()
    {

        //fovText.text = mainCam.fieldOfView.ToString();

        if (!UIManager.isUsingUI && !CursorController.OverUI)
        {
            //leftBound = (horzExtent - (SpriteBounds.bounds.size.x / 2.0f));
            //rightBound = ((SpriteBounds.bounds.size.x / 2.0f - horzExtent));

            //bottomBound = ((vertExtent - SpriteBounds.rect.size.y / 2.0f)) / SpriteBounds.pixelsPerUnit + 2;
            //topBound = ((SpriteBounds.rect.size.y / 2.0f - vertExtent)) / SpriteBounds.pixelsPerUnit + 2;

            if (Input.touchCount > 0 && !GameManager.Instance.levelStarted)
            {
                touch = Input.GetTouch(0);

                if (isInDenScreen)
                {
                    DenScreenControls();
                }
                else
                {
                    WorldMapControls();
                }
            }
            else
            {
                isZoom = false;
            }
        }
    }


    void WorldMapControls()
    {
        if (Input.touchCount < 2)
        {
            if (!isZoom)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    isDragging = true;
                    //Vector2 touchDeltaPos = touch.deltaPosition;

                    //Vector3 newPos = new Vector3(mainCam.transform.position.x - touchDeltaPos.x, mainCam.transform.position.y - touchDeltaPos.y, 0);

                    //mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, newPos, panSpeed * Time.deltaTime);

                    //mainCam.transform.Translate(-touchDeltaPos.x * panSpeed, -touchDeltaPos.y * panSpeed, 0);





                    //mainCam.transform.position = new Vector3(
                    //    Mathf.Clamp(mainCam.transform.position.x, leftBound, rightBound),
                    //    Mathf.Clamp(mainCam.transform.position.y, bottomBound, topBound),
                    //    -3f);
                }
            }
        }

        if (touch.phase == TouchPhase.Ended)
        {
            isZoom = false;
            isDragging = false;
        }

        //if (Input.touchCount == 2 && canZoom)
        //{
        //    Touch FirstFinger = Input.GetTouch(0);
        //    Touch SecondFinger = Input.GetTouch(1);

        //    Vector2 FirstFingerPrevPos = FirstFinger.position - FirstFinger.deltaPosition;
        //    Vector2 SecondFingerPrevPos = SecondFinger.position - SecondFinger.deltaPosition;

        //    float PrevMagnitude = (FirstFingerPrevPos - SecondFingerPrevPos).magnitude;
        //    float CurrentMagnitude = (FirstFinger.position - SecondFinger.position).magnitude;

        //    float Difference = PrevMagnitude - CurrentMagnitude;

        //    Camera.main.fieldOfView += Difference * 0.1f;
        //    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, MinZoom, MaxZoom);

        //    Camera.main.GetComponent<HideTiles>().updateMaxDistance(Difference);
        //}
    }

    void DenScreenControls()
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (GameManager.Instance.currentLevel.isSpecificTutorial && GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                return;
            }
        }

        if (Input.touchCount < 2)
        {
            if (!isZoom)
            {
                if (touch.phase == TouchPhase.Moved)
                {

                    Vector2 touchDeltaPos = touch.deltaPosition;


                    mainCam.transform.Translate(-touchDeltaPos.x * panSpeedDen, -touchDeltaPos.y * panSpeedDen, 0);

                    mainCam.transform.position = new Vector3(
                        Mathf.Clamp(mainCam.transform.position.x, leftBoundDen, rightBoundDen),
                        Mathf.Clamp(mainCam.transform.position.y, bottomBoundDen, topBoundDen),
                        -3f);
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
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, MinZoomDen, MaxZoomDen);
            panSpeedDen += Difference * 0.1f;
            panSpeedDen = Mathf.Clamp(panSpeedDen, panSpeedDenMinSpeed, panSpeedDenMaxSpeed);
            Camera.main.GetComponent<HideTiles>().updateMaxDistance(Difference);
        }
    }
}
