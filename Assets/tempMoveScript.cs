using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempMoveScript : MonoBehaviour
{
    private Vector2 mousePosition;

    public List<GameObject> otherDevices;

    public List<GameObject> lines;

    private void Start()
    {
        lines = new List<GameObject>();
    }

    private void OnMouseDown()
    {
        int num = 1;
        foreach (GameObject g in otherDevices)
        {
            GameObject go = Instantiate(CorruptedZonesManager.instance.lineGO, transform);

            go.name = "Line " + num;
            LineController LC = go.GetComponent<LineController>();

            LC.gameObject1 = gameObject;
            LC.gameObject2 = g;

            lines.Add(go);

            num++;
        }
    }

    private void OnMouseUp()
    {
        foreach (GameObject g in lines)
        {
            Destroy(g);
        }

        lines.Clear();
    }

    private void OnMouseDrag()
    {
        Debug.Log("BLA");
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector2(mousePosition.x, mousePosition.y);

        foreach (GameObject g in lines)
        {
            LineController LC = g.GetComponent<LineController>();

            Debug.Log("Line is: " + g.name + " " + Vector3.Distance(LC.gameObject1.transform.position, LC.gameObject2.transform.position));

            if(Vector3.Distance(LC.gameObject1.transform.position, LC.gameObject2.transform.position) > 0.5f)
            {
                Color color = LC.lineMat.color;

                color.a = 0;

                LC.lineMat.color = color;


                Debug.Log("alpha zero");
            }
        }
    }

    void CalculateDisance()
    {

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1.2f,1.2f, 0f));
    }
}
