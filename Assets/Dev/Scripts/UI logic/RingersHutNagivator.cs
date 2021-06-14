using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingersHutNagivator : MonoBehaviour
{

    public Vector2 originalWidthHeight;

    float scaler = 1; // 1 is the minimum zoom out

    public float maxScale = 5000, minScale = 1; // 1 is the minimum zoom out
    public float sensitivityModifire;

    public RectTransform BG;

    public ScrollRect scrollRect;

    public RectTransform viewportTransform;
    float rightExtentVP;
    float upExtentVP;

    Vector2 bgCenter;

    private void Start()
    {
        originalWidthHeight = new Vector2(BG.rect.width, BG.rect.height);

        rightExtentVP = viewportTransform.rect.width / 2;
        upExtentVP = viewportTransform.rect.height / 2;
    }
    void Update()
    {
        if(Input.touches.Length > 1)
        {
            scrollRect.enabled = false;
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            float pinchDelta = Vector2.Distance(firstTouch.position, secondTouch.position);
            float previousPinchDelta = Vector2.Distance(firstTouch.position + firstTouch.deltaPosition, secondTouch.position + secondTouch.deltaPosition);

            if(previousPinchDelta - pinchDelta < 0)
            {
                /// zoom out or nothing
                ZoomOut(previousPinchDelta - pinchDelta);
                Debug.Log("zoom out");
            }
            else
            {
                /// zoom in
                Debug.Log("zoom in");
                ZoomIn((previousPinchDelta - pinchDelta));

            }
        }
        else
        {
            scrollRect.enabled = true;
            scrollRect.scrollSensitivity = scaler * sensitivityModifire;
        }
    }

    private void LateUpdate()
    {
        float bgHalfHeight = scaler * BG.rect.height;
        float bgHalfWidth = scaler * BG.rect.width;

        bgCenter = BG.localPosition;

        float minY = viewportTransform.position.y + upExtentVP;
        float maxX = viewportTransform.position.x + rightExtentVP;

        bgCenter.x = Mathf.Clamp(bgCenter.x, -maxX, maxX); 
        bgCenter.y = Mathf.Clamp(bgCenter.y, -minY, minY); ////// minY is less than 0

        BG.localPosition = bgCenter;
    }

    public void ZoomIn(float mag)
    {
        mag *= Time.deltaTime;

        scaler += mag;

        scaler = Mathf.Clamp(scaler, minScale, maxScale);

        BG.localScale = scaler * Vector3.one;
    }

    public void ZoomOut(float mag)
    {
        mag *= Time.deltaTime;

        scaler += mag;

        scaler = Mathf.Clamp(scaler, minScale, maxScale);

        BG.localScale = scaler * Vector3.one;


    }
}
