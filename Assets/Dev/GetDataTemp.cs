using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class GetDataTemp : CanvasScaler
    {
        public Text test;
        public Text posX, posY, posZ, ScaleX, ScaleY, ScaleZ, text1, text2, text3, text4;

        //public bool isOne;

        //public CanvasScaler canvasScaler;

        //private void Start()
        //{
        //    canvasScaler = GetComponent<CanvasScaler>();

        //    if (isOne)
        //    {
        //        posX = GeneralHelpDataRef.Instance.posX1;
        //        posY = GeneralHelpDataRef.Instance.posY1;
        //        posZ = GeneralHelpDataRef.Instance.posZ1;

        //        ScaleX = GeneralHelpDataRef.Instance.S1;
        //        ScaleY = GeneralHelpDataRef.Instance.S12;
        //        ScaleZ = GeneralHelpDataRef.Instance.S13;
        //    }
        //    else
        //    {
        //        posX = GeneralHelpDataRef.Instance.posX2;
        //        posY = GeneralHelpDataRef.Instance.posY2;
        //        posZ = GeneralHelpDataRef.Instance.posZ2;

        //        ScaleX = GeneralHelpDataRef.Instance.S21;
        //        ScaleY = GeneralHelpDataRef.Instance.S22;
        //        ScaleZ = GeneralHelpDataRef.Instance.S23;
        //    }
        //}
        //// Update is called once per frame
        //void Update()
        //{
        //    RectTransform parentR = transform.parent.GetComponent<RectTransform>();
        //    r.sizeDelta = new Vector2(parentR.rect.width, parentR.rect.height);

        //    //posZ.text = rect.localPosition.z.ToString();

        //    ScaleX.text = r.localPosition.x.ToString();
        //    ScaleY.text = r.localPosition.y.ToString();
        //    ScaleZ.text = r.localPosition.z.ToString();

        //}

        protected override void HandleScaleWithScreenSize()
        {
            test = GeneralHelpDataRef.Instance.S21;
            test.text = "IN HERE NOW!!!!";

            RectTransform r = GetComponent<RectTransform>();

            //r.sizeDelta = new Vector2(r.rect.width, 5000);

            posX = GeneralHelpDataRef.Instance.posX1;
            posY = GeneralHelpDataRef.Instance.posY1;
            posZ = GeneralHelpDataRef.Instance.posZ1;
            ScaleX = GeneralHelpDataRef.Instance.S1;
            ScaleY = GeneralHelpDataRef.Instance.S12;
            ScaleZ = GeneralHelpDataRef.Instance.S13;
            text1 = GeneralHelpDataRef.Instance.posX2;
            text2 = GeneralHelpDataRef.Instance.posY2;
            text3 = GeneralHelpDataRef.Instance.S21;
            text4 = GeneralHelpDataRef.Instance.S22;

            posX.text = "rect width: " + r.rect.width.ToString();
            posY.text = "rect height: " + r.rect.height.ToString();

            Canvas m_Canvas = transform.GetComponent<Canvas>();

            float kLogBase = 2;
            Vector2 screenSize = new Vector2(Display.main.systemWidth, Display.main.systemHeight);
            posZ.text = "screen size width " + screenSize.x + " " + "screen size Height " + screenSize.y;

            // Multiple display support only when not the main display. For display 0 the reported
            // resolution is always the desktops resolution since its part of the display API,
            // so we use the standard none multiple display method. (case 741751)
            int displayIndex = m_Canvas.targetDisplay;
            if (displayIndex > 0 && displayIndex < Display.displays.Length)
            {
                Display disp = Display.displays[displayIndex];
                screenSize = new Vector2(disp.renderingWidth, disp.renderingHeight);
            }

            float scaleFactor = 0;
            switch (m_ScreenMatchMode)
            {
                case ScreenMatchMode.MatchWidthOrHeight:
                    {
                        // We take the log of the relative width and height before taking the average.
                        // Then we transform it back in the original space.
                        // the reason to transform in and out of logarithmic space is to have better behavior.
                        // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                        // In normal space the average would be (0.5 + 2) / 2 = 1.25
                        // In logarithmic space the average is (-1 + 1) / 2 = 0
                        float logWidth = Mathf.Log(screenSize.x / m_ReferenceResolution.x, kLogBase);
                        float logHeight = Mathf.Log(screenSize.y / m_ReferenceResolution.y, kLogBase);

                        text1.text = "log width: " + logWidth.ToString();
                        text2.text = "log height: " + logHeight.ToString();

                        float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, m_MatchWidthOrHeight);
                        text3.text = "Log weighted Average: " + logWeightedAverage.ToString();

                        scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);

                        text4.text = "scaleFactor POW: " + scaleFactor.ToString();
                        break;
                    }
                case ScreenMatchMode.Expand:
                    {
                        scaleFactor = Mathf.Min(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                        break;
                    }
                case ScreenMatchMode.Shrink:
                    {
                        scaleFactor = Mathf.Max(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                        break;
                    }
            }

            ScaleX.text = "Scale Factor: " + m_Canvas.scaleFactor;
            ScaleY.text = "Ref resolution X: " + m_ReferenceResolution.x;
            ScaleZ.text = "Red resolution Y: " + m_ReferenceResolution.y;

            SetScaleFactor(scaleFactor);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }
    }

}
