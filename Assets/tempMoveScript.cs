using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class tempMoveScript : MonoBehaviour
{
    public CorruptedDevicesData connectedCDD = null;

    public List<GameObject> otherDevices;

    public List<GameObject> lines;

    private Vector2 mousePosition;

    public bool hasBeenSet = false;

    Image myImage;

    private void Start()
    {
        lines = new List<GameObject>();
        myImage = GetComponent<Image>();
    }

    private void OnMouseDown()
    {
        if (!hasBeenSet && !UIManager.Instance.corruptedZoneSureMessage.activeInHierarchy)
        {
            transform.SetParent(CorruptedZonesManager.instance.currentActiveZone.transform);

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
            otherDevices.Clear();

            foreach (tempMoveScript TMS in CorruptedZonesManager.instance.currentActiveZone.currentDevicesInZone)
            {
                if (TMS != this)
                {
                    otherDevices.Add(TMS.gameObject);
                }
            }

            int num = 1;
            foreach (GameObject g in otherDevices)
            {
                GameObject go = Instantiate(CorruptedZonesManager.instance.lineGO, transform);

                go.name = "Line " + num;
                LineController LC = go.GetComponent<LineController>();

                LC.CDDOrigin = gameObject;
                LC.CDDTarget = g;

                lines.Add(go);

                num++;
            }

            CorruptedZonesManager.instance.currentDeviceToPlace = this;
        }

    }

    private void OnMouseUp()
    {
        if (CorruptedZonesManager.instance.currentDeviceToPlace)
        {
            UIManager.Instance.ActivateAreYouSureCorruptZone(connectedCDD.deviceName);
        }
    }

    public void SetDeviceOnZone()
    {
        if (!hasBeenSet)
        {
            foreach (GameObject g in lines)
            {
                LineRenderer LR = g.GetComponent<LineRenderer>();

                Color c = LR.material.GetColor("_BaseColor");

                if (c.a <= 0)
                {
                    Destroy(g);
                }
            }

            lines.Clear();

            CorruptedZonesManager.instance.currentActiveZone.currentDevicesInZone.Add(this);

            myImage.enabled = false;

            GameObject go = Resources.Load<GameObject>(connectedCDD.prefabPath);

            Instantiate(go, transform);

            hasBeenSet = true;
        }
    }

    private void OnMouseDrag()
    {
        if (!hasBeenSet && !UIManager.Instance.corruptedZoneSureMessage.activeInHierarchy)
        {
            //Debug.Log("BLA");
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mousePosition.x = Mathf.Clamp(mousePosition.x, -0.5f, 0.5f);
            mousePosition.y = Mathf.Clamp(mousePosition.y, -10, -8);


            Debug.Log(mousePosition);
            Vector2 pos = new Vector2(mousePosition.x, mousePosition.y);

            //pos.x = Mathf.Clamp(pos.x, -620, 720);
            //pos.y = Mathf.Clamp(pos.y, -1280, 1180);

            //Debug.Log(pos);

            transform.position = pos;

            if (CorruptedZonesManager.instance.currentActiveZone.currentDevicesInZone.Count > 0)
            {
                foreach (GameObject g in lines)
                {
                    LineController LC = g.GetComponent<LineController>();
                    LC.distnaces = new List<float>();

                    if (connectedCDD != null)
                    {
                        DeviceConnections DC = connectedCDD.deviceConnectionsList.Where(p => p.deviceToConnectWith == LC.CDDTarget.name).Single();
                        LC.distnaces.AddRange(DC.distances);
                    }

                    float distance = CalculateDisance(LC.CDDOrigin, LC.CDDTarget);

                    //Debug.Log("Line is: " + g.name + " " + distance * 10);

                    UpdateLineAlpha(distance, LC);
                }
            }
        }
    }

    float CalculateDisance(GameObject one, GameObject two)
    {
        return Vector3.Distance(one.transform.position, two.transform.position);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1.2f,1.2f, 0f));
    }
    
    private void UpdateLineAlpha(float distance, LineController LC)
    {
        //Debug.Log(distance * 10);
        LineRenderer LR = LC.gameObject.GetComponent<LineRenderer>();

        if (distance * 10 >= LC.distnaces[0])
        {
            Color color = LR.materials[0].color;

            color.a = 0;

            LR.material.SetColor("_BaseColor", new Color(1f, 1f, 1f, color.a));
        }
        else if (distance * 10 <= LC.distnaces[2])
        {
            Color color = LR.materials[0].color;

            color.a = 255;

            LR.material.SetColor("_BaseColor", new Color(1f, 1f, 1f, color.a));
        }
        else
        {
            float lerpAmt = 1.0f - Mathf.Clamp01(((distance * 10) - LC.distnaces[2]) / (LC.distnaces[0] - LC.distnaces[2]));

            //Debug.Log(lerpAmt);


            Color color = LR.materials[0].color;

            color.a = lerpAmt;

            LR.material.SetColor("_BaseColor", new Color(1f, 1f, 1f, color.a));

            lerpAmt = Mathf.Clamp(lerpAmt, 0, 0.5f);
            LC.GetComponent<LineRenderer>().startWidth = lerpAmt / 10;
            LC.GetComponent<LineRenderer>().endWidth = lerpAmt / 10;
        }
    }

    public void DiscradLines()
    {
        otherDevices.Clear();

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        lines.Clear();
    }
}
