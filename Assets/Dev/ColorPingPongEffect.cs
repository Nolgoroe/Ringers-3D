using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPingPongEffect : MonoBehaviour
{
    public SpriteRenderer connectedSpriteRenderer;

    public bool isActive;
    private void Start()
    {
        connectedSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PingPongColorNormal(Color targetColor)
    {
        connectedSpriteRenderer.color = targetColor;
    }

    public void TweenPingPongColor(Color targetColor)
    {
        LeanTween.value(gameObject,
            new Color(connectedSpriteRenderer.color.r, connectedSpriteRenderer.color.g, connectedSpriteRenderer.color.b),
            new Color(targetColor.r, targetColor.g, targetColor.b),
            3).setLoopPingPong();
    }
    public void TweenPingPongColor(Color startColor, Color targetColor)
    {
        LeanTween.value(gameObject,
            new Color(startColor.r, startColor.g, startColor.b),
            new Color(targetColor.r, targetColor.g, targetColor.b), 3);
    }
}
