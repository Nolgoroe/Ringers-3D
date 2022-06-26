using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectClickOnUIObject : MonoBehaviour, IPointerClickHandler
{
    public GameObject toClose;
    public bool isImmidiateClose;

    private void OnEnable()
    {
        UIManager.Instance.isUsingUI = true;
    }

    private void OnDisable()
    {
        if (isImmidiateClose)
        {
            UIManager.Instance.isUsingUI = false;
        }
    }

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
        if (isImmidiateClose)
        {
            UIManager.Instance.CloseWindowImmidiate(toClose);
        }
        else
        {
            UIManager.Instance.closeWindow(toClose);
        }
    }
}
