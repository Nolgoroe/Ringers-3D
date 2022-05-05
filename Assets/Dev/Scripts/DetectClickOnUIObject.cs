using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectClickOnUIObject : MonoBehaviour, IPointerClickHandler
{
    public GameObject toClose;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked HERE");

        if (!TutorialSequence.Instacne.duringSequence)
        {
            CloseWindow();
        }
    }

    void CloseWindow()
    {
        UIManager.Instance.closeWindow(toClose);
        //UIManager.isUsingUI = false;
    }
}
