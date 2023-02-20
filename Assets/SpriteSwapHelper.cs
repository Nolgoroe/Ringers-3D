using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwapHelper : MonoBehaviour
{
    public SpriteRenderer renderer;

    public Sprite spriteA, spriteB;

    private void OnValidate()
    {
        TryGetComponent<SpriteRenderer>(out renderer);

        if(renderer)
        {
            renderer.sprite = null;
        }
    }

    public void SwitchSprites(bool versionA)
    {
        renderer.sprite = versionA == true ? spriteA : spriteB;
    }
}
