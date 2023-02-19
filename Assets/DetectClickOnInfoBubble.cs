using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectClickOnInfoBubble : MonoBehaviour, IPointerClickHandler
{
    public GameObject toRemove;

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.CallSetIsUsingUI(false);

        Destroy(toRemove);

        if(TutorialSequence.Instacne.maskImage.gameObject.activeInHierarchy)
        {
            TutorialSequence.Instacne.ClearAllHighlights();
            TutorialSequence.Instacne.maskImage.gameObject.SetActive(false);
        }
        return;

    }
}
