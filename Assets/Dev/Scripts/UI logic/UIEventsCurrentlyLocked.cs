using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventsCurrentlyLocked : MonoBehaviour, IPointerClickHandler
{
    public GameObject connectedUIScreen;
    bool isInventory, isAlbum, isDen;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!UIManager.Instance.isUsingUI)
        {
            if (isInventory && !TutorialSaveData.Instance.hasFinishedPotion)
            {
                connectedUIScreen.SetActive(true);
            }

            if (isAlbum && !TutorialSaveData.Instance.hasFinishedAnimalAlbum)
            {
                connectedUIScreen.SetActive(true);
            }

            if (isDen && !TutorialSaveData.Instance.hasFinishedDen)
            {
                connectedUIScreen.SetActive(true);
            }
        }
    }
}
