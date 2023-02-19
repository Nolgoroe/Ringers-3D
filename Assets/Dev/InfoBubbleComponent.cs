using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InfoBubbleType
{
    limiter,
    Potion
}
public class InfoBubbleComponent : MonoBehaviour, IPointerClickHandler
{
    public InfoBubbleType bubbleType;

    public InfoBubblePrefab limiterInfoBubblePrefab;
    public InfoBubblePrefab PotionInfoBubblePrefab;

    public Slice connectedSlice;
    public PowerupProperties connectedPower;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TutorialSequence.Instacne.duringSequence) return;
        StartCoroutine(UIManager.Instance.SetIsUsingUI(true));

        StartCoroutine(SpawnInfoBubble());
    }

    private void OnValidate()
    {     
        TryGetComponent<Slice>(out connectedSlice);

        if(connectedSlice != null)
        {
            bubbleType = InfoBubbleType.limiter;
            return;
        }

        TryGetComponent<PowerupProperties>(out connectedPower);

        if (connectedPower != null)
        {
            bubbleType = InfoBubbleType.Potion;
            return;
        }
    }

    private IEnumerator SpawnInfoBubble()
    {
        if(UIManager.Instance.currentInfoBubble != null)
        {
            yield return StartCoroutine(DestroyCurrentInfoBubble());
        }

        switch (bubbleType)
        {
            case InfoBubbleType.limiter:
                UIManager.Instance.currentInfoBubble = Instantiate(limiterInfoBubblePrefab, UIManager.Instance.sliceInfoBubblesParent);
                UIManager.Instance.currentInfoBubble.SetDescriptionText(connectedSlice);
                break;
            case InfoBubbleType.Potion:
                UIManager.Instance.currentInfoBubble = Instantiate(PotionInfoBubblePrefab, UIManager.Instance.potionInfoBubblesParent);
                UIManager.Instance.currentInfoBubble.SetDescriptionText(connectedPower);
                break;
            default:
                break;
        }

    }

    IEnumerator DestroyCurrentInfoBubble()
    {
        Destroy(UIManager.Instance.currentInfoBubble.gameObject);

        yield return new WaitForEndOfFrame();
    }


}
