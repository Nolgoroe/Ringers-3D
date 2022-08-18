using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventsCurrentlyLocked : MonoBehaviour, IPointerClickHandler
{
    public GameObject connectedUIScreen;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!UIManager.Instance.isUsingUI && !TutorialSaveData.Instance.hasFinishedDen)
        {
            connectedUIScreen.SetActive(true);
        }
    }
}
