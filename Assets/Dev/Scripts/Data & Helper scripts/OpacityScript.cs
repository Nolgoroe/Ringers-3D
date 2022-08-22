using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityScript : MonoBehaviour
{
    [Range(0, 1)]
    public float opacityLevel = 1;

    public SpriteRenderer[] childRenderers;
    private void Update()
    {
        childRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (childRenderers.Length > 0)
        {
            foreach (SpriteRenderer renderer in childRenderers)
            {
                Color color = renderer.color;

                color.a = opacityLevel;

                renderer.color = color;
            }
        }
    }
}
